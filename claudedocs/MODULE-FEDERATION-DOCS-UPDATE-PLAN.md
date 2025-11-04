# Module Federation 架構決策 - 文檔更新計劃

**決策**: 採用 VueFlow + React 微前端架構 (Module Federation)
**日期**: 2025-11-01
**狀態**: 已批准 ✅
**影響範圍**: 7 個文檔層級需要更新

---

## 執行摘要

基於 n8n 源代碼分析和 PoC 6 驗證結果,我們決定採用 **微前端架構 (Module Federation)** 方案:

- **工作流編輯器**: Vue 3 + VueFlow (獨立模塊)
- **其他頁面**: React 18 + Material-UI (現有標準)
- **整合方式**: Webpack 5 Module Federation
- **核心理由**: 35% 開發時間節省、完整 n8n 參考實現、PoC 6 已驗證

**關鍵文檔**:
- 技術決策報告: `claudedocs/WORKFLOW-EDITOR-TECH-DECISION.md`
- 微前端架構指南: 本對話中提供的完整方案

---

## 文檔更新矩陣

### 層級 1: 項目根目錄

#### 📄 `README.md` - **更新優先級: 🔴 高**

**需要更新的部分**:

1. **技術棧章節** (Line 56-86)

```yaml
當前狀態:
  前端: React 18 + TypeScript 5 + Material-UI + Redux Toolkit + Vite

需要更新為:
  前端框架:
    - 主應用: React 18 + TypeScript 5 (95% 頁面)
    - 工作流編輯器: Vue 3 + TypeScript 5 (獨立模塊)
  UI 組件庫:
    - React 頁面: Material-UI (MUI)
    - Vue 編輯器: Element Plus + VueFlow
  狀態管理:
    - React: Redux Toolkit
    - Vue: Pinia
  整合方式:
    - 微前端: Webpack 5 Module Federation
    - 視覺一致性: 共享 Design Tokens (CSS Variables)
  構建工具:
    - React 主應用: Vite 5
    - Vue 編輯器: Webpack 5 (Module Federation)
```

2. **快速開始章節** (Line 113-150)

添加工作流編輯器開發環境說明:

```bash
# 5. 啟動 Frontend Dev Server (React 主應用)
cd packages/frontend-host
npm install
npm run dev  # Port 3000

# 6. 啟動 Workflow Editor (Vue 模塊)
cd packages/workflow-editor
npm install
npm run dev  # Port 3001
```

**更新文件**: `README.md`

---

### 層級 2: `docs/brief.md` - **更新優先級: 🟡 中**

**需要更新的部分**:

**Section: Proposed Solution - Technology Stack** (預計在 Line 300+)

添加前端架構說明:

```markdown
### 前端架構 (微前端方案)

基於 n8n 源代碼分析和 PoC 6 (VueFlow + CRDT) 驗證結果,我們採用微前端架構:

**架構決策 (ADR-012)**:
- **工作流編輯器**: Vue 3 + VueFlow (參考 n8n 實現)
- **其他頁面**: React 18 + Material-UI (項目標準)
- **整合方式**: Webpack 5 Module Federation

**技術理由**:
1. **35% 時間節省**: VueFlow 方案 9 週 vs React Flow 方案 14 週
2. **n8n 生產驗證**: 1000+ 節點規模已驗證
3. **完整參考實現**: 可直接參考 n8n Canvas.vue, Node.vue 等組件
4. **PoC 6 驗證**: Vue 3 + VueFlow + Yjs CRDT 已通過實時協作測試

**架構隔離**:
- Vue 模塊獨立開發、測試、部署
- 通過 Module Federation 動態載入
- 共享 Design Tokens 確保視覺一致性
```

**更新文件**: `docs/brief.md` (在 Technology Stack 章節)

---

### 層級 3: `docs/user-stories/README.md` - **更新優先級: 🟡 中**

**需要更新的部分**:

**模組 4: Multi-Agent 協作** (Line 127-135)

```yaml
當前描述:
  US 4.1: 可視化工作流編輯器 ⭐ | P0 | 13 Story Points

需要補充技術實施細節:
  US 4.1: 可視化工作流編輯器 ⭐
    優先級: P0
    Story Points: 13
    技術選型: Vue 3 + VueFlow (微前端模塊)
    參考實現: n8n Canvas.vue 架構
    核心能力:
      - 28 種節點類型（參考 n8n）
      - 內聯配置面板 (ExperimentalEmbeddedNodeDetails)
      - 實時協作 (Yjs CRDT, PoC 6 已驗證)
      - 16px 網格系統
      - 執行視覺化與數據流動畫
```

**更新文件**: `docs/user-stories/README.md`

