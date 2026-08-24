# US-008 — implementation plan

## Approval — Gate D1

| Field | Value |
| ----- | ----- |
| Status | approved |
| Approved by | Vishal Harne <vharne@degreed.com> |
| Approved on | 2026-08-25 |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f |

## Steps

1. `NotificationPreferences` entity + migration
2. Preference service + WebPush sender (file fallback in dev)
3. Hook push into booking notifications; SCR-007 UI + service worker
4. API endpoints + traceability

## Open questions

| Question | Resolution |
| -------- | ---------- |
| Tests | Skip (US-001–007 pattern) |
| VAPID keys | Config placeholders; file sender when empty |
