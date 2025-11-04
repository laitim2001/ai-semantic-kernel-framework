# 編碼標準與規範

**版本**: 2.0.0
**日期**: 2025-10-30
**狀態**: 📋 準備就緒
**目的**: 統一代碼風格與質量標準

**基於**: TECH-STACK-ANALYSIS v2.0.0

---

## 🎯 編碼標準目標

- ✅ **一致性**: 整個團隊使用統一的代碼風格
- ✅ **可讀性**: 代碼清晰易懂，降低維護成本
- ✅ **可維護性**: 遵循最佳實踐，便於長期維護
- ✅ **質量保證**: 自動化檢查，減少人工 Code Review 負擔
- ✅ **安全性**: 遵循安全編碼實踐，減少漏洞

---

## 📋 技術標準總覽

基於 TECH-STACK-ANALYSIS v2.0.0 確認的技術棧：

| 技術領域 | 標準文檔 | 適用範圍 | 狀態 |
|----------|----------|----------|------|
| **C# 12** | [csharp-coding-standards.md](./csharp-coding-standards.md) | 後端 API, 核心業務邏輯 | 📋 待撰寫 |
| **TypeScript 5.8** | [typescript-coding-standards.md](./typescript-coding-standards.md) | React + Vue 共用 | 📋 待撰寫 |
| **React 19** | [react-coding-standards.md](./react-coding-standards.md) | 主應用 (11 頁面) | 📋 待撰寫 |
| **Vue 3.5** | [vue3-coding-standards.md](./vue3-coding-standards.md) | Workflow Editor | 📋 待撰寫 |
| **SQL** | [sql-coding-standards.md](./sql-coding-standards.md) | PostgreSQL 查詢 | 📋 待撰寫 |
| **Git** | [git-workflow.md](./git-workflow.md) | 版本控制流程 | 📋 待撰寫 |
| **Code Review** | [code-review-checklist.md](./code-review-checklist.md) | 代碼審查清單 | 📋 待撰寫 |
| **Commit Message** | [commit-message-conventions.md](./commit-message-conventions.md) | Commit 訊息格式 | 📋 待撰寫 |

---

## 🔧 自動化工具配置

### 後端 (.NET C#)

#### EditorConfig (.editorconfig)

```ini
# 根配置文件
root = true

# 所有文件
[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true

# C# 文件
[*.cs]
indent_style = space
indent_size = 4

# 命名約定
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.severity = warning
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.symbols = interface
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.style = begins_with_i

dotnet_naming_symbols.interface.applicable_kinds = interface
dotnet_naming_style.begins_with_i.required_prefix = I
dotnet_naming_style.begins_with_i.capitalization = pascal_case

# 代碼風格
csharp_prefer_braces = true:warning
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_namespace_declarations = file_scoped:warning

# 使用 var 的情況
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = false:suggestion

# 表達式偏好
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_style_expression_bodied_constructors = false:suggestion
csharp_style_expression_bodied_properties = true:suggestion
csharp_style_expression_bodied_indexers = true:suggestion
csharp_style_expression_bodied_accessors = true:suggestion
```

#### StyleCop 配置

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <AnalysisMode>All</AnalysisMode>
    <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

---

### 前端 (React + Vue)

#### ESLint 配置 (.eslintrc.json)

```json
{
  "root": true,
  "env": {
    "browser": true,
    "es2024": true,
    "node": true
  },
  "extends": [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:@typescript-eslint/recommended-requiring-type-checking",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended",
    "plugin:jsx-a11y/recommended",
    "prettier"
  ],
  "parser": "@typescript-eslint/parser",
  "parserOptions": {
    "ecmaVersion": 2024,
    "sourceType": "module",
    "project": "./tsconfig.json",
    "ecmaFeatures": {
      "jsx": true
    }
  },
  "plugins": [
    "@typescript-eslint",
    "react",
    "react-hooks",
    "jsx-a11y",
    "import"
  ],
  "rules": {
    "@typescript-eslint/no-unused-vars": ["error", {
      "argsIgnorePattern": "^_",
      "varsIgnorePattern": "^_"
    }],
    "@typescript-eslint/explicit-function-return-type": ["warn", {
      "allowExpressions": true,
      "allowTypedFunctionExpressions": true
    }],
    "@typescript-eslint/no-explicit-any": "error",
    "@typescript-eslint/no-non-null-assertion": "error",

    "react/react-in-jsx-scope": "off",
    "react/prop-types": "off",
    "react-hooks/rules-of-hooks": "error",
    "react-hooks/exhaustive-deps": "warn",

    "import/order": ["error", {
      "groups": [
        "builtin",
        "external",
        "internal",
        "parent",
        "sibling",
        "index"
      ],
      "newlines-between": "always",
      "alphabetize": {
        "order": "asc",
        "caseInsensitive": true
      }
    }],

    "no-console": ["warn", { "allow": ["warn", "error"] }],
    "prefer-const": "error",
    "no-var": "error"
  },
  "settings": {
    "react": {
      "version": "19.0"
    }
  }
}
```

