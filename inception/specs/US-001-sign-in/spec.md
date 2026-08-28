# US-001 — Sign in and sign out

> The technical expansion of one approved story. The story says what the business needs; this says what the code must do. Written by DEV, reviewed by the human at Gate D1 alongside `implementation-plan.md`.

|                   |                                                                 |
| ----------------- | --------------------------------------------------------------- |
| **Story**         | `inception/stories/user-stories/US-001-sign-in.md`              |
| **Traces to**     | REQ-001, REQ-002, REQ-003, REQ-004, REQ-005, NFR-003, NFR-004   |
| **Screen**        | SCR-001 — Sign In                                               |
| **Covering ADRs** | none — auth design is in `inception/architecture/app-architecture.md` §Authentication |
| **Tier**          | Complex                                                         |
| **Status**        | draft                                                           |
| **Updated**       | 2026-08-28                                                      |

## Problem

The Web host runs with placeholder `UseAuthorization()` and no sign-in surface. Employees and Admins cannot authenticate; SCR-001 is undelivered. The system must establish cookie-based sessions for the MVC app, validate credentials against persisted users, route by role after login, and clear sessions on sign-out.

## Functional requirements

| ID    | Requirement | Priority | Serves | Status      |
| ----- | ----------- | -------- | ------ | ----------- |
| FR-01 | Present SCR-001 sign-in form at `/Account/Login` for anonymous users | Must | AC-03, AC-04 | not started |
| FR-02 | On valid Employee credentials, create auth cookie and redirect to `Book/Index` (SCR-002) | Must | AC-01 | not started |
| FR-03 | On valid Admin credentials, create auth cookie and redirect to `AdminBookings/Index` (SCR-004) | Must | AC-02 | not started |
| FR-04 | Reject unknown email or wrong password with one generic error (V-01); do not create a session | Must | AC-03 | not started |
| FR-05 | Reject deactivated accounts with a distinct deactivated message (SCR-001 ST-04); do not create a session | Must | AC-04 | not started |
| FR-06 | Sign-out POST clears the auth cookie and redirects to `/Account/Login` | Must | AC-05 | not started |
| FR-07 | Seed at least one Employee and one Admin via `DbInitializer` when no users exist (BRD Q #1) | Must | QA notes | not started |

## Non-functional requirements

| ID     | Requirement | Serves |
| ------ | ----------- | ------ |
| NFR-01 | Auth cookie `HttpOnly`; `Secure` and `SameSite=Strict` in non-Development (NFR-003) | NFR-003 |
| NFR-02 | Sign-in view uses Bootstrap 5 and design tokens from site CSS (NFR-004) | NFR-004 |

## Technical constraints

- Presentation calls **Application** only (`IAuthService`); Web must not use `AppDbContext` directly.
- Password verification uses `IPasswordHasher<User>`; passwords never logged (RISK-005).
- Role claim values `Employee` and `Admin` match REQ-004 and `[Authorize(Roles = "...")]`.

## Out of scope

- JWT Bearer API login (`POST /api/auth/login`) — same architecture doc, separate delivery slice if needed for API clients.
- Self-service password reset, SSO, registration (BRD §10).
- Browser push, booking, admin CRUD (later stories).
