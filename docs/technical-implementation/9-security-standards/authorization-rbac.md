# 授權與 RBAC 實現指南 (Authorization & Role-Based Access Control)

## 概述

本文檔詳細說明 AI Workflow Platform 的授權系統實現,涵蓋 Role-Based Access Control (RBAC)、Claims-Based Authorization、Policy-Based Authorization、資源級授權和動態權限管理。

### 目標

- ✅ 實現靈活的 RBAC 權限模型
- ✅ 支持細粒度的資源級授權
- ✅ 實現 Claims-Based Authorization
- ✅ 實現 Policy-Based Authorization
- ✅ 支持動態權限檢查和繼承
- ✅ 提供 Workflow 和 Execution 權限控制
- ✅ 實現權限審計和日誌記錄

### 適用範圍

- Backend API (ASP.NET Core 9.0)
- React 19 Frontend
- Vue 3 Workflow Editor
- Admin Dashboard

---

## 技術背景

### 授權模型對比

| 模型 | 優點 | 缺點 | 適用場景 |
|------|------|------|----------|
| **RBAC** | 簡單、易管理 | 缺乏靈活性 | 企業系統、固定角色 |
| **ABAC** | 極度靈活 | 複雜、難管理 | 複雜業務規則 |
| **Claims-Based** | 靈活、可擴展 | 需要設計 | 現代 Web 應用 |
| **Policy-Based** | 集中管理、可測試 | 學習曲線 | ASP.NET Core 推薦 |

### 技術棧

```yaml
Backend:
  Framework: ASP.NET Core 9.0
  Authorization:
    - Microsoft.AspNetCore.Authorization
    - Microsoft.AspNetCore.Identity
  Database: PostgreSQL 16 (權限數據)
  Cache: Redis 7 (權限緩存)

Frontend:
  React: React 19 + Context API
  Vue: Vue 3.5 + Pinia
  Permission Check: Custom hooks/composables
```

---

## 權限模型設計

### 數據庫架構

#### Entity Relationship Diagram

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│    Users    │         │  UserRoles  │         │    Roles    │
├─────────────┤         ├─────────────┤         ├─────────────┤
│ Id (PK)     │         │ UserId (FK) │         │ Id (PK)     │
│ Email       │────────<│ RoleId (FK) │>────────│ Name        │
│ Name        │         │ AssignedAt  │         │ Description │
│ IsActive    │         │ AssignedBy  │         │ IsActive    │
└─────────────┘         └─────────────┘         └─────────────┘
                                                        │
                                                        │
                                                        │
┌─────────────┐         ┌─────────────┐         ┌─────┴───────┐
│ Permissions │         │RolePerms    │         │             │
├─────────────┤         ├─────────────┤         │             │
│ Id (PK)     │         │ RoleId (FK) │         │             │
│ Name        │────────<│ PermId (FK) │>────────┘             │
│ Resource    │         │ GrantedAt   │                       │
│ Action      │         └─────────────┘                       │
│ Description │                                               │
└─────────────┘         ┌─────────────┐                       │
                        │  UserPerms  │                       │
                        ├─────────────┤                       │
                        │ UserId (FK) │                       │
                        │ PermId (FK) │                       │
                        │ GrantedAt   │                       │
                        └─────────────┘
```

### Entity 定義

#### Role.cs

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 角色實體
/// </summary>
public sealed class Role : AuditableEntity
{
    public int Id { get; set; }

    /// <summary>
    /// 角色名稱 (例: Admin, User, WorkflowCreator)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 角色顯示名稱
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否為系統角色 (不可刪除)
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 優先級 (用於角色衝突解決)
    /// </summary>
    public int Priority { get; set; }

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

#### Permission.cs

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 權限實體
/// </summary>
public sealed class Permission : AuditableEntity
{
    public int Id { get; set; }

    /// <summary>
    /// 權限名稱 (例: workflow:create, workflow:execute)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 資源類型 (例: Workflow, WorkflowNode, Execution)
    /// </summary>
    public required string Resource { get; set; }

    /// <summary>
    /// 操作類型 (例: Create, Read, Update, Delete, Execute)
    /// </summary>
    public required string Action { get; set; }

    /// <summary>
    /// 權限顯示名稱
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// 權限描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 權限分組 (用於 UI 展示)
    /// </summary>
    public required string Group { get; set; }

    /// <summary>
    /// 是否為系統權限 (不可刪除)
    /// </summary>
    public bool IsSystemPermission { get; set; }

    // Navigation Properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
```

