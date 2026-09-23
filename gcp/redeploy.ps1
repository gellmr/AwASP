$ErrorActionPreference = "Stop"

# Configuration Variables
$ProjectId       = "awasp-gcp"
$Region          = "australia-southeast2" # Melbourne
$RepoName        = "awasp-gcp-docker-repo"
$ServiceName     = "awasp-app"
$SqlInstanceName = "awasp-sql-instance"
$DatabaseName    = "awasp_gcp_db"
$BucketName      = "awasp-gcp-userpics"

# Secrets File (not in repo)
$SecretsFile = "C:\sensitive\awasp-gcp\secrets.md"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " GCP Redeploy Script" -ForegroundColor Cyan
Write-Host " Project ID: $ProjectId" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1. Parse required secrets
Write-Host "`nReading secrets from $SecretsFile..."
$DbPassword = $null
$GoogleClientId = $null
$GoogleClientSecret = $null

if (-not (Test-Path $SecretsFile)) {
    Write-Error "Secrets file not found at $SecretsFile!"
    exit 1
}

$secretsContent = Get-Content $SecretsFile
$matchDb = $secretsContent | Select-String -Pattern "^DB_PASSWORD=(.*)$"
$matchGClientId = $secretsContent | Select-String -Pattern "^GOOGLE_CLIENT_ID=(.*)$"
$matchGSecret = $secretsContent | Select-String -Pattern "^GOOGLE_CLIENT_SECRET=(.*)$"

if ($matchDb) { $DbPassword = $matchDb.Matches.Groups[1].Value.Trim() }
if ($matchGClientId) { $GoogleClientId = $matchGClientId.Matches.Groups[1].Value.Trim() }
if ($matchGSecret) { $GoogleClientSecret = $matchGSecret.Matches.Groups[1].Value.Trim() }

if ([string]::IsNullOrWhiteSpace($DbPassword) -or [string]::IsNullOrWhiteSpace($GoogleClientId) -or [string]::IsNullOrWhiteSpace($GoogleClientSecret)) {
    Write-Error "Missing required secrets (DB_PASSWORD, GOOGLE_CLIENT_ID, or GOOGLE_CLIENT_SECRET) in $SecretsFile."
    exit 1
}

# 2. Build the database connection string
# 2. Fetch the SQL Public IP and build connection string
Write-Host "Fetching Cloud SQL public IP..."
$SqlIp = gcloud sql instances describe $SqlInstanceName --format="value(ipAddresses[0].ipAddress)"
if ([string]::IsNullOrWhiteSpace($SqlIp)) {
    Write-Error "Could not fetch SQL IP. Is the instance running?"
    exit 1
}
$ConnectionString = "Server=$SqlIp;Database=$DatabaseName;User Id=sqlserver;Password=$DbPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"

# 3. Create image tag
$ImageTag = "$Region-docker.pkg.dev/$ProjectId/$RepoName/awasp-image:latest"

Write-Host "`n[1/2] Submitting build to Google Cloud Build..." -ForegroundColor Green
$RootPath = Resolve-Path "$PSScriptRoot\.."
gcloud builds submit "$RootPath" `
  --config "$RootPath\gcp\cloudbuild.yaml" `
  --substitutions="_IMAGE_TAG=$ImageTag,_VITE_GOOGLE_CLIENT_ID=$GoogleClientId"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Cloud Build failed!"
    exit 1
}

Write-Host "`n[2/2] Deploying image to Cloud Run..." -ForegroundColor Green
# We deploy and pass all the necessary environment variables securely
gcloud run deploy $ServiceName --image $ImageTag --region $Region --allow-unauthenticated --set-env-vars="ConnectionStrings__StoreContext=$ConnectionString,Authentication__Google__ClientId=$GoogleClientId,Authentication__Google__ClientSecret=$GoogleClientSecret,GCP__StorageBucketName=$BucketName,RUN_MIGRATIONS=true"

Write-Host "[3/3] Updating Firebase Hosting proxy..."
# Ensure the proxy public folder exists so Firebase deploy doesn't fail
if (!(Test-Path "proxypub")) {
    New-Item -ItemType Directory -Force -Path (Join-Path $PSScriptRoot "proxypub") | Out-Null
}
$ConfigPath = Join-Path $PSScriptRoot "firebase.json"
& cmd /c "npx firebase-tools deploy --only hosting --config `"$ConfigPath`" --project awasp-gcp"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nRedeploy complete!" -ForegroundColor Cyan
    $Url = gcloud run services describe $ServiceName --region $Region --format="value(status.url)"
    Write-Host "Your app is live at: $Url" -ForegroundColor Green
} else {
    Write-Error "Cloud Run deploy failed!"
}

