## MVP Scope - 最小可行產品定義

### MVP 哲學：「完整但精簡」

```
完整度策略
    ↓
【架構完整】+ 【功能精簡】+ 【場景聚焦】
    ↓
80% 即用 + 20% 擴展
```

**設計原則**：
1. **架構不妥協**：所有關鍵架構組件都要有，但功能可精簡
2. **單場景深度**：客服場景做深做透，證明框架能力
3. **雙語言平等**：.NET 和 Python 同等重要，都要有完整 SDK
4. **開發者優先**：Phase 1 只服務開發團隊，不強求業務部門

---

### 核心交付物（Must Have）

#### 🏗️ 第一層：Framework Core（框架核心）

**1. Agentic Framework Core Library**
- **技術棧**：.NET 8+ / C#
- **核心功能**：
  - ✅ Semantic Kernel 深度集成與抽象
  - ✅ Agent 生命週期管理（創建、執行、監控、銷毀）
  - ✅ 插件系統（Plugin 註冊、發現、調用機制）
  - ✅ Memory 管理（短期/長期記憶抽象）
  - ✅ Prompt 模板引擎
  - ✅ 配置管理系統（YAML/JSON 配置）
- **品質要求**：
  - 單元測試覆蓋率 ≥ 80%
  - 完整的 XML 文檔註釋
  - NuGet 包可發布狀態
- **交付標準**：
  - 至少支持 3 種 Agent 類型（Simple、ReAct、Multi-agent）
  - 可擴展的 Agent 基類和接口設計

**2. Agent Orchestration Engine**
- **功能範圍**：
  - ✅ **Task Generator（任務生成器）**：
    - 自動將用戶輸入分解為具體可執行任務
    - LLM 驅動的智能任務規劃
    - 任務依賴關係識別
  - ✅ Multi-agent 協作編排（Sequential、Parallel、Conditional）
  - ✅ Agent 間消息傳遞機制
  - ✅ 工作流程定義與執行引擎
  - ✅ Agent 狀態追蹤與日誌
  - ✅ 錯誤處理與重試機制
  - ✅ **Feedback Loop（反饋循環）**：
    - 任務執行結果反饋
    - 動態任務調整機制
    - 多輪優化循環
- **MVP 限制**：
  - 僅支持同步編排（異步編排推遲到 Phase 2）
  - Task Generator 作為 Orchestration 的一部分（不獨立組件）
  - 不包含視覺化編排器（Phase 2 功能）
  - 最多支持 5 個 agent 並發

**3. Knowledge Management System**
- **核心能力**：
  - ✅ RAG 基礎設施（Vector DB 集成 - Qdrant 或 Chroma）
  - ✅ 文檔處理 Pipeline（支持 PDF、Word、Markdown、TXT）
  - ✅ Chunk 策略（固定大小、語義分割）
  - ✅ 3 種檢索策略：
    - Vector Search（語義搜索 - 使用 SK Semantic Memory）
    - Keyword Match（關鍵字匹配 - 自建）
    - Hybrid Search（混合搜索 - 自建）
  - ✅ Re-ranking 機制（基本版 - 自建）
- **MVP 限制**：
  - 僅支持文本文檔（圖片、表格 OCR 推遲到 Phase 1.5）
  - 不包含知識圖譜（Phase 2）
  - 單一 Vector DB 選擇（不支持切換）

**4. Multimodal Output Engine**
- **交付範圍**：
  - ✅ 文本生成（Markdown、純文本 - SK 原生）
  - ✅ 圖片生成集成（DALL-E API 調用 - SK 輔助）
  - ✅ 圖表生成（基於 Mermaid 或 Chart.js - 自建）
  - ✅ 輸出格式轉換（JSON、Markdown、HTML - 自建）
- **MVP 限制**：
  - 圖片和圖表為 API 調用方式，不包含本地生成
  - 不支持視頻生成（Phase 2）
  - 不支持實時串流輸出（Phase 1.5）

---

#### 🔌 第二層：Integration & API Layer（集成與 API 層）

**5. API Gateway**
- **核心功能**：
  - ✅ RESTful API（完整的 CRUD + Agent 執行端點）
  - ✅ WebSocket（基本版 - 支持執行狀態推送）
  - ✅ Authentication（JWT Token 驗證）
  - ✅ Authorization（基於角色的訪問控制 - RBAC）
  - ✅ Rate Limiting（API 頻率限制）
  - ✅ Request/Response 日誌
- **API 範圍**：
  ```
  /api/agents          # Agent CRUD
  /api/agents/{id}/execute  # Agent 執行
  /api/knowledge       # 知識庫管理
  /api/workflows       # 工作流程管理
  /api/code-interpreter # Code Interpreter 執行
  /ws/agents/{id}      # WebSocket 連接
  ```
- **MVP 限制**：
  - 不支持 gRPC（Phase 2）
  - 不支持 GraphQL（Phase 2）
  - 單體部署（不包含微服務架構）

**6. System Integration Connectors（至少 3 個）**
- **Must Have Connectors**：
  1. **Azure OpenAI Connector**（完整集成 - SK 原生）
     - GPT-4, GPT-3.5-turbo
     - 函數調用支持
     - 流式輸出
  2. **Database Connector**（選一：SQL Server 或 PostgreSQL - 自建）
     - 基本 CRUD
     - 查詢執行
  3. **File Storage Connector**（Azure Blob 或 AWS S3 - 自建）
     - 文件上傳/下載
     - 知識庫文檔存儲
- **擴展機制**：
  - 提供 Connector 基類和接口
  - 文檔說明如何創建自定義 Connector

---

#### 💻 第三層：Client SDKs（客戶端 SDK）

**7. Python SDK**（⭐ 一等公民地位）
- **完整功能**：
  - ✅ 所有 API Gateway 端點的 Python 封裝
  - ✅ 類型提示（Type Hints）完整
  - ✅ 異步支持（async/await）
  - ✅ WebSocket 客戶端
  - ✅ 錯誤處理與重試機制
- **品質標準**：
  - PyPI 可發布狀態
  - pytest 測試覆蓋率 ≥ 80%
  - 完整的 docstring（Sphinx 兼容）
  - 至少 5 個完整的使用範例
- **交付物**：
  ```python
  from semantic_kernel_agent import AgentClient

  client = AgentClient(api_url="...", token="...")
  agent = client.agents.create(name="CustomerService", config={...})
  result = await agent.execute(input="用戶問題")

  # Code Interpreter 使用
  code_result = await client.code_interpreter.execute(
      code="import pandas as pd; df = pd.read_csv('data.csv'); print(df.head())"
  )
  ```

**8. .NET SDK**
- **完整功能**：
  - ✅ 所有 API Gateway 端點的 C# 封裝
  - ✅ 強類型（使用泛型和接口）
  - ✅ 異步支持（Task/async/await）
  - ✅ WebSocket 客戶端
  - ✅ Dependency Injection 友好
- **品質標準**：
  - NuGet 可發布狀態
  - xUnit 測試覆蓋率 ≥ 80%
  - 完整的 XML 註釋
  - 至少 5 個完整的使用範例
- **交付物**：
  ```csharp
  var client = new AgentClient(apiUrl: "...", token: "...");
  var agent = await client.Agents.CreateAsync("CustomerService", config);
  var result = await agent.ExecuteAsync("用戶問題");

  // Code Interpreter 使用
  var codeResult = await client.CodeInterpreter.ExecuteAsync(@"
      import pandas as pd
      df = pd.read_csv('data.csv')
      print(df.head())
  ");
  ```

---

#### 🎨 第四層：Reference Application & Tools（參考應用與工具）

**9. Complete Reference Application - Customer Service Scenario**
- **應用範圍**：完整的客服 AI Agent 應用（端到端）
- **Frontend**：
  - ✅ React 18+ SPA
  - ✅ 聊天界面（Chat UI）
  - ✅ 知識庫管理界面
  - ✅ Agent 配置界面（簡化版）
  - ✅ 執行日誌查詢界面
  - ✅ 響應式設計（Desktop + Tablet）
- **Backend**：
  - ✅ .NET 8 Web API（使用 Framework Core）
  - ✅ Authentication（JWT）
  - ✅ 業務邏輯層
- **Database**：
  - ✅ 數據庫 Schema（用戶、對話、知識庫）
  - ✅ 遷移腳本（Entity Framework Migrations）
- **Deployment**：
  - ✅ Docker Compose 配置（一鍵啟動）
  - ✅ Kubernetes Manifests（基本版 - Deployment, Service, Ingress）
- **Testing**：
  - ✅ E2E 測試（至少 5 個關鍵流程）
  - ✅ 集成測試
- **文檔**：
  - ✅ 部署指南
  - ✅ 配置說明
  - ✅ 使用手冊
- **目標**：開發者可以在 1 小時內部署並運行起來

**10. UI Component Library**
- **React Components**：
  - ✅ `<ChatWindow />` - 聊天窗口組件
  - ✅ `<AgentConfigForm />` - Agent 配置表單
  - ✅ `<KnowledgeUploader />` - 知識庫文件上傳
  - ✅ `<ExecutionLogger />` - 執行日誌顯示
  - ✅ `<MultimodalOutput />` - 多模態輸出展示
  - ✅ `<CodeExecutionResult />` - Code Interpreter 結果展示
- **品質要求**：
  - TypeScript
  - Storybook 文檔
  - 至少 3 個主題配色
  - npm 可發布狀態

**11. CLI Scaffolding Tool**
- **功能**：
  - ✅ 項目初始化（`sk-agent init`）
  - ✅ Agent 腳手架生成（`sk-agent create agent <name>`）
  - ✅ Connector 腳手架生成（`sk-agent create connector <name>`）
  - ✅ 配置文件生成（`sk-agent config generate`）
- **支持語言**：
  - .NET 項目腳手架
  - Python 項目腳手架
- **交付標準**：
  - 生成的代碼可直接運行
  - 包含基本的單元測試模板

---

#### 🔬 第五層：Code Interpreter（代碼解釋器）- 🆕 差異化競爭力

**12. Code Interpreter（自建 Docker 沙箱）**

**核心價值**：
- 🎯 對標 Fujitsu Kozuchi 的殺手級功能
- 🎯 支持數據分析、複雜計算、動態工具生成場景
- 🎯 差異化競爭優勢

**功能範圍**：
- ✅ **Python 代碼執行**（MVP 階段）
  - 支持標準 Python 3.11
  - 預裝數據科學核心庫：
    - pandas（數據處理）
    - numpy（數值計算）
    - matplotlib（可視化）
