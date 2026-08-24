# US-003 — View and cancel my bookings

| **Story** | `inception/stories/user-stories/US-003-my-bookings.md` |
| **Traces to** | REQ-001, REQ-003, REQ-009, REQ-010, NFR-004 |
| **Screen** | SCR-003 — My Bookings |
| **Tier** | Medium |
| **Status** | implemented |
| **Updated** | 2026-08-25 |

## Problem

Employees can book desks but cannot view or cancel their reservations. The system must list all bookings with status and allow cancellation of Confirmed bookings for today or future dates only.

## Functional requirements

| ID    | Requirement | Serves | Status |
| ----- | ----------- | ------ | ------ |
| FR-01 | List employee bookings with date, desk, status | AC-01 | implemented |
| FR-02 | Cancel Confirmed booking for today or future | AC-02 | implemented |
| FR-03 | No cancel action for past or Completed | AC-03 | implemented |
| FR-04 | Empty state with link to Book Desk | AC-04 | implemented |
| FR-05 | API list mine + cancel endpoints | API | implemented |

## Out of scope

- Admin cancel (US-004)
- Auto-complete to Completed (US-009)
- No automated tests (human scope reduction)
