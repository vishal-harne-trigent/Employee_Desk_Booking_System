# US-009 — implementation plan

## Approval — Gate D1

| Field | Value |
| ----- | ----- |
| Status | approved |
| Approved by | Vishal Harne <vharne@degreed.com> |
| Approved on | 2026-08-25 |
| Plan commit approved | 1e29b8c |

## Steps

1. Repository query for Confirmed bookings before office-local today
2. `CompletePastBookingsAsync` on `BookingService`
3. `CompletePastBookingsHostedService` — daily 00:05 office local
4. Register job on Web host; update seed + traceability

## Open questions

| Question | Resolution |
| -------- | ---------- |
| Tests | Skip (US-001–008 pattern) |
| Schedule | 00:05 office local per app architecture |
