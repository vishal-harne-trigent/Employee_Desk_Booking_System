# US-001 — implementation plan

> **The Gate D1 artifact.** The human reads this file and `impact-analysis.md`, then approves in chat. DEV stamps the approval below; the developer commits the stamp. No code is written before that stamp exists.

|           |                                                  |
| --------- | ------------------------------------------------ |
| **Story** | `inception/stories/user-stories/US-001-sign-in.md` |
| **Spec**  | `spec.md`                                        |
| **Tier**  | Complex                                          |

## Approval — Gate D1

| Field                | Value                                              |
| -------------------- | -------------------------------------------------- |
| Status               | approved                                           |
| Approved by          | Vishal Harne <vharne@degreed.com>                  |
| Approved on          | 2026-08-24                                         |
| Plan commit approved | 57cb0ee73afd71624fca848e91e81e90bf0b9994           |

## Steps

### Step 1 — Domain and persistence

| Field    | Value                                                                 |
| -------- | --------------------------------------------------------------------- |
| Advances | FR-01 … FR-04 (data layer)                                            |
| Files    | `User` entity, `UserRole` enum, EF config, migration, `DbInitializer` |
| Verify   | `dotnet build` — expected: success                                    |

### Step 2 — Application auth service

| Field    | Value                                                          |
| -------- | -------------------------------------------------------------- |
| Advances | FR-01 … FR-04                                                  |
| Files    | `IAuthService`, `AuthService`, `IUserRepository`, `IPasswordVerifier` |
| Verify   | `dotnet build` — expected: success                             |

### Step 3 — Web cookie auth and SCR-001 login UI

| Field    | Value                                                                 |
| -------- | --------------------------------------------------------------------- |
| Advances | FR-01, FR-02, FR-03, FR-04, FR-05                                     |
| Files    | `AccountController`, Login view, cookie middleware, stub Book/Admin pages |
| Verify   | `dotnet run --project src/EmployeeDeskBooking.Web` — manual sign-in    |

### Step 4 — API JWT login

| Field    | Value                                      |
| -------- | ------------------------------------------ |
| Advances | FR-06                                      |
| Files    | `AuthController`, JWT middleware in Api      |
| Verify   | Swagger POST /api/auth/login — 200 + token   |

## Rollback

Revert the story PR. Drop the database or roll back the EF migration if schema was applied.

## Open questions

| Question | Owner | Blocks |
| -------- | ----- | ------ |
