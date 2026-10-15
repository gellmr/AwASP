# -------------------------------------------------------
# Helper script to reset the database to seeded baseline.
# Loads secrets, and then invokes curl against the special admin endpoint.
# Example usage: 
#   ./master-reset.ps1
#   ./master-reset.ps1 angular.mikegelldemo.tech
#   ./master-reset.ps1 https://angular.mikegelldemo.tech/

param(
    [string]$TargetUrl
)

# Ensure we are in location where the current ps1 is physically stored.
Set-Location $PSScriptRoot

# Load secrets into the local powershell enviro session...
& "C:\sensitive\awasp-gcp\load-secrets.ps1"

# Determine base URL
if ([string]::IsNullOrWhiteSpace($TargetUrl)) {
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
Invoke-RestMethod -Uri "$baseUrl/api/admin-special/master-reset" -Method Post -Body $body -ContentType "application/json"


