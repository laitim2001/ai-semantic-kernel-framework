# 12. Docker Compose 配置 (Docker Compose Configuration)

## 文檔資訊

| 項目 | 內容 |
|------|------|
| **文檔版本** | 1.0.0 |
| **創建日期** | 2025-01-15 |
| **最後更新** | 2025-01-15 |
| **狀態** | Draft |
| **作者** | AI Workflow Platform Team |
| **關聯文檔** | 11-DEPLOYMENT-ARCHITECTURE.md, 13-KUBERNETES.md |

---

## 目錄

- [12.1 Docker Compose 總覽](#121-docker-compose-總覽)
- [12.2 完整 Docker Compose 配置](#122-完整-docker-compose-配置)
- [12.3 服務配置詳解](#123-服務配置詳解)
- [12.4 網路配置](#124-網路配置)
- [12.5 持久化配置](#125-持久化配置)
- [12.6 環境變數管理](#126-環境變數管理)
- [12.7 開發工作流](#127-開發工作流)
- [12.8 故障排除](#128-故障排除)

---

## 12.1 Docker Compose 總覽

### 12.1.1 用途與目標

**Docker Compose 用於:**
- **本地開發環境**: 快速啟動完整開發環境
- **集成測試**: 提供獨立測試環境
- **PoC 驗證**: 快速驗證新功能和架構
- **開發者入職**: 新團隊成員快速上手

**設計目標:**
1. **一鍵啟動**: `docker-compose up` 啟動所有服務
2. **環境隔離**: 獨立於主機環境
3. **配置簡化**: 最小化配置，使用合理默認值
4. **資源輕量**: 本地開發機器友好 (8 GB RAM 可運行)
5. **快速迭代**: 支持熱重載和快速重啟

### 12.1.2 架構拓撲

```mermaid
graph TB
    subgraph "Development Machine"
        subgraph "Docker Compose Network"
            Gateway[API Gateway<br/>:8000]
            Auth[Auth Service<br/>:8001]
            Agent[Agent Service<br/>:8002]
            Persona[Persona Service<br/>:8003]
            Code[Code Execution<br/>:8004]
            SQL[Text-to-SQL<br/>:8005]
            Knowledge[Knowledge Base<br/>:8006]
            Workflow[Workflow Service<br/>:8007]

            PostgreSQL[(PostgreSQL<br/>:5432)]
            Redis[(Redis<br/>:6379)]
            RabbitMQ[RabbitMQ<br/>:5672, :15672]

            Prometheus[Prometheus<br/>:9090]
            Grafana[Grafana<br/>:3000]
        end
    end

    Browser[Browser] --> Gateway
    Gateway --> Auth
    Gateway --> Agent
    Gateway --> Persona
    Gateway --> Code
    Gateway --> SQL
    Gateway --> Knowledge
    Gateway --> Workflow

    Auth --> PostgreSQL
    Agent --> PostgreSQL
    Persona --> PostgreSQL
    Code --> PostgreSQL
    SQL --> PostgreSQL
    Knowledge --> PostgreSQL
    Workflow --> PostgreSQL

    Auth --> Redis
    Agent --> Redis
    Persona --> Redis

    Agent --> RabbitMQ
    Workflow --> RabbitMQ

    Gateway --> Prometheus
    Auth --> Prometheus
    Agent --> Prometheus
```

**端口映射:**

| 服務 | 容器端口 | 主機端口 | 用途 |
|------|---------|---------|------|
| API Gateway | 8080 | 8000 | API 入口 |
| Auth Service | 8080 | 8001 | 認證服務 |
| Agent Service | 8080 | 8002 | Agent 管理 |
| Persona Service | 8080 | 8003 | Persona 管理 |
| Code Execution | 8080 | 8004 | 代碼執行 |
| Text-to-SQL | 8080 | 8005 | SQL 生成 |
| Knowledge Base | 8080 | 8006 | 知識庫 |
| Workflow Service | 8080 | 8007 | 工作流 |
| PostgreSQL | 5432 | 5432 | 數據庫 |
| Redis | 6379 | 6379 | 緩存 |
| RabbitMQ (AMQP) | 5672 | 5672 | 消息隊列 |
| RabbitMQ (Management) | 15672 | 15672 | 管理界面 |
| Prometheus | 9090 | 9090 | 監控 |
| Grafana | 3000 | 3000 | 可視化 |

---

## 12.2 完整 Docker Compose 配置

### 12.2.1 主配置文件 (docker-compose.yml)

```yaml
version: '3.9'

services:
  #########################################
  # Infrastructure Services
  #########################################

  # PostgreSQL Database
  postgresql:
    image: postgres:16-alpine
    container_name: ai-workflow-postgresql
    hostname: postgresql
    restart: unless-stopped
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres_dev_password
      POSTGRES_MULTIPLE_DATABASES: auth_db,agent_db,persona_db,code_db,sql_db,knowledge_db,workflow_db
    volumes:
      - postgresql_data:/var/lib/postgresql/data
      - ./scripts/init-databases.sh:/docker-entrypoint-initdb.d/init-databases.sh
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis Cache
  redis:
    image: redis:7-alpine
    container_name: ai-workflow-redis
    hostname: redis
    restart: unless-stopped
    ports:
      - "6379:6379"
    command: redis-server --appendonly yes --requirepass redis_dev_password
    volumes:
      - redis_data:/data
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD", "redis-cli", "--raw", "incr", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  # RabbitMQ Message Queue
  rabbitmq:
    image: rabbitmq:3.12-management-alpine
    container_name: ai-workflow-rabbitmq
    hostname: rabbitmq
    restart: unless-stopped
    ports:
      - "5672:5672"    # AMQP
      - "15672:15672"  # Management UI
    environment:
      RABBITMQ_DEFAULT_USER: rabbitmq
      RABBITMQ_DEFAULT_PASS: rabbitmq_dev_password
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 30s
      timeout: 10s
      retries: 5

  #########################################
  # Application Services
  #########################################

  # API Gateway
  api-gateway:
    build:
      context: ../../src/ApiGateway
      dockerfile: Dockerfile
    container_name: ai-workflow-gateway
    hostname: api-gateway
    restart: unless-stopped
    ports:
      - "8000:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ServiceDiscovery__Services__AuthService: http://auth-service:8080
      ServiceDiscovery__Services__AgentService: http://agent-service:8080
      ServiceDiscovery__Services__PersonaService: http://persona-service:8080
      ServiceDiscovery__Services__CodeExecutionService: http://code-execution-service:8080
      ServiceDiscovery__Services__TextToSQLService: http://text-to-sql-service:8080
      ServiceDiscovery__Services__KnowledgeBaseService: http://knowledge-base-service:8080
      ServiceDiscovery__Services__WorkflowService: http://workflow-service:8080
    depends_on:
      auth-service:
        condition: service_healthy
      agent-service:
        condition: service_healthy
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Auth Service
  auth-service:
    build:
      context: ../../src/AuthService
      dockerfile: Dockerfile
    container_name: ai-workflow-auth
    hostname: auth-service
    restart: unless-stopped
    ports:
      - "8001:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=auth_db;Username=postgres;Password=postgres_dev_password
      Redis__ConnectionString: redis:6379,password=redis_dev_password
      Jwt__Secret: dev_jwt_secret_key_min_32_characters_long_for_hs256
      Jwt__Issuer: ai-workflow-dev
      Jwt__Audience: ai-workflow-dev
      Jwt__AccessTokenExpirationMinutes: 15
      Jwt__RefreshTokenExpirationDays: 30
    depends_on:
      postgresql:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/auth-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Agent Service
  agent-service:
    build:
      context: ../../src/AgentService
      dockerfile: Dockerfile
    container_name: ai-workflow-agent
    hostname: agent-service
    restart: unless-stopped
    ports:
      - "8002:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=agent_db;Username=postgres;Password=postgres_dev_password
      Redis__ConnectionString: redis:6379,password=redis_dev_password
      RabbitMQ__Host: rabbitmq
      RabbitMQ__Username: rabbitmq
      RabbitMQ__Password: rabbitmq_dev_password
      OpenAI__ApiKey: ${OPENAI_API_KEY}
      OpenAI__Model: gpt-4
      OpenAI__MaxTokens: 4000
      SemanticKernel__Version: 1.66.0
    depends_on:
      postgresql:
        condition: service_healthy
      redis:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/agent-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Persona Service
  persona-service:
    build:
      context: ../../src/PersonaService
      dockerfile: Dockerfile
    container_name: ai-workflow-persona
    hostname: persona-service
    restart: unless-stopped
    ports:
      - "8003:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=persona_db;Username=postgres;Password=postgres_dev_password
      Redis__ConnectionString: redis:6379,password=redis_dev_password
      OpenAI__ApiKey: ${OPENAI_API_KEY}
    depends_on:
      postgresql:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/persona-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Code Execution Service
  code-execution-service:
    build:
      context: ../../src/CodeExecutionService
      dockerfile: Dockerfile
    container_name: ai-workflow-code
    hostname: code-execution-service
    restart: unless-stopped
    ports:
      - "8004:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=code_db;Username=postgres;Password=postgres_dev_password
      Docker__Host: unix:///var/run/docker.sock
      Security__EnableSeccomp: "true"
      Security__EnableAppArmor: "true"
      Security__MaxExecutionTime: 30
      Security__MaxMemoryMB: 512
      Security__MaxCpuPercent: 50
    depends_on:
      postgresql:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock  # Docker socket for sandbox
      - ./logs/code-execution-service:/app/logs
    privileged: true  # Required for Docker-in-Docker
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Text-to-SQL Service
  text-to-sql-service:
    build:
      context: ../../src/TextToSQLService
      dockerfile: Dockerfile
    container_name: ai-workflow-sql
    hostname: text-to-sql-service
    restart: unless-stopped
    ports:
      - "8005:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=sql_db;Username=postgres;Password=postgres_dev_password
      OpenAI__ApiKey: ${OPENAI_API_KEY}
      OpenAI__Model: gpt-4
    depends_on:
      postgresql:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/text-to-sql-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Knowledge Base Service
  knowledge-base-service:
    build:
      context: ../../src/KnowledgeBaseService
      dockerfile: Dockerfile
    container_name: ai-workflow-knowledge
    hostname: knowledge-base-service
    restart: unless-stopped
    ports:
      - "8006:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=knowledge_db;Username=postgres;Password=postgres_dev_password
      OpenAI__ApiKey: ${OPENAI_API_KEY}
      OpenAI__EmbeddingModel: text-embedding-3-small
      # Note: Azure AI Search not available in local dev, using PostgreSQL pgvector
      VectorStore__Provider: PostgreSQL
    depends_on:
      postgresql:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/knowledge-base-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Workflow Service
  workflow-service:
    build:
      context: ../../src/WorkflowService
      dockerfile: Dockerfile
    container_name: ai-workflow-workflow
    hostname: workflow-service
    restart: unless-stopped
    ports:
      - "8007:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=workflow_db;Username=postgres;Password=postgres_dev_password
      RabbitMQ__Host: rabbitmq
      RabbitMQ__Username: rabbitmq
      RabbitMQ__Password: rabbitmq_dev_password
    depends_on:
      postgresql:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    networks:
      - ai-workflow-network
    volumes:
      - ./logs/workflow-service:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  #########################################
  # Monitoring Services
  #########################################

  # Prometheus
  prometheus:
    image: prom/prometheus:latest
    container_name: ai-workflow-prometheus
    hostname: prometheus
    restart: unless-stopped
    ports:
      - "9090:9090"
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/usr/share/prometheus/console_libraries'
      - '--web.console.templates=/usr/share/prometheus/consoles'
    volumes:
      - ./monitoring/prometheus/prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD", "wget", "--spider", "-q", "http://localhost:9090/-/healthy"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Grafana
  grafana:
    image: grafana/grafana:latest
    container_name: ai-workflow-grafana
    hostname: grafana
    restart: unless-stopped
    ports:
      - "3000:3000"
    environment:
      GF_SECURITY_ADMIN_USER: admin
      GF_SECURITY_ADMIN_PASSWORD: admin
      GF_USERS_ALLOW_SIGN_UP: "false"
    volumes:
      - grafana_data:/var/lib/grafana
      - ./monitoring/grafana/provisioning:/etc/grafana/provisioning
      - ./monitoring/grafana/dashboards:/var/lib/grafana/dashboards
    depends_on:
      - prometheus
    networks:
      - ai-workflow-network
    healthcheck:
      test: ["CMD", "wget", "--spider", "-q", "http://localhost:3000/api/health"]
      interval: 30s
      timeout: 10s
      retries: 3

#########################################
# Networks
#########################################

networks:
  ai-workflow-network:
    driver: bridge
    ipam:
      driver: default
      config:
        - subnet: 172.28.0.0/16

#########################################
# Volumes
#########################################

volumes:
  postgresql_data:
    driver: local
  redis_data:
    driver: local
  rabbitmq_data:
    driver: local
  prometheus_data:
    driver: local
  grafana_data:
    driver: local
```

### 12.2.2 覆蓋配置文件 (docker-compose.override.yml)

```yaml
# docker-compose.override.yml
# 開發環境覆蓋配置 (自動應用)

version: '3.9'

services:
  # 開發環境啟用熱重載
  agent-service:
    environment:
      DOTNET_USE_POLLING_FILE_WATCHER: "true"
    volumes:
      - ../../src/AgentService:/app:delegated
      - /app/bin
      - /app/obj

  persona-service:
    environment:
      DOTNET_USE_POLLING_FILE_WATCHER: "true"
    volumes:
      - ../../src/PersonaService:/app:delegated
      - /app/bin
      - /app/obj

  # 開發環境啟用詳細日誌
  api-gateway:
    environment:
      Logging__LogLevel__Default: Debug
      Logging__LogLevel__Microsoft: Information

  # PostgreSQL 啟用查詢日誌
  postgresql:
    command: postgres -c log_statement=all -c log_duration=on

  # Redis 啟用詳細日誌
  redis:
    command: redis-server --appendonly yes --requirepass redis_dev_password --loglevel debug
```

---

## 12.3 服務配置詳解

### 12.3.1 PostgreSQL 多數據庫初始化

**初始化腳本 (scripts/init-databases.sh):**

```bash
#!/bin/bash
set -e

# 從環境變數讀取數據庫列表
DATABASES=(auth_db agent_db persona_db code_db sql_db knowledge_db workflow_db)

echo "Initializing multiple databases..."

for DB in "${DATABASES[@]}"; do
  echo "Creating database: $DB"
  psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE DATABASE $DB;
    GRANT ALL PRIVILEGES ON DATABASE $DB TO $POSTGRES_USER;
EOSQL

  # 為 knowledge_db 安裝 pgvector 擴展
  if [ "$DB" = "knowledge_db" ]; then
    echo "Installing pgvector extension for $DB"
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$DB" <<-EOSQL
      CREATE EXTENSION IF NOT EXISTS vector;
EOSQL
  fi
done

echo "All databases created successfully"
```

**權限設置:**

```bash
chmod +x scripts/init-databases.sh
```

### 12.3.2 Code Execution Service 沙箱配置

**Seccomp Profile (security/seccomp-profile.json):**

```json
{
  "defaultAction": "SCMP_ACT_ERRNO",
  "defaultErrnoRet": 1,
  "architectures": ["SCMP_ARCH_X86_64"],
  "syscalls": [
    {
      "names": [
        "read", "write", "open", "openat", "close",
        "stat", "fstat", "lstat", "lseek", "mmap",
        "mprotect", "munmap", "brk", "rt_sigaction",
        "rt_sigprocmask", "ioctl", "access", "pipe",
        "select", "sched_yield", "mremap", "msync",
        "mincore", "madvise", "shmget", "shmat", "shmctl",
        "dup", "dup2", "pause", "nanosleep", "getitimer",
        "alarm", "setitimer", "getpid", "sendfile", "socket"
      ],
      "action": "SCMP_ACT_ALLOW"
    },
    {
      "names": ["ptrace", "reboot", "mount", "umount", "swapon", "swapoff"],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block dangerous syscalls"
    }
  ]
}
```

**Code Execution Dockerfile (src/CodeExecutionService/Dockerfile):**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 安裝 Docker CLI (用於創建沙箱容器)
RUN apt-get update && apt-get install -y \
    docker.io \
    curl \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CodeExecutionService.csproj", "./"]
RUN dotnet restore "CodeExecutionService.csproj"
COPY . .
RUN dotnet build "CodeExecutionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeExecutionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 複製 Seccomp profile
COPY security/seccomp-profile.json /etc/docker/seccomp/code-sandbox.json

ENTRYPOINT ["dotnet", "CodeExecutionService.dll"]
```

### 12.3.3 Prometheus 配置

**監控配置 (monitoring/prometheus/prometheus.yml):**

```yaml
# Prometheus 配置
global:
  scrape_interval: 15s
  evaluation_interval: 15s
  external_labels:
    environment: 'development'

# 告警規則
rule_files:
  - 'alerts/*.yml'

# 抓取配置
scrape_configs:
  # Prometheus 自身
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']

  # API Gateway
  - job_name: 'api-gateway'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['api-gateway:8080']
        labels:
          service: 'api-gateway'

  # Auth Service
  - job_name: 'auth-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['auth-service:8080']
        labels:
          service: 'auth-service'

  # Agent Service
  - job_name: 'agent-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['agent-service:8080']
        labels:
          service: 'agent-service'

  # Persona Service
  - job_name: 'persona-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['persona-service:8080']
        labels:
          service: 'persona-service'

  # Code Execution Service
  - job_name: 'code-execution-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['code-execution-service:8080']
        labels:
          service: 'code-execution-service'

  # Text-to-SQL Service
  - job_name: 'text-to-sql-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['text-to-sql-service:8080']
        labels:
          service: 'text-to-sql-service'

  # Knowledge Base Service
  - job_name: 'knowledge-base-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['knowledge-base-service:8080']
        labels:
          service: 'knowledge-base-service'

  # Workflow Service
  - job_name: 'workflow-service'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['workflow-service:8080']
        labels:
          service: 'workflow-service'

  # PostgreSQL Exporter
  - job_name: 'postgresql'
    static_configs:
      - targets: ['postgresql:5432']
        labels:
          service: 'postgresql'

  # Redis Exporter
  - job_name: 'redis'
    static_configs:
      - targets: ['redis:6379']
        labels:
          service: 'redis'

  # RabbitMQ
  - job_name: 'rabbitmq'
    static_configs:
      - targets: ['rabbitmq:15672']
        labels:
          service: 'rabbitmq'
```

### 12.3.4 Grafana Dashboard 配置

**數據源配置 (monitoring/grafana/provisioning/datasources/prometheus.yml):**

```yaml
apiVersion: 1

datasources:
  - name: Prometheus
    type: prometheus
    access: proxy
    url: http://prometheus:9090
    isDefault: true
    editable: false
    jsonData:
      timeInterval: 15s
```

**Dashboard 配置 (monitoring/grafana/provisioning/dashboards/default.yml):**

```yaml
apiVersion: 1

providers:
  - name: 'Default'
    orgId: 1
    folder: ''
    type: file
    disableDeletion: false
    updateIntervalSeconds: 10
    allowUiUpdates: true
    options:
      path: /var/lib/grafana/dashboards
```

---

## 12.4 網路配置

### 12.4.1 自定義網路

```yaml
networks:
  ai-workflow-network:
    driver: bridge
    ipam:
      driver: default
      config:
        - subnet: 172.28.0.0/16
          gateway: 172.28.0.1
    driver_opts:
      com.docker.network.bridge.name: ai-workflow-br0
      com.docker.network.bridge.enable_ip_masquerade: "true"
      com.docker.network.bridge.enable_icc: "true"
      com.docker.network.driver.mtu: "1500"
```

### 12.4.2 服務間通信

**DNS 解析:**
- Docker Compose 自動為每個服務創建 DNS 記錄
- 服務名稱即為主機名 (例如: `postgresql`, `redis`, `agent-service`)

**連接字串範例:**
```csharp
// PostgreSQL 連接
Host=postgresql;Database=agent_db;Username=postgres;Password=postgres_dev_password

// Redis 連接
redis:6379,password=redis_dev_password

// RabbitMQ 連接
amqp://rabbitmq:rabbitmq_dev_password@rabbitmq:5672

// 服務間 HTTP 調用
http://agent-service:8080/api/agents
```

### 12.4.3 外部訪問

**從主機訪問服務:**

```bash
# API Gateway
curl http://localhost:8000/health

# Auth Service 登入
curl -X POST http://localhost:8001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'

# Agent Service 列表
curl http://localhost:8002/api/agents \
  -H "Authorization: Bearer <token>"

# RabbitMQ Management UI
open http://localhost:15672  # username: rabbitmq, password: rabbitmq_dev_password

# Prometheus UI
open http://localhost:9090

# Grafana UI
open http://localhost:3000  # username: admin, password: admin
```

---

## 12.5 持久化配置

### 12.5.1 Named Volumes

```yaml
volumes:
  # PostgreSQL 數據持久化
  postgresql_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./data/postgresql

  # Redis 數據持久化
  redis_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./data/redis

  # RabbitMQ 數據持久化
  rabbitmq_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./data/rabbitmq

  # Prometheus 數據持久化
  prometheus_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./data/prometheus

  # Grafana 數據持久化
  grafana_data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: ./data/grafana
```

### 12.5.2 數據備份腳本

**備份腳本 (scripts/backup.sh):**

```bash
#!/bin/bash
# backup.sh - 備份所有數據

BACKUP_DIR="./backups/$(date +%Y%m%d_%H%M%S)"
mkdir -p $BACKUP_DIR

echo "Starting backup to $BACKUP_DIR..."

# 備份 PostgreSQL
echo "Backing up PostgreSQL..."
docker exec ai-workflow-postgresql pg_dumpall -U postgres > $BACKUP_DIR/postgresql_backup.sql
gzip $BACKUP_DIR/postgresql_backup.sql

# 備份 Redis
echo "Backing up Redis..."
docker exec ai-workflow-redis redis-cli --pass redis_dev_password SAVE
docker cp ai-workflow-redis:/data/dump.rdb $BACKUP_DIR/redis_dump.rdb

# 備份 RabbitMQ 配置
echo "Backing up RabbitMQ..."
docker exec ai-workflow-rabbitmq rabbitmqctl export_definitions $BACKUP_DIR/rabbitmq_definitions.json

# 備份 Grafana 儀表板
echo "Backing up Grafana..."
docker cp ai-workflow-grafana:/var/lib/grafana $BACKUP_DIR/grafana

echo "Backup completed: $BACKUP_DIR"
```

**恢復腳本 (scripts/restore.sh):**

```bash
#!/bin/bash
# restore.sh - 恢復數據

BACKUP_DIR=$1

if [ -z "$BACKUP_DIR" ]; then
  echo "Usage: ./restore.sh <backup_directory>"
  exit 1
fi

echo "Restoring from $BACKUP_DIR..."

# 恢復 PostgreSQL
echo "Restoring PostgreSQL..."
gunzip -c $BACKUP_DIR/postgresql_backup.sql.gz | \
  docker exec -i ai-workflow-postgresql psql -U postgres

# 恢復 Redis
echo "Restoring Redis..."
docker cp $BACKUP_DIR/redis_dump.rdb ai-workflow-redis:/data/dump.rdb
docker restart ai-workflow-redis

# 恢復 RabbitMQ
echo "Restoring RabbitMQ..."
docker exec ai-workflow-rabbitmq rabbitmqctl import_definitions $BACKUP_DIR/rabbitmq_definitions.json

echo "Restore completed"
```

---

## 12.6 環境變數管理

### 12.6.1 .env 文件

**根目錄 .env 文件:**

```bash
# .env
# 環境變數配置 (不要提交到 Git)

# OpenAI API Key
OPENAI_API_KEY=sk-your-openai-api-key-here

# Azure (生產環境使用)
AZURE_SUBSCRIPTION_ID=your-subscription-id
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-client-secret

# Database Passwords
POSTGRES_PASSWORD=postgres_dev_password
REDIS_PASSWORD=redis_dev_password
RABBITMQ_PASSWORD=rabbitmq_dev_password

# JWT Secret
JWT_SECRET=dev_jwt_secret_key_min_32_characters_long_for_hs256

# Log Level
LOG_LEVEL=Debug
```

**.env.example 文件 (提交到 Git):**

```bash
# .env.example
# 環境變數範本

# OpenAI API Key
OPENAI_API_KEY=sk-your-openai-api-key-here

# Azure (生產環境使用)
AZURE_SUBSCRIPTION_ID=
AZURE_TENANT_ID=
AZURE_CLIENT_ID=
AZURE_CLIENT_SECRET=

# Database Passwords
POSTGRES_PASSWORD=postgres_dev_password
REDIS_PASSWORD=redis_dev_password
RABBITMQ_PASSWORD=rabbitmq_dev_password

# JWT Secret (至少 32 字符)
JWT_SECRET=dev_jwt_secret_key_min_32_characters_long_for_hs256

# Log Level
LOG_LEVEL=Debug
```

### 12.6.2 環境變數加載

**在服務中使用環境變數:**

```yaml
services:
  agent-service:
    environment:
      # 直接引用 .env 文件中的變數
      OpenAI__ApiKey: ${OPENAI_API_KEY}
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=agent_db;Username=postgres;Password=${POSTGRES_PASSWORD}
      Redis__ConnectionString: redis:6379,password=${REDIS_PASSWORD}
```

**C# 代碼中讀取:**

```csharp
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 從環境變數讀取配置
        var openAIKey = Configuration["OpenAI:ApiKey"];
        var dbConnection = Configuration["ConnectionStrings:DefaultConnection"];

        // 綁定到強型別配置類
        services.Configure<OpenAIOptions>(Configuration.GetSection("OpenAI"));
        services.Configure<DatabaseOptions>(Configuration.GetSection("ConnectionStrings"));
    }
}
```

---

## 12.7 開發工作流

### 12.7.1 快速啟動命令

**完整啟動:**

```bash
# 1. 複製環境變數範本
cp .env.example .env

# 2. 編輯 .env 填入 OpenAI API Key
nano .env

# 3. 啟動所有服務
docker-compose up -d

# 4. 查看日誌
docker-compose logs -f

# 5. 查看服務狀態
docker-compose ps

# 6. 健康檢查
curl http://localhost:8000/health
```

**選擇性啟動:**

```bash
# 只啟動基礎設施
docker-compose up -d postgresql redis rabbitmq

# 只啟動特定服務 (用於開發單個服務)
docker-compose up -d agent-service

# 啟動監控服務
docker-compose up -d prometheus grafana
```

### 12.7.2 開發調試流程

**熱重載開發 (使用 override 配置):**

```bash
# 1. 啟動服務 (自動應用 docker-compose.override.yml)
docker-compose up -d agent-service

# 2. 查看實時日誌
docker-compose logs -f agent-service

# 3. 代碼修改後自動重新編譯 (DOTNET_USE_POLLING_FILE_WATCHER=true)
# 編輯 src/AgentService/Controllers/AgentsController.cs

# 4. 測試 API
curl http://localhost:8002/api/agents
```

**調試特定服務:**

```bash
# 進入容器調試
docker-compose exec agent-service bash

# 查看環境變數
docker-compose exec agent-service env | grep OpenAI

# 執行資料庫遷移
docker-compose exec agent-service dotnet ef database update

# 重啟單個服務
docker-compose restart agent-service
```

### 12.7.3 測試工作流

**運行集成測試:**

```bash
# 1. 啟動測試環境
docker-compose -f docker-compose.test.yml up -d

# 2. 運行測試
docker-compose -f docker-compose.test.yml run --rm integration-tests

# 3. 清理測試環境
docker-compose -f docker-compose.test.yml down -v
```

**docker-compose.test.yml 範例:**

```yaml
version: '3.9'

services:
  # 繼承基礎配置
  postgresql:
    extends:
      file: docker-compose.yml
      service: postgresql
    environment:
      POSTGRES_PASSWORD: test_password

  agent-service:
    extends:
      file: docker-compose.yml
      service: agent-service
    environment:
      ASPNETCORE_ENVIRONMENT: Test
      ConnectionStrings__DefaultConnection: Host=postgresql;Database=agent_db_test;Username=postgres;Password=test_password

  # 集成測試容器
  integration-tests:
    build:
      context: ../../tests/IntegrationTests
      dockerfile: Dockerfile
    environment:
      API_BASE_URL: http://agent-service:8080
    depends_on:
      - agent-service
    networks:
      - ai-workflow-network
```

### 12.7.4 清理命令

```bash
# 停止所有服務
docker-compose down

# 停止並刪除 volumes (清除所有數據)
docker-compose down -v

# 重建所有映像
docker-compose build --no-cache

# 完全清理 (刪除所有容器、網路、映像)
docker-compose down --rmi all --volumes --remove-orphans

# 清理未使用的 Docker 資源
docker system prune -a --volumes
```

---

## 12.8 故障排除

### 12.8.1 常見問題

**問題 1: 服務啟動失敗 - 端口已被佔用**

```bash
# 錯誤訊息:
# Error starting userland proxy: listen tcp4 0.0.0.0:8080: bind: address already in use

# 解決方法:
# 1. 查找佔用端口的進程
netstat -ano | findstr :8080  # Windows
lsof -i :8080                 # Linux/Mac

# 2. 停止進程或修改 docker-compose.yml 中的端口映射
services:
  agent-service:
    ports:
      - "8082:8080"  # 改為 8082
```

**問題 2: 數據庫連接失敗**

```bash
# 錯誤訊息:
# Npgsql.NpgsqlException: Connection refused

# 解決方法:
# 1. 檢查 PostgreSQL 是否正在運行
docker-compose ps postgresql

# 2. 檢查健康狀態
docker-compose ps

# 3. 查看 PostgreSQL 日誌
docker-compose logs postgresql

# 4. 手動測試連接
docker-compose exec postgresql psql -U postgres -d agent_db
```

**問題 3: OpenAI API Key 無效**

```bash
# 錯誤訊息:
# OpenAI API error: Incorrect API key provided

# 解決方法:
# 1. 確認 .env 文件存在並包含正確的 API Key
cat .env | grep OPENAI_API_KEY

# 2. 重啟服務以重新載入環境變數
docker-compose restart agent-service

# 3. 驗證環境變數已載入
docker-compose exec agent-service env | grep OPENAI_API_KEY
```

**問題 4: Code Execution Service 權限錯誤**

```bash
# 錯誤訊息:
# Cannot connect to the Docker daemon

# 解決方法:
# 1. 確認 Docker socket 已掛載
docker-compose exec code-execution-service ls -la /var/run/docker.sock

# 2. 檢查權限
docker-compose exec code-execution-service docker ps

# 3. 如果是 Linux，將用戶加入 docker 組
sudo usermod -aG docker $USER
newgrp docker
```

### 12.8.2 調試工具

**日誌查看:**

```bash
# 查看所有服務日誌
docker-compose logs

# 查看特定服務日誌
docker-compose logs agent-service

# 實時跟蹤日誌
docker-compose logs -f agent-service

# 查看最近 100 行日誌
docker-compose logs --tail=100 agent-service

# 查看帶時間戳的日誌
docker-compose logs -t agent-service
```

**性能監控:**

```bash
# 查看資源使用情況
docker stats

# 查看特定容器資源使用
docker stats ai-workflow-agent

# 查看容器進程
docker top ai-workflow-agent
```

**網路調試:**

```bash
# 查看網路
docker network ls

# 查看網路詳情
docker network inspect ai-workflow-network

# 進入容器測試網路連接
docker-compose exec agent-service bash
curl http://postgresql:5432
curl http://redis:6379
curl http://rabbitmq:15672
```

### 12.8.3 性能優化

**減少啟動時間:**

```yaml
# 使用 build cache
services:
  agent-service:
    build:
      context: ../../src/AgentService
      dockerfile: Dockerfile
      cache_from:
        - aiworkflow.azurecr.io/agent-service:latest
```

**減少內存使用:**

```yaml
# 限制服務資源
services:
  agent-service:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 2G
        reservations:
          cpus: '0.5'
          memory: 1G
```

**加速數據庫查詢:**

```yaml
# PostgreSQL 性能優化
postgresql:
  command: postgres -c shared_buffers=256MB -c max_connections=200 -c effective_cache_size=1GB
```

---

## 附錄 A: Makefile 便捷命令

**Makefile:**

```makefile
# Makefile for AI Workflow Platform

.PHONY: help start stop restart logs build clean test backup restore

# 默認目標
help:
	@echo "AI Workflow Platform - Docker Compose Commands"
	@echo ""
	@echo "Usage:"
	@echo "  make start          - 啟動所有服務"
	@echo "  make stop           - 停止所有服務"
	@echo "  make restart        - 重啟所有服務"
	@echo "  make logs           - 查看所有日誌"
	@echo "  make build          - 重新構建所有映像"
	@echo "  make clean          - 清理所有容器和資料"
	@echo "  make test           - 運行集成測試"
	@echo "  make backup         - 備份數據"
	@echo "  make restore        - 恢復數據"
	@echo "  make ps             - 查看服務狀態"
	@echo "  make db-shell       - 進入 PostgreSQL Shell"
	@echo "  make redis-shell    - 進入 Redis Shell"

# 啟動服務
start:
	docker-compose up -d
	@echo "Services started. Access:"
	@echo "  API Gateway:  http://localhost:8000"
	@echo "  Grafana:      http://localhost:3000 (admin/admin)"
	@echo "  Prometheus:   http://localhost:9090"
	@echo "  RabbitMQ:     http://localhost:15672 (rabbitmq/rabbitmq_dev_password)"

# 停止服務
stop:
	docker-compose down

# 重啟服務
restart:
	docker-compose restart

# 查看日誌
logs:
	docker-compose logs -f

# 構建映像
build:
	docker-compose build --no-cache

# 清理
clean:
	docker-compose down -v --rmi all
	rm -rf ./data/*
	rm -rf ./logs/*

# 運行測試
test:
	docker-compose -f docker-compose.test.yml up --abort-on-container-exit
	docker-compose -f docker-compose.test.yml down -v

# 備份
backup:
	./scripts/backup.sh

# 恢復
restore:
	@read -p "Enter backup directory: " dir; \
	./scripts/restore.sh $$dir

# 查看服務狀態
ps:
	docker-compose ps

# PostgreSQL Shell
db-shell:
	docker-compose exec postgresql psql -U postgres

# Redis Shell
redis-shell:
	docker-compose exec redis redis-cli -a redis_dev_password

# 健康檢查
health:
	@echo "Checking service health..."
	@curl -s http://localhost:8000/health | jq . || echo "API Gateway: DOWN"
	@curl -s http://localhost:8001/health | jq . || echo "Auth Service: DOWN"
	@curl -s http://localhost:8002/health | jq . || echo "Agent Service: DOWN"
```

**使用範例:**

```bash
# 顯示幫助
make help

# 啟動所有服務
make start

# 查看實時日誌
make logs

# 進入數據庫 Shell
make db-shell

# 備份數據
make backup

# 清理環境
make clean
```

---

## 總結

本文檔提供了 AI Workflow Platform 的完整 Docker Compose 配置，涵蓋:

1. **完整配置**: 8 個應用服務 + 3 個基礎設施服務 + 2 個監控服務
2. **網路配置**: 自定義橋接網路，服務間 DNS 解析
3. **持久化**: Named Volumes 數據持久化，備份/恢復腳本
4. **環境管理**: .env 文件管理敏感信息
5. **開發工作流**: 熱重載、調試、測試流程
6. **故障排除**: 常見問題解決方案和調試工具

**快速啟動:**
```bash
cp .env.example .env
# 編輯 .env 填入 OPENAI_API_KEY
make start
```

**相關文檔:**
- 11-DEPLOYMENT-ARCHITECTURE.md - 部署架構總覽
- 13-KUBERNETES.md - Kubernetes 生產環境配置
- 14-CICD-PIPELINE.md - CI/CD 自動化流水線

---

**版本歷史:**

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0.0 | 2025-01-15 | AI Workflow Team | 初始版本 |
