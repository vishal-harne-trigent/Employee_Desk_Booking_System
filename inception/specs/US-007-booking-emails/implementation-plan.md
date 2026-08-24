# US-007 — implementation plan

## Approval — Gate D1

| Field | Value |
| ----- | ----- |
| Status | approved |
| Approved by | Vishal Harne <vharne@degreed.com> |
| Approved on | 2026-08-25 |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f |

## Steps

1. Domain entities + migration (`EmailDeliveryLogs`, `BookingReminders`)
2. `IEmailSender` (file dev / MailKit SMTP), `IBookingNotificationService`
3. Hook BookingService create/cancel; `ReminderEmailHostedService`
4. Traceability + aidlc-check

## Open questions

| Question | Resolution |
| -------- | ---------- |
| Tests | Skip (US-001–006 pattern) |
| SMTP in dev | File sender to `App_Data/sent-emails` when `Smtp:Host` empty |
