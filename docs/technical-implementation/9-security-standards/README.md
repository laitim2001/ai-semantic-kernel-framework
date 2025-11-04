# Phase 9: 安全標準 (Security Standards)

## 階段概述

本階段定義 AI Workflow Platform 的完整安全標準,包括身份認證、授權、數據加密、安全審計和 OWASP Top 10 防護措施。

## 目標

- ✅ 實現多層次安全防護
- ✅ 確保數據機密性、完整性和可用性
- ✅ 符合安全合規要求 (GDPR, SOC 2)
- ✅ 建立安全審計機制
- ✅ 防範常見安全威脅

## 文檔結構

### 1. 身份認證與授權
**文件**: `authentication-authorization.md`

**內容**:
- JWT Token 認證
- Refresh Token 機制
- OAuth 2.0 / OpenID Connect
- Multi-Factor Authentication (MFA)
- 基於角色的訪問控制 (RBAC)
- 基於策略的授權 (Policy-Based Authorization)
- API Key 管理

**技術重點**:
- Microsoft.AspNetCore.Authentication.JwtBearer
- Identity Server / Azure AD B2C
- TOTP MFA (Time-based One-Time Password)

### 2. 數據加密
**文件**: `data-encryption.md`

**內容**:
- 傳輸層加密 (TLS/HTTPS)
- 數據庫加密 (Transparent Data Encryption)
- 敏感欄位加密
- 密鑰管理 (Azure Key Vault)
- 加密演算法選擇
- 密碼雜湊 (Bcrypt, Argon2)

**技術重點**:
- TLS 1.3
- AES-256-GCM
- RSA-4096
- Azure Key Vault / AWS KMS

### 3. 安全審計與日誌
**文件**: `security-audit-logging.md`

**內容**:
- 審計日誌設計
- 敏感操作追蹤
- 登錄/登出記錄
- 權限變更記錄
- 數據訪問日誌
- 異常行為檢測
- 日誌保留策略

**技術重點**:
- Serilog Audit
- ELK Stack (Elasticsearch, Logstash, Kibana)
- Azure Log Analytics

### 4. OWASP Top 10 防護
**文件**: `owasp-top10-protection.md`

**內容**:
- SQL Injection 防護
- XSS (Cross-Site Scripting) 防護
- CSRF (Cross-Site Request Forgery) 防護
- 不安全的反序列化
- 安全配置錯誤
- 敏感數據洩露防護
- 訪問控制失效
- 已知漏洞組件
- 日誌和監控不足
- SSRF (Server-Side Request Forgery)

**技術重點**:
- ASP.NET Core Security Middleware
- Content Security Policy (CSP)
- CORS Configuration
- Rate Limiting

### 5. 安全合規
**文件**: `security-compliance.md`

**內容**:
- GDPR 合規
- SOC 2 合規
- 個人數據保護
- 數據隱私政策
- 安全事件響應
- 滲透測試
- 安全掃描

**技術重點**:
- GDPR Right to be Forgotten
- Data Anonymization
- Security Headers
- OWASP ZAP / Burp Suite

## 認證與授權架構

### JWT Token 流程

```
┌─────────┐                          ┌─────────┐
│ Client  │                          │  API    │
└────┬────┘                          └────┬────┘
     │                                    │
     │  1. POST /api/auth/login           │
     │    { email, password }             │
     ├──────────────────────────────────>│
     │                                    │
     │                                    │  2. Validate Credentials
     │                                    │     + Generate Tokens
     │                                    │
     │  3. { accessToken, refreshToken }  │
     │<────────────────────────────────────┤
     │                                    │
     │  4. GET /api/workflows             │
     │     Authorization: Bearer {token}  │
     ├──────────────────────────────────>│
     │                                    │
     │                                    │  5. Validate Token
     │                                    │     + Check Permissions
     │                                    │
     │  6. { workflows: [...] }           │
     │<────────────────────────────────────┤
     │                                    │
     │  7. Token Expired                  │
     ├──────────────────────────────────>│
     │                                    │
     │  8. 401 Unauthorized               │
     │<────────────────────────────────────┤
     │                                    │
     │  9. POST /api/auth/refresh         │
     │     { refreshToken }               │
     ├──────────────────────────────────>│
     │                                    │
     │                                    │  10. Validate Refresh Token
     │                                    │      + Generate New Access Token
     │                                    │
     │  11. { accessToken, refreshToken } │
     │<────────────────────────────────────┤
     │                                    │
```

### RBAC 權限模型

```
┌──────────────────────────────────────┐
│            Users                     │
└──────────────┬───────────────────────┘
               │ Many-to-Many
               │
┌──────────────▼───────────────────────┐
│            User Roles                │
└──────────────┬───────────────────────┘
               │ Many-to-Many
               │
┌──────────────▼───────────────────────┐
│             Roles                    │
│  - Admin                             │
│  - User                              │
│  - Guest                             │
└──────────────┬───────────────────────┘
               │ Many-to-Many
               │
┌──────────────▼───────────────────────┐
│          Role Permissions            │
└──────────────┬───────────────────────┘
               │ Many-to-Many
               │
┌──────────────▼───────────────────────┐
│          Permissions                 │
│  - Workflow.Create                   │
│  - Workflow.Edit                     │
│  - Workflow.Delete                   │
│  - Workflow.Execute                  │
│  - User.Manage                       │
└──────────────────────────────────────┘
```

