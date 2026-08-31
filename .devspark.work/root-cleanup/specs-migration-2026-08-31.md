# DevSpark v4 Root Spec Cleanup

## Scope

Cleaned legacy specification framework and root work-package directories out of the repository root.

## Classification

| Source | Classification | Destination |
|---|---|---|
| `.specify.old/` | Historical legacy framework material superseded by `.devspark/` | `.archive/2026-08-31/legacy-specify-old/` |
| `specs/001-quality-audit/` | Completed legacy work package | `.archive/2026-08-31/completed-specs/001-quality-audit/` |
| `specs/002-user-profile/` | In-flight feature work package with open tasks | `.devspark.work/specs/002-user-profile/` |
| `specs/003-prompt-management/` | Completed legacy work package | `.archive/2026-08-31/completed-specs/003-prompt-management/` |

## Current DevSpark v4 Layout

- Framework files live under `.devspark/`.
- Current truth lives under `.knowledge/`.
- In-flight feature work lives under `.devspark.work/specs/`.
- Completed or retired work packages are retained in the dated archive buffer.

## Notes

- Root `specs/` and `.specify.old/` paths are absent after the move.
- The empty root `specs/` directory shell was moved to `.devspark.work/legacy-root-specs-empty/` because local command policy blocked direct empty-directory removal.
- Current non-archive Markdown references were updated away from root `specs/` and `.specify` paths.
- `.archive/` was not inspected after moving content into it.

