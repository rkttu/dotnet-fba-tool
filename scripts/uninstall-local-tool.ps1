<#
uninstall-local-tool.ps1
Uninstalls the global tool installed as 'dotnet-fba'. If not installed, exits gracefully.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$ToolId = 'dotnet-fba'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error 'dotnet CLI not found in PATH.'
    exit 2
}

Write-Host "Checking if '$ToolId' is installed..."
$installed = (& dotnet tool list -g) -match "^$ToolId\s"
if (-not $installed) {
    Write-Host "Tool '$ToolId' is not installed. Nothing to do."
    exit 0
}

Write-Host "Uninstalling '$ToolId'..."
dotnet tool uninstall --global $ToolId
Write-Host 'Done.'
