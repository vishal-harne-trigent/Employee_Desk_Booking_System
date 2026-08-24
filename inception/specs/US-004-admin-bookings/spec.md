# US-004 — Admin view and cancel all bookings

|                   |                                                                 |
| ----------------- | --------------------------------------------------------------- |
| **Story**         | `inception/stories/user-stories/US-004-admin-bookings.md`        |
| **Traces to**     | REQ-001, REQ-003, REQ-011, REQ-012, REQ-013, REQ-014, NFR-004   |
| **Screen**        | SCR-004 — Admin All Bookings                                    |
| **Covering ADRs** | none — design in `inception/architecture/app-architecture.md` § Bookings |
| **Tier**          | Complex                                                         |
| **Status**        | implemented                                                     |
| **Updated**       | 2026-08-25                                                      |

## Problem

The Admin **All Bookings** page is a stub (`Areas/Admin/Views/Bookings/Index.cshtml:7`). Admins cannot list employee bookings, filter by date or status, or cancel on an employee's behalf. `IBookingService` has employee-scoped list/cancel only (`IBookingService.cs:16-23`); no admin query or admin cancel path exists.

## Functional requirements

| ID    | Requirement                                                                 | Priority | Serves | Status      |
| ----- | --------------------------------------------------------------------------- | -------- | ------ | ----------- |
| FR-01 | Admin sees all bookings with date, desk, employee email, and status         | Must     | AC-01  | not started |
| FR-02 | Admin filters the list by a single booking date                             | Must     | AC-02  | not started |
| FR-03 | Admin filters the list by Confirmed, Cancelled, or Completed              | Must     | AC-03  | not started |
| FR-04 | Admin cancels a Confirmed today-or-future booking on behalf of the employee | Must     | AC-04  | not started |
| FR-05 | Past or Completed bookings cannot be cancelled (same BR-001.6 as employee)  | Must     | AC-04  | not started |
| FR-06 | Empty filter results show SCR-004 ST-03 empty state with clear-filters link | Should   | AC-02, AC-03 | not started |

## Non-functional requirements

| ID     | Requirement                                      | Serves  |
| ------ | ------------------------------------------------ | ------- |
| NFR-01 | Admin-only access; employees receive 403/redirect | NFR-004 |

## Technical constraints

- Reuse `BookingCancellationRules.CanCancel` (`BookingDateRules.cs:30-34`) — no separate admin cancel rule.
- Admin cancel sets `CancelledById` to the admin's user id (`Booking.cs:23`).
- Follow existing MVC modal pattern from My Bookings (`MyBookingsController.cs`, `Views/MyBookings/Index.cshtml`).
- API routes per architecture: `GET /api/admin/bookings`, `POST /api/admin/bookings/{id}/cancel` (`app-architecture.md:212`).
- No schema migration — query existing `Bookings` table.

## Out of scope

- Desk management (US-005), user management (US-006)
- Auto-completing past bookings (US-009) — Completed rows may appear from seed data only
- Email notifications on admin cancel (US-007)
