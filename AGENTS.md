# AGENTS.md
## DevSpark

- DevSpark framework files live in `.devspark/`.
- Project artifacts and team overrides live in `.knowledge/`.
- Resolve DevSpark commands through the first existing file:
  1. `.knowledge/overrides/{git-user}/commands/devspark.{name}.md`
  2. `.knowledge/overrides/commands/devspark.{name}.md`
  3. `.devspark/defaults/commands/devspark.{name}.md`
- Preserve user work in `.knowledge/`; upgrades refresh `.devspark/` only.
