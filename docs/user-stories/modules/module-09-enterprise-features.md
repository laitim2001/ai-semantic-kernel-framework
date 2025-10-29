# 模組 09: 企業級基礎能力

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

---