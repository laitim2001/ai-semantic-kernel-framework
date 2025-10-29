# User Stories - 完整備份

**Semantic Kernel Agentic Framework**

**版本**: 2.0.1 (完整備份)
**日期**: 2025-10-28
**狀態**: 完整備份（包含所有 43 個 User Stories）
**作者**: Product Owner (BMad Method)

---

## 文檔說明

**本文檔是原始 user-stories.md 的完整備份**

原始文件已重組為三層模塊化結構（`docs/user-stories/` 目錄），但此備份文件保留了完整的原始格式，包含所有 43 個 User Stories 的詳細內容，方便查閱和參考。

**新的模塊化結構**: [docs/user-stories/README.md](./user-stories/README.md)

---

## User Stories 總覽

```yaml
總計:
  User Stories: 43 個
  Epic: 10 個功能模組

優先級分佈:
  P0 (MVP 必須): 29 個 (67%)
  P1 (MVP 高優先): 10 個 (23%)
  P2 (Phase 2): 4 個 (9%)

核心差異化能力:
  Persona Framework - 個性化 Agent 系統
  Structured Data + Text-to-SQL - 結構化數據查詢
  Code Interpreter - 4 層安全沙箱
  Knowledge Management - 90%+ 準確率
  Multimodal Chat - 多模態對話介面

企業級基礎能力:
  RBAC - 角色權限管理
  Multi-Tenant - 多租戶架構
  i18n - 多語言支援
  API Metering - 計量與限流
```

### 優先級說明

**P0 (MVP 必須)**: 核心功能，MVP 必須交付，無此功能無法上線
**P1 (MVP 高優先)**: 重要功能，盡量在 MVP 中包含，提升產品競爭力
**P2 (Phase 2)**: 增強功能，Phase 2 或更後期交付

### MVP 時程規劃

**時程**: 10-12 個月 (18 個 Sprint)
**Story Points**: ~215 (P0: 145, P1: 50, P2: 20)

---

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

---# 模組 02: Plugin 系統

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 02
**User Stories**: US 2.1-2.3
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 2: Plugin 系統

#### Epic 2: Plugin 開發與管理

**目標**: 提供靈活的 Plugin 擴展能力，支援 .NET 開發和熱更新

---

#### US 2.1 - 註冊 .NET Plugin（Phase 1 優先）

**作為** .NET 開發者
**我想要** 開發並註冊自定義 .NET Plugin
**以便** 擴展 Agent 的業務能力

**驗收標準**:

✅ 必須項（SDK 與腳手架）:
- [ ] 提供 Plugin SDK NuGet 套件（`SemanticKernel.Agentic.PluginSDK`）
- [ ] 提供 dotnet new 模板（`dotnet new sk-plugin -n MyPlugin`）
- [ ] 腳手架自動生成基礎結構（Plugin 類、測試專案、README）
- [ ] 完整的開發文檔和教學

✅ 必須項（Plugin 開發）:
- [ ] 使用 Attribute 標註 Plugin 函數（`[PluginFunction]`）
- [ ] 支援定義輸入/輸出 Schema（JSON Schema 或強類型）
- [ ] 支援異步函數（async/await）
- [ ] 支援依賴注入（IServiceProvider）
- [ ] 內建日誌記錄（ILogger）

✅ 必須項（註冊與管理）:
- [ ] 通過 Web UI 上傳 Plugin DLL
- [ ] 通過 API 註冊 Plugin（CI/CD 整合）
- [ ] 自動提取 Plugin 元數據（名稱、版本、函數列表）
- [ ] 自動生成 OpenAPI 文檔
- [ ] 支援版本標註（SemVer: 1.0.0, 1.1.0...）
- [ ] Plugin 狀態管理（已註冊、已啟用、已停用）

**範例代碼**:
```csharp
using SemanticKernel.Agentic.PluginSDK;

[Plugin("Weather", "1.0.0")]
[Description("Get weather information for locations")]
public class WeatherPlugin
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherPlugin> _logger;

    public WeatherPlugin(
        IHttpClientFactory httpClientFactory,
        ILogger<WeatherPlugin> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [PluginFunction("GetCurrentWeather")]
    [Description("Get current weather for a specific city")]
    public async Task<WeatherResult> GetCurrentWeatherAsync(
        [Description("City name (e.g., Taipei, Tokyo)")] string city,
        [Description("Temperature unit (Celsius or Fahrenheit)")]
        TemperatureUnit unit = TemperatureUnit.Celsius,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting weather for {City}", city);

        var client = _httpClientFactory.CreateClient("WeatherAPI");
        var response = await client.GetAsync(
            $"/weather?city={city}&unit={unit}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var weather = await response.Content
            .ReadFromJsonAsync<WeatherResult>(cancellationToken);

        return weather;
    }
}

public record WeatherResult(
    string City,
    double Temperature,
    string Condition,
    int Humidity
);

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}
```

**技術要求**:
- .NET 8 SDK
- Reflection 提取元數據
- AssemblyLoadContext 隔離

**📊 優先級**: P0 (MVP 必須)
**🎯 驗收方式**: 開發者可在 30 分鐘內完成 Plugin 開發並註冊
**🔗 相關**: US 2.2 (熱更新), ADR (Plugin 架構)

---

#### US 2.2 - Plugin 熱更新（不重啟系統）⭐ 運維關鍵

**作為** DevOps 工程師
**我想要** 更新 Plugin 版本而不需要重啟系統
**以便** 實現零停機時間的功能升級

**背景**:
傳統的 Plugin 更新需要重啟應用程式，導致服務中斷。本功能使用 .NET AssemblyLoadContext 技術實現 Plugin 動態載入和卸載，確保零停機升級。

**驗收標準**:

✅ 必須項（熱更新流程）:
- [ ] 上傳新版本 Plugin DLL（通過 UI 或 API）
- [ ] 系統自動檢測版本變更
- [ ] 執行中的 Agent 完成當前任務後自動切換到新版本
- [ ] 新啟動的 Agent 立即使用新版本
- [ ] 熱更新操作記錄和審計日誌

✅ 必須項（版本管理）:
- [ ] 支援多版本並存（v1.0.0 和 v1.1.0 同時存在）
- [ ] A/B 測試支援（部分 Agent 使用新版本）
- [ ] 版本切換策略配置（立即切換、優雅切換、手動切換）
- [ ] 版本回滾機制（5 分鐘內可回滾到舊版本）
- [ ] 版本相容性檢查（API 破壞性變更警告）

✅ 必須項（安全性與穩定性）:
- [ ] 新版本載入前驗證（語法檢查、依賴檢查）
- [ ] 熱更新失敗自動回滾
- [ ] 舊版本優雅卸載（等待執行中任務完成）
- [ ] 記憶體洩漏防護（確保舊版本資源釋放）
- [ ] 熱更新狀態監控（成功率、回滾次數）

✅ 必須項（運維介面）:
- [ ] Plugin 版本歷史展示
- [ ] 當前使用版本統計（各版本使用 Agent 數量）
- [ ] 熱更新進度展示（已切換 Agent 數量）
- [ ] 手動觸發回滾按鈕
- [ ] 熱更新日誌查詢

**技術實現**:
```csharp
// AssemblyLoadContext 隔離不同版本
public class PluginLoadContext : AssemblyLoadContext
{
    private readonly string _pluginPath;

    public PluginLoadContext(string pluginPath)
        : base(isCollectible: true) // 可卸載
    {
        _pluginPath = pluginPath;
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        var assemblyPath = Path.Combine(_pluginPath, $"{assemblyName.Name}.dll");
        if (File.Exists(assemblyPath))
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }
}

// Plugin 版本管理器
public class PluginVersionManager
{
    private readonly ConcurrentDictionary<string, List<PluginVersion>> _plugins = new();

    public async Task<bool> LoadNewVersionAsync(
        string pluginName,
        string version,
        string dllPath)
    {
        var loadContext = new PluginLoadContext(Path.GetDirectoryName(dllPath));
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);

        // 驗證 Plugin
        if (!ValidatePlugin(assembly))
        {
            loadContext.Unload();
            return false;
        }

        // 註冊新版本
        var pluginVersion = new PluginVersion(pluginName, version, loadContext, assembly);
        _plugins.AddOrUpdate(pluginName,
            new List<PluginVersion> { pluginVersion },
            (key, list) => { list.Add(pluginVersion); return list; });

        return true;
    }

    public async Task UnloadOldVersionAsync(string pluginName, string version)
    {
        // 等待所有使用舊版本的 Agent 完成
        await WaitForActiveAgentsCompleteAsync(pluginName, version);

        // 卸載 AssemblyLoadContext
        var pluginVersion = GetPluginVersion(pluginName, version);
        pluginVersion.LoadContext.Unload();

        // 等待 GC 回收
        for (int i = 0; i < 10 && pluginVersion.LoadContext.IsCollectible; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            await Task.Delay(100);
        }
    }
}
```

**UI 設計**:
```
┌─────────────────────────────────────────┐
│ Plugin: WeatherPlugin                   │
├─────────────────────────────────────────┤
│ 版本歷史:                                │
│ ┌───────────────────────────────────┐   │
│ │ ● v1.2.0 (最新) - 2025-10-28      │   │
│ │   使用中 Agent: 15 個             │   │
│ │   [設為預設] [回滾]               │   │
│ │                                    │   │
│ │ ○ v1.1.0 - 2025-10-20             │   │
│ │   使用中 Agent: 5 個 (遷移中)    │   │
│ │   [手動切換全部]                  │   │
│ │                                    │   │
│ │ ○ v1.0.0 - 2025-10-01             │   │
│ │   使用中 Agent: 0 個              │   │
│ │   [已停用]                        │   │
│ └───────────────────────────────────┘   │
│                                          │
│ 熱更新策略:                              │
│ (•) 優雅切換（完成當前任務後切換）      │
│ ( ) 立即切換（強制中斷）                │
│ ( ) 手動切換（需管理員確認）            │
└─────────────────────────────────────────┘
```

**📊 優先級**: P1 (MVP 高優先)
**🎯 技術難度**: High
**⏱️ 開發工期**: 2-3 週
**🔗 相關**: US 2.1 (Plugin 開發), 運維監控

---

#### US 2.3 - Plugin Marketplace（發現與安裝）

**作為** IT 開發者
**我想要** 瀏覽和安裝可用的 Plugin
**以便** 快速獲得所需功能

**驗收標準**:

✅ 必須項（Marketplace UI）:
- [ ] Plugin 列表頁（網格或列表視圖）
- [ ] 搜尋功能（名稱、描述、標籤）
- [ ] 分類篩選（工具類、數據類、整合類等）
- [ ] 排序（熱門度、評分、最新）
- [ ] Plugin 評分和評論展示

✅ 必須項（Plugin 詳情）:
- [ ] Plugin 詳細說明（功能、使用方法）
- [ ] 版本歷史和變更日誌
- [ ] 範例代碼和使用教學
- [ ] 依賴項列表
- [ ] 安裝統計（下載次數、使用數量）

✅ 必須項（安裝功能）:
- [ ] 一鍵安裝按鈕
- [ ] 版本相容性自動檢查
- [ ] 安裝進度顯示
- [ ] 安裝失敗回滾
- [ ] 已安裝 Plugin 管理（更新/卸載）

✅ 進階項（Phase 2）:
- [ ] 第三方 Plugin 上傳（社群貢獻）
- [ ] Plugin 審核流程
- [ ] 付費 Plugin 支援
- [ ] Plugin 收益分成

**📊 優先級**: P2 (Phase 2)
**🎯 驗收方式**: 用戶可在 2 分鐘內找到並安裝 Plugin
**🔗 相關**: US 2.1 (註冊), US 2.2 (熱更新)

---# 模組 03: Code Interpreter

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 03
**User Stories**: US 3.1-3.3
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 3: Code Interpreter ⭐ 核心競爭力

#### Epic 3: 代碼執行與可視化

**目標**: 提供安全的 Python 代碼執行環境，支援自動生成可視化圖表

---

#### US 3.1 - Python 代碼執行（4 層安全沙箱）

**作為** Agent（系統內部）
**我想要** 在安全沙箱中執行 Python 代碼
**以便** 進行數據分析和複雜計算

**背景**:
這是本項目的**核心差異化能力之一**，彌補 Copilot Studio 無法執行代碼的限制。安全性是首要考量，必須實施 4 層防護。

**驗收標準**:

✅ 必須項（4 層安全防護）:

**Layer 1: Network Isolation（網絡隔離）**
- [ ] Docker Container 使用 `--network=none`（無外網訪問）
- [ ] 禁止訪問任何外部 API 或服務
- [ ] 僅允許與宿主機通訊（代碼輸入、結果輸出）

**Layer 2: Resource Limits（資源限制）**
- [ ] CPU 限制: 1 core（`--cpus=1`）
- [ ] Memory 限制: 512MB（`--memory=512m`）
- [ ] Disk 空間限制: 100MB（`--storage-opt size=100m`）
- [ ] Process 數量限制: 50（`--pids-limit=50`）
- [ ] 資源超限自動終止

**Layer 3: Execution Timeout（執行超時）**
- [ ] 硬性超時: 30 秒（強制終止）
- [ ] 軟性超時: 25 秒（警告提示）
- [ ] 超時後返回部分結果（如果有）
- [ ] 超時統計和告警

**Layer 4: Read-Only Filesystem（唯讀文件系統）**
- [ ] 文件系統唯讀掛載（`--read-only`）
- [ ] 僅 `/tmp` 可寫（100MB, `noexec`）
- [ ] 禁止寫入系統目錄
- [ ] 禁止執行二進制文件（`noexec` mount option）

✅ 必須項（Python 環境）:
- [ ] 預安裝套件: NumPy, Pandas, Matplotlib, Seaborn, SciPy
- [ ] Python 版本: 3.11+
- [ ] 支援中文字體（Matplotlib 顯示中文）
- [ ] 禁止安裝新套件（`pip install` 不可用）

✅ 必須項（執行功能）:
- [ ] 支援多行代碼執行
- [ ] 返回 stdout（標準輸出）
- [ ] 返回 stderr（錯誤輸出）
- [ ] 返回 return value（函數返回值）
- [ ] 返回生成的圖表（PNG/SVG）
- [ ] 返回生成的文件列表（/tmp 下）

✅ 必須項（錯誤處理）:
- [ ] 語法錯誤友好提示（行號、錯誤類型）
- [ ] 執行錯誤完整堆疊追蹤
- [ ] 超時錯誤明確提示
- [ ] 資源超限錯誤提示
- [ ] 安全違規錯誤提示（嘗試訪問網絡等）

✅ 必須項（Container Pool）:
- [ ] 預熱 5 個 Standby Containers
- [ ] 從 Pool 獲取 Container <2 秒
- [ ] 執行後銷毀 Container（不重用）
- [ ] 異步非阻塞補充 Pool
- [ ] Pool 使用率監控

**範例執行**:
```python
# 用戶代碼
import pandas as pd
import matplotlib.pyplot as plt

data = pd.DataFrame({
    'month': ['Jan', 'Feb', 'Mar', 'Apr'],
    'sales': [1000, 1200, 1100, 1300]
})

plt.figure(figsize=(10, 6))
plt.plot(data['month'], data['sales'], marker='o')
plt.title('月度銷售趨勢', fontsize=14)
plt.xlabel('月份')
plt.ylabel('銷售額')
plt.grid(True)
plt.savefig('/tmp/sales_trend.png', dpi=150)

print(data.describe())
```

**返回結果**:
```json
{
  "status": "success",
  "execution_time": 1.23,
  "stdout": "       sales\ncount    4.0\nmean  1150.0\nstd    129.1\nmin   1000.0\nmax   1300.0",
  "stderr": "",
  "return_value": null,
  "files": [
    {
      "path": "/tmp/sales_trend.png",
      "url": "https://blob.storage/executions/abc123/sales_trend.png",
      "mime_type": "image/png",
      "size_bytes": 45678
    }
  ]
}
```

**技術實現**:
- Docker Engine API
- Container Orchestration
- Azure Blob Storage（圖片儲存）

**📊 優先級**: P0 (MVP 必須，核心競爭力)
**🎯 性能目標**: P95 執行時間 <8 秒
**🔒 安全要求**: 通過安全審計，無 Container 逃逸風險
**⏱️ 開發工期**: 3-4 週
**🔗 相關**: US 3.2 (自動生成代碼), ADR-008 (執行模型)

---

#### US 3.2 - Agent 自動生成可視化代碼 ⭐ 關鍵能力

**作為** 業務用戶
**我想要** Agent 自動生成數據分析代碼並執行
**以便** 獲得視覺化的分析結果

**背景**:
這是 Code Interpreter 的核心價值體現。Agent 不只執行用戶提供的代碼，更重要的是能夠**自主生成代碼**來完成分析任務。

**驗收標準**:

✅ 必須項（代碼生成能力）:
- [ ] Agent 可根據數據和問題自動生成 Python 代碼
- [ ] 支援常見數據分析任務（統計、聚合、過濾、排序）
- [ ] 支援常見圖表類型：
  - 折線圖（Line Chart）
  - 柱狀圖（Bar Chart）
  - 散點圖（Scatter Plot）
  - 圓餅圖（Pie Chart）
  - 熱力圖（Heatmap）
  - 箱型圖（Box Plot）

✅ 必須項（代碼質量）:
- [ ] 生成的代碼可讀性良好（變量命名清晰）
- [ ] 包含註解說明關鍵步驟
- [ ] 自動處理中文顯示（設定字體）
- [ ] 圖表包含標題、軸標籤、圖例
- [ ] 錯誤處理（數據為空、類型錯誤等）

✅ 必須項（執行流程）:
- [ ] Agent 生成代碼 → 展示代碼給用戶（可選）
- [ ] 執行代碼 → 獲取結果
- [ ] 返回：圖片 URL + 文字說明
- [ ] 如果執行失敗，Agent 修正代碼並重試（最多 2 次）

✅ 必須項（多步驟分析）:
- [ ] 支援多步驟代碼執行（Step 1 加載數據 → Step 2 清理 → Step 3 可視化）
- [ ] 步驟間數據傳遞（通過文件或變量）
- [ ] 執行進度展示（當前步驟、總步驟數）

**互動範例**:

**場景 1: 銷售趨勢分析**
```
用戶: "分析這個銷售數據的趨勢"
[上傳 sales.csv]

Agent:
"我來幫您分析銷售趨勢。首先載入數據..."

[Agent 生成並執行代碼:]
```python
import pandas as pd
import matplotlib.pyplot as plt

# 載入數據
data = pd.read_csv('/tmp/sales.csv')
print(f"數據概覽:\n{data.head()}")

