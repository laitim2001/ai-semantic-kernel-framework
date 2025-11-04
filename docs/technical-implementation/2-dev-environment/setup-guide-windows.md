# Windows 開發環境設置指南

## 版本信息
- **文檔版本**: 1.0
- **最後更新**: 2025-01-01
- **適用平台**: Windows 10/11 (64-bit)

## 前置條件

### 系統要求
- **作業系統**: Windows 10 (version 1909+) 或 Windows 11
- **處理器**: Intel Core i5 或 AMD Ryzen 5 以上
- **記憶體**: 最小 8GB RAM (推薦 16GB+)
- **硬碟空間**: 至少 50GB 可用空間 (推薦 100GB+)
- **網路**: 穩定的網際網路連接

### 管理員權限
某些安裝步驟需要管理員權限。建議以管理員身份執行 PowerShell。

---

## 步驟 1: 安裝 .NET 8.0 SDK

### 1.1 下載 .NET SDK

訪問官方網站下載 .NET 8.0 SDK:
- **下載連結**: https://dotnet.microsoft.com/download/dotnet/8.0
- 選擇 **SDK 8.0.x** (x64) 版本

### 1.2 安裝 .NET SDK

1. 執行下載的安裝程序 (`dotnet-sdk-8.0.xxx-win-x64.exe`)
2. 按照安裝嚮導完成安裝
3. 安裝完成後重啟 PowerShell/命令提示字元

### 1.3 驗證安裝

```powershell
dotnet --version
```

**預期輸出**:
```
8.0.xxx
```

### 1.4 驗證 .NET Runtime

```powershell
dotnet --list-runtimes
```

**預期輸出** (應包含):
```
Microsoft.AspNetCore.App 8.0.x [C:\Program Files\dotnet\shared\...]
Microsoft.NETCore.App 8.0.x [C:\Program Files\dotnet\shared\...]
```

---

## 步驟 2: 安裝 Node.js 和 npm

### 2.1 下載 Node.js

訪問官方網站下載 Node.js:
- **下載連結**: https://nodejs.org/
- 選擇 **LTS** (Long Term Support) 版本
- 下載 Windows Installer (.msi) 64-bit

### 2.2 安裝 Node.js

1. 執行下載的安裝程序 (`node-vxx.xx.x-x64.msi`)
2. 安裝選項:
   - ✅ 自動安裝必要的工具
   - ✅ 添加到 PATH
   - ✅ 安裝 npm package manager
3. 完成安裝後重啟 PowerShell

### 2.3 驗證安裝

```powershell
node --version
npm --version
```

**預期輸出**:
```
v20.x.x  # Node.js version
10.x.x   # npm version
```

### 2.4 配置 npm (可選但推薦)

設置 npm 全局套件安裝位置:

```powershell
# 創建全局套件目錄
mkdir "$env:APPDATA\npm-global"

# 配置 npm 使用新目錄
npm config set prefix "$env:APPDATA\npm-global"

# 添加到 PATH (需要管理員權限)
[Environment]::SetEnvironmentVariable(
    "Path",
    "$env:Path;$env:APPDATA\npm-global",
    "User"
)
```

---

## 步驟 3: 安裝 Git

### 3.1 下載 Git for Windows

- **下載連結**: https://git-scm.com/download/win
- 選擇最新的 64-bit Git for Windows Setup

### 3.2 安裝配置

執行安裝程序並使用以下推薦設置:

1. **編輯器**: Visual Studio Code (如果已安裝) 或 Vim
2. **PATH 環境**: "Git from the command line and also from 3rd-party software"
3. **HTTPS 傳輸**: "Use the OpenSSL library"
4. **換行符轉換**: "Checkout Windows-style, commit Unix-style line endings"
5. **終端模擬器**: "Use Windows' default console window"
6. **Git Credential Manager**: 啟用

### 3.3 驗證安裝

```powershell
git --version
```

**預期輸出**:
```
git version 2.xx.x.windows.x
```

### 3.4 配置 Git

設置用戶信息:

```powershell
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
git config --global init.defaultBranch main
```

---

## 步驟 4: 安裝 Docker Desktop

### 4.1 系統要求檢查

啟用 Windows 功能 (需要管理員權限):

```powershell
# 啟用 WSL 2 (Windows Subsystem for Linux)
wsl --install

# 啟用虛擬機器平台
dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart

# 啟用 Hyper-V (Windows 10 Pro/Enterprise/Education)
Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All

# 重啟電腦
Restart-Computer
```

**注意**: Windows 10 Home 用戶只能使用 WSL 2 後端,不支援 Hyper-V。

### 4.2 下載 Docker Desktop

- **下載連結**: https://www.docker.com/products/docker-desktop
- 選擇 Docker Desktop for Windows

### 4.3 安裝 Docker Desktop

1. 執行安裝程序 (`Docker Desktop Installer.exe`)
2. 安裝選項:
   - ✅ Use WSL 2 instead of Hyper-V (推薦)
   - ✅ Add shortcut to desktop
3. 完成安裝後重啟電腦
4. 啟動 Docker Desktop (第一次啟動可能需要幾分鐘)

