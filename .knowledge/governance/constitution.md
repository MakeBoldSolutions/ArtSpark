<!--
Sync Impact Report
Version change: 0.0.0 -> 1.0.0
Modified principles:
- [PRINCIPLE_1_NAME] -> Live Web First
- [PRINCIPLE_2_NAME] -> Contract-Stable Shared Libraries
- [PRINCIPLE_3_NAME] -> Production Reliability & Observability
- [PRINCIPLE_4_NAME] -> Responsible AI & Cultural Respect
- [PRINCIPLE_5_NAME] -> Transparent Documentation & Deployment Records
Added sections: Architecture & Dependency Constraints; Delivery Workflow & Quality Gates
Removed sections: None
Templates requiring updates:
- ✅ .devspark/templates/plan-template.md
- ✅ .devspark/templates/spec-template.md
- ✅ .devspark/templates/tasks-template.md
Follow-up TODOs: None
-->

# ArtSpark Constitution

## Core Principles

### Live Web First
All roadmap items MUST articulate how they improve or safeguard `ArtSpark.Web`, the live
site. Supporting projects MAY change only when the work demonstrably benefits the web app's user
experience, resiliency, or delivery pipeline. If implementation begins outside the web app (for
example inside the client or agent libraries) the plan MUST include the web integration path prior
to merge. This keeps the repository centered on the public experience delivered at
https://art.makeboldspark.com.

### Contract-Stable Shared Libraries
`ArtSpark.Client`, `ArtSpark.Agent`, and `ArtSpark.Console` exist to serve
the web app and MUST preserve backwards-compatible contracts unless a coordinated web change ships in
the same release. Library breaking changes require semver-major tags, updated release notes, and
consumer impact testing in the web app before merge. Console utilities MUST remain optional developer
tooling and cannot ship user-facing features unavailable in the web app. This keeps shared components
stable while still enabling innovation.

### Production Reliability & Observability
The web app runs as a public site and MUST maintain production-grade quality. Every change requires
automated tests covering the affected layers (unit, integration, or UI) and MUST keep Serilog
instrumentation, build metadata surfacing, and EF Core migrations in sync. Performance-sensitive
APIs MUST document load expectations and regression guardrails inside specs. This discipline keeps
the live site trustworthy and diagnosable.

### Responsible AI & Cultural Respect
AI personas and generated responses MUST comply with cultural sensitivity guidance in `docs/` and
with OpenAI policy. Features that invoke AI MUST declare guardrails for prompt filtering, persona
voice, and data retention. New personas or AI endpoints cannot ship without reviewer confirmation
that safety scaffolding exists in web controllers, services, and UI messaging. This ensures the
experience honors the Art Institute's collections and users.

### Transparent Documentation & Deployment Records
Feature work MUST update relevant README and `docs/` guides so that operators can reproduce
configuration, secrets handling, and deployment steps. Release artifacts MUST record web build
numbers and SemVer tags, and any environment drift MUST be captured in
`docs/Documentation-Update-Summary.md` or successor logs. Clear documentation keeps contributors and
operators aligned across the multi-project solution.

## Architecture & Dependency Constraints

- The only sanctioned projects are the web app, Client library, Agent library, and Console
  tooling. Creating additional projects requires governance approval with a documented web-first
  rationale and rollback plan.
- Dependency direction is fixed: `ArtSpark.Web` may depend on Agent and Client; the Agent
  may depend on the Client; the Console may depend on the Client; reverse dependencies are
  prohibited unless the Constitution is amended.
- Cross-project changes MUST ship in a single pull request unless staged integration tests prove
  safety; staging PRs MUST reference the coordinating web update.
- Public endpoints MUST document rate limits, caching policy, and monitoring hooks before deployment
  to production infrastructure.

## Delivery Workflow & Quality Gates

- Every feature starts with `/devspark.specify` and `/devspark.plan` artifacts that explicitly describe
  web impact, shared library changes, testing strategy, and rollback steps.
- Plans MUST pass the Constitution Check gate: web alignment, library contract review, AI safety
  confirmation (when applicable), and observability coverage. Violations require entries in the
  plan's Complexity Tracking table.
- `/devspark.tasks` outputs MUST trace tasks to user stories and identify web-facing files, Agent
  service updates, and Client contract edits so reviewers can validate scope.
- Before merge, contributors MUST run solution-wide tests (`dotnet test` across relevant projects)
  and document results or the rationale for deferring runs.
- Releases MUST label build artifacts with the constitution version and web release notes so the
  operations log stays traceable.

## Governance

- This constitution supersedes conflicting process documents. Amendments require a dedicated PR with
  rationale, impact analysis on web stability, and updated templates.
- Constitution versions follow SemVer. Major revisions cover breaking governance changes; minor
  revisions add principles or tighten gates; patch revisions clarify wording. Version bumps MUST be
  recorded in the Sync Impact Report above.
- Ratified principles undergo quarterly compliance reviews: maintainers sample recent features to
  confirm web-first alignment, AI safeguards, and documentation freshness. Deficiencies trigger
  remediation plans before new feature work proceeds.

**Version**: 1.0.0 | **Ratified**: 2025-11-16 | **Last Amended**: 2025-11-16