# 繪製趨勢圖
plt.figure(figsize=(12, 6))
plt.plot(data['date'], data['revenue'], marker='o', linewidth=2)
plt.title('銷售收入趨勢', fontsize=16)
plt.xlabel('日期')
plt.ylabel('收入（元）')
plt.grid(True, alpha=0.3)
plt.xticks(rotation=45)
plt.tight_layout()
plt.savefig('/tmp/sales_trend.png', dpi=150)
```

Agent:
"從趨勢圖可以看出：
📈 銷售收入呈現穩定上升趨勢
💡 Q3 增長最快，增長率達 25%
⚠️ 10月有小幅下滑，建議關注

[圖片: sales_trend.png]"
```

**場景 2: 多維度數據分析**
```
用戶: "比較不同地區的銷售表現"

Agent:
"我來分析各地區的銷售表現，包括總銷售額和平均客單價。"

[Agent 生成並執行代碼:]
```python
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# 載入數據
data = pd.read_csv('/tmp/sales.csv')

# 按地區聚合
region_stats = data.groupby('region').agg({
    'revenue': ['sum', 'mean', 'count']
}).round(2)

print(region_stats)

# 繪製對比圖
fig, axes = plt.subplots(1, 2, figsize=(14, 6))

# 左圖：總銷售額
axes[0].bar(region_stats.index, region_stats[('revenue', 'sum')])
axes[0].set_title('各地區總銷售額')
axes[0].set_ylabel('銷售額（元）')

# 右圖：平均客單價
axes[1].bar(region_stats.index, region_stats[('revenue', 'mean')])
axes[1].set_title('各地區平均客單價')
axes[1].set_ylabel('平均金額（元）')

plt.tight_layout()
plt.savefig('/tmp/region_comparison.png', dpi=150)
```

Agent:
"各地區銷售表現分析：

🏆 台北：總銷售額第一（$500,000），但客單價中等
💼 新竹：客單價最高（$1,200），科技園區消費力強
📊 台中：訂單量最多，適合薄利多銷策略

[圖片: region_comparison.png]

建議：
1. 加強台北地區的高端產品推廣
2. 複製新竹的高單價銷售模式到其他地區"
```

✅ 必須項（質量控制）:
- [ ] 圖表生成成功率 >95%
- [ ] 代碼執行成功率 >90%（首次嘗試）
- [ ] 修正後成功率 >98%（含重試）
- [ ] 用戶滿意度（UAT 測試）>85%

**技術實現**:
- GPT-4o 或 GPT-4 生成代碼
- Prompt Engineering（Few-Shot 範例）
- Code Validation（語法檢查）

**📊 優先級**: P0 (MVP 必須)
**🎯 準確率目標**: 圖表生成成功率 >95%
**⏱️ 開發工期**: 2-3 週
**🔗 相關**: US 3.1 (代碼執行), US 6.3 (圖表展示)

---

#### US 3.3 - Code Interpreter 結果展示（UI 增強）

**作為** 業務用戶
**我想要** 在 Chat 介面中清晰展示代碼執行結果
**以便** 理解 Agent 的分析過程和結果

**驗收標準**:

✅ 必須項（代碼塊展示）:
- [ ] 代碼塊語法高亮（Python 語法）
- [ ] 顯示語言標籤（```python）
- [ ] 一鍵複製代碼按鈕
- [ ] 行號顯示（可選開關）
- [ ] 代碼折疊/展開（超過 20 行自動折疊）
- [ ] 深色/淺色主題適配

✅ 必須項（執行結果展示）:
- [ ] 執行結果與代碼區分展示（不同背景色）
- [ ] stdout 輸出格式化（保持空白和縮排）
- [ ] stderr 錯誤訊息紅色標註
- [ ] 執行時間顯示（1.23s）
- [ ] 執行狀態圖示（成功✅、失敗❌、執行中⏳）

✅ 必須項（圖片展示）:
- [ ] 圖片內嵌顯示（自動載入）
- [ ] 圖片點擊放大預覽（Lightbox）
- [ ] 圖片下載按鈕
- [ ] 支援格式: PNG, SVG
- [ ] 圖片載入失敗顯示佔位符

✅ 必須項（互動功能）:
- [ ] 代碼可編輯並重新執行（進階功能）
- [ ] 執行歷史記錄（查看之前的代碼版本）
- [ ] 分享代碼和結果（生成分享連結）

**UI 設計參考**:
- Jupyter Notebook 代碼單元格樣式
- Claude/ChatGPT Code Interpreter UI
- GitHub 代碼塊樣式

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ Agent 回應:                              │
│ 我來幫您分析銷售數據的趨勢。             │
│                                          │
│ [代碼塊]                                 │
│ ┌───────────────────────────────────┐   │
│ │ ```python            [複製] [▼]   │   │
│ │  1  import pandas as pd           │   │
│ │  2  import matplotlib.pyplot as...│   │
│ │  3                                │   │
│ │  4  data = pd.read_csv(...)       │   │
│ │  ...                              │   │
│ └───────────────────────────────────┘   │
│                                          │
│ [執行結果] ✅ 1.23s                      │
│ ┌───────────────────────────────────┐   │
│ │ 數據概覽:                          │   │
│ │    date     revenue  region       │   │
│ │ 0  2024-01  50000    台北         │   │
│ │ 1  2024-02  52000    台北         │   │
│ └───────────────────────────────────┘   │
│                                          │
│ [圖片] 📊 sales_trend.png [下載] [🔍]   │
│ [圖片內嵌顯示]                           │
│                                          │
│ 從趨勢圖可以看出...                      │
└─────────────────────────────────────────┘
```

**技術要求**:
- Syntax Highlighter: Prism.js 或 Highlight.js
- Lightbox: React Image Lightbox
- Code Editor: CodeMirror 或 Monaco Editor（進階功能）

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 用戶可清晰理解代碼執行過程
**🔗 相關**: US 3.1 (代碼執行), US 6.4 (代碼塊通用展示)

---# 模組 04: Multi-Agent 協作

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 04
**User Stories**: US 4.1-4.3
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 4: Multi-Agent 協作

#### Epic 4: Agent 編排與工作流

**目標**: 提供可視化的工作流編輯器，支援複雜的 Multi-Agent 協作場景

---

#### US 4.1 - 可視化工作流編輯器（類似 n8n, Dify）⭐ 關鍵 UX

**作為** 業務分析師
**我想要** 使用拖拽方式設計 Multi-Agent 工作流
**以便** 無需寫代碼即可編排複雜業務流程

**背景**:
這是提升產品易用性的關鍵功能。通過可視化編輯器，非技術用戶也能設計複雜的 Multi-Agent 協作流程。

**驗收標準**:

✅ 必須項（畫布功能）:
- [ ] 無限畫布（可縮放、移動）
- [ ] 縮放控制（25% - 200%）
- [ ] 網格對齊（Snap to Grid）
- [ ] 小地圖導航（Overview）
- [ ] 撤銷/重做（Undo/Redo，至少 20 步）
- [ ] 快捷鍵支援（Ctrl+Z, Ctrl+C, Ctrl+V 等）

✅ 必須項（節點類型）:

**1. Agent 節點**
- [ ] 從 Agent 列表拖拽到畫布
- [ ] 配置 Agent 輸入變量
- [ ] 設定超時時間
- [ ] 錯誤處理策略（重試、跳過、終止）

**2. 判斷節點（Condition）**
- [ ] If-Else 分支
- [ ] 支援條件表達式（變量比較、正則匹配）
- [ ] 多分支判斷（Switch-Case）

**3. 合併節點（Merge）**
- [ ] 等待所有分支完成
- [ ] 取第一個完成的分支
- [ ] 數據合併策略

**4. 迴圈節點（Loop）**
- [ ] For-Each 迭代
- [ ] While 條件迴圈
- [ ] 迴圈次數限制（防止無限迴圈）

**5. 延遲節點（Delay）**
- [ ] 固定延遲（秒/分鐘）
- [ ] 等待特定時間點

**6. 自定義代碼節點（Script）**
- [ ] JavaScript/Python 代碼執行
- [ ] 訪問工作流變量
- [ ] 轉換數據格式

✅ 必須項（連線功能）:
- [ ] 拖拽連線（從輸出點到輸入點）
- [ ] 自動路由（避免節點重疊）
- [ ] 連線標籤（顯示數據流）
- [ ] 條件分支標註（True/False）
- [ ] 刪除連線（點擊連線 → Delete）

✅ 必須項（節點配置）:
- [ ] 點擊節點打開配置面板（右側 Panel）
- [ ] 配置 Agent 參數
- [ ] 變量映射（將上游輸出映射到當前節點輸入）
- [ ] 配置驗證（必填欄位、類型檢查）
- [ ] 配置預覽（即時顯示配置效果）

✅ 必須項（工作流管理）:
- [ ] 工作流命名和描述
- [ ] 儲存工作流（自動儲存 + 手動儲存）
- [ ] 工作流版本歷史
- [ ] 工作流發佈（Draft → Published）
- [ ] 工作流測試執行（Debug 模式）

✅ 必須項（自動佈局）:
- [ ] 自動排列節點（水平/垂直）
- [ ] 自動對齊（左對齊、居中、右對齊）
- [ ] 分組功能（選中多個節點分組）

**UI 設計參考**:
- n8n Workflow Editor
- Dify Workflow Canvas
- Node-RED Flow Editor
- Microsoft Power Automate

**UI 示例**:
```
┌─────────────────────────────────────────────────────────┐
│ 工作流: 客戶服務流程                    [儲存] [發佈]    │
├────┬────────────────────────────────────────────────────┤
│節點│                                                     │
│列表│  ┌───────┐                                         │
│    │  │ Start │                                         │
│🤖  │  └───┬───┘                                         │
│Agent│      │                                             │
│    │      ▼                                              │
│⚙️  │  ┌────────────┐                                    │
│判斷│  │  分類Agent │ ← 配置面板打開                     │
│    │  └─────┬──────┘                                    │
│🔄  │        │                                            │
│迴圈│   ┌────┴────┐                                      │
│    │   │         │                                       │
│⏱️  │   ▼         ▼                                       │
│延遲│ ┌──────┐ ┌──────┐                                  │
│    │ │退款? │ │技術?│                                   │
│📝  │ └──┬───┘ └───┬──┘                                  │
│代碼│    │         │                                      │
│    │    ▼         ▼                                       │
│    │ ┌──────┐ ┌──────┐                                  │
│    │ │退款   │ │技術  │                                  │
│    │ │Agent │ │Agent │                                  │
│    │ └──────┘ └──────┘                                  │
│    │                                                     │
└────┴─────────────────────────────────────────────────────┘
```

**技術實現**:
- React Flow 或 ReactFlow
- Canvas API
- WebSocket 即時協作（Phase 2）

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 業務分析師 1 小時內學會使用
**⏱️ 開發工期**: 4-5 週
**🔗 相關**: US 4.2 (配置文件), US 4.3 (執行監控)

---

#### US 4.2 - 工作流配置文件匯出/匯入

**作為** IT 開發者
**我想要** 將工作流匯出為 YAML/JSON 配置文件
**以便** 版本控制和批量部署

**驗收標準**:

✅ 必須項（匯出功能）:
- [ ] 匯出為 YAML 格式（推薦，更可讀）
- [ ] 匯出為 JSON 格式（機器友好）
- [ ] 配置文件包含完整工作流定義
- [ ] 包含節點配置、連線關係、變量定義
- [ ] 人類可讀（帶註解、縮排）

✅ 必須項（匯入功能）:
- [ ] 匯入 YAML/JSON 文件建立工作流
- [ ] 自動驗證配置文件正確性
- [ ] 檢查引用的 Agent 是否存在
- [ ] 檢查變量類型匹配
- [ ] 匯入錯誤友好提示（行號、錯誤原因）

✅ 必須項（配置文件格式）:
- [ ] 支援變量替換（環境變量、參數化）
- [ ] 支援註解（# 或 //）
- [ ] Git 友好（合併衝突少）
- [ ] Schema 驗證（JSON Schema）

**YAML 範例**:
```yaml
workflow:
  name: "客戶服務流程"
  version: "1.0.0"
  description: "自動分類客戶問題並路由到對應 Agent"

  variables:
    # 全局變量
    max_retries: 3
    timeout_seconds: 30

  nodes:
    - id: "start"
      type: "start"

    - id: "classifier"
      type: "agent"
      config:
        agent_id: "${CLASSIFIER_AGENT_ID}" # 環境變量
        timeout: "${timeout_seconds}"

    - id: "is_refund"
      type: "condition"
      config:
        expression: "classifier.output.intent == 'refund'"

    - id: "refund_agent"
      type: "agent"
      config:
        agent_id: "agent-refund-001"
        input:
          user_message: "${start.input.message}"
          classification: "${classifier.output}"

  edges:
    - from: "start"
      to: "classifier"

    - from: "classifier"
      to: "is_refund"

    - from: "is_refund"
      to: "refund_agent"
      condition: "true"

    - from: "is_refund"
      to: "tech_agent"
      condition: "false"
```

**技術要求**:
- YAML Parser: YamlDotNet (.NET)
- JSON Schema Validation
- 環境變量替換

**📊 優先級**: P1 (MVP 高優先)
**🎯 驗收方式**: 可在 Git 中追蹤工作流變更
**🔗 相關**: US 4.1 (編輯器), CI/CD 整合

---

#### US 4.3 - 工作流執行與監控

**作為** IT 開發者
**我想要** 查看工作流的即時執行狀態
**以便** 診斷問題和優化流程

**驗收標準**:

✅ 必須項（即時執行可視化）:
- [ ] 工作流執行時在畫布上即時高亮當前節點
- [ ] 已完成節點顯示綠色邊框
- [ ] 執行中節點顯示藍色動畫邊框
- [ ] 失敗節點顯示紅色邊框
- [ ] 顯示每個節點的執行耗時

✅ 必須項（節點執行詳情）:
- [ ] 點擊節點查看輸入/輸出數據
- [ ] 顯示節點執行日誌
- [ ] 顯示錯誤堆疊（如果失敗）
- [ ] 支援重新執行單個節點（Debug）

✅ 必須項（執行歷史）:
- [ ] 執行歷史列表（最近 50 筆）
- [ ] 執行狀態篩選（成功/失敗/執行中）
- [ ] 執行時間篩選（今天/本週/本月）
- [ ] 執行詳情可重播（查看執行過程）

✅ 必須項（性能分析）:
- [ ] 每個節點的平均執行時間
- [ ] 工作流總執行時間分佈（P50, P95, P99）
- [ ] 瓶頸節點識別（耗時最長的節點）
- [ ] 失敗率統計（每個節點的失敗率）

✅ 必須項（告警功能）:
- [ ] 工作流執行失敗時 Email 通知
- [ ] 執行時間超過閾值時告警
- [ ] 失敗率超過 10% 時告警

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ 執行監控: 客戶服務流程                   │
├─────────────────────────────────────────┤
│ 當前執行: #12345                         │
│ 狀態: 執行中 ⏳ | 耗時: 5.2s             │
│                                          │
│ [畫布 - 即時更新]                        │
│  ┌───────┐                               │
│  │ Start │ ✅ 0.1s                       │
│  └───┬───┘                               │
│      │                                    │
│  ┌───▼─────┐                             │
│  │分類Agent│ 🔵執行中 3.2s...            │
│  └─────────┘                             │
│                                          │
│ [右側面板 - 分類Agent 執行詳情]          │
│ 輸入:                                    │
│ {                                        │
│   "message": "我要退款"                  │
│ }                                        │
│                                          │
│ 輸出: (執行中...)                        │
└─────────────────────────────────────────┘
```

**技術要求**:
- WebSocket 即時更新
- 執行日誌串流
- 時序數據存儲

**📊 優先級**: P1 (MVP 高優先)
**🎯 驗收方式**: 可在 30 秒內定位工作流瓶頸
**🔗 相關**: US 4.1 (編輯器), US 1.4 (Agent 監控)

---# 模組 05: Knowledge 管理

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 05
**User Stories**: US 5.1-5.7
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 5: Knowledge 管理 ⭐ 90%+ 準確率

#### Epic 5: 知識庫與精確檢索

**目標**: 提供高準確率的知識檢索能力，支援文檔和結構化數據

---

#### US 5.1 - 知識庫文件上傳與處理

**作為** IT 開發者
**我想要** 批量上傳文件到 Agent 知識庫
**以便** 快速建立知識系統

**驗收標準**:

✅ 必須項（文件上傳）:
- [ ] 支援格式: PDF, DOCX, TXT, MD, HTML
- [ ] 批量上傳（拖拽多個文件）
- [ ] 單檔大小限制: 50MB
- [ ] 總大小限制（依訂閱計劃）
- [ ] 上傳進度顯示（百分比、速度）
- [ ] 斷點續傳（大文件上傳失敗可續傳）

✅ 必須項（文件處理）:
- [ ] 自動文字提取（PDF OCR、DOCX 解析）
- [ ] 自動分段處理（Chunking）
  - Chunk 大小可配置（500-2000 tokens）
  - Chunk 重疊可配置（0-200 tokens）
- [ ] 自動向量化（使用 text-embedding-ada-002 或類似）
- [ ] 索引到向量資料庫（Azure AI Search）
- [ ] 處理進度即時顯示
- [ ] 處理失敗自動重試（最多 3 次）

✅ 必須項（文件管理）:
- [ ] 上傳完成後可預覽內容（前 1000 字）
- [ ] 文件列表展示（名稱、大小、上傳時間、狀態）
- [ ] 文件搜尋和篩選
- [ ] 文件刪除（軟刪除，可恢復）
- [ ] 文件重新索引（更新 Embedding）

✅ 必須項（處理流程）:
```
1. 上傳文件 → Azure Blob Storage
2. 提取文字 → Azure Document Intelligence
3. 分段處理 → Chunking Engine
4. 向量化 → Azure OpenAI Embedding
5. 索引 → Azure AI Search
6. 完成 → 通知用戶
```

**技術要求**:
- Azure Blob Storage（文件存儲）
- Azure Document Intelligence（PDF 提取）
- Azure OpenAI Embedding API
- Azure AI Search（向量索引）

**📊 優先級**: P0 (MVP 必須)
**🎯 性能目標**: 100 頁 PDF <2 分鐘處理完成
**🔗 相關**: US 5.2 (檢索策略), US 5.4 (結構化數據)

---

#### US 5.2 - 精確檢索策略配置（達成 90%+ 準確率）⭐ 關鍵

**作為** IT 開發者
**我想要** 精細調整知識檢索策略
**以便** 達到 90% 以上的檢索準確率

**背景**:
這是本項目的核心競爭力之一，必須顯著超越 Copilot Studio 的檢索準確率。通過多層檢索策略和 Re-ranking，達到 90%+ 的目標。

