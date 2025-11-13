# 項目文件清單與用途說明

**生成時間**: 2025-11-12
**目的**: 提供所有規劃和執行文件的快速參考指南

---

## 📁 /docs - 項目規劃文件層 (Reference Layer)

### 根目錄文件

| 文件 | 用途 | 使用時機 |
|------|------|---------|
| `README.md` | 項目文檔總覽和導航 | 開始任何工作前閱讀 |
| `brief.md` | 完整項目簡介 (合併版本) | 全面了解項目背景 |
| `brief-1-overview.md` | 項目背景、問題陳述、解決方案 | 理解項目起源和目標 |
| `brief-2-requirements.md` | 目標用戶、需求、成功指標 | 了解業務需求和驗收標準 |
| `brief-3-technical.md` | MVP範圍、技術規格、交付標準 | 技術決策和範圍界定 |
| `brief-4-constraints.md` | 預算、時程、資源、技術限制 | 項目約束條件參考 |
| `brief-README.md` | Brief文件使用指南 | 如何使用Brief文件 |
| `BMAD-STAGE-ANALYSIS.md` | BMad Method階段分析 | 了解開發方法論 |
| `COMPLETE-DEVELOPMENT-PLAN.md` | 完整開發計劃 (18個月) | 長期規劃參考 |
| `PROJECT-INITIALIZATION-DECISION.md` | 項目初始化決策 (Monorepo, 技術棧) | 理解技術選型理由 |
| `DOCUMENT-SHARDING-REPORT.md` | 文檔切分報告 | 了解文檔組織邏輯 |
| `DOCUMENT-USAGE-STRATEGY.md` | 文檔使用策略 | 如何使用不同類型文檔 |
| `analysis-microsoft-agent-framework-impact.md` | Microsoft Agent Framework戰略分析 | 理解框架選型決策 |

### /architecture - 架構設計文件

| 文件 | 用途 | 使用時機 |
|------|------|---------|
| `Architecture-Design-Document.md` | 系統架構設計文檔 (核心) | 架構決策、組件設計 |
| `C4-architecture-diagrams.md` | C4架構圖 (Context, Container, Component) | 理解系統結構 |
| `database-schema.md` | 數據庫Schema設計 | 數據庫開發參考 |
| `performance-scalability-strategy.md` | 性能和擴展策略 | 性能優化決策 |
| `ADR-006-agent-state-management.md` | Agent狀態管理決策 | Agent狀態設計 |
| `ADR-007-multi-agent-communication.md` | Multi-Agent通訊機制 | 多Agent協作設計 |
| `ADR-008-code-interpreter-execution-model.md` | Code Interpreter執行模型 | Code Interpreter安全設計 |
| `ADR-011-framework-migration-strategy.md` | 框架遷移策略 (SK → MAF) | 框架升級路徑 |
| `ADR-012-workflow-editor-technology.md` | Workflow Editor技術選型 | Workflow Editor開發 |
| `components/README.md` | 組件文檔索引 | 查找特定組件文檔 |

### /user-stories - 用戶故事和需求

| 文件 | 用途 | 使用時機 |
|------|------|---------|
| `README.md` | User Stories總覽 | 開始功能開發前 |
| `user-stories.md` | User Stories匯總 | 快速查找Story |
| `mvp-planning.md` | MVP規劃 | MVP範圍確認 |
| `implementation-strategy.md` | 實施策略 | 開發優先級參考 |

#### /user-stories/modules - 模組化需求

| 文件 | 模組 | 用途 |
|------|------|------|
| `module-01-agent-creation.md` | Agent創建與管理 | US 1.1-1.4 |
| `module-02-plugin-system.md` | Plugin系統 | US 2.1-2.3 |
| `module-03-code-interpreter.md` | Code Interpreter | US 3.1-3.2 |
| `module-04-multi-agent.md` | Multi-Agent協作 | US 4.1-4.3 |
| `module-05-agent-memory.md` | Agent記憶 | US 5.1-5.3 |
| `module-06-chat-interface.md` | Chat介面 | US 6.1-6.2 |
| `module-07-persona-framework.md` | Persona框架 | US 7.1-7.2 |
| `module-08-structured-data.md` | 結構化數據 (Text-to-SQL) | US 8.1-8.2 |
| `module-09-enterprise-features.md` | 企業功能 | US 9.1-9.3 |
| `module-10-monitoring.md` | 監控與分析 | US 10.1-10.3 |

