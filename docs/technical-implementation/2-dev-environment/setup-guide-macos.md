# macOS 開發環境設置指南

## 版本信息
- **文檔版本**: 1.0
- **最後更新**: 2025-01-01
- **適用平台**: macOS 12 (Monterey) 或更高版本

## 前置條件

### 系統要求
- **作業系統**: macOS 12 (Monterey), macOS 13 (Ventura), 或 macOS 14 (Sonoma)
- **處理器**: Intel Core i5 或 Apple Silicon (M1/M2/M3)
- **記憶體**: 最小 8GB RAM (推薦 16GB+)
- **硬碟空間**: 至少 50GB 可用空間 (推薦 100GB+)
- **網路**: 穩定的網際網路連接

### 管理員權限
某些安裝步驟需要管理員權限 (sudo)。

---

## 步驟 0: 安裝 Homebrew (套件管理器)

Homebrew 是 macOS 上最流行的套件管理器,可以簡化大部分工具的安裝。

### 0.1 安裝 Homebrew

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

### 0.2 配置 Shell 環境

安裝完成後,根據提示將 Homebrew 添加到 PATH:

**對於 Apple Silicon (M1/M2/M3)**:
```bash
echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
eval "$(/opt/homebrew/bin/brew shellenv)"
```

**對於 Intel Mac**:
```bash
echo 'eval "$(/usr/local/bin/brew shellenv)"' >> ~/.zprofile
eval "$(/usr/local/bin/brew shellenv)"
```

### 0.3 驗證安裝

```bash
brew --version
```

**預期輸出**:
```
Homebrew 4.x.x
```

### 0.4 更新 Homebrew

```bash
brew update
```

---

## 步驟 1: 安裝 .NET 8.0 SDK

### 1.1 使用 Homebrew 安裝

```bash
brew install --cask dotnet-sdk
```

### 1.2 驗證安裝

```bash
dotnet --version
```

**預期輸出**:
```
8.0.xxx
```

### 1.3 驗證 .NET Runtime

```bash
dotnet --list-runtimes
```

**預期輸出** (應包含):
```
Microsoft.AspNetCore.App 8.0.x [/usr/local/share/dotnet/shared/...]
Microsoft.NETCore.App 8.0.x [/usr/local/share/dotnet/shared/...]
```

### 1.4 配置 PATH (如果需要)

如果 `dotnet` 命令未找到,添加到 PATH:

```bash
# 對於 zsh (macOS 預設)
echo 'export PATH="/usr/local/share/dotnet:$PATH"' >> ~/.zshrc
source ~/.zshrc

# 對於 bash
echo 'export PATH="/usr/local/share/dotnet:$PATH"' >> ~/.bash_profile
source ~/.bash_profile
```

---

## 步驟 2: 安裝 Node.js 和 npm

### 2.1 使用 Homebrew 安裝

```bash
brew install node
```

這會同時安裝 Node.js 和 npm。

### 2.2 驗證安裝

```bash
node --version
npm --version
```

**預期輸出**:
```
v20.x.x  # Node.js version
10.x.x   # npm version
```

### 2.3 配置 npm 全局套件目錄 (可選但推薦)

```bash
# 創建全局套件目錄
mkdir -p ~/.npm-global

# 配置 npm 使用新目錄
npm config set prefix '~/.npm-global'

# 添加到 PATH
echo 'export PATH=~/.npm-global/bin:$PATH' >> ~/.zshrc
source ~/.zshrc
```

### 2.4 替代方案: 使用 nvm (Node Version Manager)

如果需要管理多個 Node.js 版本:

```bash
# 安裝 nvm
brew install nvm

# 配置 nvm
mkdir ~/.nvm
echo 'export NVM_DIR="$HOME/.nvm"' >> ~/.zshrc
echo '[ -s "/opt/homebrew/opt/nvm/nvm.sh" ] && \. "/opt/homebrew/opt/nvm/nvm.sh"' >> ~/.zshrc
source ~/.zshrc

# 安裝 Node.js LTS
nvm install --lts
nvm use --lts
```

---

## 步驟 3: 安裝 Git

macOS 通常預裝了 Git,但建議安裝最新版本。

### 3.1 使用 Homebrew 安裝

```bash
brew install git
```

### 3.2 驗證安裝

```bash
git --version
```

**預期輸出**:
```
git version 2.xx.x
```

### 3.3 配置 Git

