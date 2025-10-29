# ADR-007: Multi-Agent 通訊機制

## 狀態

✅ **已接受** - 2025-10-28

## 背景

Multi-Agent Orchestration 是本框架的核心差異化能力。我們需要設計一個靈活的通訊機制，讓多個 Agent 能夠協作完成複雜任務。

## 決策

採用 **Phase-Based Strategy（階段性策略）**：

```yaml
Phase 1 (MVP - Month 1-8):
  技術選型: MediatR (內存 Event Bus)
  部署模式: 單進程 (Docker Compose)
  通訊模式: In-Process Events

Phase 2 (Kubernetes - Month 9-18):
  技術選型: Azure Service Bus (Message Queue)
  部署模式: 多實例 (Kubernetes Pods)
  通訊模式: Distributed Messaging

遷移策略:
  抽象層: IAgentMessaging 接口
  實現切換: MediatR → Azure Service Bus (無需修改業務邏輯)
```

## 架構設計

### 通訊抽象層

```csharp
// Agent 消息抽象
public interface IAgentMessage
{
    string MessageId { get; }
    string FromAgentId { get; }
    string ToAgentId { get; }
    string MessageType { get; }
    object Payload { get; }
    DateTime Timestamp { get; }
}

// Agent 通訊抽象層
public interface IAgentMessaging
{
    // 發送消息
    Task SendAsync(IAgentMessage message, CancellationToken cancellationToken = default);

    // 發送並等待響應（Request-Response 模式）
    Task<TResponse> SendAndWaitAsync<TResponse>(
        IAgentMessage message,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    // 訂閱消息
    void Subscribe<TMessage>(Func<TMessage, Task> handler) where TMessage : IAgentMessage;

    // 發布事件（一對多）
    Task PublishAsync(IAgentMessage message, CancellationToken cancellationToken = default);
}
```

### Phase 1 實現：MediatR (Event Bus)

```csharp
// MediatR 實現（內存事件總線）
public class MediatRAgentMessaging : IAgentMessaging
{
    private readonly IMediator _mediator;

    public MediatRAgentMessaging(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SendAsync(IAgentMessage message, CancellationToken cancellationToken = default)
    {
        // 轉換為 MediatR Notification
        var notification = new AgentMessageNotification(message);
        await _mediator.Publish(notification, cancellationToken);
    }

    public async Task<TResponse> SendAndWaitAsync<TResponse>(
        IAgentMessage message,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        // 轉換為 MediatR Request
        var request = new AgentMessageRequest<TResponse>(message);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        return await _mediator.Send(request, cts.Token);
    }

    public void Subscribe<TMessage>(Func<TMessage, Task> handler) where TMessage : IAgentMessage
    {
        // MediatR 通過註冊 INotificationHandler 實現
        // 在 DI 容器中註冊 handler
    }

    public async Task PublishAsync(IAgentMessage message, CancellationToken cancellationToken = default)
    {
        await SendAsync(message, cancellationToken);
    }
}

// MediatR Handler 範例
public class TaskDelegationHandler : INotificationHandler<AgentMessageNotification>
{
    private readonly IAgentOrchestrator _orchestrator;

    public async Task Handle(AgentMessageNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message.MessageType == "TaskDelegation")
        {
            // 處理任務委派邏輯
            var task = (TaskDelegationPayload)message.Payload;
            await _orchestrator.DelegateTaskAsync(task, cancellationToken);
        }
    }
}
```

### Phase 2 實現：Azure Service Bus (Message Queue)

