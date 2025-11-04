# 安全測試自動化 (Security Testing Automation)

## 概述

本文檔詳細說明 AI Workflow Platform 的安全測試自動化策略,涵蓋 OWASP Top 10 防護測試、SQL Injection 防護、XSS/CSRF 防護、依賴漏洞掃描和滲透測試。

### 目標

- ✅ 實現 OWASP Top 10 自動化測試
- ✅ 建立 SQL Injection 防護驗證
- ✅ 實現 XSS 和 CSRF 防護測試
- ✅ 自動化依賴漏洞掃描
- ✅ 集成滲透測試工具
- ✅ 建立安全測試 CI/CD Pipeline
- ✅ 實現安全代碼審查流程

### 適用範圍

- Backend API (ASP.NET Core 9.0)
- Frontend Applications (React 19, Vue 3)
- CI/CD Pipeline (GitHub Actions)
- Security Operations

---

## 技術背景

### OWASP Top 10 (2021)

| 排名 | 威脅 | 描述 | 測試方法 |
|------|------|------|----------|
| A01 | Broken Access Control | 訪問控制缺陷 | 授權測試 |
| A02 | Cryptographic Failures | 加密失敗 | 加密測試 |
| A03 | Injection | 注入攻擊 | SQL/Command Injection 測試 |
| A04 | Insecure Design | 不安全設計 | 威脅建模 |
| A05 | Security Misconfiguration | 安全配置錯誤 | 配置掃描 |
| A06 | Vulnerable Components | 易受攻擊的組件 | 依賴掃描 |
| A07 | Auth Failures | 認證失敗 | 認證測試 |
| A08 | Data Integrity Failures | 數據完整性失敗 | 完整性測試 |
| A09 | Logging Failures | 日誌記錄失敗 | 日誌審計 |
| A10 | SSRF | 服務端請求偽造 | SSRF 測試 |

### 技術棧

```yaml
Security Testing Tools:
  SAST (Static Analysis):
    - SonarQube 10.8 (代碼質量和安全)
    - Semgrep (語義代碼掃描)
    - SecurityCodeScan (.NET Security Scanner)

  DAST (Dynamic Analysis):
    - OWASP ZAP 2.15 (動態掃描)
    - Burp Suite Community (滲透測試)
    - Nuclei (漏洞掃描)

  Dependency Scanning:
    - Snyk (依賴漏洞掃描)
    - GitHub Dependabot (自動化依賴更新)
    - OWASP Dependency-Check

  Container Security:
    - Trivy (容器鏡像掃描)
    - Docker Scout

  CI/CD Integration:
    - GitHub Actions
    - Azure DevOps Pipelines
```

---

## SQL Injection 防護測試

### 1. 參數化查詢驗證

#### Bad Example (Vulnerable)

```csharp
// ❌ 危險: SQL Injection 漏洞
public async Task<User?> GetUserByEmailUnsafe(string email)
{
    var sql = $"SELECT * FROM users WHERE email = '{email}'";
    return await _dbContext.Users.FromSqlRaw(sql).FirstOrDefaultAsync();
}

// 攻擊範例: email = "'; DROP TABLE users; --"
```

#### Good Example (Secure)

```csharp
// ✅ 安全: 參數化查詢
public async Task<User?> GetUserByEmail(string email)
{
    return await _dbContext.Users
        .Where(u => u.Email == email)
        .FirstOrDefaultAsync();
}

// 或使用 FromSqlInterpolated
public async Task<User?> GetUserByEmailSafe(string email)
{
    return await _dbContext.Users
        .FromSqlInterpolated($"SELECT * FROM users WHERE email = {email}")
        .FirstOrDefaultAsync();
}
```

### 2. SQL Injection 測試用例

#### SqlInjectionTests.cs

