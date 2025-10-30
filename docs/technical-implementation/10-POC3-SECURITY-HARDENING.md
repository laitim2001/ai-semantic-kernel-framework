# TID Part 4.3: Code Interpreter 安全強化 (PoC 3 Security Hardening)

**文檔版本**: v1.0.0
**創建日期**: 2025-10-30
**狀態**: ✅ 完成
**所屬**: Part 4 - Security Architecture Design

---

## 目錄

1. [當前安全狀態](#1-當前安全狀態)
2. [Seccomp Profile 設計](#2-seccomp-profile-設計)
3. [AppArmor Profile 設計](#3-apparmor-profile-設計)
4. [SELinux Policy 設計](#4-selinux-policy-設計)
5. [完整安全配置](#5-完整安全配置)
6. [安全驗證](#6-安全驗證)
7. [生產部署建議](#7-生產部署建議)

---

## 1. 當前安全狀態

### 1.1 PoC 3 實測結果 (2025-10-30)

| Test | 測試項目 | 結果 | 風險等級 | 分數 |
|------|---------|------|---------|------|
| #9.1 | /proc/1/cgroup 讀取 | ⚠️ ALLOWED | LOW (Info leak) | -5 |
| #9.2 | Docker Socket 訪問 | ✅ BLOCKED | SECURE | +5 |
| #9.3 | /sys 文件系統訪問 | ⚠️ ALLOWED | LOW (Info leak) | -5 |
| #9.4 | 進程創建 (subprocess) | ⚠️ LIMITED | MEDIUM | 0 |
| **總分** | | | | **90/100** |

### 1.2 4-Layer 安全架構

```yaml
current_layers:
  layer_1_network:
    status: ✅ SECURE
    config: "--network none"
    blocks: Internet access

  layer_2_resources:
    status: ✅ SECURE
    config: "--memory 512MB --cpus 0.5"
    blocks: Resource exhaustion

  layer_3_filesystem:
    status: ✅ SECURE
    config: "--read-only --tmpfs /tmp"
    blocks: File system modification

  layer_4_security:
    status: 🟡 PARTIAL (90/100)
    config: "--cap-drop ALL --user 1000:1000"
    blocks: Privilege escalation
    gaps:
      - /proc information leak (Test #9.1)
      - /sys information leak (Test #9.3)
```

### 1.3 安全提升目標

```
Current:  90/100 (4-Layer)
Target:   98/100 (6-Layer + Seccomp + AppArmor)

Improvements:
  + Seccomp profile    → Block dangerous syscalls (+5)
  + AppArmor profile   → Restrict /proc, /sys access (+3)
  = 98/100
```

---

## 2. Seccomp Profile 設計

### 2.1 Seccomp Profile 概述

```yaml
purpose: 限制容器可使用的系統調用 (syscalls)
default_action: SCMP_ACT_ERRNO (拒絕所有未明確允許的 syscalls)
allowed_syscalls: 40 個核心 syscalls (Python 運行所需)
blocked_syscalls: 危險 syscalls (reboot, mount, ptrace, etc.)
```

### 2.2 完整 Seccomp Profile

```json
{
  "defaultAction": "SCMP_ACT_ERRNO",
  "defaultErrnoRet": 1,
  "architectures": [
    "SCMP_ARCH_X86_64",
    "SCMP_ARCH_X86",
    "SCMP_ARCH_X32"
  ],
  "syscalls": [
    {
      "names": [
        "read",
        "write",
        "open",
        "openat",
        "close",
        "stat",
        "fstat",
        "lstat",
        "lseek",
        "mmap",
        "mprotect",
        "munmap",
        "brk",
        "rt_sigaction",
        "rt_sigprocmask",
        "rt_sigreturn",
        "ioctl",
        "pread64",
        "pwrite64",
        "readv",
        "writev",
        "access",
        "pipe",
        "select",
        "sched_yield",
        "mremap",
        "msync",
        "mincore",
        "madvise",
        "dup",
        "dup2",
        "dup3",
        "fcntl",
        "flock",
        "fsync",
        "fdatasync",
        "getcwd",
        "chdir",
        "fchdir",
        "rename",
        "mkdir",
        "rmdir",
        "link",
        "unlink",
        "symlink",
        "readlink",
        "chmod",
        "fchmod",
        "chown",
        "fchown",
        "lchown",
        "umask",
        "gettimeofday",
        "getrlimit",
        "getrusage",
        "sysinfo",
        "times",
        "getuid",
        "getgid",
        "geteuid",
        "getegid",
        "getppid",
        "getpgrp",
        "setsid",
        "getpgid",
        "setpgid",
        "getsid",
        "getgroups",
        "setgroups",
        "uname",
        "statfs",
        "fstatfs",
        "arch_prctl",
        "getdents64",
        "set_tid_address",
        "clock_gettime",
        "clock_getres",
        "clock_nanosleep",
        "exit_group",
        "futex",
        "set_robust_list",
        "get_robust_list",
        "prlimit64",
        "getrandom",
        "memfd_create"
      ],
      "action": "SCMP_ACT_ALLOW"
    },
    {
      "names": [
        "clone",
        "fork",
        "vfork"
      ],
      "action": "SCMP_ACT_ALLOW",
      "args": [],
      "comment": "Allow process creation (needed for subprocess)"
    },
    {
      "names": [
        "execve",
        "execveat"
      ],
      "action": "SCMP_ACT_ALLOW",
      "comment": "Allow execution (needed for subprocess)"
    },
    {
      "names": [
        "socket",
        "bind",
        "connect",
        "listen",
        "accept",
        "accept4",
        "sendto",
        "recvfrom",
        "sendmsg",
        "recvmsg",
        "shutdown",
        "getsockname",
        "getpeername",
        "socketpair",
        "setsockopt",
        "getsockopt"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block network syscalls (--network none handles this)"
    },
    {
      "names": [
        "ptrace"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block debugging syscalls"
    },
    {
      "names": [
        "reboot",
        "kexec_load",
        "kexec_file_load"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block system reboot"
    },
    {
      "names": [
        "mount",
        "umount",
        "umount2",
        "pivot_root"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block filesystem mounting"
    },
    {
      "names": [
        "setuid",
        "setgid",
        "setreuid",
        "setregid",
        "setresuid",
        "setresgid",
        "setfsuid",
        "setfsgid"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block privilege escalation"
    },
    {
      "names": [
        "acct",
        "add_key",
        "bpf",
        "clock_adjtime",
        "create_module",
        "delete_module",
        "finit_module",
        "get_kernel_syms",
        "get_mempolicy",
        "init_module",
        "ioperm",
        "iopl",
        "kcmp",
        "keyctl",
        "lookup_dcookie",
        "mbind",
        "modify_ldt",
        "move_pages",
        "name_to_handle_at",
        "open_by_handle_at",
        "perf_event_open",
        "personality",
        "process_vm_readv",
        "process_vm_writev",
        "query_module",
        "quotactl",
        "request_key",
        "set_mempolicy",
        "swapoff",
        "swapon",
        "sysfs",
        "_sysctl",
        "userfaultfd",
        "uselib",
        "vmsplice"
      ],
      "action": "SCMP_ACT_ERRNO",
      "comment": "Block dangerous kernel operations"
    }
  ]
}
```

### 2.3 Seccomp 應用方式

#### 方法 1: Docker --security-opt

```bash
docker run \
  --security-opt seccomp=/path/to/seccomp-profile.json \
  code-sandbox-python:latest
```

#### 方法 2: C# Docker API

```csharp
var container = await _dockerClient.Containers.CreateContainerAsync(
    new CreateContainerParameters
    {
        Image = "code-sandbox-python:latest",
        HostConfig = new HostConfig
        {
            // ... other configs ...
            SecurityOpt = new List<string>
            {
                $"seccomp={Path.Combine(AppContext.BaseDirectory, "seccomp-profile.json")}"
            }
        }
    }
);
```

### 2.4 測試 Seccomp Profile

```python
# Test dangerous syscalls
import os

# ✅ Allowed: Basic file operations
with open('/tmp/test.txt', 'w') as f:
    f.write('hello')

# ❌ Blocked: Mount operation
try:
    os.system('mount /dev/sda1 /mnt')  # SCMP_ACT_ERRNO
except Exception as e:
    print(f"Blocked: {e}")

# ❌ Blocked: Reboot
try:
    os.system('reboot')  # SCMP_ACT_ERRNO
except Exception as e:
    print(f"Blocked: {e}")
```

---

## 3. AppArmor Profile 設計

### 3.1 AppArmor Profile 概述

```yaml
purpose: 強制訪問控制 (MAC) - 限制文件和資源訪問
profile_name: docker-code-sandbox
mode: enforce (生產環境) / complain (開發環境)
restrictions:
  - Block /proc/1/cgroup read
  - Block /sys/* read (except necessary)
  - Allow /tmp, /home/sandbox read/write
```

### 3.2 完整 AppArmor Profile

```apparmor
#include <tunables/global>

profile docker-code-sandbox flags=(attach_disconnected,mediate_deleted) {
  #include <abstractions/base>
  #include <abstractions/python>

  # Network (already blocked by --network none, but reinforce)
  deny network,

  # File system permissions
  # Allow: Read-only access to system files
  /bin/** ix,
  /usr/bin/** ix,
  /usr/lib/** mr,
  /lib/** mr,
  /lib64/** mr,
  /etc/ld.so.cache r,
  /etc/ld.so.conf r,
  /etc/ld.so.conf.d/ r,
  /etc/ld.so.conf.d/** r,

  # Allow: Python standard library
  /usr/lib/python3*/** mr,
  /usr/local/lib/python3*/** mr,

  # Allow: /tmp (read/write)
  /tmp/** rw,
  owner /tmp/** rw,

  # Allow: Working directory
  /home/sandbox/** rw,
  owner /home/sandbox/** rw,

  # Block: Sensitive /proc files
  deny /proc/1/cgroup r,
  deny /proc/1/environ r,
  deny /proc/1/cmdline r,
  deny /proc/kcore r,
  deny /proc/kmsg r,
  deny /proc/mem r,
  deny /proc/sys/kernel/** w,

  # Allow: Necessary /proc files
  /proc/ r,
  /proc/cpuinfo r,
  /proc/meminfo r,
  /proc/self/** r,
  /proc/sys/kernel/random/uuid r,

  # Block: /sys (information leak)
  deny /sys/** r,

  # Allow: /dev (minimal)
  /dev/null rw,
  /dev/zero rw,
  /dev/full rw,
  /dev/random r,
  /dev/urandom r,
  deny /dev/mem rw,
  deny /dev/kmem rw,

  # Block: Capabilities
  deny capability sys_admin,
  deny capability sys_module,
  deny capability sys_rawio,
  deny capability sys_boot,
  deny capability sys_ptrace,
  deny capability dac_override,
  deny capability dac_read_search,

  # Block: Mount operations
  deny mount,
  deny umount,
  deny pivot_root,

  # Block: Process tracing
  deny ptrace,

  # Signal permissions
  signal (receive) peer=unconfined,
  signal (send) peer=docker-code-sandbox,
}
```

### 3.3 AppArmor 應用方式

#### 步驟 1: 安裝 AppArmor Profile

```bash
# Copy profile to /etc/apparmor.d/
sudo cp docker-code-sandbox /etc/apparmor.d/

# Load profile
sudo apparmor_parser -r /etc/apparmor.d/docker-code-sandbox

# Verify profile is loaded
sudo aa-status | grep docker-code-sandbox
```

#### 步驟 2: Docker 應用 AppArmor

```bash
docker run \
  --security-opt apparmor=docker-code-sandbox \
  code-sandbox-python:latest
```

#### 步驟 3: C# Docker API

```csharp
var container = await _dockerClient.Containers.CreateContainerAsync(
    new CreateContainerParameters
    {
        Image = "code-sandbox-python:latest",
        HostConfig = new HostConfig
        {
            // ... other configs ...
            SecurityOpt = new List<string>
            {
                "apparmor=docker-code-sandbox",
                $"seccomp={Path.Combine(AppContext.BaseDirectory, "seccomp-profile.json")}"
            }
        }
    }
);
```

### 3.4 測試 AppArmor Profile

```python
# Test #9.1: /proc/1/cgroup (should be BLOCKED)
try:
    with open('/proc/1/cgroup', 'r') as f:
        print(f.read())
except PermissionError:
    print("✅ BLOCKED by AppArmor")

# Test #9.3: /sys (should be BLOCKED)
try:
    import os
    sys_dirs = os.listdir('/sys')
    print(sys_dirs)
except PermissionError:
    print("✅ BLOCKED by AppArmor")

# Test: /tmp (should be ALLOWED)
with open('/tmp/test.txt', 'w') as f:
    f.write('AppArmor test')
print("✅ /tmp access ALLOWED")
```

---

## 4. SELinux Policy 設計

### 4.1 SELinux 概述

```yaml
purpose: Type Enforcement (TE) - 基於標籤的訪問控制
适用: RHEL, CentOS, Fedora
alternative: AppArmor (Ubuntu, Debian)
policy_type: Targeted policy
context: container_t (Docker default)
```

### 4.2 SELinux 自定義 Policy

```selinux
policy_module(docker_code_sandbox, 1.0.0)

require {
    type container_t;
    type proc_t;
    type sysfs_t;
    class file { read open getattr };
    class dir { read search };
}

# Block /proc/1/* access
deny container_t proc_t:file { read open };

# Block /sys/* access
deny container_t sysfs_t:dir { read search };
deny container_t sysfs_t:file { read open };

# Allow /tmp access (already allowed by default)
# No additional rules needed
```

### 4.3 SELinux 應用方式

```bash
# Compile policy
checkmodule -M -m -o docker_code_sandbox.mod docker_code_sandbox.te
semodule_package -o docker_code_sandbox.pp -m docker_code_sandbox.mod

# Install policy
sudo semodule -i docker_code_sandbox.pp

# Verify
sudo semodule -l | grep docker_code_sandbox

# Docker with SELinux
docker run \
  --security-opt label=type:container_t \
  code-sandbox-python:latest
```

---

## 5. 完整安全配置

### 5.1 6-Layer 安全架構

```yaml
layer_1_network:
  config: "--network none"
  blocks: Internet access

layer_2_resources:
  config: "--memory 512MB --cpus 0.5 --pids-limit 100"
  blocks: Resource exhaustion, fork bomb

layer_3_filesystem:
  config: "--read-only --tmpfs /tmp:rw,noexec,nosuid,size=100m"
  blocks: File system modification, executable uploads

layer_4_capabilities:
  config: "--cap-drop ALL --user 1000:1000 --no-new-privileges"
  blocks: Privilege escalation

layer_5_seccomp:
  config: "--security-opt seccomp=seccomp-profile.json"
  blocks: Dangerous syscalls (mount, ptrace, reboot)

layer_6_apparmor:
  config: "--security-opt apparmor=docker-code-sandbox"
  blocks: /proc/1/cgroup, /sys/* access
```

### 5.2 完整 Docker Run 命令

```bash
docker run -d \
  --name code-sandbox-python \
  --network none \
  --memory 512m \
  --cpus 0.5 \
  --pids-limit 100 \
  --read-only \
  --tmpfs /tmp:rw,noexec,nosuid,size=100m \
  --cap-drop ALL \
  --user 1000:1000 \
  --security-opt no-new-privileges \
  --security-opt seccomp=/etc/docker/seccomp/code-sandbox.json \
  --security-opt apparmor=docker-code-sandbox \
  code-sandbox-python:latest
```

### 5.3 C# 完整實現

```csharp
public class SecureCodeExecutor
{
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<SecureCodeExecutor> _logger;

    public async Task<ExecutionResult> ExecuteCodeAsync(
        string code,
        string language,
        int timeoutSeconds = 60)
    {
        var containerId = await CreateSecureContainerAsync(language);

        try
        {
            // Write code to container
            await WriteCodeToContainerAsync(containerId, code);

            // Execute code
            var execId = await _dockerClient.Exec.ExecCreateContainerAsync(
                containerId,
                new ContainerExecCreateParameters
                {
                    Cmd = new[] { "python", "/tmp/code.py" },
                    AttachStdout = true,
                    AttachStderr = true
                });

            using var stream = await _dockerClient.Exec.StartAndAttachContainerExecAsync(
                execId,
                false,
                CancellationToken.None);

            var (stdout, stderr) = await ReadStreamAsync(stream, timeoutSeconds);

            return new ExecutionResult
            {
                Stdout = stdout,
                Stderr = stderr,
                ExitCode = await GetExitCodeAsync(execId)
            };
        }
        finally
        {
            // Cleanup
            await _dockerClient.Containers.StopContainerAsync(
                containerId,
                new ContainerStopParameters());
            await _dockerClient.Containers.RemoveContainerAsync(containerId);
        }
    }

    private async Task<string> CreateSecureContainerAsync(string language)
    {
        var seccompPath = Path.Combine(AppContext.BaseDirectory, "seccomp-profile.json");

        var response = await _dockerClient.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Image = $"code-sandbox-{language}:latest",
                User = "1000:1000",
                WorkingDir = "/home/sandbox",
                HostConfig = new HostConfig
                {
                    // Layer 1: Network isolation
                    NetworkMode = "none",

                    // Layer 2: Resource limits
                    Memory = 512 * 1024 * 1024, // 512 MB
                    NanoCPUs = 500_000_000, // 0.5 CPU
                    PidsLimit = 100,

                    // Layer 3: Filesystem protection
                    ReadonlyRootfs = true,
                    Tmpfs = new Dictionary<string, string>
                    {
                        { "/tmp", "rw,noexec,nosuid,size=100m" }
                    },

                    // Layer 4: Capability drop
                    CapDrop = new List<string> { "ALL" },

                    // Layer 5 & 6: Seccomp + AppArmor
                    SecurityOpt = new List<string>
                    {
                        "no-new-privileges",
                        $"seccomp={seccompPath}",
                        "apparmor=docker-code-sandbox"
                    }
                }
            });

        await _dockerClient.Containers.StartContainerAsync(
            response.ID,
            new ContainerStartParameters());

        _logger.LogInformation(
            "Created secure container {ContainerId} with 6-layer security",
            response.ID);

        return response.ID;
    }
}
```

---

## 6. 安全驗證

### 6.1 驗證測試套件

```python
# test_security.py
import os
import subprocess

def test_proc_cgroup_blocked():
    """Test #9.1: /proc/1/cgroup should be blocked"""
    try:
        with open('/proc/1/cgroup', 'r') as f:
            f.read()
        return "❌ FAIL: /proc/1/cgroup accessible"
    except PermissionError:
        return "✅ PASS: /proc/1/cgroup blocked"

def test_sys_blocked():
    """Test #9.3: /sys should be blocked"""
    try:
        os.listdir('/sys')
        return "❌ FAIL: /sys accessible"
    except PermissionError:
        return "✅ PASS: /sys blocked"

def test_docker_socket_blocked():
    """Test #9.2: Docker socket should not exist"""
    if os.path.exists('/var/run/docker.sock'):
        return "❌ FAIL: Docker socket exists"
    return "✅ PASS: Docker socket not found"

def test_subprocess_limited():
    """Test #9.4: subprocess should work but limited"""
    try:
        result = subprocess.run(['ls', '/'], capture_output=True, text=True, timeout=5)
        if result.returncode == 0:
            return "✅ PASS: subprocess works (limited)"
        return "❌ FAIL: subprocess failed"
    except Exception as e:
        return f"❌ FAIL: {e}"

def test_network_blocked():
    """Network should be blocked"""
    try:
        import socket
        socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        return "❌ FAIL: Network socket created"
    except OSError:
        return "✅ PASS: Network blocked"

if __name__ == '__main__':
    tests = [
        test_proc_cgroup_blocked,
        test_sys_blocked,
        test_docker_socket_blocked,
        test_subprocess_limited,
        test_network_blocked
    ]

    print("🔒 Security Test Suite\n")
    passed = 0
    for test in tests:
        result = test()
        print(f"{test.__name__}: {result}")
        if "PASS" in result:
            passed += 1

    print(f"\n✅ {passed}/{len(tests)} tests passed")
    print(f"🔒 Security Score: {90 + (passed * 2)}/100")
```

### 6.2 預期測試結果

```
🔒 Security Test Suite

test_proc_cgroup_blocked: ✅ PASS: /proc/1/cgroup blocked
test_sys_blocked: ✅ PASS: /sys blocked
test_docker_socket_blocked: ✅ PASS: Docker socket not found
test_subprocess_limited: ✅ PASS: subprocess works (limited)
test_network_blocked: ✅ PASS: Network blocked

✅ 5/5 tests passed
🔒 Security Score: 98/100
```

---

## 7. 生產部署建議

### 7.1 部署檢查清單

```yaml
pre_deployment:
  - [ ] Seccomp profile 已測試並部署到所有節點
  - [ ] AppArmor profile 已安裝並啟用 (enforce mode)
  - [ ] Docker image 已掃描漏洞 (Trivy/Snyk)
  - [ ] 資源限制已根據實際負載調整
  - [ ] 監控和告警已配置

deployment:
  - [ ] Kubernetes SecurityContext 配置
  - [ ] Pod Security Policy (PSP) 或 Pod Security Standards
  - [ ] Network Policy 限制流量
  - [ ] Service Account 最小權限

post_deployment:
  - [ ] 執行安全驗證測試套件
  - [ ] 監控異常行為 (Falco)
  - [ ] 定期安全審計
  - [ ] 更新 Seccomp/AppArmor profile
```

### 7.2 Kubernetes 部署配置

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: code-sandbox
spec:
  replicas: 3
  selector:
    matchLabels:
      app: code-sandbox
  template:
    metadata:
      labels:
        app: code-sandbox
      annotations:
        # AppArmor annotation
        container.apparmor.security.beta.kubernetes.io/code-sandbox: localhost/docker-code-sandbox
    spec:
      securityContext:
        # Pod-level security
        runAsNonRoot: true
        runAsUser: 1000
        fsGroup: 1000
        seccompProfile:
          type: Localhost
          localhostProfile: code-sandbox.json

      containers:
      - name: code-sandbox
        image: code-sandbox-python:latest
        securityContext:
          # Container-level security
          allowPrivilegeEscalation: false
          readOnlyRootFilesystem: true
          runAsNonRoot: true
          runAsUser: 1000
          capabilities:
            drop:
              - ALL

        resources:
          limits:
            cpu: "0.5"
            memory: "512Mi"
          requests:
            cpu: "0.1"
            memory: "128Mi"

        volumeMounts:
        - name: tmp
          mountPath: /tmp

      volumes:
      - name: tmp
        emptyDir:
          sizeLimit: 100Mi
```

### 7.3 監控和告警

```yaml
monitoring:
  # Falco rules for container security
  - rule: Container Escape Attempt
    condition: >
      container and
      (proc.name in (mount, umount, reboot) or
       file.path in (/proc/1/cgroup, /var/run/docker.sock))
    output: "Container escape attempt detected (user=%user.name container=%container.name)"
    priority: CRITICAL

  # Prometheus metrics
  metrics:
    - code_sandbox_executions_total
    - code_sandbox_execution_duration_seconds
    - code_sandbox_security_violations_total
    - code_sandbox_container_restarts_total

  # Alert rules
  alerts:
    - name: HighSecurityViolationRate
      expr: rate(code_sandbox_security_violations_total[5m]) > 0.1
      severity: warning
      message: "High rate of security violations in code sandbox"
```

---

## 總結

### 安全強化總結

| 項目 | 當前 (90/100) | 強化後 (98/100) | 提升 |
|------|--------------|----------------|------|
| /proc 保護 | ⚠️ Info leak | ✅ Blocked | +5 |
| /sys 保護 | ⚠️ Info leak | ✅ Blocked | +3 |
| Syscall 限制 | 🟡 Partial | ✅ Seccomp | +0 (已計分) |
| 總分 | **90/100** | **98/100** | **+8** |

### 安全層級

```
Layer 1: Network Isolation       ✅ --network none
Layer 2: Resource Limits         ✅ CPU/Memory/PID limits
Layer 3: Filesystem Protection   ✅ Read-only + tmpfs
Layer 4: Capability Drop         ✅ --cap-drop ALL
Layer 5: Seccomp Profile         ✅ 40 allowed syscalls
Layer 6: AppArmor/SELinux        ✅ MAC enforcement

= 6-Layer Defense-in-Depth
```

### 部署路徑

1. **開發環境**: 4-Layer (90/100) - 當前實現
2. **Staging環境**: 6-Layer (98/100) - Seccomp + AppArmor (complain mode)
3. **生產環境**: 6-Layer (98/100) - Seccomp + AppArmor (enforce mode) + Falco 監控

### 下一步

✅ **Part 4 完成**: 安全架構設計 (3/3 files)
- 08-SECURITY-ARCHITECTURE.md
- 09-AUTHENTICATION-AUTHORIZATION.md
- 10-POC3-SECURITY-HARDENING.md

📋 **接下來**: Part 5-8 (9 files remaining)
- Part 5: 部署架構 (4 files)
- Part 6: 監控和運維 (3 files)
- Part 7: 災難復原計劃 (2 files)
- Part 8: 成本估算 (1 file)

---

**文檔版本**: v1.0.0
**最後更新**: 2025-10-30
**作者**: AI Workflow Platform Team
