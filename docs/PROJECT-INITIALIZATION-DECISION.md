# 專案初始化決策文檔

**Semantic Kernel Agentic Framework - Project Initialization Decisions**

**版本**: 1.0.0
**日期**: 2025-11-02
**狀態**: 待決策
**決策者**: Product Owner + Tech Lead

[📚 返回主索引](README.md) | [🎯 BMad 階段分析](BMAD-STAGE-ANALYSIS.md) | [✅ PO Validation](../claudedocs/PO-VALIDATION-REPORT-FINAL.md)

---

## 📋 文檔目的

本文檔提供 **Semantic Kernel Agentic Framework** 專案初始化的關鍵技術決策建議,涵蓋:

1. **倉庫策略**: Monorepo vs Polyrepo
2. **前端技術棧**: React + Vue 混合方案
3. **專案結構**: 目錄組織和命名
4. **開發工具鏈**: IDE, CI/CD, 協作工具

**決策時間**: Week 0 (2025-11-02)
**實施時間**: Week 1 (專案結構建立)

---

## 🎯 決策 1: 倉庫策略

### 選項分析

#### Option A: Monorepo (推薦 ✅)

**定義**: 單一 Git 倉庫包含所有代碼 (前端 + 後端 + 文檔)

**架構範例**:
```
ai-workflow-platform/
├── apps/
│   ├── web/                    # React 主應用
│   ├── workflow-editor/        # Vue 3 Workflow Editor (Phase 2)
│   ├── api-gateway/            # ASP.NET Core API Gateway
│   ├── agent-service/          # Semantic Kernel Agent Service
│   ├── code-interpreter/       # Code Interpreter Service
│   ├── knowledge-service/      # RAG Service
│   └── multi-agent-orchestrator/ # Multi-Agent Service
│
├── packages/
│   ├── shared-types/           # TypeScript 類型定義 (前後端共享)
│   ├── ui-components/          # React 共享組件庫
│   ├── sdk-dotnet/             # .NET SDK
│   └── sdk-typescript/         # TypeScript SDK
│
├── docs/                       # 所有規劃文檔 (已存在)
│   ├── brief.md
│   ├── user-stories/
│   ├── architecture/
│   ├── ux-design/
│   └── technical-implementation/
│
├── infra/                      # Infrastructure as Code
│   ├── terraform/
│   ├── kubernetes/
│   └── docker-compose/
│
├── tools/                      # 開發工具腳本
│   ├── scripts/
│   └── generators/
│
├── .github/                    # CI/CD workflows
│   └── workflows/
│
├── pnpm-workspace.yaml         # Monorepo 配置
├── turbo.json                  # Turborepo 配置
└── README.md
```

**優勢** (✅):
1. **代碼共享簡單**:
   - 前端可直接引用 `packages/shared-types`
   - 無需發布 npm package
   - 類型定義同步更新

2. **原子性提交**:
   - 前後端 API 變更在同一 commit
   - 避免版本不一致問題
   - Git history 完整

3. **統一 CI/CD**:
   - 單一 Pipeline 構建所有服務
   - 依賴變更自動檢測 (Turborepo)
   - 減少 CI 配置複雜度

4. **重構友好**:
   - 跨服務重構容易 (單一 IDE workspace)
   - Find & Replace 跨所有代碼
   - 統一 Linting/Formatting

5. **新人 Onboarding**:
   - Clone 一次即可開發
   - 無需配置多個倉庫
   - 文檔集中管理

**劣勢** (⚠️):
1. **倉庫大小**: 隨時間增長可能變大 (可用 Git LFS 緩解)
2. **構建時間**: 需要智能 Caching (Turborepo 解決)
3. **權限管理**: 無法細粒度控制 (通常不是問題)

**推薦工具**:
- **Turborepo**: Meta-build system,智能 caching
- **pnpm**: Fast, disk-efficient package manager
- **Changesets**: Version management & changelog

**成功案例**:
- Google (所有代碼在單一倉庫)
- Meta (React Monorepo)
- Vercel (Next.js Monorepo)

---

#### Option B: Polyrepo (不推薦 ❌)

**定義**: 多個獨立 Git 倉庫

