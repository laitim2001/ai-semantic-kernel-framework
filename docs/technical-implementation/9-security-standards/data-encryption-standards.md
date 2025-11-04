# 數據加密標準 (Data Encryption Standards)

## 概述

本文檔詳細說明 AI Workflow Platform 的數據加密標準,涵蓋傳輸層加密、靜態數據加密、敏感數據保護、Azure Key Vault 集成和密鑰管理策略。

### 目標

- ✅ 實現 TLS 1.3 傳輸層加密
- ✅ 實現數據庫靜態加密 (Encryption at Rest)
- ✅ 保護敏感數據 (API Keys, Credentials, PII)
- ✅ 集成 Azure Key Vault 進行密鑰管理
- ✅ 實現加密密鑰輪換策略
- ✅ 確保 GDPR 和數據保護合規性
- ✅ 提供安全的配置管理

### 適用範圍

- Backend API (ASP.NET Core 9.0)
- PostgreSQL Database
- Redis Cache
- Azure Services
- Frontend Applications

---

## 技術背景

### 加密類型

| 類型 | 用途 | 算法 | 密鑰長度 |
|------|------|------|----------|
| **對稱加密** | 數據加密 | AES-256-GCM | 256-bit |
| **非對稱加密** | 密鑰交換、數字簽名 | RSA-2048/4096 | 2048/4096-bit |
| **哈希** | 密碼存儲、完整性驗證 | SHA-256/SHA-512 | - |
| **HMAC** | 消息認證碼 | HMAC-SHA256 | 256-bit |

### 技術棧

```yaml
Backend:
  Encryption:
    - System.Security.Cryptography (AES-256-GCM)
    - Azure.Security.KeyVault.Secrets 4.7.0
    - Azure.Security.KeyVault.Keys 4.7.0
  Password Hashing: BCrypt.Net-Next 4.0.3
  Configuration: Microsoft.Extensions.Configuration.AzureKeyVault

Database:
  PostgreSQL:
    - pgcrypto extension (Database-level encryption)
    - Transparent Data Encryption (TDE)
  Redis:
    - TLS/SSL encryption
    - AUTH password protection

Azure Services:
  - Azure Key Vault (Secret & Key Management)
  - Azure Storage Encryption (Blob Storage)
  - Azure Disk Encryption (Managed Disks)
```

---

## 傳輸層加密 (Encryption in Transit)

### TLS 1.3 配置

#### Program.cs (ASP.NET Core)

```csharp
var builder = WebApplication.CreateBuilder(args);

// HTTPS 重定向配置
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// Kestrel HTTPS 配置
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        // 強制 TLS 1.2 和 TLS 1.3
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
    });
});

// HSTS (HTTP Strict Transport Security)
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
```

### Security Headers

```csharp
app.Use(async (context, next) =>
{
    // HTTPS Strict Transport Security
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

    // Content Security Policy
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' data:; " +
        "connect-src 'self' https://api.aiworkflow.com;");

    // X-Frame-Options
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // X-Content-Type-Options
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // X-XSS-Protection
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Referrer-Policy
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Permissions-Policy
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    await next();
});
```

### PostgreSQL TLS 連接

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aiworkflow-db.postgres.database.azure.com;Database=aiworkflow;Username=admin;Password=***;SSL Mode=Require;Trust Server Certificate=false;"
  }
}
```

```csharp
// Startup configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);

        // 強制 SSL
        npgsqlOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
        {
            // 生產環境應驗證證書
            return builder.Environment.IsProduction() ? sslPolicyErrors == System.Net.Security.SslPolicyErrors.None : true;
        };
    });
});
```

### Redis TLS 連接

```csharp
// appsettings.json
{
  "Redis": {
    "ConnectionString": "aiworkflow-redis.redis.cache.windows.net:6380,password=***,ssl=True,abortConnect=False"
  }
}
```

```csharp
// Startup configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration["Redis:ConnectionString"]!);

    // 強制 SSL
    configuration.Ssl = true;
    configuration.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;

    // 證書驗證 (生產環境)
    configuration.CertificateValidation += (sender, cert, chain, errors) =>
    {
        return builder.Environment.IsProduction() ? errors == System.Net.Security.SslPolicyErrors.None : true;
    };

    return ConnectionMultiplexer.Connect(configuration);
});
```

---

## 靜態數據加密 (Encryption at Rest)

### PostgreSQL 數據庫加密

#### 1. Transparent Data Encryption (TDE)

Azure PostgreSQL Flexible Server 自動啟用 TDE:

```bash
# Azure CLI 驗證 TDE 狀態
az postgres flexible-server show \
  --resource-group aiworkflow-rg \
  --name aiworkflow-db \
  --query "storage.encryptionStatus"
