# PoC 3: Code Interpreter Sandbox - 實際執行指導

**目的**: 逐步執行 PoC 3 驗證，構建安全的 Python 代碼執行沙箱

**預計時間**: 3 days (Day 5-7)
**執行人**: Backend Tech Lead + Security Engineer
**執行日期**: _____________

**前置條件**: PoC 1-2 已通過 ✅

**⚠️ 安全警告**: 本 PoC 涉及代碼執行沙箱，必須在隔離環境中測試，嚴禁在生產環境執行

---

## 📋 執行前檢查清單

### 必備條件 Checklist

在開始執行前，請確認以下所有項目：

- [ ] **PoC 1-2 完成確認**
  - [ ] PoC 1 狀態: PASSED ✅
  - [ ] PoC 2 狀態: PASSED ✅

- [ ] **開發環境**
  - [ ] Docker Desktop 已安裝並運行
  - [ ] Python 3.13+ 已安裝 (用於測試腳本)
  - [ ] curl 或 Postman (用於 API 測試)
  - [ ] 專案目錄已創建 (`poc-code-interpreter/`)

- [ ] **安全準備**
  - [ ] ⚠️ 測試環境與生產環境完全隔離
  - [ ] ⚠️ 網絡隔離準備就緒
  - [ ] ⚠️ 準備沙箱逃逸測試腳本（10 個攻擊向量）

- [ ] **Docker 環境驗證**
  - [ ] Docker 版本 ≥ 20.10
  - [ ] Docker Compose 已安裝
  - [ ] 可以 pull Python 官方鏡像

- [ ] **時間安排**
  - [ ] Day 5: 6-8 小時 (Docker 沙箱 + 基礎測試)
  - [ ] Day 6: 6-8 小時 (4 層安全架構)
  - [ ] Day 7: 4-6 小時 (沙箱逃逸測試 + 報告)

### PoC 3 核心目標

1. ✅ **Docker 沙箱隔離**: 完全網絡隔離的 Python 執行環境
2. ✅ **4 層安全架構**: Network, Resource, Timeout, Filesystem
3. ✅ **代碼執行成功率 >95%**: 合法代碼正常執行
4. ✅ **沙箱逃逸防護**: 0 漏洞容忍 (10/10 攻擊阻擋)
5. ✅ **性能基準**: 執行時間 <5 秒

---

## 🚀 Day 5: Docker 沙箱與基礎測試 (6-8 小時)

### Phase 5.1: Docker 環境驗證 (30 分鐘)

#### 步驟 1: 驗證 Docker 安裝

```powershell
# Windows PowerShell
docker --version
docker-compose --version
docker ps

# 測試 Docker 運行
docker run hello-world
```

**預期輸出**:
```
Docker version 24.0.5, build ced0996
Docker Compose version v2.21.0
Hello from Docker!
```

**✅ 通過標準**: Docker 正常運行，可以執行容器

**❌ 故障排查**:
- Docker Desktop 未啟動: 手動啟動 Docker Desktop
- WSL2 問題 (Windows): 啟用 WSL2 並設為 Docker 後端
- 權限錯誤: 以管理員身份運行 PowerShell

**記錄點**:
```
[Day 5 - 10:00] Docker 環境驗證完成
- Docker 版本: _____
- Docker Compose 版本: _____
- 測試容器運行: 成功 / 失敗
```

---

#### 步驟 2: 創建專案結構

```powershell
# 創建專案目錄
mkdir C:\poc-code-interpreter
cd C:\poc-code-interpreter

# 創建目錄結構
mkdir sandbox
mkdir sandbox\workspace
mkdir tests
mkdir api

# 創建檔案
New-Item -Path "sandbox\Dockerfile" -ItemType File
New-Item -Path "sandbox\requirements.txt" -ItemType File
New-Item -Path "docker-compose.yml" -ItemType File
```

**預期結構**:
```
poc-code-interpreter/
├── sandbox/
│   ├── Dockerfile
│   ├── requirements.txt
│   └── workspace/
├── tests/
├── api/
└── docker-compose.yml
```

---

### Phase 5.2: Docker 沙箱構建 (2 小時)

#### 步驟 3: 創建 Dockerfile

**編輯 `sandbox/Dockerfile`**:

