# ADR-011: Framework 遷移策略與抽象層設計

## 狀態

✅ **已接受** - 2025-10-28

## 背景

2025年10月1日，Microsoft 發布了 **Microsoft Agent Framework**，這是一個整合了 Semantic Kernel 和 AutoGen 的新框架。考慮到：

1. **當前技術選型**：本項目已選擇 Semantic Kernel 1.x 作為 MVP 核心框架
2. **新框架風險**：Microsoft Agent Framework 僅發布 4 週，穩定性未經驗證
3. **未來靈活性**：需要保留未來遷移到新框架的可能性
4. **開放標準趨勢**：行業正在推動 MCP、OpenAPI、OpenTelemetry 等開放標準

我們需要做出框架選型決策，並設計一個能夠支持未來遷移的架構。

## 決策

採用 **Semantic Kernel 1.x + Framework Abstraction Layer + Open Standards** 策略：

```yaml
Phase 1 (MVP - Month 1-8):
  核心框架: Semantic Kernel 1.x
  抽象層: Framework Abstraction Layer (5 個核心接口)
  開放標準:
    - MCP (Model Context Protocol) - Plugin/Tool 標準
    - OpenAPI 3.0+ - API-First Tool Definition
    - OpenTelemetry - 可觀察性標準

Phase 2 評估期 (Month 9-12):
  評估條件（必須全部滿足）:
    ✅ Microsoft Agent Framework 穩定運行 >6 個月
    ✅ 有 3+ 個 Fortune 500 企業生產案例
    ✅ GitHub Issues 關閉率 >90%
    ✅ 遷移成本 <2 個月開發時間
    ✅ 性能提升 >20%

  決策: 評估是否遷移到 Microsoft Agent Framework

Phase 3 遷移期 (可選 - Month 13-15):
  如果評估通過:
    - 實現 MicrosoftAgentFrameworkAdapter
    - 使用 Feature Flag 進行灰度切換
    - 並行運行兩個框架（A/B Testing）
    - 逐步切換流量 (10% → 50% → 100%)

Phase 4 優化期 (Month 16+):
  - 移除舊框架適配器（如果遷移成功）
  - 利用新框架特性進行優化
  - 持續監控性能和穩定性
```

## 架構設計

### Framework Abstraction Layer

#### 核心接口設計

```csharp
// 1. Agent Runtime 抽象
public interface IAgentRuntime
{
    Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);

    Task<AgentState> GetStateAsync(string agentId);

    Task<AgentResponse> ResumeAsync(
        string executionId,
        CancellationToken cancellationToken = default);
}

// 2. Tool Registry 抽象（支持 MCP）
public interface IToolRegistry
{
    // MCP 標準支持
    Task RegisterMcpToolAsync(McpToolDefinition tool);
    Task RegisterFromOpenApiAsync(string openApiSpecUrl);

    // Tool 管理
    Task<IEnumerable<ToolDefinition>> GetAvailableToolsAsync();
    Task<ToolResult> InvokeToolAsync(
        string toolName,
        Dictionary<string, object> parameters);
}

// 3. Agent Memory 抽象
public interface IAgentMemory
{
    Task<string> StoreAsync(string agentId, string content, MemoryMetadata metadata);
    Task<IEnumerable<MemoryChunk>> SearchAsync(string query, int topK = 5);
    Task<bool> DeleteAsync(string memoryId);
}

// 4. Workflow Orchestrator 抽象（支持 AutoGen 模式）
public interface IWorkflowOrchestrator
{
    Task<WorkflowResult> ExecuteAsync(
        WorkflowDefinition workflow,
        OrchestrationMode mode); // Sequential, Concurrent, GroupConversation, Handoff, Supervisor

    Task<WorkflowState> GetWorkflowStateAsync(string workflowId);
    Task PauseWorkflowAsync(string workflowId);
    Task ResumeWorkflowAsync(string workflowId);
}

// 5. Observability 抽象（OpenTelemetry 標準）
public interface IObservability
{
    ISpan StartSpan(string operationName, SpanContext? parentContext = null);
    void RecordMetric(string metricName, double value, Dictionary<string, string> tags);
    void LogEvent(string eventName, LogLevel level, Dictionary<string, object> properties);
}
```

#### Semantic Kernel 適配器實現

