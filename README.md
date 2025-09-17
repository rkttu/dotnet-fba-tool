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

## License

This repository is licensed under the terms in the `LICENSE` file.