```

#### 2. 列級加密 (Column-Level Encryption)

使用 PostgreSQL `pgcrypto` 擴展:

```sql
-- 啟用 pgcrypto 擴展
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- 創建加密表
CREATE TABLE identity.user_credentials (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES identity.users(id),
    credential_type VARCHAR(50) NOT NULL,
    encrypted_value BYTEA NOT NULL,  -- 加密後的數據
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

-- 插入加密數據
INSERT INTO identity.user_credentials (user_id, credential_type, encrypted_value)
VALUES (
    '550e8400-e29b-41d4-a716-446655440000',
    'OpenAI_API_Key',
    pgp_sym_encrypt('sk-proj-...', 'encryption-key-from-key-vault')
);

-- 查詢解密數據
SELECT
    user_id,
    credential_type,
    pgp_sym_decrypt(encrypted_value, 'encryption-key-from-key-vault') AS decrypted_value
FROM identity.user_credentials
WHERE user_id = '550e8400-e29b-41d4-a716-446655440000';
```

#### 3. Application-Level Encryption (推薦)

使用 .NET 加密敏感數據後再存儲:

```csharp
namespace AIWorkflow.Infrastructure.Services;

public interface IEncryptionService
{
    /// <summary>
    /// 加密字符串
    /// </summary>
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密字符串
    /// </summary>
    Task<string> DecryptAsync(string cipherText, CancellationToken cancellationToken = default);

    /// <summary>
    /// 加密敏感數據 (使用 Data Encryption Key)
    /// </summary>
    Task<byte[]> EncryptDataAsync(byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密敏感數據
    /// </summary>
    Task<byte[]> DecryptDataAsync(byte[] encryptedData, CancellationToken cancellationToken = default);
}

public sealed class EncryptionService(
    IKeyVaultService keyVaultService,
    ILogger<EncryptionService> logger) : IEncryptionService
{
    private const string DataEncryptionKeyName = "data-encryption-key";

    public async Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));
        }

        try
        {
            // 從 Key Vault 獲取加密密鑰
            var key = await keyVaultService.GetSecretAsync(DataEncryptionKeyName, cancellationToken);
            var keyBytes = Convert.FromBase64String(key);

            // 生成隨機 IV (Initialization Vector)
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.GCM; // 使用 GCM 模式 (推薦)
            aes.GenerateIV();

            var iv = aes.IV;

            // 加密數據
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // 組合 IV + Cipher Text + Tag
            var tag = new byte[16]; // GCM Tag
            var result = new byte[iv.Length + cipherBytes.Length + tag.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, result, iv.Length + cipherBytes.Length, tag.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encryption failed");
            throw;
        }
    }

    public async Task<string> DecryptAsync(string cipherText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            throw new ArgumentException("Cipher text cannot be null or empty", nameof(cipherText));
        }

        try
        {
            // 從 Key Vault 獲取解密密鑰
            var key = await keyVaultService.GetSecretAsync(DataEncryptionKeyName, cancellationToken);
            var keyBytes = Convert.FromBase64String(key);

            // 解析 IV + Cipher Text + Tag
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.GCM;

            var iv = new byte[12]; // GCM IV length
            var cipher = new byte[cipherBytes.Length - iv.Length - 16];
            var tag = new byte[16];

            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, iv.Length, cipher, 0, cipher.Length);
            Buffer.BlockCopy(cipherBytes, iv.Length + cipher.Length, tag, 0, tag.Length);

            aes.IV = iv;

            // 解密數據
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Decryption failed");
            throw;
        }
    }

    public async Task<byte[]> EncryptDataAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty", nameof(data));
        }

        var key = await keyVaultService.GetSecretAsync(DataEncryptionKeyName, cancellationToken);
        var keyBytes = Convert.FromBase64String(key);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.GCM;
        aes.GenerateIV();

        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(data, 0, data.Length);

        var tag = new byte[16];
        var result = new byte[iv.Length + cipherBytes.Length + tag.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length + cipherBytes.Length, tag.Length);

        return result;
    }

    public async Task<byte[]> DecryptDataAsync(byte[] encryptedData, CancellationToken cancellationToken = default)
    {
        if (encryptedData == null || encryptedData.Length == 0)
        {
            throw new ArgumentException("Encrypted data cannot be null or empty", nameof(encryptedData));
        }

        var key = await keyVaultService.GetSecretAsync(DataEncryptionKeyName, cancellationToken);
        var keyBytes = Convert.FromBase64String(key);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.GCM;

        var iv = new byte[12];
        var cipher = new byte[encryptedData.Length - iv.Length - 16];
        var tag = new byte[16];

        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, iv.Length, cipher, 0, cipher.Length);
        Buffer.BlockCopy(encryptedData, iv.Length + cipher.Length, tag, 0, tag.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
    }
}
```

### Entity 自動加密

#### UserCredential Entity

```csharp
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// 用戶憑證實體 (自動加密)
/// </summary>
public sealed class UserCredential : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    /// <summary>
    /// 憑證類型 (OpenAI_API_Key, GitHub_Token, etc.)
    /// </summary>
    public required string CredentialType { get; set; }

    /// <summary>
    /// 加密後的值 (存儲為 Base64)
    /// </summary>
    public required string EncryptedValue { get; set; }

    /// <summary>
    /// 明文值 (不映射到數據庫)
    /// </summary>
    [NotMapped]
    public string? PlainValue { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
}
```

#### UserCredentialConfiguration

```csharp
public sealed class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.ToTable("user_credentials", "identity");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.CredentialType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(uc => uc.EncryptedValue)
            .IsRequired()
            .HasColumnType("text"); // 存儲 Base64 字符串

        builder.HasIndex(uc => new { uc.UserId, uc.CredentialType })
            .IsUnique()
            .HasDatabaseName("ix_user_credentials_user_id_type");

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Credentials)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

