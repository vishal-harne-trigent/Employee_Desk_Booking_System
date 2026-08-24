# US-005 — impact analysis

| **Story** | US-005 | **Tier** | Complex | **Updated** | 2026-08-25 |

## Surfaces crossed

| Surface | Crossed? | What exactly |
| ------- | -------- | ------------ |
| Contract | yes | `GET/POST/PATCH /api/admin/desks` |
| Persistence | no | Existing `Desks` table |
| Trust | yes | Admin role on MVC + API |
| Dependency | no | — |
| Operational | no | — |

## Regression risk

| Area | Risk | Covered by |
| ---- | ---- | ---------- |
| Employee booking availability | medium — deactivate must exclude desk | manual |
| Duplicate desk numbers | low — unique index + app check | manual |

## Deliberately not touched

- Booking service logic beyond existing inactive-desk filter
