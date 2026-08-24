# US-001 — impact analysis

|             |                                                  |
| ----------- | ------------------------------------------------ |
| **Story**   | `inception/stories/user-stories/US-001-sign-in.md` |
| **Tier**    | Complex                                          |
| **Updated** | 2026-08-24                                       |

## Surfaces crossed

| Surface                  | Crossed? | What exactly                                    |
| ------------------------ | -------- | ----------------------------------------------- |
| Contract                 | yes      | `/Account/Login`, `/Account/Logout`, `POST /api/auth/login` |
| Persistence              | yes      | `Users` table, initial EF migration             |
| Trust                    | yes      | Cookie auth, JWT, password verify, role claims  |
| Dependency & integration | yes      | `Microsoft.Extensions.Identity.Core`, JWT Bearer |
| Operational              | yes      | DbInitializer on Web startup, JWT config keys   |

## Files and callers

| File                              | Symbol              | Change   | Callers found                         |
| --------------------------------- | ------------------- | -------- | ------------------------------------- |
| `Web/Program.cs`                  | middleware pipeline | add auth | ASP.NET host                          |
| `Api/Program.cs`                  | middleware pipeline | add JWT  | ASP.NET host                          |
| `Infrastructure/AppDbContext.cs`  | `Users` DbSet       | add      | repositories, initializer             |
| `Application/IAuthService.cs`     | `SignInAsync`       | new      | `AccountController`, `AuthController` |

## Regression risk

| Area           | Risk   | Why                              | Covered by        |
| -------------- | ------ | -------------------------------- | ----------------- |
| Health endpoint | low   | Anonymous route unchanged        | manual smoke      |
| Home scaffold  | low    | Redirect behaviour added         | manual smoke      |

## Deliberately not touched

- Booking, desk, and user admin services (later stories)
- Email and push infrastructure
- Automated test project (human scope reduction)
