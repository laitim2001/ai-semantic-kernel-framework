# Workflow Editor 設計文檔 V2 - Part 2

**內聯配置與表達式系統 ({{$json}} 語法)**

> 版本: 2.0
> 日期: 2025-10-29
> 狀態: 設計中
> 基於: n8n 開源項目架構分析
> 前置: Part 1 - 核心設計理念與大卡片節點系統

---

## 目錄

1. [內聯配置系統](#1-內聯配置系統)
2. [表達式語法系統](#2-表達式語法系統)
3. [智能提示與自動完成](#3-智能提示與自動完成)
4. [語法高亮系統](#4-語法高亮系統)
5. [參數類型系統](#5-參數類型系統)
6. [實時預覽與驗證](#6-實時預覽與驗證)
7. [表達式編輯器實現](#7-表達式編輯器實現)
8. [完整組件示例](#8-完整組件示例)

---

## 1. 內聯配置系統

### 1.1 展開/收起交互設計

基於 n8n 的實驗性 NDV，我們將其提升為核心功能並改進交互體驗。

**狀態轉換流程**：
```
收起狀態 (Collapsed)
    ↓ [點擊節點 / 點擊展開按鈕]
展開狀態 (Expanded)
    ↓ [點擊收起按鈕 / 點擊外部]
收起狀態 (Collapsed)
    ↓ [雙擊節點]
完整編輯模式 (Full Editor - 側邊欄)
```

### 1.2 收起狀態設計

**視覺呈現**：
```
┌──────────────────────────────────────────┐
│  [🌐]  HTTP Request    GET /api/users [>]│  48px
└──────────────────────────────────────────┘
                320px
```

**組件結構**：
```vue
<template>
  <div class="node-collapsed" @click="handleExpand">
    <!-- 左側：圖標 + 基本信息 -->
    <div class="node-header-left">
      <NodeIcon :icon="nodeIcon" :size="18" />
      <div class="node-info">
        <span class="node-name">{{ nodeName }}</span>
        <span class="node-subtitle">{{ subtitle }}</span>
      </div>
    </div>

    <!-- 右側：快捷操作 -->
    <div class="node-header-right">
      <button
        class="btn-expand"
        @click.stop="handleExpand"
        title="展開配置"
      >
        <IconChevronDown :size="16" />
      </button>
    </div>
  </div>
</template>

<style scoped lang="scss">
.node-collapsed {
  width: 320px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 12px;
  background: var(--color--background--light-3);
  border-radius: var(--radius--lg);
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    background: var(--color--background--light-2);
    box-shadow: var(--shadow--md);
  }

  .node-header-left {
    display: flex;
    align-items: center;
    gap: 12px;
    flex: 1;
    min-width: 0;

    .node-info {
      display: flex;
      align-items: center;
      gap: 8px;
      min-width: 0;

      .node-name {
        font-size: 14px;
        font-weight: 600;
        color: var(--color--text);
      }

      .node-subtitle {
        font-size: 13px;
        color: var(--color--text--tint-1);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }
  }

  .node-header-right {
    display: flex;
    gap: 4px;
  }
}
</style>
```

### 1.3 展開狀態設計

**視覺呈現**：
```
┌──────────────────────────────────────────────────────────────┐
│  [🌐]  HTTP Request                              [↗] [−]     │  48px
├──────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ Method:  [GET ▼]                                       │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ URL:     https://api.example.com/users  {{$json.id}} │  │  最大 300px
│  └────────────────────────────────────────────────────────┘  │  或視口 80%
│                                                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ Headers: [+ Add Header]                                │  │
│  │   Content-Type: application/json                       │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                                │
└──────────────────────────────────────────────────────────────┘
                            480px
```

**組件結構**：
```vue
<template>
  <div
    class="node-expanded"
    :style="dynamicStyles"
    @click.stop
  >
    <!-- Header (固定) -->
    <div class="node-header">
      <div class="node-header-left">
        <NodeIcon :icon="nodeIcon" :size="18" />
        <span class="node-name">{{ nodeName }}</span>
      </div>
      <div class="node-header-actions">
        <button
          class="btn-icon"
          @click="handleOpenFullEditor"
          title="打開完整編輯器"
        >
          <IconMaximize2 :size="16" />
        </button>
        <button
          class="btn-icon"
          @click="handleCollapse"
          title="收起"
        >
          <IconChevronUp :size="16" />
        </button>
      </div>
    </div>

    <!-- Configuration Panel (可滾動) -->
    <div class="node-config-panel" ref="configPanel">
      <div class="config-content">
        <!-- 動態渲染參數組件 -->
        <ParameterField
          v-for="param in visibleParameters"
          :key="param.name"
          :parameter="param"
          :value="getParameterValue(param.name)"
          :node-id="nodeId"
          @update="handleParameterUpdate"
        />
      </div>
    </div>

    <!-- Footer (可選) -->
    <div v-if="hasFooter" class="node-footer">
      <button class="btn-text" @click="showMoreParameters">
        顯示更多參數 ({{ hiddenParametersCount }})
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';

interface Props {
  nodeId: string;
  nodeName: string;
  nodeIcon: string;
  parameters: Parameter[];
  isSelected: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'collapse': [];
  'open-full-editor': [];
  'update-parameter': [name: string, value: any];
}>();

// 動態樣式計算
const dynamicStyles = computed(() => {
  const baseWidth = 480;
  const maxHeight = props.isSelected
    ? 'min(80vh, 600px)'  // 選中時更高
    : '300px';

  return {
    width: `${baseWidth}px`,
    maxHeight,
  };
});

// 可見參數（前5個主要參數）
const visibleParameters = computed(() =>
  props.parameters.filter(p => p.displayPriority === 'high').slice(0, 5)
);

const hiddenParametersCount = computed(() =>
  props.parameters.length - visibleParameters.value.length
);

function handleCollapse() {
  emit('collapse');
}

function handleOpenFullEditor() {
  emit('open-full-editor');
}

function handleParameterUpdate(name: string, value: any) {
  emit('update-parameter', name, value);
}
</script>

<style scoped lang="scss">
.node-expanded {
  display: flex;
  flex-direction: column;
  background: var(--color--background);
  border-radius: var(--radius--lg);
  box-shadow: var(--shadow--lg);
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);

  .node-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    border-bottom: 1px solid var(--color--foreground--tint-2);
    background: var(--color--background--light-2);
    flex-shrink: 0;

    .node-header-left {
      display: flex;
      align-items: center;
      gap: 12px;

      .node-name {
        font-size: 16px;
        font-weight: 600;
      }
    }

    .node-header-actions {
      display: flex;
      gap: 4px;
    }
  }

  .node-config-panel {
    flex: 1;
    overflow-y: auto;
    overflow-x: hidden;
    min-height: 0;  // 允許 flex 子元素收縮

    // 自定義滾動條
    &::-webkit-scrollbar {
      width: 6px;
    }

    &::-webkit-scrollbar-track {
      background: var(--color--background--shade-1);
    }

    &::-webkit-scrollbar-thumb {
      background: var(--color--foreground--tint-1);
      border-radius: 3px;

      &:hover {
        background: var(--color--foreground);
      }
    }

    .config-content {
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
  }

  .node-footer {
    padding: 8px 16px;
    border-top: 1px solid var(--color--foreground--tint-2);
    background: var(--color--background--light-2);
    flex-shrink: 0;

    .btn-text {
      font-size: 13px;
      color: var(--color--primary);
      cursor: pointer;
      background: none;
      border: none;
      padding: 4px 0;

      &:hover {
        text-decoration: underline;
      }
    }
  }
}
</style>
```

### 1.4 展開/收起動畫

```scss
// 動畫定義
@keyframes expand-node {
  from {
    max-height: 48px;
    opacity: 0.8;
  }
  to {
    max-height: 300px;
    opacity: 1;
  }
}

@keyframes collapse-node {
  from {
    max-height: 300px;
    opacity: 1;
  }
  to {
    max-height: 48px;
    opacity: 0.8;
  }
}

// 應用動畫
.node-configurable {
  &.expanding {
    animation: expand-node 0.3s cubic-bezier(0.4, 0, 0.2, 1) forwards;
  }

  &.collapsing {
    animation: collapse-node 0.3s cubic-bezier(0.4, 0, 0.2, 1) forwards;
  }
}
```

**動畫參數**：
- ⏱️ **持續時間** - 300ms（快速但不倉促）
- 📈 **緩動函數** - cubic-bezier(0.4, 0, 0.2, 1)（標準 Material Design 緩動）
- 🎯 **屬性** - max-height, opacity（避免 height 動畫的性能問題）

### 1.5 鍵盤快捷鍵

```typescript
// 快捷鍵映射
export const NODE_CONFIG_SHORTCUTS = {
  'Enter': '展開/收起節點',
  'Escape': '收起節點',
  'Ctrl+Enter': '打開完整編輯器',
  'Tab': '切換到下一個參數',
  'Shift+Tab': '切換到上一個參數',
} as const;

// 實現
function handleKeydown(event: KeyboardEvent) {
  switch (event.key) {
    case 'Enter':
      if (!event.ctrlKey) {
        toggleExpand();
      } else {
        openFullEditor();
      }
      break;

    case 'Escape':
      if (isExpanded.value) {
        collapse();
      }
      break;

    // Tab 導航由瀏覽器原生處理
  }
}
```

---

## 2. 表達式語法系統

### 2.1 表達式語法概覽

基於 n8n 的表達式系統，支持動態數據訪問和 JavaScript 表達式。

**基本語法**：
```javascript
// 雙花括號包裹表達式
{{ expression }}

// 示例
{{ $json.name }}                    // 訪問當前節點輸出的 name 字段
{{ $json.user.email }}              // 訪問嵌套字段
{{ $node["HTTP Request"].json.id }} // 訪問其他節點的輸出
{{ $env.API_KEY }}                  // 訪問環境變量
{{ $now.format('YYYY-MM-DD') }}     // 使用內置函數
```

### 2.2 上下文變量

#### $json - 當前節點輸入數據

```typescript
// 類型定義
interface $json {
  [key: string]: any;  // 上一個節點傳遞的 JSON 數據
}

// 示例
{{ $json.id }}           // 123
{{ $json.user.name }}    // "John Doe"
{{ $json.items[0] }}     // 第一個元素
{{ $json.items.length }} // 數組長度
```

**使用場景**：
- 🔗 構建 URL：`https://api.example.com/users/{{ $json.id }}`
- 📝 動態文本：`Hello, {{ $json.user.name }}!`
- 🔢 計算：`{{ $json.price * 1.1 }}`（價格加 10%）
- ✅ 條件：`{{ $json.status === 'active' }}`

#### $node - 訪問其他節點數據

```typescript
// 類型定義
interface $node {
  [nodeName: string]: {
    json: any;           // 節點輸出的 JSON 數據
    binary?: any;        // 二進制數據
    parameter: any;      // 節點參數
  };
}

// 示例
{{ $node["HTTP Request"].json.id }}
{{ $node["Database Query"].json.users[0].email }}
{{ $node["Get User"].parameter.userId }}
```

**使用場景**：
- 🔗 跨節點引用：從之前的節點獲取數據
- 📊 數據聚合：組合多個節點的輸出
- 🔄 條件邏輯：基於其他節點的結果做判斷

#### $env - 環境變量

```typescript
// 類型定義
interface $env {
  [key: string]: string;  // 環境變量鍵值對
}

// 示例
{{ $env.API_KEY }}
{{ $env.DATABASE_URL }}
{{ $env.NODE_ENV }}
```

**使用場景**：
- 🔑 API 密鑰：`Authorization: Bearer {{ $env.API_TOKEN }}`
- 🌍 環境配置：不同環境使用不同的 URL
- 🔒 敏感信息：避免硬編碼密碼

#### $now - 當前時間

```typescript
// 類型定義（基於 Luxon DateTime）
interface $now {
  format(fmt: string): string;
  plus(duration: Duration): DateTime;
  minus(duration: Duration): DateTime;
  toISO(): string;
  toISODate(): string;
  toISOTime(): string;
  // ... 更多 Luxon 方法
}

// 示例
{{ $now.format('YYYY-MM-DD') }}          // "2025-10-29"
{{ $now.format('HH:mm:ss') }}            // "14:30:45"
{{ $now.plus({ days: 7 }).toISODate() }} // 7天後
{{ $now.minus({ hours: 1 }).toISO() }}   // 1小時前
```

**使用場景**：
- 📅 時間戳：記錄當前時間
- ⏰ 時間計算：未來或過去的時間
- 🗓️ 日期格式化：轉換為特定格式

#### $workflow - 工作流元數據

```typescript
// 類型定義
interface $workflow {
  id: string;           // 工作流 ID
  name: string;         // 工作流名稱
  active: boolean;      // 是否激活
}

// 示例
{{ $workflow.id }}
{{ $workflow.name }}
```

#### $execution - 執行上下文

```typescript
// 類型定義
interface $execution {
  id: string;           // 執行 ID
  mode: 'manual' | 'trigger' | 'webhook';
  resumeUrl?: string;   // 恢復 URL（用於等待）
}

// 示例
{{ $execution.id }}
{{ $execution.mode }}
```

### 2.3 JavaScript 表達式

支持完整的 JavaScript 表達式（安全沙箱內）：

```javascript
// 字符串操作
{{ $json.name.toUpperCase() }}
{{ $json.email.split('@')[1] }}
{{ `Hello, ${$json.firstName} ${$json.lastName}!` }}

// 數字計算
{{ $json.price * 1.1 }}
{{ Math.round($json.value) }}
{{ Math.max(...$json.numbers) }}

// 數組操作
{{ $json.items.map(i => i.name) }}
{{ $json.items.filter(i => i.active) }}
{{ $json.items.reduce((sum, i) => sum + i.price, 0) }}

// 條件表達式
{{ $json.age >= 18 ? 'Adult' : 'Minor' }}
{{ $json.status === 'active' && $json.verified }}

// 對象操作
{{ Object.keys($json.data) }}
{{ Object.values($json.data) }}
{{ { ...$json, updated: true } }}

// 日期操作
{{ new Date($json.timestamp).toISOString() }}
{{ Date.now() }}
```

**安全限制**：
- ❌ 禁止訪問 `eval()`, `Function()`, `setTimeout()` 等危險函數
- ❌ 禁止訪問全局對象（`window`, `document`, `process` 等）
- ✅ 允許使用標準 JavaScript 方法和操作符
- ✅ 提供安全的沙箱環境

### 2.4 表達式解析器實現

```typescript
// expression-parser.ts
import { parse } from '@babel/parser';
import { evaluate } from 'expression-eval';

/**
 * 解析表達式字符串，提取所有表達式
 */
export function parseExpressions(input: string): ParsedExpression[] {
  const regex = /\{\{(.+?)\}\}/g;
  const expressions: ParsedExpression[] = [];
  let match;

  while ((match = regex.exec(input)) !== null) {
    const expression = match[1].trim();
    const start = match.index;
    const end = start + match[0].length;

    expressions.push({
      raw: match[0],
      expression,
      start,
      end,
    });
  }

  return expressions;
}

/**
 * 求值表達式
 */
export function evaluateExpression(
  expression: string,
  context: ExpressionContext
): EvaluationResult {
  try {
    // 1. 語法檢查
    const ast = parse(expression, {
      sourceType: 'script',
      plugins: ['objectRestSpread'],
    });

    // 2. 安全檢查
    validateSafety(ast);

    // 3. 求值
    const result = evaluate(ast, context);

    return {
      success: true,
      value: result,
    };
  } catch (error) {
    return {
      success: false,
      error: error.message,
    };
  }
}

/**
 * 安全檢查 - 防止危險操作
 */
function validateSafety(ast: any): void {
  // 檢查危險函數調用
  const dangerousFunctions = [
    'eval', 'Function', 'setTimeout', 'setInterval',
    'require', 'import', 'process', 'window', 'document'
  ];

  // 遍歷 AST，檢測危險模式
  // ... 實現細節
}

/**
 * 構建表達式上下文
 */
export function buildContext(
  currentNodeData: any,
  allNodesData: Record<string, any>,
  env: Record<string, string>
): ExpressionContext {
  return {
    $json: currentNodeData,
    $node: allNodesData,
    $env: env,
    $now: DateTime.now(),
    $workflow: {
      id: workflowId,
      name: workflowName,
      active: isActive,
    },
    $execution: {
      id: executionId,
      mode: executionMode,
    },
    // 輔助函數
    Math,
    Date,
    Object,
    Array,
    String,
    Number,
    Boolean,
    JSON,
  };
}
```

### 2.5 表達式類型定義

```typescript
// expression.types.ts

/**
 * 解析後的表達式
 */
export interface ParsedExpression {
  raw: string;        // 原始字符串（包含 {{ }}）
  expression: string; // 純表達式（不含 {{ }}）
  start: number;      // 開始位置
  end: number;        // 結束位置
}

/**
 * 表達式求值結果
 */
export interface EvaluationResult {
  success: boolean;
  value?: any;
  error?: string;
}

/**
 * 表達式上下文
 */
export interface ExpressionContext {
  $json: any;
  $node: Record<string, NodeData>;
  $env: Record<string, string>;
  $now: DateTime;
  $workflow: WorkflowMetadata;
  $execution: ExecutionMetadata;

  // 允許的全局對象
  Math: typeof Math;
  Date: typeof Date;
  Object: typeof Object;
  Array: typeof Array;
  String: typeof String;
  Number: typeof Number;
  Boolean: typeof Boolean;
  JSON: typeof JSON;
}

/**
 * 節點數據
 */
export interface NodeData {
  json: any;
  binary?: any;
  parameter: Record<string, any>;
}
```

---

## 3. 智能提示與自動完成

### 3.1 自動完成系統架構

```typescript
// autocomplete.ts
import type { CompletionContext, CompletionResult } from '@codemirror/autocomplete';

/**
 * 自動完成提供器
 */
export function createAutocompleteProvider(
  context: ExpressionContext
): (ctx: CompletionContext) => CompletionResult | null {
  return (completionContext: CompletionContext) => {
    const { state, pos } = completionContext;
    const line = state.doc.lineAt(pos);
    const textBefore = line.text.slice(0, pos - line.from);

    // 1. 檢測當前輸入的上下文
    const inputContext = detectInputContext(textBefore);

    // 2. 根據上下文生成建議
    const suggestions = generateSuggestions(inputContext, context);

    // 3. 過濾和排序
    const filtered = filterAndSort(suggestions, inputContext.prefix);

    return {
      from: pos - inputContext.prefix.length,
      options: filtered,
    };
  };
}

/**
 * 檢測輸入上下文
 */
function detectInputContext(textBefore: string): InputContext {
  // 檢測是否在表達式內
  const inExpression = /\{\{[^}]*$/.test(textBefore);
  if (!inExpression) {
    return { type: 'none', prefix: '' };
  }

  // 提取表達式內容
  const match = textBefore.match(/\{\{([^}]*)$/);
  const expression = match ? match[1] : '';

  // 檢測上下文變量
  if (expression.startsWith('$json.')) {
    return {
      type: 'json-field',
      prefix: expression.slice(6),  // 移除 "$json."
      path: [],
    };
  }

  if (expression.startsWith('$node["')) {
    const nodeMatch = expression.match(/\$node\["([^"]+)"\]\.json\.(.*)/);
    if (nodeMatch) {
      return {
        type: 'node-field',
        nodeName: nodeMatch[1],
        prefix: nodeMatch[2],
        path: nodeMatch[2].split('.').slice(0, -1),
      };
    }
  }

  if (expression.startsWith('$env.')) {
    return {
      type: 'env-var',
      prefix: expression.slice(5),
    };
  }

  if (expression.startsWith('$now.')) {
    return {
      type: 'now-method',
      prefix: expression.slice(5),
    };
  }

  // 默認：上下文變量建議
  return {
    type: 'context-variable',
    prefix: expression.replace(/^\$/, ''),
  };
}

/**
 * 生成建議
 */
function generateSuggestions(
  inputContext: InputContext,
  expressionContext: ExpressionContext
): CompletionItem[] {
  switch (inputContext.type) {
    case 'json-field':
      return generateJsonFieldSuggestions(
        expressionContext.$json,
        inputContext.path
      );

    case 'node-field':
      const nodeData = expressionContext.$node[inputContext.nodeName];
      return generateJsonFieldSuggestions(
        nodeData?.json,
        inputContext.path
      );

    case 'env-var':
      return generateEnvVarSuggestions(expressionContext.$env);

    case 'now-method':
      return generateNowMethodSuggestions();

    case 'context-variable':
      return generateContextVariableSuggestions();

    default:
      return [];
  }
}

/**
 * 生成 JSON 字段建議
 */
function generateJsonFieldSuggestions(
  data: any,
  path: string[]
): CompletionItem[] {
  if (!data || typeof data !== 'object') {
    return [];
  }

  // 導航到路徑
  let current = data;
  for (const key of path) {
    if (current && typeof current === 'object' && key in current) {
      current = current[key];
    } else {
      return [];
    }
  }

  // 生成當前級別的字段建議
  if (typeof current !== 'object' || current === null) {
    return [];
  }

  const keys = Object.keys(current);
  return keys.map(key => ({
    label: key,
    type: 'property',
    detail: getTypeLabel(current[key]),
    info: formatValue(current[key]),
  }));
}

/**
 * 生成環境變量建議
 */
function generateEnvVarSuggestions(
  env: Record<string, string>
): CompletionItem[] {
  return Object.keys(env).map(key => ({
    label: key,
    type: 'variable',
    detail: 'environment variable',
    info: env[key],
  }));
}

/**
 * 生成 $now 方法建議
 */
function generateNowMethodSuggestions(): CompletionItem[] {
  return [
    {
      label: 'format',
      type: 'method',
      detail: '(fmt: string) => string',
      info: '格式化日期，例如：format("YYYY-MM-DD")',
    },
    {
      label: 'toISO',
      type: 'method',
      detail: '() => string',
      info: '轉換為 ISO 8601 字符串',
    },
    {
      label: 'toISODate',
      type: 'method',
      detail: '() => string',
      info: '轉換為 ISO 日期字符串（YYYY-MM-DD）',
    },
    {
      label: 'plus',
      type: 'method',
      detail: '(duration: Duration) => DateTime',
      info: '增加時間，例如：plus({ days: 7 })',
    },
    {
      label: 'minus',
      type: 'method',
      detail: '(duration: Duration) => DateTime',
      info: '減少時間，例如：minus({ hours: 1 })',
    },
  ];
}

/**
 * 生成上下文變量建議
 */
function generateContextVariableSuggestions(): CompletionItem[] {
  return [
    {
      label: '$json',
      type: 'variable',
      detail: 'object',
      info: '當前節點輸入數據',
    },
    {
      label: '$node',
      type: 'variable',
      detail: 'object',
      info: '訪問其他節點數據',
    },
    {
      label: '$env',
      type: 'variable',
      detail: 'object',
      info: '環境變量',
    },
    {
      label: '$now',
      type: 'variable',
      detail: 'DateTime',
      info: '當前時間（Luxon DateTime）',
    },
    {
      label: '$workflow',
      type: 'variable',
      detail: 'object',
      info: '工作流元數據',
    },
    {
      label: '$execution',
      type: 'variable',
      detail: 'object',
      info: '執行上下文',
    },
  ];
}
```

### 3.2 自動完成 UI 設計

```vue
<!-- AutocompleteTooltip.vue -->
<template>
  <div class="autocomplete-tooltip" :style="position">
    <div class="autocomplete-list">
      <div
        v-for="(item, index) in items"
        :key="index"
        :class="['autocomplete-item', { active: index === activeIndex }]"
        @click="selectItem(item)"
        @mouseenter="activeIndex = index"
      >
        <!-- 圖標 -->
        <span class="item-icon" :class="`icon-${item.type}`">
          <IconVariable v-if="item.type === 'variable'" :size="16" />
          <IconFunction v-else-if="item.type === 'method'" :size="16" />
          <IconKey v-else-if="item.type === 'property'" :size="16" />
        </span>

        <!-- 主要信息 -->
        <div class="item-info">
          <span class="item-label">{{ item.label }}</span>
          <span v-if="item.detail" class="item-detail">{{ item.detail }}</span>
        </div>

        <!-- 快捷鍵提示 -->
        <span v-if="index === activeIndex" class="item-hint">
          <kbd>Enter</kbd>
        </span>
      </div>
    </div>

    <!-- 詳細信息面板 -->
    <div v-if="activeItem" class="autocomplete-info">
      <div class="info-header">
        <span class="info-label">{{ activeItem.label }}</span>
        <span class="info-type">{{ activeItem.detail }}</span>
      </div>
      <div v-if="activeItem.info" class="info-content">
        {{ activeItem.info }}
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.autocomplete-tooltip {
  position: fixed;
  z-index: 1000;
  display: flex;
  gap: 1px;
  background: var(--color--background);
  border-radius: var(--radius);
  box-shadow: var(--shadow--lg);
  max-width: 600px;

  .autocomplete-list {
    min-width: 250px;
    max-height: 300px;
    overflow-y: auto;
    border-right: 1px solid var(--color--foreground--tint-2);

    .autocomplete-item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 12px;
      cursor: pointer;
      transition: background 0.15s ease;

      &:hover,
      &.active {
        background: var(--color--primary--tint-3);
      }

      .item-icon {
        flex-shrink: 0;
        color: var(--color--text--tint-1);
      }

      .item-info {
        flex: 1;
        min-width: 0;
        display: flex;
        flex-direction: column;
        gap: 2px;

        .item-label {
          font-size: 14px;
          font-weight: 500;
          color: var(--color--text);
        }

        .item-detail {
          font-size: 12px;
          color: var(--color--text--tint-1);
          font-family: 'Monaco', 'Menlo', monospace;
        }
      }

      .item-hint {
        flex-shrink: 0;
        font-size: 11px;
        color: var(--color--text--tint-2);

        kbd {
          padding: 2px 6px;
          background: var(--color--background--shade-1);
          border-radius: 3px;
          border: 1px solid var(--color--foreground--tint-1);
        }
      }
    }
  }

  .autocomplete-info {
    width: 300px;
    padding: 12px;
    background: var(--color--background--light-2);

    .info-header {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-bottom: 12px;

      .info-label {
        font-size: 16px;
        font-weight: 600;
        color: var(--color--text);
      }

      .info-type {
        font-size: 12px;
        color: var(--color--text--tint-1);
        font-family: 'Monaco', 'Menlo', monospace;
      }
    }

    .info-content {
      font-size: 13px;
      line-height: 1.5;
      color: var(--color--text--tint-1);
    }
  }
}
</style>
```

### 3.3 鍵盤導航

```typescript
// 自動完成的鍵盤控制
export function handleAutocompleteKeydown(
  event: KeyboardEvent,
  state: AutocompleteState
): boolean {
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault();
      state.activeIndex = Math.min(
        state.activeIndex + 1,
        state.items.length - 1
      );
      return true;

    case 'ArrowUp':
      event.preventDefault();
      state.activeIndex = Math.max(state.activeIndex - 1, 0);
      return true;

    case 'Enter':
    case 'Tab':
      event.preventDefault();
      if (state.items[state.activeIndex]) {
        selectItem(state.items[state.activeIndex]);
        return true;
      }
      return false;

    case 'Escape':
      event.preventDefault();
      closeAutocomplete();
      return true;

    default:
      return false;
  }
}
```

---

## 4. 語法高亮系統

### 4.1 CodeMirror 6 配置

```typescript
// expression-editor-setup.ts
import { EditorView, basicSetup } from 'codemirror';
import { javascript } from '@codemirror/lang-javascript';
import { syntaxHighlighting, HighlightStyle } from '@codemirror/language';
import { tags } from '@lezer/highlight';

/**
 * 創建表達式編輯器
 */
export function createExpressionEditor(
  element: HTMLElement,
  options: ExpressionEditorOptions
): EditorView {
  const extensions = [
    basicSetup,
    javascript(),
    createExpressionTheme(),
    createExpressionHighlighter(),
    createAutocompleteExtension(options.context),
    EditorView.updateListener.of(update => {
      if (update.docChanged) {
        options.onChange?.(update.state.doc.toString());
      }
    }),
  ];

  return new EditorView({
    parent: element,
    doc: options.initialValue,
    extensions,
  });
}

/**
 * 創建主題
 */
function createExpressionTheme() {
  return EditorView.theme({
    '&': {
      fontSize: '14px',
      fontFamily: '"Monaco", "Menlo", "Consolas", monospace',
    },
    '.cm-content': {
      padding: '8px',
      minHeight: '36px',
    },
    '.cm-line': {
      padding: '0 4px',
    },
    '&.cm-focused': {
      outline: 'none',
    },
    '.cm-cursor': {
      borderLeftColor: 'var(--color--primary)',
    },
    '.cm-selectionBackground': {
      backgroundColor: 'var(--color--primary--tint-3) !important',
    },
  });
}

/**
 * 創建語法高亮
 */
function createExpressionHighlighter() {
  const highlightStyle = HighlightStyle.define([
    // 上下文變量（$json, $node, $env等）
    {
      tag: tags.variableName,
      color: 'var(--color--primary)',
      fontWeight: '600',
    },
    // 字符串
    {
      tag: tags.string,
      color: '#22863a',  // 綠色
    },
    // 數字
    {
      tag: tags.number,
      color: '#005cc5',  // 藍色
    },
    // 布爾值和 null
    {
      tag: tags.bool,
      color: '#d73a49',  // 紅色
    },
    {
      tag: tags.null,
      color: '#d73a49',
    },
    // 關鍵字（if, else, return等）
    {
      tag: tags.keyword,
      color: '#d73a49',
      fontWeight: '600',
    },
    // 函數名
    {
      tag: tags.function(tags.variableName),
      color: '#6f42c1',  // 紫色
    },
    // 屬性名
    {
      tag: tags.propertyName,
      color: '#005cc5',
    },
    // 操作符
    {
      tag: tags.operator,
      color: '#d73a49',
    },
    // 註釋
    {
      tag: tags.comment,
      color: '#6a737d',
      fontStyle: 'italic',
    },
  ]);

  return syntaxHighlighting(highlightStyle);
}
```

### 4.2 自定義語法高亮 - 表達式包裹器

```typescript
// expression-wrapper-highlighter.ts
import { Decoration, DecorationSet, EditorView, ViewPlugin, ViewUpdate } from '@codemirror/view';

/**
 * 高亮 {{ }} 包裹器
 */
export const expressionWrapperHighlighter = ViewPlugin.fromClass(
  class {
    decorations: DecorationSet;

    constructor(view: EditorView) {
      this.decorations = this.buildDecorations(view);
    }

    update(update: ViewUpdate) {
      if (update.docChanged || update.viewportChanged) {
        this.decorations = this.buildDecorations(update.view);
      }
    }

    buildDecorations(view: EditorView): DecorationSet {
      const decorations: any[] = [];
      const regex = /\{\{|\}\}/g;

      for (let { from, to } of view.visibleRanges) {
        const text = view.state.doc.sliceString(from, to);
        let match;

        while ((match = regex.exec(text)) !== null) {
          const start = from + match.index;
          const end = start + match[0].length;

          decorations.push(
            Decoration.mark({
              class: 'cm-expression-wrapper',
            }).range(start, end)
          );
        }
      }

      return Decoration.set(decorations);
    }
  },
  {
    decorations: v => v.decorations,
  }
);

// CSS 樣式
const styles = `
.cm-expression-wrapper {
  color: var(--color--primary);
  font-weight: 700;
  opacity: 0.7;
}
`;
```

### 4.3 錯誤標記

```typescript
// expression-linter.ts
import { Diagnostic, linter } from '@codemirror/lint';

/**
 * 表達式 Linter
 */
export const expressionLinter = linter(view => {
  const diagnostics: Diagnostic[] = [];
  const text = view.state.doc.toString();

  // 解析所有表達式
  const expressions = parseExpressions(text);

  for (const expr of expressions) {
    try {
      // 嘗試解析表達式
      const ast = parse(expr.expression, {
        sourceType: 'script',
        plugins: ['objectRestSpread'],
      });

      // 安全檢查
      validateSafety(ast);
    } catch (error) {
      // 添加錯誤標記
      diagnostics.push({
        from: expr.start,
        to: expr.end,
        severity: 'error',
        message: error.message,
      });
    }
  }

  return diagnostics;
});

// 錯誤標記樣式
const lintStyles = EditorView.theme({
  '.cm-diagnostic-error': {
    borderBottom: '2px wavy var(--color--danger)',
  },
  '.cm-diagnostic': {
    padding: '2px 4px',
    marginLeft: '4px',
  },
  '.cm-diagnostic-error .cm-diagnostic-icon': {
    color: 'var(--color--danger)',
  },
});
```

---

## 5. 參數類型系統

### 5.1 參數類型定義

```typescript
// parameter.types.ts

/**
 * 參數類型枚舉
 */
export enum ParameterType {
  STRING = 'string',
  NUMBER = 'number',
  BOOLEAN = 'boolean',
  JSON = 'json',
  OPTIONS = 'options',          // 下拉選擇
  MULTI_OPTIONS = 'multiOptions', // 多選
  COLLECTION = 'collection',    // 鍵值對集合
  FIXED_COLLECTION = 'fixedCollection',
  COLOR = 'color',
  DATE_TIME = 'dateTime',
  CREDENTIALS = 'credentials',
  NOTICE = 'notice',            // 純文本提示
}

/**
 * 基礎參數定義
 */
export interface BaseParameter {
  name: string;                 // 參數名稱
  displayName: string;          // 顯示名稱
  type: ParameterType;          // 參數類型
  default?: any;                // 默認值
  description?: string;         // 描述
  required?: boolean;           // 是否必填
  placeholder?: string;         // 佔位符
  hint?: string;                // 提示文本
  displayOptions?: {            // 顯示條件
    show?: Record<string, any[]>;
    hide?: Record<string, any[]>;
  };
  supportsExpression?: boolean; // 是否支持表達式
  displayPriority?: 'high' | 'medium' | 'low';  // 顯示優先級
}

/**
 * 字符串參數
 */
export interface StringParameter extends BaseParameter {
  type: ParameterType.STRING;
  typeOptions?: {
    password?: boolean;         // 密碼類型
    multiLine?: boolean;        // 多行文本
    rows?: number;              // 多行行數
    maxLength?: number;         // 最大長度
  };
}

/**
 * 數字參數
 */
export interface NumberParameter extends BaseParameter {
  type: ParameterType.NUMBER;
  typeOptions?: {
    min?: number;
    max?: number;
    step?: number;
    numberPrecision?: number;
  };
}

/**
 * 布爾參數
 */
export interface BooleanParameter extends BaseParameter {
  type: ParameterType.BOOLEAN;
}

/**
 * 選項參數
 */
export interface OptionsParameter extends BaseParameter {
  type: ParameterType.OPTIONS;
  options: Array<{
    name: string;
    value: string | number;
    description?: string;
  }>;
}

/**
 * 集合參數
 */
export interface CollectionParameter extends BaseParameter {
  type: ParameterType.COLLECTION;
  typeOptions: {
    multipleValues?: boolean;
    multipleValueButtonText?: string;
  };
  options: BaseParameter[];
}

// 聯合類型
export type Parameter =
  | StringParameter
  | NumberParameter
  | BooleanParameter
  | OptionsParameter
  | CollectionParameter;
```

### 5.2 參數組件映射

```vue
<!-- ParameterField.vue -->
<template>
  <div class="parameter-field" :class="{ required: parameter.required }">
    <!-- 標籤和提示 -->
    <div class="field-header">
      <label class="field-label">
        {{ parameter.displayName }}
        <span v-if="parameter.required" class="required-mark">*</span>
      </label>
      <button
        v-if="parameter.supportsExpression"
        class="btn-expression"
        :class="{ active: isExpression }"
        @click="toggleExpressionMode"
        title="使用表達式"
      >
        <IconFunction :size="14" />
      </button>
    </div>

    <!-- 描述 -->
    <p v-if="parameter.description" class="field-description">
      {{ parameter.description }}
    </p>

    <!-- 輸入組件 -->
    <component
      :is="getInputComponent(parameter.type)"
      :parameter="parameter"
      :value="value"
      :is-expression="isExpression"
      @update="handleUpdate"
    />

    <!-- 提示 -->
    <p v-if="parameter.hint" class="field-hint">
      {{ parameter.hint }}
    </p>

    <!-- 驗證錯誤 -->
    <p v-if="validationError" class="field-error">
      {{ validationError }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { Parameter } from '@/types/parameter.types';
import ParameterInputString from './inputs/ParameterInputString.vue';
import ParameterInputNumber from './inputs/ParameterInputNumber.vue';
import ParameterInputBoolean from './inputs/ParameterInputBoolean.vue';
import ParameterInputOptions from './inputs/ParameterInputOptions.vue';
import ParameterInputCollection from './inputs/ParameterInputCollection.vue';
import ParameterInputExpression from './inputs/ParameterInputExpression.vue';

interface Props {
  parameter: Parameter;
  value: any;
  nodeId: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update': [value: any];
}>();

const isExpression = ref(false);
const validationError = ref<string | null>(null);

function getInputComponent(type: ParameterType) {
  if (isExpression.value) {
    return ParameterInputExpression;
  }

  const componentMap = {
    [ParameterType.STRING]: ParameterInputString,
    [ParameterType.NUMBER]: ParameterInputNumber,
    [ParameterType.BOOLEAN]: ParameterInputBoolean,
    [ParameterType.OPTIONS]: ParameterInputOptions,
    [ParameterType.COLLECTION]: ParameterInputCollection,
  };

  return componentMap[type] || ParameterInputString;
}

function toggleExpressionMode() {
  isExpression.value = !isExpression.value;
}

function handleUpdate(newValue: any) {
  // 驗證
  const error = validateValue(newValue, props.parameter);
  validationError.value = error;

  if (!error) {
    emit('update', newValue);
  }
}
</script>

<style scoped lang="scss">
.parameter-field {
  display: flex;
  flex-direction: column;
  gap: 6px;

  &.required {
    .field-label::after {
      content: '*';
      color: var(--color--danger);
      margin-left: 4px;
    }
  }

  .field-header {
    display: flex;
    align-items: center;
    justify-content: space-between;

    .field-label {
      font-size: 13px;
      font-weight: 500;
      color: var(--color--text);

      .required-mark {
        color: var(--color--danger);
      }
    }

    .btn-expression {
      padding: 4px;
      background: none;
      border: 1px solid var(--color--foreground--tint-1);
      border-radius: var(--radius);
      cursor: pointer;
      color: var(--color--text--tint-1);
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--color--primary);
        color: var(--color--primary);
      }

      &.active {
        background: var(--color--primary--tint-3);
        border-color: var(--color--primary);
        color: var(--color--primary);
      }
    }
  }

  .field-description {
    font-size: 12px;
    color: var(--color--text--tint-1);
    line-height: 1.4;
    margin: 0;
  }

  .field-hint {
    font-size: 12px;
    color: var(--color--text--tint-2);
    font-style: italic;
    margin: 0;
  }

  .field-error {
    font-size: 12px;
    color: var(--color--danger);
    margin: 0;
  }
}
</style>
```

### 5.3 具體參數輸入組件

#### 字符串輸入
```vue
<!-- ParameterInputString.vue -->
<template>
  <textarea
    v-if="parameter.typeOptions?.multiLine"
    :value="value"
    :placeholder="parameter.placeholder"
    :rows="parameter.typeOptions?.rows || 3"
    class="input-textarea"
    @input="handleInput"
  />
  <input
    v-else
    :type="parameter.typeOptions?.password ? 'password' : 'text'"
    :value="value"
    :placeholder="parameter.placeholder"
    class="input-text"
    @input="handleInput"
  />
</template>

<style scoped lang="scss">
.input-text,
.input-textarea {
  width: 100%;
  padding: 8px 12px;
  font-size: 14px;
  border: 1px solid var(--color--foreground--tint-1);
  border-radius: var(--radius);
  background: var(--color--background);
  color: var(--color--text);
  transition: border-color 0.2s ease;

  &:focus {
    outline: none;
    border-color: var(--color--primary);
    box-shadow: 0 0 0 3px var(--color--primary--tint-3);
  }

  &::placeholder {
    color: var(--color--text--tint-2);
  }
}

.input-textarea {
  resize: vertical;
  font-family: inherit;
}
</style>
```

#### 數字輸入
```vue
<!-- ParameterInputNumber.vue -->
<template>
  <div class="input-number-wrapper">
    <input
      type="number"
      :value="value"
      :min="parameter.typeOptions?.min"
      :max="parameter.typeOptions?.max"
      :step="parameter.typeOptions?.step"
      :placeholder="parameter.placeholder"
      class="input-number"
      @input="handleInput"
    />
    <div v-if="parameter.typeOptions" class="input-stepper">
      <button
        class="btn-step"
        @click="increment"
        :disabled="isMaxReached"
      >
        <IconChevronUp :size="14" />
      </button>
      <button
        class="btn-step"
        @click="decrement"
        :disabled="isMinReached"
      >
        <IconChevronDown :size="14" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const isMaxReached = computed(() =>
  props.parameter.typeOptions?.max !== undefined &&
  props.value >= props.parameter.typeOptions.max
);

const isMinReached = computed(() =>
  props.parameter.typeOptions?.min !== undefined &&
  props.value <= props.parameter.typeOptions.min
);

function increment() {
  const step = props.parameter.typeOptions?.step || 1;
  emit('update', props.value + step);
}

function decrement() {
  const step = props.parameter.typeOptions?.step || 1;
  emit('update', props.value - step);
}
</script>
```

#### 下拉選擇
```vue
<!-- ParameterInputOptions.vue -->
<template>
  <select
    :value="value"
    class="input-select"
    @change="handleChange"
  >
    <option value="" disabled>
      {{ parameter.placeholder || '請選擇...' }}
    </option>
    <option
      v-for="option in parameter.options"
      :key="option.value"
      :value="option.value"
    >
      {{ option.name }}
    </option>
  </select>
</template>

<style scoped lang="scss">
.input-select {
  width: 100%;
  padding: 8px 32px 8px 12px;
  font-size: 14px;
  border: 1px solid var(--color--foreground--tint-1);
  border-radius: var(--radius);
  background: var(--color--background);
  color: var(--color--text);
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml,...");  // 自定義下拉箭頭
  background-repeat: no-repeat;
  background-position: right 12px center;
  transition: border-color 0.2s ease;

  &:focus {
    outline: none;
    border-color: var(--color--primary);
    box-shadow: 0 0 0 3px var(--color--primary--tint-3);
  }
}
</style>
```

---

## 6. 實時預覽與驗證

### 6.1 表達式實時預覽

```vue
<!-- ExpressionPreview.vue -->
<template>
  <div class="expression-preview" :class="{ error: hasError }">
    <div class="preview-header">
      <span class="preview-label">預覽</span>
      <span v-if="isLoading" class="preview-loading">
        <IconLoader :size="14" class="spin" />
      </span>
      <span v-else-if="hasError" class="preview-status error">
        <IconAlertCircle :size="14" />
        錯誤
      </span>
      <span v-else class="preview-status success">
        <IconCheck :size="14" />
        有效
      </span>
    </div>

    <div class="preview-content">
      <pre v-if="!hasError" class="preview-result">{{ formattedResult }}</pre>
      <div v-else class="preview-error">
        <p class="error-message">{{ error }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { debounce } from 'lodash-es';
import { evaluateExpression } from '@/utils/expression-parser';

interface Props {
  expression: string;
  context: ExpressionContext;
}

const props = defineProps<Props>();

const result = ref<any>(null);
const error = ref<string | null>(null);
const isLoading = ref(false);

const hasError = computed(() => error.value !== null);

const formattedResult = computed(() => {
  if (result.value === null || result.value === undefined) {
    return 'null';
  }
  if (typeof result.value === 'object') {
    return JSON.stringify(result.value, null, 2);
  }
  return String(result.value);
});

// 防抖更新
const debouncedEvaluate = debounce(async () => {
  if (!props.expression.trim()) {
    result.value = null;
    error.value = null;
    return;
  }

  isLoading.value = true;

  try {
    const evaluation = await evaluateExpression(
      props.expression,
      props.context
    );

    if (evaluation.success) {
      result.value = evaluation.value;
      error.value = null;
    } else {
      result.value = null;
      error.value = evaluation.error || '未知錯誤';
    }
  } catch (e) {
    result.value = null;
    error.value = e.message;
  } finally {
    isLoading.value = false;
  }
}, 300);

watch(
  () => props.expression,
  () => {
    debouncedEvaluate();
  },
  { immediate: true }
);
</script>

<style scoped lang="scss">
.expression-preview {
  border: 1px solid var(--color--foreground--tint-1);
  border-radius: var(--radius);
  background: var(--color--background--shade-1);
  overflow: hidden;

  &.error {
    border-color: var(--color--danger--tint-1);
    background: rgba(244, 67, 54, 0.05);
  }

  .preview-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 8px 12px;
    background: var(--color--background--shade-2);
    border-bottom: 1px solid var(--color--foreground--tint-2);

    .preview-label {
      font-size: 12px;
      font-weight: 600;
      color: var(--color--text--tint-1);
      text-transform: uppercase;
    }

    .preview-status {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 12px;
      font-weight: 500;

      &.success {
        color: var(--color--success);
      }

      &.error {
        color: var(--color--danger);
      }
    }

    .preview-loading {
      .spin {
        animation: spin 1s linear infinite;
      }
    }
  }

  .preview-content {
    padding: 12px;

    .preview-result {
      margin: 0;
      font-size: 13px;
      font-family: 'Monaco', 'Menlo', monospace;
      color: var(--color--text);
      white-space: pre-wrap;
      word-break: break-all;
    }

    .preview-error {
      .error-message {
        margin: 0;
        font-size: 13px;
        color: var(--color--danger);
        line-height: 1.5;
      }
    }
  }
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>
```

### 6.2 參數驗證系統

```typescript
// parameter-validator.ts

/**
 * 驗證參數值
 */
export function validateValue(
  value: any,
  parameter: Parameter
): string | null {
  // 必填檢查
  if (parameter.required && (value === null || value === undefined || value === '')) {
    return `${parameter.displayName} 是必填項`;
  }

  // 類型特定驗證
  switch (parameter.type) {
    case ParameterType.STRING:
      return validateString(value, parameter as StringParameter);

    case ParameterType.NUMBER:
      return validateNumber(value, parameter as NumberParameter);

    case ParameterType.OPTIONS:
      return validateOptions(value, parameter as OptionsParameter);

    default:
      return null;
  }
}

/**
 * 驗證字符串
 */
function validateString(
  value: string,
  parameter: StringParameter
): string | null {
  if (typeof value !== 'string') {
    return '必須是字符串類型';
  }

  const { maxLength } = parameter.typeOptions || {};

  if (maxLength && value.length > maxLength) {
    return `最大長度為 ${maxLength} 個字符`;
  }

  return null;
}

/**
 * 驗證數字
 */
function validateNumber(
  value: number,
  parameter: NumberParameter
): string | null {
  if (typeof value !== 'number' || isNaN(value)) {
    return '必須是有效數字';
  }

  const { min, max } = parameter.typeOptions || {};

  if (min !== undefined && value < min) {
    return `最小值為 ${min}`;
  }

  if (max !== undefined && value > max) {
    return `最大值為 ${max}`;
  }

  return null;
}

/**
 * 驗證選項
 */
function validateOptions(
  value: any,
  parameter: OptionsParameter
): string | null {
  const validValues = parameter.options.map(opt => opt.value);

  if (!validValues.includes(value)) {
    return '無效的選項值';
  }

  return null;
}
```

---

## 7. 表達式編輯器實現

### 7.1 完整的表達式編輯器組件

```vue
<!-- ExpressionEditor.vue -->
<template>
  <div class="expression-editor-wrapper">
    <div class="expression-editor" ref="editorElement"></div>
    <ExpressionPreview
      v-if="showPreview"
      :expression="currentValue"
      :context="expressionContext"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref, watch } from 'vue';
import { EditorView } from '@codemirror/view';
import { createExpressionEditor } from '@/utils/expression-editor-setup';
import type { ExpressionContext } from '@/types/expression.types';
import ExpressionPreview from './ExpressionPreview.vue';

interface Props {
  value: string;
  context: ExpressionContext;
  placeholder?: string;
  showPreview?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showPreview: true,
});

const emit = defineEmits<{
  'update': [value: string];
}>();

const editorElement = ref<HTMLElement | null>(null);
const currentValue = ref(props.value);
let editorView: EditorView | null = null;

onMounted(() => {
  if (!editorElement.value) return;

  editorView = createExpressionEditor(editorElement.value, {
    initialValue: props.value,
    context: props.context,
    placeholder: props.placeholder,
    onChange: (value) => {
      currentValue.value = value;
      emit('update', value);
    },
  });
});

onUnmounted(() => {
  if (editorView) {
    editorView.destroy();
  }
});

watch(
  () => props.value,
  (newValue) => {
    if (editorView && newValue !== currentValue.value) {
      const transaction = editorView.state.update({
        changes: {
          from: 0,
          to: editorView.state.doc.length,
          insert: newValue,
        },
      });
      editorView.dispatch(transaction);
      currentValue.value = newValue;
    }
  }
);
</script>

<style scoped lang="scss">
.expression-editor-wrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.expression-editor {
  border: 1px solid var(--color--foreground--tint-1);
  border-radius: var(--radius);
  overflow: hidden;
  transition: border-color 0.2s ease;

  &:focus-within {
    border-color: var(--color--primary);
    box-shadow: 0 0 0 3px var(--color--primary--tint-3);
  }

  :deep(.cm-editor) {
    background: var(--color--background);
  }
}
</style>
```

### 7.2 內聯表達式編輯器（簡化版）

```vue
<!-- InlineExpressionEditor.vue -->
<template>
  <div class="inline-expression-editor">
    <div class="editor-input" ref="editorElement"></div>
    <button
      v-if="hasExpression"
      class="btn-clear"
      @click="clearExpression"
      title="清除表達式"
    >
      <IconX :size="14" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { EditorView, minimalSetup } from 'codemirror';
import { javascript } from '@codemirror/lang-javascript';

const props = defineProps<{
  value: string;
}>();

const emit = defineEmits<{
  'update': [value: string];
}>();

const editorElement = ref<HTMLElement | null>(null);
const hasExpression = computed(() =>
  /\{\{.+?\}\}/.test(props.value)
);

onMounted(() => {
  if (!editorElement.value) return;

  new EditorView({
    parent: editorElement.value,
    doc: props.value,
    extensions: [
      minimalSetup,
      javascript(),
      EditorView.updateListener.of(update => {
        if (update.docChanged) {
          emit('update', update.state.doc.toString());
        }
      }),
    ],
  });
});

function clearExpression() {
  // 移除所有表達式
  const cleaned = props.value.replace(/\{\{.+?\}\}/g, '');
  emit('update', cleaned);
}
</script>

<style scoped lang="scss">
.inline-expression-editor {
  position: relative;
  display: flex;
  align-items: center;
  gap: 4px;

  .editor-input {
    flex: 1;
    min-width: 0;
  }

  .btn-clear {
    flex-shrink: 0;
    padding: 4px;
    background: var(--color--danger--tint-3);
    border: none;
    border-radius: var(--radius);
    color: var(--color--danger);
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: var(--color--danger--tint-2);
    }
  }
}
</style>
```

---

## 8. 完整組件示例

### 8.1 HTTP Request 節點配置（使用表達式）

```vue
<!-- NodeConfigHttpRequest.vue -->
<template>
  <NodeConfigurable
    :node-id="nodeId"
    :node-name="data.name"
    :node-icon="httpIcon"
    :is-expanded="isExpanded"
    @toggle-expand="toggleExpand"
    @open-detail="openDetail"
  >
    <template #config-content>
      <!-- HTTP Method -->
      <ParameterField
        :parameter="parameters.method"
        :value="config.method"
        :node-id="nodeId"
        @update="updateConfig('method', $event)"
      />

      <!-- URL with Expression Support -->
      <ParameterField
        :parameter="parameters.url"
        :value="config.url"
        :node-id="nodeId"
        @update="updateConfig('url', $event)"
      >
        <template #input="{ isExpression }">
          <ExpressionEditor
            v-if="isExpression"
            :value="config.url"
            :context="expressionContext"
            placeholder="https://api.example.com/users/{{ $json.id }}"
            @update="updateConfig('url', $event)"
          />
          <input
            v-else
            type="text"
            :value="config.url"
            placeholder="https://api.example.com/users"
            @input="updateConfig('url', $event.target.value)"
          />
        </template>
      </ParameterField>

      <!-- Headers Collection -->
      <ParameterField
        :parameter="parameters.headers"
        :value="config.headers"
        :node-id="nodeId"
        @update="updateConfig('headers', $event)"
      />

      <!-- Body (for POST/PUT/PATCH) -->
      <ParameterField
        v-if="['POST', 'PUT', 'PATCH'].includes(config.method)"
        :parameter="parameters.body"
        :value="config.body"
        :node-id="nodeId"
        @update="updateConfig('body', $event)"
      />
    </template>
  </NodeConfigurable>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useExpressionContext } from '@/composables/useExpressionContext';
import NodeConfigurable from './NodeConfigurable.vue';
import ParameterField from './ParameterField.vue';
import ExpressionEditor from './ExpressionEditor.vue';

const props = defineProps<{
  nodeId: string;
  data: NodeData;
}>();

const isExpanded = ref(false);

const config = computed(() => props.data.parameters);

const expressionContext = useExpressionContext(props.nodeId);

const parameters = {
  method: {
    name: 'method',
    displayName: 'HTTP Method',
    type: ParameterType.OPTIONS,
    default: 'GET',
    options: [
      { name: 'GET', value: 'GET' },
      { name: 'POST', value: 'POST' },
      { name: 'PUT', value: 'PUT' },
      { name: 'PATCH', value: 'PATCH' },
      { name: 'DELETE', value: 'DELETE' },
    ],
    displayPriority: 'high',
  },
  url: {
    name: 'url',
    displayName: 'URL',
    type: ParameterType.STRING,
    required: true,
    placeholder: 'https://api.example.com/endpoint',
    supportsExpression: true,
    displayPriority: 'high',
  },
  headers: {
    name: 'headers',
    displayName: 'Headers',
    type: ParameterType.COLLECTION,
    default: [],
    typeOptions: {
      multipleValues: true,
      multipleValueButtonText: 'Add Header',
    },
    options: [
      {
        name: 'key',
        displayName: 'Key',
        type: ParameterType.STRING,
      },
      {
        name: 'value',
        displayName: 'Value',
        type: ParameterType.STRING,
        supportsExpression: true,
      },
    ],
    displayPriority: 'medium',
  },
  body: {
    name: 'body',
    displayName: 'Body',
    type: ParameterType.JSON,
    supportsExpression: true,
    displayPriority: 'high',
  },
};

function toggleExpand() {
  isExpanded.value = !isExpanded.value;
}

function openDetail() {
  // 打開完整編輯器
  emit('open-detail', props.nodeId);
}

function updateConfig(key: string, value: any) {
  emit('update-parameter', key, value);
}
</script>
```

---

## 9. 總結

Part 2 建立了完整的內聯配置和表達式系統：

### ✅ 已實現

1. **內聯配置系統**
   - 展開/收起動畫和交互
   - 收起狀態（48px 高度）
   - 展開狀態（最大 300px 或視口 80%）
   - 鍵盤快捷鍵支持

2. **表達式語法系統**
   - `$json`, `$node`, `$env`, `$now` 等上下文變量
   - 完整的 JavaScript 表達式支持
   - 安全沙箱機制
   - 表達式解析器實現

3. **智能提示與自動完成**
   - 上下文感知的自動完成
   - JSON 字段提示
   - 節點數據提示
   - 環境變量提示
   - 方法簽名提示

4. **語法高亮系統**
   - 基於 CodeMirror 6
   - 自定義高亮規則
   - 表達式包裝器（{{ }}）高亮
   - 錯誤標記

5. **參數類型系統**
   - 9+ 種參數類型
   - 類型特定的輸入組件
   - 表達式模式切換
   - 參數驗證

6. **實時預覽與驗證**
   - 表達式實時求值
   - 錯誤檢測和顯示
   - 防抖更新（300ms）
   - 格式化輸出

### 📋 下一步 (Part 3)

在 Part 3 中，我們將設計：

1. **執行狀態可視化** - 節點執行動畫和狀態轉換
2. **數據流可視化** - 連接線數據計數和流動動畫
3. **執行日誌** - 詳細的執行記錄和調試信息
4. **錯誤處理** - 錯誤顯示和恢復機制
5. **Pinned 數據** - 固定測試數據功能

---

**文檔版本**: 2.0
**創建日期**: 2025-10-29
**最後更新**: 2025-10-29
**下一部分**: Part 3 - 執行視覺化與數據流
