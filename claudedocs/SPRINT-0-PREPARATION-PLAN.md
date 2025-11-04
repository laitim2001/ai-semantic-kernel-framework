# Sprint 0: 環境與基礎設施準備計劃

**Semantic Kernel Agentic Framework - Sprint 0 Detailed Plan**

**版本**: 1.0.0
**創建日期**: 2025-11-03
**執行時間**: Week 4 (1週)
**狀態**: 📋 計劃完成,準備執行
**負責人**: DevOps Lead + Tech Lead

[返回開發計劃](../docs/COMPLETE-DEVELOPMENT-PLAN.md) | [Sprint Allocation](../docs/user-stories/sprints/sprint-allocation.md)

---

## 📋 目錄

1. [Sprint 0 目標與範圍](#sprint-0-目標與範圍)
2. [Day 1-2: 開發環境設置](#day-1-2-開發環境設置)
3. [Day 3-4: Azure 雲端資源部署](#day-3-4-azure-雲端資源部署)
4. [Day 5: CI/CD Pipeline 建立](#day-5-cicd-pipeline-建立)
5. [驗收標準 (DoD)](#驗收標準-dod)
6. [風險與緩解措施](#風險與緩解措施)
7. [下一步: Sprint 1 啟動](#下一步-sprint-1-啟動)

---

## Sprint 0 目標與範圍

### 🎯 核心目標

**主要目標**: 建立完整的開發、測試、部署基礎設施,確保 Sprint 1 團隊可以立即開始編碼

**成功標準**:
```yaml
環境就緒:
  - ✅ 本地開發環境 (3+ 開發者驗證)
  - ✅ Azure 資源部署完成 (Dev, Test 環境)
  - ✅ CI/CD Pipeline 運行成功
  - ✅ 開發文檔完整且可訪問

技術驗證:
  - ✅ Monorepo 架構正常運作
  - ✅ .NET + React 編譯無誤
  - ✅ 單元測試框架可執行
  - ✅ Docker 容器構建成功

團隊準備:
  - ✅ 所有開發者完成環境設置
  - ✅ Coding Standards 已宣導
  - ✅ Git Workflow 達成共識
  - ✅ Sprint 1 User Stories 已準備
```

### 📦 交付物清單

| # | 交付物 | 負責人 | 完成日期 | 狀態 |
|---|--------|--------|---------|------|
| 1 | Monorepo 初始化 (pnpm + Turborepo) | Tech Lead | Day 1 | ⏳ Pending |
| 2 | Backend 項目腳手架 (.NET 8) | Backend Lead | Day 1-2 | ⏳ Pending |
| 3 | Frontend 項目腳手架 (React 18 + TS) | Frontend Lead | Day 1-2 | ⏳ Pending |
| 4 | Azure 資源部署腳本 (Bicep/Terraform) | DevOps Lead | Day 2-3 | ⏳ Pending |
| 5 | PostgreSQL Dev 實例 + Schema | Backend Lead | Day 3 | ⏳ Pending |
| 6 | Azure OpenAI 連接測試 | Backend Lead | Day 3 | ⏳ Pending |
| 7 | CI/CD Pipeline (Azure DevOps/GitHub Actions) | DevOps Lead | Day 4-5 | ⏳ Pending |
| 8 | Docker Compose 本地環境 | DevOps Lead | Day 4 | ⏳ Pending |
| 9 | 單元測試框架 (xUnit + Vitest) | QA Lead | Day 4 | ⏳ Pending |
| 10 | E2E 測試框架 (Playwright) | QA Lead | Day 5 | ⏳ Pending |
| 11 | 開發文檔更新 (Setup Guide) | Tech Lead | Day 5 | ⏳ Pending |
| 12 | Sprint 1 Kickoff 簡報 | PO + Scrum Master | Day 5 | ⏳ Pending |

**總計**: 12 個核心交付物

---

## Day 1-2: 開發環境設置

### Day 1 Morning: Monorepo 初始化

**負責人**: Tech Lead + Backend Lead

**任務清單**:

#### 1.1 創建 Monorepo 根目錄結構

```bash
# 項目根目錄
C:\AI Semantic Kernel\
├── apps/                          # 應用程序
│   ├── web-app/                   # React 主應用
│   ├── workflow-editor/           # Vue 3 Workflow Editor (Phase 3)
│   └── api-gateway/               # .NET API Gateway (Optional)
├── packages/                      # 共享包
│   ├── dotnet-sdk/                # .NET Client SDK
│   ├── python-sdk/                # Python Client SDK
│   ├── ui-components/             # React 共享組件
│   └── shared-types/              # TypeScript 共享類型
├── services/                      # 後端服務
│   ├── agent-service/             # Agent 管理服務
│   ├── knowledge-service/         # Knowledge 管理服務
│   ├── code-interpreter-service/  # Code Interpreter 服務
│   └── workflow-service/          # Workflow 編排服務
├── infrastructure/                # 基礎設施
│   ├── bicep/                     # Azure Bicep 模板
│   ├── terraform/                 # Terraform (備選)
│   └── docker/                    # Docker Compose 文件
├── tests/                         # 測試
│   ├── unit/                      # 單元測試
│   ├── integration/               # 集成測試
│   └── e2e/                       # E2E 測試
├── docs/                          # 文檔 (已存在)
├── claudedocs/                    # Claude 文檔 (已存在)
├── poc-projects/                  # PoC 項目 (已存在)
├── .github/                       # GitHub Actions
│   └── workflows/
├── pnpm-workspace.yaml            # pnpm Workspace 配置
├── turbo.json                     # Turborepo 配置
├── package.json                   # 根 package.json
├── .gitignore                     # Git 忽略文件
└── README.md                      # 項目 README
```

**執行步驟**:

```bash
# 1. 初始化 pnpm 工作區
cd "C:\AI Semantic Kernel"
pnpm init

# 2. 創建 pnpm-workspace.yaml
cat > pnpm-workspace.yaml << 'EOF'
packages:
  - 'apps/*'
  - 'packages/*'
  - 'services/*'
EOF

# 3. 安裝 Turborepo
pnpm add -Dw turbo

# 4. 創建 turbo.json
cat > turbo.json << 'EOF'
{
  "$schema": "https://turbo.build/schema.json",
  "pipeline": {
    "build": {
      "dependsOn": ["^build"],
      "outputs": ["dist/**", "build/**", ".next/**", "bin/**", "obj/**"]
    },
    "test": {
      "dependsOn": ["build"],
      "outputs": ["coverage/**"]
    },
    "lint": {
      "outputs": []
    },
    "dev": {
      "cache": false,
      "persistent": true
    }
  }
}
EOF

# 5. 創建目錄結構
mkdir -p apps/web-app apps/workflow-editor apps/api-gateway
mkdir -p packages/dotnet-sdk packages/python-sdk packages/ui-components packages/shared-types
mkdir -p services/agent-service services/knowledge-service services/code-interpreter-service services/workflow-service
mkdir -p infrastructure/bicep infrastructure/terraform infrastructure/docker
mkdir -p tests/unit tests/integration tests/e2e
mkdir -p .github/workflows
```

**驗證**:
```bash
# 驗證 pnpm 工作區
pnpm list -r --depth 0

# 驗證 Turborepo 配置
pnpm turbo run build --dry-run
```

**預計時間**: 2 小時

---

#### 1.2 Backend 項目腳手架 (.NET 8)

**負責人**: Backend Lead

**任務清單**:

1. **創建 Agent Service (.NET 8 Web API)**

```bash
cd services/agent-service

# 創建 .NET 8 Web API 項目
dotnet new webapi -n AgentService -f net8.0

# 項目結構
AgentService/
├── Controllers/
│   └── AgentsController.cs
├── Models/
│   ├── Agent.cs
│   └── AgentDto.cs
├── Services/
│   ├── IAgentService.cs
│   └── AgentService.cs
├── Data/
│   ├── AgentDbContext.cs
│   └── Repositories/
│       ├── IAgentRepository.cs
│       └── AgentRepository.cs
├── Configuration/
│   └── SemanticKernelConfig.cs
├── Middleware/
│   └── ExceptionMiddleware.cs
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── AgentService.csproj
```

2. **安裝核心 NuGet 套件**

```bash
cd AgentService

# Semantic Kernel
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI --version 1.66.0

# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0

# ASP.NET Core
dotnet add package Microsoft.AspNetCore.OpenApi --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0

# 驗證與安全
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0

# 測試
dotnet add package xUnit --version 2.9.0
dotnet add package Moq --version 4.20.0
dotnet add package FluentAssertions --version 6.12.0
```

3. **Program.cs 基礎配置**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework Core
builder.Services.AddDbContext<AgentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Semantic Kernel
builder.Services.AddSingleton(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endpoint: builder.Configuration["AzureOpenAI:Endpoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
    );
    return kernelBuilder.Build();
});

// Application Services
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IAgentService, AgentService>();

// CORS (開發環境)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**驗證**:
```bash
# 編譯測試
dotnet build

# 運行測試
dotnet run
# 預期: https://localhost:7001/swagger
```

**預計時間**: 3 小時

---

#### 1.3 Frontend 項目腳手架 (React 18 + TypeScript)

**負責人**: Frontend Lead

**任務清單**:

1. **創建 React 18 + Vite 項目**

```bash
cd apps/web-app

# 使用 Vite 創建 React + TypeScript 項目
pnpm create vite web-app --template react-ts

cd web-app

# 項目結構
web-app/
├── src/
│   ├── components/
│   │   ├── agents/
│   │   │   ├── AgentCreateForm.tsx
│   │   │   ├── AgentList.tsx
│   │   │   └── AgentDetail.tsx
│   │   ├── chat/
│   │   │   ├── ChatWindow.tsx
│   │   │   ├── MessageList.tsx
│   │   │   └── ChatInput.tsx
│   │   ├── common/
│   │   │   ├── Layout.tsx
│   │   │   ├── Header.tsx
│   │   │   └── Sidebar.tsx
│   │   └── ui/
│   │       └── Button.tsx
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   ├── Agents.tsx
│   │   ├── Chat.tsx
│   │   └── Settings.tsx
│   ├── services/
│   │   ├── api.ts
│   │   └── agentService.ts
│   ├── stores/
│   │   ├── agentStore.ts
│   │   └── chatStore.ts
│   ├── types/
│   │   └── agent.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── vite-env.d.ts
├── public/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── README.md
```

2. **安裝核心依賴**

```bash
# UI 框架
pnpm add @mui/material @mui/icons-material @emotion/react @emotion/styled

# 路由
pnpm add react-router-dom

# 狀態管理
pnpm add zustand

# HTTP 請求
pnpm add axios

# 表單處理
pnpm add react-hook-form yup @hookform/resolvers

# 實時通信
pnpm add @microsoft/signalr

# Markdown 渲染
pnpm add react-markdown

# 圖表
pnpm add chart.js react-chartjs-2

# 代碼高亮
pnpm add react-syntax-highlighter @types/react-syntax-highlighter

# 開發依賴
pnpm add -D @types/react @types/react-dom
pnpm add -D vitest @vitest/ui @testing-library/react @testing-library/jest-dom
pnpm add -D eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
pnpm add -D prettier eslint-config-prettier
```

3. **Vite 配置 (vite.config.ts)**

```typescript
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@components': path.resolve(__dirname, './src/components'),
      '@pages': path.resolve(__dirname, './src/pages'),
      '@services': path.resolve(__dirname, './src/services'),
      '@stores': path.resolve(__dirname, './src/stores'),
      '@types': path.resolve(__dirname, './src/types'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7001',
        changeOrigin: true,
        secure: false,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/setupTests.ts',
  },
});
```

4. **TypeScript 配置 (tsconfig.json)**

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"],
      "@components/*": ["./src/components/*"],
      "@pages/*": ["./src/pages/*"],
      "@services/*": ["./src/services/*"],
      "@stores/*": ["./src/stores/*"],
      "@types/*": ["./src/types/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

5. **ESLint 配置 (.eslintrc.cjs)**

```javascript
// .eslintrc.cjs
module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
    'plugin:react-hooks/recommended',
    'prettier',
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs'],
  parser: '@typescript-eslint/parser',
  plugins: ['react-refresh'],
  rules: {
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    '@typescript-eslint/no-explicit-any': 'warn',
    '@typescript-eslint/no-unused-vars': ['error', { argsIgnorePattern: '^_' }],
  },
};
```

**驗證**:
```bash
# 編譯測試
pnpm build

# 運行開發服務器
pnpm dev
# 預期: http://localhost:5173

# Lint 檢查
pnpm lint

# 單元測試
pnpm test
```

**預計時間**: 3 小時

---

### Day 1 Afternoon: Git Workflow 設置

**負責人**: Tech Lead

**任務清單**:

#### 1.4 Git 分支策略與保護規則

**分支策略**:

```yaml
主分支:
  main:
    用途: 生產環境代碼
    保護: 只允許通過 PR 合併
    CI/CD: 自動部署到 Production

  develop:
    用途: 開發集成分支
    保護: 只允許通過 PR 合併
    CI/CD: 自動部署到 Dev 環境

功能分支:
  feature/*:
    命名: feature/epic-{number}-{short-description}
    範例: feature/epic-1-agent-creation
    來源: develop
    合併目標: develop
    生命週期: 完成後刪除

  bugfix/*:
    命名: bugfix/{issue-number}-{short-description}
    來源: develop 或 main
    合併目標: develop + main (hotfix)

  release/*:
    命名: release/v{major}.{minor}.{patch}
    範例: release/v1.0.0
    來源: develop
    合併目標: main + develop
    用途: 發布準備
```

**PR 規範**:

```markdown
PR Title Format:
  [Epic-{N}] {Type}: {Short Description}

  範例:
  - [Epic-1] feat: Implement Agent CRUD API
  - [Epic-2] fix: Fix plugin loading issue
  - [Epic-6] refactor: Optimize chat message rendering

Type:
  - feat: 新功能
  - fix: Bug 修復
  - refactor: 重構
  - docs: 文檔更新
  - test: 測試相關
  - chore: 構建/工具相關

PR Description Template:
  ## 📝 變更摘要
  (簡要說明此 PR 的目的)

  ## 🎯 相關 User Story
  - US {number}: {Story Title}
  - Epic {number}: {Epic Title}

  ## ✅ 變更清單
  - [ ] 功能實現
  - [ ] 單元測試覆蓋率 ≥80%
  - [ ] API 文檔更新
  - [ ] E2E 測試通過

  ## 🧪 測試
  (如何測試此變更)

  ## 📸 截圖 (如適用)

  ## ⚠️ Breaking Changes
  (是否有破壞性變更)
```

**GitHub 分支保護規則** (如使用 GitHub):

```yaml
main分支:
  - Require pull request reviews (≥2 approvers)
  - Require status checks to pass:
      - Build
      - Unit Tests
      - Integration Tests
      - SonarQube
  - Require conversation resolution
  - Require linear history
  - Include administrators

develop分支:
  - Require pull request reviews (≥1 approver)
  - Require status checks to pass:
      - Build
      - Unit Tests
  - Require conversation resolution
```

**執行步驟**:

```bash
# 1. 創建 develop 分支
git checkout -b develop
git push -u origin develop

# 2. 設置默認分支為 develop
# (在 GitHub/Azure DevOps 設置中操作)

# 3. 創建 .github 目錄
mkdir -p .github
cd .github

# 4. 創建 PR 模板
cat > PULL_REQUEST_TEMPLATE.md << 'EOF'
## 📝 變更摘要
<!-- 簡要說明此 PR 的目的 -->

## 🎯 相關 User Story
- US {number}: {Story Title}
- Epic {number}: {Epic Title}

## ✅ 變更清單
- [ ] 功能實現
- [ ] 單元測試覆蓋率 ≥80%
- [ ] API 文檔更新 (如適用)
- [ ] E2E 測試通過 (如適用)

## 🧪 測試
<!-- 如何測試此變更 -->

## 📸 截圖 (如適用)

## ⚠️ Breaking Changes
<!-- 是否有破壞性變更? -->
EOF

# 5. 創建 Issue 模板
mkdir -p .github/ISSUE_TEMPLATE

cat > .github/ISSUE_TEMPLATE/bug_report.md << 'EOF'
---
name: Bug Report
about: 報告系統錯誤
title: '[BUG] '
labels: bug
---

## 🐛 錯誤描述
<!-- 清楚簡潔地描述錯誤 -->

## 📋 復現步驟
1.
2.
3.

## 💡 預期行為
<!-- 描述你預期應該發生什麼 -->

## 📸 截圖
<!-- 如果適用,添加截圖 -->

## 🖥️ 環境
- OS:
- Browser:
- Version:
EOF
```

**預計時間**: 2 小時

---

### Day 2 Morning: Docker 本地環境

**負責人**: DevOps Lead

**任務清單**:

#### 2.1 創建 Docker Compose 開發環境

**目標**: 提供一鍵啟動的本地開發環境 (PostgreSQL + Redis + Qdrant)

**文件位置**: `infrastructure/docker/docker-compose.dev.yml`

```yaml
# infrastructure/docker/docker-compose.dev.yml
version: '3.8'

services:
  # PostgreSQL 數據庫
  postgres:
    image: postgres:16-alpine
    container_name: sk-postgres-dev
    environment:
      POSTGRES_DB: sk_agentic_framework_dev
      POSTGRES_USER: sk_admin
      POSTGRES_PASSWORD: Dev@Pass123
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./init-scripts:/docker-entrypoint-initdb.d
    networks:
      - sk-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U sk_admin -d sk_agentic_framework_dev"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis (用於 Rate Limiting 和 Caching)
  redis:
    image: redis:7-alpine
    container_name: sk-redis-dev
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - sk-network
    command: redis-server --appendonly yes
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  # Qdrant Vector Database
  qdrant:
    image: qdrant/qdrant:v1.7.4
    container_name: sk-qdrant-dev
    ports:
      - "6333:6333"  # HTTP API
      - "6334:6334"  # gRPC API
    volumes:
      - qdrant_data:/qdrant/storage
    networks:
      - sk-network
    environment:
      QDRANT__SERVICE__GRPC_PORT: 6334
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:6333/healthz"]
      interval: 10s
      timeout: 3s
      retries: 5

  # pgAdmin (可選,用於數據庫管理)
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: sk-pgadmin-dev
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@sk-framework.local
      PGADMIN_DEFAULT_PASSWORD: Admin@123
      PGADMIN_CONFIG_SERVER_MODE: 'False'
    ports:
      - "5050:80"
    volumes:
      - pgadmin_data:/var/lib/pgadmin
    networks:
      - sk-network
    depends_on:
      - postgres

volumes:
  postgres_data:
  redis_data:
  qdrant_data:
  pgadmin_data:

networks:
  sk-network:
    driver: bridge
```

**初始化腳本**: `infrastructure/docker/init-scripts/01-init-schema.sql`

```sql
-- 01-init-schema.sql
-- 創建基礎 Schema 和 Extensions

-- 安裝 UUID 擴展
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 安裝 Vector 擴展 (如果使用 PostgreSQL 作為 Vector DB)
-- CREATE EXTENSION IF NOT EXISTS vector;

-- 創建 Schema
CREATE SCHEMA IF NOT EXISTS agents;
CREATE SCHEMA IF NOT EXISTS knowledge;
CREATE SCHEMA IF NOT EXISTS workflows;
CREATE SCHEMA IF NOT EXISTS monitoring;

-- 創建基礎表: Agents
CREATE TABLE IF NOT EXISTS agents.agents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    model_provider VARCHAR(50) NOT NULL,
    model_name VARCHAR(100) NOT NULL,
    system_prompt TEXT,
    temperature DECIMAL(3,2) DEFAULT 0.7,
    max_tokens INT DEFAULT 2000,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255),
    is_deleted BOOLEAN DEFAULT FALSE
);

-- 創建索引
CREATE INDEX idx_agents_created_at ON agents.agents(created_at DESC);
CREATE INDEX idx_agents_is_deleted ON agents.agents(is_deleted);

-- 創建基礎表: Knowledge Base
CREATE TABLE IF NOT EXISTS knowledge.knowledge_bases (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255)
);

-- 插入測試數據 (開發環境)
INSERT INTO agents.agents (name, description, model_provider, model_name, system_prompt)
VALUES
    ('Demo Customer Service Agent', 'A helpful customer service agent', 'AzureOpenAI', 'gpt-4', 'You are a helpful customer service agent.'),
    ('Demo Data Analyst Agent', 'An agent specialized in data analysis', 'AzureOpenAI', 'gpt-4', 'You are a data analysis expert.')
ON CONFLICT DO NOTHING;

COMMENT ON SCHEMA agents IS 'Agent 管理相關表';
COMMENT ON SCHEMA knowledge IS 'Knowledge Base 相關表';
COMMENT ON SCHEMA workflows IS 'Workflow 編排相關表';
COMMENT ON SCHEMA monitoring IS '監控和日誌相關表';
```

**使用指南**: `infrastructure/docker/README.md`

```markdown
# Docker 開發環境使用指南

## 啟動環境

### 首次啟動
```bash
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d
```

### 查看日誌
```bash
docker-compose -f docker-compose.dev.yml logs -f
```

### 停止環境
```bash
docker-compose -f docker-compose.dev.yml down
```

### 清除數據並重新啟動
```bash
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d
```

## 連接信息

### PostgreSQL
- Host: localhost
- Port: 5432
- Database: sk_agentic_framework_dev
- User: sk_admin
- Password: Dev@Pass123
- Connection String: `Host=localhost;Port=5432;Database=sk_agentic_framework_dev;Username=sk_admin;Password=Dev@Pass123`

### Redis
- Host: localhost
- Port: 6379
- Connection String: `localhost:6379`

### Qdrant
- HTTP API: http://localhost:6333
- gRPC API: localhost:6334
- Dashboard: http://localhost:6333/dashboard

### pgAdmin (數據庫管理)
- URL: http://localhost:5050
- Email: admin@sk-framework.local
- Password: Admin@123

## 健康檢查

```bash
# 檢查所有服務狀態
docker-compose -f docker-compose.dev.yml ps

# 測試 PostgreSQL 連接
docker exec sk-postgres-dev psql -U sk_admin -d sk_agentic_framework_dev -c "SELECT version();"

# 測試 Redis 連接
docker exec sk-redis-dev redis-cli ping

# 測試 Qdrant 連接
curl http://localhost:6333/healthz
```

## 故障排除

### PostgreSQL 無法啟動
```bash
# 查看日誌
docker logs sk-postgres-dev

# 清除數據重新啟動
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d postgres
```

### Qdrant 無法訪問
```bash
# 檢查端口是否被占用
netstat -ano | findstr 6333

# 重啟 Qdrant
docker-compose -f docker-compose.dev.yml restart qdrant
```
```

**驗證**:
```bash
# 啟動所有服務
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d

# 等待 30 秒讓服務完全啟動
timeout /t 30

# 驗證 PostgreSQL
docker exec sk-postgres-dev psql -U sk_admin -d sk_agentic_framework_dev -c "SELECT COUNT(*) FROM agents.agents;"

# 驗證 Redis
docker exec sk-redis-dev redis-cli ping

# 驗證 Qdrant
curl http://localhost:6333/healthz
```

**預計時間**: 3 小時

---

## Day 3-4: Azure 雲端資源部署

### Day 3 Morning: Azure Bicep 基礎設施即代碼

**負責人**: DevOps Lead + Backend Lead

**任務清單**:

#### 3.1 創建 Azure Bicep 模板

**目標**: 使用 Infrastructure as Code (IaC) 部署 Dev 和 Test 環境

**文件結構**:
```
infrastructure/bicep/
├── main.bicep                      # 主模板
├── modules/
│   ├── app-service.bicep           # App Service Plan + Web App
│   ├── postgres.bicep              # Azure Database for PostgreSQL
│   ├── openai.bicep                # Azure OpenAI Service
│   ├── storage.bicep               # Azure Storage Account
│   ├── search.bicep                # Azure AI Search
│   └── container-registry.bicep   # Azure Container Registry
├── parameters/
│   ├── dev.bicepparam              # 開發環境參數
│   └── test.bicepparam             # 測試環境參數
└── README.md                       # 部署指南
```

**主模板**: `infrastructure/bicep/main.bicep`

```bicep
// main.bicep
targetScope = 'resourceGroup'

@description('Environment name (dev, test, prod)')
@allowed(['dev', 'test', 'prod'])
param environment string

@description('Azure region for resources')
param location string = resourceGroup().location

@description('Project name prefix')
param projectName string = 'sk-framework'

// 變量
var commonTags = {
  Project: 'Semantic Kernel Agentic Framework'
  Environment: environment
  ManagedBy: 'Bicep'
}

// 資源命名
var appServicePlanName = '${projectName}-asp-${environment}'
var webAppName = '${projectName}-api-${environment}'
var postgresServerName = '${projectName}-pg-${environment}'
var openAIAccountName = '${projectName}-openai-${environment}'
var storageAccountName = replace('${projectName}sa${environment}', '-', '')
var searchServiceName = '${projectName}-search-${environment}'
var containerRegistryName = replace('${projectName}acr${environment}', '-', '')

// Module: App Service
module appService 'modules/app-service.bicep' = {
  name: 'appService-deployment'
  params: {
    appServicePlanName: appServicePlanName
    webAppName: webAppName
    location: location
    environment: environment
    tags: commonTags
  }
}

// Module: PostgreSQL
module postgres 'modules/postgres.bicep' = {
  name: 'postgres-deployment'
  params: {
    serverName: postgresServerName
    location: location
    administratorLogin: 'sk_admin'
    administratorLoginPassword: 'ChangeMeInProduction!' // 實際部署時使用 Key Vault
    databaseName: 'sk_agentic_framework_${environment}'
    tags: commonTags
  }
}

// Module: Azure OpenAI
module openai 'modules/openai.bicep' = {
  name: 'openai-deployment'
  params: {
    accountName: openAIAccountName
    location: location
    tags: commonTags
  }
}

// Module: Storage Account
module storage 'modules/storage.bicep' = {
  name: 'storage-deployment'
  params: {
    storageAccountName: storageAccountName
    location: location
    tags: commonTags
  }
}

// Module: Azure AI Search
module search 'modules/search.bicep' = {
  name: 'search-deployment'
  params: {
    searchServiceName: searchServiceName
    location: location
    sku: environment == 'prod' ? 'standard' : 'basic'
    tags: commonTags
  }
}

// Module: Container Registry
module containerRegistry 'modules/container-registry.bicep' = {
  name: 'acr-deployment'
  params: {
    registryName: containerRegistryName
    location: location
    sku: environment == 'prod' ? 'Premium' : 'Basic'
    tags: commonTags
  }
}

// Outputs
output appServiceUrl string = appService.outputs.appServiceUrl
output postgresServerFqdn string = postgres.outputs.serverFqdn
output openAIEndpoint string = openai.outputs.endpoint
output storageAccountName string = storage.outputs.storageAccountName
output searchServiceEndpoint string = search.outputs.searchServiceEndpoint
output containerRegistryLoginServer string = containerRegistry.outputs.loginServer
```

**PostgreSQL Module**: `infrastructure/bicep/modules/postgres.bicep`

```bicep
// modules/postgres.bicep
@description('Server Name for Azure Database for PostgreSQL')
param serverName string

@description('Location for all resources')
param location string

@description('Administrator username for PostgreSQL')
param administratorLogin string

@description('Administrator password for PostgreSQL')
@secure()
param administratorLoginPassword string

@description('Database name')
param databaseName string

@description('PostgreSQL version')
param postgresVersion string = '16'

@description('Resource tags')
param tags object

// PostgreSQL Server
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: serverName
  location: location
  tags: tags
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    version: postgresVersion
    storage: {
      storageSizeGB: 32
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    highAvailability: {
      mode: 'Disabled'
    }
  }
}

// PostgreSQL Database
resource postgresDatabase 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-03-01-preview' = {
  parent: postgresServer
  name: databaseName
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

// Firewall Rule: Allow Azure Services
resource firewallRuleAzure 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = {
  parent: postgresServer
  name: 'AllowAllAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Firewall Rule: Allow Local Development (移除在生產環境)
resource firewallRuleDev 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = {
  parent: postgresServer
  name: 'AllowLocalDevelopment'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

// Outputs
output serverFqdn string = postgresServer.properties.fullyQualifiedDomainName
output databaseName string = postgresDatabase.name
```

**開發環境參數**: `infrastructure/bicep/parameters/dev.bicepparam`

```bicep
// parameters/dev.bicepparam
using '../main.bicep'

param environment = 'dev'
param location = 'eastus'
param projectName = 'sk-framework'
```

**部署腳本**: `infrastructure/bicep/deploy.ps1`

```powershell
# deploy.ps1
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('dev', 'test', 'prod')]
    [string]$Environment,

    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "rg-sk-framework-$Environment",

    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus"
)

# 登錄 Azure (如果尚未登錄)
$context = Get-AzContext
if (-not $context) {
    Connect-AzAccount
}

# 創建資源組 (如果不存在)
$rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (-not $rg) {
    Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Green
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location
}

# 部署 Bicep 模板
Write-Host "Deploying Bicep template to: $ResourceGroupName" -ForegroundColor Green
$deployment = New-AzResourceGroupDeployment `
    -ResourceGroupName $ResourceGroupName `
    -TemplateFile "./main.bicep" `
    -TemplateParameterFile "./parameters/$Environment.bicepparam" `
    -Verbose

# 顯示輸出
Write-Host "`n=== Deployment Outputs ===" -ForegroundColor Cyan
$deployment.Outputs | Format-Table -AutoSize

# 保存輸出到文件
$outputFile = "deployment-outputs-$Environment.json"
$deployment.Outputs | ConvertTo-Json -Depth 10 | Out-File $outputFile
Write-Host "`nOutputs saved to: $outputFile" -ForegroundColor Green
```

**驗證**:
```powershell
# 部署開發環境
cd infrastructure/bicep
.\deploy.ps1 -Environment dev

# 驗證資源
az resource list --resource-group rg-sk-framework-dev --output table

# 測試 PostgreSQL 連接
az postgres flexible-server connect `
    --name sk-framework-pg-dev `
    --admin-user sk_admin `
    --database sk_agentic_framework_dev
```

**預計時間**: 4 小時

---

### Day 3 Afternoon: Azure OpenAI 連接測試

**負責人**: Backend Lead

**任務清單**:

#### 3.2 Azure OpenAI 服務配置與測試

**目標**: 驗證 Azure OpenAI 連接和 Semantic Kernel 集成

**步驟**:

1. **部署 GPT-4 模型**

```bash
# 使用 Azure CLI 部署模型
az cognitiveservices account deployment create \
  --name sk-framework-openai-dev \
  --resource-group rg-sk-framework-dev \
  --deployment-name gpt-4 \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --sku-capacity 10 \
  --sku-name "Standard"
```

2. **創建測試控制台應用**

```bash
cd services/agent-service
dotnet new console -n AzureOpenAITest
cd AzureOpenAITest

# 安裝必要套件
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0
```

3. **測試代碼**: `AzureOpenAITest/Program.cs`

```csharp
// Program.cs
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

// 讀取配置
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;

Console.WriteLine("=== Azure OpenAI Connection Test ===\n");
Console.WriteLine($"Endpoint: {endpoint}");
Console.WriteLine($"Deployment: {deploymentName}\n");

try
{
    // 創建 Kernel
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: apiKey
    );
    var kernel = kernelBuilder.Build();

    Console.WriteLine("✅ Kernel created successfully\n");

    // 測試簡單對話
    Console.WriteLine("Testing simple prompt...");
    var chatFunction = kernel.CreateFunctionFromPrompt("What is the capital of France? Answer in one word.");

    var result = await kernel.InvokeAsync(chatFunction);
    Console.WriteLine($"✅ Response: {result}\n");

    // 測試 Semantic Function
    Console.WriteLine("Testing semantic function...");
    var semanticFunction = kernel.CreateFunctionFromPrompt(
        "Summarize the following text in one sentence: {{$input}}",
        functionName: "Summarize"
    );

    var summaryResult = await kernel.InvokeAsync(
        semanticFunction,
        new() { ["input"] = "Semantic Kernel is a lightweight SDK that integrates AI Large Language Models (LLMs) with conventional programming languages." }
    );
    Console.WriteLine($"✅ Summary: {summaryResult}\n");

    // 測試 Streaming
    Console.WriteLine("Testing streaming response...");
    var streamFunction = kernel.CreateFunctionFromPrompt("Count from 1 to 5 slowly.");

    Console.Write("Streaming output: ");
    await foreach (var chunk in kernel.InvokeStreamingAsync(streamFunction))
    {
        Console.Write(chunk);
    }
    Console.WriteLine("\n\n✅ All tests passed!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    return 1;
}

return 0;
```

4. **配置文件**: `AzureOpenAITest/appsettings.json`

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://sk-framework-openai-dev.openai.azure.com/",
    "DeploymentName": "gpt-4",
    "ApiKey": "<YOUR_API_KEY_HERE>"
  }
}
```

**驗證**:
```bash
# 運行測試
cd AzureOpenAITest
dotnet run

# 預期輸出:
# ✅ Kernel created successfully
# ✅ Response: Paris
# ✅ Summary: Semantic Kernel integrates LLMs with programming languages.
# ✅ All tests passed!
```

**預計時間**: 2 小時

---

## Day 4-5: CI/CD Pipeline 建立

### Day 4: GitHub Actions CI Pipeline

**負責人**: DevOps Lead

**任務清單**:

#### 4.1 創建 GitHub Actions Workflows

**文件結構**:
```
.github/workflows/
├── ci-backend.yml          # Backend CI
├── ci-frontend.yml         # Frontend CI
├── cd-dev.yml              # Deploy to Dev
├── cd-test.yml             # Deploy to Test
└── pr-validation.yml       # PR 驗證
```

**Backend CI**: `.github/workflows/ci-backend.yml`

```yaml
# .github/workflows/ci-backend.yml
name: Backend CI

on:
  push:
    branches: [ develop, main ]
    paths:
      - 'services/**'
      - 'packages/dotnet-sdk/**'
  pull_request:
    branches: [ develop ]
    paths:
      - 'services/**'
      - 'packages/dotnet-sdk/**'

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET 8
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: |
        cd services/agent-service/AgentService
        dotnet restore

    - name: Build
      run: |
        cd services/agent-service/AgentService
        dotnet build --no-restore --configuration Release

    - name: Run Unit Tests
      run: |
        cd services/agent-service/AgentService.Tests
        dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: backend-test-results
        path: '**/test-results.trx'

    - name: Upload Coverage Report
      uses: codecov/codecov-action@v4
      with:
        files: '**/coverage.cobertura.xml'
        flags: backend

    - name: Run SonarQube Analysis
      if: github.event_name == 'pull_request'
      uses: sonarsource/sonarqube-scan-action@master
      env:
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
```

**Frontend CI**: `.github/workflows/ci-frontend.yml`

```yaml
# .github/workflows/ci-frontend.yml
name: Frontend CI

on:
  push:
    branches: [ develop, main ]
    paths:
      - 'apps/web-app/**'
      - 'packages/ui-components/**'
  pull_request:
    branches: [ develop ]
    paths:
      - 'apps/web-app/**'
      - 'packages/ui-components/**'

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup Node.js 20
      uses: actions/setup-node@v4
      with:
        node-version: '20'

    - name: Setup pnpm
      uses: pnpm/action-setup@v3
      with:
        version: 8

    - name: Get pnpm store directory
      id: pnpm-cache
      run: |
        echo "STORE_PATH=$(pnpm store path)" >> $GITHUB_OUTPUT

    - name: Setup pnpm cache
      uses: actions/cache@v4
      with:
        path: ${{ steps.pnpm-cache.outputs.STORE_PATH }}
        key: ${{ runner.os }}-pnpm-store-${{ hashFiles('**/pnpm-lock.yaml') }}
        restore-keys: |
          ${{ runner.os }}-pnpm-store-

    - name: Install dependencies
      run: pnpm install --frozen-lockfile

    - name: Lint
      run: |
        cd apps/web-app
        pnpm lint

    - name: Type Check
      run: |
        cd apps/web-app
        pnpm tsc --noEmit

    - name: Run Unit Tests
      run: |
        cd apps/web-app
        pnpm test:coverage

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: frontend-test-results
        path: 'apps/web-app/coverage'

    - name: Upload Coverage Report
      uses: codecov/codecov-action@v4
      with:
        files: 'apps/web-app/coverage/coverage-final.json'
        flags: frontend

    - name: Build
      run: |
        cd apps/web-app
        pnpm build

    - name: Upload Build Artifacts
      uses: actions/upload-artifact@v4
      with:
        name: frontend-build
        path: 'apps/web-app/dist'
```

**PR Validation**: `.github/workflows/pr-validation.yml`

```yaml
# .github/workflows/pr-validation.yml
name: PR Validation

on:
  pull_request:
    branches: [ develop, main ]

jobs:
  validate-pr-title:
    runs-on: ubuntu-latest
    steps:
    - name: Validate PR Title
      uses: amannn/action-semantic-pull-request@v5
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        types: |
          feat
          fix
          refactor
          docs
          test
          chore
        scopes: |
          epic-1
          epic-2
          epic-3
          epic-4
          epic-5
          epic-6
          epic-7
          epic-8
        requireScope: false

  check-code-quality:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Run Backend CI
      uses: ./.github/workflows/ci-backend.yml

    - name: Run Frontend CI
      uses: ./.github/workflows/ci-frontend.yml

  require-approvals:
    runs-on: ubuntu-latest
    if: github.base_ref == 'main'
    steps:
    - name: Check PR Approvals
      uses: pullreminders/slack-action@master
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        args: 'PR to main requires 2 approvals'
```

**預計時間**: 4 小時

---

### Day 5: 驗收與文檔

**負責人**: Tech Lead + 全團隊

**任務清單**:

#### 5.1 Sprint 0 驗收檢查清單

**環境驗收**:

```yaml
本地開發環境:
  - [ ] PostgreSQL 可連接 (localhost:5432)
  - [ ] Redis 可連接 (localhost:6379)
  - [ ] Qdrant 可連接 (localhost:6333)
  - [ ] pgAdmin 可訪問 (localhost:5050)
  - [ ] Backend API 可啟動 (https://localhost:7001)
  - [ ] Frontend App 可啟動 (http://localhost:5173)
  - [ ] Swagger UI 可訪問 (https://localhost:7001/swagger)

Azure 雲端資源:
  - [ ] Resource Group 已創建
  - [ ] PostgreSQL Server 已部署
  - [ ] Azure OpenAI 已部署
  - [ ] GPT-4 模型已部署
  - [ ] Storage Account 已創建
  - [ ] Azure AI Search 已創建 (可選)
  - [ ] Container Registry 已創建 (可選)

CI/CD Pipeline:
  - [ ] Backend CI 運行成功
  - [ ] Frontend CI 運行成功
  - [ ] PR Validation 正常工作
  - [ ] Code Coverage ≥80% (目標)
  - [ ] SonarQube 掃描無 Critical Issues

團隊準備:
  - [ ] 所有開發者完成環境設置
  - [ ] Git Workflow 已宣導
  - [ ] Coding Standards 已宣導
  - [ ] Sprint 1 User Stories 已 Ready
```

#### 5.2 更新開發文檔

**文檔清單**:

1. **環境設置指南** (`docs/technical-implementation/2-dev-environment/SETUP-GUIDE.md`)
2. **Docker 使用指南** (`infrastructure/docker/README.md`) - 已完成
3. **Azure 部署指南** (`infrastructure/bicep/README.md`)
4. **CI/CD 使用指南** (`.github/workflows/README.md`)
5. **Sprint 1 Kickoff 簡報** (`claudedocs/SPRINT-1-KICKOFF.md`)

#### 5.3 Sprint 1 Kickoff 會議準備

**簡報大綱**:

```markdown
# Sprint 1 Kickoff

## 1. Sprint 0 回顧
- 完成的交付物
- 環境驗收結果
- 遇到的問題與解決方案

## 2. Sprint 1 目標
- User Stories: US 1.1, 1.2, 1.3
- Story Points: 13
- Sprint 目標: Agent CRUD 功能完成

## 3. 技術準備
- 開發環境已就緒
- Azure 資源已部署
- CI/CD Pipeline 已建立

## 4. 團隊分工
- Backend: US 1.1 API 實現
- Frontend: US 1.1 UI 實現
- SDK: US 1.2 .NET SDK
- QA: US 1.3 測試

## 5. Definition of Done
- 所有驗收標準滿足
- 單元測試覆蓋率 ≥80%
- Code Review 通過
- API 響應時間 <200ms
- PO 驗收通過

## 6. Sprint 1 時間表
- Week 1 (Day 1-3): 開發
- Week 2 (Day 4-5): 測試與修復
- Week 3 (Day 1): Sprint Review
- Week 3 (Day 2): Sprint Retrospective
```

**預計時間**: 4 小時

---

## 驗收標準 (DoD)

### Sprint 0 完成定義

**必須滿足的條件**:

```yaml
環境基礎設施:
  ✅ 必須: Monorepo 結構已建立且正常運作
  ✅ 必須: Backend 項目可編譯無錯誤
  ✅ 必須: Frontend 項目可編譯無錯誤
  ✅ 必須: Docker Compose 可啟動所有服務
  ✅ 必須: PostgreSQL 可連接且有測試數據
  ✅ 必須: Azure 資源已部署 (Dev 環境)
  ✅ 必須: Azure OpenAI 連接測試通過

CI/CD:
  ✅ 必須: Backend CI Pipeline 運行成功
  ✅ 必須: Frontend CI Pipeline 運行成功
  ✅ 必須: PR Validation 正常工作
  🟡 可選: Code Coverage 報告可生成
  🟡 可選: SonarQube 集成完成

文檔:
  ✅ 必須: 環境設置指南已更新
  ✅ 必須: Docker 使用指南已創建
  ✅ 必須: Sprint 1 User Stories 已 Ready
  🟡 可選: Azure 部署指南已完成

團隊準備:
  ✅ 必須: ≥3 名開發者完成環境設置驗證
  ✅ 必須: Git Workflow 達成共識
  ✅ 必須: Coding Standards 已宣導
  ✅ 必須: Sprint 1 Kickoff 會議已完成
```

### 驗收測試清單

**環境驗證腳本**: `scripts/validate-sprint0.ps1`

```powershell
# validate-sprint0.ps1
Write-Host "=== Sprint 0 Environment Validation ===" -ForegroundColor Cyan

$passed = 0
$failed = 0

# 1. 檢查 Docker 服務
Write-Host "`n1. Checking Docker services..." -ForegroundColor Yellow
try {
    docker ps | Select-String "sk-postgres-dev|sk-redis-dev|sk-qdrant-dev"
    Write-Host "✅ Docker services running" -ForegroundColor Green
    $passed++
} catch {
    Write-Host "❌ Docker services not running" -ForegroundColor Red
    $failed++
}

# 2. 檢查 PostgreSQL 連接
Write-Host "`n2. Testing PostgreSQL connection..." -ForegroundColor Yellow
try {
    docker exec sk-postgres-dev psql -U sk_admin -d sk_agentic_framework_dev -c "SELECT COUNT(*) FROM agents.agents;" | Out-Null
    Write-Host "✅ PostgreSQL connection successful" -ForegroundColor Green
    $passed++
} catch {
    Write-Host "❌ PostgreSQL connection failed" -ForegroundColor Red
    $failed++
}

# 3. 檢查 Backend 編譯
Write-Host "`n3. Building Backend..." -ForegroundColor Yellow
try {
    cd services/agent-service/AgentService
    dotnet build --no-incremental | Out-Null
    Write-Host "✅ Backend build successful" -ForegroundColor Green
    $passed++
} catch {
    Write-Host "❌ Backend build failed" -ForegroundColor Red
    $failed++
}

# 4. 檢查 Frontend 編譯
Write-Host "`n4. Building Frontend..." -ForegroundColor Yellow
try {
    cd ../../../apps/web-app
    pnpm build | Out-Null
    Write-Host "✅ Frontend build successful" -ForegroundColor Green
    $passed++
} catch {
    Write-Host "❌ Frontend build failed" -ForegroundColor Red
    $failed++
}

# 5. 檢查 Azure 資源
Write-Host "`n5. Checking Azure resources..." -ForegroundColor Yellow
try {
    $rg = az group show --name rg-sk-framework-dev 2>$null | ConvertFrom-Json
    if ($rg) {
        Write-Host "✅ Azure Resource Group exists" -ForegroundColor Green
        $passed++
    }
} catch {
    Write-Host "❌ Azure Resource Group not found" -ForegroundColor Red
    $failed++
}

# 總結
Write-Host "`n=== Validation Summary ===" -ForegroundColor Cyan
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red

if ($failed -eq 0) {
    Write-Host "`n🎉 Sprint 0 environment is ready!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n⚠️  Please fix the failed checks before starting Sprint 1" -ForegroundColor Yellow
    exit 1
}
```

**運行驗證**:
```powershell
cd "C:\AI Semantic Kernel"
.\scripts\validate-sprint0.ps1
```

---

## 風險與緩解措施

### 識別的風險

| # | 風險 | 機率 | 影響 | 緩解措施 | 負責人 |
|---|------|------|------|---------|--------|
| 1 | Azure 資源部署失敗 | 中 | 高 | 準備 ARM 模板作為 Bicep 備選方案 | DevOps Lead |
| 2 | Azure OpenAI 配額不足 | 低 | 中 | 提前申請配額增加 | Backend Lead |
| 3 | Docker 在 Windows 上性能問題 | 中 | 低 | 提供 WSL2 配置指南 | DevOps Lead |
| 4 | 團隊成員環境設置困難 | 中 | 中 | 準備詳細故障排除文檔 | Tech Lead |
| 5 | CI/CD Pipeline 配置時間超出預期 | 中 | 低 | 使用簡化版 CI,後續迭代改進 | DevOps Lead |
| 6 | PostgreSQL 遷移腳本問題 | 低 | 中 | 使用 EF Core Migrations | Backend Lead |
| 7 | Monorepo 構建緩慢 | 低 | 低 | Turborepo 緩存優化 | Tech Lead |

### 應急計劃

**如果 Azure 部署失敗**:
1. 使用 Azure Portal 手動創建資源
2. 使用 ARM 模板代替 Bicep
3. 降級到僅 Dev 環境,Test 環境推遲到 Sprint 1

**如果 Docker 性能問題嚴重**:
1. 使用雲端 PostgreSQL 代替本地 Docker
2. 僅保留 Redis 和 Qdrant 在 Docker
3. 提供原生 PostgreSQL 安裝指南

---

## 下一步: Sprint 1 啟動

### Sprint 1 準備檢查清單

```yaml
技術準備:
  - [ ] Sprint 0 驗收完成
  - [ ] 所有開發者環境就緒
  - [ ] Azure 資源訪問權限已分配
  - [ ] 數據庫 Schema 已初始化

User Stories:
  - [ ] US 1.1 已 Ready (DoR 滿足)
  - [ ] US 1.2 已 Ready
  - [ ] US 1.3 已 Ready
  - [ ] 驗收標準已明確定義

團隊:
  - [ ] Sprint Planning 會議已完成
  - [ ] Sprint Goal 已定義
  - [ ] 任務分工已明確
  - [ ] Daily Standup 時間已確定
```

### Sprint 1 關鍵里程碑

```yaml
Week 1 (Day 1-3):
  - Day 1: Sprint Planning + US 1.1 開發開始
  - Day 2-3: US 1.1 Backend API 完成

Week 2 (Day 4-5):
  - Day 4: US 1.1 Frontend UI 完成
  - Day 5: US 1.2 .NET SDK 開發

Week 3 (Day 1-3):
  - Day 1: US 1.3 測試與修復
  - Day 2: Sprint Review
  - Day 3: Sprint Retrospective + Sprint 2 Planning
```

---

## 附錄

### A. 快速命令參考

```bash
# 啟動本地開發環境
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d

# 運行 Backend
cd services/agent-service/AgentService
dotnet run

# 運行 Frontend
cd apps/web-app
pnpm dev

# 運行測試
# Backend
cd services/agent-service/AgentService.Tests
dotnet test

# Frontend
cd apps/web-app
pnpm test

# 部署 Azure 資源
cd infrastructure/bicep
.\deploy.ps1 -Environment dev

# 驗證環境
cd "C:\AI Semantic Kernel"
.\scripts\validate-sprint0.ps1
```

### B. 故障排除

**問題: Docker 服務無法啟動**
```bash
# 檢查端口占用
netstat -ano | findstr "5432|6379|6333"

# 清除所有容器重新啟動
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d
```

**問題: Backend API 無法連接 PostgreSQL**
```bash
# 檢查連接字符串
cat services/agent-service/AgentService/appsettings.Development.json

# 測試 PostgreSQL 連接
docker exec sk-postgres-dev psql -U sk_admin -d sk_agentic_framework_dev -c "\\dt agents.*"
```

**問題: Azure OpenAI 連接失敗**
```bash
# 檢查配置
az cognitiveservices account show --name sk-framework-openai-dev --resource-group rg-sk-framework-dev

# 測試端點
curl https://sk-framework-openai-dev.openai.azure.com/openai/deployments?api-version=2023-05-15
```

---

**文檔版本**: 1.0.0
**最後更新**: 2025-11-03
**負責人**: DevOps Lead + Tech Lead
**狀態**: ✅ 計劃完成,準備執行

**下一步**: [Sprint 1 執行計劃](./SPRINT-1-EXECUTION-PLAN.md)
