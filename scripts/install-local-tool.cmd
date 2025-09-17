@echo off
setlocal enabledelayedexpansion

REM install-local-tool.cmd
REM Builds a local .nupkg from the file-based project and installs/updates the global tool from the local feed.
REM Requirements: .NET SDK 10.0+ with `dotnet project convert` command available.

set "ROOT=%~dp0.."
pushd "%ROOT%" >nul 2>&1

set "PROJECT_FILE=dotnet_fba.cs"
set "OUT_DIR=projects"
set "NUPKG_DIR=%OUT_DIR%\nupkg"
set "TOOL_ID=dotnet-fba"

REM 1) Clean projects directory
if exist "%OUT_DIR%" (
  echo Removing existing "%OUT_DIR%" directory...
  rmdir /s /q "%OUT_DIR%"
)

REM 2) Convert file-based project
echo Converting file-based project to regular project...
 dotnet project convert .\%PROJECT_FILE% -o .\%OUT_DIR%
if errorlevel 1 (
  echo ERROR: Conversion failed.
  popd & exit /b 2
)

REM 3) Pack to create nupkg
echo Packing project...
 dotnet pack .\%OUT_DIR%
if errorlevel 1 (
  echo ERROR: Pack failed.
  popd & exit /b 2
)

REM 4) Find nupkg
if not exist "%NUPKG_DIR%" (
  echo ERROR: Could not find nupkg directory at "%NUPKG_DIR%".
  popd & exit /b 2
)

echo Listing generated packages:
dir /b "%NUPKG_DIR%\*.nupkg" || (
  echo ERROR: No nupkg files found.
  popd & exit /b 2
)

REM 5) Install/Update global tool from local feed
where dotnet >nul 2>&1 || (
  echo ERROR: dotnet CLI not found in PATH.
  popd & exit /b 2
)

echo Attempting to install the global tool from local feed...
 dotnet tool install --global %TOOL_ID% --add-source .\%NUPKG_DIR%
if errorlevel 1 (
  echo Install may have failed (possibly already installed). Trying update...
  dotnet tool update --global %TOOL_ID% --add-source .\%NUPKG_DIR%
  if errorlevel 1 (
    echo ERROR: Both install and update failed.
    popd & exit /b 2
  )
)

echo Done. You can now run the tool: %TOOL_ID%

popd >nul 2>&1
exit /b 0
