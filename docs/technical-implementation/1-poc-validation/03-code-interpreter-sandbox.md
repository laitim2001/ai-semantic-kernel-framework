# PoC 3: Code Interpreter Sandbox 驗證

**優先級**: 🔴 P0 (最高優先級)
**預計時間**: 3 days (Day 5-7)
**狀態**: ⏳ 待開始
**負責人**: Security Engineer + Backend Tech Lead

**基於**: Docker + Python 3.13 + 4-Layer Security (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Code Interpreter 沙箱環境的安全性與可行性：

1. ✅ **Docker 沙箱執行**: 隔離環境運行 Python 代碼
2. ✅ **4 層安全防護**: Network, Resource, Timeout, Filesystem
3. ✅ **沙箱逃逸測試**: 0 漏洞容忍
4. ✅ **Python 3.13 兼容性**: NumPy, Pandas, Matplotlib 支持
5. ✅ **性能基準**: 執行時間與資源限制

---

## 📋 4 層安全架構

```
┌──────────────────────────────────────────┐
│     Layer 1: Network Isolation           │
│  - No internet access                    │
│  - No internal network access            │
└──────────────────────────────────────────┘
        ↓
┌──────────────────────────────────────────┐
│     Layer 2: Resource Limits             │
│  - CPU: 1 core, 80% max                  │
│  - Memory: 512MB                          │
│  - Disk: 100MB (tmpfs)                    │
└──────────────────────────────────────────┘
        ↓
┌──────────────────────────────────────────┐
│     Layer 3: Timeout Control             │
│  - Max execution: 30 seconds              │
│  - Auto-terminate on timeout              │
└──────────────────────────────────────────┘
        ↓
┌──────────────────────────────────────────┐
│     Layer 4: Filesystem Restriction      │
│  - Read-only base filesystem              │
│  - Isolated /tmp (tmpfs)                  │
│  - No sensitive host mounts               │
└──────────────────────────────────────────┘
```

---

## 🔧 環境準備

### Dockerfile (Python 3.13-slim)

```dockerfile
FROM python:3.13-slim

# 安裝數據科學套件
RUN pip install --no-cache-dir \
    numpy==2.3.0 \
    pandas==2.2.3 \
    matplotlib==3.9.0 \
    seaborn==0.13.0 \
    scipy==1.14.0

# 創建非 root 用戶
RUN useradd -m -u 1000 sandbox && \
    mkdir -p /workspace && \
    chown -R sandbox:sandbox /workspace

USER sandbox
WORKDIR /workspace

# 入口點
CMD ["python3"]
```

### Docker Compose 配置

```yaml
version: '3.8'

services:
  code-interpreter:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: code-interpreter-sandbox

    # Layer 1: Network Isolation
    network_mode: "none"

    # Layer 2: Resource Limits
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M

    # Layer 3: Timeout (handled by orchestrator)
    # Layer 4: Filesystem Restrictions
    read_only: true
    tmpfs:
      - /tmp:size=100M,mode=1777
      - /workspace:size=100M,mode=1777

    security_opt:
      - no-new-privileges:true
      - seccomp=unconfined  # 可選：更嚴格的 seccomp profile

    cap_drop:
      - ALL

    cap_add:
      - CHOWN
      - SETUID
      - SETGID
```

---

## 💻 實現步驟

### 步驟 1: 基礎 Python 代碼執行

**C# Orchestrator**:

```csharp
using System.Diagnostics;
using Docker.DotNet;

public class CodeInterpreter
{
    private readonly DockerClient _docker;

    public async Task<ExecutionResult> ExecuteAsync(string pythonCode, int timeoutSeconds = 30)
    {
        // 創建臨時容器
        var containerId = await CreateSandboxContainerAsync();

        try
        {
            // 寫入代碼到容器
            await WriteCodeToContainerAsync(containerId, pythonCode);

            // 執行並限制時間
            var result = await ExecuteWithTimeoutAsync(containerId, timeoutSeconds);

            return result;
        }
        finally
        {
            // 清理容器（execute-and-destroy 策略）
            await RemoveContainerAsync(containerId);
        }
    }

    private async Task<string> CreateSandboxContainerAsync()
    {
        var createParams = new CreateContainerParameters
        {
            Image = "code-interpreter:latest",
            NetworkDisabled = true,
            HostConfig = new HostConfig
            {
                Memory = 512 * 1024 * 1024, // 512MB
                NanoCPUs = 1_000_000_000,   // 1 CPU
                ReadonlyRootfs = true,
                Tmpfs = new Dictionary<string, string>
                {
                    ["/tmp"] = "size=100M",
                    ["/workspace"] = "size=100M"
                }
            }
        };

        var response = await _docker.Containers.CreateContainerAsync(createParams);
        await _docker.Containers.StartContainerAsync(response.ID, null);

        return response.ID;
    }
}
```

### 步驟 2: 沙箱逃逸測試

**測試用例集合**:

```python
# Test 1: 網路訪問嘗試
try:
    import socket
    s = socket.socket()
    s.connect(("google.com", 80))
    print("❌ Network access succeeded - SECURITY BREACH")
except:
    print("✅ Network access blocked")

# Test 2: 檔案系統寫入嘗試
try:
    with open("/etc/passwd", "w") as f:
        f.write("hacked")
    print("❌ Filesystem write succeeded - SECURITY BREACH")
except:
    print("✅ Filesystem write blocked")

# Test 3: 記憶體炸彈
try:
    a = [0] * (1024 ** 3)  # 嘗試分配 1GB
    print("❌ Memory bomb succeeded - SECURITY BREACH")
except MemoryError:
    print("✅ Memory limit enforced")

# Test 4: CPU 炸彈
import time
start = time.time()
while time.time() - start < 60:  # 嘗試運行 60 秒
    pass
print("❌ CPU bomb succeeded - SECURITY BREACH")
# 應該在 30 秒 timeout 被終止
```

---

## 🧪 測試用例

### 測試套件 1: 正常執行

| 測試場景 | 代碼 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-1.1 | `print("Hello")` | 正常輸出 | 100% 成功 |
| TC-1.2 | NumPy 計算 | 正確結果 | 100% 成功 |
| TC-1.3 | Pandas DataFrame | 正確處理 | 100% 成功 |
| TC-1.4 | Matplotlib 繪圖 | 生成圖片 | 100% 成功 |

### 測試套件 2: 安全測試

| 測試場景 | 攻擊類型 | 預期結果 | 成功標準 |
|---------|---------|----------|----------|
| TC-2.1 | 網路訪問 | 被阻擋 | 0 成功 |
| TC-2.2 | 檔案系統寫入 | 被阻擋 | 0 成功 |
| TC-2.3 | 記憶體炸彈 | 被限制 | 0 成功 |
| TC-2.4 | CPU 炸彈 | 被終止 | 0 成功 |
| TC-2.5 | 沙箱逃逸 | 失敗 | 0 成功 |

### 測試套件 3: 性能測試

| 測試場景 | 操作 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-3.1 | 簡單計算 | <1 秒 | 通過 |
| TC-3.2 | NumPy 矩陣運算 | <5 秒 | 通過 |
| TC-3.3 | Pandas 數據處理 | <5 秒 | 通過 |
| TC-3.4 | Matplotlib 繪圖 | <5 秒 | 通過 |

---

## ✅ 成功標準驗證

### 1. 0 沙箱逃逸漏洞 ✅ / ❌

**測試結果**:
- 網路隔離: ✅ / ❌
- 檔案系統保護: ✅ / ❌
- 資源限制: ✅ / ❌
- Timeout 控制: ✅ / ❌

**結果**: ✅ / ❌ (發現漏洞數: _____)

### 2. 執行成功率 >95% ✅ / ❌

**結果**: ✅ / ❌ (實際成功率: _____%);

### 3. 平均執行時間 <5 秒 ✅ / ❌

**結果**: ✅ / ❌ (實際平均時間: _____ 秒)

### 4. 資源限制有效 ✅ / ❌

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 安全發現

1. **沙箱逃逸測試結果**:
   - 發現漏洞數: _____
   - 漏洞詳情: _________________________

2. **資源限制效果**:
   - CPU 限制: _________________________
   - Memory 限制: _________________________
   - Disk 限制: _________________________

3. **Python 3.13 兼容性**:
   - NumPy: ✅ / ❌
   - Pandas: ✅ / ❌
   - Matplotlib: ✅ / ❌

---

## ⚠️ 風險識別

| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| Docker 逃逸漏洞 | 極高 | 使用 gVisor (Phase 2) |
| 資源耗盡攻擊 | 高 | Container Pool 管理 |
| Timeout 繞過 | 中 | 多層 Timeout 機制 |

---

## 💡 建議與下一步

- Phase 2 升級至 gVisor
- 實現 Container Pool
- 設計監控告警機制

---

**最後更新**: 2025-10-30
**PoC 負責人**: Security Engineer

[← 上一個 PoC](./02-persona-builder.md) | [下一個 PoC: Text-to-SQL →](./04-text-to-sql-engine.md)
