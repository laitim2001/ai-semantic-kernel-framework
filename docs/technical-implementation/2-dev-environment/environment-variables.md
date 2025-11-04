# 環境變數配置指南

## 版本信息
- **文檔版本**: 1.0
- **最後更新**: 2025-01-01

## 後端 API (.NET) 環境變數

### .env 文件模板

創建 `backend/.env`:

```env
# ===========================================
# Application Settings
# ===========================================
ASPNETCORE_ENVIRONMENT=Development
APP_NAME=AI Workflow Platform
APP_VERSION=1.0.0

# ===========================================
# Server Configuration
# ===========================================
ASPNETCORE_URLS=https://localhost:7001;http://localhost:5001
ASPNETCORE_HTTPS_PORT=7001

# ===========================================
# Database - PostgreSQL
# ===========================================
ConnectionStrings__PostgreSQL=Host=localhost;Port=5432;Database=aiworkflow;Username=postgres;Password=postgres123

# ===========================================
# Cache - Redis
# ===========================================
Redis__Host=localhost
Redis__Port=6379
Redis__Password=redis123
Redis__Database=0
Redis__SslEnabled=false

# ===========================================
# Database - MongoDB
# ===========================================
MongoDB__ConnectionString=mongodb://admin:mongo123@localhost:27017
MongoDB__DatabaseName=aiworkflow
MongoDB__CollectionName=workflows

# ===========================================
# Azure OpenAI Configuration
# ===========================================
AzureOpenAI__Endpoint=https://your-resource.openai.azure.com/
AzureOpenAI__ApiKey=your-api-key-here
AzureOpenAI__DeploymentName=gpt-4
AzureOpenAI__ApiVersion=2024-02-01
AzureOpenAI__MaxTokens=4096
AzureOpenAI__Temperature=0.7

# Azure OpenAI Embedding
AzureOpenAI__EmbeddingDeployment=text-embedding-3-small
AzureOpenAI__EmbeddingEndpoint=https://your-resource.openai.azure.com/
AzureOpenAI__EmbeddingApiKey=your-api-key-here
AzureOpenAI__EmbeddingDimensions=1536

# ===========================================
# Azure AI Search (for RAG)
# ===========================================
AzureAISearch__Endpoint=https://your-search-service.search.windows.net
AzureAISearch__ApiKey=your-search-api-key
AzureAISearch__IndexName=knowledge-base

# ===========================================
# Logging Configuration
# ===========================================
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__System=Warning

# ===========================================
# CORS Settings
# ===========================================
CORS__AllowedOrigins=http://localhost:3000,http://localhost:5173
CORS__AllowCredentials=true

# ===========================================
# JWT Authentication
# ===========================================
JWT__SecretKey=your-super-secret-jwt-key-change-this-in-production
JWT__Issuer=AIWorkflowAPI
JWT__Audience=AIWorkflowClient
JWT__ExpirationMinutes=60

# ===========================================
# File Upload Settings
# ===========================================
FileUpload__MaxFileSize=10485760
FileUpload__AllowedExtensions=.pdf,.txt,.docx,.md
FileUpload__StoragePath=./uploads

# ===========================================
# Rate Limiting
# ===========================================
RateLimit__PermitLimit=100
RateLimit__WindowSeconds=60

# ===========================================
# Feature Flags
# ===========================================
Features__EnableSwagger=true
Features__EnableDetailedErrors=true
Features__EnableHealthChecks=true
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*",
  "DetailedErrors": true
}
```

## 前端 (React/Vue) 環境變數

### React - .env.local

創建 `frontend-react/.env.local`:

```env
# ===========================================
# API Configuration
# ===========================================
VITE_API_BASE_URL=http://localhost:5001
VITE_API_TIMEOUT=30000

# ===========================================
# WebSocket Configuration
# ===========================================
VITE_WS_URL=ws://localhost:5001/ws

# ===========================================
# Authentication
# ===========================================
VITE_AUTH_ENABLED=true
VITE_TOKEN_STORAGE_KEY=aiworkflow_token

# ===========================================
# Feature Flags
# ===========================================
VITE_ENABLE_DEBUG=true
VITE_ENABLE_ANALYTICS=false
VITE_ENABLE_ERROR_REPORTING=true

# ===========================================
# Application Metadata
# ===========================================
VITE_APP_NAME=AI Workflow Platform
VITE_APP_VERSION=1.0.0
VITE_APP_DESCRIPTION=Enterprise AI Workflow Management
```

### Vue 3 - .env.local

創建 `frontend-vue/.env.local`:

