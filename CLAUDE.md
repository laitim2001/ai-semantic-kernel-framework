# AI Agent Platform - Claude Code 指南

此文件為 Claude Code 提供在此專案中操作所需的關鍵資訊。

## 專案概述

**企業級 AI Agent 平台** - 基於 Semantic Kernel 的多代理系統框架，提供五大核心能力：

1. **Persona Framework** - 10 種專業角色的 AI 代理系統
2. **Code Interpreter** - 安全的代碼執行環境
3. **Text-to-SQL** - 自然語言資料庫查詢
4. **Knowledge Management** - RAG 知識管理系統
5. **Multi-Agent Workflow** - 使用 VueFlow + CRDT 的視覺化工作流

**技術棧**：.NET 9 + React 18 + TypeScript + PostgreSQL 16 + Redis 7 + Qdrant 1.7.4

**當前進度**：Sprint 1 完成，Sprint 2 進行中 (65% 完成)

## 常用命令

### 開發環境啟動

```powershell
# 啟動基礎設施 (PostgreSQL, Redis, Qdrant, pgAdmin)
docker-compose up -d

# 後端 API (.NET)
cd src/AIAgentPlatform.API
dotnet run
# 運行於 http://localhost:5095

# 前端 (React + Vite)
cd apps/web-app
npm run dev
# 運行於 http://localhost:5177
```

### 建置與測試

```powershell
# 後端
dotnet build                          # 建置整個解決方案
dotnet test                          # 執行所有測試
dotnet run --project src/AIAgentPlatform.API  # 運行 API

# 前端
cd apps/web-app
npm run build                        # 生產建置
npm run typecheck                    # TypeScript 類型檢查
npm run lint                         # ESLint 檢查
npm run preview                      # 預覽生產建置
```

### 資料庫管理

```powershell
# 新增 Migration
cd src/AIAgentPlatform.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../AIAgentPlatform.API

# 更新資料庫
dotnet ef database update --startup-project ../AIAgentPlatform.API
```

## 架構設計

### 分層架構 (Clean Architecture + DDD + CQRS)

```
┌─────────────────────────────────────────────┐
│  Presentation Layer (API)                   │
│  - Controllers, Middleware, DTOs            │
├─────────────────────────────────────────────┤
│  Application Layer                          │
│  - CQRS (Commands/Queries via MediatR)     │
│  - Use Cases, DTOs, Interfaces             │
├─────────────────────────────────────────────┤
│  Domain Layer                               │
│  - Entities, Value Objects, Domain Events  │
│  - Business Rules, Domain Services         │
├─────────────────────────────────────────────┤
│  Infrastructure Layer                       │
│  - EF Core, Repositories, External APIs    │
│  - Redis, Qdrant, Email Services           │
├─────────────────────────────────────────────┤
│  Shared Layer                               │
│  - Common utilities, Constants, Extensions │
└─────────────────────────────────────────────┘
```

### 關鍵架構決策 (ADR)

- **ADR-006**: 混合狀態管理 - Redis (快取) + PostgreSQL (持久化)
- **ADR-007**: 階段式通訊架構 - Phase 1: MediatR → Phase 2: Service Bus
- **ADR-008**: Code Interpreter 容器池 - 動態生命週期管理
- **ADR-011**: Framework Abstraction Layer - 統一 AI 框架介面

### 專案結構

```
src/
├── AIAgentPlatform.API/              # Web API 層
├── AIAgentPlatform.Application/      # 應用層 (CQRS)
├── AIAgentPlatform.Domain/           # 領域層 (Entities, VOs)
├── AIAgentPlatform.Infrastructure/   # 基礎設施層 (EF Core, Redis)
├── AIAgentPlatform.Shared/           # 共享工具
tests/
├── AIAgentPlatform.UnitTests/        # 單元測試
├── AIAgentPlatform.IntegrationTests/ # 整合測試
apps/
└── web-app/                          # React 前端應用
    ├── src/
    │   ├── features/                 # 功能模組 (chat, agents, etc.)
    │   ├── components/               # 共用元件
    │   ├── services/                 # API 服務
    │   └── store/                    # Zustand 狀態管理
```

## 文檔架構 (BMad 方法論)

此專案採用 **雙層文檔架構**：

### 1. 參考層 (`/docs`)
靜態規劃與設計文檔：
- `architecture/` - 架構設計文件、ADR
- `api/` - API 規格與設計
- `bmad/` - BMad 方法論文檔

### 2. 執行層 (`/claudedocs`)
動態追蹤與執行文檔：
- `0-overview/` - 專案概覽
- `1-planning/` - Sprint 計劃
- `2-sprints/` - Sprint 執行狀態
- `3-progress/` - 工作進度追蹤
- `4-changes/` - 變更日誌
- `5-processes/` - 開發流程
- `6-implementation-plans/` - 實作計劃
- `7-ai-assistant/` - AI 輔助記錄

