# Docker 容器化完整指南

**版本**: 1.0.0
**最後更新**: 2025-11-01
**文檔狀態**: ✅ 完整
**適用環境**: Development, Staging, Production

---

## 📋 文檔概述

本文檔提供 AI Workflow Platform 的完整 Docker 容器化方案,包括所有微服務的 Dockerfile、Docker Compose 配置、多階段構建優化和容器安全最佳實踐。

### 目標讀者
- DevOps 工程師
- 後端開發工程師
- 系統架構師
- 運維團隊

### 前置需求
```yaml
工具版本:
  Docker Engine: ≥24.0
  Docker Compose: ≥2.20
  BuildKit: Enabled (export DOCKER_BUILDKIT=1)

系統要求:
  OS: Linux/macOS/Windows with WSL2
  RAM: ≥8GB (開發環境), ≥16GB (生產環境)
  Disk: ≥50GB
```

---

## 🏗️ 容器化架構概覽

### 服務容器清單

```yaml
後端服務 (5 個容器):
  1. api-gateway          # ASP.NET Core API Gateway (Port 5000)
  2. agent-service        # Agent 管理服務 (Port 5001)
  3. workflow-service     # 工作流引擎服務 (Port 5002)
  4. plugin-service       # Plugin 執行服務 (Port 5003)
  5. execution-service    # 代碼執行服務 (Port 5004)

前端服務 (2 個容器):
  6. frontend-host        # React 主應用 (Port 3000)
  7. frontend-remote      # Vue 工作流編輯器 (Port 3001)

基礎設施服務 (6 個容器):
  8. postgres             # PostgreSQL 16 (Port 5432)
  9. redis                # Redis 7 (Port 6379)
  10. rabbitmq            # RabbitMQ 3.12 (Port 5672, 15672)
  11. nginx               # NGINX Reverse Proxy (Port 80, 443)
  12. websocket-server    # WebSocket 協作服務 (Port 3002)
  13. monitoring          # Prometheus + Grafana

總計: 13 個容器
```

### 容器網絡架構

```
┌─────────────────────────────────────────────────────────┐
│                    Docker Host                           │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │           Network: ai-workflow-network              │ │
│  │              (bridge mode)                          │ │
│  │                                                     │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐         │ │
│  │  │  NGINX   │  │   API    │  │ Frontend │         │ │
│  │  │  :80/443 │  │  Gateway │  │   :3000  │         │ │
│  │  └────┬─────┘  └────┬─────┘  └────┬─────┘         │ │
│  │       │             │             │                │ │
│  │  ┌────┴─────────────┴─────────────┴─────┐         │ │
│  │  │       Internal Service Network        │         │ │
│  │  │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐    │         │ │
│  │  │  │Agent│ │Work │ │Plug │ │Exec │    │         │ │
│  │  │  │:5001│ │:5002│ │:5003│ │:5004│    │         │ │
│  │  │  └──┬──┘ └──┬──┘ └──┬──┘ └──┬──┘    │         │ │
│  │  │     └────────┴────────┴────────┘       │         │ │
│  │  └────────────────┬──────────────────────┘         │ │
│  │                   │                                 │ │
│  │  ┌────────────────┴──────────────────────┐         │ │
│  │  │       Data Layer                      │         │ │
│  │  │  ┌──────────┐  ┌──────┐  ┌──────┐   │         │ │
│  │  │  │PostgreSQL│  │ Redis│  │RabbitMQ   │         │ │
│  │  │  │  :5432   │  │ :6379│  │ :5672│   │         │ │
│  │  │  └──────────┘  └──────┘  └──────┘   │         │ │
│  │  └───────────────────────────────────────┘         │ │
│  └─────────────────────────────────────────────────────┘ │
│                                                          │
│  Volumes (Persistent Storage):                          │
│  - postgres-data    (PostgreSQL 數據持久化)              │
│  - redis-data       (Redis 持久化)                      │
│  - rabbitmq-data    (RabbitMQ 持久化)                   │
│  - nginx-certs      (SSL 證書)                          │
│  - app-logs         (應用日誌)                          │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Dockerfile 詳細設計

### 1. API Gateway Dockerfile (Multi-stage Build)

```dockerfile
# File: src/ApiGateway/Dockerfile
# 多階段構建: Build → Test → Publish → Runtime
# 優化: 最小鏡像大小, 安全加固, 緩存層優化

# ============================================
# Stage 1: Build Base
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-base
WORKDIR /src

# 安裝構建工具
RUN apk add --no-cache \
    git \
    ca-certificates \
    && update-ca-certificates

# 複製解決方案文件和項目文件（緩存層優化）
COPY ["Directory.Build.props", "./"]
COPY ["nuget.config", "./"]
COPY ["*.sln", "./"]

