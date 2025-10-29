# ADR-008: Code Interpreter 執行模型

## 狀態

✅ **已接受** - 2025-10-28

## 背景

Code Interpreter 是本框架的核心差異化能力之一，允許 Agent 執行 Python/R 代碼進行數據分析。我們需要在**安全性**、**性能**和**成本**之間找到平衡。

## 決策

採用 **Hybrid Container Pool（混合容器池）**：

```yaml
架構策略:
  Container Pool:
    預熱池大小: 5 個 Standby Containers
    啟動時間: <2 秒（從池中獲取）
    安全策略: 執行後銷毀，不重用
    補充策略: 異步非阻塞補充

  執行流程:
    1. 從 Pool 獲取 Container（若無則臨時創建）
    2. 注入代碼 + 上下文
    3. 執行並收集輸出（30秒超時）
    4. 銷毀 Container（確保隔離）
    5. 異步補充 Pool（保持 5 個 Standby）

  安全防護:
    Layer 1: Network Isolation (無外網訪問)
    Layer 2: Resource Limits (CPU/Memory/Disk)
    Layer 3: Execution Timeout (30秒強制終止)
    Layer 4: Read-Only Filesystem (僅 /tmp 可寫)
```

## 架構設計

### Container Pool 管理

```csharp
// Code Interpreter Pool 抽象
public interface ICodeInterpreterPool
{
    Task<ICodeInterpreter> AcquireAsync(CancellationToken cancellationToken = default);
    Task ReleaseAsync(ICodeInterpreter interpreter, CancellationToken cancellationToken = default);
    Task<int> GetAvailableCountAsync();
}

// Hybrid Container Pool 實現
public class HybridCodeInterpreterPool : ICodeInterpreterPool
{
    private readonly IDockerClient _dockerClient;
    private readonly SemaphoreSlim _poolLock = new(1, 1);
    private readonly Queue<DockerCodeInterpreter> _availableContainers = new();
    private readonly int _minPoolSize = 5;
    private readonly int _maxPoolSize = 20;
    private readonly string _containerImage = "python:3.11-slim-security"; // 自定義安全鏡像
    private bool _isInitialized = false;

    public HybridCodeInterpreterPool(IDockerClient dockerClient)
    {
        _dockerClient = dockerClient;
    }

    // 初始化：預熱 5 個容器
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized) return;

        await _poolLock.WaitAsync(cancellationToken);
        try
        {
            for (int i = 0; i < _minPoolSize; i++)
            {
                var container = await CreateContainerAsync(cancellationToken);
                _availableContainers.Enqueue(container);
            }

            _isInitialized = true;
        }
        finally
        {
            _poolLock.Release();
        }
    }

    // 獲取 Container（優先從 Pool，否則臨時創建）
    public async Task<ICodeInterpreter> AcquireAsync(CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            await InitializeAsync(cancellationToken);
        }

        await _poolLock.WaitAsync(cancellationToken);
        try
        {
            // 1. 優先從 Pool 獲取
            if (_availableContainers.TryDequeue(out var container))
            {
                // 異步補充 Pool（非阻塞）
                _ = Task.Run(() => ReplenishPoolAsync(CancellationToken.None));
                return container;
            }

            // 2. Pool 為空，臨時創建（性能降級，但不阻塞）
            return await CreateContainerAsync(cancellationToken);
        }
        finally
        {
            _poolLock.Release();
        }
    }

    // 釋放 Container（銷毀後補充 Pool）
    public async Task ReleaseAsync(ICodeInterpreter interpreter, CancellationToken cancellationToken = default)
    {
        var dockerInterpreter = interpreter as DockerCodeInterpreter;
        if (dockerInterpreter == null) return;

        try
        {
            // 1. 銷毀 Container（安全隔離）
            await _dockerClient.Containers.StopContainerAsync(
                dockerInterpreter.ContainerId,
                new ContainerStopParameters { WaitBeforeKillSeconds = 5 },
                cancellationToken
            );

            await _dockerClient.Containers.RemoveContainerAsync(
                dockerInterpreter.ContainerId,
                new ContainerRemoveParameters { Force = true, RemoveVolumes = true },
                cancellationToken
            );

            // 2. 異步補充 Pool（非阻塞）
            _ = Task.Run(() => ReplenishPoolAsync(CancellationToken.None));
        }
        catch (Exception ex)
        {
            // 記錄錯誤，但不拋出（避免影響主流程）
            Console.WriteLine($"Error releasing container: {ex.Message}");
        }
    }

    // 創建新 Container（帶 4 層安全防護）
    private async Task<DockerCodeInterpreter> CreateContainerAsync(CancellationToken cancellationToken)
    {
        var createParams = new CreateContainerParameters
        {
            Image = _containerImage,
            Cmd = new[] { "python3", "-u" }, // Unbuffered output

            // Layer 1: Network Isolation
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { "none", new EndpointSettings() } // 無網絡訪問
                }
            },

            // Layer 2: Resource Limits
            HostConfig = new HostConfig
            {
                Memory = 512 * 1024 * 1024, // 512MB RAM
                MemorySwap = 512 * 1024 * 1024, // No swap
                NanoCPUs = 500_000_000, // 0.5 CPU
                PidsLimit = 100, // 最多 100 個進程
                DiskQuota = 1024 * 1024 * 1024, // 1GB disk

                // Layer 4: Read-Only Filesystem
                ReadonlyRootfs = true,
                Tmpfs = new Dictionary<string, string>
                {
                    { "/tmp", "rw,noexec,nosuid,size=100m" } // 僅 /tmp 可寫（100MB）
                }
            },

            // 環境變量
            Env = new[]
            {
                "PYTHONUNBUFFERED=1",
                "EXECUTION_TIMEOUT=30"
            }
        };

        var response = await _dockerClient.Containers.CreateContainerAsync(
            createParams,
            cancellationToken
        );

        // 啟動 Container
        await _dockerClient.Containers.StartContainerAsync(
            response.ID,
            new ContainerStartParameters(),
            cancellationToken
        );

        return new DockerCodeInterpreter(response.ID, _dockerClient);
    }

    // 異步補充 Pool（保持 5 個 Standby）
    private async Task ReplenishPoolAsync(CancellationToken cancellationToken)
    {
        await _poolLock.WaitAsync(cancellationToken);
        try
        {
            int currentCount = _availableContainers.Count;
            int needed = _minPoolSize - currentCount;

            for (int i = 0; i < needed && _availableContainers.Count < _maxPoolSize; i++)
            {
                var container = await CreateContainerAsync(cancellationToken);
                _availableContainers.Enqueue(container);
            }
        }
        finally
        {
            _poolLock.Release();
        }
    }

    public Task<int> GetAvailableCountAsync()
    {
        return Task.FromResult(_availableContainers.Count);
    }
}
```

