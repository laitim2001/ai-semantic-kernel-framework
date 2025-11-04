# Folder Naming Conventions (文件夾命名約定)

## 版本信息
- **標準**: 跨平台統一命名規範
- **風格**: kebab-case, PascalCase, camelCase 混合使用
- **最後更新**: 2025-01-01

## 命名風格定義

### 1. kebab-case (小寫短橫線)
```
example-folder-name
```
- **使用場景**: 項目根目錄, Docker 配置, 腳本目錄
- **優點**: URL 友好, 跨平台兼容性好
- **示例**: `docker-compose`, `build-scripts`, `deployment-configs`

### 2. PascalCase (大駝峰)
```
ExampleFolderName
```
- **使用場景**: .NET 項目, C# 命名空間
- **優點**: 與 C# 類命名一致
- **示例**: `AIWorkflow.Domain`, `WorkflowNodes`, `AgentPersonas`

### 3. camelCase (小駝峰)
```
exampleFolderName
```
- **使用場景**: TypeScript/JavaScript 模塊
- **優點**: 與變量命名一致
- **示例**: `workflowEditor`, `nodeTypes`, `apiClient`

### 4. lowercase (全小寫)
```
examplefoldername
```
- **使用場景**: 配置文件夾, 工具目錄
- **優點**: 簡單, 無歧義
- **示例**: `public`, `assets`, `dist`, `node_modules`

## 項目層級命名規範

### Level 1: 項目根目錄

```
ai-workflow-platform/               # kebab-case
├── backend/                        # lowercase
├── frontend/                       # lowercase
├── shared/                         # lowercase
├── docker/                         # lowercase
├── scripts/                        # lowercase
├── docs/                           # lowercase
└── .github/                        # lowercase with dot prefix
```

**規則**:
- ✅ 使用 kebab-case 或 lowercase
- ✅ 描述性名稱,清晰表達用途
- ❌ 避免縮寫 (`fe` vs `frontend`)
- ❌ 避免特殊字符 (除 `-` 和 `.` 外)

### Level 2: 主要模塊目錄

#### Backend (.NET)

```
backend/
├── src/                            # lowercase
│   ├── AIWorkflow.Domain/          # PascalCase with namespace
│   ├── AIWorkflow.Application/     # PascalCase with namespace
│   ├── AIWorkflow.Infrastructure/  # PascalCase with namespace
│   └── AIWorkflow.API/             # PascalCase with namespace
└── tests/                          # lowercase
    ├── AIWorkflow.UnitTests/       # PascalCase with namespace
    └── AIWorkflow.IntegrationTests/
```

**規則**:
- ✅ C# 項目使用 PascalCase
- ✅ 項目名包含命名空間前綴 (`AIWorkflow.`)
- ✅ 測試項目以 `.Tests` 或 `.UnitTests` 結尾
- ❌ 避免縮寫 (`.App` vs `.Application`)

#### Frontend (React/Vue)

```
frontend/
├── react-app/                      # kebab-case
│   ├── src/
│   ├── public/                     # lowercase
│   └── dist/                       # lowercase
└── vue-workflow-editor/            # kebab-case
    ├── src/
    ├── public/
    └── server/                     # lowercase
```

**規則**:
- ✅ 應用目錄使用 kebab-case
- ✅ 標準目錄使用 lowercase (`src`, `public`, `dist`)
- ❌ 避免混用大小寫 (`React-App` vs `react-app`)

#### Shared

```
shared/
├── types/                          # lowercase
├── utils/                          # lowercase
├── contracts/                      # lowercase
└── constants/                      # lowercase
```

**規則**:
- ✅ 共享庫使用 lowercase
- ✅ 簡潔, 單數或複數根據語義
- ❌ 避免過於通用的名稱 (`common`, `misc`)

### Level 3: 功能模塊目錄

#### .NET 項目內部

```
AIWorkflow.Domain/
├── Entities/                       # PascalCase, 複數
├── ValueObjects/                   # PascalCase, 複數
├── Events/                         # PascalCase, 複數
├── Interfaces/                     # PascalCase, 複數
├── Services/                       # PascalCase, 複數
└── Exceptions/                     # PascalCase, 複數
```

**規則**:
- ✅ 使用 PascalCase
- ✅ 通常使用複數 (集合概念)
- ✅ 清晰的分類名稱
- ❌ 避免縮寫 (`Svcs` vs `Services`)

#### TypeScript/React 項目內部

```
src/
├── components/                     # lowercase, 複數
│   ├── common/                     # lowercase
│   ├── forms/                      # lowercase
│   └── workflow/                   # lowercase
├── features/                       # lowercase, 複數
│   ├── auth/                       # lowercase
│   ├── workflows/                  # lowercase
│   └── agents/                     # lowercase
├── hooks/                          # lowercase, 複數
├── stores/                         # lowercase, 複數
├── types/                          # lowercase, 複數
└── utils/                          # lowercase, 複數
```

