# Entity Relations

| Subject | Type | Object |
|---|---|---|
| `artspark-agent` | uses | `artspark-client` |
| `artspark-agent` | supports | `artspark-web` |
| `artspark-agent` | validated_by | `artspark-tests` |
| `artspark-client` | supports | `artspark-web` |
| `artspark-client` | supports | `artspark-agent` |
| `artspark-client` | validated_by | `artspark-tests` |
| `artspark-console` | uses | `artspark-client` |
| `artspark-console` | uses | `artspark-agent` |
| `artspark-console` | validated_by | `artspark-tests` |
| `artspark-quality-audit` | validates | `artspark-solution` |
| `artspark-solution` | scopes | `artspark-web` |
| `artspark-solution` | scopes | `artspark-client` |
| `artspark-solution` | scopes | `artspark-agent` |
| `artspark-solution` | scopes | `artspark-console` |
| `artspark-solution` | scopes | `artspark-tests` |
| `artspark-solution` | scopes | `artspark-quality-audit` |
| `artspark-tests` | validates | `artspark-web` |
| `artspark-tests` | validates | `artspark-client` |
| `artspark-tests` | validates | `artspark-agent` |
| `artspark-tests` | validates | `artspark-console` |
| `artspark-web` | uses | `artspark-client` |
| `artspark-web` | uses | `artspark-agent` |
| `artspark-web` | validated_by | `artspark-tests` |
