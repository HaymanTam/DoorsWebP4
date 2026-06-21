using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DoorsWeb.Licensing
{
    /// <summary>
    /// Encodes / verifies a license as a compact two-segment string:
    /// <c>base64url(payloadJson) + "." + base64url(signature)</c>.
    /// <para>
    /// Signing uses ECDSA over the NIST P-256 curve with SHA-256. The signature is made over the
    /// raw payload JSON bytes, so verification re-checks those exact bytes — there is no need for
    /// the verifier to reproduce the serialization. Only the holder of the private key can mint a
    /// token; the application embeds only the public key, so a token cannot be forged from the app.
    /// </para>
    /// </summary>
    public static class LicenseToken
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            // Compact; property names are stable so existing tokens keep verifying.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>Signs a payload and returns the license key string.</summary>
        public static string Sign(LicensePayload payload, ECDsa privateKey)
        {
            ArgumentNullException.ThrowIfNull(payload);
            ArgumentNullException.ThrowIfNull(privateKey);

            var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload, JsonOptions);
            var signature = privateKey.SignData(payloadBytes, HashAlgorithmName.SHA256);
            return $"{Base64Url.Encode(payloadBytes)}.{Base64Url.Encode(signature)}";
        }

        /// <summary>
        /// Verifies a license key string against the public key and, if valid, returns its payload.
        /// Returns false (with a reason) for a malformed token, a bad signature, or an unparseable
        /// payload. Does NOT check expiry — that's an application policy decision.
        /// </summary>
        public static bool TryVerify(string? token, ECDsa publicKey, out LicensePayload? payload, out string? error)
        {
            payload = null;
            error = null;

            if (string.IsNullOrWhiteSpace(token))
            {
                error = "No license key.";
                return false;
            }
            ArgumentNullException.ThrowIfNull(publicKey);

            var parts = token.Trim().Split('.');
            if (parts.Length != 2)
            {
                error = "License key is malformed.";
                return false;
            }

            byte[] payloadBytes;
            byte[] signature;
            try
            {
                payloadBytes = Base64Url.Decode(parts[0]);
                signature = Base64Url.Decode(parts[1]);
            }
            catch
            {
                error = "License key is not valid Base64.";
                return false;
            }

            bool ok;
            try
            {
                ok = publicKey.VerifyData(payloadBytes, signature, HashAlgorithmName.SHA256);
            }
            catch
            {
                error = "License signature could not be checked.";
                return false;
            }
            if (!ok)
            {
                error = "License signature is invalid.";
                return false;
            }

            try
            {
                payload = JsonSerializer.Deserialize<LicensePayload>(payloadBytes, JsonOptions);
            }
            catch
            {
                error = "License contents could not be read.";
                return false;
            }
            if (payload is null)
            {
                error = "License contents are empty.";
                return false;
            }

            return true;
        }
    }

    /// <summary>URL-safe Base64 without padding (RFC 4648 §5), used for the token segments.</summary>
    internal static class Base64Url
    {
        public static string Encode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        public static byte[] Decode(string text)
        {
            var s = text.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }
}
