# ArtSpark Agent

## Purpose And Boundaries

`ArtSpark.Agent` provides AI chat capabilities for artwork conversations. It owns persona selection, prompt generation, artwork enrichment, guard rail validation, conversation memory, suggested questions, and optional vision-enabled messages.

## Owned Source Paths

- `ArtSpark.Agent/Configuration/`
- `ArtSpark.Agent/Extensions/`
- `ArtSpark.Agent/Interfaces/`
- `ArtSpark.Agent/Models/`
- `ArtSpark.Agent/Personas/`
- `ArtSpark.Agent/Services/`

## Public Interfaces

Primary contracts include `IArtworkChatAgent`, `IPersonaHandler`, `IPersonaFactory`, `IArtworkDataProvider`, `IChatMemory`, and `IChatInputValidator`. The web project registers the agent through `AddArtSparkAgent`.

## Runtime And Configuration Notes

The project targets `net10.0` and uses Microsoft Semantic Kernel, OpenAI connector packages, Microsoft Extensions options/hosting/HTTP packages, WebSpark HTTP utilities, and OpenTelemetry packages. Runtime behavior is configured through `AgentConfiguration`.

## Tests And Validation Evidence

Agent behavior is represented by tests under `ArtSpark.Tests/Agent/`, including persona, prompt loader, prompt parser, token replacement, hot reload, audit logging, and configuration validation tests. Current bootstrap did not execute the full test suite.

