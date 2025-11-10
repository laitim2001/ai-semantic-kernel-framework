# Sprint 1-2 執行路線圖

**版本**: 1.0.0
**日期**: 2025-11-03
**執行時間**: Week 1-6 (6週)
**狀態**: 📋 準備執行

---

## 📊 總覽

### Sprint 1 (Week 1-3): Agent 創建與管理
**目標**: Agent CRUD 功能完成
**User Stories**: US 1.1, 1.2, 1.3
**Story Points**: 13

**核心交付物**:
- ✅ Agent CRUD API (Backend .NET 8)
- ✅ Agent 管理 UI (Frontend React 18)
- ✅ .NET Client SDK
- ✅ PostgreSQL 數據持久化
- ✅ EF Core Migrations

### Sprint 2 (Week 4-6): Agent 執行引擎
**目標**: Agent 可執行與 Plugin 系統
**User Stories**: US 1.4, 2.1, 6.1
**Story Points**: 13

**核心交付物**:
- ✅ Agent 執行引擎 (Semantic Kernel)
- ✅ Plugin 註冊與調用
- ✅ Chat 實時對話 (SignalR)
- ✅ 執行歷史記錄
- ✅ 基礎 Plugin 範例

---

## 🗓️ Sprint 1 詳細時間表

### Week 1: US 1.1 - Agent CRUD

#### Day 1: Sprint Planning + Backend 開始
**時間**: 9:00-18:00

**Sprint Planning (9:00-12:00)**:
- Sprint 0 回顧
- US 1.1-1.3 詳細說明
- 任務拆解與估算
- 團隊承諾與分工

**Backend 開發 (14:00-18:00)**:
- 創建 Agent 數據模型
- 創建 DTOs (CreateAgentRequest, AgentResponse)
- 創建 DbContext (AgentDbContext)
- 創建 Repository (IAgentRepository, AgentRepository)

**交付物**:
```
services/agent-service/AgentService/
├── Models/
│   ├── Agent.cs
│   └── DTOs/
│       └── AgentDto.cs
├── Data/
│   ├── AgentDbContext.cs
│   └── Repositories/
│       ├── IAgentRepository.cs
│       └── AgentRepository.cs
```

---

#### Day 2: Backend API 完成
**時間**: 10:00-18:00

**任務**:
- 創建 Service Layer (IAgentService, AgentApplicationService)
- 創建 Controller (AgentsController)
- 配置 Program.cs (DI, CORS, Swagger)
- 創建 EF Core Migrations
- API 測試

**驗證**:
```bash
# Migration
dotnet ef migrations add InitialCreate
dotnet ef database update

# 運行 API
dotnet run

# 測試
curl https://localhost:7001/api/v1/agents
curl -X POST https://localhost:7001/api/v1/agents \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Agent","modelProvider":"AzureOpenAI","modelName":"gpt-4"}'
```

---

#### Day 3: Frontend UI 實現
**時間**: 10:00-18:00

**任務**:
- 創建 TypeScript 類型定義 (agent.ts)
- 創建 API Service (agentService.ts)
- 創建 Zustand Store (agentStore.ts)
- 創建 AgentList 組件 (列表、分頁、刪除)
- 創建 AgentCreateForm 組件

**交付物**:
```
apps/web-app/src/
├── types/
│   └── agent.ts
├── services/
│   └── agentService.ts
├── stores/
│   └── agentStore.ts
└── components/agents/
    ├── AgentList.tsx
    └── AgentCreateForm.tsx
```

**驗證**: 可在 UI 創建、列表、刪除 Agent

---

### Week 2: US 1.2 (.NET SDK) + US 1.3 (配置管理)

#### Day 4-5: .NET SDK 開發 (US 1.2)
**負責人**: Backend Lead

**任務**:
- 創建 NuGet 套件項目 (SK.Framework.Client.SDK)
- 實現 AgentClient 類
- 實現 Fluent API Builder Pattern
- 添加 XML 文檔註解
- 創建單元測試
- 創建範例代碼

