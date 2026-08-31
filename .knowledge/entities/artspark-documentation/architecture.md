# ArtSpark Documentation

## Purpose And Boundaries

`docs/` is the repository documentation set for current implementation guides, AI prompt authoring guidance, operational notes, testing checklists, quality audit outputs, and feature completion records. It supports contributors and operators rather than shipping runtime behavior.

## Owned Source Paths

- `docs/*.md`
- `docs/copilot/*.md`
- `docs/copilot/YYYY-MM-DD/*.md`

## Public Interfaces

The main entry points are linked from the root README, including the prompt authoring guide at `docs/copilot/prompt-authoring-guide.md` and quality audit reports under `docs/copilot/YYYY-MM-DD/`. Other top-level guides document AI chat, SEO, navigation, theme setup, live testing, and implementation history.

## Runtime And Configuration Notes

Documentation is Markdown-only. The quality audit workflow writes generated reports under `docs/copilot/YYYY-MM-DD/` by default, while hand-authored guides describe how to configure and operate ArtSpark features.

## Tests And Validation Evidence

Current discovery inspected representative documents and README references. The documentation set does not have an executable validation suite.

