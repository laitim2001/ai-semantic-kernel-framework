# API 安全性設計 (API Security Design)

## 版本信息
- **標準版本**: v1.0.0
- **適用框架**: ASP.NET Core 8.0
- **安全標準**: OWASP API Security Top 10
- **最後更新**: 2025-01-01

## 目錄

1. [安全原則](#安全原則)
2. [認證機制 (JWT)](#認證機制-jwt)
3. [授權策略](#授權策略)
4. [Rate Limiting](#rate-limiting)
5. [CORS 配置](#cors-配置)
6. [輸入驗證與 Sanitization](#輸入驗證與-sanitization)
7. [SQL Injection 防護](#sql-injection-防護)
8. [XSS 防護](#xss-防護)
9. [CSRF 防護](#csrf-防護)
10. [敏感數據處理](#敏感數據處理)

---

## 安全原則

### OWASP API Security Top 10 (2023)

| 風險 | 描述 | 緩解措施 |
|------|------|----------|
| **API1:2023 Broken Object Level Authorization** | 對象級授權失敗 | 實現嚴格的授權檢查 |
| **API2:2023 Broken Authentication** | 認證機制失敗 | 使用 JWT + refresh token 機制 |
| **API3:2023 Broken Object Property Level Authorization** | 對象屬性級授權失敗 | 數據 DTO 過濾敏感欄位 |
| **API4:2023 Unrestricted Resource Consumption** | 無限制資源消耗 | 實現 Rate Limiting |
| **API5:2023 Broken Function Level Authorization** | 功能級授權失敗 | 基於角色的訪問控制 (RBAC) |
| **API6:2023 Unrestricted Access to Sensitive Business Flows** | 敏感業務流程無限制訪問 | 業務邏輯驗證 |
| **API7:2023 Server Side Request Forgery** | SSRF 攻擊 | 限制外部請求 URL |
| **API8:2023 Security Misconfiguration** | 安全配置錯誤 | 安全配置審計 |
| **API9:2023 Improper Inventory Management** | 不當庫存管理 | API 版本管理和棄用策略 |
| **API10:2023 Unsafe Consumption of APIs** | 不安全的 API 消費 | 驗證外部 API 響應 |

---

## 認證機制 (JWT)

### JWT Token 結構

```
header.payload.signature
```

**Header**:
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload**:
```json
{
  "sub": "42",
  "email": "user@example.com",
  "role": "user",
  "iat": 1704067200,
  "exp": 1704153600
}
```

### ASP.NET Core JWT 配置

#### 安裝包

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

#### appsettings.json 配置

```json
{
  "Jwt": {
    "Secret": "Your-256-bit-secret-key-here-change-this-in-production",
    "Issuer": "https://api.aiworkflow.com",
    "Audience": "https://aiworkflow.com",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

#### Program.cs 配置

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)
        ),
        ClockSkew = TimeSpan.Zero  // 移除默認的 5 分鐘容差
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

#### JWT 服務實現

```csharp
public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)
            ),
            ValidateLifetime = false  // 不驗證過期時間
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
```

#### 登入端點

```csharp
app.MapPost("/api/v1/auth/login", async (
    LoginDto dto,
    IMediator mediator,
    IJwtTokenService jwtTokenService) =>
{
    var command = new LoginCommand(dto.Email, dto.Password);
    var result = await mediator.Send(command);

    if (!result.IsSuccess)
        return Results.Unauthorized();

    var user = result.Value;
    var accessToken = jwtTokenService.GenerateAccessToken(user);
    var refreshToken = jwtTokenService.GenerateRefreshToken();

    // 保存 refresh token 到數據庫
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

    return Results.Ok(new
    {
        accessToken,
        refreshToken,
        expiresIn = 3600  // 秒
    });
})
.WithTags("Authentication");
```

#### Token 刷新端點

```csharp
app.MapPost("/api/v1/auth/refresh", async (
    RefreshTokenDto dto,
    IMediator mediator,
    IJwtTokenService jwtTokenService) =>
{
    var principal = jwtTokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

    if (principal is null)
        return Results.BadRequest("Invalid access token");

    var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var query = new GetUserByIdQuery(userId);
    var result = await mediator.Send(query);

    if (!result.IsSuccess)
        return Results.Unauthorized();

    var user = result.Value;

    if (user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        return Results.Unauthorized();

    var newAccessToken = jwtTokenService.GenerateAccessToken(user);
    var newRefreshToken = jwtTokenService.GenerateRefreshToken();

    user.RefreshToken = newRefreshToken;
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

    return Results.Ok(new
    {
        accessToken = newAccessToken,
        refreshToken = newRefreshToken
    });
})
.WithTags("Authentication");
```

---

## 授權策略

### 基於角色的訪問控制 (RBAC)

```csharp
// 定義角色
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Guest = "Guest";
}

// 配置授權策略
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole(Roles.Admin));

    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole(Roles.User, Roles.Admin));

    options.AddPolicy("WorkflowOwner", policy =>
        policy.Requirements.Add(new WorkflowOwnerRequirement()));
});
```

### 自定義授權處理器

```csharp
public sealed class WorkflowOwnerRequirement : IAuthorizationRequirement { }

public sealed class WorkflowOwnerAuthorizationHandler(IApplicationDbContext context)
    : AuthorizationHandler<WorkflowOwnerRequirement, Workflow>
{
    private readonly IApplicationDbContext _context = context;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        WorkflowOwnerRequirement requirement,
        Workflow workflow)
    {
        var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (workflow.OwnerId == userId || context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
        }

        await Task.CompletedTask;
    }
}

// 註冊處理器
builder.Services.AddScoped<IAuthorizationHandler, WorkflowOwnerAuthorizationHandler>();
```

### 應用授權策略

```csharp
app.MapDelete("/api/v1/workflows/{id:int}", async (
    int id,
    IMediator mediator,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var query = new GetWorkflowByIdQuery(id);
    var result = await mediator.Send(query);

    if (!result.IsSuccess)
        return Results.NotFound();

    var workflow = result.Value;

    // 授權檢查
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User,
        workflow,
        "WorkflowOwner"
    );

    if (!authResult.Succeeded)
        return Results.Forbid();

    var command = new DeleteWorkflowCommand(id);
    await mediator.Send(command);

    return Results.NoContent();
})
.RequireAuthorization();
```

---

## Rate Limiting

### 安裝包

```xml
<PackageReference Include="AspNetCoreRateLimit" Version="5.0.0" />
```

### 配置

```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "*",
        "Period": "1h",
        "Limit": 1000
      }
    ]
  }
}
```

### Program.cs 配置

```csharp
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

