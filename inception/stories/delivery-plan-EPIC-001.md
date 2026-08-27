# Delivery plan — EPIC-001 Employee Desk Booking

> **Baseline revised** by delivery manager review on **2026-08-27** (supersedes prior drafts). Estimates are **Composer 2.5 Fast agent active runtime for implementation only** — spec drafting and feature code. **Unit test creation, test execution, and `/qa` agent sessions are excluded.** Human review, approval gates (D1/D2), and CI queue wait are also excluded.

|                    |                                                                                          |
| ------------------ | ---------------------------------------------------------------------------------------- |
| **Epic**           | EPIC-001                                                                                 |
| **Traces to**      | BRD-001, SRS-001, US-001 … US-009                                                        |
| **Locked by**      | Delivery manager (human approval in conversation, 2026-08-27)                            |
| **Team**           | 1 Developer (drives `/dev` agent) · 1 QA (out of scope for this estimate)                |
| **AI agent**       | Cursor · **Composer 2.5 Fast** · AIDLC personas (`/dev`, `/architect`, `/devops`)        |
| **Estimate basis** | Agent wall-clock while Composer 2.5 Fast drafts specs and implementation code            |

## What we measure

| Included | Excluded |
| -------- | -------- |
| `/dev` spec package (plan, impact, traceability seed) | Unit / integration test authoring |
| `/dev` feature implementation (API, services, UI) | Test execution (`dotnet test`, CI test runs) |
| `/architect` and `/devops` agent sessions | `/qa` test derivation and AC verification |
| Agent fix loops for lint/build (non-test) failures | Human D1/D2 review wait |
| Re-prompting agent when output needs correction | Manual exploratory testing |

**One story = one PR** (`feat/US-###-<slug>`). Rows below cover agent time to produce **implementation code only** — not a merge-ready AIDLC story PR (which still requires tests under `ai/gates/delivery.md`; that effort is tracked separately).

> **Note on existing code:** Scaffold implementation (API, Web, domain) is already in the repo. Agent time below assumes **retrofit to match story ACs**, not greenfield — roughly 30–40 % less than building from scratch.

## Agent session model (per story — implementation only)

| Agent session | Persona | Composer 2.5 Fast runtime |
| ------------- | ------- | ------------------------- |
| Spec package (plan, impact, traceability seed) | `/dev` | 20–35 min |
| Feature implementation (no tests) | `/dev` | 30–90 min |
| Lint/build fix loop (1 round, non-test) | `/dev` | 10–20 min |
| Advisory design note (Complex tier only) | `/architect` | 15–25 min |

## Story point → agent time mapping (implementation only)

| Story Points | Spec | Implementation | Fix loop | **Total agent** |
| ------------ | ---- | -------------- | -------- | --------------- |
| **3 SP**     | 20 min | 25–40 min | 10 min | **~1–1.25 h** |
| **5 SP**     | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** |
| **8 SP**     | 30 min | 60–90 min | 20 min | **~1.75–2.5 h** |

## Per-story agent effort (implementation only)

| Story | Title | SP | Spec | Code | Fix | **Total agent** | Jira |
| ----- | ----- | -- | ---- | ---- | --- | --------------- | ---- |
| US-001 | Sign in / sign out | 5 | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** | EDBS-38 |
| US-002 | Book a desk | 8 | 30 min | 60–90 min | 20 min | **~1.75–2.5 h** | EDBS-39 |
| US-003 | My bookings | 5 | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** | EDBS-40 |
| US-004 | Admin all bookings | 5 | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** | EDBS-41 |
| US-005 | Admin manage desks | 5 | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** | EDBS-42 |
| US-006 | Admin manage users | 8 | 30 min | 60–90 min | 20 min | **~1.75–2.5 h** | EDBS-43 |
| US-007 | Booking email notifications | 8 | 30 min | 70–100 min | 20 min | **~2–2.5 h** | EDBS-44 |
| US-008 | Browser push preferences | 5 | 25 min | 40–65 min | 15 min | **~1.25–1.75 h** | EDBS-45 |
| US-009 | Auto-complete past bookings | 3 | 20 min | 25–40 min | 10 min | **~1–1.25 h** | EDBS-46 |
| **Stories subtotal** | | **52 SP** | | | | **~12–18 h** | |

## Sprint 0 — Foundation (fixed)

| Work | Persona | Agent runtime |
| ---- | ------- | ------------- |
| Validate / refine db + app architecture | `/architect` | 25–40 min |
| CI pipeline + `aidlc-check` required status | `/devops` | 45–90 min |
| **Sprint 0 subtotal** | | **~1.25–2.25 h** |

Test project scaffold and AC test naming — **excluded** from this estimate (QA `/qa` scope).

**Exit criteria** (human/PO — not agent time):

