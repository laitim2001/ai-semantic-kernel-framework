# Code Interpreter 安全架構 (Code Interpreter Security)

## 概述

本文檔詳細說明 AI Workflow Platform 的 Code Interpreter 4 層安全架構,涵蓋代碼驗證、沙箱執行、資源限制和輸出清理,確保用戶提交的代碼在安全環境中執行。

### 目標

- ✅ 實現 4 層安全防護架構
- ✅ 防止惡意代碼執行和系統入侵
- ✅ 限制資源使用 (CPU、Memory、Disk、Network)
- ✅ 保護敏感數據和系統安全
- ✅ 提供安全的 Python 代碼執行環境
- ✅ 實現完整的審計和監控機制

### 適用範圍

- Code Interpreter Workflow Node
- Python Code Execution Service
- Docker Container Sandbox
- Kubernetes Resource Management

---

## 技術背景

### 安全威脅模型

| 威脅類別 | 描述 | 防護層 |
|----------|------|--------|
| **代碼注入** | 執行惡意 Python 代碼 | Layer 1: 代碼驗證 |
| **系統訪問** | 訪問主機文件系統、網絡 | Layer 2: 沙箱隔離 |
| **資源耗盡** | DoS 攻擊、無限循環 | Layer 3: 資源限制 |
| **數據泄露** | 輸出敏感信息 | Layer 4: 輸出清理 |
| **網絡攻擊** | 對外發起攻擊 | 網絡隔離 |
| **權限提升** | 嘗試獲取 root 權限 | 非特權容器 |

### 技術棧

```yaml
Execution Environment:
  Container: Docker 24+ / Kubernetes Pods
  Base Image: python:3.11-slim-bookworm
  Runtime: Python 3.11 (RestrictedPython)

Security Layers:
  Layer 1 - Code Validation:
    - AST (Abstract Syntax Tree) Analysis
    - Dangerous Function Blacklist
    - Import Statement Whitelist

  Layer 2 - Sandbox Execution:
    - Docker Container Isolation
    - gVisor Runtime (Optional)
    - AppArmor / SELinux Profiles

  Layer 3 - Resource Limitation:
    - CPU Quota (0.5 CPU)
    - Memory Limit (512 MB)
    - Execution Timeout (30 seconds)
    - Disk I/O Limit
    - Network Bandwidth Limit

  Layer 4 - Output Sanitization:
    - Output Size Limit (1 MB)
    - PII Detection and Redaction
    - Malicious Content Filtering

Monitoring:
  - Prometheus Metrics
  - ELK Stack Logging
  - Azure Application Insights
```

---

## 4 層安全架構

### 架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│                         User Input (Python Code)                 │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  Layer 1: Code Validation                                        │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ • AST Parsing and Analysis                                │  │
│  │ • Blacklist: os, subprocess, eval, exec, open, __import__│  │
│  │ • Whitelist: math, json, datetime, numpy, pandas         │  │
│  │ • Syntax and Semantic Validation                          │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼ (Valid Code)
┌─────────────────────────────────────────────────────────────────┐
│  Layer 2: Sandbox Execution                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Docker Container:                                         │  │
│  │  • Isolated Network (No Internet)                        │  │
│  │  • Read-only Root Filesystem                             │  │
│  │  • tmpfs for /tmp (In-Memory)                            │  │
│  │  • Non-root User (uid 1000)                              │  │
│  │  • No Privileged Mode                                    │  │
│  │  • AppArmor/SELinux Profile                              │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼ (Execution)
┌─────────────────────────────────────────────────────────────────┐
│  Layer 3: Resource Limitation                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ • CPU: 0.5 vCPU (--cpus=0.5)                            │  │
│  │ • Memory: 512 MB (--memory=512m)                         │  │
│  │ • Timeout: 30 seconds (kill after)                       │  │
│  │ • Disk Write: 100 MB (tmpfs size)                        │  │
│  │ • Processes: Max 50 (--pids-limit=50)                    │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼ (Output)
┌─────────────────────────────────────────────────────────────────┐
│  Layer 4: Output Sanitization                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ • Output Size: Max 1 MB                                   │  │
│  │ • PII Redaction: Email, Phone, SSN, Credit Card          │  │
│  │ • Content Filtering: Remove HTML/JS                      │  │
│  │ • Error Sanitization: Hide system paths                  │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
                          ┌───────────────┐
                          │ Safe Output   │
                          └───────────────┘
