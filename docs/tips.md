# Tips and Caveats

- On Windows, the shebang line does not affect execution; it’s primarily useful on Linux (WSL)/macOS.
- Minimal required properties like `TargetFramework` and `OutputType` are not added automatically—pass them via `-p` as needed.
- Use `-o -` to write to STDOUT for piping.
- The tool won’t overwrite existing files unless `--force` is specified.

## Aspire AppHost notes

- Requires a container runtime running locally—Podman is recommended; Docker Desktop also works.
- The sample defines containers for MySQL, phpMyAdmin, and WordPress and exposes ports (e.g., 3306, 8080, 8081). Ensure these ports are free.
- Volumes are persisted across runs; if you change DB user/password, you may need to recreate volumes.

## Stdio MCP server notes

- The example template exposes a stdio transport; when run via `dotnet run`, it typically serves a single client session.
- The sample `IpAddressTool` calls ipify to return the public IP address; it requires internet access and may return IPv4 or IPv6 depending on the `ipv6` parameter.
- Use an MCP-compatible client to connect over stdio. Consider hardening and adding authentication for production scenarios.