#### UserCredentialService

```csharp
public interface IUserCredentialService
{
    Task<UserCredential> CreateCredentialAsync(Guid userId, string credentialType, string plainValue, CancellationToken cancellationToken = default);
    Task<string> GetCredentialValueAsync(Guid userId, string credentialType, CancellationToken cancellationToken = default);
    Task UpdateCredentialAsync(Guid userId, string credentialType, string newPlainValue, CancellationToken cancellationToken = default);
    Task DeleteCredentialAsync(Guid userId, string credentialType, CancellationToken cancellationToken = default);
}

public sealed class UserCredentialService(
    ApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ILogger<UserCredentialService> logger) : IUserCredentialService
{
    public async Task<UserCredential> CreateCredentialAsync(Guid userId, string credentialType, string plainValue, CancellationToken cancellationToken = default)
    {
        // 加密明文值
        var encryptedValue = await encryptionService.EncryptAsync(plainValue, cancellationToken);

        var credential = new UserCredential
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CredentialType = credentialType,
            EncryptedValue = encryptedValue,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.UserCredentials.Add(credential);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Credential {CredentialType} created for user {UserId}", credentialType, userId);

        return credential;
    }

    public async Task<string> GetCredentialValueAsync(Guid userId, string credentialType, CancellationToken cancellationToken = default)
    {
        var credential = await dbContext.UserCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CredentialType == credentialType, cancellationToken);

        if (credential == null)
        {
            throw new NotFoundException($"Credential {credentialType} not found for user {userId}");
        }

        // 解密值
        var plainValue = await encryptionService.DecryptAsync(credential.EncryptedValue, cancellationToken);

        return plainValue;
    }

    public async Task UpdateCredentialAsync(Guid userId, string credentialType, string newPlainValue, CancellationToken cancellationToken = default)
    {
        var credential = await dbContext.UserCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CredentialType == credentialType, cancellationToken);

        if (credential == null)
        {
            throw new NotFoundException($"Credential {credentialType} not found for user {userId}");
        }

        // 加密新值
        credential.EncryptedValue = await encryptionService.EncryptAsync(newPlainValue, cancellationToken);
        credential.LastModifiedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Credential {CredentialType} updated for user {UserId}", credentialType, userId);
    }

    public async Task DeleteCredentialAsync(Guid userId, string credentialType, CancellationToken cancellationToken = default)
    {
        var credential = await dbContext.UserCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CredentialType == credentialType, cancellationToken);

        if (credential == null)
        {
            throw new NotFoundException($"Credential {credentialType} not found for user {userId}");
        }

        dbContext.UserCredentials.Remove(credential);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Credential {CredentialType} deleted for user {UserId}", credentialType, userId);
    }
}
```