**連結新文檔**:
```markdown
**技術決策文檔**:
- [工作流編輯器技術選型](../../claudedocs/WORKFLOW-EDITOR-TECH-DECISION.md)
- [Module Federation 實施指南](../technical-implementation/3-project-structure/frontend-vue-structure.md)
```

---

### 層級 4: `docs/architecture/Architecture-Design-Document.md` - **更新優先級: 🔴 高**

**需要新增 ADR**:

創建 **ADR-012: Workflow Editor Technology Stack Exception**

**位置**: `docs/architecture/ADR-012-workflow-editor-technology.md`

```markdown
# ADR-012: Workflow Editor Technology Stack Exception

**Status**: ✅ Approved
**Date**: 2025-11-01
**Decision Makers**: Tech Lead, Frontend Architect, PO

## Context

項目標準前端技術棧為 React 18 + Material-UI (95% 頁面)。但工作流編輯器有特殊需求:

1. **n8n 參考實現**: n8n 使用 Vue 3 + VueFlow,提供完整參考
2. **開發效率**: VueFlow 方案 9 週 vs React Flow 方案 14 週 (35% 節省)
3. **PoC 驗證**: PoC 6 已驗證 Vue 3 + VueFlow + Yjs CRDT 協作能力
4. **技術成熟度**: VueFlow 在 n8n 生產環境驗證,支持 1000+ 節點規模

## Decision

**採用微前端架構 (Module Federation)**:

- **工作流編輯器**: Vue 3 + VueFlow (獨立模塊,Port 3001)
- **其他頁面**: React 18 + Material-UI (保持標準,Port 3000)
- **整合方式**: Webpack 5 Module Federation

## Consequences

### Positive

✅ **35% 時間節省**: 9 週 vs 14 週 = ~$143,000 成本節省
✅ **完整參考實現**: 可直接參考 n8n 源代碼
✅ **降低技術風險**: n8n 生產驗證,無需從零設計
✅ **實時協作已驗證**: PoC 6 通過 60 FPS, <200ms 同步延遲測試

### Negative

⚠️ **雙技術棧**: 需維護 React + Vue 兩套技術棧
⚠️ **文檔不一致**: 與 95% 文檔的 React 標準衝突
⚠️ **團隊學習曲線**: Vue 團隊需學習 Vue 3 Composition API

### Mitigation

1. **架構隔離**: 通過 Module Federation 完全隔離
2. **視覺一致性**: 共享 Design Tokens (CSS Variables)
3. **文檔更新**: 創建 ADR-012 記錄例外情況
4. **團隊培訓**: Vue 3 Composition API 培訓 (1 週)

## Implementation

### Project Structure

```
packages/
├── frontend-host/          # React 主應用 (Host)
│   ├── webpack.config.js   # Host 配置 (remotes)
│   └── port: 3000
└── workflow-editor/        # Vue 工作流編輯器 (Remote)
    ├── webpack.config.js   # Remote 配置 (exposes)
    └── port: 3001
```

### Module Federation Configuration

**Host (React)**:
```javascript
new ModuleFederationPlugin({
  name: 'host',
  remotes: {
    workflowEditor: 'workflowEditor@http://localhost:3001/remoteEntry.js'
  }
})
```

**Remote (Vue)**:
```javascript
new ModuleFederationPlugin({
  name: 'workflowEditor',
  filename: 'remoteEntry.js',
  exposes: {
    './WorkflowEditor': './src/components/WorkflowEditor.vue'
  }
})
```

## References

- [n8n 源代碼分析](../../n8n-analysis/claudedocs/n8n-architecture-analysis.md)
- [PoC 6 驗證報告](../technical-implementation/1-poc-validation/06-vueflow-crdt-collaboration.md)
- [工作流編輯器技術決策](../../claudedocs/WORKFLOW-EDITOR-TECH-DECISION.md)
```

**更新 ADD 主文檔** (`Architecture-Design-Document.md`):

在 **Section 2: 核心架構決策 (ADRs)** 添加:

```markdown
### 2.5 ADR-012: Workflow Editor Technology Stack Exception

**文檔**: [`ADR-012-workflow-editor-technology.md`](./ADR-012-workflow-editor-technology.md)

**決策**: Vue 3 + VueFlow (微前端架構)

**背景**:
項目標準為 React 18,但工作流編輯器基於以下理由採用 Vue 3:

1. n8n (行業領先) 使用 VueFlow,提供完整參考實現
2. 開發時間節省 35% (9 週 vs 14 週)
3. PoC 6 驗證通過 (VueFlow + Yjs CRDT)

**架構**:
```yaml
微前端方案 (Module Federation):
  Host (React):
    - Dashboard, Agent 管理, Settings
    - Port: 3000

  Remote (Vue):
    - Workflow Editor (VueFlow 編輯器)
    - Port: 3001

  整合:
    - Webpack 5 Module Federation
    - 共享 Design Tokens (CSS Variables)
    - Props + Custom Events 通訊
