# Self-Service License Website — Build Instructions

How to stand up a small website that **takes a payment and automatically emails the buyer a working
DoorsWeb license key**. The key is the exact same signed token the desktop *Keygen* produces — it is
just minted server-side the moment the payment succeeds, instead of by hand.

This document is the spec/runbook. It is **not** wired into the main solution; build the site as a
separate service (it lives wherever you host public web apps).

---

## 1. How it fits together

```
 Buyer ──▶ Checkout page ──▶ Stripe Checkout ──▶ (pays)
                                   │
                                   ▼
                      Stripe sends "checkout.session.completed" webhook
                                   │
                                   ▼
   Your site:  verify webhook  ─▶  build LicensePayload (tier → doors/cards/expiry)
                               ─▶  sign it with the PRIVATE key (DoorsWeb.Licensing)
                               ─▶  store the key + order  ─▶  email it to the buyer
                                   │
                                   ▼
                       Buyer pastes the key into
                   DoorsWeb → System Settings → License
```

The customer's DoorsWeb install verifies that key offline using the **public** key already in its
`Licensing:PublicKey` config. The website and the desktop Keygen both hold the **private** key; the
shipped product never does.

**Key rule:** the license site signs with *the same private key* whose public half is configured in
every customer install. One key pair per product line. If you sell to many customers who each run
their own server, they all share the one public key — that's fine, only you can sign.

---

## 2. The signing key

You already have a key pair (created by the Keygen, `--newkeys`, or the "Generate New Key Pair"
button):

- `doorsweb.private.pem` — **secret**. Goes only into the website's secret store. Never in git, never
  in a Docker image layer, never in client-side code.
- The public key (base64) — already pasted into each install's `Licensing:PublicKey`.

Store the private key as a secret in your host:

- **Azure**: Key Vault → reference from App Service as `Licensing__PrivateKeyPem`.
- **AWS**: Secrets Manager / SSM Parameter Store (SecureString).
- **Fly/Render/Heroku**: an encrypted env var / secret, e.g. `LICENSING_PRIVATE_KEY_PEM`.
- **Plain VM**: a file readable only by the service account (`chmod 600`), outside the web root.

Rotate by generating a new pair, shipping the new public key to customers in an app update, then
switching the website to the new private key. Keep the old key only as long as you must re-issue old
licenses.

---

## 3. Reuse the existing crypto — don't reimplement it

The website should reference the **`DoorsWeb.Licensing`** project (or a NuGet package built from it)
so the signing is byte-for-byte identical to the desktop Keygen and the app's verifier. Do **not**
hand-roll ECDSA in the website — a tiny difference in serialization or curve will make keys that the
app rejects.

```
dotnet add <LicenseSite> reference ../DoorsWeb.Licensing/DoorsWeb.Licensing.csproj
```

Minting a key is then exactly what the Keygen does:

```csharp
using DoorsWeb.Licensing;

string MintKey(string privatePem, string customer, string licenseId,
               int maxDoors, int maxCards, DateTime expiryUtc)
{
    var payload = new LicensePayload
    {
        LicenseId = licenseId,
        Customer  = customer,
        MaxDoors  = maxDoors,
        MaxCards  = maxCards,
        ExpiryUtc = expiryUtc,
        IssuedUtc = DateTime.UtcNow
    };

    using var key = LicenseKeys.LoadPrivateKeyPem(privatePem);
    return LicenseToken.Sign(payload, key);   // the string you email to the buyer
}
```

---

## 4. Define your products (tier → limits)

Map each purchasable product to the three license dimensions. Put this in config, not code, so prices
and limits can change without a redeploy.

| Tier        | Max Doors | Max Cardholders | Term     | Stripe Price ID      |
|-------------|-----------|-----------------|----------|----------------------|
| Starter     | 10        | 100             | 1 year   | `price_starter_1y`   |
| Professional| 50        | 1,000           | 1 year   | `price_pro_1y`       |
| Enterprise  | 500       | 25,000          | 1 year   | `price_ent_1y`       |

`ExpiryUtc = DateTime.UtcNow + term` at the moment of purchase (or end-of-day in UTC, matching the
Keygen). For renewals you re-issue with a later expiry — the customer just pastes the new key.

---

## 5. Stripe Checkout (recommended — Stripe hosts the card form, you never touch card data)

1. **Create a Stripe account**, grab the **test** keys first (`sk_test_…`, `pk_test_…`).
2. **Create Products + Prices** in the Stripe dashboard (one Price per tier above). Note each
   `price_…` id.
3. **Create a Checkout Session** when the buyer clicks "Buy" on your site:

```csharp
// POST /buy?tier=pro   (server-side; needs your Stripe SECRET key)
StripeConfiguration.ApiKey = stripeSecretKey;

var session = new SessionService().Create(new SessionCreateOptions
{
    Mode = "payment",                              // one-time; use "subscription" for auto-renew
    LineItems = new() { new() { Price = priceIdForTier, Quantity = 1 } },
    CustomerEmail = buyerEmail,                    // so we know where to send the key
    SuccessUrl = "https://yoursite/success?session_id={CHECKOUT_SESSION_ID}",
    CancelUrl  = "https://yoursite/pricing",
    Metadata = new() { ["tier"] = "pro" }          // carried into the webhook
});

return Results.Redirect(session.Url);              // Stripe-hosted payment page
```