#### /user-stories/epics - Epic詳細規格

| 文件 | Epic | 用途 |
|------|------|------|
| `epic-05.1-conversation-memory.md` | 對話記憶 | US 5.1詳細需求 |
| `epic-05.2-knowledge-rag.md` | Knowledge RAG | US 5.2詳細需求 |
| `epic-09.1-authentication.md` | 身份驗證 | US 9.1詳細需求 |
| `epic-09.2-multi-tenant.md` | 多租戶 | US 9.2詳細需求 |
| `epic-09.3-i18n-api-metering.md` | 國際化與API計量 | US 9.3詳細需求 |
| `epic-10.1-monitoring-dashboard.md` | 監控儀表板 | US 10.1詳細需求 |
| `epic-10.2-quality-monitoring.md` | 質量監控 | US 10.2詳細需求 |
| `epic-10.3-user-analytics.md` | 用戶分析 | US 10.3詳細需求 |

#### /user-stories/sprints - Sprint分配

| 文件 | 用途 |
|------|------|
| `sprint-allocation.md` | US到Sprint的分配 |
| `sprint-backlog.md` | Sprint Backlog管理 |

#### /user-stories/templates - 模板

| 文件 | 用途 |
|------|------|
| `user-story-template.md` | User Story標準模板 |
| `definition-of-done.md` | 完成定義 (DoD) |

### /ux-design - UX設計文件

#### /ux-design/design-system - 設計系統

| 文件 | 用途 |
|------|------|
| `README.md` | 設計系統總覽 |
| `colors.md` / `color-palette.md` | 色彩系統 |
| `typography.md` | 字體系統 |
| `design-tokens.md` | 設計Token |
| `component-library.md` | 組件庫 |
| `accessibility-guidelines.md` | 無障礙指南 |

#### /ux-design/wireframes/low-fidelity - 低保真線框圖

| 文件 | 頁面 | 用途 |
|------|------|------|
| `01-dashboard.md` | 儀表板 | Dashboard UI設計 |
| `02-agent-list.md` | Agent列表 | Agent管理UI |
| `03-agent-create.md` | 創建Agent | Agent創建流程 |
| `04-agent-detail.md` | Agent詳情 | Agent詳情頁 |
| `05-conversation.md` | 對話介面 | Chat UI設計 (1890行詳細設計) |
| `06-knowledge-base.md` | 知識庫 | Knowledge管理UI |
| `07-code-interpreter.md` | Code Interpreter | Code執行UI |
| `08-text-to-sql.md` | Text-to-SQL | SQL生成UI |
| `09-workflow-editor-v2-*.md` | Workflow Editor | 工作流編輯器 (7部分) |
| `10-persona-builder.md` | Persona Builder | Persona配置UI |
| `11-settings.md` | 設置 | 設置頁面 |
| `12-monitoring.md` | 監控 | 監控儀表板 |

#### /ux-design/information-architecture - 信息架構

| 文件 | 用途 |
|------|------|
| `sitemap.md` | 網站地圖 |
| `navigation-structure.md` | 導航結構 |
| `content-hierarchy.md` | 內容層級 |

#### /ux-design/user-research - 用戶研究

| 文件 | 用途 |
|------|------|
| `personas.md` | 用戶畫像 |
| `user-journey-maps.md` | 用戶旅程圖 |

### /technical-implementation - 技術實施文件

#### 根目錄