```

**理由**:
- ✅ 完整 n8n 參考實現 (Canvas.vue, Node.vue, Composables)
- ✅ 35% 時間節省 = ~$143,000 成本節省
- ✅ 技術風險降低 (生產驗證 1000+ 節點)
- ⚠️ 需維護雙技術棧 (通過架構隔離緩解)

**實現**: `IWorkflowEditorModule` 接口 + `VueWorkflowEditorModule` (Remote)
```

**更新文件**:
- 創建 `docs/architecture/ADR-012-workflow-editor-technology.md`
- 更新 `docs/architecture/Architecture-Design-Document.md`

---

### 層級 5: `docs/ux-design/README.md` - **更新優先級: 🟢 低**

**需要更新的部分**:

**工具和技術章節** (Line 277-292)

```yaml
當前狀態:
  Implementation:
    前端框架: React 18 + TypeScript

需要更新為:
  Implementation:
    前端框架:
      - React 18 + TypeScript (主應用)
      - Vue 3 + TypeScript (工作流編輯器)
    UI 組件庫:
      - Material-UI (MUI) - React 頁面
      - Element Plus - Vue 編輯器
    整合方式:
      - Webpack 5 Module Federation (微前端)
```

**Workflow Editor 線框圖章節** (Line 165-173)

添加技術實施註記:

```markdown
9. **✅ Multi-Agent Workflow Editor V2** ⭐ (可視化編輯器)
   - 完整 7 部分設計文檔（~20,000 行）
   - n8n 風格大卡片節點系統
   - **技術實施**: Vue 3 + VueFlow (微前端模塊)
   - **參考實現**: n8n Canvas.vue 架構
   - **PoC 驗證**: PoC 6 通過實時協作測試
```

**更新文件**: `docs/ux-design/README.md`

---

### 層級 6: `docs/technical-implementation/README.md` - **更新優先級: 🔴 高**

**需要大幅更新的章節**:

#### 1. **前端技術棧表格** (Line 192-203)

```yaml
當前狀態:
  框架: React 18
  UI 組件: Material-UI (MUI)

需要更新為:
  | 組件 | 技術選型 | 理由 | 適用範圍 |
  |------|---------|------|---------|
  | **React 主應用** | React 18 | 生態豐富 | 95% 頁面 |
  | **Vue 編輯器** | Vue 3 + VueFlow | n8n 參考 | 工作流編輯器 |
  | **微前端整合** | Module Federation | 技術隔離 | Host ↔ Remote |
  | **UI 組件 (React)** | Material-UI | 可訪問性 | Dashboard, Settings |
  | **UI 組件 (Vue)** | Element Plus | n8n 一致 | Workflow Editor |
  | **狀態 (React)** | Redux Toolkit | 標準選擇 | 主應用 |
  | **狀態 (Vue)** | Pinia | Vue 官方 | 編輯器 |
```

#### 2. **PoC 驗證計劃** (Line 246-278)

**PoC 6 已完成**,需要更新為:

```markdown
**Day 8-10: VueFlow + CRDT 實時協作 PoC** ✅ 已完成
- ✅ Vue 3 + VueFlow 基礎畫布
- ✅ Yjs CRDT 文檔同步
- ✅ WebSocket 服務器 (Socket.io)
- ✅ 多用戶協作驗證
- ✅ Playwright 多瀏覽器測試

**驗證結果** (PoC 6):
- ✅ 性能: 60 FPS 流暢渲染
- ✅ 延遲: <200ms 同步延遲
- ✅ 衝突: CRDT 自動解決
- ✅ 離線: 斷線重連恢復同步
- ✅ 規模: 支持 3+ 用戶同時編輯

**成功標準**: 全部達成 ✅
- 60 FPS 渲染性能 ✅
- <200ms 同步延遲 ✅
- 支持 3+ 用戶協作 ✅
```

#### 3. **項目腳手架章節** (Line 148-175)

```yaml
當前結構:
  src/
    ├── Api/
    ├── Core/
    ├── Web/  # Frontend (React)

需要更新為:
  SemanticKernel.Agentic/
  ├── src/
  │   ├── Api/
  │   ├── Core/
  │   ├── Infrastructure/
  │   └── Plugins/
  ├── packages/  # ⭐ 新增 Monorepo 結構
  │   ├── frontend-host/       # React 主應用 (Host)
  │   │   ├── webpack.config.js
  │   │   └── package.json
  │   ├── workflow-editor/     # Vue 編輯器 (Remote)
  │   │   ├── webpack.config.js
  │   │   └── package.json
  │   └── shared/              # 共享資源
  │       ├── design-tokens/   # CSS Variables
  │       └── types/           # TypeScript 類型
  ├── pnpm-workspace.yaml
  └── package.json
```

