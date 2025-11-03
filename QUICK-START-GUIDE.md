# 快速啟動指南 (Quick Start Guide)

**版本**: 1.0.0
**日期**: 2025-11-03
**專案**: Semantic Kernel Agentic Framework MVP
**狀態**: ✅ 準備就緒 (98%)

---

## 👋 歡迎加入專案!

這份指南幫助您快速了解專案狀態、找到所需文檔、開始貢獻代碼。

---

## 🎯 專案概覽 (5分鐘速讀)

### 什麼是這個專案?

構建一個基於 **Microsoft Semantic Kernel** 的 **企業級 AI Agent 框架**,讓內部開發團隊和業務部門能夠快速創建、管理、部署 AI Agent。

### 為什麼需要它?

- ❌ **Copilot Studio 的教訓**: No-Code 工具的根本性限制
- ❌ **內部開發挑戰**: 重複造輪子、技術債務、集成困難
- ✅ **解決方案**: Pro-code 框架 + SDK + 可視化工具 + 企業級功能

### 核心差異化功能 (5個)

1. ⭐ **Persona Builder** - 引導式 Persona 創建 (50+ 模板)
2. ⭐ **Precise Retrieval** - >90% 準確率檢索 (超越 Copilot Studio)
3. ⭐ **Code Interpreter** - 安全沙箱執行 (對標 Fujitsu Kozuchi)
4. ⭐ **Text-to-SQL** - 自然語言轉 SQL
5. ⭐ **Multi-Agent Workflow** - 可視化 Workflow 編排

### 技術棧

**Backend**: .NET 8 + C# 12 + Entity Framework Core 8 + Semantic Kernel 1.66
**Frontend**: React 18 + TypeScript 5.8 + Vite + Material-UI + Zustand
**Database**: PostgreSQL 16 + Qdrant (Vector DB) + Redis
**DevOps**: Docker + Kubernetes (Azure AKS) + GitHub Actions + Azure Bicep
**Monitoring**: OpenTelemetry + Prometheus + Grafana

### 時程與里程碑

- **Week 0-3**: 準備階段 ✅ (已完成, 98% 準備度)
- **Week 4**: Sprint 0 - 環境設置 ⏳ (下一步)
- **Week 5-58**: MVP 開發 (Sprint 1-18, 54週)
- **Week 59-68**: Workflow Editor (Phase 2, Optional)

**預期 MVP 完成時間**: 12-13個月 (54-58週)

---

## 📚 文檔導航 (按角色分類)

### 🏢 管理層 / 決策者 (15分鐘閱讀)

**首先閱讀**:
1. `docs/brief-1-overview.md` - 專案背景、核心問題、解決方案
2. `docs/brief-2-requirements.md` (Goals & Success Metrics 章節) - 成功標準與 KPI
3. `claudedocs/PROJECT-STATUS-REPORT.md` (執行摘要) - 當前狀態與風險

**關注重點**:
- 為什麼需要這個框架? (Problem Statement)
- ROI 和成功指標是什麼?
- 主要風險和緩解措施

---

### 💼 產品經理 / 業務分析師 (45分鐘閱讀)

**首先閱讀**:
1. `docs/brief-README.md` - 文檔導航索引
2. `docs/brief-1-overview.md` - 概覽與願景
3. `docs/brief-2-requirements.md` - 需求與用戶分析 (完整閱讀)
4. `docs/user-stories/mvp-planning.md` - MVP 規劃 (300-350 SP)
5. `claudedocs/COMPLETE-DEVELOPMENT-TIMELINE.md` - 68週時間表

**關注重點**:
- 目標用戶是誰?痛點是什麼?
- MVP 包含哪些功能?排除什麼?
- Phase 1 vs Phase 2 的邊界
- 125+ User Stories 詳細內容

**次要閱讀**:
- `docs/user-stories/sprints/sprint-allocation.md` - Sprint 1-18 分配矩陣
- `docs/ux-design/wireframes/low-fidelity/` - 12個頁面線框圖

---

### 🏗️ 技術負責人 / 架構師 (90分鐘閱讀)