**驗收標準**:

✅ 必須項（檢索策略）:

**策略 1: 向量搜尋（Semantic Search）**
- [ ] 使用 Embedding 進行語義搜尋
- [ ] 支援混合檢索（向量 + 關鍵字）
- [ ] 可調整向量相似度閾值（0.7-0.9）
- [ ] 支援多語言 Embedding

**策略 2: 關鍵字搜尋（Keyword Search）**
- [ ] BM25 演算法
- [ ] 全文檢索（Full-Text Search）
- [ ] 支援同義詞擴展
- [ ] 支援中文分詞

**策略 3: 混合檢索（Hybrid Search）**
- [ ] 向量搜尋 + 關鍵字搜尋
- [ ] 權重可調（向量 70% + 關鍵字 30%）
- [ ] Reciprocal Rank Fusion（RRF）合併結果

**策略 4: Re-ranking**
- [ ] 使用 Cross-Encoder 重排序
- [ ] 考慮文件新鮮度（最近更新優先）
- [ ] 考慮用戶反饋（點讚/點踩）

✅ 必須項（參數控制）:
- [ ] Top-K 設定（返回前 K 個結果，1-20）
- [ ] 相似度閾值（過濾低相關結果，0-1）
- [ ] Chunk 大小設定（500-2000 tokens）
- [ ] Chunk 重疊設定（0-200 tokens）
- [ ] Re-ranking 模型選擇

✅ 必須項（準確率測試）:

**建立標準測試集**:
- [ ] 準備 100+ 問答對
- [ ] 涵蓋不同問題類型（事實、推理、摘要）
- [ ] 涵蓋不同領域（產品、技術、政策）
- [ ] UAT 用戶標註正確答案和相關文檔

**自動評估**:
- [ ] 自動計算準確率（正確答案/總問題數）
- [ ] 計算 MRR（Mean Reciprocal Rank）
- [ ] 計算 NDCG（Normalized Discounted Cumulative Gain）
- [ ] 準確率 Dashboard 即時顯示

**A/B 測試**:
- [ ] 支援測試不同檢索策略
- [ ] 並排比較準確率
- [ ] 自動選擇最佳策略

**測試集範例**:
```json
{
  "test_cases": [
    {
      "question": "公司的退款政策是什麼？",
      "expected_answer": "30 天內可全額退款",
      "expected_sources": ["退款政策.pdf", "客服手冊.docx"],
      "expected_page": 2,
      "difficulty": "easy"
    },
    {
      "question": "如果產品有質量問題，退款後還能再次購買嗎？",
      "expected_answer": "可以，退款不影響再次購買資格",
      "expected_sources": ["退款政策.pdf", "FAQ.md"],
      "difficulty": "medium"
    }
  ]
}
```

**準確率計算**:
```
準確率 = (返回正確文檔的問題數 / 總問題數) × 100%

評分標準:
- 返回的 Top-1 結果包含正確答案: +1 分
- 返回的 Top-3 結果包含正確答案: +0.5 分
- 未返回正確答案: 0 分

目標: ≥90%（80% 是最低可接受門檻）
```

✅ 必須項（UI 介面）:
- [ ] 檢索策略配置頁面
- [ ] 參數滑桿調整（即時預覽效果）
- [ ] 測試查詢介面（輸入問題，查看檢索結果）
- [ ] 準確率 Dashboard
- [ ] 檢索結果解釋（為什麼返回這些結果）

**技術實現**:
- Azure AI Search Hybrid Search
- Cross-Encoder Re-ranking（HuggingFace）
- 測試框架（pytest + 自動化）

**📊 優先級**: P0 (MVP 必須，核心競爭力)
**🎯 準確率目標**: ≥90%（80% 是最低門檻）
**🔬 測試要求**: 通過 UAT 用戶驗收
**⏱️ 開發工期**: 2-3 週
**🔗 相關**: US 5.1 (文件上傳), ADR (Knowledge 架構)

---

#### US 5.3 - 知識庫管理與更新

**作為** IT 開發者
**我想要** 管理和更新 Agent 的知識庫文件
**以便** 維護知識庫的準確性和時效性

**驗收標準**:

✅ 必須項（文件列表管理）:
- [ ] 顯示所有已上傳的知識庫文件
- [ ] 文件列表包含：名稱、大小、上傳時間、狀態、Chunk 數量
- [ ] 支援搜尋（按文件名、標籤）
- [ ] 支援篩選（按日期、狀態、標籤）
- [ ] 支援排序（按名稱、時間、大小）
- [ ] 分頁顯示（每頁 20 筆）

✅ 必須項（文件更新）:
- [ ] 可重新上傳同名文件（覆蓋舊版本）
- [ ] 版本歷史記錄（保留最近 5 個版本）
- [ ] 重新索引功能（重新 Embedding + 更新向量資料庫）
- [ ] 更新後自動通知使用該知識庫的 Agent
- [ ] 支援批量重新索引

✅ 必須項（文件刪除）:
- [ ] 軟刪除（標記為已刪除，實際保留 30 天）
- [ ] 刪除確認對話框（防止誤刪）
- [ ] 顯示刪除影響（哪些 Agent 正在使用）
- [ ] 刪除後從向量資料庫移除索引
- [ ] 支援批量刪除
- [ ] 回收站功能（30 天內可恢復）

✅ 必須項（文件元數據管理）:
- [ ] 可編輯文件標籤（Tag）
- [ ] 可編輯文件描述
- [ ] 可設定文件優先級（檢索時影響排序）
- [ ] 可設定文件過期時間（過期自動標記）

**BDD 驗收標準**:

```gherkin
Scenario 1: 查看知識庫文件列表
  Given 已上傳 50 個知識庫文件
  When 開發者訪問知識庫管理頁面
  Then 顯示文件列表（分頁）
  And 每個文件顯示名稱、大小、上傳時間、Chunk 數量
  And 可以搜尋和篩選文件

Scenario 2: 重新索引文件
  Given 已上傳的文件內容已修改
  When 開發者點擊「重新索引」按鈕
  Then 系統重新提取文字、Chunking、Embedding
  And 更新向量資料庫索引
  And <5 分鐘完成（100 頁文檔）
  And 通知使用該知識庫的 Agent

Scenario 3: 刪除文件（軟刪除）
  Given 選中一個知識庫文件
  When 開發者點擊刪除
  Then 顯示確認對話框（列出影響的 Agent）
  And 確認後文件標記為已刪除
  And 從向量資料庫移除索引
  And 文件移至回收站（30 天內可恢復）

Scenario 4: 批量管理文件
  Given 選中 10 個文件
  When 開發者執行批量操作（重新索引/刪除/標籤）
  Then 所有文件依次處理
  And 顯示處理進度
  And 處理完成後顯示結果摘要
```

**技術要求**:
- **Backend**: ASP.NET Core 8 Web API
- **Database**: PostgreSQL (文件元數據), Azure AI Search (向量索引)
- **Storage**: Azure Blob Storage (文件存儲)
- **API Endpoints**:
  - `GET /api/v1/knowledge-base/files` (列表，支援搜尋、篩選、排序、分頁)
  - `PUT /api/v1/knowledge-base/files/{id}` (更新元數據)
  - `DELETE /api/v1/knowledge-base/files/{id}` (軟刪除)
  - `POST /api/v1/knowledge-base/files/{id}/reindex` (重新索引)
  - `POST /api/v1/knowledge-base/files/batch` (批量操作)
  - `GET /api/v1/knowledge-base/files/trash` (回收站)
  - `POST /api/v1/knowledge-base/files/{id}/restore` (恢復)

**UI/UX**:
- 文件列表頁（Table with filters）
- 文件詳情側邊欄（Metadata editor）
- 批量操作工具欄
- 回收站頁面

**安全需求**:
- RBAC 權限檢查（僅 Admin、Developer 可刪除）
- 刪除操作審計日誌
- 敏感文件警告標記

**性能標準**:
- 文件列表加載 <500ms（1000 筆記錄）
- 重新索引 100 頁文檔 <5 分鐘
- 批量操作（10 檔案）<10 分鐘

**測試場景**:
- 文件列表 CRUD 完整測試
- 搜尋、篩選、排序功能測試
- 重新索引邏輯測試
- 軟刪除和恢復流程測試
- 批量操作併發測試
- 權限驗證測試

**📊 優先級**: P0 (MVP 必須)
**🎯 Story Points**: 5
**⏱️ 預估時間**: 1 週
**🔗 相關**: US 5.1 (文件上傳), US 5.2 (檢索策略)

---

#### US 5.4 - Excel/CSV 數據源上傳 ⭐ 結構化數據

**作為** IT 開發者
**我想要** 上傳 Excel/CSV 文件作為 Agent 的知識庫
**以便** Agent 可以查詢和分析結構化數據

**驗收標準**:

✅ 必須項（文件處理）:
- [ ] 支援格式: .xlsx, .xls, .csv
- [ ] 自動檢測表頭和數據類型
- [ ] 處理多 Sheet Excel 文件（選擇要導入的 Sheet）
- [ ] 支援大文件（最多 100MB, 100 萬行）
- [ ] 數據預覽（前 100 行）
- [ ] 編碼自動檢測（UTF-8, Big5, GB2312）

✅ 必須項（Schema 理解）:
- [ ] 自動識別欄位名稱、類型、範例值
- [ ] 數據類型推斷（文字、數字、日期、布林）
- [ ] AI 生成欄位描述和業務含義
- [ ] 支援人工標註和修正
- [ ] 檢測主鍵、外鍵關係（多表場景）
- [ ] 數據質量報告（缺失值比例、異常值檢測）

✅ 必須項（語義標註）:
```
欄位名: revenue
類型: Number
描述: 銷售金額（自動生成或手動標註）
單位: 新台幣
別名: 營收、銷售額、收入
範例值: 50000, 12000, 8500
```

✅ 必須項（數據索引）:
- [ ] 將數據導入到數據庫（PostgreSQL 或專用表）
- [ ] 建立索引加速查詢
- [ ] 支援數據更新（覆蓋或追加）
- [ ] 數據版本管理

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ 步驟 2/3: Schema 理解與標註              │
├─────────────────────────────────────────┤
│ 📊 檢測到的表格結構: sales_data.xlsx    │
│                                          │
│ ┌──────┬──────┬──────────┬──────────┐  │
│ │欄位名│ 類型 │  描述    │  範例值  │  │
│ ├──────┼──────┼──────────┼──────────┤  │
│ │date  │ Date │ 銷售日期 │2024-01-01│  │
│ │product Text │ 產品名稱 │ iPhone   │  │
│ │revenue Number│ 銷售金額 │ 50000    │  │
│ │region│ Text │ 銷售地區 │ 台北     │  │
│ └──────┴──────┴──────────┴──────────┘  │
│                                          │
│ 🏷️ 語義標註:                            │
│ revenue: [銷售金額] 單位: [新台幣]      │
│          別名: [營收, 銷售額]           │
│ region:  [銷售地區] 別名: [區域]        │
│                                          │
│ 💡 AI 建議:                              │
│ 檢測到時間序列數據，建議啟用趨勢分析    │
│                                          │
│     [< 上一步]  [儲存並啟用]             │
└─────────────────────────────────────────┘
```

**📊 優先級**: P0 (MVP 必須，核心差異化)
**🎯 驗收方式**: 可成功導入 10 萬行 Excel 數據
**🔗 相關**: US 5.5 (Text-to-SQL), US 3.1 (Code Interpreter)

---

#### US 5.5 - 數據庫連接（Text-to-SQL）⭐ 核心能力

**作為** IT 開發者
**我想要** 連接數據庫作為 Agent 的知識源
**以便** Agent 可以直接查詢數據庫回答問題

**背景**:
這是選擇 Semantic Kernel 的重要原因之一。通過 Text-to-SQL，Agent 可以自然語言查詢結構化數據。

**驗收標準**:

✅ 必須項（連接管理）:
- [ ] 支援數據庫: SQL Server, PostgreSQL, MySQL
- [ ] 連接字串配置（加密儲存）
- [ ] 連接測試和健康檢查
- [ ] 連接池管理（避免連接耗盡）
- [ ] 連接逾時設定（5 秒）

✅ 必須項（Schema 抓取）:
- [ ] 自動抓取 Database Schema
- [ ] 提取表名、欄位名、資料類型
- [ ] 提取主鍵、外鍵關係
- [ ] 提取索引資訊
- [ ] 生成 ERD（實體關係圖）

✅ 必須項（Text-to-SQL 生成）:
- [ ] 自然語言 → SQL 查詢（使用 GPT-4）
- [ ] 支援基本查詢（SELECT, WHERE, ORDER BY）
- [ ] 支援聚合查詢（COUNT, SUM, AVG, GROUP BY）
- [ ] 支援 JOIN 查詢（多表關聯）
- [ ] 支援子查詢
- [ ] SQL 語法優化（效能考量）

✅ 必須項（SQL 安全性）⭐ 關鍵:

**唯讀權限**:
- [ ] 數據庫連接使用 **READ ONLY** 權限
- [ ] 禁止 DML: INSERT, UPDATE, DELETE
- [ ] 禁止 DDL: DROP, ALTER, CREATE
- [ ] 禁止系統表查詢（information_schema 除外）

**SQL Injection 防護**:
- [ ] 參數化查詢（Prepared Statements）
- [ ] SQL 白名單過濾（僅允許 SELECT）
- [ ] 特殊字元轉義
- [ ] SQL 語法驗證（防止惡意 SQL）

**執行限制**:
- [ ] 查詢超時: 5 秒（防止慢查詢）
- [ ] 結果集大小限制: 1000 行（防止記憶體溢出）
- [ ] 並發查詢限制: 每用戶 5 個/分鐘
- [ ] 查詢日誌審計（記錄所有 SQL）

✅ 必須項（執行流程）:
```
1. 用戶問題 → LLM 理解意圖
2. Schema 查詢 → 找到相關表和欄位
3. SQL 生成 → GPT-4 生成 SQL
4. SQL 驗證 → 安全性檢查、語法檢查
5. SQL 執行 → 數據庫查詢
6. 結果格式化 → 表格 + 可視化
7. 自然語言解釋 → Agent 解釋結果
```

**互動範例**:

**場景 1: 簡單查詢**
```
用戶: "上個月哪個產品賣最好？"

Agent: "我來查詢上個月的銷售數據..."

[生成 SQL]
SELECT product_name, SUM(revenue) as total_revenue
FROM sales
WHERE sale_date >= DATE_SUB(CURDATE(), INTERVAL 1 MONTH)
GROUP BY product_name
ORDER BY total_revenue DESC
LIMIT 1

[執行結果]
product_name | total_revenue
-------------|---------------
iPhone 15    | $1,250,000

Agent: "上個月銷售最好的產品是 **iPhone 15**，
       總銷售額達到 $1,250,000。"
```

**場景 2: 多表 JOIN 查詢**
```
用戶: "比較各地區的平均客單價"

Agent: "我需要關聯銷售表和訂單表..."

[生成 SQL]
SELECT
    r.region_name,
    AVG(o.total_amount) as avg_order_value,
    COUNT(o.order_id) as order_count
FROM orders o
JOIN customers c ON o.customer_id = c.customer_id
JOIN regions r ON c.region_id = r.region_id
WHERE o.order_date >= DATE_SUB(CURDATE(), INTERVAL 3 MONTH)
GROUP BY r.region_name
ORDER BY avg_order_value DESC

[執行結果 + 柱狀圖]

Agent: "各地區平均客單價分析：
🏆 台北: $1,200（訂單數: 1500）
📊 新竹: $1,150（訂單數: 800）
📉 台中: $950（訂單數: 2000）

新竹雖然訂單數較少，但客單價較高，
建議加強新竹地區的高端產品推廣。"
```

✅ 必須項（錯誤處理）:
- [ ] SQL 語法錯誤友好提示
- [ ] 查詢超時提示（建議優化查詢）
- [ ] 表/欄位不存在提示
- [ ] 權限不足提示
- [ ] Agent 自動修正 SQL 並重試（最多 2 次）

**技術實現**:
- ADO.NET / Dapper（數據庫訪問）
- GPT-4 生成 SQL（Few-Shot Prompting）
- SQL Parser（語法驗證）
- SQL Formatter（可讀性優化）

**📊 優先級**: P0 (MVP 必須，核心原因)
**🎯 準確率目標**: SQL 生成成功率 >85%
**🔒 安全要求**: 通過安全審計，零 SQL Injection 風險
**⏱️ 開發工期**: 4-5 週
**🔗 相關**: US 5.4 (Excel/CSV), US 5.6 (可視化)

---

#### US 5.6 - 智能數據可視化（自動圖表生成）

**作為** 業務用戶
**我想要** Agent 自動將查詢結果可視化
**以便** 更直觀理解數據

**驗收標準**:

✅ 必須項（自動圖表選擇）:
- [ ] 根據數據類型自動選擇圖表
- [ ] 時間 + 數值 → 折線圖/區域圖
- [ ] 類別 + 數值 → 柱狀圖/條形圖
- [ ] 兩個數值 → 散點圖
- [ ] 比例數據 → 圓餅圖/環形圖
- [ ] 多維度數據 → 熱力圖/堆疊柱狀圖

✅ 必須項（支援圖表類型）:
- [ ] 折線圖（Line Chart）
- [ ] 柱狀圖（Bar Chart）
- [ ] 圓餅圖（Pie Chart）
- [ ] 散點圖（Scatter Plot）
- [ ] 熱力圖（Heatmap）
- [ ] 區域圖（Area Chart）
- [ ] 箱型圖（Box Plot）

✅ 必須項（圖表功能）:
- [ ] 多維度數據自動分組和聚合
- [ ] 圖表標題自動生成（基於查詢問題）
- [ ] 軸標籤自動設定（單位、格式）
- [ ] 圖例自動生成
- [ ] 中文顯示支援
- [ ] 顏色主題配置

✅ 必須項（互動功能）:
- [ ] 圖表縮放（Zoom）
- [ ] 圖表平移（Pan）
- [ ] Tooltip 顯示詳細數據
- [ ] 圖例點擊顯示/隱藏數據系列
- [ ] 圖表可下載（PNG, SVG, PDF）

**智能推薦邏輯**:
```javascript
function selectChartType(data) {
  const columnTypes = analyzeColumnTypes(data);

  // 時間序列數據
  if (hasTimeColumn(columnTypes) && hasNumericColumn(columnTypes)) {
    return 'line_chart'; // 折線圖
  }

  // 類別數據
  if (hasCategoryColumn(columnTypes) && hasNumericColumn(columnTypes)) {
    if (data.length <= 10) {
      return 'bar_chart'; // 柱狀圖
    } else {
      return 'horizontal_bar_chart'; // 條形圖（數據多時橫向更易讀）
    }
  }

  // 兩個數值欄位
  if (numericColumnCount(columnTypes) >= 2) {
    return 'scatter_plot'; // 散點圖
  }

  // 比例數據（加總為 100%）
  if (isTotalPercentage(data)) {
    return 'pie_chart'; // 圓餅圖
  }

  // 預設
  return 'table'; // 表格
}
```

**技術實現**:
- Plotly.js / ECharts（圖表庫）
- D3.js（進階可視化，Phase 2）
- Chart.js（輕量級選項）

**📊 優先級**: P1 (MVP 高優先)
**🎯 UX 目標**: 90% 數據自動選擇正確圖表類型
**🔗 相關**: US 5.5 (Text-to-SQL), US 3.2 (Code Interpreter 可視化)

---

#### US 5.7 - 跨數據源關聯查詢

**作為** IT 開發者
**我想要** Agent 能夠關聯多個數據源進行查詢
**以便** 回答複雜的跨源問題

**驗收標準**:

✅ 必須項（關聯定義）:
- [ ] 手動定義數據源間的關聯關係
- [ ] 指定關聯鍵（例如: sales.product_id = products.id）
- [ ] 支援一對一、一對多關聯
- [ ] 關聯關係可視化展示（ERD）

✅ 必須項（跨源查詢）:
- [ ] Agent 自動識別需要關聯的數據源
- [ ] 生成跨源查詢計劃
- [ ] 本地 JOIN 處理（當無法在資料庫層 JOIN 時）
- [ ] 查詢結果合併和去重

**範例場景**:
```
數據源 1: sales.xlsx（銷售數據）
- product_id
- quantity
- revenue