app.UseIpRateLimiting();

app.Run();
```

---

## CORS 配置

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins(
                "https://aiworkflow.com",
                "https://app.aiworkflow.com",
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("X-Pagination", "X-RateLimit-Remaining");
    });
});

var app = builder.Build();

app.UseCors("AllowedOrigins");
```

---

## 輸入驗證與 Sanitization

### FluentValidation

```csharp
public sealed class CreateWorkflowDtoValidator : AbstractValidator<CreateWorkflowDto>
{
    public CreateWorkflowDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Matches("^[a-zA-Z0-9 _-]+$")
            .WithMessage("Name contains invalid characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
```

### HTML Sanitization

```csharp
using Ganss.Xss;

public sealed class HtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer = new();

    public string Sanitize(string html)
    {
        return _sanitizer.Sanitize(html);
    }
}
```

---

## SQL Injection 防護

### 使用參數化查詢 (Entity Framework Core)

```csharp
// ✅ 安全 - 參數化查詢
var workflows = await context.Workflows
    .Where(w => w.Name == name)
    .ToListAsync();

// ❌ 危險 - 字符串拼接
var workflows = await context.Workflows
    .FromSqlRaw($"SELECT * FROM Workflows WHERE Name = '{name}'")
    .ToListAsync();

// ✅ 安全 - 參數化原始 SQL
var workflows = await context.Workflows
    .FromSqlRaw("SELECT * FROM Workflows WHERE Name = {0}", name)
    .ToListAsync();
```

---

## XSS 防護

### Content Security Policy (CSP)

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' data:; " +
        "connect-src 'self' https://api.aiworkflow.com; " +
        "frame-ancestors 'none';");

    await next();
});
```

---

## CSRF 防護

對於 API,通常使用 JWT Token 認證,CSRF 風險較低。但對於傳統 Cookie-based 認證,需要啟用 CSRF 保護:

```csharp
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

app.UseAntiforgery();
```

---

## 敏感數據處理

### 密碼哈希

```csharp
using BCrypt.Net;

public sealed class PasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### 數據加密

```csharp
using System.Security.Cryptography;

public sealed class DataProtectionService(IDataProtectionProvider dataProtectionProvider)
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("AIWorkflow.DataProtection");

    public string Protect(string plainText)
    {
        return _protector.Protect(plainText);
    }

    public string Unprotect(string cipherText)
    {
        return _protector.Unprotect(cipherText);
    }
}
```

---

## 最佳實踐檢查清單

### 認證階段
- [ ] 實現 JWT 認證
- [ ] 配置 Token 過期時間
- [ ] 實現 Refresh Token 機制
- [ ] Token 安全存儲

### 授權階段
- [ ] 實現基於角色的訪問控制 (RBAC)
- [ ] 實現資源級授權檢查
- [ ] 自定義授權策略

### API 保護
- [ ] 實現 Rate Limiting
- [ ] 配置 CORS
- [ ] 啟用 HTTPS
- [ ] 添加安全響應頭

### 輸入安全
- [ ] 輸入驗證 (FluentValidation)
- [ ] HTML Sanitization
- [ ] SQL Injection 防護
- [ ] XSS 防護

### 數據安全
- [ ] 密碼哈希 (BCrypt)
- [ ] 敏感數據加密
- [ ] 日誌脫敏
- [ ] HTTPS 傳輸加密

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
