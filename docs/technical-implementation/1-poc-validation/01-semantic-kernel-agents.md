# PoC 1: Semantic Kernel Agents 驗證

**優先級**: 🔴 P0 (最高優先級)
**預計時間**: 2 days (Day 1-2)
**狀態**: ⏳ 待開始
**負責人**: Backend Tech Lead

**基於**: Semantic Kernel 1.66.0+ (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Semantic Kernel 1.66+ Agents Framework 的以下核心功能：

1. ✅ **Agent 創建與執行**: 快速創建並執行 Agent
2. ✅ **多 LLM Provider 支持**: Azure OpenAI, OpenAI, Anthropic Claude 切換
3. ✅ **Plugin 機制**: 註冊和調用自定義 Plugin
4. ✅ **對話歷史管理**: 上下文保持與記憶管理
5. ✅ **性能基準**: Agent 創建與響應時間

---

## 📋 技術背景

### Semantic Kernel 架構

```
┌─────────────────────────────────────────┐
│         Semantic Kernel Core            │
│  ┌─────────────────────────────────┐   │
│  │   Agents Framework (1.66+)      │   │
│  │  - AgentBuilder                 │   │
│  │  - ChatCompletionAgent          │   │
│  │  - AgentGroupChat               │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │   Plugin System                 │   │
│  │  - Plugin Registration          │   │
│  │  - Function Calling             │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │   LLM Connectors                │   │
│  │  - Azure OpenAI                 │   │
│  │  - OpenAI                       │   │
│  │  - Anthropic                    │   │
│  └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

### 關鍵版本信息

- **Semantic Kernel**: 1.66.0+ (Agents Framework GA)
- **.NET SDK**: 8.0.x LTS
- **C# 語言**: 12.0
- **Azure OpenAI**: GPT-4o / GPT-4.1
- **OpenAI**: GPT-4 / GPT-3.5-turbo
- **Anthropic**: Claude 3.5 Sonnet

---

## 🔧 環境準備

### 1. 安裝必需工具

```bash
# 驗證 .NET SDK 版本
dotnet --version  # 應顯示 8.0.x

# 安裝 Semantic Kernel NuGet 套件
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.SemanticKernel.Agents.Core --version 1.66.0-preview
```

### 2. 配置 API 密鑰

創建 `appsettings.Development.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-azure-openai-key",
    "DeploymentName": "gpt-4o"
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-key"
  },
  "Anthropic": {
    "ApiKey": "sk-ant-your-anthropic-key"
  }
}
```

### 3. 創建 PoC 項目

```bash
# 創建新的 .NET Console 項目
mkdir poc-sk-agents
cd poc-sk-agents
dotnet new console
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

---

## 💻 實現步驟

### 步驟 1: Agent 基礎創建

**文件**: `Program.cs`

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

// 讀取配置
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json")
    .Build();

// 創建 Kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
    endpoint: configuration["AzureOpenAI:Endpoint"]!,
    apiKey: configuration["AzureOpenAI:ApiKey"]!
);
var kernel = builder.Build();

// 創建 Agent
var agent = new ChatCompletionAgent
{
    Name = "AssistantAgent",
    Instructions = "You are a helpful AI assistant. Answer questions concisely.",
    Kernel = kernel
};

Console.WriteLine("✅ Agent created successfully!");
Console.WriteLine($"Agent Name: {agent.Name}");
```

**驗證點**:
- ✅ 編譯無錯誤
- ✅ Agent 創建時間 <5 秒
- ✅ 輸出顯示 Agent 名稱

---

### 步驟 2: Agent 對話測試

```csharp
// 創建 Chat History
var history = new ChatHistory();
history.AddUserMessage("What is Semantic Kernel?");

// Agent 執行對話
Console.WriteLine("\n📨 Sending message to agent...");
var startTime = DateTime.UtcNow;

