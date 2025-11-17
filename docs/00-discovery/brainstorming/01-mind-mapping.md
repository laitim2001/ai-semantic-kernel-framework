# Mind Mapping - Microsoft Agent Framework Platform

## 會議資訊
- **日期**: 2025-11-16
- **時間**: Brainstorming Session 1
- **參與者**: Product Owner, AI Assistant (Analyst)
- **方法**: Mind Mapping (思維導圖)

---

## 中心節點：Microsoft Agent Framework Management Platform

一個基於 Microsoft Agent Framework 的企業級 Agent 和工作流編排管理平台

---

## 第一層分支：核心價值定位

### 1. 核心能力關注點 ⭐
**感興趣的功能**（全部都感興趣）：
- ✅ SK 和 AutoGen 原本提供的功能（統一整合）
- ✅ 多 Agent 協作
- ✅ 工作流編排
- ✅ 檢查點和時間旅行
- ✅ 人機協作（Human-in-the-loop）
- ✅ DevUI 可視化開發
- ✅ OpenTelemetry 觀測性
- ✅ 基本上 Agent Framework 有提供的都感興趣

**洞察**: 全功能平台，不是選擇性支持，而是完整利用 Agent Framework 的所有能力

---

### 2. 核心問題解決 🎯
- ✅ 降低 Agent 開發門檻
- ✅ 管理複雜工作流
- ✅ 多團隊協作開發 Agent
- ✅ 企業級 Agent 部署和運維
- ✅ **對接其他平台/工具，為 Agent 增強能力** ← 重要差異點

**洞察**: 不只是管理，更是增強和整合的平台

---

### 3. 與 SK Agent 平台的核心差異 📊
- ✅ 從單 Agent → 多 Agent
- ✅ 從管理 → 編排
- ✅ 從程式碼 → 可視化
- ✅ **Agent 配合工作流可以滿足更多不同的複雜場景** ← 關鍵價值

**洞察**: 平台進化路徑明確：管理 → 編排 → 場景化

---

## 第二層分支：深度探索

### 分支 A：技術能力維度

#### A1. Agent Framework 核心功能整合
讓我們探討具體實現：

**問題組 A1:**
1. **SK 原生功能**：您希望平台如何包裝 SK 的能力？
   - Kernel 初始化
   - Plugin/Function 管理
   - Prompt 模板
   - Memory/Embedding
   - 還是其他？

2. **AutoGen 原生功能**：您希望平台如何包裝 AutoGen 的能力？
   - Agent 角色定義
   - 對話模式（Two-agent, Group chat）
   - Code execution
   - 還是其他？

3. **Agent Framework 新增功能**：最想利用哪些？
   - Graph-based Workflows
   - Streaming capabilities
   - Checkpointing
   - Time-travel debugging
   - 還是全部？

---

#### A2. 多 Agent 協作場景 ⭐⭐⭐ (核心價值驗證)

**✅ 已確認：實際業務場景（全部符合）**

---

### 🔴 場景 A: 客戶服務工單處理 (CS 流程) - MVP 優先

**觸發**: ServiceNow 收到客戶 Ticket

**Agent 工作流**:
```
[Ticket Analyzer Agent]
   ↓ 分類問題類型（退款、技術支持、產品諮詢）
   
[Customer Data Agent] (並行執行)
   ├→ 查詢 Dynamics 365 CE 客戶歷史
   └→ 查詢 Snowflake 訂單和交易記錄
   ↓
   
[Decision Agent] - 智能路由
   ├→ 簡單問題 → 自動回覆
   └→ 複雜問題 → 轉專家 Agent
   ↓
   
[Solution Generator Agent]
   ├→ 搜尋 SharePoint 知識庫
   └→ 生成解決方案
   ↓
   
[Human-in-the-loop] - CS 人員審核
   ↓ (批准/修改)
   
[Action Agent] (並行執行)
   ├→ 更新 ServiceNow Ticket
   ├→ 發送 Teams/Outlook 通知
   └→ 記錄到 Dynamics 365
```

**關鍵技術需求**:
- ServiceNow REST API 整合
- Dynamics 365 Web API
- Snowflake 數據庫連接器
- SharePoint Graph API (知識庫搜尋)
- Human-in-the-loop 審批機制
- Multi-system 並行寫入

**價值**:
- 減少 CS 人員重複查詢時間 70%
- 提升回應速度
- 知識庫自動化應用

---

### 🔴 場景 D: IT 運維自動化 - MVP 優先

**觸發**: 系統告警 / ServiceNow Incident