**架構範例**:
```
ai-workflow-frontend/           # 倉庫 1
├── apps/web/
├── packages/ui-components/
└── packages/sdk-typescript/

ai-workflow-backend/            # 倉庫 2
├── services/api-gateway/
├── services/agent-service/
└── packages/sdk-dotnet/

ai-workflow-docs/               # 倉庫 3
└── docs/
```

**優勢** (✅):
1. 獨立部署節奏
2. 團隊權限隔離
3. 倉庫大小小

**劣勢** (⚠️):
1. **版本管理複雜**: 前後端版本對應困難
2. **跨倉庫重構困難**: 需要多個 PR
3. **開發體驗差**: 需要同時 clone 多個倉庫
4. **CI/CD 複雜**: 多個 Pipeline,依賴管理困難
5. **共享代碼發布**: 需要 npm/NuGet 發布流程

**適用場景**:
- 大型組織,團隊完全獨立
- 服務完全解耦,無共享代碼
- 需要細粒度權限控制

**不適合本專案的原因**:
- 本專案前後端緊密耦合
- 團隊規模小,無需權限隔離
- 共享類型定義多

---

### ✅ 決策建議: Monorepo (Option A)

**理由**:
1. ✅ 前後端 TypeScript 類型共享 (critical for type safety)
2. ✅ 原子性 API 變更 (避免版本不一致)
3. ✅ 簡化開發體驗 (單一 clone, 單一 IDE workspace)
4. ✅ 統一 CI/CD 和工具鏈
5. ✅ 適合團隊規模 (1-5 開發者)

**實施工具**:
```yaml
Monorepo 工具鏈:
  Build System: Turborepo
  Package Manager: pnpm
  Version Management: Changesets
  Linting: ESLint + Prettier (統一配置)
  Type Checking: TypeScript Project References
```

---

## 🎨 決策 2: 前端技術棧

### 技術選型分析

根據 **Architecture Design Document** 和 **UX Design**:

#### 前端框架組合

**主應用**: React 18 + TypeScript ✅

**理由**:
1. ✅ **生態系統成熟**: 豐富的組件庫和工具
2. ✅ **團隊熟悉度**: 廣泛採用,人才充足
3. ✅ **性能優秀**: React 18 Concurrent Rendering
4. ✅ **TypeScript 支持**: 一流的類型定義
5. ✅ **企業級應用**: 大量成功案例

**技術棧**:
```yaml
UI 框架: React 18
語言: TypeScript 5
狀態管理: Zustand (輕量) 或 Redux Toolkit
UI 組件庫:
  - shadcn/ui (推薦) - Tailwind CSS based
  - Ant Design (備選) - 企業級
路由: React Router v6
表單管理: React Hook Form + Zod
API 客戶端: TanStack Query (React Query)
構建工具: Vite
```

**Workflow Editor**: Vue 3 + VueFlow ✅ (Phase 2)

**理由**:
1. ✅ **VueFlow 專為 Vue 設計**: Vue 3 Composition API 原生支持
2. ✅ **PoC 驗證完成**: PoC 6 已驗證技術可行性 (98.2% 質量)
3. ✅ **性能優秀**: 60 FPS, <200ms 延遲
4. ✅ **CRDT 整合**: Yjs + VueFlow 完美配合

**技術棧**:
```yaml
UI 框架: Vue 3 (Composition API)
語言: TypeScript 5
Flow Library: VueFlow
協作: Yjs (CRDT)
狀態管理: Pinia
構建工具: Vite
```

**整合策略** (React + Vue 共存):

```
Monorepo 整合架構:
├── apps/
│   ├── web/                # React 18 (主應用)
│   │   ├── src/
│   │   └── vite.config.ts
│   │
│   └── workflow-editor/    # Vue 3 (Phase 2)
│       ├── src/
│       └── vite.config.ts
│
├── packages/
│   ├── shared-types/       # 前後端共享類型 (TypeScript)
│   ├── ui-components/      # React 共享組件
│   └── workflow-types/     # Workflow 專用類型
```