```csharp
// Azure Service Bus 實現（分布式消息隊列）
public class ServiceBusAgentMessaging : IAgentMessaging
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusProcessor _processor;
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers;

    public ServiceBusAgentMessaging(ServiceBusClient client, string queueName)
    {
        _client = client;
        _sender = _client.CreateSender(queueName);
        _processor = _client.CreateProcessor(queueName);
        _handlers = new Dictionary<Type, List<Func<object, Task>>>();

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    public async Task SendAsync(IAgentMessage message, CancellationToken cancellationToken = default)
    {
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message))
        {
            MessageId = message.MessageId,
            ContentType = "application/json",
            Subject = message.MessageType
        };

        // 添加自定義屬性
        serviceBusMessage.ApplicationProperties["FromAgentId"] = message.FromAgentId;
        serviceBusMessage.ApplicationProperties["ToAgentId"] = message.ToAgentId;

        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    public async Task<TResponse> SendAndWaitAsync<TResponse>(
        IAgentMessage message,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        // 實現 Request-Response 模式
        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<TResponse>();

        // 1. 訂閱響應隊列
        var replyQueueName = $"reply-{correlationId}";
        var replyReceiver = _client.CreateReceiver(replyQueueName);

        // 2. 發送請求消息（帶 ReplyTo 和 CorrelationId）
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message))
        {
            MessageId = message.MessageId,
            CorrelationId = correlationId,
            ReplyTo = replyQueueName,
            TimeToLive = timeout
        };

        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        // 3. 等待響應（或超時）
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            var response = await replyReceiver.ReceiveMessageAsync(timeout, cts.Token);
            if (response != null)
            {
                var responsePayload = JsonSerializer.Deserialize<TResponse>(response.Body.ToString());
                await replyReceiver.CompleteMessageAsync(response, cancellationToken);
                return responsePayload;
            }
        }
        finally
        {
            await replyReceiver.DisposeAsync();
        }

        throw new TimeoutException($"No response received within {timeout}");
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var message = JsonSerializer.Deserialize<IAgentMessage>(args.Message.Body.ToString());
        var messageType = Type.GetType(message.MessageType);

        if (_handlers.TryGetValue(messageType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                await handler(message);
            }
        }

        await args.CompleteMessageAsync(args.Message);
    }

    public void Subscribe<TMessage>(Func<TMessage, Task> handler) where TMessage : IAgentMessage
    {
        var messageType = typeof(TMessage);
        if (!_handlers.ContainsKey(messageType))
        {
            _handlers[messageType] = new List<Func<object, Task>>();
        }

        _handlers[messageType].Add(async (msg) => await handler((TMessage)msg));
    }

    public async Task StartAsync()
    {
        await _processor.StartProcessingAsync();
    }

    public async Task StopAsync()
    {
        await _processor.StopProcessingAsync();
    }
}
```

### Multi-Agent 協作場景

#### 場景 1：任務委派（Task Delegation）

```csharp
// Orchestrator Agent 委派任務給 Worker Agent
public class OrchestratorAgent
{
    private readonly IAgentMessaging _messaging;

    public async Task ExecuteComplexTaskAsync(string taskDescription)
    {
        // 1. 將複雜任務分解為子任務
        var subtasks = await _taskGenerator.DecomposeAsync(taskDescription);

        // 2. 為每個子任務委派給專門的 Worker Agent
        foreach (var subtask in subtasks)
        {
            var message = new AgentMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                FromAgentId = "orchestrator-001",
                ToAgentId = subtask.AssignedAgentId,
                MessageType = "TaskDelegation",
                Payload = new TaskDelegationPayload
                {
                    TaskId = subtask.Id,
                    TaskDescription = subtask.Description,
                    Parameters = subtask.Parameters
                }
            };

            // 發送任務委派消息
            await _messaging.SendAsync(message);
        }
    }
}

// Worker Agent 接收並執行子任務
public class WorkerAgent
{
    private readonly IAgentMessaging _messaging;

    public void Initialize()
    {
        // 訂閱任務委派消息
        _messaging.Subscribe<AgentMessage>(async (message) =>
        {
            if (message.MessageType == "TaskDelegation")
            {
                var payload = (TaskDelegationPayload)message.Payload;
                await ExecuteSubtaskAsync(payload);

                // 任務完成後，回報結果
                var resultMessage = new AgentMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    FromAgentId = this.AgentId,
                    ToAgentId = message.FromAgentId,
                    MessageType = "TaskCompleted",
                    Payload = new TaskResultPayload
                    {
                        TaskId = payload.TaskId,
                        Result = "任務完成",
                        Success = true
                    }
                };

                await _messaging.SendAsync(resultMessage);
            }
        });
    }
}
```

#### 場景 2：協作決策（Collaborative Decision）