**Agent 工作流**:
```
[Alert Analyzer Agent]
   ├→ 解析告警信息
   └→ 查詢 ServiceNow 歷史 Incident (相似問題)
   ↓
   
[Diagnostic Agent] (並行執行)
   ├→ 查詢數據庫 (MSSQL/PostgreSQL) 系統狀態
   └→ 檢查 SharePoint 日誌
   ↓
   
[Solution Finder Agent]
   ├→ 搜尋 SharePoint Runbook
   └→ 查找類似問題的解決方案
   ↓
   
[Decision Agent]
   ├→ 簡單問題 → 自動修復
   └→ 複雜問題 → 升級人工
   ↓
   
[Human-in-the-loop] - IT 工程師確認 (複雜問題)
   ↓
   
[Action Agent] (並行執行)
   ├→ 執行修復腳本
   ├→ 更新 ServiceNow Incident (狀態 + 解決方案)
   └→ 記錄到 SharePoint 知識庫 (新的解決方案)
```

**關鍵技術需求**:
- ServiceNow Event/Incident API
- 數據庫監控查詢 (MSSQL, PostgreSQL)
- SharePoint 日誌訪問
- 腳本執行能力 (Python/PowerShell)
- 自動化 Runbook 執行
- 知識庫自動更新

**價值**:
- 24/7 自動化監控和響應
- 減少 MTTR (Mean Time To Repair)
- 知識庫持續積累

---

### 🟡 場景 B: Sales 流程自動化 - Phase 2

**觸發**: 業務人員在 Teams 中提問 "幫我準備 XXX 公司的提案"

**Agent 工作流**:
```
[Research Agent] (並行執行)
   ├→ 查詢 Dynamics 365 CE 客戶資料
   ├→ 查詢 Snowflake 歷史交易數據
   └→ 搜尋 SharePoint 過往提案
   ↓
   
[Market Analysis Agent]
   ├→ 分析客戶行業和需求
   └→ 比對產品適配性
   ↓
   
[Proposal Generator Agent]
   ├→ 生成提案大綱
   └→ 選擇產品和定價策略
   ↓
   
[Pricing Agent]
   ├→ 查詢 SAP / Dynamics 365 FO 定價規則
   └→ 計算折扣和最終報價
   ↓
   
[Human-in-the-loop] - Sales 審核
   ↓ (確認/調整)
   
[Documentation Agent] (並行執行)
   ├→ 生成提案文檔 (Word/PDF)
   ├→ 儲存到 SharePoint
   ├→ 創建 Dynamics 365 Opportunity
   └→ 發送 Outlook Email 給客戶
```

**關鍵技術需求**:
- Teams Bot 整合 (對話式觸發)
- Dynamics 365 CE + FO API
- SAP API 整合
- Snowflake 分析查詢
- 文檔生成 (Word/PDF)
- SharePoint 文檔管理
- Outlook Graph API

**價值**:
- 提案準備時間從 2-3 天縮短到 1 小時
- 數據驅動的定價決策
- 提案質量一致性

---

### 🟡 場景 C: Finance 報表生成和異常檢測 - Phase 2

**觸發**: 每月 1 號自動執行 / 財務人員手動觸發

**Agent 工作流**:
```
[Data Collector Agent] (並行執行)
   ├→ 查詢 SAP 財務數據
   ├→ 查詢 Dynamics 365 FO 數據
   └→ 查詢 Snowflake 數據倉庫
   ↓
   
[Data Validator Agent]
   ├→ 檢查數據完整性
   └→ 發現異常項 (金額異常、缺失記錄)
   ↓
   
[Analysis Agent] (並行執行)
   ├→ 計算財務指標 (收入、成本、利潤)
   └→ 生成趨勢分析
   ↓
   
[Anomaly Detection Agent]
   ├→ 識別異常交易 (規則 + ML)
   └→ 標記需要人工檢查的項目
   ↓
   
[Human-in-the-loop] - 財務審核
   ↓ (確認異常項)
   
[Report Generator Agent] (並行執行)
   ├→ 生成財務報表 (Excel/PDF)
   ├→ 儲存到 SharePoint
   ├→ 創建 ServiceNow Task (異常項跟進)
   └→ Teams 通知相關人員
```

**關鍵技術需求**:
- SAP API 整合
- Dynamics 365 FO API
- Snowflake 大數據查詢
- 異常檢測演算法
- 排程執行 (Cron/Azure Functions)
- Excel/PDF 生成
- Multi-channel 通知

**價值**:
- 月結時間縮短 50%
- 自動化異常檢測
- 降低人工錯誤

---

### 📊 場景優先級總結

| 場景 | 優先級 | MVP 階段 | 複雜度 | 業務價值 | 技術挑戰 |
|------|--------|----------|--------|----------|----------|
| **A: CS 工單處理** | 🔴 最高 | ✅ MVP | 中 | 高（日常高頻） | 中 |
| **D: IT 運維** | 🔴 最高 | ✅ MVP | 中-高 | 高（降低 MTTR） | 中-高 |
| **B: Sales 流程** | 🟡 中 | Phase 2 | 高 | 高（商業價值大） | 高 |
| **C: Finance 報表** | 🟡 中 | Phase 2 | 高 | 中（月度執行） | 高 |

