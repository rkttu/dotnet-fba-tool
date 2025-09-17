# What the Generated File Looks Like

The tool emits the following sections separated by one or two blank lines (skipped if empty):

1. Optional: shebang line
   - `#!/usr/bin/env dotnet`

2. SDK declarations
   - e.g., `#:sdk Microsoft.NET.Sdk.Web`

3. Project properties
   - e.g., `#:property TargetFramework=net10.0`

4. NuGet package references
   - e.g., `#:package Spectre.Console@0.49.1`

5. Top-level program code (top-level statements)
   - Default: `Console.WriteLine("Hello, World!");`

Note: All SDKs you specify are emitted. If you include `Microsoft.NET.Sdk`, it will be present as `#:sdk Microsoft.NET.Sdk`.

Merging and overrides

- SDKs: Duplicates are removed; all unique values are emitted in `#:sdk` lines.
- Properties: If the same key appears multiple times (from template + CLI `-p`), the last value wins, and is emitted once as `#:property KEY=VALUE`.
- Packages: If a package is specified multiple times (from template + CLI `--nuget`), the last `@version` wins and is emitted once as `#:package NAME@VERSION`.
