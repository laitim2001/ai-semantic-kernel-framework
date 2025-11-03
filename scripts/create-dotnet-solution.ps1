# =============================================================================
# .NET Solution Scaffold Script
# =============================================================================
# 自動創建完整的 Clean Architecture 專案結構

param(
    [string]$SolutionName = "AIAgentPlatform",
    [string]$Framework = "net9.0"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " .NET Solution Scaffold" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Solution Name: $SolutionName" -ForegroundColor Yellow
Write-Host "Framework: $Framework" -ForegroundColor Yellow
Write-Host ""

# =============================================================================
# 驗證 .NET SDK
# =============================================================================
Write-Host "🔍 Checking .NET SDK..." -NoNewline
try {
    $dotnetVersion = dotnet --version
    Write-Host " ✅ Found version $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host " ❌ .NET SDK not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install .NET SDK first:" -ForegroundColor Yellow
    Write-Host "  winget install Microsoft.DotNet.SDK.9" -ForegroundColor Gray
    Write-Host ""
    Write-Host "For more details, see: DEVELOPMENT-SETUP.md" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# =============================================================================
# 創建目錄結構
# =============================================================================
Write-Host "📁 Creating directory structure..." -ForegroundColor Cyan

$directories = @(
    "src",
    "tests"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "  ✅ Created $dir" -ForegroundColor Green
    } else {
        Write-Host "  ⏭️  $dir already exists" -ForegroundColor Gray
    }
}

Write-Host ""

# =============================================================================
# 創建解決方案
# =============================================================================
Write-Host "🏗️ Creating solution..." -ForegroundColor Cyan

Push-Location src

if (Test-Path "$SolutionName.sln") {
    Write-Host "  ⚠️  Solution already exists, skipping..." -ForegroundColor Yellow
} else {
    dotnet new sln -n $SolutionName
    Write-Host "  ✅ Created $SolutionName.sln" -ForegroundColor Green
}

Write-Host ""

# =============================================================================
# 創建 Domain Layer
# =============================================================================
Write-Host "📦 Creating Domain Layer..." -ForegroundColor Cyan

$domainProject = "$SolutionName.Domain"
if (!(Test-Path $domainProject)) {
    dotnet new classlib -n $domainProject -f $Framework
    dotnet sln add "$domainProject/$domainProject.csproj"

    # 刪除默認 Class1.cs
    Remove-Item "$domainProject/Class1.cs" -ErrorAction SilentlyContinue

    # 創建目錄結構
    $domainDirs = @("Entities", "ValueObjects", "Events", "Interfaces", "Services", "Exceptions", "Common")
    foreach ($dir in $domainDirs) {
        New-Item -ItemType Directory -Path "$domainProject/$dir" -Force | Out-Null
    }

    Write-Host "  ✅ Created $domainProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $domainProject already exists" -ForegroundColor Gray
}

Write-Host ""

# =============================================================================
# 創建 Application Layer
# =============================================================================
Write-Host "📦 Creating Application Layer..." -ForegroundColor Cyan

$applicationProject = "$SolutionName.Application"
if (!(Test-Path $applicationProject)) {
    dotnet new classlib -n $applicationProject -f $Framework
    dotnet sln add "$applicationProject/$applicationProject.csproj"

    Remove-Item "$applicationProject/Class1.cs" -ErrorAction SilentlyContinue

    # 創建目錄結構
    $appDirs = @("Commands", "Queries", "DTOs", "Interfaces", "Mappings", "Validators", "Behaviors")
    foreach ($dir in $appDirs) {
        New-Item -ItemType Directory -Path "$applicationProject/$dir" -Force | Out-Null
    }

    # 添加專案引用
    Push-Location $applicationProject
    dotnet add reference "../$domainProject/$domainProject.csproj"
    Pop-Location

    Write-Host "  ✅ Created $applicationProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $applicationProject already exists" -ForegroundColor Gray
}

Write-Host ""

# =============================================================================
# 創建 Infrastructure Layer
# =============================================================================
Write-Host "📦 Creating Infrastructure Layer..." -ForegroundColor Cyan

$infrastructureProject = "$SolutionName.Infrastructure"
if (!(Test-Path $infrastructureProject)) {
    dotnet new classlib -n $infrastructureProject -f $Framework
    dotnet sln add "$infrastructureProject/$infrastructureProject.csproj"

    Remove-Item "$infrastructureProject/Class1.cs" -ErrorAction SilentlyContinue

    # 創建目錄結構
    $infraDirs = @("Persistence", "Repositories", "Services", "Configuration")
    foreach ($dir in $infraDirs) {
        New-Item -ItemType Directory -Path "$infrastructureProject/$dir" -Force | Out-Null
    }

    # 添加專案引用
    Push-Location $infrastructureProject
    dotnet add reference "../$domainProject/$domainProject.csproj"
    dotnet add reference "../$applicationProject/$applicationProject.csproj"
    Pop-Location

    Write-Host "  ✅ Created $infrastructureProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $infrastructureProject already exists" -ForegroundColor Gray
}

Write-Host ""

# =============================================================================
# 創建 API Layer
# =============================================================================
Write-Host "📦 Creating API Layer..." -ForegroundColor Cyan

$apiProject = "$SolutionName.API"
if (!(Test-Path $apiProject)) {
    dotnet new webapi -n $apiProject -f $Framework
    dotnet sln add "$apiProject/$apiProject.csproj"

    # 創建目錄結構
    $apiDirs = @("Controllers", "Middleware", "Filters", "Extensions")
    foreach ($dir in $apiDirs) {
        New-Item -ItemType Directory -Path "$apiProject/$dir" -Force | Out-Null
    }

    # 添加專案引用
    Push-Location $apiProject
    dotnet add reference "../$applicationProject/$applicationProject.csproj"
    dotnet add reference "../$infrastructureProject/$infrastructureProject.csproj"
    Pop-Location

    Write-Host "  ✅ Created $apiProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $apiProject already exists" -ForegroundColor Gray
}

Write-Host ""

# =============================================================================
# 創建 Shared Library
# =============================================================================
Write-Host "📦 Creating Shared Library..." -ForegroundColor Cyan

$sharedProject = "$SolutionName.Shared"
if (!(Test-Path $sharedProject)) {
    dotnet new classlib -n $sharedProject -f $Framework
    dotnet sln add "$sharedProject/$sharedProject.csproj"

    Remove-Item "$sharedProject/Class1.cs" -ErrorAction SilentlyContinue

    # 創建目錄結構
    $sharedDirs = @("Constants", "Extensions", "Helpers", "Models")
    foreach ($dir in $sharedDirs) {
        New-Item -ItemType Directory -Path "$sharedProject/$dir" -Force | Out-Null
    }

    Write-Host "  ✅ Created $sharedProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $sharedProject already exists" -ForegroundColor Gray
}

Pop-Location

Write-Host ""

# =============================================================================
# 創建測試專案
# =============================================================================
Write-Host "🧪 Creating Test Projects..." -ForegroundColor Cyan

Push-Location tests

# Unit Tests
$unitTestProject = "$SolutionName.UnitTests"
if (!(Test-Path $unitTestProject)) {
    dotnet new xunit -n $unitTestProject -f $Framework
    dotnet sln ../src/$SolutionName.sln add "$unitTestProject/$unitTestProject.csproj"

    # 添加專案引用
    Push-Location $unitTestProject
    dotnet add reference "../../src/$domainProject/$domainProject.csproj"
    dotnet add reference "../../src/$applicationProject/$applicationProject.csproj"
    Pop-Location

    Write-Host "  ✅ Created $unitTestProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $unitTestProject already exists" -ForegroundColor Gray
}

# Integration Tests
$integrationTestProject = "$SolutionName.IntegrationTests"
if (!(Test-Path $integrationTestProject)) {
    dotnet new xunit -n $integrationTestProject -f $Framework
    dotnet sln ../src/$SolutionName.sln add "$integrationTestProject/$integrationTestProject.csproj"

    # 添加專案引用
    Push-Location $integrationTestProject
    dotnet add reference "../../src/$apiProject/$apiProject.csproj"
    dotnet add reference "../../src/$infrastructureProject/$infrastructureProject.csproj"
    Pop-Location

    Write-Host "  ✅ Created $integrationTestProject" -ForegroundColor Green
} else {
    Write-Host "  ⏭️  $integrationTestProject already exists" -ForegroundColor Gray
}

Pop-Location

Write-Host ""

# =============================================================================
# 安裝 NuGet 套件
# =============================================================================
Write-Host "📦 Installing NuGet packages..." -ForegroundColor Cyan
Write-Host "  (This may take a few minutes...)" -ForegroundColor Gray
Write-Host ""

# Infrastructure - EF Core + PostgreSQL
Write-Host "  Installing EF Core packages..." -NoNewline
Push-Location "src/$infrastructureProject"
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0 --no-restore | Out-Null
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0 --no-restore | Out-Null
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0 --no-restore | Out-Null
Pop-Location
Write-Host " ✅" -ForegroundColor Green

# Application - MediatR + FluentValidation
Write-Host "  Installing MediatR packages..." -NoNewline
Push-Location "src/$applicationProject"
dotnet add package MediatR --version 12.4.0 --no-restore | Out-Null
dotnet add package FluentValidation --version 11.11.0 --no-restore | Out-Null
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.11.0 --no-restore | Out-Null
Pop-Location
Write-Host " ✅" -ForegroundColor Green

# API - Swagger + Authentication + Logging
Write-Host "  Installing API packages..." -NoNewline
Push-Location "src/$apiProject"
dotnet add package Swashbuckle.AspNetCore --version 7.2.0 --no-restore | Out-Null
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0 --no-restore | Out-Null
dotnet add package Serilog.AspNetCore --version 8.0.3 --no-restore | Out-Null
Pop-Location
Write-Host " ✅" -ForegroundColor Green

# Unit Tests - Moq + FluentAssertions
Write-Host "  Installing test packages..." -NoNewline
Push-Location "tests/$unitTestProject"
dotnet add package Moq --version 4.20.72 --no-restore | Out-Null
dotnet add package FluentAssertions --version 7.0.0 --no-restore | Out-Null
dotnet add package xunit.runner.visualstudio --version 2.8.2 --no-restore | Out-Null
Pop-Location
Write-Host " ✅" -ForegroundColor Green

# Integration Tests - ASP.NET Testing + Testcontainers
Write-Host "  Installing integration test packages..." -NoNewline
Push-Location "tests/$integrationTestProject"
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.0 --no-restore | Out-Null
dotnet add package Testcontainers.PostgreSql --version 4.1.0 --no-restore | Out-Null
Pop-Location
Write-Host " ✅" -ForegroundColor Green

Write-Host ""

# =============================================================================
# Restore and Build
# =============================================================================
Write-Host "🔨 Restoring and building solution..." -ForegroundColor Cyan

Push-Location src
dotnet restore
$buildResult = dotnet build --no-restore 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✅ Build successful!" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Build completed with warnings" -ForegroundColor Yellow
}

