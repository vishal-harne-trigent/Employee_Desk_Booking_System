# US-007 — impact analysis

**Surfaces:** Persistence (new tables), Dependency (MailKit), Operational (hosted service), Integration (SMTP).

**Regression:** Booking create/cancel must succeed even if email fails — notifications log failures only.
