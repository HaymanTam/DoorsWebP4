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

---

## Planned / To Be Implemented

_Backlog — add agreed-on features here; nothing is in flight unless noted._

- _(none currently tracked — add items as they come up)_

---

## Operational / Deployment Notes

- **Pending deploy:** rebuild Docker containers to ship recent changes —
  `docker compose up -d --build doorsweb.api doorsweb.client`.
- Dev secrets (connection string, `Jwt:SecretKey`, Kestrel cert) live in .NET user-secrets,
  **not** in committed appsettings.
