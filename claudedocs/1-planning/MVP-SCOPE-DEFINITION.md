# MVP 範圍定義文檔

**文檔版本**: 2.0
**建立日期**: 2025-11-12
**最後更新**: 2025-11-12
**負責人**: Project Manager
**狀態**: ✅ 已完成

---

## 📋 文檔目的

本文檔定義 **Semantic Kernel Agentic Framework** 的最小可行產品（MVP）範圍，明確界定 Phase 1（8 個月）需交付的核心功能、技術邊界、驗收標準及排除項目，確保團隊聚焦於最高價值的功能開發。

---

## 🎯 MVP 目標與願景

### 核心目標

基於 [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md)，MVP 的核心目標為：

1. **快速驗證市場需求** - 8 個月內交付可用系統，驗證企業級 AI Agent 平台的價值主張
2. **建立技術基礎** - 奠定穩固的 Clean Architecture + DDD + CQRS 架構基礎（參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md)）
3. **服務核心用戶** - 滿足三大核心用戶群（IT Developer, Business Analyst, Enterprise Admin）的關鍵需求（參考 [docs/ux-design/user-research/personas.md](../../docs/ux-design/user-research/personas.md)）
4. **差異化能力** - 實現 Persona Framework 和 Multi-Agent Workflow 兩大核心競爭優勢

### 產品願景

打造一個企業級 AI Agent 平台，讓開發者、業務分析師和系統管理員能夠：
- **5 分鐘**創建並部署一個 AI Agent（參考 [docs/ux-design/user-research/user-journey-maps.md](../../docs/ux-design/user-research/user-journey-maps.md) - Journey 1）
- **無代碼**配置 Agent 個性和對話風格（Persona Framework）
- **可視化**設計複雜的 Multi-Agent 工作流（參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)）
- **企業級**安全、監控和管理能力

---

## ✅ MVP 包含功能（In Scope）

### 1. Agent 管理核心（US 1.x）

基於 [docs/user-stories/01-agent-management/README.md](../../docs/user-stories/01-agent-management/README.md)，MVP 包含完整的 Agent CRUD 和生命週期管理：

#### US 1.1: Agent CRUD API ✅
- **完整 RESTful API** - 符合 [docs/api/API-Design-Guidelines.md](../../docs/api/API-Design-Guidelines.md) 規範
- **CQRS 實作** - 基於 [docs/architecture/layered-architecture/Application-Layer.md](../../docs/architecture/layered-architecture/Application-Layer.md) 的 MediatR 模式
- **Repository Pattern** - 實現 [docs/architecture/layered-architecture/Infrastructure-Layer.md](../../docs/architecture/layered-architecture/Infrastructure-Layer.md) 的資料存取層
- **驗收標準**:
  - ✅ 支援 Create, Read, Update, Delete, List 操作
  - ✅ 請求驗證（Fluent Validation）
  - ✅ 錯誤處理（Result Pattern）
  - ✅ Swagger 文檔完整

#### US 1.2: Agent 搜尋與篩選 ✅
- **多條件查詢** - Name, Description, Status, Model Type
- **分頁支援** - Page size, page number, total count
- **排序功能** - CreatedAt, UpdatedAt, Name
- **驗收標準**: 參考 [docs/user-stories/01-agent-management/US-1.2-Agent-Search-Filter.md](../../docs/user-stories/01-agent-management/US-1.2-Agent-Search-Filter.md)

#### US 1.3: Agent 狀態管理 ✅
- **五階段生命週期** - Draft → Active → Paused → Archived → Deleted
- **狀態轉換邏輯** - 符合 Domain Layer 業務規則
- **批量操作** - 批量啟用、暫停、封存、刪除
- **驗收標準**: 參考 [docs/user-stories/01-agent-management/US-1.3-Agent-Status-Management.md](../../docs/user-stories/01-agent-management/US-1.3-Agent-Status-Management.md)

#### US 1.4: Agent 執行引擎 ✅
- **Semantic Kernel 整合** - 基於 [docs/technical-implementation/semantic-kernel/SK-Integration-Design.md](../../docs/technical-implementation/semantic-kernel/SK-Integration-Design.md)
- **多模型支援** - Azure OpenAI GPT-4, GPT-3.5-turbo
- **Plugin 載入機制** - 動態載入 Native 和 Semantic Functions
- **執行管理** - 超時控制（30 秒）、錯誤處理、日誌記錄

### 2. Plugin 系統（US 2.x - 70% 完成）