- ✅ **安全沙箱環境**（Docker 容器隔離）
  - 多層安全機制（詳見下文技術方案）
  - 資源限制（CPU、Memory、執行時間）
  - 網絡隔離（無外網訪問）
- ✅ **執行管理**
  - 靜態代碼安全分析（執行前檢查危險模式）
  - 執行超時控制（30 秒）
  - 結果捕獲（stdout、stderr、生成文件）
  - 容器自動清理
- ✅ **API 集成**
  - RESTful API 端點：`POST /api/code-interpreter/execute`
  - SDK 封裝（Python 和 .NET）
  - 與 Agent Framework 無縫集成

**安全機制（多層防護）**：

**Layer 1：靜態代碼分析（執行前）**
- 危險模式檢測（os.system、subprocess、eval、exec 等）
- 文件系統危險操作檢測
- 網絡操作檢測
- 代碼注入模式檢測

**Layer 2：Docker 容器隔離**
```yaml
安全配置:
  網絡隔離: NetworkMode = none（無網絡訪問）
  只讀根文件系統: ReadonlyRootfs = true
  工作目錄: /workspace（唯一可寫目錄）
  資源限制:
    CPU: 0.5 核心
    Memory: 512MB
    執行時間: 30 秒超時
    進程數: 最多 50 個（防止 fork bomb）
  用戶權限: nobody:nogroup（非 root 用戶）
```

**Layer 3：用戶命名空間隔離**
- 容器內的 root 映射到宿主機普通用戶
- 即使容器逃逸，無法獲得宿主機 root 權限

**Layer 4：Seccomp 系統調用過濾**
- 只允許必要的系統調用（read、write、open 等）
- 禁止危險操作（mount、reboot、ptrace 等）

**使用場景**：
1. **數據分析**：
   - 用戶上傳 CSV/Excel → Agent 生成 pandas 代碼分析
   - 生成統計報表、趨勢圖
2. **複雜計算**：
   - 業務規則計算（保險理賠、定價模型）
   - 數學建模
3. **動態工具生成**：
   - Agent 根據任務即時生成工具代碼執行

**MVP 限制**：
- ❌ 僅支持 Python（C# 推遲到 Phase 2）
- ❌ 不支持外部包安裝（只有預裝的 3 個庫）
- ❌ 不支持多文件項目（單文件執行）
- ❌ 不支持文件上傳（Phase 1.5）
- ❌ 不支持持久化會話（每次執行獨立）

**技術實現**（詳細方案）：

**架構**：
```
Agent Framework
    ↓
CodeInterpreterService (.NET)
    ↓
Docker Execution Engine
    ↓
Docker Container (Python 3.11 + Alpine)
```

**Docker 鏡像**：
```dockerfile
FROM python:3.11-alpine
RUN apk add --no-cache gcc musl-dev libffi-dev
RUN pip install --no-cache-dir pandas==2.0.3 numpy==1.24.3 matplotlib==3.7.2
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser
WORKDIR /workspace
```

**執行流程**：
1. 接收代碼 → 靜態安全檢查
2. 創建臨時工作目錄 → 寫入代碼文件
3. 創建 Docker 容器（安全配置）
4. 執行代碼（30 秒超時）
5. 捕獲輸出（stdout、stderr）
6. 銷毀容器 → 清理臨時文件
7. 返回執行結果

**開發時間估算**：
- Docker 鏡像構建：2 天
- Execution Engine 開發：5 天
- 靜態代碼分析器：3 天
- Code Interpreter Service：5 天
- 集成測試與安全測試：5 天
- 文檔編寫：2 天
- **總計**：22 工作日（約 **2 個月**，含安全審查）

**風險與緩解**：
- 🔴 **安全漏洞風險**：外部安全審計 + 渗透測試
- 🟡 **性能問題**（容器啟動慢）：容器池 + 預熱機制（Phase 2）
- 🟢 **維護成本**：自動化測試 + 監控

---

#### 📚 第六層：Documentation & Examples（文檔與範例）

**13. Comprehensive Documentation**
- **架構文檔**：
  - ✅ 系統架構圖（使用 Mermaid）
  - ✅ 組件交互流程圖
  - ✅ 數據流向圖
  - ✅ **Semantic Kernel 能力映射圖**（標註哪些功能來自 SK、哪些自建）
- **開發者文檔**：
  - ✅ Quick Start（5 分鐘快速開始）
  - ✅ Installation Guide（安裝指南）
  - ✅ API Reference（完整 API 文檔）
  - ✅ SDK Usage Guide（SDK 使用指南 - .NET & Python）
  - ✅ Configuration Guide（配置指南）
  - ✅ Deployment Guide（部署指南 - Docker/K8s）
  - ✅ **Code Interpreter Guide**（代碼執行器使用指南）
  - ✅ Troubleshooting Guide（故障排除）
- **教學範例（每種語言至少 5 個）**：
  - .NET Examples:
    1. Simple Agent（單一 Agent）
    2. Multi-Agent Workflow（多 Agent 協作）
    3. RAG Knowledge Integration（RAG 知識集成）
    4. Custom Plugin Development（自定義插件）
    5. Custom Connector Development（自定義連接器）
    6. **Code Interpreter Integration**（代碼執行器集成）
  - Python Examples:
    1. Simple Agent（單一 Agent）
    2. Multi-Agent Workflow（多 Agent 協作）
    3. RAG Knowledge Integration（RAG 知識集成）
    4. Custom Plugin Development（自定義插件）
    5. Async Agent Execution（異步執行）
    6. **Data Analysis with Code Interpreter**（數據分析）
- **視頻教學**（可選但推薦）：
  - 15 分鐘快速入門視頻
  - 架構講解視頻（30 分鐘）
  - Code Interpreter 安全機制講解（15 分鐘）

---

### MVP 明確排除範圍（Out of Scope）

#### ⛔ Phase 1 不包含但重要的功能

**1. 業務場景多樣化**
- ❌ 多個業務場景的 Reference Application（僅客服場景）
- ❌ 行業模板庫（金融、醫療、零售等）
- ⏰ 推遲到：Phase 1.5（業務驗證階段會增加 2-3 個場景）

**2. 商業化功能**
- ❌ Multi-Tenant 架構（多租戶隔離）
- ❌ 計量計費系統（Usage Metering & Billing）
- ❌ 白標化支持（White-label Customization）
- ❌ SaaS 部署版本
- ⏰ 推遲到：Phase 2（商業化準備階段）

**3. 高級 AI 功能**
- ❌ 知識圖譜（Knowledge Graph）
- ❌ 自動 Agent 生成（AI 生成 Agent 配置）
- ❌ Agent 自我學習與優化
- ❌ 多模型自動選擇（Model Router）
- ⏰ 推遲到：Phase 2

**4. 高級編排功能**
- ❌ 視覺化工作流程設計器（Drag & Drop）
- ❌ 異步 Agent 編排（Long-running Workflows）
- ❌ 分布式 Agent 執行（跨機器）
- ❌ Task Generator 獨立組件（MVP 作為 Orchestration 的一部分）
- ⏰ 推遲到：Phase 2

**5. 移動端支持**
- ❌ iOS SDK
- ❌ Android SDK
- ❌ React Native Components
- ⏰ 推遲到：Phase 2

**6. 高級安全功能**
- ❌ SSO 集成（SAML、OAuth2）
- ❌ Audit Logging（完整審計日誌）
- ❌ Data Encryption at Rest（靜態數據加密）
- ⏰ 推遲到：Phase 2（企業級功能）

**7. 高級運維功能**
- ❌ Auto-scaling（自動擴展）
- ❌ Multi-region Deployment（多區域部署）
- ❌ Advanced Monitoring Dashboard（Grafana/Prometheus 完整集成）
- ❌ A/B Testing Framework（Agent 版本對比）
- ⏰ 推遲到：Phase 2

**8. 社區生態功能**
- ❌ Plugin Marketplace（插件市場）
- ❌ Community Forums（社區論壇）
- ❌ Contributor Dashboard（貢獻者儀表板）
- ⏰ 推遲到：Phase 2+

**9. Code Interpreter 高級功能**
- ❌ C# 代碼執行（僅 Python）
- ❌ 自定義包安裝
- ❌ 持久化會話（Stateful Execution）
- ❌ 文件上傳/下載
- ❌ 容器池（性能優化）
- ⏰ 推遲到：Phase 1.5 / Phase 2

---

### MVP 成功標準（Phase 1 Exit Criteria）

#### ✅ 技術完成標準

**代碼品質門檻**：
- [ ] 所有 13 項核心交付物 100% 完成
- [ ] 單元測試覆蓋率 ≥ 80%（Framework Core + SDKs）
- [ ] 集成測試通過率 100%（至少 20 個測試案例）
- [ ] E2E 測試通過率 100%（Reference Application）
- [ ] 零 P0/P1 級別 Bug（Critical/High Priority）
- [ ] 技術債務評估 ≤ 5%（使用 SonarQube）
- [ ] **Code Interpreter 安全測試通過**（外部安全審計）

**性能基線達成**：
- [ ] Knowledge 查詢 Precision@5 ≥ 80%（使用測試數據集）
- [ ] API 響應時間 P95 ≤ 3 秒（100 並發）
- [ ] System Uptime ≥ 99%（連續運行 1 週無崩潰）
- [ ] Agent 執行成功率 ≥ 95%（排除外部 API 失敗）
- [ ] **Code Interpreter 執行時間** P95 < 5 秒（含容器啟動）

**文檔完整性**：
- [ ] 所有公共 API 100% 文檔化
- [ ] Quick Start 可用性測試通過（新開發者 ≤ 1 小時完成部署）
- [ ] 至少 12 個完整範例（.NET 6 個 + Python 6 個）
- [ ] Troubleshooting Guide 包含至少 20 個常見問題
- [ ] **Code Interpreter 安全最佳實踐文檔**

**可發布狀態**：
- [ ] .NET NuGet 包可發布（版本 1.0.0-beta）
- [ ] Python PyPI 包可發布（版本 1.0.0-beta）
- [ ] Docker Image 可發布到 Container Registry
- [ ] GitHub Repository 公開準備（License、README、CONTRIBUTING）

---

#### ✅ 開發者驗證標準

