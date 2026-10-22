# -------------------------------------------------------
# Helper script to reset the database to seeded baseline.
# Loads secrets, and then invokes curl against the special admin endpoint.
# Example usage: 
#   ./master-reset.ps1 localhost
#   ./master-reset.ps1 angular.mikegelldemo.tech
#   ./master-reset.ps1 https://angular.mikegelldemo.tech/

param(
    [string]$TargetUrl
)

# Ensure we are in location where the current ps1 is physically stored.
Set-Location $PSScriptRoot

# If no target URL is provided, print usage and exit
if ([string]::IsNullOrWhiteSpace($TargetUrl)) {
    Write-Host ""
    Write-Host "This script will reset the database to seeded baseline. Pass localhost or the site url." -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor DarkGray
    Write-Host "  ./master-reset.ps1 localhost" -ForegroundColor DarkGray
    Write-Host "  ./master-reset.ps1 angular.mikegelldemo.tech" -ForegroundColor DarkGray
    Write-Host "  ./master-reset.ps1 https://angular.mikegelldemo.tech/" -ForegroundColor DarkGray
    Write-Host ""
    exit 1
}

# Load secrets into the local powershell enviro session...
& "C:\sensitive\awasp-gcp\load-secrets.ps1"

# If arg is 'localhost' then target the correct local angular backend endpoint
if ($TargetUrl -eq 'localhost') {
    $baseUrl = "https://localhost:7225"
} else {
    # Ensure it starts with http:// or https:// (defaulting to https if missing)
    if ($TargetUrl -notmatch '^(http://|https://)') {
        $TargetUrl = "https://$TargetUrl"
    }
    # Trim any trailing slash to prevent double slashes before the endpoint path
    $baseUrl = $TargetUrl.TrimEnd('/')
}

$endpoint = "$baseUrl/api/admin-special/master-reset"

# Build JSON body using the session environment variable
$body = @{ password = $env:Authentication__VIP__Password } | ConvertTo-Json

# Invoke the master-reset endpoint via curl
Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"


