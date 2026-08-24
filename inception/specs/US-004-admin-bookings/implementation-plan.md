# US-004 — implementation plan

|           |                                                              |
| --------- | ------------------------------------------------------------ |
| **Story** | `inception/stories/user-stories/US-004-admin-bookings.md`     |
| **Spec**  | `spec.md`                                                    |
| **Tier**  | Complex                                                      |

## Approval — Gate D1

| Field                | Value           |
| -------------------- | --------------- |
| Status               | approved |
| Approved by          | Vishal Harne <vharne@degreed.com> |
| Approved on          | 2026-08-25 |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f |

## Steps

### Step 1 — Repository and application layer

| Field    | Value |
| -------- | ----- |
| Advances | FR-01, FR-02, FR-03, FR-04, FR-05 |
| Files    | `AdminBookingFilters`, `AdminBookingItem` models; `IBookingRepository.GetAllAsync`, `GetByIdAsync`; `IBookingService.GetAllBookingsAsync`, `CancelBookingAsAdminAsync`; `BookingService` implementations |
| Verify   | `dotnet build` |

### Step 2 — Admin MVC UI (SCR-004)

| Field    | Value |
| -------- | ----- |
| Advances | FR-01 … FR-06 (UI) |
| Files    | `AdminBookingsViewModel`; extend `Areas/Admin/Controllers/BookingsController` (Index with filters, StartCancel, ConfirmCancel); replace stub `Areas/Admin/Views/Bookings/Index.cshtml` |
| Verify   | Sign in as `admin@trigent.com`; view all bookings; filter; cancel eligible row |

### Step 3 — Admin API endpoints

| Field    | Value |
| -------- | ----- |
| Advances | FR-01 … FR-05 (API) |
| Files    | `Api/Controllers/AdminBookingsController.cs`, request/response DTOs |
| Verify   | Swagger — `GET /api/admin/bookings?date=&status=`; `POST /api/admin/bookings/{id}/cancel` with Admin JWT |

### Step 4 — Traceability and verification

| Field    | Value |
| -------- | ----- |
| Advances | all FR/NFR |
| Files    | `traceability.md`, `manifest.json` tests[], `change-log.md` |
| Verify   | `dotnet build`; `node tools/aidlc-check.mjs` |

## Rollback

Revert the story PR. No migration to roll back. Cancelled bookings remain cancelled (acceptable for demo).

## Open questions

| Question | Owner | Blocks |
| -------- | ----- | ------ |
| ~~Include AC-citing automated tests?~~ | Vishal | **Resolved:** skip tests (same as US-001–003) |
| ~~Branch timing for US-002/003?~~ | Vishal | **Resolved:** implement on current working tree |