### Code Interpreter 實現

```csharp
// Docker-Based Code Interpreter
public class DockerCodeInterpreter : ICodeInterpreter
{
    public string ContainerId { get; }
    private readonly IDockerClient _dockerClient;
    private readonly TimeSpan _executionTimeout = TimeSpan.FromSeconds(30); // Layer 3: Timeout

    public DockerCodeInterpreter(string containerId, IDockerClient dockerClient)
    {
        ContainerId = containerId;
        _dockerClient = dockerClient;
    }

    public async Task<CodeExecutionResult> ExecuteAsync(
        string code,
        string language,
        Dictionary<string, object> context,
        CancellationToken cancellationToken = default)
    {
        if (language != "python" && language != "r")
        {
            throw new NotSupportedException($"Language '{language}' is not supported");
        }

        try
        {
            // 1. 準備執行腳本
            var script = PrepareScript(code, context, language);

            // 2. 寫入腳本到容器
            await WriteScriptToContainerAsync(script, cancellationToken);

            // 3. 執行腳本（帶超時控制）
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_executionTimeout);

            var execResult = await ExecuteScriptInContainerAsync(language, cts.Token);

            // 4. 收集輸出
            return new CodeExecutionResult
            {
                Success = execResult.ExitCode == 0,
                Output = execResult.Stdout,
                Error = execResult.Stderr,
                ExitCode = execResult.ExitCode,
                ExecutionTimeMs = execResult.ExecutionTimeMs
            };
        }
        catch (OperationCanceledException)
        {
            // Layer 3: Timeout 強制終止
            return new CodeExecutionResult
            {
                Success = false,
                Error = $"Execution timeout after {_executionTimeout.TotalSeconds} seconds",
                ExitCode = -1
            };
        }
        catch (Exception ex)
        {
            return new CodeExecutionResult
            {
                Success = false,
                Error = $"Execution error: {ex.Message}",
                ExitCode = -1
            };
        }
    }

    private string PrepareScript(string code, Dictionary<string, object> context, string language)
    {
        if (language == "python")
        {
            var contextJson = JsonSerializer.Serialize(context);
            return $@"
import json
import sys

# 注入上下文
context = json.loads('''{contextJson}''')
globals().update(context)

# 執行用戶代碼
try:
    {code}
except Exception as e:
    print(f'Error: {{e}}', file=sys.stderr)
    sys.exit(1)
";
        }
        else // R
        {
            var contextJson = JsonSerializer.Serialize(context);
            return $@"
library(jsonlite)

# 注入上下文
context <- fromJSON('{contextJson}')
list2env(context, .GlobalEnv)

# 執行用戶代碼
tryCatch({{
    {code}
}}, error = function(e) {{
    cat(paste('Error:', e$message), file=stderr())
    quit(status=1)
}})
";
        }
    }

    private async Task WriteScriptToContainerAsync(string script, CancellationToken cancellationToken)
    {
        // 使用 Docker Exec API 寫入腳本到 /tmp/script
        var execCreateResponse = await _dockerClient.Exec.ExecCreateContainerAsync(
            ContainerId,
            new ContainerExecCreateParameters
            {
                Cmd = new[] { "sh", "-c", $"cat > /tmp/script.py << 'EOF'\n{script}\nEOF" },
                AttachStdout = false,
                AttachStderr = false
            },
            cancellationToken
        );

        await _dockerClient.Exec.StartContainerExecAsync(execCreateResponse.ID, cancellationToken);
    }

    private async Task<(int ExitCode, string Stdout, string Stderr, long ExecutionTimeMs)> ExecuteScriptInContainerAsync(
        string language,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        var execCreateResponse = await _dockerClient.Exec.ExecCreateContainerAsync(
            ContainerId,
            new ContainerExecCreateParameters
            {
                Cmd = language == "python"
                    ? new[] { "python3", "/tmp/script.py" }
                    : new[] { "Rscript", "/tmp/script.R" },
                AttachStdout = true,
                AttachStderr = true
            },
            cancellationToken
        );

        using var stream = await _dockerClient.Exec.StartContainerExecAsync(
            execCreateResponse.ID,
            false,
            cancellationToken
        );

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        await stream.CopyOutputToAsync(
            Stream.Null,
            stdout,
            stderr,
            cancellationToken
        );

        var execInspectResponse = await _dockerClient.Exec.InspectContainerExecAsync(
            execCreateResponse.ID,
            cancellationToken
        );

        var executionTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

        return (
            ExitCode: execInspectResponse.ExitCode,
            Stdout: stdout.ToString(),
            Stderr: stderr.ToString(),
            ExecutionTimeMs: (long)executionTime
        );
    }
}

// 執行結果
public class CodeExecutionResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
    public int ExitCode { get; set; }
    public long ExecutionTimeMs { get; set; }
}
```

