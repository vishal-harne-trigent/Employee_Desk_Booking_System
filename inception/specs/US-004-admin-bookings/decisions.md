# US-004 — decisions

| ID   | Decision | Rationale | Alternatives rejected |
| ---- | -------- | --------- | --------------------- |
| D-01 | Separate `CancelBookingAsAdminAsync(adminUserId, bookingId)` | Keeps employee ownership check in existing `CancelBookingAsync`; admin uses `GetByIdAsync` without user filter | Refactor shared method with role flag — harder to test and review |
| D-02 | Server-side filter on Apply (query string / form POST) | Matches SCR-004 explicit Apply; clear fetch boundary | Client-side instant filter — wrong for paginated/server data |
| D-03 | Default list = all bookings, no date/status filter | SCR-004 ST-01 unfiltered default | Default to today only — hides historical rows admins may need |
| D-04 | Status filter includes "All" plus Confirmed/Cancelled/Completed | AC-03 names three statuses; All required for clearing filter | Separate clear button only — less discoverable |