**首先閱讀**:
1. `docs/brief-3-technical.md` - 技術方案與MVP (完整閱讀)
2. `claudedocs/SPRINT-0-PREPARATION-PLAN.md` - Sprint 0 詳細準備計劃
3. `claudedocs/SPRINT-1-2-ROADMAP.md` - Sprint 1-2 執行路線圖
4. `docs/technical-implementation/TECH-STACK-ANALYSIS.md` - 技術選型分析
5. `poc-projects/POC-1-6-COMPLETE-VALIDATION-REPORT.md` - PoC 驗證報告

**關注重點**:
- 技術棧選擇理由
- Semantic Kernel 依賴度和能力映射 (35% SK + 65% 自建)
- Code Interpreter 安全機制 (4層防護)
- 13項核心交付物的技術細節
- 性能基線和質量門檻 (API <200ms, Agent <5s)

**次要閱讀**:
- `docs/architecture/ADR-012-workflow-editor-technology.md` - Workflow Editor 技術選型
- `docs/technical-implementation/01-SYSTEM-ARCHITECTURE.md` - 系統架構設計
- `docs/technical-implementation/4-coding-standards/` - C# 和 TypeScript 編碼標準

---

### 👨‍💻 後端開發工程師 (.NET) (60分鐘閱讀)

**首先閱讀**:
1. `claudedocs/SPRINT-0-PREPARATION-PLAN.md` (Backend 部分) - 項目腳手架
2. `claudedocs/SPRINT-1-2-ROADMAP.md` (Sprint 1 Week 1) - Agent CRUD 實現
3. `docs/technical-implementation/4-coding-standards/csharp-coding-standards.md` - C# 編碼標準
4. `docs/technical-implementation/5-api-design/restful-api-standards.md` - API 設計規範
5. `docs/technical-implementation/6-database-standards/` - 數據庫設計與 EF Core

**關注重點**:
- .NET 8 + C# 12 項目結構
- Entity Framework Core 8.0 最佳實踐
- Repository Pattern、Service Layer、Controller 實現
- Semantic Kernel 1.66 集成方式
- 單元測試標準 (xUnit, 80%+ 覆蓋率)

**實用代碼範例**:
- `Agent.cs` Entity Model (SPRINT-1-2-ROADMAP.md 中)
- `AgentRepository.cs` Repository Pattern
- `AgentExecutionService.cs` Semantic Kernel 集成
- `Program.cs` DI 配置

**次要閱讀**:
- `poc-projects/poc1-sk-agents/` - PoC 1 Semantic Kernel Agents 實現
- `docs/technical-implementation/7-testing-strategy/unit-testing-standards.md` - 單元測試標準

---

### 👩‍💻 前端開發工程師 (React) (60分鐘閱讀)

**首先閱讀**:
1. `claudedocs/SPRINT-0-PREPARATION-PLAN.md` (Frontend 部分) - React 18 腳手架
2. `claudedocs/SPRINT-1-2-ROADMAP.md` (Sprint 1 Day 3) - Agent 管理 UI 實現
3. `docs/technical-implementation/4-coding-standards/typescript-coding-standards.md` - TypeScript 編碼標準
4. `docs/technical-implementation/4-coding-standards/react-coding-standards.md` - React 組件設計標準
5. `docs/ux-design/wireframes/low-fidelity/01-agent-list.md` - Agent 列表頁線框圖

**關注重點**:
- React 18 + TypeScript 5.8 + Vite 項目結構
- Zustand 狀態管理模式
- Material-UI v5 組件使用
- API Service 層實現 (axios)
- E2E 測試標準 (Playwright)

**實用代碼範例**:
- `agentStore.ts` Zustand Store (SPRINT-1-2-ROADMAP.md 中)
- `agentService.ts` API Service
- `AgentList.tsx` React 組件
- `vite.config.ts` Vite 配置

**次要閱讀**:
- `docs/ux-design/design-system/` - 設計系統 (顏色、組件庫、Accessibility)
- `docs/ux-design/wireframes/low-fidelity/` - 12個頁面線框圖

---

### 🎨 UX 設計師 / 前端設計 (30分鐘閱讀)

**首先閱讀**:
1. `docs/brief-1-overview.md` (Web/Mobile App 連接方式) - 系統架構概覽
2. `docs/ux-design/design-system/` - 完整設計系統 (4個文檔)
   - `accessibility-guidelines.md` - WCAG 2.1 AA 標準
   - `color-palette.md` - 色彩系統
   - `component-library.md` - UI 組件庫
   - `design-tokens.md` - Design Tokens
