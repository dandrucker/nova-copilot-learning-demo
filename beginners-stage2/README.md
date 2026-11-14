# NOVA Beginners, Stage 2: Good habits with Copilot agents

Three 30-minute sessions on using GitHub Copilot agents well:

| Session | Topic | Sample project |
| --- | --- | --- |
| 4 | Plan before you build | Plan, correct, then build a CSV export for Inspection Log |
| 5 | Tests as guardrails | Tests-first inspection due-date calculator |
| 6 | Understanding code you didn't write | Explain, verify and document the MeterReadings API (repo root) |

The talk tracks, prompts and timings are in [SPEAKER-SCRIPTS.md](SPEAKER-SCRIPTS.md).

## The starter app: Inspection Log

A small .NET 8 console app that records field inspection notes in `entries.json`.
Sessions 4 and 5 build on it.

Open the `starter` folder itself in VS Code, not the repo root, so the
Advanced demo's `.github` Copilot files don't apply.

```text
cd beginners-stage2/starter
dotnet run --project src/InspectionLog -- list
dotnet run --project src/InspectionLog -- add "Meter Station 9" "D. Smith" Gauge reading normal
dotnet test
```

`add` writes to `entries.json`, so put the 3 seeded entries back before you present
(the Session 4 demo needs them and a clean `git status`):

```text
git checkout -- entries.json
```

Requires the .NET 8 SDK and VS Code with GitHub Copilot Chat.
