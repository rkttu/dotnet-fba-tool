# Usage

## Basic usage

Create `hello.cs` and pass minimal properties:

Bash/Zsh:

```bash
dotnet-fba -o ./hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

PowerShell (pwsh):

```pwsh
dotnet-fba -o .\hello.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

## With packages, SDK, shebang

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

## Piping to STDOUT

```bash
dotnet-fba -o - -p TargetFramework=net10.0 > ./Program.cs
```

```pwsh
dotnet-fba -o - -p TargetFramework=net10.0 | Set-Content .\Program.cs -NoNewline
```
