# 模組 01: Agent 管理系統

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 01
**User Stories**: US 1.1-1.6
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 1: Agent 管理系統

#### Epic 1: Agent 生命週期管理

**目標**: 提供 UI 和 SDK 雙軌道的 Agent 建立和管理能力

---

#### US 1.1 - 通過 Web UI 建立 Agent

**作為** IT 開發者或業務分析師
**我想要** 通過 Web UI 快速建立新的 Agent
**以便** 無需寫代碼即可部署 AI 助手

**驗收標準**:

✅ 必須項:
- [ ] 填寫 Agent 基本資訊（名稱、描述、角色類型）
- [ ] 從下拉選單選擇 LLM 模型（GPT-4, GPT-4o, GPT-4o-mini）
- [ ] 使用文字編輯器設定 System Prompt（支援模板）
- [ ] 設定模型參數（temperature: 0-2, max_tokens: 1-4096, top_p: 0-1）
- [ ] 建立操作 <5 秒完成，返回 Agent ID
- [ ] 自動驗證必填欄位和參數範圍
- [ ] 提供預覽功能（即時測試對話）

**技術要求**:
- React + TypeScript 前端
- 表單驗證（Yup/Zod）
- 響應式設計（RWD）

**📊 優先級**: P0 (MVP 必須)
**🎯 驗收方式**: 開發者可在 2 分鐘內建立可用的 Agent
**🔗 相關**: US 1.2 (SDK), US 1.5 (Persona)

---

#### US 1.2 - 通過 .NET SDK 建立 Agent

**作為** .NET 開發者
**我想要** 使用 C# SDK 程式化建立 Agent
**以便** 在 CI/CD 流程中自動化部署

**驗收標準**:

✅ 必須項:
- [ ] 提供 NuGet 套件安裝（`Install-Package SemanticKernel.Agentic`）
- [ ] Fluent API 設計（builder pattern）
- [ ] 支援強類型配置（避免 magic string）
- [ ] 完整的 XML 文檔註解（IntelliSense 支援）
- [ ] 提供完整範例代碼和教學文檔
- [ ] 支援異步操作（async/await）
- [ ] 錯誤處理和友好異常訊息

**範例代碼**:
```csharp
using SemanticKernel.Agentic;

var agentClient = new AgentClient(apiKey, endpoint);

var agent = await agentClient.CreateAsync(builder => builder
    .WithName("CustomerServiceAgent")
    .WithDescription("24/7 customer service assistant")
    .WithRole(AgentRole.CustomerService)
    .WithModel(LLMModel.GPT4o)
    .WithSystemPrompt("You are a helpful customer service agent...")
    .WithTemperature(0.7)
    .WithMaxTokens(2000)
    .WithPlugins(plugin => plugin
        .Add<FileSearchPlugin>()
        .Add<CodeInterpreterPlugin>()
    )
    .Build()
);

Console.WriteLine($"Agent created: {agent.Id}");

// 執行 Agent
var response = await agentClient.InvokeAsync(
    agentId: agent.Id,
    message: "How can I track my order?",
    sessionId: "session-123"
);

Console.WriteLine(response.Content);
```

**技術要求**:
- .NET 8 SDK
- System.Text.Json 序列化
- HttpClient 整合

**📊 優先級**: P0 (MVP 必須)
**🎯 驗收方式**: 開發者可在 5 分鐘內完成 SDK 整合
**🔗 相關**: US 1.1 (Web UI), ADR-006 (狀態管理)

---

#### US 1.3 - Agent 配置管理

**作為** IT 開發者
**我想要** 查看、編輯、刪除已建立的 Agent
**以便** 持續優化和維護 Agent 配置

**驗收標準**:

✅ 必須項（列表功能）:
- [ ] Agent 列表頁面（卡片或表格視圖切換）
- [ ] 分頁功能（每頁 20 筆）
- [ ] 搜尋功能（名稱、描述、ID）
- [ ] 篩選功能（角色類型、模型、狀態）
- [ ] 排序功能（建立時間、修改時間、名稱）

✅ 必須項（詳情頁面）:
- [ ] Agent 完整配置展示
- [ ] Plugin 列表展示
- [ ] 執行統計（總次數、成功率、平均響應時間）
- [ ] 最近執行歷史（最近 10 筆）

✅ 必須項（編輯功能）:
- [ ] 線上編輯 System Prompt（即時生效或需確認）
- [ ] 修改模型參數
- [ ] 添加/移除 Plugin
- [ ] 版本歷史記錄（可回滾到先前版本）
- [ ] 變更日誌自動記錄

