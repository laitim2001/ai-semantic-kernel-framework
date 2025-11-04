# 認證實現指南 (Authentication Implementation)

## 概述

本文檔詳細說明 AI Workflow Platform 的認證系統實現,涵蓋 JWT Token、OAuth 2.0、Azure Active Directory B2C 集成、Multi-Factor Authentication (MFA) 和 Session Management。

### 目標

- ✅ 實現安全的 JWT Token 認證機制
- ✅ 集成 OAuth 2.0 / OpenID Connect
- ✅ 整合 Azure Active Directory B2C
- ✅ 支持 Multi-Factor Authentication (MFA)
- ✅ 實現 Refresh Token 機制
- ✅ 提供 Session Management 策略
- ✅ 確保密碼安全 (BCrypt/Argon2 Hashing)

### 適用範圍

- Backend API (ASP.NET Core 9.0)
- React 19 Frontend
- Vue 3 Workflow Editor
- Mobile Applications (未來擴展)

---

## 技術背景

### 認證 vs 授權

**認證 (Authentication)**:
- 驗證用戶身份 - "你是誰?"
- 使用 Username/Password, OAuth, MFA 等方式

**授權 (Authorization)**:
- 驗證用戶權限 - "你能做什麼?"
- 使用 RBAC, Claims-Based, Policy-Based 等方式

### 技術棧

```yaml
Backend:
  Framework: ASP.NET Core 9.0
  Authentication:
    - Microsoft.AspNetCore.Authentication.JwtBearer 9.0
    - Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0
    - Microsoft.Identity.Web 3.3.1
  Password Hashing: BCrypt.Net-Next 4.0.3
  Token Storage: StackExchange.Redis 2.9.0

Frontend:
  React: React 19 + axios
  Vue: Vue 3.5 + pinia
  Token Storage: localStorage (short-term) / httpOnly cookies (long-term)
```

---

## 架構設計

### 認證流程

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Client    │         │   API Server │         │  Token Store│
│ (React/Vue) │         │  (ASP.NET)   │         │   (Redis)   │
└─────────────┘         └──────────────┘         └─────────────┘
       │                       │                        │
       │ 1. Login Request      │                        │
       │ (username/password)   │                        │
       │──────────────────────>│                        │
       │                       │                        │
       │                       │ 2. Validate Credentials│
       │                       │ (Hash + Compare)       │
       │                       │                        │
       │                       │ 3. Generate JWT Token  │
       │                       │──────────────────────> │
       │                       │                        │
       │                       │ 4. Store Refresh Token │
       │                       │<─────────────────────  │
       │                       │                        │
       │ 5. Return Tokens      │                        │
       │   (Access + Refresh)  │                        │
       │<─────────────────────-│                        │
       │                       │                        │
       │ 6. API Request        │                        │
       │   (Authorization: Bearer {token})             │
       │──────────────────────>│                        │
       │                       │                        │
       │                       │ 7. Validate JWT        │
       │                       │                        │
       │ 8. Response           │                        │
       │<─────────────────────-│                        │
       │                       │                        │
       │ 9. Token Expired?     │                        │
       │   Use Refresh Token   │                        │
       │──────────────────────>│                        │
       │                       │                        │
       │                       │ 10. Validate Refresh   │
       │                       │────────────────────────>│
       │                       │                        │
       │                       │ 11. Issue New Token    │
       │                       │<────────────────────── │
       │                       │                        │
       │ 12. New Access Token  │                        │
       │<─────────────────────-│                        │
```

### Token 設計

#### Access Token (JWT)
```json
{
  "sub": "user-guid-12345",
  "email": "user@example.com",
  "name": "John Doe",
  "roles": ["User", "WorkflowCreator"],
  "permissions": ["workflow:create", "workflow:execute"],
  "iat": 1730563200,
  "exp": 1730566800,  // 1 hour expiration
  "iss": "https://api.aiworkflow.com",
  "aud": "aiworkflow-frontend"
}
```

#### Refresh Token
```
- 隨機生成的 256-bit Token
- 存儲在 Redis (Key: refresh_token:{userId}:{tokenId})
- 有效期: 7 天 (可配置)
- 支持 Token Family (Refresh Token Rotation)
```

---

## JWT 認證實現

### 1. JWT 配置

#### appsettings.json

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "https://api.aiworkflow.com",
    "Audience": "aiworkflow-frontend",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7,
    "RequireHttpsMetadata": true,
    "ValidateIssuerSigningKey": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ClockSkew": "00:05:00"
  }
}
```

