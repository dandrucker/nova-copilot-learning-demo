---
name: pr-description
description: Write an Azure DevOps pull request description in the team format. Use when the user asks for a PR description, PR summary or release note for the current changes.
---
# Writing a PR description

1. Run `git diff main...HEAD --stat` and read the changed files.
2. Find the linked work item ID in the branch name or commit messages.
3. Write the description in this format:

```
## What changed
One or two sentences a reviewer can read in ten seconds.

## Why
Link the work item: AB#<id>

## How it was tested
- Tests added or updated
- Manual checks, if any

## Risk
Low, medium or high, with one line of reasoning.
```

Keep it under 200 words. Don't list every file; name only what a reviewer should look at first.