**MVP 策略建議**:
1. **優先實現場景 A + D**（3-4 個月）
2. 建立核心能力：
   - ServiceNow 整合
   - Dynamics 365 整合
   - SharePoint 知識庫
   - 數據庫連接器
   - Human-in-the-loop 機制
3. Phase 2 擴展到 B + C（需要更複雜的 SAP 整合和文檔生成）

**共同技術需求**（跨場景復用）:
- ✅ ServiceNow API (場景 A, D, C)
- ✅ SharePoint Graph API (場景 A, D, B, C)
- ✅ Dynamics 365 API (場景 A, B, C)
- ✅ Snowflake Connector (場景 A, B, C)
- ✅ Database Connectors - MSSQL, PostgreSQL (場景 D)
- ✅ Human-in-the-loop Framework (全部場景)
- ✅ Multi-system Orchestration (全部場景)

---

#### A3. 工作流編排需求
**問題組 A3:**
1. **工作流複雜度**：您預期的工作流有多複雜？
   - 簡單：線性流程（A → B → C）
   - 中等：條件分支（if-else, switch）
   - 複雜：循環、並行、子流程、錯誤處理

2. **工作流設計方式**：
   - 純程式碼定義（YAML/JSON）
   - 可視化拖拽設計（類似 n8n, Langflow）
   - 混合模式（可視化 + 程式碼）

3. **工作流執行模式**：
   - 同步執行（等待完成）
   - 異步執行（背景執行）
   - 排程執行（定時觸發）
   - 事件驅動執行（Webhook 觸發）

---

#### A4. 檢查點和時間旅行
**問題組 A4:**
1. **使用場景**：
   - 調試失敗的 Agent 執行？
   - 回溯決策過程？
   - 從中間狀態恢復執行？
   - 對比不同執行路徑？

2. **儲存策略**：
   - 每一步都儲存？
   - 關鍵節點儲存？
   - 用戶手動設置檢查點？

---

#### A5. 人機協作（Human-in-the-loop）
**問題組 A5:**
1. **介入點**：
   - Agent 決策前請求批准？
   - 工作流特定節點暫停等待輸入？
   - 錯誤發生時人工介入？
   - 質量檢查點？

2. **通知機制**：
   - Web UI 通知？
   - Email/Slack 通知？
   - Webhook 整合？

---

### 分支 B：平台功能維度

#### B1. 降低開發門檻
**問題組 B1:**
1. **目標用戶**：
   - 開發者（會寫程式碼）
   - 業務人員（不會寫程式碼）
   - 混合團隊

2. **簡化方式**：
   - Agent 模板庫（預設 Agent）
   - 工作流模板庫（預設流程）
   - 拖拽式 UI Builder
   - 自然語言生成 Agent？

---

#### B2. 企業級部署和運維
**問題組 B2:**
1. **多環境支持**：
   - 開發 → 測試 → 生產環境隔離？
   - 不同團隊/部門隔離（Multi-tenancy）？

2. **監控和告警**：
   - Agent 執行成功率
   - 工作流執行時間
   - Token 使用量
   - 錯誤率和異常告警

3. **安全和權限**：
   - Agent 訪問控制（誰可以調用/修改）
   - 工作流權限管理
   - API Key 管理
   - 審計日誌

---

#### B3. 對接其他平台/工具 🔧 ⭐⭐⭐ (核心差異化能力)
**問題組 B3:**（這是一個重要差異點）

**✅ 已確認：公司/團隊目前使用的系統/工具**
- **辦公協作**: Microsoft Teams, Outlook
- **數據存儲**: SharePoint, OneDrive
- **開發工具**: Visual Studio, GitHub Copilot, Claude Code
- **業務系統**: Windows 11, Dynamics 365 (CE - Customer Engagement, FO - Finance & Operations), SAP, ServiceNow

**✅ 已確認：最希望 Agent 能訪問的系統/工具（Top 3-5）**
1. ✅ **SharePoint / OneDrive** - 讀取文檔作為知識庫
2. ✅ **ServiceNow** - 自動創建 Ticket
3. ✅ **多數據庫支持** - 查詢 MSSQL, PostgreSQL, Snowflake

**✅ 已確認："為 Agent 增強能力"的具體含義**
1. ✅ **動態添加 Tools/Functions**
   - 自定義 Tools/Functions
   - 針對不同場景或需求靈活擴展
   
2. ✅ **連接企業私有數據**
   - 例如：數據倉庫 (Snowflake)
   - 企業內部數據源
   
3. ✅ **執行特定業務邏輯**
   - Sales 流程自動化
   - Customer Service (CS) 流程
   - Finance 流程
   - 其他業務流程
   
4. ✅ **對接能力（最重要）**
   - 主要訴求：具備能力對接不同系統/工具
   - 不限於特定系統，而是通用的對接能力

**🔍 洞察和延伸思考**：

**典型企業 IT 架構**：
```
Microsoft 生態系核心：
├── 協作層：Teams, Outlook, SharePoint, OneDrive
├── 業務層：Dynamics 365 (CRM + ERP), SAP
├── 服務管理：ServiceNow
└── 數據層：MSSQL, PostgreSQL, Snowflake
```

