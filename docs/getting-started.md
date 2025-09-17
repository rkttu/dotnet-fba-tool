# Getting Started

This guide helps you build the tool locally and install it from a local feed.

## Install from NuGet (Global Tool)

Install stable:

```bash
dotnet tool install --global dotnet-fba
```

Include prerelease:

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

## Build locally (optional)

Bash/Zsh:

```bash
rm -rf ./projects

dotnet project convert ./dotnet_fba.cs -o ./projects

dotnet pack ./projects

ls ./projects/nupkg/*.nupkg
```

PowerShell (pwsh):

```pwsh
Remove-Item -Recurse -Force .\projects -ErrorAction SilentlyContinue

dotnet project convert .\dotnet_fba.cs -o .\projects

dotnet pack .\projects

Get-ChildItem .\projects\nupkg\*.nupkg
```

## Quick run with DNX (no install)

If you're on .NET 10+, you can run the tool directly from NuGet without installing:

```bash
dnx -y dotnet-fba -- --help
```

Include prerelease:

```bash
dnx -y --prerelease dotnet-fba -- --help
```

More details and examples: see [Streaming](./streaming.md#dnx-run-directly-from-nuget-net-10).

Install from local package:

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