### 4.4 驗證安裝

```powershell
docker --version
docker-compose --version
```

**預期輸出**:
```
Docker version 24.x.x, build xxxxxxx
Docker Compose version v2.x.x
```

### 4.5 測試 Docker

```powershell
# 運行測試容器
docker run hello-world
```

**預期輸出**: 應該顯示 "Hello from Docker!" 訊息

---

## 步驟 5: 安裝 Visual Studio Code

### 5.1 下載 VS Code

- **下載連結**: https://code.visualstudio.com/
- 選擇 **Windows** 版本 (User Installer 64-bit)

### 5.2 安裝 VS Code

1. 執行安裝程序 (`VSCodeUserSetup-x64-x.xx.x.exe`)
2. 安裝選項 (推薦全選):
   - ✅ 添加到 PATH
   - ✅ 添加"通過 Code 打開"操作到 Windows 資源管理器文件上下文選單
   - ✅ 添加"通過 Code 打開"操作到 Windows 資源管理器目錄上下文選單
   - ✅ 將 Code 註冊為支援的文件類型的編輯器
3. 完成安裝

### 5.3 驗證安裝

```powershell
code --version
```

**預期輸出**:
```
1.xx.x
xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
x64
```

### 5.4 安裝推薦的 VS Code 擴展

打開 VS Code 並安裝以下擴展:

#### .NET 開發
```powershell
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

#### JavaScript/TypeScript 開發
```powershell
code --install-extension dbaeumer.vscode-eslint
code --install-extension esbenp.prettier-vscode
code --install-extension Vue.volar
```

#### 容器開發
```powershell
code --install-extension ms-azuretools.vscode-docker
```

#### 版本控制
```powershell
code --install-extension eamodio.gitlens
```

#### 其他推薦
```powershell
code --install-extension editorconfig.editorconfig
code --install-extension streetsidesoftware.code-spell-checker
```

---

## 步驟 6: 安裝 Azure CLI (可選)

如果需要管理 Azure 資源,安裝 Azure CLI:

### 6.1 下載並安裝

```powershell
# 使用 Windows Package Manager (winget)
winget install Microsoft.AzureCLI

# 或下載 MSI 安裝程序
# https://aka.ms/installazurecliwindows
```

### 6.2 驗證安裝

```powershell
az --version
```

### 6.3 登入 Azure

```powershell
az login
```

---

## 步驟 7: 克隆專案並設置

### 7.1 克隆專案倉庫

```powershell
# 導航到工作目錄
cd C:\Projects  # 或您偏好的目錄

# 克隆專案
git clone <repository-url>
cd <project-directory>
```

### 7.2 安裝後端依賴

```powershell
# 導航到後端專案目錄
cd backend  # 或對應的目錄名稱

# 還原 .NET 依賴
dotnet restore

# 驗證構建
dotnet build
```

### 7.3 安裝前端依賴

```powershell
# 導航到前端專案目錄
cd ../frontend  # 或對應的目錄名稱

# 安裝 npm 依賴
npm install

# 或使用 npm ci (更快,適合 CI/CD)
npm ci
```

### 7.4 複製環境變數文件

```powershell
# 後端
cp .env.example .env

# 前端
cp .env.local.example .env.local
```

編輯 `.env` 和 `.env.local` 文件,填入必要的配置值。

---

## 步驟 8: 啟動開發環境

### 8.1 啟動 Docker 服務

```powershell
# 確保 Docker Desktop 正在運行
# 啟動所有服務 (PostgreSQL, Redis, MongoDB)
docker-compose up -d

# 檢查服務狀態
docker-compose ps
```

**預期輸出**:
```
NAME                          STATUS              PORTS
aiworkflow-postgres           running             0.0.0.0:5432->5432/tcp
aiworkflow-redis              running             0.0.0.0:6379->6379/tcp
aiworkflow-mongodb            running             0.0.0.0:27017->27017/tcp
aiworkflow-pgadmin            running             0.0.0.0:5050->80/tcp
```

### 8.2 啟動後端 API

```powershell
# 在後端目錄中
cd backend
dotnet run
```

**預期輸出**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

### 8.3 啟動前端開發伺服器

在新的 PowerShell 視窗中:

```powershell
# 在前端目錄中
cd frontend
npm run dev
```

**預期輸出**:
```
  VITE v5.x.x  ready in xxx ms

  ➜  Local:   http://localhost:3000/
  ➜  Network: use --host to expose
  ➜  press h to show help
```

---

## 步驟 9: 驗證環境設置

### 9.1 檢查所有服務

打開瀏覽器並訪問:

- **前端應用**: http://localhost:3000
- **後端 API**: http://localhost:5001/swagger (Swagger UI)
- **pgAdmin**: http://localhost:5050
  - 預設登入: `admin@example.com` / `admin123`

### 9.2 運行環境檢查腳本

```powershell
# 在專案根目錄
.\scripts\check-environment.ps1
```

### 9.3 運行測試

```powershell
# 後端測試
cd backend
dotnet test

