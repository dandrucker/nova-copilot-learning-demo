---
project: nova-copilot-learning-demo
status: active
phase: talks and demos in use; Beginners Stage 2 in progress
priority: P1
last_activity: 2026-11-14
updated: 2026-11-17
tags: [nova, copilot, talk, demo]
---
# Status: NOVA Copilot Learning Demo

Demo repo for the NOVA talks: context engineering (MeterReadings API), "Teaching Copilot to Learn" (work-order API on `phase-1-baseline` / `demo-runbook` / `demo-backup`), and the Beginners Stage 2 sessions (`beginners-stage2/`).

## Where it stands
- `main` was updated 2026-11-12: Beginners Stage 2 speaker scripts (Sessions 4–6) and the Inspection Log starter
- **Unmerged:** `claude/project-thread-cz72pj` (2026-11-14) has one commit: "reset seed data after add; flag stale validation claim for Session 6"

## Open
- [ ] Review and merge (or drop) `claude/project-thread-cz72pj`
- [ ] Fix the stale validation claim in Session 6 of `beginners-stage2/SPEAKER-SCRIPTS.md` (the branch above flags it)
- [ ] README on `main` says "You're on the `demo-solution` branch". That line came in with the merge of PR #1 and is wrong on `main`.
- [ ] Rehearse each session against the starter; note any Copilot UI drift since November
- [ ] Decide on the 6 branches. Document which ones are permanent (`demo-runbook`, `demo-backup`, `phase-1-baseline`, `demo-solution`) and delete the rest.

## Related
- [[connectorrepo]]: an earlier home of the "Teaching Copilot to Learn" demo, which now appears to live here
- [[copilot-field-guide]]: companion learning material
