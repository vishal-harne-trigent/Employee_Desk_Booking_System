# US-002 — Book a desk

|                   |                                                   |
| ----------------- | ------------------------------------------------- |
| **Story**         | `inception/stories/user-stories/US-002-book-desk.md` |
| **Traces to**     | REQ-001, REQ-003, REQ-006, REQ-007, REQ-008, NFR-001, NFR-002, NFR-004 |
| **Screen**        | SCR-002 — Book a Desk                             |
| **Covering ADRs** | none                                              |
| **Tier**          | Complex                                           |
| **Status**        | implemented                                       |
| **Updated**       | 2026-08-25                                        |

## Problem

Employees can sign in and reach a Book desk stub, but cannot view availability or create bookings. The system must enforce the booking window, working-day rules, one booking per employee per day, and one booking per desk per day.

## Functional requirements

| ID    | Requirement                                                                 | Priority | Serves | Status      |
| ----- | --------------------------------------------------------------------------- | -------- | ------ | ----------- |
| FR-01 | Employee selects a date from today through +30 office-local working days    | Must     | AC-01  | implemented |
| FR-02 | Dates before today, after +30, or on weekends are rejected                  | Must     | AC-02  | implemented |
| FR-03 | Active desks show desk number and available/booked status                   | Must     | AC-03  | implemented |
| FR-04 | Employee confirms an available desk and gets a Confirmed booking            | Must     | AC-04  | implemented |
| FR-05 | Second booking same day by same employee is rejected                          | Must     | AC-05  | implemented |
| FR-06 | Inactive or already-booked desks cannot be booked                           | Must     | AC-06  | implemented |
| FR-07 | API exposes availability query and booking create                           | Should   | API    | implemented |

## Non-functional requirements

| ID     | Requirement                              | Serves  |
| ------ | ---------------------------------------- | ------- |
| NFR-01 | All “today” logic uses Office:TimeZone   | NFR-001 |
| NFR-02 | Concurrent desk book: one wins via DB index | NFR-002 |

## Technical constraints

- Filtered unique indexes on Bookings per db-design (BR-001.1, V-04)
- Layered N-tier; Presentation calls Application only
- No automated tests in this delivery (human scope reduction)

## Out of scope

- Cancel / change desk (US-003)
- Admin desk management (US-005)
- Email notifications (US-007)