**規則**:
- ✅ 使用 lowercase
- ✅ 功能目錄通常複數 (`components`, `features`)
- ✅ 特性目錄根據語義決定單複數 (`auth` 單數, `workflows` 複數)
- ❌ 避免縮寫

### Level 4: 組件和功能目錄

#### React 組件

```
components/
├── common/
│   ├── Button/                     # PascalCase (組件名)
│   │   ├── Button.tsx
│   │   ├── Button.test.tsx
│   │   └── Button.module.css
│   └── Input/                      # PascalCase
│       ├── Input.tsx
│       └── Input.module.css
└── workflow/
    └── WorkflowCard/               # PascalCase
        ├── WorkflowCard.tsx
        └── index.ts
```

**規則**:
- ✅ 組件文件夾使用 PascalCase (與組件名一致)
- ✅ 每個組件獨立文件夾
- ✅ 包含組件文件, 樣式, 測試
- ❌ 避免縮寫

#### Vue 組件

```
components/
├── nodes/
│   ├── PromptNode.vue              # PascalCase 文件名
│   ├── AgentNode.vue
│   └── FunctionNode.vue
└── panels/
    ├── NodePanel.vue
    └── PropertiesPanel.vue
```

**規則**:
- ✅ Vue 文件使用 PascalCase
- ✅ 可選: 組件文件夾或直接文件
- ✅ 單文件組件 (`.vue`)

#### Feature 功能模塊

```
features/
├── auth/                           # lowercase, 單數 (功能名)
│   ├── components/                 # lowercase, 複數
│   ├── hooks/                      # lowercase, 複數
│   └── stores/                     # lowercase, 複數
└── workflows/                      # lowercase, 複數 (多個 workflow)
    ├── components/
    │   ├── WorkflowEditor/         # PascalCase (組件)
    │   └── WorkflowList/
    ├── hooks/
    └── types.ts
```

**規則**:
- ✅ Feature 目錄根據語義決定單複數
- ✅ 內部子目錄使用 lowercase 複數
- ✅ 組件子目錄使用 PascalCase
- ❌ 避免嵌套過深 (≤ 4 層)

## 特殊目錄命名

### 配置和構建

```
project-root/
├── .github/                        # 點前綴, lowercase
│   ├── workflows/
│   └── ISSUE_TEMPLATE/
├── .vscode/                        # 點前綴, lowercase
├── docker/                         # lowercase
│   ├── nginx/
│   └── postgres/
└── scripts/                        # lowercase
    ├── build/
    ├── deploy/
    └── test/
```

**規則**:
- ✅ 隱藏配置目錄使用點前綴
- ✅ 工具配置使用 lowercase
- ✅ 構建腳本使用 lowercase
- ❌ 避免混合大小寫

### 測試目錄

```
tests/
├── unit/                           # lowercase
│   ├── domain/
│   └── application/
├── integration/                    # lowercase
│   ├── api/
│   └── database/
└── e2e/                            # lowercase (縮寫可接受)
    ├── workflows/
    └── auth/
```

**規則**:
- ✅ 測試類型使用 lowercase
- ✅ 縮寫保持一致性 (`e2e`, `api`)
- ✅ 按測試範圍組織
- ❌ 避免 `__tests__` (React/Jest 風格可接受)

### 資源和資產

```
assets/
├── images/                         # lowercase
│   ├── icons/
│   ├── logos/
│   └── backgrounds/
├── fonts/                          # lowercase
└── data/                           # lowercase
    ├── mock/
    └── fixtures/
```

**規則**:
- ✅ 所有資源目錄使用 lowercase
- ✅ 按資源類型分類
- ✅ 語義化命名
- ❌ 避免通用名稱 (`stuff`, `temp`)

## 跨語言一致性

### C# 項目

```
AIWorkflow.Application/
├── Features/                       # PascalCase
│   ├── Users/                      # PascalCase, 複數
│   │   ├── Commands/               # PascalCase, 複數
│   │   │   ├── RegisterUser/      # PascalCase (功能)
│   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   └── RegisterUserCommandHandler.cs
│   │   └── Queries/                # PascalCase, 複數
│   └── Workflows/                  # PascalCase, 複數
└── Common/                         # PascalCase
    ├── Interfaces/                 # PascalCase, 複數
    └── Behaviors/                  # PascalCase, 複數
```

### TypeScript/JavaScript 項目

```
src/
├── features/                       # lowercase
│   ├── users/                      # lowercase, 複數
│   │   ├── commands/               # lowercase, 複數
│   │   │   └── register-user/     # kebab-case (功能)
│   │   │       ├── RegisterUserCommand.ts
│   │   │       └── RegisterUserCommandHandler.ts
│   │   └── queries/                # lowercase, 複數
│   └── workflows/                  # lowercase, 複數
└── common/                         # lowercase
    ├── interfaces/                 # lowercase, 複數
    └── behaviors/                  # lowercase, 複數
```

