#! /usr/bin/pwsh
param(
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $VersionSuffix = "",
    [Parameter(Mandatory = $false)][string] $OutputPath = "",
    [Parameter(Mandatory = $false)][switch] $SkipTests
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$solutionPath = Split-Path $MyInvocation.MyCommand.Definition
$solutionFile = Join-Path $solutionPath "PseudoLocalizer.sln"
$sdkFile = Join-Path $solutionPath "global.json"

$packageProjects = @(
	(Join-Path $solutionPath "PseudoLocalizer.Core\PseudoLocalizer.Core.csproj"),
	(Join-Path $solutionPath "PseudoLocalizer.Humanizer\PseudoLocalizer.Humanizer.csproj"),
	(Join-Path $solutionPath "PseudoLocalize\PseudoLocalize.csproj")
)

$testProjects = @(
    (Join-Path $solutionPath "PseudoLocalizer.Core.Tests\PseudoLocalizer.Core.Tests.csproj"),
    (Join-Path $solutionPath "PseudoLocalize.Tests\PseudoLocalize.Tests.csproj")
)

$dotnetVersion = (Get-Content $sdkFile | Out-String | ConvertFrom-Json).sdk.version

if ($OutputPath -eq "") {
    $OutputPath = Join-Path "$(Convert-Path "$PSScriptRoot")" "artifacts"
}

$installDotNetSdk = $false;

if (($null -eq (Get-Command "dotnet" -ErrorAction SilentlyContinue)) -and ($null -eq (Get-Command "dotnet.exe" -ErrorAction SilentlyContinue))) {
    Write-Host "The .NET SDK is not installed."
    $installDotNetSdk = $true
}
else {
    Try {
        $installedDotNetVersion = (dotnet --version 2>&1 | Out-String).Trim()
    }
    Catch {
        $installedDotNetVersion = "?"
    }

    if ($installedDotNetVersion -ne $dotnetVersion) {
        Write-Host "The required version of the .NET SDK is not installed. Expected $dotnetVersion."
        $installDotNetSdk = $true
    }
}

if ($installDotNetSdk -eq $true) {

    $env:DOTNET_INSTALL_DIR = Join-Path "$(Convert-Path "$PSScriptRoot")" ".dotnetcli"
    $sdkPath = Join-Path $env:DOTNET_INSTALL_DIR "sdk\$dotnetVersion"

    if (!(Test-Path $sdkPath)) {
        if (!(Test-Path $env:DOTNET_INSTALL_DIR)) {
            mkdir $env:DOTNET_INSTALL_DIR | Out-Null
        }
        [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor "Tls12"

        if (($PSVersionTable.PSVersion.Major -ge 6) -And !$IsWindows) {
            $installScript = Join-Path $env:DOTNET_INSTALL_DIR "install.sh"
            Invoke-WebRequest "https://dot.net/v1/dotnet-install.sh" -OutFile $installScript -UseBasicParsing
            chmod +x $installScript
            & $installScript --version "$dotnetVersion" --install-dir "$env:DOTNET_INSTALL_DIR" --no-path
        }
        else {
            $installScript = Join-Path $env:DOTNET_INSTALL_DIR "install.ps1"
            Invoke-WebRequest "https://dot.net/v1/dotnet-install.ps1" -OutFile $installScript -UseBasicParsing
            & $installScript -Version "$dotnetVersion" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath
        }
    }
}
else {
    $env:DOTNET_INSTALL_DIR = Split-Path -Path (Get-Command dotnet).Path
}

$dotnet = Join-Path "$env:DOTNET_INSTALL_DIR" "dotnet"

if ($installDotNetSdk -eq $true) {
    $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
}

function DotNetBuild {
    param([string]$Project)

    if ($VersionSuffix) {
        & $dotnet build $Project --output $OutputPath --configuration $Configuration --version-suffix "$VersionSuffix"
    }
    else {
        & $dotnet build $Project --output $OutputPath --configuration $Configuration
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

function DotNetPack {
    param([string]$Project)

    if ($VersionSuffix) {
        & $dotnet pack $Project --output (Join-Path $OutputPath "packages") --configuration $Configuration --version-suffix "$VersionSuffix" --include-symbols --include-source
    }
    else {
        & $dotnet pack $Project --output (Join-Path $OutputPath "packages") --configuration $Configuration --include-symbols --include-source
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}

function DotNetTest {
    param([string]$Project)

    & $dotnet test $Project --output $OutputPath --no-build

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test failed with exit code $LASTEXITCODE"
    }
}

Write-Host "Building solution..." -ForegroundColor Green
DotNetBuild $solutionFile

Write-Host "Creating packages..." -ForegroundColor Green
ForEach ($packageProject in $packageProjects) {
    DotNetPack $packageProject
}

Write-Host "Running tests..." -ForegroundColor Green
ForEach ($testProject in $testProjects) {
    DotNetTest $testProject
}
