<#
install-local-tool.ps1
Builds a local .nupkg from the file-based project and installs/updates the global tool from the local feed.
Requirements: .NET SDK 10.0+ with `dotnet project convert` command available.
#>
[CmdletBinding()]
param(
  [switch]$Force
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = (Resolve-Path "$PSScriptRoot/..").Path
Push-Location $Root
try {
    $ProjectFile = 'dotnet_fba.cs'
    $OutDir      = 'projects'
    $NugetDir    = Join-Path $OutDir 'nupkg'
    $ToolId      = 'dotnet-fba'

    # 1) Clean projects directory
    if (Test-Path $OutDir) {
        Write-Host "Removing existing '$OutDir' directory..."
        Remove-Item -Recurse -Force $OutDir -ErrorAction SilentlyContinue
    }

    # 2) Convert file-based project
    Write-Host 'Converting file-based project to regular project...'
    dotnet project convert ./$ProjectFile -o ./$OutDir

    # 3) Pack to create nupkg
    Write-Host 'Packing project...'
    dotnet pack ./$OutDir

    # 4) Verify nupkg exists
    if (-not (Test-Path $NugetDir)) {
        throw "Could not find nupkg directory at '$NugetDir'"
    }

    Write-Host 'Generated packages:'
    Get-ChildItem "$NugetDir/*.nupkg" | ForEach-Object { $_.Name }

    # 5) Install/Update global tool from local feed
    Write-Host 'Installing or updating global tool from local feed...'
    $InstallSucceeded = $false
    try {
        dotnet tool install --global $ToolId --add-source "./$NugetDir"
        $InstallSucceeded = $true
    } catch {
        Write-Host 'Install may have failed (possibly already installed). Trying update...'
        dotnet tool update --global $ToolId --add-source "./$NugetDir"
        $InstallSucceeded = $true
    }

    if ($InstallSucceeded) {
        Write-Host "Done. You can now run the tool: $ToolId"
    } else {
        throw 'Both install and update failed.'
    }
}
finally {
    Pop-Location
}
