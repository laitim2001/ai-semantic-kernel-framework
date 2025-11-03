# =============================================================================
# Docker Services Connection Test Script
# =============================================================================
# 測試 PostgreSQL, Redis, Qdrant 連接狀態

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Docker Services Connection Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 測試 PostgreSQL
Write-Host "[1/3] Testing PostgreSQL Connection..." -ForegroundColor Yellow
try {
    $pgTest = docker exec sk-postgres pg_isready -U postgres
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL: Connected successfully" -ForegroundColor Green
        Write-Host "    - Host: localhost:5432" -ForegroundColor Gray
        Write-Host "    - Database: semantic_kernel" -ForegroundColor Gray
        Write-Host "    - Status: $pgTest" -ForegroundColor Gray
    } else {
        Write-Host "❌ PostgreSQL: Connection failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ PostgreSQL: Test failed - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 測試 Redis
Write-Host "[2/3] Testing Redis Connection..." -ForegroundColor Yellow
try {
    $redisTest = docker exec sk-redis redis-cli -a redis123 ping 2>&1
    if ($redisTest -match "PONG") {
        Write-Host "✅ Redis: Connected successfully" -ForegroundColor Green
        Write-Host "    - Host: localhost:6379" -ForegroundColor Gray
        Write-Host "    - Status: PONG" -ForegroundColor Gray

        # 獲取 Redis 信息
        $redisInfo = docker exec sk-redis redis-cli -a redis123 INFO server 2>&1 | Select-String "redis_version"
        Write-Host "    - $redisInfo" -ForegroundColor Gray
    } else {
        Write-Host "❌ Redis: Connection failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Redis: Test failed - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 測試 Qdrant
Write-Host "[3/3] Testing Qdrant Connection..." -ForegroundColor Yellow
try {
    $qdrantTest = Invoke-RestMethod -Uri "http://localhost:6333/healthz" -Method Get -TimeoutSec 5
    if ($qdrantTest) {
        Write-Host "✅ Qdrant: Connected successfully" -ForegroundColor Green
        Write-Host "    - HTTP: localhost:6333" -ForegroundColor Gray
        Write-Host "    - GRPC: localhost:6334" -ForegroundColor Gray

        # 獲取 Qdrant 信息
        $qdrantInfo = Invoke-RestMethod -Uri "http://localhost:6333" -Method Get
        Write-Host "    - Version: $($qdrantInfo.version)" -ForegroundColor Gray
        Write-Host "    - Title: $($qdrantInfo.title)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Qdrant: Connection failed - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 測試總結
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Connection Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 檢查容器健康狀態
Write-Host "Container Health Status:" -ForegroundColor Yellow
docker-compose ps --format "table {{.Name}}`t{{.Status}}`t{{.Ports}}"
Write-Host ""

Write-Host "✨ All connection tests completed!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Update Agent Service appsettings.json with connection strings" -ForegroundColor Gray
Write-Host "  2. Run database migrations: dotnet ef migrations add InitialCreate" -ForegroundColor Gray
Write-Host "  3. Start developing Sprint 1 features" -ForegroundColor Gray