✅ 必須項（刪除功能）:
- [ ] 軟刪除機制（可恢復）
- [ ] 刪除前二次確認
- [ ] 批量操作（批量啟用/停用/刪除）
- [ ] 刪除影響提示（關聯工作流、執行歷史）

**UI 設計要求**:
- Material-UI 或類似設計系統
- 響應式設計（支援移動端）
- 載入狀態和骨架屏

**📊 優先級**: P0 (MVP 必須)
**🎯 驗收方式**: 完成 10 個 Agent 的管理操作 <5 分鐘
**🔗 相關**: US 1.4 (監控), US 8.3 (權限)

---

#### US 1.4 - Agent 執行與監控

**作為** IT 開發者
**我想要** 查看 Agent 的執行狀態和歷史記錄
**以便** 診斷問題和優化性能

**驗收標準**:

✅ 必須項（即時監控）:
- [ ] Agent 即時執行狀態（執行中/成功/失敗/超時）
- [ ] 當前執行數量和並發數
- [ ] 執行進度顯示（對於長時間任務）
- [ ] WebSocket 即時更新（無需手動刷新）

✅ 必須項（執行歷史）:
- [ ] 執行歷史列表（分頁，最近 100 筆）
- [ ] 篩選功能（狀態、時間範圍、用戶）
- [ ] 詳細執行日誌（輸入、輸出、中間步驟）
- [ ] 錯誤堆疊追蹤（失敗時）
- [ ] 支援搜尋（關鍵字搜尋對話內容）

✅ 必須項（性能指標）:
- [ ] 響應時間分佈（P50, P95, P99）
- [ ] Token 使用量統計
- [ ] Plugin 調用次數和耗時
- [ ] 成功率趨勢圖（過去 7 天/30 天）

✅ 必須項（匯出功能）:
- [ ] 匯出日誌（CSV/JSON）
- [ ] 匯出性能報告（PDF）
- [ ] 設定匯出範圍（時間、篩選條件）

**技術要求**:
- SignalR 即時通訊
- Application Insights 整合
- 時序數據庫（Prometheus 或 PostgreSQL TimescaleDB）

**📊 優先級**: P1 (MVP 高優先)
**🎯 驗收方式**: 可在 1 分鐘內定位問題執行記錄
**🔗 相關**: US 10.1 (API 計量), 性能監控架構

---

#### US 1.5 - 引導式 Persona Builder ⭐ 核心差異化

**作為** IT 開發者或業務分析師
**我想要** 通過 6 步引導式介面建立具有個性的 Agent
**以便** 創建能模擬特定人格的 AI 助手

**背景**:
傳統的 Agent 只能通過簡單的 System Prompt 定義，導致個性不穩定、風格不一致。本功能提供結構化的 Persona 定義框架，通過多維度配置和 Few-Shot 範例，確保 Agent 個性的一致性和真實性。

**驗收標準**:

✅ 必須項（6 步驟引導流程）:

**Step 1: 基本身份設定**
- [ ] 輸入 Agent 姓名（中文或英文）
- [ ] 選擇或自定義角色（產品經理、客服、技術顧問等）
- [ ] 撰寫背景描述（100-500 字）
- [ ] 提供幫助提示和範例

**Step 2: 個性特質滑桿**
- [ ] 正式程度滑桿（1-10，非正式 ↔ 非常正式）
- [ ] 熱情度滑桿（1-10，冷靜 ↔ 熱情）
- [ ] 同理心滑桿（1-10，理性 ↔ 同理心強）
- [ ] 決策風格多選（數據驅動、快速決策、謹慎保守、用戶優先）
- [ ] 即時預覽個性描述

**Step 3: 專業領域標註**
- [ ] 核心專長標籤（最多 5 個，可自定義）
- [ ] 次要專長標籤（最多 5 個）
- [ ] 知識盲區標籤（Agent 會坦誠承認不懂的領域）
- [ ] 預定義領域標籤庫（產品管理、數據分析、UX 設計等）

**Step 4: Few-Shot 對話範例** ⭐ 最關鍵
- [ ] 提供至少 3 個對話範例（建議 5-10 個）
- [ ] 每個範例包含：用戶輸入 + Agent 回應
- [ ] 支援多輪對話範例（展示上下文處理）
- [ ] 範例質量檢查（長度、多樣性、代表性）
- [ ] 提供預設範例模板

**Step 5: 說話習慣和禁忌**
- [ ] 常用口頭禪/習慣用語（最多 10 個）
- [ ] 禁止行為列表（不使用術語、不做承諾等）
- [ ] 特殊指令（條件觸發行為）
- [ ] 範例展示