#### Vue 3 ESLint 配置

```json
{
  "extends": [
    "plugin:vue/vue3-recommended",
    "@vue/eslint-config-typescript",
    "@vue/eslint-config-prettier"
  ],
  "parser": "vue-eslint-parser",
  "parserOptions": {
    "parser": "@typescript-eslint/parser",
    "ecmaVersion": 2024,
    "sourceType": "module"
  },
  "rules": {
    "vue/multi-word-component-names": "error",
    "vue/component-name-in-template-casing": ["error", "PascalCase"],
    "vue/require-default-prop": "error",
    "vue/require-prop-types": "error",
    "vue/no-unused-properties": "warn",
    "vue/component-api-style": ["error", ["script-setup", "composition"]],
    "vue/block-lang": ["error", {
      "script": { "lang": "ts" },
      "style": { "lang": "scss" }
    }]
  }
}
```

#### Prettier 配置 (.prettierrc.json)

```json
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": true,
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false,
  "arrowParens": "always",
  "endOfLine": "lf",
  "bracketSpacing": true,
  "jsxBracketSameLine": false,
  "plugins": ["prettier-plugin-organize-imports"]
}
```

---

## 📐 命名約定

### C# 命名規範

| 類型 | 約定 | 範例 | 備註 |
|------|------|------|------|
| **命名空間** | PascalCase | `SemanticKernel.Agentic.Core` | 使用公司/專案前綴 |
| **類別** | PascalCase | `AgentService`, `UserRepository` | 名詞 |
| **接口** | IPascalCase | `IAgentService`, `IRepository<T>` | 以 I 開頭 |
| **方法** | PascalCase | `CreateAgent()`, `GetUserById()` | 動詞 |
| **公開屬性** | PascalCase | `AgentId`, `CreatedAt` | 名詞 |
| **私有字段** | _camelCase | `_logger`, `_dbContext` | 以 _ 開頭 |
| **常量** | PascalCase | `MaxRetryCount`, `DefaultTimeout` | UPPER_CASE 不推薦 |
| **局部變量** | camelCase | `userId`, `agentName` | 名詞 |
| **參數** | camelCase | `agentId`, `userName` | 名詞 |

### TypeScript/JavaScript 命名規範

| 類型 | 約定 | 範例 | 備註 |
|------|------|------|------|
| **文件名** | kebab-case | `agent-service.ts`, `user-list.tsx` | 組件用 PascalCase.tsx |
| **類別/組件** | PascalCase | `AgentService`, `UserList` | 名詞 |
| **接口/類型** | PascalCase | `IAgent`, `UserProps` | 可選 I 前綴 |
| **函數** | camelCase | `createAgent()`, `getUserById()` | 動詞 |
| **變量** | camelCase | `userId`, `agentList` | 名詞 |
| **常量** | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT`, `API_BASE_URL` | 全大寫 |
| **私有屬性** | _camelCase | `_internalState` | 以 _ 開頭（可選） |
| **React Hooks** | use* | `useAgent`, `useAuth` | 以 use 開頭 |
| **Vue Composables** | use* | `useWorkflow`, `useCollaboration` | 以 use 開頭 |

### 數據庫命名規範

| 類型 | 約定 | 範例 | 備註 |
|------|------|------|------|
| **表名** | snake_case | `agents`, `user_sessions` | 複數形式 |
| **列名** | snake_case | `agent_id`, `created_at` | 小寫 + 下劃線 |
| **主鍵** | id | `id` (auto) | 統一使用 id |
| **外鍵** | *_id | `user_id`, `agent_id` | 關聯表名 + _id |
| **索引** | idx_* | `idx_agents_user_id` | idx_ + 表名 + 列名 |
| **約束** | * | `chk_agents_status` | chk/fk/uk + 表名 |

---

## 🎨 代碼組織規範

### C# 文件結構

```csharp
// 1. Using 語句（按字母順序）
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SemanticKernel.Agentic.Core.Entities;

// 2. 命名空間（使用 File-Scoped Namespace - C# 10+）
namespace SemanticKernel.Agentic.Services;

// 3. 類別定義
public class AgentService : IAgentService
{
    // 3.1 常量
    private const int MaxRetryCount = 3;

    // 3.2 私有只讀字段（依賴注入）
    private readonly ILogger<AgentService> _logger;
    private readonly IAgentRepository _repository;

    // 3.3 建構函數
    public AgentService(
        ILogger<AgentService> logger,
        IAgentRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // 3.4 公開屬性
    public string ServiceName => "AgentService";

    // 3.5 公開方法（按邏輯分組）
    public async Task<Agent> CreateAgentAsync(CreateAgentRequest request)
    {
        // 實現...
    }

    public async Task<Agent?> GetAgentByIdAsync(string agentId)
    {
        // 實現...
    }

    // 3.6 私有方法（輔助方法）
    private void ValidateRequest(CreateAgentRequest request)
    {
        // 實現...
    }

    private async Task<bool> CheckDuplicateAsync(string name)
    {
        // 實現...
    }
}
```

### TypeScript/React 文件結構

```typescript
// 1. Import 語句（按順序：外部庫 → 內部模塊 → 類型 → 樣式）
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, TextField } from '@mui/material';

