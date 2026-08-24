# US-001 — Sign in and sign out

> The technical expansion of one approved story. The story says what the business needs; this says what the code must do. Written by DEV, reviewed by the human at Gate D1 alongside `implementation-plan.md`.

|                   |                                                   |
| ----------------- | ------------------------------------------------- |
| **Story**         | `inception/stories/user-stories/US-001-sign-in.md` |
| **Traces to**     | REQ-001, REQ-002, REQ-003, REQ-004, REQ-005, NFR-003, NFR-004 |
| **Screen**        | SCR-001 — Sign In                                 |
| **Covering ADRs** | none                                              |
| **Tier**          | Complex                                           |
| **Status**        | implemented                                       |
| **Updated**       | 2026-08-24                                        |

## Problem

The MVC and API hosts exist as scaffolds with no authentication. Users cannot sign in, sessions are not created, and protected routes are unreachable. The system must authenticate Employees and Admins by email and password, route each role to the correct landing screen, reject invalid and deactivated accounts with the correct messages, and support sign-out.

## Functional requirements

| ID    | Requirement                                                                 | Priority | Serves | Status      |
| ----- | --------------------------------------------------------------------------- | -------- | ------ | ----------- |
| FR-01 | Valid Employee credentials create a session and redirect to Book Desk       | Must     | AC-01  | implemented |
| FR-02 | Valid Admin credentials create a session and redirect to Admin All Bookings | Must     | AC-02  | implemented |
| FR-03 | Unknown email or wrong password show a generic error; no session is created | Must     | AC-03  | implemented |
| FR-04 | Deactivated account with correct password shows deactivated message         | Must     | AC-04  | implemented |
| FR-05 | Sign out clears the session and returns to the sign-in screen               | Must     | AC-05  | implemented |
| FR-06 | API clients can obtain a JWT via POST /api/auth/login                       | Should   | API    | implemented |

## Non-functional requirements

| ID     | Requirement                                              | Serves  |
| ------ | -------------------------------------------------------- | ------- |
| NFR-01 | Auth cookie is HttpOnly; Secure in production            | NFR-003 |
| NFR-02 | Passwords verified via IPasswordHasher; never logged     | NFR-003 |

## Technical constraints

- Layered N-tier: Presentation calls Application only; EF stays in Infrastructure
- Story AC-04 uses a distinct deactivated message (overrides architecture doc generic-error note for deactivated users)
- DbInitializer seeds Admin + Employee when no users exist (architecture decision 2026-08-21)
- No automated tests in this delivery (human-directed scope reduction)

## Out of scope

- Forgot-password flow
- US-002 booking logic (stub landing pages only)
- Full admin CRUD (US-006)
