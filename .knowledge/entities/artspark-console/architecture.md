# ArtSpark Console

## Purpose And Boundaries

`ArtSpark.Console` is developer tooling for exercising ArtSpark client behavior from a command-line entry point. It configures the same HTTP utility services used by the web application, builds an `ArtInstituteClient`, prints prompt-management configuration when present, retrieves artworks, and runs artist-search examples.

## Owned Source Paths

- `ArtSpark.Console/Program.cs`
- `ArtSpark.Console/ArtistSearchTest.cs`
- `ArtSpark.Console/appsettings.json`
- `ArtSpark.Console/appsettings.Development.json`

## Public Interfaces

The command surface is the console executable produced by `ArtSpark.Console.csproj`. It writes API connectivity and artist-search results to standard output and waits for user input before exiting.

## Runtime And Configuration Notes

The project targets `net10.0`, references `ArtSpark.Client` and `ArtSpark.Agent`, and uses Microsoft configuration/logging packages, `WebSpark.HttpClientUtility`, OpenTelemetry packages, and appsettings files copied to output.

## Tests And Validation Evidence

`ArtSpark.Tests/Console/PromptVariantTests.cs` validates console-style prompt variant configuration. Current discovery inspected the console project but did not run the full test suite.