**Agent 使用場景示例**：
- **Sales Agent**: 
  - 讀取 SharePoint 產品文檔 → 查詢 Dynamics 365 客戶資料 → 生成報價 → 創建 ServiceNow 任務
  
- **CS Agent**:
  - 接收 ServiceNow Ticket → 查詢 Snowflake 訂單記錄 → 生成解決方案 → 更新 Ticket → 通知 Teams
  
- **Finance Agent**:
  - 查詢 SAP 財務數據 → 分析 Snowflake 數據倉庫 → 生成財報 → 儲存至 SharePoint

**技術實施考量**：
1. **Microsoft Graph API** - 統一訪問 Teams, Outlook, SharePoint, OneDrive
2. **Dynamics 365 Web API** - CRM/ERP 數據訪問
3. **ServiceNow REST API** - Ticket 管理
4. **Multi-DB Connector** - MSSQL, PostgreSQL, Snowflake 抽象層
5. **Custom Tool Framework** - 動態註冊和載入自定義 Tools

**對接方式**：
- ✅ REST API 整合（主要）
- ✅ OAuth 2.0 認證（Microsoft, ServiceNow）
- ✅ SDK/Plugin 開發（自定義業務邏輯）
- ✅ MCP (Model Context Protocol) 支持（未來擴展）
- ✅ Webhook 訂閱（事件驅動場景）

---

### 分支 C：使用者體驗維度

#### C1. DevUI 可視化開發
**問題組 C1:**
1. **優先使用場景**：
   - Agent 設計和測試
   - 工作流設計和調試
   - 執行結果可視化
   - 效能分析

2. **與 Agent Framework 原生 DevUI 的關係**：
   - 直接使用原生 DevUI
   - 在原生基礎上擴展
   - 完全自建 UI

---

#### C2. 多團隊協作
**問題組 C2:**
1. **協作場景**：
   - Agent 共享和重用
   - 工作流協同開發
   - 版本控制和分支管理
   - Code Review 流程

2. **團隊結構**：
   - 集中式（一個團隊管理所有 Agent）
   - 分散式（各團隊管理自己的 Agent）
   - 混合式

---

## 第三層分支：差異化和創新

### 競爭對手分析
**問題組 D:**
1. 市場上有類似產品嗎？
   - Langflow / Flowise（開源工作流工具）
   - Dify.ai（LLM 應用開發平台）
   - n8n（工作流自動化）
   - 企業內部工具

2. 我們的差異化在哪？
   - 基於最新的 Microsoft Agent Framework
   - 完整整合 SK + AutoGen 能力
   - 企業級功能（權限、多租戶、審計）
   - 對接能力強（與企業系統整合）
   - 其他？

---

## 第四層分支：實施策略

### 🎯 MVP 定義 (已確認)

---

#### E1. MVP 必須包含的核心功能

**✅ 確認必須功能**：

1. **Agent 管理** ⭐⭐⭐
   - ✅ Agent 建立、編輯、刪除
   - ✅ Agent 配置（System Prompt, LLM 選擇, Tools）
   - ✅ Agent 測試介面

2. **工作流編排** ⭐⭐⭐⭐⭐ (核心能力，技術選型關鍵)
   - ✅ 可視化工作流設計器（拖拽式）
   - ✅ 程式碼定義工作流（YAML/JSON）
   - ✅ 條件分支、並行執行、錯誤處理
   - **🔴 關鍵問題**: Agent Framework 有提供嗎？還是使用 n8n？
   - **🔴 不建議自行開發**（用戶明確表示太複雜）

3. **ServiceNow 整合** ⭐⭐⭐
   - ✅ Incident CRUD
   - ✅ Incident 監控
   - ✅ 自動更新狀態

4. **SharePoint 整合** ⭐⭐⭐
   - ✅ 文檔搜尋（知識庫）
   - ✅ 文檔讀取
   - ✅ 文檔寫入（日誌、報告）

5. **Dynamics 365 整合** ⭐⭐⭐
   - ✅ CE (Customer Engagement) - 客戶資料查詢
   - ✅ 記錄寫入

6. **數據庫連接器** ⭐⭐⭐
   - ✅ MSSQL, PostgreSQL 查詢
   - ✅ Snowflake 查詢

7. **Human-in-the-loop** ⭐⭐⭐
   - ✅ 工作流暫停等待批准
   - ✅ Web UI 審批介面
   - ✅ Teams 通知

8. **執行歷史和監控** ⭐⭐⭐
   - ✅ 工作流執行日誌
   - ✅ Agent 對話歷史
   - ✅ 錯誤追蹤

9. **REST API** ⭐⭐⭐
   - ✅ 觸發工作流 API
   - ✅ 查詢執行狀態 API