基於 [docs/user-stories/02-plugin-system/README.md](../../docs/user-stories/02-plugin-system/README.md)，MVP 包含 Plugin 的基礎架構和版本管理：

#### US 2.1: Plugin 系統基礎架構 ✅
- **Plugin 生命週期管理** - Create, Activate, Deactivate, Delete
- **Plugin 類型支援** - Native Plugin (C#), Semantic Plugin (Prompt)
- **安全隔離** - Plugin 執行在獨立 AppDomain
- **驗收標準**: 參考 [docs/user-stories/02-plugin-system/US-2.1-Plugin-System-Foundation.md](../../docs/user-stories/02-plugin-system/US-2.1-Plugin-System-Foundation.md)

#### US 2.2: Plugin 版本管理 🔄 (70% 完成)
- **版本控制** - Semantic Versioning (SemVer 2.0)
- **版本歷史追蹤** - PluginVersion, PluginVersionHistory 實體
- **版本回溯** - 回退到先前版本
- **當前狀態**: 實體和 Repository 已完成，API 端點開發中

#### US 2.3: Plugin 熱重載機制 🔄 (30% 完成)
- **動態載入** - IPluginLoader 介面實作
- **熱部署** - 不停機更新 Plugin
- **依賴管理** - NuGet 套件動態解析
- **當前狀態**: 設計階段，部分實作完成

### 3. 對話管理（US 3.x - MVP 基礎版）

基於 [docs/user-stories/03-conversation-management/README.md](../../docs/user-stories/03-conversation-management/README.md)，MVP 包含基本對話功能：

#### US 3.1: 對話 CRUD API（MVP 範圍）
- **基本對話管理** - Create, Read, Update, Delete, List
- **多模態消息支援** - Text, Image, Code, Chart（UI 展示層面）
- **對話歷史** - 查詢和分頁
- **簡化版 Chat UI** - 參考 [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)

**MVP 限制**:
- ❌ 不包含 US 3.2（對話分支管理）- 留待 Phase 2
- ❌ 不包含 US 3.3（對話分享和導出進階功能）- 僅支援基本導出

### 4. Persona Framework（核心差異化功能）

基於 [docs/user-stories/08-persona-framework/README.md](../../docs/user-stories/08-persona-framework/README.md)，MVP 包含完整的 Persona 配置能力：

#### US 8.1: Persona Designer（完整實作）
- **6 步引導式向導** - 參考 [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md)
  1. 角色定義（名稱、職業、背景）
  2. 個性特質（5 維度滑桿：正式度、友善度、簡潔度、創造力、謹慎度）
  3. 對話風格（語氣、用詞、句式範例）
  4. Few-Shot 範例（3-5 個對話範例）
  5. 知識領域和限制
  6. 預覽和測試
- **10 種預設 Persona** - IT 專家、業務分析師、客服代表等
- **一致性評分** - 檢測 Persona 配置的一致性
- **Prompt 生成** - 自動生成結構化 System Prompt

#### US 8.2: Persona 應用和測試
- **Persona 套用到 Agent** - 將 Persona 配置應用到 Agent 實例
- **A/B 測試** - 比較不同 Persona 的表現
- **實時預覽** - 即時查看 Persona 效果

### 5. Knowledge Base（US 4.x - MVP 基礎版）

基於 [docs/user-stories/04-knowledge-base/README.md](../../docs/user-stories/04-knowledge-base/README.md) 和 [docs/technical-implementation/rag-knowledge-base/RAG-Design.md](../../docs/technical-implementation/rag-knowledge-base/RAG-Design.md)：

#### US 4.1: 文檔上傳和索引（簡化版）
- **支援格式** - PDF, DOCX, TXT, MD（僅限文字型檔案）
- **文檔解析** - 基於 [docs/technical-implementation/rag-knowledge-base/Document-Processing.md](../../docs/technical-implementation/rag-knowledge-base/Document-Processing.md)
- **向量索引** - Qdrant 1.7.4 整合
- **批量上傳** - 最多 10 個文件 / 次

**MVP 限制**:
- ❌ 不支援 Excel, PPT, 圖片 OCR - 留待 Phase 2
- ❌ 不支援進階分塊策略 - 僅使用固定大小分塊（512 tokens）

#### US 4.2: RAG 檢索（基礎版）
- **語義檢索** - 基於 OpenAI Embeddings（text-embedding-ada-002）
- **檢索策略** - 僅支援 Hybrid Search（向量 + 關鍵字）
- **結果重排** - 基於相似度分數

**MVP 限制**:
- ❌ 不包含 Reranking 模型 - 留待 Phase 2
- ❌ 不包含 HyDE（假設性文檔嵌入）- 留待 Phase 2

### 6. Code Interpreter（US 5.x）

基於 [docs/user-stories/05-code-interpreter/README.md](../../docs/user-stories/05-code-interpreter/README.md) 和 [docs/technical-implementation/code-interpreter/Code-Interpreter-Design.md](../../docs/technical-implementation/code-interpreter/Code-Interpreter-Design.md)：

#### US 5.1: Python 代碼執行（完整實作）
- **Docker 容器隔離** - 基於 [docs/technical-implementation/code-interpreter/Container-Management.md](../../docs/technical-implementation/code-interpreter/Container-Management.md)
- **容器池管理** - 動態生命週期管理（ADR-008）
- **安全沙箱** - 4 層安全防護（參考 [docs/security/Code-Execution-Security.md](../../docs/security/Code-Execution-Security.md)）
- **執行超時** - 30 秒強制超時
- **資源限制** - CPU 1 core, Memory 512MB

#### US 5.2: 結果可視化
- **圖表展示** - Matplotlib, Seaborn 生成的圖表
- **表格展示** - Pandas DataFrame 渲染
- **多種導出格式** - PNG, CSV, JSON

**MVP 限制**:
- ❌ 不支援 Jupyter Notebook 完整環境 - 留待 Phase 2
- ❌ 不支援 R, Julia 等其他語言 - 僅支援 Python

### 7. Text-to-SQL（US 6.x - 基礎版）

基於 [docs/user-stories/06-text-to-sql/README.md](../../docs/user-stories/06-text-to-sql/README.md)：

#### US 6.1: 自然語言轉 SQL（簡化版）
- **自然語言輸入** - 中英文支援
- **SQL 生成** - 基於 GPT-4 Few-Shot Prompting
- **SQL 預覽** - 執行前確認
- **查詢執行** - PostgreSQL 支援

**MVP 限制**:
- ❌ 僅支援 SELECT 語句 - 不支援 INSERT, UPDATE, DELETE
- ❌ 不支援複雜 JOIN（最多 2 張表）- 留待 Phase 2
- ❌ 不支援多資料庫（僅 PostgreSQL）- 留待 Phase 2

#### US 6.2: 結果展示和導出
- **表格展示** - 分頁、排序、篩選
- **圖表生成** - 自動識別數據類型並推薦圖表
- **導出功能** - CSV, Excel

### 8. Multi-Agent Workflow（US 7.x - MVP 簡化版）

基於 [docs/user-stories/07-multi-agent-workflow/README.md](../../docs/user-stories/07-multi-agent-workflow/README.md) 和 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)：

