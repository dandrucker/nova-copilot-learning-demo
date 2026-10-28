<#
.SYNOPSIS
  No-agent fallback for the NOVA "Teaching Copilot to Learn" demo. Runs every phase in a fresh,
  non-interactive Copilot CLI session and pauses for Enter between steps.
.EXAMPLE
  ./run-demo.ps1 -RepoPath ..\nova-copilot-learning-demo
#>
param([Parameter(Mandatory = $true)][string]$RepoPath)
$ErrorActionPreference = 'Stop'
$prompts = Join-Path $PSScriptRoot 'prompts'
$RepoPath = (Resolve-Path $RepoPath).Path

function Say($text) { Write-Host ''; Write-Host ">> $text" -ForegroundColor Cyan }
function Pause-Demo { Read-Host 'Press Enter to continue' | Out-Null }
function Prompt-Text($name) { (Get-Content -Raw (Join-Path $prompts $name)).Trim() }

function Invoke-Copilot($prompt) {
    Write-Host ''
    Write-Host "Copilot prompt: $prompt" -ForegroundColor DarkGray
    Push-Location $RepoPath
    try {
        copilot -p $prompt "--allow-tool=write, shell(dotnet:*)" --no-ask-user
        if ($LASTEXITCODE -ne 0) { Write-Host 'Copilot CLI exited with an error.' -ForegroundColor Yellow }
    }
    finally { Pop-Location }
}

function Checkpoint($phase) {
    git -C $RepoPath add -A
    git -C $RepoPath -c user.name='NOVA demo' -c user.email='nova-demo@localhost' commit -q -m "Phase $phase" --allow-empty
    Say "Phase $phase checkpoint: changes to .github/copilot-instructions.md"
    git -C $RepoPath --no-pager diff HEAD~1 -- .github/copilot-instructions.md
    Say 'Files changed'
    git -C $RepoPath --no-pager diff HEAD~1 --stat
    Say 'dotnet build'
    dotnet build $RepoPath --nologo -v q
    if ($LASTEXITCODE -eq 0) { Write-Host 'Build passed.' -ForegroundColor Green }
    else { Write-Host 'Build FAILED.' -ForegroundColor Red }
}

if (-not (Get-Command copilot -ErrorAction SilentlyContinue)) {
    throw 'Copilot CLI is not installed. See DEMO_RUNBOOK.md, or run the demo in VS Code Copilot Chat instead.'
}

Say 'Step 0: reset to the Phase 1 baseline (Copilot default style).'
& (Join-Path $PSScriptRoot 'reset.ps1') -RepoPath $RepoPath
Pause-Demo

Say 'Phase 2, Teach: we teach Copilot our house rules once and have it write them down.'
Pause-Demo
Invoke-Copilot (Prompt-Text 'phase2-teach.txt')
Checkpoint 2
Pause-Demo

Say 'Phase 3, Prove it: brand-new session, no rules in the prompt.'
Pause-Demo
Invoke-Copilot (Prompt-Text 'phase3-prove.txt')
Checkpoint 3
Say 'Rule-by-rule check'
& (Join-Path $PSScriptRoot 'check-rules.ps1') -RepoPath $RepoPath
Pause-Demo

Say 'Phase 4, Self-improve: add the self-improvement rule, then correct Copilot once.'
$instructions = Join-Path $RepoPath '.github/copilot-instructions.md'
if (-not (Test-Path $instructions)) { throw 'Phase 2 did not create .github/copilot-instructions.md.' }
Add-Content -Path $instructions -Value ("`n" + (Prompt-Text 'phase4-rule.txt'))
git -C $RepoPath add -A
git -C $RepoPath -c user.name='NOVA demo' -c user.email='nova-demo@localhost' commit -q -m 'Phase 4: add self-improvement rule'
Write-Host 'Added the self-improvement rule:' -ForegroundColor DarkGray
git -C $RepoPath --no-pager diff HEAD~1 -- .github/copilot-instructions.md
Pause-Demo
Invoke-Copilot (Prompt-Text 'phase4-cli-combined.txt')
Checkpoint 4
Say 'Look for the DateTimeOffset lesson Copilot wrote into the instructions file above.'
Say 'Done. Run this script again to reset and repeat.'