| 文件 | 用途 |
|------|------|
| `README.md` | 技術實施總覽 |
| `IMPLEMENTATION-STRUCTURE.md` | 實施結構說明 |
| `TID-INDEX.md` | 技術實施文檔索引 |
| `TECH-STACK-ANALYSIS.md` | 技術棧分析 |
| `01-SYSTEM-ARCHITECTURE.md` | 系統架構 |
| `02-MICROSERVICES-DESIGN.md` | 微服務設計 |
| `03-DATA-FLOW.md` | 數據流設計 |
| `04-API-SPECIFICATION.md` | API規格 |
| `06-DATABASE-SCHEMA.md` | 數據庫Schema |
| `07-ENTITY-RELATIONSHIPS.md` | 實體關係圖 |
| `08-SECURITY-ARCHITECTURE.md` | 安全架構 |
| `09-AUTHENTICATION-AUTHORIZATION.md` | 認證授權 |
| `11-DEPLOYMENT-ARCHITECTURE.md` | 部署架構 |
| `12-DOCKER-COMPOSE.md` | Docker Compose配置 |
| `13-KUBERNETES.md` | Kubernetes配置 |
| `14-CICD-PIPELINE.md` | CI/CD流程 |
| `15-MONITORING.md` | 監控 |
| `16-LOGGING.md` | 日誌 |
| `17-ALERTING.md` | 告警 |
| `18-DISASTER-RECOVERY.md` | 災難恢復 |
| `19-BACKUP-RESTORE.md` | 備份恢復 |
| `20-COST-ESTIMATION.md` | 成本估算 |

#### /technical-implementation/1-poc-validation - POC驗證

| 文件 | POC | 用途 |
|------|-----|------|
| `01-semantic-kernel-agents.md` | SK Agents | POC 1規格 |
| `01-semantic-kernel-agents-EXECUTION-GUIDE.md` | SK Agents | POC 1執行指南 |
| `02-persona-builder.md` | Persona Builder | POC 2規格 |
| `02-persona-builder-EXECUTION-GUIDE.md` | Persona Builder | POC 2執行指南 |
| `03-code-interpreter-sandbox.md` | Code Interpreter | POC 3規格 |
| `03-code-interpreter-EXECUTION-GUIDE.md` | Code Interpreter | POC 3執行指南 |
| `04-text-to-sql-engine.md` | Text-to-SQL | POC 4規格 |
| `04-text-to-sql-EXECUTION-GUIDE.md` | Text-to-SQL | POC 4執行指南 |
| `05-knowledge-rag-accuracy.md` | Knowledge RAG | POC 5規格 |
| `05-knowledge-rag-EXECUTION-GUIDE.md` | Knowledge RAG | POC 5執行指南 |
| `06-vueflow-crdt-collaboration.md` | VueFlow CRDT | POC 6規格 |
| `06-vueflow-crdt-EXECUTION-GUIDE.md` | VueFlow CRDT | POC 6執行指南 |
| `poc-validation-report.md` | POC驗證報告 | 所有POC結果匯總 |

#### /technical-implementation/2-dev-environment - 開發環境

| 文件 | 用途 |
|------|------|
| `README.md` | 開發環境總覽 |
| `setup-guide-windows.md` | Windows環境設置 |
| `setup-guide-macos.md` | macOS環境設置 |
| `setup-guide-linux.md` | Linux環境設置 |
| `docker-configuration.md` | Docker配置 |
| `environment-variables.md` | 環境變量配置 |

#### /technical-implementation/3-project-structure - 項目結構

| 文件 | 用途 |
|------|------|
| `solution-architecture.md` | Solution架構 |
| `backend-project-structure.md` | 後端項目結構 |
| `frontend-react-structure.md` | React項目結構 |
| `frontend-vue-structure.md` | Vue項目結構 |
| `shared-libraries.md` | 共享庫 |
| `folder-naming-conventions.md` | 命名規範 |
| `dependency-management.md` | 依賴管理 |

#### /technical-implementation/4-coding-standards - 編碼標準

| 文件 | 語言/框架 | 用途 |
|------|----------|------|
| `csharp-coding-standards.md` | C# | C#編碼規範 |
| `typescript-coding-standards.md` | TypeScript | TS編碼規範 |
| `react-coding-standards.md` | React | React編碼規範 |
| `vue-coding-standards.md` | Vue | Vue編碼規範 |

#### /technical-implementation/5-api-design - API設計

| 文件 | 用途 |
|------|------|
| `restful-api-standards.md` | RESTful API標準 |
| `api-versioning.md` | API版本控制 |
| `api-security.md` | API安全 |
| `api-documentation.md` | API文檔 |
| `error-handling.md` | 錯誤處理 |

#### /technical-implementation/6-database-standards - 數據庫標準

| 文件 | 用途 |
|------|------|
| `database-design-principles.md` | 數據庫設計原則 |
| `entity-framework-core-configuration.md` | EF Core配置 |
| `database-migration-strategy.md` | 數據庫遷移策略 |
| `query-performance-optimization.md` | 查詢性能優化 |