#### US 7.1: Workflow Editor（簡化版）
- **可視化編輯器** - VueFlow 基礎實作
- **10 種節點類型**（MVP）:
  1. Agent 節點
  2. Input 節點
  3. Output 節點
  4. Decision 節點（If/Else）
  5. Loop 節點
  6. Transform 節點
  7. Merge 節點
  8. Split 節點
  9. Delay 節點
  10. HTTP 節點
- **基本連接線** - 直線連接，無 Bezier 曲線
- **節點配置** - 內聯配置（參考 Workflow Editor V2 設計）

**MVP 限制**:
- ❌ 完整 28 種節點類型 - 僅實作 10 種核心節點
- ❌ 高級連接線樣式（Bezier, Smooth Step）- 留待 Phase 2
- ❌ 多用戶協作（CRDT）- 留待 Phase 2
- ❌ 版本控制和回溯 - 留待 Phase 2

#### US 7.2: Workflow 執行引擎（基礎版）
- **同步執行** - 順序執行節點
- **錯誤處理** - 節點執行失敗時停止工作流
- **執行日誌** - 記錄每個節點的輸入/輸出

**MVP 限制**:
- ❌ 異步執行和並行執行 - 留待 Phase 2
- ❌ 斷點續傳 - 留待 Phase 2
- ❌ 複雜錯誤處理（重試、Fallback）- 留待 Phase 2

### 9. 前端應用（12 個核心頁面）

基於 [docs/ux-design/README.md](../../docs/ux-design/README.md)，MVP 包含完整的 12 個核心頁面：

