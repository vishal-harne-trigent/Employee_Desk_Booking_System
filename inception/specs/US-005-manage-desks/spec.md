# US-005 — Admin manage desks

|                   |                                                                 |
| ----------------- | --------------------------------------------------------------- |
| **Story**         | `inception/stories/user-stories/US-005-manage-desks.md`          |
| **Traces to**     | REQ-001, REQ-003, REQ-004, REQ-015, REQ-016, REQ-017, NFR-004   |
| **Screen**        | SCR-005 — Manage Desks                                          |
| **Covering ADRs** | none — design in `inception/architecture/app-architecture.md` § Desks |
| **Tier**          | Complex                                                         |
| **Status**        | implemented                                                     |
| **Updated**       | 2026-08-25                                                      |

## Problem

Desk inventory exists in the database (`Desk.cs`, seeded in `DbInitializer.cs:53-71`) but Admins have no UI or service to add, edit, or activate/deactivate desks. Employee booking already excludes inactive desks via `GetActiveDesksAsync` (`BookingRepository` / `DeskRepository`).

## Functional requirements

| ID    | Requirement                                                                 | Priority | Serves | Status      |
| ----- | --------------------------------------------------------------------------- | -------- | ------ | ----------- |
| FR-01 | Admin adds a desk with a unique number; created as Active                   | Must     | AC-01  | not started |
| FR-02 | Duplicate desk number on add/edit rejected with validation error (V-08)     | Must     | AC-02  | not started |
| FR-03 | Admin edits an existing desk number to another unique value                 | Must     | AC-03  | not started |
| FR-04 | Admin deactivates desk with no Confirmed today/future bookings              | Must     | AC-04  | not started |
| FR-05 | Deactivate blocked when Confirmed bookings exist today or future (V-09)     | Must     | AC-05  | not started |
| FR-06 | Admin reactivates an Inactive desk                                          | Should   | edge   | not started |

## Non-functional requirements

| ID     | Requirement                                      | Serves  |
| ------ | ------------------------------------------------ | ------- |
| NFR-01 | Admin-only access                                | NFR-004 |

## Technical constraints

- Reuse unique index on `DeskNumberNormalized` (`DeskConfiguration.cs:24-25`) — normalize case-insensitively.
- Block-only deactivate per BR-001.9 default (no cascade cancel).
- No schema migration.

## Out of scope

- User management (US-006)
- Cancel bookings during deactivate flow (open question #6 — block only)