10. **Web UI 管理介面** ⭐⭐⭐
    - ✅ Agent 管理頁面
    - ✅ 工作流管理頁面
    - ✅ 執行監控 Dashboard
    - **🔴 關鍵問題**: Agent Framework 有提供嗎？還是使用 n8n？

---

#### E2. 可以延後到 Phase 2 的功能

**✅ 確認可延後**：
- ✅ Teams Bot 整合（對話式觸發）
- ✅ SAP 整合
- ✅ 文檔生成（Word/PDF）
- ✅ Outlook Email 發送
- ✅ 排程執行（Cron Jobs）
- ✅ DevUI 整合（Agent Framework 原生 DevUI）
- ✅ 檢查點和時間旅行
- ✅ 多租戶和權限管理
- ✅ 進階監控（OpenTelemetry Dashboard）
- ✅ 其他 Agent Framework 原生提供的功能

---

#### E3. 技術和資源限制

**✅ 已確認**：

**時間**:
- ✅ MVP 開發週期：**3 個月**

**團隊**:
- ✅ 開發團隊規模：**6 人**
- ✅ 技能組成：**全端工程師**

**技術風險容忍度**:
- ✅ **高風險容忍**：先用 Agent Framework，遇到問題再解決

**部署環境**:
- ✅ **本地開發（On-premise）** + **Azure 部署生產**

---

#### E4. 關鍵技術決策

**✅ 已確認**：

**後端框架**:
- ✅ **兩者都用（Microservices）**
  - Python + .NET 混合架構
  - Agent Framework 支持兩種語言

**前端框架**:
- ✅ **React + TypeScript**
- 🔴 **關鍵問題**: 使用 Agent Framework DevUI 的話有什麼好處？而且 DevUI 可以客製化嗎？

**工作流設計方式** (MVP 階段):
- ✅ **混合：先程式碼，後續加可視化**
- 🔴 **重要原則**: 不建議自行開發工作流設計介面
- 🔴 **技術選型**: 盡量使用其他工具或平台（例如 n8n 或 Agent Framework 提供的）

**資料庫選擇**:
- ✅ **PostgreSQL**（推薦）

---

## 🔴 核心技術決策問題（需要立即解決）

### 問題 1: 工作流編排技術選型 ⭐⭐⭐⭐⭐

**用戶訴求**：
- ✅ 需要可視化工作流設計器（拖拽式）
- ✅ 需要程式碼定義工作流（YAML/JSON）
- ✅ 需要條件分支、並行執行、錯誤處理
- ❌ **不建議自行開發**（太複雜）

**技術選項分析**：

#### 選項 A: Microsoft Agent Framework Workflows ⭐⭐⭐⭐

**有的能力**（官方文檔確認）：
- ✅ Graph-based 工作流架構
- ✅ 程式碼定義（Python/C# 程式碼）
- ✅ Type-safe message flow
- ✅ 條件分支、並行執行
- ✅ Checkpointing（狀態保存）
- ✅ Human-in-the-loop 支持
- ✅ Multi-Agent 編排模式（sequential, concurrent, hand-off）
- ✅ External API 整合

**沒有的能力**：
- ❌ **沒有可視化工作流設計器**（目前是純程式碼）
- ❌ DevUI 主要用於 Agent 測試和調試，**不是工作流設計工具**

**結論**：Agent Framework Workflows 是**底層編排引擎**，但缺少可視化設計 UI。

---

#### 選項 B: n8n (開源工作流工具) ⭐⭐⭐⭐⭐

**有的能力**：
- ✅ **強大的可視化工作流設計器**（拖拽式）
- ✅ JSON 工作流定義
- ✅ 條件分支、並行執行、錯誤處理
- ✅ 400+ 預建整合（包括 ServiceNow, HTTP, Database, Webhook）
- ✅ Human-in-the-loop（手動批准節點）
- ✅ 自託管（符合本地部署需求）
- ✅ REST API（可以從外部觸發）
- ✅ 可以執行自定義程式碼（Python, JavaScript）

**沒有的能力**：
- ❌ 不原生支持 Agent Framework（需要整合）
- ❌ Agent 對話歷史需要自行記錄

**結論**：n8n 是**成熟的工作流平台**，可視化能力強，但需要與 Agent Framework 整合。

---

#### 選項 C: 混合架構（推薦） ⭐⭐⭐⭐⭐

**架構設計**：
```
┌─────────────────────────────────────────────────┐
│          Web UI (React + TypeScript)            │
│  - Agent 管理                                   │
│  - 執行監控 Dashboard                           │
│  - Human-in-the-loop 審批介面                   │
└─────────────────────────────────────────────────┘
                        ↓ REST API
┌─────────────────────────────────────────────────┐
│      Backend Orchestration Layer (Python)       │
│  - Agent Framework Runtime                      │
│  - Agent 執行引擎                               │
│  - 系統整合 (ServiceNow, D365, SharePoint)     │
└─────────────────────────────────────────────────┘
                        ↓ HTTP API / Webhook
┌─────────────────────────────────────────────────┐
│              n8n (Workflow Engine)              │
│  - 可視化工作流設計器                           │
│  - 工作流執行引擎                               │
│  - 觸發 Backend API 執行 Agent                  │
└─────────────────────────────────────────────────┘
```