---

## Azure Key Vault 集成

### Key Vault Service

#### IKeyVaultService.cs

```csharp
namespace AIWorkflow.Application.Interfaces.Services;

public interface IKeyVaultService
{
    /// <summary>
    /// 獲取 Secret
    /// </summary>
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 設置 Secret
    /// </summary>
    Task SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除 Secret
    /// </summary>
    Task DeleteSecretAsync(string secretName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出所有 Secrets
    /// </summary>
    Task<IEnumerable<string>> ListSecretsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 加密數據 (使用 Key Vault Key)
    /// </summary>
    Task<byte[]> EncryptWithKeyAsync(string keyName, byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密數據 (使用 Key Vault Key)
    /// </summary>
    Task<byte[]> DecryptWithKeyAsync(string keyName, byte[] encryptedData, CancellationToken cancellationToken = default);
}
```

#### KeyVaultService.cs

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace AIWorkflow.Infrastructure.Services;

public sealed class KeyVaultService : IKeyVaultService
{
    private readonly SecretClient _secretClient;
    private readonly KeyClient _keyClient;
    private readonly ILogger<KeyVaultService> _logger;

    public KeyVaultService(IConfiguration configuration, ILogger<KeyVaultService> logger)
    {
        var keyVaultUrl = configuration["Azure:KeyVault:Url"]
            ?? throw new InvalidOperationException("Azure Key Vault URL not configured");

        var credential = new DefaultAzureCredential();

        _secretClient = new SecretClient(new Uri(keyVaultUrl), credential);
        _keyClient = new KeyClient(new Uri(keyVaultUrl), credential);
        _logger = logger;
    }

