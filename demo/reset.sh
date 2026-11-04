#!/usr/bin/env bash
# Resets the demo repo to the Phase 1 baseline on a throwaway demo-live branch.
# Usage: ./reset.sh <repo-path> [--force]
set -euo pipefail
repo="${1:?usage: reset.sh <repo-path> [--force]}"
force="${2:-}"

if [ -n "$(git -C "$repo" status --porcelain)" ] && [ "$force" != "--force" ]; then
  read -r -p "The repo has uncommitted changes that the reset will throw away. Continue? (y/n) " answer
  [ "$answer" = "y" ] || { echo "Reset cancelled."; exit 1; }
fi

git -C "$repo" reset --hard
git -C "$repo" clean -fd
# Use the remote branch, so a fresh clone works without checking out phase-1-baseline first.
git -C "$repo" checkout -B demo-live origin/phase-1-baseline
# Copilot CLI's write tool can't always create folders, so make the one Phase 2 writes into.
mkdir -p "$repo/.github"
dotnet build "$repo" --nologo -v q
echo "Reset to phase-1-baseline on branch demo-live. Baseline builds."