設置用戶信息:

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
git config --global init.defaultBranch main
```

### 3.4 配置 Git Credential Helper

```bash
git config --global credential.helper osxkeychain
```

這會使用 macOS 鑰匙圈來安全儲存 Git 憑證。

---

## 步驟 4: 安裝 Docker Desktop

### 4.1 下載 Docker Desktop

- **下載連結**: https://www.docker.com/products/docker-desktop
- 選擇適合您 Mac 的版本:
  - **Apple Silicon (M1/M2/M3)**: Apple Chip
  - **Intel**: Intel Chip

### 4.2 安裝 Docker Desktop

1. 打開下載的 `.dmg` 文件
2. 將 Docker 圖標拖放到 Applications 文件夾
3. 從 Applications 文件夾啟動 Docker Desktop
4. 按照設置嚮導完成配置:
   - 允許 Docker 獲取必要的系統權限
   - 選擇是否啟用自動更新
5. 等待 Docker 完成初始化 (可能需要幾分鐘)

### 4.3 驗證安裝

```bash
docker --version
docker-compose --version
```

**預期輸出**:
```
Docker version 24.x.x, build xxxxxxx
Docker Compose version v2.x.x
```

### 4.4 測試 Docker

```bash
docker run hello-world
```

**預期輸出**: 應該顯示 "Hello from Docker!" 訊息

### 4.5 配置 Docker 資源限制 (可選)

打開 Docker Desktop → Preferences → Resources:
- **CPUs**: 至少 2 (推薦 4+)
- **Memory**: 至少 4GB (推薦 8GB+)
- **Swap**: 1GB
- **Disk image size**: 至少 60GB

---

## 步驟 5: 安裝 Visual Studio Code

### 5.1 使用 Homebrew 安裝

```bash
brew install --cask visual-studio-code
```

### 5.2 或手動下載安裝

- **下載連結**: https://code.visualstudio.com/
- 選擇 **macOS** 版本 (Universal 或對應您的處理器)

手動安裝步驟:
1. 打開下載的 `.zip` 文件
2. 將 Visual Studio Code.app 拖放到 Applications 文件夾
3. 從 Applications 文件夾啟動 VS Code

### 5.3 安裝 code 命令

啟動 VS Code,然後:
1. 按 `Cmd + Shift + P` 打開命令面板
2. 輸入 "shell command"
3. 選擇 **"Shell Command: Install 'code' command in PATH"**

### 5.4 驗證安裝

```bash
code --version
```

**預期輸出**:
```
1.xx.x
xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
arm64  # 或 x64 for Intel
```

### 5.5 安裝推薦的 VS Code 擴展

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

## 步驟 6: 安裝 Azure CLI (可選)

如果需要管理 Azure 資源:

### 6.1 使用 Homebrew 安裝

```bash
brew install azure-cli
```

### 6.2 驗證安裝

```bash
az --version
```

### 6.3 登入 Azure

```bash
az login
```

---

## 步驟 7: 克隆專案並設置

### 7.1 克隆專案倉庫

```bash
# 導航到工作目錄
cd ~/Projects  # 或您偏好的目錄

# 克隆專案
git clone <repository-url>
cd <project-directory>
```

### 7.2 安裝後端依賴

```bash
# 導航到後端專案目錄
cd backend  # 或對應的目錄名稱

# 還原 .NET 依賴
dotnet restore

# 驗證構建
dotnet build
```

### 7.3 安裝前端依賴

```bash
# 導航到前端專案目錄
cd ../frontend  # 或對應的目錄名稱

# 安裝 npm 依賴
npm install

# 或使用 npm ci (更快,適合 CI/CD)
npm ci
```

### 7.4 複製環境變數文件

```bash
# 後端
cp .env.example .env

# 前端
cp .env.local.example .env.local
```

編輯 `.env` 和 `.env.local` 文件,填入必要的配置值。

---

## 步驟 8: 啟動開發環境

### 8.1 啟動 Docker 服務

```bash
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

### 8.3 啟動前端開發伺服器

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

## 步驟 9: 驗證環境設置

### 9.1 檢查所有服務

打開瀏覽器並訪問:

- **前端應用**: http://localhost:3000
- **後端 API**: http://localhost:5001/swagger (Swagger UI)
- **pgAdmin**: http://localhost:5050
  - 預設登入: `admin@example.com` / `admin123`

### 9.2 運行環境檢查腳本

```bash
# 在專案根目錄
./scripts/check-environment.sh
```

### 9.3 運行測試

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
2. 重啟終端
3. 檢查 PATH 環境變數:
   ```bash
   echo $PATH | grep dotnet
   ```
4. 如果仍然失敗,手動添加到 PATH:
   ```bash
   export PATH="/usr/local/share/dotnet:$PATH"
   echo 'export PATH="/usr/local/share/dotnet:$PATH"' >> ~/.zshrc
   ```

