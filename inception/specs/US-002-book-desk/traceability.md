# US-002 — traceability

| **Updated** | 2026-08-25 |

## Requirement to code

| Req    | File                                                              | Symbol              | Proven by | Status      |
| ------ | ----------------------------------------------------------------- | ------------------- | --------- | ----------- |
| FR-01  | `src/EmployeeDeskBooking.Web/Controllers/BookController.cs`     | CheckAvailability   | —         | implemented |
| FR-02  | `src/EmployeeDeskBooking.Application/Bookings/BookingDateRules.cs`| Validate            | —         | implemented |
| FR-03  | `src/EmployeeDeskBooking.Web/Views/Book/Index.cshtml`           | desk table          | —         | implemented |
| FR-04  | `src/EmployeeDeskBooking.Application/Bookings/BookingService.cs`| CreateBookingAsync  | —         | implemented |
| FR-05  | `src/EmployeeDeskBooking.Application/Bookings/BookingService.cs`| CreateBookingAsync  | —         | implemented |
| FR-06  | `src/EmployeeDeskBooking.Application/Bookings/BookingService.cs`| CreateBookingAsync  | —         | implemented |
| FR-07  | `src/EmployeeDeskBooking.Api/Controllers/BookingsController.cs`   | API routes          | —         | implemented |
| NFR-01 | `src/EmployeeDeskBooking.Infrastructure/Time/OfficeClock.cs`      | Today               | —         | implemented |
| NFR-02 | `src/EmployeeDeskBooking.Infrastructure/Data/Configurations/DeskConfiguration.cs` | filtered unique indexes | — | implemented |