3. `docs/ux-design/wireframes/low-fidelity/` - 12個頁面線框圖
4. `docs/brief-2-requirements.md` (業務部門用戶需求) - 用戶角色與場景

**關注重點**:
- React 18+ SPA 架構
- 6個核心 UI 組件 (ChatWindow, AgentConfigForm 等)
- 響應式設計要求 (Desktop + Tablet)
- 客服場景的完整用戶流程
- Accessibility 要求 (WCAG 2.1 AA)

---

### 🚀 DevOps 工程師 (45分鐘閱讀)

**首先閱讀**:
1. `claudedocs/SPRINT-0-PREPARATION-PLAN.md` (Day 2-5) - Docker + Azure + CI/CD
2. `docs/technical-implementation/8-deployment-architecture/` - 部署架構 (4個文檔)
   - `docker-containerization.md` - Docker 容器化
   - `kubernetes-deployment.md` - Kubernetes 部署
   - `cicd-pipeline-github-actions.md` - CI/CD 管線
   - `azure-infrastructure-setup.md` - Azure 基礎設施
3. `docs/technical-implementation/10-monitoring-operations/` - 監控與運維 (5個文檔)

**關注重點**:
- Docker Compose 本地開發環境
- Azure Bicep IaC 模板
- GitHub Actions CI/CD 工作流
- Kubernetes (Azure AKS) 部署
- Prometheus + Grafana 監控
- OpenTelemetry 日誌與追蹤

**實用代碼範例**:
- `docker-compose.dev.yml` (SPRINT-0-PREPARATION-PLAN.md 中)
- `main.bicep` Azure Bicep 模板
- `.github/workflows/ci-backend.yml` GitHub Actions
- `.github/workflows/ci-frontend.yml` GitHub Actions

---

### 🧪 QA 工程師 / 測試工程師 (45分鐘閱讀)

**首先閱讀**:
1. `docs/technical-implementation/7-testing-strategy/` - 測試策略 (5個文檔)
   - `unit-testing-standards.md` - 單元測試標準 (xUnit, Vitest)
   - `integration-testing-standards.md` - 集成測試標準
   - `end-to-end-testing-standards.md` - E2E 測試標準 (Playwright)
   - `test-coverage-strategy.md` - 測試覆蓋率策略
   - `test-automation-cicd.md` - 測試自動化與 CI/CD
2. `docs/technical-implementation/9-security-standards/security-testing-automation.md` - 安全測試標準
3. `claudedocs/SPRINT-1-2-ROADMAP.md` (DoD 部分) - 驗收標準

**關注重點**:
- 測試覆蓋率目標 ≥80%
- xUnit (.NET Backend) + Vitest (React Frontend) + Playwright (E2E)
- API 性能測試 (響應時間 <200ms)
- Agent 執行時間測試 (<5s average)
- 安全測試 (OWASP Top 10, Code Interpreter 滲透測試)

---

## 🚀 立即開始 (Sprint 0 執行)

### 前置條件檢查

**1. 開發環境**:
- [ ] .NET 8 SDK 已安裝
- [ ] Node.js 18+ (含 pnpm) 已安裝
- [ ] Docker Desktop 已安裝
- [ ] Git 已配置 (SSH Key 或 HTTPS)
- [ ] IDE 已設定 (Visual Studio / VS Code / Rider)

**2. Azure 訂閱**:
- [ ] Azure 訂閱已就緒
- [ ] 權限已確認 (Contributor 或以上)
- [ ] Azure CLI 已安裝並登錄

**3. 工具**:
- [ ] Azure OpenAI 資源已創建 (或有權限創建)
- [ ] PostgreSQL 16 可用 (Docker 或 Azure)

---

### Day 1 快速啟動 (2小時)

**Step 1: Monorepo 初始化** (30分鐘)