1. ✅ **Dashboard** - [docs/ux-design/wireframes/low-fidelity/01-dashboard.md](../../docs/ux-design/wireframes/low-fidelity/01-dashboard.md)
2. ✅ **Agent List** - [docs/ux-design/wireframes/low-fidelity/02-agent-list.md](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)
3. ✅ **Agent Creation** - [docs/ux-design/wireframes/low-fidelity/03-agent-create.md](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md)
4. ✅ **Agent Detail** - [docs/ux-design/wireframes/low-fidelity/04-agent-detail.md](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)
5. ✅ **Chat Interface** - [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)
6. ✅ **Knowledge Base** - [docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md](../../docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md)
7. ✅ **Code Interpreter** - [docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md](../../docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md)
8. ✅ **Text-to-SQL** - [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)
9. ✅ **Workflow Editor** - [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)（簡化版）
10. ✅ **Persona Builder** - [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md)
11. ✅ **Settings** - [docs/ux-design/wireframes/low-fidelity/11-settings.md](../../docs/ux-design/wireframes/low-fidelity/11-settings.md)
12. ✅ **Monitoring** - [docs/ux-design/wireframes/low-fidelity/12-monitoring.md](../../docs/ux-design/wireframes/low-fidelity/12-monitoring.md)

**Design System**: 完整實作基於 [docs/ux-design/design-system/README.md](../../docs/ux-design/design-system/README.md)
- Material-UI (MUI) v5
- Design Tokens（顏色、字體、間距、陰影、動畫）
- WCAG 2.1 AA 無障礙標準

### 10. 監控與管理（基礎版）

#### 系統監控（簡化版）
- **基本指標** - Agent 執行次數、成功率、平均響應時間
- **日誌記錄** - Serilog 結構化日誌
- **健康檢查** - API Health Check 端點

**MVP 限制**:
- ❌ 不包含 Application Insights - 留待 Phase 2
- ❌ 不包含分散式追蹤 - 留待 Phase 2
- ❌ 不包含告警系統 - 留待 Phase 2

#### 使用者和權限（基礎版）
- **RBAC** - 4 種角色（Admin, Developer, Analyst, Viewer）
- **基本認證** - JWT Token（參考 [docs/security/Authentication-Authorization.md](../../docs/security/Authentication-Authorization.md)）

**MVP 限制**:
- ❌ 不包含 OAuth 2.0 / OIDC - 留待 Phase 2
- ❌ 不包含細粒度權限控制 - 僅角色級別

---

## ❌ MVP 排除功能（Out of Scope）

### 1. 進階 Plugin 功能（留待 Phase 2）
- **Plugin Marketplace** - Plugin 分享和下載平台
- **Plugin 評分和評論** - 社群功能
- **Plugin 相依性管理** - 複雜依賴解析
- **Plugin 沙箱高級隔離** - Kubernetes-based 隔離

### 2. 進階對話管理（留待 Phase 2）
- **US 3.2: 對話分支管理** - Tree-based 對話歷史
- **對話模板高級功能** - 條件渲染、變數替換
- **對話分析** - 情感分析、主題檢測

### 3. 進階 RAG 功能（留待 Phase 2）
- **Reranking 模型** - Cross-Encoder 重排
- **HyDE（假設性文檔嵌入）** - 高級檢索策略
- **多模態 RAG** - 圖片、影片內容檢索
- **進階文檔解析** - Excel, PPT, 圖片 OCR

### 4. Multi-Agent 高級功能（留待 Phase 2）
- **完整 28 種節點類型** - MVP 僅實作 10 種
- **多用戶協作（CRDT）** - Yjs 整合
- **版本控制和回溯** - Git-like 工作流版本管理
- **異步執行和並行執行** - 複雜執行策略
- **斷點續傳** - 工作流中斷後恢復

### 5. Kubernetes 部署（Phase 2）
- **Service Mesh** - Istio 整合
- **自動伸縮** - HPA（Horizontal Pod Autoscaler）
- **消息隊列** - RabbitMQ / Azure Service Bus
- **分散式追蹤** - Jaeger / Zipkin
- **告警系統** - Prometheus + Alertmanager

### 6. 企業級功能（Phase 2）
- **SSO 整合** - SAML 2.0, OAuth 2.0, OIDC
- **審計日誌** - 完整的操作審計追蹤
- **數據加密** - 資料庫欄位級加密
- **災難恢復** - 多區域備份和故障轉移（參考 [docs/deployment/Disaster-Recovery.md](../../docs/deployment/Disaster-Recovery.md)）
- **合規性** - GDPR, SOC 2, ISO 27001

