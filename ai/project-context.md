# Project context — Employee Desk Booking System

> **Project-owned.** Every AI-DLC persona loads this file with `ai/AI-DLC.md` before working. It is the single briefing for *this* product — domain, stack, layout, how to run it, and where delivery stands. Keep it current when the stack or baseline changes.

|                  |                                                                                  |
| ---------------- | -------------------------------------------------------------------------------- |
| **Product**      | Employee Desk Booking System (EDBS)                                              |
| **Client**       | Trigent — single hybrid office (one location)                                    |
| **Requirements** | BRD-001, SRS-001 · Epic EPIC-001 · Stories US-001 … US-009                       |
| **Architecture** | `inception/architecture/` (DB design + app architecture — advisory, Gate 1)      |

---

## What this system does

A browser-based web application lets **Employees** reserve a specific desk before coming into the office, and lets **Admins** oversee bookings, maintain desk inventory, and manage user accounts.

Core flows:

- Sign in / sign out with email and password (no SSO in scope)
- Employee selects a date → sees desk availability → books one desk per date
- Employee views and cancels own bookings (today or future only)
- Admin views/filters/cancels all bookings; manages desks and users
- Email on book/cancel; day-before reminder email; optional browser push on book/cancel
- Past confirmed bookings auto-complete via a background job

Out of scope (BRD-001 §10): SSO, self-service password reset, multi-office, weekend booking, and related items.

---

## Actors

| Actor    | Role in the app | Primary surfaces                          |
| -------- | --------------- | ----------------------------------------- |
| Employee | `Employee`      | SCR-002 Book, SCR-003 My Bookings, SCR-007 Notifications |
| Admin    | `Admin`         | SCR-004 Admin Bookings, SCR-005 Desks, SCR-006 Users |
| System   | Hosted jobs     | Booking completion, reminder emails       |

---

## Technology stack

| Layer        | Choice                                      | Notes                                      |
| ------------ | ------------------------------------------- | ------------------------------------------ |
| Runtime      | **.NET 8**                                  | `net8.0` on all projects                   |
| Web UI       | **ASP.NET Core MVC**, Razor, Bootstrap 5    | Server-rendered; primary user interface    |
| API          | **ASP.NET Core Web API**, Swashbuckle       | REST + OpenAPI at `/swagger`               |
| ORM          | **Entity Framework Core 8.0.11**            | Migrations in Infrastructure               |
| Database     | **SQL Server** (LocalDB in dev)             | Connection string in `appsettings`         |
| Auth (Web)   | Cookie authentication                       | `HttpOnly`, `Secure`, `SameSite=Strict` in prod |
| Auth (API)   | JWT Bearer                                  | For API clients and progressive enhancement |
| Passwords    | `IPasswordHasher<User>` (ASP.NET Identity)  | Min 8 chars; upper, lower, digit, special  |
| Email        | MailKit                                     | SMTP; dev may write to `App_Data/sent-emails` |
| Push         | WebPush (VAPID)                             | Opt-in; day-before reminders are email only |
| Tests        | **xUnit** + `WebApplicationFactory`         | `tests/EmployeeDeskBooking.Tests/`         |

---

## Solution layout

```
EmployeeDeskBooking.sln
src/
  EmployeeDeskBooking.Domain/           Entities, enums — no framework refs
  EmployeeDeskBooking.Application/      Services, DTOs, validators, interfaces
  EmployeeDeskBooking.Infrastructure/   EF Core, repositories, MailKit, WebPush, jobs
  EmployeeDeskBooking.Web/              MVC presentation (cookies)
  EmployeeDeskBooking.Api/              REST presentation (JWT, Swagger)
tests/
  EmployeeDeskBooking.Tests/            Unit + integration (xUnit)
inception/                              Requirements, stories, design, architecture
knowledge/traceability/                 manifest.json (source of truth for IDs)
tools/                                  aidlc-check, scaffold utilities
```

### Layered (N-tier) dependency rules

```
Presentation (Web, Api)  →  Application  →  Domain
                               ↑
                    Infrastructure  →  Domain
```

- Controllers are thin — inject Application services, never `AppDbContext`
- Business rules (BR-001.*) live in Application
- Domain has zero upward dependencies
- Background jobs live in Infrastructure but call Application services

Full detail: `inception/architecture/app-architecture.md` and `db-design.md`.

---

## Local development

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (ships with Visual Studio / SQL Express LocalDB on Windows)
- Node.js 20+ (for `node tools/aidlc-check.mjs` only)

### Database

Default connection (both Web and Api):

