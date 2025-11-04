# 開發環境設置指南

**版本**: 2.0.0
**日期**: 2025-10-30
**狀態**: 📋 準備就緒
**目的**: 開發者本地環境快速設置

**基於**: TECH-STACK-ANALYSIS v2.0.0 (45 項技術確認)

---

## 🎯 設置目標

新開發者應能在 **1 小時內**完成以下設置：

- ✅ 安裝所有必需工具
- ✅ 啟動本地開發環境
- ✅ 運行後端 API Server (.NET 8)
- ✅ 運行 React 19 主應用
- ✅ 運行 Vue 3 Workflow Editor
- ✅ 驗證所有依賴服務正常

---

## 📋 系統要求

### 最低配置

| 組件 | 要求 |
|------|------|
| **作業系統** | Windows 11 / macOS 14+ / Ubuntu 22.04+ |
| **處理器** | Intel i5 / AMD Ryzen 5 (4 核心+) |
| **記憶體** | 16GB RAM (推薦 32GB) |
| **硬碟空間** | 50GB+ 可用空間 (SSD 推薦) |
| **網路** | 穩定網路連接（用於下載依賴） |

### 推薦配置

| 組件 | 推薦 |
|------|------|
| **處理器** | Intel i7 / AMD Ryzen 7 (8 核心+) |
| **記憶體** | 32GB+ RAM |
| **硬碟** | NVMe SSD 100GB+ |

---

## 📦 必需工具清單

基於 TECH-STACK-ANALYSIS v2.0.0 確認的版本：

### 後端開發工具

