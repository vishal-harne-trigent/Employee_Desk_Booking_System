# US-006 — Admin manage users

| **Story** | US-006 | **Traces to** | REQ-001, REQ-003, REQ-004, REQ-005, REQ-018–022, NFR-004 |
| **Screen** | SCR-006 | **Tier** | Complex | **Status** | implemented | **Updated** | 2026-08-25 |

## Problem

Only seed users exist; Admins have no UI to create or maintain accounts. `IUserRepository` supports sign-in lookup only (`FindByEmailAsync` at `UserRepository.cs:10`). `AuthService` already blocks inactive users (`AuthService.cs:24-27`).

## Functional requirements

| ID | Requirement | Serves | Status |
| --- | --- | --- | --- |
| FR-01 | Create user with email, name, role, password | AC-01 | not started |
| FR-02 | Reject duplicate email on create/edit | AC-02 | not started |
| FR-03 | Edit name and email | AC-03 | not started |
| FR-04 | Deactivate user (not last admin) | AC-04 | not started |
| FR-05 | Reset password; show once to Admin | AC-05 | not started |
| FR-06 | Change role Employee ↔ Admin | AC-06 | not started |
| FR-07 | Block deactivate/role change for last active Admin | AC-07 | not started |

## Out of scope

- Password complexity V-12 (TBD) — enforce non-empty, min 8 chars until decided
- First Admin bootstrap / reactivate user (not in ACs)
