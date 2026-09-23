param (
    [switch]$Up,
    [switch]$Down
)

$ErrorActionPreference = "Stop"

if (($Up -and $Down) -or (-not $Up -and -not $Down)) {
    Write-Error "Please specify exactly one flag: -Up or -Down"
    exit 1
}

# Configuration Variables
$ProjectId       = "awasp-gcp"
$Region          = "australia-southeast2" # Melbourne, Australia

# Resource Names
$BucketName      = "awasp-gcp-userpics"
$RepoName        = "awasp-gcp-docker-repo"
$ServiceName     = "awasp-app"
$SqlInstanceName = "awasp-sql-instance"
$DatabaseName    = "awasp_gcp_db"

# Secrets File (outside version control)
$SecretsFile = "C:\sensitive\awasp-gcp\secrets.md"

# Helper to read DB Password from secrets.md
$DbPassword = $null
if (Test-Path $SecretsFile) {
    $secretsContent = Get-Content $SecretsFile
    $match = $secretsContent | Select-String -Pattern "^DB_PASSWORD=(.*)$"
    if ($match) {
        $DbPassword = $match.Matches.Groups[1].Value.Trim()
    }
}

Write-Host "========================================" -ForegroundColor Cyan
if ($Up) { Write-Host " GCP Provisioning Script (-Up)" -ForegroundColor Cyan }
if ($Down) { Write-Host " GCP Provisioning Script (-Down)" -ForegroundColor Cyan }
Write-Host " Project ID: $ProjectId" -ForegroundColor Cyan
Write-Host " Secrets File: $SecretsFile" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Ensure gcloud is pointing to the right project
Write-Host "`nSetting active GCP project to $ProjectId..."
& cmd /c "gcloud config set project $ProjectId 2>NUL" | Out-Null

