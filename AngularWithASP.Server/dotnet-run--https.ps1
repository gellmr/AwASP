# Helper script to start the .NET backend on localhost.
# Loads secrets and then kicks off dotnet run.

# Example usage
# ./dotnet-run--https.ps1

# Ensure we are in location where the current ps1 is physically stored.
Set-Location $PSScriptRoot

# Load secrets into the local powershell enviro session...
& "C:\sensitive\awasp-gcp\load-secrets.ps1"

# Kick off dotnet run with https
Write-Host 'dotnet run --launch-profile "https"' -ForegroundColor Magenta
dotnet run --launch-profile "https"
