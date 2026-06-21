# Doors Web P4 — Feature Log

A living record of what's been built and what's still planned. **Maintained as we go** —
when a feature ships, move it from *Planned* to *Implemented* with the date; when a new
idea is agreed, add it to *Planned*.

- **Stack:** ASP.NET Core (net10.0) Web API + Blazor WebAssembly (standalone) client, EF Core over
  legacy Postgres `T_` tables, SignalR for live state, Docker Compose for deployment.
- **Security boundary:** the API. Client-side permission/validation checks are convenience only.
- _Last updated: 2026-06-21_

---

## Implemented

### Authentication & Authorization
- JWT auth — access token (15 min) + refresh token (7 day).
- Policy-based authorization across four permission areas — **Card Manager, Site Settings,
  User Settings, Reports** — each at `None` / `Read` / `ReadWrite`.
- **Super** (Administrator) role: implicit `ReadWrite` everywhere; only role that can manage users;
  at least one Super must always exist.
- BCrypt password hashing (cost 11) with legacy-plaintext fallback for restored DoorsClient data.
- Force-change-password on first sign-in when the stored secret is the seeded default or a
  legacy plaintext value.
- Login page: remembers the last-used username on the machine (localStorage) and pre-fills it;
  on a fresh install, surfaces the seeded Super's username + default password as a first-run hint
  that self-disposes once the password is changed.

### Password Policy (NIST SP 800-63B aligned)
- Flat 12-character minimum — no per-role tiers, no character-class complexity rules, no routine expiry.
- Offline screening against a bundled ~1M known-breached/common-password list.
- Paste / passphrases allowed; no MFA (deliberate, per NIST guidance).
- Enforced on change-password (incl. first-login force-change) and on user create/edit.
- Client-side password strength meter (length-based) + inline guidance.

### User Management
- CRUD for system users with per-area access levels.
- At-least-one-Super invariant enforced server-side.

### Card Manager
- Cardholder CRUD; card list streamed (incremental paint) as a JSON array.
- Card photo upload/storage (filesystem, keyed by card number); photo filter + photo indicator column.
- CardPresso CSV export — UTF-8 BOM for Windows importers; PIN deliberately excluded.
- Custom card format support (e.g., Corporate 1000 code field).

### Site / System Settings
- System settings persisted as JSON via a settings service + controller.
- Controller communication UI; new doors auto-attach to a UDP connector (auto-created if none).
- Software version display (assembly version) in System Settings.
- License status UI.

### Doors — Live State & Floorplan
- Live door state broadcast over SignalR.
- Door command/control API.
- Floorplan layout persistence with separate **view** and **edit** pages + nav links.
- **One floorplan per site** (a single background image + door placements per site).
- Legacy restore imports each site's floorplan background image from the backup's `Floors/` folder
  (one plan per site, lowest plan code wins). Door positions aren't carried over — the legacy
  vector-design coordinates don't map onto the percentage model — so doors are re-pinned in the editor.

### Licensing
- ECDSA-signed license crypto library (`DoorsWeb.Licensing`); private key never committed/shipped.
- License-limit enforcement + read-only mode on expiry.
- WinForms keygen app ("Keygen Web") + demo key pair wired to the public key.
- Payment-website instructions (`Keygen Web/PAYMENT-WEBSITE.md`).

### Reports
- Report engine: shared DTOs, `IReport` abstraction, registry.
- Access History report (vertical-slice reference report).
- CSV + PDF output (QuestPDF — Community license, valid under $1M revenue).
- Reports hub UI + generic runner + nav entry.

### Diagnostics & Support
- **Request correlation IDs** — every API request gets a stable ID (`CorrelationIdMiddleware`): honours an
  inbound `X-Correlation-Id`, else mints one; sets `HttpContext.TraceIdentifier` so the ProblemDetails
  `requestId` in error responses matches; echoes it back in the `X-Correlation-Id` response header; and
  pushes it into the Serilog `LogContext` so every log line for the request is tagged with it.
- **Structured logging to rolling files** (Serilog) — daily roll, 14-file retention, written under
  `Storage:LogDirectory` (defaults to a `Logs/` folder on the settings volume) so logs persist across
  container restarts on offline sites. One concise completion line per request via `UseSerilogRequestLogging`.
- _Foundation for the planned bug-report support bundle: a user-quoted correlation/reference ID resolves
  directly to the exact log lines for the failing request._

---

## Planned / To Be Implemented

_Backlog — add agreed-on features here; nothing is in flight unless noted._

