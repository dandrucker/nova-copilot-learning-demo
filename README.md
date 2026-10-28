# NOVA Copilot Learning Demo

> You're on the `demo-solution` branch: validation is done and every Copilot file is applied. Switch to `main` for the starting point.

A small ASP.NET Core (.NET 8) API for the NOVA Advanced talk on context engineering
for GitHub Copilot agent mode. It records gas meter readings, and it has one deliberate
gap: **no input validation**. The live demo uses Copilot to close that gap from an
Azure DevOps work item, adding one layer of context at a time.

## Also in this repo: Teaching Copilot to Learn

The `phase-1-baseline` branch holds a work order tracker API for the "Teaching Copilot to Learn" demo, and the `demo-runbook` branch holds its presenter runbook and kit. See `DEMO_RUNBOOK.md` on that branch.

## What's in here

| Path | What it is |
| --- | --- |
| `src/MeterReadings.Api` | The API (minimal APIs, in-memory storage) |
| `tests/MeterReadings.Api.Tests` | xUnit endpoint tests using `WebApplicationFactory` |
| `WORK-ITEM.md` | The work item to create in your sandbox ADO project |
| `demo-steps/` | The Copilot files for each demo step, kept out of the repo root until you apply them |
| `scripts/apply-step.ps1` / `.sh` | Copies one step's files into place, or resets back to the baseline |
| `src/MeterReadings.Api/MeterReadings.Api.http` | Sample requests for the REST Client extension |

Branches:

- `main`: the baseline the demo starts from (no validation, no Copilot files).
- `demo-solution`: the finished result (FluentValidation, tests for each rule, all Copilot files applied). Use it as a backup if the live run goes sideways.

## Before the talk

1. Install the .NET 8 SDK, Node.js 20+, and VS Code with GitHub Copilot Chat and C# Dev Kit.
2. Check the baseline builds and passes:

   ```powershell
   dotnet test
   ```

3. Run it and try the requests in `MeterReadings.Api.http`:

   ```powershell
   dotnet run --project src/MeterReadings.Api
   ```

   The API listens on http://localhost:5080. The second request in the `.http` file is a bad reading that gets accepted, which is the bug.
4. Create the work item from `WORK-ITEM.md` in a sandbox ADO project and note its ID.
5. Confirm MCP servers are allowed by your Copilot policy, then run step 3 once ahead of time so the ADO sign-in doesn't happen on stage.
6. If NuGet access is restricted, run `dotnet add src/MeterReadings.Api package FluentValidation` once beforehand so the package is cached.
7. Reset to the baseline: `./scripts/apply-step.ps1 -Step reset`

## Running the demo

| Step | Apply | Then, in Copilot Chat |
| --- | --- | --- |
| 1. Baseline | nothing | Agent mode: "Add validation to the meter reading endpoint." Note that it guesses the rules. Undo its changes. |
| 2. Repo instructions | `./scripts/apply-step.ps1 -Step 2` | Same ask again. It now uses FluentValidation, ProblemDetails, `TimeProvider` and adds tests. Undo again. |
| 3. Azure DevOps MCP | `./scripts/apply-step.ps1 -Step 3` | Start the `ado` server from `.vscode/mcp.json`, enter your org name, then: "Get work item <ID> and summarize its acceptance criteria. Do not use previously fetched data." |
| 4. Planner agent | `./scripts/apply-step.ps1 -Step 4` | Pick **Planner** in the agents dropdown. Ask it to plan work item <ID>. Show that it can't edit files. |
| 5. Prompt file | `./scripts/apply-step.ps1 -Step 5` | Type `/plan-work-item`, enter the ID, review the plan, click **Implement plan**, then run `dotnet test`. |
| Optional skill | `./scripts/apply-step.ps1 -Step skill` | "Write a PR description for these changes." |

On macOS or Linux use `./scripts/apply-step.sh 2` and so on.

The line to land: the prompt barely changed between step 1 and step 5. Only the context did.

## Putting it in a repo

The download already contains a git repository with both branches. To push it to a new, empty repo on GitHub or Azure Repos:

```powershell
git remote add origin <your-empty-repo-url>
git push -u origin main demo-solution
```

## Note

This code was written without a .NET SDK available, so it has not been compiled or run yet.
Run `dotnet test` on both branches before the talk.
