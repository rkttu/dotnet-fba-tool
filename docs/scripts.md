# Helper Scripts for Local Development

This repository includes helper scripts to automate local build, pack, install/update, and uninstall.

## Install or update the tool

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

## Uninstall the tool

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

## Notes

- Requires .NET SDK 10.0+ and `dotnet project convert`.
- After installing a global tool, a new shell session may be needed for PATH updates.
