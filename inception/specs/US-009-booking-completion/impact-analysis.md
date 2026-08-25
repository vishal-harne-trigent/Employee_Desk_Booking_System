# US-009 — impact analysis

| **Updated** | 2026-08-25 |

## Affected areas

| Layer | Change |
| ----- | ------ |
| Application | `CompletePastBookingsAsync` on booking service |
| Infrastructure | Repository query + hosted service |
| Web | Register completion job |
| Seed | Past demo booking starts Confirmed for job demo |

## No change

- API surface, MVC views, database schema (CompletedAt already exists)