    public async Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        try
        {
            var secret = await _secretClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);
            _logger.LogInformation("Retrieved secret {SecretName} from Key Vault", secretName);
            return secret.Value.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve secret {SecretName}", secretName);
            throw;
        }
    }

    public async Task SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default)
    {
        try
        {
            await _secretClient.SetSecretAsync(secretName, secretValue, cancellationToken);
            _logger.LogInformation("Set secret {SecretName} in Key Vault", secretName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set secret {SecretName}", secretName);
            throw;
        }
    }

    public async Task DeleteSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        try
        {
            var operation = await _secretClient.StartDeleteSecretAsync(secretName, cancellationToken);
            await operation.WaitForCompletionAsync(cancellationToken);
            _logger.LogInformation("Deleted secret {SecretName} from Key Vault", secretName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete secret {SecretName}", secretName);
            throw;
        }
    }

    public async Task<IEnumerable<string>> ListSecretsAsync(CancellationToken cancellationToken = default)
    {
        var secretNames = new List<string>();

        await foreach (var secret in _secretClient.GetPropertiesOfSecretsAsync(cancellationToken))
        {
            secretNames.Add(secret.Name);
        }

        return secretNames;
    }

    public async Task<byte[]> EncryptWithKeyAsync(string keyName, byte[] data, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = await _keyClient.GetKeyAsync(keyName, cancellationToken: cancellationToken);
            var cryptoClient = new CryptographyClient(key.Value.Id, new DefaultAzureCredential());

            var encryptResult = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep256, data, cancellationToken);

            _logger.LogInformation("Encrypted data with key {KeyName}", keyName);

            return encryptResult.Ciphertext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt with key {KeyName}", keyName);
            throw;
        }
    }

    public async Task<byte[]> DecryptWithKeyAsync(string keyName, byte[] encryptedData, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = await _keyClient.GetKeyAsync(keyName, cancellationToken: cancellationToken);
            var cryptoClient = new CryptographyClient(key.Value.Id, new DefaultAzureCredential());

            var decryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep256, encryptedData, cancellationToken);

            _logger.LogInformation("Decrypted data with key {KeyName}", keyName);

            return decryptResult.Plaintext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt with key {KeyName}", keyName);
            throw;
        }
    }
}
```

### Configuration Integration

#### appsettings.json

```json
{
  "Azure": {
    "KeyVault": {
      "Url": "https://aiworkflow-kv.vault.azure.net/"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=#{KeyVault:PostgreSQL-ConnectionString}#"
  },
  "Redis": {
    "ConnectionString": "#{KeyVault:Redis-ConnectionString}#"
  },
  "JwtSettings": {
    "SecretKey": "#{KeyVault:JWT-SecretKey}#",
    "Issuer": "https://api.aiworkflow.com",
    "Audience": "aiworkflow-frontend"
  }
}
```

#### Program.cs Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

// 集成 Azure Key Vault
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = builder.Configuration["Azure:KeyVault:Url"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }
}

// 註冊 Key Vault Service
builder.Services.AddSingleton<IKeyVaultService, KeyVaultService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
```

---

## 密鑰輪換策略

### 自動化密鑰輪換

#### KeyRotationService.cs

```csharp
namespace AIWorkflow.Infrastructure.Services;

public interface IKeyRotationService
{
    Task RotateDataEncryptionKeyAsync(CancellationToken cancellationToken = default);
    Task RotateJwtSecretKeyAsync(CancellationToken cancellationToken = default);
}

public sealed class KeyRotationService(
    IKeyVaultService keyVaultService,
    ApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ILogger<KeyRotationService> logger) : IKeyRotationService
{
    public async Task RotateDataEncryptionKeyAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting data encryption key rotation");

        try
        {
            // 1. 生成新密鑰
            var newKey = GenerateRandomKey(256);
            var newKeyBase64 = Convert.ToBase64String(newKey);

            // 2. 保存舊密鑰為備份
            var oldKey = await keyVaultService.GetSecretAsync("data-encryption-key", cancellationToken);
            await keyVaultService.SetSecretAsync("data-encryption-key-old", oldKey, cancellationToken);

            // 3. 設置新密鑰
            await keyVaultService.SetSecretAsync("data-encryption-key", newKeyBase64, cancellationToken);

            // 4. 重新加密所有敏感數據
            await ReEncryptAllCredentialsAsync(oldKey, newKeyBase64, cancellationToken);

            // 5. 刪除舊密鑰備份 (可選,建議保留一段時間)
            // await keyVaultService.DeleteSecretAsync("data-encryption-key-old", cancellationToken);

            logger.LogInformation("Data encryption key rotation completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data encryption key rotation failed");
            throw;
        }
    }

    public async Task RotateJwtSecretKeyAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting JWT secret key rotation");

        try
        {
            // 生成新 JWT Secret Key (最少 256-bit)
            var newKey = GenerateRandomKey(256);
            var newKeyBase64 = Convert.ToBase64String(newKey);

            // 保存到 Key Vault
            await keyVaultService.SetSecretAsync("jwt-secret-key", newKeyBase64, cancellationToken);

            // 注意: JWT 密鑰輪換會使所有現有 Token 失效
            // 建議實現 Token Versioning 或雙密鑰驗證策略

            logger.LogInformation("JWT secret key rotation completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "JWT secret key rotation failed");
            throw;
        }
    }

    private async Task ReEncryptAllCredentialsAsync(string oldKeyBase64, string newKeyBase64, CancellationToken cancellationToken)
    {
        var credentials = await dbContext.UserCredentials.ToListAsync(cancellationToken);

        logger.LogInformation("Re-encrypting {Count} credentials", credentials.Count);

        foreach (var credential in credentials)
        {
            try
            {
                // 使用舊密鑰解密
                var plainValue = await DecryptWithKeyAsync(credential.EncryptedValue, oldKeyBase64);

                // 使用新密鑰加密
                var newEncryptedValue = await EncryptWithKeyAsync(plainValue, newKeyBase64);

                credential.EncryptedValue = newEncryptedValue;
                credential.LastModifiedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to re-encrypt credential {CredentialId}", credential.Id);
                throw;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("All credentials re-encrypted successfully");
    }

    private static byte[] GenerateRandomKey(int bitLength)
    {
        var keyBytes = new byte[bitLength / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(keyBytes);
        return keyBytes;
    }

    private async Task<string> DecryptWithKeyAsync(string cipherText, string keyBase64)
    {
        // 簡化版本,實際應使用 EncryptionService
        var keyBytes = Convert.FromBase64String(keyBase64);
        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.GCM;

        // 解密邏輯 (省略詳細實現)
        // ...

        return "decrypted-value";
    }

    private async Task<string> EncryptWithKeyAsync(string plainText, string keyBase64)
    {
        // 簡化版本,實際應使用 EncryptionService
        var keyBytes = Convert.FromBase64String(keyBase64);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.Mode = CipherMode.GCM;

        // 加密邏輯 (省略詳細實現)
        // ...

        return "encrypted-value";
    }
}
```

### 定期輪換計劃

```csharp
// Hosted Service for Key Rotation
public sealed class KeyRotationBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<KeyRotationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Key Rotation Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 每 90 天輪換一次密鑰
                await Task.Delay(TimeSpan.FromDays(90), stoppingToken);

                using var scope = serviceProvider.CreateScope();
                var keyRotationService = scope.ServiceProvider.GetRequiredService<IKeyRotationService>();

                await keyRotationService.RotateDataEncryptionKeyAsync(stoppingToken);

                logger.LogInformation("Scheduled key rotation completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Scheduled key rotation failed");
            }
        }
    }
}

// Register in Program.cs
builder.Services.AddHostedService<KeyRotationBackgroundService>();
```

---

## Personal Data Protection (GDPR Compliance)

### PII (Personally Identifiable Information) 加密

#### PII Fields 識別

```csharp
// 需要加密的 PII 字段
public sealed class User : AuditableEntity
{
    public Guid Id { get; set; }

    // 🔒 敏感: Email
    public required string Email { get; set; }

    // 🔒 敏感: 全名
    public required string Name { get; set; }

    // 🔒 敏感: 電話號碼 (可選)
    public string? PhoneNumber { get; set; }

    // 非敏感
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### GDPR Data Export

```csharp
public interface IGdprService
{
    Task<GdprDataExport> ExportUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AnonymizeUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
}

public sealed class GdprService(
    ApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ILogger<GdprService> logger) : IGdprService
{
    public async Task<GdprDataExport> ExportUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .Include(u => u.Workflows)
            .Include(u => u.WorkflowExecutions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User {userId} not found");
        }

        // 解密所有敏感數據
        var credentials = await dbContext.UserCredentials
            .Where(uc => uc.UserId == userId)
            .ToListAsync(cancellationToken);

        var decryptedCredentials = new List<CredentialExport>();
        foreach (var credential in credentials)
        {
            var plainValue = await encryptionService.DecryptAsync(credential.EncryptedValue, cancellationToken);
            decryptedCredentials.Add(new CredentialExport
            {
                Type = credential.CredentialType,
                Value = plainValue, // 僅用於導出,不存儲
                CreatedAt = credential.CreatedAt
            });
        }

        return new GdprDataExport
        {
            User = new UserExport
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                CreatedAt = user.CreatedAt
            },
            Workflows = user.Workflows.Select(w => new WorkflowExport
            {
                Id = w.Id,
                Name = w.Name,
                CreatedAt = w.CreatedAt
            }).ToList(),
            Credentials = decryptedCredentials,
            ExportDate = DateTime.UtcNow
        };
    }

    public async Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // GDPR Right to Erasure (Right to be Forgotten)
        logger.LogInformation("Deleting all data for user {UserId}", userId);

        var user = await dbContext.Users
            .Include(u => u.Workflows)
            .Include(u => u.Credentials)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return;
        }

        // 刪除所有相關數據
        dbContext.UserCredentials.RemoveRange(user.Credentials);
        dbContext.Workflows.RemoveRange(user.Workflows);
        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("All data deleted for user {UserId}", userId);
    }

    public async Task AnonymizeUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 匿名化用戶數據 (保留統計數據但移除 PII)
        var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            return;
        }

        user.Email = $"anonymized-{Guid.NewGuid()}@deleted.local";
        user.Name = "Anonymized User";
        user.PhoneNumber = null;
        user.IsActive = false;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} data anonymized", userId);
    }
}
```

---

## 最佳實踐

### ✅ Do's

1. **使用 TLS 1.3**: 所有傳輸層通信使用 TLS 1.3
2. **AES-256-GCM**: 使用 GCM 模式進行對稱加密
3. **Key Vault**: 所有密鑰存儲在 Azure Key Vault
4. **密鑰輪換**: 每 90 天輪換加密密鑰
5. **PII 加密**: 所有個人身份信息必須加密
6. **最小權限**: 只授予必要的 Key Vault 訪問權限
7. **審計日誌**: 記錄所有加密/解密操作

### ❌ Don'ts

1. **不要硬編碼密鑰**: 所有密鑰通過 Key Vault 管理
2. **不要使用 ECB 模式**: 使用 GCM 或 CBC 模式
3. **不要重用 IV**: 每次加密生成新的 IV
4. **不要在日誌中記錄明文密碼或密鑰**
5. **不要使用弱加密算法**: 避免 DES, 3DES, RC4
6. **不要在 Git 提交密鑰或敏感數據**

---

## 測試策略

### Unit Tests

```csharp
[Fact]
public async Task EncryptAsync_ValidPlainText_ReturnsEncryptedString()
{
    // Arrange
    var plainText = "sensitive-data";

    // Act
    var cipherText = await _encryptionService.EncryptAsync(plainText);

    // Assert
    Assert.NotNull(cipherText);
    Assert.NotEqual(plainText, cipherText);
}

