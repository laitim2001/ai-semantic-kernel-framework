# Docker 開發環境

Sprint 0 Day 2 完成的 Docker Compose 開發環境配置。

## 服務清單

| 服務 | 鏡像 | 端口 | 用途 |
|------|------|------|------|
| **PostgreSQL** | postgres:16-alpine | 5432 | 主數據庫 |
| **Redis** | redis:7-alpine | 6379 | 緩存與會話 |
| **Qdrant** | qdrant/qdrant:v1.7.4 | 6333 (HTTP)<br>6334 (GRPC) | 向量數據庫 |
| **pgAdmin** | dpage/pgadmin4 | 5050 | PostgreSQL 管理工具 (可選) |

## 快速開始

### 1. 啟動所有服務

```bash
# 啟動核心服務 (PostgreSQL, Redis, Qdrant)
docker-compose up -d

# 啟動包含 pgAdmin 管理工具
docker-compose --profile tools up -d
```

### 2. 檢查服務狀態

```bash
# 查看容器狀態
docker-compose ps

# 查看服務日誌
docker-compose logs -f
```

### 3. 測試連接

```bash
# 使用 PowerShell 測試腳本
powershell -File infrastructure/docker/test-connections.ps1

# 或手動測試
docker exec sk-postgres pg_isready -U postgres
docker exec sk-redis redis-cli -a redis123 ping
curl http://localhost:6333/healthz
```

## 服務連接信息

### PostgreSQL

```
Host: localhost
Port: 5432
Database: semantic_kernel
Username: postgres
Password: postgres123 (dev only)

Connection String (EF Core):
Host=localhost;Port=5432;Database=semantic_kernel;Username=postgres;Password=postgres123
```

### Redis

```
Host: localhost
Port: 6379
Password: redis123 (dev only)

Connection String:
localhost:6379,password=redis123
```

### Qdrant

```
HTTP API: http://localhost:6333
GRPC API: localhost:6334
Dashboard: http://localhost:6333/dashboard

No authentication required in development
```

### pgAdmin (可選)

```
URL: http://localhost:5050
Email: admin@localhost.com
Password: admin123
```

## 數據持久化

所有服務數據都通過 Docker volumes 持久化:

- `postgres_data`: PostgreSQL 數據
- `redis_data`: Redis 數據
- `qdrant_data`: Qdrant 向量存儲
- `qdrant_snapshots`: Qdrant 備份快照
- `pgadmin_data`: pgAdmin 配置

## 常用命令

### 服務管理

```bash
# 啟動服務
docker-compose up -d

# 停止服務
docker-compose stop

# 重啟服務
docker-compose restart

# 停止並刪除容器 (保留數據)
docker-compose down

# 完全清理 (包括數據卷)
docker-compose down -v
```

### 日誌查看

```bash
# 所有服務日誌
docker-compose logs -f

# 特定服務日誌
docker-compose logs -f postgres
docker-compose logs -f redis
docker-compose logs -f qdrant
```

### 健康檢查

```bash
# 查看健康狀態
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Health}}"

# 等待所有服務健康
docker-compose up -d --wait
```

## 數據庫初始化

PostgreSQL 容器首次啟動時會自動執行初始化腳本:

- `01-init-database.sql`: 創建擴展、Schema、審計函數

查看初始化日誌:
```bash
docker-compose logs postgres | grep "Database initialization"
```

## 備份與恢復

### PostgreSQL 備份

```bash
# 備份數據庫
docker exec sk-postgres pg_dump -U postgres semantic_kernel > backup.sql

# 恢復數據庫
docker exec -i sk-postgres psql -U postgres semantic_kernel < backup.sql
```

### Qdrant 備份

```bash
# 創建快照
curl -X POST http://localhost:6333/collections/{collection_name}/snapshots

# 下載快照
curl http://localhost:6333/collections/{collection_name}/snapshots/{snapshot_name} -o snapshot.zip
```

## 故障排除

### 端口被占用

```bash
# Windows 查找占用進程
netstat -ano | findstr :5432
netstat -ano | findstr :6379

# 修改端口 (在 .env 文件)
POSTGRES_PORT=5433
REDIS_PORT=6380
```

### 容器無法啟動

```bash
# 查看詳細日誌
docker-compose logs <service-name>

# 檢查配置
docker-compose config

# 重新創建容器
docker-compose up -d --force-recreate
```

### 權限問題

```bash
# 刪除並重新創建數據卷
docker-compose down -v
docker-compose up -d
```

## 性能優化

### 資源限制

在 `docker-compose.yml` 中已配置健康檢查,可根據需要添加資源限制:

```yaml
deploy:
  resources:
    limits:
      cpus: '2.0'
      memory: 2G
```

### 網絡優化

使用專用網絡 `semantic-kernel-network` 確保服務間通信效率。

## 安全注意事項

⚠️ **本配置僅用於開發環境**

生產環境需要:
1. 修改所有預設密碼
2. 啟用 TLS/SSL
3. 配置防火牆規則
4. 使用 secrets 管理密碼
5. 啟用訪問日誌和審計

## 下一步

✅ Sprint 0 Day 2 完成

**Sprint 0 Day 3-4**: Azure 基礎設施部署
- 配置 Azure Bicep 模板
- 部署 Azure PostgreSQL
- 配置 Azure OpenAI
- 設置 Azure Container Registry

查看 `docs/technical-implementation/8-deployment-architecture/` 獲取詳細信息。
