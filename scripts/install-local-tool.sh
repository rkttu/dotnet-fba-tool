#!/usr/bin/env bash
set -euo pipefail

# install-local-tool.sh
# Builds a local .nupkg from the file-based project and installs/updates the global tool from the local feed.
# Requirements: .NET SDK 10.0+ with `dotnet project convert` command available.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="${SCRIPT_DIR}/.."
cd "$ROOT"

PROJECT_FILE="dotnet_fba.cs"
OUT_DIR="projects"
NUPKG_DIR="${OUT_DIR}/nupkg"
TOOL_ID="dotnet-fba"

# 1) Clean projects directory
if [ -d "$OUT_DIR" ]; then
  echo "Removing existing '$OUT_DIR' directory..."
  rm -rf "$OUT_DIR"
fi

# 2) Convert file-based project
echo "Converting file-based project to regular project..."
dotnet project convert "./${PROJECT_FILE}" -o "./${OUT_DIR}"

# 3) Pack to create nupkg
echo "Packing project..."
dotnet pack "./${OUT_DIR}"

# 4) Verify nupkg exists
if [ ! -d "$NUPKG_DIR" ]; then
  echo "ERROR: Could not find nupkg directory at '$NUPKG_DIR'" >&2
  exit 2
fi

echo "Generated packages:"
ls -1 "${NUPKG_DIR}"/*.nupkg || { echo "ERROR: No nupkg files found." >&2; exit 2; }

# 5) Install/Update global tool from local feed
set +e

dotnet tool install --global "$TOOL_ID" --add-source "./${NUPKG_DIR}"
INSTALL_RC=$?
if [ $INSTALL_RC -ne 0 ]; then
  echo "Install may have failed (possibly already installed). Trying update..."
  dotnet tool update --global "$TOOL_ID" --add-source "./${NUPKG_DIR}"
  UPDATE_RC=$?
  if [ $UPDATE_RC -ne 0 ]; then
    echo "ERROR: Both install and update failed." >&2
    exit 2
  fi
fi

set -e

echo "Done. You can now run the tool: $TOOL_ID"