**工作流程**：
1. 用戶在 n8n 設計工作流（拖拽式）
2. n8n 節點調用 Backend API：`POST /api/agents/{name}/execute`
3. Backend 使用 Agent Framework 執行 Agent
4. 結果返回 n8n，繼續下一個節點
5. Human-in-the-loop：n8n 暫停 → Web UI 審批 → n8n 恢復

**優勢**：
- ✅ **可視化工作流設計**（n8n 提供）
- ✅ **Agent 編排能力**（Agent Framework 提供）
- ✅ **不用自行開發工作流 UI**（符合用戶要求）
- ✅ **快速開發**（利用成熟工具）
- ✅ **靈活擴展**（可以整合任何系統）

**劣勢**：
- ⚠️ 多一層架構複雜度
- ⚠️ n8n 和 Agent Framework 兩套系統要維護

---

### 問題 2: Agent Framework DevUI 的價值 ⭐⭐⭐

**DevUI 是什麼**：
- 🎯 **開發者工具**：用於 Agent 開發、測試、調試
- 🎯 **可視化對話流**：查看 Agent 對話歷史
- 🎯 **實時監控**：Agent 執行狀態、工具調用

**DevUI 的好處**：
- ✅ 快速測試 Agent（不用寫程式碼觸發）
- ✅ 可視化 Agent 內部運作（工具調用、思考過程）
- ✅ 調試錯誤（查看錯誤堆疊）

**DevUI 可以客製化嗎**：
- ⚠️ **有限**：DevUI 是 Agent Framework 的內建工具
- ⚠️ 主要用於**開發階段**，不適合作為**生產環境 UI**
- ⚠️ 如果需要企業級 UI（權限、多用戶、Dashboard），需要自行開發

**建議**：
- ✅ 開發階段：使用 DevUI 快速測試
- ✅ 生產環境：自建 React + TypeScript UI

---

## 📊 技術選型建議（基於分析）

### 🏆 推薦方案：混合架構

```yaml
架構組成:
  工作流設計:
    工具: n8n (Self-hosted)
    原因: 
      - 強大的可視化設計器
      - 400+ 預建整合
      - 不用自行開發
      - 符合用戶要求
  
  Agent 執行:
    框架: Microsoft Agent Framework (Python + .NET)
    原因:
      - 原生 Agent 支持
      - Multi-Agent 編排
      - Checkpointing
      - Human-in-the-loop
  
  系統整合:
    層: Backend Orchestration Layer (Python FastAPI)
    負責:
      - Agent Framework Runtime
      - ServiceNow, SharePoint, Dynamics 365 整合
      - 數據庫連接器
      - REST API 提供給 n8n 調用
  
  UI 介面:
    框架: React + TypeScript
    負責:
      - Agent 管理
      - 執行監控 Dashboard
      - Human-in-the-loop 審批介面
      - 不包含工作流設計（由 n8n 提供）
  
  數據庫:
    主資料庫: PostgreSQL
    用途:
      - Agent 配置
      - 執行歷史
      - 審批記錄
```

### 技術棧總結

| 層級 | 技術 | 用途 |
|------|------|------|
| **工作流設計** | n8n | 可視化工作流設計器 |
| **Agent 引擎** | Agent Framework (Python) | Agent 執行、編排 |
| **系統整合** | Python FastAPI | REST API、系統連接器 |
| **前端 UI** | React + TypeScript | Agent 管理、監控 |
| **資料庫** | PostgreSQL | 數據持久化 |

---

## ✅ 關鍵決策已確認

### Q1: 混合架構決策
**✅ 確認接受混合架構**
- n8n 負責工作流設計和執行
- Agent Framework 負責 Agent 執行和編排
- 自建 React UI 負責管理介面

### Q2: n8n 團隊熟悉度
**✅ 確認：團隊很熟悉，已經在用**
- 技術風險：**低** ✅
- 學習曲線：**無** ✅
- 可以立即開始開發

### Q3: 替代方案
**✅ 確認：沒有替代方案，就用 n8n**
- 技術選型已鎖定
- 可以進入架構設計階段

### DevUI 定位確認
**✅ 確認**：
- 開發階段：使用 Agent Framework DevUI 快速測試
- 生產環境：使用自建 React + TypeScript UI

---

## 🎉 Mind Mapping 總結

### 核心架構決策（已鎖定）