```bash
# 1. 克隆專案 (或創建新專案)
mkdir semantic-kernel-agentic-framework
cd semantic-kernel-agentic-framework
git init
git branch -M main

# 2. 創建 pnpm-workspace.yaml
cat > pnpm-workspace.yaml <<EOF
packages:
  - 'apps/*'
  - 'packages/*'
  - 'services/*'
EOF

# 3. 創建基礎目錄結構
mkdir -p apps/web-app apps/workflow-editor apps/api-gateway
mkdir -p packages/dotnet-sdk packages/python-sdk packages/ui-components packages/shared-types
mkdir -p services/agent-service services/knowledge-service services/code-interpreter-service services/workflow-service
mkdir -p infrastructure/bicep infrastructure/docker infrastructure/kubernetes
```

**Step 2: Backend 項目腳手架** (45分鐘)

```bash
# 1. 初始化 .NET 8 Agent Service
cd services/agent-service
dotnet new webapi -n AgentService -f net8.0
cd AgentService

# 2. 安裝必要套件
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0

# 3. 驗證 Build
dotnet build
```

**Step 3: Frontend 項目腳手架** (45分鐘)

```bash
# 1. 初始化 React 18 Web App
cd ../../../apps/web-app
pnpm create vite@latest . --template react-ts

# 2. 安裝必要套件
pnpm install
pnpm add @mui/material @emotion/react @emotion/styled
pnpm add zustand axios react-router-dom
pnpm add -D @types/react @types/react-dom

# 3. 驗證 Build
pnpm build
pnpm dev
```

**Step 4: Git 初始 Commit** (10分鐘)