[Fact]
public async Task DecryptAsync_ValidCipherText_ReturnsOriginalPlainText()
{
    // Arrange
    var plainText = "sensitive-data";
    var cipherText = await _encryptionService.EncryptAsync(plainText);

    // Act
    var decrypted = await _encryptionService.DecryptAsync(cipherText);

    // Assert
    Assert.Equal(plainText, decrypted);
}
```

---

## 故障排查

### 常見問題

#### 1. Key Vault 訪問失敗

**原因**: Managed Identity 權限不足

**解決方案**:
```bash
# 授予 App Service Managed Identity 訪問權限
az keyvault set-policy \
  --name aiworkflow-kv \
  --object-id <managed-identity-object-id> \
  --secret-permissions get list \
  --key-permissions get list encrypt decrypt
```

#### 2. 解密失敗: "Padding is invalid"

**原因**: 密鑰不匹配或數據損壞

**解決方案**:
- 檢查是否使用正確的密鑰
- 驗證數據完整性

---

## 實施檢查清單

- [ ] TLS 1.3 配置
- [ ] Security Headers 實現
- [ ] PostgreSQL TLS 連接
- [ ] Redis TLS 連接
- [ ] EncryptionService 實現
- [ ] Key Vault 集成
- [ ] 密鑰輪換策略
- [ ] PII 加密
- [ ] GDPR 合規功能
- [ ] 單元測試覆蓋 >80%

---

## 相關文檔

- [Phase 9: 認證實現](./authentication-implementation.md)
- [Phase 9: 授權與 RBAC](./authorization-rbac.md)
- [Phase 9: 安全測試自動化](./security-testing-automation.md)

## 參考資源

### 官方文檔
- [Azure Key Vault Documentation](https://learn.microsoft.com/en-us/azure/key-vault/)
- [.NET Cryptography](https://learn.microsoft.com/en-us/dotnet/standard/security/cryptography-model)
- [OWASP Cryptographic Storage](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

### 最佳實踐
- [NIST Cryptographic Standards](https://csrc.nist.gov/projects/cryptographic-standards-and-guidelines)
- [GDPR Encryption Requirements](https://gdpr.eu/encryption/)

---

**文檔維護**: Security Team
**最後更新**: 2025-11-02
**狀態**: ✅ 完整
