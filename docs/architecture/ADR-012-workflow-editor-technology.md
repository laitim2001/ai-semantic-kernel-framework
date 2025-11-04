# ADR-012: 工作流編輯器技術選型 - VueFlow + Module Federation

**狀態**: ✅ 已接受
**日期**: 2025-11-01
**決策者**: 架構團隊
**影響範圍**: 前端架構、工作流模組、開發流程

---

## 背景與問題

### 業務需求
AI Workflow Platform 的核心功能是可視化工作流編輯器（US 4.1），需要支持:
- 28 種節點類型的拖放式編輯
- 節點內聯配置（inline configuration）
- 實時多用戶協作（CRDT）
- 高性能渲染（60 FPS，1000+ 節點）
- 響應式設計（Desktop/Tablet/Mobile）

### 技術挑戰
1. **既定技術棧**: 項目已確定使用 **React 18 + Material-UI** 作為主應用框架（95% 頁面）
2. **工作流引擎需求**: 需要成熟的 Canvas 畫布引擎，支持複雜節點操作
3. **參考實現**: n8n（行業領先的工作流平台）使用 **Vue 3 + VueFlow** 實現
4. **時間壓力**: 需要在 3 個月內完成 MVP

### 核心問題
**如何在 React 主應用中整合 Vue 工作流編輯器，同時保持架構清晰和可維護性？**

---

## 決策

### 採用技術方案
**微前端架構 (Microfrontend Architecture) + Module Federation**

### 技術棧分工
| 模組 | 技術棧 | 構建工具 | 端口 | 職責範圍 |
|------|--------|---------|------|---------|
| **主應用 (Host)** | React 18 + TypeScript + Material-UI + Redux Toolkit | Vite 5 | 3000 | 95% 功能頁面（Dashboard, Agent 管理, 設置等） |
| **工作流編輯器 (Remote)** | Vue 3 + TypeScript + VueFlow + Pinia | Webpack 5 | 3001 | 工作流畫布編輯器（獨立模組） |

### 整合機制
1. **Module Federation (Webpack 5+)**: 運行時動態加載遠程模組
2. **Props 傳遞**: React → Vue（單向數據流）
3. **自定義事件**: Vue → React（事件發射）
4. **Event Bus**: 跨框架狀態同步（Redux ↔ Pinia）
5. **共享設計令牌**: CSS Variables 保證視覺一致性

---

## 備選方案

### 方案 1: 純 React + React Flow ❌ 不採用
**優點**:
- 統一技術棧，降低維護成本
- 開發團隊熟悉 React 生態

**缺點**:
- ❌ **無成熟參考實現**: 需要從零設計 28 種節點類型
- ❌ **開發週期長**: 預估需要 **6 個月**（vs. 3 個月）
- ❌ **功能差距**: React Flow 缺少內聯配置、協作等高級功能
- ❌ **風險高**: 自研方案缺乏生產驗證

**評估結果**: 開發成本高，時間風險不可接受

### 方案 2: 全面遷移至 Vue 3 ❌ 不採用
**優點**:
- 統一技術棧為 Vue 3
- 充分利用 VueFlow 生態

**缺點**:
- ❌ **推翻既有工作**: 已完成的 React 架構和組件需重寫
- ❌ **團隊技能**: 團隊主要熟悉 React，Vue 學習曲線
- ❌ **Material-UI 替代**: 需要替換為 Vuetify/Element Plus
- ❌ **時間成本**: 全面重構需要 **4-5 個月**

**評估結果**: 成本過高，不符合業務時間線

### 方案 3: iframe 隔離 ❌ 不採用
**優點**:
- 完全隔離，避免框架衝突

**缺點**:
- ❌ **通信複雜**: postMessage API 性能差，調試困難
- ❌ **樣式隔離**: 無法共享設計系統
- ❌ **SEO 不友好**: 影響搜索引擎索引
- ❌ **用戶體驗差**: 無法實現無縫整合