### 使用範例

```csharp
// Agent 使用 Code Interpreter 執行代碼
public class DataAnalysisAgent
{
    private readonly ICodeInterpreterPool _interpreterPool;

    public async Task<string> AnalyzeDataAsync(string pythonCode, Dictionary<string, object> data)
    {
        // 1. 從 Pool 獲取 Interpreter
        var interpreter = await _interpreterPool.AcquireAsync();

        try
        {
            // 2. 執行代碼
            var result = await interpreter.ExecuteAsync(
                code: pythonCode,
                language: "python",
                context: data
            );

            // 3. 處理結果
            if (result.Success)
            {
                return $"分析完成：\n{result.Output}\n執行時間：{result.ExecutionTimeMs}ms";
            }
            else
            {
                return $"執行錯誤：\n{result.Error}";
            }
        }
        finally
        {
            // 4. 釋放 Interpreter（銷毀容器）
            await _interpreterPool.ReleaseAsync(interpreter);
        }
    }
}
```

## 4 層安全防護詳細設計

### Layer 1: Network Isolation（網絡隔離）

```yaml
實現方式: Docker Network Mode = "none"
防護目標:
  - 阻止惡意代碼訪問外網（下載病毒、DDoS 攻擊）
  - 阻止訪問內網資源（數據庫、API）
  - 阻止橫向移動（攻擊其他容器）

配置:
  NetworkingConfig:
    EndpointsConfig:
      none: {} # 無任何網絡訪問

風險等級: 🔴 Critical
緩解效果: ✅ 完全阻斷網絡攻擊
```

