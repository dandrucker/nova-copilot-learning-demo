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

The demo runbook and presenter kit are on the `demo-runbook` branch.