數據源 2: SQL DB products 表（產品詳情）
- id
- name
- category
- price

關聯: sales.product_id = products.id

用戶問: "銷售前 10 的產品類別是什麼？"

Agent 查詢計劃:
1. 從 sales.xlsx 查詢銷售 Top 10 產品（按 revenue 排序）
2. 提取 product_id 列表
3. 從 SQL DB 查詢這些產品的類別
4. JOIN 並聚合結果
5. 返回: 類別分佈表 + 圓餅圖
```

**技術實現**:
- 數據整合層（Data Integration Layer）
- 本地 SQL 引擎（SQLite in-memory）
- 查詢優化器

**📊 優先級**: P2 (Phase 2)
**🎯 技術難度**: High
**🔗 相關**: US 5.4 (Excel), US 5.5 (SQL)

---# 模組 06: Chat/對話介面

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 06
**User Stories**: US 6.1-6.6
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 6: Chat/對話介面 ⭐ 多模態增強

#### Epic 6: 多模態對話體驗

**目標**: 提供豐富的多模態對話介面，支援文字、圖片、圖表、代碼

---

#### US 6.1 - 文字對話（基礎）

**作為** 業務用戶
**我想要** 通過文字與 Agent 對話
**以便** 完成我的任務

**驗收標準**:

✅ 必須項（輸入功能）:
- [ ] 文字輸入框（支援多行，自動調整高度）
- [ ] Enter 發送（Shift+Enter 換行）
- [ ] 發送按鈕
- [ ] 輸入字數統計（剩餘字數提示）
- [ ] 輸入字數限制（2000 字）

✅ 必須項（Streaming 顯示）:
- [ ] 逐字顯示 Agent 回應（打字機效果）
- [ ] Streaming 速度自然（不太快不太慢）
- [ ] 支援中斷 Streaming（停止生成按鈕）
- [ ] Streaming 過程中可滾動查看
- [ ] Streaming 完成後自動捲動到底部

✅ 必須項（Markdown 支援）:
- [ ] 粗體、斜體、刪除線
- [ ] 標題（H1-H6）
- [ ] 列表（有序、無序）
- [ ] 連結（可點擊）
- [ ] 引用塊
- [ ] 表格

✅ 必須項（對話管理）:
- [ ] 對話歷史自動保存
- [ ] 會話列表（可切換不同對話）
- [ ] 新建對話
- [ ] 重命名對話
- [ ] 刪除對話（軟刪除）
- [ ] 匯出對話（Markdown/PDF）

✅ 必須項（上下文管理）:
- [ ] 多輪對話上下文保持
- [ ] 上下文 Token 數量顯示
- [ ] 上下文過長時自動截斷（保留最近 N 輪）
- [ ] 支援手動清除上下文（重新開始）

**UI 設計要求**:
- 類似 ChatGPT/Claude 的對話介面
- 用戶訊息右對齊（藍色氣泡）
- Agent 訊息左對齊（灰色氣泡）
- 清晰的時間戳記

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 對話延遲 <500ms 感知
**🔗 相關**: US 6.2-6.6 (多模態功能)

---

#### US 6.2 - 圖片展示與上傳 ⭐ 多模態

**作為** 業務用戶
**我想要** 在對話中看到圖片和上傳圖片
**以便** 更豐富的互動體驗

**驗收標準**:

✅ 必須項（圖片顯示）:
- [ ] Agent 回應中的圖片自動內嵌展示
- [ ] 圖片點擊放大預覽（Lightbox）
- [ ] 支援格式: PNG, JPG, JPEG, GIF, SVG
- [ ] 圖片載入失敗顯示佔位符（錯誤訊息）
- [ ] 圖片 Lazy Loading（滾動時才載入）
- [ ] 圖片 Alt 文字（無障礙支援）

✅ 必須項（圖片上傳）:
- [ ] 點擊上傳按鈕選擇圖片
- [ ] 拖拽上傳支援（Drag & Drop）
- [ ] 圖片預覽（上傳前）
- [ ] 上傳進度顯示
- [ ] 圖片大小限制: 10MB
- [ ] 支援批量上傳（最多 5 張）
- [ ] 圖片壓縮（自動縮小過大圖片）

✅ 必須項（圖片功能）:
- [ ] 圖片下載按鈕
- [ ] 複製圖片到剪貼簿
- [ ] 圖片分享（生成分享連結）

**應用場景**:
- 上傳數據圖表讓 Agent 分析
- Agent 返回視覺化分析結果
- 上傳產品圖片進行辨識（Phase 2: Vision 支援）
- 上傳錯誤截圖請求協助

**技術要求**:
- Azure Blob Storage（圖片存儲）
- Image Compression Library（壓縮）
- Lightbox Library（預覽）

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 圖片上傳 <3 秒（5MB 圖片）
**🔗 相關**: US 6.3 (圖表), US 3.2 (Code Interpreter 圖片)

---

#### US 6.3 - 圖表互動展示（來自 Code Interpreter）⭐ 核心價值

**作為** 業務用戶
**我想要** 在對話中看到互動式圖表
**以便** 深入探索數據分析結果

**驗收標準**:

✅ 必須項（靜態圖表 - MVP）:
- [ ] 顯示 Code Interpreter 生成的圖表
- [ ] 圖表內嵌在對話流中
- [ ] 支援 PNG, SVG 格式
- [ ] 圖表可點擊放大
- [ ] 圖表可下載

✅ 進階項（互動圖表 - Phase 2）:
- [ ] 使用 Plotly/ECharts 生成互動圖表
- [ ] 支援縮放（Zoom In/Out）
- [ ] 支援平移（Pan）
- [ ] Tooltip 顯示詳細數據
- [ ] 圖例點擊顯示/隱藏數據系列
- [ ] 圖表可匯出為圖片或數據

**展示範例**:
```
[對話框]
用戶: "分析這三個月的銷售趨勢"

Agent:
"我來分析銷售趨勢數據...

[代碼執行中...]

分析完成！以下是銷售趨勢圖：

[圖表: sales_trend.png]
📈 互動提示: 點擊圖表可放大查看

從圖表可以看出：
• 1月銷售額: $500,000（基準）
• 2月銷售額: $650,000（增長 30%）
• 3月銷售額: $750,000（增長 15%）

💡 總體趨勢: 穩定增長
⚠️ 注意: 3月增速放緩，建議加強促銷活動"
```

**技術實現**:
- 圖片存儲: Azure Blob Storage
- 圖表庫: Plotly.js / ECharts（Phase 2）
- 圖片 CDN 加速

**📊 優先級**: P0 (MVP 必須 - 靜態圖表)
**🎯 UX 目標**: 圖表載入 <1 秒
**🔗 相關**: US 3.2 (代碼生成圖表), US 5.6 (SQL 可視化)

---

#### US 6.4 - 代碼塊展示與複製 ⭐ 開發者友好

**作為** 業務用戶/開發者
**我想要** 清晰展示 Agent 生成或執行的代碼
**以便** 理解 Agent 的邏輯和複用代碼

**驗收標準**:

✅ 必須項（代碼展示）:
- [ ] 代碼塊語法高亮（支援 Python, JavaScript, SQL, C#, JSON, YAML 等）
- [ ] 語言標籤顯示（```python → 顯示 "Python"）
- [ ] 一鍵複製代碼按鈕（點擊後顯示 "已複製"）
- [ ] 行號顯示（可選開關）
- [ ] 深色/淺色主題適配（跟隨系統或用戶設定）
- [ ] 長代碼可折疊/展開（超過 20 行自動折疊）

✅ 必須項（代碼功能）:
- [ ] 代碼格式化（自動縮排、對齊）
- [ ] 代碼高亮選中行（點擊行號）
- [ ] 代碼搜尋功能（Ctrl+F 在代碼塊內搜尋）
- [ ] 代碼下載（儲存為 .py, .js 等文件）

**UI 參考**:
- GitHub 代碼塊樣式
- Claude/ChatGPT 代碼展示
- VS Code 語法高亮

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ Agent 回應:                              │
│                                          │
│ 這段代碼可以分析銷售數據：               │
│                                          │
│ ┌───────────────────────────────────┐   │
│ │ ```python          [複製] [▼折疊] │   │
│ │  1  import pandas as pd           │   │
│ │  2  import matplotlib.pyplot as...│   │
│ │  3                                │   │
│ │  4  # 載入數據                     │   │
│ │  5  data = pd.read_csv('sales.csv'│   │
│ │  ...                              │   │
│ │ 20  plt.savefig('chart.png')      │   │
│ │ [展開顯示更多]                     │   │
│ └───────────────────────────────────┘   │
│                                          │
│ 執行這段代碼即可生成趨勢圖。             │
└─────────────────────────────────────────┘
```

**技術要求**:
- Syntax Highlighter: Prism.js 或 Highlight.js
- Clipboard API（複製功能）
- Line Numbers Plugin

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 代碼高亮渲染 <100ms
**🔗 相關**: US 3.3 (Code Interpreter 展示)

---

#### US 6.5 - 表格數據展示

**作為** 業務用戶
**我想要** 在對話中看到結構化的表格數據
**以便** 快速理解數據結果

**驗收標準**:

✅ 必須項（表格展示）:
- [ ] Markdown 表格自動渲染
- [ ] 表格可排序（點擊表頭按欄位排序）
- [ ] 表格可搜尋（內建搜尋框）
- [ ] 大表格支援分頁（每頁 20 行）
- [ ] 表格可匯出（CSV/Excel）
- [ ] 響應式設計（移動端橫向滾動）

✅ 必須項（表格美化）:
- [ ] 斑馬紋（奇偶行不同背景色）
- [ ] Hover 效果（滑鼠懸停高亮）
- [ ] 對齊方式（數字右對齊、文字左對齊）
- [ ] 數字格式化（千分位、小數點）
- [ ] 長文字省略（Tooltip 顯示完整內容）

**展示範例**:
```
Agent: "以下是各地區銷售統計：

| 地區 | 銷售額 | 增長率 | 訂單數 |
|------|--------|--------|--------|
| 台北 | $1,000,000 | +15% | 1,500 |
| 新竹 | $800,000 | +25% | 800 |
| 台中 | $600,000 | -5% | 2,000 |

[搜尋: ___] [排序: 銷售額 ▼] [匯出 CSV]"
```

**技術實現**:
- React Table 或 TanStack Table
- 排序和篩選邏輯
- CSV 匯出庫（PapaParse）

**📊 優先級**: P1 (MVP 高優先)
**🎯 UX 目標**: 表格渲染 <200ms（100 行數據）
**🔗 相關**: US 5.5 (SQL 查詢結果), US 5.4 (Excel 數據)

---

#### US 6.6 - 移動端友好設計（RWD）

**作為** 移動端用戶
**我想要** 在手機上也能順暢使用 Chat 介面
**以便** 隨時隨地與 Agent 互動

**驗收標準**:

✅ 必須項（響應式佈局）:
- [ ] 支援螢幕寬度: 320px - 1920px
- [ ] 自動適配手機、平板、桌面
- [ ] 縱向和橫向模式支援
- [ ] 字體大小自動調整（em/rem 單位）
- [ ] 按鈕和點擊區域足夠大（至少 44px）

✅ 必須項（觸控手勢）:
- [ ] 滑動手勢切換對話（左右滑動）
- [ ] 下拉刷新（Pull to Refresh）
- [ ] 捏合縮放圖片（Pinch to Zoom）
- [ ] 長按複製文字
- [ ] 雙擊放大圖片

✅ 必須項（虛擬鍵盤適配）:
- [ ] 輸入框不被虛擬鍵盤遮擋
- [ ] 鍵盤彈出時自動捲動到輸入框
- [ ] 支援鍵盤工具列（格式化按鈕）

✅ 必須項（性能優化）:
- [ ] 圖片自適應大小（避免載入過大圖片）
- [ ] 表格橫向滾動（避免破版）
- [ ] Lazy Loading（延遲載入非可見內容）
- [ ] 減少移動端流量消耗（圖片壓縮）

✅ 必須項（離線支援 - Phase 2）:
- [ ] 對話歷史離線可查看
- [ ] Service Worker 緩存
- [ ] 離線狀態提示

**測試設備**:
- iPhone SE（小螢幕）
- iPhone 15 Pro（標準螢幕）
- iPad（平板）
- Android 手機（多種解析度）

**技術要求**:
- CSS Media Queries
- Flexbox/Grid 佈局
- Touch Events API
- Viewport Meta Tag

**📊 優先級**: P1 (MVP 高優先)
**🎯 UX 目標**: 移動端體驗與桌面端一致
**🔗 相關**: 所有 Chat UI 功能

---# 模組 07: Persona Framework

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 07
**User Stories**: US 7.1-7.4
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 7: Persona Framework ⭐ 核心差異化能力

**Epic**: 實現可配置的 Agent 個性化系統，讓每個 Agent 具有獨特的對話風格、行為模式和專業領域

**差異化價值**:
- 超越 Copilot Studio 的個性化能力
- 讓 Agent 不只是工具，而是具有個性的助手
- 提升用戶體驗和情感連結

**User Stories 數量**: 4 個（US 7.1 - 7.4）

---

#### US 7.1 - Persona 模板配置

**作為** 業務分析師
**我想要** 透過配置檔定義 Agent 的個性和行為
**以便** 快速建立具有特定風格的 Agent

**驗收標準**:

✅ 必須項（Persona 配置）:
- [ ] 支援 JSON/YAML 格式的 Persona 配置檔
- [ ] 定義 Agent 的姓名、角色、專業領域
- [ ] 定義對話風格（正式/輕鬆、簡潔/詳細）
- [ ] 定義回應模式（主動/被動、探索式/指令式）
- [ ] 定義專業術語使用偏好
- [ ] 定義禁止使用的詞彙或主題（安全守則）

✅ 必須項（預設 Persona 模板）:
- [ ] 提供 5+ 個預設模板（專業顧問、技術專家、客服助理、創意夥伴、數據分析師）
- [ ] 每個模板包含完整配置範例
- [ ] 模板可直接使用或作為起點修改

✅ 必須項（驗證機制）:
- [ ] 配置檔格式驗證（Schema Validation）
- [ ] 必填欄位檢查
- [ ] 禁止詞彙清單驗證
- [ ] 配置衝突檢測（例如：正式風格 + 過於隨意的用詞）

**配置範例**:
```yaml
# persona-config.yaml
persona:
  name: "Alice"
  role: "技術顧問"
  expertise: ["雲端架構", "DevOps", "安全性"]

  communication_style:
    formality: "professional"      # professional, casual, friendly
    verbosity: "balanced"           # concise, balanced, detailed
    tone: "encouraging"             # neutral, encouraging, assertive

  response_pattern:
    proactivity: "moderate"         # passive, moderate, proactive
    style: "exploratory"            # directive, exploratory, collaborative

  language_preferences:
    technical_terms: "always_explain"  # use_freely, explain_when_complex, always_explain
    examples: "provide_when_helpful"   # minimal, provide_when_helpful, abundant

  safety_guardrails:
    forbidden_topics: ["政治", "宗教"]
    forbidden_words: ["絕對", "保證", "一定"]
    max_speculation: "low"          # none, low, moderate

  personality_traits:
    - "耐心解釋複雜概念"
    - "使用類比和實例"
    - "鼓勵最佳實踐"
    - "避免過度技術術語"
```

**技術實現**:
- JSON Schema 或 YAML Schema 驗證
- Persona 配置載入器
- 預設模板資料庫
- 配置熱重載（開發模式）

**📊 優先級**: P0 (MVP 必須) - 核心差異化能力
**🎯 UX 目標**: 配置檔載入 <100ms
**🔗 相關**: US 1.2 (Agent 配置), US 7.2 (Prompt Engineering)

---

#### US 7.2 - Persona-Driven Prompt Engineering

**作為** AI Agent
**我想要** 根據 Persona 配置動態調整我的 System Prompt
**以便** 展現一致的個性和行為

**驗收標準**:

✅ 必須項（Prompt 模板系統）:
- [ ] System Prompt 模板引擎（支援變數插值）
- [ ] Persona 配置自動轉換為 Prompt 指令
- [ ] 多層次 Prompt 組合（基礎 Prompt + Persona Prompt + Task Prompt）
- [ ] Prompt 版本控制和 A/B 測試支援

✅ 必須項（Persona 行為注入）:
- [ ] 對話風格指令自動生成（"以專業但友善的語氣回應..."）
- [ ] 回應長度控制（根據 verbosity 設定）
- [ ] 範例提供策略（根據 examples 偏好）
- [ ] 專業術語解釋策略（根據 technical_terms 設定）

✅ 必須項（安全守則注入）:
- [ ] 禁止主題和詞彙自動加入 Prompt
- [ ] 思辨限制設定（"避免過度推測..."）
- [ ] 道德和法律守則（"拒絕不當請求..."）
- [ ] 品牌和語氣一致性守則

✅ 必須項（動態 Prompt 調整）:
- [ ] 根據對話歷史調整 Prompt（例如：用戶多次要求簡潔 → 自動切換到簡潔模式）
- [ ] 根據任務類型調整（例如：程式碼生成 → 增加技術細節）
- [ ] A/B 測試不同 Prompt 版本