```csharp
// 多個 Agent 協作做出決策
public class CollaborativeDecisionMaker
{
    private readonly IAgentMessaging _messaging;

    public async Task<Decision> MakeDecisionAsync(string question)
    {
        // 1. 向所有相關 Agent 廣播問題
        var broadcastMessage = new AgentMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            FromAgentId = "decision-maker",
            ToAgentId = "*", // 廣播
            MessageType = "DecisionRequest",
            Payload = new DecisionRequestPayload { Question = question }
        };

        await _messaging.PublishAsync(broadcastMessage);

        // 2. 收集所有 Agent 的意見（30秒超時）
        var opinions = new List<Opinion>();
        var timeout = TimeSpan.FromSeconds(30);
        var deadline = DateTime.UtcNow.Add(timeout);

        _messaging.Subscribe<AgentMessage>(async (message) =>
        {
            if (message.MessageType == "DecisionResponse")
            {
                var payload = (DecisionResponsePayload)message.Payload;
                opinions.Add(payload.Opinion);
            }
        });

        // 等待收集意見
        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(100);
        }

        // 3. 綜合所有意見，做出最終決策
        var finalDecision = await _decisionEngine.SynthesizeAsync(opinions);
        return finalDecision;
    }
}
```

## 理由

### 為什麼 Phase 1 選擇 MediatR？

1. **實現簡單**：
   - MediatR 是成熟的 .NET 庫，廣泛使用
   - 與 .NET DI 容器無縫集成
   - 學習曲線低

2. **低延遲**：
   - 內存級別通訊（微秒級）
   - 無網絡開銷
   - 適合 MVP 快速驗證

3. **易於調試**：
   - 所有事件流在同一進程內
   - 可以直接斷點調試
   - 日誌追蹤簡單

4. **符合 MVP 部署模式**：
   - Docker Compose 單進程部署
   - 無需額外的消息隊列基礎設施

### 為什麼 Phase 2 遷移到 Azure Service Bus？

1. **支持分布式**：
   - Kubernetes 多 Pod 部署
   - Agent 可以在不同實例上運行
   - 跨地區通訊

2. **高可靠性**：
   - 消息持久化（不怕進程崩潰）
   - 自動重試機制
   - Dead Letter Queue（處理失敗消息）

3. **可擴展性**：
   - 支持高吞吐（每秒數千消息）
   - 支持 Topic/Subscription（複雜路由）
   - 支持優先級隊列

4. **企業級特性**：
   - 消息去重
   - 事務性發送
   - 消息調度（延遲發送）

### 為什麼使用抽象層（IAgentMessaging）？

- **平滑遷移**：從 MediatR 切換到 Service Bus，業務邏輯無需修改
- **測試友好**：可以輕鬆 Mock `IAgentMessaging` 進行單元測試
- **靈活性**：未來可以切換到其他消息隊列（RabbitMQ、Kafka 等）

## 影響

### 正面影響

- ✅ **低風險 MVP**：Phase 1 使用成熟的 MediatR，實現快速
- ✅ **平滑遷移**：通過抽象層，Phase 2 遷移無痛
- ✅ **高性能**：Phase 1 內存通訊低延遲，Phase 2 分布式高吞吐
- ✅ **靈活性**：未來可以支持其他消息隊列

### 負面影響

- ⚠️ **Phase 1 限制**：僅支持單進程，無法跨實例通訊
- ⚠️ **遷移成本**：Phase 2 需要測試和驗證遷移
- ⚠️ **運維複雜度**：Phase 2 需要管理 Azure Service Bus

### 緩解措施

1. **充分測試**：Phase 1 結束時，進行完整的集成測試
2. **藍綠部署**：Phase 2 遷移時，使用藍綠部署降低風險
3. **監控與警報**：Service Bus 消息隊列長度、延遲監控

## 遷移策略

### Phase 1 → Phase 2 遷移步驟

```yaml
Step 1: 準備工作 (Week 25-26)
  - 創建 Azure Service Bus 命名空間
  - 創建 Queue 和 Topic
  - 實現 ServiceBusAgentMessaging 類
  - 編寫遷移測試用例

Step 2: 並行運行 (Week 27-28)
  - 配置 Feature Flag（雙寫模式）
  - MediatR 和 Service Bus 並行發送消息
  - 驗證消息一致性

Step 3: 逐步切換 (Week 29-30)
  - 20% 流量切換到 Service Bus
  - 監控錯誤率和延遲
  - 逐步增加到 100%

Step 4: 清理 (Week 31-32)
  - 移除 MediatR 相關代碼
  - 更新文檔和部署配置
```

## 相關決策

- ADR-006: Agent 狀態管理策略
- ADR-008: Code Interpreter 執行模型
- ADR-005: 選擇 Docker Compose (MVP) → Kubernetes (Phase 2)

## 參考資料

- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
- [Enterprise Integration Patterns](https://www.enterpriseintegrationpatterns.com/)