### 7. 進階分析和優化（Phase 2）
- **A/B 測試平台** - 系統化實驗管理
- **成本優化** - Token 使用分析和優化建議
- **性能優化** - 高級快取策略、CDN 整合
- **使用者行為分析** - 完整的使用者旅程追蹤

---

## 🏗️ 技術範圍與邊界

### MVP 技術棧（完整清單）

**後端**: 參考 [docs/technical-implementation/backend/README.md](../../docs/technical-implementation/backend/README.md)
- ✅ .NET 9（C# 13）
- ✅ ASP.NET Core Web API
- ✅ Entity Framework Core 9
- ✅ MediatR（CQRS）
- ✅ FluentValidation
- ✅ Semantic Kernel 1.0+

**前端**: 參考 [docs/technical-implementation/frontend/README.md](../../docs/technical-implementation/frontend/README.md)
- ✅ React 18
- ✅ TypeScript 5
- ✅ Material-UI (MUI) v5
- ✅ Zustand（狀態管理）
- ✅ React Router v6
- ✅ Vite（建置工具）
- ✅ VueFlow（工作流編輯器，簡化版）

**基礎設施**: 參考 [docs/deployment/Docker-Compose-Setup.md](../../docs/deployment/Docker-Compose-Setup.md)
- ✅ PostgreSQL 16（主資料庫）
- ✅ Redis 7（快取和 Session）
- ✅ Qdrant 1.7.4（向量資料庫）
- ✅ Docker & Docker Compose（開發環境）
- ❌ **Kubernetes** - 留待 Phase 2

**AI 模型**: 參考 [docs/technical-implementation/semantic-kernel/Model-Configuration.md](../../docs/technical-implementation/semantic-kernel/Model-Configuration.md)
- ✅ Azure OpenAI GPT-4
- ✅ Azure OpenAI GPT-3.5-turbo
- ✅ Azure OpenAI text-embedding-ada-002
- ❌ Anthropic Claude - 留待 Phase 2
- ❌ 本地開源模型（Llama, Mistral）- 留待 Phase 2

### 架構邊界

**架構模式**: 參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md)
- ✅ Clean Architecture（分層架構）
- ✅ DDD（Domain-Driven Design）
- ✅ CQRS（Command Query Responsibility Segregation）
- ✅ Repository Pattern
- ✅ Unit of Work Pattern

**通訊模式**: 參考 [docs/architecture/adrs/ADR-007-Communication-Architecture.md](../../docs/architecture/adrs/ADR-007-Communication-Architecture.md)
- ✅ Phase 1: MediatR（進程內通訊）
- ❌ Phase 2: Service Bus（分散式通訊）- 留待 Phase 2

**狀態管理**: 參考 [docs/architecture/adrs/ADR-006-State-Management.md](../../docs/architecture/adrs/ADR-006-State-Management.md)
- ✅ 混合模式：Redis（快取）+ PostgreSQL（持久化）
- ❌ Phase 2: Redis Cluster（高可用）- 留待 Phase 2

---

## ✅ 驗收標準（MVP 完成定義）

### 功能驗收

基於 [docs/testing/Testing-Strategy.md](../../docs/testing/Testing-Strategy.md)，MVP 需滿足以下驗收標準：

#### 1. Agent 管理
- ✅ 所有 CRUD 操作正常運作
- ✅ 狀態轉換符合業務規則
- ✅ 搜尋和篩選功能準確
- ✅ 批量操作成功執行
- ✅ Agent 執行引擎穩定運行（成功率 >95%）

#### 2. Plugin 系統
- ✅ Plugin 載入和卸載正常
- ✅ 版本管理功能完整
- ✅ 熱重載機制運作正常（70% 完成即可）
- ✅ 安全隔離有效（無逃逸風險）

#### 3. 對話管理
- ✅ 對話 CRUD 操作正常
- ✅ 多模態消息正確展示
- ✅ 對話歷史查詢準確
- ✅ Chat UI 流暢（響應時間 <2 秒）

#### 4. Persona Framework
- ✅ 6 步向導流程完整
- ✅ 10 種預設 Persona 可用
- ✅ Persona 配置正確應用到 Agent
- ✅ 一致性評分準確（誤差 <10%）

#### 5. Knowledge Base
- ✅ 文檔上傳和解析成功（支援格式）
- ✅ 向量索引建立正常
- ✅ RAG 檢索準確（Top-5 準確率 >80%）
- ✅ 檢索延遲 <1 秒