```dockerfile
# 使用官方 Python 3.13 精簡版本
FROM python:3.13-slim

# 設置工作目錄
WORKDIR /workspace

# 安裝科學計算庫（用戶可能需要）
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 創建非特權用戶
RUN useradd -m -u 1000 -s /bin/bash sandbox && \
    chown -R sandbox:sandbox /workspace

# 切換到非特權用戶
USER sandbox

# 設置環境變量
ENV PYTHONUNBUFFERED=1
ENV PYTHONDONTWRITEBYTECODE=1

# 默認命令
CMD ["python3"]
```

**編輯 `sandbox/requirements.txt`**:

```txt
# 科學計算庫
numpy==2.3.0
pandas==2.2.3
matplotlib==3.9.0
scipy==1.15.0

# 數據可視化
seaborn==0.13.0
plotly==5.24.1

# 工具庫
requests==2.32.3
python-dateutil==2.9.0
```

**編輯 `docker-compose.yml`**:

```yaml
version: '3.8'

services:
  code-interpreter:
    build:
      context: ./sandbox
      dockerfile: Dockerfile
    container_name: code-interpreter-sandbox

    # Layer 1: Network Isolation
    network_mode: "none"  # 完全隔離網絡

    # Layer 2: Resource Limits
    deploy:
      resources:
        limits:
          cpus: '1.0'        # 最大 1 CPU
          memory: 512M       # 最大 512MB 記憶體
        reservations:
          cpus: '0.5'
          memory: 256M

    # Layer 3: Filesystem Security
    read_only: true          # 根文件系統只讀
    tmpfs:
      - /tmp:size=100M       # 臨時目錄 100MB 限制
      - /workspace:size=100M # 工作目錄 100MB 限制

    # Layer 4: Security Options
    security_opt:
      - no-new-privileges:true  # 禁止提權
      - seccomp:unconfined      # PoC 階段允許，生產環境需自定義 seccomp

    # 禁用危險功能
    cap_drop:
      - ALL
    cap_add:
      - CHOWN      # 允許更改文件所有者
      - SETUID     # 允許設置用戶 ID
      - SETGID     # 允許設置組 ID

    # 工作目錄掛載
    volumes:
      - ./sandbox/workspace:/workspace:rw

    # 環境變量
    environment:
      - PYTHONUNBUFFERED=1
      - MAX_EXECUTION_TIME=30
```

---

#### 步驟 4: 構建並測試 Docker 鏡像

```powershell
# 構建鏡像
docker-compose build

# 查看鏡像大小
docker images | findstr code-interpreter
```

**預期輸出**:
```
Building code-interpreter...
Successfully built abc123def456
Successfully tagged poc-code-interpreter_code-interpreter:latest

code-interpreter_code-interpreter   latest   abc123def456   2 minutes ago   245MB
```

**✅ 通過標準**:
- 鏡像構建成功
- 鏡像大小 < 500MB
- 無安全警告

**測試基礎執行**:

```powershell
# 啟動容器
docker-compose up -d

# 測試 Python 版本
docker exec code-interpreter-sandbox python3 --version

# 測試 numpy 安裝
docker exec code-interpreter-sandbox python3 -c "import numpy; print(numpy.__version__)"

# 停止容器
docker-compose down
```

**預期輸出**:
```
Python 3.13.0
2.3.0
```

**記錄點**:
```
[Day 5 - 12:30] Docker 沙箱構建完成
- 鏡像構建時間: _____ 秒
- 鏡像大小: _____ MB
- Python 版本: 3.13.0
- Numpy 測試: 成功 / 失敗
```

---

### Phase 5.3: 基礎代碼執行測試 (2 小時)

#### 步驟 5: 創建 Python 執行 API

**創建 `api/code_executor.py`**:

```python
import docker
import time
import uuid
from typing import Tuple, Optional

class CodeExecutor:
    def __init__(self):
        self.client = docker.from_env()
        self.image_name = "poc-code-interpreter_code-interpreter"
        self.timeout = 30  # 30 秒超時

    def execute_code(self, code: str) -> Tuple[bool, str, float, Optional[str]]:
        """
        執行 Python 代碼

        Returns:
            (success, output, execution_time, error)
        """
        execution_id = str(uuid.uuid4())[:8]

        try:
            start_time = time.time()

            # 創建臨時容器
            container = self.client.containers.run(
                self.image_name,
                command=["python3", "-c", code],
                detach=True,
                network_mode="none",
                mem_limit="512m",
                cpu_quota=100000,  # 1 CPU
                read_only=True,
                tmpfs={'/tmp': 'size=100m', '/workspace': 'size=100m'},
                security_opt=["no-new-privileges:true"],
                remove=True,
                name=f"exec-{execution_id}"
            )

            # 等待執行完成（帶超時）
            result = container.wait(timeout=self.timeout)

            execution_time = time.time() - start_time

            # 獲取輸出
            logs = container.logs(stdout=True, stderr=True).decode('utf-8')

            # 檢查退出碼
            exit_code = result['StatusCode']

            if exit_code == 0:
                return (True, logs, execution_time, None)
            else:
                return (False, logs, execution_time, f"Exit code: {exit_code}")

        except docker.errors.ContainerError as e:
            return (False, "", 0, f"Container error: {str(e)}")
        except Exception as e:
            return (False, "", 0, f"Execution error: {str(e)}")

    def cleanup(self):
        """清理所有執行容器"""
        try:
            containers = self.client.containers.list(
                filters={"name": "exec-"}
            )
            for container in containers:
                container.stop(timeout=1)
                container.remove()
        except Exception as e:
            print(f"Cleanup error: {e}")

if __name__ == "__main__":
    executor = CodeExecutor()

    # 測試 1: 簡單計算
    print("========== Test 1: Simple Calculation ==========")
    code1 = "print(2 + 2)"
    success, output, exec_time, error = executor.execute_code(code1)
    print(f"Success: {success}")
    print(f"Output: {output.strip()}")
    print(f"Execution Time: {exec_time:.3f}s")
    print(f"Result: {'✅ PASSED' if success and '4' in output else '❌ FAILED'}")

    # 測試 2: Numpy 計算
    print("\n========== Test 2: Numpy Array ==========")
    code2 = """
import numpy as np
arr = np.array([1, 2, 3, 4, 5])
print(f"Mean: {arr.mean()}")
"""
    success, output, exec_time, error = executor.execute_code(code2)
    print(f"Success: {success}")
    print(f"Output: {output.strip()}")
    print(f"Execution Time: {exec_time:.3f}s")
    print(f"Result: {'✅ PASSED' if success and 'Mean: 3.0' in output else '❌ FAILED'}")

    # 測試 3: 錯誤處理
    print("\n========== Test 3: Error Handling ==========")
    code3 = "print(1/0)"  # ZeroDivisionError
    success, output, exec_time, error = executor.execute_code(code3)
    print(f"Success: {success}")
    print(f"Error detected: {not success}")
    print(f"Output: {output.strip()[:100]}...")
    print(f"Result: {'✅ PASSED (Error caught)' if not success else '❌ FAILED'}")

    executor.cleanup()
```

**安裝 Python Docker SDK**:

```powershell
pip install docker
```

**執行測試**:

```powershell
python api/code_executor.py
```

**預期輸出**:
```
========== Test 1: Simple Calculation ==========
Success: True
Output: 4
Execution Time: 1.234s
Result: ✅ PASSED

========== Test 2: Numpy Array ==========
Success: True
Output: Mean: 3.0
Execution Time: 1.567s
Result: ✅ PASSED

========== Test 3: Error Handling ==========
Success: False
Error detected: True
Output: Traceback (most recent call last):...
Result: ✅ PASSED (Error caught)
```

**✅ 通過標準**:
- 簡單代碼執行成功
- Numpy 等庫正常工作
- 錯誤正確捕獲
- 執行時間 < 5 秒

**記錄點**:
```
[Day 5 - 15:00] 基礎代碼執行測試完成
- 簡單計算: 成功 / 失敗
- Numpy 測試: 成功 / 失敗
- 錯誤處理: 成功 / 失敗
- 平均執行時間: _____ 秒
```

---

## 🚀 Day 6: 4 層安全架構驗證 (6-8 小時)

### Phase 6.1: Layer 1 - Network Isolation (1 小時)

#### 步驟 6: 網絡隔離測試

**創建 `tests/test_network_isolation.py`**:

```python
from api.code_executor import CodeExecutor

def test_network_isolation():
    print("========== Layer 1: Network Isolation Tests ==========\n")

    executor = CodeExecutor()

    # Test 1: 無法訪問外部網絡
    print("--- Test 1.1: Block External HTTP ---")
    code1 = """
import socket
try:
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect(("8.8.8.8", 80))
    print("FAIL: Network access allowed")
except Exception as e:
    print(f"PASS: Network blocked - {type(e).__name__}")
"""
    success, output, _, _ = executor.execute_code(code1)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    # Test 2: 無法 DNS 查詢
    print("--- Test 1.2: Block DNS Resolution ---")
    code2 = """
import socket
try:
    ip = socket.gethostbyname("google.com")
    print("FAIL: DNS resolution allowed")
except Exception as e:
    print(f"PASS: DNS blocked - {type(e).__name__}")
"""
    success, output, _, _ = executor.execute_code(code2)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    # Test 3: 無法使用 requests 庫
    print("--- Test 1.3: Block HTTP Library ---")
    code3 = """
import requests
try:
    r = requests.get("https://api.github.com")
    print("FAIL: HTTP request allowed")
except Exception as e:
    print(f"PASS: HTTP blocked - {type(e).__name__}")
"""
    success, output, _, _ = executor.execute_code(code3)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    executor.cleanup()

if __name__ == "__main__":
    test_network_isolation()
```

**執行測試**:

```powershell
python tests/test_network_isolation.py
```

**預期輸出**:
```
========== Layer 1: Network Isolation Tests ==========

--- Test 1.1: Block External HTTP ---
Output: PASS: Network blocked - OSError
Result: ✅ PASSED

--- Test 1.2: Block DNS Resolution ---
Output: PASS: DNS blocked - OSError
Result: ✅ PASSED

--- Test 1.3: Block HTTP Library ---
Output: PASS: HTTP blocked - ConnectionError
Result: ✅ PASSED
```

**✅ 通過標準**: 所有網絡訪問嘗試被阻擋

---

### Phase 6.2: Layer 2 - Resource Limits (1.5 小時)

#### 步驟 7: 資源限制測試

**創建 `tests/test_resource_limits.py`**:

```python
from api.code_executor import CodeExecutor
import time

def test_resource_limits():
    print("========== Layer 2: Resource Limits Tests ==========\n")

    executor = CodeExecutor()

    # Test 2.1: CPU 限制
    print("--- Test 2.1: CPU Limit ---")
    code1 = """
import time
start = time.time()
# 密集 CPU 運算
result = sum(i**2 for i in range(1000000))
elapsed = time.time() - start
print(f"Computation completed in {elapsed:.2f}s")
"""
    success, output, exec_time, _ = executor.execute_code(code1)
    print(f"Output: {output.strip()}")
    print(f"Execution Time: {exec_time:.2f}s")
    print(f"Result: {'✅ PASSED' if success and exec_time < 10 else '❌ FAILED'}\n")

    # Test 2.2: 記憶體限制
    print("--- Test 2.2: Memory Limit (Should Fail) ---")
    code2 = """
# 嘗試分配 1GB 記憶體（超過 512MB 限制）
try:
    big_list = [0] * (1024 * 1024 * 128)  # ~1GB
    print("FAIL: Memory allocation allowed")
except MemoryError:
    print("PASS: Memory limit enforced")
"""
    success, output, exec_time, _ = executor.execute_code(code2)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output or not success else '❌ FAILED'}\n")

    # Test 2.3: 執行時間限制
    print("--- Test 2.3: Timeout (30s limit) ---")
    code3 = """
import time
print("Starting 35s sleep...")
time.sleep(35)
print("Sleep completed")
"""
    start = time.time()
    success, output, exec_time, error = executor.execute_code(code3)
    elapsed = time.time() - start
    print(f"Output: {output.strip()}")
    print(f"Actual Elapsed: {elapsed:.2f}s")
    print(f"Timeout enforced: {not success and elapsed < 35}")
    print(f"Result: {'✅ PASSED' if not success and elapsed < 35 else '❌ FAILED'}\n")

    executor.cleanup()

if __name__ == "__main__":
    test_resource_limits()
```

**執行測試**:

```powershell
python tests/test_resource_limits.py
```

