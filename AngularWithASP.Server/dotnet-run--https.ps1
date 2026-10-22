# Helper script to start the .NET backend on localhost.
# Loads secrets and then kicks off dotnet run.

# Example usage
# ./dotnet-run--https.ps1
# ./dotnet-run--https.ps1 -UseDebug

[CmdletBinding()]
param (
  [switch]$UseDebug
)

# Ensure we are in location where the current ps1 is physically stored.
Set-Location $PSScriptRoot

# Load secrets into the local powershell enviro session...
& "C:\sensitive\awasp-gcp\load-secrets.ps1"

# Build the argument list dynamically based on the -UseDebug switch
$dotnetArgs = @('--launch-profile', 'https')

if ($UseDebug) {
  $dotnetArgs += '--configuration'
  $dotnetArgs += 'Debug'
  Write-Host "dotnet run $($dotnetArgs -join ' ')" -ForegroundColor Magenta
  dotnet run @dotnetArgs
} else {
  Write-Host 'dotnet run --launch-profile "https"' -ForegroundColor Magenta
  dotnet run --launch-profile "https"
}
