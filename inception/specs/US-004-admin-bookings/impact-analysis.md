# US-004 — impact analysis

|             |                                                              |
| ----------- | ------------------------------------------------------------ |
| **Story**   | `inception/stories/user-stories/US-004-admin-bookings.md`     |
| **Tier**    | Complex                                                      |
| **Updated** | 2026-08-25                                                   |

## Surfaces crossed

| Surface                  | Crossed? | What exactly                                                       |
| ------------------------ | -------- | ------------------------------------------------------------------ |
| Contract                 | yes      | New `GET /api/admin/bookings`; new `POST /api/admin/bookings/{id}/cancel` |
| Persistence              | no       | Read/write existing `Bookings` — no migration                      |
| Trust                    | yes      | Admin role on MVC area + API controllers (`AuthRoles.Admin`)       |
| Dependency & integration | no       | —                                                                  |
| Operational              | no       | —                                                                  |

## Files and callers

| File | Symbol | Change | Callers found |
| ---- | ------ | ------ | ------------- |
| `Application/Bookings/IBookingService.cs` | interface | add `GetAllBookingsAsync`, `CancelBookingAsAdminAsync` | Web Admin controller, API admin controller |
| `Application/Bookings/BookingService.cs` | service | implement admin list + cancel | DI registration unchanged |
| `Application/Bookings/IBookingRepository.cs` | interface | add `GetAllAsync`, `GetByIdAsync` | BookingService |
| `Infrastructure/Repositories/BookingRepository.cs` | repository | implement filtered query | BookingService |
| `Web/Areas/Admin/Controllers/BookingsController.cs` | controller | list, filter, cancel actions | Admin nav `_Layout.cshtml:26` |
| `Web/Areas/Admin/Views/Bookings/Index.cshtml` | view | SCR-004 table + filters + modal | BookingsController |
| `Api/Controllers/AdminBookingsController.cs` | controller | new — list + cancel endpoints | Swagger clients |

## Regression risk

| Area | Risk | Why | Covered by |
| ---- | ---- | --- | ---------- |
| Employee cancel (US-003) | medium | Shared cancel rules; admin path must not change employee ownership check | Existing `CancelBookingAsync` unchanged |
| Employee My Bookings list | low | Separate query path | Manual / test |
| Book desk availability (US-002) | low | Cancel frees desk; no change to availability logic | Manual |

## Deliberately not touched

- Employee `BookingsController` API and `MyBookingsController` — unchanged
- `CancelBookingAsync(userId, bookingId)` signature and ownership check — employee path stays as-is
- Desk and User admin stubs (US-005, US-006)