### Layer 2: Resource Limits（資源限制）

```yaml
實現方式: Docker HostConfig
防護目標:
  - 防止 CPU 耗盡（死循環、挖礦）
  - 防止內存耗盡（內存炸彈）
  - 防止磁盤耗盡（寫入大量數據）
  - 防止進程炸彈（fork bomb）

配置:
  Memory: 512MB # 最大內存
  MemorySwap: 512MB # 禁用 Swap
  NanoCPUs: 500_000_000 # 0.5 CPU 核心
  PidsLimit: 100 # 最多 100 個進程
  DiskQuota: 1GB # 最大磁盤使用

風險等級: 🟡 High
緩解效果: ✅ 防止資源耗盡攻擊
```

### Layer 3: Execution Timeout（執行超時）

```yaml
實現方式: CancellationTokenSource.CancelAfter(30 seconds)
防護目標:
  - 防止無限循環
  - 防止長時間占用資源
  - 確保系統響應性

配置:
  Timeout: 30 秒
  超時行為: 強制終止容器
  退出碼: -1 (Timeout)

風險等級: 🟡 High
緩解效果: ✅ 保證系統響應性
```

### Layer 4: Read-Only Filesystem（只讀文件系統）

```yaml
實現方式: Docker ReadonlyRootfs = true
防護目標:
  - 防止惡意代碼修改系統文件
  - 防止植入後門
  - 防止篡改 Python/R 解釋器

配置:
  ReadonlyRootfs: true
  Tmpfs:
    /tmp: "rw,noexec,nosuid,size=100m"

  可寫區域: 僅 /tmp (100MB, 不可執行)
  禁止執行: noexec flag

風險等級: 🟡 High
緩解效果: ✅ 防止系統篡改
```

### 安全防護矩陣

```
┌─────────────────────────────────────────────────────────────┐
│  攻擊類型                    │  防護層級  │  緩解效果      │
├─────────────────────────────────────────────────────────────┤
│  🌐 網絡攻擊（DDoS、下載）    │  Layer 1   │  ✅ 完全阻斷  │
│  💻 CPU 耗盡（挖礦、死循環）  │  Layer 2   │  ✅ 限制資源  │
│  💾 內存耗盡（內存炸彈）      │  Layer 2   │  ✅ 限制資源  │
│  📁 磁盤耗盡（寫入攻擊）      │  Layer 2   │  ✅ 限制資源  │
│  🔁 無限循環                  │  Layer 3   │  ✅ 強制終止  │
│  🛠️ 系統篡改（植入後門）      │  Layer 4   │  ✅ 只讀文件  │
│  🔧 執行惡意二進制文件        │  Layer 4   │  ✅ noexec   │
│  🔀 進程炸彈（fork bomb）     │  Layer 2   │  ✅ 限制進程  │
└─────────────────────────────────────────────────────────────┘
```

## 理由

### 為什麼選擇 Hybrid Container Pool？

1. **性能優化**：
   - Pool 預熱：從池中獲取容器 <2 秒（vs 臨時創建 5-10 秒）
   - 用戶體驗：Agent 執行代碼時無需等待容器啟動
   - 吞吐量：5 個 Standby 容器可並行處理多個請求

2. **安全保證**：
   - 執行後銷毀：每次執行後立即銷毀容器（不重用）
   - 完全隔離：4 層防護確保惡意代碼無法逃逸
   - 零信任：每個代碼執行都在全新的隔離環境中

3. **成本優化**：
   - 小型池：5 個容器（vs 20 個），節省資源
   - 彈性擴展：Pool 為空時臨時創建（性能降級但不阻塞）
   - 異步補充：非阻塞補充，不影響主流程

4. **可靠性**：
   - 容錯機制：Pool 為空時降級為臨時創建
   - 自動恢復：異步補充確保 Pool 始終有容器
   - 超時保護：30 秒強制終止防止死鎖

### 為什麼不選擇純 On-Demand？

- **啟動延遲高**：每次創建容器需 5-10 秒（用戶體驗差）
- **吞吐量低**：無法並行處理多個請求
- **性能波動**：每次請求延遲不一致

### 為什麼不選擇純 Pool（重用容器）？

- **安全風險**：容器重用可能殘留惡意代碼（狀態污染）
- **難以清理**：無法保證 100% 清理容器狀態
- **合規問題**：多租戶環境不允許容器重用（數據隱私）