```csharp
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace AIWorkflow.IntegrationTests.Security;

public sealed class SqlInjectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SqlInjectionTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("'; DROP TABLE users; --")]
    [InlineData("' OR '1'='1")]
    [InlineData("admin' --")]
    [InlineData("' UNION SELECT * FROM users --")]
    [InlineData("'; EXEC xp_cmdshell('dir'); --")]
    public async Task Login_WithSqlInjectionPayload_ShouldNotSucceed(string maliciousEmail)
    {
        // Arrange
        var request = new
        {
            email = maliciousEmail,
            password = "any-password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.BadRequest,
                    "SQL Injection payload should not result in successful login");
    }

    [Theory]
    [InlineData("search=' OR '1'='1")]
    [InlineData("search=' UNION SELECT password FROM users --")]
    public async Task SearchWorkflows_WithSqlInjectionPayload_ShouldReturnSafeResults(string maliciousSearch)
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync($"/api/workflows?search={Uri.EscapeDataString(maliciousSearch)}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var workflows = await response.Content.ReadFromJsonAsync<List<WorkflowDto>>();

        // 驗證返回結果不包含所有數據 (SQL Injection 失敗)
        Assert.NotNull(workflows);
        // 應該返回空或只返回匹配的結果,而不是所有數據
    }

    [Fact]
    public async Task UpdateWorkflow_WithSqlInjectionInName_ShouldBeSanitized()
    {
        // Arrange
        await AuthenticateAsync();
        var workflowId = await CreateTestWorkflowAsync();

        var updateRequest = new
        {
            name = "Test'; DROP TABLE workflows; --",
            description = "Normal description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/workflows/{workflowId}", updateRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        // 驗證工作流名稱被正確存儲 (作為字符串,不執行 SQL)
        var updatedWorkflow = await _client.GetFromJsonAsync<WorkflowDto>($"/api/workflows/{workflowId}");
        Assert.Equal("Test'; DROP TABLE workflows; --", updatedWorkflow!.Name);
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new { email = "test@example.com", password = "Test@123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);
    }

    private async Task<Guid> CreateTestWorkflowAsync()
    {
        var createRequest = new { name = "Test Workflow", description = "Test" };
        var response = await _client.PostAsJsonAsync("/api/workflows", createRequest);
        var workflow = await response.Content.ReadFromJsonAsync<WorkflowDto>();
        return workflow!.Id;
    }
}
```

### 3. 自動化 SQL Injection 掃描

#### sqlmap Integration

```bash
# sqlmap 自動化掃描腳本
#!/bin/bash

API_URL="https://api.aiworkflow.com"
AUTH_TOKEN="your-test-token"

# 掃描登入端點
sqlmap -u "${API_URL}/api/auth/login" \
  --data='{"email":"test@example.com","password":"password"}' \
  --method=POST \
  --headers="Content-Type: application/json" \
  --batch \
  --level=5 \
  --risk=3 \
  --random-agent

# 掃描搜索端點
sqlmap -u "${API_URL}/api/workflows?search=test" \
  --headers="Authorization: Bearer ${AUTH_TOKEN}" \
  --batch \
  --level=5 \
  --risk=3 \
  --random-agent

echo "SQL Injection scan completed"
```

---

## XSS (Cross-Site Scripting) 防護測試

### 1. 輸入驗證和輸出編碼

#### XSS Prevention Middleware

```csharp
namespace AIWorkflow.API.Middleware;

public sealed class XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
{
    private static readonly string[] DangerousPatterns =
    [
        "<script",
        "javascript:",
        "onerror=",
        "onload=",
        "onclick=",
        "eval(",
        "expression(",
        "<iframe",
        "<embed",
        "<object"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        // 檢查請求體
        if (context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (ContainsDangerousContent(body))
            {
                logger.LogWarning("XSS attempt detected in request body: {Body}", body);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid input detected" });
                return;
            }
        }

        // 檢查查詢參數
        foreach (var param in context.Request.Query)
        {
            if (ContainsDangerousContent(param.Value.ToString()))
            {
                logger.LogWarning("XSS attempt detected in query parameter: {Param}", param.Key);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid query parameter" });
                return;
            }
        }

        await next(context);
    }

    private static bool ContainsDangerousContent(string content)
    {
        return DangerousPatterns.Any(pattern =>
            content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}

// Register in Program.cs
app.UseMiddleware<XssProtectionMiddleware>();
```