#### 6. Code Interpreter
- ✅ Python 代碼執行成功（成功率 >90%）
- ✅ 安全沙箱有效（無容器逃逸）
- ✅ 結果可視化正確展示
- ✅ 執行超時機制運作正常

#### 7. Text-to-SQL
- ✅ 自然語言轉 SQL 準確（準確率 >70%）
- ✅ SQL 預覽和確認流程完整
- ✅ 查詢執行正常
- ✅ 結果展示和導出功能正常

#### 8. Multi-Agent Workflow
- ✅ 10 種節點類型可用
- ✅ 節點連接和配置正常
- ✅ 工作流執行引擎穩定（成功率 >85%）
- ✅ 執行日誌完整記錄

#### 9. 前端應用
- ✅ 12 個核心頁面全部實作
- ✅ Design System 完整應用
- ✅ WCAG 2.1 AA 無障礙標準達成
- ✅ 響應式設計（Desktop, Tablet, Mobile）

#### 10. 監控與管理
- ✅ 基本監控指標可查看
- ✅ 日誌記錄完整（結構化日誌）
- ✅ RBAC 權限控制正常
- ✅ 健康檢查端點運作正常

### 非功能驗收

參考 [docs/development-standards/Non-Functional-Requirements.md](../../docs/development-standards/Non-Functional-Requirements.md)：

#### 性能標準
- ✅ API 響應時間 <2 秒（P95）
- ✅ Agent 執行時間 <30 秒
- ✅ 首頁載入時間 <3 秒
- ✅ RAG 檢索延遲 <1 秒
- ✅ 支援併發用戶數 ≥50

#### 可靠性標準
- ✅ 系統可用性 ≥99%
- ✅ Agent 執行成功率 ≥95%
- ✅ 錯誤恢復時間 <5 分鐘
- ✅ 資料備份頻率：每日

#### 安全標準
參考 [docs/security/Security-Requirements.md](../../docs/security/Security-Requirements.md)：
- ✅ JWT Token 認證機制
- ✅ RBAC 權限控制
- ✅ Code Interpreter 4 層安全防護
- ✅ SQL Injection 防護
- ✅ XSS 防護
- ✅ HTTPS 加密傳輸

#### 測試覆蓋率
參考 [docs/testing/Unit-Testing-Guidelines.md](../../docs/testing/Unit-Testing-Guidelines.md)：
- ✅ 單元測試覆蓋率 ≥80%（Domain + Application Layer）
- ✅ 整合測試覆蓋核心 API 端點
- ✅ E2E 測試覆蓋關鍵用戶旅程（至少 5 個）

---

## 📅 時間與資源限制

### 開發時間線

基於 [docs/project-management/Project-Management-Plan.md](../../docs/project-management/Project-Management-Plan.md)：

**Phase 1 (MVP): 8 個月（36 週）**

| 里程碑 | 週數 | 交付內容 |
|--------|------|----------|
| M1: 項目啟動 | Week 1 | 團隊組建、環境搭建、文檔初始化 |
| M2: 基礎設施就緒 | Week 4 | Docker Compose 環境、資料庫、Redis、Qdrant |
| M3: 核心功能完成 | Week 12 | Agent 管理（US 1.x）+ Plugin 系統（US 2.1-2.2） |
| M4: Code Interpreter 完成 | Week 20 | Code Interpreter（US 5.x）+ 安全防護 |
| M5: Multi-Agent 完成 | Week 28 | Workflow Editor + 執行引擎（US 7.x） |
| M6: 前端完成 | Week 32 | 12 個核心頁面 + Design System |
| M7: MVP 驗收 | Week 36 | 功能驗收、性能測試、安全測試 |

### 資源配置

**團隊結構**:
- 1 名 Project Manager
- 3 名 Backend Developers (.NET, Semantic Kernel)
- 2 名 Frontend Developers (React, TypeScript)
- 1 名 DevOps Engineer (Docker, Infrastructure)

**預算限制**: TWD $300,000（Phase 1）
- 人力成本: TWD $240,000（80%）
- 基礎設施成本: TWD $30,000（10%）
- LLM API 成本: TWD $20,000（7%）
- 其他成本: TWD $10,000（3%）

---

## 🎯 成功指標（MVP KPIs）

### 業務指標

1. **用戶採用率**
   - 目標：≥20 個早期用戶（Alpha/Beta Testing）
   - 目標：用戶留存率 ≥60%（30 天）