## 認證實現

### JWT Configuration

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ClockSkew = TimeSpan.Zero  // 移除默認 5 分鐘時鐘偏差
        };

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
            OnMessageReceived = context =>
            {
                // WebSocket 支持
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("WorkflowOwner", policy =>
        policy.Requirements.Add(new WorkflowOwnerRequirement()));
});
```

### Token Service

```csharp
public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),  // Access Token: 1 hour
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),  // Refresh Token: 7 days
            Created = DateTime.UtcNow
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var validation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,  // 不驗證過期
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
        };

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token, validation, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
```

## 數據加密

### Sensitive Data Encryption

```csharp
public sealed class DataEncryptionService(IConfiguration configuration) : IDataEncryptionService
{
    private readonly byte[] _key = Convert.FromBase64String(configuration["Encryption:Key"]!);

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        var iv = aes.IV;
        var encrypted = ms.ToArray();

        // 組合 IV 和加密數據
        var result = new byte[iv.Length + encrypted.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        // 提取 IV
        var iv = new byte[aes.IV.Length];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
```

### Password Hashing

```csharp
public sealed class PasswordHasher : IPasswordHasher
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

## Security Headers

### Middleware Configuration

```csharp
// Program.cs
app.UseSecurityHeaders(policies =>
{
    policies.AddDefaultSecurityHeaders()
        .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 31536000)
        .AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddScriptSrc().Self().UnsafeInline();
            builder.AddStyleSrc().Self().UnsafeInline();
            builder.AddImgSrc().Self().Data();
            builder.AddFontSrc().Self();
            builder.AddConnectSrc().Self();
            builder.AddFrameAncestors().None();
        })
        .RemoveServerHeader()
        .AddXssProtectionBlock()
        .AddFrameOptionsDeny()
        .AddContentTypeOptionsNoSniff()
        .AddReferrerPolicyNoReferrer();
});
```

## Rate Limiting

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.AddPolicy("LoginRateLimit", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(5)
            }));
});

app.UseRateLimiter();

// API 端點
app.MapPost("/api/auth/login", LoginAsync)
    .RequireRateLimiting("LoginRateLimit");
```

## 安全審計

### Audit Log Model

```csharp
public sealed class AuditLog
{
    public long Id { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public string? EntityId { get; set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }
    public Dictionary<string, object>? Changes { get; set; }
    public required DateTime Timestamp { get; set; }
}
```

### Audit Middleware

```csharp
public sealed class AuditMiddleware(RequestDelegate next, IAuditLogService auditLogService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldAudit(context))
        {
            await LogRequestAsync(context);
        }

        await next(context);
    }

    private bool ShouldAudit(HttpContext context)
    {
        var sensitiveEndpoints = new[]
        {
            "/api/auth/login",
            "/api/auth/logout",
            "/api/users",
            "/api/workflows"
        };

        return context.Request.Method != "GET" &&
               sensitiveEndpoints.Any(e => context.Request.Path.StartsWithSegments(e));
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        var auditLog = new AuditLog
        {
            UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous",
            UserName = context.User.Identity?.Name ?? "Anonymous",
            Action = $"{context.Request.Method} {context.Request.Path}",
            EntityType = ExtractEntityType(context.Request.Path),
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            Timestamp = DateTime.UtcNow
        };

        await auditLogService.LogAsync(auditLog);
    }

    private string ExtractEntityType(PathString path)
    {
        var segments = path.Value?.Split('/');
        return segments?.Length > 2 ? segments[2] : "Unknown";
    }
}
```

## 檢查清單

### 認證與授權
- [ ] 實現 JWT 認證
- [ ] 配置 Refresh Token 機制
- [ ] 實現 RBAC 權限控制
- [ ] 配置 MFA (可選)
- [ ] 實現 OAuth 2.0 (可選)

### 數據加密
- [ ] 啟用 HTTPS/TLS
- [ ] 加密敏感欄位
- [ ] 使用 Bcrypt/Argon2 雜湊密碼
- [ ] 配置密鑰管理 (Azure Key Vault)
- [ ] 加密數據庫連接字符串

### OWASP Top 10
- [ ] 防護 SQL Injection
- [ ] 防護 XSS
- [ ] 防護 CSRF
- [ ] 配置 Security Headers
- [ ] 實現 Rate Limiting
- [ ] 防護敏感數據洩露

### 審計與監控
- [ ] 實現審計日誌
- [ ] 記錄敏感操作
- [ ] 配置異常檢測
- [ ] 實現日誌保留策略
- [ ] 定期審查安全日誌

### 合規
- [ ] GDPR 合規 (Right to be Forgotten)
- [ ] 數據隱私政策
- [ ] 定期滲透測試
- [ ] 安全漏洞掃描
- [ ] 安全事件響應計劃

## 相關文檔

- [Phase 5: API 設計規範](../5-api-design/README.md)
- [Phase 6: 數據庫標準](../6-database-standards/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc8725)

### 安全標準
- [GDPR](https://gdpr.eu/)
- [SOC 2](https://www.aicpa.org/soc4so)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

---

**文檔維護**: Security Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