**Prompt 生成範例**:
```python
# 基於 Persona 配置自動生成的 System Prompt

# === 基礎 Prompt ===
You are Alice, a professional technical consultant specializing in cloud architecture, DevOps, and security.

# === 風格指令（基於 communication_style）===
- Maintain a professional and encouraging tone
- Provide balanced detail level (neither too brief nor too verbose)
- Use exploratory approach: ask clarifying questions when needed

# === 語言策略（基於 language_preferences）===
- Always explain technical terms when first introduced
- Provide examples when helpful for understanding
- Use analogies to clarify complex concepts

# === 安全守則（基於 safety_guardrails）===
- NEVER discuss politics or religion
- AVOID using absolute terms like "絕對", "保證", "一定"
- Keep speculation to minimum; clearly mark uncertain information

# === 個性特質（基於 personality_traits）===
- Be patient when explaining complex concepts
- Use real-world examples and analogies
- Encourage best practices
- Avoid unnecessary technical jargon
```

**技術實現**:
- Jinja2 或 Handlebars 模板引擎
- Prompt 組合邏輯
- 動態 Prompt 調整演算法
- A/B 測試框架

**📊 優先級**: P0 (MVP 必須) - 核心實現
**🎯 技術目標**: Prompt 生成 <50ms
**🔗 相關**: US 7.1 (Persona 配置), US 1.3 (LLM 整合)

---

#### US 7.3 - 對話風格一致性驗證

**作為** 品質保證人員
**我想要** 驗證 Agent 的回應是否符合 Persona 定義
**以便** 確保對話品質和一致性

**驗收標準**:

✅ 必須項（風格指標檢測）:
- [ ] 語氣分析（Sentiment Analysis）：檢測回應是否符合設定的 tone
- [ ] 詳細程度檢測（Response Length Analysis）：驗證是否符合 verbosity 設定
- [ ] 專業術語使用檢測：確認術語解釋策略是否執行
- [ ] 禁用詞彙掃描：自動檢測 forbidden_words 和 forbidden_topics

✅ 必須項（一致性評分）:
- [ ] 對每個回應計算 Persona 一致性分數（0-100%）
- [ ] 分數過低時自動告警（例如 <70%）
- [ ] 提供改進建議（"回應過於簡短，建議增加範例"）
- [ ] 持續監控和趨勢分析

✅ 必須項（測試框架）:
- [ ] Persona 單元測試（給定相同問題，不同 Persona 應有不同回應風格）
- [ ] 回歸測試（Persona 更新後，確保風格改變符合預期）
- [ ] 對話流程測試（多輪對話中 Persona 保持一致）

✅ 必須項（視覺化儀表板）:
- [ ] 即時風格一致性監控
- [ ] 違反安全守則的告警
- [ ] 對話品質趨勢圖
- [ ] Persona 表現對比（不同 Agent 的風格差異）

**檢測範例**:
```python
# 風格檢測報告
{
  "message_id": "msg_12345",
  "persona": "Alice (技術顧問)",
  "consistency_score": 85,  # 總體一致性分數

  "analysis": {
    "tone": {
      "expected": "professional + encouraging",
      "detected": "professional",
      "score": 90,
      "note": "缺少鼓勵性語氣"
    },
    "verbosity": {
      "expected": "balanced",
      "word_count": 150,
      "score": 95,
      "note": "符合預期"
    },
    "technical_terms": {
      "terms_used": ["Kubernetes", "CI/CD"],
      "explained": ["Kubernetes"],
      "not_explained": ["CI/CD"],
      "score": 70,
      "note": "CI/CD 應該解釋"
    },
    "forbidden_content": {
      "violations": [],
      "score": 100
    }
  },

  "recommendations": [
    "增加鼓勵性語句，例如「這是個好問題！」",
    "解釋 CI/CD 概念，因為設定為 always_explain"
  ]
}
```

**技術實現**:
- NLP 語氣分析（HuggingFace Transformers）
- 詞彙掃描和模式匹配
- 統計分析和評分演算法
- 即時監控儀表板（Grafana 或自訂前端）

**📊 優先級**: P1 (MVP 高優先) - 品質保證
**🎯 技術目標**: 檢測延遲 <200ms（不阻塞回應）
**🔗 相關**: US 7.2 (Prompt Engineering), US 10.2 (品質監控)

---

#### US 7.4 - Persona 演化和學習（Phase 2）

**作為** 系統管理員
**我想要** Agent 能從使用者互動中學習和優化 Persona
**以便** 持續改善用戶體驗

**驗收標準**:

✅ 必須項（使用者反饋收集）:
- [ ] 每個回應都有「滿意度」按鈕（👍 / 👎）
- [ ] 詳細反饋表單（"回應太簡短" / "語氣不對" / "太多技術術語"）
- [ ] 反饋自動關聯到 Persona 配置項目
- [ ] 反饋趨勢分析

✅ 必須項（自動優化建議）:
- [ ] 基於反饋數據分析 Persona 弱點
- [ ] 自動生成優化建議（"考慮將 verbosity 從 balanced 改為 detailed"）
- [ ] A/B 測試優化方案
- [ ] 優化效果驗證

✅ 可選項（AI 驅動的 Persona 調整 - Phase 2）:
- [ ] 使用 LLM 分析對話品質
- [ ] 自動微調 Persona 參數
- [ ] 個性化 Persona（每個用戶有客製化體驗）
- [ ] Persona 版本管理和回滾

**反饋分析範例**:
```yaml
# Persona 優化報告
agent_id: "alice_tech_consultant"
analysis_period: "2025-10-01 to 2025-10-31"

feedback_summary:
  total_interactions: 1500
  thumbs_up: 1200 (80%)
  thumbs_down: 300 (20%)

issues_identified:
  - issue: "回應太簡短"
    frequency: 120 occurrences (40% of complaints)
    related_config: "verbosity: balanced"
    recommendation: "建議改為 verbosity: detailed"

  - issue: "太多技術術語"
    frequency: 90 occurrences (30% of complaints)
    related_config: "technical_terms: use_freely"
    recommendation: "建議改為 technical_terms: explain_when_complex"

optimization_suggestions:
  - action: "調整 verbosity 設定"
    impact: "預期滿意度提升至 85%"
    ab_test: "已建立 A/B 測試（對照組 vs 實驗組）"
```

**技術實現**:
- 反饋資料收集和儲存
- 數據分析和模式識別
- A/B 測試框架
- （Phase 2）LLM-driven Persona 微調

**📊 優先級**: P2 (Phase 2) - 進階優化
**🎯 業務目標**: 滿意度提升 10%+
**🔗 相關**: US 7.3 (風格驗證), US 10.3 (使用者分析)

---# 模組 08: Structured Data + Text-to-SQL

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 08
**User Stories**: US 8.1-8.4
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 8: Structured Data + Text-to-SQL ⭐ 核心差異化能力

**Epic**: 讓 Agent 能夠理解和查詢結構化數據（SQL、Excel、CSV），並自動生成安全的 SQL 查詢

**差異化價值**:
- 超越 Copilot Studio 的數據查詢能力
- 自然語言轉 SQL，無需技術背景
- 企業級安全防護（SQL Injection、權限控制）

**User Stories 數量**: 4 個（US 8.1 - 8.4）

---

#### US 8.1 - Schema 理解與上下文構建

**作為** IT 開發者
**我想要** Agent 能夠理解資料庫 Schema 和表關係
**以便** 準確回答業務查詢

**驗收標準**:

✅ 必須項（Schema 自動發現）:
- [ ] 支援主流資料庫（SQL Server、PostgreSQL、MySQL、SQLite）
- [ ] 自動讀取 Table Schema（表名、欄位名、資料型別）
- [ ] 自動偵測 Primary Key 和 Foreign Key 關係
- [ ] 自動提取 Index 和 Constraint 資訊
- [ ] 支援 Schema 快取（避免每次查詢都重新讀取）

✅ 必須項（Schema 語義增強）:
- [ ] 允許為表和欄位添加業務描述（例如：`customer_id` → "客戶唯一識別碼"）
- [ ] 支援同義詞配置（例如：`訂單` = `order` = `orders`）
- [ ] 支援業務術語對應（例如：`銷售額` = `SUM(amount)`）
- [ ] Schema 版本控制（Schema 變更時自動更新）

✅ 必須項（上下文構建）:
- [ ] 將 Schema 資訊轉換為 LLM 友好的描述
- [ ] 包含表關係說明（"customers 和 orders 通過 customer_id 關聯"）
- [ ] 包含業務邏輯說明（"銷售額 = 單價 × 數量"）
- [ ] 包含範例查詢（Few-shot Learning）

**Schema 描述範例**:
```yaml
# Schema Context for LLM

tables:
  - name: customers
    description: "客戶資料表，儲存所有客戶基本資訊"
    columns:
      - name: customer_id
        type: INT
        description: "客戶唯一識別碼"
        primary_key: true
      - name: name
        type: VARCHAR(100)
        description: "客戶姓名"
      - name: email
        type: VARCHAR(100)
        description: "客戶電子郵件"

  - name: orders
    description: "訂單資料表，儲存所有訂單紀錄"
    columns:
      - name: order_id
        type: INT
        description: "訂單唯一識別碼"
        primary_key: true
      - name: customer_id
        type: INT
        description: "客戶ID（關聯到 customers 表）"
        foreign_key: customers.customer_id
      - name: amount
        type: DECIMAL(10,2)
        description: "訂單金額"
      - name: order_date
        type: DATE
        description: "訂單日期"

relationships:
  - type: "one-to-many"
    from: "customers.customer_id"
    to: "orders.customer_id"
    description: "一個客戶可以有多筆訂單"

business_terms:
  - term: "銷售額"
    definition: "SUM(orders.amount)"
  - term: "客戶總消費"
    definition: "SUM(orders.amount) GROUP BY customer_id"

synonyms:
  客戶: ["customer", "顧客", "用戶"]
  訂單: ["order", "訂購單"]

example_queries:
  - question: "列出所有客戶"
    sql: "SELECT * FROM customers;"
  - question: "查詢最近一個月的訂單"
    sql: "SELECT * FROM orders WHERE order_date >= DATEADD(month, -1, GETDATE());"
```

**技術實現**:
- JDBC/ODBC Schema Metadata API
- Schema Cache（Redis 或 In-Memory）
- Schema 語義增強配置檔（YAML）
- LLM Context Builder

**📊 優先級**: P0 (MVP 必須) - Text-to-SQL 基礎
**🎯 技術目標**: Schema 載入 <500ms（含快取）
**🔗 相關**: US 8.2 (Text-to-SQL 生成), US 5.4 (Excel 資料)

---

#### US 8.2 - 安全的 Text-to-SQL 生成

**作為** 業務用戶
**我想要** 用自然語言查詢資料庫
**以便** 無需學習 SQL 語法即可獲取資料

**驗收標準**:

✅ 必須項（自然語言理解）:
- [ ] 支援中文和英文查詢
- [ ] 理解複雜查詢（JOIN、聚合、子查詢、排序）
- [ ] 理解模糊查詢（"類似 ABC 的客戶" → `LIKE '%ABC%'`）
- [ ] 理解時間範圍（"最近一週" → `WHERE date >= DATEADD(week, -1, GETDATE())`）
- [ ] 理解業務術語（"銷售額" → `SUM(amount)`）

✅ 必須項（SQL 生成品質）:
- [ ] 生成可執行的標準 SQL
- [ ] 自動選擇最佳 JOIN 策略
- [ ] 避免笛卡爾積和性能問題
- [ ] 添加適當的 WHERE 條件避免全表掃描
- [ ] 限制查詢結果數量（預設 TOP 1000）

✅ 必須項（安全防護 - SQL Injection）:
- [ ] 使用參數化查詢（Prepared Statements）
- [ ] 禁止執行 DDL 語句（CREATE、DROP、ALTER）
- [ ] 禁止執行 DML 語句（INSERT、UPDATE、DELETE）- 僅允許 SELECT
- [ ] 禁止執行系統命令（xp_cmdshell、EXEC 等）
- [ ] SQL 語句黑名單過濾
- [ ] 自動偵測惡意 SQL 模式

✅ 必須項（查詢驗證）:
- [ ] 生成 SQL 後先進行語法驗證
- [ ] 執行前估算查詢成本（避免過於複雜的查詢）
- [ ] 設定執行時間限制（例如 30 秒）
- [ ] 設定記憶體限制（避免 OOM）
- [ ] 失敗後提供友善的錯誤訊息

**Text-to-SQL 範例**:
```python
# 使用者查詢
"顯示最近一個月銷售額最高的前 10 名客戶"

# Agent 內部流程
Step 1: 理解意圖
- 需要表: customers, orders
- 時間範圍: 最近一個月
- 聚合: SUM(amount)
- 排序: DESC
- 限制: TOP 10

Step 2: 生成 SQL（含安全防護）
generated_sql = """
SELECT TOP 10
    c.customer_id,
    c.name,
    SUM(o.amount) AS total_sales
FROM customers c
INNER JOIN orders o ON c.customer_id = o.customer_id
WHERE o.order_date >= DATEADD(month, -1, GETDATE())
GROUP BY c.customer_id, c.name
ORDER BY total_sales DESC
"""

Step 3: 安全檢查
✅ 僅使用 SELECT
✅ 無 DDL/DML
✅ 無系統命令
✅ 使用參數化查詢（時間範圍）
✅ 限制結果數量（TOP 10）

Step 4: 驗證和執行
- 語法驗證: ✅ 通過
- 成本估算: ✅ 可接受（預估 100ms）
- 執行查詢: ✅ 成功
- 結果: 10 筆客戶資料

Step 5: 格式化回應
"以下是最近一個月銷售額最高的前 10 名客戶：

| 客戶ID | 姓名 | 總銷售額 |
|--------|------|----------|
| 1001   | ABC  | $50,000  |
| 1002   | XYZ  | $48,000  |
...
"
```

**技術實現**:
- LLM Text-to-SQL（GPT-4、Claude、或開源模型）
- SQL Parser 和 Validator
- Prepared Statement Engine
- SQL 黑名單和模式檢測
- Query Cost Estimator

**📊 優先級**: P0 (MVP 必須) - 核心功能
**🎯 準確率目標**: >85%（簡單查詢）、>70%（複雜查詢）
**🔗 相關**: US 8.1 (Schema 理解), US 8.3 (權限控制)

---

#### US 8.3 - 細粒度權限控制（Row-Level Security）

**作為** 系統管理員
**我想要** 控制不同用戶能查詢哪些資料
**以便** 確保資料安全和隱私合規

**驗收標準**:

✅ 必須項（表級權限）:
- [ ] 配置每個用戶/角色可存取的表
- [ ] 支援白名單模式（僅允許明確授權的表）
- [ ] 支援黑名單模式（禁止特定敏感表）
- [ ] 動態權限檢查（每次查詢前驗證）

✅ 必須項（欄位級權限）:
- [ ] 配置每個用戶/角色可查詢的欄位
- [ ] 自動過濾敏感欄位（例如：密碼、信用卡號）
- [ ] 支援欄位遮罩（例如：顯示 `***-**-1234` 而非完整身分證）
- [ ] 禁止查詢未授權欄位（自動從 SELECT 中移除）

✅ 必須項（行級權限 - Row-Level Security）:
- [ ] 根據用戶角色自動添加 WHERE 條件
- [ ] 支援多租戶隔離（`WHERE tenant_id = @current_user_tenant`）
- [ ] 支援部門隔離（`WHERE department = @current_user_department`）
- [ ] 支援區域隔離（銷售人員僅能看自己區域的資料）

✅ 必須項（審計日誌）:
- [ ] 記錄所有查詢（誰、何時、查詢什麼、結果多少筆）
- [ ] 記錄權限拒絕事件（嘗試存取未授權資料）
- [ ] 查詢歷史可追溯（至少保留 90 天）
- [ ] 異常查詢告警（例如：大量資料匯出）

**權限配置範例**:
```yaml
# 權限配置檔
permissions:
  role: "sales_representative"  # 銷售代表

  table_access:
    allowed_tables:
      - customers
      - orders
      - products
    forbidden_tables:
      - employees
      - salaries
      - internal_memos

  column_access:
    customers:
      allowed: ["customer_id", "name", "email", "phone"]
      forbidden: ["ssn", "credit_card"]  # 敏感資料
      masked: ["phone"]  # 部分遮罩: "0912-***-456"

  row_level_security:
    customers:
      # 銷售人員僅能看自己區域的客戶
      filter: "region = @current_user_region"

    orders:
      # 銷售人員僅能看自己區域的訂單
      filter: "customer_id IN (SELECT customer_id FROM customers WHERE region = @current_user_region)"

  query_limits:
    max_rows: 1000  # 每次查詢最多返回 1000 筆
    max_execution_time: 30  # 查詢最多執行 30 秒
```

**執行範例**:
```sql
-- 用戶查詢（銷售代表 John，區域=北區）
"顯示所有客戶"

-- Agent 自動添加 RLS 條件
SELECT customer_id, name, email, CONCAT(SUBSTRING(phone, 1, 4), '-***-', SUBSTRING(phone, 9, 3)) AS phone
FROM customers
WHERE region = '北區'  -- 自動添加
LIMIT 1000;

-- 審計日誌
{
  "user": "john@company.com",
  "role": "sales_representative",
  "timestamp": "2025-10-28T14:30:00Z",
  "query": "顯示所有客戶",
  "generated_sql": "SELECT ... WHERE region = '北區'",
  "rows_returned": 150,
  "execution_time_ms": 45
}
```

**技術實現**:
- 權限配置管理系統
- SQL Rewriter（自動添加 RLS 條件）
- 欄位遮罩引擎
- 審計日誌系統（寫入資料庫或 Log 系統）

**📊 優先級**: P0 (MVP 必須) - 企業級安全
**🎯 安全目標**: 0 權限繞過漏洞
**🔗 相關**: US 8.2 (Text-to-SQL), US 9.1 (RBAC)

---

#### US 8.4 - 查詢結果展示與互動

**作為** 業務用戶
**我想要** 清晰易讀的查詢結果展示
**以便** 快速理解資料洞察

**驗收標準**:

✅ 必須項（結果展示）:
- [ ] 表格格式展示（支援排序、分頁）
- [ ] 數據視覺化（自動生成圖表，當適合時）
- [ ] 摘要統計（總筆數、平均值、最大/最小值）
- [ ] 資料型別友善顯示（日期格式化、貨幣符號、千分位）

