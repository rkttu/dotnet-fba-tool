# Getting Started

This guide helps you build the tool locally and install it from a local feed.

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
