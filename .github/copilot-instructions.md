# Repository instructions for GitHub Copilot

- C# on .NET 8, ASP.NET Core minimal APIs. Use dependency injection; no static helper classes.
- Validate request input with FluentValidation and return a 400 ValidationProblem (ProblemDetails) listing each field error.
- Use the injected `TimeProvider` for the current time, never `DateTime.Now` or `DateTimeOffset.UtcNow`, so tests can control time.
- Every new rule or public method gets an xUnit test in `tests/MeterReadings.Api.Tests`.
- Never log connection strings, tokens or customer data.
- Keep changes small and explain them in a short summary when you finish.