```csharp
public class SemanticKernelRuntime : IAgentRuntime
{
    private readonly Kernel _kernel;
    private readonly ILogger<SemanticKernelRuntime> _logger;

    public SemanticKernelRuntime(Kernel kernel, ILogger<SemanticKernelRuntime> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _kernel.InvokePromptAsync(
            request.Prompt,
            new KernelArguments(request.Parameters),
            cancellationToken);

        return new AgentResponse
        {
            Content = result.ToString(),
            Metadata = new Dictionary<string, object>
            {
                ["model"] = result.Metadata?["model"],
                ["usage"] = result.Metadata?["usage"]
            }
        };
    }
}

public class SemanticKernelToolRegistry : IToolRegistry
{
    private readonly Kernel _kernel;
    private readonly HttpClient _httpClient;

    public async Task RegisterMcpToolAsync(McpToolDefinition tool)
    {
        // 將 MCP Tool 轉換為 Semantic Kernel Plugin
        var function = KernelFunctionFactory.CreateFromMethod(
            method: (string input) => InvokeMcpToolAsync(tool, input),
            functionName: tool.Name,
            description: tool.Description);

        _kernel.Plugins.AddFromFunctions(tool.Name, new[] { function });
    }

    public async Task RegisterFromOpenApiAsync(string openApiSpecUrl)
    {
        // 使用 Semantic Kernel 的 OpenAPI Plugin 支持
        var plugin = await _kernel.ImportPluginFromOpenApiAsync(
            pluginName: "OpenApiTool",
            uri: new Uri(openApiSpecUrl),
            executionParameters: new OpenApiFunctionExecutionParameters
            {
                HttpClient = _httpClient
            });
    }

    private async Task<string> InvokeMcpToolAsync(McpToolDefinition tool, string input)
    {
        // 根據 MCP Transport 調用工具
        return tool.Transport switch
        {
            McpTransport.Http => await InvokeHttpToolAsync(tool.Endpoint, input),
            McpTransport.Stdio => await InvokeStdioToolAsync(tool.Endpoint, input),
            McpTransport.SSE => await InvokeSseToolAsync(tool.Endpoint, input),
            _ => throw new NotSupportedException($"Transport {tool.Transport} not supported")
        };
    }
}
```

#### Microsoft Agent Framework 適配器（未來實現）

```csharp
public class MicrosoftAgentRuntime : IAgentRuntime
{
    private readonly AgentHost _agentHost;
    private readonly ILogger<MicrosoftAgentRuntime> _logger;

    public MicrosoftAgentRuntime(AgentHost agentHost, ILogger<MicrosoftAgentRuntime> logger)
    {
        _agentHost = agentHost;
        _logger = logger;
    }

    public async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken)
    {
        // 使用 Microsoft Agent Framework 的原生 API
        var result = await _agentHost.RunAsync(
            new AgentRunRequest
            {
                Prompt = request.Prompt,
                Parameters = request.Parameters
            },
            cancellationToken);

        return new AgentResponse
        {
            Content = result.Output,
            Metadata = result.Metadata
        };
    }
}
```

### Dependency Injection 配置

```csharp
public static class FrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddAgentFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var frameworkType = configuration.GetValue<string>("AgentFramework:Type");

        switch (frameworkType)
        {
            case "SemanticKernel":
                services.AddSemanticKernelAdapter(configuration);
                break;

            case "MicrosoftAgentFramework":
                services.AddMicrosoftAgentAdapter(configuration);
                break;

            case "Hybrid":
                services.AddHybridAdapter(configuration);
                break;

            default:
                throw new InvalidOperationException($"Unknown framework type: {frameworkType}");
        }

        // 註冊 MCP Tool Registry（框架無關）
        services.AddSingleton<IMcpToolDiscovery, McpToolDiscovery>();

        // 註冊 OpenTelemetry（框架無關）
        services.AddOpenTelemetryObservability(configuration);

        return services;
    }

    private static IServiceCollection AddSemanticKernelAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置 Semantic Kernel
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: configuration["AzureOpenAI:DeploymentName"],
            endpoint: configuration["AzureOpenAI:Endpoint"],
            apiKey: configuration["AzureOpenAI:ApiKey"]);

        var kernel = kernelBuilder.Build();

        // 註冊適配器
        services.AddSingleton(kernel);
        services.AddScoped<IAgentRuntime, SemanticKernelRuntime>();
        services.AddScoped<IToolRegistry, SemanticKernelToolRegistry>();
        services.AddScoped<IAgentMemory, SemanticKernelMemory>();
        services.AddScoped<IWorkflowOrchestrator, SemanticKernelOrchestrator>();

        return services;
    }
}
```