#### /technical-implementation/7-testing-strategy - 測試策略

| 文件 | 用途 |
|------|------|
| `unit-testing-standards.md` | 單元測試標準 |
| `integration-testing-standards.md` | 集成測試標準 |
| `end-to-end-testing-standards.md` | E2E測試標準 |
| `test-coverage-strategy.md` | 測試覆蓋率策略 |
| `test-automation-cicd.md` | CI/CD測試自動化 |

#### /technical-implementation/8-deployment-architecture - 部署架構

| 文件 | 用途 |
|------|------|
| `docker-containerization.md` | Docker容器化 |
| `kubernetes-deployment.md` | Kubernetes部署 |
| `azure-infrastructure-setup.md` | Azure基礎設施 |
| `environment-configuration.md` | 環境配置 |
| `cicd-pipeline-github-actions.md` | GitHub Actions CI/CD |

#### /technical-implementation/9-security-standards - 安全標準

| 文件 | 用途 |
|------|------|
| `authentication-implementation.md` | 身份驗證實現 |
| `authorization-rbac.md` | 授權與RBAC |
| `code-interpreter-security.md` | Code Interpreter安全 |
| `data-encryption-standards.md` | 數據加密標準 |
| `security-testing-automation.md` | 安全測試自動化 |

#### /technical-implementation/10-monitoring-operations - 監控運維

| 文件 | 用途 |
|------|------|
| `performance-monitoring.md` | 性能監控 |
| `application-logging-standards.md` | 應用日誌標準 |
| `prometheus-grafana-setup.md` | Prometheus + Grafana |
| `health-checks-readiness.md` | 健康檢查 |
| `alerting-notification-strategy.md` | 告警通知策略 |

#### /technical-implementation/11-disaster-recovery - 災難恢復

| 文件 | 用途 |
|------|------|
| `high-availability-design.md` | 高可用設計 |
| `backup-restore-strategy.md` | 備份恢復策略 |
| `database-recovery-procedures.md` | 數據庫恢復流程 |
| `failover-procedures.md` | 故障轉移流程 |
| `disaster-recovery-testing.md` | 災難恢復測試 |

#### /technical-implementation/12-cost-estimation - 成本估算

| 文件 | 用途 |
|------|------|
| `azure-service-pricing.md` | Azure服務定價 |
| `cost-optimization-strategies.md` | 成本優化策略 |
| `cost-monitoring-alerts.md` | 成本監控告警 |
| `resource-scaling-economics.md` | 資源擴展經濟學 |
| `financial-forecasting.md` | 財務預測 |

### /project-management - 項目管理

| 文件 | 用途 |
|------|------|
| `Project-Management-Plan.md` | 項目管理計劃 |

---

## 📁 /claudedocs - 執行層文件 (Execution Layer)

### 根目錄文件

| 文件 | 用途 | 使用時機 |
|------|------|---------|
| `README.md` | Claudedocs使用指南 | AI助手開始工作前必讀 |
| `PROJECT-STATUS-REPORT-v8.0.md` | 項目狀態報告 (最新版) | 了解當前進度和狀態 |
| `UI-IMPROVEMENT-PLAN.md` | UI改進計劃 | Chat UI優化參考 |

### /1-planning - 規劃文件

| 文件 | 用途 |
|------|------|
| `MASTER-DEVELOPMENT-SCHEDULE.md` | 主開發時程表 (18 Sprints) |

### /2-sprints - Sprint執行文件

| Sprint | 文件 | 狀態 |
|--------|------|------|
| Sprint 1 | `sprint-1/SPRINT-1-OVERVIEW.md` | ✅ 完成 |
| Sprint 2 | `sprint-2/SPRINT-2-OVERVIEW.md` | 🔄 進行中 (65%) |
| Sprint 3-18 | `sprint-X/SPRINT-X-OVERVIEW.md` | 📋 待開始 |

**當前問題**: 每個Sprint只有OVERVIEW文件,缺乏詳細的執行指引

### /3-progress - 進度追蹤

| 文件 | 用途 |
|------|------|
| `FEATURE-COMPLETION-MATRIX.md` | 功能完成度矩陣 |
| `USER-STORY-STATUS.md` | User Story狀態追蹤 |

### /4-changes - 變更管理

