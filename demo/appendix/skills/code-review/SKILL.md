---
name: code-review
description: Review the current changes for bugs, risky patterns and readability before a pull request. Use when the user asks for a code review of their changes.
---
# Reviewing the current changes

1. Run `git diff main...HEAD`, or `git diff` if the changes aren't committed yet, and read every
   changed file in full, not just the diff.
2. Look for, in this order:
   - Bugs: wrong logic, unhandled nulls, off-by-one errors, missing awaits.
   - Risk: secrets or customer data in code or logs, unvalidated input, breaking API changes.
   - Readability: unclear names, long methods, dead code.
3. Report each finding in this format:

```
### <short title>
- File: path:line
- Severity: high, medium or low
- Why: one or two sentences
- Fix: the change you'd make
```

List high severity first. If you find nothing, say so in one line. Don't edit any files.
