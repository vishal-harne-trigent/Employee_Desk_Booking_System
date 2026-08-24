# US-007 — traceability

| **Updated** | 2026-08-25 |

| Req | File | Symbol | Status |
| --- | ---- | ------ | ------ |
| FR-01 | `src/EmployeeDeskBooking.Application/Notifications/BookingNotificationService.cs` | `SendConfirmationAsync` | implemented |
| FR-02 | same | `SendCancellationAsync` | implemented |
| FR-03 | `src/EmployeeDeskBooking.Application/Notifications/BookingEmailTemplates.cs` | templates | implemented |
| FR-04 | `src/EmployeeDeskBooking.Infrastructure/HostedServices/ReminderEmailHostedService.cs` | reminder job | implemented |
| FR-05 | `src/EmployeeDeskBooking.Infrastructure/Repositories/EmailDeliveryRepository.cs` | delivery logs | implemented |

## Key symbols

| Symbol | Location |
| ------ | -------- |
| `IBookingNotificationService` | `src/EmployeeDeskBooking.Application/Notifications/IBookingNotificationService.cs` |
| `IEmailSender` | `src/EmployeeDeskBooking.Application/Notifications/IEmailSender.cs` |