await foreach (var response in agent.InvokeAsync(history))
{
    Console.WriteLine($"🤖 Agent: {response.Content}");
}

var elapsedTime = DateTime.UtcNow - startTime;
Console.WriteLine($"\n⏱️ Response time: {elapsedTime.TotalMilliseconds}ms");
```

**驗證點**:
- ✅ Agent 正確響應用戶問題
- ✅ 響應時間記錄
- ✅ 對話內容有意義

---

### 步驟 3: 多 LLM Provider 切換

#### Azure OpenAI Provider

```csharp
public static ChatCompletionAgent CreateAzureAgent(IConfiguration config)
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
        deploymentName: config["AzureOpenAI:DeploymentName"]!,
        endpoint: config["AzureOpenAI:Endpoint"]!,
        apiKey: config["AzureOpenAI:ApiKey"]!
    );

    return new ChatCompletionAgent
    {
        Name = "AzureAgent",
        Instructions = "You are powered by Azure OpenAI GPT-4o.",
        Kernel = builder.Build()
    };
}
```

#### OpenAI Provider

```csharp
public static ChatCompletionAgent CreateOpenAIAgent(IConfiguration config)
{
    var builder = Kernel.CreateBuilder();
    builder.AddOpenAIChatCompletion(
        modelId: "gpt-4",
        apiKey: config["OpenAI:ApiKey"]!
    );

    return new ChatCompletionAgent
    {
        Name = "OpenAIAgent",
        Instructions = "You are powered by OpenAI GPT-4.",
        Kernel = builder.Build()
    };
}
```

#### Anthropic Claude Provider

```csharp
// 注意: Semantic Kernel 1.66 可能需要自定義 Connector
// 參考: https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/
public static ChatCompletionAgent CreateAnthropicAgent(IConfiguration config)
{
    // 使用 HttpClient 實現 Anthropic API 調用
    // 或等待官方 Anthropic Connector
    throw new NotImplementedException("Anthropic provider to be implemented");
}
```

**驗證點**:
- ✅ Azure OpenAI Agent 正常工作
- ✅ OpenAI Agent 正常工作
- ✅ Provider 切換無錯誤
- ⚠️ Anthropic 支持需驗證 (可能需要自定義實現)

---

### 步驟 4: Plugin 註冊與調用

**創建自定義 Plugin**:

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;

public class WeatherPlugin
{
    [KernelFunction, Description("Get the current weather for a city")]
    public string GetWeather(
        [Description("The city name")] string city)
    {
        // 模擬天氣 API 調用
        return $"The weather in {city} is sunny, 25°C.";
    }

    [KernelFunction, Description("Get weather forecast for the next N days")]
    public string GetForecast(
        [Description("The city name")] string city,
        [Description("Number of days")] int days)
    {
        return $"Forecast for {city} next {days} days: Mostly sunny.";
    }
}
```

**註冊 Plugin 到 Agent**:

```csharp
// 添加 Plugin 到 Kernel
kernel.Plugins.AddFromType<WeatherPlugin>();

// 創建帶 Plugin 的 Agent
var agentWithPlugin = new ChatCompletionAgent
{
    Name = "WeatherAssistant",
    Instructions = @"You are a weather assistant.
                     Use the GetWeather function to provide weather information.
                     Always use the available functions when appropriate.",
    Kernel = kernel
};

// 測試 Plugin 調用
history.Clear();
history.AddUserMessage("What's the weather in Tokyo?");

await foreach (var response in agentWithPlugin.InvokeAsync(history))
{
    Console.WriteLine($"🤖 Agent (with plugin): {response.Content}");
}
```

**驗證點**:
- ✅ Plugin 成功註冊
- ✅ Agent 自動調用 Plugin 函數
- ✅ Plugin 響應正確返回

---

### 步驟 5: 對話歷史與上下文管理

