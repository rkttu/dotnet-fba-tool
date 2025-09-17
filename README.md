# dotnet-fba-tool

File-based app source scaffolding tool for .NET 10.0 and later

## Overview

`dotnet-fba-tool` helps you scaffold a “File‑Based App (FBA)” in a single C# file that can include SDK declarations, project properties, and package references. It generates a runnable template file. You can later convert it to a regular project with `dotnet project convert`, or keep using it as-is in your build pipeline.

This repository itself is authored in a file-based style (`dotnet_fba.cs`). After converting, you can `dotnet pack` to create a .NET Global Tool.

## Quick start

Create a one-file app scaffold and run with minimal properties:

```bash
dotnet-fba -o ./hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

More examples and options:

- Basic usage and variations: see [docs/usage.md](./docs/usage.md)
- CLI options reference: see [docs/cli-options.md](./docs/cli-options.md)

## Run directly from the web

Stream and run without cloning or installing:

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs | dotnet run - -o - | dotnet run -
```

Details and variations: see [docs/streaming.md](./docs/streaming.md)

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

This repository is licensed under the terms in the `LICENSE` file.
