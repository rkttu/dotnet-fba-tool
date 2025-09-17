# dotnet-fba-tool

File-based app source scaffolding tool for .NET 10.0 and later

## Overview

`dotnet-fba-tool` helps you scaffold a “File‑Based App (FBA)” in a single C# file that can include SDK declarations, project properties, and package references. It generates a runnable template file. You can later convert it to a regular project with `dotnet project convert`, or keep using it as-is in your build pipeline.

This repository itself is authored in a file-based style (`dotnet_fba.cs`). After converting, you can `dotnet pack` to create a .NET Global Tool.

## Quick start

### 1) Build the tool package locally (optional)

If you don’t use a published NuGet package yet, you can build a local tool from this repository.

Bash/Zsh example:

```bash
# 1) Remove the existing 'projects' folder if it exists.
rm -rf ./projects

# 2) Convert the file-based project to a regular project.
dotnet project convert ./dotnet_fba.cs -o ./projects

# 3) Create the NuGet tool package.
dotnet pack ./projects

# 4) Inspect the generated nupkg.
ls ./projects/nupkg/*.nupkg
```

PowerShell (pwsh) example:

```pwsh
# 1) Remove the existing 'projects' folder if it exists.
Remove-Item -Recurse -Force .\projects -ErrorAction SilentlyContinue

# 2) Convert the file-based project to a regular project.
dotnet project convert .\dotnet_fba.cs -o .\projects

# 3) Create the NuGet tool package.
dotnet pack .\projects

# 4) Inspect the generated nupkg.
Get-ChildItem .\projects\nupkg\*.nupkg
```

Install the global tool from the local package:

```bash
dotnet tool install --global dotnet-fba --add-source ./projects/nupkg
# Update
dotnet tool update  --global dotnet-fba --add-source ./projects/nupkg
# Uninstall
dotnet tool uninstall --global dotnet-fba
```

```pwsh
dotnet tool install --global dotnet-fba --add-source .\projects\nupkg
# Update
dotnet tool update  --global dotnet-fba --add-source .\projects\nupkg
# Uninstall
dotnet tool uninstall --global dotnet-fba
```

### 2) Basic usage

The following command creates `hello.cs` in the current directory. It’s recommended to include the minimal properties (`TargetFramework`, `OutputType`) so the file-based app can build successfully.

Bash/Zsh:

```bash
dotnet-fba -o ./hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

PowerShell (pwsh):

```pwsh
dotnet-fba -o .\hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

To include package references, additional SDKs, and a Unix shebang:

Bash/Zsh:

```bash
dotnet-fba -o ./hello.cs \
   --shebang \
   --sdk "Microsoft.NET.Sdk.Web" \
   -p TargetFramework=net10.0 -p OutputType=Exe -p LangVersion=preview \
   --nuget "Spectre.Console@0.49.1" --nuget "System.CommandLine@2.0.0-beta4.24105.1"
```

PowerShell (pwsh):

```pwsh
dotnet-fba -o .\hello.cs `
   --shebang `
   --sdk "Microsoft.NET.Sdk.Web" `
   -p TargetFramework=net10.0 -p OutputType=Exe -p LangVersion=preview `
   --nuget "Spectre.Console@0.49.1" --nuget "System.CommandLine@2.0.0-beta4.24105.1"
```

You can also pipe to STDOUT:

Bash/Zsh:

```bash
dotnet-fba -o - -p TargetFramework=net10.0 > ./Program.cs
```

PowerShell (pwsh):

```pwsh
dotnet-fba -o - -p TargetFramework=net10.0 | Set-Content .\Program.cs -NoNewline
```

## Run directly from the web (zero-install stream)

You can run this tool without cloning or installing anything, by streaming the file-based source from the web and executing it in-memory. The following one-liner downloads the tool source, runs it to generate a template to STDOUT, and immediately runs the generated app—entirely via pipes.

Bash/Zsh:

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs \
   | dotnet run - -o - \
   | dotnet run -
```

PowerShell (pwsh):

```pwsh
curl.exe -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs `
   | dotnet run - -o - `
   | dotnet run -
```

### How it works

This pipeline uses .NET’s file-based app execution model and standard streams:

1) Fetch tool source
    - `curl` downloads the single-file tool source (`dotnet_fba.cs`) and writes it to STDOUT.

2) Run the tool from STDIN
    - `dotnet run -` consumes the C# source from STDIN and compiles/executes it as a File‑Based App.
    - We pass `-o -` to the tool so it emits the scaffolded C# template to STDOUT instead of a file.

3) Run the generated app from STDIN
    - The second `dotnet run -` reads the generated C# program from STDIN and runs it immediately.

Effectively: stream source → run tool → stream generated app → run app — no local files required unless you choose to persist them.

### Customizing the generated app

You can provide properties/packages to the middle stage (the tool) as usual. For example, to ensure minimal properties are set:

Bash/Zsh:

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs \
   | dotnet run - -o - -p TargetFramework=net10.0 -p OutputType=Exe \
   | dotnet run -
```