**🔒 安全建議**:
- SecretKey 應至少 256-bit (32 字符)
- 生產環境使用 Azure Key Vault 存儲密鑰
- 定期輪換密鑰 (每 90 天)

### 2. JWT Service 實現

#### IJwtTokenService.cs

```csharp
namespace AIWorkflow.Application.Interfaces.Services;

public interface IJwtTokenService
{
    /// <summary>
    /// 生成 Access Token
    /// </summary>
    string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// 生成 Refresh Token
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// 驗證 Access Token
    /// </summary>
    ClaimsPrincipal? ValidateAccessToken(string token);

    /// <summary>
    /// 驗證 Refresh Token
    /// </summary>
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤銷 Refresh Token
    /// </summary>
    Task RevokeRefreshTokenAsync(string userId, string? refreshToken = null, CancellationToken cancellationToken = default);
}
```

#### JwtTokenService.cs

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace AIWorkflow.Infrastructure.Services;

public sealed class JwtTokenService(
    IOptions<JwtSettings> jwtSettings,
    IConnectionMultiplexer redis,
    ILogger<JwtTokenService> logger) : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // 添加角色
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 添加權限
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        // 生成安全的隨機 Token
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);

        // 生成唯一 Token ID
        var tokenId = Guid.NewGuid().ToString();

        // 存儲到 Redis
        var key = $"refresh_token:{user.Id}:{tokenId}";
        var expiration = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays);

        var tokenData = new
        {
            Token = refreshToken,
            UserId = user.Id.ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(expiration)
        };

        await _redisDb.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(tokenData), expiration);

        logger.LogInformation("Refresh token generated for user {UserId}", user.Id);

        return $"{tokenId}:{refreshToken}";
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = _jwtSettings.ClockSkew
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        // 解析 tokenId 和 token
        var parts = refreshToken.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        var tokenId = parts[0];
        var token = parts[1];

        // 從 Redis 獲取
        var key = $"refresh_token:{userId}:{tokenId}";
        var storedData = await _redisDb.StringGetAsync(key);

        if (storedData.IsNullOrEmpty)
        {
            logger.LogWarning("Refresh token not found for user {UserId}", userId);
            return false;
        }

        var tokenData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(storedData!);

        // 驗證 Token 和過期時間
        if (tokenData?.Token != token)
        {
            logger.LogWarning("Invalid refresh token for user {UserId}", userId);
            return false;
        }

        return true;
    }

    public async Task RevokeRefreshTokenAsync(string userId, string? refreshToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            // 撤銷用戶所有 Refresh Token
            var pattern = $"refresh_token:{userId}:*";
            var server = redis.GetServer(redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            foreach (var key in keys)
            {
                await _redisDb.KeyDeleteAsync(key);
            }

            logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);
        }
        else
        {
            // 撤銷特定 Token
            var parts = refreshToken.Split(':');
            if (parts.Length == 2)
            {
                var tokenId = parts[0];
                var key = $"refresh_token:{userId}:{tokenId}";
                await _redisDb.KeyDeleteAsync(key);

                logger.LogInformation("Refresh token {TokenId} revoked for user {UserId}", tokenId, userId);
            }
        }
    }
}
```

### 3. ASP.NET Core Authentication 配置

#### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

    options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = jwtSettings.ValidateIssuer,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = jwtSettings.ValidateAudience,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = jwtSettings.ValidateLifetime,
        ClockSkew = jwtSettings.ClockSkew
    };

    // 自定義事件處理
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("Token validated for user {UserId}", userId);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Unauthorized", message = "Invalid or expired token" });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

// Register JWT Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## 登入與註冊 API

### AuthController.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtTokenService jwtTokenService,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// 用戶登入
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        // 驗證輸入
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Email and password are required" });
        }

        // 查找用戶
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("Login failed: User {Email} not found", request.Email);
            return Unauthorized(new { error = "Invalid credentials" });
        }

        // 檢查帳號狀態
        if (!user.IsActive)
        {
            return Unauthorized(new { error = "Account is disabled" });
        }

        // 驗證密碼
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                logger.LogWarning("Account {Email} is locked out", request.Email);
                return Unauthorized(new { error = "Account is locked due to multiple failed login attempts" });
            }

            logger.LogWarning("Login failed: Invalid password for user {Email}", request.Email);
            return Unauthorized(new { error = "Invalid credentials" });
        }

        // 獲取角色和權限
        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user.Id);

        // 生成 Token
        var accessToken = jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        logger.LogInformation("User {Email} logged in successfully", request.Email);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600, // 1 hour
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Roles = roles.ToList()
            }
        });
    }

    /// <summary>
    /// 用戶註冊
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        // 驗證輸入
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Email and password are required" });
        }

        // 檢查是否已存在
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Conflict(new { error = "User already exists" });
        }

        // 創建用戶
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("User registration failed: {Errors}", errors);
            return BadRequest(new { error = "Registration failed", details = result.Errors });
        }

        // 分配默認角色
        await userManager.AddToRoleAsync(user, "User");

        logger.LogInformation("User {Email} registered successfully", request.Email);

        // 自動登入
        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user.Id);

        var accessToken = jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Roles = roles.ToList()
            }
        });
    }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        // 驗證 Refresh Token
        var principal = jwtTokenService.ValidateAccessToken(request.AccessToken);
        if (principal == null)
        {
            return Unauthorized(new { error = "Invalid access token" });
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid token claims" });
        }

        var isValidRefreshToken = await jwtTokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken, cancellationToken);
        if (!isValidRefreshToken)
        {
            return Unauthorized(new { error = "Invalid refresh token" });
        }

        // 獲取用戶
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { error = "User not found or disabled" });
        }

        // 撤銷舊 Token
        await jwtTokenService.RevokeRefreshTokenAsync(userId, request.RefreshToken, cancellationToken);

        // 生成新 Token
        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user.Id);

        var newAccessToken = jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var newRefreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        logger.LogInformation("Token refreshed for user {UserId}", userId);

        return Ok(new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Roles = roles.ToList()
            }
        });
    }

    /// <summary>
    /// 登出
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // 撤銷所有 Refresh Token
        await jwtTokenService.RevokeRefreshTokenAsync(userId, cancellationToken: cancellationToken);

        logger.LogInformation("User {UserId} logged out", userId);

        return Ok(new { message = "Logged out successfully" });
    }

    private async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        // TODO: 實現權限查詢邏輯
        // 這裡可以從數據庫查詢用戶的權限
        return new List<string> { "workflow:read", "workflow:create" };
    }
}

// DTOs
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Name);
public record RefreshTokenRequest(string AccessToken, string RefreshToken);

public record LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required string TokenType { get; init; }
    public required int ExpiresIn { get; init; }
    public required UserDto User { get; init; }
}

public record UserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required List<string> Roles { get; init; }
}
```