**評估結果**: 技術債務高，用戶體驗差

---

## 決策依據

### 1. n8n 源代碼分析（PoC 階段）
**分析範圍**: `n8n-analysis/packages/editor-ui/src/components/canvas/`

**關鍵發現**:
```vue
<!-- n8n 核心架構 -->
Canvas.vue (1,800+ 行)
├── useCanvas.ts              # Canvas 邏輯（540 行）
├── useCanvasNode.ts          # 節點操作（420 行）
├── useCanvasMapping.ts       # 數據映射（380 行）
├── VueFlow 整合              # 核心畫布引擎
└── 28 種節點類型              # 生產級節點庫
```

**技術優勢**:
- ✅ **成熟的節點系統**: 28 種節點類型（Trigger, Action, AI, Transform 等）
- ✅ **內聯配置模式**: `ExperimentalEmbeddedNodeDetails.vue`（300+ 行）
- ✅ **網格系統**: 16px base unit（`GRID_SIZE = 16`）
- ✅ **Composables 架構**: 可復用的 Vue 3 Composition API
- ✅ **性能優化**: 虛擬滾動、懶加載、節點緩存

**參考價值**: 節省 **35% 開發時間**（預估從 6 個月縮短至 3 個月）

### 2. PoC 6 驗證結果（VueFlow + CRDT）
**驗證範圍**: `poc-projects/poc6-vueflow-crdt/`

**性能測試**:
| 指標 | 目標 | 實測結果 | 狀態 |
|------|------|---------|------|
| 渲染性能 | >60 FPS | 60 FPS | ✅ 達標 |
| 同步延遲 | <500ms | <200ms | ✅ 超標 |
| 並發用戶 | 3+ | 3+ | ✅ 達標 |
| 衝突解決 | 自動 | Yjs CRDT | ✅ 達標 |

**協作功能**:
- ✅ **實時同步**: Yjs + WebSocket (Socket.io)
- ✅ **衝突解決**: CRDT 自動合併
- ✅ **光標顯示**: 多用戶光標追蹤
- ✅ **變更歷史**: 操作記錄和撤銷

**結論**: VueFlow 滿足所有功能和性能要求

### 3. Module Federation 成熟度評估
**工業實踐**:
- ✅ **Microsoft Teams**: React + Angular 混合架構
- ✅ **Spotify**: 多團隊微前端整合
- ✅ **Alibaba**: 飛豬/淘寶使用 Webpack 5 Module Federation

**技術優勢**:
- ✅ **運行時整合**: 動態加載，無需重新編譯
- ✅ **依賴共享**: Singleton 模式避免重複打包
- ✅ **版本管理**: 靈活的版本控制策略
- ✅ **獨立部署**: Host 和 Remote 可獨立發布

**生態支持**:
- ✅ Webpack 5+ 官方支持
- ✅ Vite 實驗性支持（`vite-plugin-federation`）
- ✅ 豐富的社區實踐和文檔

---

## 架構設計

