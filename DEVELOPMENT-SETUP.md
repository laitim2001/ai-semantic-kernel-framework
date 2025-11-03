# 開發環境設置指南

**當前狀態**: ✅ Docker 環境就緒 | 🟡 .NET SDK 待安裝
**目標**: 完成 Sprint 1 User Story 1.1 (Agent CRUD API) 開發環境

---

## 📋 當前環境狀態

### ✅ 已就緒
- Docker Desktop 運行中
- PostgreSQL 16 (localhost:5432) - 健康
- Redis 7 (localhost:6379) - 健康
- Qdrant 1.7.4 (localhost:6333) - 健康
- Git 配置完成
- Feature 分支已創建: `feature/us-1.1-agent-crud-api`

### 🟡 待安裝
- .NET 9 SDK (推薦) 或 .NET 8 SDK (最低要求)
- IDE: Visual Studio 2022 或 Visual Studio Code
- 可選: Postman (API 測試)

---

## 🚀 快速安裝 (.NET SDK)

### 選項 A: .NET 9 SDK (推薦)

**Windows 安裝**:
```powershell
# 使用 winget (推薦)
winget install Microsoft.DotNet.SDK.9

# 或使用 Chocolatey
choco install dotnet-sdk

# 或手動下載安裝
# https://dotnet.microsoft.com/download/dotnet/9.0
```

**驗證安裝**:
```powershell
dotnet --version
# 預期輸出: 9.0.x

dotnet --list-sdks
# 應顯示 9.0.x 版本
```

### 選項 B: .NET 8 SDK (最低要求)

```powershell
winget install Microsoft.DotNet.SDK.8
```

---

## 🛠️ IDE 安裝

### 選項 A: Visual Studio Code (輕量級，推薦)

**安裝 VS Code**:
```powershell
winget install Microsoft.VisualStudioCode
```

**必需擴充套件**:
1. **C# Dev Kit** (ms-dotnettools.csdevkit)
2. **C#** (ms-dotnettools.csharp)
3. **.NET Install Tool** (ms-dotnettools.vscode-dotnet-runtime)

**可選擴充套件**:
- REST Client (humao.rest-client) - API 測試
- Docker (ms-azuretools.vscode-docker)
- GitLens (eamodio.gitlens)
- Thunder Client (rangav.vscode-thunder-client) - Postman 替代

**安裝擴充套件 (CLI)**:
```powershell
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
code --install-extension humao.rest-client
code --install-extension rangav.vscode-thunder-client
```

### 選項 B: Visual Studio 2022 (完整功能)

```powershell
winget install Microsoft.VisualStudio.2022.Community
```

**必需工作負載**:
- ASP.NET and web development
- .NET desktop development

---

## ✅ 完成環境設置後

### Step 1: 驗證所有工具

```powershell
# 切換到專案目錄
cd "C:\AI Semantic Kernel"

# 驗證 .NET SDK
dotnet --version

# 驗證 Git 分支
git branch
# 應顯示: * feature/us-1.1-agent-crud-api

# 驗證 Docker 服務
.\scripts\health-check.ps1
```

### Step 2: 創建 .NET 專案結構

```powershell
# 執行專案腳手架腳本 (安裝 .NET 後執行)
.\scripts\create-dotnet-solution.ps1
```

或手動執行以下命令：

```powershell
# 創建解決方案
dotnet new sln -n AIAgentPlatform -o src

# 創建各層項目
cd src

# Domain Layer
dotnet new classlib -n AIAgentPlatform.Domain -f net9.0
dotnet sln add AIAgentPlatform.Domain/AIAgentPlatform.Domain.csproj

# Application Layer
dotnet new classlib -n AIAgentPlatform.Application -f net9.0
dotnet sln add AIAgentPlatform.Application/AIAgentPlatform.Application.csproj

# Infrastructure Layer
dotnet new classlib -n AIAgentPlatform.Infrastructure -f net9.0
dotnet sln add AIAgentPlatform.Infrastructure/AIAgentPlatform.Infrastructure.csproj

# API Layer
dotnet new webapi -n AIAgentPlatform.API -f net9.0
dotnet sln add AIAgentPlatform.API/AIAgentPlatform.API.csproj

# Shared Library
dotnet new classlib -n AIAgentPlatform.Shared -f net9.0
dotnet sln add AIAgentPlatform.Shared/AIAgentPlatform.Shared.csproj

# 返回專案根目錄
cd ..

# 創建測試項目
mkdir tests
cd tests

# Unit Tests
dotnet new xunit -n AIAgentPlatform.UnitTests -f net9.0
dotnet sln ../src/AIAgentPlatform.sln add AIAgentPlatform.UnitTests/AIAgentPlatform.UnitTests.csproj

# Integration Tests
dotnet new xunit -n AIAgentPlatform.IntegrationTests -f net9.0
dotnet sln ../src/AIAgentPlatform.sln add AIAgentPlatform.IntegrationTests/AIAgentPlatform.IntegrationTests.csproj

cd ..
```

