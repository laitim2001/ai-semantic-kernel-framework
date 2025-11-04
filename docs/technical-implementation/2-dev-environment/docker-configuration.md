# Docker 配置指南

## 版本信息
- **文檔版本**: 1.0
- **最後更新**: 2025-01-01

## Docker Compose 配置

### 完整 docker-compose.yml

```yaml
version: '3.8'

services:
  # PostgreSQL 數據庫
  postgres:
    image: postgres:16-alpine
    container_name: aiworkflow-postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: aiworkflow
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres123}
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./scripts/init-db.sql:/docker-entrypoint-initdb.d/init.sql
    networks:
      - aiworkflow-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis 緩存
  redis:
    image: redis:7-alpine
    container_name: aiworkflow-redis
    restart: unless-stopped
    command: redis-server --requirepass ${REDIS_PASSWORD:-redis123}
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - aiworkflow-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  # MongoDB
  mongodb:
    image: mongo:7
    container_name: aiworkflow-mongodb
    restart: unless-stopped
    environment:
      MONGO_INITDB_ROOT_USERNAME: ${MONGO_ROOT_USER:-admin}
      MONGO_INITDB_ROOT_PASSWORD: ${MONGO_ROOT_PASSWORD:-mongo123}
      MONGO_INITDB_DATABASE: aiworkflow
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
      - mongodb_config:/data/configdb
    networks:
      - aiworkflow-network
    healthcheck:
      test: echo 'db.runCommand("ping").ok' | mongosh localhost:27017/test --quiet
      interval: 10s
      timeout: 10s
      retries: 5

  # pgAdmin (PostgreSQL 管理工具)
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: aiworkflow-pgadmin
    restart: unless-stopped
    environment:
      PGADMIN_DEFAULT_EMAIL: ${PGADMIN_EMAIL:-admin@example.com}
      PGADMIN_DEFAULT_PASSWORD: ${PGADMIN_PASSWORD:-admin123}
      PGADMIN_CONFIG_SERVER_MODE: 'False'
    ports:
      - "5050:80"
    volumes:
      - pgadmin_data:/var/lib/pgadmin
    networks:
      - aiworkflow-network
    depends_on:
      - postgres

volumes:
  postgres_data:
    driver: local
  redis_data:
    driver: local
  mongodb_data:
    driver: local
  mongodb_config:
    driver: local
  pgadmin_data:
    driver: local

networks:
  aiworkflow-network:
    driver: bridge
```

## 基本操作

### 啟動所有服務
```bash
docker compose up -d
```

### 查看服務狀態
```bash
docker compose ps
```

### 查看日誌
```bash
# 所有服務
docker compose logs -f

# 特定服務
docker compose logs -f postgres
docker compose logs -f redis
```

### 停止服務
```bash
docker compose down
```

### 完全清理 (包括數據卷)
```bash
docker compose down -v
```

## 數據持久化

### 備份數據庫

**PostgreSQL 備份**:
```bash
docker exec aiworkflow-postgres pg_dump -U postgres aiworkflow > backup_$(date +%Y%m%d_%H%M%S).sql
```

**MongoDB 備份**:
```bash
docker exec aiworkflow-mongodb mongodump --username admin --password mongo123 --authenticationDatabase admin --out /backup
docker cp aiworkflow-mongodb:/backup ./mongodb_backup_$(date +%Y%m%d_%H%M%S)
```

### 恢復數據庫

**PostgreSQL 恢復**:
```bash
docker exec -i aiworkflow-postgres psql -U postgres aiworkflow < backup.sql
```

**MongoDB 恢復**:
```bash
docker cp ./mongodb_backup aiworkflow-mongodb:/backup
docker exec aiworkflow-mongodb mongorestore --username admin --password mongo123 --authenticationDatabase admin /backup
```

## 健康檢查

查看容器健康狀態:
```bash
docker compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Health}}"
```

## 性能優化

### 資源限制配置

在 `docker-compose.yml` 中添加資源限制:

```yaml
services:
  postgres:
    # ... 其他配置
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '1.0'
          memory: 1G
```

### 網路優化

使用 host 網路模式 (僅限 Linux):
```yaml
services:
  postgres:
    network_mode: "host"
```

## 安全配置

### 使用環境變數文件

創建 `.env` 文件:
```env
# PostgreSQL
POSTGRES_PASSWORD=your_secure_password_here

# Redis
REDIS_PASSWORD=your_redis_password_here

# MongoDB
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=your_mongo_password_here

# pgAdmin
PGADMIN_EMAIL=admin@example.com
PGADMIN_PASSWORD=your_pgadmin_password_here
```

### 安全最佳實踐

1. 不要在 docker-compose.yml 中硬編碼密碼
2. 使用 `.env` 文件並添加到 `.gitignore`
3. 定期更新鏡像: `docker compose pull`
4. 使用非 root 用戶運行容器
5. 限制容器網路訪問

## 故障排除

### 常見問題

**端口衝突**:
```bash
# 查看佔用端口的進程
lsof -i :5432  # macOS/Linux
netstat -ano | findstr :5432  # Windows

# 修改 docker-compose.yml 使用不同端口
ports:
  - "5433:5432"  # 本地5433映射到容器5432
```

**數據卷權限問題**:
```bash
# 檢查卷權限
docker volume inspect postgres_data

# 如果需要,刪除並重新創建卷
docker compose down -v
docker compose up -d
```

**容器無法啟動**:
```bash
# 查看詳細錯誤
docker compose logs <service-name>

# 檢查容器配置
docker compose config
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