```

---

## Layer 1: 代碼驗證 (Code Validation)

### 1. AST 分析

#### CodeValidator.cs

```csharp
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AIWorkflow.Infrastructure.Services.CodeInterpreter;

public interface ICodeValidator
{
    Task<CodeValidationResult> ValidateAsync(string code, CancellationToken cancellationToken = default);
}

public sealed class CodeValidator(ILogger<CodeValidator> logger) : ICodeValidator
{
    // 危險函數黑名單
    private static readonly HashSet<string> DangerousFunctions =
    [
        "eval", "exec", "compile", "__import__", "open", "input",
        "os.system", "os.popen", "os.exec", "subprocess",
        "socket", "urllib", "requests", "http",
        "pickle", "shelve", "marshal", "ctypes",
        "__builtins__", "globals", "locals", "vars",
        "dir", "getattr", "setattr", "delattr",
        "file", "execfile", "reload"
    ];

    // 允許的庫白名單
    private static readonly HashSet<string> AllowedLibraries =
    [
        "math", "json", "datetime", "time", "random",
        "collections", "itertools", "functools",
        "re", "string", "textwrap",
        "decimal", "fractions", "statistics",
        "numpy", "pandas", "matplotlib", "seaborn",
        "scipy", "sklearn"
    ];

    public async Task<CodeValidationResult> ValidateAsync(string code, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // 1. 基本檢查
            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add("Code cannot be empty");
                return new CodeValidationResult(false, errors, warnings);
            }

            if (code.Length > 10_000)
            {
                errors.Add("Code exceeds maximum length of 10,000 characters");
                return new CodeValidationResult(false, errors, warnings);
            }

            // 2. 語法檢查 (使用 Python AST)
            var syntaxCheckResult = await ValidateSyntaxAsync(code, cancellationToken);
            if (!syntaxCheckResult.IsValid)
            {
                errors.AddRange(syntaxCheckResult.Errors);
                return new CodeValidationResult(false, errors, warnings);
            }

            // 3. 危險函數檢查
            foreach (var dangerousFunc in DangerousFunctions)
            {
                if (code.Contains(dangerousFunc, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"Dangerous function detected: {dangerousFunc}");
                }
            }

            // 4. Import 語句檢查
            var importStatements = ExtractImportStatements(code);
            foreach (var importStmt in importStatements)
            {
                if (!IsAllowedImport(importStmt))
                {
                    errors.Add($"Disallowed import: {importStmt}");
                }
            }

            // 5. 文件操作檢查
            if (Regex.IsMatch(code, @"\b(open|read|write|delete|remove|unlink)\b", RegexOptions.IgnoreCase))
            {
                warnings.Add("File operations detected - will be restricted in sandbox");
            }

            // 6. 網絡操作檢查
            if (Regex.IsMatch(code, @"\b(socket|urllib|requests|http|ftp)\b", RegexOptions.IgnoreCase))
            {
                errors.Add("Network operations are not allowed");
            }

            // 7. 系統命令檢查
            if (Regex.IsMatch(code, @"\b(system|popen|subprocess|exec|eval)\b", RegexOptions.IgnoreCase))
            {
                errors.Add("System command execution is not allowed");
            }

            var isValid = errors.Count == 0;

