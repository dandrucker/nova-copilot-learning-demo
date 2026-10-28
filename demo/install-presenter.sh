#!/usr/bin/env bash
# Installs the NOVA Presenter custom agent at user level (~/.copilot/agents), outside the repo.
set -euo pipefail
target="$HOME/.copilot/agents"
mkdir -p "$target"
cp "$(dirname "$0")/presenter.agent.md" "$target/nova-presenter.agent.md"
echo "Installed NOVA Presenter to $target. Reload VS Code, then pick 'NOVA Presenter' in the agents dropdown."