**更新文件**: `docs/technical-implementation/README.md`

---

### 層級 7: `docs/technical-implementation/3-project-structure/` - **更新優先級: 🔴 高**

#### 新增文件:

1. **`frontend-vue-structure.md`** - 完整的 Vue 工作流編輯器結構

```markdown
# Frontend Vue Structure (Vue 3 - Workflow Editor)

## 概述

Vue 3 工作流編輯器作為獨立的微前端模塊 (Remote),通過 Module Federation 與 React 主應用整合。

**技術棧**:
- Vue 3.4+ (Composition API)
- VueFlow 1.45+ (參考 n8n)
- Pinia (狀態管理)
- Yjs + Socket.io (實時協作)
- Webpack 5 (Module Federation)

## 項目結構

```
packages/workflow-editor/
├── public/
├── src/
│   ├── components/
│   │   ├── Canvas/
│   │   │   ├── Canvas.vue           # 參考 n8n Canvas.vue
│   │   │   ├── CanvasNode.vue       # 參考 n8n CanvasNode.vue
│   │   │   └── CanvasConnection.vue
│   │   ├── NodeTypes/               # 28 種節點類型
│   │   │   ├── PromptNode.vue
│   │   │   ├── AgentNode.vue
│   │   │   ├── FunctionNode.vue
│   │   │   └── ...
│   │   ├── Panels/
│   │   │   ├── NodePanel.vue        # 節點面板
│   │   │   ├── ConfigPanel.vue      # 配置面板
│   │   │   └── ExpressionPanel.vue  # 表達式編輯器
│   │   └── Collaboration/
│   │       ├── UserCursor.vue
│   │       └── UserAvatar.vue
│   ├── composables/
│   │   ├── useCanvas.ts             # 參考 n8n
│   │   ├── useCanvasNode.ts         # 參考 n8n
│   │   ├── useCanvasMapping.ts      # 參考 n8n
│   │   └── useCollaboration.ts      # Yjs CRDT
│   ├── stores/
│   │   ├── workflowStore.ts         # Pinia Store
│   │   ├── nodeStore.ts
│   │   └── collaborationStore.ts
│   ├── types/
│   │   ├── workflow.ts
│   │   ├── node.ts
│   │   └── collaboration.ts
│   ├── utils/
│   │   ├── nodeFactory.ts
│   │   └── validation.ts
│   ├── WorkflowEditor.vue           # 主組件 (Exposed)
│   └── main.ts
├── webpack.config.js                # Module Federation Remote
├── package.json
└── tsconfig.json
```

## Module Federation 配置

### Remote 配置 (Vue 編輯器)

```javascript
// webpack.config.js
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

module.exports = {
  plugins: [
    new ModuleFederationPlugin({
      name: 'workflowEditor',
      filename: 'remoteEntry.js',
      exposes: {
        './WorkflowEditor': './src/WorkflowEditor.vue'
      },
      shared: {
        vue: {
          singleton: true,
          requiredVersion: '^3.4.0'
        },
        pinia: {
          singleton: true
        }
      }
    })
  ]
};
```

## 核心組件

### 1. WorkflowEditor.vue (Exposed Component)

```vue
<template>
  <div class="workflow-editor">
    <VueFlow
      v-model="nodes"
      v-model:edges="edges"
      :node-types="nodeTypes"
      @node-drag-stop="handleNodeDragStop"
      @connect="handleConnect"
    >
      <Background :pattern-color="#aaa" gap="16" />
      <Controls />
      <MiniMap />
    </VueFlow>

    <NodePanel />
    <ConfigPanel v-if="selectedNode" :node="selectedNode" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { VueFlow, Background, Controls, MiniMap } from '@vue-flow/core';
import { useWorkflowStore } from '@/stores/workflowStore';
import { useCollaboration } from '@/composables/useCollaboration';
import NodePanel from '@/components/Panels/NodePanel.vue';
import ConfigPanel from '@/components/Panels/ConfigPanel.vue';

// Props (從 React 傳入)
interface Props {
  workflowId: string;
  mode?: 'edit' | 'view';
  onSave?: (data: WorkflowData) => void;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  save: [data: WorkflowData];
  error: [error: Error];
}>();

// Store
const workflowStore = useWorkflowStore();
const { nodes, edges, selectedNode } = storeToRefs(workflowStore);

// Collaboration (Yjs CRDT)
const { connectToRoom, syncState } = useCollaboration(props.workflowId);

onMounted(() => {
  connectToRoom();
});

const handleNodeDragStop = (event) => {
  syncState({ type: 'node-move', nodeId: event.node.id, position: event.node.position });
};

const handleConnect = (connection) => {
  workflowStore.addEdge(connection);
  syncState({ type: 'edge-add', edge: connection });
};