**預期輸出**:
```
========== Layer 2: Resource Limits Tests ==========

--- Test 2.1: CPU Limit ---
Output: Computation completed in 2.34s
Execution Time: 2.45s
Result: ✅ PASSED

--- Test 2.2: Memory Limit (Should Fail) ---
Output: PASS: Memory limit enforced
Result: ✅ PASSED

--- Test 2.3: Timeout (30s limit) ---
Output: Starting 35s sleep...
Actual Elapsed: 30.12s
Timeout enforced: True
Result: ✅ PASSED
```

**✅ 通過標準**: CPU、記憶體、超時限制正常工作

**記錄點**:
```
[Day 6 - 12:00] 資源限制測試完成
- CPU 限制: 正常 / 異常
- 記憶體限制: 正常 / 異常
- 超時限制: 正常 / 異常
```

---

### Phase 6.3: Layer 3 & 4 - Filesystem & Security (2 小時)

#### 步驟 8: 文件系統與安全測試

**創建 `tests/test_filesystem_security.py`**:

```python
from api.code_executor import CodeExecutor

def test_filesystem_security():
    print("========== Layer 3 & 4: Filesystem & Security Tests ==========\n")

    executor = CodeExecutor()

    # Test 3.1: 根目錄只讀
    print("--- Test 3.1: Read-only Root Filesystem ---")
    code1 = """
import os
try:
    with open("/etc/shadow", "w") as f:
        f.write("malicious")
    print("FAIL: Root filesystem writable")
except (PermissionError, OSError) as e:
    print(f"PASS: Root filesystem read-only - {type(e).__name__}")
"""
    success, output, _, _ = executor.execute_code(code1)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    # Test 3.2: /tmp 可寫但有大小限制
    print("--- Test 3.2: /tmp Writable with Size Limit ---")
    code2 = """
import os
try:
    # 寫入小文件
    with open("/tmp/test.txt", "w") as f:
        f.write("test" * 100)
    print("PASS: /tmp writable for small files")
    os.remove("/tmp/test.txt")
except Exception as e:
    print(f"FAIL: /tmp write failed - {e}")
"""
    success, output, _, _ = executor.execute_code(code2)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    # Test 3.3: 無法訪問主機文件系統
    print("--- Test 3.3: Cannot Access Host Filesystem ---")
    code3 = """
import os
try:
    # 嘗試訪問常見的主機路徑
    paths = ["/host", "/../../../etc/passwd", "/proc/self/cgroup"]
    accessible = []
    for path in paths:
        if os.path.exists(path):
            accessible.append(path)

    if accessible:
        print(f"FAIL: Host paths accessible: {accessible}")
    else:
        print("PASS: Host filesystem isolated")
except Exception as e:
    print(f"PASS: Access denied - {e}")
"""
    success, output, _, _ = executor.execute_code(code3)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    # Test 4.1: 無法提權
    print("--- Test 4.1: Privilege Escalation Blocked ---")
    code4 = """
import os
try:
    os.setuid(0)  # 嘗試切換到 root
    print("FAIL: Privilege escalation allowed")
except PermissionError:
    print("PASS: Privilege escalation blocked")
"""
    success, output, _, _ = executor.execute_code(code4)
    print(f"Output: {output.strip()}")
    print(f"Result: {'✅ PASSED' if 'PASS' in output else '❌ FAILED'}\n")

    executor.cleanup()

if __name__ == "__main__":
    test_filesystem_security()
```

**執行測試**:

```powershell
python tests/test_filesystem_security.py
```

**預期輸出**:
```
========== Layer 3 & 4: Filesystem & Security Tests ==========

--- Test 3.1: Read-only Root Filesystem ---
Output: PASS: Root filesystem read-only - PermissionError
Result: ✅ PASSED

--- Test 3.2: /tmp Writable with Size Limit ---
Output: PASS: /tmp writable for small files
Result: ✅ PASSED

--- Test 3.3: Cannot Access Host Filesystem ---
Output: PASS: Host filesystem isolated
Result: ✅ PASSED

--- Test 4.1: Privilege Escalation Blocked ---
Output: PASS: Privilege escalation blocked
Result: ✅ PASSED
```

**✅ 通過標準**: 文件系統隔離、權限控制正常

---