**內部採用指標**：
- [ ] 至少 2 個新項目使用框架完成開發（POC 或 MVP）
- [ ] 開發團隊 100% 認知度（所有開發者知道框架存在）
- [ ] 至少 10 名活躍開發者（至少執行過 1 次 Agent）
- [ ] 開發者滿意度 ≥ 4.0/5.0（問卷調查）
- [ ] **Python 和 .NET 開發者滿意度均達標**

**學習曲線驗證**：
- [ ] 新開發者 Onboarding ≤ 2 天（從零到創建第一個 Agent）
- [ ] Quick Start 文檔完成度 100%（開發者可獨立完成）
- [ ] 技術支持請求 ≤ 5 次/週（內部 Slack 或郵件）
- [ ] **Code Interpreter 使用成功率** ≥ 90%（首次嘗試）

**技術社群初步形成**：
- [ ] 內部技術分享會完成（至少 1 次，參與人數 ≥ 15 人）
- [ ] 內部文檔訪問量增長 ≥ 20%/月
- [ ] 至少 3 個開發者貢獻了代碼或文檔改進

---

#### ✅ 業務準備標準

**Phase 1.5 就緒檢查**：
- [ ] 至少 1 個業務部門表達試用意願（非強制，自願）
- [ ] 業務場景分析完成（識別出 Phase 1.5 的 2-3 個目標場景）
- [ ] 業務用戶培訓材料準備完成（簡化版，非技術人員可理解）

**成本與 ROI 初步驗證**：
- [ ] 開發成本實際支出 ≤ 預算 110%（允許 10% 超支）
- [ ] 至少 1 個項目產生可量化的效率提升（時間節省 ≥ 30%）
- [ ] 框架維護成本評估完成（預估 Phase 2 所需資源）

---

#### ✅ 決策門檻（Go/No-Go Decision）

**GO 條件（進入 Phase 1.5）**：
1. **技術完成度 ≥ 95%**（13 項交付物中至少 12 項完全完成）
2. **性能基線 100% 達成**（所有 KPI 達標）
3. **開發者採用 ≥ 50%**（至少 2 個項目 + 10 名活躍開發者）
4. **零 Critical Bug**
5. **業務部門興趣確認**（至少 1 個部門願意試用）
6. **Code Interpreter 安全審計通過**

**NO-GO 條件（需要延長 Phase 1 或重新規劃）**：
1. **技術完成度 < 85%**（超過 2 項核心交付物未完成）
2. **性能基線達成率 < 80%**（關鍵 KPI 未達標）
3. **開發者採用率 < 30%**（少於 1 個項目或少於 5 名開發者）
4. **存在 P0/P1 級別 Bug ≥ 3 個**
5. **無業務部門表達興趣**（表示框架吸引力不足）
6. **Code Interpreter 存在嚴重安全漏洞**

**DELAY 條件（需要額外 1-2 個月）**：
- 技術完成度 85-94%
- 性能基線達成率 80-95%
- 開發者採用率 30-49%
- 存在 1-2 個 P1 Bug
- Code Interpreter 需要額外安全加固

---

### MVP 交付時間表（調整後）

```
Month 1-2    Month 3-4    Month 5-6    Month 7-8
  ↓            ↓            ↓            ↓
基礎架構    → 集成開發  → 應用開發  → 測試與文檔
  ↓            ↓            ↓            ↓
Core         API/SDK      Reference   Code Interpreter
Framework    Integration  Application  + Validation
```

**詳細里程碑**：
- **M1-M2**：Framework Core + Orchestration + Knowledge + Multimodal
- **M3-M4**：API Gateway + Connectors + .NET SDK + Python SDK
- **M5-M6**：Reference Application + UI Components + CLI Tool
- **M7-M8**：**Code Interpreter 開發與安全測試** + 文檔完善 + Bug 修復 + Beta 發布

**調整說明**：
- 原計劃：6 個月
- 加入 Code Interpreter：+2 個月
- **總計**：**8 個月（32 週）**

**資源需求**：
- 開發團隊：5 人（2 後端 + 1 前端 + 1 全棧 + 1 DevOps）
- 兼職支持：1 安全工程師（Code Interpreter 安全審查）
- 預算：800K（8 個月 × 5 人，含外部安全審計費用）

---

### Semantic Kernel 能力映射分析

#### 📊 框架組件與 SK 依賴關係

**依賴程度分類**：
```
🟢 SK 原生提供（直接使用，~10%）
🟡 SK 基礎 + 包裝抽象（30-50% 工作量，~25%）
🟠 SK 輔助 + 大量外部開發（70%+ 工作量，~15%）
🔴 完全外部開發（SK 不涉及，~50%）
```

#### 組件依賴詳解

| MVP 組件 | SK 依賴 | 說明 |
|---------|---------|------|
| **Agent 抽象層** | 🟡 SK 基礎 + 包裝 | SK 提供 Kernel，我們包裝 Agent 類、狀態、生命週期 |
| **Plugin 系統** | 🟢 SK 原生 | SK 的 Plugin 系統可直接使用 |
| **Memory 管理** | 🟡 SK 基礎 + 包裝 | SK 提供 Semantic Memory，我們包裝短期/長期記憶 |
| **Prompt 引擎** | 🟢 SK 原生 | 直接使用 SK Prompt Template |
| **Task Generator** | 🔴 完全外部開發 | SK 無此功能，需自己設計 |
| **Multi-Agent 編排** | 🔴 完全外部開發 | SK 不支持，我們的核心價值 |
| **Feedback Loop** | 🔴 完全外部開發 | SK 無此機制 |
| **RAG - Vector Search** | 🟢 SK 原生 | SK Semantic Memory 提供 |
| **RAG - Hybrid Search** | 🔴 完全外部開發 | SK 只有 Vector Search |
| **Re-ranking** | 🔴 完全外部開發 | SK 無此功能 |
| **文檔處理 Pipeline** | 🔴 完全外部開發 | SK 不涉及 |
| **Multimodal - 文本** | 🟢 SK 原生 | SK 核心功能 |
| **Multimodal - 圖片/圖表** | 🟠 SK 輔助 + 外部 | SK 可調用 API，生成邏輯需自己做 |
| **API Gateway** | 🔴 完全外部開發 | ASP.NET Core，與 SK 無關 |
| **Connectors - OpenAI** | 🟢 SK 原生 | SK 官方提供 |
| **Connectors - Database/Storage** | 🔴 完全外部開發 | 需自己開發 |
| **Python SDK** | 🔴 完全外部開發 | 調用 API，與 SK 無關 |
| **.NET SDK** | 🔴 完全外部開發 | 調用 API，與 SK 無關 |
| **Reference Application** | 🟡 間接使用 SK | 後端業務邏輯使用 Framework（含 SK） |
| **UI Components** | 🔴 完全外部開發 | 前端，與 SK 無關 |
| **CLI Tool** | 🔴 完全外部開發 | 腳手架工具，與 SK 無關 |
| **Code Interpreter** | 🔴 完全外部開發 | 完全自建，SK 不涉及 |
| **Documentation** | 🟡 部分介紹 SK | 需說明 SK，但主要是我們的文檔 |

**整體依賴統計**：
- **SK 原生提供**：~10%（Plugin、Prompt、部分 Memory、OpenAI Connector）
- **SK 基礎 + 包裝**：~25%（Agent 抽象、Memory 管理、部分 Multimodal）
- **SK 輔助**：~15%（圖片/圖表生成調用）
- **完全外部開發**：~50%（Orchestration、Code Interpreter、API、SDK、UI、RAG 增強）

**關鍵洞察**：
> Semantic Kernel 提供了約 **35%** 的基礎能力（主要是 LLM 調用、Plugin、基礎 Memory），框架的核心價值在於 SK 之上的 **65%** 自主開發部分（Agent 編排、完整 RAG、Code Interpreter、API 生態）。

---

### 競品對比：Fujitsu Kozuchi vs 我們的 MVP

#### 📊 能力對比總結

| 能力維度 | Fujitsu Kozuchi | 我們的 MVP | 差距 |
|---------|----------------|-----------|------|
| **Core Agent 能力** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | 持平 |
| **Multi-Agent 編排** | ⭐⭐ | ⭐⭐⭐⭐⭐ | +3（我們領先） |
| **Code Interpreter** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 持平（我們加入後） |
| **Task Generator** | ⭐⭐⭐⭐ | ⭐⭐⭐ | -1（Kozuchi 略勝） |
| **Knowledge Management** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | +2（我們領先） |
| **開發者生態（SDK/API）** | ⭐⭐ | ⭐⭐⭐⭐⭐ | +3（我們領先） |
| **Data Manipulation** | ⭐⭐⭐⭐ | ⭐⭐⭐ | -1（Kozuchi 略勝） |
| **Reference Application** | ⭐⭐ | ⭐⭐⭐⭐⭐ | +3（我們領先） |
| **Feedback Loop** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | 持平（我們加入後） |

**總分**：
- **Kozuchi**：31/45（69%）
- **我們的 MVP**（加入 Code Interpreter 後）：**37/45（82%）**

**結論**：✅ **我們的 MVP 能力全面超越 Kozuchi（+6 分優勢）**

#### 戰略定位差異

**Kozuchi**：
- 封閉式 AI Agent **產品**（Buy and Use）
- 面向企業終端用戶
- Teams 集成為主
- 商業化 SaaS 模式

**我們**：
- 開放式 AI Agent **開發框架**（Build Your Own）
- 面向開發者和企業 IT 團隊
- API-first + SDK 生態
- 內部驗證 → 商業化（雙軌道）

**類比**：
```
Kozuchi ≈ Salesforce（封閉產品）
我們 ≈ Django/Rails（開放框架）
```

---

### 🎯 MVP 核心理念總結

> "我們交付的不是一個半成品框架，而是一個**架構完整、功能精簡、立即可用**的開發平台。開發者可以在 1 小時內部署運行，在 2 天內掌握使用，在 4 週內開發出第一個生產級 AI Agent 應用。"

**關鍵差異化**：
1. ✅ **Multi-Agent 編排能力**（Kozuchi 不具備）
2. ✅ **Code Interpreter**（對標 Kozuchi，達到同等水平）
3. ✅ **完整的開發者生態**（SDK、API、Reference App、文檔）
4. ✅ **深度 Knowledge Management**（Hybrid Search、Re-ranking）
5. ✅ **開放框架定位**（vs Kozuchi 的封閉產品）

---

## Technical Considerations - 技術選型與架構決策

### 核心技術棧總覽

