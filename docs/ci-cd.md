# CI/CD with GitHub Actions

This repo includes two workflows under `.github/workflows`:

- `CI` (ci.yml): On every push and PR, builds the package from the file-based project and uploads the resulting `.nupkg` as a build artifact.
- `Publish` (publish.yml): On tag push, publishes to NuGet depending on the tag naming convention below.

## Publish rules (by tag name)

- `preview/vX.Y.Z` → Publishes a prerelease package with version `X.Y.Z-preview` to NuGet.
- `release/vX.Y.Z` → Publishes a stable package with version `X.Y.Z` to NuGet.

## Requirements for publishing

- Create a repository secret `NUGET_API_KEY` containing your NuGet.org API key.

## Tagging examples (pwsh)

```pwsh
# Preview publish (creates version 1.0.1-preview on NuGet)
git tag preview/v1.0.1
git push origin preview/v1.0.1

# Release publish (creates version 1.0.2 on NuGet)
git tag release/v1.0.2
git push origin release/v1.0.2
```

## How the CI build works

1. Converts `dotnet_fba.cs` into a regular project with `dotnet project convert` into `./projects`.
2. Packs with `dotnet pack` into `./artifacts/nupkg`.
3. Uploads `*.nupkg` as an artifact so you can download and test locally.