- Architecture confirmed current (`inception/architecture/db-design.md`, `app-architecture.md`)
- First Admin bootstrap decided (BRD open Q #1)
- Password policy decided (BRD open Q #5)
- CI runs `aidlc-check` and test runner shell

## Sprint schedule (implementation agent time)

| Sprint | Stories | SP | Agent time (`/dev` only) | Cumulative agent |
| ------ | ------- | -- | ------------------------ | ---------------- |
| **0 — Foundation** | Architecture, CI | — | **~1.25–2.25 h** | 1.25–2.25 h |
| **1 — Auth + booking core** | US-001, US-002 | 13 | **~3–4.25 h** | 4.25–6.5 h |
| **2 — Employee wrap-up** | US-003, US-009 | 8 | **~2.25–3 h** | 6.5–9.5 h |
| **3 — Admin** | US-004, US-005, US-006 | 18 | **~4.25–6 h** | 10.75–15.5 h |
| **4 — Notifications (MVP)** | US-007, US-008 | 13 | **~3.25–4.25 h** | 14–20 h |

**Grand total agent runtime (implementation only):** **~14–20 hours** (~17 h midpoint)

### Calendar projection (agent time only)

| Throughput | Agent-active calendar | Notes |
| ---------- | --------------------- | ----- |
| **Aggressive** — ~8 h agent sessions/day | **~2–2.5 working days** | Developer driving `/dev` back-to-back |
| **Steady** — ~4 h agent sessions/day | **~3.5–5 working days** | One story chain at a time |
| **Conservative** — ~2 h agent sessions/day | **~7–10 working days** | Part-time agent driving |

Real calendar will be **longer** because of human gates, test work (excluded here), environment setup, and blocked decisions (SMTP, Admin bootstrap).

### Sequential execution pattern

```
Sprint 1 (implementation agent ~3–4.25 h):

  Dev agent:  [US-001 spec+code 1.25-1.75h] → [US-002 spec+code 1.75-2.5h]

Sprint 3 (admin stories — sequential, shared area):

  US-004 (1.25-1.75h) → US-005 (1.25-1.75h) → US-006 (1.75-2.5h)  = 4.25-6h
```

### Sprint dependencies

| Sprint | Key rule |
| ------ | -------- |
| **1** | US-001 merges before US-002 starts |
| **2** | US-003 needs US-001+002; US-009 needs US-002 |
| **3** | US-004/005/006 need US-001; resolve BRD Q #6 before US-005 |
| **4** | US-007 needs booking events from US-002–004; US-008 can parallel US-007 |

**MVP code complete = Sprint 4 implementation merged.** Full AIDLC delivery still requires test coverage per gate — tracked outside this estimate.

## Agent session playbook (implementation only)

| Order | Prompt | Expected runtime |
| ----- | ------ | ---------------- |
| 1 | `/dev` → spec package for US-### | 20–35 min |
| 2 | `/dev` → implement US-### (no tests) | 30–90 min |
| 3 | `/dev` → fix lint/build findings | 10–20 min |

Human D1/D2 gates and all test work sit **outside** this estimate.

## Risks (agent-specific)

| Risk | Agent-time impact | Mitigation |
| ---- | ----------------- | ---------- |
| Complex-tier story misclassified as Simple | +45–90 min rework | Escalate to thinking model for US-002, US-006, US-007 |
| Build red on first push (non-test) | +10–20 min fix loop | Build locally before ending agent session |
| Existing code drift vs story ACs | +20–40 min reconciliation | Story AC is source of truth |
| SMTP undecided (BRD Q #7) | Blocks US-007 merge, not draft | Agent drafts with dev mail catcher |
| Multi-story PR (scope creep) | +30–60 min untangle | One story per `/dev` prompt |

## Shareable summary

> **Employee Desk Booking — EPIC-001 (implementation agent-time, 2026-08-27)**  
> **Agent:** Composer 2.5 Fast via Cursor + AIDLC (`/dev`, `/architect`, `/devops`)  
> **Scope:** 9 stories · 52 SP · 44 ACs · 7 screens  
> **Agent runtime (implementation only):** **~14–20 hours** (~17 h midpoint)  
> **Unit tests:** excluded from this estimate  
> **Agent-active calendar:** ~2–2.5 days (aggressive) · ~3.5–5 days (steady) · ~7–10 days (conservative)  
> **Next:** Sprint 0 — `/architect` + `/devops` (~1.25–2.25 h agent time)

## Related

- Epic: `inception/stories/epics/EPIC-001-desk-booking.md`
- Requirements: `inception/product/requirements/BRD-001-desk-booking.md`, `SRS-001-desk-booking.md`
- Traceability: `knowledge/traceability/manifest.json`
- Delivery gate: `ai/gates/delivery.md`