```
┌─────────────────────────────────────────────────────────────┐
│  Technology Stack Summary                                   │
├─────────────────────────────────────────────────────────────┤
│  Backend Core:        .NET 8 + C# 12                        │
│  AI Foundation:       Semantic Kernel 1.x                   │
│  API Layer:           ASP.NET Core Web API                  │
│  Frontend:            React 18+ + TypeScript                │
│  Client SDKs:         .NET 8 + Python 3.11+                 │
│  Vector DB:           Qdrant (primary) / Chroma (fallback) │
│  Database:            PostgreSQL (primary) / SQL Server     │
│  Cache & Queue:       Redis + Azure Service Bus            │
│  Container:           Docker + Kubernetes                   │
│  Code Interpreter:    Docker + Python 3.11 Alpine          │
│  Monitoring:          Prometheus + Grafana + App Insights  │
│  CI/CD:               GitHub Actions / Azure DevOps        │
└─────────────────────────────────────────────────────────────┘
```

---

### 第一部分：分層技術選型決策

#### 🏗️ Layer 1: Framework Core（框架核心層）

##### 1.1 主要技術棧：.NET 8 + C#

**選擇理由**：
1. ✅ **企業級成熟度**：
   - Microsoft 官方支持，長期穩定性保證
   - 豐富的企業級開發工具和生態系統
   - 團隊已有 .NET 專業技能

2. ✅ **Semantic Kernel 原生支持**：
   - SK 最初就是為 .NET 設計的
   - .NET 版本功能最完整、更新最快
   - 與 Azure 服務深度整合

3. ✅ **性能優勢**：
   - .NET 8 性能優異（接近 C++ 級別）
   - AOT 編譯支持（Phase 2 可用）
   - 低內存佔用

4. ✅ **現代語言特性**：
   - C# 12 支持 Pattern Matching、Record Types、Nullable Reference Types
   - 強類型系統減少運行時錯誤
   - 優秀的異步編程支持（async/await）

**替代方案對比**：

| 方案 | 優勢 | 劣勢 | 決策 |
|------|------|------|------|
| **.NET 8** | 企業級、SK 原生、性能優異 | Windows 傳統印象 | ✅ **選擇** |
| **Python + FastAPI** | AI 生態豐富、快速開發 | 性能較差、類型安全弱 | ❌ 不適合核心 |
| **Node.js + TypeScript** | 前後端統一、社群活躍 | SK 支持較弱、企業級經驗少 | ❌ 不適合 |
| **Java + Spring Boot** | 企業級成熟 | SK 不支持、團隊技能不匹配 | ❌ 不考慮 |

**技術風險**：
- 🟡 **跨平台部署**：.NET 在 Linux 上的部署經驗相對較少
- **緩解**：Docker 容器化部署，消除平台差異

---

##### 1.2 AI 基礎：Semantic Kernel 1.x

**選擇理由**：
1. ✅ **官方支持與生態**：
   - Microsoft 官方項目，持續維護
   - 與 Azure OpenAI 深度整合
   - 活躍的社群和豐富的文檔

2. ✅ **核心能力完整**：
   - LLM 抽象層（支持多種模型）
   - Plugin 系統（可擴展）
   - Semantic Memory（向量搜索）
   - Planner（任務規劃參考）

3. ✅ **企業級設計**：
   - 穩定的 API 設計
   - 良好的錯誤處理
   - 可觀測性支持

**版本選擇**：
- **MVP 階段**：Semantic Kernel 1.x（Latest Stable）
- **升級策略**：跟隨 SK 主版本升級，但保持穩定性優先

**技術風險**：
- 🟡 **版本升級風險**：SK 仍在快速發展，API 可能變動
- **緩解**：
  - 抽象層隔離（我們的 Agent 層不直接暴露 SK API）
  - 版本鎖定（MVP 期間不升級 SK 大版本）
  - 測試覆蓋（確保升級時不破壞功能）

**替代方案對比**：

| 方案 | 優勢 | 劣勢 | 決策 |
|------|------|------|------|
| **Semantic Kernel** | 官方支持、企業級、.NET 原生 | API 仍在演進 | ✅ **選擇** |
| **LangChain (.NET 移植)** | 生態豐富 | .NET 版本不成熟 | ❌ 不成熟 |
| **自建 LLM 抽象層** | 完全可控 | 開發成本高、缺乏生態 | ❌ 成本高 |

---

#### 🔌 Layer 2: Integration & API Layer（集成與 API 層）

##### 2.1 API Gateway：ASP.NET Core Web API

**選擇理由**：
1. ✅ **與核心技術棧統一**：與 .NET 框架核心無縫整合
2. ✅ **性能與穩定性**：高性能、低延遲、成熟穩定
3. ✅ **豐富的功能**：
   - 內建 JWT 認證
   - Middleware 架構（靈活的請求處理）
   - OpenAPI/Swagger 自動文檔生成
   - 健康檢查（Health Checks）
   - Rate Limiting（.NET 8 內建）

**架構模式**：
- **Minimal API**（簡潔端點）+ **Controller**（複雜邏輯）混合模式
- **Vertical Slice Architecture**（按功能垂直切分）
- **CQRS**（Command Query Responsibility Segregation）適用於複雜場景

**技術細節**：
```csharp
// Minimal API 示例（簡潔端點）
app.MapPost("/api/agents/{id}/execute",
    async (string id, ExecuteRequest request, IAgentService service) =>
    {
        var result = await service.ExecuteAsync(id, request);
        return Results.Ok(result);
    })
    .RequireAuthorization()
    .WithOpenApi();

// Controller 示例（複雜邏輯）
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;

    [HttpPost("{id}/execute")]
    [Authorize]
    public async Task<ActionResult<ExecutionResult>> Execute(
        string id,
        [FromBody] ExecuteRequest request)
    {
        // 複雜的業務邏輯處理
    }
}
```

**通訊協議**：
- **RESTful API**（主要，80% 場景）：
  - HTTP/1.1 + HTTP/2
  - JSON 序列化（System.Text.Json）
  - 標準 HTTP 狀態碼
- **WebSocket**（實時通訊，15% 場景）：
  - SignalR（.NET 原生 WebSocket 封裝）
  - 用於 Agent 執行狀態推送
- **gRPC**（Phase 2，5% 場景）：
  - 高性能內部服務通訊
  - 暫不包含在 MVP

**認證與授權**：
- **JWT Token**：
  - 使用 `Microsoft.AspNetCore.Authentication.JwtBearer`
  - Token 過期時間：1 小時（可配置）
  - Refresh Token 機制（Phase 1.5）
- **RBAC**（Role-Based Access Control）：
  - 預定義角色：Admin、Developer、User
  - 基於 ASP.NET Core Identity
  - 細粒度權限控制（Phase 2）

**技術風險**：
- 🟢 **低風險**：ASP.NET Core 是成熟技術，團隊經驗豐富

---

##### 2.2 Vector Database：Qdrant（主要）/ Chroma（備選）

**選擇決策流程**：

**Qdrant（推薦 - 70% 可能性）**：
- ✅ **性能優異**：Rust 編寫，高性能、低延遲
- ✅ **功能豐富**：
  - 支持 HNSW 索引（高效近似最近鄰搜索）
  - Payload 過濾（屬性篩選）
  - 混合搜索（向量 + 屬性）
- ✅ **企業級特性**：
  - 分布式支持（Phase 2 可擴展）
  - 數據持久化
  - 監控與可觀測性
- ✅ **易於部署**：Docker 一鍵部署
- ✅ **.NET 客戶端**：官方提供 C# SDK

**Chroma（備選 - 30% 可能性）**：
- ✅ **輕量級**：易於開發和測試
- ✅ **開源友好**：Apache 2.0 許可證
- ⚠️ **功能較少**：相對 Qdrant 功能較簡單
- ⚠️ **.NET 支持**：需要通過 HTTP API 調用（無官方 SDK）

**最終推薦**：**Qdrant**
- MVP 階段使用 Qdrant（單節點部署）
- Phase 2 可擴展為分布式集群
- 提供 Chroma 作為可選後端（通過抽象層切換）

**技術細節**：
```csharp
// Vector Store 抽象層
public interface IVectorStore
{
    Task<string> UpsertAsync(string collectionName, VectorRecord record);
    Task<IEnumerable<VectorSearchResult>> SearchAsync(
        string collectionName,
        float[] vector,
        int topK = 5,
        Dictionary<string, object>? filter = null);
}

// Qdrant 實現
public class QdrantVectorStore : IVectorStore
{
    private readonly QdrantClient _client;

    public async Task<IEnumerable<VectorSearchResult>> SearchAsync(...)
    {
        var searchResult = await _client.SearchAsync(
            collectionName: collectionName,
            vector: vector,
            limit: (ulong)topK,
            filter: ConvertFilter(filter)
        );
        return searchResult.Select(r => new VectorSearchResult
        {
            Id = r.Id.ToString(),
            Score = r.Score,
            Payload = r.Payload
        });
    }
}
```

**其他替代方案對比**：

| Vector DB | 優勢 | 劣勢 | 決策 |
|-----------|------|------|------|
| **Qdrant** | 性能、功能、企業級 | Rust 生態（團隊不熟悉） | ✅ **主選** |
| **Chroma** | 輕量、易用 | 功能較少、.NET 支持弱 | 🟡 **備選** |
| **Pinecone** | 全託管、高性能 | 商業服務、成本高、數據隱私 | ❌ 不適合內部 |
| **Weaviate** | 功能豐富、GraphQL | 複雜度高、資源消耗大 | ❌ 過於複雜 |
| **Milvus** | 高性能、可擴展 | 重量級、運維複雜 | ❌ MVP 過重 |
| **Azure AI Search** | Azure 原生、全託管 | 成本高、供應商鎖定 | 🟡 Phase 2 考慮 |

**技術風險**：
- 🟡 **Qdrant 團隊經驗不足**：團隊對 Qdrant 不熟悉
- **緩解**：
  - 詳細的部署文檔
  - 開發環境使用 Docker Compose 簡化
  - 抽象層設計（可切換到 Chroma）

---

##### 2.3 關係型數據庫：PostgreSQL（主要）/ SQL Server（備選）

**選擇理由（PostgreSQL 推薦）**：
1. ✅ **開源免費**：無授權成本，適合內部和商業化
2. ✅ **功能強大**：
   - JSONB 支持（靈活的半結構化數據）
   - 全文搜索（可輔助 Keyword Match）
   - 豐富的擴展（如 pg_vector）
