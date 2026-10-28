<#
.SYNOPSIS
  Copies one demo step's Copilot files into the repository root.
.EXAMPLE
  ./scripts/apply-step.ps1 -Step 2
#>
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('2', '3', '4', '5', 'skill', 'reset')]
    [string]$Step
)

$root = Split-Path -Parent $PSScriptRoot

if ($Step -eq 'reset') {
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue (Join-Path $root '.github')
    Remove-Item -Force -ErrorAction SilentlyContinue (Join-Path $root '.vscode/mcp.json')
    Write-Host 'Removed the demo Copilot files. Back to the baseline.'
    return
}

$folder = if ($Step -eq 'skill') { 'optional-skill' } else { "step$Step" }
$source = Join-Path $root "demo-steps/$folder"
Copy-Item -Path (Join-Path $source '*') -Destination $root -Recurse -Force
Write-Host "Applied demo step $Step from demo-steps/$folder."
