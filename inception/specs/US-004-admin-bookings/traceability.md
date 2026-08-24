# US-004 — traceability

|             |                                                              |
| ----------- | ------------------------------------------------------------ |
| **Story**   | `inception/stories/user-stories/US-004-admin-bookings.md`     |
| **Updated** | 2026-08-25                                                   |

## Requirement to code

| Req    | File | Symbol / location | Proven by | Status      |
| ------ | ---- | ----------------- | --------- | ----------- |
| FR-01  | `src/EmployeeDeskBooking.Application/Bookings/BookingService.cs` | `GetAllBookingsAsync` | manual | implemented |
| FR-02  | `src/EmployeeDeskBooking.Infrastructure/Repositories/BookingRepository.cs` | `GetAllAsync` date filter | manual | implemented |
| FR-03  | `src/EmployeeDeskBooking.Infrastructure/Repositories/BookingRepository.cs` | `GetAllAsync` status filter | manual | implemented |
| FR-04  | `src/EmployeeDeskBooking.Application/Bookings/BookingService.cs` | `CancelBookingAsAdminAsync` | manual | implemented |
| FR-05  | `src/EmployeeDeskBooking.Application/Bookings/BookingDateRules.cs` | `BookingCancellationRules.CanCancel` | manual | implemented |
| FR-06  | `src/EmployeeDeskBooking.Web/Areas/Admin/Views/Bookings/Index.cshtml` | empty filter state | manual | implemented |
| NFR-01 | `src/EmployeeDeskBooking.Web/Areas/Admin/Controllers/BookingsController.cs` | `[Authorize(Roles = Admin)]` | manual | implemented |

## Key symbols

| Symbol | Location |
| ------ | -------- |
| `GetAllBookingsAsync` | `src/EmployeeDeskBooking.Application/Bookings/IBookingService.cs` |
| `CancelBookingAsAdminAsync` | `src/EmployeeDeskBooking.Application/Bookings/IBookingService.cs` |
| `AdminBookingsController` | `src/EmployeeDeskBooking.Api/Controllers/AdminBookingsController.cs` |
