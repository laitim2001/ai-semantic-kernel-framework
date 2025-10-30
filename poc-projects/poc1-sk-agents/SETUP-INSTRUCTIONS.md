# PoC 1 設置說明

這是 PoC 1: Semantic Kernel Agents 驗證項目的快速設置指南。

---

## 📋 Step-by-Step 設置

### Step 1: 驗證前置條件

```bash
# 檢查 .NET SDK 版本
dotnet --version
# 應顯示 8.0.x 或更高
```

如果沒有安裝 .NET 8 SDK:
- **Windows**: https://dotnet.microsoft.com/download/dotnet/8.0
- **macOS**: `brew install dotnet@8`
- **Linux**: https://learn.microsoft.com/en-us/dotnet/core/install/linux

---

### Step 2: 創建項目

```bash
# 導航到項目目錄
cd poc-projects/poc1-sk-agents

# 創建 .NET Console 項目
dotnet new console -n SemanticKernelAgentsPoc
cd SemanticKernelAgentsPoc

# 驗證項目創建成功
dotnet build
```

**預期輸出**: `Build succeeded`

---

### Step 3: 安裝 NuGet 套件

```bash
# 安裝 Semantic Kernel 核心套件
dotnet add package Microsoft.SemanticKernel --version 1.66.0

# 安裝配置管理套件
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables

# 驗證套件安裝
dotnet list package
```

**預期輸出**: 列出所有已安裝的套件及版本

---

### Step 4: 配置 API 密鑰

#### 4.1 複製配置模板

```bash
# 從項目根目錄複製模板
cp ../appsettings.template.json appsettings.Development.json
```

#### 4.2 編輯配置文件

編輯 `appsettings.Development.json`，替換以下佔位符：

**Azure OpenAI 配置** (必需):
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://YOUR-RESOURCE-NAME.openai.azure.com/",
    "ApiKey": "YOUR-ACTUAL-AZURE-OPENAI-KEY",
    "DeploymentName": "gpt-4o"
  }
}
```

**獲取 Azure OpenAI 配置的步驟**:
1. 登入 [Azure Portal](https://portal.azure.com)
2. 前往您的 Azure OpenAI 資源
3. 左側選單 → "Keys and Endpoint"
4. 複製 **KEY 1** (API Key) 和 **Endpoint** (Endpoint URL)
5. 前往 "Model deployments" 查看 **Deployment Name**

**OpenAI 配置** (可選):
```json
{
  "OpenAI": {
    "ApiKey": "sk-YOUR-ACTUAL-OPENAI-KEY",
    "ModelId": "gpt-4-turbo-preview"
  }
}
```

**獲取 OpenAI API Key 的步驟**:
1. 前往 [OpenAI Platform](https://platform.openai.com/api-keys)
2. 點擊 "Create new secret key"
3. 複製 API Key (以 `sk-` 開頭)

**Anthropic 配置** (可選):
```json
{
  "Anthropic": {
    "ApiKey": "sk-ant-YOUR-ACTUAL-ANTHROPIC-KEY",
    "ModelId": "claude-3-5-sonnet-20241022"
  }
}
```

**獲取 Anthropic API Key 的步驟**:
1. 前往 [Anthropic Console](https://console.anthropic.com/settings/keys)
2. 點擊 "Create Key"
3. 複製 API Key (以 `sk-ant-` 開頭)

#### 4.3 驗證配置

```bash
# 確認配置文件存在
ls appsettings.Development.json

# 確認配置文件不會被 Git 追蹤
git status
# 應該不會看到 appsettings.Development.json
```

---

### Step 5: 複製代碼

從執行指導文檔 (`01-semantic-kernel-agents-EXECUTION-GUIDE.md`) 複製代碼：

#### 5.1 創建目錄結構

```bash
mkdir Configuration
mkdir Services
mkdir Plugins
mkdir Tests
```

#### 5.2 創建文件

根據執行指導文檔，創建以下文件：

- `Configuration/SemanticKernelConfig.cs`
- `Services/KernelBuilderService.cs`
- `Services/AgentService.cs`
- `Services/PluginService.cs`
- `Plugins/MathPlugin.cs`
- `Plugins/TimePlugin.cs`
- `Tests/TestRunner.cs`
- `Tests/PerformanceTests.cs`
- `Tests/ProviderTests.cs`
- `Tests/ContextTests.cs`
- `Program.cs`

#### 5.3 或使用預先準備的腳手架

如果有提供完整的代碼腳手架，可以直接複製：

```bash
# 從 docs/technical-implementation/1-poc-validation/
# 參考 01-semantic-kernel-agents-EXECUTION-GUIDE.md 中的完整代碼
```

---

### Step 6: 編譯項目

```bash
# 編譯項目
dotnet build

# 預期輸出: Build succeeded, 0 Warning(s), 0 Error(s)
```

**常見編譯錯誤排查**:

1. **NuGet 套件版本衝突**:
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   dotnet build
   ```

