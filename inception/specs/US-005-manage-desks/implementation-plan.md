# US-005 — implementation plan

|           |                                                              |
| --------- | ------------------------------------------------------------ |
| **Story** | `inception/stories/user-stories/US-005-manage-desks.md`       |
| **Spec**  | `spec.md`                                                    |
| **Tier**  | Complex                                                      |

## Approval — Gate D1

| Field                | Value |
| -------------------- | ----- |
| Status               | approved |
| Approved by          | Vishal Harne <vharne@degreed.com> |
| Approved on          | 2026-08-25 |
| Plan commit approved | fd62f6d108547099d02c5f79fbae5b1c9831e05f |

## Steps

1. Extend `IDeskRepository`; add `IDeskService` + `DeskService`
2. Admin MVC — `DesksController` + SCR-005 view (add/edit/deactivate/activate modals)
3. API — `GET/POST/PATCH /api/admin/desks`
4. Admin nav — add Desks link; fill traceability

## Open questions

| Question | Owner | Blocks |
| -------- | ----- | ------ |
| ~~Automated tests?~~ | Vishal | **Resolved:** skip (same as US-001–004) |
| ~~Working tree?~~ | Vishal | **Resolved:** implement on current tree |