            if (isValid)
            {
                logger.LogInformation("Code validation passed with {WarningCount} warnings", warnings.Count);
            }
            else
            {
                logger.LogWarning("Code validation failed with {ErrorCount} errors", errors.Count);
            }

            return new CodeValidationResult(isValid, errors, warnings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Code validation error");
            errors.Add($"Validation error: {ex.Message}");
            return new CodeValidationResult(false, errors, warnings);
        }
    }

    private async Task<CodeValidationResult> ValidateSyntaxAsync(string code, CancellationToken cancellationToken)
    {
        try
        {
            // 使用 Python 的 ast.parse() 驗證語法
            var tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFile, code, cancellationToken);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = $"-m py_compile {tempFile}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync(cancellationToken);

            var error = await process.StandardError.ReadToEndAsync(cancellationToken);

            File.Delete(tempFile);

            if (process.ExitCode != 0)
            {
                return new CodeValidationResult(false, [error], []);
            }

            return new CodeValidationResult(true, [], []);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Syntax validation error");
            return new CodeValidationResult(false, [$"Syntax validation error: {ex.Message}"], []);
        }
    }

    private static List<string> ExtractImportStatements(string code)
    {
        var imports = new List<string>();
        var lines = code.Split('\n');

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("import ") || trimmedLine.StartsWith("from "))
            {
                imports.Add(trimmedLine);
            }
        }

        return imports;
    }

    private static bool IsAllowedImport(string importStatement)
    {
        foreach (var allowedLib in AllowedLibraries)
        {
            if (importStatement.Contains(allowedLib, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

public record CodeValidationResult(
    bool IsValid,
    List<string> Errors,
    List<string> Warnings);
```

---

## Layer 2: 沙箱執行 (Sandbox Execution)

### 1. Docker 沙箱配置

#### Dockerfile.sandbox

```dockerfile
# Code Interpreter Sandbox Image
FROM python:3.11-slim-bookworm

# 創建非特權用戶
RUN useradd -m -u 1000 -s /bin/bash sandbox

# 安裝允許的庫
RUN pip install --no-cache-dir \
    numpy==1.26.4 \
    pandas==2.2.2 \
    matplotlib==3.9.0 \
    seaborn==0.13.2 \
    scipy==1.13.1 \
    scikit-learn==1.5.0

# 設置工作目錄
WORKDIR /workspace

# 創建 tmpfs 掛載點 (內存文件系統)
RUN mkdir -p /workspace/tmp && chown sandbox:sandbox /workspace/tmp

# 切換到非特權用戶
USER sandbox

# 設置環境變數
ENV PYTHONUNBUFFERED=1
ENV PYTHONDONTWRITEBYTECODE=1

# 入口點
ENTRYPOINT ["python3"]
```

### 2. Docker 執行配置

#### DockerSandboxExecutor.cs

```csharp
using Docker.DotNet;
using Docker.DotNet.Models;

namespace AIWorkflow.Infrastructure.Services.CodeInterpreter;

public interface ICodeExecutor
{
    Task<CodeExecutionResult> ExecuteAsync(string code, Dictionary<string, object> inputs, CancellationToken cancellationToken = default);
}

public sealed class DockerSandboxExecutor(
    ILogger<DockerSandboxExecutor> logger) : ICodeExecutor
{
    private const string SandboxImage = "aiworkflow-code-sandbox:latest";
    private const int ExecutionTimeoutSeconds = 30;
    private const long MemoryLimitBytes = 512 * 1024 * 1024; // 512 MB
    private const long CpuQuota = 50_000; // 0.5 CPU (50% of 100,000)
    private const long CpuPeriod = 100_000;

    public async Task<CodeExecutionResult> ExecuteAsync(
        string code,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken = default)
    {
        using var client = new DockerClientConfiguration().CreateClient();

        var containerName = $"sandbox-{Guid.NewGuid():N}";

        try
        {
            // 1. 準備代碼文件
            var codeFile = PrepareCodeFile(code, inputs);

            // 2. 創建容器
            var containerId = await CreateSandboxContainerAsync(client, containerName, codeFile, cancellationToken);

            // 3. 啟動容器
            await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), cancellationToken);

            // 4. 等待執行完成 (帶超時)
            var completed = await WaitForCompletionAsync(client, containerId, ExecutionTimeoutSeconds, cancellationToken);

            if (!completed)
            {
                // 超時,強制停止
                await client.Containers.KillContainerAsync(containerId, new ContainerKillParameters(), cancellationToken);
                logger.LogWarning("Code execution timeout for container {ContainerId}", containerId);

                return new CodeExecutionResult(
                    false,
                    string.Empty,
                    "Execution timeout: exceeded 30 seconds",
                    ExecutionTimeoutSeconds,
                    0);
            }

            // 5. 獲取輸出和日誌
            var logs = await GetContainerLogsAsync(client, containerId, cancellationToken);

            // 6. 獲取退出代碼
            var exitCode = await GetContainerExitCodeAsync(client, containerId, cancellationToken);

            // 7. 清理容器
            await client.Containers.RemoveContainerAsync(
                containerId,
                new ContainerRemoveParameters { Force = true },
                cancellationToken);

            var success = exitCode == 0;
            var output = logs.stdout;
            var error = logs.stderr;

            logger.LogInformation(
                "Code execution completed: Success={Success}, ExitCode={ExitCode}, OutputLength={OutputLength}",
                success, exitCode, output.Length);

            return new CodeExecutionResult(success, output, error, ExecutionTimeoutSeconds, exitCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Code execution error in container {ContainerName}", containerName);

            // 清理容器 (如果存在)
            try
            {
                await client.Containers.RemoveContainerAsync(
                    containerName,
                    new ContainerRemoveParameters { Force = true },
                    cancellationToken);
            }
            catch
            {
                // Ignore cleanup errors
            }

            return new CodeExecutionResult(false, string.Empty, $"Execution error: {ex.Message}", 0, -1);
        }
    }

    private async Task<string> CreateSandboxContainerAsync(
        DockerClient client,
        string containerName,
        string codeFile,
        CancellationToken cancellationToken)
    {
        var hostConfig = new HostConfig
        {
            // 資源限制
            Memory = MemoryLimitBytes,
            MemorySwap = MemoryLimitBytes, // 禁用 swap
            CPUQuota = CpuQuota,
            CPUPeriod = CpuPeriod,
            PidsLimit = 50, // 最多 50 個進程

            // 網絡隔離
            NetworkMode = "none", // 無網絡訪問

            // 文件系統
            ReadonlyRootfs = true, // 只讀根文件系統
            Tmpfs = new Dictionary<string, string>
            {
                { "/tmp", "rw,noexec,nosuid,size=100m" }, // tmpfs (內存,100MB)
                { "/workspace/tmp", "rw,noexec,nosuid,size=100m" }
            },

            // 安全選項
            SecurityOpt = new List<string>
            {
                "no-new-privileges", // 禁止權限提升
                "apparmor=docker-default" // AppArmor profile
            },

            // 能力限制 (移除所有特權能力)
            CapDrop = new List<string> { "ALL" },

            // 日誌限制
            LogConfig = new LogConfig
            {
                Type = "json-file",
                Config = new Dictionary<string, string>
                {
                    { "max-size", "1m" },
                    { "max-file", "1" }
                }
            }
        };

        var config = new CreateContainerParameters
        {
            Name = containerName,
            Image = SandboxImage,
            Cmd = new[] { "-c", $"cat {codeFile} | python3" },
            HostConfig = hostConfig,
            User = "1000:1000", // 非 root 用戶
            WorkingDir = "/workspace",
            Env = new[]
            {
                "PYTHONUNBUFFERED=1",
                "PYTHONDONTWRITEBYTECODE=1"
            }
        };

        var response = await client.Containers.CreateContainerAsync(config, cancellationToken);

        return response.ID;
    }

    private static string PrepareCodeFile(string code, Dictionary<string, object> inputs)
    {
        // 將輸入參數注入代碼
        var inputsJson = System.Text.Json.JsonSerializer.Serialize(inputs);
        var wrappedCode = $"""
            import json
            import sys

            # Injected inputs
            inputs = json.loads('''{inputsJson}''')

            # User code
            try:
                {code}
            except Exception as e:
                print(f"Error: {{type(e).__name__}}: {{e}}", file=sys.stderr)
                sys.exit(1)
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, wrappedCode);

        return tempFile;
    }

    private async Task<bool> WaitForCompletionAsync(
        DockerClient client,
        string containerId,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            await client.Containers.WaitContainerAsync(containerId, cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private async Task<(string stdout, string stderr)> GetContainerLogsAsync(
        DockerClient client,
        string containerId,
        CancellationToken cancellationToken)
    {
        var logsParams = new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true
        };

        using var logsStream = await client.Containers.GetContainerLogsAsync(
            containerId,
            logsParams,
            cancellationToken);

        using var reader = new StreamReader(logsStream);
        var logs = await reader.ReadToEndAsync(cancellationToken);

        // 分離 stdout 和 stderr (簡化版本)
        var stdout = logs;
        var stderr = string.Empty;

        return (stdout, stderr);
    }

    private async Task<long> GetContainerExitCodeAsync(
        DockerClient client,
        string containerId,
        CancellationToken cancellationToken)
    {
        var inspectResponse = await client.Containers.InspectContainerAsync(containerId, cancellationToken);
        return inspectResponse.State.ExitCode;
    }
}

public record CodeExecutionResult(
    bool Success,
    string Output,
    string Error,
    int ExecutionTimeSeconds,
    long ExitCode);
```

---

## Layer 3: 資源限制 (Resource Limitation)

### Kubernetes Resource Quotas

#### code-interpreter-pod.yaml

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: code-interpreter-sandbox
  namespace: aiworkflow
  labels:
    app: code-interpreter
    security: sandbox
spec:
  # Security Context
  securityContext:
    runAsNonRoot: true
    runAsUser: 1000
    runAsGroup: 1000
    fsGroup: 1000
    seccompProfile:
      type: RuntimeDefault

  containers:
  - name: sandbox
    image: aiworkflow-code-sandbox:latest
    imagePullPolicy: Always

    # Resource Limits
    resources:
      limits:
        cpu: "500m"          # 0.5 CPU
        memory: "512Mi"      # 512 MB
        ephemeral-storage: "100Mi"
      requests:
        cpu: "250m"
        memory: "256Mi"
        ephemeral-storage: "50Mi"

    # Security Context (Container Level)
    securityContext:
      allowPrivilegeEscalation: false
      readOnlyRootFilesystem: true
      runAsNonRoot: true
      runAsUser: 1000
      capabilities:
        drop:
        - ALL

    # Volume Mounts (tmpfs only)
    volumeMounts:
    - name: tmp-volume
      mountPath: /tmp
    - name: workspace-volume
      mountPath: /workspace/tmp

    # Liveness and Readiness Probes
    livenessProbe:
      exec:
        command:
        - python3
        - --version
      initialDelaySeconds: 5
      periodSeconds: 10
      timeoutSeconds: 5

  # Volumes (tmpfs - in-memory)
  volumes:
  - name: tmp-volume
    emptyDir:
      medium: Memory
      sizeLimit: 50Mi
  - name: workspace-volume
    emptyDir:
      medium: Memory
      sizeLimit: 100Mi

  # Network Policy (Isolated)
  restartPolicy: Never
  dnsPolicy: None
  hostNetwork: false
  hostIPC: false
  hostPID: false

---
apiVersion: v1
kind: ResourceQuota
metadata:
  name: code-interpreter-quota
  namespace: aiworkflow
spec:
  hard:
    pods: "100"
    requests.cpu: "50"
    requests.memory: "50Gi"
    limits.cpu: "100"
    limits.memory: "100Gi"

---
apiVersion: v1
kind: LimitRange
metadata:
  name: code-interpreter-limits
  namespace: aiworkflow
spec:
  limits:
  - max:
      cpu: "1"
      memory: "1Gi"
    min:
      cpu: "100m"
      memory: "128Mi"
    default:
      cpu: "500m"
      memory: "512Mi"
    defaultRequest:
      cpu: "250m"
      memory: "256Mi"
    type: Container
```

### Network Policy (禁止外部訪問)

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: code-interpreter-network-policy
  namespace: aiworkflow
spec:
  podSelector:
    matchLabels:
      app: code-interpreter
  policyTypes:
  - Ingress
  - Egress
  ingress: []  # 拒絕所有入站流量
  egress: []   # 拒絕所有出站流量
```

---

## Layer 4: 輸出清理 (Output Sanitization)

### OutputSanitizer.cs

```csharp
using System.Text.RegularExpressions;

namespace AIWorkflow.Infrastructure.Services.CodeInterpreter;

public interface IOutputSanitizer
{
    string Sanitize(string output);
}

public sealed class OutputSanitizer(ILogger<OutputSanitizer> logger) : IOutputSanitizer
{
    private const int MaxOutputSize = 1024 * 1024; // 1 MB

    // PII 檢測正則表達式
    private static readonly Regex EmailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex SsnRegex = new(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex CreditCardRegex = new(@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex IpAddressRegex = new(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", RegexOptions.Compiled);

    // 系統路徑檢測
    private static readonly Regex SystemPathRegex = new(@"(/[a-z]+/[a-z]+/[^\s]+|[A-Z]:\\[^\s]+)", RegexOptions.Compiled);

    public string Sanitize(string output)
    {
        if (string.IsNullOrEmpty(output))
        {
            return string.Empty;
        }

        try
        {
            // 1. 大小限制
            if (output.Length > MaxOutputSize)
            {
                logger.LogWarning("Output exceeded max size: {Size} bytes", output.Length);
                output = output[..MaxOutputSize] + "\n... [Output truncated due to size limit]";
            }

            // 2. PII 清理
            output = RedactPii(output);

            // 3. 系統路徑清理
            output = RedactSystemPaths(output);

            // 4. HTML/JavaScript 清理
            output = RemoveHtmlAndJavaScript(output);

            // 5. ANSI 轉義碼清理
            output = RemoveAnsiEscapeCodes(output);

            return output;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Output sanitization error");
            return "[Output sanitization error]";
        }
    }

    private string RedactPii(string text)
    {
        // Email
        text = EmailRegex.Replace(text, "[EMAIL_REDACTED]");

        // Phone
        text = PhoneRegex.Replace(text, "[PHONE_REDACTED]");

        // SSN
        text = SsnRegex.Replace(text, "[SSN_REDACTED]");

        // Credit Card
        text = CreditCardRegex.Replace(text, "[CC_REDACTED]");

        // IP Address
        text = IpAddressRegex.Replace(text, "[IP_REDACTED]");

        return text;
    }

    private string RedactSystemPaths(string text)
    {
        return SystemPathRegex.Replace(text, "[PATH_REDACTED]");
    }

    private static string RemoveHtmlAndJavaScript(string text)
    {
        // 移除 HTML 標籤
        text = Regex.Replace(text, @"<[^>]+>", string.Empty);

        // 移除 JavaScript
        text = Regex.Replace(text, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", string.Empty, RegexOptions.IgnoreCase);

        return text;
    }

    private static string RemoveAnsiEscapeCodes(string text)
    {
        // 移除 ANSI 轉義碼
        return Regex.Replace(text, @"\x1B\[[^@-~]*[@-~]", string.Empty);
    }
}
```

---

## 完整執行流程

### CodeInterpreterService.cs

```csharp
namespace AIWorkflow.Application.Services;

public interface ICodeInterpreterService
{
    Task<CodeInterpreterResult> ExecuteCodeAsync(
        string code,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken = default);
}

public sealed class CodeInterpreterService(
    ICodeValidator codeValidator,
    ICodeExecutor codeExecutor,
    IOutputSanitizer outputSanitizer,
    ILogger<CodeInterpreterService> logger) : ICodeInterpreterService
{
    public async Task<CodeInterpreterResult> ExecuteCodeAsync(
        string code,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            // Layer 1: 代碼驗證
            logger.LogInformation("Layer 1: Validating code");
            var validationResult = await codeValidator.ValidateAsync(code, cancellationToken);

            if (!validationResult.IsValid)
            {
                logger.LogWarning("Code validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                return new CodeInterpreterResult(
                    false,
                    string.Empty,
                    $"Code validation failed: {string.Join("; ", validationResult.Errors)}",
                    0,
                    validationResult.Warnings);
            }

            // Layer 2 & 3: 沙箱執行 + 資源限制
            logger.LogInformation("Layer 2-3: Executing in sandbox with resource limits");
            var executionResult = await codeExecutor.ExecuteAsync(code, inputs, cancellationToken);

            // Layer 4: 輸出清理
            logger.LogInformation("Layer 4: Sanitizing output");
            var sanitizedOutput = outputSanitizer.Sanitize(executionResult.Output);
            var sanitizedError = outputSanitizer.Sanitize(executionResult.Error);

            var executionTime = (DateTime.UtcNow - startTime).TotalSeconds;

            logger.LogInformation(
                "Code execution completed: Success={Success}, ExecutionTime={ExecutionTime}s",
                executionResult.Success, executionTime);

            return new CodeInterpreterResult(
                executionResult.Success,
                sanitizedOutput,
                sanitizedError,
                executionTime,
                validationResult.Warnings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Code interpreter service error");
            return new CodeInterpreterResult(
                false,
                string.Empty,
                $"Service error: {ex.Message}",
                (DateTime.UtcNow - startTime).TotalSeconds,
                []);
        }
    }
}

public record CodeInterpreterResult(
    bool Success,
    string Output,
    string Error,
    double ExecutionTimeSeconds,
    List<string> Warnings);
```

---

## 監控和審計

### 執行日誌

```csharp
public sealed class CodeExecutionAuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkflowExecutionId { get; set; }
    public string CodeHash { get; set; } = string.Empty; // SHA256
    public bool ValidationPassed { get; set; }
    public bool ExecutionSuccess { get; set; }
    public double ExecutionTimeSeconds { get; set; }
    public long MemoryUsedBytes { get; set; }
    public string ContainerId { get; set; } = string.Empty;
    public List<string> ValidationErrors { get; set; } = [];
    public List<string> ValidationWarnings { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
```

### Prometheus Metrics

```csharp
public sealed class CodeInterpreterMetrics
{
    private static readonly Counter ExecutionsTotal = Metrics.CreateCounter(
        "code_interpreter_executions_total",
        "Total code executions",
        new CounterConfiguration { LabelNames = new[] { "status", "user_id" } });

    private static readonly Histogram ExecutionDuration = Metrics.CreateHistogram(
        "code_interpreter_execution_duration_seconds",
        "Code execution duration",
        new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.1, 2, 10) });

    private static readonly Gauge ActiveExecutions = Metrics.CreateGauge(
        "code_interpreter_active_executions",
        "Number of active code executions");

    public static void RecordExecution(bool success, string userId, double durationSeconds)
    {
        ExecutionsTotal.WithLabels(success ? "success" : "failure", userId).Inc();
        ExecutionDuration.Observe(durationSeconds);
    }

    public static void IncrementActive()
    {
        ActiveExecutions.Inc();
    }

    public static void DecrementActive()
    {
        ActiveExecutions.Dec();
    }
}
```

---

## 測試策略

### Security Tests

```csharp
[Fact]
public async Task ExecuteCode_WithMaliciousImport_ShouldBeRejected()
{
    // Arrange
    var code = @"
import os
os.system('rm -rf /')
";

    // Act
    var result = await _codeInterpreterService.ExecuteCodeAsync(code, new Dictionary<string, object>());

    // Assert
    Assert.False(result.Success);
    Assert.Contains("Disallowed import", result.Error);
}

[Fact]
public async Task ExecuteCode_WithInfiniteLoop_ShouldTimeout()
{
    // Arrange
    var code = @"
while True:
    pass
";

    // Act
    var result = await _codeInterpreterService.ExecuteCodeAsync(code, new Dictionary<string, object>());

    // Assert
    Assert.False(result.Success);
    Assert.Contains("timeout", result.Error, StringComparison.OrdinalIgnoreCase);
}

[Fact]
public async Task ExecuteCode_WithExcessiveMemory_ShouldFail()
{
    // Arrange
    var code = @"
# Try to allocate 1GB memory (exceeds 512MB limit)
data = [0] * (1024 * 1024 * 1024)
";

    // Act
    var result = await _codeInterpreterService.ExecuteCodeAsync(code, new Dictionary<string, object>());

    // Assert
    Assert.False(result.Success);
}
```

---

## 最佳實踐

### ✅ Do's

1. **4 層防護**: 嚴格執行所有 4 層安全檢查
2. **資源限制**: CPU、Memory、Timeout 都必須設置
3. **網絡隔離**: 禁止所有外部網絡訪問
4. **審計日誌**: 記錄所有執行詳情
5. **監控告警**: 異常行為立即告警
6. **定期審查**: 定期審查黑名單和白名單

### ❌ Don'ts

1. **不要信任用戶代碼**: 所有代碼都是潛在威脅
2. **不要跳過驗證**: 每一層都必須執行
3. **不要暴露系統信息**: 錯誤消息不應包含路徑
4. **不要使用特權容器**: 必須使用非 root 用戶
5. **不要允許網絡訪問**: 除非有特殊需求並經過審批

---

## 實施檢查清單

- [ ] Layer 1: CodeValidator 實現
- [ ] Layer 2: Docker Sandbox 配置
- [ ] Layer 3: Resource Limits 設置
- [ ] Layer 4: OutputSanitizer 實現
- [ ] Kubernetes Pod 配置
- [ ] Network Policy 配置
- [ ] 審計日誌實現
- [ ] Prometheus Metrics
- [ ] 安全測試完成
- [ ] 文檔和 Runbook 完成

---

## 相關文檔

- [Phase 9: 認證實現](./authentication-implementation.md)
- [Phase 9: 安全測試自動化](./security-testing-automation.md)
- [Phase 8: 部署架構 - Docker](../8-deployment-architecture/docker-containerization.md)
- [Phase 8: 部署架構 - Kubernetes](../8-deployment-architecture/kubernetes-deployment.md)

## 參考資源

### 官方文檔
- [Docker Security](https://docs.docker.com/engine/security/)
- [Kubernetes Security Best Practices](https://kubernetes.io/docs/concepts/security/)
- [gVisor - Container Runtime Sandbox](https://gvisor.dev/)

### 安全指南
- [OWASP Secure Coding Practices](https://owasp.org/www-project-secure-coding-practices-quick-reference-guide/)
- [CIS Docker Benchmark](https://www.cisecurity.org/benchmark/docker)
- [CIS Kubernetes Benchmark](https://www.cisecurity.org/benchmark/kubernetes)

---

**文檔維護**: Security Team
**最後更新**: 2025-11-02
**狀態**: ✅ 完整
