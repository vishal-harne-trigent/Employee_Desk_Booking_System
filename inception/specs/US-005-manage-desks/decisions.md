# US-005 — decisions

| ID   | Decision | Rationale | Alternatives rejected |
| ---- | -------- | --------- | --------------------- |
| D-01 | `IDeskService` in `Application/Desks/` | Separates desk admin from booking flows | Extend `IBookingService` — wrong boundary |
| D-02 | Block deactivate only (BR-001.9 default) | Story edge case + SCR-005 ST-08 | Cancel-in-same-flow |
| D-03 | Single PATCH for number and status | Matches architecture `PATCH /api/admin/desks` | Separate activate/deactivate routes |
