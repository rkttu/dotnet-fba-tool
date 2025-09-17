# Run Directly from the Web (Zero-Install Stream)

Run the tool without cloning or installing by streaming the file-based source and executing it in-memory.

## One-liner

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

## How it works

1) Fetch tool source via `curl` to STDOUT.
2) `dotnet run -` compiles and executes from STDIN (File‑Based App), emit generated app to STDOUT with `-o -`.
3) Pipe into another `dotnet run -` to execute the generated app from STDIN.

## Customize

```bash
curl -sSL -o - https://rkttu.github.io/dotnet-fba-tool/dotnet_fba.cs \
   | dotnet run - -o - -p TargetFramework=net10.0 -p OutputType=Exe \
   | dotnet run -
```

Add SDKs or NuGet packages similarly.

## Notes

- Execute streamed code only from trusted sources.
- On PowerShell, prefer `curl.exe` over the `curl` alias.
- Requires .NET SDK 10.0+.

## DNX: Run directly from NuGet (.NET 10+)

If you're on .NET 10+, you can execute the published NuGet package without installing a global tool.

### Basics

- Latest stable:

```bash
dnx -y dotnet-fba -- --help
```

- Include prerelease versions:

```bash
dnx -y --prerelease dotnet-fba -- --help
```

### Passing options

Everything after `--` goes to the tool, e.g. generate a one-file app and run later:

```bash
dnx -y dotnet-fba -- -o ./Program.cs -p TargetFramework=net10.0 -p OutputType=Exe
```

### Version pinning and feeds

You can pin a specific version and use custom feeds; see `dnx --help` for details.

### PowerShell note

In PowerShell, the same commands work as-is. If mixing with curl pipelines, prefer `curl.exe` over the alias.

### Security

Run packages from sources you trust, especially when using `--prerelease`.
