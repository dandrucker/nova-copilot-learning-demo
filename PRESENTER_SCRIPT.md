# Presenter Script: Copilot Forgets. Your Repo Doesn't.

The talk track for the "Teaching Copilot to Learn" demo, in about 22 minutes. `DEMO_RUNBOOK.md`
has the setup and the commands; this file is what you say and when.

**Say** lines are spoken. **Do** lines are actions. Times are a target, not a stopwatch. The
Copilot run times come from a dry run on 2026-11-04 (Phase 2 about 2 minutes, Phase 3 about
1 minute, Phase 4 about 30 seconds), so each phase has talking points to fill the wait.

## Before you walk on

- Run the reset so you're on `demo-live` with a clean build.
- VS Code open on the repo, zoomed in, with `Program.cs` open. Terminal visible.
- Chat panel open on the built-in **Agent** (or **NOVA Presenter** if you're using it).
- Close every other chat tab. Fresh sessions are the whole point.
- Have the fallbacks ready in a terminal: the reset script and `run-demo.ps1`.

## 1. Cold open (0:00 to 2:00)

**Say:** "Show of hands. Who has told Copilot the same thing twice? 'We use records.' 'Don't
interpolate log messages.' 'Not DateTime, DateTimeOffset.' Keep your hand up if it was more than
twice."

**Say:** "Here's why. Every chat with an agent starts from zero. It doesn't remember yesterday, or
the last session, or the correction you gave it five minutes ago in another tab. The model doesn't
learn from you. It never changes."

**Say:** "But the repo does. So in the next twenty minutes we'll give Copilot a memory that lives in
the repo, gets reviewed like code, and that Copilot keeps up to date itself. Three sessions, one
file."

## 2. The baseline (2:00 to 4:00)

**Do:** Show `Program.cs`. Point at the inline `app.MapGet` and the interpolated `LogInformation`.

**Say:** "This is a small work order API for pipeline maintenance. It's written the way Copilot
writes code when nobody tells it otherwise. Classes for DTOs, synchronous code, string
interpolation in the logs, plain-text errors, and every endpoint in `Program.cs`."

**Say:** "None of that is wrong. It's just not how *we* write code. Our team has five house rules,
and right now they live in people's heads and in review comments."

**Do:** Show the five rules on a slide, or just read them:

1. DTOs are records.
2. Async all the way down, with a CancellationToken.
3. Structured logging, never interpolation.
4. Validation errors are Problem Details.
5. Endpoints grouped with MapGroup, out of `Program.cs`.

## 3. Phase 2, Teach (4:00 to 9:00)

**Say:** "Session one. We teach the rules once, and we ask Copilot to write them down for every
future session."

**Do:** New chat, built-in Agent. Paste `phase2-teach.txt`. Send.

**Say** (while it runs, about 2 minutes): "Notice the last part of that prompt. I'm not just asking
for a refactor. I'm asking it to create `.github/copilot-instructions.md`, with a reason for each
rule. That file is special: Copilot reads it automatically at the start of every session, in chat,
in agent mode, in the CLI, and in code review on GitHub."

**Say:** "Why the reasons? Because a rule without a reason gets applied badly. 'Use records' is a
rule. 'Use records because DTOs should be immutable contracts' tells the agent where the rule
stops."

**Say:** "And notice what this file is. It's markdown, in the repo, in source control. When it
changes, that's a diff. Somebody reviews it. That matters in phase four."

**Do:** When it finishes, run the checkpoint: commit "Phase 2", then show the instructions diff.

**Say:** "Five rules, five reasons. And it refactored the code to match, and the build passes. But
that's the easy part. I told it the rules in the prompt. The real question is the next session."

## 4. Phase 3, Prove it (9:00 to 14:00)

**Say:** "Session two. Brand-new chat. Nothing carried over. And I won't mention a single rule."

**Do:** Open a **new** chat. Show the audience it's empty. Paste `phase3-prove.txt`. Send.

**Say** (while it runs, about 1 minute): "Read the prompt with me. An overdue endpoint and an
Assignee field. No records, no async, no logging, no MapGroup. If this session follows the rules,
it's because it read them from the file."

**Do:** When it finishes, commit "Phase 3" and run `check-rules.ps1`. Then show the new endpoint
in the extension class.

**Say:** "Async, with a CancellationToken. A structured log message with a named placeholder. It's
in the extension class, and `Program.cs` didn't change. The prompt didn't say any of that."

**If a rule misses, say:** "There's a miss. That's actually the useful part. The fix isn't a better
prompt; it's a clearer rule. Let's look at how that rule is worded." Read the rule and say what you'd
change. Then move on.

## 5. Phase 4, Self-improve (14:00 to 20:00)

**Say:** "So far *I* wrote the rules. But most conventions don't arrive as a list. They arrive as a
correction in the middle of a task: 'No, we don't do it that way.' And those corrections usually
die with the chat."

**Do:** Open `.github/copilot-instructions.md`. Type the last line yourself, on screen, and save:

```text
- When the user corrects a convention or pattern, update .github/copilot-instructions.md with the lesson before finishing the task.
```

**Say:** "One line. It tells Copilot that when I correct it, the lesson goes in the file."

**Do:** New chat. Paste `phase4-task.txt`. Send.

**Say** (while it runs, about 30 seconds): "This is a normal task. Add a ClosedAt timestamp when a
work order closes."

**Do:** When it finishes, point at the type it chose. (It usually picks `DateTime`.) Then send
`phase4-correction.txt` in the same chat.

**Say:** "And there it is: DateTime. That isn't wrong in general, but it's wrong *here*. So I
correct it, the way I'd correct a teammate. Once."

**Do:** When it finishes, commit "Phase 4" and show the instructions diff.

**Say:** "It fixed the code, including the existing OpenedAt. And look at the file: it added a new
rule. I didn't write that one. The next session, and everyone else on the team, gets it for free."

**Say:** "And because it's a diff, it goes through review like any other change. If the agent
writes down a bad lesson, you catch it in the pull request. The memory is visible, versioned and
reviewable. That's the difference from a memory feature hidden in a product."

**Optional, if the new rule has no reason:** "Notice it didn't write a reason for this one. That's a
review comment. We'd ask for one, exactly like we would from a person."

## 6. Close (20:00 to 22:00)

**Do:** Show `git log --oneline` with the three phase commits.

**Say:** "Three sessions, one file. Copilot didn't learn; the model is exactly the same as when we
started. What changed is the context it starts with, and that context now improves every time
someone corrects it."

**Say:** "Three things to take home:"

1. "Put your conventions in `.github/copilot-instructions.md`, with a reason for each."
2. "Add the one line that tells Copilot to write corrections down."
3. "Review that file like code. It's now part of your codebase."

**Say:** "And if you want to take this further, the appendix shows the same loop applied to code
review: a reviewer that remembers when you tell it a finding was wrong." See
`demo/appendix/README.md`.

## Q&A prep

- **"Doesn't the file get huge?"** It's loaded into every request, so keep it short and specific.
  Prune it in review like any other code. Path-specific rules can go in `.github/instructions/*.instructions.md`
  with an `applyTo` glob so they load only for matching files.
- **"What stops it writing down something wrong?"** Pull request review. The lesson is a diff.
- **"What stops a pull request from teaching it bad habits?"** The rule says to learn from the
  *user's* corrections, not from code it reads. Keep it that way.
- **"Does this work outside VS Code?"** Copilot CLI, the Copilot coding agent and Copilot code
  review on GitHub all read the same file. Claude Code and other agents have their own equivalents
  (`CLAUDE.md`, `AGENTS.md`).
- **"Is this the same as Copilot Memory?"** No. A built-in memory feature stores what it learns
  inside the product. This file lives in your repo, so you can read it, review it and roll it back.

## If something breaks

- **The build fails after a phase:** "Agents make mistakes too." Ask it to fix the build in the
  same chat, then continue.
- **It puts the rules file in the wrong place:** run the reset and start Phase 2 again. The reset
  creates `.github/` so this shouldn't happen.
- **You're running out of time:** skip the Phase 3 rule-by-rule walkthrough. Phase 4 is the payoff.
- **Total failure:** run `run-demo.ps1` and narrate over it.
