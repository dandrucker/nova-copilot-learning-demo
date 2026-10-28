#!/usr/bin/env bash
# No-agent fallback for the NOVA "Teaching Copilot to Learn" demo. Runs every phase in a fresh,
# non-interactive Copilot CLI session and pauses for Enter between steps.
# Usage: ./run-demo.sh <repo-path>
set -euo pipefail
here="$(cd "$(dirname "$0")" && pwd)"
repo="$(cd "${1:?usage: run-demo.sh <repo-path>}" && pwd)"
prompts="$here/prompts"

say() { printf '\n\033[36m>> %s\033[0m\n' "$1"; }
pause_demo() { read -r -p "Press Enter to continue" _; }
prompt_text() { tr -d '\r' < "$prompts/$1" | sed -e 's/[[:space:]]*$//'; }

invoke_copilot() {
  printf '\n\033[90mCopilot prompt: %s\033[0m\n' "$1"
  (cd "$repo" && copilot -p "$1" --allow-tool='write, shell(dotnet:*)' --no-ask-user) || echo "Copilot CLI exited with an error."
}

checkpoint() {
  git -C "$repo" add -A
  git -C "$repo" -c user.name="NOVA demo" -c user.email="nova-demo@localhost" commit -q -m "Phase $1" --allow-empty
  say "Phase $1 checkpoint: changes to .github/copilot-instructions.md"
  git -C "$repo" --no-pager diff HEAD~1 -- .github/copilot-instructions.md
  say "Files changed"
  git -C "$repo" --no-pager diff HEAD~1 --stat
  say "dotnet build"
  if dotnet build "$repo" --nologo -v q; then echo "Build passed."; else echo "Build FAILED."; fi
}

command -v copilot >/dev/null || { echo "Copilot CLI is not installed. See DEMO_RUNBOOK.md." >&2; exit 1; }

say "Step 0: reset to the Phase 1 baseline (Copilot default style)."
"$here/reset.sh" "$repo"
pause_demo

say "Phase 2, Teach: we teach Copilot our house rules once and have it write them down."
pause_demo
invoke_copilot "$(prompt_text phase2-teach.txt)"
checkpoint 2
pause_demo

say "Phase 3, Prove it: brand-new session, no rules in the prompt."
pause_demo
invoke_copilot "$(prompt_text phase3-prove.txt)"
checkpoint 3
say "Rule-by-rule check"
"$here/check-rules.sh" "$repo" || true
pause_demo

say "Phase 4, Self-improve: add the self-improvement rule, then correct Copilot once."
instructions="$repo/.github/copilot-instructions.md"
[ -f "$instructions" ] || { echo "Phase 2 did not create .github/copilot-instructions.md." >&2; exit 1; }
printf '\n%s\n' "$(prompt_text phase4-rule.txt)" >> "$instructions"
git -C "$repo" add -A
git -C "$repo" -c user.name="NOVA demo" -c user.email="nova-demo@localhost" commit -q -m "Phase 4: add self-improvement rule"
git -C "$repo" --no-pager diff HEAD~1 -- .github/copilot-instructions.md
pause_demo
invoke_copilot "$(prompt_text phase4-cli-combined.txt)"
checkpoint 4
say "Look for the DateTimeOffset lesson Copilot wrote into the instructions file above."
say "Done. Run this script again to reset and repeat."