**通訊方式**:
1. **獨立應用**: React 和 Vue 各自獨立運行
2. **iframe 嵌入** (推薦): React 主應用通過 iframe 嵌入 Vue Workflow Editor
3. **postMessage API**: 跨應用通訊
4. **共享狀態**: 通過 API Gateway 同步

**iframe 整合範例**:
```typescript
// React 主應用
import { WorkflowEditorFrame } from '@/components/WorkflowEditorFrame';

function WorkflowPage() {
  return (
    <WorkflowEditorFrame
      workflowId={workflowId}
      onSave={handleSave}
      onExecute={handleExecute}
    />
  );
}

// WorkflowEditorFrame.tsx
export function WorkflowEditorFrame({ workflowId, onSave, onExecute }) {
  useEffect(() => {
    // postMessage 通訊
    window.addEventListener('message', (event) => {
      if (event.data.type === 'workflow:save') {
        onSave(event.data.workflow);
      }
    });
  }, []);

  return (
    <iframe
      src={`/workflow-editor?workflowId=${workflowId}`}
      style={{ width: '100%', height: '100vh' }}
    />
  );
}
```

---

### ✅ 決策建議: React 主應用 + Vue Workflow Editor (Phase 2)

**理由**:
1. ✅ **最佳工具適配**: React 適合 CRUD, Vue + VueFlow 適合可視化編輯
2. ✅ **技術驗證完成**: PoC 6 已證明可行性
3. ✅ **獨立演進**: React 和 Vue 可獨立升級
4. ✅ **團隊技能**: 利用各技術的優勢
5. ✅ **Phase 2 風險控制**: Workflow Editor 延後,不影響 MVP

**替代方案**:
- 如果希望統一技術棧 → 全 React + ReactFlow
- 但 VueFlow 性能和開發體驗更優

---

## 📁 決策 3: 專案結構與命名

### Monorepo 完整結構

