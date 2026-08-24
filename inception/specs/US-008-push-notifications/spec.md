# US-008 — Browser push notification preferences

| **Story** | US-008 | **Tier** | Complex | **Status** | implemented | **Updated** | 2026-08-25 |

## Problem

Employees receive email on book/cancel (US-007) but cannot opt into browser push. `NotificationPreferences` table and WebPush integration are not implemented.

## Functional requirements

| ID | Requirement | Serves |
| --- | --- | --- |
| FR-01 | Default opt-out — no push unless enabled | AC-01 |
| FR-02 | Settings UI to opt in with browser subscription | AC-02 |
| FR-03 | Push on Confirmed/Cancelled when opted in | AC-03 |
| FR-04 | Opt out stops push | AC-04 |
| FR-05 | Reminders email-only (no push hook in reminder path) | AC-05 |

## Out of scope

- Push for day-before reminders (BR-001.16)
- Email opt-out
