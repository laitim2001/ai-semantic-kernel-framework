# Linux 開發環境設置指南

## 版本信息
- **文檔版本**: 1.0
- **最後更新**: 2025-01-01
- **適用平台**: Ubuntu 20.04+, Debian 11+, Fedora 36+, CentOS Stream 9+

## 前置條件

### 系統要求
- **作業系統**:
  - Ubuntu 20.04 LTS / 22.04 LTS / 24.04 LTS
  - Debian 11 (Bullseye) / 12 (Bookworm)
  - Fedora 36+
  - CentOS Stream 9+
  - openSUSE Leap 15.4+
- **處理器**: Intel Core i5 或 AMD Ryzen 5 以上
- **記憶體**: 最小 8GB RAM (推薦 16GB+)
- **硬碟空間**: 至少 50GB 可用空間 (推薦 100GB+)
- **網路**: 穩定的網際網路連接

### 管理員權限
某些安裝步驟需要 sudo 權限。

---

## 步驟 1: 更新系統套件

### Ubuntu/Debian

```bash
sudo apt update
sudo apt upgrade -y
```

### Fedora

```bash
sudo dnf update -y
```

### CentOS Stream

```bash
sudo dnf update -y
```

---

## 步驟 2: 安裝 .NET 8.0 SDK

### Ubuntu 22.04/24.04

```bash
# 添加 Microsoft 套件倉庫
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 更新套件索引
sudo apt update

# 安裝 .NET SDK
sudo apt install -y dotnet-sdk-8.0
```

### Ubuntu 20.04

```bash
# 添加 Microsoft 套件倉庫
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 更新套件索引
sudo apt update

# 安裝 .NET SDK
sudo apt install -y dotnet-sdk-8.0
```

### Debian 11/12

```bash
# 添加 Microsoft 套件倉庫
wget https://packages.microsoft.com/config/debian/$(lsb_release -rs | cut -d. -f1)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 更新套件索引
sudo apt update

# 安裝 .NET SDK
sudo apt install -y dotnet-sdk-8.0
```

### Fedora 36+

```bash
# 添加 Microsoft 套件倉庫
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
sudo wget -O /etc/yum.repos.d/microsoft-prod.repo https://packages.microsoft.com/config/fedora/$(rpm -E %fedora)/prod.repo

# 安裝 .NET SDK
sudo dnf install -y dotnet-sdk-8.0
```

### CentOS Stream 9

```bash
# 添加 Microsoft 套件倉庫
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
sudo wget -O /etc/yum.repos.d/microsoft-prod.repo https://packages.microsoft.com/config/centos/9/prod.repo

# 安裝 .NET SDK
sudo dnf install -y dotnet-sdk-8.0
```

### 驗證安裝

```bash
dotnet --version
```

**預期輸出**:
```
8.0.xxx
```

### 驗證 .NET Runtime

```bash
dotnet --list-runtimes
```

**預期輸出** (應包含):
```
Microsoft.AspNetCore.App 8.0.x [/usr/share/dotnet/shared/...]
Microsoft.NETCore.App 8.0.x [/usr/share/dotnet/shared/...]
```

---

## 步驟 3: 安裝 Node.js 和 npm

### Ubuntu/Debian - 使用 NodeSource

```bash
# 安裝 curl (如果尚未安裝)
sudo apt install -y curl

# 添加 NodeSource 倉庫 (Node.js 20.x LTS)
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -

# 安裝 Node.js 和 npm
sudo apt install -y nodejs

# 安裝構建工具 (某些 npm 套件需要)
sudo apt install -y build-essential
```

### Fedora - 使用 NodeSource

```bash
# 添加 NodeSource 倉庫 (Node.js 20.x LTS)
curl -fsSL https://rpm.nodesource.com/setup_20.x | sudo bash -

# 安裝 Node.js 和 npm
sudo dnf install -y nodejs

# 安裝開發工具
sudo dnf groupinstall -y "Development Tools"
```

### CentOS Stream - 使用 NodeSource

```bash
# 添加 NodeSource 倉庫 (Node.js 20.x LTS)
curl -fsSL https://rpm.nodesource.com/setup_20.x | sudo bash -

# 安裝 Node.js 和 npm
sudo dnf install -y nodejs

# 安裝開發工具
sudo dnf groupinstall -y "Development Tools"
```

