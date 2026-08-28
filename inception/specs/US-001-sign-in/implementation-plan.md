# US-001 — implementation plan

> **The Gate D1 artifact.** The human reads this file and `impact-analysis.md`, then approves in chat. DEV stamps the approval below; the developer commits the stamp. No code is written before that stamp exists.

|           |                                                  |
| --------- | ------------------------------------------------ |
| **Story** | `inception/stories/user-stories/US-001-sign-in.md` |
| **Spec**  | `spec.md`                                        |
| **Tier**  | Complex                                          |

## Approval — Gate D1

| Field                | Value           |
| -------------------- | --------------- |
| Status               | awaiting review |
| Approved by          | —               |
| Approved on          | —               |
| Plan commit approved | —               |

## Steps

Ordered. Test-first per AC: failing test named `... (US-001/AC-##)` before implementation.

### Step 1 — Domain and persistence

| Field    | Value |
| -------- | ----- |
| Advances | FR-07 |
| Files    | `EmployeeDeskBooking.Domain/Entities/User.cs` (create), `EmployeeDeskBooking.Infrastructure/Data/AppDbContext.cs` (modify), `EmployeeDeskBooking.Infrastructure/Data/DbInitializer.cs` (create), initial EF migration |
| Verify   | `dotnet build` — expected: success; `dotnet ef migrations add InitialUsers --project src/EmployeeDeskBooking.Infrastructure --startup-project src/EmployeeDeskBooking.Web` — expected: migration files created |

### Step 2 — Application auth service

| Field    | Value |
| -------- | ----- |
| Advances | FR-04, FR-05 |
| Files    | `IAuthService.cs`, `AuthService.cs`, `DependencyInjection.cs` (Application), unit tests `AuthServiceTests.cs` |
| Verify   | `dotnet test tests/EmployeeDeskBooking.Tests --filter "US-001"` — expected: red until Step 3–4 complete |

### Step 3 — Sign-in AC tests (WebApplicationFactory)

| Field    | Value |
| -------- | ----- |
| Advances | FR-02, FR-03, FR-04, FR-05 |
| Files    | `SignInTests.cs` (or equivalent) citing `US-001/AC-01` … `AC-04` |
| Verify   | `dotnet test` — expected: AC tests fail until Step 4 |

### Step 4 — AccountController and SCR-001 view

| Field    | Value |
| -------- | ----- |
| Advances | FR-01, FR-02, FR-03, FR-06 |
| Files    | `AccountController.cs`, `Views/Account/Login.cshtml`, cookie auth in `Program.cs`, `_Layout` sign-out |
| Verify   | `dotnet test` — expected: all US-001 AC tests green; manual `dotnet run --project src/EmployeeDeskBooking.Web` — login redirects Employee to `/Book`, Admin to `/Admin/Bookings` |

### Step 5 — Sign-out AC test

| Field    | Value |
| -------- | ----- |
| Advances | FR-06 |
| Files    | `SignOutTests.cs` citing `US-001/AC-05` |
| Verify   | `dotnet test` — expected: AC-05 green |

## Rollback

Revert the story PR. Drop migration with `dotnet ef database update 0` in dev if applied locally.

## Open questions

| Question | Owner | Blocks |
| -------- | ----- | ------ |
| Deactivated-account banner copy (SCR-001 conflict #1) | PO/client | Step 4 view text only — default: "This account has been deactivated. Contact your administrator." |
