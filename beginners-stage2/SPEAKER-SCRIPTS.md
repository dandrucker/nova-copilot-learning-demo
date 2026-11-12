# NOVA Beginners Stage 2: Speaker Scripts

Timed talk tracks for sessions 4, 5 and 6 of the NOVA roadmap *Using GitHub Copilot Agents Safely and Effectively*. Each script is 30 minutes: a 5-minute open, a 15-minute live demo, 8 minutes hands-on and a 2-minute wrap. **Say** lines are written to be read aloud; **Do** lines are what to click or type.

## Before you present

Run every demo once beforehand, so you know what the agent tends to do and have a fallback.

- [ ] Clone this repo and open the `beginners-stage2/starter` folder in VS Code (open that folder itself, so the repo-root `.github` Copilot files from the Advanced demo don't apply).
- [ ] Run `dotnet run --project src/InspectionLog -- list`. You should see 3 seeded entries, one with a comma in its notes ("Valve 3, minor corrosion"). Session 4 uses it.
- [ ] Run `dotnet test`. The 2 starter tests should pass.
- [ ] Check that Copilot Chat shows **Ask**, **Plan** and **Agent** in the agent picker.
- [ ] Rehearse each demo on its own branch (`rehearsal-s4`, `rehearsal-s5`, `rehearsal-s6`) and commit the result. If a live run stalls, say "here's one I made earlier" and switch branches.
- [ ] Start each demo from a clean `git status`.
- [ ] Keep a timer visible. If you run long, cut the hands-on to 5 minutes, never the wrap.

## Session 4: Plan before you build

**Takeaway:** A plan is cheap to change and code is expensive, so review the plan first. **Safety habit:** catch wrong assumptions before any code exists.

### 0:00–0:05 Open

**Say:** "Last session we reviewed the agent's work after it was written. Today we move the review earlier. Before the agent writes a line, we ask it for a plan and we read that plan the way you'd read a work order before anyone goes out to the field."

**Say:** "Fixing a wrong assumption in a plan takes ten seconds. Fixing it after the agent has touched six files takes ten minutes, if you even notice it."

### 0:05–0:20 Demo: CSV export

**Do:** Show a clean `git status`. Switch the agent picker to **Plan**. Type:

```text
Plan how to add an "export" command to Inspection Log that writes all entries
to a CSV file, e.g. `dotnet run --project src/InspectionLog -- export --out entries.csv`.
Don't write code yet. List the files you'll change, the steps in order,
and any questions or assumptions you're making.
```

**Say:** "Now we read. I'm looking for four things: what it assumed, what it added that we didn't ask for, what it left out, and anything it should have asked us."

**Do:** Read the plan aloud. Point at whatever it actually assumed. The usual misses:

- Adding a CSV NuGet package we didn't ask for
- No handling for commas or quotes inside the notes field (our seed data has "Valve 3, minor corrosion")
- No date format chosen
- Silently overwriting an existing file

**Do:** Reply in the same chat to correct the plan:

```text
Changes to the plan:
- No new packages. Write the CSV by hand.
- Quote any field that contains a comma, quote, or newline; double any quotes inside it.
- Dates in ISO 8601 (yyyy-MM-dd).
- If the output file exists, stop with an error unless --force is passed.
- Add a short section to README.md showing the command.
```

**Say:** "Notice I didn't write code. I wrote rules. That's the job now."

**Do:** When the plan looks right, hand it to **Agent** mode (use the plan's "Start implementation" button, or switch to Agent and type `Implement the plan above.`). Approve the build command when it asks.

**Do:** Open the diff and check it against the plan, item by item. Then run:

```text
dotnet run --project src/InspectionLog -- export --out entries.csv
dotnet run --project src/InspectionLog -- export --out entries.csv
```

**Say:** "The second run should refuse to overwrite. And when I open the CSV, the valve note stays in one column. Both of those exist because we fixed the plan, not the code."

### 0:20–0:28 Hands-on

**Say:** "Your turn. Plan, don't build: an `import` command that reads a CSV back in. Your goal is to find at least one wrong or missing assumption in the agent's plan and correct it."

**Prompt to share:**

```text
Plan how to add an "import" command to Inspection Log that reads entries from a CSV
file. Don't write code yet. List files, steps, and your assumptions.
```

**Hints if people get stuck:** What happens with duplicate IDs? A bad date? An empty file? Does it append or replace?

**Do:** At 0:26, ask two people to share the assumption they caught.

### 0:28–0:30 Wrap

**Say:** "Plan first, read it like a work order, correct it in plain English, then build. Next time we give the agent something it can't argue with: tests."

**Do:** Show the safety checklist slide.

## Session 5: Tests as guardrails

**Takeaway:** Tests are a spec the agent can't argue with. Write them first, review them like requirements, then let the agent code until they pass. **Safety habit:** watch for the agent changing a test just to make it pass.

### 0:00–0:05 Open

**Say:** "How do you know the agent's code works? Usually we run it once, it looks fine, and we move on. That's a guess, not a check."

**Say:** "Today we flip the order. We have the agent write the tests first, we review those tests like a requirements document, and only then does it write code. If the tests are right and they pass, we know something real."

### 0:05–0:20 Demo: inspection due-date calculator

**Do:** Clean `git status`, **Agent** mode. The `starter` folder already has an xUnit test project (`tests/InspectionLog.Tests`). Type:

```text
Write xUnit tests ONLY (no implementation yet) for a static class
DueDateCalculator in InspectionLog with two methods:
- DateOnly NextDue(DateOnly lastInspection, int intervalMonths)
- bool IsOverdue(DateOnly lastInspection, int intervalMonths, DateOnly today)
Rules:
- NextDue = lastInspection plus intervalMonths calendar months.
- If that day doesn't exist in the target month, use the month's last day.
- IsOverdue is true only when today is AFTER NextDue (due today is not overdue).
- intervalMonths must be 1 or more, otherwise throw ArgumentOutOfRangeException.
Include: Jan 31 2027 + 1 month, Feb 29 2028 + 12 months, due today,
one day late, and interval 0.
```

**Say:** "Notice `today` is a parameter, not `DateTime.Now`. Code that reads the clock is hard to test. That's a design choice the tests forced on us, and it's a good one."

**Do:** Before running anything, read the expected values aloud and check them with the room:

- Jan 31 2027 + 1 month = **Feb 28 2027** (2027 isn't a leap year)
- Feb 29 2028 + 12 months = **Feb 28 2029**
- Due today = not overdue; one day after = overdue

**Say:** "If a test expects the wrong answer, the agent will happily write code that matches the wrong answer. Reviewing tests is reviewing requirements."

**Do:** Run `dotnet test`. It fails to build or goes red. **Say:** "Red is good. It means the tests are actually testing something."

**Do:** Commit the tests. Then type:

```text
Implement DueDateCalculator so all tests pass. Do not modify any test files.
```

**Do:** Approve `dotnet test`, watch it go green, and show that the diff touches only `DueDateCalculator.cs`.

**The trap (2 minutes):** Change the rule without updating the tests:

```text
Change IsOverdue so an inspection due today counts as overdue. Make the tests pass.
```

**Say:** "Watch the diff. The honest fix is to update the 'due today' test, because the requirement changed. But sometimes an agent edits tests in ways we didn't ask for, or weakens an assertion so it passes. Rule of thumb: if a test file shows up in the diff, stop and ask why."

**Do:** Undo this change.

### 0:20–0:28 Hands-on

**Say:** "Your turn: add `DaysUntilDue`, tests first. It returns a negative number when overdue."

**Prompt to share:**

```text
Write xUnit tests only for DueDateCalculator.DaysUntilDue(DateOnly lastInspection,
int intervalMonths, DateOnly today). It returns NextDue minus today in days
(negative when overdue). Include due today, 10 days early, and 3 days late.
```

**Then:** `Implement DaysUntilDue so all tests pass. Do not modify any test files.`

**Do:** At 0:26, ask whether anyone caught a wrong expected value in the agent's tests.

### 0:28–0:30 Wrap

**Say:** "Tests first, review them like requirements, and treat a test file in the diff as a red flag. Next time we use the agent to understand code none of us wrote."

**Do:** Show the safety checklist slide.

## Session 6: Understanding code you didn't write

**Takeaway:** The agent is a fast guide to unfamiliar code, and a `CONTEXT.md` saves what it learned for the next person or agent. **Safety habit:** agents sound sure and can still be wrong, so verify claims against the code.

### 0:00–0:05 Open

**Say:** "Most of our day isn't writing new code. It's reading code someone else wrote: a vendor app, an old Access database, a teammate's project from three years ago."

**Say:** "An agent can read a whole repo in seconds and explain it. That's powerful. But it explains with total confidence whether it's right or not. So today's skill is two halves: ask good questions, then check the answers."

### 0:05–0:20 Demo: the MeterReadings API

**Do:** Open the **repo root** in VS Code (not `beginners-stage2`). The target is `src/MeterReadings.Api`, a small API that records gas meter readings. Nobody in the room wrote it. Use **Ask** mode, since we're reading, not changing.

**1. Overview.** Type:

```text
Give me a 10-line overview of src/MeterReadings.Api: what it does, where it starts,
the main folders, how data is stored, and how to run it and its tests.
```

**2. Trace one path.** Type:

```text
Walk me through what happens when a client POSTs a new meter reading,
file by file, in order. Name the file and method at each step.
```

**Say:** "Now the important part. We don't trust it yet. Pick three claims and check them."

**3. Verify.** **Do:** Pick three claims from the answers, for example "data is stored in memory", "input is validated before saving" and "the tests use WebApplicationFactory". Open each file and confirm or refute it with the room. Mark each one right, partly right or wrong.

**Say:** "One question I always ask:"

```text
Which of those statements did you confirm by reading the code,
and which did you infer? List the files you actually opened.
```

**4. Find the risk.** Type:

```text
If I had to change how meter readings are validated, which files would I touch,
what could break, and what isn't covered by tests?
```

**5. Save it.** Switch to **Agent** mode and type:

```text
Create CONTEXT.md in src/MeterReadings.Api with: purpose, how to run and test,
the request flow for POST, key files, gotchas, and open questions.
Mark anything you inferred rather than read as (unverified).
```

**Say:** "Next time anyone, or any agent, opens this project, they start here instead of from zero. That's the idea we go deeper on in NOVA Advanced."

**Do:** Don't commit the demo's `CONTEXT.md` to `main`. Undo it, or keep it on your rehearsal branch.

### 0:20–0:28 Hands-on

**Say:** "In pairs: ask the agent to explain the tests folder, `tests/MeterReadings.Api.Tests`. Then verify three of its claims and keep score."

**Prompt to share:**

```text
Explain tests/MeterReadings.Api.Tests: what each test file covers, how the tests
start the API, and what important behavior is NOT tested.
```

**Do:** At 0:26, take a quick tally of how many claims were right, partly right and wrong. The "partly right" ones are the best discussion.

### 0:28–0:30 Wrap

**Say:** "Ask for the overview, trace one path, verify three claims, then save it to a context file. That finishes Stage 2. Next stage is all about safety: terminal commands, secrets and untrusted input."

**Do:** Show the safety checklist slide.

## Safety checklist (the last slide every session)

1. Commit before the agent starts.
2. Keep the task small enough to review in five minutes.
3. Read every diff before you keep it.
4. Run the code and the tests yourself.
5. Approve terminal commands one by one, and never auto-approve destructive ones.
6. No secrets, credentials or production data in the workspace or the prompt.
7. Verify every new package the agent adds.
8. A human reviews and merges. The agent never merges its own work.