### 替代方案: 使用 nvm (Node Version Manager)

```bash
# 安裝 nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash

# 重新載入 shell 配置
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
[ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"

# 添加到 shell 配置文件
echo 'export NVM_DIR="$HOME/.nvm"' >> ~/.bashrc
echo '[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"' >> ~/.bashrc
source ~/.bashrc

# 安裝 Node.js LTS
nvm install --lts
nvm use --lts
```

### 驗證安裝

```bash
node --version
npm --version
```

**預期輸出**:
```
v20.x.x  # Node.js version
10.x.x   # npm version
```

### 配置 npm 全局套件目錄 (可選但推薦)

```bash
# 創建全局套件目錄
mkdir -p ~/.npm-global

# 配置 npm 使用新目錄
npm config set prefix '~/.npm-global'

# 添加到 PATH
echo 'export PATH=~/.npm-global/bin:$PATH' >> ~/.bashrc
source ~/.bashrc
```

---

## 步驟 4: 安裝 Git

### Ubuntu/Debian

```bash
sudo apt install -y git
```

### Fedora

```bash
sudo dnf install -y git
```

### CentOS Stream

```bash
sudo dnf install -y git
```

### 驗證安裝

```bash
git --version
```

**預期輸出**:
```
git version 2.xx.x
```

### 配置 Git

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
git config --global init.defaultBranch main

# 配置憑證助手
git config --global credential.helper store
```

---

## 步驟 5: 安裝 Docker

### Ubuntu

```bash
# 移除舊版本 (如果存在)
sudo apt remove docker docker-engine docker.io containerd runc

# 安裝依賴
sudo apt update
sudo apt install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release

# 添加 Docker 官方 GPG 金鑰
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# 設置倉庫
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# 安裝 Docker Engine
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# 啟動 Docker 服務
sudo systemctl start docker
sudo systemctl enable docker
```

### Debian

```bash
# 移除舊版本
sudo apt remove docker docker-engine docker.io containerd runc

# 安裝依賴
sudo apt update
sudo apt install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release

# 添加 Docker 官方 GPG 金鑰
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# 設置倉庫
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# 安裝 Docker Engine
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# 啟動 Docker 服務
sudo systemctl start docker
sudo systemctl enable docker
```

### Fedora

```bash
# 移除舊版本
sudo dnf remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine

# 安裝 dnf-plugins-core
sudo dnf install -y dnf-plugins-core

# 添加 Docker 倉庫
sudo dnf config-manager --add-repo https://download.docker.com/linux/fedora/docker-ce.repo

# 安裝 Docker Engine
sudo dnf install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# 啟動 Docker 服務
sudo systemctl start docker
sudo systemctl enable docker
```

### CentOS Stream

```bash
# 移除舊版本
sudo dnf remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine

# 安裝 yum-utils
sudo dnf install -y yum-utils

# 添加 Docker 倉庫
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

# 安裝 Docker Engine
sudo dnf install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# 啟動 Docker 服務
sudo systemctl start docker
sudo systemctl enable docker
```

### 將當前用戶添加到 docker 組 (避免使用 sudo)

```bash
# 創建 docker 組 (通常已存在)
sudo groupadd docker

# 將當前用戶添加到 docker 組
sudo usermod -aG docker $USER

# 重新載入組成員資格 (或登出並重新登入)
newgrp docker
```

### 驗證安裝

```bash
docker --version
docker compose version
```

**預期輸出**:
```
Docker version 24.x.x, build xxxxxxx
Docker Compose version v2.x.x
```

### 測試 Docker

```bash
docker run hello-world
```

**預期輸出**: 應該顯示 "Hello from Docker!" 訊息

---

## 步驟 6: 安裝 Visual Studio Code

### Ubuntu/Debian

```bash
# 導入 Microsoft GPG 金鑰
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > packages.microsoft.gpg
sudo install -D -o root -g root -m 644 packages.microsoft.gpg /etc/apt/keyrings/packages.microsoft.gpg
rm packages.microsoft.gpg

# 添加 VS Code 倉庫
sudo sh -c 'echo "deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" > /etc/apt/sources.list.d/vscode.list'

# 更新套件索引並安裝
sudo apt update
sudo apt install -y code
```

### Fedora/CentOS Stream

```bash
# 導入 Microsoft GPG 金鑰
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc

# 添加 VS Code 倉庫
sudo sh -c 'echo -e "[code]\nname=Visual Studio Code\nbaseurl=https://packages.microsoft.com/yumrepos/vscode\nenabled=1\ngpgcheck=1\ngpgkey=https://packages.microsoft.com/keys/microsoft.asc" > /etc/yum.repos.d/vscode.repo'

# 安裝
sudo dnf install -y code
```

### 替代方案: 使用 Snap

```bash
# 安裝 Snap (如果尚未安裝)
# Ubuntu 16.04+: 預裝
# Debian/Fedora/CentOS: 需要安裝 snapd

# 安裝 VS Code
sudo snap install --classic code
```

### 驗證安裝

```bash
code --version
```

**預期輸出**:
```
1.xx.x
xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
x64
```

### 安裝推薦的 VS Code 擴展

#### .NET 開發
```bash
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

#### JavaScript/TypeScript 開發
```bash
code --install-extension dbaeumer.vscode-eslint
code --install-extension esbenp.prettier-vscode
code --install-extension Vue.volar
```

#### 容器開發
```bash
code --install-extension ms-azuretools.vscode-docker
```

#### 版本控制
```bash
code --install-extension eamodio.gitlens
```

#### 其他推薦
```bash
code --install-extension editorconfig.editorconfig
code --install-extension streetsidesoftware.code-spell-checker
```

---

## 步驟 7: 安裝 Azure CLI (可選)

### Ubuntu/Debian

```bash
# 安裝先決條件
sudo apt update
sudo apt install -y ca-certificates curl apt-transport-https lsb-release gnupg

# 導入 Microsoft 倉庫金鑰
curl -sL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor | sudo tee /etc/apt/trusted.gpg.d/microsoft.gpg > /dev/null

# 添加 Azure CLI 倉庫
AZ_REPO=$(lsb_release -cs)
echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | sudo tee /etc/apt/sources.list.d/azure-cli.list

# 安裝 Azure CLI
sudo apt update
sudo apt install -y azure-cli
```

### Fedora/CentOS Stream

```bash
# 導入 Microsoft 倉庫金鑰
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc

# 添加 Azure CLI 倉庫
sudo sh -c 'echo -e "[azure-cli]\nname=Azure CLI\nbaseurl=https://packages.microsoft.com/yumrepos/azure-cli\nenabled=1\ngpgcheck=1\ngpgkey=https://packages.microsoft.com/keys/microsoft.asc" > /etc/yum.repos.d/azure-cli.repo'

# 安裝 Azure CLI
sudo dnf install -y azure-cli
```

### 驗證安裝

```bash
az --version
```

### 登入 Azure

```bash
az login
```

---

## 步驟 8: 克隆專案並設置

### 8.1 克隆專案倉庫

```bash
# 導航到工作目錄
cd ~/Projects  # 或您偏好的目錄

# 如果目錄不存在,創建它
mkdir -p ~/Projects

# 克隆專案
git clone <repository-url>
cd <project-directory>
```

### 8.2 安裝後端依賴

```bash
# 導航到後端專案目錄
cd backend  # 或對應的目錄名稱

# 還原 .NET 依賴
dotnet restore

# 驗證構建
dotnet build
```

### 8.3 安裝前端依賴

```bash
# 導航到前端專案目錄
cd ../frontend  # 或對應的目錄名稱

# 安裝 npm 依賴
npm install

# 或使用 npm ci (更快,適合 CI/CD)
npm ci
```

### 8.4 複製環境變數文件

```bash
# 後端
cp .env.example .env

# 前端
cp .env.local.example .env.local
```

編輯 `.env` 和 `.env.local` 文件,填入必要的配置值。

---

## 步驟 9: 啟動開發環境

### 9.1 啟動 Docker 服務

```bash
# 確保 Docker 服務正在運行
sudo systemctl status docker

# 如果未運行,啟動它
sudo systemctl start docker

# 啟動所有服務 (PostgreSQL, Redis, MongoDB)
docker compose up -d

# 檢查服務狀態
docker compose ps
```

**預期輸出**:
```
NAME                          STATUS              PORTS
aiworkflow-postgres           running             0.0.0.0:5432->5432/tcp
aiworkflow-redis              running             0.0.0.0:6379->6379/tcp
aiworkflow-mongodb            running             0.0.0.0:27017->27017/tcp
aiworkflow-pgadmin            running             0.0.0.0:5050->80/tcp
```

