# C# 12 編碼標準

## 版本信息
- **C# 版本**: 12.0
- **.NET 版本**: 8.0
- **最後更新**: 2025-01-01

## 命名約定

### PascalCase
- **類型名稱**: `public class UserService { }`
- **方法名稱**: `public void ProcessData() { }`
- **屬性**: `public string UserName { get; set; }`
- **事件**: `public event EventHandler DataLoaded;`
- **命名空間**: `namespace AIWorkflow.Core.Services`

### camelCase
- **參數**: `void SaveUser(string userName)`
- **局部變數**: `var userList = new List<User>();`
- **私有字段** (with `_` prefix): `private readonly ILogger _logger;`

### ALL_CAPS
- **常量**: `public const int MAX_RETRY_COUNT = 3;`

## 文件組織

### 標準文件結構
```csharp
// 1. Using 語句 (按字母排序)
using System;
using System.Collections.Generic;
using System.Linq;
using AIWorkflow.Core.Interfaces;

// 2. 命名空間
namespace AIWorkflow.Core.Services;

// 3. 類型定義
public class UserService : IUserService
{
    // 4. 常量
    private const int DefaultPageSize = 20;

    // 5. 私有字段
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    // 6. 構造函數
    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // 7. 公共屬性
    public int TotalUsers { get; private set; }

    // 8. 公共方法
    public async Task<User> GetUserAsync(int id)
    {
        _logger.LogInformation("Fetching user {UserId}", id);
        return await _repository.GetByIdAsync(id);
    }

    // 9. 私有方法
    private bool ValidateUser(User user)
    {
        return !string.IsNullOrWhiteSpace(user.Name);
    }
}
```

## C# 12 現代特性

### File-Scoped Namespaces
```csharp
// ✅ 推薦
namespace AIWorkflow.Core.Services;

public class UserService { }

// ❌ 避免 (舊式)
namespace AIWorkflow.Core.Services
{
    public class UserService { }
}
```

### Primary Constructors
```csharp
// ✅ 推薦 (C# 12)
public class UserService(IUserRepository repository, ILogger<UserService> logger)
{
    public async Task<User> GetUserAsync(int id)
    {
        logger.LogInformation("Fetching user {UserId}", id);
        return await repository.GetByIdAsync(id);
    }
}
```

### Collection Expressions
```csharp
// ✅ 推薦 (C# 12)
int[] numbers = [1, 2, 3, 4, 5];
List<string> names = ["Alice", "Bob", "Charlie"];

// Spread operator
int[] moreNumbers = [..numbers, 6, 7, 8];
```

### Required Members
```csharp
// ✅ 推薦
public class User
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
}

// 使用
var user = new User
{
    Name = "Alice",        // Required
    Email = "alice@example.com"  // Required
};
```

### Pattern Matching Enhancements
```csharp
// Switch expression with patterns
string GetUserRole(User user) => user switch
{
    { IsAdmin: true } => "Administrator",
    { IsPremium: true, SubscriptionExpiry: var exp } when exp > DateTime.Now => "Premium",
    { IsVerified: true } => "Verified User",
    _ => "Regular User"
};
```

## 異步編程

### Async/Await 最佳實踐
```csharp
// ✅ 推薦: 使用 ConfigureAwait(false) 在庫代碼中
public async Task<User> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id).ConfigureAwait(false);
    return user;
}

// ✅ 推薦: 並行執行
public async Task<(User user, Order[] orders)> GetUserDataAsync(int userId)
{
    var userTask = _userRepo.GetByIdAsync(userId);
    var ordersTask = _orderRepo.GetByUserIdAsync(userId);

    await Task.WhenAll(userTask, ordersTask);

    return (await userTask, await ordersTask);
}

// ❌ 避免: 阻塞調用
public User GetUser(int id)
{
    return _repository.GetByIdAsync(id).Result;  // 可能導致死鎖
}
```

## LINQ 最佳實踐

```csharp
// ✅ 推薦: 方法鏈
var activeUsers = users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Name)
    .Select(u => u.Email)
    .ToList();

// ✅ 推薦: Query syntax (複雜查詢)
var result = from user in users
             join order in orders on user.Id equals order.UserId
             where order.Total > 100
             select new { user.Name, order.Total };

// ❌ 避免: 多次枚舉
var query = users.Where(u => u.IsActive);
var count = query.Count();  // 第一次枚舉
var list = query.ToList();  // 第二次枚舉
// 解決方案: var list = query.ToList(); var count = list.Count;
```

## 錯誤處理

### 異常處理
```csharp
// ✅ 推薦: 具體異常類型
public async Task<User> GetUserAsync(int id)
{
    try
    {
        return await _repository.GetByIdAsync(id);
    }
    catch (EntityNotFoundException ex)
    {
        _logger.LogWarning(ex, "User {UserId} not found", id);
        throw new UserNotFoundException($"User with ID {id} not found", ex);
    }
    catch (DbException ex)
    {
        _logger.LogError(ex, "Database error while fetching user {UserId}", id);
        throw new DataAccessException("Failed to retrieve user", ex);
    }
}

// ❌ 避免: 吞噬異常
try
{
    ProcessData();
}
catch
{
    // 什麼都不做 - 非常糟糕!
}
```

## Null 處理

### Nullable Reference Types
```csharp
#nullable enable

public class UserService
{
    // 不可為 null
    private readonly ILogger<UserService> _logger;

    // 可為 null
    private string? _cachedData;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string? GetCachedData() => _cachedData;

    public void SetCachedData(string? data)
    {
        _cachedData = data;
    }
}
```

### Null-coalescing Operators
```csharp
// ✅ Null-coalescing operator
string name = user.Name ?? "Unknown";

// ✅ Null-conditional operator
int? length = user.Name?.Length;

// ✅ Null-coalescing assignment
_cache ??= new Dictionary<string, object>();

// ✅ Pattern matching
if (user is { Name: var name, Email: var email })
{
    // user is not null and has Name and Email
}
```

## 依賴注入

### 構造函數注入 (推薦)
```csharp
public class UserService(
    IUserRepository repository,
    ILogger<UserService> logger,
    IMapper mapper)
{
    public async Task<UserDto> GetUserAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
        return mapper.Map<UserDto>(user);
    }
}
```

### 服務註冊
```csharp
// Program.cs
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddTransient<IEmailService, EmailService>();
```

## 註釋規範

### XML 文檔註釋
```csharp
/// <summary>
/// 根據用戶 ID 獲取用戶信息
/// </summary>
/// <param name="id">用戶的唯一標識符</param>
/// <returns>包含用戶信息的 User 對象,如果未找到則返回 null</returns>
/// <exception cref="ArgumentException">當 id 小於或等於 0 時拋出</exception>
public async Task<User?> GetUserAsync(int id)
{
    if (id <= 0)
        throw new ArgumentException("User ID must be greater than 0", nameof(id));

    return await _repository.GetByIdAsync(id);
}
```

## 性能優化

### String Builder
```csharp
// ✅ 大量字符串連接使用 StringBuilder
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.AppendLine(item.ToString());
}
return sb.ToString();

// ❌ 避免在循環中使用 + 連接
string result = "";
foreach (var item in items)
{
    result += item.ToString();  // 創建新字符串對象
}
```

### Span<T> 和 Memory<T>
```csharp
// ✅ 推薦: 零分配字符串操作
public bool IsValidPrefix(string text, string prefix)
{
    ReadOnlySpan<char> textSpan = text.AsSpan();
    ReadOnlySpan<char> prefixSpan = prefix.AsSpan();
    return textSpan.StartsWith(prefixSpan);
}
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