### 整體架構圖
```
┌─────────────────────────────────────────────────────────────┐
│                     瀏覽器 (Browser)                          │
│  ┌──────────────────────────────────────────────────────┐   │
│  │           React Host App (Port 3000)                  │   │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐      │   │
│  │  │ Dashboard  │  │ Agent List │  │  Settings  │      │   │
│  │  └────────────┘  └────────────┘  └────────────┘      │   │
│  │                                                        │   │
│  │  ┌────────────────────────────────────────────┐       │   │
│  │  │    WorkflowEditorWrapper (React Component) │       │   │
│  │  │  ┌──────────────────────────────────────┐  │       │   │
│  │  │  │ 動態載入 Vue Remote App (Runtime)     │  │       │   │
│  │  │  │  ┌────────────────────────────────┐  │  │       │   │
│  │  │  │  │  Vue Workflow Editor Module    │  │  │       │   │
│  │  │  │  │  - VueFlow 畫布                │  │  │       │   │
│  │  │  │  │  - 28 種節點類型               │  │  │       │   │
│  │  │  │  │  - 內聯配置面板                │  │  │       │   │
│  │  │  │  │  - Yjs CRDT 協作               │  │  │       │   │
│  │  │  │  └────────────────────────────────┘  │  │       │   │
│  │  │  └──────────────────────────────────────┘  │       │   │
│  │  └────────────────────────────────────────────┘       │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
│  通信層 (Communication Layer)                                │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │ Props 傳遞   │  │ Custom Events │  │  Event Bus   │        │
│  │ React → Vue │  │  Vue → React  │  │ Redux ↔ Pinia│        │
│  └─────────────┘  └──────────────┘  └──────────────┘        │
└─────────────────────────────────────────────────────────────┘
         ↓                               ↓
┌─────────────────┐          ┌─────────────────────────┐
│ React Host      │          │ Vue Remote              │
│ (localhost:3000)│ ←────→   │ (localhost:3001)        │
│ Vite Dev Server │   HTTP   │ Webpack Dev Server      │
└─────────────────┘          └─────────────────────────┘
```

### 技術棧對比
```yaml
React Host (主應用):
  框架: React 18.2+
  語言: TypeScript 5.0+
  UI 庫: Material-UI (MUI) 5.x
  狀態管理: Redux Toolkit 2.0
  路由: React Router v6
  構建工具: Vite 5.x
  測試: Vitest + React Testing Library
  端口: 3000

Vue Remote (工作流編輯器):
  框架: Vue 3.4+ (Composition API)
  語言: TypeScript 5.0+
  畫布引擎: VueFlow 1.45.0
  UI 庫: Element Plus 2.x
  狀態管理: Pinia 2.x
  構建工具: Webpack 5.x
  測試: Vitest + Vue Test Utils
  端口: 3001

共享層 (Shared):
  設計令牌: CSS Variables (design-tokens/tokens.css)
  類型定義: TypeScript (.d.ts)
  Event Bus: mitt (事件總線)
  工具函數: @ai-workflow/shared-utils
```

### Module Federation 配置

#### Host 配置 (React App - Vite)
```javascript
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import federation from '@originjs/vite-plugin-federation';

export default defineConfig({
  plugins: [
    react(),
    federation({
      name: 'host',
      remotes: {
        workflowEditor: 'http://localhost:3001/assets/remoteEntry.js'
      },
      shared: {
        react: { singleton: true, requiredVersion: '^18.2.0' },
        'react-dom': { singleton: true, requiredVersion: '^18.2.0' }
      }
    })
  ],
  server: {
    port: 3000,
    strictPort: true
  }
});
```

#### Remote 配置 (Vue App - Webpack)
```javascript
// webpack.config.js
const { ModuleFederationPlugin } = require('webpack').container;

module.exports = {
  entry: './src/main.ts',
  output: {
    publicPath: 'http://localhost:3001/',
    filename: '[name].[contenthash].js'
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'workflowEditor',
      filename: 'remoteEntry.js',
      exposes: {
        './WorkflowEditor': './src/WorkflowEditor.vue'
      },
      shared: {
        vue: { singleton: true, requiredVersion: '^3.4.0' },
        pinia: { singleton: true, requiredVersion: '^2.0.0' }
      }
    })
  ],
  devServer: {
    port: 3001,
    headers: {
      'Access-Control-Allow-Origin': '*'
    }
  }
};
```