### Bug Report Pipeline (offline-first)
End-to-end flow from a user hitting a problem to the vendor shipping a fix: a user generates a
self-contained diagnostic report in-app and sends it to a vendor-hosted intake site; when the site is
offline the same report is downloaded as a file and uploaded from any connected machine. Generation never
needs connectivity — only transport does. Phase 1 (correlation IDs + rolling file logs) is **done** (see
*Diagnostics & Support* above). Remaining phases:
- **Phase 2 — Capture + offline file:** Blazor `<ErrorBoundary>` replacing the dead default error bar with
  a friendly "something went wrong — reference `X`"; a "Report a problem" dialog that captures the recent
  failed-request correlation ID(s) + the operator's description and produces a downloadable report.
- **Phase 3 — Full support bundle:** server-side `.zip` (Super-gated) bundling recent logs + app version +
  license/DB status, keyed by a globally-unique **Report ID** (`{siteCode}-{timestamp}-{guid}`), with a
  root `manifest.json`. **Redaction is mandatory** — never include `Jwt:SecretKey`, the connection string,
  password hashes, tokens, or cardholder PII/photos. Optionally **sign the manifest with the existing
  `DoorsWeb.Licensing` ECDSA scheme** so the vendor can verify authenticity + originating site.
- **Phase 4 — Hosted intake + auto-upload:** the receiving website + client logic that POSTs the bundle
  when reachable and falls back to file download when offline.
- **Phase 5 — Triage → fix → release loop:** vendor-side intake dashboard that indexes bundles by Report
  ID / site / version, dedupes recurring crashes, and links a report to the fix + the version it ships in
  (closing the loop back to the version shown in System Settings, so a site can confirm "my issue is fixed
  in this build").

### Testing & CI Pipeline
Goal: lock in behaviour with automated tests and run them on every change, so regressions surface before
a build reaches a site. No test project exists yet — this is greenfield. Proposed layering (thin pyramid):
- **Phase 1 — Test project + unit tests:** add `DoorsWeb.Tests` (xUnit) to `DoorsWebP4.slnx`; cover the
  pure, bug-prone logic first — legacy-restore floor selection (`IsFloorImage`, one-plan-per-site
  lowest-code-wins), JWT parse/expiry (`JwtAuthStateProvider`), and password-policy/strength scoring.
  Some logic needs a small refactor to be testable (e.g. extract the floor-selection rule into a pure
  method).
- **Phase 2 — Behaviour tests with mocks:** `JwtAuthorizationHandler` 401 → refresh → replay → sign-out
  flow (mock `IJSRuntime` + stub `HttpMessageHandler`), and the concurrent-refresh dedup path.
- **Phase 3 — Integration tests:** legacy backup restore against a throwaway Postgres (Testcontainers)
  using a sample backup ZIP — assert `FloorPlansRestored`, that the image lands on the store, and that
  imported layouts have zero doors. (Pragmatic fallback: a documented manual run against the dev DB.)
- **Phase 4 — CI automation:** a pipeline (e.g. GitHub Actions) that restores, builds API + Client, and
  runs the test suite on push/PR, gating merges on green. Optionally publish a coverage summary.
- **Optional — UI/E2E:** bUnit for Blazor component logic (e.g. restore-modal acknowledgement gating) and
  Playwright for the real expired-session → redirect-to-login flow, added once the lower layers exist.

---

## Operational / Deployment Notes

- **Pending deploy:** rebuild Docker containers to ship recent changes —
  `docker compose up -d --build doorsweb.api doorsweb.client`.
- Dev secrets (connection string, `Jwt:SecretKey`, Kestrel cert) live in .NET user-secrets,
  **not** in committed appsettings.
- **TLS / same-origin:** the Blazor app and API are served under a single origin — nginx hosts the
  app and reverse-proxies `/api`, `/auth`, `/eventHub`, `/backupHub`, `/media` to the API container.
  One browser-trusted certificate (nginx's), no CORS, no hardcoded API host. The cert is **mounted**
  at runtime into `/etc/nginx/certs` (not baked into the image).
  - **Dev:** `mkcert -install` once per machine, then
    `mkcert -cert-file certs/server.crt -key-file certs/server.key localhost 127.0.0.1 ::1`.
    `certs/` is gitignored — the private key is never committed.
  - **Per-site deploy (offline-safe):** generate the cert at install time for that site's hostname/IP
    and drop it in `certs/`; import mkcert's `rootCA.pem` (from `mkcert -CAROOT`) into each client
    machine's trust store. No internet required at any point.