3. ✅ **性能優異**：ACID 完整性、並發性能好
4. ✅ **跨平台**：Linux/Windows/macOS 無縫支持
5. ✅ **.NET 支持**：Npgsql（成熟的 .NET 驅動）

**SQL Server 作為備選**（如果團隊偏好）：
- ✅ 團隊可能更熟悉
- ✅ Azure SQL Database 深度整合
- ⚠️ 授權成本（Express 版本免費但有限制）
- ⚠️ 主要運行在 Windows（Linux 版本功能有限）

**最終推薦**：**PostgreSQL**
- 開源、成本優勢
- 更適合容器化部署
- 商業化時無授權障礙

**技術細節**：
```csharp
// Entity Framework Core + PostgreSQL
services.AddDbContext<AgenticDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
        npgsqlOptions.CommandTimeout(30);
    }));

// Database Schema 設計
public class AgenticDbContext : DbContext
{
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<KnowledgeDocument> KnowledgeDocuments { get; set; }
    public DbSet<ExecutionLog> ExecutionLogs { get; set; }
}

// JSONB 使用示例（靈活的配置存儲）
public class Agent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // JSONB 存儲複雜配置
    [Column(TypeName = "jsonb")]
    public AgentConfiguration Configuration { get; set; }
}
```

**技術風險**：
- 🟢 **低風險**：PostgreSQL 和 EF Core 都是成熟技術

---

##### 2.4 Cache & Message Queue：Redis + Azure Service Bus

**Redis（緩存與會話存儲）**：
- **用途**：
  - API Rate Limiting 計數器
  - 會話狀態緩存
  - Agent 執行結果臨時存儲
  - 分布式鎖（Phase 2）
- **選擇理由**：
  - ✅ 高性能（內存級速度）
  - ✅ 豐富的數據結構（String、Hash、List、Set）
  - ✅ .NET 成熟客戶端（StackExchange.Redis）
- **部署**：
  - 開發環境：Docker 單節點
  - 生產環境：Azure Cache for Redis（託管服務）

**Azure Service Bus（消息隊列）**：
- **用途**：
  - 異步任務處理（文檔處理、長時間執行的 Agent 任務）
  - 事件驅動架構（Agent 執行事件、系統通知）
  - 可靠消息傳遞
- **選擇理由**：
  - ✅ 企業級可靠性（持久化、Dead Letter Queue）
  - ✅ Azure 原生整合
  - ✅ .NET SDK 支持完善
- **MVP 限制**：
  - 基本的隊列功能（不使用 Topic/Subscription）
  - Phase 2 擴展為複雜的事件驅動架構

**替代方案**：
- **RabbitMQ**：開源、功能豐富，但運維複雜度較高
- **Azure Event Hub**：適合大規模事件流，MVP 暫不需要

**技術風險**：
- 🟢 **低風險**：Redis 和 Service Bus 都是成熟技術

---

#### 💻 Layer 3: Client SDKs & Frontend

##### 3.1 Python SDK：Python 3.11+

**技術選型**：
- **Python 版本**：3.11+（支持最新特性，性能優化）
- **HTTP 客戶端**：`httpx`（異步支持、現代化 API）
- **WebSocket 客戶端**：`websockets`
- **類型檢查**：`mypy` + Type Hints
- **測試框架**：`pytest` + `pytest-asyncio`
- **文檔生成**：`Sphinx` + `autodoc`

**設計原則**：
- ✅ **Pythonic API**：符合 Python 慣例（snake_case、with 語句）
- ✅ **異步優先**：所有 IO 操作使用 async/await
- ✅ **類型安全**：完整的 Type Hints
- ✅ **錯誤處理**：清晰的異常層次結構

**技術細節**：
```python
# Python SDK 設計示例
from semantic_kernel_agent import AgentClient
from semantic_kernel_agent.models import AgentConfig, ExecutionResult
from typing import Optional

class AgentClient:
    """Client for interacting with Semantic Kernel Agent Framework."""

    def __init__(
        self,
        api_url: str,
        token: str,
        timeout: int = 30
    ):
        self._client = httpx.AsyncClient(
            base_url=api_url,
            headers={"Authorization": f"Bearer {token}"},
            timeout=timeout
        )

    async def create_agent(
        self,
        name: str,
        config: AgentConfig
    ) -> str:
        """Create a new agent.

        Args:
            name: Agent name
            config: Agent configuration

        Returns:
            Agent ID

        Raises:
            AgentAPIError: If API request fails
        """
        response = await self._client.post(
            "/api/agents",
            json={"name": name, "config": config.dict()}
        )
        response.raise_for_status()
        return response.json()["id"]

    async def execute_agent(
        self,
        agent_id: str,
        input_text: str,
        session_id: Optional[str] = None
    ) -> ExecutionResult:
        """Execute an agent with input text."""
        response = await self._client.post(
            f"/api/agents/{agent_id}/execute",
            json={
                "input": input_text,
                "sessionId": session_id
            }
        )
        response.raise_for_status()
        return ExecutionResult(**response.json())

    async def __aenter__(self):
        return self

    async def __aexit__(self, *args):
        await self._client.aclose()

# 使用示例
async with AgentClient(api_url="...", token="...") as client:
    agent_id = await client.create_agent("CustomerService", config)
    result = await client.execute_agent(agent_id, "用戶問題")
    print(result.output)
```

**技術風險**：
- 🟡 **API 版本同步**：Python SDK 需要與 .NET API 保持同步
- **緩解**：
  - 自動化測試（集成測試覆蓋所有端點）
  - OpenAPI 規範（從 .NET 生成，Python SDK 參考）

---

##### 3.2 .NET SDK：.NET 8 + C# 12

**技術選型**：
- **.NET 版本**：.NET 8（與核心框架一致）
- **HTTP 客戶端**：`HttpClient` + `IHttpClientFactory`
- **WebSocket 客戶端**：`ClientWebSocket` / `SignalR Client`
- **測試框架**：`xUnit` + `Moq`
- **文檔生成**：XML 註釋 + DocFX

**設計原則**：
- ✅ **.NET 慣例**：符合 .NET 命名規範（PascalCase）
- ✅ **強類型**：泛型、接口、記錄類型
- ✅ **DI 友好**：與 ASP.NET Core DI 無縫整合
- ✅ **異步優先**：所有 IO 操作使用 async/await

**技術細節**：
```csharp
// .NET SDK 設計示例
public interface IAgentClient
{
    Task<string> CreateAgentAsync(
        string name,
        AgentConfiguration config,
        CancellationToken ct = default);

    Task<ExecutionResult> ExecuteAgentAsync(
        string agentId,
        string input,
        string? sessionId = null,
        CancellationToken ct = default);
}

public class AgentClient : IAgentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AgentClient> _logger;

    public AgentClient(HttpClient httpClient, ILogger<AgentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> CreateAgentAsync(
        string name,
        AgentConfiguration config,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/agents",
            new { name, config },
            ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateAgentResponse>(ct);
        return result!.Id;
    }

    public async Task<ExecutionResult> ExecuteAgentAsync(
        string agentId,
        string input,
        string? sessionId = null,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/agents/{agentId}/execute",
            new { input, sessionId },
            ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExecutionResult>(ct);
    }
}

// DI 注冊
services.AddHttpClient<IAgentClient, AgentClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
});

// 使用示例
public class MyService
{
    private readonly IAgentClient _agentClient;

    public MyService(IAgentClient agentClient)
    {
        _agentClient = agentClient;
    }

    public async Task ProcessAsync()
    {
        var agentId = await _agentClient.CreateAgentAsync("CS", config);
        var result = await _agentClient.ExecuteAgentAsync(agentId, "問題");
    }
}
```

**技術風險**：
- 🟢 **低風險**：與核心框架技術棧一致

---

##### 3.3 Frontend：React 18+ + TypeScript

**技術選型**：
- **框架**：React 18+（UI 函數庫市場領導者）
- **語言**：TypeScript 5+（類型安全）
- **構建工具**：Vite（快速、現代化）
- **狀態管理**：
  - Zustand（輕量級，適合中小型應用）
  - TanStack Query（服務端狀態管理）
- **UI 組件庫**：
  - **Shadcn/ui**（推薦）：無樣式依賴、可定制
  - 備選：Material-UI、Ant Design
- **表單處理**：React Hook Form + Zod（驗證）
- **樣式方案**：Tailwind CSS（實用優先）
- **測試**：
  - Vitest（單元測試）
  - React Testing Library（組件測試）
  - Playwright（E2E 測試）

**選擇理由**：
1. ✅ **React 生態成熟**：
   - 最大的開發者社群
   - 豐富的第三方庫
   - 優秀的開發體驗
2. ✅ **TypeScript 類型安全**：
   - 減少運行時錯誤
   - 更好的 IDE 支持
   - 與後端 API 類型同步（通過 OpenAPI 生成）
3. ✅ **Vite 性能優異**：
   - 極快的 HMR（Hot Module Replacement）
   - 現代化的構建工具
4. ✅ **Shadcn/ui 可定制性**：
   - 非 npm 依賴（代碼直接複製到項目）
   - 完全可定制
   - 基於 Radix UI（無障礙性好）

**項目結構**：
```
frontend/
├── src/
│   ├── app/              # 應用入口
│   ├── components/       # 可復用組件
│   │   ├── ui/          # Shadcn/ui 組件
│   │   ├── chat/        # 聊天相關
│   │   ├── agent/       # Agent 配置
│   │   └── knowledge/   # 知識庫管理
│   ├── features/         # 功能模組（按業務切分）
│   │   ├── agents/
│   │   ├── chat/
│   │   └── knowledge/
│   ├── lib/              # 工具函數
│   │   ├── api/         # API 客戶端
│   │   └── utils/
│   ├── hooks/            # 自定義 Hooks
│   ├── stores/           # 狀態管理（Zustand）
│   └── types/            # TypeScript 類型定義
├── public/
└── tests/
```

**技術細節**：
```typescript
// API 客戶端（使用 TanStack Query）
import { useQuery, useMutation } from '@tanstack/react-query';

export function useAgents() {
  return useQuery({
    queryKey: ['agents'],
    queryFn: async () => {
      const response = await fetch('/api/agents', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      return response.json();
    }
  });
}

export function useExecuteAgent() {
  return useMutation({
    mutationFn: async ({ agentId, input }: ExecuteAgentParams) => {
      const response = await fetch(`/api/agents/${agentId}/execute`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ input })
      });
      return response.json();
    }
  });
}

// 組件使用
function ChatInterface() {
  const { data: agents } = useAgents();
  const executeAgent = useExecuteAgent();

  const handleSubmit = async (input: string) => {
    const result = await executeAgent.mutateAsync({
      agentId: selectedAgentId,
      input
    });
    // 處理結果
  };

  return (
    <div>
      {/* UI 組件 */}
    </div>
  );
}
```