2. **命名空間錯誤**:
   - 確認所有 `using` 語句正確
   - 確認文件命名空間與目錄結構一致

3. **API 版本不兼容**:
   - 確認 Semantic Kernel 版本為 1.66.0
   - 確認 .NET SDK 版本為 8.0+

---

### Step 7: 執行驗證

```bash
# 執行 PoC 驗證程序
dotnet run
```

**預期輸出**:

```
✅ Configuration loaded successfully!
✅ Kernel created successfully!

========== Test 1: Simple Conversation ==========
🤖 Agent Response: Semantic Kernel is a lightweight SDK...
⏱️ Response Time: 1234ms

✅ Test Result: PASSED
   - Response received: True
   - Response time acceptable: True

========== Test 2: Plugin Function Calling ==========
✅ WeatherPlugin registered
   [Plugin] GetWeather called with city: Tokyo
🤖 Agent Response: The weather in Tokyo is sunny, 25°C.
⏱️ Response Time: 2156ms

✅ Test Result: PASSED
   - Plugin function called: True

========== Performance Tests ==========
...
```

---

## 🔧 故障排查

### 問題 1: "Configuration file not found"

**錯誤訊息**:
```
System.IO.FileNotFoundException: Could not find file 'appsettings.Development.json'
```

**解決方案**:
1. 確認 `appsettings.Development.json` 在專案根目錄 (與 .csproj 同級)
2. 執行 `ls` 確認文件存在
3. 檢查檔名拼寫是否正確

---

### 問題 2: "Access denied due to invalid credentials"

**錯誤訊息**:
```
Azure.RequestFailedException: Access denied due to invalid credentials
```

**解決方案**:
1. 前往 Azure Portal 重新複製 API Key
2. 確認 Endpoint URL 格式正確 (以 `https://` 開頭，以 `/` 結尾)
3. 檢查 API Key 是否包含空格或特殊字符
4. 嘗試在 Azure Portal 重新生成 API Key

---

### 問題 3: "Deployment not found"

**錯誤訊息**:
```
DeploymentNotFoundException: The deployment 'gpt-4o' was not found
```

**解決方案**:
1. 登入 Azure Portal
2. 前往 Azure OpenAI 資源
3. 左側選單 → "Model deployments"
4. 確認部署名稱 (可能是 `gpt-4`, `gpt-4-turbo`, `gpt-35-turbo` 等)
5. 更新 `appsettings.Development.json` 中的 `DeploymentName`

---

### 問題 4: "Request timeout"

**錯誤訊息**:
```
TaskCanceledException: The operation was canceled
```

**解決方案**:
1. 檢查網絡連接
2. 檢查防火牆設置
3. 檢查代理設置 (如果在公司網絡)
4. 嘗試增加超時時間:
   ```csharp
   var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
   ```

---

### 問題 5: "Semantic Kernel version mismatch"

**錯誤訊息**:
```
Could not load file or assembly 'Microsoft.SemanticKernel, Version=...'
```

**解決方案**:
1. 清除 NuGet 緩存:
   ```bash
   dotnet nuget locals all --clear
   ```
2. 刪除 `bin/` 和 `obj/` 目錄:
   ```bash
   rm -rf bin obj
   ```
3. 重新安裝套件:
   ```bash
   dotnet restore
   dotnet build
   ```

---

## 📞 獲取幫助

如果遇到未在此處列出的問題:

1. **查閱官方文檔**:
   - [Semantic Kernel Docs](https://learn.microsoft.com/en-us/semantic-kernel/)
   - [Azure OpenAI Docs](https://learn.microsoft.com/en-us/azure/ai-services/openai/)

2. **查看項目文檔**:
   - [PoC 1 驗證文檔](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents.md)
   - [PoC 1 執行指導](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents-EXECUTION-GUIDE.md)

3. **聯繫團隊**:
   - Tech Lead: _____________
   - Backend Engineer: _____________

---

## ✅ 設置完成檢查清單

在開始執行 PoC 驗證之前，請確認以下所有項目:

- [ ] .NET 8 SDK 已安裝 (`dotnet --version` 顯示 8.0.x)
- [ ] 項目已創建 (`dotnet build` 成功)
- [ ] NuGet 套件已安裝 (`dotnet list package` 顯示 Semantic Kernel 1.66.0)
- [ ] `appsettings.Development.json` 已創建並配置
- [ ] Azure OpenAI API Key 已填入且有效
- [ ] (可選) OpenAI API Key 已填入
- [ ] (可選) Anthropic API Key 已填入
- [ ] 所有代碼文件已創建
- [ ] 項目編譯成功 (0 errors, 0 warnings)
- [ ] `dotnet run` 可以執行

---

**最後更新**: 2025-10-30
**文檔版本**: 1.0.0

---

[← 返回項目 README](./README.md) | [開始執行 PoC 1 →](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents-EXECUTION-GUIDE.md)