The buyer enters card details on Stripe's page. **No card data ever touches your server** — this
keeps you out of PCI scope.

---

## 6. The webhook — where the key is generated (the important part)

Stripe calls your webhook after a successful payment. This is the only trustworthy "they paid"
signal — **never generate a key from the browser success page**, which a user can fake by visiting
the URL directly.

```csharp
// POST /webhooks/stripe   (raw body required for signature verification)
app.MapPost("/webhooks/stripe", async (HttpRequest req) =>
{
    var json = await new StreamReader(req.Body).ReadToEndAsync();

    Event stripeEvent;
    try
    {
        // Verify it really came from Stripe and wasn't replayed/forged.
        stripeEvent = EventUtility.ConstructEvent(
            json, req.Headers["Stripe-Signature"], webhookSigningSecret);
    }
    catch (StripeException)
    {
        return Results.BadRequest();   // bad signature → reject
    }

    if (stripeEvent.Type == "checkout.session.completed")
    {
        var session = (Session)stripeEvent.Data.Object;

        // Idempotency: Stripe may deliver the same event more than once.
        // Skip if we already issued a key for this session id.
        if (await store.AlreadyIssued(session.Id))
            return Results.Ok();

        var tier      = session.Metadata["tier"];
        var limits    = TierTable[tier];                 // doors/cards/term from §4
        var email     = session.CustomerEmail ?? session.CustomerDetails?.Email;
        var licenseId = session.Id;                      // stable, unique per order
        var expiry    = DateTime.UtcNow.Add(limits.Term);

        var key = MintKey(privatePem, customer: email!, licenseId,
                          limits.MaxDoors, limits.MaxCards, expiry);

        await store.Save(licenseId, email!, key, tier, expiry);  // persist for re-sends/support
        await mailer.SendLicenseEmail(email!, key, limits, expiry);
    }

    return Results.Ok();   // 2xx tells Stripe to stop retrying
});
```

Register the webhook endpoint in the Stripe dashboard (Developers → Webhooks), subscribe to
`checkout.session.completed`, and copy its **signing secret** (`whsec_…`) into your config as
`webhookSigningSecret`.

Test locally with the Stripe CLI:

```
stripe listen --forward-to https://localhost:5001/webhooks/stripe
stripe trigger checkout.session.completed
```

---

## 7. Deliver the key

Email is simplest (SendGrid, AWS SES, Postmark, etc.). Send a plain, copy-pasteable block:

```
Subject: Your DoorsWeb license key

Thanks for your purchase! Your license:

  Plan:    Professional (50 doors, 1,000 cardholders)
  Expires: 2027-06-21

License key — copy everything between the lines and paste it into
DoorsWeb → System Settings → License → License key, then click OK:

------------------------------------------------------------
eyJsaWNlbnNlSWQiOiJ...   (the long key string)
------------------------------------------------------------
```

Also show the key on the `/success` page (looked up by `session_id`) so they have it immediately, and
keep it retrievable from an order-lookup page in case the email is lost.

---

## 8. Security checklist

- [ ] Private key only in the secret store; not in git, images, logs, or client code.
- [ ] **Always** verify the Stripe webhook signature (`EventUtility.ConstructEvent`). Reject on
      failure. This is what stops someone forging a "paid" event to get a free key.
- [ ] Generate the key **only** in the webhook, never from the browser success page.
- [ ] Idempotency: one key per `session.Id`; ignore duplicate webhook deliveries.
- [ ] Serve everything over HTTPS.
- [ ] Don't log full keys or the private key. Log the `licenseId` only.
- [ ] Persist issued keys (id, email, tier, expiry, key) so you can re-send and support renewals.
- [ ] Start in Stripe **test mode**; only switch to live keys once the full flow works end-to-end.

---

## 9. Renewals (optional)

- **One-time payments**: send a "your license expires soon" email near `ExpiryUtc` with a link to buy
  again. On purchase, mint a fresh key with a later expiry and the same (or new) limits.
- **Stripe subscriptions**: use `Mode = "subscription"` and handle `invoice.paid` — on each renewal
  mint a key whose `ExpiryUtc` is the new period end, and email it. The customer pastes the new key;
  the app stays active because the new expiry is in the future.

---

## 10. Non-.NET stacks

If you build the site in Node/Python/etc., you must reproduce the **exact** token format from
`DoorsWeb.Licensing/LicenseToken.cs`, or the app will reject your keys:

1. Curve **ECDSA NIST P-256 (secp256r1)**, hash **SHA-256**, signature in **IEEE-P1363** form
   (raw r‖s, 64 bytes) — *not* DER. (.NET's `SignData` default is P1363; Node's `crypto.sign`
   defaults to DER, so set `dsaEncoding: 'ieee-p1363'`.)
2. Payload JSON with **camelCase** property names: `licenseId, customer, maxDoors, maxCards,
   expiryUtc, issuedUtc`. Dates as ISO-8601 UTC.
3. Token = `base64url(payloadJsonUtf8Bytes) + "." + base64url(signature)` — base64url is standard
   base64 with `+`→`-`, `/`→`_`, and `=` padding stripped.
4. Sign the **raw payload bytes** (the same bytes you base64url-encode for segment 1), not a hash you
   compute yourself and not the base64 text.

Easiest path: keep using the .NET `DoorsWeb.Licensing` library for the signing step (even as a tiny
internal microservice the rest of your site calls) so it can't drift from the verifier.