```bash
cd ../../
git add .
git commit -m "feat: Initialize monorepo structure (Sprint 0 Day 1)

- pnpm workspace configuration
- .NET 8 Agent Service scaffolding
- React 18 Web App scaffolding
- Monorepo directory structure

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

---

### Day 2-5 任務清單

**Day 2: Docker Compose 開發環境** (參照 SPRINT-0-PREPARATION-PLAN.md)
- [ ] 創建 `docker-compose.dev.yml`
- [ ] PostgreSQL 16 容器配置
- [ ] Redis 7 容器配置
- [ ] Qdrant 1.7.4 容器配置
- [ ] 驗證所有容器運行正常

**Day 3-4: Azure 基礎設施** (參照 SPRINT-0-PREPARATION-PLAN.md)
- [ ] 創建 Bicep 模板 (`main.bicep`)
- [ ] 部署 Azure PostgreSQL
- [ ] 配置 Azure OpenAI
- [ ] 配置 Azure Storage
- [ ] 驗證所有 Azure 資源

**Day 5: CI/CD Pipeline** (參照 SPRINT-0-PREPARATION-PLAN.md)
- [ ] 創建 `.github/workflows/ci-backend.yml`
- [ ] 創建 `.github/workflows/ci-frontend.yml`
- [ ] 驗證 CI 管線運行正常
- [ ] 配置代碼覆蓋率報告

---

## 📋 常見問題 (FAQ)

### Q1: 我應該從哪裡開始?

**A**: 根據您的角色,參照上方 **"文檔導航 (按角色分類)"** 章節,找到您的角色對應的閱讀清單。

---

### Q2: 所有文檔都在哪裡?

**A**: 文檔分為3個主要目錄:
- `claudedocs/` - 規劃文檔 (9個文件, 決策、驗證、時間表)
- `docs/` - 專案文檔 (Brief、User Stories、技術實施、UX 設計、架構)
- `poc-projects/` - PoC 驗證報告 (6個 PoC 驗證)

**快速導航**:
- Brief 導航: `docs/brief-README.md`
- 技術實施索引: `docs/technical-implementation/TID-INDEX.md`
- PoC 驗證報告: `poc-projects/POC-1-6-COMPLETE-VALIDATION-REPORT.md`

---

### Q3: Sprint 0 什麼時候開始?

**A**: **可立即開始** (Week 4)

**前置條件**:
- ✅ 文檔完整 (98%)
- ✅ 技術選型確定
- ✅ 團隊組建完成 (假設)
- ✅ Azure 訂閱準備

**執行計劃**: 參照 `claudedocs/SPRINT-0-PREPARATION-PLAN.md`

---

### Q4: MVP 什麼時候完成?

**A**: 預期 **Week 54-58** (約 **12-13個月**)

**里程碑**:
- M0: Planning Complete (Week 0-3) ✅
- M1: Environment Ready (Week 4) ⏳
- M2: Agent Foundation (Week 7) ⏳
- M3: Core Capabilities (Week 13) ⏳
- M4: Differentiators (Week 27) ⏳
- M5: Multi-Agent (Week 39) ⏳
- M6: Enterprise (Week 51) ⏳
- M7: MVP Complete (Week 54) ⏳

---

### Q5: 我的代碼應該遵循什麼標準?

**A**:
- **C# / .NET**: `docs/technical-implementation/4-coding-standards/csharp-coding-standards.md`
- **TypeScript / React**: `docs/technical-implementation/4-coding-standards/typescript-coding-standards.md`
- **API 設計**: `docs/technical-implementation/5-api-design/restful-api-standards.md`
- **測試**: `docs/technical-implementation/7-testing-strategy/unit-testing-standards.md`

**關鍵要求**:
- 測試覆蓋率 ≥80%
- API 響應時間 <200ms
- 遵循 Repository Pattern、Service Layer 等設計模式

---

### Q6: 如何參與 Sprint Planning?

**A**: Sprint 1 Planning Meeting 將在 **Week 5 Day 1** 舉行 (4小時)

**議程**:
- Part 1: Sprint 0 回顧 (30分鐘)
- Part 2: Sprint 1 規劃 (2小時) - US 1.1-1.3
- Part 3: 技術準備 (1小時) - 設計討論
- Part 4: Sprint 1 Kickoff (30分鐘)

**準備**:
- 閱讀 `claudedocs/SPRINT-1-2-ROADMAP.md`
- 閱讀 `docs/user-stories/sprints/sprint-allocation.md` (Sprint 1 部分)
- 準備問題與技術挑戰討論

---

### Q7: 遇到技術問題怎麼辦?

**A**:
1. **查閱技術實施文檔**: `docs/technical-implementation/` (50+ 文檔)
2. **查看 PoC 範例代碼**: `poc-projects/poc1-sk-agents/`, `poc-projects/poc3-code-interpreter/` 等
3. **詢問 Tech Lead / Backend Lead / Frontend Lead**
4. **在 Daily Standup 中提出阻塞**

---

### Q8: 如何提交代碼?

**A**: 遵循 Git 工作流:
1. 創建 Feature Branch (`git checkout -b feature/US-1.1-agent-crud`)
2. 開發並提交 (`git commit -m "feat: implement Agent CRUD API"`)
3. 推送到遠端 (`git push -u origin feature/US-1.1-agent-crud`)
4. 創建 Pull Request (PR)
5. 代碼審查 (Code Review) - 目標 <4 小時
6. CI/CD 自動測試通過
7. 合併到 `develop` 分支

**Commit Message 規範**: 參照 `docs/technical-implementation/4-coding-standards/` (Git 部分)

---

## 📞 聯絡方式

### 團隊角色

- **Product Owner**: [待填寫]
- **Scrum Master**: [待填寫]
- **Tech Lead**: [待填寫]
- **Backend Lead**: [待填寫]
- **Frontend Lead**: [待填寫]
- **DevOps Engineer**: [待填寫]

### 會議時間

- **Daily Standup**: [待確定] (15分鐘)
- **Sprint Planning**: Week 5 Day 1, 09:00-13:00 (4小時)
- **Sprint Review**: 每 Sprint 最後一天 (2小時)
- **Sprint Retrospective**: 每 Sprint 最後一天 (1小時)

---

## 🎉 結語

歡迎加入 **Semantic Kernel Agentic Framework MVP** 專案!

我們已經完成了 **98% 的準備工作**,現在是時候開始構建了!

**下一步**: 參加 Sprint 0 Kickoff Meeting,開始 Day 1 任務。

**Let's Build Something Amazing! 🚀**

---

**文檔維護**:
- 當前版本: 1.0.0 (2025-11-03)
- 負責人: Tech Lead
- 更新頻率: 每個里程碑後更新

**相關文檔**:
- 準備階段完成總結: `claudedocs/PREPARATION-PHASE-COMPLETION-SUMMARY.md`
- 專案狀態報告: `claudedocs/PROJECT-STATUS-REPORT.md`
- Sprint 0 準備計劃: `claudedocs/SPRINT-0-PREPARATION-PLAN.md`
