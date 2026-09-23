# Helper script to start the angular frontend on localhost.
# Loads secrets and then kicks off ng serve.

# Example usage
# ./ng-serve--hmr.ps1

# Ensure we are in location where the current ps1 is physically stored.
Set-Location $PSScriptRoot

# Load secrets into the local powershell enviro session...
& "C:\sensitive\awasp-gcp\load-secrets.ps1"

# Kick off ng serve with HMR
Write-Host 'ng serve --hmr' -ForegroundColor Magenta
ng serve --hmr