```
ai-workflow-platform/
│
├── apps/                           # 應用層
│   ├── web/                        # React 主應用 (MVP)
│   │   ├── src/
│   │   │   ├── app/                # App 組件和路由
│   │   │   ├── features/           # 功能模組 (按 Epic 組織)
│   │   │   │   ├── agents/         # Epic 1: Agent 管理
│   │   │   │   ├── plugins/        # Epic 2: Plugin 系統
│   │   │   │   ├── chat/           # Epic 6: Chat Interface
│   │   │   │   ├── persona/        # Epic 7: Persona Framework
│   │   │   │   ├── code-interpreter/ # Epic 3: Code Interpreter
│   │   │   │   ├── text-to-sql/    # Epic 8: Text-to-SQL
│   │   │   │   ├── knowledge/      # Epic 5: Knowledge RAG
│   │   │   │   └── monitoring/     # Epic 10: Monitoring
│   │   │   ├── components/         # 共享 UI 組件
│   │   │   ├── lib/                # Utilities, API 客戶端
│   │   │   └── main.tsx            # Entry point
│   │   ├── public/
│   │   ├── package.json
│   │   ├── vite.config.ts
│   │   └── tsconfig.json
│   │
│   ├── workflow-editor/            # Vue 3 Workflow Editor (Phase 2)
│   │   ├── src/
│   │   │   ├── components/
│   │   │   ├── composables/
│   │   │   ├── stores/
│   │   │   └── main.ts
│   │   ├── package.json
│   │   └── vite.config.ts
│   │
│   ├── api-gateway/                # ASP.NET Core API Gateway
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── agent-service/              # Semantic Kernel Agent Service
│   │   ├── Services/
│   │   │   ├── AgentExecutionService.cs
│   │   │   ├── PluginService.cs
│   │   │   └── PersonaService.cs
│   │   ├── Models/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── code-interpreter/           # Code Interpreter Service
│   │   ├── Sandbox/
│   │   ├── Execution/
│   │   └── Security/
│   │
│   ├── knowledge-service/          # Knowledge RAG Service
│   │   ├── Retrieval/
│   │   ├── Embeddings/
│   │   └── TextToSQL/
│   │
│   └── multi-agent-orchestrator/  # Multi-Agent Orchestration
│       ├── Orchestration/
│       └── Communication/
│
├── packages/                       # 共享套件
│   ├── shared-types/               # TypeScript 類型定義
│   │   ├── src/
│   │   │   ├── api/                # API 請求/響應類型
│   │   │   ├── domain/             # Domain 模型
│   │   │   └── index.ts
│   │   ├── package.json
│   │   └── tsconfig.json
│   │
│   ├── ui-components/              # React 共享組件庫
│   │   ├── src/
│   │   │   ├── button/
│   │   │   ├── input/
│   │   │   ├── modal/
│   │   │   └── index.ts
│   │   ├── package.json
│   │   └── tsconfig.json
│   │
│   ├── sdk-dotnet/                 # .NET SDK
│   │   ├── AgentClient.cs
│   │   ├── PluginClient.cs
│   │   └── AgentFramework.SDK.csproj
│   │
│   └── sdk-typescript/             # TypeScript SDK
│       ├── src/
│       │   ├── client.ts
│       │   └── index.ts
│       ├── package.json
│       └── tsconfig.json
│
├── docs/                           # 所有規劃文檔 (已存在)
│   ├── brief.md
│   ├── user-stories/
│   │   ├── epics/                  # Epic 切分文檔
│   │   └── modules/
│   ├── architecture/
│   │   ├── components/             # 組件文檔
│   │   └── ADRs/
│   ├── ux-design/
│   ├── technical-implementation/
│   ├── project-management/
│   ├── BMAD-STAGE-ANALYSIS.md
│   ├── PO-VALIDATION-REPORT.md
│   ├── DOCUMENT-SHARDING-REPORT.md
│   └── PROJECT-INITIALIZATION-DECISION.md (本文檔)
│
├── infra/                          # Infrastructure as Code
│   ├── terraform/                  # Terraform 配置
│   │   ├── modules/
│   │   │   ├── aks/
│   │   │   ├── postgresql/
│   │   │   └── redis/
│   │   └── main.tf
│   │
│   ├── kubernetes/                 # Kubernetes Manifests
│   │   ├── base/
│   │   │   ├── api-gateway/
│   │   │   ├── agent-service/
│   │   │   └── ingress.yaml
│   │   └── overlays/
│   │       ├── dev/
│   │       ├── staging/
│   │       └── prod/
│   │
│   └── docker-compose/             # 本地開發環境
│       ├── docker-compose.yml
│       ├── docker-compose.dev.yml
│       └── docker-compose.test.yml
│
├── tools/                          # 開發工具
│   ├── scripts/
│   │   ├── setup-dev-env.sh
│   │   ├── run-migrations.sh
│   │   └── generate-types.sh
│   └── generators/
│       ├── epic-to-story.js
│       └── component-template.js
│
├── .github/                        # GitHub 配置
│   ├── workflows/
│   │   ├── ci.yml
│   │   ├── cd-staging.yml
│   │   └── cd-prod.yml
│   └── CODEOWNERS
│
├── .vscode/                        # VSCode 配置
│   ├── settings.json
│   ├── extensions.json
│   └── launch.json
│
├── pnpm-workspace.yaml             # pnpm Monorepo 配置
├── turbo.json                      # Turborepo 配置
├── .gitignore
├── .eslintrc.js                    # 統一 ESLint 配置
├── .prettierrc                     # 統一 Prettier 配置
├── tsconfig.base.json              # 基礎 TypeScript 配置
├── package.json                    # Root package.json
└── README.md
```

---

### 命名規範

#### 目錄命名
- **kebab-case**: `agent-service`, `code-interpreter`, `ui-components`
- **複數形式**: `apps`, `packages`, `tools`, `docs`

#### 文件命名

**TypeScript/JavaScript**:
- **組件**: PascalCase - `AgentCard.tsx`, `ChatMessage.tsx`
- **Utilities**: camelCase - `formatDate.ts`, `apiClient.ts`
- **類型定義**: PascalCase - `Agent.ts`, `Plugin.ts`
- **配置**: kebab-case - `vite.config.ts`, `tsconfig.json`

**C#**:
- **類**: PascalCase - `AgentService.cs`, `PluginController.cs`
- **接口**: IPascalCase - `IAgentRepository.cs`

