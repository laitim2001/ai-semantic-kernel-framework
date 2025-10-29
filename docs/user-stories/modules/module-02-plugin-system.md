# 模組 02: Plugin 系統

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

---