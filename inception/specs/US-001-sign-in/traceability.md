# US-001 — traceability

|             |                                                  |
| ----------- | ------------------------------------------------ |
| **Story**   | `inception/stories/user-stories/US-001-sign-in.md` |
| **Updated** | 2026-08-24                                       |

## Requirement to code

| Req    | File                                                              | Symbol / location   | Proven by | Status      |
| ------ | ----------------------------------------------------------------- | ------------------- | --------- | ----------- |
| FR-01  | `src/EmployeeDeskBooking.Web/Controllers/AccountController.cs`    | Employee redirect   | —         | implemented |
| FR-02  | `src/EmployeeDeskBooking.Web/Controllers/AccountController.cs`    | Admin redirect      | —         | implemented |
| FR-03  | `src/EmployeeDeskBooking.Application/Auth/AuthService.cs`         | InvalidCredentials  | —         | implemented |
| FR-04  | `src/EmployeeDeskBooking.Application/Auth/AuthService.cs`         | DeactivatedAccount  | —         | implemented |
| FR-05  | `src/EmployeeDeskBooking.Web/Controllers/AccountController.cs`    | Logout              | —         | implemented |
| FR-06  | `src/EmployeeDeskBooking.Api/Controllers/AuthController.cs`       | Login               | —         | implemented |
| NFR-01 | `src/EmployeeDeskBooking.Web/Program.cs`                          | Cookie options      | —         | implemented |
| NFR-02 | `src/EmployeeDeskBooking.Infrastructure/Security/PasswordVerifier.cs` | Verify / Hash   | —         | implemented |

## Key symbols

| Symbol              | Location                                                          |
| ------------------- | ----------------------------------------------------------------- |
| `IAuthService`      | `src/EmployeeDeskBooking.Application/Auth/IAuthService.cs`        |
| `AuthService`       | `src/EmployeeDeskBooking.Application/Auth/AuthService.cs`         |
| `AccountController` | `src/EmployeeDeskBooking.Web/Controllers/AccountController.cs`    |
| `AuthController`    | `src/EmployeeDeskBooking.Api/Controllers/AuthController.cs`       |
| `DbInitializer`     | `src/EmployeeDeskBooking.Infrastructure/Data/DbInitializer.cs`    |
