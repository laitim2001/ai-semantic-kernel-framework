# Dependency Management (依賴管理策略)

## 版本信息
- **.NET**: Central Package Management (CPM)
- **Frontend**: pnpm Workspaces
- **策略**: 中央化版本控制
- **最後更新**: 2025-01-01

## 整體策略

### 核心原則
1. **中央化版本管理**: 所有依賴版本在根目錄統一定義
2. **最小化重複**: 共享依賴只聲明一次
3. **明確版本號**: 避免使用 `^`, `~` 等範圍符號
4. **定期更新**: 每月檢查並更新依賴
5. **安全掃描**: 自動化漏洞檢測

## .NET Backend 依賴管理

### Directory.Build.props (全局屬性)

```xml
<!-- backend/Directory.Build.props -->
<Project>
  <PropertyGroup>
    <!-- 目標框架 -->
    <TargetFramework>net8.0</TargetFramework>

    <!-- 語言版本 -->
    <LangVersion>12.0</LangVersion>

    <!-- 可空引用類型 -->
    <Nullable>enable</Nullable>

    <!-- 隱式 using -->
    <ImplicitUsings>enable</ImplicitUsings>

    <!-- 警告設定 -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningLevel>5</WarningLevel>

    <!-- 輸出路徑 -->
    <BaseOutputPath>$(MSBuildThisFileDirectory)bin\$(MSBuildProjectName)\</BaseOutputPath>
    <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)obj\$(MSBuildProjectName)\</BaseIntermediateOutputPath>
  </PropertyGroup>

  <!-- 共享項目引用 -->
  <ItemGroup>
    <PackageReference Include="SonarAnalyzer.CSharp" Version="9.32.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

### Directory.Packages.props (中央包管理)

```xml
<!-- backend/Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <!-- 啟用中央包管理 -->
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>

    <!-- 禁用隱式框架引用 (可選) -->
    <DisableImplicitFrameworkReferences>false</DisableImplicitFrameworkReferences>
  </PropertyGroup>

  <ItemGroup Label="Core Frameworks">
    <!-- ASP.NET Core -->
    <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11" />
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="6.8.1" />

    <!-- Entity Framework Core -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />

    <!-- Authentication & Authorization -->
    <PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
    <PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
  </ItemGroup>

  <ItemGroup Label="AI & Semantic Kernel">
    <!-- Semantic Kernel -->
    <PackageVersion Include="Microsoft.SemanticKernel" Version="1.66.0" />
    <PackageVersion Include="Microsoft.SemanticKernel.Connectors.AzureOpenAI" Version="1.66.0" />
    <PackageVersion Include="Microsoft.SemanticKernel.Connectors.Memory.AzureAISearch" Version="1.66.0-alpha" />
  </ItemGroup>

  <ItemGroup Label="Application Libraries">
    <!-- MediatR (CQRS) -->
    <PackageVersion Include="MediatR" Version="12.4.1" />

    <!-- FluentValidation -->
    <PackageVersion Include="FluentValidation" Version="11.10.0" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.10.0" />

    <!-- AutoMapper -->
    <PackageVersion Include="AutoMapper" Version="13.0.1" />
    <PackageVersion Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="13.0.3" />
  </ItemGroup>

  <ItemGroup Label="Infrastructure">
    <!-- Redis -->
    <PackageVersion Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.11" />
    <PackageVersion Include="StackExchange.Redis" Version="2.8.16" />

    <!-- SignalR -->
    <PackageVersion Include="Microsoft.AspNetCore.SignalR.Client" Version="8.0.11" />

    <!-- Azure SDKs -->
    <PackageVersion Include="Azure.Storage.Blobs" Version="12.22.2" />
    <PackageVersion Include="Azure.Identity" Version="1.13.1" />
  </ItemGroup>

  <ItemGroup Label="Utilities">
    <!-- Logging -->
    <PackageVersion Include="Serilog.AspNetCore" Version="8.0.3" />
    <PackageVersion Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.File" Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.Seq" Version="8.0.0" />

    <!-- Polly (Resilience) -->
    <PackageVersion Include="Polly" Version="8.5.0" />
    <PackageVersion Include="Polly.Extensions.Http" Version="3.0.0" />
  </ItemGroup>

  <ItemGroup Label="Testing">
    <!-- xUnit -->
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />

    <!-- Moq -->
    <PackageVersion Include="Moq" Version="4.20.72" />

    <!-- FluentAssertions -->
    <PackageVersion Include="FluentAssertions" Version="6.12.2" />

    <!-- Test Containers -->
    <PackageVersion Include="Testcontainers" Version="3.10.0" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="3.10.0" />
  </ItemGroup>

  <ItemGroup Label="Code Analysis">
    <PackageVersion Include="SonarAnalyzer.CSharp" Version="9.32.0" />
    <PackageVersion Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />
  </ItemGroup>
