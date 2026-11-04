# Start Here: "Copilot Forgets. Your Repo Doesn't."

The whole demo in one place: get it onto the presentation machine, rehearse it, and present it,
including what to say. It takes about 22 minutes on stage.

You need the .NET 10 SDK, VS Code, and the **GitHub Copilot Chat** and **C# Dev Kit** extensions,
signed in to Copilot. Everything below runs in PowerShell.

---

## Part 1. Set up the machine (once, about 15 minutes)

### 1. Get the code

You need two copies of the repo, side by side. The first is the app that Copilot changes. The
second is the demo kit (scripts and prompts). The kit has to live **outside** the app folder,
otherwise Copilot could read the house rules from it during the demo and spoil Phase 3.

```powershell
mkdir C:\nova
cd C:\nova
git clone https://github.com/dandrucker/nova-copilot-learning-demo.git
git clone -b demo-runbook --single-branch https://github.com/dandrucker/nova-copilot-learning-demo.git nova-demo-kit
```

You now have:

| Folder | What it is |
| --- | --- |
| `C:\nova\nova-copilot-learning-demo` | The app. Open this one in VS Code. |
| `C:\nova\nova-demo-kit\demo` | The kit: reset script, rule checker, prompts, fallback script |
| `C:\nova\nova-demo-kit\START_HERE.md` | This file. Keep it open on a second screen or printed. |

### 2. Make sure git can commit

The demo commits after each phase so you can show the diff. If this prints nothing, set a name:

```powershell
git -C C:\nova\nova-copilot-learning-demo config user.name
git -C C:\nova\nova-copilot-learning-demo config user.name "NOVA demo"
git -C C:\nova\nova-copilot-learning-demo config user.email "nova-demo@localhost"
```

### 3. Reset to the starting point and check it builds

```powershell
C:\nova\nova-demo-kit\demo\reset.ps1 -RepoPath C:\nova\nova-copilot-learning-demo
```

It should end with `Reset to phase-1-baseline on branch demo-live. Baseline builds.` You run this
same command before every rehearsal and before you go on stage.

### 4. Set up VS Code

1. Open the folder `C:\nova\nova-copilot-learning-demo` in VS Code and trust it.
2. Open the Chat view and switch the mode dropdown to **Agent**.
3. Settings (Ctrl+,): search for `instruction files` and make sure the setting that tells
   Copilot to use instruction files is on. It's on by default, and Phase 3 depends on it.
4. Settings: turn on auto-approve for terminal commands, or be ready to click **Continue** when
   Copilot asks to run `dotnet build`.
5. Zoom in (Ctrl+= twice) so the back row can read the code.

### 5. Rehearse once, end to end

Run Part 3 exactly as written, then reset (step 3 above). Note any phase that misses, so it's not a
surprise on stage. A dry run on 2026-11-04 passed every phase: Phase 2 took about 2 minutes,
Phase 3 about 1 minute, Phase 4 about 30 seconds.

---

## Part 2. Just before you go on (5 minutes)

1. Run the reset (Part 1, step 3).
2. In VS Code: open `src/WorkOrderTracker.Api/Program.cs`. Close every chat tab. Open a terminal
   panel at the bottom.
3. Have this file open where you can read it but the audience can't.
4. Optional: a slide with the five house rules from Phase 2.

---

## Part 3. The demo

**Do** = what you do. **Say** = what you say. Copy the prompts from the grey boxes exactly.

### Open (0:00 to 2:00)

**Say:** "Show of hands. Who has told Copilot the same thing twice? 'We use records.' 'Don't
interpolate log messages.' 'Not DateTime, DateTimeOffset.' Keep your hand up if it was more than
twice."

**Say:** "Here's why. Every chat with an agent starts from zero. It doesn't remember yesterday, or
the correction you gave it five minutes ago in another tab. The model never changes."

**Say:** "But the repo does. In the next twenty minutes we'll give Copilot a memory that lives in
the repo, gets reviewed like code, and that Copilot keeps up to date itself. Three sessions, one
file."

### The starting point (2:00 to 4:00)

**Do:** Show `Program.cs`. Point at an inline `app.MapGet(...)` and a log line with `$"..."`.