## 影響

### 正面影響

- ✅ **低延遲**：從 Pool 獲取容器 <2 秒（vs 臨時創建 5-10 秒）
- ✅ **高安全性**：4 層防護 + 執行後銷毀，杜絕容器逃逸
- ✅ **高吞吐量**：5 個 Standby 容器可並行處理請求
- ✅ **成本可控**：小型 Pool（5 個）節省資源

### 負面影響

- ⚠️ **資源占用**：5 個 Standby 容器持續占用內存（~2.5GB）
- ⚠️ **Pool 耗盡風險**：高並發時 Pool 可能為空（降級為臨時創建）
- ⚠️ **實現複雜度**：需要管理 Pool 生命週期和補充邏輯

### 緩解措施

1. **資源優化**：
   - 使用輕量級鏡像（python:3.11-slim）
   - 限制單容器內存（512MB）
   - 監控 Pool 使用率，動態調整大小

2. **Pool 耗盡處理**：
   - 降級機制：Pool 為空時臨時創建（不阻塞用戶）
   - 異步補充：非阻塞補充 Pool
   - 監控警報：Pool 使用率 >80% 時告警

3. **運維監控**：
   - 容器健康檢查（每 30 秒）
   - Pool 大小監控（可視化 Dashboard）
   - 執行失敗率監控（SLA 99.9%）

## 性能指標

### 延遲分析

```yaml
Container 獲取延遲:
  從 Pool 獲取: <2 秒 (目標: P99 <2s)
  臨時創建: 5-10 秒 (降級模式)

代碼執行延遲:
  簡單腳本 (print): <100ms
  數據分析 (Pandas): 1-5 秒
  複雜計算 (ML): 10-30 秒 (受 Timeout 限制)

總體 P99 延遲: <3 秒 (Pool 命中)
總體 P99 延遲: <12 秒 (Pool 未命中)
```

### 吞吐量分析

```yaml
並行執行能力:
  Standby Containers: 5 個
  最大並行請求: 5 個 (Pool 滿載)
  降級模式: 無限制 (臨時創建)

預期 QPS:
  平均執行時間 3 秒: ~1.67 QPS (5 containers / 3s)
  Pool 補充時間 5 秒: 降級至 ~1 QPS

擴展性:
  Phase 2 (Kubernetes): Pool 大小可擴展至 20-50 個
  自動伸縮: 基於 CPU/Memory 使用率
```

### 資源消耗

```yaml
單容器資源:
  Memory: 512MB (限制)
  CPU: 0.5 Core (限制)
  Disk: 1GB (限制)

Pool 資源消耗:
  5 Standby Containers: ~2.5GB RAM, ~2.5 CPU Cores
  MVP 部署: 可接受（單機 8GB RAM, 4 Cores）
  Phase 2: Kubernetes 自動伸縮
```

## 遷移策略

### Phase 1 (MVP)

```yaml
部署方式: Docker Compose
Container Pool 大小: 5 個
Container 鏡像: python:3.11-slim-security
限制:
  - 單機部署
  - 最大並行請求: 5 個
  - 無自動伸縮
```

### Phase 2 (Kubernetes)

```yaml
部署方式: Kubernetes Deployment
Container Pool 大小: 動態伸縮 (5-50 個)
Container 鏡像: 自定義安全鏡像 (基於 python:3.11-slim)
增強功能:
  - 自動伸縮 (HPA - Horizontal Pod Autoscaler)
  - 多 Pod 部署（跨節點）
  - 高可用性（Pod 崩潰自動恢復）
  - 資源配額（Namespace-level Quotas）

遷移步驟:
  Week 33-34: 創建 Kubernetes Deployment YAML
  Week 35-36: 實現 HPA 自動伸縮
  Week 37-38: 測試與優化
  Week 39-40: 生產環境遷移（藍綠部署）
```

## 相關決策

- ADR-006: Agent 狀態管理策略
- ADR-007: Multi-Agent 通訊機制
- ADR-009: Docker 安全鏡像構建（待定）

## 參考資料

- [Docker Security Best Practices](https://docs.docker.com/engine/security/)
- [OWASP Container Security](https://owasp.org/www-community/vulnerabilities/Container_Security)
- [CIS Docker Benchmark](https://www.cisecurity.org/benchmark/docker)
- [Kubernetes Pod Security Standards](https://kubernetes.io/docs/concepts/security/pod-security-standards/)