# 複製所有項目的 .csproj 文件
COPY ["src/ApiGateway/ApiGateway.csproj", "src/ApiGateway/"]
COPY ["src/Core/Core.csproj", "src/Core/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

# 還原 NuGet 依賴（獨立緩存層）
RUN dotnet restore "src/ApiGateway/ApiGateway.csproj" \
    --runtime linux-musl-x64

# ============================================
# Stage 2: Build
# ============================================
FROM build-base AS build
WORKDIR /src

# 複製所有源代碼
COPY ["src/", "src/"]

# 構建項目（Release 模式）
WORKDIR /src/src/ApiGateway
RUN dotnet build "ApiGateway.csproj" \
    -c Release \
    -o /app/build \
    --no-restore \
    --runtime linux-musl-x64 \
    /p:PublishTrimmed=false

# ============================================
# Stage 3: Test (可選階段)
# ============================================
FROM build AS test
WORKDIR /src

# 複製測試項目
COPY ["tests/ApiGateway.Tests/ApiGateway.Tests.csproj", "tests/ApiGateway.Tests/"]
COPY ["tests/", "tests/"]

# 運行單元測試
RUN dotnet test "tests/ApiGateway.Tests/ApiGateway.Tests.csproj" \
    -c Release \
    --no-build \
    --verbosity normal \
    --logger "trx;LogFileName=test-results.trx"

# ============================================
# Stage 4: Publish
# ============================================
FROM build AS publish
WORKDIR /src/src/ApiGateway

# 發布應用（Self-contained, Trimmed）
RUN dotnet publish "ApiGateway.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishTrimmed=true \
    /p:TrimMode=partial \
    /p:PublishSingleFile=false

# ============================================
# Stage 5: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS final

# 創建非 root 用戶
RUN addgroup -g 1000 appgroup && \
    adduser -D -u 1000 -G appgroup appuser

WORKDIR /app

# 安裝運行時依賴
RUN apk add --no-cache \
    ca-certificates \
    tzdata \
    curl \
    && update-ca-certificates

# 從 publish 階段複製應用
COPY --from=publish --chown=appuser:appgroup /app/publish .

# 創建日誌目錄
RUN mkdir -p /app/logs && \
    chown -R appuser:appgroup /app/logs

# 切換到非 root 用戶
USER appuser

# 健康檢查
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# 環境變量
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# 暴露端口
EXPOSE 5000

# 入口點
ENTRYPOINT ["./ApiGateway"]
```

**關鍵優化點**:
1. ✅ **多階段構建**: 5 個階段,最終鏡像僅包含運行時依賴
2. ✅ **緩存層優化**: 先復制 `.csproj` 文件,獨立還原依賴
3. ✅ **Self-contained 發布**: 無需 .NET Runtime,鏡像更小
4. ✅ **Trimming**: 移除未使用的代碼,減少鏡像大小
5. ✅ **非 root 用戶**: 安全最佳實踐
6. ✅ **健康檢查**: 自動檢測容器健康狀態

**鏡像大小對比**:
```yaml
傳統構建（包含 SDK）: ~850 MB
優化後（Alpine + Self-contained）: ~120 MB
優化率: 86% ↓
```

### 2. Frontend Host Dockerfile (React + Vite)

```dockerfile
# File: packages/host/Dockerfile
# Multi-stage Build: Dependencies → Build → Production Server

# ============================================
# Stage 1: Dependencies
# ============================================
FROM node:20-alpine AS deps

# 安裝 pnpm
RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

# 複製依賴清單（緩存層優化）
COPY package.json pnpm-lock.yaml ./
COPY .npmrc ./

# 安裝依賴（僅生產依賴）
RUN pnpm install --frozen-lockfile --prod

# ============================================
# Stage 2: Build
# ============================================
FROM node:20-alpine AS build

RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

# 從 deps 階段複製 node_modules
COPY --from=deps /app/node_modules ./node_modules

# 複製源代碼
COPY . .

# 構建應用（Vite）
ARG VITE_API_BASE_URL=http://localhost:5000
ARG VITE_WORKFLOW_EDITOR_URL=http://localhost:3001
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_WORKFLOW_EDITOR_URL=$VITE_WORKFLOW_EDITOR_URL

RUN pnpm build

# ============================================
# Stage 3: Production Server (NGINX)
# ============================================
FROM nginx:1.25-alpine AS production

# 安裝額外工具
RUN apk add --no-cache curl

# 複製自定義 NGINX 配置
COPY nginx.conf /etc/nginx/nginx.conf
COPY default.conf /etc/nginx/conf.d/default.conf

# 從 build 階段複製構建產物
COPY --from=build /app/dist /usr/share/nginx/html

# 創建非 root 用戶
RUN addgroup -g 101 -S nginx && \
    adduser -S -D -H -u 101 -h /var/cache/nginx -s /sbin/nologin -G nginx -g nginx nginx

# 健康檢查
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:3000/health || exit 1

# 暴露端口
EXPOSE 3000

# 啟動 NGINX
CMD ["nginx", "-g", "daemon off;"]
```

**NGINX 配置文件**:

```nginx
# File: packages/host/nginx.conf

user nginx;
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 1024;
    use epoll;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    # 日誌格式
    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';

    access_log /var/log/nginx/access.log main;

    # 性能優化
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;

    # Gzip 壓縮
    gzip on;
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types text/plain text/css text/xml text/javascript
               application/json application/javascript application/xml+rss
               application/rss+xml font/truetype font/opentype
               application/vnd.ms-fontobject image/svg+xml;

    # 引入站點配置
    include /etc/nginx/conf.d/*.conf;
}
```

```nginx
# File: packages/host/default.conf

server {
    listen 3000;
    server_name _;

    root /usr/share/nginx/html;
    index index.html;

    # 安全頭
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;
    add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' http://localhost:5000 http://localhost:3001 ws://localhost:3002;" always;

    # React Router SPA 支持
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 靜態資源緩存
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # API 代理（可選）
    location /api/ {
        proxy_pass http://api-gateway:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # 健康檢查端點
    location /health {
        access_log off;
        return 200 "healthy\n";
        add_header Content-Type text/plain;
    }

    # 禁止訪問隱藏文件
    location ~ /\. {
        deny all;
        access_log off;
        log_not_found off;
    }
}
```

### 3. Frontend Remote Dockerfile (Vue + Webpack)

```dockerfile
# File: packages/remote/Dockerfile
# Multi-stage Build for Vue Workflow Editor

# ============================================
# Stage 1: Dependencies
# ============================================
FROM node:20-alpine AS deps

RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

COPY package.json pnpm-lock.yaml ./
COPY .npmrc ./

RUN pnpm install --frozen-lockfile --prod

# ============================================
# Stage 2: Build
# ============================================
FROM node:20-alpine AS build

RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY . .

# 構建應用（Webpack 5 + Module Federation）
ARG NODE_ENV=production
ARG VITE_API_BASE_URL=http://localhost:5000
ENV NODE_ENV=$NODE_ENV
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN pnpm build

# ============================================
# Stage 3: Production Server
# ============================================
FROM nginx:1.25-alpine AS production

RUN apk add --no-cache curl

COPY nginx.conf /etc/nginx/nginx.conf
COPY default.conf /etc/nginx/conf.d/default.conf

# 複製構建產物（包含 remoteEntry.js）
COPY --from=build /app/dist /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:3001/health || exit 1

EXPOSE 3001

CMD ["nginx", "-g", "daemon off;"]
```

**NGINX 配置（Remote）**:

```nginx
# File: packages/remote/default.conf

server {
    listen 3001;
    server_name _;

    root /usr/share/nginx/html;
    index index.html;

    # CORS 配置（Module Federation 需要）
    add_header Access-Control-Allow-Origin "*" always;
    add_header Access-Control-Allow-Methods "GET, POST, OPTIONS" always;
    add_header Access-Control-Allow-Headers "Content-Type" always;

    # remoteEntry.js 特殊處理（Module Federation 入口）
    location /remoteEntry.js {
        add_header Access-Control-Allow-Origin "*" always;
        add_header Cache-Control "no-cache, no-store, must-revalidate";
        try_files $uri =404;
    }

    # 靜態資源
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 健康檢查
    location /health {
        access_log off;
        return 200 "healthy\n";
        add_header Content-Type text/plain;
    }
}
```

### 4. PostgreSQL Dockerfile（自定義擴展）

```dockerfile
# File: infrastructure/postgres/Dockerfile
# 基於官方鏡像,添加自定義擴展和初始化腳本

FROM postgres:16-alpine

# 安裝 PostgreSQL 擴展
RUN apk add --no-cache \
    postgresql-contrib \
    postgresql-plpython3

# 複製初始化腳本
COPY init-scripts/ /docker-entrypoint-initdb.d/

# 複製自定義配置
COPY postgresql.conf /etc/postgresql/postgresql.conf
COPY pg_hba.conf /etc/postgresql/pg_hba.conf

# 健康檢查
HEALTHCHECK --interval=10s --timeout=3s --start-period=30s --retries=3 \
    CMD pg_isready -U postgres || exit 1

EXPOSE 5432

CMD ["postgres", "-c", "config_file=/etc/postgresql/postgresql.conf"]
```

**PostgreSQL 初始化腳本**:

```sql
-- File: infrastructure/postgres/init-scripts/01-init-database.sql
-- 創建數據庫和用戶

-- 創建應用數據庫
CREATE DATABASE ai_workflow_db;

-- 創建應用用戶
CREATE USER ai_workflow_user WITH ENCRYPTED PASSWORD 'change_me_in_production';

-- 授予權限
GRANT ALL PRIVILEGES ON DATABASE ai_workflow_db TO ai_workflow_user;

-- 切換到應用數據庫
\c ai_workflow_db;

-- 啟用擴展
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";        -- UUID 生成
CREATE EXTENSION IF NOT EXISTS "pgcrypto";         -- 加密函數
CREATE EXTENSION IF NOT EXISTS "pg_trgm";          -- 模糊搜索
CREATE EXTENSION IF NOT EXISTS "btree_gin";        -- GIN 索引優化

-- 創建 Schema
CREATE SCHEMA IF NOT EXISTS app;
CREATE SCHEMA IF NOT EXISTS audit;

-- 授予 Schema 權限
GRANT ALL ON SCHEMA app TO ai_workflow_user;
GRANT ALL ON SCHEMA audit TO ai_workflow_user;

-- 設置默認權限
ALTER DEFAULT PRIVILEGES IN SCHEMA app GRANT ALL ON TABLES TO ai_workflow_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA app GRANT ALL ON SEQUENCES TO ai_workflow_user;
```

```sql
-- File: infrastructure/postgres/init-scripts/02-create-audit-table.sql
-- 創建審計日誌表

\c ai_workflow_db;

SET search_path TO audit, public;

-- 審計日誌表
CREATE TABLE IF NOT EXISTS audit_log (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    table_name VARCHAR(255) NOT NULL,
    operation VARCHAR(50) NOT NULL,
    old_data JSONB,
    new_data JSONB,
    changed_by VARCHAR(255),
    changed_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    ip_address INET,
    user_agent TEXT
);

CREATE INDEX idx_audit_log_table_name ON audit_log(table_name);
CREATE INDEX idx_audit_log_changed_at ON audit_log(changed_at DESC);
CREATE INDEX idx_audit_log_changed_by ON audit_log(changed_by);

-- 審計觸發器函數
CREATE OR REPLACE FUNCTION audit.log_changes()
RETURNS TRIGGER AS $$
BEGIN
    IF (TG_OP = 'DELETE') THEN
        INSERT INTO audit.audit_log (table_name, operation, old_data)
        VALUES (TG_TABLE_NAME, TG_OP, row_to_json(OLD));
        RETURN OLD;
    ELSIF (TG_OP = 'UPDATE') THEN
        INSERT INTO audit.audit_log (table_name, operation, old_data, new_data)
        VALUES (TG_TABLE_NAME, TG_OP, row_to_json(OLD), row_to_json(NEW));
        RETURN NEW;
    ELSIF (TG_OP = 'INSERT') THEN
        INSERT INTO audit.audit_log (table_name, operation, new_data)
        VALUES (TG_TABLE_NAME, TG_OP, row_to_json(NEW));
        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;
```

**PostgreSQL 配置文件**:

```conf
# File: infrastructure/postgres/postgresql.conf

# 連接設置
listen_addresses = '*'
port = 5432
max_connections = 200

# 內存設置
shared_buffers = 256MB              # 25% of RAM (1GB RAM)
effective_cache_size = 768MB        # 75% of RAM
maintenance_work_mem = 64MB
work_mem = 4MB

# WAL 設置
wal_level = replica
max_wal_size = 1GB
min_wal_size = 80MB
checkpoint_completion_target = 0.9

# 查詢優化
random_page_cost = 1.1              # SSD 優化
effective_io_concurrency = 200      # SSD 並發

# 日誌設置
logging_collector = on
log_directory = 'pg_log'
log_filename = 'postgresql-%Y-%m-%d_%H%M%S.log'
log_rotation_age = 1d
log_rotation_size = 100MB
log_line_prefix = '%t [%p]: [%l-1] user=%u,db=%d,app=%a,client=%h '
log_checkpoints = on
log_connections = on
log_disconnections = on
log_duration = off
log_lock_waits = on
log_statement = 'ddl'
log_timezone = 'UTC'

# 自動清理
autovacuum = on
autovacuum_max_workers = 3
autovacuum_naptime = 1min

# 時區
timezone = 'UTC'
```

### 5. Redis Dockerfile（持久化配置）

```dockerfile
# File: infrastructure/redis/Dockerfile

FROM redis:7-alpine

# 複製自定義配置
COPY redis.conf /usr/local/etc/redis/redis.conf

# 創建數據目錄
RUN mkdir -p /data && \
    chown -R redis:redis /data

# 健康檢查
HEALTHCHECK --interval=5s --timeout=3s --start-period=10s --retries=3 \
    CMD redis-cli ping || exit 1

EXPOSE 6379

CMD ["redis-server", "/usr/local/etc/redis/redis.conf"]
```

**Redis 配置文件**:

```conf
# File: infrastructure/redis/redis.conf

# 網絡設置
bind 0.0.0.0
protected-mode yes
port 6379
timeout 300
tcp-keepalive 300

# 持久化設置
save 900 1          # 900 秒內至少 1 個鍵變更
save 300 10         # 300 秒內至少 10 個鍵變更
save 60 10000       # 60 秒內至少 10000 個鍵變更

# RDB 文件
dbfilename dump.rdb
dir /data
rdbcompression yes
rdbchecksum yes

# AOF 持久化
appendonly yes
appendfilename "appendonly.aof"
appendfsync everysec
no-appendfsync-on-rewrite no
auto-aof-rewrite-percentage 100
auto-aof-rewrite-min-size 64mb

# 內存管理
maxmemory 512mb
maxmemory-policy allkeys-lru
maxmemory-samples 5

# 慢查詢日誌
slowlog-log-slower-than 10000
slowlog-max-len 128

# 日誌
loglevel notice
logfile ""

# 客戶端
maxclients 10000
```

### 6. RabbitMQ Dockerfile

```dockerfile
# File: infrastructure/rabbitmq/Dockerfile

FROM rabbitmq:3.12-management-alpine

# 安裝插件
RUN rabbitmq-plugins enable --offline \
    rabbitmq_management \
    rabbitmq_prometheus \
    rabbitmq_shovel \
    rabbitmq_shovel_management

# 複製配置文件
COPY rabbitmq.conf /etc/rabbitmq/rabbitmq.conf
COPY definitions.json /etc/rabbitmq/definitions.json

# 健康檢查
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=3 \
    CMD rabbitmq-diagnostics -q ping || exit 1

EXPOSE 5672 15672

CMD ["rabbitmq-server"]
```

**RabbitMQ 配置**:

```conf
# File: infrastructure/rabbitmq/rabbitmq.conf

# 網絡設置
listeners.tcp.default = 5672
management.tcp.port = 15672

# 日誌
log.console = true
log.console.level = info

# 內存和磁盤限制
vm_memory_high_watermark.relative = 0.6
disk_free_limit.absolute = 2GB

# 默認用戶（僅開發環境）
default_user = admin
default_pass = admin_change_me

# Management 插件
management.load_definitions = /etc/rabbitmq/definitions.json
```

```json
// File: infrastructure/rabbitmq/definitions.json
{
  "rabbit_version": "3.12.0",
  "rabbitmq_version": "3.12.0",
  "users": [
    {
      "name": "admin",
      "password_hash": "...",
      "hashing_algorithm": "rabbit_password_hashing_sha256",
      "tags": "administrator"
    },
    {
      "name": "app_user",
      "password_hash": "...",
      "hashing_algorithm": "rabbit_password_hashing_sha256",
      "tags": ""
    }
  ],
  "vhosts": [
    { "name": "/" },
    { "name": "ai-workflow" }
  ],
  "permissions": [
    {
      "user": "app_user",
      "vhost": "ai-workflow",
      "configure": ".*",
      "write": ".*",
      "read": ".*"
    }
  ],
  "exchanges": [
    {
      "name": "workflow.events",
      "vhost": "ai-workflow",
      "type": "topic",
      "durable": true,
      "auto_delete": false
    }
  ],
  "queues": [
    {
      "name": "workflow.execution",
      "vhost": "ai-workflow",
      "durable": true,
      "auto_delete": false,
      "arguments": {
        "x-message-ttl": 86400000,
        "x-max-length": 10000
      }
    }
  ],
  "bindings": [
    {
      "source": "workflow.events",
      "vhost": "ai-workflow",
      "destination": "workflow.execution",
      "destination_type": "queue",
      "routing_key": "workflow.#"
    }
  ]
}
```

---

## 🐳 Docker Compose 完整配置

### 開發環境 Docker Compose

```yaml
# File: docker-compose.yml
# 開發環境完整配置

version: '3.9'

# ============================================
# Networks
# ============================================
networks:
  ai-workflow-network:
    driver: bridge
    ipam:
      config:
        - subnet: 172.28.0.0/16

# ============================================
# Volumes
# ============================================
volumes:
  postgres-data:
    driver: local
  redis-data:
    driver: local
  rabbitmq-data:
    driver: local
  nginx-certs:
    driver: local
  app-logs:
    driver: local

# ============================================
# Services
# ============================================
services:
  # ------------------------------------------
  # Database Layer
  # ------------------------------------------
  postgres:
    build:
      context: ./infrastructure/postgres
      dockerfile: Dockerfile
    container_name: ai-workflow-postgres
    hostname: postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: ai_workflow_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres_dev_password}
      POSTGRES_INITDB_ARGS: "--encoding=UTF8 --locale=C"
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
      - ./infrastructure/postgres/init-scripts:/docker-entrypoint-initdb.d:ro
      - app-logs:/var/log/postgresql
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.10
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  redis:
    build:
      context: ./infrastructure/redis
      dockerfile: Dockerfile
    container_name: ai-workflow-redis
    hostname: redis
    restart: unless-stopped
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.11
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 3s
      retries: 5

  rabbitmq:
    build:
      context: ./infrastructure/rabbitmq
      dockerfile: Dockerfile
    container_name: ai-workflow-rabbitmq
    hostname: rabbitmq
    restart: unless-stopped
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD:-admin_dev_password}
    ports:
      - "5672:5672"    # AMQP
      - "15672:15672"  # Management UI
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.12
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "-q", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  # ------------------------------------------
  # Backend Services
  # ------------------------------------------
  api-gateway:
    build:
      context: ./src/ApiGateway
      dockerfile: Dockerfile
      target: final
    container_name: ai-workflow-api-gateway
    hostname: api-gateway
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:5000
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=ai_workflow_db;Username=postgres;Password=${POSTGRES_PASSWORD:-postgres_dev_password}"
      Redis__ConnectionString: "redis:6379,abortConnect=false"
      RabbitMQ__HostName: rabbitmq
      RabbitMQ__UserName: admin
      RabbitMQ__Password: ${RABBITMQ_PASSWORD:-admin_dev_password}
      Logging__LogLevel__Default: Information
      Logging__LogLevel__Microsoft: Warning
    ports:
      - "5000:5000"
    volumes:
      - app-logs:/app/logs
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.20
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 40s

  agent-service:
    build:
      context: ./src/AgentService
      dockerfile: Dockerfile
      target: final
    container_name: ai-workflow-agent-service
    hostname: agent-service
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:5001
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=ai_workflow_db;Username=postgres;Password=${POSTGRES_PASSWORD:-postgres_dev_password}"
      Redis__ConnectionString: "redis:6379"
      RabbitMQ__HostName: rabbitmq
      SemanticKernel__AzureOpenAI__Endpoint: ${AZURE_OPENAI_ENDPOINT}
      SemanticKernel__AzureOpenAI__ApiKey: ${AZURE_OPENAI_API_KEY}
      SemanticKernel__AzureOpenAI__DeploymentName: ${AZURE_OPENAI_DEPLOYMENT_NAME:-gpt-4}
    ports:
      - "5001:5001"
    volumes:
      - app-logs:/app/logs
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.21
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy

  workflow-service:
    build:
      context: ./src/WorkflowService
      dockerfile: Dockerfile
      target: final
    container_name: ai-workflow-workflow-service
    hostname: workflow-service
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:5002
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=ai_workflow_db;Username=postgres;Password=${POSTGRES_PASSWORD:-postgres_dev_password}"
      Redis__ConnectionString: "redis:6379"
      RabbitMQ__HostName: rabbitmq
    ports:
      - "5002:5002"
    volumes:
      - app-logs:/app/logs
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.22
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy

  # ------------------------------------------
  # Frontend Services
  # ------------------------------------------
  frontend-host:
    build:
      context: ./packages/host
      dockerfile: Dockerfile
      target: production
      args:
        VITE_API_BASE_URL: http://localhost:5000
        VITE_WORKFLOW_EDITOR_URL: http://localhost:3001
    container_name: ai-workflow-frontend-host
    hostname: frontend-host
    restart: unless-stopped
    ports:
      - "3000:3000"
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.30
    depends_on:
      - api-gateway
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000/health"]
      interval: 30s
      timeout: 3s
      retries: 3

  frontend-remote:
    build:
      context: ./packages/remote
      dockerfile: Dockerfile
      target: production
      args:
        VITE_API_BASE_URL: http://localhost:5000
    container_name: ai-workflow-frontend-remote
    hostname: frontend-remote
    restart: unless-stopped
    ports:
      - "3001:3001"
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.31
    depends_on:
      - api-gateway
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3001/health"]
      interval: 30s
      timeout: 3s
      retries: 3

  # ------------------------------------------
  # WebSocket Server (協作)
  # ------------------------------------------
  websocket-server:
    build:
      context: ./services/websocket-server
      dockerfile: Dockerfile
    container_name: ai-workflow-websocket-server
    hostname: websocket-server
    restart: unless-stopped
    environment:
      NODE_ENV: development
      PORT: 3002
      REDIS_URL: redis://redis:6379
    ports:
      - "3002:3002"
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.32
    depends_on:
      redis:
        condition: service_healthy

  # ------------------------------------------
  # Reverse Proxy (NGINX)
  # ------------------------------------------
  nginx:
    image: nginx:1.25-alpine
    container_name: ai-workflow-nginx
    hostname: nginx
    restart: unless-stopped
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./infrastructure/nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./infrastructure/nginx/conf.d:/etc/nginx/conf.d:ro
      - nginx-certs:/etc/nginx/certs:ro
      - app-logs:/var/log/nginx
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.40
    depends_on:
      - api-gateway
      - frontend-host
      - frontend-remote
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/health"]
      interval: 30s
      timeout: 3s
      retries: 3

  # ------------------------------------------
  # Monitoring (Prometheus + Grafana)
  # ------------------------------------------
  prometheus:
    image: prom/prometheus:latest
    container_name: ai-workflow-prometheus
    hostname: prometheus
    restart: unless-stopped
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/usr/share/prometheus/console_libraries'
      - '--web.console.templates=/usr/share/prometheus/consoles'
    ports:
      - "9090:9090"
    volumes:
      - ./infrastructure/prometheus/prometheus.yml:/etc/prometheus/prometheus.yml:ro
      - prometheus-data:/prometheus
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.50

  grafana:
    image: grafana/grafana:latest
    container_name: ai-workflow-grafana
    hostname: grafana
    restart: unless-stopped
    environment:
      GF_SECURITY_ADMIN_USER: admin
      GF_SECURITY_ADMIN_PASSWORD: ${GRAFANA_PASSWORD:-admin}
      GF_INSTALL_PLUGINS: grafana-piechart-panel
    ports:
      - "3100:3000"
    volumes:
      - ./infrastructure/grafana/provisioning:/etc/grafana/provisioning:ro
      - grafana-data:/var/lib/grafana
    networks:
      ai-workflow-network:
        ipv4_address: 172.28.0.51
    depends_on:
      - prometheus
```

### 生產環境 Docker Compose

```yaml
# File: docker-compose.prod.yml
# 生產環境配置（簡化版，實際生產使用 Kubernetes）

version: '3.9'

# 繼承開發環境配置
include:
  - docker-compose.yml

services:
  # 覆蓋開發環境設置
  api-gateway:
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      Logging__LogLevel__Default: Warning
      Logging__LogLevel__Microsoft: Error
    deploy:
      replicas: 3
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M

  postgres:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '1.0'
          memory: 1G

  redis:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
```

---

## 🔐 容器安全最佳實踐

### 1. 非 Root 用戶執行

**所有應用容器**必須使用非 root 用戶運行：

```dockerfile
# ❌ 錯誤：以 root 用戶運行
USER root

# ✅ 正確：創建並使用非 root 用戶
RUN addgroup -g 1000 appgroup && \
    adduser -D -u 1000 -G appgroup appuser

USER appuser
```

### 2. 最小化鏡像攻擊面

```dockerfile
# ✅ 使用 Alpine 基礎鏡像（最小化）
FROM node:20-alpine AS build

# ✅ 移除不必要的包
RUN apk del build-dependencies && \
    rm -rf /var/cache/apk/*

# ✅ Multi-stage build 移除構建工具
FROM alpine:latest AS final
COPY --from=build /app/dist /app
```

### 3. 掃描漏洞

```bash
# 使用 Trivy 掃描鏡像漏洞
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  aquasec/trivy image ai-workflow/api-gateway:latest

# 使用 Snyk 掃描
snyk container test ai-workflow/api-gateway:latest

# 集成到 CI/CD
# .github/workflows/security-scan.yml
- name: Run Trivy vulnerability scanner
  uses: aquasecurity/trivy-action@master
  with:
    image-ref: 'ai-workflow/api-gateway:${{ github.sha }}'
    format: 'sarif'
    output: 'trivy-results.sarif'
```

### 4. 密鑰管理

```yaml
# ❌ 錯誤：硬編碼密碼
environment:
  POSTGRES_PASSWORD: "my_secret_password"

# ✅ 正確：使用環境變量
environment:
  POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}

# ✅ 正確：使用 Docker Secrets（Swarm/Kubernetes）
secrets:
  - postgres_password

secrets:
  postgres_password:
    file: ./secrets/postgres_password.txt
```

### 5. 網絡隔離

```yaml
# ✅ 分離前端和後端網絡
networks:
  frontend-network:
    driver: bridge
  backend-network:
    driver: bridge
    internal: true  # 禁止外部訪問

services:
  api-gateway:
    networks:
      - frontend-network
      - backend-network

  postgres:
    networks:
      - backend-network  # 僅內部網絡訪問
```

### 6. 只讀文件系統

```yaml
services:
  api-gateway:
    read_only: true  # 文件系統只讀
    tmpfs:
      - /tmp          # 臨時文件系統
      - /app/logs     # 日誌目錄
```

### 7. 資源限制

```yaml
services:
  api-gateway:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M
    ulimits:
      nofile:
        soft: 65536
        hard: 65536
```

---

## 📊 容器監控和日誌

### Prometheus 配置

```yaml
# File: infrastructure/prometheus/prometheus.yml

global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  # .NET 應用 (Prometheus Exporter)
  - job_name: 'dotnet-api'
    static_configs:
      - targets:
          - 'api-gateway:5000'
          - 'agent-service:5001'
          - 'workflow-service:5002'
    metrics_path: '/metrics'

  # Node.js 應用
  - job_name: 'nodejs-websocket'
    static_configs:
      - targets:
          - 'websocket-server:3002'
    metrics_path: '/metrics'

  # PostgreSQL Exporter
  - job_name: 'postgres'
    static_configs:
      - targets:
          - 'postgres-exporter:9187'

  # Redis Exporter
  - job_name: 'redis'
    static_configs:
      - targets:
          - 'redis-exporter:9121'

  # RabbitMQ
  - job_name: 'rabbitmq'
    static_configs:
      - targets:
          - 'rabbitmq:15692'
    metrics_path: '/metrics'

  # Docker 容器
  - job_name: 'docker'
    static_configs:
      - targets:
          - 'cadvisor:8080'
```

### Grafana Dashboards

```yaml
# File: infrastructure/grafana/provisioning/dashboards/dashboard.yml

apiVersion: 1

providers:
  - name: 'default'
    orgId: 1
    folder: ''
    type: file
    disableDeletion: false
    updateIntervalSeconds: 10
    allowUiUpdates: true
    options:
      path: /etc/grafana/provisioning/dashboards
```

### 日誌收集（Fluentd）

```yaml
# File: infrastructure/fluentd/fluent.conf

<source>
  @type forward
  port 24224
  bind 0.0.0.0
</source>

<filter docker.**>
  @type parser
  key_name log
  <parse>
    @type json
  </parse>
</filter>

<match docker.**>
  @type elasticsearch
  host elasticsearch
  port 9200
  logstash_format true
  logstash_prefix docker
  <buffer>
    flush_interval 10s
  </buffer>
</match>
```

---

## 🚀 部署命令

### 開發環境啟動

```bash
# 1. 創建 .env 文件
cat > .env << 'EOF'
POSTGRES_PASSWORD=postgres_dev_password
RABBITMQ_PASSWORD=admin_dev_password
AZURE_OPENAI_ENDPOINT=https://your-endpoint.openai.azure.com/
AZURE_OPENAI_API_KEY=your_api_key_here
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4
GRAFANA_PASSWORD=admin
EOF

# 2. 構建所有鏡像
docker-compose build --parallel

# 3. 啟動所有服務
docker-compose up -d

# 4. 查看服務狀態
docker-compose ps

# 5. 查看日誌
docker-compose logs -f api-gateway

# 6. 停止所有服務
docker-compose down

# 7. 停止並刪除所有數據
docker-compose down -v
```

### 單獨構建和運行

```bash
# 構建單個服務
docker-compose build api-gateway

# 僅啟動後端服務
docker-compose up -d postgres redis rabbitmq api-gateway

# 重啟服務
docker-compose restart api-gateway

# 查看資源使用
docker stats
```

### 生產環境部署

```bash
# 使用生產配置
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# 擴展服務
docker-compose up -d --scale api-gateway=3 --scale agent-service=2
```

---

## 🔧 故障排查

### 常見問題

**Q1: 容器啟動失敗，健康檢查一直失敗?**

```bash
# 檢查容器日誌
docker logs ai-workflow-api-gateway

# 進入容器調試
docker exec -it ai-workflow-api-gateway sh

# 手動執行健康檢查
curl -f http://localhost:5000/health
```

**Q2: 數據庫連接失敗?**

```bash
# 檢查 PostgreSQL 是否就緒
docker exec -it ai-workflow-postgres pg_isready -U postgres

# 測試連接
docker exec -it ai-workflow-postgres psql -U postgres -d ai_workflow_db -c "SELECT 1;"

# 檢查網絡連接
docker exec -it ai-workflow-api-gateway ping postgres
```

**Q3: 前端無法訪問後端 API?**

```bash
# 檢查 NGINX 配置
docker exec -it ai-workflow-nginx nginx -t

# 查看 NGINX 日誌
docker logs ai-workflow-nginx

# 測試 API 連接
curl http://localhost:80/api/health
```

**Q4: 容器佔用過多資源?**

```bash
# 查看資源使用
docker stats --no-stream

# 限制資源
docker update --cpus="0.5" --memory="256m" ai-workflow-api-gateway

# 清理未使用的資源
docker system prune -a --volumes
```

---

## 📚 參考資料

### 官方文檔
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Dockerfile Reference](https://docs.docker.com/engine/reference/builder/)
- [Docker Compose Specification](https://docs.docker.com/compose/compose-file/)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)

### 安全指南
- [Docker Security Best Practices](https://docs.docker.com/engine/security/)
- [CIS Docker Benchmark](https://www.cisecurity.org/benchmark/docker)
- [OWASP Container Security](https://owasp.org/www-project-docker-top-10/)

### 優化資源
- [Alpine Linux](https://alpinelinux.org/)
- [Dive - Image Size Analyzer](https://github.com/wagoodman/dive)
- [Hadolint - Dockerfile Linter](https://github.com/hadolint/hadolint)

---

**文檔維護**: DevOps 團隊
**最後更新**: 2025-11-01
**版本**: 1.0.0
**狀態**: ✅ 完整
