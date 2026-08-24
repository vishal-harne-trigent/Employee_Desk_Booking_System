# US-006 — traceability

| **Updated** | 2026-08-25 |

| Req | File | Symbol | Status |
| --- | ---- | ------ | ------ |
| FR-01 | `src/EmployeeDeskBooking.Application/Users/UserAdminService.cs` | `CreateUserAsync` | implemented |
| FR-02 | same | duplicate email check | implemented |
| FR-03 | same | `UpdateUserAsync` | implemented |
| FR-04 | same | `DeactivateUserAsync` | implemented |
| FR-05 | same | `ResetPasswordAsync` | implemented |
| FR-06 | same | role update | implemented |
| FR-07 | same | last-admin guard | implemented |
| NFR-01 | `src/EmployeeDeskBooking.Web/Areas/Admin/Controllers/UsersController.cs` | `[Authorize Admin]` | implemented |

## Key symbols

| Symbol | Location |
| ------ | -------- |
| `IUserAdminService` | `src/EmployeeDeskBooking.Application/Users/IUserAdminService.cs` |
| `AdminUsersController` | `src/EmployeeDeskBooking.Api/Controllers/AdminUsersController.cs` |
