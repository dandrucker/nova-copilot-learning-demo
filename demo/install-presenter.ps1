<#
.SYNOPSIS
  Installs the NOVA Presenter custom agent at user level (~/.copilot/agents), outside the repo,
  so it survives the reset to phase-1-baseline and the Phase 3 session can't read it.
#>
$target = Join-Path $HOME '.copilot/agents'
New-Item -ItemType Directory -Force -Path $target | Out-Null
Copy-Item -Path (Join-Path $PSScriptRoot 'presenter.agent.md') -Destination (Join-Path $target 'nova-presenter.agent.md') -Force
Write-Host "Installed NOVA Presenter to $target. Reload VS Code, then pick 'NOVA Presenter' in the agents dropdown."