✅ 必須項（互動式探索）:
- [ ] 點擊欄位可進一步查詢（"點擊客戶名稱查看詳細資料"）
- [ ] 結果可匯出（CSV、Excel、JSON）
- [ ] 支援 Follow-up 查詢（"再顯示這些客戶的訂單明細"）
- [ ] 查詢歷史導航（"回到上一個查詢"）

✅ 必須項（智能建議）:
- [ ] 根據查詢結果推薦下一步（"您可能想看：銷售趨勢圖"）
- [ ] 異常值告警（"客戶 ABC 的消費突然增加 300%"）
- [ ] 業務洞察提示（"本月銷售額比上月下降 15%"）

✅ 必須項（錯誤處理）:
- [ ] SQL 語法錯誤時，提供修正建議
- [ ] 查詢超時時，建議簡化查詢
- [ ] 無結果時，提供替代查詢建議
- [ ] 權限錯誤時，說明原因和解決方法

**展示範例**:
```
User: "最近一個月銷售額最高的客戶"

Agent: "✅ 查詢完成（45ms，共 10 筆資料）

📊 最近一個月銷售額最高的前 10 名客戶

| 排名 | 客戶ID | 姓名 | 總銷售額 | 訂單數 | 平均訂單金額 |
|------|--------|------|----------|--------|--------------|
| 1    | 1001   | ABC  | $50,000  | 25     | $2,000       |
| 2    | 1002   | XYZ  | $48,000  | 30     | $1,600       |
| ...

📈 摘要統計
- 總銷售額: $350,000
- 平均銷售額: $35,000
- 最高單筆: $50,000 (ABC)

💡 洞察
- 🔥 客戶 ABC 的銷售額比上月成長 45%
- ⚠️ 客戶 XYZ 的平均訂單金額下降 20%

🔍 您可能還想查詢：
1️⃣ 這些客戶的訂單明細
2️⃣ 銷售額趨勢圖（過去 6 個月）
3️⃣ 按產品類別分析

[匯出 CSV] [匯出 Excel] [查看 SQL]
"
```

**技術實現**:
- 結果格式化引擎
- 圖表自動生成（Chart.js、D3.js）
- 統計分析模組
- 洞察生成（LLM 分析結果）

**📊 優先級**: P1 (MVP 高優先) - UX 增強
**🎯 UX 目標**: 結果渲染 <500ms
**🔗 相關**: US 6.5 (表格展示), US 5.5 (Knowledge 查詢)

---# 模組 09: 企業級基礎能力

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 09
**User Stories**: US 9.1-9.4
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 9: 企業級基礎能力

**Epic**: 提供企業級的安全、隔離、國際化和資源管理能力

**企業價值**:
- 符合企業 IT 安全和合規要求
- 支援多租戶 SaaS 部署模式
- 全球化市場支援
- 成本控制和公平使用

**User Stories 數量**: 4 個（US 9.1 - 9.4）

---

#### US 9.1 - RBAC 角色權限管理

**作為** 系統管理員
**我想要** 細粒度的角色權限控制
**以便** 確保用戶僅能執行被授權的操作

**驗收標準**:

✅ 必須項（角色定義）:
- [ ] 預定義角色（超級管理員、租戶管理員、開發者、業務分析師、業務用戶、唯讀用戶）
- [ ] 自定義角色建立
- [ ] 角色繼承（例如：開發者繼承業務分析師的所有權限）
- [ ] 角色可停用/啟用

✅ 必須項（權限粒度）:
- [ ] Agent 級權限（建立、讀取、更新、刪除、執行）
- [ ] Plugin 級權限（安裝、配置、啟用/停用）
- [ ] Knowledge 級權限（上傳、查詢、刪除）
- [ ] Code Interpreter 權限（啟用/停用、執行權限）
- [ ] 系統設定權限（租戶設定、用戶管理、計費設定）

✅ 必須項（權限檢查）:
- [ ] API 層權限檢查（每個 API 呼叫都驗證權限）
- [ ] UI 層權限控制（未授權功能隱藏或禁用）
- [ ] 資源級權限（用戶僅能操作自己建立的 Agent）
- [ ] 權限拒絕時提供清楚的錯誤訊息

✅ 必須項（權限審計）:
- [ ] 記錄所有權限檢查事件
- [ ] 記錄權限變更歷史
- [ ] 異常權限使用告警
- [ ] 權限合規報告

**角色權限矩陣範例**:
```yaml
roles:
  super_admin:
    description: "超級管理員，擁有所有權限"
    permissions:
      - "*"  # 所有權限

  tenant_admin:
    description: "租戶管理員，管理租戶內所有資源"
    permissions:
      agent: ["create", "read", "update", "delete", "execute"]
      plugin: ["install", "configure", "enable", "disable"]
      knowledge: ["upload", "query", "delete"]
      code_interpreter: ["enable", "execute"]
      user: ["create", "read", "update", "delete", "assign_role"]
      tenant_settings: ["read", "update"]

  developer:
    description: "開發者，建立和配置 Agent"
    permissions:
      agent: ["create", "read", "update", "delete", "execute"]
      plugin: ["install", "configure", "enable", "disable"]
      knowledge: ["upload", "query"]
      code_interpreter: ["enable", "execute"]
    resource_scope: "own"  # 僅能操作自己建立的資源

  business_analyst:
    description: "業務分析師，配置 Agent 行為"
    permissions:
      agent: ["read", "update", "execute"]
      knowledge: ["upload", "query"]
    resource_scope: "team"  # 可以操作團隊共享資源

  business_user:
    description: "業務用戶，使用 Agent"
    permissions:
      agent: ["read", "execute"]
      knowledge: ["query"]
    resource_scope: "shared"  # 僅能使用公開共享的資源

  readonly_user:
    description: "唯讀用戶，僅能查看"
    permissions:
      agent: ["read"]
      knowledge: ["read"]
```

**權限檢查範例**:
```csharp
// API 層權限檢查
[Authorize(Permission = "agent.delete")]
public async Task<IActionResult> DeleteAgent(string agentId)
{
    // 驗證用戶是否有刪除權限
    if (!await _authService.HasPermission(User, "agent.delete"))
    {
        return Forbid("您沒有刪除 Agent 的權限");
    }

    // 驗證資源擁有者（Resource-level check）
    var agent = await _agentService.GetAgent(agentId);
    if (!await _authService.CanAccessResource(User, agent))
    {
        return Forbid("您只能刪除自己建立的 Agent");
    }

    // 執行刪除
    await _agentService.DeleteAgent(agentId);

    // 記錄審計日誌
    await _auditService.Log(new AuditEvent
    {
        UserId = User.Id,
        Action = "agent.delete",
        ResourceId = agentId,
        Timestamp = DateTime.UtcNow
    });

    return Ok();
}
```

**技術實現**:
- ASP.NET Core Authorization
- Policy-Based Authorization
- 權限資料庫（Role、Permission、RolePermission 表）
- 審計日誌系統

**📊 優先級**: P0 (MVP 必須) - 企業安全基礎
**🎯 安全目標**: 0 權限繞過漏洞
**🔗 相關**: US 8.3 (Row-Level Security), US 9.2 (Multi-Tenant)

---

#### US 9.2 - Multi-Tenant 多租戶架構

**作為** SaaS 平台營運者
**我想要** 完全隔離的多租戶架構
**以便** 安全地服務多個企業客戶

**驗收標準**:

✅ 必須項（租戶隔離）:
- [ ] 資料完全隔離（租戶 A 無法存取租戶 B 的任何資料）
- [ ] 每個 API 請求自動加入 `tenant_id` 過濾
- [ ] 資料庫查詢自動添加 `WHERE tenant_id = @current_tenant`
- [ ] 租戶間無法互相查看或操作資源

✅ 必須項（租戶管理）:
- [ ] 租戶建立和停用
- [ ] 租戶配額設定（Agent 數量、儲存空間、API 呼叫量）
- [ ] 租戶計費方案（免費版、專業版、企業版）
- [ ] 租戶自訂品牌（Logo、顏色、網域名稱）

✅ 必須項（資源配額）:
- [ ] 每個租戶的 Agent 數量限制
- [ ] 每個租戶的儲存空間限制
- [ ] 每個租戶的 API 呼叫量限制
- [ ] 超出配額時的友善提示和升級引導

✅ 必須項（租戶層級設定）:
- [ ] 獨立的 LLM 配置（模型選擇、API Key）
- [ ] 獨立的安全設定（密碼策略、SSO 整合）
- [ ] 獨立的 i18n 設定（預設語言、時區）
- [ ] 獨立的通知設定（Email、Webhook）

**租戶資料模型**:
```csharp
public class Tenant
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string SubDomain { get; set; }  // acme.framework.com
    public TenantPlan Plan { get; set; }   // Free, Pro, Enterprise

    // 配額設定
    public TenantQuota Quota { get; set; }

    // 品牌設定
    public TenantBranding Branding { get; set; }

    // 狀態
    public TenantStatus Status { get; set; }  // Active, Suspended, Deleted
    public DateTime CreatedAt { get; set; }
}

public class TenantQuota
{
    public int MaxAgents { get; set; }           // 最多建立多少個 Agent
    public long MaxStorageBytes { get; set; }    // 最大儲存空間（Bytes）
    public int MaxApiCallsPerMonth { get; set; } // 每月 API 呼叫量
    public int MaxUsersPerTenant { get; set; }   // 最多用戶數

    // 當前使用量
    public int CurrentAgents { get; set; }
    public long CurrentStorageBytes { get; set; }
    public int CurrentApiCallsThisMonth { get; set; }
}

public class TenantBranding
{
    public string LogoUrl { get; set; }
    public string PrimaryColor { get; set; }    // #007bff
    public string CustomDomain { get; set; }    // agent.acme.com
}
```

**租戶隔離實現**:
```csharp
// 自動過濾租戶資料（EF Core Global Query Filter）
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // 所有實體自動加入 TenantId 過濾
    modelBuilder.Entity<Agent>()
        .HasQueryFilter(a => a.TenantId == _currentTenant.Id);

    modelBuilder.Entity<Plugin>()
        .HasQueryFilter(p => p.TenantId == _currentTenant.Id);

    modelBuilder.Entity<Knowledge>()
        .HasQueryFilter(k => k.TenantId == _currentTenant.Id);
}

// Middleware 自動提取當前租戶
public class TenantResolutionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // 從 JWT Token 中提取 tenant_id
        var tenantId = context.User.FindFirst("tenant_id")?.Value;

        // 或從子網域提取 (acme.framework.com → acme)
        var subDomain = context.Request.Host.Host.Split('.').First();
        var tenant = await _tenantService.GetBySubDomain(subDomain);

        // 設定當前租戶上下文
        _currentTenant.SetTenant(tenant);

        await _next(context);
    }
}
```

**配額檢查範例**:
```csharp
public async Task<Result> CreateAgent(CreateAgentRequest request)
{
    // 檢查租戶配額
    var quota = await _tenantService.GetQuota(_currentTenant.Id);

    if (quota.CurrentAgents >= quota.MaxAgents)
    {
        return Result.Fail($"已達到 Agent 數量上限（{quota.MaxAgents}）。請升級方案以建立更多 Agent。");
    }

    // 建立 Agent
    var agent = new Agent
    {
        TenantId = _currentTenant.Id,  // 自動設定租戶 ID
        Name = request.Name,
        // ...
    };

    await _agentRepository.Add(agent);

    // 更新配額使用量
    await _tenantService.IncrementAgentCount(_currentTenant.Id);

    return Result.Ok(agent);
}
```

**技術實現**:
- EF Core Global Query Filters
- Tenant Resolution Middleware
- 多租戶資料模型設計
- 配額檢查和限流

**📊 優先級**: P0 (MVP 必須) - SaaS 基礎
**🎯 隔離目標**: 100% 資料隔離，0 跨租戶資料洩漏
**🔗 相關**: US 9.1 (RBAC), US 9.4 (API Metering)

---

#### US 9.3 - i18n 國際化支援

**作為** 全球用戶
**我想要** 使用自己的語言與 Agent 互動
**以便** 提升使用體驗和理解準確性

**驗收標準**:

✅ 必須項（UI 國際化）:
- [ ] 支援語言：中文（繁體/簡體）、英文、日文
- [ ] 所有 UI 文字可翻譯（按鈕、標籤、訊息、錯誤提示）
- [ ] 用戶可切換語言（即時生效，無需重新整理）
- [ ] 語言偏好儲存（用戶設定、租戶預設）

✅ 必須項（日期時間本地化）:
- [ ] 根據用戶時區顯示時間
- [ ] 日期格式本地化（美國：MM/DD/YYYY、台灣：YYYY/MM/DD）
- [ ] 相對時間顯示（"2 小時前"、"昨天"、"上週"）
- [ ] 時區選擇和轉換

✅ 必須項（數字與貨幣本地化）:
- [ ] 數字格式（千分位：美國 1,000 vs 歐洲 1.000）
- [ ] 貨幣符號和格式（$1,000.00、¥1,000、€1.000,00）
- [ ] 百分比格式

✅ 必須項（Agent 多語言對話）:
- [ ] Agent 可根據用戶語言偏好自動切換對話語言
- [ ] 支援混合語言查詢（中英混合）
- [ ] LLM 回應的語言與用戶查詢語言一致
- [ ] 翻譯敏感欄位名稱（例如：`customer_name` → "客戶姓名"）

**翻譯資源檔範例**:
```json
// locales/zh-TW.json (繁體中文)
{
  "common": {
    "create": "建立",
    "edit": "編輯",
    "delete": "刪除",
    "save": "儲存",
    "cancel": "取消"
  },
  "agent": {
    "title": "AI Agent",
    "create_agent": "建立 Agent",
    "agent_name": "Agent 名稱",
    "agent_description": "Agent 描述"
  },
  "validation": {
    "required": "此欄位為必填",
    "email_invalid": "電子郵件格式不正確",
    "password_too_short": "密碼長度至少 8 個字元"
  },
  "errors": {
    "quota_exceeded": "已達到 {resource} 上限（{limit}）",
    "permission_denied": "您沒有權限執行此操作",
    "agent_not_found": "找不到 Agent (ID: {id})"
  }
}

// locales/en-US.json (英文)
{
  "common": {
    "create": "Create",
    "edit": "Edit",
    "delete": "Delete",
    "save": "Save",
    "cancel": "Cancel"
  },
  "agent": {
    "title": "AI Agent",
    "create_agent": "Create Agent",
    "agent_name": "Agent Name",
    "agent_description": "Agent Description"
  },
  "validation": {
    "required": "This field is required",
    "email_invalid": "Invalid email format",
    "password_too_short": "Password must be at least 8 characters"
  },
  "errors": {
    "quota_exceeded": "You have reached the {resource} limit ({limit})",
    "permission_denied": "You don't have permission for this operation",
    "agent_not_found": "Agent not found (ID: {id})"
  }
}
```

**i18n 使用範例（前端 React）**:
```tsx
import { useTranslation } from 'react-i18next';

function CreateAgentButton() {
  const { t, i18n } = useTranslation();

  const changeLanguage = (lang: string) => {
    i18n.changeLanguage(lang);
  };

  return (
    <div>
      <button>{t('agent.create_agent')}</button>

      {/* 語言切換器 */}
      <select onChange={(e) => changeLanguage(e.target.value)}>
        <option value="zh-TW">繁體中文</option>
        <option value="zh-CN">简体中文</option>
        <option value="en-US">English</option>
        <option value="ja-JP">日本語</option>
      </select>

      {/* 錯誤訊息（帶參數） */}
      <p>{t('errors.quota_exceeded', { resource: 'Agent', limit: 10 })}</p>
    </div>
  );
}
```

**後端 i18n 範例（.NET）**:
```csharp
public class AgentService
{
    private readonly IStringLocalizer<AgentService> _localizer;

    public AgentService(IStringLocalizer<AgentService> localizer)
    {
        _localizer = localizer;
    }

    public async Task<Result> CreateAgent(CreateAgentRequest request)
    {
        var quota = await _tenantService.GetQuota(_currentTenant.Id);

        if (quota.CurrentAgents >= quota.MaxAgents)
        {
            // 使用本地化訊息
            return Result.Fail(_localizer["QuotaExceeded", "Agent", quota.MaxAgents]);
        }

        // ...
    }
}

// Resources/AgentService.zh-TW.resx
// QuotaExceeded = "已達到 {0} 上限（{1}）"

// Resources/AgentService.en-US.resx
// QuotaExceeded = "You have reached the {0} limit ({1})"
```

**Agent 多語言對話範例**:
```python
# 用戶語言偏好：中文
user_query = "顯示最近一週的銷售報告"

# Agent 自動以中文回應
agent_response = """
✅ 查詢完成

📊 最近一週銷售報告（2025/10/21 - 2025/10/28）

| 日期 | 銷售額 | 訂單數 |
|------|--------|--------|
| 10/28 | ¥50,000 | 25 |
| 10/27 | ¥48,000 | 30 |
...
"""

# 如果用戶切換為英文
user_query = "Show me last week's sales report"

agent_response = """
✅ Query completed

📊 Sales Report (Last Week: 10/21/2025 - 10/28/2025)

| Date  | Revenue | Orders |
|-------|---------|--------|
| 10/28 | $50,000 | 25     |
| 10/27 | $48,000 | 30     |
...
"""
```

**技術實現**:
- 前端：react-i18next 或 next-i18next
- 後端：ASP.NET Core Localization
- LLM Prompt 自動插入語言指令
- 翻譯資源管理（JSON/RESX）

**📊 優先級**: P1 (MVP 高優先) - 全球化必須
**🎯 覆蓋目標**: 100% UI 文字可翻譯
**🔗 相關**: US 7.2 (Persona Prompt), US 6.1 (Chat UI)

---

#### US 9.4 - API Metering 與限流

**作為** 平台營運者
**我想要** 追蹤和限制 API 使用量
**以便** 控制成本和防止濫用

**驗收標準**:

✅ 必須項（使用量計量）:
- [ ] 記錄每個 API 呼叫（端點、用戶、租戶、時間戳）
- [ ] 統計 LLM Token 使用量（Input Tokens、Output Tokens）
- [ ] 統計儲存空間使用量（Knowledge、Code Interpreter）
- [ ] 統計 Code Interpreter 執行時間

✅ 必須項（限流策略）:
- [ ] 租戶級限流（每個租戶每月 API 呼叫量上限）
- [ ] 用戶級限流（每個用戶每分鐘請求數上限）
- [ ] LLM Token 限流（每個租戶每月 Token 用量上限）
- [ ] Rate Limiting（防止短時間內大量請求）

✅ 必須項（配額告警）:
- [ ] 達到 80% 配額時發送告警郵件
- [ ] 達到 100% 配額時暫停服務（可設定為軟限制或硬限制）
- [ ] 提供配額儀表板（即時查看使用量）
- [ ] 自動重置配額（每月 1 號重置）