</Project>
```

### 項目引用示例

```xml
<!-- backend/src/AIWorkflow.API/AIWorkflow.API.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- 項目引用 -->
    <ProjectReference Include="..\AIWorkflow.Application\AIWorkflow.Application.csproj" />
    <ProjectReference Include="..\AIWorkflow.Infrastructure\AIWorkflow.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- NuGet 引用 (版本號在 Directory.Packages.props 中定義) -->
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" />
    <PackageReference Include="Swashbuckle.AspNetCore" />
    <PackageReference Include="Serilog.AspNetCore" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
  </ItemGroup>

</Project>
```

## Frontend 依賴管理

### pnpm Workspaces

#### pnpm-workspace.yaml

```yaml
# ai-workflow-platform/pnpm-workspace.yaml
packages:
  # Shared packages
  - 'shared/types'
  - 'shared/utils'
  - 'shared/contracts'
  - 'shared/constants'

  # Frontend apps
  - 'frontend/react-app'
  - 'frontend/vue-workflow-editor'
```

#### Root package.json

```json
{
  "name": "ai-workflow-platform",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "pnpm -r --parallel dev",
    "build": "pnpm -r build",
    "test": "pnpm -r test",
    "typecheck": "pnpm -r typecheck",
    "lint": "pnpm -r lint",
    "update:deps": "pnpm -r update --latest"
  },
  "devDependencies": {
    "@types/node": "^22.0.0",
    "typescript": "^5.8.0",
    "prettier": "^3.0.0",
    "eslint": "^9.0.0",
    "vitest": "^2.0.0"
  },
  "packageManager": "pnpm@9.0.0",
  "engines": {
    "node": ">=20.0.0",
    "pnpm": ">=9.0.0"
  }
}
```

### React App Dependencies

```json
{
  "name": "react-app",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "test": "vitest run",
    "test:watch": "vitest",
    "typecheck": "tsc --noEmit",
    "lint": "eslint . --ext .ts,.tsx"
  },
  "dependencies": {
    "react": "^19.0.0",
    "react-dom": "^19.0.0",
    "react-router-dom": "^6.28.0",

    "@tanstack/react-query": "^5.62.0",
    "zustand": "^4.5.0",
    "axios": "^1.7.0",
    "react-hot-toast": "^2.4.0",

    "@aiworkflow/types": "workspace:*",
    "@aiworkflow/utils": "workspace:*",
    "@aiworkflow/contracts": "workspace:*",
    "@aiworkflow/constants": "workspace:*"
  },
  "devDependencies": {
    "@vitejs/plugin-react": "^4.3.0",
    "vite": "^5.0.0",

    "@types/react": "^18.3.0",
    "@types/react-dom": "^18.3.0",
    "typescript": "^5.8.0",

    "@testing-library/react": "^16.0.0",
    "@testing-library/jest-dom": "^6.6.0",
    "vitest": "^2.0.0",

    "eslint": "^9.0.0",
    "eslint-plugin-react": "^7.37.0",
    "eslint-plugin-react-hooks": "^5.0.0"
  }
}
```

### Vue Editor Dependencies

```json
{
  "name": "vue-workflow-editor",
  "version": "1.0.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "preview": "vite preview",
    "ws-server": "node server/websocket-server.js",
    "test": "vitest run",
    "typecheck": "vue-tsc --noEmit"
  },
  "dependencies": {
    "vue": "^3.5.0",
    "pinia": "^2.2.0",

    "@vue-flow/core": "^1.33.0",
    "@vue-flow/background": "^1.3.0",
    "@vue-flow/controls": "^1.1.0",
    "@vue-flow/minimap": "^1.4.0",

    "yjs": "^13.6.0",
    "y-websocket": "^2.0.0",

    "axios": "^1.7.0",

    "@aiworkflow/types": "workspace:*",
    "@aiworkflow/utils": "workspace:*",
    "@aiworkflow/constants": "workspace:*"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^5.0.0",
    "vite": "^5.0.0",

    "typescript": "^5.8.0",
    "vue-tsc": "^2.0.0",

    "ws": "^8.18.0"
  }
}
```

### Shared Package (Types) Example

```json
{
  "name": "@aiworkflow/types",
  "version": "1.0.0",
  "description": "Shared TypeScript types",
  "type": "module",
  "main": "./dist/index.js",
  "types": "./dist/index.d.ts",
  "exports": {
    ".": {
      "types": "./dist/index.d.ts",
      "import": "./dist/index.js",
      "require": "./dist/index.cjs"
    },
    "./workflow": {
      "types": "./dist/workflow/index.d.ts",
      "import": "./dist/workflow/index.js"
    }
  },
  "files": [
    "dist"
  ],
  "scripts": {
    "build": "tsup src/index.ts --format cjs,esm --dts --clean",
    "dev": "tsup src/index.ts --format cjs,esm --dts --watch",
    "typecheck": "tsc --noEmit"
  },
  "devDependencies": {
    "tsup": "^8.0.0",
    "typescript": "^5.8.0"
  },
  "publishConfig": {
    "access": "restricted"
  }
}
```

## 版本控制策略

### 語義化版本 (Semantic Versioning)

```
MAJOR.MINOR.PATCH

