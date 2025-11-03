# =============================================================================
# Azure Infrastructure Deployment Script
# =============================================================================
# 使用 Bicep 部署 Semantic Kernel Agentic Framework 基礎設施

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('dev', 'staging', 'prod')]
    [string]$Environment,

    [Parameter(Mandatory=$false)]
    [string]$Location = 'eastus',

    [Parameter(Mandatory=$false)]
    [switch]$WhatIf,

    [Parameter(Mandatory=$false)]
    [switch]$ValidateOnly,

    [Parameter(Mandatory=$false)]
    [string]$PostgresAdminPassword
)

# =============================================================================
# 設置
# =============================================================================

$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Azure Infrastructure Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Location: $Location" -ForegroundColor Yellow
Write-Host "WhatIf Mode: $($WhatIf.IsPresent)" -ForegroundColor Yellow
Write-Host "Validate Only: $($ValidateOnly.IsPresent)" -ForegroundColor Yellow
Write-Host ""

# =============================================================================
# 檢查 Azure CLI
# =============================================================================

Write-Host "[1/6] Checking Azure CLI..." -ForegroundColor Green
try {
    $azVersion = az version --output json | ConvertFrom-Json
    Write-Host "✅ Azure CLI Version: $($azVersion.'azure-cli')" -ForegroundColor Gray
} catch {
    Write-Host "❌ Azure CLI not found. Please install: https://aka.ms/install-azure-cli" -ForegroundColor Red
    exit 1
}

# =============================================================================
# 檢查 Bicep
# =============================================================================

Write-Host "[2/6] Checking Bicep..." -ForegroundColor Green
try {
    $bicepVersion = az bicep version
    Write-Host "✅ Bicep Version: $bicepVersion" -ForegroundColor Gray
} catch {
    Write-Host "⚠️  Bicep not found. Installing..." -ForegroundColor Yellow
    az bicep install
    Write-Host "✅ Bicep installed successfully" -ForegroundColor Green
}

# =============================================================================
# Azure 登入檢查
# =============================================================================

Write-Host "[3/6] Checking Azure login status..." -ForegroundColor Green
$account = az account show --output json 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "❌ Not logged in to Azure. Running 'az login'..." -ForegroundColor Yellow
    az login
    $account = az account show --output json | ConvertFrom-Json
}
Write-Host "✅ Logged in as: $($account.user.name)" -ForegroundColor Gray
Write-Host "   Subscription: $($account.name) ($($account.id))" -ForegroundColor Gray
Write-Host ""

# =============================================================================
# 參數處理
# =============================================================================

Write-Host "[4/6] Preparing deployment parameters..." -ForegroundColor Green

$parametersFile = "parameters/$Environment.bicepparam"
if (-not (Test-Path $parametersFile)) {
    Write-Host "❌ Parameters file not found: $parametersFile" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Using parameters file: $parametersFile" -ForegroundColor Gray

# PostgreSQL 密碼處理
if (-not $PostgresAdminPassword) {
    if ($Environment -eq 'dev') {
        Write-Host "⚠️  Using default dev password for PostgreSQL" -ForegroundColor Yellow
        $PostgresAdminPassword = "P@ssw0rd123!"
    } else {
        Write-Host "❌ PostgreSQL admin password required for $Environment environment" -ForegroundColor Red
        Write-Host "   Usage: ./deploy.ps1 -Environment $Environment -PostgresAdminPassword '<secure-password>'" -ForegroundColor Yellow
        exit 1
    }
}

# =============================================================================
# 部署驗證
# =============================================================================

Write-Host "[5/6] Validating deployment..." -ForegroundColor Green

$deploymentName = "skagentic-$Environment-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

$validateCommand = "az deployment sub validate " +
    "--name $deploymentName " +
    "--location $Location " +
    "--template-file main.bicep " +
    "--parameters $parametersFile " +
    "--parameters postgresAdminPassword='$PostgresAdminPassword' " +
    "--output json"

Write-Host "Running validation..." -ForegroundColor Gray
$validation = Invoke-Expression $validateCommand | ConvertFrom-Json

if ($validation.error) {
    Write-Host "❌ Validation failed:" -ForegroundColor Red
    Write-Host $validation.error.message -ForegroundColor Red
    exit 1
}

Write-Host "✅ Validation successful" -ForegroundColor Green
Write-Host ""

if ($ValidateOnly) {
    Write-Host "✅ Validation complete (--validate-only flag set)" -ForegroundColor Green
    exit 0
}

# =============================================================================
# 部署確認
# =============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Deployment Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Name: $deploymentName" -ForegroundColor Yellow
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Location: $Location" -ForegroundColor Yellow
Write-Host "Template: main.bicep" -ForegroundColor Yellow
Write-Host "Parameters: $parametersFile" -ForegroundColor Yellow
Write-Host ""

if (-not $WhatIf) {
    $confirmation = Read-Host "Proceed with deployment? (yes/no)"
    if ($confirmation -ne 'yes') {
        Write-Host "❌ Deployment cancelled by user" -ForegroundColor Yellow
        exit 0
    }
}

# =============================================================================
# 執行部署
# =============================================================================

Write-Host "[6/6] Deploying infrastructure..." -ForegroundColor Green
Write-Host "⏳ This may take 15-30 minutes..." -ForegroundColor Yellow
Write-Host ""

$deployCommand = "az deployment sub create " +
    "--name $deploymentName " +
    "--location $Location " +
    "--template-file main.bicep " +
    "--parameters $parametersFile " +
    "--parameters postgresAdminPassword='$PostgresAdminPassword' " +
    "--output json"

if ($WhatIf) {
    $deployCommand += " --what-if"
}

$startTime = Get-Date
$deployment = Invoke-Expression $deployCommand | ConvertFrom-Json
$endTime = Get-Date
$duration = $endTime - $startTime

# =============================================================================
# 部署結果
# =============================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Deployment Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Status: $($deployment.properties.provisioningState)" -ForegroundColor Green
Write-Host "Duration: $($duration.ToString('hh\:mm\:ss'))" -ForegroundColor Gray
Write-Host ""

if ($deployment.properties.provisioningState -eq 'Succeeded') {
    Write-Host "✅ Deployment successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Outputs:" -ForegroundColor Yellow
    $deployment.properties.outputs.PSObject.Properties | ForEach-Object {
        Write-Host "  $($_.Name): $($_.Value.value)" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "  1. Update .env file with connection strings" -ForegroundColor Gray
    Write-Host "  2. Store secrets in Azure Key Vault" -ForegroundColor Gray
    Write-Host "  3. Configure CI/CD pipelines" -ForegroundColor Gray
    Write-Host "  4. Run database migrations" -ForegroundColor Gray
} else {
    Write-Host "❌ Deployment failed" -ForegroundColor Red
    Write-Host "Error: $($deployment.properties.error.message)" -ForegroundColor Red
    exit 1
}
