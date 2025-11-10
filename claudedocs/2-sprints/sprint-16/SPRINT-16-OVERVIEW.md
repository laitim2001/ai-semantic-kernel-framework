# Sprint 16 概覽 - 企業級功能 (RBAC 與 Metering)

**Sprint**: Sprint 16 | **週次**: Week 46-48 | **日期**: 2026-09-21 ~ 2026-10-11 (21 days) | **狀態**: ⏳ 未開始

## 🎯 Sprint 目標
實現企業級**角色權限管理 (RBAC)** 和 **API Metering 與限流**,為企業生產環境做好準備。

## 📊 User Stories (16 SP)

| User Story | SP | 模組 | 優先級 |
|-----------|----|----- |-------|
| US 9.1 - RBAC 角色權限管理 | 8 | Module 9 | P0 |
| US 9.4 - API Metering 與限流 | 8 | Module 9 | P0 |

## 📋 核心技術任務

### US 9.1 - RBAC 權限管理

#### 數據模型
```csharp
Role:
  - Id, Name, Description
  - Permissions: Permission[]

Permission:
  - Id, Name, Resource, Action
  - 示例: "Agent.Create", "Agent.Execute"

UserRole:
  - UserId, RoleId
```

#### 預定義角色
```yaml
Admin:
  - 所有權限
  - 用戶管理, 角色管理

Developer:
  - Agent.Create, Agent.Update, Agent.Execute
  - Workflow.Create, Workflow.Execute
  - Plugin.Register

Analyst:
  - Agent.Execute (限定 Agent)
  - KnowledgeBase.Read
  - TextToSQL.Query

User:
  - Agent.Execute (限定 Agent)
  - Chat.Send
```

#### Microsoft Entra ID 集成
```yaml
配置:
  - Azure AD App Registration
  - OAuth 2.0 + OpenID Connect
  - JWT Token 驗證

權限同步:
  - Azure AD Groups → Roles
  - 自動角色分配
```

#### 權限中間件
```csharp
[Authorize(Policy = "Agent.Create")]
public async Task<IActionResult> CreateAgent(...)
```

### US 9.4 - API Metering 與限流

#### Redis Rate Limiting
```yaml
配置:
  - Redis 緩存
  - Sliding Window 算法

限流策略:
  User-level:
    - 100 requests/minute
    - 1000 requests/hour

  API-level:
    - /execute: 10 requests/minute
    - /chat: 50 requests/minute
```

#### 計量數據收集
```yaml
追蹤指標:
  - API 調用次數
  - Agent 執行次數
  - Token 使用量
  - 存儲用量

存儲:
  - TimeSeries Database (InfluxDB 或 PostgreSQL)
```

#### Usage Dashboard
```yaml
Frontend:
  - 使用統計儀表板
  - 實時用量顯示
  - 配額提醒

API:
  GET /api/v1/usage/summary:
    - 用戶使用統計
    - 配額剩餘
```

## 🎯 驗收標準

### 功能驗收
- ✅ RBAC 角色系統完整
- ✅ 權限驗證覆蓋所有 API
- ✅ Rate Limiting 正常運作
- ✅ 計量數據準確

### 安全驗收
- ✅ 無權限操作被阻止
- ✅ Azure AD 集成正常

### 性能驗收
- ✅ 權限驗證 overhead < 10ms
- ✅ Rate Limiting overhead < 5ms

## 🎓 關鍵風險

### 風險 1: Azure AD 集成複雜
**影響**: 中 🟡
**緩解**: Microsoft 官方文檔參考, 技術預研

### 風險 2: Redis 可用性
**影響**: 中 🟡
**緩解**: Redis 高可用配置, Fallback 機制

## 📦 技術依賴

### 新增 NuGet 套件
```yaml
Microsoft.Identity.Web: v2.x (Azure AD)
StackExchange.Redis: v2.x
AspNetCoreRateLimit: v5.x
```

### Azure 資源
```yaml
Azure AD:
  - App Registration

Azure Cache for Redis:
  - SKU: Basic C0 (開發) / Standard C1 (生產)
```

## 🔗 依賴
- **依賴**: 所有核心功能完成 (Sprint 1-15)
- **後續**: Sprint 17 (監控系統)

**狀態**: ⏳ 規劃中