1.0.0 → 1.0.1 (PATCH)  - Bug 修復
1.0.0 → 1.1.0 (MINOR)  - 新功能 (向後兼容)
1.0.0 → 2.0.0 (MAJOR)  - 破壞性變更
```

### 版本鎖定

#### .NET (NuGet)
```xml
<!-- 明確版本 (推薦) -->
<PackageVersion Include="Microsoft.SemanticKernel" Version="1.66.0" />

<!-- 範圍版本 (不推薦用於生產) -->
<PackageVersion Include="SomePackage" Version="[1.0.0,2.0.0)" />
```

#### Frontend (npm/pnpm)
```json
{
  "dependencies": {
    "react": "19.0.0",              // 精確版本 (推薦)
    "axios": "^1.7.0",              // 兼容版本 (小心使用)
    "lodash": "~4.17.21"            // 補丁版本 (避免)
  }
}
```

### package-lock 和 pnpm-lock

```bash
# 提交到版本控制
git add package-lock.json       # npm
git add pnpm-lock.yaml          # pnpm
git add yarn.lock               # yarn

# .NET 不需要 lock 文件 (使用 Directory.Packages.props)
```

## 依賴更新策略

### 定期更新 (每月)

#### .NET
```bash
# 檢查過時的套件
dotnet list package --outdated

# 更新到最新 minor/patch
dotnet add package Microsoft.SemanticKernel --version 1.66.0
```

#### Frontend
```bash
# 檢查過時的依賴
pnpm outdated

# 更新所有依賴到最新版本
pnpm update --latest

# 更新特定套件
pnpm update react@latest
```

### 自動化更新 (Dependabot)

```yaml
# .github/dependabot.yml
version: 2
updates:
  # .NET 依賴
  - package-ecosystem: "nuget"
    directory: "/backend"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 10
    reviewers:
      - "tech-lead"
    labels:
      - "dependencies"
      - "dotnet"

  # npm 依賴 (React)
  - package-ecosystem: "npm"
    directory: "/frontend/react-app"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 5
    reviewers:
      - "frontend-team"
    labels:
      - "dependencies"
      - "react"

  # npm 依賴 (Vue)
  - package-ecosystem: "npm"
    directory: "/frontend/vue-workflow-editor"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 5
    labels:
      - "dependencies"
      - "vue"

  # GitHub Actions
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "monthly"
```

## 安全掃描

### .NET 漏洞掃描

```bash
# 啟用 NuGet 審計
dotnet restore --disable-parallel

# 檢查已知漏洞
dotnet list package --vulnerable

