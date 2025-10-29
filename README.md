# Semantic Kernel Agentic Framework

企業級 Multi-Agent 協作框架，基於 Microsoft Semantic Kernel 構建，專為彌補 Microsoft Copilot Studio 的核心能力缺失而設計。

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-latest-brightgreen.svg)](docs/README.md)
[![BMad Method](https://img.shields.io/badge/BMad-Method-orange.svg)](.bmad-core/user-guide.md)

---

## 🎯 項目概覽

**Semantic Kernel Agentic Framework** 提供完整的企業級 AI Agent 開發和管理平台，具備以下核心差異化能力：

### 核心差異化能力 ⭐

| 能力 | 描述 | 優勢 |
|------|------|------|
| **Persona Framework** | 結構化個性配置系統 | 一致的對話風格和行為模式（>85% 一致性） |
| **Code Interpreter** | 4 層安全沙箱 | 安全執行 Python/R 代碼，數據分析能力 |
| **Text-to-SQL** | 智能結構化數據查詢 | 自然語言轉 SQL（>85% 準確率） |
| **Knowledge Management** | 混合檢索 + Reranking | 90%+ 檢索準確率（Recall@10） |
| **Multi-Agent Workflow** | 可視化工作流編輯器 | 拖拽式 Multi-Agent 編排 |
| **Multimodal Chat** | 多模態對話介面 | 圖片、圖表、代碼互動展示 |

---

## 📚 文檔架構

完整的項目文檔使用 **BMad Method** 開發方法論組織：

### 快速導航

- **[📚 主索引](docs/README.md)** - 完整文檔導航和進度追蹤
- **[📋 User Stories](docs/user-stories/README.md)** - 43 個 User Stories，分 10 個模組
- **[🏗️ 架構設計](docs/architecture/Architecture-Design-Document.md)** - 系統架構和技術決策
- **[📊 項目管理](docs/project-management/Project-Management-Plan.md)** - 項目管理計劃
- **[📋 實施策略](docs/user-stories/implementation-strategy.md)** - 4 大核心能力技術難點與解決方案
- **[🎯 MVP 規劃](docs/user-stories/mvp-planning.md)** - MVP 範圍與時程規劃

### BMad Method 進度追蹤

```
✅ Phase 1 - Business (Analyst): Project Brief
✅ Phase 2 - Management (PM): Project Management Plan
✅ Phase 3.1 - Architecture: ADD + 4 ADRs
✅ Phase 3.2 - Requirements: 43 User Stories + Sprint Planning
⏸️ Phase 3.3 - UI/UX Designer (待開始 - 2025-11-01)
⏸️ Phase 3.4 - Tech Lead (待開始 - 2025-11-01)
⏸️ Phase 3.5 - Integration (待開始 - 2025-11-22)
⏸️ Phase 4 - Development: 18 Sprints × 3 weeks = 54 weeks
```

---

## 🏗️ 技術棧

### 後端
- **.NET 8** - Runtime 和 Web Framework
- **ASP.NET Core 8** - API Server
- **Entity Framework Core 8** - ORM
- **Semantic Kernel 1.x** - LLM 整合框架
- **PostgreSQL 16** - 主數據庫
- **Redis 7** - 緩存和狀態管理
- **Azure AI Search** - 向量數據庫

### 前端
- **React 18** - UI 框架
- **TypeScript 5** - 類型安全
- **Material-UI (MUI)** - UI 組件庫
- **Redux Toolkit** - 狀態管理
- **Vite** - 構建工具

### LLM 整合
- **Azure OpenAI** - 主要 LLM Provider
- **OpenAI API** - 備選 Provider
- **Anthropic Claude** - 備選 Provider
- **text-embedding-ada-002** - Embedding 模型

### DevOps
- **Docker** - 容器化
- **Kubernetes (AKS)** - 容器編排
- **GitHub Actions** - CI/CD
- **Terraform** - Infrastructure as Code
- **Prometheus + Grafana** - 監控

---

## 📊 項目統計

```yaml
文檔統計:
  總文件數: 32 個
  總行數: 29,319 行
  User Stories: 43 個（P0: 29, P1: 10, P2: 4）
  Story Points: 215 點
  預計工時: 300-350 Story Points（含測試和文檔）

開發規劃:
  MVP 時程: 10-12 個月（18 Sprints）
  Sprint 長度: 3 週
  核心團隊: 5 人
  預算: TWD $800,000

架構決策:
  ADRs: 4 個核心架構決策記錄
  技術棧決策: 後端、前端、LLM、DevOps 完整定義
  安全層級: 4 層安全防護（Code Interpreter & Text-to-SQL）
```

---

## 🚀 快速開始

### Prerequisites

```bash
# Windows
winget install Microsoft.DotNet.SDK.8
winget install OpenJS.NodeJS.LTS
winget install Docker.DockerDesktop

# macOS
brew install dotnet@8
brew install node@20
brew install --cask docker
```

### 本地開發環境

```bash
# 1. Clone Repository
git clone https://github.com/laitim2001/ai-semantic-kernel-framework.git
cd ai-semantic-kernel-framework

# 2. 啟動依賴服務（Docker Compose）
docker-compose up -d

# 3. 初始化數據庫
dotnet ef database update

# 4. 啟動 API Server
cd src/Api
dotnet run

# 5. 啟動 Frontend Dev Server
cd src/Web
npm install
npm run dev
```

---

## 🤝 貢獻指南

本項目使用 **BMad Method** 開發方法論。如果您想貢獻：

1. **閱讀文檔**: 從 [主索引](docs/README.md) 開始
2. **理解 User Stories**: 查看 [User Stories README](docs/user-stories/README.md)
3. **遵循架構**: 參考 [Architecture Design Document](docs/architecture/Architecture-Design-Document.md)
4. **提交 PR**: 確保通過所有測試和代碼審查

### 開發工作流

```bash
# 1. 創建功能分支
git checkout -b feature/your-feature-name

# 2. 開發和測試
# ... 進行開發 ...

# 3. 提交變更
git add .
git commit -m "feat: your feature description

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>"

# 4. 推送和創建 PR
git push origin feature/your-feature-name
```

---

## 📖 BMad Method

本項目採用 **BMad Method** - 一個完整的 AI 輔助軟體開發方法論：

- **[.bmad-core/](.bmad-core/)** - 核心 BMad Method 框架
- **[.bmad-creative-writing/](.bmad-creative-writing/)** - 創意寫作擴展
- **[.bmad-infrastructure-devops/](.bmad-infrastructure-devops/)** - 基礎設施擴展

BMad Method 提供：
- ✅ 10 個專業角色（Analyst, Architect, PM, PO, Dev, QA, SM, UX...）
- ✅ 50+ 標準化任務和工作流
- ✅ 20+ 檢查清單
- ✅ 完整的文檔模板

詳見：[.bmad-core/user-guide.md](.bmad-core/user-guide.md)

---

## 📄 授權

本項目採用 MIT License - 詳見 [LICENSE](LICENSE) 文件。

---

## 🙏 致謝

- **Microsoft Semantic Kernel** - 核心 LLM 整合框架
- **BMad Method** - AI 輔助開發方法論
- **Claude Code** - AI 輔助開發工具

---

## 📞 聯繫方式

- **GitHub Issues**: [提交問題或建議](https://github.com/laitim2001/ai-semantic-kernel-framework/issues)
- **GitHub Discussions**: [參與討論](https://github.com/laitim2001/ai-semantic-kernel-framework/discussions)

---

**最後更新**: 2025-10-29
**版本**: 1.0.0 (文檔規劃階段)
**狀態**: 📋 文檔完成，準備進入 Stage 3.3-3.5

🤖 Generated with [Claude Code](https://claude.com/claude-code)
