# US-005 — traceability

| **Story** | US-005 | **Updated** | 2026-08-25 |

## Requirement to code

| Req | File | Symbol | Proven by | Status |
| --- | ---- | ------ | --------- | ------ |
| FR-01 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | `CreateDeskAsync` | manual | implemented |
| FR-02 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | duplicate check | manual | implemented |
| FR-03 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | `UpdateDeskNumberAsync` | manual | implemented |
| FR-04 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | `SetDeskStatusAsync` Inactive | manual | implemented |
| FR-05 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | `HasFutureBookings` guard | manual | implemented |
| FR-06 | `src/EmployeeDeskBooking.Application/Desks/DeskService.cs` | `SetDeskStatusAsync` Active | manual | implemented |
| NFR-01 | `src/EmployeeDeskBooking.Web/Areas/Admin/Controllers/DesksController.cs` | `[Authorize Admin]` | manual | implemented |

## Key symbols

| Symbol | Location |
| ------ | -------- |
| `IDeskService` | `src/EmployeeDeskBooking.Application/Desks/IDeskService.cs` |
| `AdminDesksController` | `src/EmployeeDeskBooking.Api/Controllers/AdminDesksController.cs` |
