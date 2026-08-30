# ArtSpark Solution

## Purpose And Boundaries

ArtSpark is a multi-project .NET 10 repository for working with the Art Institute of Chicago public API. The root README describes four main projects: `ArtSpark.Client`, `ArtSpark.Agent`, `ArtSpark.Web`, and `ArtSpark.Console`, with tests and documentation supporting those projects.

## Owned Source Paths

- `ArtSpark.sln`
- `ArtSpark.Client/`
- `ArtSpark.Agent/`
- `ArtSpark.Web/`
- `ArtSpark.Console/`
- `ArtSpark.Tests/`
- `docs/`
- `scripts/audit/`

## Public Interfaces

The primary user-facing interface is `ArtSpark.Web`. Shared project interfaces include the Art Institute client contracts in `ArtSpark.Client/Interfaces/`, AI agent contracts in `ArtSpark.Agent/Interfaces/`, and developer command-line behavior in `ArtSpark.Console/Program.cs`.

## Runtime And Configuration Notes

Project files target `net10.0`. The web app uses ASP.NET Core, Identity, EF Core SQLite, Serilog, WebSpark packages, OpenTelemetry packages, and the ArtSpark client and agent projects.

## Evidence

- `README.md`
- `ArtSpark.sln`
- `ArtSpark.Web/ArtSpark.Web.csproj`
- `ArtSpark.Agent/ArtSpark.Agent.csproj`
- `ArtSpark.Client/ArtSpark.Client.csproj`