### React Wrapper Component
```typescript
// src/components/WorkflowEditorWrapper.tsx
import React, { useEffect, useRef } from 'react';
import { createApp, type App as VueApp } from 'vue';

interface Props {
  workflowId: string;
  mode?: 'edit' | 'view';
  onSave?: (data: WorkflowData) => void;
  onError?: (error: Error) => void;
}

const WorkflowEditorWrapper: React.FC<Props> = ({
  workflowId,
  mode = 'edit',
  onSave,
  onError
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const vueAppRef = useRef<VueApp | null>(null);

  useEffect(() => {
    // 動態導入 Vue Remote Module
    import('workflowEditor/WorkflowEditor')
      .then(({ default: WorkflowEditor }) => {
        if (containerRef.current) {
          // 創建 Vue 應用實例
          vueAppRef.current = createApp(WorkflowEditor, {
            workflowId,
            mode,
            onSave,
            onError
          });

          // 掛載到 DOM
          vueAppRef.current.mount(containerRef.current);
        }
      })
      .catch((error) => {
        console.error('Failed to load Workflow Editor:', error);
        onError?.(error);
      });

    // 清理函數
    return () => {
      if (vueAppRef.current) {
        vueAppRef.current.unmount();
        vueAppRef.current = null;
      }
    };
  }, [workflowId, mode]);

  return (
    <div
      ref={containerRef}
      style={{ width: '100%', height: '100vh' }}
      data-testid="workflow-editor-container"
    />
  );
};

export default WorkflowEditorWrapper;
```

### Vue Workflow Editor
```vue
<!-- src/WorkflowEditor.vue -->
<template>
  <div class="workflow-editor">
    <VueFlow
      v-model="nodes"
      v-model:edges="edges"
      :node-types="nodeTypes"
      :default-viewport="{ zoom: 1, x: 0, y: 0 }"
      @node-drag-stop="handleNodeDragStop"
      @connect="handleConnect"
      @pane-click="handlePaneClick"
    >
      <Background :pattern-color="#aaa" :gap="16" />
      <Controls />
      <MiniMap />

      <!-- 自定義節點類型 -->
      <template #node-custom="{ data }">
        <CustomNode :data="data" />
      </template>
    </VueFlow>

    <!-- 內聯配置面板 -->
    <NodeConfigPanel
      v-if="selectedNode"
      :node="selectedNode"
      @update="handleNodeUpdate"
      @close="selectedNode = null"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { VueFlow, useVueFlow } from '@vue-flow/core';
import { Background, Controls, MiniMap } from '@vue-flow/additional-components';
import { useWorkflowStore } from '@/stores/workflowStore';
import { useCollaboration } from '@/composables/useCollaboration';
import CustomNode from '@/components/nodes/CustomNode.vue';
import NodeConfigPanel from '@/components/NodeConfigPanel.vue';

const props = defineProps<{
  workflowId: string;
  mode?: 'edit' | 'view';
  onSave?: (data: WorkflowData) => void;
  onError?: (error: Error) => void;
}>();

// Pinia Store
const workflowStore = useWorkflowStore();
const { nodes, edges, selectedNode } = storeToRefs(workflowStore);

// VueFlow 實例
const { addNodes, addEdges, onNodesChange, onEdgesChange } = useVueFlow();

// CRDT 協作（Yjs）
const { connectToRoom, syncState, disconnect } = useCollaboration(props.workflowId);

// 生命週期
onMounted(async () => {
  await connectToRoom();
  syncState();
});

onBeforeUnmount(() => {
  disconnect();
});

// 節點類型註冊
const nodeTypes = computed(() => ({
  trigger: CustomNode,
  action: CustomNode,
  ai: CustomNode,
  transform: CustomNode
}));

// 事件處理
const handleNodeDragStop = (event: NodeDragEvent) => {
  workflowStore.updateNodePosition(event.node.id, event.node.position);
};

const handleConnect = (params: Connection) => {
  workflowStore.addEdge(params);
};

const handleNodeUpdate = (nodeId: string, data: any) => {
  workflowStore.updateNodeData(nodeId, data);
  props.onSave?.(workflowStore.getWorkflowData());
};
</script>

<style scoped>
.workflow-editor {
  width: 100%;
  height: 100vh;
  background: var(--color-background);
}
</style>
```

