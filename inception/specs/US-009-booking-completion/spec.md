# US-009 — Complete past bookings automatically

| **Story** | US-009 | **Tier** | Medium | **Status** | implemented | **Updated** | 2026-08-25 |

## Problem

Past **Confirmed** bookings stay Confirmed until manually updated. My Bookings and Admin filters cannot show accurate **Completed** history after the desk date passes (BR-001.5).

## Functional requirements

| ID | Requirement | Serves |
| --- | --- | --- |
| FR-01 | Confirmed bookings dated before today (office local) become Completed | AC-01 |
| FR-02 | Cancelled bookings are untouched | AC-02 |
| FR-03 | Today's Confirmed bookings stay Confirmed | AC-03 |

## Out of scope

- Notifications on completion
- New API endpoints (background job only)