| 工具 | 版本 | 用途 | 安裝指南 |
|------|------|------|----------|
| **.NET SDK** | **8.0.x LTS** | 後端開發 | [setup-guide-windows.md](#) / [setup-guide-macos.md](#) |
| **Visual Studio 2022** | 17.8+ (可選) | IDE | Windows 推薦 |
| **Rider** | 2024.3+ (可選) | IDE | 跨平台替代方案 |

### 前端開發工具

| 工具 | 版本 | 用途 | 安裝指南 |
|------|------|------|----------|
| **Node.js** | **20.x LTS** | 前端開發 | [setup-guide-windows.md](#) / [setup-guide-macos.md](#) |
| **npm** | 10.x+ | 套件管理 | 隨 Node.js 安裝 |
| **VS Code** | 最新 | 前端 IDE | 推薦編輯器 |

### 容器化與數據庫

| 工具 | 版本 | 用途 | 安裝指南 |
|------|------|------|----------|
| **Docker Desktop** | 4.26+ | 容器化 | [setup-guide-windows.md](#) / [setup-guide-macos.md](#) |
| **PostgreSQL Client** | 16.x | 數據庫管理 | 可選（GUI 工具） |
| **Redis CLI** | 7.x | 緩存管理 | 可選 |

### 雲端工具

| 工具 | 版本 | 用途 | 安裝指南 |
|------|------|------|----------|
| **Azure CLI** | 2.55+ | Azure 資源管理 | [azure-resources-setup.md](#) |
| **kubectl** | 1.28+ | Kubernetes 管理 | 可選（生產部署） |

### 版本控制

| 工具 | 版本 | 用途 | 安裝指南 |
|------|------|------|----------|
| **Git** | 2.40+ | 版本控制 | 必需 |
| **GitHub CLI** | 2.40+ | GitHub 操作 | 可選 |

---

## 🚀 快速開始

### 1. 克隆代碼庫

```bash
# 克隆主倉庫
git clone https://github.com/your-org/semantic-kernel-agentic.git
cd semantic-kernel-agentic

# 克隆 Workflow Editor（如果分離倉庫）
# git clone https://github.com/your-org/workflow-editor.git
```

### 2. 一鍵啟動本地環境

```bash
# 啟動所有依賴服務（PostgreSQL, Redis, MongoDB）
docker-compose up -d

# 等待服務啟動（約 30 秒）
docker-compose ps  # 驗證所有服務狀態為 Up
```

### 3. 初始化數據庫

```bash
# 安裝 EF Core CLI 工具（首次）
dotnet tool install --global dotnet-ef --version 9.0.x

# 應用數據庫遷移
cd src/Api
dotnet ef database update

# 種子數據（可選）
dotnet run --seed-data
```

### 4. 啟動後端 API Server

```bash
# 在 src/Api 目錄
cd src/Api
dotnet restore
dotnet run

# API 將運行在: https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### 5. 啟動 React 主應用

```bash
# 在新終端，導航到 src/Web
cd src/Web
npm install
npm run dev

# React 應用將運行在: http://localhost:5173
```

### 6. 啟動 Vue 3 Workflow Editor

```bash
# 在新終端，導航到 src/WorkflowEditor
cd src/WorkflowEditor
npm install
npm run dev

# Vue 應用將運行在: http://localhost:5174
```

### 7. 驗證環境

打開瀏覽器訪問：

- ✅ **React 主應用**: http://localhost:5173
- ✅ **Vue Workflow Editor**: http://localhost:5174
- ✅ **API Swagger**: https://localhost:5001/swagger
- ✅ **Health Check**: https://localhost:5001/health

---

## 🐳 Docker Compose 配置

### 服務概覽

```yaml
version: '3.8'

services:
  # PostgreSQL 16 - 主數據庫
  postgres:
    image: postgres:16-alpine
    container_name: sk-postgres
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: password123
      POSTGRES_DB: semantic_kernel_db
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U admin"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis 7 - 緩存與會話
  redis:
    image: redis:7-alpine
    container_name: sk-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: redis-server --appendonly yes
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  # MongoDB - Workflow Editor 日誌
  mongodb:
    image: mongo:7.0
    container_name: sk-mongodb
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: password123
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
    healthcheck:
      test: ["CMD", "mongosh", "--eval", "db.adminCommand('ping')"]
      interval: 10s
      timeout: 5s
      retries: 5

  # pgAdmin - PostgreSQL 管理界面（可選）
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: sk-pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    depends_on:
      - postgres

volumes:
  postgres_data:
  redis_data:
  mongodb_data:
```

### 常用命令

```bash
# 啟動所有服務
docker-compose up -d

# 查看服務狀態
docker-compose ps

# 查看日誌
docker-compose logs -f [service-name]

# 停止所有服務
docker-compose down

# 停止並刪除數據卷（重置環境）
docker-compose down -v

# 重啟特定服務
docker-compose restart postgres
```

---

## 🔧 環境變數配置

### 後端 API (.NET)

創建 `src/Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=semantic_kernel_db;Username=admin;Password=password123",
    "Redis": "localhost:6379",
    "MongoDB": "mongodb://admin:password123@localhost:27017"
  },
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "gpt-4o",
    "EmbeddingDeploymentName": "text-embedding-ada-002"
  },
  "AzureAISearch": {
    "Endpoint": "https://your-search.search.windows.net",
    "ApiKey": "your-search-key-here",
    "IndexName": "knowledge-base"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:5174"
    ]
  }
}
```

### React 主應用

創建 `src/Web/.env.local`:

```bash
# API Endpoint
VITE_API_BASE_URL=https://localhost:5001
VITE_WS_URL=wss://localhost:5001/ws

# Feature Flags
VITE_ENABLE_CODE_INTERPRETER=true
VITE_ENABLE_TEXT_TO_SQL=true
VITE_ENABLE_WORKFLOW_EDITOR=true

# Analytics (可選)
VITE_ANALYTICS_ID=

# Environment
VITE_ENV=development
```

### Vue 3 Workflow Editor

創建 `src/WorkflowEditor/.env.local`:

```bash
# API Endpoint
VITE_API_BASE_URL=https://localhost:5001
VITE_WS_URL=wss://localhost:5001/ws

