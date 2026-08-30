# ArtSpark Client

## Purpose And Boundaries

`ArtSpark.Client` provides typed access to the Art Institute of Chicago API. `ArtInstituteClient` centralizes URL building, response parsing, search endpoints, batch ID lookups, and IIIF image URL construction.

## Owned Source Paths

- `ArtSpark.Client/Clients/`
- `ArtSpark.Client/Interfaces/`
- `ArtSpark.Client/Models/`
- `ArtSpark.Client/Examples/`

## Public Interfaces

The public surface is `IArtInstituteClient`, implemented by `ArtInstituteClient`. It exposes artwork, agent, place, gallery, exhibition, media, shop, mobile, scholarly catalog, static archive, and website methods, plus search and IIIF helpers.

## Runtime And Configuration Notes

The library targets `net10.0` and depends on `WebSpark.HttpClientUtility`, `Microsoft.AspNet.WebApi.Client`, and OpenTelemetry packages. JSON handling uses `System.Text.Json` snake case naming and case-insensitive property binding.

## Tests And Validation Evidence

Client behavior is represented in tests such as `ArtSpark.Tests/Utilities/ArtInstituteImageUrlTests.cs` and client model tests under `ArtSpark.Client/Tests/`. Current bootstrap did not execute the full test suite.