**替代方案對比**：

| 框架 | 優勢 | 劣勢 | 決策 |
|------|------|------|------|
| **React** | 生態最強、社群最大 | 需要配置較多工具 | ✅ **選擇** |
| **Vue 3** | 學習曲線低、性能好 | 企業採用率較低 | 🟡 可考慮 |
| **Angular** | 完整框架、TypeScript 原生 | 笨重、學習曲線陡 | ❌ 不適合 |
| **Svelte** | 性能優異、代碼簡潔 | 生態較小、企業採用少 | ❌ 生態不足 |

**技術風險**：
- 🟡 **前端團隊技能**：需確認團隊 React + TypeScript 經驗
- **緩解**：
  - 提供腳手架和參考實現
  - 詳細的開發指南
  - Code Review 確保代碼質量

---

#### 🔬 Layer 4: Code Interpreter

##### 4.1 容器技術：Docker

**選擇理由**：
- ✅ **安全隔離**：容器級別隔離，防止代碼逃逸
- ✅ **資源限制**：CPU、Memory、Disk 精確控制
- ✅ **快速啟動**：相比虛擬機啟動快（2-5 秒）
- ✅ **易於管理**：Docker CLI、Docker Compose、Kubernetes 支持
- ✅ **跨平台**：Linux/Windows/macOS 一致體驗

**基礎鏡像選擇**：
- **Python 3.11 Alpine**：
  - Alpine Linux（輕量級，~50MB）
  - Python 3.11（最新穩定版）
  - 最終鏡像大小：~300MB（含 pandas、numpy、matplotlib）

**替代方案對比**：

| 方案 | 優勢 | 劣勢 | 決策 |
|------|------|------|------|
| **Docker** | 成熟、易用、生態豐富 | 容器逃逸風險（低） | ✅ **選擇** |
| **Firecracker (microVM)** | 硬件級隔離、極高安全 | 複雜度高、啟動較慢 | 🟡 Phase 2 考慮 |
| **gVisor** | 安全性高於 Docker | 性能開銷、兼容性問題 | ❌ 複雜度高 |
| **Kubernetes Pod Sandbox** | K8s 原生 | MVP 不需要 K8s 級別 | ❌ 過於複雜 |
| **Process 隔離（chroot）** | 輕量 | 安全性不足 | ❌ 不安全 |

**安全加固**（已在 MVP Scope 詳述）：
- Layer 1：靜態代碼分析
- Layer 2：Docker 容器隔離（網絡隔離、只讀文件系統、資源限制）
- Layer 3：用戶命名空間隔離
- Layer 4：Seccomp 系統調用過濾

**技術風險**：
- 🟡 **容器逃逸風險**：雖然概率低，但仍需外部安全審計
- **緩解**：
  - 多層安全機制
  - 定期安全更新（Docker Engine、基礎鏡像）
  - 外部安全審計（預算 $5K-10K）

---

#### 📊 Layer 5: Monitoring & Observability

##### 5.1 監控方案：Prometheus + Grafana + Application Insights

**技術選型**：
- **Prometheus**（指標收集）：
  - 時間序列數據庫
  - Pull 模式（從應用抓取指標）
  - PromQL 查詢語言
  - 使用 `prometheus-net` 在 .NET 中暴露指標
- **Grafana**（可視化）：
  - 豐富的儀表板
  - 多數據源支持（Prometheus、Azure Monitor）
  - 告警規則
- **Application Insights**（Azure 原生 APM）：
  - 分布式追蹤（Distributed Tracing）
  - 異常監控
  - 性能分析
  - 與 Azure 深度整合

**監控指標（關鍵）**：
```yaml
業務指標:
  - agent_execution_total: Agent 執行次數（按 agent_id、status）
  - agent_execution_duration_seconds: 執行時間分佈（P50/P95/P99）
  - knowledge_query_total: 知識查詢次數
  - knowledge_query_precision: 查詢準確率
  - code_interpreter_execution_total: 代碼執行次數
  - code_interpreter_failure_total: 執行失敗次數

技術指標:
  - http_request_duration_seconds: API 請求時間
  - http_requests_total: API 請求總數（按狀態碼）
  - dotnet_gc_heap_size_bytes: .NET GC 堆大小
  - process_cpu_seconds_total: CPU 使用量
  - process_resident_memory_bytes: 內存使用量

基礎設施指標:
  - redis_connected_clients: Redis 連接數
  - postgres_connections: PostgreSQL 連接數
  - qdrant_vector_count: Qdrant 向量數量
```

**告警規則（示例）**：
```yaml
# Prometheus 告警規則
groups:
  - name: agentic_framework
    rules:
      - alert: HighErrorRate
        expr: |
          rate(http_requests_total{status=~"5.."}[5m]) > 0.05
        for: 5m
        annotations:
          summary: "High error rate detected"

      - alert: SlowAgentExecution
        expr: |
          histogram_quantile(0.95,
            rate(agent_execution_duration_seconds_bucket[5m])) > 10
        for: 10m
        annotations:
          summary: "Agent execution is slow (P95 > 10s)"

      - alert: CodeInterpreterFailureSpike
        expr: |
          rate(code_interpreter_failure_total[5m]) > 0.1
        for: 5m
        annotations:
          summary: "Code Interpreter failure rate is high"
```

**日誌方案**：
- **結構化日誌**：Serilog（.NET）
- **日誌聚合**：
  - 開發環境：Console + File
  - 生產環境：Azure Log Analytics / ELK Stack
- **日誌級別**：
  - Trace/Debug：開發調試
  - Information：關鍵業務事件
  - Warning：潛在問題
  - Error/Critical：錯誤和異常

**技術風險**：
- 🟢 **低風險**：監控工具都是成熟方案

---

#### 🚀 Layer 6: Deployment & CI/CD

##### 6.1 容器編排：Docker Compose（MVP）→ Kubernetes（Phase 2）

**MVP 階段：Docker Compose**
- **選擇理由**：
  - ✅ 簡單易用（單文件配置）
  - ✅ 本地開發友好
  - ✅ 滿足小規模部署需求（單節點）
  - ✅ 快速上手（學習曲線低）

**Docker Compose 配置示例**：
```yaml
version: '3.8'

services:
  # API Gateway
  api:
    build: ./src/AgenticFramework.Api
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__Postgres=Host=postgres;Database=agentic;Username=...
      - ConnectionStrings__Redis=redis:6379
      - VectorStore__Endpoint=http://qdrant:6333
    depends_on:
      - postgres
      - redis
      - qdrant
    restart: unless-stopped

  # PostgreSQL
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: agentic
      POSTGRES_USER: agenticuser
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    restart: unless-stopped

  # Redis
  redis:
    image: redis:7-alpine
    volumes:
      - redis_data:/data
    restart: unless-stopped

  # Qdrant
  qdrant:
    image: qdrant/qdrant:latest
    ports:
      - "6333:6333"
    volumes:
      - qdrant_data:/qdrant/storage
    restart: unless-stopped

  # Frontend
  frontend:
    build: ./frontend
    ports:
      - "3000:80"
    environment:
      - API_URL=http://api:80
    depends_on:
      - api
    restart: unless-stopped

volumes:
  postgres_data:
  redis_data:
  qdrant_data:
```

**Phase 2：Kubernetes**
- **遷移路徑**：
  - Docker Compose → Helm Charts
  - 支持水平擴展（多實例 API Gateway）
  - 支持滾動更新（零停機部署）
  - 支持自動擴展（HPA - Horizontal Pod Autoscaler）

**Kubernetes 優勢**（Phase 2）：
- ✅ 高可用性（多節點、自動故障轉移）
- ✅ 水平擴展（根據負載自動調整）
- ✅ 服務發現（內建 DNS）
- ✅ 配置管理（ConfigMaps、Secrets）

**技術風險**：
- 🟡 **Kubernetes 學習曲線**：團隊需要學習 K8s
- **緩解**：
  - MVP 階段不使用 K8s（降低複雜度）
  - Phase 2 前提供 K8s 培訓
  - 使用 Helm Charts 簡化配置

---

##### 6.2 CI/CD Pipeline：GitHub Actions / Azure DevOps

**選擇理由**：
- **GitHub Actions**（推薦，如果代碼在 GitHub）：
  - ✅ 與 GitHub 原生整合
  - ✅ 免費額度充足（公開倉庫無限、私有 2000 分鐘/月）
  - ✅ 豐富的 Action 市場
  - ✅ YAML 配置簡單
- **Azure DevOps**（備選，如果使用 Azure 生態）：
  - ✅ 與 Azure 深度整合
  - ✅ 企業級功能（審批流程、發布管理）
  - ✅ 強大的 Pipeline 功能

**CI/CD 流程**：
```yaml
# GitHub Actions 示例
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  # Build & Test
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage"

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3

  # Security Scan
  security-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Run Trivy vulnerability scanner
        uses: aquasecurity/trivy-action@master
        with:
          scan-type: 'fs'
          scan-ref: '.'

      - name: Run SonarQube scan
        uses: sonarsource/sonarqube-scan-action@master
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}

  # Build Docker Image
  build-docker:
    needs: [build-and-test, security-scan]
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - uses: actions/checkout@v4

      - name: Build Docker image
        run: docker build -t agentic-framework:${{ github.sha }} .

      - name: Push to Container Registry
        run: |
          echo ${{ secrets.REGISTRY_PASSWORD }} | docker login -u ${{ secrets.REGISTRY_USERNAME }} --password-stdin
          docker push agentic-framework:${{ github.sha }}

  # Deploy to Dev
  deploy-dev:
    needs: build-docker
    runs-on: ubuntu-latest
    environment: development
    steps:
      - name: Deploy to Dev Environment
        run: |
          # kubectl apply or docker-compose commands
```

