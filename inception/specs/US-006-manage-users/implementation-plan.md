# US-006 — implementation plan

| **Story** | US-006 | **Tier** | Complex |

## Approval — Gate D1

| Field | Value |
| ----- | ----- |
| Status | approved |
| Approved by | Vishal Harne <vharne@degreed.com> |
| Approved on | 2026-08-25 |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f |

## Steps

1. Extend `IUserRepository`; add `IUserAdminService`
2. Admin MVC — `UsersController` + SCR-006 view
3. API — `GET/POST/PATCH /api/admin/users`, `POST …/reset-password`
4. Nav + traceability

## Open questions

| Question | Resolution |
| -------- | ---------- |
| Tests | Skip (US-001–005 pattern) |
| V-12 password rules | Min 8 chars until PO decides |