## 命名反模式 (應避免)

### ❌ 不好的命名

```
project/
├── Utils/                          # 混用大小寫
├── Temp/                           # 模糊用途
├── New folder/                     # 空格
├── test1/                          # 無意義數字
├── bak/                            # 過度縮寫
├── 臨時文件夾/                      # 非英文
└── TODO/                           # 全大寫保留詞
```

### ✅ 好的命名

```
project/
├── utilities/                      # 清晰, lowercase
├── temporary-migrations/           # 描述性, kebab-case
├── user-profile-backup/            # 無空格, kebab-case
├── feature-auth-v2/                # 版本號合理使用
├── shared-utilities/               # 全拼, 無縮寫
├── localization/                   # 英文, 標準拼寫
└── tasks/                          # 清晰, lowercase
```

## 長度和深度限制

### 路徑長度
- **建議**: 單個目錄名 ≤ 30 字符
- **最大**: 完整路徑 ≤ 260 字符 (Windows 限制)
- **優化**: 避免過長嵌套路徑

### 嵌套深度
- **建議**: ≤ 5 層深度
- **最大**: ≤ 7 層深度
- **重構**: 超過 5 層考慮扁平化

**示例**:

```
✅ 良好深度 (4 層)
src/features/workflows/components/WorkflowEditor/

❌ 過深 (7 層)
src/features/workflows/components/editor/panels/properties/fields/
```

## 平台兼容性

### 大小寫敏感性

**問題**: Windows 不區分大小寫, Linux/macOS 區分

**解決方案**:
- ✅ 統一使用小寫或 kebab-case 作為默認
- ✅ Git 配置 `core.ignoreCase = false`
- ✅ ESLint/Prettier 強制文件名格式
- ❌ 避免僅大小寫不同的目錄名

```bash
# 避免
src/Components/  # Windows 視為相同
src/components/  # Linux 視為不同

# 推薦
src/components/  # 統一小寫
```

### 保留字符

**避免使用的字符**:
- Windows: `< > : " / \ | ? *`
- Linux/macOS: `/` (路徑分隔符)

**保留名稱 (Windows)**:
- `CON`, `PRN`, `AUX`, `NUL`
- `COM1-COM9`, `LPT1-LPT9`

```
❌ 避免
aux/
con.txt

✅ 使用
auxiliary/
configuration.txt
```

## 自動化檢查

### ESLint 配置

```javascript
// .eslintrc.cjs
module.exports = {
  rules: {
    // 文件名必須為 kebab-case 或 PascalCase
    'unicorn/filename-case': [
      'error',
      {
        cases: {
          kebabCase: true,
          pascalCase: true,
        },
      },
    ],
  },
}
```

### Git Hooks

```bash
# .husky/pre-commit
#!/bin/sh

# 檢查文件夾命名約定
find . -type d -name "*\ *" | while read dir; do
  echo "錯誤: 目錄名包含空格: $dir"
  exit 1
done

# 檢查大小寫混用
find . -type d -name "*[A-Z]*" -name "*[a-z]*" -name "*-*" | while read dir; do
  echo "警告: 可能的大小寫混用: $dir"
done
```

## 遷移指南

### 重命名現有目錄

```bash
# Git 重命名保留歷史
git mv oldName newName
git commit -m "refactor: rename folder to follow conventions"

# 批量重命名
find . -depth -name "*OldPattern*" -execdir bash -c '
  for f; do
    mv "$f" "$(echo "$f" | sed s/OldPattern/new-pattern/)"
  done
' bash {} +
```

## 決策樹

```
選擇命名風格:
│
├─ 是 .NET 項目?
│  └─ 是 → PascalCase (AIWorkflow.Domain)
│  └─ 否 → 繼續
│
├─ 是 TypeScript/React 組件?
│  └─ 是 → PascalCase (Button/)
│  └─ 否 → 繼續
│
├─ 是項目根目錄或配置?
│  └─ 是 → kebab-case 或 lowercase (docker/, scripts/)
│  └─ 否 → 繼續
│
└─ 默認
   └─ lowercase (components/, utils/, features/)
```

## 最佳實踐總結

### DO ✅
1. 保持一致性 - 同一層級使用相同風格
2. 語義化命名 - 清晰表達目錄用途
3. 避免縮寫 - 除非是廣為接受的 (`api`, `ui`, `db`)
4. 使用複數 - 對於集合類目錄 (`components`, `utils`)
5. 扁平化結構 - 避免過深嵌套
6. 自動化檢查 - 使用 Linter 和 Git Hooks

### DON'T ❌
1. 混用大小寫風格
2. 使用空格
3. 過度縮寫
4. 超過 5 層嵌套
5. 使用特殊字符 (除 `-` 和 `_`)
6. 使用保留關鍵字

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