### 配置文件（Feature Flag 支持）

```json
{
  "AgentFramework": {
    "Type": "SemanticKernel",
    "Hybrid": {
      "DefaultRuntime": "SemanticKernel",
      "FeatureFlags": {
        "UseMAFForMonitoring": false,
        "UseMAFForPlugins": false,
        "UseMAFForMultiAgent": false
      },
      "TrafficSplit": {
        "SemanticKernel": 100,
        "MicrosoftAgentFramework": 0
      }
    }
  },
  "OpenStandards": {
    "MCP": {
      "Enabled": true,
      "DiscoveryEndpoint": "https://mcp.example.com/discover",
      "Transport": "HTTP"
    },
    "OpenAPI": {
      "Enabled": true,
      "ToolDefinitionPath": "/api/tools/openapi.json"
    },
    "OpenTelemetry": {
      "Enabled": true,
      "Endpoint": "http://localhost:4317",
      "ServiceName": "semantic-kernel-agent",
      "Exporters": ["Console", "OTLP", "Prometheus"]
    }
  }
}
```

### MCP Tool Definition 示例

```json
{
  "name": "web_search",
  "description": "搜索網路資料",
  "version": "1.0.0",
  "transport": "HTTP",
  "endpoint": "https://api.example.com/tools/web_search",
  "inputSchema": {
    "type": "object",
    "properties": {
      "query": {
        "type": "string",
        "description": "搜索查詢"
      },
      "max_results": {
        "type": "integer",
        "description": "最大結果數",
        "default": 10
      }
    },
    "required": ["query"]
  },
  "outputSchema": {
    "type": "object",
    "properties": {
      "results": {
        "type": "array",
        "items": {
          "type": "object",
          "properties": {
            "title": { "type": "string" },
            "url": { "type": "string" },
            "snippet": { "type": "string" }
          }
        }
      }
    }
  }
}
```

## 理由

### 為什麼選擇 Semantic Kernel 1.x（Phase 1）？

1. **穩定性優先**：
   - Semantic Kernel 1.x 已發布超過 1 年，穩定性已驗證
   - 有大量生產環境案例（Microsoft 內部和社群）
   - 社群支持完善，文檔齊全

2. **風險控制**：
   - Microsoft Agent Framework 僅發布 4 週，穩定性未知
   - 缺乏生產環境驗證，可能存在未知問題
   - 文檔和社群支持尚未成熟

3. **交付保障**：
   - MVP 目標是 10-12 個月內交付
   - 使用成熟框架可降低交付風險
   - 避免因框架問題影響項目進度

### 為什麼需要 Framework Abstraction Layer？

1. **未來靈活性**：
   - Microsoft Agent Framework 未來可能成為更好的選擇
   - 抽象層允許未來平滑遷移，無需重寫業務邏輯
   - 保護業務邏輯不受框架變更影響

2. **多框架並存**：
   - 支持 Hybrid 模式（部分功能使用新框架）
   - Feature Flag 支持灰度切換和 A/B Testing
   - 降低遷移風險，支持逐步切換

3. **開放標準兼容**：
   - MCP、OpenAPI、OpenTelemetry 是框架無關的標準
   - 未來可以支持更多框架（如 LangChain、Haystack）
   - 提升系統的開放性和互操作性

### 為什麼採用開放標準？

1. **MCP (Model Context Protocol)**：
   - 行業標準的 Plugin/Tool 協議
   - 支持多種 Transport（HTTP、stdio、SSE）
   - Semantic Kernel 和 Microsoft Agent Framework 都支持

2. **OpenAPI 3.0+**：
   - API-First 設計，Tool 定義即文檔
   - 自動生成 Client SDK，降低集成成本
   - 支持豐富的第三方工具生態

