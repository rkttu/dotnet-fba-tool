# Tips and Caveats

- On Windows, the shebang line does not affect execution; it’s primarily useful on Linux (WSL)/macOS.
- Minimal required properties like `TargetFramework` and `OutputType` are not added automatically—pass them via `-p` as needed.
- Use `-o -` to write to STDOUT for piping.
- The tool won’t overwrite existing files unless `--force` is specified.

## Aspire AppHost notes

- Requires a container runtime running locally—Podman is recommended; Docker Desktop also works.
- The sample defines containers for MySQL, phpMyAdmin, and WordPress and exposes ports (e.g., 3306, 8080, 8081). Ensure these ports are free.
- Volumes are persisted across runs; if you change DB user/password, you may need to recreate volumes.
