# Workflow Editor 設計文檔 V2 - Part 3

**執行視覺化與數據流 (n8n 風格)**

> 版本: 2.0
> 日期: 2025-10-29
> 狀態: 設計中
> 基於: n8n 開源項目架構分析
> 前置: Part 1 (核心設計), Part 2 (表達式系統)

---

## 目錄

1. [執行狀態系統](#1-執行狀態系統)
2. [節點執行動畫](#2-節點執行動畫)
3. [連接線可視化](#3-連接線可視化)
4. [執行日誌系統](#4-執行日誌系統)
5. [錯誤處理機制](#5-錯誤處理機制)
6. [Pinned 數據功能](#6-pinned-數據功能)
7. [執行控制面板](#7-執行控制面板)
8. [完整執行流程](#8-完整執行流程)

---

## 1. 執行狀態系統

### 1.1 執行狀態定義

```typescript
// execution-state.types.ts

/**
 * 節點執行狀態
 */
export enum NodeExecutionStatus {
  NEW = 'new',                 // 新建，未執行
  WAITING = 'waiting',         // 等待中（等待觸發或上游節點）
  RUNNING = 'running',         // 執行中
  SUCCESS = 'success',         // 執行成功
  ERROR = 'error',             // 執行錯誤
  CANCELED = 'canceled',       // 已取消
  WAITING_FOR_NEXT = 'waiting_for_next', // 等待下一次執行（循環）
}

/**
 * 節點執行數據
 */
export interface NodeExecutionData {
  nodeId: string;
  nodeName: string;
  status: NodeExecutionStatus;
  startedAt?: number;          // 開始時間戳
  finishedAt?: number;         // 結束時間戳
  duration?: number;           // 執行時長（毫秒）
  inputData?: any[];           // 輸入數據
  outputData?: any[];          // 輸出數據
  error?: ExecutionError;      // 錯誤信息
  metadata?: {
    itemsProcessed: number;    // 處理的數據項數量
    iterations: number;        // 迭代次數
  };
}

/**
 * 執行錯誤
 */
export interface ExecutionError {
  message: string;
  description?: string;
  stack?: string;
  httpCode?: number;
  cause?: any;
}

/**
 * 工作流執行狀態
 */
export interface WorkflowExecutionState {
  id: string;                  // 執行 ID
  workflowId: string;          // 工作流 ID
  status: 'running' | 'success' | 'error' | 'canceled';
  mode: 'manual' | 'trigger' | 'webhook';
  startedAt: number;
  finishedAt?: number;
  duration?: number;
  nodeExecutions: Record<string, NodeExecutionData>;
  triggerNode?: string;        // 觸發節點
}
```

### 1.2 狀態顏色映射

基於 n8n 的設計，每種狀態都有獨特的視覺表現：

```scss
// execution-colors.scss

// 狀態顏色變量
$status-colors: (
  'new': (
    border: var(--color--foreground--shade-2),
    background: var(--color--background),
    text: var(--color--text--tint-1),
  ),
  'waiting': (
    border: var(--color--secondary),
    background: var(--color--secondary--tint-2),
    text: var(--color--secondary--shade-1),
  ),
  'running': (
    border: var(--color--primary),
    background: var(--node--color--background--executing),
    text: var(--color--primary--shade-1),
  ),
  'success': (
    border: var(--color--success),
    background: var(--color--background),
    text: var(--color--success--shade-1),
  ),
  'error': (
    border: var(--color--danger),
    background: var(--color--danger--tint-4),
    text: var(--color--danger--shade-1),
  ),
  'canceled': (
    border: var(--color--text--tint-2),
    background: var(--color--background--shade-1),
    text: var(--color--text--tint-1),
  ),
);

// 應用狀態顏色
.node {
  @each $status, $colors in $status-colors {
    &.status-#{$status} {
      border-color: map-get($colors, 'border');
      background: map-get($colors, 'background');

      .node-status-text {
        color: map-get($colors, 'text');
      }
    }
  }
}
```

### 1.3 狀態優先級規則

當一個節點同時具有多個狀態標記時，按以下優先級顯示：

```typescript
/**
 * 狀態優先級（從高到低）
 */
export const STATUS_PRIORITY = [
  NodeExecutionStatus.ERROR,           // 最高優先級
  NodeExecutionStatus.RUNNING,
  NodeExecutionStatus.WAITING_FOR_NEXT,
  NodeExecutionStatus.WAITING,
  NodeExecutionStatus.CANCELED,
  NodeExecutionStatus.SUCCESS,
  NodeExecutionStatus.NEW,             // 最低優先級
] as const;

/**
 * 計算節點的有效狀態
 */
export function getEffectiveStatus(
  executionStatus: NodeExecutionStatus,
  hasError: boolean,
  isRunning: boolean,
  isPinned: boolean,
  isDisabled: boolean
): NodeExecutionStatus {
  // 禁用狀態覆蓋所有
  if (isDisabled) {
    return NodeExecutionStatus.NEW;
  }

  // Pinned 數據覆蓋執行狀態
  if (isPinned) {
    return NodeExecutionStatus.SUCCESS;
  }

  // 執行中覆蓋歷史狀態
  if (isRunning) {
    return NodeExecutionStatus.RUNNING;
  }

  // 錯誤覆蓋成功
  if (hasError) {
    return NodeExecutionStatus.ERROR;
  }

  return executionStatus;
}
```

---

## 2. 節點執行動畫

### 2.1 執行中動畫（脈衝效果）

```scss
// node-execution-animation.scss

// 脈衝動畫 - 執行中
@keyframes pulse-border {
  0%, 100% {
    border-color: var(--color--primary);
    box-shadow: 0 0 0 0 var(--color--primary--tint-2);
  }
  50% {
    border-color: var(--color--primary--shade-1);
    box-shadow: 0 0 0 8px var(--color--primary--tint-3);
  }
}

// 背景閃爍動畫
@keyframes shimmer-background {
  0% {
    background-position: -200% 0;
  }
  100% {
    background-position: 200% 0;
  }
}

// 圖標旋轉動畫
@keyframes spin-icon {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.node {
  // 執行中狀態
  &.status-running {
    animation: pulse-border 2s ease-in-out infinite;

    // 漸變背景動畫
    background: linear-gradient(
      90deg,
      var(--color--background) 0%,
      var(--color--primary--tint-3) 50%,
      var(--color--background) 100%
    );
    background-size: 200% 100%;
    animation: shimmer-background 2s linear infinite;

    // 圖標旋轉
    .node-icon {
      animation: spin-icon 2s linear infinite;
    }
  }

  // 等待中狀態（較慢的脈衝）
  &.status-waiting {
    animation: pulse-border 3s ease-in-out infinite;
  }
}
```

### 2.2 狀態轉換動畫

```scss
// 狀態轉換過渡
.node {
  transition:
    border-color 0.3s ease,
    background 0.3s ease,
    box-shadow 0.3s ease,
    transform 0.2s ease;

  // 成功閃爍
  &.status-success-flash {
    animation: success-flash 0.6s ease-out;
  }

  // 錯誤抖動
  &.status-error-shake {
    animation: error-shake 0.5s ease-out;
  }
}

// 成功閃爍動畫
@keyframes success-flash {
  0%, 100% {
    border-color: var(--color--success);
    transform: scale(1);
  }
  50% {
    border-color: var(--color--success--shade-1);
    transform: scale(1.05);
    box-shadow: 0 0 20px var(--color--success--tint-2);
  }
}

// 錯誤抖動動畫
@keyframes error-shake {
  0%, 100% {
    transform: translateX(0);
  }
  10%, 30%, 50%, 70%, 90% {
    transform: translateX(-5px);
  }
  20%, 40%, 60%, 80% {
    transform: translateX(5px);
  }
}
```

### 2.3 狀態指示器組件

```vue
<!-- NodeStatusIndicator.vue -->
<template>
  <div class="node-status-indicator" :class="`status-${status}`">
    <!-- 狀態圖標 -->
    <component
      :is="statusIcon"
      :size="16"
      :class="{ spinning: status === 'running' }"
    />

    <!-- 狀態文本 -->
    <span v-if="showText" class="status-text">
      {{ statusText }}
    </span>

    <!-- 執行時間 -->
    <span v-if="duration" class="status-duration">
      {{ formatDuration(duration) }}
    </span>

    <!-- 數據項計數 -->
    <span v-if="itemCount" class="status-items">
      {{ itemCount }} {{ itemCount === 1 ? 'item' : 'items' }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import {
  IconClock,
  IconCheck,
  IconX,
  IconLoader,
  IconAlertCircle,
  IconPause,
} from 'lucide-vue-next';
import type { NodeExecutionStatus } from '@/types/execution-state.types';

interface Props {
  status: NodeExecutionStatus;
  duration?: number;
  itemCount?: number;
  showText?: boolean;
}

const props = defineProps<Props>();

const statusIcon = computed(() => {
  const iconMap = {
    new: IconClock,
    waiting: IconClock,
    running: IconLoader,
    success: IconCheck,
    error: IconX,
    canceled: IconPause,
    waiting_for_next: IconClock,
  };
  return iconMap[props.status] || IconAlertCircle;
});

const statusText = computed(() => {
  const textMap = {
    new: '未執行',
    waiting: '等待中',
    running: '執行中',
    success: '成功',
    error: '錯誤',
    canceled: '已取消',
    waiting_for_next: '等待下次',
  };
  return textMap[props.status] || '未知';
});

function formatDuration(ms: number): string {
  if (ms < 1000) return `${ms}ms`;
  if (ms < 60000) return `${(ms / 1000).toFixed(1)}s`;
  return `${(ms / 60000).toFixed(1)}m`;
}
</script>

<style scoped lang="scss">
.node-status-indicator {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  border-radius: var(--radius);
  font-size: 12px;
  font-weight: 500;
  transition: all 0.2s ease;

  .spinning {
    animation: spin-icon 1s linear infinite;
  }

  // 狀態特定樣式
  &.status-new {
    background: var(--color--background--shade-1);
    color: var(--color--text--tint-1);
  }

  &.status-waiting {
    background: var(--color--secondary--tint-2);
    color: var(--color--secondary--shade-1);
  }

  &.status-running {
    background: var(--color--primary--tint-3);
    color: var(--color--primary);
  }

  &.status-success {
    background: var(--color--success--tint-3);
    color: var(--color--success);
  }

  &.status-error {
    background: var(--color--danger--tint-3);
    color: var(--color--danger);
  }

  &.status-canceled {
    background: var(--color--background--shade-1);
    color: var(--color--text--tint-2);
  }

  .status-text {
    white-space: nowrap;
  }

  .status-duration,
  .status-items {
    opacity: 0.8;
    font-size: 11px;
  }
}
</style>
```

---

## 3. 連接線可視化

### 3.1 連接線數據計數

```vue
<!-- EdgeDataCount.vue -->
<template>
  <div class="edge-data-count" :class="`status-${status}`">
    <!-- 數據項計數 -->
    <span class="count-badge">
      <IconDatabase :size="12" />
      {{ formattedCount }}
    </span>

    <!-- 迭代次數（多次執行） -->
    <span v-if="iterations > 1" class="iterations-badge">
      <IconRepeat :size="10" />
      {{ iterations }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { IconDatabase, IconRepeat } from 'lucide-vue-next';

interface Props {
  count: number;
  iterations?: number;
  status?: 'success' | 'error' | 'running' | 'pinned';
}

const props = withDefaults(defineProps<Props>(), {
  iterations: 1,
  status: 'success',
});

const formattedCount = computed(() => {
  if (props.count === 0) return '0 items';
  if (props.count === 1) return '1 item';

  // 多次迭代時顯示總數
  if (props.iterations > 1) {
    return `${props.count} total`;
  }

  return `${props.count} items`;
});
</script>

<style scoped lang="scss">
.edge-data-count {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  background: var(--color--background);
  border-radius: 12px;
  box-shadow: var(--shadow--sm);
  font-size: 11px;
  font-weight: 600;
  border: 1px solid;
  transition: all 0.2s ease;

  &.status-success {
    border-color: var(--color--success);
    color: var(--color--success);
  }

  &.status-error {
    border-color: var(--color--danger);
    color: var(--color--danger);
  }

  &.status-running {
    border-color: var(--color--primary);
    color: var(--color--primary);
    animation: pulse-badge 1.5s ease-in-out infinite;
  }

  &.status-pinned {
    border-color: var(--color--secondary);
    color: var(--color--secondary);
  }

  .count-badge,
  .iterations-badge {
    display: inline-flex;
    align-items: center;
    gap: 3px;
  }

  .iterations-badge {
    padding-left: 4px;
    border-left: 1px solid currentColor;
    opacity: 0.7;
  }
}

@keyframes pulse-badge {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}
</style>
```

### 3.2 數據流動動畫

```scss
// edge-flow-animation.scss

// 流動粒子動畫
@keyframes flow-particles {
  0% {
    stroke-dashoffset: 1000;
  }
  100% {
    stroke-dashoffset: 0;
  }
}

// 脈衝波動畫
@keyframes flow-pulse {
  0%, 100% {
    stroke-width: 2;
    opacity: 1;
  }
  50% {
    stroke-width: 4;
    opacity: 0.7;
  }
}

.canvas-edge {
  // 執行中的流動效果
  &.status-running {
    stroke-dasharray: 10 5;
    animation: flow-particles 2s linear infinite;

    // 添加發光效果
    filter: drop-shadow(0 0 4px var(--color--primary));
  }

  // 成功後的快速流動
  &.status-success-flow {
    animation: flow-particles 0.8s ease-out;
  }

  // 錯誤時的警告脈衝
  &.status-error {
    animation: flow-pulse 1s ease-in-out 3;
  }
}
```

### 3.3 連接線工具提示

```vue
<!-- EdgeTooltip.vue -->
<template>
  <div
    v-if="visible"
    class="edge-tooltip"
    :style="{ left: `${position.x}px`, top: `${position.y}px` }"
  >
    <div class="tooltip-header">
      <span class="source-node">{{ sourceNodeName }}</span>
      <IconArrowRight :size="14" />
      <span class="target-node">{{ targetNodeName }}</span>
    </div>

    <div class="tooltip-content">
      <!-- 數據統計 -->
      <div class="stat-row">
        <span class="stat-label">數據項:</span>
        <span class="stat-value">{{ dataCount }}</span>
      </div>

      <div v-if="iterations > 1" class="stat-row">
        <span class="stat-label">迭代次數:</span>
        <span class="stat-value">{{ iterations }}</span>
      </div>

      <div v-if="executionTime" class="stat-row">
        <span class="stat-label">執行時間:</span>
        <span class="stat-value">{{ executionTime }}ms</span>
      </div>

      <!-- 數據預覽 -->
      <div v-if="showPreview && previewData" class="data-preview">
        <div class="preview-label">數據預覽:</div>
        <pre class="preview-content">{{ previewData }}</pre>
      </div>
    </div>

    <div class="tooltip-actions">
      <button class="btn-tooltip" @click="viewFullData">
        查看完整數據
      </button>
    </div>
  </div>
</template>

<style scoped lang="scss">
.edge-tooltip {
  position: fixed;
  z-index: 1000;
  background: var(--color--background);
  border: 1px solid var(--color--foreground--tint-1);
  border-radius: var(--radius);
  box-shadow: var(--shadow--lg);
  padding: 12px;
  min-width: 250px;
  max-width: 400px;
  pointer-events: auto;

  .tooltip-header {
    display: flex;
    align-items: center;
    gap: 8px;
    padding-bottom: 8px;
    border-bottom: 1px solid var(--color--foreground--tint-2);
    margin-bottom: 8px;
    font-size: 13px;
    font-weight: 600;

    .source-node,
    .target-node {
      color: var(--color--text);
    }
  }

  .tooltip-content {
    display: flex;
    flex-direction: column;
    gap: 6px;

    .stat-row {
      display: flex;
      justify-content: space-between;
      font-size: 12px;

      .stat-label {
        color: var(--color--text--tint-1);
      }

      .stat-value {
        font-weight: 600;
        color: var(--color--text);
      }
    }

    .data-preview {
      margin-top: 8px;
      padding-top: 8px;
      border-top: 1px solid var(--color--foreground--tint-2);

      .preview-label {
        font-size: 11px;
        font-weight: 600;
        color: var(--color--text--tint-1);
        margin-bottom: 4px;
      }

      .preview-content {
        font-size: 11px;
        font-family: 'Monaco', 'Menlo', monospace;
        background: var(--color--background--shade-1);
        padding: 8px;
        border-radius: var(--radius);
        max-height: 150px;
        overflow: auto;
        margin: 0;
      }
    }
  }

  .tooltip-actions {
    margin-top: 8px;
    padding-top: 8px;
    border-top: 1px solid var(--color--foreground--tint-2);

    .btn-tooltip {
      width: 100%;
      padding: 6px 12px;
      font-size: 12px;
      background: var(--color--primary--tint-3);
      color: var(--color--primary);
      border: none;
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--color--primary--tint-2);
      }
    }
  }
}
</style>
```

---

## 4. 執行日誌系統

### 4.1 日誌數據結構

```typescript
// execution-log.types.ts

/**
 * 日誌級別
 */
export enum LogLevel {
  DEBUG = 'debug',
  INFO = 'info',
  WARN = 'warn',
  ERROR = 'error',
}

/**
 * 日誌條目
 */
export interface LogEntry {
  id: string;
  timestamp: number;
  level: LogLevel;
  nodeId?: string;
  nodeName?: string;
  message: string;
  details?: any;
  metadata?: {
    duration?: number;
    itemCount?: number;
    errorStack?: string;
  };
}

/**
 * 執行日誌
 */
export interface ExecutionLog {
  executionId: string;
  workflowId: string;
  entries: LogEntry[];
  startedAt: number;
  finishedAt?: number;
}
```

### 4.2 執行日誌面板

```vue
<!-- ExecutionLogPanel.vue -->
<template>
  <div class="execution-log-panel" :class="{ collapsed: isCollapsed }">
    <!-- 面板頭部 -->
    <div class="panel-header">
      <div class="header-left">
        <IconFileText :size="16" />
        <span class="panel-title">執行日誌</span>
        <span class="log-count">({{ filteredLogs.length }})</span>
      </div>
      <div class="header-actions">
        <!-- 日誌級別過濾 -->
        <select v-model="selectedLevel" class="level-filter">
          <option value="all">全部</option>
          <option value="debug">Debug</option>
          <option value="info">Info</option>
          <option value="warn">Warning</option>
          <option value="error">Error</option>
        </select>

        <!-- 清除日誌 -->
        <button class="btn-icon" @click="clearLogs" title="清除日誌">
          <IconTrash2 :size="14} />
        </button>

        <!-- 折疊/展開 -->
        <button class="btn-icon" @click="toggleCollapse" title="折疊/展開">
          <IconChevronDown :size="14" :class="{ rotated: !isCollapsed }" />
        </button>
      </div>
    </div>

    <!-- 日誌列表 -->
    <div v-if="!isCollapsed" class="panel-content">
      <div
        v-for="log in filteredLogs"
        :key="log.id"
        :class="['log-entry', `level-${log.level}`]"
        @click="selectLog(log)"
      >
        <!-- 時間戳 -->
        <span class="log-time">
          {{ formatTime(log.timestamp) }}
        </span>

        <!-- 級別圖標 -->
        <component
          :is="getLevelIcon(log.level)"
          :size="14"
          class="log-icon"
        />

        <!-- 節點名稱 -->
        <span v-if="log.nodeName" class="log-node">
          [{{ log.nodeName }}]
        </span>

        <!-- 消息 -->
        <span class="log-message">
          {{ log.message }}
        </span>

        <!-- 元數據 -->
        <span v-if="log.metadata?.duration" class="log-meta">
          ({{ log.metadata.duration }}ms)
        </span>
      </div>

      <!-- 空狀態 -->
      <div v-if="filteredLogs.length === 0" class="empty-state">
        <IconInbox :size="32" />
        <p>暫無日誌記錄</p>
      </div>
    </div>

    <!-- 詳細信息抽屜 -->
    <LogDetailDrawer
      v-if="selectedLog"
      :log="selectedLog"
      @close="selectedLog = null"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import {
  IconFileText,
  IconTrash2,
  IconChevronDown,
  IconInfo,
  IconAlertTriangle,
  IconXCircle,
  IconBug,
  IconInbox,
} from 'lucide-vue-next';
import type { LogEntry, LogLevel } from '@/types/execution-log.types';

interface Props {
  logs: LogEntry[];
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'clear': [];
}>();

const isCollapsed = ref(false);
const selectedLevel = ref<LogLevel | 'all'>('all');
const selectedLog = ref<LogEntry | null>(null);

const filteredLogs = computed(() => {
  if (selectedLevel.value === 'all') {
    return props.logs;
  }
  return props.logs.filter(log => log.level === selectedLevel.value);
});

function getLevelIcon(level: LogLevel) {
  const iconMap = {
    debug: IconBug,
    info: IconInfo,
    warn: IconAlertTriangle,
    error: IconXCircle,
  };
  return iconMap[level];
}

function formatTime(timestamp: number): string {
  const date = new Date(timestamp);
  return date.toLocaleTimeString('zh-TW', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    fractionalSecondDigits: 3,
  });
}

function toggleCollapse() {
  isCollapsed.value = !isCollapsed.value;
}

function clearLogs() {
  emit('clear');
}

function selectLog(log: LogEntry) {
  selectedLog.value = log;
}
</script>

<style scoped lang="scss">
.execution-log-panel {
  display: flex;
  flex-direction: column;
  background: var(--color--background);
  border-top: 1px solid var(--color--foreground--tint-2);
  max-height: 400px;
  transition: max-height 0.3s ease;

  &.collapsed {
    max-height: 40px;
  }

  .panel-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 8px 12px;
    background: var(--color--background--shade-1);
    border-bottom: 1px solid var(--color--foreground--tint-2);
    flex-shrink: 0;

    .header-left {
      display: flex;
      align-items: center;
      gap: 8px;

      .panel-title {
        font-size: 14px;
        font-weight: 600;
        color: var(--color--text);
      }

      .log-count {
        font-size: 12px;
        color: var(--color--text--tint-1);
      }
    }

    .header-actions {
      display: flex;
      align-items: center;
      gap: 8px;

      .level-filter {
        padding: 4px 8px;
        font-size: 12px;
        border: 1px solid var(--color--foreground--tint-1);
        border-radius: var(--radius);
        background: var(--color--background);
        cursor: pointer;
      }

      .btn-icon {
        padding: 4px;
        background: none;
        border: none;
        cursor: pointer;
        color: var(--color--text--tint-1);
        transition: color 0.2s ease;

        &:hover {
          color: var(--color--text);
        }

        .rotated {
          transform: rotate(180deg);
        }
      }
    }
  }

  .panel-content {
    flex: 1;
    overflow-y: auto;
    padding: 8px;

    .log-entry {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 6px 8px;
      border-radius: var(--radius);
      font-size: 12px;
      font-family: 'Monaco', 'Menlo', monospace;
      cursor: pointer;
      transition: background 0.2s ease;

      &:hover {
        background: var(--color--background--shade-1);
      }

      .log-time {
        color: var(--color--text--tint-2);
        flex-shrink: 0;
      }

      .log-icon {
        flex-shrink: 0;
      }

      .log-node {
        color: var(--color--primary);
        font-weight: 600;
        flex-shrink: 0;
      }

      .log-message {
        flex: 1;
        color: var(--color--text);
      }

      .log-meta {
        color: var(--color--text--tint-2);
        font-size: 11px;
        flex-shrink: 0;
      }

      // 級別特定樣式
      &.level-debug .log-icon {
        color: var(--color--text--tint-1);
      }

      &.level-info .log-icon {
        color: var(--color--primary);
      }

      &.level-warn .log-icon {
        color: var(--color--warning);
      }

      &.level-error {
        background: var(--color--danger--tint-4);

        .log-icon {
          color: var(--color--danger);
        }

        .log-message {
          color: var(--color--danger);
          font-weight: 600;
        }
      }
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 40px 20px;
      color: var(--color--text--tint-2);

      p {
        margin-top: 12px;
        font-size: 13px;
      }
    }
  }
}
</style>
```

---

## 5. 錯誤處理機制

### 5.1 錯誤顯示組件

```vue
<!-- NodeErrorDisplay.vue -->
<template>
  <div class="node-error-display">
    <!-- 錯誤圖標標記 -->
    <div class="error-badge" @click="showDetails = !showDetails">
      <IconAlertCircle :size="16" />
      <span class="error-count" v-if="errorCount > 1">
        {{ errorCount }}
      </span>
    </div>

    <!-- 錯誤詳情彈出框 -->
    <Teleport to="body">
      <div
        v-if="showDetails"
        class="error-popover"
        :style="popoverPosition"
        @click.stop
      >
        <div class="popover-header">
          <IconXCircle :size="18} />
          <span>執行錯誤</span>
          <button class="btn-close" @click="showDetails = false">
            <IconX :size="14} />
          </button>
        </div>

        <div class="popover-content">
          <!-- 錯誤消息 -->
          <div class="error-section">
            <div class="section-label">錯誤消息:</div>
            <div class="error-message">{{ error.message }}</div>
          </div>

          <!-- 錯誤描述 -->
          <div v-if="error.description" class="error-section">
            <div class="section-label">詳細描述:</div>
            <div class="error-description">{{ error.description }}</div>
          </div>

          <!-- HTTP 狀態碼 -->
          <div v-if="error.httpCode" class="error-section">
            <div class="section-label">HTTP 狀態碼:</div>
            <div class="error-code">{{ error.httpCode }}</div>
          </div>

          <!-- 堆棧跟蹤 -->
          <div v-if="error.stack && showStack" class="error-section">
            <div class="section-label">堆棧跟蹤:</div>
            <pre class="error-stack">{{ error.stack }}</pre>
          </div>

          <!-- 切換堆棧顯示 -->
          <button
            v-if="error.stack"
            class="btn-toggle-stack"
            @click="showStack = !showStack"
          >
            {{ showStack ? '隱藏' : '顯示' }}堆棧跟蹤
          </button>
        </div>

        <div class="popover-actions">
          <button class="btn-action btn-retry" @click="handleRetry">
            <IconRotateCcw :size="14} />
            重試
          </button>
          <button class="btn-action btn-copy" @click="copyError">
            <IconCopy :size="14} />
            複製錯誤
          </button>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import {
  IconAlertCircle,
  IconXCircle,
  IconX,
  IconRotateCcw,
  IconCopy,
} from 'lucide-vue-next';
import type { ExecutionError } from '@/types/execution-state.types';

interface Props {
  error: ExecutionError;
  errorCount?: number;
  nodeId: string;
}

const props = withDefaults(defineProps<Props>(), {
  errorCount: 1,
});

const emit = defineEmits<{
  'retry': [nodeId: string];
}>();

const showDetails = ref(false);
const showStack = ref(false);
const popoverPosition = computed(() => ({
  // 計算彈出框位置
}));

function handleRetry() {
  showDetails.value = false;
  emit('retry', props.nodeId);
}

async function copyError() {
  const errorText = JSON.stringify(props.error, null, 2);
  await navigator.clipboard.writeText(errorText);
  // 顯示複製成功提示
}
</script>

<style scoped lang="scss">
.node-error-display {
  position: absolute;
  bottom: 8px;
  right: 8px;
  z-index: 10;

  .error-badge {
    display: flex;
    align-items: center;
    gap: 4px;
    padding: 6px 8px;
    background: var(--color--danger);
    color: white;
    border-radius: 12px;
    cursor: pointer;
    box-shadow: var(--shadow--md);
    transition: all 0.2s ease;

    &:hover {
      background: var(--color--danger--shade-1);
      transform: scale(1.05);
    }

    .error-count {
      font-size: 11px;
      font-weight: 700;
      min-width: 16px;
      text-align: center;
    }
  }
}

.error-popover {
  position: fixed;
  z-index: 2000;
  background: var(--color--background);
  border: 2px solid var(--color--danger);
  border-radius: var(--radius--lg);
  box-shadow: var(--shadow--xl);
  min-width: 400px;
  max-width: 600px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;

  .popover-header {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 12px 16px;
    background: var(--color--danger--tint-4);
    color: var(--color--danger);
    font-weight: 600;
    border-bottom: 1px solid var(--color--danger--tint-3);

    .btn-close {
      margin-left: auto;
      padding: 4px;
      background: none;
      border: none;
      cursor: pointer;
      color: var(--color--danger);

      &:hover {
        color: var(--color--danger--shade-1);
      }
    }
  }

  .popover-content {
    flex: 1;
    overflow-y: auto;
    padding: 16px;

    .error-section {
      margin-bottom: 16px;

      .section-label {
        font-size: 12px;
        font-weight: 600;
        color: var(--color--text--tint-1);
        margin-bottom: 6px;
        text-transform: uppercase;
      }

      .error-message {
        font-size: 14px;
        color: var(--color--danger);
        font-weight: 600;
        line-height: 1.5;
      }

      .error-description,
      .error-code {
        font-size: 13px;
        color: var(--color--text);
        line-height: 1.5;
      }

      .error-stack {
        font-size: 11px;
        font-family: 'Monaco', 'Menlo', monospace;
        background: var(--color--background--shade-2);
        padding: 12px;
        border-radius: var(--radius);
        overflow-x: auto;
        margin: 0;
        white-space: pre-wrap;
        word-break: break-all;
      }
    }

    .btn-toggle-stack {
      padding: 6px 12px;
      font-size: 12px;
      background: var(--color--background--shade-1);
      border: 1px solid var(--color--foreground--tint-1);
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--color--background--shade-2);
      }
    }
  }

  .popover-actions {
    display: flex;
    gap: 8px;
    padding: 12px 16px;
    border-top: 1px solid var(--color--foreground--tint-2);

    .btn-action {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
      padding: 8px 16px;
      font-size: 13px;
      font-weight: 600;
      border: none;
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;

      &.btn-retry {
        background: var(--color--primary);
        color: white;

        &:hover {
          background: var(--color--primary--shade-1);
        }
      }

      &.btn-copy {
        background: var(--color--background--shade-1);
        color: var(--color--text);
        border: 1px solid var(--color--foreground--tint-1);

        &:hover {
          background: var(--color--background--shade-2);
        }
      }
    }
  }
}
</style>
```

### 5.2 錯誤恢復機制

```typescript
// error-recovery.ts

/**
 * 錯誤恢復策略
 */
export interface ErrorRecoveryStrategy {
  type: 'retry' | 'skip' | 'fallback' | 'manual';
  maxRetries?: number;
  retryDelay?: number;
  fallbackNodeId?: string;
}

/**
 * 執行錯誤恢復
 */
export async function executeErrorRecovery(
  nodeId: string,
  error: ExecutionError,
  strategy: ErrorRecoveryStrategy
): Promise<RecoveryResult> {
  switch (strategy.type) {
    case 'retry':
      return await retryExecution(nodeId, strategy.maxRetries, strategy.retryDelay);

    case 'skip':
      return skipNode(nodeId);

    case 'fallback':
      return executeFallbackNode(strategy.fallbackNodeId);

    case 'manual':
      return awaitManualResolution(nodeId, error);
  }
}

/**
 * 重試執行
 */
async function retryExecution(
  nodeId: string,
  maxRetries: number = 3,
  delay: number = 1000
): Promise<RecoveryResult> {
  let attempts = 0;

  while (attempts < maxRetries) {
    try {
      await new Promise(resolve => setTimeout(resolve, delay * attempts));
      const result = await executeNode(nodeId);

      return {
        success: true,
        result,
        attempts: attempts + 1,
      };
    } catch (error) {
      attempts++;
      if (attempts >= maxRetries) {
        return {
          success: false,
          error,
          attempts,
        };
      }
    }
  }
}
```

---

## 6. Pinned 數據功能

### 6.1 Pinned 數據面板

```vue
<!-- PinnedDataPanel.vue -->
<template>
  <div class="pinned-data-panel">
    <div class="panel-header">
      <div class="header-left">
        <IconPin :size="16} />
        <span class="panel-title">固定測試數據</span>
      </div>
      <div class="header-actions">
        <button
          class="btn-add"
          @click="addPinnedData"
          title="添加固定數據"
        >
          <IconPlus :size="14} />
          添加
        </button>
      </div>
    </div>

    <div class="panel-content">
      <!-- JSON 編輯器 -->
      <div class="json-editor-wrapper">
        <div class="editor-toolbar">
          <span class="data-count">
            {{ itemCount }} {{ itemCount === 1 ? 'item' : 'items' }}
          </span>
          <button class="btn-format" @click="formatJson">
            <IconCode :size="14} />
            格式化
          </button>
        </div>
        <codemirror
          v-model="jsonContent"
          :options="editorOptions"
          @change="handleJsonChange"
        />
      </div>

      <!-- 數據項列表預覽 -->
      <div v-if="parsedItems.length > 0" class="items-preview">
        <div class="preview-header">
          <span>數據項預覽</span>
        </div>
        <div
          v-for="(item, index) in parsedItems"
          :key="index"
          class="item-card"
        >
          <div class="item-header">
            <span class="item-index">Item {{ index }}</span>
            <button
              class="btn-remove"
              @click="removeItem(index)"
              title="移除此項"
            >
              <IconX :size="12} />
            </button>
          </div>
          <div class="item-content">
            <div
              v-for="(value, key) in item"
              :key="key"
              class="item-field"
            >
              <span class="field-key">{{ key }}:</span>
              <span class="field-value">{{ formatValue(value) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="panel-footer">
      <button class="btn-clear" @click="clearPinnedData">
        清除固定數據
      </button>
      <button class="btn-apply" @click="applyPinnedData">
        應用
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { IconPin, IconPlus, IconCode, IconX } from 'lucide-vue-next';

interface Props {
  nodeId: string;
  initialData?: any[];
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update': [data: any[]];
  'clear': [];
}>();

const jsonContent = ref(
  JSON.stringify(props.initialData || [], null, 2)
);

const parsedItems = computed(() => {
  try {
    return JSON.parse(jsonContent.value);
  } catch {
    return [];
  }
});

const itemCount = computed(() => parsedItems.value.length);

const editorOptions = {
  mode: 'application/json',
  theme: 'default',
  lineNumbers: true,
  lineWrapping: true,
  tabSize: 2,
};

function handleJsonChange() {
  // 實時驗證 JSON
}

function formatJson() {
  try {
    const data = JSON.parse(jsonContent.value);
    jsonContent.value = JSON.stringify(data, null, 2);
  } catch (error) {
    // 顯示錯誤提示
  }
}

function addPinnedData() {
  const newItem = { field: 'value' };
  const data = parsedItems.value;
  data.push(newItem);
  jsonContent.value = JSON.stringify(data, null, 2);
}

function removeItem(index: number) {
  const data = parsedItems.value;
  data.splice(index, 1);
  jsonContent.value = JSON.stringify(data, null, 2);
}

function clearPinnedData() {
  jsonContent.value = '[]';
  emit('clear');
}

function applyPinnedData() {
  try {
    const data = JSON.parse(jsonContent.value);
    emit('update', data);
  } catch (error) {
    // 顯示錯誤提示
  }
}

function formatValue(value: any): string {
  if (typeof value === 'string') return `"${value}"`;
  if (typeof value === 'object') return JSON.stringify(value);
  return String(value);
}
</script>

<style scoped lang="scss">
.pinned-data-panel {
  display: flex;
  flex-direction: column;
  height: 100%;
  background: var(--color--background);

  .panel-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    border-bottom: 1px solid var(--color--foreground--tint-2);
    background: var(--color--secondary--tint-3);

    .header-left {
      display: flex;
      align-items: center;
      gap: 8px;
      color: var(--color--secondary);

      .panel-title {
        font-size: 14px;
        font-weight: 600;
      }
    }

    .btn-add {
      display: flex;
      align-items: center;
      gap: 4px;
      padding: 6px 12px;
      font-size: 13px;
      font-weight: 600;
      background: var(--color--secondary);
      color: white;
      border: none;
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--color--secondary--shade-1);
      }
    }
  }

  .panel-content {
    flex: 1;
    overflow-y: auto;
    display: flex;
    flex-direction: column;
    gap: 16px;
    padding: 16px;

    .json-editor-wrapper {
      border: 1px solid var(--color--foreground--tint-1);
      border-radius: var(--radius);
      overflow: hidden;

      .editor-toolbar {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 8px 12px;
        background: var(--color--background--shade-1);
        border-bottom: 1px solid var(--color--foreground--tint-2);

        .data-count {
          font-size: 12px;
          color: var(--color--text--tint-1);
          font-weight: 600;
        }

        .btn-format {
          display: flex;
          align-items: center;
          gap: 4px;
          padding: 4px 8px;
          font-size: 12px;
          background: none;
          border: 1px solid var(--color--foreground--tint-1);
          border-radius: var(--radius);
          cursor: pointer;
          transition: all 0.2s ease;

          &:hover {
            background: var(--color--background--shade-2);
          }
        }
      }
    }

    .items-preview {
      display: flex;
      flex-direction: column;
      gap: 12px;

      .preview-header {
        font-size: 13px;
        font-weight: 600;
        color: var(--color--text--tint-1);
        text-transform: uppercase;
      }

      .item-card {
        border: 1px solid var(--color--foreground--tint-1);
        border-radius: var(--radius);
        overflow: hidden;

        .item-header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          padding: 8px 12px;
          background: var(--color--background--shade-1);
          border-bottom: 1px solid var(--color--foreground--tint-2);

          .item-index {
            font-size: 12px;
            font-weight: 600;
            color: var(--color--text);
          }

          .btn-remove {
            padding: 4px;
            background: none;
            border: none;
            color: var(--color--danger);
            cursor: pointer;
            border-radius: var(--radius);
            transition: all 0.2s ease;

            &:hover {
              background: var(--color--danger--tint-3);
            }
          }
        }

        .item-content {
          padding: 12px;
          display: flex;
          flex-direction: column;
          gap: 6px;

          .item-field {
            display: flex;
            gap: 8px;
            font-size: 12px;
            font-family: 'Monaco', 'Menlo', monospace;

            .field-key {
              color: var(--color--primary);
              font-weight: 600;
            }

            .field-value {
              color: var(--color--text);
            }
          }
        }
      }
    }
  }

  .panel-footer {
    display: flex;
    gap: 8px;
    padding: 12px 16px;
    border-top: 1px solid var(--color--foreground--tint-2);

    button {
      flex: 1;
      padding: 8px 16px;
      font-size: 13px;
      font-weight: 600;
      border: none;
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .btn-clear {
      background: var(--color--background--shade-1);
      color: var(--color--text);
      border: 1px solid var(--color--foreground--tint-1);

      &:hover {
        background: var(--color--background--shade-2);
      }
    }

    .btn-apply {
      background: var(--color--secondary);
      color: white;

      &:hover {
        background: var(--color--secondary--shade-1);
      }
    }
  }
}
</style>
```

### 6.2 Pinned 數據視覺標識

```scss
// 帶 Pinned 數據的節點樣式
.node {
  &.has-pinned-data {
    border-color: var(--color--secondary);
    position: relative;

    // Pinned 標記
    &::before {
      content: '';
      position: absolute;
      top: -6px;
      right: -6px;
      width: 20px;
      height: 20px;
      background: var(--color--secondary);
      border-radius: 50%;
      border: 2px solid var(--color--background);
      box-shadow: var(--shadow--sm);
    }

    &::after {
      content: '📌';
      position: absolute;
      top: -6px;
      right: -6px;
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
    }
  }
}
```

---

## 7. 執行控制面板

```vue
<!-- ExecutionControlPanel.vue -->
<template>
  <div class="execution-control-panel">
    <!-- 主控制按鈕 -->
    <div class="main-controls">
      <button
        v-if="!isExecuting"
        class="btn-control btn-execute"
        @click="handleExecute"
        :disabled="!canExecute"
      >
        <IconPlay :size="18} />
        <span>執行工作流</span>
      </button>

      <button
        v-else
        class="btn-control btn-stop"
        @click="handleStop"
      >
        <IconSquare :size="18} />
        <span>停止執行</span>
      </button>

      <!-- 執行選項下拉 -->
      <div class="execution-options">
        <button class="btn-options" @click="showOptions = !showOptions">
          <IconChevronDown :size="14} />
        </button>

        <div v-if="showOptions" class="options-menu">
          <div class="menu-item" @click="executeFrom">
            <IconCornerDownRight :size="14} />
            從此節點執行
          </div>
          <div class="menu-item" @click="executeSelected">
            <IconCheckSquare :size="14} />
            執行選中節點
          </div>
          <div class="menu-separator"></div>
          <div class="menu-item" @click="clearExecutionData">
            <IconTrash2 :size="14} />
            清除執行數據
          </div>
        </div>
      </div>
    </div>

    <!-- 執行狀態顯示 -->
    <div v-if="executionState" class="execution-status">
      <div class="status-info">
        <component
          :is="statusIcon"
          :size="16"
          :class="{ spinning: isExecuting }"
        />
        <span class="status-text">{{ statusText }}</span>
      </div>

      <div class="execution-stats">
        <span class="stat">
          {{ executedNodes }}/{{ totalNodes }} 節點
        </span>
        <span class="stat-separator">•</span>
        <span class="stat">
          {{ formatDuration(executionDuration) }}
        </span>
      </div>
    </div>

    <!-- 進度條 -->
    <div v-if="isExecuting" class="execution-progress">
      <div
        class="progress-bar"
        :style="{ width: `${executionProgress}%` }"
      ></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import {
  IconPlay,
  IconSquare,
  IconChevronDown,
  IconCornerDownRight,
  IconCheckSquare,
  IconTrash2,
  IconLoader,
  IconCheck,
  IconX,
} from 'lucide-vue-next';

const isExecuting = computed(() =>
  executionState.value?.status === 'running'
);

const executionProgress = computed(() =>
  totalNodes.value > 0
    ? (executedNodes.value / totalNodes.value) * 100
    : 0
);

const statusIcon = computed(() => {
  if (isExecuting.value) return IconLoader;
  if (executionState.value?.status === 'success') return IconCheck;
  if (executionState.value?.status === 'error') return IconX;
  return IconPlay;
});

const statusText = computed(() => {
  if (isExecuting.value) return '執行中...';
  if (executionState.value?.status === 'success') return '執行成功';
  if (executionState.value?.status === 'error') return '執行失敗';
  return '就緒';
});
</script>

<style scoped lang="scss">
.execution-control-panel {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 16px;
  background: var(--color--background);
  border-bottom: 1px solid var(--color--foreground--tint-2);

  .main-controls {
    display: flex;
    gap: 8px;

    .btn-control {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 12px 24px;
      font-size: 15px;
      font-weight: 600;
      border: none;
      border-radius: var(--radius);
      cursor: pointer;
      transition: all 0.2s ease;

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      &.btn-execute {
        background: var(--color--success);
        color: white;

        &:hover:not(:disabled) {
          background: var(--color--success--shade-1);
          transform: scale(1.02);
        }
      }

      &.btn-stop {
        background: var(--color--danger);
        color: white;

        &:hover {
          background: var(--color--danger--shade-1);
        }
      }
    }

    .execution-options {
      position: relative;

      .btn-options {
        padding: 12px;
        background: var(--color--background--shade-1);
        border: 1px solid var(--color--foreground--tint-1);
        border-radius: var(--radius);
        cursor: pointer;
        transition: all 0.2s ease;

        &:hover {
          background: var(--color--background--shade-2);
        }
      }

      .options-menu {
        position: absolute;
        top: 100%;
        right: 0;
        margin-top: 4px;
        background: var(--color--background);
        border: 1px solid var(--color--foreground--tint-1);
        border-radius: var(--radius);
        box-shadow: var(--shadow--lg);
        min-width: 200px;
        z-index: 100;

        .menu-item {
          display: flex;
          align-items: center;
          gap: 8px;
          padding: 10px 12px;
          font-size: 13px;
          cursor: pointer;
          transition: background 0.2s ease;

          &:hover {
            background: var(--color--background--shade-1);
          }
        }

        .menu-separator {
          height: 1px;
          background: var(--color--foreground--tint-2);
          margin: 4px 0;
        }
      }
    }
  }

  .execution-status {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 8px 12px;
    background: var(--color--background--shade-1);
    border-radius: var(--radius);

    .status-info {
      display: flex;
      align-items: center;
      gap: 8px;

      .spinning {
        animation: spin 1s linear infinite;
      }

      .status-text {
        font-size: 13px;
        font-weight: 600;
        color: var(--color--text);
      }
    }

    .execution-stats {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 12px;
      color: var(--color--text--tint-1);

      .stat-separator {
        opacity: 0.5;
      }
    }
  }

  .execution-progress {
    height: 4px;
    background: var(--color--background--shade-2);
    border-radius: 2px;
    overflow: hidden;

    .progress-bar {
      height: 100%;
      background: linear-gradient(
        90deg,
        var(--color--primary),
        var(--color--primary--tint-1)
      );
      transition: width 0.3s ease;
      animation: progress-shimmer 2s ease-in-out infinite;
    }
  }
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes progress-shimmer {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}
</style>
```

---

## 8. 完整執行流程

### 8.1 執行狀態機

```typescript
// execution-state-machine.ts

/**
 * 執行狀態機
 */
export class ExecutionStateMachine {
  private currentState: WorkflowExecutionState;
  private listeners: Map<string, Function[]> = new Map();

  /**
   * 開始執行
   */
  async start(workflowId: string, mode: 'manual' | 'trigger'): Promise<void> {
    // 1. 初始化執行狀態
    this.currentState = {
      id: generateExecutionId(),
      workflowId,
      status: 'running',
      mode,
      startedAt: Date.now(),
      nodeExecutions: {},
    };

    this.emit('execution:start', this.currentState);

    // 2. 獲取執行順序
    const executionOrder = this.getExecutionOrder();

    // 3. 依次執行節點
    for (const nodeId of executionOrder) {
      await this.executeNode(nodeId);

      // 檢查是否需要停止
      if (this.currentState.status === 'canceled') {
        break;
      }
    }

    // 4. 完成執行
    this.finish();
  }

  /**
   * 執行單個節點
   */
  private async executeNode(nodeId: string): Promise<void> {
    const node = this.getNode(nodeId);

    // 1. 更新節點狀態為執行中
    this.updateNodeStatus(nodeId, {
      status: NodeExecutionStatus.RUNNING,
      startedAt: Date.now(),
    });

    try {
      // 2. 獲取輸入數據
      const inputData = await this.getNodeInputData(nodeId);

      // 3. 執行節點邏輯
      const outputData = await node.execute(inputData);

      // 4. 更新節點狀態為成功
      this.updateNodeStatus(nodeId, {
        status: NodeExecutionStatus.SUCCESS,
        finishedAt: Date.now(),
        outputData,
        metadata: {
          itemsProcessed: outputData.length,
          iterations: 1,
        },
      });
    } catch (error) {
      // 5. 處理錯誤
      this.updateNodeStatus(nodeId, {
        status: NodeExecutionStatus.ERROR,
        finishedAt: Date.now(),
        error: {
          message: error.message,
          stack: error.stack,
        },
      });

      // 根據錯誤策略決定是否繼續
      if (!node.settings.continueOnFail) {
        this.currentState.status = 'error';
        throw error;
      }
    }
  }

  /**
   * 更新節點執行狀態
   */
  private updateNodeStatus(
    nodeId: string,
    update: Partial<NodeExecutionData>
  ): void {
    const current = this.currentState.nodeExecutions[nodeId] || {};
    this.currentState.nodeExecutions[nodeId] = {
      ...current,
      ...update,
      duration: update.finishedAt
        ? update.finishedAt - (current.startedAt || Date.now())
        : undefined,
    };

    this.emit('node:status-update', {
      nodeId,
      status: this.currentState.nodeExecutions[nodeId],
    });
  }

  /**
   * 完成執行
   */
  private finish(): void {
    this.currentState.finishedAt = Date.now();
    this.currentState.duration =
      this.currentState.finishedAt - this.currentState.startedAt;

    // 判斷整體狀態
    const hasErrors = Object.values(this.currentState.nodeExecutions).some(
      exec => exec.status === NodeExecutionStatus.ERROR
    );

    if (hasErrors) {
      this.currentState.status = 'error';
    } else if (this.currentState.status !== 'canceled') {
      this.currentState.status = 'success';
    }

    this.emit('execution:finish', this.currentState);
  }

  /**
   * 事件監聽
   */
  on(event: string, callback: Function): void {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, []);
    }
    this.listeners.get(event)!.push(callback);
  }

  /**
   * 發送事件
   */
  private emit(event: string, data: any): void {
    const callbacks = this.listeners.get(event) || [];
    callbacks.forEach(cb => cb(data));
  }
}
```

---

## 9. 總結

Part 3 建立了完整的執行視覺化系統：

### ✅ 已實現

1. **執行狀態系統** - 7 種狀態，清晰的顏色映射
2. **節點執行動畫** - 脈衝、閃爍、抖動效果
3. **連接線可視化** - 數據計數、流動動畫
4. **執行日誌系統** - 完整的日誌收集和顯示
5. **錯誤處理機制** - 錯誤顯示、重試、恢復
6. **Pinned 數據功能** - 測試數據固定和管理
7. **執行控制面板** - 執行、停止、進度顯示

### 📋 下一步 (Part 4)

在 Part 4 中，我們將設計：

1. **28 種節點類型** - 每種節點的詳細設計
2. **節點分類系統** - 合理的分類和組織
3. **節點圖標設計** - 統一的視覺風格
4. **節點參數配置** - 每種節點的專屬參數

---

**文檔版本**: 2.0
**創建日期**: 2025-10-29
**最後更新**: 2025-10-29
**下一部分**: Part 4 - 節點類型庫設計