# 前端測試
cd ../frontend
npm test
```

---

## 常見問題排除

### 問題 1: .NET SDK 未找到

**錯誤**: `'dotnet' is not recognized as an internal or external command`

**解決方案**:
1. 確認 .NET SDK 已正確安裝
2. 重啟 PowerShell/命令提示字元
3. 檢查 PATH 環境變數:
   ```powershell
   $env:Path -split ';' | Select-String dotnet
   ```
4. 如果仍然失敗,手動添加到 PATH:
   ```powershell
   [Environment]::SetEnvironmentVariable(
       "Path",
       "$env:Path;C:\Program Files\dotnet\",
       "User"
   )
   ```

### 問題 2: Docker 無法啟動

**錯誤**: `Docker Desktop starting...` (無限等待)

**解決方案**:
1. 確認已啟用 WSL 2 或 Hyper-V
2. 檢查 BIOS 中是否啟用虛擬化 (VT-x/AMD-V)
3. 重啟 Docker Desktop
4. 如果仍失敗,重新安裝 Docker Desktop

### 問題 3: npm install 失敗

**錯誤**: `EACCES: permission denied` 或網路錯誤

**解決方案**:
1. 清除 npm 緩存:
   ```powershell
   npm cache clean --force
   ```
2. 刪除 `node_modules` 和 `package-lock.json`:
   ```powershell
   Remove-Item -Recurse -Force node_modules
   Remove-Item package-lock.json
   npm install
   ```
3. 如果是代理問題,配置 npm 代理:
   ```powershell
   npm config set proxy http://proxy.company.com:8080
   npm config set https-proxy http://proxy.company.com:8080
   ```

### 問題 4: 端口已被佔用

**錯誤**: `Port 5432 is already allocated` 或類似錯誤

**解決方案**:
1. 檢查佔用端口的進程:
   ```powershell
   netstat -ano | findstr :5432
   ```
2. 終止進程:
   ```powershell
   taskkill /PID <process-id> /F
   ```
3. 或修改 `docker-compose.yml` 使用不同端口

### 問題 5: Git 憑證問題

**錯誤**: Git push/pull 需要反覆輸入密碼

**解決方案**:
1. 配置 Git Credential Manager:
   ```powershell
   git config --global credential.helper manager-core
   ```
2. 使用 SSH 金鑰代替 HTTPS:
   ```powershell
   # 生成 SSH 金鑰
   ssh-keygen -t ed25519 -C "your.email@example.com"

   # 添加到 ssh-agent
   ssh-add ~/.ssh/id_ed25519

   # 將公鑰添加到 GitHub/GitLab
   cat ~/.ssh/id_ed25519.pub
   ```

---

## PowerShell 腳本工具

### 一鍵環境檢查腳本

創建 `scripts/check-environment.ps1`:

```powershell
# Environment Check Script
Write-Host "====================================" -ForegroundColor Cyan
Write-Host "  Environment Verification Script" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

# Check .NET SDK
Write-Host "`n[1/7] Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK not found" -ForegroundColor Red
}

# Check Node.js
Write-Host "`n[2/7] Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✓ Node.js: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Node.js not found" -ForegroundColor Red
}

# Check npm
Write-Host "`n[3/7] Checking npm..." -ForegroundColor Yellow
try {
    $npmVersion = npm --version
    Write-Host "✓ npm: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ npm not found" -ForegroundColor Red
}

# Check Git
Write-Host "`n[4/7] Checking Git..." -ForegroundColor Yellow
try {
    $gitVersion = git --version
    Write-Host "✓ $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Git not found" -ForegroundColor Red
}

# Check Docker
Write-Host "`n[5/7] Checking Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✓ $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker not found" -ForegroundColor Red
}

# Check Docker Compose
Write-Host "`n[6/7] Checking Docker Compose..." -ForegroundColor Yellow
try {
    $composeVersion = docker-compose --version
    Write-Host "✓ $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker Compose not found" -ForegroundColor Red
}

# Check VS Code
Write-Host "`n[7/7] Checking VS Code..." -ForegroundColor Yellow
try {
    $codeVersion = code --version | Select-Object -First 1
    Write-Host "✓ VS Code: $codeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ VS Code not found" -ForegroundColor Red
}

Write-Host "`n====================================" -ForegroundColor Cyan
Write-Host "  Environment Check Complete" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
```

執行檢查:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\check-environment.ps1
```

---

## 下一步

環境設置完成後,請參考:
- **[環境變數配置指南](./environment-variables.md)**: 配置應用程式環境變數
- **[Docker 配置指南](./docker-configuration.md)**: 詳細的 Docker 和容器配置
- **[故障排除指南](./troubleshooting.md)**: 更多問題排除方案

---

## 參考資源

### 官方文檔
- [.NET 8.0 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [Node.js Documentation](https://nodejs.org/docs/)
- [Docker Desktop for Windows](https://docs.docker.com/desktop/windows/)
- [Git for Windows](https://gitforwindows.org/)
- [Visual Studio Code](https://code.visualstudio.com/docs)

### 社群資源
- [Microsoft Developer Community](https://developercommunity.visualstudio.com/)
- [Stack Overflow](https://stackoverflow.com/)
- [Docker Community Forums](https://forums.docker.com/)

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
**下次審核**: 2025-04-01
