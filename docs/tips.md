# Tips and Caveats

- On Windows, the shebang line does not affect execution; it’s primarily useful on Linux (WSL)/macOS.
- Minimal required properties like `TargetFramework` and `OutputType` are not added automatically—pass them via `-p` as needed.
- Use `-o -` to write to STDOUT for piping.
- The tool won’t overwrite existing files unless `--force` is specified.