---

## 密碼安全

### Password Hashing (BCrypt)

ASP.NET Core Identity 默認使用 PBKDF2,但我們可以配置使用更安全的 BCrypt 或 Argon2。

#### PasswordHasher 配置

```csharp
builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
    options.IterationCount = 100_000; // 增加迭代次數提高安全性
});
```

#### Password Policy

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12; // 最少 12 字符
    options.Password.RequiredUniqueChars = 4;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
});
```

---

## Multi-Factor Authentication (MFA)

### TOTP (Time-Based One-Time Password) 實現

#### MFA 設置流程

```csharp
[HttpPost("mfa/enable")]
[Authorize]
public async Task<ActionResult<MfaSetupResponse>> EnableMfa()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var user = await userManager.FindByIdAsync(userId!);

    if (user == null)
    {
        return NotFound();
    }

    // 生成 MFA 密鑰
    var key = await userManager.GetAuthenticatorKeyAsync(user);
    if (string.IsNullOrEmpty(key))
    {
        await userManager.ResetAuthenticatorKeyAsync(user);
        key = await userManager.GetAuthenticatorKeyAsync(user);
    }

    // 生成 QR Code URI
    var email = await userManager.GetEmailAsync(user);
    var authenticatorUri = GenerateQrCodeUri(email!, key!);

    return Ok(new MfaSetupResponse
    {
        SecretKey = key!,
        QrCodeUri = authenticatorUri
    });
}

[HttpPost("mfa/verify")]
[Authorize]
public async Task<ActionResult> VerifyMfa([FromBody] VerifyMfaRequest request)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var user = await userManager.FindByIdAsync(userId!);

    if (user == null)
    {
        return NotFound();
    }

    // 驗證 TOTP 代碼
    var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
        user,
        userManager.Options.Tokens.AuthenticatorTokenProvider,
        request.Code);

    if (!is2faTokenValid)
    {
        return BadRequest(new { error = "Invalid verification code" });
    }

    // 啟用 MFA
    await userManager.SetTwoFactorEnabledAsync(user, true);

    logger.LogInformation("MFA enabled for user {UserId}", userId);

    return Ok(new { message = "MFA enabled successfully" });
}

private string GenerateQrCodeUri(string email, string key)
{
    const string authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    return string.Format(
        authenticatorUriFormat,
        Uri.EscapeDataString("AIWorkflow"),
        Uri.EscapeDataString(email),
        key);
}

