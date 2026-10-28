# NOVA Copilot Presenter Runbook: Teaching Copilot to Learn

A live demo that shows GitHub Copilot learning a team's conventions from
`.github/copilot-instructions.md`, then keeping that file up to date itself. Everything runs on
GitHub Copilot: VS Code Copilot Chat for the presenter and the coding, and Copilot CLI only if you
have it. No other agent is needed.

## How it fits together

| Piece | Where it lives | Why |
| --- | --- | --- |
| Work order API (the app Copilot changes) | `phase-1-baseline` branch | The starting point. Each run resets to it on a throwaway `demo-live` branch. |
| This runbook and the `demo/` kit | `demo-runbook` branch, and the `nova-demo-kit` folder in the zip | Kept outside the working tree during the demo, so the Phase 3 session can't read the house rules from it. |
| NOVA Presenter custom agent | `~/.copilot/agents/nova-presenter.agent.md` | User level, so it survives the reset and works in both VS Code and Copilot CLI. |

Lay the folders out side by side, for example `C:\nova\nova-copilot-learning-demo` (the repo) and
`C:\nova\nova-demo-kit` (the kit).

## One-time setup

1. Install the .NET 8 SDK and VS Code with GitHub Copilot Chat and C# Dev Kit.
2. Check out the baseline and build it:

   ```powershell
   cd C:\nova\nova-copilot-learning-demo
   git checkout phase-1-baseline
   dotnet build
   ```

3. Install the presenter agent, then reload VS Code:

   ```powershell
   C:\nova\nova-demo-kit\install-presenter.ps1
   ```

4. Open the repo folder in VS Code. Open Chat, open the agents dropdown, and check that
   **NOVA Presenter** is listed. In **Configure Tools**, make sure terminal commands are enabled for it.
5. Optional, for CLI mode and the fallback script: install Copilot CLI and sign in.

   ```powershell
   winget install GitHub.Copilot
   # or: npm install -g @github/copilot   (needs Node.js 22+)
   copilot
   ```

   In the CLI, run `/login` and trust the repo folder when asked, then exit. If you get Copilot
   through ONEOK, the Copilot CLI policy has to be enabled for the org.

## Three ways to run it

- **A. VS Code with the NOVA Presenter (recommended).** Pick **NOVA Presenter** in Copilot Chat and
  type `start`. It narrates, pauses after every checkpoint (type `next`), and checks the results.
  If Copilot CLI is installed, it runs each phase as a fresh CLI session in the terminal. If not, it
  tells you to open a new chat for each phase and paste the prompt, then type `done`.
- **B. VS Code by hand.** Follow the phases below yourself. Use **Chat: New Chat Editor** and the
  built-in **Agent** for each phase, so nothing carries over between sessions.
- **C. Fallback script, no agent.** If the presenter misbehaves live:

  ```powershell
  C:\nova\nova-demo-kit\run-demo.ps1 -RepoPath C:\nova\nova-copilot-learning-demo
  ```

  It needs Copilot CLI, pauses for Enter between steps, and runs each phase in a fresh
  non-interactive session (`copilot -p ... --allow-tool ... --no-ask-user`). On macOS or Linux use
  `run-demo.sh`.

## Step 0: Reset

On screen: "We start from Copilot's default style: classes for DTOs, sync code, interpolated logs,
plain-text errors, everything in Program.cs."

```powershell
C:\nova\nova-demo-kit\reset.ps1 -RepoPath C:\nova\nova-copilot-learning-demo
```

This throws away uncommitted changes, recreates `demo-live` from `phase-1-baseline`, and builds.
Show two lines in `Program.cs`: an inline `app.MapGet` and an interpolated `LogInformation`.

## Phase 2: Teach

On screen: "We teach Copilot our house rules once, and have it write them down for every future session."

New chat, built-in Agent, paste:

```text
Refactor this API to follow our house rules: (1) DTOs are C# record types. (2) Async all the way down, with a CancellationToken on every method. (3) Structured logging with ILogger message templates, never string interpolation. (4) Validation errors return Problem Details via Results.ValidationProblem. (5) Endpoints are grouped with MapGroup in separate static extension classes, not inline in Program.cs. Then create .github/copilot-instructions.md that lists each rule with a one-line reason, so future sessions follow them. Do not add a test project. Finish by running dotnet build and fixing any errors.
```

Check afterward:

```powershell
git add -A; git commit -q -m "Phase 2"
git --no-pager diff HEAD~1 -- .github/copilot-instructions.md
git --no-pager diff HEAD~1 --stat
dotnet build
```

You want: a new instructions file with five rules and a reason for each, a refactor across the
project, and a passing build.

## Phase 3: Prove it

On screen: "Brand-new session. No rules in the prompt. Does Copilot follow them anyway?"

Open a **new** chat (this is the whole point), built-in Agent, paste exactly:

```text
Add an endpoint GET /workorders/overdue that returns work orders with High or Critical priority that have been open for more than 7 days. Also add an Assignee field to work orders. Finish by running dotnet build and fixing any errors.
```

Check afterward: commit as "Phase 3", run the same diff commands and `dotnet build`, then go rule
by rule. The presenter agent does this in chat. By hand, run the quick check:

```powershell
C:\nova\nova-demo-kit\check-rules.ps1 -RepoPath C:\nova\nova-copilot-learning-demo
```

| Rule | What a pass looks like |
| --- | --- |
| DTOs are records | `Assignee` added to a `record`, no new DTO classes |
| Async with CancellationToken | The overdue query is `async`, returns `Task`, takes a `CancellationToken` |
| Structured logging | Any new log call uses a template like `"Found {Count} overdue work orders"` |
| ValidationProblem | Any new validation returns `Results.ValidationProblem` (often not needed here) |
| MapGroup in extension classes | `/overdue` added to the endpoint extension class, `Program.cs` untouched |

A miss is fine on stage. It's a good moment to talk about writing clearer rules.

## Phase 4: Self-improve

On screen: "Now we let Copilot keep its own rulebook up to date."

1. Add this line to the end of `.github/copilot-instructions.md` yourself, on screen, and save:

   ```text
   - When the user corrects a convention or pattern, update .github/copilot-instructions.md with the lesson before finishing the task.
   ```

2. Open a **new** chat, built-in Agent, and send:

   ```text
   Add a ClosedAt timestamp to work orders that is set when a work order is closed, and include it in the API responses. Finish by running dotnet build and fixing any errors.
   ```

3. When it finishes, correct it once in the same chat:

   ```text
   Correction: in this codebase we use DateTimeOffset, not DateTime, for every timestamp, including the existing OpenedAt. Please fix that.
   ```

   (The CLI runs both as one prompt: "Add a ClosedAt timestamp to work orders that is set when a work order is closed, and include it in the API responses. Correction to how this codebase handles time: we use DateTimeOffset, not DateTime, for every timestamp, including the existing OpenedAt, so fix that too. Finish by running dotnet build and fixing any errors.")

Check afterward: commit as "Phase 4", show the instructions diff, and point out the new
DateTimeOffset lesson Copilot wrote itself. Then `dotnet build`.

## Close

On screen: "Three sessions, one file. The file is what Copilot learned, and it lives in the repo,
reviewed like code."

Run Step 0 again to reset for another run.

## Copilot CLI flags used

| Flag | Why |
| --- | --- |
| `-p "<prompt>"` | Runs one prompt non-interactively, then exits, so every phase is a fresh session |
| `--allow-tool='write, shell(dotnet:*)'` | Lets Copilot edit files and run `dotnet` without asking, and nothing else |
| `--no-ask-user` | Stops it pausing for questions mid-demo |
| `--agent=<name>` | Optional: start the CLI as a custom agent, such as the NOVA Presenter, instead of using VS Code |

Copilot CLI reads `.github/copilot-instructions.md` automatically, but only when a session starts,
which is another reason each phase starts fresh. Sign-in is `/login`, or a fine-grained token with
the Copilot Requests permission in `COPILOT_GITHUB_TOKEN`.

## If something goes wrong

- **Presenter isn't in the agents list:** rerun `install-presenter.ps1` and reload VS Code.
- **Presenter can't run commands:** enable terminal tools for it in **Configure Tools**.
- **Copilot CLI says the policy is disabled:** use path B in VS Code instead.
- **The build fails after a phase:** ask Copilot in the same chat to fix the build, then continue.
- **You need a clean slate:** run `reset.ps1` again.

## Notes

- This kit was written without a .NET SDK or Copilot available, so nothing has been run end to end.
  Do one full dry run at work before presenting.
- `check-rules` is a quick pattern check, not a compiler. Read the diff for the real story.
