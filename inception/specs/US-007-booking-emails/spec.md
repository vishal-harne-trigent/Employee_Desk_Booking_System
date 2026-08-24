# US-007 — Send booking email notifications

| **Story** | US-007 | **Tier** | Complex | **Status** | implemented | **Updated** | 2026-08-25 |

## Problem

Booking create/cancel completes without notifying the employee. Architecture specifies MailKit + `EmailDeliveryLogs` + `BookingReminders` + hosted reminder job — none exist yet.

## Functional requirements

| ID | Requirement | Serves |
| --- | --- | --- |
| FR-01 | Confirmation email on Confirmed transition | AC-01 |
| FR-02 | Cancellation email on Cancelled transition | AC-02 |
| FR-03 | Emails include desk number and booking date | AC-03 |
| FR-04 | Day-before reminder for Confirmed weekday bookings | AC-04 |
| FR-05 | Log email delivery failures | AC-05 |

## Out of scope

- Push notifications (US-008)
- User email opt-out