2. **功能使用率**
   - Agent 創建數 ≥100 個
   - 對話次數 ≥500 次
   - Code Interpreter 執行次數 ≥200 次
   - Workflow 創建數 ≥30 個

3. **用戶滿意度**
   - NPS（Net Promoter Score）≥40
   - 易用性評分 ≥4.0/5.0
   - 功能完整性評分 ≥3.5/5.0

### 技術指標

1. **性能指標**
   - API 響應時間 P95 <2 秒
   - Agent 執行成功率 ≥95%
   - 系統可用性 ≥99%

2. **品質指標**
   - 單元測試覆蓋率 ≥80%
   - 重大 Bug 數 ≤5 個
   - 安全漏洞數 = 0（Critical）

3. **成本指標**
   - LLM Token 成本 <TWD $20,000/月
   - 基礎設施成本 <TWD $10,000/月

---

## 📊 風險與限制

### 高風險項目（需重點關注）

基於 [docs/project-management/Risk-Management.md](../../docs/project-management/Risk-Management.md)：

1. **R001: Container 逃逸漏洞（🔴 High）**
   - 影響：Code Interpreter 安全性
   - 緩解措施：4 層安全防護、定期安全掃描、滲透測試

2. **R002: LLM 成本超預算（🟡 Medium）**
   - 影響：營運成本控制
   - 緩解措施：Prompt 緩存、動態模型選擇、Token 優化

3. **R004: Multi-Agent 執行複雜度（🟡 Medium）**
   - 影響：Workflow 執行穩定性
   - 緩解措施：簡化 MVP 範圍（僅 10 種節點）、完善錯誤處理

### 技術限制

1. **Phase 1 限制**:
   - 不支援 Kubernetes 部署（單機 Docker Compose）
   - 不支援分散式通訊（僅 MediatR 進程內通訊）
   - 不支援多區域部署和災難恢復

2. **擴展性限制**:
   - 最大併發用戶數：50 人（Phase 1）
   - 最大 Agent 數：500 個
   - 最大 Workflow 節點數：20 個

3. **功能限制**:
   - 詳見「MVP 排除功能」章節

---

## 📝 參考文檔

本 MVP 範圍定義文檔參考以下 `/docs` 目錄文檔（共 44 個引用）：

