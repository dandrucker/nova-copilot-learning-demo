<#
.SYNOPSIS
  Resets the demo repo to the Phase 1 baseline on a throwaway demo-live branch.
.EXAMPLE
  ./reset.ps1 -RepoPath ..\nova-copilot-learning-demo
#>
param(
    [Parameter(Mandatory = $true)][string]$RepoPath,
    [switch]$Force
)
$ErrorActionPreference = 'Stop'

$dirty = git -C $RepoPath status --porcelain
if ($dirty -and -not $Force) {
    $answer = Read-Host 'The repo has uncommitted changes that the reset will throw away. Continue? (y/n)'
    if ($answer -ne 'y') { Write-Host 'Reset cancelled.'; exit 1 }
}

git -C $RepoPath reset --hard
git -C $RepoPath clean -fd
# Use the remote branch, so a fresh clone works without checking out phase-1-baseline first.
git -C $RepoPath checkout -B demo-live origin/phase-1-baseline
if ($LASTEXITCODE -ne 0) { throw 'Could not check out origin/phase-1-baseline.' }

# Copilot CLI's write tool can't always create folders, so make the one Phase 2 writes into.
New-Item -ItemType Directory -Force -Path (Join-Path $RepoPath '.github') | Out-Null

dotnet build $RepoPath --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'The baseline did not build.' }
Write-Host 'Reset to phase-1-baseline on branch demo-live. Baseline builds.'
