# PoC 1: Semantic Kernel Agents 驗證項目

**版本**: 1.0.0
**日期**: 2025-10-30
**狀態**: 🔴 P0 - 準備執行
**負責人**: Backend Tech Lead

---

## 📋 項目概述

本項目用於驗證 **Semantic Kernel 1.66+** Agents Framework 的核心功能，包括：

1. ✅ Agent 創建與執行
2. ✅ Plugin 註冊與調用
3. ✅ 多 LLM Provider 切換 (Azure OpenAI, OpenAI, Anthropic)
4. ✅ 對話歷史管理
5. ✅ 性能基準測試

---

## 🚀 快速開始

### 前置條件

- **.NET SDK 8.0+** ([下載連結](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Azure OpenAI API Key** (必需)
- **OpenAI API Key** (可選)
- **Anthropic API Key** (可選)

### Step 1: 創建項目

```bash
cd poc-projects/poc1-sk-agents
dotnet new console -n SemanticKernelAgentsPoc
cd SemanticKernelAgentsPoc
```

### Step 2: 安裝 NuGet 套件

```bash
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

### Step 3: 配置 API 密鑰

複製 `appsettings.template.json` 到 `appsettings.Development.json`：

```bash
cp appsettings.template.json appsettings.Development.json
```

編輯 `appsettings.Development.json`，填入真實 API Keys：

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "YOUR-ACTUAL-AZURE-OPENAI-KEY",
    "DeploymentName": "gpt-4o"
  },
  "OpenAI": {
    "ApiKey": "sk-YOUR-ACTUAL-OPENAI-KEY",
    "ModelId": "gpt-4-turbo-preview"
  },
  "Anthropic": {
    "ApiKey": "sk-ant-YOUR-ACTUAL-ANTHROPIC-KEY",
    "ModelId": "claude-3-5-sonnet-20241022"
  }
}
```

### Step 4: 執行驗證

```bash
dotnet build
dotnet run
```

---

## 📁 項目結構

```
SemanticKernelAgentsPoc/
├── Program.cs                     # 主程序入口
├── Configuration/
│   └── SemanticKernelConfig.cs    # 配置管理
├── Services/
│   ├── KernelBuilderService.cs    # Kernel 創建服務
│   ├── AgentService.cs            # Agent 管理服務
│   └── PluginService.cs           # Plugin 管理服務
├── Plugins/
│   ├── MathPlugin.cs              # 數學計算 Plugin
│   └── TimePlugin.cs              # 時間查詢 Plugin
├── Tests/
│   ├── TestRunner.cs              # 測試執行器
│   ├── PerformanceTests.cs        # 性能測試
│   ├── ProviderTests.cs           # Provider 切換測試
│   └── ContextTests.cs            # 對話上下文測試
├── appsettings.json               # 配置文件模板
├── appsettings.template.json      # 配置文件模板 (提交 Git)
├── appsettings.Development.json   # 開發配置 (不提交 Git)
└── SemanticKernelAgentsPoc.csproj # 項目文件
```

---

## 🧪 測試用例

### Test Suite 1: Agent 創建性能
- ✅ TC-1.1: 創建單個 Agent (<5 秒)
- ✅ TC-1.2: 創建 10 個 Agent (<10 秒)
- ✅ TC-1.3: Agent 記憶體佔用 (<100 MB/Agent)

### Test Suite 2: Plugin 調用
- ✅ TC-2.1: 註冊單個 Plugin
- ✅ TC-2.2: 註冊多個 Plugin (5 個)
- ✅ TC-2.3: Agent 自動調用 Plugin (成功率 >90%)
- ✅ TC-2.4: Plugin 參數傳遞正確 (100%)

### Test Suite 3: LLM Provider 切換
- ✅ TC-3.1: Azure OpenAI Provider (成功率 >95%)
- ✅ TC-3.2: OpenAI Provider (成功率 >95%)
- ✅ TC-3.3: Anthropic Provider (可選, 成功率 >95%)
- ✅ TC-3.4: Provider 動態切換 (100% 成功)

### Test Suite 4: 對話管理
- ✅ TC-4.1: 單輪對話 (100% 成功)
- ✅ TC-4.2: 多輪對話 (5 輪, 成功率 >95%)
- ✅ TC-4.3: 長對話 (50 輪, 成功率 >90%)
- ✅ TC-4.4: 對話歷史持久化 (100% 成功)

---

## ✅ 成功標準

| 成功標準 | 目標 | 驗證方法 | 狀態 |
|---------|------|----------|------|
| **1. Agent 創建時間** | <5 秒 | 性能測試 | ⏳ 待測試 |
| **2. Plugin 調用成功率** | >95% | 100 次調用統計 | ⏳ 待測試 |
| **3. LLM Provider 支持** | ≥2 個 | Provider 切換測試 | ⏳ 待測試 |
| **4. 對話歷史管理** | 正常 | 多輪對話測試 | ⏳ 待測試 |

---

## 📊 執行結果

**執行日期**: _____________
**執行人**: _____________

### 性能基準

| 指標 | 測試值 | 目標 | 狀態 |
|------|--------|------|------|
| Agent 創建時間 (平均) | _____ ms | <5000 ms | ⏳ |
| Plugin 調用成功率 | _____% | >95% | ⏳ |
| 支持 LLM Providers | _____ 個 | ≥2 個 | ⏳ |
| 多輪對話成功率 | _____% | >95% | ⏳ |
| 記憶體使用量 | _____ MB | <100 MB | ⏳ |

### 測試結果

- [ ] Test Suite 1: Agent 創建性能 - ✅ PASSED / ❌ FAILED
- [ ] Test Suite 2: Plugin 調用 - ✅ PASSED / ❌ FAILED
- [ ] Test Suite 3: LLM Provider 切換 - ✅ PASSED / ❌ FAILED
- [ ] Test Suite 4: 對話管理 - ✅ PASSED / ❌ FAILED

### 總體評估

**結論**: ⏳ 待執行 / ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**理由**:
___________________________________________________________________________
___________________________________________________________________________
___________________________________________________________________________

---

## ⚠️ 已知問題

### 問題 1: _________________
- **描述**: _________________
- **嚴重性**: 高 / 中 / 低
- **解決方案**: _________________

### 問題 2: _________________
- **描述**: _________________
- **嚴重性**: 高 / 中 / 低
- **解決方案**: _________________

---

## 🔗 相關文檔

- [PoC 1 驗證文檔](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents.md)
- [PoC 1 執行指導](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents-EXECUTION-GUIDE.md)
- [Stage 3.4 執行計劃](../../docs/technical-implementation/STAGE-3.4-EXECUTION-PLAN.md)
- [技術棧分析](../../docs/technical-implementation/TECH-STACK-ANALYSIS.md)

---

## 📝 執行記錄

```
Day 1 (___________):
- 10:00 AM: 環境準備開始
- 10:30 AM: API 配置完成
- 12:00 PM: Agent 基礎驗證完成
- 14:00 PM: Plugin 功能驗證完成
- 16:00 PM: 性能基準測試完成

Day 2 (___________):
- 10:00 AM: LLM Provider 測試開始
- 12:00 PM: 對話歷史測試完成
- 14:00 PM: 驗證報告撰寫完成
- 16:00 PM: PoC 1 完成

執行狀態: ⏳ 進行中 / ✅ 完成 / ❌ 失敗
Go/No-Go 決策: _____________
決策人: _____________
決策日期: _____________
```

---

**最後更新**: 2025-10-30
**項目狀態**: 📋 準備就緒 - 等待執行
**下一步**: 開始執行 Phase 1.1 環境驗證

---

[← 返回 PoC 驗證總覽](../../docs/technical-implementation/1-poc-validation/README.md) | [執行指導 →](../../docs/technical-implementation/1-poc-validation/01-semantic-kernel-agents-EXECUTION-GUIDE.md)