import { agentService } from '@/services';
import { useAuth } from '@/hooks';

import type { Agent, CreateAgentRequest } from '@/types';

import './AgentForm.scss';

// 2. 類型定義
interface AgentFormProps {
  initialData?: Agent;
  onSubmit: (agent: Agent) => void;
  onCancel: () => void;
}

// 3. 組件定義
export const AgentForm: React.FC<AgentFormProps> = ({
  initialData,
  onSubmit,
  onCancel
}) => {
  // 3.1 State
  const [name, setName] = useState(initialData?.name ?? '');
  const [isLoading, setIsLoading] = useState(false);

  // 3.2 Hooks
  const navigate = useNavigate();
  const { user } = useAuth();

  // 3.3 Effects
  useEffect(() => {
    // 副作用邏輯
  }, [initialData]);

  // 3.4 Event Handlers
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const agent = await agentService.create({ name });
      onSubmit(agent);
    } catch (error) {
      console.error('Failed to create agent:', error);
    } finally {
      setIsLoading(false);
    }
  };

  // 3.5 Render
  return (
    <form onSubmit={handleSubmit}>
      <TextField
        label="Agent Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        required
      />
      <Button type="submit" disabled={isLoading}>
        Submit
      </Button>
      <Button onClick={onCancel}>Cancel</Button>
    </form>
  );
};
```

### Vue 3 Composition API 文件結構

```vue
<script setup lang="ts">
// 1. Import 語句
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';

import { useWorkflowStore } from '@/stores';
import { workflowService } from '@/services';

import type { Workflow, Node } from '@/types';

// 2. Props 定義
interface Props {
  workflowId?: string;
  readonly?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  readonly: false,
});

// 3. Emits 定義
const emit = defineEmits<{
  save: [workflow: Workflow];
  cancel: [];
}>();

// 4. Composables / Stores
const router = useRouter();
const workflowStore = useWorkflowStore();

// 5. Reactive State
const nodes = ref<Node[]>([]);
const isLoading = ref(false);

// 6. Computed
const canSave = computed(() => nodes.value.length > 0 && !isLoading.value);

// 7. Lifecycle Hooks
onMounted(async () => {
  if (props.workflowId) {
    await loadWorkflow(props.workflowId);
  }
});

// 8. Methods
async function loadWorkflow(id: string): Promise<void> {
  isLoading.value = true;
  try {
    const workflow = await workflowService.getById(id);
    nodes.value = workflow.nodes;
  } catch (error) {
    ElMessage.error('Failed to load workflow');
    console.error(error);
  } finally {
    isLoading.value = false;
  }
}

async function handleSave(): Promise<void> {
  // 實現...
}
</script>

<template>
  <div class="workflow-editor">
    <div v-if="isLoading" class="loading">Loading...</div>
    <div v-else>
      <!-- Editor UI -->
    </div>
  </div>
</template>

<style scoped lang="scss">
.workflow-editor {
  // 樣式...
}
</style>
```

---

## ✅ Code Review Checklist

詳細清單請參閱 [code-review-checklist.md](./code-review-checklist.md)

### 快速檢查項

**功能性**:
- [ ] 代碼實現了需求中的所有功能
- [ ] 邊界條件處理正確
- [ ] 錯誤處理完整

**代碼質量**:
- [ ] 遵循命名約定
- [ ] 函數/方法長度合理（<50 行）
- [ ] 複雜度可接受（圈複雜度 <10）
- [ ] 無重複代碼

**測試**:
- [ ] 單元測試覆蓋率 >80%
- [ ] 測試案例涵蓋正常和異常情況
- [ ] 測試命名清晰

**安全性**:
- [ ] 無 SQL Injection 風險
- [ ] 無 XSS 風險
- [ ] 敏感信息未硬編碼
- [ ] 輸入驗證完整

**性能**:
- [ ] 無明顯性能問題
- [ ] 數據庫查詢優化
- [ ] 適當使用緩存

---

## 🔗 相關文檔

- [C# 編碼標準](./csharp-coding-standards.md) - 詳細 C# 標準
- [TypeScript 編碼標準](./typescript-coding-standards.md) - 詳細 TS 標準
- [React 編碼標準](./react-coding-standards.md) - React 19 最佳實踐
- [Vue 3 編碼標準](./vue3-coding-standards.md) - Vue 3.5 最佳實踐
- [Git 工作流程](./git-workflow.md) - Git 分支策略和工作流
- [Commit Message 規範](./commit-message-conventions.md) - 提交訊息格式

---

**最後更新**: 2025-10-30
**維護者**: Tech Lead Team
**支持**: tech-lead@example.com

---

[← 返回 Technical Implementation](../README.md)