public record MfaSetupResponse
{
    public required string SecretKey { get; init; }
    public required string QrCodeUri { get; init; }
}

public record VerifyMfaRequest(string Code);
```

---

## Frontend 集成

### React 19 Authentication Hook

#### useAuth.ts

```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import axios from 'axios';

interface User {
  id: string;
  email: string;
  name: string;
  roles: string[];
}

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, name: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshAccessToken: () => Promise<void>;
}

export const useAuth = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,

      login: async (email: string, password: string) => {
        try {
          const response = await axios.post('/api/auth/login', { email, password });
          const { accessToken, refreshToken, user } = response.data;

          set({
            user,
            accessToken,
            refreshToken,
            isAuthenticated: true,
          });

          // 設置 Axios 默認 Authorization Header
          axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
        } catch (error) {
          console.error('Login failed:', error);
          throw error;
        }
      },

      register: async (email: string, password: string, name: string) => {
        try {
          const response = await axios.post('/api/auth/register', { email, password, name });
          const { accessToken, refreshToken, user } = response.data;

          set({
            user,
            accessToken,
            refreshToken,
            isAuthenticated: true,
          });

          axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
        } catch (error) {
          console.error('Registration failed:', error);
          throw error;
        }
      },

      logout: async () => {
        try {
          await axios.post('/api/auth/logout');
        } catch (error) {
          console.error('Logout failed:', error);
        } finally {
          set({
            user: null,
            accessToken: null,
            refreshToken: null,
            isAuthenticated: false,
          });

          delete axios.defaults.headers.common['Authorization'];
        }
      },

      refreshAccessToken: async () => {
        const { accessToken, refreshToken } = get();

        if (!accessToken || !refreshToken) {
          throw new Error('No tokens available');
        }

        try {
          const response = await axios.post('/api/auth/refresh', {
            accessToken,
            refreshToken,
          });

          const { accessToken: newAccessToken, refreshToken: newRefreshToken, user } = response.data;

          set({
            accessToken: newAccessToken,
            refreshToken: newRefreshToken,
            user,
          });

          axios.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`;
        } catch (error) {
          console.error('Token refresh failed:', error);
          // Token 刷新失敗,強制登出
          get().logout();
          throw error;
        }
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);

// Axios Interceptor for Auto Token Refresh
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // 如果是 401 且 Token 過期
    if (error.response?.status === 401 && error.response?.headers['token-expired'] && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        await useAuth.getState().refreshAccessToken();
        // 重試原始請求
        return axios(originalRequest);
      } catch (refreshError) {
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);
```

### Vue 3 Pinia Store

#### auth.ts

```typescript
import { defineStore } from 'pinia';
import axios from 'axios';

interface User {
  id: string;
  email: string;
  name: string;
  roles: string[];
}

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
    accessToken: localStorage.getItem('accessToken'),
    refreshToken: localStorage.getItem('refreshToken'),
    isAuthenticated: !!localStorage.getItem('accessToken'),
  }),

  actions: {
    async login(email: string, password: string) {
      try {
        const response = await axios.post('/api/auth/login', { email, password });
        const { accessToken, refreshToken, user } = response.data;

        this.user = user;
        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
        this.isAuthenticated = true;

        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);

        axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
      } catch (error) {
        console.error('Login failed:', error);
        throw error;
      }
    },

    async logout() {
      try {
        await axios.post('/api/auth/logout');
      } catch (error) {
        console.error('Logout failed:', error);
      } finally {
        this.user = null;
        this.accessToken = null;
        this.refreshToken = null;
        this.isAuthenticated = false;

        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');

        delete axios.defaults.headers.common['Authorization'];
      }
    },

    async refreshAccessToken() {
      if (!this.accessToken || !this.refreshToken) {
        throw new Error('No tokens available');
      }

      try {
        const response = await axios.post('/api/auth/refresh', {
          accessToken: this.accessToken,
          refreshToken: this.refreshToken,
        });

        const { accessToken: newAccessToken, refreshToken: newRefreshToken, user } = response.data;

        this.accessToken = newAccessToken;
        this.refreshToken = newRefreshToken;
        this.user = user;

        localStorage.setItem('accessToken', newAccessToken);
        localStorage.setItem('refreshToken', newRefreshToken);

        axios.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`;
      } catch (error) {
        console.error('Token refresh failed:', error);
        this.logout();
        throw error;
      }
    },
  },
});
```

---

## 最佳實踐

### ✅ Do's

1. **使用 HTTPS**: 生產環境必須使用 TLS 1.3
2. **Token 過期時間**: Access Token 短期 (1 小時), Refresh Token 長期 (7 天)
3. **密鑰管理**: 使用 Azure Key Vault 存儲密鑰
4. **Token 撤銷**: 實現 Refresh Token 撤銷機制
5. **MFA**: 對敏感操作啟用 Multi-Factor Authentication
6. **密碼策略**: 最少 12 字符,包含大小寫、數字和特殊字符
7. **Lockout 策略**: 5 次失敗登入後鎖定 15 分鐘
8. **Secure Storage**: Frontend 使用 httpOnly cookies 存儲 Token (更安全)

### ❌ Don'ts

1. **不要在 LocalStorage 存儲敏感 Token**: 容易受 XSS 攻擊
2. **不要使用明文密碼**: 必須使用 BCrypt/Argon2 Hashing
3. **不要在 URL 傳遞 Token**: 會被記錄在日誌中
4. **不要使用過長的 Token 有效期**: Access Token 不應超過 2 小時
5. **不要忽略 Token 刷新**: 實現自動刷新機制
6. **不要在 Git 提交密鑰**: 使用環境變數或 Azure Key Vault

---

## 測試策略

### Unit Tests

```csharp
[Fact]
public void GenerateAccessToken_ShouldCreateValidJwt()
{
    // Arrange
    var jwtSettings = Options.Create(new JwtSettings
    {
        SecretKey = "test_secret_key_at_least_32_characters",
        Issuer = "test-issuer",
        Audience = "test-audience",
        AccessTokenExpirationMinutes = 60
    });

    var service = new JwtTokenService(jwtSettings, Mock.Of<IConnectionMultiplexer>(), Mock.Of<ILogger<JwtTokenService>>());

    var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" };
    var roles = new[] { "User" };
    var permissions = new[] { "workflow:read" };

    // Act
    var token = service.GenerateAccessToken(user, roles, permissions);

    // Assert
    Assert.NotNull(token);
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token);
    Assert.Equal(user.Id.ToString(), jwtToken.Subject);
    Assert.Equal("test@example.com", jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
}
```

### Integration Tests

```csharp
[Fact]
public async Task Login_WithValidCredentials_ShouldReturnTokens()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { email = "test@example.com", password = "Test@123456" };

    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
    Assert.NotNull(result);
    Assert.NotNull(result.AccessToken);
    Assert.NotNull(result.RefreshToken);
}
```

---

## 故障排查

### 常見問題

#### 1. Token 驗證失敗: "Invalid signature"

**原因**: SecretKey 不一致或算法錯誤

**解決方案**:
```csharp
// 檢查 SecretKey 是否一致
// 確認 SecurityAlgorithms.HmacSha256
```

#### 2. Token 過期無法刷新

**原因**: Refresh Token 已被撤銷或過期

**解決方案**:
```csharp
// 檢查 Redis 中是否存在 Refresh Token
// 驗證 Token 過期時間
```

#### 3. CORS 錯誤導致認證失敗

**原因**: CORS 未正確配置

**解決方案**:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

## 實施檢查清單

### Backend
- [ ] JWT Service 實現完成
- [ ] AuthController API 端點完成
- [ ] Password Hashing 配置
- [ ] Lockout 策略配置
- [ ] Refresh Token Redis 存儲
- [ ] MFA 實現 (可選)
- [ ] 單元測試覆蓋 >80%

### Frontend
- [ ] React/Vue Authentication Store
- [ ] Axios Interceptor 自動刷新
- [ ] Login/Register UI
- [ ] Protected Route 實現
- [ ] Token 存儲策略
- [ ] 錯誤處理和重試邏輯

### Security
- [ ] HTTPS 配置 (生產環境)
- [ ] Azure Key Vault 集成
- [ ] CORS 配置
- [ ] Security Headers
- [ ] Penetration Testing

---

## 相關文檔

- [Phase 9: 授權與 RBAC](./authorization-rbac.md)
- [Phase 9: 數據加密標準](./data-encryption-standards.md)
- [Phase 5: API 設計規範](../5-api-design/README.md)
- [Phase 8: 部署架構](../8-deployment-architecture/README.md)

## 參考資源

### 官方文檔
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT.io](https://jwt.io/)

### 最佳實踐
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [RFC 7519: JSON Web Token](https://datatracker.ietf.org/doc/html/rfc7519)
- [OAuth 2.0 RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749)

---

**文檔維護**: Security Team
**最後更新**: 2025-11-02
**狀態**: ✅ 完整
