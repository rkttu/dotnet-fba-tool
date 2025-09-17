# dotnet-fba-tool

[![NuGet](https://img.shields.io/nuget/v/dotnet-fba?logo=nuget&label=NuGet)](https://www.nuget.org/packages/dotnet-fba)
[![CI](https://github.com/rkttu/dotnet-fba-tool/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/rkttu/dotnet-fba-tool/actions/workflows/ci.yml)
[![Publish](https://github.com/rkttu/dotnet-fba-tool/actions/workflows/publish.yml/badge.svg)](https://github.com/rkttu/dotnet-fba-tool/actions/workflows/publish.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE)

> This project is an experimental tool, and we have decided to discontinue development after confirming that the dotnet new-based project template is more suitable for its intended purpose. For educational purposes only, this code is archived. For a working project, please refer to the rkttu/dotnet-fba-templates repository.

File-based app source scaffolding tool for .NET 10.0 and later

## Overview

`dotnet-fba-tool` helps you scaffold a “File‑Based App (FBA)” in a single C# file that can include SDK declarations, project properties, and package references. It generates a runnable template file. You can later convert it to a regular project with `dotnet project convert`, or keep using it as-is in your build pipeline.

This repository itself is authored in a file-based style (`dotnet_fba.cs`). After converting, you can `dotnet pack` to create a .NET Global Tool.

## Quickstart

- Install as a .NET Global Tool (recommended):

```bash
dotnet tool install --global dotnet-fba
dotnet-fba --help
# or alias
dotnet fba --help
```

- Run without installing (DNX, .NET 10+):

```bash
dnx -y dotnet-fba -- --help
```

- Stream and run (zero-install):

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs | dotnet run - -o - | dotnet run -
```

- First scaffold example (after installing the tool):

```bash
dotnet-fba -o ./Program.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

## Templates

`dotnet-fba-tool` provides ready-to-use templates you can select with `-t|--template`:

- Console (default): Minimal console app that prints "Hello, World!".
- GenericHost: Uses Microsoft.Extensions.Hosting to build and run a generic host with console logging.
- WebHost: Minimal ASP.NET Core Web app (`Microsoft.NET.Sdk.Web`) with a single `GET /` endpoint.
- AspireAppHost: App Host template using .NET Aspire (adds `Aspire.AppHost.Sdk@9.4.2` and package `Aspire.Hosting.AppHost@9.4.2`) with example containers (MySQL, phpMyAdmin, WordPress). Requires a container runtime—Podman recommended (Docker also works).
- StdioMcpServer: Minimal Model Context Protocol server over stdio (adds `ModelContextProtocol@0.3.0-preview.4`) with a sample tool `IpAddressTool` that returns the public IP address. Intended for MCP-compatible clients; running via `dotnet run` typically serves a single client at a time.

Example:

```bash
# Console (default)
dotnet-fba -o ./Program.cs -p TargetFramework=net10.0 -p OutputType=Exe

# WebHost
dotnet-fba -t WebHost -o ./WebApp.cs -p TargetFramework=net10.0 -p OutputType=Exe

# Aspire AppHost (container runtime required; Podman recommended)
dotnet-fba -t AspireAppHost -o ./AppHost.cs -p TargetFramework=net10.0 -p OutputType=Exe

# Stdio MCP server (stdio transport)
dotnet-fba -t StdioMcpServer -o ./McpServer.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

## Quick start

Create a one-file app scaffold and run with minimal properties:

```bash
dotnet-fba -o ./hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

More examples and options:

- Basic usage and variations: see [docs/usage.md](./docs/usage.md)
- CLI options reference: see [docs/cli-options.md](./docs/cli-options.md)
- Templates overview and examples: see [docs/usage.md#template-presets]

## Install as a .NET Global Tool

Install from NuGet:

```bash
dotnet tool install --global dotnet-fba
```

Include prerelease versions:

```bash
dotnet tool install --global dotnet-fba --prerelease
```

Update/Uninstall:

```bash
dotnet tool update    --global dotnet-fba
dotnet tool uninstall --global dotnet-fba
```

Run after install:

```bash
dotnet-fba --help
# or
dotnet fba --help
```

## Run directly from the web

Stream and run without cloning or installing:

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs | dotnet run - -o - | dotnet run -
```

Details and variations: see [docs/streaming.md](./docs/streaming.md)

## Run with DNX (NuGet, .NET 10+)

Run the tool directly from NuGet without installing a global tool.

- Latest stable:

```bash
dnx -y dotnet-fba -- --help
```

- Include prerelease versions:

```bash
dnx -y --prerelease dotnet-fba -- --help
```

Pass options after `--` just like the installed tool, for example:

```bash
dnx -y dotnet-fba -- -o ./Program.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

Notes:

- Requires .NET SDK 10.0 or later (DNX).
- Use `--prerelease` if the NuGet package version you want is a prerelease.
- You can pin versions and configure feeds; see `dnx --help` for advanced usage.
- Security: as with any streamed/remote execution, run packages from sources you trust.
- Templates: you can pass `-t WebHost`, `-t GenericHost`, or `-t AspireAppHost` after `--` to choose a preset (default: Console).
- Templates: you can pass `-t WebHost`, `-t GenericHost`, `-t AspireAppHost`, or `-t StdioMcpServer` after `--` to choose a preset (default: Console).

## Helper scripts

Automate local build/pack and global tool install/update from this repo:

- Install/update: `./scripts/install-local-tool.*`
- Uninstall: `./scripts/uninstall-local-tool.*`

See details: [docs/scripts.md](./docs/scripts.md)

### Notes and caveats

- Security: You are executing streamed code. Only run from sources you trust. Consider pinning to a specific commit/tag by hosting a fixed URL.
- PowerShell tip: Use `curl.exe` (the native curl) to avoid the `curl` alias to `Invoke-WebRequest` which returns objects instead of raw bytes.
- Requirements: .NET SDK 10.0 or later with File‑Based App support and internet access for the first stage.
- Persisting output: If you want to keep the generated file, replace the middle `-o -` with a path (e.g., `-o ./Program.cs`) and then run it separately.

## Documentation

- Getting Started (local build/install): [docs/getting-started.md](./docs/getting-started.md)
- Usage examples: [docs/usage.md](./docs/usage.md)
- CLI options: [docs/cli-options.md](./docs/cli-options.md)
- Streaming (zero-install): [docs/streaming.md](./docs/streaming.md)
- Generated output reference: [docs/generated-output.md](./docs/generated-output.md)
- Helper scripts: [docs/scripts.md](./docs/scripts.md)
- Tips and caveats: [docs/tips.md](./docs/tips.md)

## NuGet tool packaging (summary)

Short version: convert → pack → install from local feed. See [docs/getting-started.md](./docs/getting-started.md) for full commands.

## Helper scripts for local development

To make local development and testing faster, this repository includes helper scripts that automate the local build, pack, and installation of the tool from the generated local feed. They replicate the manual steps above and add safe uninstall commands.

Install or update the tool from this repo (automated):

- CMD (Windows):

```bat
.\scripts\install-local-tool.cmd
```

- PowerShell (pwsh):

```pwsh
.\scripts\install-local-tool.ps1
```

- Bash/Zsh (Linux/macOS/WSL):

```bash
chmod +x ./scripts/install-local-tool.sh
./scripts/install-local-tool.sh
```

Uninstall the tool (automated):

- CMD (Windows):

```bat
.\scripts\uninstall-local-tool.cmd
```

- PowerShell (pwsh):

```pwsh
.\scripts\uninstall-local-tool.ps1
```

- Bash/Zsh (Linux/macOS/WSL):

```bash
chmod +x ./scripts/uninstall-local-tool.sh
./scripts/uninstall-local-tool.sh
```

What the install script does:

- Cleans the `projects` directory if present.
- Converts the file-based project (`dotnet_fba.cs`) to a regular project under `./projects`.
- Runs `dotnet pack` to create a local `.nupkg` under `./projects/nupkg`.
- Installs the global tool from the local feed; if already installed, it attempts an update.

Notes:

- Requirements: .NET SDK 10.0+ and the `dotnet project convert` command available.
- Uninstall scripts are safe to run even if the tool is not installed—they exit without error.
- After installing a global tool, you may need to open a new shell session so your PATH updates are recognized (depending on your environment).

## CI/CD

See [docs/ci-cd.md](./docs/ci-cd.md) for CI workflow and publishing rules.

## License

Licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

