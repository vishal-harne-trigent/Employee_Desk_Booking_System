# US-001 — impact analysis

> What this change touches, written **before** it touches anything. Read at Gate D1 next to the plan.

|             |                                                  |
| ----------- | ------------------------------------------------ |
| **Story**   | `inception/stories/user-stories/US-001-sign-in.md` |
| **Tier**    | Complex                                          |
| **Updated** | 2026-08-28                                       |

## Surfaces crossed

| Surface                  | Crossed? | What exactly |
| ------------------------ | -------- | ------------ |
| Contract                 | yes      | MVC routes `Account/Login`, `Account/Logout`; form POST contract |
| Persistence              | yes      | `User` entity, EF migration, `DbInitializer` seed |
| Trust                    | yes      | Cookie auth middleware, `IAuthService`, password verify, role claims |
| Dependency & integration | no       | — |
| Operational              | no       | — |

## Files and callers

Greenfield in scaffold — no existing auth callers. New symbols:

| File (planned) | Symbol | Change | Callers found |
| -------------- | ------ | ------ | ------------- |
| `EmployeeDeskBooking.Domain/Entities/User.cs` | `User` | create | — |
| `EmployeeDeskBooking.Application/Services/IAuthService.cs` | `IAuthService` | create | `AccountController` (planned) |
| `EmployeeDeskBooking.Web/Controllers/AccountController.cs` | `Login`, `Logout` | create | Razor views, `_Layout` nav |
| `EmployeeDeskBooking.Infrastructure/Data/AppDbContext.cs` | `Users` DbSet | modify | repositories |

## Regression risk

| Area | Risk | Why | Covered by |
| ---- | ---- | --- | ---------- |
| Home/Health scaffold | low | Unauthenticated `/` remains reachable until `[Authorize]` on feature controllers | future stories |

## Deliberately not touched

- `EmployeeDeskBooking.Api` — no JWT wiring in this story PR unless scope expands.
- Booking, admin, notification controllers — not created yet.