### Step 3: 配置專案引用

```powershell
# Application 引用 Domain
cd src/AIAgentPlatform.Application
dotnet add reference ../AIAgentPlatform.Domain/AIAgentPlatform.Domain.csproj

# Infrastructure 引用 Domain + Application
cd ../AIAgentPlatform.Infrastructure
dotnet add reference ../AIAgentPlatform.Domain/AIAgentPlatform.Domain.csproj
dotnet add reference ../AIAgentPlatform.Application/AIAgentPlatform.Application.csproj

# API 引用 Application + Infrastructure
cd ../AIAgentPlatform.API
dotnet add reference ../AIAgentPlatform.Application/AIAgentPlatform.Application.csproj
dotnet add reference ../AIAgentPlatform.Infrastructure/AIAgentPlatform.Infrastructure.csproj

cd ../..
```

### Step 4: 安裝必需 NuGet 套件

```powershell
# Infrastructure Layer - Entity Framework Core + PostgreSQL
cd src/AIAgentPlatform.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0

# Application Layer - MediatR + FluentValidation
cd ../AIAgentPlatform.Application
dotnet add package MediatR --version 12.4.0
dotnet add package FluentValidation --version 11.11.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.11.0

# API Layer - Swagger + Authentication
cd ../AIAgentPlatform.API
dotnet add package Swashbuckle.AspNetCore --version 7.2.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package Serilog.AspNetCore --version 8.0.3

# Test Projects
cd ../../tests/AIAgentPlatform.UnitTests
dotnet add package Moq --version 4.20.72
dotnet add package FluentAssertions --version 7.0.0
dotnet add package xunit.runner.visualstudio --version 2.8.2

cd ../AIAgentPlatform.IntegrationTests
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 9.0.0
dotnet add package Testcontainers.PostgreSql --version 4.1.0

cd ../..
```

### Step 5: 驗證專案構建

```powershell
# 構建整個解決方案
cd src
dotnet restore
dotnet build

# 運行測試
cd ../tests
dotnet test

# 運行 API
cd ../src/AIAgentPlatform.API
dotnet run
```

預期輸出：
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

---

## 📚 下一步：開始開發

環境設置完成後，參考以下文檔開始開發：

1. **User Story 規格**: `docs/user-stories/modules/module-01-agent-creation.md`
2. **API 設計規範**: `docs/technical-implementation/5-api-design/`
3. **資料庫設計**: `docs/technical-implementation/6-database-standards/`
4. **Clean Architecture 指南**: `docs/technical-implementation/3-project-structure/backend-project-structure.md`

### 開發順序

1. ✅ 環境設置完成
2. 🔨 **實作 Domain Layer** (Agent Entity, Value Objects)
3. 🔨 **實作 Application Layer** (Commands, Queries, Handlers)
4. 🔨 **實作 Infrastructure Layer** (DbContext, Repositories)
5. 🔨 **實作 API Layer** (Controllers, DTOs)
6. 🧪 **編寫測試** (Unit Tests, Integration Tests)
7. 📝 **API 文檔** (Swagger/OpenAPI)
8. ✅ **測試和驗證**

---

## 🚨 常見問題

### Q1: .NET SDK 安裝後找不到命令
```powershell
# 重啟 PowerShell 或 Terminal
# 或手動添加到 PATH
$env:PATH += ";C:\Program Files\dotnet"
```

### Q2: Docker 服務無法連接
```powershell
# 重啟 Docker 服務
docker-compose restart

# 檢查健康狀態
.\scripts\health-check.ps1
```

### Q3: Entity Framework 遷移失敗
```powershell
# 確保 PostgreSQL 正在運行
docker ps | findstr postgres

# 檢查連接字串
# appsettings.Development.json 中的連接字串應該是:
# "Host=localhost;Port=5432;Database=aiagent;Username=postgres;Password=postgres"
```

### Q4: API 無法啟動 (端口占用)
```powershell
# 檢查端口占用
netstat -ano | findstr :5000
netstat -ano | findstr :5001

# 修改端口 (launchSettings.json)
# 或終止占用進程
```

---

## 📞 需要幫助？

- **文檔**: 查看 `docs/technical-implementation/` 目錄
- **快速啟動**: `QUICK-START-GUIDE.md`
- **問題排查**: `.github/README.md` 的故障排除章節

---

**最後更新**: 2025-01-03
**狀態**: 🟡 待完成 .NET SDK 安裝
**下一步**: 安裝 .NET 9 SDK 後執行 Step 2

**完成安裝後，執行**: `.\scripts\create-dotnet-solution.ps1`