✅ 必須項（計費整合）:
- [ ] 基於使用量的計費（Pay-as-you-go）
- [ ] 超額使用自動計費
- [ ] 計費報告（每月帳單）
- [ ] 使用量趨勢分析

**API 計量資料模型**:
```csharp
public class ApiUsageRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }

    // API 資訊
    public string Endpoint { get; set; }  // /api/agents/{id}/execute
    public string HttpMethod { get; set; }  // POST
    public int StatusCode { get; set; }  // 200

    // Token 使用量
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens => InputTokens + OutputTokens;

    // 執行時間
    public int ExecutionTimeMs { get; set; }

    // 時間戳
    public DateTime Timestamp { get; set; }
}

public class TenantUsageSummary
{
    public Guid TenantId { get; set; }
    public int Month { get; set; }  // 2025-10

    // API 呼叫量
    public int TotalApiCalls { get; set; }
    public int ApiCallsQuota { get; set; }

    // Token 使用量
    public long TotalTokens { get; set; }
    public long TokensQuota { get; set; }

    // 儲存空間
    public long StorageUsedBytes { get; set; }
    public long StorageQuotaBytes { get; set; }

    // 超額計費
    public decimal OverageCharges { get; set; }
}
```

**限流實現（ASP.NET Core）**:
```csharp
// Rate Limiting Middleware
public class RateLimitingMiddleware
{
    private readonly IRateLimiter _rateLimiter;

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = _currentTenant.Id;
        var userId = context.User.FindFirst("sub")?.Value;

        // 檢查租戶級限流（每月 API 呼叫量）
        var tenantUsage = await _meteringService.GetMonthlyUsage(tenantId);
        if (tenantUsage.TotalApiCalls >= tenantUsage.ApiCallsQuota)
        {
            context.Response.StatusCode = 429;  // Too Many Requests
            await context.Response.WriteAsJsonAsync(new
            {
                error = "API quota exceeded",
                quota = tenantUsage.ApiCallsQuota,
                used = tenantUsage.TotalApiCalls,
                reset_date = GetNextMonthFirstDay()
            });
            return;
        }

        // 檢查用戶級限流（每分鐘請求數）
        var rateLimitKey = $"rate_limit:{userId}";
        var requestCount = await _rateLimiter.IncrementAsync(rateLimitKey, TimeSpan.FromMinutes(1));

        if (requestCount > 60)  // 每分鐘最多 60 次請求
        {
            context.Response.StatusCode = 429;
            context.Response.Headers.Add("X-RateLimit-Limit", "60");
            context.Response.Headers.Add("X-RateLimit-Remaining", "0");
            context.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());
            return;
        }

        // 記錄 API 使用
        var startTime = DateTime.UtcNow;
        await _next(context);
        var endTime = DateTime.UtcNow;

        await _meteringService.RecordApiCall(new ApiUsageRecord
        {
            TenantId = tenantId,
            UserId = userId,
            Endpoint = context.Request.Path,
            HttpMethod = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            ExecutionTimeMs = (int)(endTime - startTime).TotalMilliseconds,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

**Token 計量範例**:
```csharp
public class LLMService
{
    public async Task<LLMResponse> GenerateResponse(string prompt)
    {
        var response = await _llmClient.ChatCompletion(new ChatRequest
        {
            Messages = new[] { new Message("user", prompt) }
        });

        // 記錄 Token 使用量
        await _meteringService.RecordTokenUsage(new TokenUsageRecord
        {
            TenantId = _currentTenant.Id,
            AgentId = _currentAgent.Id,
            InputTokens = response.Usage.PromptTokens,
            OutputTokens = response.Usage.CompletionTokens,
            Model = "gpt-4",
            Timestamp = DateTime.UtcNow
        });

        return response;
    }
}
```

**配額儀表板範例**:
```tsx
function UsageDashboard() {
  const { data: usage } = useMonthlyUsage();

  return (
    <div>
      <h2>本月使用量</h2>

      {/* API 呼叫量 */}
      <UsageCard
        title="API 呼叫"
        used={usage.totalApiCalls}
        quota={usage.apiCallsQuota}
        unit="calls"
      />

      {/* Token 使用量 */}
      <UsageCard
        title="LLM Tokens"
        used={usage.totalTokens}
        quota={usage.tokensQuota}
        unit="tokens"
      />

      {/* 儲存空間 */}
      <UsageCard
        title="儲存空間"
        used={formatBytes(usage.storageUsedBytes)}
        quota={formatBytes(usage.storageQuotaBytes)}
        unit=""
      />

      {/* 告警提示 */}
      {usage.totalApiCalls / usage.apiCallsQuota > 0.8 && (
        <Alert severity="warning">
          您已使用 {Math.round(usage.totalApiCalls / usage.apiCallsQuota * 100)}% 的 API 配額。
          考慮升級方案以獲得更多配額。
        </Alert>
      )}
    </div>
  );
}
```

**技術實現**:
- ASP.NET Core Rate Limiting Middleware
- Redis 或 In-Memory 計數器（高性能）
- 時間序列資料庫（InfluxDB 或 TimescaleDB）
- 計費整合（Stripe API）

**📊 優先級**: P1 (MVP 高優先) - 成本控制
**🎯 準確性目標**: 100% API 呼叫和 Token 使用量準確記錄
**🔗 相關**: US 9.2 (Multi-Tenant), US 10.1 (監控儀表板)

---# 模組 10: 監控與分析

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 10
**User Stories**: US 10.1-10.3
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 10: 監控與分析

**Epic**: 提供全面的系統監控、品質分析和用戶行為洞察

**業務價值**:
- 即時掌握系統健康狀態
- 快速發現和解決問題
- 持續優化產品體驗
- 數據驅動的決策支援

**User Stories 數量**: 3 個（US 10.1 - 10.3）

---

#### US 10.1 - 即時監控儀表板

**作為** 系統管理員
**我想要** 即時監控系統運行狀態
**以便** 快速發現和處理異常

**驗收標準**:

✅ 必須項（系統健康監控）:
- [ ] CPU、記憶體、磁碟使用率即時監控
- [ ] API 回應時間（P50、P95、P99）
- [ ] LLM 服務可用性和延遲
- [ ] 資料庫連線池狀態
- [ ] 背景任務執行狀態（Knowledge 索引、Code Interpreter）

✅ 必須項（業務指標監控）:
- [ ] Agent 執行成功率（成功/失敗/逾時）
- [ ] 每分鐘請求數（RPM）和每秒請求數（RPS）
- [ ] 活躍用戶數（即時、每日、每月）
- [ ] Token 使用量趨勢
- [ ] 熱門 Agent 排行榜

✅ 必須項（錯誤監控）:
- [ ] 即時錯誤追蹤（錯誤類型、頻率、堆疊追蹤）
- [ ] 5xx 錯誤自動告警
- [ ] LLM 呼叫失敗告警
- [ ] Knowledge 檢索失敗告警
- [ ] 錯誤趨勢分析（是否有突發性錯誤）

✅ 必須項（視覺化儀表板）:
- [ ] 即時更新（每 5 秒刷新）
- [ ] 時間範圍選擇（最近 1 小時、24 小時、7 天、30 天）
- [ ] 多維度篩選（租戶、Agent、用戶）
- [ ] 告警狀態顯示（正常/警告/嚴重）
- [ ] 匯出報告（PDF、Excel）

**儀表板設計範例**:
```yaml
# 監控儀表板佈局
dashboard:
  title: "Semantic Kernel Framework - 系統監控"

  panels:
    - name: "系統健康"
      widgets:
        - type: "gauge"
          metric: "cpu_usage_percent"
          threshold: { warning: 70, critical: 90 }

        - type: "gauge"
          metric: "memory_usage_percent"
          threshold: { warning: 80, critical: 95 }

        - type: "line_chart"
          metric: "api_response_time_ms"
          aggregation: ["p50", "p95", "p99"]
          time_range: "1h"

    - name: "業務指標"
      widgets:
        - type: "counter"
          metric: "active_users_now"
          icon: "👥"

        - type: "counter"
          metric: "agents_executed_today"
          icon: "🤖"

        - type: "line_chart"
          metric: "rpm_rps"
          labels: ["RPM", "RPS"]

        - type: "bar_chart"
          metric: "top_agents"
          limit: 10

    - name: "錯誤監控"
      widgets:
        - type: "error_list"
          metric: "recent_errors"
          limit: 20
          auto_refresh: 5s

        - type: "line_chart"
          metric: "error_rate"
          time_range: "24h"

    - name: "LLM 服務"
      widgets:
        - type: "gauge"
          metric: "llm_availability_percent"
          threshold: { warning: 95, critical: 90 }

        - type: "line_chart"
          metric: "llm_latency_ms"
          time_range: "1h"

        - type: "counter"
          metric: "tokens_used_today"
          format: "number"
```

**監控指標收集範例**:
```csharp
public class MetricsCollector
{
    private readonly IMetricsRegistry _metrics;

    public async Task CollectMetrics()
    {
        // 系統指標
        _metrics.RecordGauge("cpu_usage_percent", GetCpuUsage());
        _metrics.RecordGauge("memory_usage_percent", GetMemoryUsage());

        // API 回應時間（已在 Middleware 中收集）
        // 每個請求都記錄，自動計算 P50/P95/P99

        // 業務指標
        _metrics.RecordCounter("agents_executed_today", await GetTodayAgentExecutions());
        _metrics.RecordGauge("active_users_now", await GetActiveUsers());

        // LLM 指標
        var llmHealth = await _llmClient.HealthCheck();
        _metrics.RecordGauge("llm_availability_percent", llmHealth.IsAvailable ? 100 : 0);
        _metrics.RecordHistogram("llm_latency_ms", llmHealth.LatencyMs);
    }
}

// Middleware 收集 API 回應時間
public class MetricsMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        // 記錄回應時間（自動聚合為 P50/P95/P99）
        _metrics.RecordHistogram("api_response_time_ms", stopwatch.ElapsedMilliseconds, new Dictionary<string, string>
        {
            { "endpoint", context.Request.Path },
            { "method", context.Request.Method },
            { "status_code", context.Response.StatusCode.ToString() }
        });
    }
}
```

**告警規則範例**:
```yaml
# 告警規則配置
alerts:
  - name: "CPU 使用率過高"
    condition: "cpu_usage_percent > 90 for 5 minutes"
    severity: "critical"
    notify: ["email", "slack"]

  - name: "API 回應時間過慢"
    condition: "api_response_time_p95 > 2000ms for 10 minutes"
    severity: "warning"
    notify: ["slack"]

  - name: "LLM 服務不可用"
    condition: "llm_availability_percent < 95"
    severity: "critical"
    notify: ["email", "slack", "pagerduty"]

  - name: "錯誤率突增"
    condition: "error_rate > 5% for 5 minutes"
    severity: "warning"
    notify: ["slack"]

  - name: "Agent 執行失敗率過高"
    condition: "agent_failure_rate > 10% for 15 minutes"
    severity: "warning"
    notify: ["email"]
```

**技術實現**:
- Prometheus + Grafana（監控和視覺化）
- Application Insights（Azure 原生）
- SignalR（即時更新推送到前端）
- 自訂告警引擎

**📊 優先級**: P1 (MVP 高優先) - 運維必須
**🎯 可用性目標**: 儀表板 99.9% 可用性
**🔗 相關**: US 9.4 (API Metering), US 10.2 (品質監控)

---

#### US 10.2 - Agent 品質監控與告警

**作為** 產品經理
**我想要** 監控 Agent 回應品質和用戶滿意度
**以便** 持續優化 Agent 表現

**驗收標準**:

✅ 必須項（回應品質指標）:
- [ ] Agent 回應準確率（基於用戶反饋）
- [ ] 平均回應時間（從用戶提問到 Agent 回應）
- [ ] Token 使用效率（每個查詢平均 Token 數）
- [ ] Knowledge 檢索準確率（相關性分數）
- [ ] Code Interpreter 執行成功率

✅ 必須項（用戶滿意度監控）:
- [ ] 👍/👎 反饋比例
- [ ] 詳細反饋分類（回應不準確、太慢、語氣不對等）
- [ ] 用戶重複查詢率（是否需要多次提問才得到答案）
- [ ] 會話完成率（用戶是否中途放棄）
- [ ] Net Promoter Score (NPS) 調查

✅ 必須項（異常檢測）:
- [ ] 突發性失敗告警（失敗率突然上升）
- [ ] 回應時間異常告警（P95 超過閾值）
- [ ] 低滿意度告警（👎 比例超過 20%）
- [ ] Token 使用異常告警（突然暴增）
- [ ] Persona 不一致告警（風格偏離）

✅ 必須項（品質趨勢分析）:
- [ ] 每日/每週/每月品質報告
- [ ] 不同 Agent 的品質對比
- [ ] 品質改善/退化趨勢
- [ ] 根因分析（品質問題的主要原因）

**品質監控儀表板範例**:
```yaml
quality_dashboard:
  title: "Agent 品質監控"

  kpi_cards:
    - metric: "response_accuracy_rate"
      display: "回應準確率"
      value: "87.5%"
      trend: "+2.3%"  # 比上週
      status: "good"  # good/warning/critical

    - metric: "average_response_time"
      display: "平均回應時間"
      value: "2.3s"
      trend: "-0.4s"
      status: "good"

    - metric: "user_satisfaction_rate"
      display: "用戶滿意度"
      value: "👍 82%"
      trend: "+5%"
      status: "good"

    - metric: "knowledge_retrieval_accuracy"
      display: "Knowledge 準確率"
      value: "92.1%"
      trend: "+1.5%"
      status: "good"

  charts:
    - type: "line_chart"
      title: "每日滿意度趨勢"
      metrics: ["thumbs_up_rate", "thumbs_down_rate"]
      time_range: "30d"

    - type: "bar_chart"
      title: "反饋分類分佈"
      data:
        - category: "回應不準確"
          count: 120
        - category: "回應太慢"
          count: 45
        - category: "語氣不對"
          count: 30

    - type: "table"
      title: "品質最差的 Top 10 Agents"
      columns: ["Agent 名稱", "滿意度", "失敗率", "平均回應時間"]
```

**品質指標計算範例**:
```csharp
public class QualityMetricsService
{
    public async Task<AgentQualityReport> GenerateQualityReport(Guid agentId, DateTime startDate, DateTime endDate)
    {
        var executions = await _executionRepository.GetByAgentAndDateRange(agentId, startDate, endDate);

        // 回應準確率（基於用戶反饋）
        var totalExecutions = executions.Count;
        var thumbsUp = executions.Count(e => e.Feedback?.IsThumbsUp == true);
        var thumbsDown = executions.Count(e => e.Feedback?.IsThumbsUp == false);
        var accuracyRate = thumbsUp / (double)(thumbsUp + thumbsDown);

        // 平均回應時間
        var avgResponseTime = executions.Average(e => e.ResponseTimeMs);

        // Token 使用效率
        var avgTokensPerQuery = executions.Average(e => e.TotalTokens);

        // Knowledge 檢索準確率
        var knowledgeExecutions = executions.Where(e => e.UsedKnowledge);
        var avgRelevanceScore = knowledgeExecutions.Average(e => e.KnowledgeRelevanceScore);

        return new AgentQualityReport
        {
            AgentId = agentId,
            Period = new DateRange(startDate, endDate),
            AccuracyRate = accuracyRate,
            AverageResponseTimeMs = avgResponseTime,
            AverageTokensPerQuery = avgTokensPerQuery,
            KnowledgeRetrievalAccuracy = avgRelevanceScore,
            TotalExecutions = totalExecutions,
            ThumbsUpCount = thumbsUp,
            ThumbsDownCount = thumbsDown
        };
    }
}
```

**異常檢測和告警範例**:
```csharp
public class AnomalyDetectionService
{
    public async Task DetectAndAlert()
    {
        // 檢測失敗率突增
        var currentFailureRate = await GetCurrentFailureRate();
        var baselineFailureRate = await GetBaselineFailureRate(TimeSpan.FromDays(7));

        if (currentFailureRate > baselineFailureRate * 2)  // 失敗率翻倍
        {
            await _alertService.SendAlert(new Alert
            {
                Severity = AlertSeverity.Warning,
                Title = "Agent 失敗率異常上升",
                Message = $"當前失敗率 {currentFailureRate:P} 是基準值 {baselineFailureRate:P} 的兩倍",
                RecommendedAction = "檢查 LLM 服務狀態和 Knowledge 檢索是否正常"
            });
        }

        // 檢測低滿意度
        var recentFeedback = await GetRecentFeedback(TimeSpan.FromHours(1));
        var thumbsDownRate = recentFeedback.Count(f => !f.IsThumbsUp) / (double)recentFeedback.Count;

        if (thumbsDownRate > 0.3)  // 30% 不滿意
        {
            await _alertService.SendAlert(new Alert
            {
                Severity = AlertSeverity.Warning,
                Title = "用戶滿意度過低",
                Message = $"最近 1 小時內 {thumbsDownRate:P} 的用戶給予負面反饋",
                RecommendedAction = "檢查最近的 Agent 回應品質"
            });
        }
    }
}
```

**技術實現**:
- 統計分析引擎
- 異常檢測演算法（基於歷史基準）
- 即時告警系統
- NLP 情感分析（分析用戶反饋文字）

**📊 優先級**: P1 (MVP 高優先) - 品質保證
**🎯 準確性目標**: 異常檢測準確率 >90%
**🔗 相關**: US 7.3 (Persona 風格驗證), US 10.1 (監控儀表板)

---

#### US 10.3 - 用戶行為分析

**作為** 產品經理
**我想要** 分析用戶如何使用 Agent
**以便** 優化產品功能和用戶體驗

**驗收標準**:

✅ 必須項（用戶行為追蹤）:
- [ ] 用戶旅程分析（從登入到完成任務的路徑）
- [ ] 功能使用頻率（哪些 Agent、Plugin 最常用）
- [ ] 查詢模式分析（用戶最常問什麼類型的問題）
- [ ] 會話時長和互動次數
- [ ] 流失點分析（用戶在哪個步驟離開）

✅ 必須項（用戶分群）:
- [ ] 按活躍度分群（活躍用戶、偶爾使用、流失用戶）
- [ ] 按使用模式分群（重度 Knowledge 用戶、Code Interpreter 愛好者等）
- [ ] 按租戶/部門分群
- [ ] 新用戶 vs 老用戶分析

✅ 必須項（漏斗分析）:
- [ ] Agent 建立漏斗（建立 → 配置 → 測試 → 發佈）
- [ ] 用戶留存漏斗（Day 1/7/30 留存率）
- [ ] 功能採用漏斗（首次使用 → 持續使用）
- [ ] 識別轉換瓶頸

✅ 必須項（A/B 測試支援）:
- [ ] 設定實驗組和對照組
- [ ] 追蹤實驗指標（轉換率、滿意度、使用時長）
- [ ] 統計顯著性檢驗
- [ ] 實驗報告生成

**用戶行為分析儀表板範例**:
```yaml
analytics_dashboard:
  title: "用戶行為分析"

  sections:
    - name: "整體概覽"
      widgets:
        - type: "kpi_card"
          metric: "daily_active_users"
          display: "日活躍用戶（DAU）"
          value: "1,234"

        - type: "kpi_card"
          metric: "monthly_active_users"
          display: "月活躍用戶（MAU）"
          value: "8,567"

        - type: "line_chart"
          title: "用戶活躍度趨勢"
          metrics: ["dau", "wau", "mau"]
          time_range: "90d"

    - name: "功能使用分析"
      widgets:
        - type: "bar_chart"
          title: "最常用的 Agents（Top 20）"
          metric: "agent_usage_count"

        - type: "pie_chart"
          title: "Plugin 使用分佈"
          metric: "plugin_usage"

        - type: "table"
          title: "查詢類型分佈"
          columns: ["查詢類型", "使用次數", "占比"]
          data:
            - ["Knowledge 檢索", 5000, "45%"]
            - ["Text-to-SQL", 3000, "27%"]
            - ["Code Interpreter", 2000, "18%"]
            - ["一般對話", 1000, "10%"]

    - name: "用戶留存分析"
      widgets:
        - type: "cohort_table"
          title: "用戶留存 Cohort 分析"
          rows:  # 每週新增用戶的留存情況
            - week: "2025-W40"
              new_users: 100
              retention_d1: "80%"
              retention_d7: "45%"
              retention_d30: "25%"

        - type: "funnel_chart"
          title: "Agent 建立漏斗"
          steps:
            - stage: "建立 Agent"
              users: 1000
              conversion: "100%"
            - stage: "配置 Plugin"
              users: 800
              conversion: "80%"
            - stage: "添加 Knowledge"
              users: 600
              conversion: "60%"
            - stage: "測試 Agent"
              users: 500
              conversion: "50%"
            - stage: "發佈使用"
              users: 350
              conversion: "35%"

    - name: "用戶分群洞察"
      widgets:
        - type: "segment_comparison"
          segments:
            - name: "活躍用戶"
              count: 300
              avg_sessions_per_week: 15
              avg_queries_per_session: 8

            - name: "偶爾使用"
              count: 500
              avg_sessions_per_week: 2
              avg_queries_per_session: 5

            - name: "流失用戶"
              count: 200
              days_since_last_login: 30+