### 跨框架通信

#### Event Bus 實現
```typescript
// shared/eventBus.ts
import mitt, { type Emitter } from 'mitt';

type Events = {
  'workflow:updated': WorkflowData;
  'workflow:saved': { id: string; timestamp: number };
  'node:selected': string | null;
  'node:added': NodeData;
  'execution:started': string;
  'execution:completed': { workflowId: string; result: any };
  'error:occurred': Error;
};

export const eventBus: Emitter<Events> = mitt<Events>();

// React 側使用
export function useWorkflowEvents() {
  useEffect(() => {
    const handler = (data: WorkflowData) => {
      console.log('Workflow updated:', data);
      // 更新 Redux store
      dispatch(updateWorkflow(data));
    };

    eventBus.on('workflow:updated', handler);
    return () => eventBus.off('workflow:updated', handler);
  }, []);
}

// Vue 側使用
export function useWorkflowEmitter() {
  const emit = (event: keyof Events, data: any) => {
    eventBus.emit(event, data);
  };

  return { emit };
}
```

#### 狀態同步策略
```typescript
// Redux Store (React)
const workflowSlice = createSlice({
  name: 'workflow',
  initialState,
  reducers: {
    syncFromVue: (state, action) => {
      // 從 Vue 同步狀態
      state.currentWorkflow = action.payload;
    }
  }
});

// Pinia Store (Vue)
export const useWorkflowStore = defineStore('workflow', () => {
  const workflow = ref<WorkflowData | null>(null);

  // 監聽 Event Bus
  eventBus.on('workflow:sync', (data) => {
    workflow.value = data;
  });

  return { workflow };
});
```

### 共享設計系統

#### Design Tokens (CSS Variables)
```css
/* shared/design-tokens/tokens.css */
:root {
  /* Colors - Material-UI 主題 */
  --color-primary: #1976d2;
  --color-secondary: #dc004e;
  --color-success: #4caf50;
  --color-error: #f44336;
  --color-warning: #ff9800;
  --color-info: #2196f3;

  /* Background */
  --color-background: #fafafa;
  --color-surface: #ffffff;

  /* Text */
  --color-text-primary: rgba(0, 0, 0, 0.87);
  --color-text-secondary: rgba(0, 0, 0, 0.6);

  /* Spacing - 8px 基準 */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;

  /* Grid System (n8n 參考) */
  --grid-size: 16px;
  --node-size-default: 96px;  /* 6 × 16px */
  --node-size-config: 80px;   /* 5 × 16px */
  --node-size-large: 256px;   /* 16 × 16px */

  /* Typography */
  --font-family: 'Roboto', 'Helvetica', 'Arial', sans-serif;
  --font-size-sm: 12px;
  --font-size-md: 14px;
  --font-size-lg: 16px;
  --font-size-xl: 20px;

  /* Shadows */
  --shadow-sm: 0 1px 3px rgba(0,0,0,0.12);
  --shadow-md: 0 4px 6px rgba(0,0,0,0.1);
  --shadow-lg: 0 10px 20px rgba(0,0,0,0.15);

  /* Border Radius */
  --radius-sm: 4px;
  --radius-md: 8px;
  --radius-lg: 12px;

  /* Z-index Layers */
  --z-index-drawer: 1200;
  --z-index-modal: 1300;
  --z-index-snackbar: 1400;
  --z-index-tooltip: 1500;
}
```

---

## 優勢與風險

### 優勢 ✅

#### 1. 開發效率
- ✅ **快速交付**: 複用 n8n 架構，開發週期從 6 個月縮短至 **3 個月**
- ✅ **成熟方案**: VueFlow 1.45.0 生產級穩定，避免自研風險
- ✅ **參考代碼**: n8n Canvas.vue 1,800+ 行可直接參考