#### UserRole.cs

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 用戶角色關聯
/// </summary>
public sealed class UserRole : AuditableEntity
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }

    /// <summary>
    /// 分配者 ID
    /// </summary>
    public Guid AssignedBy { get; set; }

    /// <summary>
    /// 分配時間
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// 過期時間 (可選)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
```

#### RolePermission.cs

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 角色權限關聯
/// </summary>
public sealed class RolePermission : AuditableEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    /// <summary>
    /// 授予者 ID
    /// </summary>
    public Guid GrantedBy { get; set; }

    /// <summary>
    /// 授予時間
    /// </summary>
    public DateTime GrantedAt { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
```

#### UserPermission.cs

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 用戶直接權限關聯 (覆蓋角色權限)
/// </summary>
public sealed class UserPermission : AuditableEntity
{
    public Guid UserId { get; set; }
    public int PermissionId { get; set; }

    /// <summary>
    /// 是否為拒絕權限 (優先級最高)
    /// </summary>
    public bool IsDenied { get; set; }

    /// <summary>
    /// 授予者 ID
    /// </summary>
    public Guid GrantedBy { get; set; }

    /// <summary>
    /// 授予時間
    /// </summary>
    public DateTime GrantedAt { get; set; }

    /// <summary>
    /// 過期時間 (可選)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
```

---

## 權限命名規範

### 格式: `{resource}:{action}`

```csharp
// Workflow 權限
workflow:create          // 創建工作流
workflow:read            // 讀取工作流
workflow:update          // 更新工作流
workflow:delete          // 刪除工作流
workflow:publish         // 發布工作流
workflow:execute         // 執行工作流

// WorkflowNode 權限
workflow_node:create
workflow_node:update
workflow_node:delete

// Execution 權限
execution:read           // 查看執行記錄
execution:cancel         // 取消執行
execution:retry          // 重試執行

// User 權限
user:create
user:read
user:update
user:delete
user:impersonate         // 模擬用戶 (管理員)

// Role 權限
role:create
role:read
role:update
role:delete
role:assign              // 分配角色

// Permission 權限
permission:manage        // 管理權限
permission:assign        // 分配權限
```

---

## 權限檢查服務實現

### IPermissionService.cs

```csharp
namespace AIWorkflow.Application.Interfaces.Services;

public interface IPermissionService
{
    /// <summary>
    /// 檢查用戶是否擁有指定權限
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// 檢查用戶是否擁有任一權限
    /// </summary>
    Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// 檢查用戶是否擁有所有權限
    /// </summary>
    Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// 獲取用戶所有權限
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 檢查用戶是否為資源擁有者 (Resource-Level Authorization)
    /// </summary>
    Task<bool> IsResourceOwnerAsync(Guid userId, string resourceType, Guid resourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 檢查用戶是否可以訪問資源
    /// </summary>
    Task<bool> CanAccessResourceAsync(Guid userId, string resourceType, Guid resourceId, string action, CancellationToken cancellationToken = default);
}
```

### PermissionService.cs

```csharp
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace AIWorkflow.Infrastructure.Services;

public sealed class PermissionService(
    ApplicationDbContext dbContext,
    IConnectionMultiplexer redis,
    ILogger<PermissionService> logger) : IPermissionService
{
    private readonly IDatabase _redisDb = redis.GetDatabase();
    private const int CacheExpirationMinutes = 15;

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permission);
    }