#### Git 倉庫命名
- **Monorepo**: `ai-workflow-platform` ✅
- **備選**: `semantic-kernel-agentic-framework` (較長)

---

## 🛠️ 決策 4: 開發工具鏈

### IDE 推薦

**主要 IDE**: Visual Studio Code ✅

**理由**:
1. ✅ Monorepo 支持優秀 (Multi-root workspace)
2. ✅ TypeScript/React/Vue 一流支持
3. ✅ 豐富的擴展生態系統
4. ✅ 跨平台 (Windows, macOS, Linux)
5. ✅ 免費且輕量

**必裝擴展**:
```json
{
  "recommendations": [
    // TypeScript & React
    "dbaeumer.vscode-eslint",
    "esbenp.prettier-vscode",
    "bradlc.vscode-tailwindcss",

    // Vue
    "Vue.volar",
    "Vue.vscode-typescript-vue-plugin",

    // C#
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit",

    // Monorepo
    "arcanis.vscode-zipfs",

    // Git & Collaboration
    "eamodio.gitlens",
    "GitHub.copilot",

    // Productivity
    "usernamehw.errorlens",
    "gruntfuggly.todo-tree"
  ]
}
```

**備選 IDE** (C# 開發):
- **Visual Studio 2022**: 更強大的 C# 支持
- **JetBrains Rider**: 統一的 .NET + Frontend 開發

---

### Package Manager: pnpm ✅

**理由**:
1. ✅ **速度快**: 比 npm/yarn 快 2-3 倍
2. ✅ **節省磁碟**: 硬連結,共享依賴
3. ✅ **Monorepo 原生支持**: workspace 協議
4. ✅ **嚴格依賴**: 避免幽靈依賴問題

**配置**: `pnpm-workspace.yaml`
```yaml
packages:
  - 'apps/*'
  - 'packages/*'
```

---

### Build System: Turborepo ✅

**理由**:
1. ✅ **智能 Caching**: 只構建變更的部分
2. ✅ **並行執行**: 最大化 CI/CD 速度
3. ✅ **依賴圖**: 自動檢測依賴變更
4. ✅ **遠程 Caching**: 團隊共享構建緩存

**配置**: `turbo.json`
```json
{
  "$schema": "https://turbo.build/schema.json",
  "pipeline": {
    "build": {
      "dependsOn": ["^build"],
      "outputs": ["dist/**", ".next/**"]
    },
    "test": {
      "dependsOn": ["^build"],
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
```

---

### CI/CD: GitHub Actions ✅

**理由**:
1. ✅ **免費**: 對公開倉庫免費,私有倉庫 2000 分鐘/月
2. ✅ **整合度高**: 與 GitHub 原生整合
3. ✅ **生態系統**: 豐富的 Actions Marketplace
4. ✅ **矩陣構建**: 多環境並行測試

**Pipeline 範例**: `.github/workflows/ci.yml`
```yaml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - uses: pnpm/action-setup@v2
        with:
          version: 8

      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'pnpm'

      - name: Install dependencies
        run: pnpm install --frozen-lockfile

      - name: Lint
        run: pnpm turbo run lint

      - name: Type check
        run: pnpm turbo run type-check

      - name: Test
        run: pnpm turbo run test

      - name: Build
        run: pnpm turbo run build
```

---

### 協作工具

**版本控制**: Git + GitHub ✅
**專案管理**: GitHub Projects 或 Linear
**文檔**: Markdown (已在 `docs/`)
**API 文檔**: OpenAPI 3.0 + Swagger UI
**設計**: Figma (UX 高保真原型)

---

## 📋 決策總結表

| 決策項目 | 選項 | 狀態 | 理由 |
|---------|------|------|------|
| **倉庫策略** | ✅ Monorepo | 推薦 | 代碼共享,原子性提交,開發體驗 |
| **主應用框架** | ✅ React 18 + TS | 推薦 | 生態系統,團隊熟悉,企業級 |
| **Workflow Editor** | ✅ Vue 3 + VueFlow | 推薦 (Phase 2) | PoC 驗證,性能優秀 |
| **Package Manager** | ✅ pnpm | 推薦 | 速度,磁碟效率,Monorepo 支持 |
| **Build System** | ✅ Turborepo | 推薦 | 智能 Caching,並行構建 |
| **CI/CD** | ✅ GitHub Actions | 推薦 | 免費,整合度高 |
| **IDE** | ✅ VSCode | 推薦 | Monorepo 支持,跨平台,免費 |
| **倉庫命名** | ✅ `ai-workflow-platform` | 推薦 | 簡潔,描述性 |

---

## ⏭️ 下一步行動計劃

### Week 1: 專案結構建立 (4-6 小時)

#### Day 1-2: 倉庫初始化 (2-3 小時)

```bash
# 1. 初始化 Git 倉庫 (如果尚未初始化)
cd "C:\AI Semantic Kernel"
git init
git branch -M main

# 2. 建立 Monorepo 結構
mkdir -p apps/web apps/api-gateway apps/agent-service
mkdir -p packages/shared-types packages/ui-components
mkdir -p infra/docker-compose infra/kubernetes
mkdir -p tools/scripts

# 3. 初始化 pnpm workspace
cat > pnpm-workspace.yaml << EOF
packages:
  - 'apps/*'
  - 'packages/*'
EOF

# 4. Root package.json
cat > package.json << EOF
{
  "name": "ai-workflow-platform",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "dev": "turbo run dev",
    "build": "turbo run build",
    "test": "turbo run test",
    "lint": "turbo run lint",
    "type-check": "turbo run type-check"
  },
  "devDependencies": {
    "turbo": "^1.10.0",
    "@typescript-eslint/eslint-plugin": "^6.0.0",
    "@typescript-eslint/parser": "^6.0.0",
    "eslint": "^8.0.0",
    "prettier": "^3.0.0",
    "typescript": "^5.0.0"
  }
}
EOF

# 5. 安裝根依賴
pnpm install

# 6. 初始化 Turborepo
cat > turbo.json << EOF
{
  "$schema": "https://turbo.build/schema.json",
  "pipeline": {
    "build": {
      "dependsOn": ["^build"],
      "outputs": ["dist/**", ".next/**", "build/**"]
    },
    "dev": {
      "cache": false,
      "persistent": true
    },
    "test": {
      "dependsOn": ["^build"],
      "outputs": ["coverage/**"]
    },
    "lint": {
      "outputs": []
    },
    "type-check": {
      "outputs": []
    }
  }
}
EOF
```

#### Day 3-4: 前端專案設置 (2-3 小時)

```bash
# 1. 初始化 React 主應用
cd apps
pnpm create vite web --template react-ts
cd web

# 2. 安裝依賴
pnpm add @tanstack/react-query zustand react-router-dom react-hook-form zod
pnpm add -D @types/node tailwindcss postcss autoprefixer

# 3. 初始化 Tailwind CSS
pnpm dlx tailwindcss init -p

# 4. 建立功能模組目錄
mkdir -p src/features/{agents,plugins,chat,persona,monitoring}
mkdir -p src/components src/lib

# 5. 配置 TypeScript path aliases
# 編輯 vite.config.ts 和 tsconfig.json
```

#### Day 5: 後端專案設置 (1-2 小時)

```bash
# 1. 初始化 API Gateway
cd apps
dotnet new webapi -n api-gateway
cd api-gateway

# 2. 安裝必要套件
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# 3. 初始化 Agent Service
cd ../
dotnet new webapi -n agent-service
cd agent-service

# 4. 安裝 Semantic Kernel
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
```

#### Day 6: 共享套件設置 (1 小時)

```bash
# 1. Shared Types
cd packages
mkdir shared-types
cd shared-types

pnpm init
pnpm add -D typescript

# 2. 建立基礎類型
mkdir -p src/{api,domain}
cat > src/domain/Agent.ts << EOF
export interface Agent {
  id: string;
  name: string;
  description: string;
  modelName: string;
  systemPrompt: string;
  temperature: number;
  maxTokens: number;
  createdAt: Date;
  updatedAt: Date;
}
EOF

# 3. TypeScript 配置
cat > tsconfig.json << EOF
{
  "extends": "../../tsconfig.base.json",
  "compilerOptions": {
    "outDir": "./dist",
    "rootDir": "./src"
  },
  "include": ["src"]
}
EOF
```

---

### Week 2: 開發環境驗證 (2-3 小時)

#### Docker Compose 設置

```bash
cd infra/docker-compose

cat > docker-compose.yml << EOF
version: '3.8'

services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: agent_framework
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  rabbitmq:
    image: rabbitmq:3-management-alpine
    environment:
      RABBITMQ_DEFAULT_USER: rabbitmq
      RABBITMQ_DEFAULT_PASS: rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"

volumes:
  postgres_data:
  redis_data:
EOF

# 啟動服務
docker-compose up -d

# 驗證
docker-compose ps
```

---

### Week 3: CI/CD Pipeline (1-2 小時)

```bash
mkdir -p .github/workflows

cat > .github/workflows/ci.yml << EOF
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
        with:
          version: 8
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'pnpm'
      - run: pnpm install
      - run: pnpm turbo run lint test build

  backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
EOF
```

---

## ✅ 決策檢查清單

在開始實施前,請確認以下決策:

### 核心決策 (必須)
- [ ] ✅ **Monorepo 策略確認**: 團隊同意使用 Monorepo
- [ ] ✅ **React + Vue 混合方案確認**: 技術選型通過
- [ ] ✅ **倉庫命名確認**: `ai-workflow-platform`
- [ ] ✅ **工具鏈確認**: pnpm + Turborepo + GitHub Actions

### 環境準備 (必須)
- [ ] ✅ **開發環境**: Node.js 20+, .NET 8+, Docker Desktop
- [ ] ✅ **IDE 安裝**: VSCode + 必要擴展
- [ ] ✅ **Git 配置**: GitHub 倉庫建立,權限設置
- [ ] ✅ **Azure 資源**: Azure OpenAI API Key, PostgreSQL (如需)

### 團隊準備 (可選)
- [ ] 🟡 **團隊培訓**: Monorepo 工作流程培訓
- [ ] 🟡 **Vue 3 培訓**: 如果團隊不熟悉 (針對 Phase 2)
- [ ] 🟡 **文檔 Review**: 所有團隊成員閱讀 Epic 文檔

---

## 🎯 最終決策建議

### 推薦配置 (Production Ready)

```yaml
倉庫策略: Monorepo ✅
  Tool: pnpm + Turborepo
  Repository: ai-workflow-platform

前端技術棧:
  主應用: React 18 + TypeScript ✅
  Workflow Editor: Vue 3 + VueFlow (Phase 2) ✅
  UI 框架: shadcn/ui + Tailwind CSS
  狀態管理: Zustand
  Build: Vite

後端技術棧:
  API Gateway: ASP.NET Core 8
  Agent Service: Semantic Kernel + .NET 8
  數據庫: PostgreSQL 16
  快取: Redis 7
  訊息佇列: RabbitMQ 3

開發工具:
  IDE: Visual Studio Code ✅
  Package Manager: pnpm ✅
  Build System: Turborepo ✅
  CI/CD: GitHub Actions ✅
  Version Control: Git + GitHub ✅

基礎設施:
  容器化: Docker + Docker Compose
  編排: Kubernetes (AKS)
  IaC: Terraform
```

---

## 📄 相關文檔

- [BMad Stage Analysis](BMAD-STAGE-ANALYSIS.md) - BMad Method 階段分析
- [PO Validation Report](../claudedocs/PO-VALIDATION-REPORT-FINAL.md) - PO 驗證報告
- [Document Sharding Report](DOCUMENT-SHARDING-REPORT.md) - 文檔切分報告
- [Architecture Design Document](architecture/Architecture-Design-Document.md) - 架構設計
- [Technical Implementation](technical-implementation/) - 技術實施文檔

---

## ✅ 決策狀態

**決策完成**: 待確認
**決策者**: Product Owner + Tech Lead
**決策日期**: 2025-11-02
**實施開始**: Week 1 (決策通過後)

---

**文檔版本**: v1.0.0
**最後更新**: 2025-11-02
**下次更新**: 專案結構建立完成後