## BMad 方法論 - 10 種專業角色

此專案整合 BMad 框架，透過 `@角色名` 啟動特定專業角色：

- `@pm` (John) - Product Manager：PRD、產品策略、功能優先級
- `@architect` (Winston) - 系統架構師：架構設計、技術選型、API 設計
- `@dev` - 開發者：程式碼實作、技術實現
- `@qa` - 測試工程師：測試策略、品質保證
- `@sm` - Scrum Master：敏捷流程、團隊協作
- `@analyst` - 業務分析師：需求分析、業務流程
- `@ux-expert` - UX 專家：使用者體驗、介面設計
- 及其他創意寫作角色

**使用範例**：
```
@pm *create-prd           # 建立產品需求文件
@architect *create-full-stack-architecture  # 建立全棧架構文件
```

## 重要開發規範

### 1. Git 工作流
- 當前分支：`feature/us-2.2-plugin-hot-reload`
- 所有功能開發使用 feature branch
- Commit message 使用中文 + emoji（遵循專案慣例）

### 2. API 開發規範
- 遵循 RESTful 設計原則
- 使用 MediatR 實作 CQRS 模式
- 所有 API 端點需要 Swagger 文檔
- 實作 Result Pattern 進行錯誤處理

### 3. 前端開發規範
- React 18 + TypeScript + Material-UI
- Zustand 進行狀態管理
- 使用 feature-based 目錄結構
- 元件需包含 data-testid 屬性便於測試

### 4. 資料庫規範
- EF Core Code-First 開發
- 所有變更透過 Migration 管理
- 使用 Repository Pattern
- 複雜查詢使用 Specification Pattern

### 5. 測試策略
- 單元測試覆蓋核心業務邏輯
- 整合測試驗證 API 端點
- 前端使用 Vitest + Testing Library

## 當前功能狀態

### ✅ 已完成
- **US 1.1**: Agent CRUD API (完整 REST API)
- **US 1.2**: Agent 搜尋與篩選 (多條件查詢)
- **US 1.3**: Agent 狀態管理 (啟用/暫停/封存)
- **US 1.4**: Agent 執行引擎 (Semantic Kernel 整合)
- **US 2.1**: Plugin 系統基礎架構

### 🔄 進行中 (70%)
- **US 2.2**: Plugin 版本管理
- **US 2.3**: Plugin 熱重載機制

### 📋 待開發
- **US 3.x**: Conversation 管理 (Chat UI 功能)
- **US 4.x**: 使用者認證與授權

## 環境變數配置

關鍵配置位於 `src/AIAgentPlatform.API/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=aiagent;..."
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Qdrant": {
    "Endpoint": "http://localhost:6333"
  },
  "SemanticKernel": {
    "ApiKey": "配置於 User Secrets",
    "Endpoint": "Azure OpenAI 端點"
  }
}
```

**敏感資訊管理**：使用 .NET User Secrets，不要提交到版本控制。

## 常見任務

### 新增 Agent CRUD 功能
1. 在 `Domain/Entities/` 新增實體
2. 在 `Application/` 建立 Command/Query + Handler
3. 在 `Infrastructure/Repositories/` 實作 Repository
4. 在 `API/Controllers/` 新增 Controller 端點
5. 執行 `dotnet ef migrations add <名稱>`

### 新增前端功能模組
1. 在 `apps/web-app/src/features/` 建立功能目錄
2. 建立 `components/`, `hooks/`, `services/` 子目錄
3. 在 `services/api.ts` 新增 API 呼叫
4. 使用 Zustand 管理狀態（如需要）
5. 在路由中註冊新頁面

### 執行完整開發流程
```powershell
# 1. 啟動所有服務
docker-compose up -d
cd src/AIAgentPlatform.API && dotnet run
cd apps/web-app && npm run dev

# 2. 開發功能

# 3. 執行測試
dotnet test
cd apps/web-app && npm run typecheck && npm run lint

# 4. 提交變更
git add .
git commit -m "feat: 功能描述"
```

## 參考文件

- **README.md** - 專案快速入門
- **DEVELOPMENT-SETUP.md** - 開發環境詳細設定
- **docs/architecture/Architecture-Design-Document.md** - 完整架構設計
- **claudedocs/README.md** - 執行層文檔結構說明
- **claudedocs/2-sprints/sprint-2/SPRINT-2-OVERVIEW.md** - 當前 Sprint 狀態

## 重要提醒

1. **不要停止 node.js 進程** - 它同時運行 Claude Code 主程式
2. **使用繁體中文** - 與使用者溝通時使用繁體中文
3. **維持高品質** - 不因 token 限制而簡化建議或程式碼品質
4. **遵循 Clean Architecture** - 保持層次分離，依賴方向由外向內
5. **CQRS 模式** - 命令（寫入）與查詢（讀取）嚴格分離
6. **Framework Abstraction** - 所有 AI 框架操作透過抽象層進行
