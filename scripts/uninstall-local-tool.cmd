@echo off
setlocal enabledelayedexpansion

REM uninstall-local-tool.cmd
REM Uninstalls the global tool installed as 'dotnet-fba'. If not installed, exits gracefully.

set "TOOL_ID=dotnet-fba"

where dotnet >nul 2>&1 || (
  echo ERROR: dotnet CLI not found in PATH.
  exit /b 2
)

echo Checking if '%TOOL_ID%' is installed...
for /f "usebackq tokens=1" %%A in (`dotnet tool list -g ^| findstr /i "%TOOL_ID%"`) do (
  set INSTALLED=1
)

if not defined INSTALLED (
  echo Tool '%TOOL_ID%' is not installed. Nothing to do.
  exit /b 0
)

echo Uninstalling '%TOOL_ID%'...
 dotnet tool uninstall --global %TOOL_ID%
if errorlevel 1 (
  echo ERROR: Failed to uninstall '%TOOL_ID%'.
  exit /b 2
)

echo Done.
exit /b 0