### 核心架構與設計（11 個）
1. [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) - 專案簡介
2. [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 架構設計文檔
3. [docs/architecture/layered-architecture/Application-Layer.md](../../docs/architecture/layered-architecture/Application-Layer.md) - 應用層設計
4. [docs/architecture/layered-architecture/Infrastructure-Layer.md](../../docs/architecture/layered-architecture/Infrastructure-Layer.md) - 基礎設施層設計
5. [docs/architecture/adrs/ADR-006-State-Management.md](../../docs/architecture/adrs/ADR-006-State-Management.md) - 狀態管理決策
6. [docs/architecture/adrs/ADR-007-Communication-Architecture.md](../../docs/architecture/adrs/ADR-007-Communication-Architecture.md) - 通訊架構決策
7. [docs/api/API-Design-Guidelines.md](../../docs/api/API-Design-Guidelines.md) - API 設計規範
8. [docs/development-standards/Non-Functional-Requirements.md](../../docs/development-standards/Non-Functional-Requirements.md) - 非功能需求
9. [docs/testing/Testing-Strategy.md](../../docs/testing/Testing-Strategy.md) - 測試策略
10. [docs/testing/Unit-Testing-Guidelines.md](../../docs/testing/Unit-Testing-Guidelines.md) - 單元測試指南
11. [docs/project-management/Risk-Management.md](../../docs/project-management/Risk-Management.md) - 風險管理

### User Stories（11 個）
12. [docs/user-stories/01-agent-management/README.md](../../docs/user-stories/01-agent-management/README.md)
13. [docs/user-stories/01-agent-management/US-1.2-Agent-Search-Filter.md](../../docs/user-stories/01-agent-management/US-1.2-Agent-Search-Filter.md)
14. [docs/user-stories/01-agent-management/US-1.3-Agent-Status-Management.md](../../docs/user-stories/01-agent-management/US-1.3-Agent-Status-Management.md)
15. [docs/user-stories/02-plugin-system/README.md](../../docs/user-stories/02-plugin-system/README.md)
16. [docs/user-stories/02-plugin-system/US-2.1-Plugin-System-Foundation.md](../../docs/user-stories/02-plugin-system/US-2.1-Plugin-System-Foundation.md)
17. [docs/user-stories/03-conversation-management/README.md](../../docs/user-stories/03-conversation-management/README.md)
18. [docs/user-stories/04-knowledge-base/README.md](../../docs/user-stories/04-knowledge-base/README.md)
19. [docs/user-stories/05-code-interpreter/README.md](../../docs/user-stories/05-code-interpreter/README.md)
20. [docs/user-stories/06-text-to-sql/README.md](../../docs/user-stories/06-text-to-sql/README.md)
21. [docs/user-stories/07-multi-agent-workflow/README.md](../../docs/user-stories/07-multi-agent-workflow/README.md)
22. [docs/user-stories/08-persona-framework/README.md](../../docs/user-stories/08-persona-framework/README.md)

### 技術實施（8 個）
23. [docs/technical-implementation/backend/README.md](../../docs/technical-implementation/backend/README.md)
24. [docs/technical-implementation/frontend/README.md](../../docs/technical-implementation/frontend/README.md)
25. [docs/technical-implementation/semantic-kernel/SK-Integration-Design.md](../../docs/technical-implementation/semantic-kernel/SK-Integration-Design.md)
26. [docs/technical-implementation/semantic-kernel/Model-Configuration.md](../../docs/technical-implementation/semantic-kernel/Model-Configuration.md)
27. [docs/technical-implementation/rag-knowledge-base/RAG-Design.md](../../docs/technical-implementation/rag-knowledge-base/RAG-Design.md)
28. [docs/technical-implementation/rag-knowledge-base/Document-Processing.md](../../docs/technical-implementation/rag-knowledge-base/Document-Processing.md)
29. [docs/technical-implementation/code-interpreter/Code-Interpreter-Design.md](../../docs/technical-implementation/code-interpreter/Code-Interpreter-Design.md)
30. [docs/technical-implementation/code-interpreter/Container-Management.md](../../docs/technical-implementation/code-interpreter/Container-Management.md)

### UX 設計（13 個）
31. [docs/ux-design/README.md](../../docs/ux-design/README.md)
32. [docs/ux-design/user-research/personas.md](../../docs/ux-design/user-research/personas.md)
33. [docs/ux-design/user-research/user-journey-maps.md](../../docs/ux-design/user-research/user-journey-maps.md)
34. [docs/ux-design/design-system/README.md](../../docs/ux-design/design-system/README.md)
35. [docs/ux-design/wireframes/low-fidelity/01-dashboard.md](../../docs/ux-design/wireframes/low-fidelity/01-dashboard.md)
36. [docs/ux-design/wireframes/low-fidelity/02-agent-list.md](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)
37. [docs/ux-design/wireframes/low-fidelity/03-agent-create.md](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md)
38. [docs/ux-design/wireframes/low-fidelity/04-agent-detail.md](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)
39. [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)
40. [docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md](../../docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md)
41. [docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md](../../docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md)
42. [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)
43. [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)
44. [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md)
45. [docs/ux-design/wireframes/low-fidelity/11-settings.md](../../docs/ux-design/wireframes/low-fidelity/11-settings.md)
46. [docs/ux-design/wireframes/low-fidelity/12-monitoring.md](../../docs/ux-design/wireframes/low-fidelity/12-monitoring.md)

### 部署與安全（4 個）
47. [docs/deployment/Docker-Compose-Setup.md](../../docs/deployment/Docker-Compose-Setup.md)
48. [docs/deployment/Disaster-Recovery.md](../../docs/deployment/Disaster-Recovery.md)
49. [docs/security/Security-Requirements.md](../../docs/security/Security-Requirements.md)
50. [docs/security/Authentication-Authorization.md](../../docs/security/Authentication-Authorization.md)
51. [docs/security/Code-Execution-Security.md](../../docs/security/Code-Execution-Security.md)

### 專案管理（1 個）
52. [docs/project-management/Project-Management-Plan.md](../../docs/project-management/Project-Management-Plan.md)

---

## 📞 聯絡資訊

**文檔維護者**: Project Manager
**審核者**: Solution Architect, Tech Lead
**最後更新**: 2025-11-12

如有任何關於 MVP 範圍的疑問或變更請求，請參考 [claudedocs/4-changes/CHANGE-LOG.md](../../claudedocs/4-changes/CHANGE-LOG.md) 進行變更管理流程。

---

**文檔結束**