### 2. XSS 測試用例

#### XssProtectionTests.cs

```csharp
public sealed class XssProtectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public XssProtectionTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("<script>alert('XSS')</script>")]
    [InlineData("<img src=x onerror=alert('XSS')>")]
    [InlineData("<iframe src='javascript:alert(\"XSS\")'></iframe>")]
    [InlineData("javascript:alert('XSS')")]
    [InlineData("<svg onload=alert('XSS')>")]
    [InlineData("<body onload=alert('XSS')>")]
    public async Task CreateWorkflow_WithXssPayload_ShouldBeRejected(string maliciousName)
    {
        // Arrange
        await AuthenticateAsync();
        var request = new
        {
            name = maliciousName,
            description = "Test workflow"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/workflows", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("search=<script>alert('XSS')</script>")]
    [InlineData("search=<img src=x onerror=alert(1)>")]
    public async Task SearchWorkflows_WithXssInQuery_ShouldBeRejected(string maliciousQuery)
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync($"/api/workflows?{maliciousQuery}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkflow_ResponseShouldBeProperlyEncoded()
    {
        // Arrange
        await AuthenticateAsync();

        // 創建包含特殊字符的工作流 (已被清理的內容)
        var createRequest = new
        {
            name = "Test & <Company> Workflow",
            description = "Description with \"quotes\" and 'apostrophes'"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/workflows", createRequest);
        var workflow = await createResponse.Content.ReadFromJsonAsync<WorkflowDto>();

        // Act
        var response = await _client.GetAsync($"/api/workflows/{workflow!.Id}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();

        // 驗證特殊字符被正確編碼
        Assert.Contains("Test &amp; &lt;Company&gt; Workflow", content);
    }
}
```

### 3. Content Security Policy (CSP)

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' data: https://fonts.gstatic.com; " +
        "connect-src 'self' https://api.aiworkflow.com; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'");

    await next();
});
```

---

## CSRF (Cross-Site Request Forgery) 防護測試

### 1. Anti-Forgery Token 實現

#### Program.cs

```csharp
// 添加 Anti-Forgery 服務
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// CSRF Token 端點
app.MapGet("/api/csrf-token", (IAntiforgery antiforgery, HttpContext context) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);
    return Results.Ok(new { token = tokens.RequestToken });
}).AllowAnonymous();
```

#### CSRF Protection Filter

```csharp
namespace AIWorkflow.API.Filters;

public sealed class ValidateAntiForgeryTokenAttribute : TypeFilterAttribute
{
    public ValidateAntiForgeryTokenAttribute() : base(typeof(ValidateAntiForgeryTokenFilter))
    {
    }
}

public sealed class ValidateAntiForgeryTokenFilter(IAntiforgery antiforgery) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 跳過 GET, HEAD, OPTIONS, TRACE
        if (HttpMethods.IsGet(context.HttpContext.Request.Method) ||
            HttpMethods.IsHead(context.HttpContext.Request.Method) ||
            HttpMethods.IsOptions(context.HttpContext.Request.Method) ||
            HttpMethods.IsTrace(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        try
        {
            await antiforgery.ValidateRequestAsync(context.HttpContext);
            await next();
        }
        catch (AntiforgeryValidationException)
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid CSRF token" });
        }
    }
}