    public async Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Any(p => userPermissions.Contains(p));
    }

    public async Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.All(p => userPermissions.Contains(p));
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 嘗試從 Redis 緩存讀取
        var cacheKey = $"permissions:user:{userId}";
        var cachedPermissions = await _redisDb.StringGetAsync(cacheKey);

        if (!cachedPermissions.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<List<string>>(cachedPermissions!)!;
        }

        // 從數據庫查詢
        var permissions = new HashSet<string>();

        // 1. 獲取用戶直接權限 (優先級最高)
        var userPermissions = await dbContext.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId && !up.Permission.IsDeleted)
            .Where(up => up.ExpiresAt == null || up.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        // 處理拒絕權限
        var deniedPermissions = userPermissions
            .Where(up => up.IsDenied)
            .Select(up => up.Permission.Name)
            .ToHashSet();

        // 添加授予的直接權限
        foreach (var up in userPermissions.Where(up => !up.IsDenied))
        {
            if (!deniedPermissions.Contains(up.Permission.Name))
            {
                permissions.Add(up.Permission.Name);
            }
        }

        // 2. 獲取角色權限
        var rolePermissions = await dbContext.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == userId && ur.Role.IsActive && !ur.Role.IsDeleted)
            .Where(ur => ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .ToListAsync(cancellationToken);

        // 添加角色權限 (排除拒絕權限)
        foreach (var permission in rolePermissions)
        {
            if (!deniedPermissions.Contains(permission))
            {
                permissions.Add(permission);
            }
        }

        var permissionList = permissions.ToList();

        // 緩存到 Redis (15 分鐘)
        await _redisDb.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(permissionList),
            TimeSpan.FromMinutes(CacheExpirationMinutes)
        );

        logger.LogInformation("Loaded {Count} permissions for user {UserId}", permissionList.Count, userId);

        return permissionList;
    }

    public async Task<bool> IsResourceOwnerAsync(Guid userId, string resourceType, Guid resourceId, CancellationToken cancellationToken = default)
    {
        return resourceType switch
        {
            "Workflow" => await dbContext.Workflows
                .AnyAsync(w => w.Id == resourceId && w.OwnerId == userId, cancellationToken),

            "WorkflowExecution" => await dbContext.WorkflowExecutions
                .AnyAsync(we => we.Id == resourceId && we.InitiatedBy == userId, cancellationToken),

            _ => false
        };
    }

    public async Task<bool> CanAccessResourceAsync(Guid userId, string resourceType, Guid resourceId, string action, CancellationToken cancellationToken = default)
    {
        var permission = $"{resourceType.ToLower()}:{action}";

        // 1. 檢查是否為資源擁有者 (擁有者總是有權限)
        var isOwner = await IsResourceOwnerAsync(userId, resourceType, resourceId, cancellationToken);
        if (isOwner)
        {
            logger.LogInformation("User {UserId} is owner of {ResourceType}:{ResourceId}", userId, resourceType, resourceId);
            return true;
        }

        // 2. 檢查全局權限
        var hasPermission = await HasPermissionAsync(userId, permission, cancellationToken);
        if (hasPermission)
        {
            logger.LogInformation("User {UserId} has permission {Permission}", userId, permission);
            return true;
        }

        // 3. 檢查資源級共享權限 (可選實現)
        // TODO: 實現 WorkflowSharing 表來管理工作流共享

        logger.LogWarning("User {UserId} denied access to {ResourceType}:{ResourceId} for action {Action}", userId, resourceType, resourceId, action);
        return false;
    }

    /// <summary>
    /// 清除用戶權限緩存
    /// </summary>
    public async Task InvalidateUserPermissionsCacheAsync(Guid userId)
    {
        var cacheKey = $"permissions:user:{userId}";
        await _redisDb.KeyDeleteAsync(cacheKey);
        logger.LogInformation("Invalidated permission cache for user {UserId}", userId);
    }
}
```

---

## ASP.NET Core Authorization 配置

### Policy-Based Authorization

#### Program.cs

```csharp
builder.Services.AddAuthorization(options =>
{
    // 基於角色的策略
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User", "WorkflowCreator", "Admin"));

    // 基於權限的策略
    options.AddPolicy("CanCreateWorkflow", policy =>
        policy.RequireClaim("permission", "workflow:create"));

    options.AddPolicy("CanExecuteWorkflow", policy =>
        policy.RequireClaim("permission", "workflow:execute"));

    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim("permission", "user:create", "user:update", "user:delete"));

    // 自定義 Authorization Handler
    options.AddPolicy("CanAccessWorkflow", policy =>
        policy.Requirements.Add(new ResourceAccessRequirement("Workflow")));

    options.AddPolicy("CanAccessExecution", policy =>
        policy.Requirements.Add(new ResourceAccessRequirement("WorkflowExecution")));
});

// 註冊自定義 Authorization Handler
builder.Services.AddScoped<IAuthorizationHandler, ResourceAccessHandler>();
```

### Custom Authorization Handler

#### ResourceAccessRequirement.cs

```csharp
using Microsoft.AspNetCore.Authorization;

namespace AIWorkflow.API.Authorization;