#### 2. 技術成熟度
- ✅ **工業驗證**: Microsoft Teams, Spotify 實際應用
- ✅ **生態完善**: Webpack 5 Module Federation 官方支持
- ✅ **社區活躍**: 豐富的文檔和問題解決方案

#### 3. 架構靈活性
- ✅ **獨立演進**: React 和 Vue 模組可獨立升級
- ✅ **團隊分工**: 前端團隊可並行開發
- ✅ **技術多樣性**: 未來可引入其他框架模組

#### 4. 性能優化
- ✅ **按需加載**: Module Federation 運行時動態加載
- ✅ **代碼分割**: Host 和 Remote 獨立打包
- ✅ **緩存策略**: Singleton 共享依賴，減少重複加載

### 風險與緩解 ⚠️

#### 風險 1: 跨框架通信複雜度 🟡 中等風險
**風險描述**:
- React ↔ Vue 數據同步可能出現延遲
- Event Bus 事件管理可能混亂

**緩解措施**:
- ✅ **標準化通信**: 使用 TypeScript 定義事件接口
- ✅ **Event Bus 封裝**: 使用 `mitt` 庫，提供類型安全
- ✅ **單向數據流**: Props down, Events up 原則
- ✅ **調試工具**: Redux DevTools + Vue DevTools 並行使用

**監控指標**:
- Event Bus 事件延遲 <50ms
- 狀態同步錯誤率 <0.1%

#### 風險 2: 依賴管理複雜性 🟡 中等風險
**風險描述**:
- React 和 Vue 依賴版本衝突
- Singleton 模式可能導致版本鎖定

**緩解措施**:
- ✅ **Monorepo 管理**: 使用 pnpm workspaces 統一依賴
- ✅ **版本鎖定**: package.json 精確版本（無 `^` `~`）
- ✅ **Compatibility Matrix**: 維護框架版本兼容性表格
- ✅ **自動化測試**: CI/CD 檢查依賴衝突

**Compatibility Matrix**:
```yaml
react: 18.2.0
react-dom: 18.2.0
vue: 3.4.21
pinia: 2.1.7
@vue-flow/core: 1.45.0
typescript: 5.3.3
webpack: 5.90.3
vite: 5.1.4
```

#### 風險 3: 構建工具差異 🟢 低風險
**風險描述**:
- Vite (Host) 和 Webpack (Remote) 構建配置不一致
- 開發環境和生產環境行為差異

**緩解措施**:
- ✅ **統一輸出格式**: ES Modules (ESM) 標準
- ✅ **環境變量管理**: `.env` 文件統一配置
- ✅ **構建腳本**: npm scripts 標準化
- ✅ **CI/CD 驗證**: 每次部署前完整構建測試

**構建腳本範例**:
```json
{
  "scripts": {
    "dev": "concurrently \"npm run dev:host\" \"npm run dev:remote\"",
    "dev:host": "cd packages/host && vite",
    "dev:remote": "cd packages/remote && webpack serve",
    "build": "npm run build:host && npm run build:remote",
    "build:host": "cd packages/host && vite build",
    "build:remote": "cd packages/remote && webpack --mode production"
  }
}
```

#### 風險 4: 團隊學習曲線 🟢 低風險
**風險描述**:
- 團隊需要同時掌握 React 和 Vue
- Module Federation 概念較新

**緩解措施**:
- ✅ **技術培訓**: 2 週 Vue 3 Composition API 培訓
- ✅ **文檔完善**: 內部 Wiki 和最佳實踐指南
- ✅ **Code Review**: 跨框架代碼審查機制
- ✅ **Pair Programming**: React/Vue 開發者配對

**培訓計劃**:
```yaml
Week 1:
  - Vue 3 Composition API 基礎
  - VueFlow 快速上手
  - Module Federation 原理

Week 2:
  - 跨框架通信實戰
  - 設計系統整合
  - 調試和性能優化
```