const handleSave = () => {
  const workflowData = {
    nodes: nodes.value,
    edges: edges.value,
  };
  props.onSave?.(workflowData);
  emit('save', workflowData);
};
</script>

<style scoped>
.workflow-editor {
  width: 100%;
  height: 100vh;
  position: relative;
}
</style>
```

### 2. useCanvas.ts (參考 n8n)

```typescript
// src/composables/useCanvas.ts
import { ref, computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';

export function useCanvas() {
  const { viewport, fitView, zoomIn, zoomOut } = useVueFlow();
  const initialized = ref(false);
  const isExperimentalNdvActive = ref(false);

  const GRID_SIZE = 16; // n8n 使用 16px 網格

  const alignToGrid = (position: { x: number; y: number }) => ({
    x: Math.round(position.x / GRID_SIZE) * GRID_SIZE,
    y: Math.round(position.y / GRID_SIZE) * GRID_SIZE,
  });

  return {
    viewport,
    initialized,
    isExperimentalNdvActive,
    GRID_SIZE,
    alignToGrid,
    fitView,
    zoomIn,
    zoomOut,
  };
}
```

## React 整合

### Host 使用 Vue 組件

```typescript
// React Host: src/pages/Workflows/WorkflowEdit.tsx
import React, { useEffect, useRef } from 'react';
import { createApp } from 'vue';

const WorkflowEditorWrapper: React.FC = () => {
  const containerRef = useRef<HTMLDivElement>(null);
  const vueAppRef = useRef<any>(null);

  useEffect(() => {
    // 動態載入 Vue 組件
    import('workflowEditor/WorkflowEditor').then(({ default: WorkflowEditor }) => {
      if (containerRef.current) {
        vueAppRef.current = createApp(WorkflowEditor, {
          workflowId: 'wf-123',
          mode: 'edit',
          onSave: (data) => {
            console.log('Workflow saved:', data);
          },
        });
        vueAppRef.current.mount(containerRef.current);
      }
    });

    return () => {
      vueAppRef.current?.unmount();
    };
  }, []);

  return <div ref={containerRef} style={{ width: '100%', height: '100vh' }} />;
};

export default WorkflowEditorWrapper;
```

## 參考 n8n 實現

### 核心文件映射

| n8n 文件 | 我們的實現 | 說明 |
|----------|-----------|------|
| `Canvas.vue` | `src/components/Canvas/Canvas.vue` | 主畫布組件 |
| `CanvasNode.vue` | `src/components/Canvas/CanvasNode.vue` | 節點組件 |
| `useCanvas.ts` | `src/composables/useCanvas.ts` | 畫布邏輯 |
| `useCanvasNode.ts` | `src/composables/useCanvasNode.ts` | 節點邏輯 |
| `ExperimentalEmbeddedNodeDetails.vue` | `src/components/Panels/ConfigPanel.vue` | 內聯配置 |

### n8n 設計模式複用

1. **16px 網格系統**
2. **節點尺寸**: 96x96 (default), 80x80 (config), 256x96 (configurable)
3. **Composables 架構**: useCanvas, useCanvasNode, useCanvasMapping
4. **Inject/Provide**: 狀態共享機制
5. **Dynamic Height**: 根據端口數量自動調整節點高度

---

**參考文檔**:
- [n8n 源代碼分析](../../../n8n-analysis/claudedocs/n8n-architecture-analysis.md)
- [PoC 6 驗證報告](../1-poc-validation/06-vueflow-crdt-collaboration.md)
- [工作流編輯器技術決策](../../../claudedocs/WORKFLOW-EDITOR-TECH-DECISION.md)

**最後更新**: 2025-11-01
**狀態**: ✅ 已規劃
```

2. **`module-federation-guide.md`** - Module Federation 實施指南

```markdown
# Module Federation 實施指南

## 概述

本指南說明如何在 React 主應用與 Vue 工作流編輯器之間使用 Webpack 5 Module Federation 進行微前端整合。

## 架構圖

```
┌─────────────────────────────────────────────────┐
│  React 主應用 (Host) - Port 3000                 │
│  ├─ Dashboard                                   │
│  ├─ Agent Management                            │
│  ├─ Settings                                    │
│  └─ Dynamic Load ↓                              │
│      ┌──────────────────────────────────────┐   │
│      │ Vue 工作流編輯器 (Remote) - Port 3001│   │
│      │ - VueFlow Canvas                     │   │
│      │ - 28 種節點類型                      │   │
│      │ - Yjs CRDT 協作                      │   │
│      └──────────────────────────────────────┘   │
└─────────────────────────────────────────────────┘

通訊: Props (React → Vue) + Custom Events (Vue → React)
狀態: Redux (React) ↔ Event Bus ↔ Pinia (Vue)
樣式: 共享 Design Tokens (CSS Variables)
```

## 配置步驟

### Step 1: Host 配置 (React)

#### webpack.config.js

```javascript
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;

module.exports = {
  entry: './src/index',
  mode: 'development',
  devServer: {
    port: 3000,
  },
  plugins: [
    new ModuleFederationPlugin({
      name: 'host',
      remotes: {
        workflowEditor: 'workflowEditor@http://localhost:3001/remoteEntry.js',
      },
      shared: {
        react: { singleton: true, requiredVersion: '^18.2.0' },
        'react-dom': { singleton: true, requiredVersion: '^18.2.0' },
      },
    }),
  ],
};
```

### Step 2: Remote 配置 (Vue)

#### webpack.config.js

```javascript
const ModuleFederationPlugin = require('webpack').container.ModuleFederationPlugin;
const { VueLoaderPlugin } = require('vue-loader');

module.exports = {
  entry: './src/main',
  mode: 'development',
  devServer: {
    port: 3001,
    headers: {
      'Access-Control-Allow-Origin': '*',
    },
  },
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader',
      },
    ],
  },
  plugins: [
    new VueLoaderPlugin(),
    new ModuleFederationPlugin({
      name: 'workflowEditor',
      filename: 'remoteEntry.js',
      exposes: {
        './WorkflowEditor': './src/WorkflowEditor.vue',
      },
      shared: {
        vue: { singleton: true, requiredVersion: '^3.4.0' },
        pinia: { singleton: true },
      },
    }),
  ],
};
```

## React 使用 Vue 組件

### 方法 1: 簡單封裝

```typescript
// src/components/WorkflowEditorWrapper.tsx
import React, { useEffect, useRef, useState } from 'react';
import { createApp } from 'vue';
import type { App as VueApp } from 'vue';
import type { WorkflowData } from '@/types/workflow';