/// <summary>
/// 資源訪問授權需求
/// </summary>
public sealed class ResourceAccessRequirement(string resourceType) : IAuthorizationRequirement
{
    public string ResourceType { get; } = resourceType;
}
```

#### ResourceAccessHandler.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AIWorkflow.API.Authorization;

/// <summary>
/// 資源訪問授權處理器
/// </summary>
public sealed class ResourceAccessHandler(
    IPermissionService permissionService,
    ILogger<ResourceAccessHandler> logger) : AuthorizationHandler<ResourceAccessRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAccessRequirement requirement)
    {
        // 獲取用戶 ID
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            logger.LogWarning("User ID not found in claims");
            context.Fail();
            return;
        }

        // 從 HttpContext 獲取資源 ID (需要通過 Route 或 Query 傳遞)
        if (context.Resource is HttpContext httpContext)
        {
            var resourceIdStr = httpContext.Request.RouteValues["id"]?.ToString()
                ?? httpContext.Request.Query["id"].FirstOrDefault();

            if (string.IsNullOrEmpty(resourceIdStr) || !Guid.TryParse(resourceIdStr, out var resourceId))
            {
                logger.LogWarning("Resource ID not found or invalid");
                context.Fail();
                return;
            }

            // 獲取 HTTP 方法來確定操作類型
            var action = httpContext.Request.Method switch
            {
                "GET" => "read",
                "POST" => "create",
                "PUT" => "update",
                "PATCH" => "update",
                "DELETE" => "delete",
                _ => "read"
            };

            // 檢查訪問權限
            var canAccess = await permissionService.CanAccessResourceAsync(
                userId,
                requirement.ResourceType,
                resourceId,
                action);

            if (canAccess)
            {
                logger.LogInformation("User {UserId} granted access to {ResourceType}:{ResourceId}", userId, requirement.ResourceType, resourceId);
                context.Succeed(requirement);
            }
            else
            {
                logger.LogWarning("User {UserId} denied access to {ResourceType}:{ResourceId}", userId, requirement.ResourceType, resourceId);
                context.Fail();
            }
        }
        else
        {
            context.Fail();
        }
    }
}
```

---

## Authorization Attributes 使用