**SDK 使用範例**:
```csharp
var client = new AgentClientBuilder()
    .WithEndpoint("https://localhost:7001")
    .WithApiKey("your-api-key")
    .Build();

var agent = await client.Agents.CreateAsync(new CreateAgentRequest
{
    Name = "My Agent",
    ModelProvider = "AzureOpenAI",
    ModelName = "gpt-4"
});
```

---

#### Day 6-7: Frontend 配置管理 (US 1.3)
**負責人**: Frontend Lead

**任務**:
- 創建 AgentDetail 組件 (詳情頁)
- 創建 AgentEditForm 組件 (編輯頁)
- 實現路由配置
- 實現表單驗證 (Yup)
- E2E 測試 (Playwright)

---

### Week 3: 測試與修復

#### Day 8-9: 集成測試與修復
**任務**:
- Backend 集成測試 (xUnit)
- Frontend E2E 測試 (Playwright)
- Bug 修復
- 性能優化 (API 響應時間 <200ms)
- Code Coverage 檢查 (目標 ≥80%)

#### Day 10: Sprint Review + Retrospective
**時間**: 9:00-17:00

**Sprint Review (9:00-11:00)**:
- Demo US 1.1-1.3 功能
- PO 驗收
- Stakeholder 反饋

**Sprint Retrospective (14:00-16:00)**:
- What went well?
- What didn't go well?
- Action items for Sprint 2

**Sprint 2 Planning (16:00-17:00)**:
- US 1.4, 2.1, 6.1 說明
- 任務拆解

---

## 🗓️ Sprint 2 詳細時間表

### Week 4: US 1.4 - Agent 執行引擎

#### Day 11-12: Semantic Kernel 集成
**任務**:
- 創建 Agent 執行引擎 (AgentExecutionService)
- Semantic Kernel 配置與初始化
- 實現 InvokeAsync API (`POST /api/v1/agents/{id}/invoke`)
- 執行歷史記錄 (ExecutionHistory 表)
- 錯誤處理與日誌記錄

**核心代碼**:
```csharp
public class AgentExecutionService
{
    private readonly Kernel _kernel;

    public async Task<ExecutionResult> InvokeAsync(
        Guid agentId,
        string userInput,
        CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);

        var function = _kernel.CreateFunctionFromPrompt(
            agent.SystemPrompt + "\n\nUser: {{$input}}",
            functionName: "AgentResponse"
        );

        var result = await _kernel.InvokeAsync(
            function,
            new() { ["input"] = userInput },
            cancellationToken
        );

        // 保存執行歷史
        await SaveExecutionHistory(agentId, userInput, result.ToString());

        return new ExecutionResult { Response = result.ToString() };
    }
}
```

---

#### Day 13: Frontend 測試界面
**任務**:
- 創建 AgentTestPanel 組件 (測試執行)
- 實現執行結果展示
- 實現執行歷史列表

---

### Week 5: US 2.1 - Plugin 系統

#### Day 14-16: Plugin 註冊與調用
**任務**:
- 定義 IPlugin 接口
- 創建 Plugin Registry
- 實現 Plugin 加載機制
- 創建 Plugin API (`POST /api/v1/plugins`)
- 創建基礎 Plugin 範例 (WeatherPlugin, CalculatorPlugin)

**Plugin 範例**:
```csharp
public class WeatherPlugin : IPlugin
{
    [KernelFunction("get_weather")]
    public async Task<string> GetWeatherAsync(string location)
    {
        // 調用天氣 API
        return $"Weather in {location}: Sunny, 25°C";
    }
}
```

---

### Week 6: US 6.1 - Chat 實時對話

#### Day 17-18: SignalR 集成
**任務**:
- 創建 SignalR Hub (ChatHub)
- 實現 WebSocket 連接
- 實現實時消息推送
- 創建 ChatWindow 組件 (Frontend)
- 消息持久化 (Messages 表)

**SignalR Hub**:
```csharp
public class ChatHub : Hub
{
    public async Task SendMessage(Guid agentId, string userInput)
    {
        // 執行 Agent
        var response = await _agentExecutionService.InvokeAsync(agentId, userInput);

        // 推送響應給客戶端
        await Clients.Caller.SendAsync("ReceiveMessage", response.Response);
    }
}
```