3. **OpenTelemetry**：
   - 可觀察性領域的事實標準
   - 支持 Metrics、Traces、Logs 統一收集
   - 與雲平台無關，避免供應商鎖定

### 為什麼不立即遷移到 Microsoft Agent Framework？

1. **穩定性風險**：
   - 框架僅發布 4 週，缺乏生產驗證
   - 可能存在重大 Bug 或設計缺陷
   - 早期版本 API 可能頻繁變更

2. **遷移成本**：
   - 立即遷移需要重寫大量代碼
   - 學習曲線高，影響開發效率
   - 可能導致 MVP 延期交付

3. **評估時間不足**：
   - 需要更多時間評估框架優劣
   - 需要社群反饋和生產案例
   - 需要性能對比和穩定性驗證

## 影響

### 正面影響

- ✅ **低風險交付**：使用成熟框架，保障 MVP 按時交付
- ✅ **未來靈活性**：抽象層支持未來遷移，保護投資
- ✅ **開放標準**：MCP、OpenAPI、OpenTelemetry 提升互操作性
- ✅ **平滑遷移**：Feature Flag 支持灰度切換，降低遷移風險
- ✅ **多框架並存**：Hybrid 模式支持逐步遷移
- ✅ **供應商中立**：開放標準避免框架鎖定

### 負面影響

- ⚠️ **抽象層複雜度**：需要維護額外的抽象層代碼
- ⚠️ **性能開銷**：抽象層可能引入輕微性能損失（<5%）
- ⚠️ **開發成本**：需要額外開發抽象層和適配器
- ⚠️ **測試複雜度**：需要測試多個框架適配器
- ⚠️ **文檔維護**：需要維護多個框架的文檔

### 緩解措施

1. **抽象層設計優化**：
   - 保持接口簡潔，避免過度抽象
   - 使用 Dependency Injection，降低耦合
   - 充分單元測試，確保抽象層正確性

2. **性能優化**：
   - 抽象層方法使用 inline（減少調用開銷）
   - 避免不必要的對象轉換
   - 使用 Benchmark 測試，確保性能損失 <5%

3. **開發效率**：
   - 提供 Code Generator，自動生成適配器代碼
   - 充分文檔和示例，降低學習成本
   - 使用 Feature Flag，簡化多框架管理

## 遷移策略

### Phase 1: MVP 開發（Month 1-8）

**目標**：完成核心功能，使用 Semantic Kernel 1.x

**工作內容**：
1. 實現 Framework Abstraction Layer 的 5 個核心接口
2. 實現 Semantic Kernel 適配器
3. 集成 MCP、OpenAPI、OpenTelemetry 開放標準
4. 完成 User Stories 的 P0 和 P1 功能
5. 部署到 MVP 環境（Docker Compose）

**驗收標準**：
- ✅ 所有 P0 User Stories 完成
- ✅ 系統穩定性 >99%
- ✅ API 響應時間 <500ms (P95)
- ✅ 單元測試覆蓋率 >80%

### Phase 2: 評估期（Month 9-12）

**目標**：評估 Microsoft Agent Framework 是否適合遷移

**評估維度**：

| 維度 | 評估標準 | 權重 |
|------|---------|------|
| **穩定性** | GitHub Issues 關閉率 >90%<br>無重大 Bug (Severity: Critical) | 30% |
| **生產驗證** | 3+ Fortune 500 企業案例<br>社群反饋積極 (>80%) | 25% |
| **性能** | 性能提升 >20%<br>延遲降低 >15% | 20% |
| **功能** | 支持所有現有功能<br>提供額外價值功能 | 15% |
| **遷移成本** | 遷移時間 <2 個月<br>代碼變更 <30% | 10% |

**工作內容**：
1. 建立 Microsoft Agent Framework PoC 環境
2. 實現核心功能的 Proof of Concept
3. 性能對比測試（Benchmark）
4. 安全性評估和合規性檢查
5. 成本分析（License、雲資源）

**評估結果**：
- **通過**：總分 >80%，無 Critical 缺陷 → 進入 Phase 3 遷移期
- **不通過**：總分 <80% → 繼續使用 Semantic Kernel，6 個月後重新評估

### Phase 3: 遷移期（Month 13-15，如果評估通過）

**目標**：平滑遷移到 Microsoft Agent Framework

**遷移步驟**：

