# ArtSpark Tests

## Purpose And Boundaries

`ArtSpark.Tests` contains automated test coverage for the main repository components. Test folders are organized around agent behavior, controller behavior, service behavior, console prompt variants, and utilities.

## Owned Source Paths

- `ArtSpark.Tests/`
- `ArtSpark.Web/Tests/`
- root smoke-style test files such as `TestArtistSearchFunctionality.cs` and `TestWebSpark.cs`

## Public Interfaces

Tests exercise repository behavior through public controllers, services, prompt loader APIs, persona APIs, and utility methods.

## Runtime And Configuration Notes

The test project targets the solution's .NET stack and references the production projects it validates.

## Tests And Validation Evidence

Current bootstrap inspected the test layout but did not execute the full test suite.