```csharp
// 多輪對話測試
var history = new ChatHistory();

// 第一輪對話
history.AddUserMessage("My name is Alice.");
await foreach (var response in agent.InvokeAsync(history))
{
    history.AddAssistantMessage(response.Content!);
    Console.WriteLine($"🤖 Agent: {response.Content}");
}

// 第二輪對話 (測試上下文記憶)
history.AddUserMessage("What's my name?");
await foreach (var response in agent.InvokeAsync(history))
{
    history.AddAssistantMessage(response.Content!);
    Console.WriteLine($"🤖 Agent: {response.Content}");
}

// 驗證 Agent 是否記住用戶名字
// 預期輸出: "Your name is Alice."
```

**驗證點**:
- ✅ Agent 記住前面對話內容
- ✅ 上下文在多輪對話中保持
- ✅ ChatHistory 正確累積

---

## 🧪 測試用例

### 測試套件 1: Agent 創建性能

| 測試場景 | 操作 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-1.1 | 創建單個 Agent | Agent 成功創建 | <5 秒 |
| TC-1.2 | 創建 10 個 Agent | 所有 Agent 成功創建 | <10 秒 |
| TC-1.3 | Agent 內存佔用 | 記錄內存使用量 | <100 MB/Agent |

### 測試套件 2: LLM Provider 切換

| 測試場景 | Provider | 預期結果 | 成功標準 |
|---------|----------|----------|----------|
| TC-2.1 | Azure OpenAI | 正常響應 | 成功率 >95% |
| TC-2.2 | OpenAI | 正常響應 | 成功率 >95% |
| TC-2.3 | Anthropic | 正常響應 (可選) | 成功率 >95% |
| TC-2.4 | Provider 切換 | 無錯誤 | 100% 成功 |

### 測試套件 3: Plugin 調用

| 測試場景 | 操作 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-3.1 | 註冊單個 Plugin | 成功註冊 | 100% 成功 |
| TC-3.2 | 註冊多個 Plugin (5個) | 所有成功註冊 | 100% 成功 |
| TC-3.3 | Agent 自動調用 Plugin | 正確調用 | 成功率 >90% |
| TC-3.4 | Plugin 參數傳遞 | 參數正確 | 100% 正確 |

### 測試套件 4: 對話管理

| 測試場景 | 操作 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-4.1 | 單輪對話 | 正確響應 | 100% 成功 |
| TC-4.2 | 多輪對話 (5輪) | 上下文保持 | 成功率 >95% |
| TC-4.3 | 長對話 (50輪) | 上下文不丟失 | 成功率 >90% |
| TC-4.4 | 對話歷史持久化 | 正確保存/恢復 | 100% 成功 |

---

## ✅ 成功標準驗證

### 1. Agent 創建時間 <5 秒 ✅ / ❌

**測試代碼**:
```csharp
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
var agent = CreateAgent();
stopwatch.Stop();

Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000,
              $"Agent creation took {stopwatch.ElapsedMilliseconds}ms");
```

**結果**: ✅ / ❌ (記錄實際時間: _____ms)

### 2. Plugin 調用成功率 >95% ✅ / ❌

**測試代碼**:
```csharp
int totalCalls = 100;
int successfulCalls = 0;

for (int i = 0; i < totalCalls; i++)
{
    try
    {
        var result = await agentWithPlugin.InvokeAsync(history);
        if (result != null) successfulCalls++;
    }
    catch { }
}

double successRate = (double)successfulCalls / totalCalls * 100;
Assert.IsTrue(successRate >= 95.0,
              $"Plugin call success rate: {successRate}%");
```

**結果**: ✅ / ❌ (實際成功率: _____%);

### 3. 支持 3+ LLM Providers ✅ / ❌

**測試結果**:
- Azure OpenAI: ✅ / ❌
- OpenAI: ✅ / ❌
- Anthropic: ✅ / ❌ (可選)

**結果**: ✅ / ❌ (通過 Providers 數量: _____/3)