```yaml
Step 1: 實現適配器（Week 1-2）
  - 實現 MicrosoftAgentRuntime
  - 實現 MicrosoftAgentToolRegistry
  - 實現 MicrosoftAgentMemory
  - 實現 MicrosoftAgentOrchestrator
  - 單元測試覆蓋率 >80%

Step 2: 集成測試（Week 3-4）
  - 端到端測試（E2E）
  - 性能測試（Benchmark）
  - 安全測試（Security Scan）
  - 兼容性測試（MCP、OpenAPI）

Step 3: 灰度發布（Week 5-8）
  - Week 5: 10% 流量切換（內部用戶）
  - Week 6: 25% 流量切換（Beta 用戶）
  - Week 7: 50% 流量切換（一般用戶）
  - Week 8: 100% 流量切換（全量用戶）

  每階段監控指標：
    - 錯誤率 <0.1%
    - P95 延遲 <500ms
    - 用戶反饋積極 >90%

Step 4: 穩定運行（Week 9-12）
  - 持續監控性能和穩定性
  - 收集用戶反饋
  - 優化性能瓶頸
  - 修復遷移引入的 Bug
```

**回滾計劃**：
- 如果任何階段錯誤率 >1%，立即回滾到 Semantic Kernel
- Feature Flag 支持一鍵回滾
- 保留 Semantic Kernel 適配器至少 3 個月

### Phase 4: 優化期（Month 16+）

**目標**：充分利用新框架特性，優化系統性能

**工作內容**：
1. 移除 Semantic Kernel 適配器（如果遷移成功且穩定）
2. 利用 Microsoft Agent Framework 特有功能優化性能
3. 集成 AutoGen 的 Multi-Agent 特性（如果適用）
4. 優化 OpenTelemetry 可觀察性配置
5. 持續監控和改進

## 風險管理

### 技術風險

| 風險 | 影響 | 概率 | 緩解措施 |
|------|------|------|---------|
| Microsoft Agent Framework 重大 Bug | 高 | 中 | Phase 2 充分評估，保留回滾能力 |
| 抽象層性能損失 >10% | 中 | 低 | Benchmark 測試，優化關鍵路徑 |
| 遷移引入新 Bug | 中 | 中 | 灰度發布，充分測試，快速回滾 |
| 開放標準實現複雜 | 低 | 中 | 使用成熟庫（如 OpenTelemetry SDK） |

### 項目風險

| 風險 | 影響 | 概率 | 緩解措施 |
|------|------|------|---------|
| MVP 延期交付 | 高 | 低 | 使用成熟框架，降低技術風險 |
| 遷移成本超預算 | 中 | 中 | Phase 2 詳細評估，只在 ROI 正向時遷移 |
| 團隊學習曲線 | 中 | 中 | 充分培訓，逐步遷移 |
| 供應商鎖定 | 低 | 低 | 開放標準，抽象層隔離 |

## 相關決策

- ADR-006: Agent 狀態管理策略
- ADR-007: Multi-Agent 通訊機制
- ADR-008: Code Interpreter 執行模型
- Section 3.3: Framework Abstraction Layer 詳細設計（Architecture-Design-Document.md）

## 參考資料

### Microsoft 官方文檔
- [Microsoft Agent Framework 公告](https://devblogs.microsoft.com/semantic-kernel/microsoft-agent-framework/)
- [Semantic Kernel 官方文檔](https://learn.microsoft.com/en-us/semantic-kernel/)
- [AutoGen 官方文檔](https://microsoft.github.io/autogen/)

### 開放標準文檔
- [MCP (Model Context Protocol)](https://spec.modelcontextprotocol.io/)
- [OpenAPI Specification 3.0+](https://swagger.io/specification/)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)

### 架構模式
- [Adapter Pattern](https://refactoring.guru/design-patterns/adapter)
- [Strategy Pattern](https://refactoring.guru/design-patterns/strategy)
- [Feature Flags Best Practices](https://martinfowler.com/articles/feature-toggles.html)

### 遷移案例
- [Migrating to Microservices with Feature Flags](https://launchdarkly.com/blog/migrating-to-microservices/)
- [Zero-Downtime Deployment Strategies](https://www.martinfowler.com/bliki/BlueGreenDeployment.html)
