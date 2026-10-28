#!/usr/bin/env bash
# Copies one demo step's Copilot files into the repository root.
# Usage: ./scripts/apply-step.sh 2|3|4|5|skill|reset
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
step="${1:?usage: apply-step.sh 2|3|4|5|skill|reset}"

if [ "$step" = "reset" ]; then
  rm -rf "$root/.github" "$root/.vscode/mcp.json"
  echo "Removed the demo Copilot files. Back to the baseline."
  exit 0
fi

case "$step" in
  2|3|4|5) folder="step$step" ;;
  skill) folder="optional-skill" ;;
  *) echo "Unknown step: $step" >&2; exit 1 ;;
esac

cp -R "$root/demo-steps/$folder/." "$root/"
echo "Applied demo step $step from demo-steps/$folder."