---

#### Day 19: Sprint 2 測試與修復

#### Day 20: Sprint Review + Retrospective

---

## ✅ 驗收標準 (DoD)

### Sprint 1 完成定義
```yaml
US 1.1 - Agent CRUD:
  - [ ] API 可創建、列表、詳情、更新、刪除 Agent
  - [ ] UI 可執行所有 CRUD 操作
  - [ ] 單元測試覆蓋率 ≥80%
  - [ ] API 響應時間 <200ms
  - [ ] Swagger 文檔完整

US 1.2 - .NET SDK:
  - [ ] NuGet 套件可安裝
  - [ ] SDK 可成功調用 API
  - [ ] 範例代碼可運行
  - [ ] XML 文檔註解完整
  - [ ] SDK 測試覆蓋率 ≥80%

US 1.3 - 配置管理:
  - [ ] Agent 詳情頁可查看所有配置
  - [ ] Agent 編輯頁可修改配置
  - [ ] 表單驗證正常工作
  - [ ] E2E 測試通過
```

### Sprint 2 完成定義
```yaml
US 1.4 - Agent 執行:
  - [ ] Agent 可接收輸入並返回響應
  - [ ] 執行歷史記錄完整
  - [ ] 錯誤處理健全
  - [ ] 執行時間 <5 秒 (一般請求)

US 2.1 - Plugin 系統:
  - [ ] Plugin 可註冊
  - [ ] Agent 可調用 Plugin
  - [ ] 至少 2 個 Plugin 範例
  - [ ] Plugin 隔離機制正常

US 6.1 - Chat 對話:
  - [ ] Chat UI 可發送消息
  - [ ] 實時接收 Agent 響應
  - [ ] 消息歷史正常顯示
  - [ ] WebSocket 連接穩定
```

---

## 📊 成功指標

### Sprint 1-2 KPI
```yaml
交付質量:
  - Story Points 完成率: 100% (26/26)
  - 測試覆蓋率: ≥80%
  - Bug 數量: <5 個 Critical/High

性能指標:
  - API 平均響應時間: <200ms
  - Agent 執行平均時間: <5s
  - UI 頁面加載時間: <2s

團隊效率:
  - Sprint Velocity: 13 points/sprint
  - Code Review 平均時間: <4 hours
  - Build 成功率: ≥95%
```

---

## ⚠️ 風險管理

### 識別的風險

| 風險 | 機率 | 影響 | 緩解措施 |
|------|------|------|---------|
| Semantic Kernel API 不穩定 | 中 | 高 | 提前驗證所有 SK API,準備降級方案 |
| SignalR 連接問題 | 中 | 中 | 提供 HTTP Polling 備選方案 |
| 測試覆蓋率不達標 | 低 | 中 | 每日檢查覆蓋率,及時補測試 |
| Frontend 性能問題 | 低 | 低 | 使用 React.memo 和虛擬滾動 |

---

## 📚 技術參考

### 核心技術棧
- Backend: .NET 8, ASP.NET Core, Entity Framework Core 8, Semantic Kernel 1.66
- Frontend: React 18, TypeScript 5.8, Vite, Material-UI, Zustand
- Database: PostgreSQL 16
- Real-time: SignalR
- Testing: xUnit, Vitest, Playwright

### 關鍵文檔
- [Coding Standards (C#)](../docs/technical-implementation/4-coding-standards/csharp-coding-standards.md)
- [Coding Standards (TypeScript)](../docs/technical-implementation/4-coding-standards/typescript-coding-standards.md)
- [API Guidelines](../docs/technical-implementation/5-api-design/restful-api-standards.md)
- [Testing Guide](../docs/technical-implementation/7-testing-strategy/unit-testing-guide.md)

---

**文檔版本**: 1.0.0
**最後更新**: 2025-11-03
**下一步**: [Sprint 3 規劃](./SPRINT-3-ROADMAP.md)