if ($Up) {
    Write-Host "`n[UP] Enabling required GCP APIs..." -ForegroundColor Green
    & cmd /c "gcloud services enable compute.googleapis.com run.googleapis.com sqladmin.googleapis.com artifactregistry.googleapis.com storage.googleapis.com cloudbuild.googleapis.com firebase.googleapis.com firebasehosting.googleapis.com 2>NUL" | Out-Null

    Write-Host "`nGranting VPC Network permissions to Cloud Run Service Agent..." -ForegroundColor Green
    $projectNum = (& cmd /c "gcloud projects describe $ProjectId --format=value(projectNumber) 2>NUL").Trim()
    if (![string]::IsNullOrWhiteSpace($projectNum)) {
        & cmd /c "gcloud projects add-iam-policy-binding $ProjectId --member=serviceAccount:service-$projectNum@serverless-robot-prod.iam.gserviceaccount.com --role=roles/compute.networkUser --condition=None 2>NUL" | Out-Null
    }

    Write-Host "`n[UP] Configuring Firebase Hosting site target..." -ForegroundColor Green
    # Ensure Firebase hosting site target is linked so deployments never prompt interactively
    & cmd /c "npx firebase-tools target:apply hosting awasp-gcp awasp-gcp --project $ProjectId 2>NUL" | Out-Null

    Write-Host "`n[UP] Checking Artifact Registry repository '$RepoName'..." -ForegroundColor Green
    
    # Run silently and check exit code
    & cmd /c "gcloud artifacts repositories describe $RepoName --location=$Region --format=value(name) 2>NUL" | Out-Null
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Creating Artifact Registry repository '$RepoName'..."
        gcloud artifacts repositories create $RepoName `
            --repository-format=docker `
            --location=$Region `
            --description="Docker repository for AwASP"
    } else {
        Write-Host "Repository '$RepoName' already exists."
    }

    Write-Host "`n[UP] Checking Cloud Storage bucket '$BucketName'..." -ForegroundColor Green
    
    & cmd /c "gcloud storage ls gs://$BucketName 2>NUL" | Out-Null

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Creating Storage Bucket '$BucketName'..."
        gcloud storage buckets create "gs://$BucketName" --location=$Region --uniform-bucket-level-access

        Write-Host "Making Storage Bucket publicly readable (for FE app to load images)..."
        gcloud storage buckets add-iam-policy-binding "gs://$BucketName" `
            --member="allUsers" `
            --role="roles/storage.objectViewer"
            
        Write-Host "Configuring CORS on the bucket..."
        $corsJson = '[{"origin": ["*"],"method": ["GET", "OPTIONS"],"responseHeader": ["Content-Type"],"maxAgeSeconds": 3600}]'
        $corsJson | Out-File -FilePath "$env:TEMP\cors.json" -Encoding UTF8
        gcloud storage buckets update "gs://$BucketName" --cors-file="$env:TEMP\cors.json"
        Remove-Item "$env:TEMP\cors.json"
    } else {
        Write-Host "Storage Bucket '$BucketName' already exists."
    }

    Write-Host "`n[UP] Checking Cloud SQL instance '$SqlInstanceName'..." -ForegroundColor Green
    
    & cmd /c "gcloud sql instances describe $SqlInstanceName --format=value(name) 2>NUL" | Out-Null

    if ($LASTEXITCODE -ne 0) {
        if ([string]::IsNullOrWhiteSpace($DbPassword)) {
            Write-Error "Cannot provision database: DB_PASSWORD not found in $SecretsFile. Please add a line like 'DB_PASSWORD=YourPassword' and run again."
            exit 1
        }
        Write-Host "Creating Cloud SQL instance '$SqlInstanceName' (this will take ~10-15 minutes)..."
        gcloud sql instances create $SqlInstanceName `
            --database-version=SQLSERVER_2019_EXPRESS `
            --tier=db-custom-2-7680 `
            --region=$Region `
            --root-password="$DbPassword" `
              --authorized-networks="0.0.0.0/0"

        # GCP Cloud Run dynamically allocates IPs from a massive, shared Google pool.
        # In an enterprise environment we would need Private Service Connect, or NAT Gateway.
        # For this non-commercial portfolio project, I will allow all IPs (0.0.0.0/0)
        # and rely on strict TLS, with very strong runtime password.

        Write-Host "Creating database '$DatabaseName' inside instance..."
        gcloud sql databases create $DatabaseName --instance=$SqlInstanceName
    } else {
        Write-Host "Cloud SQL instance '$SqlInstanceName' already exists."
    }

    Write-Host "`nProvisioning UP complete!" -ForegroundColor Cyan
}

if ($Down) {
    Write-Host "`n[DOWN] Deleting Cloud Run service '$ServiceName'..." -ForegroundColor DarkGray
    & cmd /c "gcloud run services delete $ServiceName --region=$Region --quiet 2>NUL" | Out-Null
    if ($LASTEXITCODE -eq 0) { Write-Host "Service deleted." } else { Write-Host "Service not found or already deleted." }

    Write-Host "`n[DOWN] Deleting Artifact Registry repository '$RepoName'..." -ForegroundColor DarkGray
    & cmd /c "gcloud artifacts repositories delete $RepoName --location=$Region --quiet 2>NUL" | Out-Null
    if ($LASTEXITCODE -eq 0) { Write-Host "Repository deleted." } else { Write-Host "Repository not found or already deleted." }

    Write-Host "`n[DOWN] Deleting Storage Bucket '$BucketName' (including all files)..." -ForegroundColor DarkGray
    & cmd /c "gcloud storage rm --recursive gs://$BucketName 2>NUL" | Out-Null
    if ($LASTEXITCODE -eq 0) { Write-Host "Bucket deleted." } else { Write-Host "Bucket not found or already deleted." }

    Write-Host "`n[DOWN] Deleting Cloud SQL instance '$SqlInstanceName'..." -ForegroundColor DarkGray
    & cmd /c "gcloud sql instances delete $SqlInstanceName --quiet 2>NUL" | Out-Null
    if ($LASTEXITCODE -eq 0) { Write-Host "SQL instance deleted." } else { Write-Host "SQL instance not found or already deleted." }

    Write-Host "`nTeardown DOWN complete!" -ForegroundColor Cyan
}
