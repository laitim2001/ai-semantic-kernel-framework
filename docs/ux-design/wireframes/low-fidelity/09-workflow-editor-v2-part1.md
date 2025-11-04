# Workflow Editor 設計文檔 V2 - Part 1

**核心設計理念與大卡片節點系統 (基於 n8n 風格)**

> 版本: 2.0
> 日期: 2025-10-29
> 狀態: 設計中
> 基於: n8n 開源項目架構分析

---

## 目錄

1. [核心設計理念](#1-核心設計理念)
2. [網格系統](#2-網格系統)
3. [節點大小體系](#3-節點大小體系)
4. [節點形態設計](#4-節點形態設計)
5. [視覺設計系統](#5-視覺設計系統)
6. [技術架構基礎](#6-技術架構基礎)
7. [組件結構](#7-組件結構)

---

## 1. 核心設計理念

### 1.1 設計原則

基於對 n8n 源代碼的深入分析，我們採用以下核心設計原則：

```yaml
視覺清晰:
  - 使用大卡片式節點 (96x96px 起)
  - 清晰的圖標和標籤
  - 充足的視覺呼吸空間
  - 狀態一目了然

交互直觀:
  - 拖拽式連接
  - 網格自動對齊
  - 即時視覺反饋
  - 內聯配置編輯

性能優先:
  - 基於 VueFlow 的虛擬化渲染
  - 增量更新和緩存
  - 懶加載和按需渲染
  - 流暢的 60fps 動畫

可擴展性:
  - 模組化的節點系統
  - 插件式的節點類型
  - 可定制的主題
  - 靈活的佈局算法
```

### 1.2 與 n8n 的對比

| 設計維度 | n8n | 我們的設計 | 改進方向 |
|---------|-----|-----------|---------|
| **節點基礎大小** | 96x96px | 96x96px | ✅ 保持（經驗證的最佳實踐） |
| **網格系統** | 16px | 16px | ✅ 保持（統一對齊） |
| **內聯配置** | 🧪 實驗性 | ✅ 核心功能 | 🔧 從實驗變為核心，更成熟實現 |
| **節點寬度（配置狀態）** | 256px (16 grid) | 320px (20 grid) | 🔧 增加寬度以容納更多配置項 |
| **節點類型** | 4 種基礎類型 | 28 種專業節點 | 🆕 豐富的節點庫 |
| **畫布引擎** | VueFlow | VueFlow | ✅ 保持（成熟可靠） |
| **協作功能** | ❌ 無 | ✅ 實時協作 | 🆕 多人同時編輯 |

### 1.3 設計目標

**核心目標**：
1. ✅ **易用性優先** - 非技術用戶也能快速上手
2. 🎨 **視覺愉悅** - 美觀且專業的界面設計
3. ⚡ **高性能** - 支持 100+ 節點的大型工作流
4. 🔧 **高度可配置** - 節點內部直接配置，減少跳轉
5. 👥 **實時協作** - 多人同時編輯工作流

---

## 2. 網格系統

### 2.1 基礎網格單位

```typescript
// 基礎常量定義
export const GRID_SIZE = 16;  // 基礎網格單位（像素）

// 所有尺寸都是 GRID_SIZE 的倍數
export const GRID_MULTIPLIERS = {
  tiny: 1,      // 16px
  small: 2,     // 32px
  medium: 4,    // 64px
  large: 6,     // 96px
  xlarge: 10,   // 160px
  xxlarge: 16,  // 256px
  xxxlarge: 20, // 320px
};
```

**為什麼是 16px？**
- ✅ 符合設計系統常見標準（8px, 16px）
- ✅ 易於計算和記憶（2的冪）
- ✅ 與常見字體大小對齊良好
- ✅ n8n 驗證過的最佳實踐

### 2.2 網格對齊規則

```typescript
// 位置對齊到網格
export function snapToGrid(position: [number, number]): [number, number] {
  return [
    Math.round(position[0] / GRID_SIZE) * GRID_SIZE,
    Math.round(position[1] / GRID_SIZE) * GRID_SIZE
  ];
}

// 示例
snapToGrid([123, 456]) // → [128, 464]
snapToGrid([50, 75])   // → [48, 80]
```

**對齊規則**：
- 📌 **節點位置** - 左上角必須對齊到網格
- 📏 **節點大小** - 寬度和高度必須是 GRID_SIZE 的倍數
- 🔗 **連接點** - 輸入/輸出端口位置基於網格計算
- 📐 **間距** - 節點之間的最小間距為 2 * GRID_SIZE (32px)

### 2.3 畫布視覺輔助

```scss
// 網格背景樣式
.workflow-canvas {
  background-color: var(--color--background);
  background-image:
    // 主網格線（每 4 個單位）
    linear-gradient(var(--color--foreground--tint-2) 1px, transparent 1px),
    linear-gradient(90deg, var(--color--foreground--tint-2) 1px, transparent 1px),
    // 次網格線（每 1 個單位）
    linear-gradient(var(--color--foreground--tint-3) 1px, transparent 1px),
    linear-gradient(90deg, var(--color--foreground--tint-3) 1px, transparent 1px);
  background-size:
    64px 64px,  // 主網格
    64px 64px,
    16px 16px,  // 次網格
    16px 16px;
  background-position:
    -1px -1px,
    -1px -1px,
    -1px -1px,
    -1px -1px;
}
```

**網格顯示控制**：
- 🎯 **縮放 > 1.0** - 顯示主網格線和次網格線
- 🎯 **縮放 0.5-1.0** - 只顯示主網格線
- 🎯 **縮放 < 0.5** - 隱藏網格線（避免視覺噪音）

---

## 3. 節點大小體系

### 3.1 節點大小常量

```typescript
// 基於 n8n 的節點大小系統，並進行改進
export const NODE_SIZES = {
  // 標準節點（正方形）
  DEFAULT: {
    width: GRID_SIZE * 6,   // 96px
    height: GRID_SIZE * 6,  // 96px
  },

  // 配置節點（圓形）
  CONFIGURATION: {
    width: GRID_SIZE * 5,   // 80px
    height: GRID_SIZE * 5,  // 80px
    radius: GRID_SIZE * 2.5 // 40px (圓形)
  },

  // 可配置節點（橫長矩形）- 比 n8n 更寬
  CONFIGURABLE: {
    width: GRID_SIZE * 20,  // 320px (n8n 是 256px)
    height: GRID_SIZE * 6,  // 96px
  },

  // 便簽節點（可調整大小）
  STICKY_NOTE: {
    minWidth: GRID_SIZE * 10,   // 160px
    minHeight: GRID_SIZE * 8,   // 128px
    defaultWidth: GRID_SIZE * 15,  // 240px
    defaultHeight: GRID_SIZE * 12, // 192px
  }
} as const;
```

### 3.2 動態高度計算

節點的高度會根據輸入/輸出端口數量動態調整：

```typescript
/**
 * 計算節點的實際高度
 * 基於端口數量動態調整
 */
export function calculateNodeHeight(
  baseHeight: number,
  inputCount: number,
  outputCount: number
): number {
  // 取輸入和輸出端口的最大值
  const maxPorts = Math.max(inputCount, outputCount, 1);

  // 基礎高度可容納 2 個端口
  // 每增加 1 個端口，增加 2 個網格單位 (32px)
  const additionalHeight = Math.max(0, maxPorts - 2) * GRID_SIZE * 2;

  return baseHeight + additionalHeight;
}

// 示例
calculateNodeHeight(96, 1, 1) // → 96px  (最多2個端口，無需增加)
calculateNodeHeight(96, 3, 2) // → 128px (3個端口，增加32px)
calculateNodeHeight(96, 5, 4) // → 192px (5個端口，增加96px)
```

**高度調整規則**：
- 📏 **最小高度** = BASE_HEIGHT (96px)
- 📏 **每增加1個端口** = +32px (2 * GRID_SIZE)
- 📏 **最大高度** = BASE_HEIGHT + (MAX_PORTS - 2) * 32px
- 📏 **始終對齊網格** = 高度必須是 16px 的倍數

### 3.3 節點寬度變體

```typescript
/**
 * 節點寬度變體枚舉
 */
export enum NodeWidthVariant {
  COMPACT = 'compact',        // 96px - 標準執行節點
  CONFIGURATION = 'config',   // 80px - 圓形配置節點
  NORMAL = 'normal',          // 96px - 標準節點
  WIDE = 'wide',              // 320px - 可配置節點
  FULL = 'full',              // 根據內容自適應
}

/**
 * 根據節點類型和狀態計算寬度
 */
export function calculateNodeWidth(
  variant: NodeWidthVariant,
  isExpanded: boolean = false,
  portCount: number = 0
): number {
  switch (variant) {
    case NodeWidthVariant.COMPACT:
      return NODE_SIZES.DEFAULT.width;

    case NodeWidthVariant.CONFIGURATION:
      return NODE_SIZES.CONFIGURATION.width;

    case NodeWidthVariant.WIDE:
      // 可配置節點，展開時增加 50%
      return isExpanded
        ? NODE_SIZES.CONFIGURABLE.width * 1.5  // 480px
        : NODE_SIZES.CONFIGURABLE.width;        // 320px

    case NodeWidthVariant.FULL:
      // 根據端口數量計算
      const baseWidth = NODE_SIZES.CONFIGURATION.width;
      const additionalWidth = Math.max(0, portCount - 1) * GRID_SIZE * 3;
      return baseWidth + additionalWidth;

    default:
      return NODE_SIZES.DEFAULT.width;
  }
}
```

---

## 4. 節點形態設計

### 4.1 Standard Node (標準節點)

**用途**：常規執行節點（HTTP Request, Database Query, Function 等）

```
┌─────────────────┐
│                 │
│    ┌─────┐      │
│    │     │      │  96px
│    │ [i] │      │
│    │     │      │
│    └─────┘      │
│                 │
│   Node Name     │
└─────────────────┘
     96px
```

**視覺特徵**：
- 📐 **尺寸** - 96x96px（可根據端口動態增高）
- 🎨 **形狀** - 圓角矩形（border-radius: 8px）
- 🖼️ **圖標** - 40x40px 居中顯示
- 📝 **標籤** - 節點名稱在底部外側顯示
- 🔗 **端口** - 左側輸入，右側輸出

**CSS 示例**：
```scss
.node-standard {
  width: 96px;
  height: 96px;
  border-radius: var(--radius--lg);  // 8px
  border: 2px solid var(--color--foreground--shade-2);
  background: var(--color--background--light-3);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  transition: all 0.2s ease;

  &:hover {
    border-color: var(--color--primary);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  .node-icon {
    width: 40px;
    height: 40px;
    color: var(--color--foreground--shade-1);
  }

  .node-label {
    position: absolute;
    top: 100%;
    left: 50%;
    transform: translateX(-50%);
    margin-top: 8px;
    font-size: 16px;
    font-weight: 500;
    white-space: nowrap;
    text-align: center;
    max-width: 192px;  // 2倍節點寬度
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
```

### 4.2 Configuration Node (配置節點)

**用途**：設置型節點（API Credentials, Database Connection, Variables 等）

```
    ┌───────┐
   ╱         ╲
  │   ┌─┐   │
  │   │i│   │    80px
  │   └─┘   │
   ╲         ╱
    └───────┘
      80px
```

**視覺特徵**：
- 📐 **尺寸** - 80x80px（圓形）
- 🎨 **形狀** - 完美圓形（border-radius: 50%）
- 🖼️ **圖標** - 30x30px 居中顯示
- 📝 **標籤** - 節點名稱在底部外側顯示
- 🔗 **端口** - 通常無端口或少量端口

**CSS 示例**：
```scss
.node-configuration {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  border: 2px solid var(--color--foreground--shade-1);
  background: var(--node-type--supplemental--color--background);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;

  .node-icon {
    width: 30px;
    height: 30px;
    color: var(--color--foreground--shade-1);
  }
}
```

### 4.3 Configurable Node (可配置節點 - 橫長型)

**用途**：需要內聯配置的複雜節點（AI Agent, Code Interpreter, If/Switch 等）

**收起狀態 (Collapsed)**：
```
┌──────────────────────────────────────────┐
│  [i]  Node Name         Subtitle    [...] │  96px
└──────────────────────────────────────────┘
                320px
```

**展開狀態 (Expanded)**：
```
┌──────────────────────────────────────────────────────────────┐
│  [i]  Node Name                                      [−] [↗]  │
├──────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Parameter 1:  [Input Field                       ]   │    │
│  └──────────────────────────────────────────────────────┘    │
│                                                                │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Parameter 2:  [Dropdown ▼                        ]   │    │  最大 300px
│  └──────────────────────────────────────────────────────┘    │  或視口高度 80%
│                                                                │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Expression:   {{$json.field}}                        │    │
│  └──────────────────────────────────────────────────────┘    │
│                                                                │
└──────────────────────────────────────────────────────────────┘
                            480px (320px * 1.5)
```

**視覺特徵（收起狀態）**：
- 📐 **尺寸** - 320x96px（比 n8n 更寬）
- 🎨 **形狀** - 圓角矩形（border-radius: 8px）
- 🖼️ **圖標** - 18x18px 左側顯示
- 📝 **標籤** - 節點名稱和副標題水平排列
- 🔘 **操作按鈕** - 右側顯示展開按鈕

**視覺特徵（展開狀態）**：
- 📐 **尺寸** - 480x300px（或更高，取決於內容）
- 🎨 **形狀** - 圓角矩形，帶內部分隔線
- 📋 **配置區域** - 顯示完整的參數配置表單
- 🔘 **操作按鈕** - 收起按鈕、打開詳細視圖按鈕
- 📏 **最大高度** - 300px 或視口高度的 80%（選中時）
- 📜 **滾動** - 內容超出時可滾動

**組件結構**：
```vue
<template>
  <div
    :class="[
      'node-configurable',
      isExpanded ? 'expanded' : 'collapsed',
      isSelected ? 'selected' : ''
    ]"
    :style="dynamicStyles"
  >
    <!-- Header (始終顯示) -->
    <div class="node-header" @click="toggleExpand">
      <div class="node-header-left">
        <NodeIcon :icon="nodeIcon" :size="18" />
        <div class="node-info">
          <span class="node-name">{{ nodeName }}</span>
          <span v-if="!isExpanded" class="node-subtitle">{{ subtitle }}</span>
        </div>
      </div>
      <div class="node-header-actions">
        <button
          class="btn-icon"
          @click.stop="handleOpenFullView"
          title="打開完整編輯器"
        >
          <IconExpand :size="16" />
        </button>
        <button
          class="btn-icon"
          @click.stop="toggleExpand"
          :title="isExpanded ? '收起' : '展開'"
        >
          <IconChevron :size="16" :direction="isExpanded ? 'up' : 'down'" />
        </button>
      </div>
    </div>

    <!-- Configuration Panel (僅展開時顯示) -->
    <div v-if="isExpanded" class="node-config-panel">
      <div class="config-content">
        <!-- 動態渲染配置項 -->
        <component
          v-for="param in parameters"
          :key="param.name"
          :is="getParameterComponent(param.type)"
          :value="param.value"
          :config="param"
          @update="handleParamUpdate(param.name, $event)"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';

const isExpanded = ref(false);
const isSelected = ref(false);

const dynamicStyles = computed(() => ({
  width: isExpanded.value ? '480px' : '320px',
  maxHeight: isExpanded.value
    ? (isSelected.value ? 'calc(80vh)' : '300px')
    : '96px',
}));

function toggleExpand() {
  isExpanded.value = !isExpanded.value;
}

function handleOpenFullView() {
  // 打開側邊欄的完整編輯器
  emit('open-detail-view', nodeId);
}
</script>
```

**CSS 示例**：
```scss
.node-configurable {
  border-radius: var(--radius--lg);
  border: 2px solid var(--color--foreground--shade-2);
  background: var(--color--background--light-3);
  transition: all 0.3s ease;
  overflow: hidden;
  display: flex;
  flex-direction: column;

  &.collapsed {
    height: 96px;
    cursor: pointer;

    &:hover {
      border-color: var(--color--primary);
    }
  }

  &.expanded {
    min-height: 96px;
    max-height: 300px;
    cursor: default;

    &.selected {
      max-height: 80vh;
    }

    .node-config-panel {
      display: flex;
      flex-direction: column;
      flex: 1;
      overflow: hidden;
    }

    .config-content {
      flex: 1;
      overflow-y: auto;
      padding: 16px;
      gap: 12px;
      display: flex;
      flex-direction: column;
    }
  }

  .node-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    border-bottom: 1px solid var(--color--foreground--tint-2);
    background: var(--color--background--light-2);
    min-height: 48px;

    .node-header-left {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
      min-width: 0;  // 允許文本截斷

      .node-info {
        display: flex;
        flex-direction: column;
        min-width: 0;

        .node-name {
          font-size: 16px;
          font-weight: 600;
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
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

    .node-header-actions {
      display: flex;
      gap: 4px;
      flex-shrink: 0;
    }
  }
}
```

### 4.4 Trigger Node (觸發節點)

**用途**：工作流入口節點（Webhook, Schedule, Manual Trigger 等）

```
  ╭─────────────────╮
╭─┤                 ├─╮
│ │    ┌─────┐      │ │
│ │    │     │      │ │  96px
│ │    │ [!] │      │ │
│ │    │     │      │ │
│ │    └─────┘      │ │
╰─┤                 ├─╯
  ╰─────────────────╯
         96px
```

**視覺特徵**：
- 📐 **尺寸** - 96x96px（同標準節點）
- 🎨 **形狀** - 左側特殊圓角（標識觸發點）
- 🎨 **左側圓角** - border-top-left-radius: 36px, border-bottom-left-radius: 36px
- 🎨 **右側圓角** - border-top-right-radius: 8px, border-bottom-right-radius: 8px
- 🖼️ **圖標** - 40x40px 居中顯示，通常是觸發相關圖標
- 🔗 **端口** - 通常只有輸出端口（右側）

**CSS 示例**：
```scss
.node-trigger {
  width: 96px;
  height: 96px;
  // 特殊的不對稱圓角
  border-top-left-radius: 36px;
  border-bottom-left-radius: 36px;
  border-top-right-radius: var(--radius--lg);
  border-bottom-right-radius: var(--radius--lg);
  border: 2px solid var(--color--primary);
  background: linear-gradient(135deg,
    var(--color--primary--tint-3) 0%,
    var(--color--background--light-3) 100%
  );
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;

  // 觸發節點的特殊視覺標記
  &::before {
    content: '';
    position: absolute;
    left: -6px;
    top: 50%;
    transform: translateY(-50%);
    width: 12px;
    height: 12px;
    border-radius: 50%;
    background: var(--color--primary);
    border: 2px solid var(--color--background);
  }

  .node-icon {
    width: 40px;
    height: 40px;
    color: var(--color--primary);
  }
}
```

---

## 5. 視覺設計系統

### 5.1 顏色系統

基於 CSS 變量的主題系統，來自 n8n 和我們的設計擴展：

```scss
// 主色調
--color--primary: #FF6D5A;           // 主品牌色
--color--primary--shade-1: #E05A47;  // 深色變體
--color--primary--tint-1: #FF8B7A;   // 淺色變體
--color--primary--tint-2: #FFA99B;   // 更淺變體
--color--primary--tint-3: #FFC7BC;   // 最淺變體

// 狀態顏色
--color--success: #4CAF50;           // 成功 - 綠色
--color--success--shade-1: #388E3C;
--color--success--tint-1: #66BB6A;

--color--warning: #FFA726;           // 警告 - 橙色
--color--warning--shade-1: #F57C00;
--color--warning--tint-1: #FFB74D;

--color--danger: #F44336;            // 錯誤 - 紅色
--color--danger--shade-1: #D32F2F;
--color--danger--tint-1: #EF5350;

--color--secondary: #9C27B0;         // 次要色 - 紫色（Pinned）
--color--secondary--shade-1: #7B1FA2;
--color--secondary--tint-1: #AB47BC;

// 文本顏色
--color--text: #1A1A1A;              // 主文本
--color--text--shade-1: #000000;     // 深色文本
--color--text--tint-1: #666666;      // 次要文本
--color--text--tint-2: #999999;      // 輔助文本
--color--text--tint-3: #CCCCCC;      // 禁用文本

// 前景色（邊框、圖標）
--color--foreground: #E0E0E0;
--color--foreground--shade-1: #BDBDBD;
--color--foreground--shade-2: #9E9E9E;
--color--foreground--tint-1: #EEEEEE;
--color--foreground--tint-2: #F5F5F5;

// 背景色
--color--background: #FFFFFF;
--color--background--shade-1: #FAFAFA;
--color--background--shade-2: #F5F5F5;
--color--background--light-2: #F9F9F9;
--color--background--light-3: #FCFCFC;
```

### 5.2 節點狀態顏色映射

```typescript
export const NODE_STATE_COLORS = {
  // 執行狀態
  NEW: '--color--foreground--shade-2',      // 灰色 - 未執行
  RUNNING: '--color--primary',              // 主色 - 執行中（帶動畫）
  SUCCESS: '--color--success',              // 綠色 - 執行成功
  ERROR: '--color--danger',                 // 紅色 - 執行錯誤
  WAITING: '--color--secondary',            // 紫色 - 等待中

  // 特殊狀態
  PINNED: '--color--secondary',             // 紫色 - 固定數據
  DISABLED: '--color--foreground',          // 灰色 - 禁用
  SELECTED: '--color--primary',             // 主色 - 選中（外圍光暈）
  WARNING: '--color--warning',              // 橙色 - 警告
} as const;
```

**狀態優先級（從高到低）**：
```
waiting > running > disabled > pinned > error > warning > success > selected > new
```

### 5.3 陰影系統

```scss
// 陰影層級
--shadow--sm: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);
--shadow--md: 0 3px 6px rgba(0, 0, 0, 0.15), 0 2px 4px rgba(0, 0, 0, 0.12);
--shadow--lg: 0 10px 20px rgba(0, 0, 0, 0.15), 0 3px 6px rgba(0, 0, 0, 0.10);
--shadow--xl: 0 15px 25px rgba(0, 0, 0, 0.15), 0 5px 10px rgba(0, 0, 0, 0.05);

// 節點陰影使用
.node {
  box-shadow: var(--shadow--sm);

  &:hover {
    box-shadow: var(--shadow--md);
  }

  &.selected {
    box-shadow: var(--shadow--lg),
                0 0 0 8px var(--color--primary--tint-3);  // 外圍光暈
  }

  &.dragging {
    box-shadow: var(--shadow--xl);
  }
}
```

### 5.4 圖標系統

**圖標尺寸標準**：
```typescript
export const ICON_SIZES = {
  xs: 12,   // 極小圖標（狀態指示器）
  sm: 16,   // 小圖標（工具欄按鈕）
  md: 18,   // 中等圖標（可配置節點 header）
  lg: 24,   // 大圖標（側邊欄）
  xl: 30,   // 超大圖標（配置節點）
  xxl: 40,  // 特大圖標（標準節點）
} as const;
```

**圖標來源**：
- 🎨 **自定義圖標** - 節點類型專用（SVG）
- 📦 **Lucide Icons** - 通用圖標庫
- 🔧 **FontAwesome** - 輔助圖標（可選）

**圖標顏色規則**：
```scss
.node-icon {
  // 默認繼承節點狀態顏色
  color: currentColor;

  // 禁用狀態
  .node.disabled & {
    opacity: 0.5;
    filter: grayscale(100%);
  }

  // 執行中狀態
  .node.running & {
    animation: pulse 2s ease-in-out infinite;
  }

  // 錯誤狀態
  .node.error & {
    color: var(--color--danger);
  }
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}
```

---

## 6. 技術架構基礎

### 6.1 技術棧選擇

基於 n8n 的成功經驗，我們採用以下技術棧：

```yaml
前端框架:
  核心: Vue 3 (Composition API)
  理由:
    - 輕量高效
    - TypeScript 支持優秀
    - n8n 驗證過的可靠選擇

畫布引擎:
  核心: VueFlow (@vue-flow/core)
  理由:
    - 專為 Vue 3 設計
    - 內置虛擬化渲染
    - 豐富的功能和擴展性
    - n8n 使用並驗證

狀態管理:
  核心: Pinia
  理由:
    - Vue 3 官方推薦
    - TypeScript 原生支持
    - 簡潔的 API
    - 優秀的 DevTools

構建工具:
  核心: Vite
  理由:
    - 極快的開發服務器
    - 優化的生產構建
    - Vue 官方推薦
    - 豐富的插件生態

UI 組件庫:
  核心: 自研設計系統
  輔助: Headless UI (可選)
  理由:
    - 完全定制化
    - 精確控制樣式
    - 輕量無依賴

語言:
  核心: TypeScript
  理由:
    - 類型安全
    - 更好的開發體驗
    - 代碼可維護性
```

### 6.2 核心依賴項

```json
{
  "dependencies": {
    // 核心框架
    "vue": "^3.4.0",
    "@vue-flow/core": "^1.33.0",
    "@vue-flow/background": "^1.3.0",
    "@vue-flow/controls": "^1.1.0",
    "@vue-flow/minimap": "^1.4.0",

    // 狀態管理
    "pinia": "^2.1.0",

    // 工具庫
    "@vueuse/core": "^10.7.0",
    "lodash-es": "^4.17.21",

    // 圖標
    "lucide-vue-next": "^0.300.0",

    // 實時協作 (Part 6 詳述)
    "yjs": "^13.6.0",
    "y-websocket": "^1.5.0",

    // 表達式系統 (Part 2 詳述)
    "codemirror": "^6.0.0",
    "@codemirror/lang-javascript": "^6.0.0"
  },
  "devDependencies": {
    "typescript": "^5.3.0",
    "vite": "^5.0.0",
    "@vitejs/plugin-vue": "^5.0.0",
    "sass": "^1.69.0",
    "vitest": "^1.1.0",
    "@playwright/test": "^1.40.0"
  }
}
```

### 6.3 項目結構

```
src/
├── components/
│   ├── workflow-canvas/          # 畫布組件
│   │   ├── Canvas.vue            # 主畫布容器
│   │   ├── nodes/                # 節點組件
│   │   │   ├── NodeStandard.vue
│   │   │   ├── NodeConfiguration.vue
│   │   │   ├── NodeConfigurable.vue
│   │   │   └── NodeTrigger.vue
│   │   ├── edges/                # 連接線組件
│   │   │   ├── Edge.vue
│   │   │   └── EdgeLabel.vue
│   │   └── controls/             # 控制項
│   │       ├── Toolbar.vue
│   │       ├── Minimap.vue
│   │       └── ZoomControls.vue
│   └── node-config/              # 節點配置組件 (Part 2)
│       ├── ParameterInput.vue
│       ├── ExpressionEditor.vue
│       └── ...
├── composables/                  # 組合式函數
│   ├── useCanvas.ts              # 畫布狀態
│   ├── useCanvasNode.ts          # 節點狀態
│   ├── useCanvasMapping.ts       # 數據映射
│   └── useNodeExecution.ts       # 執行狀態
├── stores/                       # Pinia Stores
│   ├── workflow.ts               # 工作流數據
│   ├── nodes.ts                  # 節點數據
│   ├── execution.ts              # 執行狀態
│   └── ui.ts                     # UI 狀態
├── types/                        # TypeScript 類型
│   ├── canvas.types.ts
│   ├── node.types.ts
│   └── workflow.types.ts
├── utils/                        # 工具函數
│   ├── canvas.utils.ts
│   ├── node.utils.ts
│   └── grid.utils.ts
└── assets/                       # 靜態資源
    ├── styles/
    │   ├── variables.scss        # CSS 變量
    │   ├── mixins.scss
    │   └── global.scss
    └── icons/                    # 節點圖標
        ├── http-request.svg
        ├── database-query.svg
        └── ...
```

---

## 7. 組件結構

### 7.1 組件層級關係

```
WorkflowEditor.vue (頁面)
│
└─┬─ WorkflowCanvas.vue (畫布容器)
  │
  ├─── Canvas.vue (VueFlow 包裝器)
  │    │
  │    ├─── VueFlow (第三方庫)
  │    │    │
  │    │    ├─── CanvasNode.vue (節點包裝器)
  │    │    │    │
  │    │    │    ├─── NodeStandard.vue
  │    │    │    ├─── NodeConfiguration.vue
  │    │    │    ├─── NodeConfigurable.vue
  │    │    │    └─── NodeTrigger.vue
  │    │    │
  │    │    └─── CanvasEdge.vue (連接線)
  │    │         └─── EdgeLabel.vue
  │    │
  │    ├─── Background (網格背景)
  │    ├─── MiniMap (小地圖)
  │    └─── Controls (縮放控制)
  │
  ├─── CanvasToolbar.vue (頂部工具欄)
  │    ├─── ToolbarButton
  │    └─── ToolbarDropdown
  │
  └─── NodeDetailPanel.vue (右側詳細面板 - Part 2)
       └─── NodeConfigForm
```

### 7.2 核心組件接口

#### Canvas.vue
```typescript
// Canvas.vue - 主畫布組件
interface CanvasProps {
  workflowId: string;
  readonly?: boolean;
  showGrid?: boolean;
  showMinimap?: boolean;
  snapToGrid?: boolean;
}

interface CanvasEmits {
  'node:add': [nodeType: string, position: [number, number]];
  'node:delete': [nodeId: string];
  'node:select': [nodeId: string];
  'node:move': [nodeId: string, position: [number, number]];
  'connection:create': [connection: Connection];
  'connection:delete': [connectionId: string];
  'workflow:save': [];
  'workflow:execute': [];
}
```

#### CanvasNode.vue
```typescript
// CanvasNode.vue - 節點包裝器
interface CanvasNodeProps {
  id: string;
  type: NodeType;
  data: NodeData;
  selected?: boolean;
  draggable?: boolean;
}

interface CanvasNodeEmits {
  'node:activate': [nodeId: string];  // 雙擊打開詳細視圖
  'node:expand': [nodeId: string];    // 展開內聯配置
  'node:collapse': [nodeId: string];  // 收起內聯配置
  'context-menu': [event: MouseEvent];
}
```

#### NodeConfigurable.vue
```typescript
// NodeConfigurable.vue - 可配置節點
interface NodeConfigurableProps {
  nodeId: string;
  nodeName: string;
  nodeIcon: string;
  subtitle?: string;
  parameters: Parameter[];
  isExpanded?: boolean;
  isReadonly?: boolean;
}

interface NodeConfigurableEmits {
  'toggle-expand': [];
  'open-detail': [];
  'update-parameter': [paramName: string, value: any];
}
```

### 7.3 數據流架構

```mermaid
graph TB
    A[WorkflowStore] -->|workflow data| B[WorkflowCanvas]
    B -->|transform| C[useCanvasMapping]
    C -->|CanvasNode[]| D[Canvas.vue]
    C -->|CanvasEdge[]| D
    D -->|props| E[VueFlow]
    E -->|render| F[CanvasNode]
    E -->|render| G[CanvasEdge]
    F -->|events| H[useCanvasNode]
    H -->|actions| A
    G -->|events| I[useCanvasEdge]
    I -->|actions| A
```

**數據流說明**：
1. **WorkflowStore** - Pinia store，存儲工作流的原始數據
2. **useCanvasMapping** - 將 n8n 格式轉換為 VueFlow 格式
3. **Canvas.vue** - VueFlow 包裝器，傳遞數據給 VueFlow
4. **VueFlow** - 渲染節點和連接線
5. **useCanvasNode / useCanvasEdge** - 處理交互事件
6. **Actions** - 更新 WorkflowStore，觸發重新渲染

---

## 8. 示例：完整節點實現

### 8.1 HTTP Request 節點

```vue
<!-- NodeHttpRequest.vue -->
<template>
  <NodeConfigurable
    :node-id="id"
    :node-name="data.name"
    :node-icon="httpMethodIcon"
    :subtitle="requestUrl"
    :parameters="configParameters"
    :is-expanded="isExpanded"
    :is-readonly="readonly"
    @toggle-expand="handleToggleExpand"
    @open-detail="handleOpenDetail"
    @update-parameter="handleUpdateParameter"
  >
    <template #icon>
      <IconGlobe :size="18" :color="httpMethodColor" />
    </template>

    <template #config-content>
      <!-- HTTP Method -->
      <ParameterSelect
        label="HTTP Method"
        :value="data.parameters.method"
        :options="['GET', 'POST', 'PUT', 'PATCH', 'DELETE']"
        @update="updateParameter('method', $event)"
      />

      <!-- Request URL -->
      <ParameterInput
        label="URL"
        :value="data.parameters.url"
        placeholder="https://api.example.com/endpoint"
        :supports-expression="true"
        @update="updateParameter('url', $event)"
      />

      <!-- Headers -->
      <ParameterCollection
        label="Headers"
        :value="data.parameters.headers"
        :fields="['key', 'value']"
        @update="updateParameter('headers', $event)"
      />

      <!-- Body (for POST/PUT/PATCH) -->
      <ParameterCodeEditor
        v-if="['POST', 'PUT', 'PATCH'].includes(data.parameters.method)"
        label="Request Body"
        :value="data.parameters.body"
        language="json"
        :supports-expression="true"
        @update="updateParameter('body', $event)"
      />
    </template>
  </NodeConfigurable>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useCanvasNode } from '@/composables/useCanvasNode';
import NodeConfigurable from './NodeConfigurable.vue';
import { IconGlobe } from 'lucide-vue-next';

const { id, data, isExpanded, readonly } = useCanvasNode();

const httpMethodIcon = computed(() => {
  const method = data.value.parameters.method;
  return `http-${method.toLowerCase()}`;
});

const httpMethodColor = computed(() => {
  const colors = {
    GET: '#4CAF50',
    POST: '#2196F3',
    PUT: '#FF9800',
    PATCH: '#FFC107',
    DELETE: '#F44336'
  };
  return colors[data.value.parameters.method] || '#757575';
});

const requestUrl = computed(() => {
  const url = data.value.parameters.url;
  return url.length > 40 ? url.substring(0, 40) + '...' : url;
});

const configParameters = computed(() => [
  {
    name: 'method',
    type: 'select',
    label: 'HTTP Method',
    value: data.value.parameters.method,
    options: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE']
  },
  {
    name: 'url',
    type: 'string',
    label: 'URL',
    value: data.value.parameters.url,
    supportsExpression: true
  },
  // ... 更多參數
]);

function handleToggleExpand() {
  // 實現展開/收起邏輯
}

function handleOpenDetail() {
  // 打開側邊欄的完整編輯器
}

function handleUpdateParameter(paramName: string, value: any) {
  // 更新參數值
  emit('update-parameter', paramName, value);
}
</script>
```

---

## 9. 性能優化策略

### 9.1 虛擬化渲染

VueFlow 內置虛擬化，只渲染視口內的節點：

```typescript
// Canvas.vue
const vueFlowOptions = {
  // 啟用虛擬化
  nodesDraggable: true,
  nodesConnectable: true,
  elementsSelectable: true,

  // 性能優化
  minZoom: 0.1,
  maxZoom: 4,
  snapToGrid: true,
  snapGrid: [GRID_SIZE, GRID_SIZE],

  // 只渲染可見節點
  onlyRenderVisibleElements: true,
};
```

### 9.2 懶加載節點配置

```vue
<script setup lang="ts">
import { defineAsyncComponent } from 'vue';

// 懶加載節點配置組件
const NodeConfigPanel = defineAsyncComponent({
  loader: () => import('./NodeConfigPanel.vue'),
  loadingComponent: LoadingSpinner,
  delay: 200,
  timeout: 3000
});
</script>
```

### 9.3 Computed 緩存

```typescript
// 使用 computed 緩存計算結果
const mappedNodes = computed(() => {
  return nodes.value.map(node => ({
    ...node,
    // 計算結果會被緩存
    size: calculateNodeSize(node),
    connections: getNodeConnections(node),
  }));
});
```

### 9.4 節流和防抖

```typescript
import { throttledWatch } from '@vueuse/core';

// 節流更新執行數據（120ms）
throttledWatch(
  executionData,
  (newData) => {
    updateExecutionState(newData);
  },
  { throttle: 120 }
);
```

---

## 10. 無障礙性 (Accessibility)

### 10.1 鍵盤導航

```typescript
// 鍵盤快捷鍵
const keyboardShortcuts = {
  'Delete': '刪除選中節點',
  'Ctrl+C': '複製選中節點',
  'Ctrl+V': '粘貼節點',
  'Ctrl+Z': '撤銷',
  'Ctrl+Shift+Z': '重做',
  'Ctrl+A': '全選節點',
  'Space': '平移模式',
  'Ctrl+F': '搜索節點',
  'Escape': '取消選擇',
};
```

### 10.2 ARIA 屬性

```vue
<template>
  <div
    role="application"
    aria-label="工作流編輯器"
    :aria-describedby="descriptionId"
  >
    <div
      v-for="node in nodes"
      :key="node.id"
      role="button"
      :aria-label="`${node.type} 節點: ${node.name}`"
      :aria-selected="node.selected"
      :aria-disabled="node.disabled"
      tabindex="0"
      @keydown.enter="activateNode(node.id)"
      @keydown.space.prevent="selectNode(node.id)"
    >
      <!-- 節點內容 -->
    </div>
  </div>
</template>
```

---

## 11. 總結

Part 1 建立了 Workflow Editor 的核心設計基礎：

### ✅ 已定義

1. **網格系統** - 16px 基礎單位，統一對齊
2. **節點大小體系** - 96x96 標準，320x96 可配置
3. **四種節點形態** - Standard, Configuration, Configurable, Trigger
4. **視覺設計系統** - 顏色、陰影、圖標
5. **技術架構** - Vue 3 + VueFlow + Pinia
6. **組件結構** - 清晰的組件層級和數據流

### 📋 下一步 (Part 2)

在 Part 2 中，我們將深入設計：

1. **內聯配置系統** - 節點展開/收起的詳細交互
2. **表達式編輯器** - `{{$json.xxx}}` 語法實現
3. **參數類型系統** - String, Number, Boolean, Object, Array 等
4. **智能提示系統** - 自動完成和語法高亮
5. **表達式求值** - 實時預覽和錯誤檢測

---

**文檔版本**: 2.0
**創建日期**: 2025-10-29
**最後更新**: 2025-10-29
**下一部分**: Part 2 - 內聯配置與表達式系統