### 4. 對話歷史管理正常 ✅ / ❌

**測試場景**:
- 多輪對話上下文保持: ✅ / ❌
- 對話歷史持久化: ✅ / ❌

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 技術發現

記錄驗證過程中的技術發現：

1. **Semantic Kernel Agents Framework 穩定性**:
   - [ ] GA 版本穩定，可用於生產
   - [ ] 發現 Bug: _________________________
   - [ ] 性能問題: _________________________

2. **LLM Provider 整合**:
   - [ ] Azure OpenAI 整合流暢
   - [ ] OpenAI 整合需注意: _________________________
   - [ ] Anthropic 支持情況: _________________________

3. **Plugin 機制**:
   - [ ] Function Calling 自動調用可靠
   - [ ] Plugin 參數解析正確
   - [ ] 發現限制: _________________________

4. **性能基準**:
   - Agent 創建平均時間: _____ ms
   - 首次響應平均時間: _____ ms
   - 記憶體佔用: _____ MB/Agent

---

## ⚠️ 風險識別

### 識別到的風險

| 風險 | 嚴重性 | 影響 | 緩解方案 |
|------|--------|------|----------|
| Semantic Kernel 版本不穩定 | 高 | 生產可靠性 | 等待穩定版本 / 鎖定版本 |
| Anthropic 官方支持缺失 | 中 | Provider 多樣性 | 自定義 Connector / 使用第三方庫 |
| Plugin 調用失敗率高 | 高 | 功能可用性 | 增加錯誤處理 / 重試機制 |
| 長對話上下文丟失 | 中 | 用戶體驗 | 實現對話壓縮 / 摘要機制 |
| (其他風險) | - | - | - |

---

## 💡 建議與下一步

### 技術建議

1. **Agent 架構設計**:
   - 建議採用 Factory Pattern 創建不同類型的 Agent
   - 實現 Agent Pool 以提高創建效率
   - 設計 Agent 生命週期管理機制

2. **Plugin 開發規範**:
   - 制定 Plugin 開發最佳實踐
   - 實現 Plugin 版本管理
   - 設計 Plugin 安全沙箱機制

3. **性能優化**:
   - 實現 Agent 實例緩存
   - 優化對話歷史存儲
   - 實現 LLM 響應緩存

### 下一步行動

- [ ] 完成所有測試用例
- [ ] 撰寫詳細驗證報告
- [ ] 更新 PoC 驗證狀態
- [ ] 準備 Demo 演示
- [ ] 團隊評審會議

---

## 📊 驗證結果總結

**執行日期**: _____________
**執行人**: _____________
**狀態**: ⏳ 進行中 / ✅ 通過 / ❌ 未通過

### 成功標準達成情況

| 成功標準 | 要求 | 實際結果 | 狀態 |
|---------|------|----------|------|
| Agent 創建時間 | <5 秒 | _____ 秒 | ✅ / ❌ |
| Plugin 調用成功率 | >95% | _____ % | ✅ / ❌ |
| LLM Provider 支持 | 3+ | _____ 個 | ✅ / ❌ |
| 對話歷史管理 | 正常 | ✅ / ❌ | ✅ / ❌ |

### 總體評估

**結論**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**理由**:
___________________________________________________________________________
___________________________________________________________________________
___________________________________________________________________________

---

## 🔗 相關文檔

- [PoC 驗證計劃總覽](./README.md)
- [TECH-STACK-ANALYSIS.md](../TECH-STACK-ANALYSIS.md)
- [Semantic Kernel 官方文檔](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Agents Framework 指南](https://learn.microsoft.com/en-us/semantic-kernel/agents/)

---

**最後更新**: 2025-10-30
**PoC 負責人**: Backend Tech Lead
**評審日期**: _____________

---

[← 返回 PoC 驗證總覽](./README.md) | [下一個 PoC: Persona Builder →](./02-persona-builder.md)
