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

Note: `Microsoft.NET.Sdk` is considered the default SDK and won’t be included even if you pass it to `--sdk`.