```
┌─────────────────────────────────────────────────────────────┐
│                    用戶介面層 (User Interface)               │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────┐          ┌─────────────────┐           │
│  │   n8n Web UI    │          │  React Web UI   │           │
│  │  工作流設計器    │          │   Agent 管理    │           │
│  │  (已有，復用)    │          │   執行監控      │           │
│  └─────────────────┘          │   審批介面      │           │
│                                └─────────────────┘           │
└─────────────────────────────────────────────────────────────┘
                           ↓ REST API
┌─────────────────────────────────────────────────────────────┐
│                   編排引擎層 (Orchestration)                 │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌────────────────────────────────────────────────────┐     │
│  │              n8n Workflow Engine                   │     │
│  │  - 可視化工作流執行                                │     │
│  │  - 條件分支、並行、錯誤處理                       │     │
│  │  - Human-in-the-loop (手動批准節點)                │     │
│  │  - 定時觸發、Webhook 觸發                          │     │
│  │  - 調用 Backend API 執行 Agent                     │     │
│  └────────────────────────────────────────────────────┘     │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                           ↓ HTTP API
┌─────────────────────────────────────────────────────────────┐
│                  Agent 執行層 (Agent Runtime)                │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌────────────────────────────────────────────────────┐     │
│  │    Backend Orchestration (Python FastAPI)          │     │
│  │                                                      │     │
│  │  ┌──────────────────────────────────────────────┐  │     │
│  │  │   Microsoft Agent Framework Runtime          │  │     │
│  │  │   - Agent 執行引擎                           │  │     │
│  │  │   - Multi-Agent 協作                         │  │     │
│  │  │   - Checkpointing                            │  │     │
│  │  │   - Human-in-the-loop 狀態管理               │  │     │
│  │  └──────────────────────────────────────────────┘  │     │
│  │                                                      │     │
│  │  REST API:                                           │     │
│  │  - POST /api/agents/{name}/execute                  │     │
│  │  - POST /api/agents/{name}/approve                  │     │
│  │  - GET  /api/executions/{id}                        │     │
│  └────────────────────────────────────────────────────┘     │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│                  系統整合層 (Integration)                    │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  ServiceNow  │  │  SharePoint  │  │ Dynamics 365 │      │
│  │  Connector   │  │  Connector   │  │  Connector   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │    MSSQL     │  │ PostgreSQL   │  │  Snowflake   │      │
│  │  Connector   │  │  Connector   │  │  Connector   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│                    數據層 (Data Storage)                     │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌────────────────────────────────────────────────────┐     │
│  │           PostgreSQL (主資料庫)                    │     │
│  │  - Agent 配置                                      │     │
│  │  - 工作流定義 (metadata)                           │     │
│  │  - 執行歷史                                        │     │
│  │  - 審批記錄                                        │     │
│  │  - 系統設定                                        │     │
│  └────────────────────────────────────────────────────┘     │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔑 核心技術棧（最終確認）

| 層級 | 技術 | 版本 | 用途 | 團隊熟悉度 |
|------|------|------|------|-----------|
| **工作流引擎** | n8n | Latest | 可視化工作流設計和執行 | ✅ 很熟悉 |
| **Agent 框架** | Microsoft Agent Framework | Preview | Agent 執行和編排 | 🆕 新技術 |
| **Backend API** | Python FastAPI | 0.100+ | REST API 和系統整合 | ✅ 熟悉 |
| **Frontend UI** | React + TypeScript | 18+ | Agent 管理和監控介面 | ✅ 熟悉 |
| **資料庫** | PostgreSQL | 16+ | 數據持久化 | ✅ 熟悉 |
| **LLM 服務** | Azure OpenAI | Latest | GPT-4o, GPT-4o-mini | ✅ 熟悉 |

---

## 💡 關鍵設計決策記錄

### 決策 1: 為什麼選擇混合架構？

**問題**：工作流編排太複雜，不想自行開發

**解決方案**：n8n (工作流) + Agent Framework (Agent 執行)

**優勢**：
- ✅ n8n 提供成熟的可視化設計器（團隊已熟悉）
- ✅ Agent Framework 提供強大的 Agent 能力
- ✅ 通過 REST API 靈活整合
- ✅ 各自專注核心能力，降低開發複雜度
- ✅ 快速 MVP（3 個月可行）

**風險**：
- ⚠️ 多一層架構，需要維護兩套系統
- ⚠️ n8n 和 Agent Framework 的狀態同步
- ✅ **緩解措施**：PostgreSQL 作為共享數據層

---

### 決策 2: 為什麼選擇 Python FastAPI？

**原因**：
- ✅ Agent Framework 的 Python SDK 更成熟
- ✅ 團隊熟悉 Python
- ✅ FastAPI 性能優異，適合高頻 API 調用
- ✅ 異步支持（async/await）適合 Agent 長時間執行

---

### 決策 3: 為什麼自建 React UI？

**原因**：
- ✅ n8n UI 負責工作流設計（已有）
- ✅ 需要自定義的 Agent 管理介面
- ✅ 需要自定義的執行監控 Dashboard
- ✅ 需要自定義的 Human-in-the-loop 審批介面
- ❌ Agent Framework DevUI 只是開發工具，不適合生產

---

## 📋 MVP 功能清單（最終版）

### Phase 1: MVP (3 個月) - 場景 A + D

#### 核心功能模組

**模組 1: Agent 管理** (Week 1-2)
- Agent CRUD API
- Agent 配置管理（System Prompt, LLM, Tools）
- Agent 測試介面（使用 DevUI 開發，React UI 生產）
- PostgreSQL 儲存

**模組 2: 系統整合層** (Week 2-4)
- ServiceNow Connector (Incident CRUD, 監控)
- SharePoint Connector (文檔搜尋、讀取、寫入)
- Dynamics 365 CE Connector (客戶資料查詢、記錄寫入)
- Database Connectors (MSSQL, PostgreSQL, Snowflake)
- Microsoft Graph API 認證

**模組 3: n8n 整合** (Week 3-4)
- Backend REST API 設計
  - `POST /api/agents/{name}/execute` - 執行 Agent
  - `POST /api/agents/{name}/approve` - 審批 Human-in-the-loop
  - `GET /api/executions/{id}` - 查詢執行狀態
- n8n Custom Node 開發（可選，簡化調用）

**模組 4: Human-in-the-loop** (Week 4-5)
- 工作流暫停機制（n8n 手動批准節點）
- Backend 狀態管理（PENDING_APPROVAL → APPROVED/REJECTED）
- React UI 審批介面
- Teams 通知（Webhook）

**模組 5: 執行歷史和監控** (Week 5-6)
- 執行日誌記錄（PostgreSQL）
- Agent 對話歷史
- 錯誤追蹤和告警
- React Dashboard 展示

**模組 6: 場景實現** (Week 7-12)
- Week 7-9: 場景 A (CS 工單處理) 完整實現
- Week 10-12: 場景 D (IT 運維自動化) 完整實現

---

### Phase 2: 功能擴展 (3-6 個月後)
- Teams Bot 整合
- SAP 整合
- 場景 B (Sales 流程)
- 場景 C (Finance 報表)
- 排程執行
- 檢查點和時間旅行
- 多租戶和權限管理
- 進階監控

---

## 🎯 Mind Mapping 完成總結

### 已確認的核心內容

1. **✅ 平台定位**：Microsoft Agent Framework + n8n 混合架構的企業級 Agent 編排管理平台

2. **✅ 核心能力**：
   - Agent 管理和執行
   - 多 Agent 協作（Agent Framework）
   - 可視化工作流編排（n8n）
   - 企業系統整合（微軟生態系為主）
   - Human-in-the-loop

3. **✅ 目標場景**（MVP）：
   - 場景 A: CS 工單處理（ServiceNow + Dynamics 365 + SharePoint）
   - 場景 D: IT 運維自動化（ServiceNow + Database + SharePoint）

4. **✅ 技術棧**：
   - 工作流：n8n（團隊已熟悉）
   - Agent 引擎：Microsoft Agent Framework (Python)
   - Backend: Python FastAPI
   - Frontend: React + TypeScript
   - Database: PostgreSQL
   - LLM: Azure OpenAI

5. **✅ 團隊和時間**：
   - 6 人全端工程師
   - 3 個月 MVP
   - 高風險容忍度
   - 本地開發 + Azure 生產

6. **✅ 差異化價值**：
   - 基於最新 Agent Framework
   - 強大的企業系統整合能力
   - 混合架構（n8n + Agent Framework）
   - 微軟生態系深度整合

---

## 🚀 下一步：SCAMPER 分析

Mind Mapping 已完成！現在我們已經有了清晰的：
- ✅ 平台核心定位
- ✅ 技術架構決策
- ✅ MVP 功能範圍
- ✅ 實際業務場景

接下來使用 **SCAMPER 技術**進行創新思考，探索：
- 如何改進現有功能
- 如何結合其他技術
- 如何簡化複雜度
- 如何創造獨特價值

準備好繼續嗎？ 🎯

---

## 待回答問題清單

請選擇您想先討論的問題組（可以跳著回答）：

### 🔴 高優先級（建議先回答）
- **問題組 A2**: 實際的多 Agent 協作場景（2-3 個例子）
- **問題組 B3**: 對接其他平台/工具的具體需求
- **問題組 E**: MVP 範圍定義

### 🟡 中優先級
- **問題組 A3**: 工作流編排需求
- **問題組 A5**: 人機協作場景
- **問題組 B2**: 企業級部署和運維需求

### 🟢 低優先級（可以後續討論）
- **問題組 A1**: SK/AutoGen 功能包裝細節
- **問題組 A4**: 檢查點和時間旅行
- **問題組 C1, C2**: DevUI 和多團隊協作細節
- **問題組 D**: 競爭對手分析

---

## 下一步

請選擇您想先回答的問題組，我們逐步建立完整的思維導圖！您也可以：
1. 補充新的想法或分支
2. 質疑或修正我的理解
3. 提出我沒想到的維度

🎯 **建議**: 先回答問題組 A2（實際場景）和 B3（對接需求），這樣我們可以更具體地定義平台價值。
