<#
.SYNOPSIS
  Quick, heuristic check of the five house rules against the current code.
  The presenter agent does a fuller review; this is the no-agent fallback.
.EXAMPLE
  ./check-rules.ps1 -RepoPath ..\nova-copilot-learning-demo
#>
param([Parameter(Mandatory = $true)][string]$RepoPath)

$src = Join-Path $RepoPath 'src'
$files = Get-ChildItem -Path $src -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
$script:fails = 0

function Report($name, [bool]$pass, $detail) {
    if ($pass) { Write-Host "PASS  $name" -ForegroundColor Green }
    else { Write-Host "FAIL  $name`: $detail" -ForegroundColor Red; $script:fails++ }
}

# 1. DTOs are records
$hits = $files | Select-String -Pattern '\bclass\s+(WorkOrder|\w+(Request|Response|Dto))\b'
Report 'DTOs are records' (-not $hits) ($hits | Select-Object -First 1)

# 2. Async with CancellationToken on repository interface methods
$iface = $files | Select-String -Pattern 'interface\s+I\w*Repository' -List | Select-Object -First 1
$bad = $null
if ($iface) {
    $bad = Select-String -Path $iface.Path -Pattern '^\s*[^ /].*\s\w+\(.*\);' |
        Where-Object { $_.Line -notmatch 'Task.*CancellationToken' }
}
$detail = if (-not $iface) { 'no repository interface found' } else { ($bad | Select-Object -First 1).Line.Trim() }
Report 'Async with CancellationToken' ($iface -and -not $bad) $detail

# 3. Structured logging
$hits = $files | Select-String -Pattern 'Log(Trace|Debug|Information|Warning|Error|Critical)\(\s*\$"'
Report 'Structured logging' (-not $hits) ($hits | Select-Object -First 1)

# 4. ValidationProblem for validation errors
$bad = $files | Select-String -Pattern 'Results\.BadRequest\('
$has = $files | Select-String -Pattern 'ValidationProblem' -List
$detail = if ($bad) { $bad | Select-Object -First 1 } else { 'no ValidationProblem found' }
Report 'ValidationProblem for validation errors' ((-not $bad) -and $has) $detail

# 5. Endpoints grouped with MapGroup outside Program.cs
$program = $files | Where-Object Name -eq 'Program.cs' | Select-Object -First 1
$inline = Select-String -Path $program.FullName -Pattern '\bapp\.Map(Get|Post|Put|Patch|Delete)\('
$group = $files | Where-Object Name -ne 'Program.cs' | Select-String -Pattern 'MapGroup\(' -List
$detail = if ($inline) { $inline | Select-Object -First 1 } else { 'no MapGroup outside Program.cs' }
Report 'Endpoints grouped with MapGroup outside Program.cs' ((-not $inline) -and $group) $detail

if ($files | Select-String -Pattern '"/?overdue"' -List) { Write-Host 'INFO  /workorders/overdue endpoint found' }
else { Write-Host 'INFO  /workorders/overdue endpoint not found' }

Write-Host "$script:fails rule(s) failed. These checks are heuristics; read the diff for the full story."
