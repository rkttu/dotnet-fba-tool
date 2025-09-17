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
