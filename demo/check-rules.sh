#!/usr/bin/env bash
# Quick, heuristic check of the five house rules against the current code.
# The presenter agent does a fuller review; this is the no-agent fallback.
# Usage: ./check-rules.sh <repo-path>
set -uo pipefail
repo="${1:?usage: check-rules.sh <repo-path>}"
src="$repo/src"
fails=0

report() { # name, pass(0/1), detail
  if [ "$2" -eq 0 ]; then echo "PASS  $1"; else echo "FAIL  $1: $3"; fails=$((fails+1)); fi
}

cs_files=$(find "$src" -name '*.cs' -not -path '*/obj/*' -not -path '*/bin/*')

# 1. DTOs are records: no class declarations for WorkOrder, *Request, *Response or *Dto types.
hits=$(grep -nE '\bclass\s+(WorkOrder|\w+(Request|Response|Dto))\b' $cs_files)
report "DTOs are records" $([ -z "$hits" ] && echo 0 || echo 1) "$(echo "$hits" | head -1)"

# 2. Async with CancellationToken: repository interface methods return Task and take a CancellationToken.
iface=$(grep -lE 'interface\s+I\w*Repository' $cs_files | head -1)
bad=""
if [ -n "$iface" ]; then
  bad=$(grep -E '^\s*[^ /].*\s\w+\(.*\);' "$iface" | grep -vE 'Task.*CancellationToken')
fi
report "Async with CancellationToken" $([ -n "$iface" ] && [ -z "$bad" ] && echo 0 || echo 1) "$(echo "${bad:-no repository interface found}" | head -1 | sed 's/^ *//')"

# 3. Structured logging: no interpolated strings passed to Log* calls.
hits=$(grep -nE 'Log(Trace|Debug|Information|Warning|Error|Critical)\(\s*\$"' $cs_files)
report "Structured logging" $([ -z "$hits" ] && echo 0 || echo 1) "$(echo "$hits" | head -1)"

# 4. Validation errors use Results.ValidationProblem, not plain-text BadRequest.
bad=$(grep -nE 'Results\.BadRequest\(' $cs_files)
has=$(grep -lE 'ValidationProblem' $cs_files)
report "ValidationProblem for validation errors" $([ -z "$bad" ] && [ -n "$has" ] && echo 0 || echo 1) "$(echo "${bad:-no ValidationProblem found}" | head -1)"

# 5. Endpoints grouped with MapGroup outside Program.cs.
program=$(find "$src" -name Program.cs -not -path '*/obj/*' | head -1)
inline=$(grep -nE '\bapp\.Map(Get|Post|Put|Patch|Delete)\(' "$program")
group=$(grep -lE 'MapGroup\(' $cs_files | grep -v 'Program.cs')
report "Endpoints grouped with MapGroup outside Program.cs" $([ -z "$inline" ] && [ -n "$group" ] && echo 0 || echo 1) "$(echo "${inline:-no MapGroup outside Program.cs}" | head -1)"

# Phase 3 feature present?
if grep -qE '"/?overdue"' $cs_files; then echo "INFO  /workorders/overdue endpoint found"; else echo "INFO  /workorders/overdue endpoint not found"; fi

echo "$fails rule(s) failed. These checks are heuristics; read the diff for the full story."
