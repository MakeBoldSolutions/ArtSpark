# ArtSpark Web

## Purpose And Boundaries

`ArtSpark.Web` is the public ASP.NET Core MVC application. It composes the client and agent libraries, manages authentication and user collections, serves artwork and upload media, exposes SEO-friendly collection routes, and provides the live site behavior described in the root README.

## Owned Source Paths

- `ArtSpark.Web/Program.cs`
- `ArtSpark.Web/Controllers/`
- `ArtSpark.Web/Controllers/Api/`
- `ArtSpark.Web/Data/`
- `ArtSpark.Web/Services/`
- `ArtSpark.Web/ViewComponents/`
- `ArtSpark.Web/Views/`
- `ArtSpark.Web/wwwroot/`
- `ArtSpark.Web/prompts/agents/`

## Public Interfaces

The app maps MVC routes for public collections, artwork browsing, account collection pages, default controller actions, and `/health`. API controllers under `ArtSpark.Web/Controllers/Api/` expose application API surfaces.

## Runtime And Configuration Notes

Startup configures ASP.NET Core Identity with SQLite, Serilog logging, OpenTelemetry packages, Bootswatch theme switching, file upload options, static files for cached artwork images and uploads, hosted cleanup/monitoring services, and `IArtInstituteClient`.

## Tests And Validation Evidence

Web behavior is covered by controller, service, routing, and utility tests under `ArtSpark.Tests/` and `ArtSpark.Web/Tests/`. Current bootstrap did not execute the full test suite.

