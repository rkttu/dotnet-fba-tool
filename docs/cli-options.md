# Command Options

- `-o, --output <path>`: Output file path. If omitted, the tool chooses the first available name in the current directory like `Program1.cs`, `Program2.cs`, … Passing `-` writes to STDOUT.
- `--force`: Overwrite the output file if it already exists (default: disabled).
- `-sh, --shebang`: Emit a Unix shebang line (`#!/usr/bin/env dotnet`) at the top.
- `--sdk <name>`: Add one or more SDKs (can be specified multiple times). `Microsoft.NET.Sdk` is treated as the default and is omitted in the output. Each value becomes a `#:sdk <name>` line.
- `-p, --property KEY=VALUE`: Override project properties (can be specified multiple times). Example: `-p TargetFramework=net10.0 -p OutputType=Exe`.
- `--disable-aot`: Force-add `PublishAot=False`.
- `--nuget <pkg@ver>`: Add `#:package <pkg@ver>` lines (can be specified multiple times). Example: `--nuget "Newtonsoft.Json@13.0.3"`.

Exit codes

- `2` on errors such as invalid output path or overwrite refusal.
