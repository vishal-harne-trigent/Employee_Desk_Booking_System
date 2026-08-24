# US-006 — impact analysis

**Tier:** Complex. **Surfaces:** Contract (new admin user API writes), Trust (Admin auth, password handling). **Persistence:** existing `Users` table, no migration.

## Regression risk

| Area | Risk | Covered by |
| ---- | ---- | ---------- |
| Sign-in (US-001) | Deactivated user must fail login | existing AuthService |

## Deliberately not touched

- Self-service password change (not in story)