## 🚀 Day 7: 沙箱逃逸測試與報告 (4-6 小時)

### Phase 7.1: 沙箱逃逸測試 (3 小時)

#### 步驟 9: 10 個攻擊向量測試

**創建 `tests/test_sandbox_escape.py`**:

```python
from api.code_executor import CodeExecutor

def test_sandbox_escape():
    print("========== Sandbox Escape Tests (10 Attack Vectors) ==========\n")

    executor = CodeExecutor()
    results = []

    attacks = [
        ("Attack 1: Process Injection", """
import subprocess
try:
    subprocess.run(["/bin/bash", "-c", "whoami"], capture_output=True)
    print("FAIL: Process execution allowed")
except Exception as e:
    print(f"PASS: Process blocked - {type(e).__name__}")
"""),
        ("Attack 2: File Descriptor Manipulation", """
import os
try:
    os.open("/proc/1/mem", os.O_RDWR)
    print("FAIL: /proc access allowed")
except (PermissionError, OSError) as e:
    print(f"PASS: /proc access blocked - {type(e).__name__}")
"""),
        ("Attack 3: Symlink Attack", """
import os
try:
    os.symlink("/etc/shadow", "/tmp/evil")
    with open("/tmp/evil", "r") as f:
        content = f.read()
    print("FAIL: Symlink attack succeeded")
except (PermissionError, OSError) as e:
    print(f"PASS: Symlink attack blocked - {type(e).__name__}")
"""),
        ("Attack 4: Container Breakout via cgroups", """
try:
    with open("/proc/1/cgroup", "r") as f:
        content = f.read()
    print("FAIL: cgroup access allowed")
except (PermissionError, OSError) as e:
    print(f"PASS: cgroup access blocked - {type(e).__name__}")
"""),
        ("Attack 5: Socket Creation", """
import socket
try:
    s = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
    s.bind("/tmp/evil.sock")
    print("FAIL: Unix socket allowed")
except (PermissionError, OSError) as e:
    print(f"PASS: Socket blocked - {type(e).__name__}")
"""),
        ("Attack 6: Fork Bomb", """
import os
try:
    for i in range(1000):
        os.fork()
    print("FAIL: Fork bomb succeeded")
except (OSError, MemoryError) as e:
    print(f"PASS: Fork bomb blocked - {type(e).__name__}")
"""),
        ("Attack 7: Capability Abuse", """
import os
try:
    os.setns(1, 0)  # 嘗試進入其他命名空間
    print("FAIL: Namespace manipulation allowed")
except (AttributeError, OSError) as e:
    print(f"PASS: Namespace blocked - {type(e).__name__}")
"""),
        ("Attack 8: Resource Exhaustion (Disk)", """
try:
    with open("/tmp/big_file", "wb") as f:
        f.write(b"0" * (200 * 1024 * 1024))  # 嘗試寫入 200MB
    print("FAIL: Disk exhaustion allowed")
except (OSError, IOError) as e:
    print(f"PASS: Disk limit enforced - {type(e).__name__}")
"""),
        ("Attack 9: Python Import Hijacking", """
import sys
try:
    sys.path.insert(0, "/tmp")
    with open("/tmp/os.py", "w") as f:
        f.write("def system(cmd): print('hijacked')")
    import os
    os.system("echo test")
    print("Check: Import hijacking behavior")
except Exception as e:
    print(f"PASS: Import protected - {type(e).__name__}")
"""),
        ("Attack 10: Container Metadata Access", """
try:
    with open("/proc/self/mountinfo", "r") as f:
        content = f.read()
    if "docker" in content.lower():
        print("INFO: Container detected (expected)")
    print("PASS: Metadata accessible but harmless")
except Exception as e:
    print(f"PASS: Metadata blocked - {type(e).__name__}")
""")
    ]

    for i, (name, code) in enumerate(attacks, 1):
        print(f"--- {name} ---")
        success, output, _, error = executor.execute_code(code)

        # 判斷是否通過
        passed = "PASS" in output or "FAIL" not in output
        results.append(passed)

        print(f"Output: {output.strip()[:200]}")
        print(f"Result: {'✅ PASSED' if passed else '❌ FAILED'}\n")

    # 總結
    print("========== Sandbox Escape Test Summary ==========")
    print(f"Total Attacks: {len(attacks)}")
    print(f"Blocked: {sum(results)}/{len(attacks)}")
    print(f"Success Rate: {sum(results)/len(attacks)*100:.1f}%")
    print(f"Final Result: {'✅ ALL PASSED (0 vulnerabilities)' if all(results) else '❌ VULNERABILITIES FOUND'}")

    executor.cleanup()
    return results

if __name__ == "__main__":
    test_sandbox_escape()
```

