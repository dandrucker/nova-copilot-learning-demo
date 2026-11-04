# Appendix: a code reviewer that learns

The demo shows Copilot writing down a lesson when you correct it while it's coding. The same loop
works for code review, where corrections are even more common: "that's not a bug, we do it on
purpose" is a review comment every team has typed more than once.

## What's here

| Path | What it is |
| --- | --- |
| `skills/add-learning-loop/SKILL.md` | A skill that upgrades any code review skill so it learns from your corrections |
| `skills/code-review/SKILL.md` | A plain code review skill to try it on |

After the upgrade, the review skill:

1. Reads `.github/copilot-instructions.md` before every review and treats each rule as a review
   criterion, so it stops flagging code that follows your conventions.
2. Names the rule behind each finding, so you can see where a finding came from.
3. Writes a new rule, with a reason, when you dismiss a finding or tell it something it missed.
4. Learns only from what you say, never from the code or pull request text it's reviewing, so a
   pull request can't teach the reviewer its own exceptions.

Because it uses the same file as the demo, a lesson learned in review also applies the next time
Copilot writes code, and the other way round.

## Try it

1. Copy both skill folders into the repo's `.github/skills/` folder (or into `~/.copilot/skills/`
   to use them in every repo). Reload VS Code.
2. In a new Agent chat: "Use the add-learning-loop skill on the code-review skill."
3. Review the diff to `.github/skills/code-review/SKILL.md`.
4. Make a change, then ask for a code review.
5. Dismiss one finding with a reason, for example: "That one's fine, we log work order IDs on
   purpose because support searches by them."
6. Check that `.github/copilot-instructions.md` has a new rule with that reason, then run the
   review again and check the finding is gone.

The upgrade skill works on other review skills too, including Claude Code ones. Give it the path,
and name `CLAUDE.md` as the lessons file if that's where your conventions live.
