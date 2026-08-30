# ArtSpark Quality Audit

## Purpose And Boundaries

`scripts/audit/` provides a contributor quality workflow that collects build diagnostics, NuGet and npm dependency currency, safeguard audit results, and Markdown reports.

## Owned Source Paths

- `scripts/audit/run-quality-audit.ps1`
- `scripts/audit/QualityAudit.psm1`
- `scripts/audit/modules/`
- `scripts/audit/tests/`

## Public Interfaces

The main command is `pwsh -File scripts/audit/run-quality-audit.ps1`, with parameters for output directory, severity filtering, item limits, and skipping build, dependency, or safeguard phases.

## Runtime And Configuration Notes

The script writes audit output under `docs/copilot/YYYY-MM-DD/` by default and imports modules for build diagnostics, package currency, and safeguard checks.

## Tests And Validation Evidence

PowerShell test coverage exists under `scripts/audit/tests/`. Current bootstrap inspected the workflow but did not execute it.