# MongoDB (日誌存儲)
VITE_MONGODB_URL=mongodb://admin:password123@localhost:27017

# Workflow Engine
VITE_ENABLE_REALTIME_COLLABORATION=true
VITE_ENABLE_VERSION_CONTROL=true

# Environment
VITE_ENV=development
```

---

## ✅ 環境驗證清單

運行以下命令驗證環境設置：

```bash
# 1. 檢查工具版本
dotnet --version          # 應顯示 8.0.x
node --version            # 應顯示 v20.x
npm --version             # 應顯示 10.x
docker --version          # 應顯示 24.x+
git --version             # 應顯示 2.40+

# 2. 檢查 Docker 服務
docker-compose ps         # 所有服務應為 Up (healthy)

# 3. 測試數據庫連接
psql -h localhost -U admin -d semantic_kernel_db -c "\dt"  # PostgreSQL
redis-cli ping            # 應返回 PONG
mongosh --eval "db.version()"  # MongoDB

# 4. 測試 API 健康檢查
curl https://localhost:5001/health  # 應返回 Healthy

# 5. 測試前端應用
curl http://localhost:5173  # React 主應用
curl http://localhost:5174  # Vue Workflow Editor
```

所有檢查項應通過 ✅

---

## 📚 詳細設置指南

- [Windows 開發環境設置](./setup-guide-windows.md) - Windows 11 詳細步驟
- [macOS 開發環境設置](./setup-guide-macos.md) - macOS 詳細步驟
- [Docker Compose 配置詳解](./docker-compose-dev.yml) - 完整配置文件
- [環境變數說明](./environment-variables.md) - 所有環境變數詳解
- [數據庫設置](./database-setup.md) - PostgreSQL / MongoDB 初始化
- [Azure 資源設置](./azure-resources-setup.md) - Azure OpenAI / AI Search
- [常見問題排查](./troubleshooting.md) - 環境設置常見問題

---

## 🆘 故障排除

### 問題 1: Docker 服務啟動失敗

```bash
# 檢查端口衝突
netstat -ano | findstr :5432  # Windows
lsof -i :5432                 # macOS/Linux

# 清理並重啟
docker-compose down -v
docker-compose up -d
```

### 問題 2: EF Core 遷移失敗

```bash
# 清理並重新遷移
dotnet ef database drop --force
dotnet ef database update

# 如果仍然失敗，檢查連接字符串
echo $DATABASE_URL  # 確認環境變數
```

### 問題 3: 前端無法連接 API

```bash
# 檢查 CORS 配置
# 確認 appsettings.Development.json 中的 AllowedOrigins

# 檢查前端環境變數
cat src/Web/.env.local  # 確認 VITE_API_BASE_URL
```

更多問題請參閱 [troubleshooting.md](./troubleshooting.md)

---

## 🎓 下一步

環境設置完成後，請繼續：

1. **閱讀架構文檔**: [Architecture Overview](../12-developer-onboarding/architecture-overview.md)
2. **熟悉項目結構**: [Codebase Walkthrough](../12-developer-onboarding/codebase-walkthrough.md)
3. **運行測試**: `dotnet test` (後端) / `npm test` (前端)
4. **開始開發**: 參考 [Common Tasks](../12-developer-onboarding/common-tasks.md)

---

## 🔗 相關文檔

- [TECH-STACK-ANALYSIS.md](../TECH-STACK-ANALYSIS.md) - 技術棧分析
- [IMPLEMENTATION-STRUCTURE.md](../IMPLEMENTATION-STRUCTURE.md) - 文檔結構
- [Project Structure](../3-project-structure/README.md) - 項目結構說明

---

**最後更新**: 2025-10-30
**維護者**: DevOps Team
**支持**: dev-support@example.com

---

[← 返回 Technical Implementation](../README.md)