---

## 實施計劃

### Week 1-2: 基礎設施 (Infrastructure)
```yaml
Day 1-3: Monorepo 搭建
  - [ ] 創建 pnpm workspace 結構
  - [ ] 配置 TypeScript 共享配置
  - [ ] 設置 ESLint/Prettier

Day 4-7: Module Federation 配置
  - [ ] Host (React): Vite + @originjs/vite-plugin-federation
  - [ ] Remote (Vue): Webpack 5 + ModuleFederationPlugin
  - [ ] 驗證動態加載和依賴共享

Day 8-10: 共享層開發
  - [ ] Design Tokens (CSS Variables)
  - [ ] Event Bus (mitt)
  - [ ] TypeScript 類型定義

Day 11-14: 開發環境優化
  - [ ] Hot Module Replacement (HMR)
  - [ ] 調試工具配置
  - [ ] Docker Compose 本地環境
```

### Week 3-4: Vue 工作流編輯器核心
```yaml
Day 15-18: VueFlow 畫布基礎
  - [ ] VueFlow 初始化和配置
  - [ ] 網格系統（16px）
  - [ ] 基礎節點類型（4 種）

Day 19-22: 節點系統（參考 n8n）
  - [ ] 28 種節點類型實現
  - [ ] 節點拖放和連接
  - [ ] 節點數據模型

Day 23-26: 內聯配置面板
  - [ ] NodeConfigPanel 組件
  - [ ] 表單驗證
  - [ ] 實時預覽

Day 27-28: 測試和優化
  - [ ] Vitest 單元測試
  - [ ] 性能測試（60 FPS）
```

### Week 5-6: React 主應用整合
```yaml
Day 29-32: Wrapper Component
  - [ ] WorkflowEditorWrapper 開發
  - [ ] Props/Events 通信
  - [ ] 錯誤邊界處理

Day 33-36: 狀態同步
  - [ ] Redux ↔ Pinia Event Bus
  - [ ] 工作流數據持久化
  - [ ] 樂觀更新機制

Day 37-40: UI 一致性
  - [ ] Material-UI 主題應用
  - [ ] 響應式布局
  - [ ] 無障礙（WCAG 2.1 AA）

Day 41-42: 集成測試
  - [ ] E2E 測試（Playwright）
  - [ ] 跨瀏覽器測試
```

### Week 7-8: CRDT 協作（基於 PoC 6）
```yaml
Day 43-46: Yjs 整合
  - [ ] Yjs Provider 配置
  - [ ] WebSocket Server (Socket.io)
  - [ ] 衝突解決邏輯

Day 47-50: 多用戶功能
  - [ ] 用戶光標顯示
  - [ ] 變更歷史記錄
  - [ ] 操作撤銷/重做

Day 51-54: 性能優化
  - [ ] 增量同步
  - [ ] 離線支持
  - [ ] 數據壓縮

Day 55-56: 壓力測試
  - [ ] 3+ 並發用戶測試
  - [ ] 1000+ 節點性能測試
```

### Week 9-10: 生產準備
```yaml
Day 57-60: 構建優化
  - [ ] 代碼分割策略
  - [ ] Tree Shaking
  - [ ] 打包體積優化

Day 61-64: 部署配置
  - [ ] Docker 多階段構建
  - [ ] Nginx 配置
  - [ ] CDN 靜態資源

Day 65-68: 監控和日誌
  - [ ] 錯誤追蹤（Sentry）
  - [ ] 性能監控（Web Vitals）
  - [ ] 用戶行為分析

Day 69-70: 文檔和培訓
  - [ ] 技術文檔
  - [ ] 操作手冊
  - [ ] 團隊培訓
```

---

## 驗收標準

