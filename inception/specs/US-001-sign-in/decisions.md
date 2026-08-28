# US-001 — decisions

> Technical choices made while implementing this story, with the reasoning that produced them.

| ID   | Decision | Rationale | Alternatives rejected |
| ---- | -------- | --------- | ---------------------- |
| D-01 | US-001 PR delivers **MVC cookie auth only**; JWT API login stays out of this PR | Story AC and SCR-001 are Web-only; keeps one story = one reviewable surface | Dual auth in one PR per app-architecture — split to avoid scope creep |
| D-02 | Invalid and deactivated failures use **separate messages** | AC-04 and SCR-001 ST-04 require distinct deactivated copy; AC-03/V-01 stay generic for bad credentials | Single generic message for both — contradicts ST-04 |
| D-03 | `DbInitializer` seeds one Employee + one Admin when `Users` is empty | BRD Q #1 / architecture approved 2026-08-21 | Manual SQL seed scripts |

Cite these as `US-001/D-01` outside this folder.
