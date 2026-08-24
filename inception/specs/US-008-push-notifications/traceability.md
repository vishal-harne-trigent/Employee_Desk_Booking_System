# US-008 — traceability

| **Updated** | 2026-08-25 |

| Req | File | Symbol | Status |
| --- | ---- | ------ | ------ |
| FR-01 | `src/EmployeeDeskBooking.Application/Notifications/NotificationPreferenceService.cs` | default opt-out | implemented |
| FR-02 | `src/EmployeeDeskBooking.Web/Controllers/NotificationSettingsController.cs` | EnablePush | implemented |
| FR-03 | `src/EmployeeDeskBooking.Application/Notifications/BookingNotificationService.cs` | TrySendPushAsync | implemented |
| FR-04 | `src/EmployeeDeskBooking.Web/Controllers/NotificationSettingsController.cs` | DisablePush | implemented |
| FR-05 | `src/EmployeeDeskBooking.Application/Notifications/BookingNotificationService.cs` | SendDueRemindersAsync | implemented |

## Key symbols

| Symbol | Location |
| ------ | -------- |
| `INotificationPreferenceService` | `src/EmployeeDeskBooking.Application/Notifications/INotificationPreferenceService.cs` |
| `NotificationsController` | `src/EmployeeDeskBooking.Api/Controllers/NotificationsController.cs` |