### 問題 2: Docker 權限問題

**錯誤**: `permission denied while trying to connect to the Docker daemon socket`

**解決方案**:
1. 確認 Docker Desktop 正在運行
2. 確認您的用戶已添加到 docker 組 (通常不需要在 macOS 上)
3. 重啟 Docker Desktop

### 問題 3: Apple Silicon 兼容性問題

**錯誤**: 某些 npm 套件或 Docker 映像無法在 Apple Silicon 上運行

**解決方案**:
1. 對於 npm 套件,嘗試使用 Rosetta 2:
   ```bash
   # 安裝 Rosetta 2 (如果尚未安裝)
   softwareupdate --install-rosetta

   # 使用 Rosetta 運行 npm
   arch -x86_64 npm install
   ```

2. 對於 Docker 映像,使用 `--platform linux/amd64`:
   ```bash
   docker run --platform linux/amd64 <image-name>
   ```

3. 或在 `docker-compose.yml` 中指定:
   ```yaml
   services:
     myservice:
       platform: linux/amd64
       image: ...
   ```

### 問題 4: npm install 失敗

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
3. 如果是權限問題,使用全局目錄配置 (見步驟 2.3)

### 問題 5: 端口已被佔用

**錯誤**: `Port 5432 is already allocated` 或類似錯誤

**解決方案**:
1. 檢查佔用端口的進程:
   ```bash
   lsof -i :5432
   ```
2. 終止進程:
   ```bash
   kill -9 <PID>
   ```
3. 或修改 `docker-compose.yml` 使用不同端口

### 問題 6: Homebrew 安裝緩慢

**錯誤**: Homebrew 下載速度非常慢

**解決方案**:
使用中國鏡像源 (如果在中國大陸):

```bash
# 替換 Homebrew 源
export HOMEBREW_BREW_GIT_REMOTE="https://mirrors.tuna.tsinghua.edu.cn/git/homebrew/brew.git"
export HOMEBREW_CORE_GIT_REMOTE="https://mirrors.tuna.tsinghua.edu.cn/git/homebrew/homebrew-core.git"
export HOMEBREW_BOTTLE_DOMAIN="https://mirrors.tuna.tsinghua.edu.cn/homebrew-bottles"

# 添加到 shell 配置
echo 'export HOMEBREW_BOTTLE_DOMAIN="https://mirrors.tuna.tsinghua.edu.cn/homebrew-bottles"' >> ~/.zshrc
```

---

## Shell 腳本工具

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
else
    echo -e "${RED}✗ Docker not found${NC}"
fi

# Check Docker Compose
echo -e "\n${YELLOW}[6/7] Checking Docker Compose...${NC}"
if command -v docker-compose &> /dev/null; then
    COMPOSE_VERSION=$(docker-compose --version)
    echo -e "${GREEN}✓ $COMPOSE_VERSION${NC}"
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

## macOS 特定優化

### 提升開發體驗的設置

#### 1. 終端自動補全

安裝 Oh My Zsh (增強的 Zsh 配置):

```bash
sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)"
```

推薦插件:
```bash
# 編輯 ~/.zshrc
plugins=(
  git
  docker
  npm
  dotnet
  vscode
  zsh-autosuggestions
  zsh-syntax-highlighting
)
```

#### 2. 提高文件監視限制

對於大型專案,提高文件監視器限制:

```bash
# 添加到 ~/.zshrc
echo 'ulimit -n 65536' >> ~/.zshrc
source ~/.zshrc
```

#### 3. 禁用 Gatekeeper 警告 (開發工具)

```bash
# 對於特定應用程式
sudo xattr -r -d com.apple.quarantine /Applications/YourApp.app
```

#### 4. 提升 Docker 性能

在 Docker Desktop 設置中:
- 啟用 **VirtioFS** (Settings → Experimental Features)
- 使用 **gRPC FUSE** 文件共享

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
- [Docker Desktop for Mac](https://docs.docker.com/desktop/mac/)
- [Homebrew Documentation](https://docs.brew.sh/)
- [Visual Studio Code](https://code.visualstudio.com/docs)

### macOS 特定資源
- [Apple Developer](https://developer.apple.com/)
- [macOS Command Line Tools](https://developer.apple.com/xcode/resources/)
- [Oh My Zsh](https://ohmyz.sh/)

### 社群資源
- [Microsoft Developer Community](https://developercommunity.visualstudio.com/)
- [Stack Overflow](https://stackoverflow.com/)
- [Docker Community Forums](https://forums.docker.com/)

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
**下次審核**: 2025-04-01