### 9.2 啟動後端 API

```bash
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

### 9.3 啟動前端開發伺服器

在新的終端視窗中:

```bash
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

## 步驟 10: 驗證環境設置

### 10.1 檢查所有服務

打開瀏覽器並訪問:

- **前端應用**: http://localhost:3000
- **後端 API**: http://localhost:5001/swagger (Swagger UI)
- **pgAdmin**: http://localhost:5050
  - 預設登入: `admin@example.com` / `admin123`

### 10.2 運行環境檢查腳本

```bash
# 在專案根目錄
./scripts/check-environment.sh
```

### 10.3 運行測試

```bash
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

**錯誤**: `command not found: dotnet`

**解決方案**:
1. 確認 .NET SDK 已正確安裝
2. 檢查 PATH 環境變數:
   ```bash
   echo $PATH | grep dotnet
   ```
3. 如果仍然失敗,手動添加到 PATH:
   ```bash
   export PATH="$PATH:/usr/share/dotnet"
   echo 'export PATH="$PATH:/usr/share/dotnet"' >> ~/.bashrc
   source ~/.bashrc
   ```

### 問題 2: Docker 權限問題

**錯誤**: `permission denied while trying to connect to the Docker daemon socket`

**解決方案**:
1. 確認當前用戶已添加到 docker 組:
   ```bash
   groups $USER
   ```
2. 如果未添加,執行:
   ```bash
   sudo usermod -aG docker $USER
   newgrp docker
   ```
3. 或登出並重新登入

### 問題 3: npm install 失敗

**錯誤**: `EACCES: permission denied` 或網路錯誤

**解決方案**:
1. 清除 npm 緩存:
   ```bash
   npm cache clean --force
   ```
2. 刪除 `node_modules` 和 `package-lock.json`:
   ```bash
   rm -rf node_modules package-lock.json
   npm install
   ```
3. 如果是權限問題,使用全局目錄配置 (見步驟 3)

### 問題 4: 端口已被佔用

**錯誤**: `Port 5432 is already allocated` 或類似錯誤

**解決方案**:
1. 檢查佔用端口的進程:
   ```bash
   sudo lsof -i :5432
   # 或
   sudo netstat -tulpn | grep :5432
   ```
2. 終止進程:
   ```bash
   sudo kill -9 <PID>
   ```
3. 或修改 `docker-compose.yml` 使用不同端口

### 問題 5: SELinux 阻止 Docker 操作 (Fedora/CentOS)

**錯誤**: Docker 容器無法訪問掛載的卷

**解決方案**:
1. 臨時禁用 SELinux (不推薦用於生產):
   ```bash
   sudo setenforce 0
   ```
2. 或為 Docker 卷添加 SELinux 上下文:
   ```bash
   chcon -Rt svirt_sandbox_file_t /path/to/volume
   ```
3. 或在 docker-compose.yml 中使用 `:z` 或 `:Z` 標籤:
   ```yaml
   volumes:
     - ./data:/data:z
   ```

### 問題 6: 防火牆阻止連接

**錯誤**: 無法從其他機器訪問服務

**解決方案** (Ubuntu/Debian - ufw):
```bash
# 允許特定端口
sudo ufw allow 3000/tcp  # 前端
sudo ufw allow 5001/tcp  # 後端
sudo ufw allow 5432/tcp  # PostgreSQL
```

**解決方案** (Fedora/CentOS - firewalld):
```bash
# 允許特定端口
sudo firewall-cmd --permanent --add-port=3000/tcp
sudo firewall-cmd --permanent --add-port=5001/tcp
sudo firewall-cmd --permanent --add-port=5432/tcp
sudo firewall-cmd --reload
```

---

## Bash 腳本工具

### 一鍵環境檢查腳本

創建 `scripts/check-environment.sh`:

```bash
#!/bin/bash

# Environment Check Script
echo "===================================="
echo "  Environment Verification Script"
echo "===================================="

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check .NET SDK
echo -e "\n${YELLOW}[1/7] Checking .NET SDK...${NC}"
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK: $DOTNET_VERSION${NC}"
else
    echo -e "${RED}✗ .NET SDK not found${NC}"
fi

