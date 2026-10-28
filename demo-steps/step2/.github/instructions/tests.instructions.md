---
applyTo: "tests/**/*.cs"
---
- Name tests `Method_Condition_ExpectedResult`.
- Use `WebApplicationFactory<Program>` for endpoint tests; don't mock the HTTP pipeline.
- One behaviour per test, with Arrange, Act and Assert separated by a blank line.