### 功能性標準 ✅
- [ ] **28 種節點類型**: 所有節點可正常拖放和配置
- [ ] **內聯配置**: 點擊節點顯示配置面板，修改實時生效
- [ ] **CRDT 協作**: 3+ 用戶並發編輯，無衝突
- [ ] **狀態同步**: React ↔ Vue 狀態延遲 <50ms
- [ ] **響應式設計**: Desktop/Tablet/Mobile 正常顯示

### 性能標準 ⚡
- [ ] **渲染性能**: 60 FPS（1000+ 節點）
- [ ] **首屏加載**: <2 秒（3G 網絡）
- [ ] **模塊加載**: Vue Remote 動態加載 <500ms
- [ ] **內存使用**: <200MB（Chrome DevTools）

### 質量標準 🛡️
- [ ] **單元測試**: 覆蓋率 >80%
- [ ] **E2E 測試**: 核心流程覆蓋 100%
- [ ] **TypeScript**: 無 `any` 類型，嚴格模式
- [ ] **無障礙**: WCAG 2.1 AA 合規
- [ ] **瀏覽器兼容**: Chrome 90+, Firefox 88+, Safari 14+

### 可維護性標準 🔧
- [ ] **代碼規範**: ESLint + Prettier 自動化
- [ ] **文檔完整**: API 文檔、架構圖、部署指南
- [ ] **CI/CD**: 自動化測試和部署流程
- [ ] **監控**: 錯誤追蹤和性能監控

---

## 參考資料

### n8n 源代碼分析
- `n8n-analysis/packages/editor-ui/src/components/canvas/`
- `Canvas.vue` (1,800+ 行核心實現)
- `useCanvas.ts`, `useCanvasNode.ts`, `useCanvasMapping.ts`

### PoC 驗證報告
- `poc-projects/poc6-vueflow-crdt/`
- `TEST-RESULTS.md` - 性能測試數據
- `FINAL-COMPLETE-TEST-REPORT.md` - 完整驗證報告

### 技術文檔
- [Webpack Module Federation](https://webpack.js.org/concepts/module-federation/)
- [VueFlow Documentation](https://vueflow.dev/)
- [Yjs CRDT](https://docs.yjs.dev/)
- [n8n Architecture](https://docs.n8n.io/integrations/creating-nodes/)

### 工業實踐案例
- [Microsoft Teams - Microfrontend Architecture](https://medium.com/@teams/teams-architecture-evolution-4b5c8a5e5e8c)
- [Spotify - Micro Frontends](https://www.youtube.com/watch?v=zGMR2SK09z0)
- [Alibaba - 飛豬技術架構](https://www.infoq.cn/article/alibaba-micro-frontend)

---

## 決策總結

| 決策項 | 選擇方案 | 替代方案 | 主要理由 |
|--------|---------|---------|---------|
| **主應用框架** | React 18 | Vue 3 | 95% 頁面已確定 React，團隊熟悉 |
| **工作流引擎** | Vue 3 + VueFlow | React Flow | n8n 成熟參考，節省 35% 開發時間 |
| **整合方案** | Module Federation | iframe / Web Components | 運行時整合，性能佳，工業驗證 |
| **構建工具** | Vite (Host) + Webpack (Remote) | 全 Webpack | Vite 開發體驗佳，Webpack 支持 Module Federation |
| **協作方案** | Yjs CRDT | WebRTC | PoC 6 驗證通過，<200ms 延遲 |

**最終決策**: ✅ 採用 **微前端架構 (React Host + Vue Remote) + Module Federation**

---

**簽署與批准**:
- **技術負責人**: [簽名] - 2025-11-01
- **架構師**: [簽名] - 2025-11-01
- **產品經理**: [簽名] - 2025-11-01

**下一步行動**:
1. 更新項目文檔（README.md, Technical Implementation）
2. 創建 Module Federation 實施指南
3. 啟動 Week 1-2 基礎設施開發

---

**文檔版本**: 1.0.0
**最後更新**: 2025-11-01
**維護者**: 架構團隊
