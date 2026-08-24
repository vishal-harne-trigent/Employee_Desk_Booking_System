# US-001 — decisions

| ID   | Decision                              | Rationale                                           | Alternatives rejected                    |
| ---- | ------------------------------------- | --------------------------------------------------- | ---------------------------------------- |
| D-01 | `IPasswordVerifier` in Application    | Keeps Application free of ASP.NET Identity types    | Inject `IPasswordHasher<User>` into Application |
| D-02 | Distinct deactivated error message    | Story AC-04 overrides architecture generic-error note | Same message for all failures (V-01)     |
| D-03 | Stub Book / Admin Bookings pages      | Redirect targets required before US-002/US-004      | Redirect to Home placeholder             |
| D-04 | No automated tests this delivery      | Human explicitly scoped out tests                   | Test-first per AC (framework default)    |
