---
description: DevSpark verify command shim.
---
## Prompt Resolution

Determine the current git user by running `git config user.name`.
Normalize to a folder-safe slug: lowercase, replace spaces with hyphens, strip non-alphanumeric/hyphen chars.

Read and execute the instructions from the **first file that exists**:
1. `.knowledge/overrides/{git-user}/commands/devspark.verify.md` (personalized override)
2. `.knowledge/overrides/commands/devspark.verify.md` (team customization)
3. `.devspark/defaults/commands/devspark.verify.md` (stock default)
## User Input

```text
$ARGUMENTS
```

Pass the user input above to the resolved prompt.
