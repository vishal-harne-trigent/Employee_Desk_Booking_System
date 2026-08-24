# US-002 — impact analysis

| **Updated** | 2026-08-25 |

## Surfaces crossed

| Surface      | Crossed? | What exactly                                      |
| ------------ | -------- | ------------------------------------------------- |
| Contract     | yes      | GET availability, POST create booking             |
| Persistence  | yes      | Desks, Bookings tables, filtered unique indexes   |
| Trust        | yes      | Employee-only book routes                         |
| Operational  | yes      | Office:TimeZone config, IOfficeClock              |

## Deliberately not touched

- User auth (US-001)
- Cancel booking (US-003)
