# =============================================================================
# Health Check Script for Local Development Environment
# =============================================================================
# 檢查所有必需的 Docker 服務是否正常運行

Write-Host "🔍 Semantic Kernel Agentic Platform - Health Check" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

$allHealthy = $true

# =============================================================================
# PostgreSQL Health Check
# =============================================================================
Write-Host "Checking PostgreSQL..." -NoNewline
try {
    $pgResult = docker exec sk-postgres pg_isready -U postgres 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host " ✅ Healthy" -ForegroundColor Green
        Write-Host "  └─ $pgResult" -ForegroundColor Gray
    } else {
        Write-Host " ❌ Unhealthy" -ForegroundColor Red
        Write-Host "  └─ $pgResult" -ForegroundColor Gray
        $allHealthy = $false
    }
} catch {
    Write-Host " ❌ Not running" -ForegroundColor Red
    Write-Host "  └─ Container not found" -ForegroundColor Gray
    $allHealthy = $false
}

Write-Host ""

# =============================================================================
# Redis Health Check
# =============================================================================
Write-Host "Checking Redis..." -NoNewline
try {
    # Redis 可能有密碼保護，先嘗試無密碼 ping
    $redisResult = docker exec sk-redis redis-cli ping 2>&1

    if ($redisResult -match "PONG") {
        Write-Host " ✅ Healthy (No auth)" -ForegroundColor Green
        Write-Host "  └─ Response: PONG" -ForegroundColor Gray
    } elseif ($redisResult -match "NOAUTH") {
        # 有密碼保護，嘗試使用環境變數中的密碼
        Write-Host " ✅ Healthy (Auth required)" -ForegroundColor Green
        Write-Host "  └─ Redis is running but requires authentication" -ForegroundColor Gray
    } else {
        Write-Host " ❌ Unhealthy" -ForegroundColor Red
        Write-Host "  └─ $redisResult" -ForegroundColor Gray
        $allHealthy = $false
    }
} catch {
    Write-Host " ❌ Not running" -ForegroundColor Red
    Write-Host "  └─ Container not found" -ForegroundColor Gray
    $allHealthy = $false
}

Write-Host ""

# =============================================================================
# Qdrant Health Check
# =============================================================================
Write-Host "Checking Qdrant..." -NoNewline
try {
    $qdrantResponse = Invoke-RestMethod -Uri "http://localhost:6333/" -Method Get -ErrorAction Stop

    if ($qdrantResponse.version) {
        Write-Host " ✅ Healthy" -ForegroundColor Green
        Write-Host "  └─ Version: $($qdrantResponse.version)" -ForegroundColor Gray
        Write-Host "  └─ Title: $($qdrantResponse.title)" -ForegroundColor Gray
    } else {
        Write-Host " ❌ Unhealthy" -ForegroundColor Red
        $allHealthy = $false
    }
} catch {
    Write-Host " ❌ Not accessible" -ForegroundColor Red
    Write-Host "  └─ $($_.Exception.Message)" -ForegroundColor Gray
    $allHealthy = $false
}

Write-Host ""

# =============================================================================
# Docker Compose Status
# =============================================================================
Write-Host "Docker Compose Services:" -ForegroundColor Cyan
docker-compose ps

Write-Host ""

# =============================================================================
# Summary
# =============================================================================
if ($allHealthy) {
    Write-Host "=================================================" -ForegroundColor Green
    Write-Host "✅ All services are healthy!" -ForegroundColor Green
    Write-Host "=================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Service URLs:" -ForegroundColor Cyan
    Write-Host "  PostgreSQL: localhost:5432" -ForegroundColor Gray
    Write-Host "  Redis:      localhost:6379" -ForegroundColor Gray
    Write-Host "  Qdrant:     http://localhost:6333" -ForegroundColor Gray
    Write-Host ""
    Write-Host "You can now start development! 🚀" -ForegroundColor Green
    exit 0
} else {
    Write-Host "=================================================" -ForegroundColor Red
    Write-Host "❌ Some services are unhealthy!" -ForegroundColor Red
    Write-Host "=================================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "  1. Check if Docker Desktop is running" -ForegroundColor Gray
    Write-Host "  2. Run: docker-compose up -d" -ForegroundColor Gray
    Write-Host "  3. Check logs: docker-compose logs" -ForegroundColor Gray
    Write-Host "  4. Restart services: docker-compose restart" -ForegroundColor Gray
    Write-Host ""
    exit 1
}
