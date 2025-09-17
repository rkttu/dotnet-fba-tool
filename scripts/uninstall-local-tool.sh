#!/usr/bin/env bash
set -euo pipefail

# uninstall-local-tool.sh
# Uninstalls the global tool installed as 'dotnet-fba'. If not installed, exits gracefully.

TOOL_ID="dotnet-fba"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERROR: dotnet CLI not found in PATH." >&2
  exit 2
fi

echo "Checking if '$TOOL_ID' is installed..."
if dotnet tool list -g | grep -E "^${TOOL_ID}[[:space:]]" >/dev/null 2>&1; then
  echo "Uninstalling '$TOOL_ID'..."
  dotnet tool uninstall --global "$TOOL_ID"
  echo "Done."
else
  echo "Tool '$TOOL_ID' is not installed. Nothing to do."
fi