| 文件 | 用途 |
|------|------|
| `CHANGE-LOG.md` | 項目變更日誌 |
| `SCOPE-CHANGE-REQUEST-TEMPLATE.md` | 範圍變更請求模板 |

### /5-processes - 流程文件

| 文件 | 用途 |
|------|------|
| `DOC-UPDATE-CHECKLIST.md` | 文檔更新檢查清單 |

### /5-status - 狀態報告

| 文件 | 用途 |
|------|------|
| `PROJECT-STATUS-REPORT.md` | 項目狀態報告 |

### /6-ai-assistant - AI助手指引

| 文件 | 用途 |
|------|------|
| `AI-ASSISTANT-INSTRUCTIONS.md` | AI助手使用說明 |

### /6-implementation-plans - 實施計劃

| 文件 | User Story | 用途 |
|------|-----------|------|
| `US-6.1-CHAT-INTERFACE-IMPLEMENTATION-PLAN.md` | US 6.1 | Chat介面實施計劃 |
| `US-6.1-UI-DESIGN-SPECIFICATION.md` | US 6.1 | Chat UI設計規格 |

### /7-archive - 歸檔文件

存放已完成或過時的文檔:
- Sprint 0完成報告
- Sprint 1-2會議記錄
- 舊版PO驗證報告
- 文檔重組計劃
- 各種分析報告

### /8-prompts - AI Prompt模板

| 文件 | 用途 |
|------|------|
| `README.md` | Prompt使用指南 |
| `PROMPT-01-PROJECT-ONBOARDING.md` | 項目入門Prompt |
| `PROMPT-02-NEW-FEATURE-PREP.md` | 新功能準備Prompt |
| `PROMPT-03-BUG-FIX-PREP.md` | Bug修復準備Prompt |
| `PROMPT-04-FEATURE-DEVELOPMENT.md` | 功能開發Prompt |
| `PROMPT-05-TESTING-PHASE.md` | 測試階段Prompt |
| `PROMPT-06-PROGRESS-SAVE.md` | 進度保存Prompt |
| `PROMPT-07-ARCHITECTURE-REVIEW.md` | 架構審查Prompt |
| `PROMPT-08-CODE-REVIEW.md` | 代碼審查Prompt |
| `PROMPT-09-SESSION-END.md` | 會話結束Prompt |

---

## 🔍 使用建議

### 開始新Sprint時:
1. 閱讀 `/claudedocs/README.md`
2. 查看 `/claudedocs/1-planning/MASTER-DEVELOPMENT-SCHEDULE.md`
3. 進入對應Sprint文件夾 `/claudedocs/2-sprints/sprint-X/`
4. **問題**: 目前只有OVERVIEW,缺乏詳細執行指引 ⚠️

### 開發新功能時:
1. 查找對應的Module文檔 `/docs/user-stories/modules/module-XX.md`
2. 查找對應的Epic文檔 `/docs/user-stories/epics/epic-XX.md`
3. 查看UI設計 `/docs/ux-design/wireframes/low-fidelity/`
4. 參考技術實施文檔 `/docs/technical-implementation/`
5. **問題**: AI助手開發時沒有自動參考這些文檔 ⚠️

### 架構決策時:
1. 查看 `/docs/architecture/Architecture-Design-Document.md`
2. 查看相關ADR `/docs/architecture/ADR-*.md`
3. 查看C4架構圖 `/docs/architecture/C4-architecture-diagrams.md`

---

## ⚠️ 當前問題總結

### 問題1: 規劃文件太多太詳細
- `/docs` 文件夾有 **180+ 個markdown文件**
- 內容非常詳細 (例如 `05-conversation.md` 有 1890 行)
- 但AI助手開發時**沒有有效引用**這些文檔

### 問題2: Sprint文件太簡單
- 每個Sprint只有 `SPRINT-X-OVERVIEW.md`
- 缺乏詳細的執行計劃、檢查清單、開發日誌
- AI助手缺乏明確的執行指引

### 問題3: 設計與實際脫節
- 設計文檔規劃了3欄布局、Function Calling可視化等高級功能
- 實際只實現了基礎2欄布局 (~15%)
- 缺乏MVP範圍與完整設計的明確區分

---

**下一步**: 查看優化方案 → `SPRINT-EXECUTION-FRAMEWORK.md`