**部署環境**：
```yaml
環境劃分:
  - Development: 開發環境（自動部署 develop 分支）
  - Staging: 預發布環境（自動部署 main 分支，需手動批准）
  - Production: 生產環境（手動觸發，需多人批准）

部署策略:
  Development:
    - 觸發: Push to develop
    - 部署方式: Docker Compose
    - 自動化測試: 單元測試 + 集成測試

  Staging:
    - 觸發: Push to main
    - 部署方式: Docker Compose / Kubernetes
    - 自動化測試: 全部測試（含 E2E）
    - 需要批准: Tech Lead

  Production:
    - 觸發: Manual trigger
    - 部署方式: Kubernetes（Phase 2）/ Docker Compose（MVP）
    - 策略: Blue-Green Deployment（Phase 2）
    - 需要批准: Tech Lead + Product Owner
```

**技術風險**：
- 🟢 **低風險**：CI/CD 工具都是成熟方案

---

### 第二部分：架構決策記錄（ADR - Architecture Decision Records）

#### ADR 格式說明

每個重大技術決策使用以下格式記錄：

```markdown
# ADR-XXX: [決策標題]

**狀態**: [提議/接受/廢棄/取代]
**日期**: YYYY-MM-DD
**決策者**: [相關人員]
**影響範圍**: [受影響的組件]

## Context（背景）
[為什麼需要做這個決策？當前面臨什麼問題？]

## Decision（決策）
[我們決定做什麼？]

## Consequences（後果）
**正面影響**:
- [好處 1]
- [好處 2]

**負面影響**:
- [代價 1]
- [代價 2]

**緩解措施**:
- [如何減輕負面影響]

## Alternatives Considered（考慮過的替代方案）
| 方案 | 優勢 | 劣勢 | 為何未選擇 |
|------|------|------|-----------|
| 方案 A | ... | ... | ... |

## References（參考資料）
- [相關文檔、討論鏈接]
```

---

#### ADR-001: 選擇 .NET 作為框架核心技術棧

**狀態**: ✅ 接受
**日期**: 2025-01-27
**決策者**: 技術團隊、架構師
**影響範圍**: 整個框架核心

**Context**:
我們需要選擇一個主要的技術棧來構建 Agentic Framework 的核心層。選擇需要考慮：團隊技能、Semantic Kernel 支持、性能、企業級成熟度、商業化可行性。

**Decision**:
選擇 **.NET 8 + C# 12** 作為框架核心技術棧。

**Consequences**:
**正面影響**:
- ✅ Semantic Kernel 原生支持（功能最完整、更新最快）
- ✅ 企業級成熟度高（Microsoft 官方支持）
- ✅ 性能優異（接近原生性能）
- ✅ 團隊已有 .NET 技能
- ✅ 強類型系統減少運行時錯誤
- ✅ 與 Azure 生態深度整合

**負面影響**:
- ⚠️ Python AI 社群可能對 .NET 不熟悉
- ⚠️ 開源社群相對 Python 較小

**緩解措施**:
- 提供高質量的 Python SDK（一等公民地位）
- 通過 RESTful API 層，語言無關
- 詳細的文檔和示例（含 Python）

**Alternatives Considered**:
| 方案 | 優勢 | 劣勢 | 為何未選擇 |
|------|------|------|-----------|
| Python + FastAPI | AI 生態豐富、快速開發 | 性能較差、類型安全弱、SK 支持不完整 | 不適合企業級框架核心 |
| Node.js + TypeScript | 前後端統一、社群活躍 | SK 支持弱、企業級經驗少 | SK 支持不足 |
| Java + Spring Boot | 企業級成熟 | SK 不支持、團隊技能不匹配 | SK 不支持 |

**References**:
- Semantic Kernel 官方文檔
- .NET 性能基準測試

---

#### ADR-002: 加入 Code Interpreter 作為 MVP 核心組件

**狀態**: ✅ 接受
**日期**: 2025-01-27
**決策者**: 產品經理、技術團隊
**影響範圍**: MVP Scope、開發時間、預算

**Context**:
通過競品分析（Fujitsu Kozuchi），發現 Code Interpreter 是差異化競爭的關鍵功能。支持數據分析、複雜計算、動態工具生成場景。但會增加開發時間（+2 個月）和安全風險。

**Decision**:
加入 **Code Interpreter（自建 Docker 沙箱）** 作為 MVP 第 12 項核心交付物。

**Consequences**:
**正面影響**:
- ✅ 達到甚至超越 Kozuchi 的能力（82% vs 69%）
- ✅ 差異化競爭優勢（Multi-Agent + Code Interpreter）
- ✅ 支持關鍵業務場景（財務數據分析、複雜計算）
- ✅ 未來商業化的賣點

**負面影響**:
- ⚠️ 開發時間延長（6 → 8 個月）
- ⚠️ 預算增加（600K → 800K）
- ⚠️ 安全風險增加（需要外部審計）
- ⚠️ 運維複雜度提升

**緩解措施**:
- 多層安全機制（4 層防護）
- 外部安全審計（預算 $5K-10K）
- 詳細的安全最佳實踐文檔
- Phase 1 限制功能（僅 Python、無文件上傳、無持久化會話）

**Alternatives Considered**:
| 方案 | 優勢 | 劣勢 | 為何未選擇 |
|------|------|------|-----------|
| 自建 Docker 沙箱 | 完全可控、無外部依賴 | 開發成本高、安全風險 | ✅ **選擇**（完全可控） |
| E2B SDK（雲端服務） | 快速集成（1 週）、高安全性 | 商業依賴、數據隱私、成本 | 內部框架不適合 |
| 不實現 Code Interpreter | 時間快、風險低 | 與 Kozuchi 有明顯差距 | 失去差異化優勢 |
| Jupyter Kernel Gateway | 成熟開源 | 過於重量級、安全性需加固 | MVP 不合適 |

**References**:
- Fujitsu Kozuchi 競品分析
- OpenAI Code Interpreter 安全機制研究
- Docker 安全最佳實踐

---

#### ADR-003: 選擇 Qdrant 作為主要 Vector Database

**狀態**: ✅ 接受
**日期**: 2025-01-27
**決策者**: 技術團隊、架構師
**影響範圍**: Knowledge Management System、RAG 性能

**Context**:
需要選擇一個 Vector Database 來支持 Knowledge Management 的語義搜索。需要考慮：性能、功能豐富度、.NET 支持、部署便利性、成本。

**Decision**:
選擇 **Qdrant** 作為主要 Vector Database，**Chroma** 作為備選（通過抽象層可切換）。

**Consequences**:
**正面影響**:
- ✅ 性能優異（Rust 編寫）
- ✅ 功能豐富（HNSW 索引、Payload 過濾、混合搜索）
- ✅ 官方提供 .NET SDK
- ✅ 易於部署（Docker 一鍵啟動）
- ✅ 支持分布式（Phase 2 可擴展）

**負面影響**:
- ⚠️ 團隊對 Qdrant 不熟悉
- ⚠️ Rust 生態（如果需要深度定制）

**緩解措施**:
- 詳細的部署文檔和運維指南
- 抽象層設計（IVectorStore 接口，可切換到 Chroma）
- 開發環境使用 Docker Compose 簡化

**Alternatives Considered**:
| Vector DB | 優勢 | 劣勢 | 為何未選擇 |
|-----------|------|------|-----------|
| **Qdrant** | 性能、功能、企業級 | 團隊不熟悉 | ✅ **選擇** |
| **Chroma** | 輕量、易用 | 功能較少、.NET 支持弱 | 🟡 備選 |
| **Pinecone** | 全託管、高性能 | 商業服務、成本高、數據隱私 | 不適合內部框架 |
| **Milvus** | 高性能、可擴展 | 重量級、運維複雜 | MVP 過重 |
| **Azure AI Search** | Azure 原生 | 成本高、供應商鎖定 | Phase 2 考慮 |

**References**:
- Qdrant 官方文檔
- Vector Database 性能對比

---

#### ADR-004: 選擇 PostgreSQL 作為主要關係型數據庫

**狀態**: ✅ 接受
**日期**: 2025-01-27
**決策者**: 技術團隊、DBA
**影響範圍**: 數據持久化、部署、商業化成本

**Context**:
需要選擇關係型數據庫來存儲 Agent 配置、用戶數據、對話歷史、知識庫元數據等。需要考慮：開源免費、性能、功能豐富度、.NET 支持、商業化友好。

**Decision**:
選擇 **PostgreSQL** 作為主要關係型數據庫，**SQL Server** 作為備選（如果團隊強烈偏好）。

**Consequences**:
**正面影響**:
- ✅ 開源免費（無授權成本）
- ✅ 功能強大（JSONB、全文搜索、擴展豐富）
- ✅ 性能優異（ACID、並發性能）
- ✅ 跨平台支持好
- ✅ 商業化無授權障礙

**負面影響**:
- ⚠️ 團隊可能更熟悉 SQL Server

**緩解措施**:
- 使用 Entity Framework Core（抽象層，切換數據庫容易）
- 提供 PostgreSQL 培訓和最佳實踐文檔
- 開發環境使用 Docker，降低安裝門檻

**Alternatives Considered**:
| 數據庫 | 優勢 | 劣勢 | 為何未選擇 |
|--------|------|------|-----------|
| **PostgreSQL** | 開源、功能強、跨平台 | 團隊可能不熟悉 | ✅ **選擇** |
| **SQL Server** | 團隊熟悉、Azure 深度整合 | 授權成本、主要運行在 Windows | 商業化有障礙 |
| **MySQL** | 流行、易用 | 功能較少、企業級特性弱 | 不如 PostgreSQL |
| **SQLite** | 極輕量 | 不支持並發寫入、不適合生產 | 僅開發測試用 |

**References**:
- PostgreSQL vs SQL Server 性能對比
- EF Core 多數據庫支持

---

#### ADR-005: MVP 階段使用 Docker Compose，Phase 2 遷移到 Kubernetes

**狀態**: ✅ 接受
**日期**: 2025-01-27
**決策者**: DevOps 團隊、架構師
**影響範圍**: 部署策略、運維複雜度、可擴展性

**Context**:
需要選擇容器編排方案。Kubernetes 功能強大但複雜，Docker Compose 簡單但能力有限。MVP 階段需要快速驗證，Phase 2 需要支持規模化。

**Decision**:
MVP 階段使用 **Docker Compose**，Phase 2 遷移到 **Kubernetes**。

**Consequences**:
**正面影響**（MVP 階段）:
- ✅ 簡單易用（單文件配置）
- ✅ 學習曲線低（團隊快速上手）
- ✅ 本地開發友好
- ✅ 滿足小規模部署需求