# Check Node.js
echo -e "\n${YELLOW}[2/7] Checking Node.js...${NC}"
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓ Node.js: $NODE_VERSION${NC}"
else
    echo -e "${RED}✗ Node.js not found${NC}"
fi

# Check npm
echo -e "\n${YELLOW}[3/7] Checking npm...${NC}"
if command -v npm &> /dev/null; then
    NPM_VERSION=$(npm --version)
    echo -e "${GREEN}✓ npm: $NPM_VERSION${NC}"
else
    echo -e "${RED}✗ npm not found${NC}"
fi

# Check Git
echo -e "\n${YELLOW}[4/7] Checking Git...${NC}"
if command -v git &> /dev/null; then
    GIT_VERSION=$(git --version)
    echo -e "${GREEN}✓ $GIT_VERSION${NC}"
else
    echo -e "${RED}✗ Git not found${NC}"
fi

# Check Docker
echo -e "\n${YELLOW}[5/7] Checking Docker...${NC}"
if command -v docker &> /dev/null; then
    DOCKER_VERSION=$(docker --version)
    echo -e "${GREEN}✓ $DOCKER_VERSION${NC}"

    # Check if user is in docker group
    if groups $USER | grep -q '\bdocker\b'; then
        echo -e "${GREEN}✓ User is in docker group${NC}"
    else
        echo -e "${YELLOW}⚠ User is not in docker group (may need sudo)${NC}"
    fi
else
    echo -e "${RED}✗ Docker not found${NC}"
fi

# Check Docker Compose
echo -e "\n${YELLOW}[6/7] Checking Docker Compose...${NC}"
if docker compose version &> /dev/null; then
    COMPOSE_VERSION=$(docker compose version --short)
    echo -e "${GREEN}✓ Docker Compose: $COMPOSE_VERSION${NC}"
else
    echo -e "${RED}✗ Docker Compose not found${NC}"
fi

# Check VS Code
echo -e "\n${YELLOW}[7/7] Checking VS Code...${NC}"
if command -v code &> /dev/null; then
    CODE_VERSION=$(code --version | head -n 1)
    echo -e "${GREEN}✓ VS Code: $CODE_VERSION${NC}"
else
    echo -e "${RED}✗ VS Code not found${NC}"
fi

echo -e "\n===================================="
echo "  Environment Check Complete"
echo "===================================="
```

執行檢查:

```bash
chmod +x ./scripts/check-environment.sh
./scripts/check-environment.sh
```

---

## Linux 特定優化

### 提升開發體驗的設置

#### 1. 提高文件監視限制

對於大型專案,提高 inotify 監視器限制:

```bash
# 臨時設置
sudo sysctl fs.inotify.max_user_watches=524288

# 永久設置
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

#### 2. 優化 Bash/Zsh 配置

安裝有用的命令行工具:

```bash
# Ubuntu/Debian
sudo apt install -y zsh htop tree ncdu

# Fedora/CentOS
sudo dnf install -y zsh htop tree ncdu

# 安裝 Oh My Zsh (可選)
sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)"
```

#### 3. 配置 Git 別名

添加有用的 Git 別名到 `~/.gitconfig`:

```bash
git config --global alias.co checkout
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.st status
git config --global alias.lg "log --graph --oneline --decorate --all"
```

#### 4. Docker 性能優化

調整 Docker 存儲驅動 (如需要):

```bash
# 編輯 /etc/docker/daemon.json
sudo nano /etc/docker/daemon.json

# 添加:
{
  "storage-driver": "overlay2"
}

# 重啟 Docker
sudo systemctl restart docker
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
- [Docker Engine on Linux](https://docs.docker.com/engine/install/)
- [Git Documentation](https://git-scm.com/doc)
- [Visual Studio Code on Linux](https://code.visualstudio.com/docs/setup/linux)

### Linux 發行版特定資源
- [Ubuntu Documentation](https://help.ubuntu.com/)
- [Debian Documentation](https://www.debian.org/doc/)
- [Fedora Documentation](https://docs.fedoraproject.org/)
- [CentOS Documentation](https://docs.centos.org/)

### 社群資源
- [Microsoft Developer Community](https://developercommunity.visualstudio.com/)
- [Stack Overflow](https://stackoverflow.com/)
- [Docker Community Forums](https://forums.docker.com/)

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
**下次審核**: 2025-04-01