### Controller 級別授權

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 所有端點都需要認證
public sealed class WorkflowsController(
    IWorkflowService workflowService,
    IPermissionService permissionService,
    ILogger<WorkflowsController> logger) : ControllerBase
{
    /// <summary>
    /// 獲取所有工作流 (需要 workflow:read 權限)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "CanReadWorkflow")]
    public async Task<ActionResult<List<WorkflowDto>>> GetWorkflows(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var workflows = await workflowService.GetUserWorkflowsAsync(userId, cancellationToken);
        return Ok(workflows);
    }

    /// <summary>
    /// 創建工作流 (需要 workflow:create 權限)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanCreateWorkflow")]
    public async Task<ActionResult<WorkflowDto>> CreateWorkflow(
        [FromBody] CreateWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var workflow = await workflowService.CreateWorkflowAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
    }

    /// <summary>
    /// 獲取單個工作流 (需要 workflow:read 權限 + 資源訪問權限)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "CanAccessWorkflow")]
    public async Task<ActionResult<WorkflowDto>> GetWorkflow(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        // 額外的權限檢查 (已在 Policy 中處理,這裡是雙重驗證)
        var canAccess = await permissionService.CanAccessResourceAsync(userId, "Workflow", id, "read", cancellationToken);
        if (!canAccess)
        {
            return Forbid();
        }

        var workflow = await workflowService.GetWorkflowByIdAsync(id, cancellationToken);
        if (workflow == null)
        {
            return NotFound();
        }

        return Ok(workflow);
    }

    /// <summary>
    /// 更新工作流 (需要 workflow:update 權限 + 擁有者或共享權限)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "CanAccessWorkflow")]
    public async Task<ActionResult<WorkflowDto>> UpdateWorkflow(
        Guid id,
        [FromBody] UpdateWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var canAccess = await permissionService.CanAccessResourceAsync(userId, "Workflow", id, "update", cancellationToken);
        if (!canAccess)
        {
            return Forbid();
        }

        var workflow = await workflowService.UpdateWorkflowAsync(id, request, cancellationToken);
        return Ok(workflow);
    }

    /// <summary>
    /// 刪除工作流 (需要 workflow:delete 權限 + 擁有者權限)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "CanAccessWorkflow")]
    public async Task<IActionResult> DeleteWorkflow(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        // 刪除操作只允許擁有者
        var isOwner = await permissionService.IsResourceOwnerAsync(userId, "Workflow", id, cancellationToken);
        if (!isOwner)
        {
            return Forbid();
        }

        await workflowService.DeleteWorkflowAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// 執行工作流 (需要 workflow:execute 權限)
    /// </summary>
    [HttpPost("{id}/execute")]
    [Authorize(Policy = "CanExecuteWorkflow")]
    public async Task<ActionResult<WorkflowExecutionDto>> ExecuteWorkflow(
        Guid id,
        [FromBody] ExecuteWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var canAccess = await permissionService.CanAccessResourceAsync(userId, "Workflow", id, "execute", cancellationToken);
        if (!canAccess)
        {
            return Forbid();
        }

        var execution = await workflowService.ExecuteWorkflowAsync(id, userId, request, cancellationToken);
        return Ok(execution);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
```

### Admin Controller (需要 Admin 角色)

```csharp
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")] // 整個 Controller 需要 Admin 角色
public sealed class UsersController(
    IUserService userService,
    IPermissionService permissionService,
    ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("{userId}/roles")]
    [Authorize(Policy = "CanManageRoles")]
    public async Task<IActionResult> AssignRole(
        Guid userId,
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        await userService.AssignRoleAsync(userId, request.RoleId, cancellationToken);
        return Ok();
    }

    [HttpPost("{userId}/permissions")]
    [Authorize(Policy = "CanManagePermissions")]
    public async Task<IActionResult> GrantPermission(
        Guid userId,
        [FromBody] GrantPermissionRequest request,
        CancellationToken cancellationToken)
    {
        await userService.GrantPermissionAsync(userId, request.PermissionId, cancellationToken);

        // 清除權限緩存
        await permissionService.InvalidateUserPermissionsCacheAsync(userId);

        return Ok();
    }
}
```

---

## Frontend 權限檢查

### React 19 Permission Hook

#### usePermission.ts

```typescript
import { create } from 'zustand';
import { useAuth } from './useAuth';

interface PermissionState {
  permissions: string[];
  loadPermissions: () => Promise<void>;
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  canAccessResource: (resourceType: string, resourceId: string, action: string) => Promise<boolean>;
}

export const usePermission = create<PermissionState>((set, get) => ({
  permissions: [],

  loadPermissions: async () => {
    try {
      const response = await axios.get('/api/permissions/my-permissions');
      set({ permissions: response.data });
    } catch (error) {
      console.error('Failed to load permissions:', error);
    }
  },

  hasPermission: (permission: string) => {
    return get().permissions.includes(permission);
  },

  hasAnyPermission: (permissions: string[]) => {
    const userPermissions = get().permissions;
    return permissions.some(p => userPermissions.includes(p));
  },

  hasAllPermissions: (permissions: string[]) => {
    const userPermissions = get().permissions;
    return permissions.every(p => userPermissions.includes(p));
  },

  canAccessResource: async (resourceType: string, resourceId: string, action: string) => {
    try {
      const response = await axios.get('/api/permissions/can-access', {
        params: { resourceType, resourceId, action }
      });
      return response.data.canAccess;
    } catch (error) {
      console.error('Failed to check resource access:', error);
      return false;
    }
  },
}));

// Permission Guard Component
interface PermissionGuardProps {
  permission: string;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

export const PermissionGuard: React.FC<PermissionGuardProps> = ({
  permission,
  children,
  fallback = null
}) => {
  const { hasPermission } = usePermission();

  if (!hasPermission(permission)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};

// Multiple Permission Guard
interface MultiPermissionGuardProps {
  permissions: string[];
  requireAll?: boolean;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

export const MultiPermissionGuard: React.FC<MultiPermissionGuardProps> = ({
  permissions,
  requireAll = false,
  children,
  fallback = null
}) => {
  const { hasAnyPermission, hasAllPermissions } = usePermission();

  const hasAccess = requireAll
    ? hasAllPermissions(permissions)
    : hasAnyPermission(permissions);

  if (!hasAccess) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};
```

#### 使用範例

```typescript
import { PermissionGuard, MultiPermissionGuard } from './hooks/usePermission';

export const WorkflowList: React.FC = () => {
  return (
    <div>
      <h1>工作流列表</h1>

      {/* 只有擁有 workflow:create 權限的用戶才能看到創建按鈕 */}
      <PermissionGuard permission="workflow:create">
        <button onClick={handleCreate}>創建工作流</button>
      </PermissionGuard>

      {/* 擁有任一權限即可看到 */}
      <MultiPermissionGuard permissions={["workflow:update", "workflow:delete"]}>
        <button onClick={handleEdit}>編輯</button>
      </MultiPermissionGuard>

      {/* 需要所有權限才能看到 */}
      <MultiPermissionGuard permissions={["workflow:delete", "workflow:publish"]} requireAll>
        <button onClick={handlePublish}>發布並刪除草稿</button>
      </MultiPermissionGuard>
    </div>
  );
};
```

### Vue 3 Permission Composable

#### usePermission.ts

```typescript
import { defineStore } from 'pinia';
import axios from 'axios';

interface PermissionState {
  permissions: string[];
}

export const usePermissionStore = defineStore('permission', {
  state: (): PermissionState => ({
    permissions: [],
  }),

  getters: {
    hasPermission: (state) => (permission: string) => {
      return state.permissions.includes(permission);
    },

    hasAnyPermission: (state) => (permissions: string[]) => {
      return permissions.some(p => state.permissions.includes(p));
    },

    hasAllPermissions: (state) => (permissions: string[]) => {
      return permissions.every(p => state.permissions.includes(p));
    },
  },

  actions: {
    async loadPermissions() {
      try {
        const response = await axios.get('/api/permissions/my-permissions');
        this.permissions = response.data;
      } catch (error) {
        console.error('Failed to load permissions:', error);
      }
    },

    async canAccessResource(resourceType: string, resourceId: string, action: string): Promise<boolean> {
      try {
        const response = await axios.get('/api/permissions/can-access', {
          params: { resourceType, resourceId, action }
        });
        return response.data.canAccess;
      } catch (error) {
        console.error('Failed to check resource access:', error);
        return false;
      }
    },
  },
});
```

#### Permission Directive

```typescript
// directives/permission.ts
import { Directive } from 'vue';
import { usePermissionStore } from '@/stores/permission';

export const vPermission: Directive = {
  mounted(el, binding) {
    const permissionStore = usePermissionStore();
    const { value } = binding;

    if (Array.isArray(value)) {
      const hasPermission = permissionStore.hasAnyPermission(value);
      if (!hasPermission) {
        el.parentNode?.removeChild(el);
      }
    } else {
      const hasPermission = permissionStore.hasPermission(value);
      if (!hasPermission) {
        el.parentNode?.removeChild(el);
      }
    }
  },
};
```

#### 使用範例

```vue
<template>
  <div>
    <h1>工作流列表</h1>

    <!-- 使用 v-permission 指令 -->
    <button v-permission="'workflow:create'" @click="handleCreate">
      創建工作流
    </button>

    <!-- 使用 hasPermission 方法 -->
    <button v-if="hasPermission('workflow:update')" @click="handleEdit">
      編輯工作流
    </button>

    <!-- 多個權限 -->
    <button v-permission="['workflow:delete', 'workflow:publish']" @click="handlePublish">
      發布
    </button>
  </div>
</template>

<script setup lang="ts">
import { usePermissionStore } from '@/stores/permission';

const permissionStore = usePermissionStore();
const { hasPermission, hasAnyPermission } = permissionStore;

const handleCreate = () => {
  // 創建工作流邏輯
};
</script>
```

---

## 權限管理 API

### PermissionsController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PermissionsController(
    IPermissionService permissionService,
    ILogger<PermissionsController> logger) : ControllerBase
{
    /// <summary>
    /// 獲取當前用戶的所有權限
    /// </summary>
    [HttpGet("my-permissions")]
    public async Task<ActionResult<List<string>>> GetMyPermissions(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var permissions = await permissionService.GetUserPermissionsAsync(userId, cancellationToken);
        return Ok(permissions);
    }

    /// <summary>
    /// 檢查當前用戶是否可以訪問資源
    /// </summary>
    [HttpGet("can-access")]
    public async Task<ActionResult<CanAccessResponse>> CanAccess(
        [FromQuery] string resourceType,
        [FromQuery] Guid resourceId,
        [FromQuery] string action,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var canAccess = await permissionService.CanAccessResourceAsync(userId, resourceType, resourceId, action, cancellationToken);

        return Ok(new CanAccessResponse { CanAccess = canAccess });
    }

    /// <summary>
    /// 獲取所有可用權限 (管理員)
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions(CancellationToken cancellationToken)
    {
        // TODO: 實現從數據庫獲取所有權限
        return Ok();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}

public record CanAccessResponse
{
    public required bool CanAccess { get; init; }
}
```

---

## 數據庫遷移和種子數據

### Initial Permission Seed

```csharp
public static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync())
        {
            return; // Already seeded
        }

        var permissions = new[]
        {
            // Workflow Permissions
            new Permission { Name = "workflow:create", Resource = "Workflow", Action = "Create", DisplayName = "創建工作流", Group = "Workflow", IsSystemPermission = true },
            new Permission { Name = "workflow:read", Resource = "Workflow", Action = "Read", DisplayName = "讀取工作流", Group = "Workflow", IsSystemPermission = true },
            new Permission { Name = "workflow:update", Resource = "Workflow", Action = "Update", DisplayName = "更新工作流", Group = "Workflow", IsSystemPermission = true },
            new Permission { Name = "workflow:delete", Resource = "Workflow", Action = "Delete", DisplayName = "刪除工作流", Group = "Workflow", IsSystemPermission = true },
            new Permission { Name = "workflow:publish", Resource = "Workflow", Action = "Publish", DisplayName = "發布工作流", Group = "Workflow", IsSystemPermission = true },
            new Permission { Name = "workflow:execute", Resource = "Workflow", Action = "Execute", DisplayName = "執行工作流", Group = "Workflow", IsSystemPermission = true },

            // Execution Permissions
            new Permission { Name = "execution:read", Resource = "Execution", Action = "Read", DisplayName = "查看執行記錄", Group = "Execution", IsSystemPermission = true },
            new Permission { Name = "execution:cancel", Resource = "Execution", Action = "Cancel", DisplayName = "取消執行", Group = "Execution", IsSystemPermission = true },
            new Permission { Name = "execution:retry", Resource = "Execution", Action = "Retry", DisplayName = "重試執行", Group = "Execution", IsSystemPermission = true },

            // User Permissions
            new Permission { Name = "user:create", Resource = "User", Action = "Create", DisplayName = "創建用戶", Group = "User", IsSystemPermission = true },
            new Permission { Name = "user:read", Resource = "User", Action = "Read", DisplayName = "讀取用戶", Group = "User", IsSystemPermission = true },
            new Permission { Name = "user:update", Resource = "User", Action = "Update", DisplayName = "更新用戶", Group = "User", IsSystemPermission = true },
            new Permission { Name = "user:delete", Resource = "User", Action = "Delete", DisplayName = "刪除用戶", Group = "User", IsSystemPermission = true },

            // Role Permissions
            new Permission { Name = "role:create", Resource = "Role", Action = "Create", DisplayName = "創建角色", Group = "Role", IsSystemPermission = true },
            new Permission { Name = "role:read", Resource = "Role", Action = "Read", DisplayName = "讀取角色", Group = "Role", IsSystemPermission = true },
            new Permission { Name = "role:update", Resource = "Role", Action = "Update", DisplayName = "更新角色", Group = "Role", IsSystemPermission = true },
            new Permission { Name = "role:delete", Resource = "Role", Action = "Delete", DisplayName = "刪除角色", Group = "Role", IsSystemPermission = true },
            new Permission { Name = "role:assign", Resource = "Role", Action = "Assign", DisplayName = "分配角色", Group = "Role", IsSystemPermission = true },
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var roles = new[]
        {
            new Role { Name = "Admin", DisplayName = "管理員", Description = "系統管理員,擁有所有權限", IsSystemRole = true, Priority = 100 },
            new Role { Name = "WorkflowCreator", DisplayName = "工作流創建者", Description = "可以創建和管理工作流", IsSystemRole = true, Priority = 50 },
            new Role { Name = "User", DisplayName = "普通用戶", Description = "基本用戶權限", IsSystemRole = true, Priority = 10 },
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // 分配權限給角色
        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        var workflowCreatorRole = await context.Roles.FirstAsync(r => r.Name == "WorkflowCreator");
        var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

        var allPermissions = await context.Permissions.ToListAsync();

        // Admin 擁有所有權限
        foreach (var permission in allPermissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id,
                GrantedBy = Guid.Empty, // System
                GrantedAt = DateTime.UtcNow
            });
        }

        // WorkflowCreator 權限
        var workflowCreatorPermissions = allPermissions
            .Where(p => p.Group is "Workflow" or "Execution")
            .ToList();

        foreach (var permission in workflowCreatorPermissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = workflowCreatorRole.Id,
                PermissionId = permission.Id,
                GrantedBy = Guid.Empty,
                GrantedAt = DateTime.UtcNow
            });
        }

        // User 基本權限
        var userPermissions = allPermissions
            .Where(p => p.Name is "workflow:read" or "execution:read")
            .ToList();

        foreach (var permission in userPermissions)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = userRole.Id,
                PermissionId = permission.Id,
                GrantedBy = Guid.Empty,
                GrantedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}
```

---

## 最佳實踐

### ✅ Do's

1. **最小權限原則**: 只授予必要的權限
2. **角色繼承**: 使用 Priority 實現角色優先級
3. **權限緩存**: 使用 Redis 緩存用戶權限 (15 分鐘)
4. **資源級授權**: 檢查用戶是否為資源擁有者
5. **拒絕權限**: 支持明確拒絕權限 (優先級最高)
6. **權限過期**: 支持臨時權限授予
7. **審計日誌**: 記錄所有權限變更操作

### ❌ Don'ts

1. **不要硬編碼權限**: 使用數據庫管理權限
2. **不要忽略緩存失效**: 權限變更後立即清除緩存
3. **不要過度授權**: 避免給用戶過多不需要的權限
4. **不要忽略資源訪問控制**: 除了權限檢查,還要檢查資源擁有權
5. **不要在 Frontend 完全依賴權限檢查**: Backend 必須再次驗證

---

## 測試策略

### Unit Tests

```csharp
[Fact]
public async Task HasPermission_UserWithDirectPermission_ReturnsTrue()
{
    // Arrange
    var userId = Guid.NewGuid();
    var permission = "workflow:create";

    // Setup mock data
    // ...

    // Act
    var hasPermission = await _permissionService.HasPermissionAsync(userId, permission);

    // Assert
    Assert.True(hasPermission);
}

[Fact]
public async Task HasPermission_UserWithDeniedPermission_ReturnsFalse()
{
    // Test denied permission logic
}

[Fact]
public async Task CanAccessResource_ResourceOwner_ReturnsTrue()
{
    // Test resource owner access
}
```

---

## 故障排查

### 常見問題

#### 1. 權限檢查總是返回 false

**原因**: 權限緩存未更新或權限名稱不匹配

**解決方案**:
```csharp
// 清除緩存
await permissionService.InvalidateUserPermissionsCacheAsync(userId);

// 檢查權限名稱是否正確
var permissions = await permissionService.GetUserPermissionsAsync(userId);
```

#### 2. 資源訪問被拒絕

**原因**: 未正確傳遞資源 ID 或用戶不是資源擁有者

**解決方案**:
```csharp
// 檢查 Route Values
var resourceId = HttpContext.Request.RouteValues["id"];

// 檢查擁有權
var isOwner = await permissionService.IsResourceOwnerAsync(userId, "Workflow", resourceId);
```

---

## 實施檢查清單

- [ ] 數據庫表和 Entity 完成
- [ ] PermissionService 實現
- [ ] Authorization Policies 配置
- [ ] Custom Authorization Handler 實現
- [ ] Controller Authorization Attributes
- [ ] Frontend Permission Hooks/Composables
- [ ] Permission Seed Data
- [ ] Redis 緩存配置
- [ ] 單元測試覆蓋 >80%
- [ ] 集成測試完成

---

## 相關文檔

- [Phase 9: 認證實現](./authentication-implementation.md)
- [Phase 9: 數據加密標準](./data-encryption-standards.md)
- [Phase 6: 數據庫標準](../6-database-standards/README.md)
- [Phase 7: 測試策略](../7-testing-strategy/README.md)

## 參考資源

### 官方文檔
- [ASP.NET Core Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/)
- [Policy-Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies)
- [Claims-Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims)

### 最佳實踐
- [OWASP Authorization Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authorization_Cheat_Sheet.html)
- [RBAC vs ABAC](https://www.okta.com/identity-101/role-based-access-control-vs-attribute-based-access-control/)

---

**文檔維護**: Security Team
**最後更新**: 2025-11-02
**狀態**: ✅ 完整