**負面影響**（長期）:
- ⚠️ 不支持水平擴展（單節點限制）
- ⚠️ 不支持自動故障轉移
- ⚠️ 不支持滾動更新（零停機部署）

**緩解措施**:
- 架構設計時考慮 K8s 遷移（無狀態設計、12-Factor App）
- Phase 2 前提供 K8s 培訓
- 使用 Helm Charts 簡化 K8s 配置
- 漸進式遷移（先非關鍵服務，後關鍵服務）

**Alternatives Considered**:
| 方案 | 優勢 | 劣勢 | 為何未選擇 |
|------|------|------|-----------|
| **Docker Compose（MVP）** | 簡單、快速 | 不可擴展 | ✅ **選擇**（MVP） |
| **Kubernetes（MVP）** | 企業級、可擴展 | 複雜、學習曲線陡 | MVP 過於複雜 |
| **Docker Swarm** | 比 K8s 簡單 | 社群小、未來不明 | 不建議投入 |

**References**:
- Docker Compose 文檔
- Kubernetes 遷移最佳實踐

---

### 第三部分：技術風險評估與緩解

#### 風險分類與優先級

| 風險等級 | 定義 | 處理策略 |
|---------|------|----------|
| 🔴 **Critical** | 可能導致項目失敗或重大返工 | 必須立即處理，預留緩衝時間 |
| 🟠 **High** | 顯著影響進度或質量 | 優先處理，制定應對計劃 |
| 🟡 **Medium** | 影響局部功能或效率 | 監控並適時處理 |
| 🟢 **Low** | 影響較小，可接受 | 記錄並定期評估 |

---

#### 技術風險清單

##### 🔴 Risk-001: Code Interpreter 安全漏洞

**描述**:
Code Interpreter 允許執行用戶提供的代碼，存在沙箱逃逸、惡意代碼執行、資源耗盡等安全風險。如果被攻擊，可能導致：
- 宿主機被入侵
- 數據洩露
- 服務癱瘓（DoS）

**影響**:
- 嚴重性：🔴 極高（可能導致整個系統安全崩潰）
- 概率：🟡 中等（雖然有多層防護，但仍有風險）

**緩解措施**:
1. **多層安全機制**（已實施）:
   - Layer 1：靜態代碼分析（執行前檢查）
   - Layer 2：Docker 容器隔離
   - Layer 3：用戶命名空間隔離
   - Layer 4：Seccomp 系統調用過濾

2. **外部安全審計**（必須）:
   - 聘請專業安全公司進行滲透測試
   - 預算：$5K-10K
   - 時間：MVP 結束前

3. **持續安全更新**:
   - 定期更新 Docker Engine
   - 定期更新基礎鏡像（Python）
   - 訂閱 CVE 安全公告

4. **Bug Bounty 計劃**（Phase 2）:
   - 公開 Bug Bounty，獎勵安全研究者

**監控指標**:
- 沙箱逃逸嘗試次數（通過日誌檢測異常行為）
- 執行失敗率（異常高時可能是攻擊）
- 系統資源異常（CPU/Memory 突增）

**應急預案**:
- 如發現嚴重漏洞，立即關閉 Code Interpreter 功能
- 發布安全公告
- 快速修復並重新部署

---

##### 🟠 Risk-002: Semantic Kernel API 變動導致兼容性問題

**描述**:
Semantic Kernel 仍在快速發展（1.x 版本），API 可能在版本升級時發生破壞性變更，導致我們的框架無法編譯或運行時錯誤。

**影響**:
- 嚴重性：🟠 高（可能導致開發停滯）
- 概率：🟡 中等（SK 會盡量保持向後兼容，但不保證）

**緩解措施**:
1. **抽象層隔離**:
   - 我們的 Agent 層不直接暴露 SK API
   - 通過 Adapter 模式封裝 SK
   - 即使 SK 變動，只需修改 Adapter

2. **版本鎖定**:
   - MVP 期間鎖定 SK 版本（不升級大版本）
   - 只安裝 Patch 版本更新（bug 修復）

3. **測試覆蓋**:
   - 完整的集成測試（測試我們與 SK 的交互）
   - 升級前在測試環境驗證

4. **社群跟蹤**:
   - 訂閱 SK GitHub Releases
   - 參與 SK Discord 社群
   - 提前了解 Breaking Changes

**監控指標**:
- SK 版本更新頻率
- Breaking Changes 數量

**應急預案**:
- 如 SK 發生重大 Breaking Change，評估是否升級
- 必要時 fork SK 代碼庫（最壞情況）

---

##### 🟡 Risk-003: 多語言 SDK 同步問題

**描述**:
Python SDK 和 .NET SDK 需要與 API Gateway 保持同步。如果 API 變更但 SDK 未同步更新，會導致客戶端調用失敗。

**影響**:
- 嚴重性：🟡 中等（影響開發者體驗）
- 概率：🟠 高（人為疏忽容易發生）

**緩解措施**:
1. **OpenAPI 規範自動生成**:
   - API Gateway 使用 Swagger/OpenAPI 自動生成 API 規範
   - SDK 開發參考 OpenAPI 規範

2. **自動化集成測試**:
   - Python SDK 和 .NET SDK 都有集成測試
   - 測試覆蓋所有 API 端點
   - CI/CD 自動運行

3. **版本號同步**:
   - API、Python SDK、.NET SDK 使用相同的版本號（Semantic Versioning）
   - 例如：API 1.2.0 對應 Python SDK 1.2.0 和 .NET SDK 1.2.0

4. **變更通知流程**:
   - API 變更必須通知 SDK 維護者
   - 使用 Pull Request 審查確保同步

**監控指標**:
- SDK 測試失敗率
- API 與 SDK 版本差異

**應急預案**:
- 如發現 SDK 與 API 不同步，立即發布 Hotfix

---

##### 🟡 Risk-004: Vector Database 性能不符合預期

**描述**:
Qdrant 在生產環境的性能可能不符合預期（查詢延遲、並發能力、準確率），導致 Knowledge 查詢性能 KPI 無法達標（P95 < 3 秒）。

**影響**:
- 嚴重性：🟡 中等（影響用戶體驗）
- 概率：🟡 中等（依賴實際數據規模和查詢模式）

**緩解措施**:
1. **早期性能測試**:
   - MVP 開發期間進行性能基準測試
   - 使用真實規模的測試數據（至少 10K 文檔、100K 向量）

2. **索引優化**:
   - 調整 HNSW 參數（m、ef_construct）
   - 使用 Payload 索引加速過濾

3. **緩存策略**:
   - 熱門查詢結果緩存到 Redis
   - TTL 設置合理（例如 5 分鐘）

4. **抽象層設計**:
   - IVectorStore 接口允許切換到其他 Vector DB
   - 如 Qdrant 性能不足，可切換到 Pinecone、Milvus 等

**監控指標**:
- Vector Search 查詢時間（P50/P95/P99）
- Qdrant 資源使用率（CPU/Memory）

**應急預案**:
- 如性能不足，優先優化索引和緩存
- 必要時切換到其他 Vector DB

---

##### 🟡 Risk-005: 團隊技能差距

**描述**:
團隊對某些技術可能不熟悉，例如：
- Qdrant（Rust 編寫，運維經驗不足）
- Kubernetes（運維複雜度高）
- React + TypeScript（前端技能）
- Docker 安全加固（Code Interpreter 安全）

**影響**:
- 嚴重性：🟡 中等（可能延長開發時間）
- 概率：🟠 高（新技術學習需要時間）

**緩解措施**:
1. **培訓計劃**:
   - MVP 開始前提供技術培訓（1-2 週）
   - 涵蓋：Qdrant、Docker、React、安全最佳實踐

2. **結對編程**:
   - 有經驗的開發者帶領新手
   - Code Review 確保代碼質量

3. **詳細文檔**:
   - 提供詳細的開發指南和最佳實踐
   - 包含腳手架和參考實現

4. **外部顧問**（可選）:
   - 如預算允許，聘請外部專家指導（例如 Docker 安全專家）

**監控指標**:
- 開發進度（是否延期）
- Code Review 發現的問題數量

**應急預案**:
- 如某技術成為瓶頸，考慮替換為團隊更熟悉的技術
- 例如：Qdrant → Chroma（如果 Qdrant 運維困難）

---

##### 🟢 Risk-006: 第三方依賴變更或停止維護

**描述**:
依賴的第三方庫（Semantic Kernel、Qdrant、React 等）可能停止維護、引入 Bug、或發生重大變更。

**影響**:
- 嚴重性：🟢 低（大部分依賴都是成熟項目）
- 概率：🟢 低

**緩解措施**:
1. **選擇成熟項目**:
   - 優先選擇活躍維護、社群大的項目
   - 檢查 GitHub Stars、Issues、最近更新時間

2. **依賴版本鎖定**:
   - 使用 lock 文件（.NET: `packages.lock.json`、Python: `requirements.txt` with versions）
   - 不自動升級依賴版本

3. **定期依賴審查**:
   - 每季度審查依賴庫的健康狀況
   - 如發現問題，提前準備替代方案

4. **抽象層設計**:
   - 關鍵依賴通過抽象層封裝
   - 便於替換（例如 IVectorStore 可以切換 Qdrant/Chroma）

**監控指標**:
- 依賴庫更新頻率
- CVE 漏洞數量

**應急預案**:
- 如關鍵依賴停止維護，評估 fork 或替換

---

#### 技術風險總結

| 風險 | 等級 | 影響 | 概率 | 緩解措施完整度 |
|------|------|------|------|---------------|
| Code Interpreter 安全 | 🔴 Critical | 極高 | 中等 | ✅ 完整（多層防護 + 外部審計） |
| SK API 變動 | 🟠 High | 高 | 中等 | ✅ 完整（抽象層 + 版本鎖定） |
| SDK 同步問題 | 🟡 Medium | 中等 | 高 | ✅ 完整（OpenAPI + 自動化測試） |
| Vector DB 性能 | 🟡 Medium | 中等 | 中等 | ✅ 完整（性能測試 + 抽象層） |
| 團隊技能差距 | 🟡 Medium | 中等 | 高 | ✅ 完整（培訓 + 文檔 + 結對） |
| 第三方依賴 | 🟢 Low | 低 | 低 | ✅ 完整（成熟項目 + 鎖定版本） |

**總體風險評估**：✅ **可控**
- 最高風險（Code Interpreter 安全）已有完整緩解措施
- 其他風險都有明確的應對計劃
- 建議：預留 10-15% 的時間緩衝應對技術風險

---

