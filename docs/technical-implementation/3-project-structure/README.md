# Phase 3: 項目結構

## 版本信息
- **版本**: 1.0
- **最後更新**: 2025-01-01
- **狀態**: ✅ 完成

## 概述

本階段定義 AI Workflow Platform 的完整項目結構,包括後端 .NET 8 解決方案、React 19 主應用、Vue 3 Workflow Editor,以及共享庫的組織方式。

## 架構概覽

```
ai-workflow-platform/
├── backend/                    # .NET 8 後端解決方案
│   ├── src/                   # 源代碼
│   ├── tests/                 # 測試項目
│   └── AIWorkflow.sln         # Visual Studio 解決方案
│
├── frontend/                   # 前端項目
│   ├── react-app/             # React 19 主應用
│   └── vue-workflow-editor/   # Vue 3 Workflow Editor
│
├── shared/                     # 共享庫和工具
│   ├── types/                 # TypeScript 類型定義
│   ├── utils/                 # 共享工具函數
│   └── contracts/             # API 契約定義
│
├── docker/                     # Docker 配置
│   ├── docker-compose.yml
│   ├── docker-compose.dev.yml
│   └── docker-compose.prod.yml
│
├── scripts/                    # 構建和部署腳本
├── docs/                      # 項目文檔
└── .github/                   # GitHub Actions 配置
```

## 技術棧

### 後端 (.NET 8)
- **框架**: ASP.NET Core 8.0
- **語言**: C# 12
- **ORM**: Entity Framework Core 9
- **AI 框架**: Microsoft Semantic Kernel 1.66.0

### 前端
- **主應用**: React 19 + TypeScript 5.8
- **Workflow Editor**: Vue 3.5 + TypeScript 5.8
- **構建工具**: Vite 5

### 數據庫
- **主數據庫**: PostgreSQL 16
- **緩存**: Redis 7
- **日誌存儲**: MongoDB 7

## 文檔列表

1. **[Solution Architecture](./solution-architecture.md)**
   - Visual Studio 解決方案結構
   - 項目依賴關係
   - 分層架構設計

2. **[Backend Project Structure](./backend-project-structure.md)**
   - .NET 8 項目組織
   - Clean Architecture 實現
   - 領域驅動設計 (DDD) 模式

3. **[Frontend React Structure](./frontend-react-structure.md)**
   - React 19 主應用結構
   - 組件組織和路由
   - 狀態管理架構

4. **[Frontend Vue Structure](./frontend-vue-structure.md)**
   - Vue 3 Workflow Editor 結構
   - VueFlow 集成
   - Y.js CRDT 協作架構

5. **[Shared Libraries](./shared-libraries.md)**
   - 跨項目共享代碼
   - TypeScript 類型定義
   - API 契約管理

6. **[Folder Naming Conventions](./folder-naming-conventions.md)**
   - 命名標準和約定
   - 文件組織規則
   - 代碼結構最佳實踐

7. **[Dependency Management](./dependency-management.md)**
   - NuGet 套件管理
   - npm 依賴管理
   - 版本控制策略

## 設計原則

### 1. 關注點分離 (Separation of Concerns)
- 業務邏輯與基礎設施分離
- UI 與業務邏輯解耦
- 數據訪問層抽象

### 2. 依賴注入 (Dependency Injection)
- 所有服務通過 DI 容器註冊
- 接口優先設計
- 鬆耦合架構

### 3. 單一職責原則 (Single Responsibility)
- 每個類/模塊只有一個職責
- 功能內聚,職責明確
- 易於測試和維護

### 4. 開放封閉原則 (Open-Closed)
- 對擴展開放
- 對修改封閉
- 插件化架構設計

## 項目組織策略

### 按功能組織 (Feature-based)
```
src/Features/
├── Users/
│   ├── Commands/
│   ├── Queries/
│   ├── Models/
│   └── Services/
├── Workflows/
└── Agents/
```

### 按層組織 (Layer-based)
```
src/
├── Domain/           # 領域模型
├── Application/      # 應用邏輯
├── Infrastructure/   # 基礎設施
└── Presentation/     # 表示層
```

## 構建和部署

### 開發環境
```bash
# 後端
cd backend
dotnet restore
dotnet build
dotnet run --project src/AIWorkflow.API

# React 前端
cd frontend/react-app
npm install
npm run dev

# Vue Workflow Editor
cd frontend/vue-workflow-editor
npm install
npm run dev
```

### Docker 構建
```bash
# 構建所有服務
docker-compose -f docker/docker-compose.yml build

# 啟動開發環境
docker-compose -f docker/docker-compose.dev.yml up
```

## 質量指標

### 代碼組織
- ✅ 清晰的項目結構
- ✅ 一致的命名約定
- ✅ 合理的文件夾層級 (≤5 層)
- ✅ 每個文件 ≤500 行代碼

### 依賴管理
- ✅ 明確的依賴關係圖
- ✅ 無循環依賴
- ✅ 最小化依賴數量
- ✅ 定期更新依賴

### 可維護性
- ✅ 代碼內聚性高
- ✅ 耦合度低
- ✅ 易於理解和修改
- ✅ 良好的文檔覆蓋

## 下一步

完成項目結構設計後,繼續:
- **Phase 4**: 編碼標準 ✅ (已完成)
- **Phase 5**: API 設計規範
- **Phase 6**: 數據庫標準

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