**Step 6: 預覽與測試**
- [ ] 個性概覽卡片（匯總所有設定）
- [ ] 即時測試對話（輸入問題，查看 Agent 回應）
- [ ] 個性一致性評分（AI 評估）
- [ ] 匯出 Persona 配置（JSON/YAML）
- [ ] 儲存並啟用按鈕

✅ 必須項（質量保證）:
- [ ] 每步驟都有幫助提示圖示（❓ Tooltip）
- [ ] 支援儲存草稿（中途可離開，自動儲存）
- [ ] 步驟間導航（上一步/下一步/跳轉）
- [ ] 進度指示器（1/6, 2/6...）
- [ ] 即時驗證（必填欄位、格式檢查）

✅ 必須項（技術實現）:
- [ ] Persona 配置轉換為結構化 System Prompt
- [ ] Few-Shot 範例注入到 LLM Context
- [ ] 個性參數影響 temperature 等模型參數
- [ ] 支援 Persona 版本管理

**Persona 配置 Schema**:
```yaml
persona:
  identity:
    name: "張明華"
    role: "資深產品經理"
    background: "10年互聯網產品經驗..."

  personality_traits:
    formality: 7/10
    enthusiasm: 8/10
    empathy: 9/10
    decision_style: ["數據驅動", "快速決策", "用戶優先"]

  expertise:
    primary: ["產品管理", "用戶研究", "敏捷開發"]
    secondary: ["數據分析", "UX 設計"]
    limitations: ["後端架構", "硬體開發"]

  conversation_patterns:
    greeting_style: "簡短問候，快速進入主題"
    question_handling: "先理解需求背景，再提供建議"

  example_conversations:
    - user: "我們要不要加這個功能？"
      assistant: "先問幾個問題：1) 解決什麼用戶痛點？..."

  habits:
    - "數據怎麼說？"
    - "用戶怎麼想？"

  constraints:
    - "不使用過於技術性的術語"
    - "不做沒有數據支持的承諾"
```

**UI/UX 要求**:
- 引導式設計（Wizard Pattern）
- 每步驟單屏展示（避免滾動）
- 響應式設計（支援平板）
- 動畫過渡（步驟切換）

**📊 優先級**: P0 (MVP 必須，核心差異化能力)
**🎯 UX 目標**: 20 分鐘內完成一個高質量 Persona
**⏱️ 開發工期**: 3-4 週
**🔗 相關**: US 1.6 (模板庫), ADR-006 (狀態管理)

---

#### US 1.6 - Persona 模板庫

**作為** IT 開發者
**我想要** 使用預定義的 Persona 模板快速開始
**以便** 節省時間並學習最佳實踐

**驗收標準**:

✅ 必須項（模板庫）:
- [ ] 提供 10+ 預定義 Persona 模板
- [ ] 模板分類（業務類、技術類、客服類、創意類）
- [ ] 模板卡片展示（名稱、角色、預覽圖、簡介）
- [ ] 模板詳情預覽（完整配置展示）
- [ ] 模板評分和使用次數

✅ 必須項（使用模板）:
- [ ] 一鍵複製模板並修改
- [ ] 複製後自動進入編輯模式
- [ ] 支援模板搜尋和篩選
- [ ] 我的最愛模板（收藏功能）

✅ 必須項（預定義模板範例）:

1. **資深產品經理**
   - 個性：數據驅動、快速決策、用戶優先
   - 專長：產品管理、用戶研究、敏捷開發
   - 風格：正式度 7/10，直接簡潔

2. **客服專員**
   - 個性：同理心強、耐心解答、積極正面
   - 專長：客戶服務、問題解決、溝通技巧
   - 風格：熱情度 9/10，友善親切

3. **技術架構師**
   - 個性：深度思考、注重細節、邏輯嚴謹
   - 專長：系統設計、技術選型、性能優化
   - 風格：正式度 8/10，技術導向

4. **創業導師**
   - 個性：啟發式提問、經驗分享、鼓勵創新
   - 專長：商業策略、團隊管理、融資
   - 風格：熱情度 8/10，引導式對話

5. **數據分析師**
   - 個性：數據驅動、客觀理性、嚴謹細緻
   - 專長：數據分析、可視化、統計建模
   - 風格：正式度 6/10，事實為本

✅ 進階項（Phase 2）:
- [ ] 社群貢獻模板（用戶可上傳分享）
- [ ] 模板評論和評分系統
- [ ] 模板版本更新通知
- [ ] 模板使用分析（熱門度追蹤）

**技術要求**:
- 模板以 JSON 格式儲存
- 支援模板匯入/匯出
- 模板版本管理

**📊 優先級**: P1 (MVP 高優先)
**🎯 驗收方式**: 用戶可在 5 分鐘內從模板建立 Agent
**🔗 相關**: US 1.5 (Persona Builder)

---