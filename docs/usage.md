# Usage

## How to run (installed global tool)

After installing the tool globally, you can run it as either of the following:

```bash
dotnet-fba --help
# or
dotnet fba --help
```

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

## Template presets

Select a preset with `-t|--template`.

Console (default):

```bash
dotnet-fba -o ./Program.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

WebHost:

```bash
dotnet-fba -t WebHost -o ./WebApp.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

GenericHost:

```bash
dotnet-fba -t GenericHost -o ./Hosted.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

Aspire AppHost (requires a container runtime; Podman recommended, Docker also works):

```bash
dotnet-fba -t AspireAppHost -o ./AppHost.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

PowerShell (pwsh) uses backticks for line continuation; paths use `\` instead of `/`.

## Piping to STDOUT

```bash
dotnet-fba -o - -p TargetFramework=net10.0 > ./Program.cs
```

```pwsh
dotnet-fba -o - -p TargetFramework=net10.0 | Set-Content .\Program.cs -NoNewline
```
