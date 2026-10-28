# Work Order Tracker (NOVA demo baseline)

A small ASP.NET Core (.NET 8) minimal API that tracks pipeline maintenance work orders.
It's the Phase 1 baseline for the NOVA Advanced demo "Teaching Copilot to Learn".

It is written in Copilot's default style on purpose, so the demo has something to fix:

- DTOs are classes, not records.
- The repository is synchronous, with no CancellationToken.
- Log messages use string interpolation.
- Validation errors are plain-text 400 responses.
- Every endpoint is inline in Program.cs.

There is no test project, by design.

```powershell
dotnet build
dotnet run --project src/WorkOrderTracker.Api
```

The API listens on http://localhost:5090. Sample requests are in
`src/WorkOrderTracker.Api/WorkOrderTracker.Api.http`.

## Demo kit (this branch)

- `DEMO_RUNBOOK.md`: the presenter script, for VS Code Copilot Chat and Copilot CLI.
- `demo/presenter.agent.md`: the NOVA Presenter custom agent. Install it with `demo/install-presenter.ps1`.
- `demo/run-demo.ps1` and `demo/run-demo.sh`: the no-agent fallback using Copilot CLI.
- `demo/reset.*`, `demo/check-rules.*` and `demo/prompts/`: reset, quick rule check and the exact prompts.

Copy the `demo/` folder outside the repo before you run the demo (the zip already has it as
`nova-demo-kit`), because every run resets the working tree to `phase-1-baseline`.