```
Server=(localdb)\mssqllocaldb;Database=EmployeeDeskBooking;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Override locally via `appsettings.Development.local.json` (gitignored) in each host project.

Apply migrations (once they exist):

```bash
dotnet ef database update --project src/EmployeeDeskBooking.Infrastructure --startup-project src/EmployeeDeskBooking.Web
```

### Run the apps

From the repository root:

```bash
dotnet run --project src/EmployeeDeskBooking.Web
dotnet run --project src/EmployeeDeskBooking.Api
```

| Host | HTTPS (dev) | HTTP (dev) | Notes                    |
| ---- | ------------- | ---------- | ------------------------ |
| Web  | https://localhost:7116 | http://localhost:5198 | MVC UI              |
| Api  | https://localhost:7164 | http://localhost:5285 | Swagger at `/swagger` |

### Tests

```bash
dotnet test
```

Every story PR must include xUnit tests whose **method names** cite the AC they prove, e.g. `SignIn_WithValidCredentials_RedirectsEmployeeToBook_US001_AC01`. CI validates citations via `node tools/aidlc-check.mjs`.

### Framework check (before opening a PR)

```bash
node tools/aidlc-check.mjs
```

---

## Authentication and routing

| Concern              | Web (MVC)                         | API                          |
| -------------------- | --------------------------------- | ---------------------------- |
| Sign-in              | `AccountController.Login` POST    | `POST /api/auth/login` → JWT |
| Session              | Cookie middleware                 | Bearer token on `/api/*`     |
| After login          | Employee → `/Book`; Admin → `/Admin/Bookings` | Claims: `sub`, `email`, `role` |
| Deactivated user     | Rejected at login (generic message) | Same                        |
| Admin-only areas     | `[Authorize(Roles = "Admin")]`  | `[Authorize(Roles = "Admin")]` on `/api/admin/*` |

First Admin is seeded by **`DbInitializer`** when no users exist (PO/Architect approved 2026-08-21).

---

## Screens and stories

| Screen  | Route (MVC)              | Story coverage        |
| ------- | ------------------------ | --------------------- |
| SCR-001 | `/Account/Login`         | US-001                |
| SCR-002 | `/Book`                  | US-002                |
| SCR-003 | `/MyBookings`            | US-003                |
| SCR-004 | `/Admin/Bookings`        | US-004                |
| SCR-005 | `/Admin/Desks`           | US-005                |
| SCR-006 | `/Admin/Users`           | US-006                |
| SCR-007 | `/Settings/Notifications`| US-008                |

Email notifications: US-007 · Booking auto-completion: US-009

Design tokens: `inception/design/tokens.css` — navy/green palette per wireframes.

---

## Delivery status (baseline 2026-08-27)

| Area                         | Status                                              |
| ---------------------------- | --------------------------------------------------- |
| Gate 1 artifacts             | BRD, stories, screens, architecture drafted         |
| Solution scaffold            | **Done** — Domain, Application, Infrastructure, Web, Api, Tests |
| US-001 Sign in               | **In progress** — scaffold only; auth not wired     |
| US-002 … US-009              | Not started (per delivery plan)                     |
| EF migrations                | Not yet applied — `AppDbContext` is empty scaffold |
| CI (`aidlc-check`)           | Workflow present; branch protection TBD (private repo) |

Delivery order: see `inception/stories/delivery-plan-EPIC-001.md`. One story = one PR on branch `feat/US-###-<slug>`.

---

## Conventions for this codebase

| Topic              | Rule                                                                 |
| ------------------ | -------------------------------------------------------------------- |
| Story branches     | `feat/US-###-<slug>`                                                 |
| Test naming        | Include `US-###` and `AC-##` in the test method name                 |
| API routes         | `/api/...`; admin under `/api/admin/...`                             |
| Time / “today”     | Always via `IOfficeClock` and configured `Office:TimeZone` (NFR-001) |
| Errors (API)       | Problem Details (RFC 7807); no stack traces to clients               |
| Errors (MVC)       | ModelState + shared error views                                      |
| Secrets            | User secrets / env vars — never commit SMTP, JWT keys, or VAPID keys |
| Generated client   | N/A — no SPA; OpenAPI is the API contract for external consumers     |

Project-specific task tiers and protected paths: **`ai/standards/task-surfaces.md`** (still carries the framework NestJS seed — rewrite on first Complex story).

Project coding rules: **`ai/standards/coding-standards.md`** (same — replace NestJS/Angular sections with .NET conventions as stories land).

---

## Configuration keys (expected)

| Key / section           | Purpose                              |
| ----------------------- | ------------------------------------ |
| `ConnectionStrings:DefaultConnection` | SQL Server                  |
| `Office:TimeZone`       | IANA timezone for booking dates      |
| `Jwt:*`                 | API token signing (Api host)         |
| `Smtp:*`                | MailKit outbound email               |
| `WebPush:*`             | VAPID keys for browser push          |

Local overrides: `appsettings.Development.local.json` per host project.

---

## Open questions (from BRD — do not invent answers)

| # | Question                         | Owner     |
| - | -------------------------------- | --------- |
| 2 | Holiday calendar                 | PO/client |
| 3 | Reminder time (default 08:00)    | PO/client |
| 4 | Mobile vs desktop priority       | PO/client |
| 6 | Desk deactivate UX detail        | PO/client |
| 7 | Production SMTP / sender address | PO/IT     |

Escalate to the human PO/BA; do not code assumptions for these.

---

## Where to go deeper

| Need                          | Location                                           |
| ----------------------------- | -------------------------------------------------- |
| Business rules & requirements | `inception/product/requirements/BRD-001-desk-booking.md` |
| User stories & AC             | `inception/stories/user-stories/US-###-*.md`       |
| DB entities & constraints     | `inception/architecture/db-design.md`              |
| Services, API routes, jobs    | `inception/architecture/app-architecture.md`       |
| Screen states & UI            | `inception/design/`                                |
| Traceability graph            | `knowledge/traceability/manifest.json`             |
| AI-DLC gates & personas       | `ai/AI-DLC.md`                                     |

Once EF migrations and Swashbuckle are live, **migrations and OpenAPI supersede** the Inception architecture docs for schema and API shape — treat those docs as history, not a second source of truth.
