# US-002 — implementation plan

|           |                                                  |
| --------- | ------------------------------------------------ |
| **Story** | `inception/stories/user-stories/US-002-book-desk.md` |
| **Spec**  | `spec.md`                                        |
| **Tier**  | Complex                                          |

## Approval — Gate D1

| Field                | Value                                              |
| -------------------- | -------------------------------------------------- |
| Status               | approved                                           |
| Approved by          | Vishal Harne <vharne@degreed.com>                  |
| Approved on          | 2026-08-25                                         |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f           |

## Steps

### Step 1 — Domain and persistence

| Field    | Value                                                         |
| -------- | ------------------------------------------------------------- |
| Advances | FR-03, FR-04                                                  |
| Files    | Desk, Booking entities; EF configs; migration; desk seed      |
| Verify   | `dotnet ef database update`                                   |

### Step 2 — Application booking service

| Field    | Value                                                         |
| -------- | ------------------------------------------------------------- |
| Advances | FR-01 … FR-06                                                 |
| Files    | IBookingService, IOfficeClock, date validation, repositories  |
| Verify   | `dotnet build`                                                |

### Step 3 — Web SCR-002 UI

| Field    | Value                                                         |
| -------- | ------------------------------------------------------------- |
| Advances | FR-01 … FR-06 (UI)                                            |
| Files    | BookController, Index view, confirm modal                    |
| Verify   | Manual sign-in as employee; book a desk                        |

### Step 4 — API endpoints

| Field    | Value                                                         |
| -------- | ------------------------------------------------------------- |
| Advances | FR-07                                                         |
| Files    | BookingsController (availability + create)                      |
| Verify   | Swagger GET/POST                                              |

## Rollback

Revert PR; roll back EF migration if applied.

## Open questions

| Question | Owner | Blocks |
| -------- | ----- | ------ |
