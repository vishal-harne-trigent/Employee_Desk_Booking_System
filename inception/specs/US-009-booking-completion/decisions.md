# US-009 — decisions

| **Updated** | 2026-08-25 |

| ID | Decision | Rationale |
| -- | -------- | --------- |
| D1 | Daily hosted job at 00:05 office local | Matches app architecture; idempotent batch update |
| D2 | Job runs on Web host only | Same pattern as reminder email job |
| D3 | Use `officeClock.Today` as cutoff | AC-03 — today's bookings stay Confirmed all day |
