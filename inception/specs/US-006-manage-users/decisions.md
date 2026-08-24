# US-006 — decisions

| ID | Decision | Rationale |
| --- | -------- | --------- |
| D-01 | `IUserAdminService` in `Application/Users/` | Keeps admin CRUD separate from sign-in `IAuthService` |
| D-02 | Auto-generate password on reset | BR-001.12 one-time display |
| D-03 | Min 8-char password until V-12 decided | Story edge case marks V-12 TBD |