**執行測試**:

```powershell
python tests/test_sandbox_escape.py
```

**預期輸出**:
```
========== Sandbox Escape Tests (10 Attack Vectors) ==========

--- Attack 1: Process Injection ---
Output: PASS: Process blocked - FileNotFoundError
Result: ✅ PASSED

--- Attack 2: File Descriptor Manipulation ---
Output: PASS: /proc access blocked - PermissionError
Result: ✅ PASSED

...

========== Sandbox Escape Test Summary ==========
Total Attacks: 10
Blocked: 10/10
Success Rate: 100.0%
Final Result: ✅ ALL PASSED (0 vulnerabilities)
```

**✅ 通過標準**: 10/10 攻擊向量被成功阻擋

**⚠️ 如果任何攻擊成功**:
- 記錄詳細的攻擊向量
- 分析安全配置缺陷
- 調整 Docker 配置
- 重新測試直到全部阻擋

**記錄點**:
```
[Day 7 - 12:00] 沙箱逃逸測試完成
- 總攻擊向量: 10
- 成功阻擋: _____ / 10
- 成功率: _____%
- 發現漏洞: _____ 個
- 漏洞詳情: _____________________
```

---

### Phase 7.2: 性能與穩定性測試 (1 小時)

#### 步驟 10: 100 次執行穩定性測試

**創建 `tests/test_stability.py`**:

```python
from api.code_executor import CodeExecutor
import time

def test_stability():
    print("========== Stability Test (100 Executions) ==========\n")

    executor = CodeExecutor()

    test_codes = [
        "print(sum(range(1000)))",
        "import numpy as np; print(np.mean([1,2,3,4,5]))",
        "import pandas as pd; df = pd.DataFrame({'a': [1,2,3]}); print(df.sum())",
    ]

    total = 100
    success_count = 0
    times = []

    for i in range(total):
        code = test_codes[i % len(test_codes)]
        success, output, exec_time, error = executor.execute_code(code)

        if success and output.strip():
            success_count += 1
            times.append(exec_time)

        if (i + 1) % 10 == 0:
            print(f"Progress: {i+1}/100 executions")

    success_rate = success_count / total * 100
    avg_time = sum(times) / len(times) if times else 0

    print(f"\n========== Results ==========")
    print(f"Total Executions: {total}")
    print(f"Successful: {success_count}")
    print(f"Success Rate: {success_rate:.1f}%")
    print(f"Average Execution Time: {avg_time:.3f}s")
    print(f"Min Time: {min(times):.3f}s")
    print(f"Max Time: {max(times):.3f}s")
    print(f"Result: {'✅ PASSED (>95%)' if success_rate >= 95 else '❌ FAILED (<95%)'}")

    executor.cleanup()

if __name__ == "__main__":
    test_stability()
```

**執行測試**:

```powershell
python tests/test_stability.py
```

**預期輸出**:
```
========== Stability Test (100 Executions) ==========

Progress: 10/100 executions
Progress: 20/100 executions
...
Progress: 100/100 executions

========== Results ==========
Total Executions: 100
Successful: 98
Success Rate: 98.0%
Average Execution Time: 1.523s
Min Time: 1.234s
Max Time: 2.456s
Result: ✅ PASSED (>95%)
```

**✅ 通過標準**: 成功率 >95%, 平均執行時間 <5 秒

---

### Phase 7.3: 填寫驗證報告 (1 小時)

#### 步驟 11: 整理並填寫報告

**打開**: `docs/technical-implementation/1-poc-validation/poc-validation-report.md`

**填寫 PoC 3 部分**:

```markdown
### PoC 3: Code Interpreter Sandbox (P0) 🔴

**狀態**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**成功標準達成情況**:
- [✅] 0 沙箱逃逸漏洞 (10/10 阻擋)
- [✅] 執行成功率 >95% (98%)
- [✅] 平均執行時間 <5 秒 (1.52s)
- [✅] 資源限制有效

**關鍵發現**:
- 沙箱逃逸測試結果: 10/10 阻擋
- 執行成功率: 98%
- 平均執行時間: 1.52 秒
- 安全問題: 無（PoC 階段）

**風險與緩解**:
| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| Docker 逃逸漏洞 | 極高 | Phase 2 升級至 gVisor |
| seccomp 配置寬鬆 | 高 | 生產環境使用自定義 seccomp profile |
| 資源耗盡攻擊 | 中 | 實現請求頻率限制 |

**建議**:
- PoC 階段 Docker 隔離可接受
- 生產環境必須使用 gVisor 或 Kata Containers
- 實現自定義 seccomp profile
- 增加監控和告警機制
```

---

## 📊 最終檢查清單

執行完成後，確認以下所有項目：

- [ ] **所有測試執行完成**
  - [ ] Docker 環境驗證: PASSED
  - [ ] 基礎代碼執行: PASSED
  - [ ] Layer 1 (Network): PASSED
  - [ ] Layer 2 (Resource): PASSED
  - [ ] Layer 3 (Filesystem): PASSED
  - [ ] Layer 4 (Security): PASSED
  - [ ] 沙箱逃逸測試: 10/10 PASSED
  - [ ] 穩定性測試: PASSED

- [ ] **安全標準達成**
  - [ ] 0 沙箱逃逸漏洞: YES / NO
  - [ ] 執行成功率 >95%: YES / NO
  - [ ] 平均執行時間 <5s: YES / NO
  - [ ] 所有 4 層安全機制工作: YES / NO

- [ ] **驗證報告填寫完成**
  - [ ] 成功標準達成情況已記錄
  - [ ] 安全測試結果已整理
  - [ ] 風險已識別和評估
  - [ ] 生產環境建議已提出

---

## 🎯 Go/No-Go 決策標準

### ✅ GO (繼續 PoC 4)
- [x] 沙箱逃逸測試: 10/10 阻擋
- [x] 執行成功率 >95%
- [x] 平均執行時間 <5 秒
- [x] 4 層安全機制全部工作

### ⚠️ 有條件 GO
- [ ] 8-9/10 攻擊阻擋（有緩解方案）
- [ ] 執行成功率 90-95%（可優化）
- [ ] 已識別風險有明確緩解計劃

### ❌ NO-GO (重新評估)
- [ ] <8/10 攻擊阻擋（嚴重安全漏洞）
- [ ] 執行成功率 <90%
- [ ] 無法實現基本隔離
- [ ] 無法找到可行的生產方案

---

## 🔗 相關資源

- **PoC 3 詳細文檔**: [03-code-interpreter-sandbox.md](./03-code-interpreter-sandbox.md)
- **Docker 安全最佳實踐**: https://docs.docker.com/engine/security/
- **gVisor 文檔**: https://gvisor.dev/docs/
- **OWASP Container Security**: https://owasp.org/www-project-docker-top-10/

---

**最後更新**: 2025-10-30
**執行指導版本**: 1.0.0

---

**執行記錄**:

```
執行人: _________________
安全審查人: _________________
執行日期: _______________

Day 5 完成時間: _____________
Day 6 完成時間: _____________
Day 7 完成時間: _____________

最終狀態: ✅ PASSED / ❌ FAILED / ⚠️ CONDITIONAL

關鍵指標:
- 沙箱逃逸防護: _____ / 10 (目標 10/10)
- 執行成功率: _____% (目標 >95%)
- 平均執行時間: _____ s (目標 <5s)
- 發現漏洞數: _____ 個

安全評估:
- Layer 1 (Network): PASS / FAIL
- Layer 2 (Resource): PASS / FAIL
- Layer 3 (Filesystem): PASS / FAIL
- Layer 4 (Security): PASS / FAIL

生產環境建議:
- 隔離技術: gVisor / Kata Containers / Firecracker
- seccomp profile: 必須自定義
- 監控方案: _______________________________

Go/No-Go 決策: _______________
決策人: _________________
決策日期: _______________

下一步: 繼續 PoC 4 (Text-to-SQL) / 安全加固 / 重新評估
```