```env
# ===========================================
# API Configuration
# ===========================================
VITE_API_BASE_URL=http://localhost:5001
VITE_API_TIMEOUT=30000

# ===========================================
# WebSocket Configuration
# ===========================================
VITE_WS_URL=ws://localhost:3001

# ===========================================
# Y.js CRDT Configuration
# ===========================================
VITE_YJS_ROOM_PREFIX=aiworkflow
VITE_YJS_PROVIDER=websocket

# ===========================================
# Authentication
# ===========================================
VITE_AUTH_ENABLED=true
VITE_TOKEN_STORAGE_KEY=aiworkflow_token

# ===========================================
# Feature Flags
# ===========================================
VITE_ENABLE_DEBUG=true
VITE_ENABLE_DEVTOOLS=true
VITE_ENABLE_COLLABORATION=true

# ===========================================
# VueFlow Configuration
# ===========================================
VITE_VUEFLOW_SNAP_TO_GRID=true
VITE_VUEFLOW_GRID_SIZE=20

# ===========================================
# Application Metadata
# ===========================================
VITE_APP_NAME=AI Workflow Builder
VITE_APP_VERSION=1.0.0
```

## Docker Compose 環境變數

### .env (用於 docker-compose.yml)

```env
# ===========================================
# PostgreSQL Configuration
# ===========================================
POSTGRES_DB=aiworkflow
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123

# ===========================================
# Redis Configuration
# ===========================================
REDIS_PASSWORD=redis123

# ===========================================
# MongoDB Configuration
# ===========================================
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=mongo123
MONGO_INITDB_DATABASE=aiworkflow

# ===========================================
# pgAdmin Configuration
# ===========================================
PGADMIN_EMAIL=admin@example.com
PGADMIN_PASSWORD=admin123

# ===========================================
# Container Resource Limits
# ===========================================
POSTGRES_MEMORY_LIMIT=2g
REDIS_MEMORY_LIMIT=1g
MONGODB_MEMORY_LIMIT=2g
```

## 環境變數載入順序

### .NET (後端)

1. `appsettings.json` (基礎配置)
2. `appsettings.{Environment}.json` (環境特定配置)
3. 環境變數 (覆蓋文件配置)
4. 命令行參數 (最高優先級)

### React/Vue (前端)

1. `.env` (所有環境共享)
2. `.env.local` (本地開發,不提交到 Git)
3. `.env.{mode}` (特定模式,如 `.env.production`)
4. `.env.{mode}.local` (特定模式的本地配置)

**優先級**: `.env.{mode}.local` > `.env.{mode}` > `.env.local` > `.env`

## 安全最佳實踐

### 1. 敏感信息管理

```bash
# .gitignore 應包含
.env
.env.local
.env.*.local
appsettings.Development.json
```

### 2. 生產環境配置

**使用 Azure Key Vault**:
```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

**使用環境變數 (推薦)**:
```bash
# 在生產環境設置
export AzureOpenAI__ApiKey="production-key"
export JWT__SecretKey="production-secret"
```

### 3. 開發環境隔離

創建 `.env.example` (提交到 Git):
```env
# 示例配置 - 不包含敏感信息
ASPNETCORE_ENVIRONMENT=Development
VITE_API_BASE_URL=http://localhost:5001
# 開發者需要替換以下值
AzureOpenAI__ApiKey=your-api-key-here
JWT__SecretKey=your-secret-here
```

團隊成員複製並修改:
```bash
cp .env.example .env
# 編輯 .env 填入真實值
```

## 驗證環境變數

### 後端驗證腳本 (C#)

```csharp
// EnvironmentValidator.cs
public static class EnvironmentValidator
{
    public static void ValidateRequiredSettings(IConfiguration config)
    {
        var required = new[]
        {
            "AzureOpenAI:ApiKey",
            "AzureOpenAI:Endpoint",
            "ConnectionStrings:PostgreSQL",
            "JWT:SecretKey"
        };

        var missing = required.Where(key =>
            string.IsNullOrEmpty(config[key])).ToList();

        if (missing.Any())
        {
            throw new InvalidOperationException(
                $"Missing required configuration: {string.Join(", ", missing)}");
        }
    }
}
```

### 前端驗證 (TypeScript)

```typescript
// env-validator.ts
const requiredEnvVars = [
  'VITE_API_BASE_URL',
  'VITE_WS_URL',
] as const;

export function validateEnv(): void {
  const missing = requiredEnvVars.filter(
    key => !import.meta.env[key]
  );

  if (missing.length > 0) {
    throw new Error(
      `Missing required environment variables: ${missing.join(', ')}`
    );
  }
}
```

## 常見問題

### 問題 1: 環境變數未生效

**解決方案**:
```bash
# 重啟開發服務器
# .NET
dotnet run

# React/Vue
npm run dev
```

### 問題 2: CORS 錯誤

檢查 `CORS__AllowedOrigins` 是否包含前端 URL:
```env
CORS__AllowedOrigins=http://localhost:3000,http://localhost:5173
```

### 問題 3: 資料庫連接失敗

驗證連接字串格式:
```env
# 正確格式
ConnectionStrings__PostgreSQL=Host=localhost;Port=5432;Database=aiworkflow;Username=postgres;Password=postgres123

# 錯誤 - 缺少分號
ConnectionStrings__PostgreSQL=Host=localhost Port=5432 ...
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