Pop-Location

Write-Host ""

# =============================================================================
# 摘要
# =============================================================================
Write-Host "========================================" -ForegroundColor Green
Write-Host " ✅ Solution scaffold completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📂 Project Structure:" -ForegroundColor Cyan
Write-Host "  src/$SolutionName.sln" -ForegroundColor Gray
Write-Host "    ├── $domainProject (Core business logic)" -ForegroundColor Gray
Write-Host "    ├── $applicationProject (Use cases + CQRS)" -ForegroundColor Gray
Write-Host "    ├── $infrastructureProject (Database + External services)" -ForegroundColor Gray
Write-Host "    ├── $apiProject (REST API)" -ForegroundColor Gray
Write-Host "    └── $sharedProject (Common utilities)" -ForegroundColor Gray
Write-Host ""
Write-Host "  tests/" -ForegroundColor Gray
Write-Host "    ├── $unitTestProject (Domain + Application tests)" -ForegroundColor Gray
Write-Host "    └── $integrationTestProject (API + Infrastructure tests)" -ForegroundColor Gray
Write-Host ""
Write-Host "🎯 Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Open solution: cd src && code $SolutionName.sln" -ForegroundColor Yellow
Write-Host "  2. Read User Story: docs/user-stories/modules/module-01-agent-creation.md" -ForegroundColor Yellow
Write-Host "  3. Start implementing: Begin with Domain Layer (Agent Entity)" -ForegroundColor Yellow
Write-Host ""
Write-Host "🚀 Run API:" -ForegroundColor Cyan
Write-Host "  cd src/$apiProject && dotnet run" -ForegroundColor Yellow
Write-Host ""
Write-Host "📖 Documentation:" -ForegroundColor Cyan
Write-Host "  - DEVELOPMENT-SETUP.md (Setup guide)" -ForegroundColor Yellow
Write-Host "  - NEXT-ACTIONS.md (Development roadmap)" -ForegroundColor Yellow
Write-Host "  - docs/technical-implementation/3-project-structure/ (Architecture)" -ForegroundColor Yellow
Write-Host ""