PowerShell (pwsh):

```pwsh
curl.exe -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs `
   | dotnet run - -o - -p TargetFramework=net10.0 -p OutputType=Exe `
   | dotnet run -
```

You can also add SDKs or NuGet packages in the same way:

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs \
   | dotnet run - -o - \
         --sdk "Microsoft.NET.Sdk.Web" \
         -p TargetFramework=net10.0 -p OutputType=Exe -p LangVersion=preview \
         --nuget "Spectre.Console@0.49.1" \
   | dotnet run -
```

### Notes and caveats

- Security: You are executing streamed code. Only run from sources you trust. Consider pinning to a specific commit/tag by hosting a fixed URL.
- PowerShell tip: Use `curl.exe` (the native curl) to avoid the `curl` alias to `Invoke-WebRequest` which returns objects instead of raw bytes.
- Requirements: .NET SDK 10.0 or later with File‑Based App support and internet access for the first stage.
- Persisting output: If you want to keep the generated file, replace the middle `-o -` with a path (e.g., `-o ./Program.cs`) and then run it separately.

## Command options

- `-o, --output <path>`: Output file path. If omitted, the tool chooses the first available name in the current directory like `Program1.cs`, `Program2.cs`, … Passing `-` writes to STDOUT.
- `--force`: Overwrite the output file if it already exists (default: disabled).
- `-sh, --shebang`: Emit a Unix shebang line (`#!/usr/bin/env dotnet`) at the top.
- `--sdk <name>`: Add one or more SDKs (can be specified multiple times). `Microsoft.NET.Sdk` is treated as the default and is omitted in the output. Each value becomes a `#:sdk <name>` line.
- `-p, --property KEY=VALUE`: Override project properties (can be specified multiple times). Example: `-p TargetFramework=net10.0 -p OutputType=Exe`.
- `--disable-aot`: Force-add `PublishAot=False`.
- `--nuget <pkg@ver>`: Add `#:package <pkg@ver>` lines (can be specified multiple times). Example: `--nuget "Newtonsoft.Json@13.0.3"`.

Exit codes

- `2` on errors such as invalid output path or overwrite refusal.

## What the generated file looks like

The tool emits the following sections separated by one or two blank lines (skipped if empty):

1. Optional: shebang line
   - `#!/usr/bin/env dotnet`

2. SDK declarations
   - e.g., `#:sdk Microsoft.NET.Sdk.Web`

3. Project properties
   - e.g., `#:property TargetFramework=net10.0`

4. NuGet package references
   - e.g., `#:package Spectre.Console@0.49.1`

5. Top-level program code (top-level statements)
   - Default: `Console.WriteLine("Hello, World!");`

> Note: `Microsoft.NET.Sdk` is considered the default SDK and won’t be included even if you pass it to `--sdk`.

## Tips and caveats

- On Windows, the shebang line does not affect execution; it’s primarily useful on Linux (WSL)/macOS.
- Minimal required properties like `TargetFramework` and `OutputType` are not added automatically—pass them via `-p` as needed.
- Use `-o -` to write to STDOUT for piping.
- The tool won’t overwrite existing files unless `--force` is specified.

## NuGet tool packaging steps (summary)

Follow the comments in the source as-is:

1. Remove the existing `projects` directory if present. (The convert command does not overwrite existing directories.)
2. `dotnet project convert .\dotnet_fba.cs -o .\projects`
3. `dotnet pack .\projects`
4. Check the output in `projects/nupkg/*.nupkg`.

Then install the global tool from the local feed:

```bash
dotnet tool install --global dotnet-fba --add-source ./projects/nupkg
```

```pwsh
dotnet tool install --global dotnet-fba --add-source .\projects\nupkg
```

## CI/CD with GitHub Actions

This repo includes two workflows under `.github/workflows`:

- `CI` (ci.yml): On every push and PR, builds the package from the file-based project and uploads the resulting `.nupkg` as a build artifact. You can download it from the Action run summary in GitHub.
- `Publish` (publish.yml): On tag push, publishes to NuGet depending on the tag naming convention below.

Publish rules (by tag name):

- `preview/vX.Y.Z` → Publishes a prerelease package with version `X.Y.Z-preview` to NuGet.
- `release/vX.Y.Z` → Publishes a stable package with version `X.Y.Z` to NuGet.

Requirements for publishing:

- Create a repository secret `NUGET_API_KEY` containing your NuGet.org API key.

Tagging examples (pwsh):

```pwsh
# Preview publish (creates version 1.0.1-preview on NuGet)
git tag preview/v1.0.1
git push origin preview/v1.0.1

# Release publish (creates version 1.0.2 on NuGet)
git tag release/v1.0.2
git push origin release/v1.0.2
```

How the CI build works:

1. Converts `dotnet_fba.cs` into a regular project with `dotnet project convert` into `./projects`.
2. Packs with `dotnet pack` into `./artifacts/nupkg`.
3. Uploads `*.nupkg` as an artifact so you can download and test locally.

## License

This repository is licensed under the terms in the `LICENSE` file.