```

**事件追蹤範例**:
```typescript
// 前端事件追蹤
class AnalyticsTracker {
  trackEvent(eventName: string, properties?: Record<string, any>) {
    analytics.track({
      event: eventName,
      userId: currentUser.id,
      tenantId: currentTenant.id,
      timestamp: new Date(),
      properties: {
        ...properties,
        // 自動附加上下文
        page_url: window.location.href,
        user_agent: navigator.userAgent,
        screen_resolution: `${window.screen.width}x${window.screen.height}`
      }
    });
  }

  // 使用範例
  trackAgentCreated(agentId: string) {
    this.trackEvent('agent_created', {
      agent_id: agentId,
      plugins_count: agent.plugins.length,
      has_knowledge: agent.knowledge.length > 0
    });
  }

  trackAgentExecuted(agentId: string, executionTime: number) {
    this.trackEvent('agent_executed', {
      agent_id: agentId,
      execution_time_ms: executionTime,
      used_knowledge: execution.usedKnowledge,
      used_code_interpreter: execution.usedCodeInterpreter
    });
  }

  trackUserFeedback(feedbackType: 'thumbs_up' | 'thumbs_down', reason?: string) {
    this.trackEvent('user_feedback', {
      feedback_type: feedbackType,
      reason: reason,
      message_id: currentMessage.id
    });
  }
}
```

**用戶分群和洞察範例**:
```csharp
public class UserSegmentationService
{
    public async Task<List<UserSegment>> SegmentUsers()
    {
        var users = await _userRepository.GetAll();

        // 按活躍度分群
        var activeUsers = users.Where(u => u.LastLoginDate >= DateTime.UtcNow.AddDays(-7));
        var occasionalUsers = users.Where(u => u.LastLoginDate >= DateTime.UtcNow.AddDays(-30)
                                             && u.LastLoginDate < DateTime.UtcNow.AddDays(-7));
        var churnedUsers = users.Where(u => u.LastLoginDate < DateTime.UtcNow.AddDays(-30));

        // 分析每個分群的行為特徵
        return new List<UserSegment>
        {
            new UserSegment
            {
                Name = "活躍用戶",
                Count = activeUsers.Count(),
                AverageSessionsPerWeek = await CalculateAvgSessions(activeUsers, TimeSpan.FromDays(7)),
                AverageQueriesPerSession = await CalculateAvgQueries(activeUsers),
                TopFeatures = await GetTopFeatures(activeUsers)
            },
            // ...
        };
    }

    public async Task<FunnelAnalysis> AnalyzeAgentCreationFunnel()
    {
        var allUsers = await _userRepository.GetAll();

        var step1 = allUsers.Count(u => u.HasCreatedAgent);
        var step2 = allUsers.Count(u => u.HasConfiguredPlugin);
        var step3 = allUsers.Count(u => u.HasAddedKnowledge);
        var step4 = allUsers.Count(u => u.HasTestedAgent);
        var step5 = allUsers.Count(u => u.HasPublishedAgent);

        return new FunnelAnalysis
        {
            Steps = new[]
            {
                new FunnelStep("建立 Agent", step1, 1.0),
                new FunnelStep("配置 Plugin", step2, step2 / (double)step1),
                new FunnelStep("添加 Knowledge", step3, step3 / (double)step1),
                new FunnelStep("測試 Agent", step4, step4 / (double)step1),
                new FunnelStep("發佈使用", step5, step5 / (double)step1)
            },
            BottleneckStep = "配置 Plugin → 添加 Knowledge"  // 流失最大的步驟
        };
    }
}
```

**技術實現**:
- Amplitude、Mixpanel 或自建分析平台
- 事件追蹤 SDK（前端和後端）
- 大數據分析（Apache Spark、BigQuery）
- 機器學習（用戶行為預測）

**📊 優先級**: P2 (Phase 2) - 產品優化
**🎯 覆蓋率目標**: 追蹤所有關鍵用戶行為
**🔗 相關**: US 7.4 (Persona 學習), US 10.2 (品質監控)

---

## 3. 技術難點與實施策略

### 🎯 核心差異化能力實施

#### Persona Framework
**技術難點**:
- Prompt Engineering 複雜度高，需要精細調校
- 風格一致性驗證缺乏標準化指標
- 多輪對話中 Persona 可能漂移

**實施策略**:
1. **Phase 1 (MVP)**: 基礎 Persona 配置和 Prompt 生成（US 7.1, 7.2）
2. **Phase 2**: 風格一致性驗證和告警（US 7.3）
3. **Phase 3**: AI 驅動的 Persona 優化（US 7.4）

**成功標準**:
- Persona 一致性分數 >85%
- 用戶滿意度比無 Persona 版本提升 15%+

---

#### Text-to-SQL 安全策略
**技術難點**:
- SQL Injection 防禦必須 100% 有效
- 複雜 SQL 生成準確率不足
- Row-Level Security 實施複雜

**實施策略**:
1. **多層安全防護**:
   - Layer 1: 參數化查詢（Prepared Statements）
   - Layer 2: SQL 黑名單過濾（DDL/DML/系統命令）
   - Layer 3: AST 分析（語法樹檢查）
   - Layer 4: 沙箱執行（隔離環境）

2. **準確率提升**:
   - Few-shot Learning（提供範例查詢）
   - Schema 語義增強（業務術語對應）
   - 查詢驗證和自動修正
   - 用戶確認機制（執行前預覽 SQL）

**成功標準**:
- 安全性：0 SQL Injection 漏洞（滲透測試驗證）
- 準確率：>85%（簡單查詢）、>70%（複雜查詢）

---

#### Code Interpreter 安全沙箱
**技術難點**:
- 沙箱逃逸風險
- 資源濫用（無限迴圈、記憶體炸彈）
- 檔案系統隔離

**實施策略**:
1. **4 層安全防護**（參考 US 3.2）:
   - Layer 1: 程式碼靜態分析（禁止危險 API）
   - Layer 2: Docker 容器隔離
   - Layer 3: 資源限制（CPU、記憶體、時間）
   - Layer 4: 網路隔離（無外部連線）

2. **測試驗證**:
   - 沙箱逃逸測試（嘗試讀取宿主機檔案）
   - 資源濫用測試（無限迴圈、記憶體炸彈）
   - 性能壓測（大量並發執行）

**成功標準**:
- 0 沙箱逃逸漏洞
- 資源限制 100% 生效
- 執行成功率 >95%

---

#### Knowledge 準確率優化
**技術難點**:
- RAG 檢索不準確（找不到相關文件）
- Chunk 切分策略影響準確率
- 多模態（圖片、表格）識別困難

**實施策略**:
1. **Chunking 優化**:
   - 語義切分（基於段落和句子邊界）
   - 保留上下文（Chunk 之間有 overlap）
   - 元數據增強（標題、來源、日期）

2. **檢索增強**:
   - Hybrid Search（向量 + 關鍵字）
   - Reranking（二次排序提升準確率）
   - Query Expansion（查詢改寫）

3. **多模態處理**:
   - 圖片 OCR（Azure Computer Vision）
   - 表格識別（Azure Form Recognizer）
   - 圖表理解（GPT-4 Vision）

**成功標準**:
- 檢索準確率（Recall@10）>90%
- 回應相關性（Relevance）>85%
- 用戶滿意度 >80%

---

## 4. MVP 範圍與時程規劃

### MVP 交付範圍（10-12 個月）

**優先級 P0（MVP 必須）- 28 個 User Stories**:
```yaml
Core_Modules:
  Agent_Management:
    - US 1.1, 1.2, 1.3, 1.4  # Agent CRUD 和執行

  Plugin_System:
    - US 2.1, 2.2, 2.3  # Plugin 機制

  Code_Interpreter:
    - US 3.1, 3.2, 3.3  # 沙箱和執行

  Multi-Agent:
    - US 4.1, 4.2  # 基礎協作（4.3 Phase 2）

  Knowledge:
    - US 5.1, 5.2, 5.3, 5.4, 5.5  # Knowledge 管理（5.6 Phase 2）

  Chat_UI:
    - US 6.1, 6.2, 6.3, 6.4  # Chat 介面（6.5, 6.6 Phase 1.5）

Differentiation:
  Persona:
    - US 7.1, 7.2  # Persona 配置和 Prompt（7.3, 7.4 Phase 2）

  Text-to-SQL:
    - US 8.1, 8.2, 8.3  # Schema、生成、權限（8.4 Phase 1.5）

Enterprise:
  RBAC:
    - US 9.1  # 角色權限

  Multi-Tenant:
    - US 9.2  # 多租戶隔離

  i18n:
    - US 9.3  # 國際化（Phase 1.5）

  API_Metering:
    - US 9.4  # 計量限流（Phase 1.5）

Monitoring:
  - US 10.1  # 監控儀表板（10.2, 10.3 Phase 2）
```

**優先級 P1（MVP 高優先）- 10 個 User Stories**:
- 部分在 MVP 中交付，部分延後到 Phase 1.5
- 包括：i18n、API Metering、Chat UI 增強、Text-to-SQL 結果展示等

**優先級 P2（Phase 2）- 4 個 User Stories**:
- Persona 演化學習（US 7.4）
- Multi-Agent 高級路由（US 4.3）
- Knowledge 自動化（US 5.6）
- 用戶行為分析（US 10.3）

---

### 時程規劃（調整為 10-12 個月）

```yaml
Phase_1_Foundation: "月 1-3 (基礎架構)"
  Month_1:
    - 系統架構設計（ADD 落地）
    - 開發環境建置
    - 核心模組框架（Agent、Plugin、LLM 整合）

  Month_2:
    - Agent 管理系統（US 1.1-1.4）
    - Plugin 系統（US 2.1-2.3）
    - RBAC 基礎（US 9.1）

  Month_3:
    - Multi-Tenant 架構（US 9.2）
    - API Metering（US 9.4）
    - 監控儀表板（US 10.1）

Phase_2_Core_Features: "月 4-6 (核心功能)"
  Month_4:
    - Code Interpreter 沙箱（US 3.1-3.2）
    - Knowledge 管理（US 5.1-5.2）

  Month_5:
    - Knowledge 檢索和增強（US 5.3-5.5）
    - Multi-Agent 基礎協作（US 4.1-4.2）

  Month_6:
    - Chat UI（US 6.1-6.4）
    - 整合測試和性能優化

Phase_3_Differentiation: "月 7-9 (差異化能力)"
  Month_7:
    - Persona Framework（US 7.1-7.2）
    - Persona 風格驗證（US 7.3）

  Month_8:
    - Text-to-SQL Schema（US 8.1）
    - Text-to-SQL 生成（US 8.2）

  Month_9:
    - Text-to-SQL 權限（US 8.3）
    - SQL 結果展示（US 8.4）

Phase_4_Polish: "月 10-12 (打磨和發布)"
  Month_10:
    - i18n 國際化（US 9.3）
    - UI/UX 優化
    - 品質監控（US 10.2）

  Month_11:
    - 完整系統測試
    - 性能優化和壓測
    - 安全審計

  Month_12:
    - UAT（User Acceptance Testing）
    - 文檔和培訓材料
    - MVP 發布準備
```

---

## 5. 附錄

### A. Story Point 估算參考

```yaml
Complexity_Guidelines:
  1-2_points: "簡單功能，1-2 天完成"
  3-5_points: "中等功能，3-5 天完成"
  8-13_points: "複雜功能，1-2 週完成"
  20+_points: "需拆分為更小的 User Story"

Estimated_Story_Points:
  # 模組 1: Agent 管理系統
  US_1.1: 5   # Agent CRUD
  US_1.2: 8   # Agent 配置
  US_1.3: 13  # LLM 整合（多個 Provider）
  US_1.4: 5   # Agent 執行
  US_1.5: 8   # Agent 版本控制
  US_1.6: 5   # Agent 測試

  # 模組 2: Plugin 系統
  US_2.1: 8   # Plugin 介面
  US_2.2: 13  # 安全機制
  US_2.3: 3   # Marketplace

  # 模組 3: Code Interpreter
  US_3.1: 13  # Python 執行
  US_3.2: 13  # 4 層安全
  US_3.3: 5   # 結果展示

  # 模組 7: Persona Framework（高複雜度）
  US_7.1: 8   # Persona 配置
  US_7.2: 13  # Prompt Engineering（需大量調校）
  US_7.3: 13  # 風格一致性驗證（需 NLP）
  US_7.4: 20  # Persona 學習（AI 驅動，Phase 2）

  # 模組 8: Text-to-SQL（高複雜度）
  US_8.1: 13  # Schema 理解
  US_8.2: 20  # Text-to-SQL（安全 + 準確率）
  US_8.3: 13  # Row-Level Security
  US_8.4: 8   # 結果展示

Total_Estimated: "約 300-350 Story Points"
Velocity_Assumption: "每月 25-30 Story Points（3 人團隊）"
Estimated_Duration: "10-12 個月"
```

---

### B. 相關 ADR 參考

建議建立以下 Architecture Decision Records：

```markdown
ADR-001: LLM Provider 抽象層設計
ADR-002: Plugin 沙箱技術選型（Docker vs gVisor）
ADR-003: Code Interpreter 4 層安全架構
ADR-004: Multi-Tenant 資料隔離策略（Shared DB + Query Filter）
ADR-005: Knowledge Embedding 模型選擇（Azure OpenAI vs OSS）
ADR-006: Persona Prompt Engineering 最佳實踐
ADR-007: Text-to-SQL 安全防護機制
ADR-008: Row-Level Security 實施方案
ADR-009: API Metering 技術選型（Redis vs InfluxDB）
ADR-010: 監控和告警架構（Prometheus + Grafana）
```

---

### C. 技術選型建議

```yaml
Backend:
  Language: "C# (.NET 8)"
  Framework: "ASP.NET Core 8.0"
  ORM: "Entity Framework Core 8.0"

Frontend:
  Framework: "React 18 + TypeScript"
  UI_Library: "Material-UI 或 Ant Design"
  i18n: "react-i18next"

LLM_Integration:
  Primary: "Azure OpenAI"
  Fallback: "OpenAI API, Anthropic Claude"
  Abstraction: "Semantic Kernel"

Knowledge_Management:
  Vector_DB: "Azure AI Search（內建向量搜尋）"
  Alternative: "Pinecone, Weaviate, Qdrant"
  Embedding: "text-embedding-ada-002"

Code_Interpreter:
  Sandbox: "Docker（Phase 1）→ gVisor（Phase 2）"
  Language: "Python 3.11"
  Execution_Engine: "IPython Kernel"

Database:
  Primary: "SQL Server（Azure SQL）"
  Cache: "Redis（Azure Cache for Redis）"
  Time_Series: "InfluxDB（Metrics 和 Logs）"

Monitoring:
  Metrics: "Prometheus + Grafana"
  APM: "Application Insights"
  Logging: "Serilog + Azure Log Analytics"

DevOps:
  Container: "Docker"
  Orchestration: "Kubernetes（AKS）"
  CI/CD: "Azure DevOps 或 GitHub Actions"
  IaC: "Terraform 或 Bicep"
```

---

## 總結

本文檔定義了 Semantic Kernel Agentic Framework 的完整 User Stories（42 個），涵蓋：

✅ **6 大核心模組**：Agent、Plugin、Code Interpreter、Multi-Agent、Knowledge、Chat
✅ **2 大差異化能力**：Persona Framework、Structured Data + Text-to-SQL
✅ **4 大企業級基礎**：RBAC、Multi-Tenant、i18n、API Metering
✅ **1 個監控分析模組**：系統監控、品質分析、用戶行為

**MVP 時程**: 10-12 個月（28 個 P0 User Stories）
**核心差異化**: Persona、Text-to-SQL、Code Interpreter、Knowledge 90%+ 準確率
**企業價值**: 安全、可擴展、可監控、全球化

**下一步**:
1. 團隊 Review 本文檔
2. 確認 MVP 範圍和時程
3. 開始 Sprint Planning（基於 Story Points 估算）
4. 建立相關 ADR 文檔

---

**文檔版本**: 1.0.0 (完整版)
**最後更新**: 2025-10-28
**狀態**: ✅ 已完成