interface Props {
  workflowId: string;
  mode?: 'edit' | 'view';
  onSave?: (data: WorkflowData) => void;
  onError?: (error: Error) => void;
}

const WorkflowEditorWrapper: React.FC<Props> = ({ workflowId, mode = 'edit', onSave, onError }) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const vueAppRef = useRef<VueApp | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    let mounted = true;

    // 動態載入 Vue Remote Module
    import('workflowEditor/WorkflowEditor')
      .then(({ default: WorkflowEditor }) => {
        if (!mounted || !containerRef.current) return;

        // 創建 Vue 應用實例
        vueAppRef.current = createApp(WorkflowEditor, {
          workflowId,
          mode,
          onSave: (data: WorkflowData) => {
            console.log('[WorkflowEditor] Save event:', data);
            onSave?.(data);
          },
          onError: (err: Error) => {
            console.error('[WorkflowEditor] Error:', err);
            setError(err);
            onError?.(err);
          },
        });

        vueAppRef.current.mount(containerRef.current);
        setIsLoading(false);
      })
      .catch((err) => {
        console.error('[WorkflowEditor] Failed to load:', err);
        setError(err);
        onError?.(err);
        setIsLoading(false);
      });

    return () => {
      mounted = false;
      if (vueAppRef.current) {
        vueAppRef.current.unmount();
        vueAppRef.current = null;
      }
    };
  }, [workflowId, mode]);

  if (error) {
    return (
      <div className="workflow-editor-error">
        <h3>工作流編輯器載入失敗</h3>
        <p>{error.message}</p>
        <button onClick={() => window.location.reload()}>重新載入</button>
      </div>
    );
  }

  if (isLoading) {
    return <div className="workflow-editor-loading">載入中...</div>;
  }

  return (
    <div
      ref={containerRef}
      className="workflow-editor-container"
      style={{ width: '100%', height: '100vh' }}
    />
  );
};

export default WorkflowEditorWrapper;
```

### 方法 2: React Hook 封裝

```typescript
// src/hooks/useVueComponent.ts
import { useEffect, useRef, useState } from 'react';
import { createApp, type App as VueApp, type Component } from 'vue';

interface UseVueComponentOptions<T> {
  props: T;
  onMounted?: () => void;
  onUnmounted?: () => void;
}

export function useVueComponent<T extends Record<string, any>>(
  loader: () => Promise<{ default: Component }>,
  options: UseVueComponentOptions<T>
) {
  const containerRef = useRef<HTMLDivElement>(null);
  const vueAppRef = useRef<VueApp | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    let mounted = true;

    loader()
      .then(({ default: VueComponent }) => {
        if (!mounted || !containerRef.current) return;

        vueAppRef.current = createApp(VueComponent, options.props);
        vueAppRef.current.mount(containerRef.current);
        setIsLoading(false);
        options.onMounted?.();
      })
      .catch((err) => {
        if (!mounted) return;
        setError(err);
        setIsLoading(false);
      });

    return () => {
      mounted = false;
      if (vueAppRef.current) {
        vueAppRef.current.unmount();
        vueAppRef.current = null;
        options.onUnmounted?.();
      }
    };
  }, []);

  return { containerRef, isLoading, error };
}