**Say:** "This is a small work order API for pipeline maintenance, written the way Copilot writes
code when nobody tells it otherwise. Classes for DTOs, synchronous code, string interpolation in
the logs, plain-text errors, and every endpoint in `Program.cs`."

**Say:** "None of that is wrong. It's just not how *we* write code. Our team has five house rules,
and right now they live in people's heads and in review comments."

**Do:** Show or read the rules: records for DTOs; async with a CancellationToken; structured
logging; Problem Details for validation errors; endpoints grouped with MapGroup, out of
`Program.cs`.

### Phase 2: Teach (4:00 to 9:00)

**Say:** "Session one. We teach the rules once, and ask Copilot to write them down for every future
session."

**Do:** In Chat, Agent mode, paste and send:

```text
Refactor this API to follow our house rules: (1) DTOs are C# record types. (2) Async all the way down, with a CancellationToken on every method. (3) Structured logging with ILogger message templates, never string interpolation. (4) Validation errors return Problem Details via Results.ValidationProblem. (5) Endpoints are grouped with MapGroup in separate static extension classes, not inline in Program.cs. Then create .github/copilot-instructions.md that lists each rule with a one-line reason, so future sessions follow them. Do not add a test project. Finish by running dotnet build and fixing any errors.
```

**Say** (while it works, about 2 minutes): "Look at the end of that prompt. I'm not only asking for
a refactor. I'm asking it to create `.github/copilot-instructions.md`, with a reason for each rule.
Copilot reads that file automatically at the start of every session: in chat, in agent mode, in the
CLI, and in code review on GitHub."

**Say:** "Why the reasons? A rule without a reason gets applied badly. 'Use records' is a rule.
'Use records because DTOs should be immutable contracts' tells the agent where the rule stops."

**Say:** "And it's markdown, in the repo, in source control. When it changes, that's a diff, and
somebody reviews it. That matters in a few minutes."

**Do:** When it's done, click **Keep** to accept the changes, then in the terminal:

```powershell
git add -A; git commit -q -m "Phase 2"
git --no-pager diff HEAD~1 --stat
```

Then open `.github/copilot-instructions.md` and show it.

**Say:** "Five rules, five reasons, the code refactored to match, and the build passes. But that's
the easy part: I told it the rules. The real question is the next session."

### Phase 3: Prove it (9:00 to 14:00)

**Say:** "Session two. Brand-new chat, nothing carried over, and I won't mention a single rule."

**Do:** Click **New Chat** (the + at the top of the Chat view). Show it's empty. Paste and send:

```text
Add an endpoint GET /workorders/overdue that returns work orders with High or Critical priority that have been open for more than 7 days. Also add an Assignee field to work orders. Finish by running dotnet build and fixing any errors.
```

**Say** (while it works, about 1 minute): "Read the prompt with me. An overdue endpoint and an
Assignee field. Nothing about records, async, logging or MapGroup. If it follows the rules, it's
because it read them from the file."

**Do:** If the response shows a **References** list, expand it and point at
`copilot-instructions.md`. When it's done, click **Keep**, then:

```powershell
git add -A; git commit -q -m "Phase 3"
C:\nova\nova-demo-kit\demo\check-rules.ps1 -RepoPath .
```

Open the endpoints extension class and show the new `/overdue` endpoint.

**Say:** "Async, with a CancellationToken. A structured log message with a named placeholder. It's
in the extension class, and `Program.cs` didn't change. The prompt said none of that."

**If a rule shows FAIL, say:** "There's a miss, and that's the useful part. The fix isn't a better
prompt; it's a clearer rule." Read the rule aloud, say how you'd reword it, and move on.

### Phase 4: Self-improve (14:00 to 20:00)

**Say:** "So far *I* wrote the rules. But most conventions don't arrive as a list. They arrive as a
correction in the middle of a task: 'No, we don't do it that way.' And those corrections usually
die with the chat."

**Do:** Open `.github/copilot-instructions.md`, go to the end, and type this line yourself, on
screen. Save.

```text
- When the user corrects a convention or pattern, update .github/copilot-instructions.md with the lesson before finishing the task.
```

**Say:** "One line. When I correct Copilot, the lesson goes in the file."

**Do:** Click **New Chat**. Paste and send:

```text
Add a ClosedAt timestamp to work orders that is set when a work order is closed, and include it in the API responses. Finish by running dotnet build and fixing any errors.
```

**Say** (while it works, about 30 seconds): "A normal task. Add a ClosedAt timestamp when a work
order closes."

**Do:** When it's done, point at the type it used for `ClosedAt`. It usually picks `DateTime`.
In the **same** chat, send:

```text
Correction: in this codebase we use DateTimeOffset, not DateTime, for every timestamp, including the existing OpenedAt. Please fix that.
```

**Say:** "DateTime. Not wrong in general, but wrong *here*. So I correct it once, the way I'd
correct a teammate."

**If it already used DateTimeOffset, say:** "It got it right on its own, so let me teach it
something it doesn't know," and send a different correction instead, for example: "Correction: in
this codebase every timestamp property name ends in Utc, for example OpenedAtUtc and ClosedAtUtc.
Please fix that."

**Do:** When it's done, click **Keep**, then:

```powershell
git add -A; git commit -q -m "Phase 4"
git --no-pager diff HEAD~1 -- .github/copilot-instructions.md
```

**Say:** "It fixed the code, including the existing OpenedAt. And look at the file: there's a new
rule. I didn't write that one. The next session, and everyone else on the team, gets it for free."

**Say:** "And because it's a diff, it goes through review like any other change. If the agent
writes down a bad lesson, you catch it in the pull request. The memory is visible, versioned and
reviewable."

**If the new rule has no reason, say:** "Notice it didn't give a reason for this one. That's a
review comment, exactly like we'd leave for a person."

### Close (20:00 to 22:00)

**Do:**

```powershell
git log --oneline -4
```

**Say:** "Three sessions, one file. Copilot didn't learn; the model is exactly the same as when we
started. What changed is the context it starts with, and that context now improves every time
someone corrects it."

**Say:** "Three things to take home. One: put your conventions in `.github/copilot-instructions.md`,
with a reason for each. Two: add the one line that tells Copilot to write corrections down. Three:
review that file like code, because it's part of your codebase now."

**Say:** "If you want to take it further, the repo has an appendix that applies the same loop to
code review: a reviewer that remembers when you tell it a finding was wrong." (It's in
`demo/appendix/README.md` on the `demo-runbook` branch.)

---

## Part 4. If something goes wrong

| Problem | What to do |
| --- | --- |
| The build fails after a phase | "Agents make mistakes too." In the same chat: "The build fails, please fix it." Then continue. |
| It asks permission to run a command | Click **Continue**. |
| It put the rules file somewhere other than `.github/` | Reset (Part 1, step 3) and start Phase 2 again. |
| Phase 3 ignores the rules | Check the **Use Instruction Files** setting (Part 1, step 4), then run Phase 3 again in a new chat. |
| Running out of time | Skip the Phase 3 code walkthrough. Phase 4 is the payoff. |
| Everything is broken | If Copilot CLI is installed, run `C:\nova\nova-demo-kit\demo\run-demo.ps1 -RepoPath C:\nova\nova-copilot-learning-demo` and narrate over it. It runs every phase itself and pauses for Enter. |

To run the demo again, just reset (Part 1, step 3).

## Part 5. Q&A prep

- **"Doesn't the file get huge?"** Every request loads it, so keep it short and specific, and prune
  it in review. Rules that only apply to some files can go in
  `.github/instructions/*.instructions.md` with an `applyTo` glob.
- **"What stops it writing down something wrong?"** Pull request review. The lesson is a diff.
- **"Can a pull request teach it bad habits?"** The rule says to learn from the *user's*
  corrections, not from code it reads. Keep it that way.
- **"Does this work outside VS Code?"** Copilot CLI, the Copilot coding agent and Copilot code
  review on GitHub read the same file. Other agents have their own (`CLAUDE.md`, `AGENTS.md`).
- **"Isn't this the same as a built-in memory feature?"** No. This file lives in your repo, so you
  can read it, review it and roll it back.

## Other files

- `DEMO_RUNBOOK.md`: other ways to run it (the NOVA Presenter agent, Copilot CLI) and the CLI flags.
- `demo/appendix/`: the code review appendix.