# 包含傳遞性依賴
dotnet list package --vulnerable --include-transitive
```

### Frontend 漏洞掃描

```bash
# npm audit
npm audit
npm audit fix

# pnpm audit
pnpm audit
pnpm audit --fix

# 使用 Snyk (推薦)
npx snyk test
npx snyk fix
```

### GitHub Security Alerts

```yaml
# .github/workflows/security-scan.yml
name: Security Scan

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  schedule:
    - cron: '0 0 * * 1'  # 每週一

jobs:
  dotnet-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore backend/AIWorkflow.sln

      - name: Check for vulnerabilities
        run: dotnet list backend/AIWorkflow.sln package --vulnerable --include-transitive

  npm-scan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20.x'

      - name: Install pnpm
        uses: pnpm/action-setup@v4
        with:
          version: 9

      - name: Install dependencies
        run: pnpm install --frozen-lockfile

      - name: Run audit
        run: pnpm audit
```

## 依賴衝突解決

### .NET Package 衝突

```bash
# 查看依賴樹
dotnet list package --include-transitive

# 解決衝突: 在 Directory.Packages.props 中明確指定版本
```

```xml
<!-- 解決 Newtonsoft.Json 版本衝突 -->
<ItemGroup>
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

### npm/pnpm 衝突

```bash
# 查看依賴樹
pnpm why <package-name>

# 解決衝突: 使用 overrides (pnpm) 或 resolutions (npm/yarn)
```

```json
{
  "pnpm": {
    "overrides": {
      "lodash": "^4.17.21"
    }
  }
}
```

## 私有套件管理 (可選)

### Azure Artifacts (.NET)

```xml
<!-- nuget.config -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="AzureArtifacts" value="https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <AzureArtifacts>
      <add key="Username" value="azure" />
      <add key="ClearTextPassword" value="%AZURE_ARTIFACTS_TOKEN%" />
    </AzureArtifacts>
  </packageSourceCredentials>
</configuration>
```

### npm Registry

```bash
# .npmrc
registry=https://registry.npmjs.org/
@aiworkflow:registry=https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/npm/registry/
always-auth=true
//pkgs.dev.azure.com/yourorg/_packaging/yourfeed/npm/registry/:_authToken=${AZURE_ARTIFACTS_TOKEN}
```

## 持續集成中的依賴管理

### CI/CD Pipeline

```yaml
# .github/workflows/ci.yml
name: CI

on: [push, pull_request]

jobs:
  build-dotnet:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

  build-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20.x'

      - name: Install pnpm
        uses: pnpm/action-setup@v4
        with:
          version: 9

      - name: Get pnpm store directory
        id: pnpm-cache
        run: echo "pnpm_cache_dir=$(pnpm store path)" >> $GITHUB_OUTPUT

      - name: Cache pnpm store
        uses: actions/cache@v4
        with:
          path: ${{ steps.pnpm-cache.outputs.pnpm_cache_dir }}
          key: ${{ runner.os }}-pnpm-${{ hashFiles('**/pnpm-lock.yaml') }}
          restore-keys: |
            ${{ runner.os }}-pnpm-

      - name: Install dependencies
        run: pnpm install --frozen-lockfile

      - name: Build
        run: pnpm build
```

## 最佳實踐總結

### DO ✅
1. **中央化版本管理** - 使用 Directory.Packages.props 和 pnpm workspaces
2. **明確版本號** - 生產環境使用精確版本
3. **提交 lock 文件** - package-lock.json, pnpm-lock.yaml
4. **定期更新** - 每月檢查並更新依賴
5. **自動化掃描** - 使用 Dependabot 和安全掃描
6. **緩存依賴** - CI/CD 中緩存 NuGet 和 pnpm store
7. **文檔記錄** - 記錄重大依賴變更

### DON'T ❌
1. **手動編輯 lock 文件** - 總是使用包管理器
2. **忽略安全警告** - 立即修復已知漏洞
3. **過度使用範圍版本** - 避免 `^` 和 `~` 在關鍵依賴
4. **不測試更新** - 更新後運行完整測試套件
5. **混合包管理器** - 不要同時使用 npm 和 pnpm
6. **忽略傳遞性依賴** - 檢查間接依賴的漏洞

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