// 使用示例
const WorkflowPage: React.FC = () => {
  const { containerRef, isLoading, error } = useVueComponent(
    () => import('workflowEditor/WorkflowEditor'),
    {
      props: {
        workflowId: 'wf-123',
        onSave: (data) => console.log('Saved:', data),
      },
      onMounted: () => console.log('Vue component mounted'),
    }
  );

  if (error) return <ErrorDisplay error={error} />;
  if (isLoading) return <Loading />;

  return <div ref={containerRef} style={{ width: '100%', height: '100vh' }} />;
};
```

## 狀態同步 (Redux ↔ Pinia)

### Event Bus 實現

```typescript
// shared/eventBus.ts
import mitt, { type Emitter } from 'mitt';

type Events = {
  'workflow:updated': WorkflowData;
  'node:selected': string;
  'execution:started': string;
  'execution:completed': { workflowId: string; result: any };
};

export const eventBus: Emitter<Events> = mitt<Events>();

// React 中使用
import { eventBus } from '@shared/eventBus';

useEffect(() => {
  const handler = (data: WorkflowData) => {
    dispatch(updateWorkflow(data));
  };

  eventBus.on('workflow:updated', handler);
  return () => {
    eventBus.off('workflow:updated', handler);
  };
}, [dispatch]);

// Vue 中使用
import { eventBus } from '@shared/eventBus';
import { onMounted, onUnmounted } from 'vue';

onMounted(() => {
  eventBus.on('workflow:updated', (data) => {
    workflowStore.updateWorkflow(data);
  });
});

onUnmounted(() => {
  eventBus.off('workflow:updated');
});
```

## 共享 Design Tokens

### CSS Variables

```css
/* shared/design-tokens/tokens.css */
:root {
  /* Colors */
  --color-primary: #1976d2;
  --color-secondary: #dc004e;
  --color-success: #2e7d32;
  --color-error: #d32f2f;
  --color-background: #ffffff;
  --color-surface: #f5f5f5;

  /* Spacing (8px 系統) */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;

  /* n8n Workflow 專用 (16px 網格) */
  --grid-size: 16px;
  --node-size-default: 96px;
  --node-size-config: 80px;
  --node-size-configurable: 256px;

  /* Typography */
  --font-family: 'Roboto', 'Noto Sans TC', sans-serif;
  --font-size-sm: 12px;
  --font-size-md: 14px;
  --font-size-lg: 16px;
}
```

### React 中使用

```tsx
const StyledComponent = styled.div`
  background-color: var(--color-background);
  padding: var(--spacing-md);
  font-family: var(--font-family);
`;
```

### Vue 中使用

```vue
<style scoped>
.workflow-canvas {
  background-color: var(--color-background);
  padding: var(--spacing-md);
  font-family: var(--font-family);
}

.workflow-node {
  width: var(--node-size-default);
  height: var(--node-size-default);
}
</style>
```

## 部署策略

### Development

```bash
# Terminal 1: Vue Remote
cd packages/workflow-editor
npm run dev  # Port 3001

# Terminal 2: React Host
cd packages/frontend-host
npm run dev  # Port 3000
```

### Production

#### Option A: 同域部署

```
https://app.example.com/                    # React Host
https://app.example.com/workflow-editor/    # Vue Remote
```

#### Option B: CDN 部署

```
https://app.example.com/                    # React Host
https://cdn.example.com/workflow-editor/    # Vue Remote (CDN)
```

## 錯誤處理

### React Error Boundary

```typescript
// src/components/WorkflowEditorErrorBoundary.tsx
class WorkflowEditorErrorBoundary extends React.Component<Props, State> {
  state = { hasError: false, error: null };

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    console.error('[Workflow Editor Error]:', error, info);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="workflow-editor-error">
          <h3>工作流編輯器載入失敗</h3>
          <p>{this.state.error?.message}</p>
          <button onClick={() => window.location.reload()}>重新載入</button>
        </div>
      );
    }

    return this.props.children;
  }
}

// 使用
<WorkflowEditorErrorBoundary>
  <Suspense fallback={<Loading />}>
    <WorkflowEditorWrapper workflowId="wf-123" />
  </Suspense>
</WorkflowEditorErrorBoundary>
```

---

**參考資源**:
- [Webpack Module Federation 官方文檔](https://webpack.js.org/concepts/module-federation/)
- [微前端架構最佳實踐](https://microfrontends.com/)

**最後更新**: 2025-11-01
**狀態**: ✅ 已驗證
```

3. **更新 `frontend-react-structure.md`**

在文件末尾添加:

```markdown
## Module Federation 整合

本 React 應用作為 **Host**,整合 Vue 工作流編輯器 (Remote Module)。

### 整合配置

詳見:
- [Vue 工作流編輯器結構](./frontend-vue-structure.md)
- [Module Federation 實施指南](./module-federation-guide.md)

### WorkflowEditor 使用

```typescript
// src/pages/Workflows/WorkflowEdit.tsx
import WorkflowEditorWrapper from '@/components/WorkflowEditorWrapper';

export function WorkflowEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const handleSave = (data: WorkflowData) => {
    // 保存到後端
    apiClient.put(`/api/workflows/${id}`, data);
    navigate('/workflows');
  };

  return (
    <div className="workflow-edit-page">
      <WorkflowEditorWrapper
        workflowId={id}
        mode="edit"
        onSave={handleSave}
      />
    </div>
  );
}
```

---

**下一步**: 查看 [Vue 編輯器結構文檔](./frontend-vue-structure.md)
```

**更新文件**:
- 創建 `docs/technical-implementation/3-project-structure/frontend-vue-structure.md`
- 創建 `docs/technical-implementation/3-project-structure/module-federation-guide.md`
- 更新 `docs/technical-implementation/3-project-structure/frontend-react-structure.md`

---

## 實施優先級

### 🔴 高優先級 (立即執行)

1. **創建 ADR-012** - 記錄架構決策
   - 文件: `docs/architecture/ADR-012-workflow-editor-technology.md`
   - 更新 ADD 主文檔

2. **更新 README.md** - 項目根目錄
   - 技術棧章節
   - 快速開始命令

3. **更新 Technical Implementation**
   - README.md (前端技術棧表格)
   - PoC 6 驗證結果更新

4. **創建 Module Federation 文檔**
   - `frontend-vue-structure.md`
   - `module-federation-guide.md`
   - 更新 `frontend-react-structure.md`

### 🟡 中優先級 (本週完成)

5. **更新 docs/brief.md**
   - Technology Stack 章節
   - 添加微前端架構說明

6. **更新 docs/user-stories/README.md**
   - US 4.1 添加技術選型註記
   - 連結技術決策文檔

7. **更新 docs/ux-design/README.md**
   - 工具和技術章節
   - Workflow Editor 註記

### 🟢 低優先級 (視需求)

8. **創建 Coding Standards** (如需要)
   - `docs/technical-implementation/4-coding-standards/vue-coding-standards.md`

9. **更新測試策略** (如需要)
   - Vue 組件測試標準
   - Module Federation 整合測試

---

## 檢查清單

在開始實施前,確認:

- [ ] **PoC 6 驗證完成**: Vue 3 + VueFlow + Yjs CRDT 已通過測試
- [ ] **團隊共識**: Tech Lead, Frontend Architect, PO 已批准決策
- [ ] **技術決策文檔**: WORKFLOW-EDITOR-TECH-DECISION.md 已完成
- [ ] **開發環境準備**: Node.js 20, pnpm, Docker 已安裝
- [ ] **團隊培訓計劃**: Vue 3 Composition API 培訓安排 (1 週)

---

## 後續行動

### 本週任務 (Week 1)

**Day 1-2**: 高優先級文檔更新
- [x] 創建 ADR-012
- [x] 更新 README.md
- [x] 更新 Technical Implementation README

**Day 3-4**: Module Federation 文檔
- [ ] 創建 frontend-vue-structure.md
- [ ] 創建 module-federation-guide.md
- [ ] 更新 frontend-react-structure.md

**Day 5**: 中優先級文檔更新
- [ ] 更新 docs/brief.md
- [ ] 更新 docs/user-stories/README.md
- [ ] 更新 docs/ux-design/README.md

### 下週任務 (Week 2)

**實際開發準備**:
1. 設置 Monorepo 結構 (pnpm workspace)
2. 配置 Module Federation (Host + Remote)
3. 建立共享 Design Tokens
4. 創建基礎 Vue 組件 (Canvas.vue, Node.vue)
5. 實施 React ↔ Vue 通訊機制

---

## 相關文檔

- [工作流編輯器技術決策完整報告](./WORKFLOW-EDITOR-TECH-DECISION.md)
- [n8n 源代碼分析](../n8n-analysis/claudedocs/n8n-architecture-analysis.md)
- [PoC 6 驗證報告](../docs/technical-implementation/1-poc-validation/06-vueflow-crdt-collaboration.md)
- [微前端架構方案](./MODULE-FEDERATION-ARCHITECTURE-GUIDE.md) (本對話內容)

---

**文檔版本**: 1.0.0
**創建日期**: 2025-11-01
**負責人**: Tech Lead + Frontend Architect
**狀態**: ✅ 已規劃,待執行

🤖 Generated with [Claude Code](https://claude.com/claude-code)
