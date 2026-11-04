# Copilot instructions

- **DTOs are C# record types.** Records provide concise, value-oriented data contracts.
- **Async all the way down; pass a `CancellationToken` to every request and data-access method.** This keeps I/O non-blocking and lets callers stop unnecessary work.
- **Use structured logging with `ILogger` message templates; never interpolate log messages.** Structured properties remain searchable and avoid eager string formatting.
- **Return validation errors with `Results.ValidationProblem`.** Clients receive a consistent RFC 7807 Problem Details response.
- **Group endpoints with `MapGroup` in separate static extension classes, not inline in `Program.cs`.** This keeps startup composition concise and endpoint concerns organized.