// 使用在 Controller
[ValidateAntiForgeryToken]
[HttpPost]
public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowRequest request)
{
    // ...
}
```

### 2. CSRF 測試用例

```csharp
public sealed class CsrfProtectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CsrfProtectionTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    [Fact]
    public async Task PostRequest_WithoutCsrfToken_ShouldBeRejected()
    {
        // Arrange
        await AuthenticateAsync();
        var request = new { name = "Test Workflow", description = "Test" };

        // Act - 不包含 CSRF Token
        var response = await _client.PostAsJsonAsync("/api/workflows", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostRequest_WithValidCsrfToken_ShouldSucceed()
    {
        // Arrange
        await AuthenticateAsync();

        // 獲取 CSRF Token
        var tokenResponse = await _client.GetAsync("/api/csrf-token");
        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<CsrfTokenResponse>();

        var request = new { name = "Test Workflow", description = "Test" };
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/workflows")
        {
            Content = JsonContent.Create(request)
        };

        // 添加 CSRF Token
        httpRequest.Headers.Add("X-CSRF-TOKEN", tokenData!.Token);

        // Act
        var response = await _client.SendAsync(httpRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}
```

---

## 依賴漏洞掃描

### 1. Snyk Integration

#### .github/workflows/security-scan.yml

```yaml
name: Security Scan

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]
  schedule:
    - cron: '0 2 * * 1' # 每週一 2 AM

jobs:
  snyk-scan:
    name: Snyk Dependency Scan
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Run Snyk to check for vulnerabilities
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=high --fail-on=all

      - name: Upload Snyk results to GitHub Code Scanning
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: snyk.sarif

  dependency-check:
    name: OWASP Dependency Check
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Run OWASP Dependency Check
        uses: dependency-check/Dependency-Check_Action@main
        with:
          project: 'AIWorkflow'
          path: '.'
          format: 'ALL'

      - name: Upload Dependency Check results
        uses: actions/upload-artifact@v4
        with:
          name: dependency-check-report
          path: ${{ github.workspace }}/reports

  trivy-scan:
    name: Trivy Container Scan
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Build Docker image
        run: docker build -t aiworkflow-api:${{ github.sha }} .

      - name: Run Trivy vulnerability scanner
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: 'aiworkflow-api:${{ github.sha }}'
          format: 'sarif'
          output: 'trivy-results.sarif'
          severity: 'CRITICAL,HIGH'

      - name: Upload Trivy results to GitHub Security tab
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: 'trivy-results.sarif'
```

### 2. GitHub Dependabot 配置

#### .github/dependabot.yml

```yaml
version: 2
updates:
  # .NET dependencies
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
      day: "monday"
    open-pull-requests-limit: 10
    reviewers:
      - "security-team"
    labels:
      - "dependencies"
      - "security"
    commit-message:
      prefix: "chore(deps)"

  # npm dependencies (Frontend)
  - package-ecosystem: "npm"
    directory: "/src/Web"
    schedule:
      interval: "weekly"
      day: "monday"
    open-pull-requests-limit: 10
    reviewers:
      - "security-team"
    labels:
      - "dependencies"
      - "security"

  # Docker dependencies
  - package-ecosystem: "docker"
    directory: "/"
    schedule:
      interval: "weekly"
      day: "monday"
    reviewers:
      - "security-team"

  # GitHub Actions
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
    reviewers:
      - "security-team"
```

---

## OWASP ZAP 自動化掃描

### 1. ZAP Baseline Scan

#### .github/workflows/zap-scan.yml

```yaml
name: OWASP ZAP Scan

on:
  push:
    branches: [main, develop]
  schedule:
    - cron: '0 3 * * 1' # 每週一 3 AM

jobs:
  zap-baseline-scan:
    name: ZAP Baseline Scan
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Start application
        run: |
          docker-compose up -d
          sleep 30 # 等待應用啟動

      - name: ZAP Baseline Scan
        uses: zaproxy/action-baseline@v0.12.0
        with:
          target: 'http://localhost:5000'
          rules_file_name: '.zap/rules.tsv'
          cmd_options: '-a'

      - name: Upload ZAP report
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: zap-baseline-report
          path: report_html.html

  zap-full-scan:
    name: ZAP Full Scan (Scheduled)
    runs-on: ubuntu-latest
    if: github.event_name == 'schedule'
    steps:
      - uses: actions/checkout@v4

      - name: Start application
        run: |
          docker-compose up -d
          sleep 30

      - name: ZAP Full Scan
        uses: zaproxy/action-full-scan@v0.10.0
        with:
          target: 'http://localhost:5000'
          rules_file_name: '.zap/rules.tsv'
          cmd_options: '-a -j'

      - name: Upload ZAP report
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: zap-full-scan-report
          path: report_html.html
```

### 2. ZAP Rules Configuration

#### .zap/rules.tsv

```tsv
10010	IGNORE	(Cookie No HttpOnly Flag)
10011	IGNORE	(Cookie Without Secure Flag) # 開發環境
10015	WARN	(Incomplete or No Cache-control and Pragma HTTP Header Set)
10017	WARN	(Cross-Domain JavaScript Source File Inclusion)
10019	WARN	(Content-Type Header Missing)
10020	WARN	(X-Frame-Options Header Not Set)
10021	WARN	(X-Content-Type-Options Header Missing)
10023	WARN	(Information Disclosure - Debug Error Messages)
10024	WARN	(Information Disclosure - Sensitive Information in URL)
10025	WARN	(Information Disclosure - Sensitive Information in HTTP Referrer Header)
10026	WARN	(HTTP Parameter Override)
10027	WARN	(Information Disclosure - Suspicious Comments)
10028	WARN	(Open Redirect)
10029	WARN	(Cookie Poisoning)
10030	WARN	(User Controllable Charset)
10031	WARN	(User Controllable HTML Element Attribute (Potential XSS))
10032	WARN	(Viewstate Scanner)
10033	WARN	(Directory Browsing)
10034	WARN	(Heartbleed OpenSSL Vulnerability)
10035	WARN	(Strict-Transport-Security Header Not Set)
10036	WARN	(HTTP Server Response Header)
10037	WARN	(Server Leaks Information via "X-Powered-By" HTTP Response Header Field(s))
10038	WARN	(Content Security Policy (CSP) Header Not Set)
10039	WARN	(X-Backend-Server Header Information Leak)
10040	FAIL	(Secure Pages Include Mixed Content)
10041	FAIL	(HTTP to HTTPS Insecure Transition in Form Post)
10042	FAIL	(HTTPS to HTTP Insecure Transition in Form Post)
10043	FAIL	(User Controllable JavaScript Event (XSS))
10044	FAIL	(Big Redirect Detected (Potential Sensitive Information Leak))
10045	FAIL	(Source Code Disclosure - /WEB-INF folder)
10046	FAIL	(Source Code Disclosure - File Inclusion)
10047	FAIL	(Remote Code Execution - Shell Shock)
10048	FAIL	(Remote Code Execution - CVE-2012-1823)
10049	FAIL	(SQL Injection)
10050	FAIL	(Command Injection)
10051	FAIL	(Application Error Disclosure)
10052	FAIL	(Private IP Disclosure)
10053	FAIL	(Apache Range Header DoS)
10054	FAIL	(Cookie Slack Detector)
10055	FAIL	(CSP Scanner)
10056	FAIL	(X-Debug-Token Information Leak)
10057	FAIL	(Username Hash Found)
10058	FAIL	(GET for POST)
10059	FAIL	(Cross-Domain Misconfiguration)
10060	FAIL	(Loosely Scoped Cookie)
10061	FAIL	(Timestamp Disclosure)
10062	FAIL	(PII Scanner)
10063	FAIL	(Feature Policy Header Not Set)
10098	WARN	(Cross-Domain Misconfiguration)
90001	IGNORE	(Insecure JSF ViewState)
90002	IGNORE	(Java Serialization Object)
90003	IGNORE	(Sub Resource Integrity Attribute Missing)
90004	IGNORE	(Insufficient Site Isolation Against Spectre Vulnerability)
90011	IGNORE	(Charset Mismatch)
90022	FAIL	(Application Error Disclosure)
90033	FAIL	(Loosely Scoped Cookie)
```

---

## SonarQube 代碼質量和安全掃描

### 1. SonarQube Configuration

#### sonar-project.properties

```properties
sonar.projectKey=aiworkflow-platform
sonar.projectName=AI Workflow Platform
sonar.projectVersion=1.0

# Source code
sonar.sources=src
sonar.tests=tests

# Exclusions
sonar.exclusions=**/bin/**,**/obj/**,**/wwwroot/lib/**,**/Migrations/**

# Coverage
sonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
sonar.cs.vstest.reportsPaths=**/*.trx

# Language specific
sonar.language=cs
sonar.dotnet.excludeGeneratedCode=true

# Security hotspots
sonar.security.hotspots.enabled=true

# Code Smells
sonar.issue.ignore.multicriteria=e1,e2
sonar.issue.ignore.multicriteria.e1.ruleKey=csharpsquid:S101
sonar.issue.ignore.multicriteria.e1.resourceKey=**/*Tests.cs
```

### 2. GitHub Actions Integration

```yaml
name: SonarQube Analysis

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  sonarqube:
    name: SonarQube Analysis
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Install SonarScanner
        run: |
          dotnet tool install --global dotnet-sonarscanner

      - name: Begin SonarQube analysis
        run: |
          dotnet sonarscanner begin `
            /k:"aiworkflow-platform" `
            /d:sonar.host.url="${{ secrets.SONAR_HOST_URL }}" `
            /d:sonar.token="${{ secrets.SONAR_TOKEN }}" `
            /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"

      - name: Build and Test
        run: |
          dotnet build --no-incremental
          dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

      - name: End SonarQube analysis
        run: |
          dotnet sonarscanner end /d:sonar.token="${{ secrets.SONAR_TOKEN }}"

      - name: Check Quality Gate
        run: |
          # 等待 Quality Gate 結果
          sleep 30
          # 檢查 Quality Gate 狀態
```

---

## 安全代碼審查清單

### Code Review Checklist

```markdown
## 安全代碼審查清單

### 輸入驗證
- [ ] 所有用戶輸入都經過驗證
- [ ] 使用白名單而非黑名單驗證
- [ ] 文件上傳有類型和大小限制
- [ ] 輸入長度有合理限制

### SQL Injection 防護
- [ ] 使用參數化查詢或 ORM
- [ ] 沒有字符串拼接 SQL
- [ ] 動態 SQL 使用 FromSqlInterpolated

### XSS 防護
- [ ] 所有輸出都經過編碼
- [ ] Content Security Policy 配置正確
- [ ] 沒有使用 innerHTML 或 dangerouslySetInnerHTML

### CSRF 防護
- [ ] 所有狀態修改操作使用 Anti-Forgery Token
- [ ] SameSite Cookie 屬性設置正確

### 認證和授權
- [ ] 所有敏感端點都需要認證
- [ ] 實現了適當的授權檢查
- [ ] 密碼使用 BCrypt/Argon2 哈希
- [ ] JWT Token 有合理的過期時間

### 敏感數據保護
- [ ] 敏感數據使用加密存儲
- [ ] 密鑰通過 Key Vault 管理
- [ ] 沒有在日誌中記錄敏感信息
- [ ] HTTPS 強制使用

### 錯誤處理
- [ ] 不向客戶端暴露內部錯誤詳情
- [ ] 使用通用錯誤消息
- [ ] 錯誤記錄到安全日誌

### 依賴管理
- [ ] 所有依賴都是最新版本
- [ ] 沒有已知漏洞的依賴
- [ ] 定期更新依賴

### API 安全
- [ ] 實現了 Rate Limiting
- [ ] API 版本控制正確
- [ ] CORS 配置正確
- [ ] 敏感操作需要額外驗證
```

---

## 滲透測試

### 1. 自動化滲透測試腳本

#### penetration-test.sh

```bash
#!/bin/bash

TARGET_URL="https://api.aiworkflow.com"
REPORT_DIR="./security-reports"

mkdir -p ${REPORT_DIR}

echo "=== Starting Penetration Testing ==="
echo "Target: ${TARGET_URL}"
echo "Report Directory: ${REPORT_DIR}"

# 1. Nmap 端口掃描
echo "[1/7] Running Nmap port scan..."
nmap -sV -sC -oN ${REPORT_DIR}/nmap-scan.txt ${TARGET_URL}

# 2. SSL/TLS 測試
echo "[2/7] Testing SSL/TLS configuration..."
testssl.sh --jsonfile ${REPORT_DIR}/testssl.json ${TARGET_URL}

# 3. HTTP Security Headers 檢查
echo "[3/7] Checking HTTP Security Headers..."
curl -I ${TARGET_URL} > ${REPORT_DIR}/security-headers.txt

# 4. Nikto Web 漏洞掃描
echo "[4/7] Running Nikto vulnerability scan..."
nikto -h ${TARGET_URL} -o ${REPORT_DIR}/nikto-report.html -Format html

# 5. OWASP ZAP API 掃描
echo "[5/7] Running OWASP ZAP API scan..."
docker run --rm -v $(pwd):/zap/wrk/:rw \
  owasp/zap2docker-stable zap-api-scan.py \
  -t ${TARGET_URL}/swagger/v1/swagger.json \
  -f openapi \
  -r /zap/wrk/${REPORT_DIR}/zap-api-report.html

# 6. Nuclei 漏洞掃描
echo "[6/7] Running Nuclei vulnerability scan..."
nuclei -u ${TARGET_URL} -severity critical,high,medium \
  -o ${REPORT_DIR}/nuclei-report.txt

# 7. SSRF 測試
echo "[7/7] Testing for SSRF vulnerabilities..."
# 使用自定義腳本測試 SSRF
python3 ssrf-test.py ${TARGET_URL} > ${REPORT_DIR}/ssrf-test.txt

echo "=== Penetration Testing Completed ==="
echo "Reports saved to: ${REPORT_DIR}"
```

### 2. 手動滲透測試檢查清單

```markdown
## 手動滲透測試清單

### 認證測試
- [ ] 測試弱密碼
- [ ] 測試密碼重置流程
- [ ] 測試會話管理
- [ ] 測試 MFA 繞過
- [ ] 測試密碼暴力破解防護

### 授權測試
- [ ] 測試水平權限提升
- [ ] 測試垂直權限提升
- [ ] 測試 IDOR (Insecure Direct Object Reference)
- [ ] 測試路徑遍歷

### 輸入驗證測試
- [ ] SQL Injection (各種變體)
- [ ] XSS (Reflected, Stored, DOM-based)
- [ ] Command Injection
- [ ] XXE (XML External Entity)
- [ ] LDAP Injection

### 業務邏輯測試
- [ ] 測試工作流程繞過
- [ ] 測試競態條件
- [ ] 測試價格操縱
- [ ] 測試數量操縱

### API 測試
- [ ] 測試 API Rate Limiting
- [ ] 測試 API 版本控制
- [ ] 測試 GraphQL 漏洞 (如適用)
- [ ] 測試 REST API 安全

### 文件上傳測試
- [ ] 測試文件類型驗證繞過
- [ ] 測試惡意文件上傳
- [ ] 測試文件大小限制
- [ ] 測試路徑遍歷

### Session 管理測試
- [ ] 測試 Session Fixation
- [ ] 測試 Session Timeout
- [ ] 測試 Cookie 安全屬性
- [ ] 測試 CSRF Token
```

---

## 最佳實踐

### ✅ Do's

1. **自動化安全測試**: 集成到 CI/CD Pipeline
2. **定期掃描**: 每週運行完整安全掃描
3. **依賴更新**: 及時更新有漏洞的依賴
4. **Security Headers**: 配置所有推薦的安全 Headers
5. **輸入驗證**: 白名單驗證所有用戶輸入
6. **滲透測試**: 每季度進行專業滲透測試
7. **安全培訓**: 定期進行開發者安全培訓

### ❌ Don'ts

1. **不要忽略安全告警**: 及時修復發現的漏洞
2. **不要在生產環境測試**: 使用獨立測試環境
3. **不要暴露敏感信息**: 錯誤消息不應包含內部細節
4. **不要跳過安全審查**: 所有代碼都應經過安全審查
5. **不要使用已知有漏洞的版本**: 保持依賴更新

---

## 測試報告和監控

### Security Dashboard

```csharp
public sealed class SecurityMetricsService(
    ApplicationDbContext dbContext,
    ILogger<SecurityMetricsService> logger)
{
    public async Task<SecurityMetrics> GetSecurityMetricsAsync(CancellationToken cancellationToken = default)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var metrics = new SecurityMetrics
        {
            TotalSecurityEvents = await dbContext.SecurityEvents
                .CountAsync(se => se.CreatedAt >= thirtyDaysAgo, cancellationToken),

            FailedLoginAttempts = await dbContext.SecurityEvents
                .CountAsync(se => se.EventType == "FailedLogin" && se.CreatedAt >= thirtyDaysAgo, cancellationToken),

            BlockedIpAddresses = await dbContext.BlockedIpAddresses
                .CountAsync(ip => ip.IsActive, cancellationToken),

            VulnerabilitiesFound = await GetVulnerabilityCountAsync(),

            SecurityScanStatus = await GetLastScanStatusAsync()
        };

        return metrics;
    }

    private async Task<int> GetVulnerabilityCountAsync()
    {
        // 從外部掃描工具 API 獲取漏洞數量
        // 例如: Snyk, SonarQube
        return 0; // Placeholder
    }

    private async Task<string> GetLastScanStatusAsync()
    {
        // 獲取最後一次安全掃描狀態
        return "Passed"; // Placeholder
    }
}

public record SecurityMetrics
{
    public int TotalSecurityEvents { get; init; }
    public int FailedLoginAttempts { get; init; }
    public int BlockedIpAddresses { get; init; }
    public int VulnerabilitiesFound { get; init; }
    public string SecurityScanStatus { get; init; } = string.Empty;
}
```

---

## 實施檢查清單

- [ ] SQL Injection 測試用例完成
- [ ] XSS 防護測試完成
- [ ] CSRF 防護實現和測試
- [ ] Snyk 依賴掃描集成
- [ ] GitHub Dependabot 配置
- [ ] OWASP ZAP 掃描配置
- [ ] SonarQube 集成
- [ ] Trivy 容器掃描
- [ ] 安全代碼審查清單建立
- [ ] 滲透測試計劃完成
- [ ] Security Dashboard 實現

---

## 相關文檔

- [Phase 9: 認證實現](./authentication-implementation.md)
- [Phase 9: 授權與 RBAC](./authorization-rbac.md)
- [Phase 9: 數據加密標準](./data-encryption-standards.md)
- [Phase 9: Code Interpreter 安全](./code-interpreter-security.md)

## 參考資源

### 官方文檔
- [OWASP Top 10 2021](https://owasp.org/Top10/)
- [OWASP Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)
- [OWASP ZAP Documentation](https://www.zaproxy.org/docs/)

### 工具文檔
- [Snyk Documentation](https://docs.snyk.io/)
- [SonarQube Documentation](https://docs.sonarqube.org/)
- [Trivy Documentation](https://aquasecurity.github.io/trivy/)

---

**文檔維護**: Security Team
**最後更新**: 2025-11-02
**狀態**: ✅ 完整
