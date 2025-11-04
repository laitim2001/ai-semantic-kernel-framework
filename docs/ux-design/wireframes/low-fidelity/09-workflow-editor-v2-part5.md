# Workflow Editor V2 - Part 5: 畫布互動與連接線（貝塞爾曲線）

> **基於 n8n 開源架構分析**
> 文檔版本：V2.5
> 創建日期：2025-01-XX
> 設計風格：完全基於 n8n 的畫布交互模式

---

## 📋 Part 5 總覽

本文檔詳細設計畫布的交互模式和連接線系統，基於 VueFlow 的能力並結合 n8n 的實際實現。

### 設計原則
1. **直觀的拖拽操作**：流暢的節點移動和連接創建
2. **貝塞爾曲線連接線**：平滑的視覺連接，自動避讓
3. **16px 網格對齊**：保持整潔的佈局
4. **響應式交互**：快速反饋，無延遲感
5. **批量操作支持**：提升大型工作流編輯效率

---

## 📚 目錄

### [1. VueFlow 畫布系統](#1-vueflow-畫布系統)
- 1.1 VueFlow 集成架構
- 1.2 畫布配置
- 1.3 視口控制

### [2. 畫布交互模式](#2-畫布交互模式)
- 2.1 拖拽模式
- 2.2 縮放模式
- 2.3 平移模式
- 2.4 選擇模式

### [3. 貝塞爾曲線連接線](#3-貝塞爾曲線連接線)
- 3.1 連接線類型
- 3.2 貝塞爾曲線算法
- 3.3 連接線樣式
- 3.4 連接線動畫

### [4. 連接創建流程](#4-連接創建流程)
- 4.1 拖拽端口連接
- 4.2 連接驗證
- 4.3 連接反饋
- 4.4 連接完成

### [5. 節點對齊與吸附](#5-節點對齊與吸附)
- 5.1 16px 網格系統
- 5.2 對齊線提示
- 5.3 智能吸附
- 5.4 分佈對齊

### [6. 批量操作](#6-批量操作)
- 6.1 多選機制
- 6.2 批量移動
- 6.3 批量複製
- 6.4 批量刪除
- 6.5 群組管理

### [7. 撤銷/重做機制](#7-撤銷重做機制)
- 7.1 Command Pattern
- 7.2 歷史棧管理
- 7.3 操作記錄
- 7.4 性能優化

### [8. 鍵盤快捷鍵](#8-鍵盤快捷鍵)
- 8.1 快捷鍵映射
- 8.2 組合鍵
- 8.3 快捷鍵提示

---

## 1. VueFlow 畫布系統

### 1.1 VueFlow 集成架構

n8n 使用 VueFlow (@vue-flow/core) 作為畫布引擎，這是一個基於 Vue 3 的流程圖庫。

```typescript
// packages/frontend/editor-ui/src/components/canvas/WorkflowCanvas.vue

<template>
  <div class="workflow-canvas">
    <VueFlow
      ref="vueFlowRef"
      :nodes="nodes"
      :edges="edges"
      :default-viewport="defaultViewport"
      :min-zoom="0.1"
      :max-zoom="2"
      :snap-to-grid="true"
      :snap-grid="[16, 16]"
      :connection-mode="ConnectionMode.Loose"
      :default-edge-options="defaultEdgeOptions"
      :elevate-edges-on-select="true"
      :delete-key-code="['Delete', 'Backspace']"
      @nodes-change="handleNodesChange"
      @edges-change="handleEdgesChange"
      @connect="handleConnect"
      @edge-update="handleEdgeUpdate"
      @pane-click="handlePaneClick"
      @node-drag-start="handleNodeDragStart"
      @node-drag="handleNodeDrag"
      @node-drag-stop="handleNodeDragStop"
      @edge-click="handleEdgeClick"
      @viewport-change="handleViewportChange"
    >
      <!-- 自定義節點 -->
      <template #node-custom="nodeProps">
        <CanvasNode v-bind="nodeProps" />
      </template>

      <!-- 自定義連接線 -->
      <template #edge-custom="edgeProps">
        <CanvasEdge v-bind="edgeProps" />
      </template>

      <!-- 背景網格 -->
      <Background
        :pattern-color="'var(--color--foreground--tint-2)'"
        :gap="16"
        :size="1"
        variant="dots"
      />

      <!-- 縮放控制 -->
      <Controls
        :show-zoom="true"
        :show-fit-view="true"
        :show-interactive="true"
      />

      <!-- 小地圖 -->
      <MiniMap
        :node-color="getNodeColor"
        :node-stroke-color="getNodeStrokeColor"
        :mask-color="'rgba(0, 0, 0, 0.1)'"
        :pannable="true"
        :zoomable="true"
      />
    </VueFlow>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { VueFlow, useVueFlow, ConnectionMode } from '@vue-flow/core';
import { Background } from '@vue-flow/background';
import { Controls } from '@vue-flow/controls';
import { MiniMap } from '@vue-flow/minimap';
import type { Node, Edge, Connection } from '@vue-flow/core';

// VueFlow 實例引用
const vueFlowRef = ref<InstanceType<typeof VueFlow>>();
const { onConnect, addEdges, addNodes, project, vueFlowRef: flowRef } = useVueFlow();

// 畫布數據
const nodes = ref<Node[]>([]);
const edges = ref<Edge[]>([]);

// 默認視口
const defaultViewport = ref({
  x: 0,
  y: 0,
  zoom: 1,
});

// 默認連接線選項
const defaultEdgeOptions = computed(() => ({
  type: 'custom',
  animated: false,
  style: {
    strokeWidth: 2,
    stroke: 'var(--color--foreground)',
  },
}));

// 事件處理
const handleConnect = (connection: Connection) => {
  // 創建連接
  console.log('Connection created:', connection);

  // 驗證連接
  if (isValidConnection(connection)) {
    addEdges([{
      ...connection,
      id: `edge-${connection.source}-${connection.target}`,
      type: 'custom',
    }]);
  }
};

const handleNodesChange = (changes: any[]) => {
  // 節點變更（位置、選擇狀態等）
  console.log('Nodes changed:', changes);
};

const handleEdgesChange = (changes: any[]) => {
  // 連接線變更（刪除、選擇等）
  console.log('Edges changed:', changes);
};

const isValidConnection = (connection: Connection): boolean => {
  // 驗證連接規則
  // 1. 不能自連接
  if (connection.source === connection.target) {
    return false;
  }

  // 2. 不能重複連接
  const existingEdge = edges.value.find(
    edge =>
      edge.source === connection.source &&
      edge.target === connection.target &&
      edge.sourceHandle === connection.sourceHandle &&
      edge.targetHandle === connection.targetHandle
  );

  return !existingEdge;
};

// 小地圖顏色
const getNodeColor = (node: Node) => {
  const category = node.data?.category || 'default';
  const colorMap: Record<string, string> = {
    core: '#7B68EE',
    logic: '#FF6B6B',
    flow: '#4ECDC4',
    database: '#95E1D3',
    ai: '#A8E6CF',
    communication: '#FFD93D',
    file: '#F38181',
    utility: '#AA96DA',
    default: '#CCCCCC',
  };
  return colorMap[category];
};

const getNodeStrokeColor = (node: Node) => {
  return node.selected ? 'var(--color--primary)' : 'transparent';
};
</script>

<style scoped lang="scss">
.workflow-canvas {
  width: 100%;
  height: 100%;
  position: relative;
  background: var(--color--background--base);

  :deep(.vue-flow) {
    background: var(--color--background--base);
  }

  :deep(.vue-flow__background) {
    background: var(--color--background--base);
  }

  :deep(.vue-flow__controls) {
    bottom: var(--spacing--md);
    left: var(--spacing--md);

    button {
      background: var(--color--background--light);
      border: 1px solid var(--color--border--base);
      color: var(--color--text--dark);
      transition: all 0.2s ease;

      &:hover {
        background: var(--color--background--light-2);
        border-color: var(--color--primary);
      }
    }
  }

  :deep(.vue-flow__minimap) {
    bottom: var(--spacing--md);
    right: var(--spacing--md);
    border: 1px solid var(--color--border--base);
    border-radius: var(--border-radius--base);
    background: var(--color--background--light);
  }
}
</style>
```

### 1.2 畫布配置

```typescript
// packages/frontend/editor-ui/src/composables/useCanvasConfig.ts

import { computed } from 'vue';
import type { VueFlowOptions } from '@vue-flow/core';

export function useCanvasConfig() {
  // 畫布配置
  const canvasConfig = computed<Partial<VueFlowOptions>>(() => ({
    // 縮放配置
    minZoom: 0.1,
    maxZoom: 2,
    defaultZoom: 1,

    // 網格配置
    snapToGrid: true,
    snapGrid: [16, 16], // 16px 網格對齊

    // 連接配置
    connectionMode: 'loose', // 允許鬆散連接（不需要精確對準）
    connectionRadius: 30,    // 連接檢測半徑

    // 選擇配置
    multiSelectionKeyCode: 'Meta', // Mac: Cmd, Windows: Ctrl
    selectionKeyCode: 'Shift',      // Shift 鍵多選
    deleteKeyCode: ['Delete', 'Backspace'], // 刪除鍵

    // 拖拽配置
    panOnDrag: [1, 2],  // 中鍵和右鍵拖拽平移畫布
    zoomOnScroll: true, // 滾輪縮放
    zoomOnPinch: true,  // 觸控板縮放
    zoomOnDoubleClick: false, // 禁用雙擊縮放

    // 邊緣配置
    elevateEdgesOnSelect: true, // 選中連接線時提升 z-index
    defaultEdgeOptions: {
      type: 'smoothstep', // 默認使用平滑階梯連接線
      animated: false,
      style: {
        strokeWidth: 2,
        stroke: 'var(--color--foreground)',
      },
    },

    // 節點配置
    defaultNodeOptions: {
      dragHandle: '.node-header', // 只能通過標題拖拽
    },

    // 視口配置
    fitViewOnInit: false, // 初始化時不自動適應視口
    attributionPosition: 'bottom-right',
  }));

  return {
    canvasConfig,
  };
}
```

### 1.3 視口控制

```typescript
// packages/frontend/editor-ui/src/composables/useViewportControl.ts

import { ref, computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Viewport } from '@vue-flow/core';

export function useViewportControl() {
  const { setViewport, getViewport, fitView, zoomIn, zoomOut, zoomTo } = useVueFlow();

  // 當前視口狀態
  const viewport = ref<Viewport>(getViewport());

  // 縮放級別（百分比）
  const zoomPercentage = computed(() => Math.round(viewport.value.zoom * 100));

  // 重置視口
  const resetViewport = () => {
    setViewport({ x: 0, y: 0, zoom: 1 }, { duration: 300 });
  };

  // 適應所有節點
  const fitAllNodes = () => {
    fitView({
      padding: 0.2, // 20% 邊距
      duration: 300,
      maxZoom: 1.5,
      minZoom: 0.5,
    });
  };

  // 居中選中的節點
  const centerSelectedNodes = () => {
    const selectedNodes = nodes.value.filter(node => node.selected);
    if (selectedNodes.length === 0) return;

    // 計算選中節點的邊界框
    const bounds = getNodesBounds(selectedNodes);
    const center = {
      x: bounds.x + bounds.width / 2,
      y: bounds.y + bounds.height / 2,
    };

    // 計算視口中心點
    const viewportCenter = {
      x: window.innerWidth / 2,
      y: window.innerHeight / 2,
    };

    // 移動視口使選中節點居中
    setViewport({
      x: viewportCenter.x - center.x * viewport.value.zoom,
      y: viewportCenter.y - center.y * viewport.value.zoom,
      zoom: viewport.value.zoom,
    }, { duration: 300 });
  };

  // 縮放到指定百分比
  const setZoomPercentage = (percentage: number) => {
    const zoom = percentage / 100;
    zoomTo(zoom, { duration: 200 });
  };

  // 自定義縮放步進
  const zoomInStep = () => {
    const currentZoom = viewport.value.zoom;
    const newZoom = Math.min(currentZoom * 1.2, 2);
    zoomTo(newZoom, { duration: 200 });
  };

  const zoomOutStep = () => {
    const currentZoom = viewport.value.zoom;
    const newZoom = Math.max(currentZoom / 1.2, 0.1);
    zoomTo(newZoom, { duration: 200 });
  };

  // 監聽視口變化
  const handleViewportChange = (newViewport: Viewport) => {
    viewport.value = newViewport;
  };

  return {
    viewport,
    zoomPercentage,
    resetViewport,
    fitAllNodes,
    centerSelectedNodes,
    setZoomPercentage,
    zoomInStep,
    zoomOutStep,
    handleViewportChange,
  };
}

// 輔助函數：獲取節點邊界框
function getNodesBounds(nodes: Node[]): { x: number; y: number; width: number; height: number } {
  if (nodes.length === 0) {
    return { x: 0, y: 0, width: 0, height: 0 };
  }

  let minX = Infinity;
  let minY = Infinity;
  let maxX = -Infinity;
  let maxY = -Infinity;

  for (const node of nodes) {
    const x = node.position.x;
    const y = node.position.y;
    const width = node.dimensions?.width || 96;
    const height = node.dimensions?.height || 96;

    minX = Math.min(minX, x);
    minY = Math.min(minY, y);
    maxX = Math.max(maxX, x + width);
    maxY = Math.max(maxY, y + height);
  }

  return {
    x: minX,
    y: minY,
    width: maxX - minX,
    height: maxY - minY,
  };
}
```

---

## 2. 畫布交互模式

### 2.1 拖拽模式

#### 節點拖拽

```typescript
// packages/frontend/editor-ui/src/composables/useNodeDrag.ts

import { ref } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

export function useNodeDrag() {
  const { updateNode, getSelectedNodes } = useVueFlow();

  // 拖拽狀態
  const isDragging = ref(false);
  const dragStartPosition = ref<{ x: number; y: number } | null>(null);

  // 開始拖拽
  const handleNodeDragStart = (event: any) => {
    isDragging.value = true;
    dragStartPosition.value = {
      x: event.node.position.x,
      y: event.node.position.y,
    };

    // 如果拖拽的節點沒有被選中，清除其他選擇並選中該節點
    if (!event.node.selected) {
      const nodes = getNodes();
      nodes.forEach(node => {
        updateNode(node.id, { selected: node.id === event.node.id });
      });
    }
  };

  // 拖拽中
  const handleNodeDrag = (event: any) => {
    if (!isDragging.value) return;

    // 獲取所有選中的節點
    const selectedNodes = getSelectedNodes();

    // 如果有多個選中的節點，同步移動
    if (selectedNodes.length > 1) {
      const deltaX = event.node.position.x - dragStartPosition.value!.x;
      const deltaY = event.node.position.y - dragStartPosition.value!.y;

      selectedNodes.forEach(node => {
        if (node.id !== event.node.id) {
          updateNode(node.id, {
            position: {
              x: node.position.x + deltaX,
              y: node.position.y + deltaY,
            },
          });
        }
      });

      // 更新起始位置
      dragStartPosition.value = {
        x: event.node.position.x,
        y: event.node.position.y,
      };
    }
  };

  // 結束拖拽
  const handleNodeDragStop = (event: any) => {
    isDragging.value = false;
    dragStartPosition.value = null;

    // 對齊到網格
    snapToGrid(event.node.id);

    // 記錄到歷史（撤銷/重做）
    recordNodeMove(event.node.id, dragStartPosition.value, event.node.position);
  };

  // 對齊到網格
  const snapToGrid = (nodeId: string) => {
    const node = getNode(nodeId);
    if (!node) return;

    const gridSize = 16;
    const snappedX = Math.round(node.position.x / gridSize) * gridSize;
    const snappedY = Math.round(node.position.y / gridSize) * gridSize;

    updateNode(nodeId, {
      position: { x: snappedX, y: snappedY },
    });
  };

  return {
    isDragging,
    handleNodeDragStart,
    handleNodeDrag,
    handleNodeDragStop,
  };
}
```

#### 從面板拖拽節點

```typescript
// packages/frontend/editor-ui/src/composables/useNodePanelDrag.ts

import { ref } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { NodeTypeDefinition } from '@/types';

export function useNodePanelDrag() {
  const { addNodes, project, vueFlowRef } = useVueFlow();

  // 拖拽中的節點類型
  const draggingNodeType = ref<NodeTypeDefinition | null>(null);

  // 開始從面板拖拽
  const handlePanelDragStart = (event: DragEvent, nodeType: NodeTypeDefinition) => {
    if (!event.dataTransfer) return;

    draggingNodeType.value = nodeType;

    // 設置拖拽數據
    event.dataTransfer.effectAllowed = 'copy';
    event.dataTransfer.setData('application/vueflow-node', JSON.stringify({
      type: nodeType.type,
      label: nodeType.displayName,
    }));

    // 設置拖拽圖像
    const dragImage = createDragImage(nodeType);
    event.dataTransfer.setDragImage(dragImage, 48, 48);
  };

  // 畫布放置
  const handleCanvasDrop = (event: DragEvent) => {
    event.preventDefault();

    if (!draggingNodeType.value || !vueFlowRef.value) return;

    // 獲取放置位置（考慮視口偏移和縮放）
    const position = project({
      x: event.clientX,
      y: event.clientY,
    });

    // 對齊到網格
    const gridSize = 16;
    position.x = Math.round(position.x / gridSize) * gridSize;
    position.y = Math.round(position.y / gridSize) * gridSize;

    // 創建新節點
    const newNode: Node = {
      id: `node-${Date.now()}`,
      type: 'custom',
      position,
      data: {
        type: draggingNodeType.value.type,
        label: draggingNodeType.value.displayName,
        icon: draggingNodeType.value.icon,
        category: draggingNodeType.value.category,
        parameters: {}, // 默認參數值
      },
    };

    addNodes([newNode]);

    // 清除拖拽狀態
    draggingNodeType.value = null;

    // 記錄到歷史
    recordNodeAdd(newNode);
  };

  // 創建拖拽圖像
  const createDragImage = (nodeType: NodeTypeDefinition): HTMLElement => {
    const div = document.createElement('div');
    div.className = 'drag-image';
    div.style.width = '96px';
    div.style.height = '96px';
    div.style.background = 'var(--color--background--light)';
    div.style.border = '2px solid var(--color--primary)';
    div.style.borderRadius = 'var(--border-radius--base)';
    div.style.display = 'flex';
    div.style.alignItems = 'center';
    div.style.justifyContent = 'center';
    div.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)';

    // 添加圖標
    const icon = document.createElement('i');
    icon.className = nodeType.icon;
    icon.style.fontSize = '24px';
    div.appendChild(icon);

    // 添加到 body（必須在 DOM 中才能作為拖拽圖像）
    document.body.appendChild(div);
    setTimeout(() => document.body.removeChild(div), 0);

    return div;
  };

  return {
    draggingNodeType,
    handlePanelDragStart,
    handleCanvasDrop,
  };
}
```

### 2.2 縮放模式

```typescript
// packages/frontend/editor-ui/src/composables/useCanvasZoom.ts

import { ref, computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';

export function useCanvasZoom() {
  const { zoomTo, getViewport } = useVueFlow();

  // 縮放級別
  const zoomLevels = [0.1, 0.25, 0.5, 0.75, 1, 1.25, 1.5, 2];
  const currentZoomIndex = ref(4); // 默認 100%

  // 當前縮放百分比
  const zoomPercentage = computed(() => {
    const viewport = getViewport();
    return Math.round(viewport.zoom * 100);
  });

  // 縮放到下一個級別
  const zoomInLevel = () => {
    if (currentZoomIndex.value < zoomLevels.length - 1) {
      currentZoomIndex.value++;
      zoomTo(zoomLevels[currentZoomIndex.value], { duration: 200 });
    }
  };

  // 縮放到上一個級別
  const zoomOutLevel = () => {
    if (currentZoomIndex.value > 0) {
      currentZoomIndex.value--;
      zoomTo(zoomLevels[currentZoomIndex.value], { duration: 200 });
    }
  };

  // 縮放到指定百分比
  const zoomToPercentage = (percentage: number) => {
    const zoom = percentage / 100;
    // 找到最接近的縮放級別
    const index = zoomLevels.findIndex(level => level >= zoom);
    currentZoomIndex.value = index === -1 ? zoomLevels.length - 1 : index;
    zoomTo(zoom, { duration: 200 });
  };

  // 重置縮放
  const resetZoom = () => {
    currentZoomIndex.value = 4; // 100%
    zoomTo(1, { duration: 300 });
  };

  // 滾輪縮放處理
  const handleWheelZoom = (event: WheelEvent) => {
    if (event.ctrlKey || event.metaKey) {
      event.preventDefault();

      const delta = -event.deltaY;
      const viewport = getViewport();
      const zoomFactor = 1 + (delta > 0 ? 0.05 : -0.05);
      const newZoom = Math.max(0.1, Math.min(2, viewport.zoom * zoomFactor));

      zoomTo(newZoom, { duration: 0 }); // 無動畫，更流暢
    }
  };

  return {
    zoomPercentage,
    zoomLevels,
    currentZoomIndex,
    zoomInLevel,
    zoomOutLevel,
    zoomToPercentage,
    resetZoom,
    handleWheelZoom,
  };
}
```

### 2.3 平移模式

```typescript
// packages/frontend/editor-ui/src/composables/useCanvasPan.ts

import { ref } from 'vue';
import { useVueFlow } from '@vue-flow/core';

export function useCanvasPan() {
  const { setViewport, getViewport } = useVueFlow();

  // 平移狀態
  const isPanning = ref(false);
  const panStart = ref({ x: 0, y: 0 });

  // 開始平移（空格 + 拖拽 或 中鍵/右鍵拖拽）
  const handlePanStart = (event: MouseEvent) => {
    // 空格鍵 + 左鍵，或中鍵/右鍵
    if ((event.button === 0 && event.shiftKey) || event.button === 1 || event.button === 2) {
      event.preventDefault();
      isPanning.value = true;
      panStart.value = { x: event.clientX, y: event.clientY };

      // 改變游標
      document.body.style.cursor = 'grabbing';
    }
  };

  // 平移中
  const handlePanMove = (event: MouseEvent) => {
    if (!isPanning.value) return;

    event.preventDefault();

    const viewport = getViewport();
    const deltaX = event.clientX - panStart.value.x;
    const deltaY = event.clientY - panStart.value.y;

    setViewport({
      x: viewport.x + deltaX,
      y: viewport.y + deltaY,
      zoom: viewport.zoom,
    }, { duration: 0 });

    panStart.value = { x: event.clientX, y: event.clientY };
  };

  // 結束平移
  const handlePanEnd = () => {
    if (!isPanning.value) return;

    isPanning.value = false;
    document.body.style.cursor = '';
  };

  // 鍵盤平移（方向鍵）
  const handleKeyboardPan = (event: KeyboardEvent) => {
    const panDistance = 50; // 每次平移 50px
    const viewport = getViewport();

    let newX = viewport.x;
    let newY = viewport.y;

    switch (event.key) {
      case 'ArrowLeft':
        newX += panDistance;
        break;
      case 'ArrowRight':
        newX -= panDistance;
        break;
      case 'ArrowUp':
        newY += panDistance;
        break;
      case 'ArrowDown':
        newY -= panDistance;
        break;
      default:
        return;
    }

    event.preventDefault();
    setViewport({ x: newX, y: newY, zoom: viewport.zoom }, { duration: 200 });
  };

  return {
    isPanning,
    handlePanStart,
    handlePanMove,
    handlePanEnd,
    handleKeyboardPan,
  };
}
```

### 2.4 選擇模式

```typescript
// packages/frontend/editor-ui/src/composables/useCanvasSelection.ts

import { ref, computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

export function useCanvasSelection() {
  const { getNodes, getEdges, updateNode, updateEdge } = useVueFlow();

  // 選擇框
  const selectionBox = ref<{
    startX: number;
    startY: number;
    endX: number;
    endY: number;
    active: boolean;
  }>({
    startX: 0,
    startY: 0,
    endX: 0,
    endY: 0,
    active: false,
  });

  // 選中的節點
  const selectedNodes = computed(() => {
    return getNodes().filter(node => node.selected);
  });

  // 選中的連接線
  const selectedEdges = computed(() => {
    return getEdges().filter(edge => edge.selected);
  });

  // 開始框選
  const handleSelectionStart = (event: MouseEvent) => {
    // 只在畫布空白區域按下左鍵時開始框選
    if (event.button === 0 && event.target === event.currentTarget) {
      selectionBox.value = {
        startX: event.clientX,
        startY: event.clientY,
        endX: event.clientX,
        endY: event.clientY,
        active: true,
      };
    }
  };

  // 框選中
  const handleSelectionMove = (event: MouseEvent) => {
    if (!selectionBox.value.active) return;

    selectionBox.value.endX = event.clientX;
    selectionBox.value.endY = event.clientY;

    // 更新選中狀態
    updateSelectionByBox();
  };

  // 結束框選
  const handleSelectionEnd = () => {
    if (!selectionBox.value.active) return;

    selectionBox.value.active = false;
    updateSelectionByBox();
  };

  // 根據框選區域更新選中狀態
  const updateSelectionByBox = () => {
    const { startX, startY, endX, endY } = selectionBox.value;

    const minX = Math.min(startX, endX);
    const maxX = Math.max(startX, endX);
    const minY = Math.min(startY, endY);
    const maxY = Math.max(startY, endY);

    const nodes = getNodes();
    nodes.forEach(node => {
      const nodeX = node.position.x;
      const nodeY = node.position.y;
      const nodeWidth = node.dimensions?.width || 96;
      const nodeHeight = node.dimensions?.height || 96;

      // 檢查節點是否在框選區域內
      const isInside =
        nodeX >= minX &&
        nodeX + nodeWidth <= maxX &&
        nodeY >= minY &&
        nodeY + nodeHeight <= maxY;

      updateNode(node.id, { selected: isInside });
    });
  };

  // 全選
  const selectAll = () => {
    const nodes = getNodes();
    nodes.forEach(node => {
      updateNode(node.id, { selected: true });
    });

    const edges = getEdges();
    edges.forEach(edge => {
      updateEdge(edge.id, { selected: true });
    });
  };

  // 取消全選
  const deselectAll = () => {
    const nodes = getNodes();
    nodes.forEach(node => {
      updateNode(node.id, { selected: false });
    });

    const edges = getEdges();
    edges.forEach(edge => {
      updateEdge(edge.id, { selected: false });
    });
  };

  // 反選
  const invertSelection = () => {
    const nodes = getNodes();
    nodes.forEach(node => {
      updateNode(node.id, { selected: !node.selected });
    });
  };

  // 點擊節點選擇（支持 Cmd/Ctrl 多選）
  const handleNodeClick = (event: MouseEvent, node: Node) => {
    if (event.metaKey || event.ctrlKey) {
      // 多選模式：切換選中狀態
      updateNode(node.id, { selected: !node.selected });
    } else {
      // 單選模式：取消其他選擇
      const nodes = getNodes();
      nodes.forEach(n => {
        updateNode(n.id, { selected: n.id === node.id });
      });
    }
  };

  return {
    selectionBox,
    selectedNodes,
    selectedEdges,
    handleSelectionStart,
    handleSelectionMove,
    handleSelectionEnd,
    selectAll,
    deselectAll,
    invertSelection,
    handleNodeClick,
  };
}
```

---

## 3. 貝塞爾曲線連接線

### 3.1 連接線類型

n8n 使用多種連接線類型，根據場景選擇：

```typescript
// packages/frontend/editor-ui/src/types/edges.ts

export enum EdgeType {
  SMOOTH_STEP = 'smoothstep',    // 平滑階梯（主要使用）
  BEZIER = 'bezier',              // 貝塞爾曲線
  STRAIGHT = 'straight',          // 直線
  STEP = 'step',                  // 階梯
  CUSTOM = 'custom',              // 自定義
}

export interface CanvasEdgeData {
  // 連接信息
  sourceNodeId: string;
  targetNodeId: string;
  sourceHandle?: string;
  targetHandle?: string;

  // 視覺狀態
  type: EdgeType;
  animated: boolean;
  status?: 'success' | 'error' | 'running' | 'pinned';

  // 數據流
  dataCount?: number;
  executionTime?: number;

  // 樣式
  style?: {
    stroke?: string;
    strokeWidth?: number;
    strokeDasharray?: string;
  };

  // 標籤
  label?: string;
  labelStyle?: Record<string, any>;
  labelShowBg?: boolean;
  labelBgStyle?: Record<string, any>;
}
```

### 3.2 貝塞爾曲線算法

```typescript
// packages/frontend/editor-ui/src/utils/bezier.ts

/**
 * 計算三次貝塞爾曲線的 SVG 路徑
 * @param sourceX 起點 X
 * @param sourceY 起點 Y
 * @param targetX 終點 X
 * @param targetY 終點 Y
 * @param sourcePosition 起點位置（left/right/top/bottom）
 * @param targetPosition 終點位置
 */
export function getBezierPath({
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition = 'right',
  targetPosition = 'left',
  curvature = 0.25,
}: {
  sourceX: number;
  sourceY: number;
  targetX: number;
  targetY: number;
  sourcePosition?: 'left' | 'right' | 'top' | 'bottom';
  targetPosition?: 'left' | 'right' | 'top' | 'bottom';
  curvature?: number;
}): string {
  // 計算控制點
  const [controlPointSource, controlPointTarget] = getControlPoints({
    sourceX,
    sourceY,
    targetX,
    targetY,
    sourcePosition,
    targetPosition,
    curvature,
  });

  // 生成 SVG 路徑
  return `M ${sourceX},${sourceY} C ${controlPointSource.x},${controlPointSource.y} ${controlPointTarget.x},${controlPointTarget.y} ${targetX},${targetY}`;
}

/**
 * 計算貝塞爾曲線的控制點
 */
function getControlPoints({
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition,
  targetPosition,
  curvature,
}: any) {
  // 計算起點和終點之間的距離
  const distance = Math.sqrt(
    Math.pow(targetX - sourceX, 2) + Math.pow(targetY - sourceY, 2)
  );

  // 控制點距離（基於曲率）
  const controlOffset = distance * curvature;

  // 根據端口位置計算控制點
  let controlPointSource = { x: sourceX, y: sourceY };
  let controlPointTarget = { x: targetX, y: targetY };

  switch (sourcePosition) {
    case 'right':
      controlPointSource.x += controlOffset;
      break;
    case 'left':
      controlPointSource.x -= controlOffset;
      break;
    case 'top':
      controlPointSource.y -= controlOffset;
      break;
    case 'bottom':
      controlPointSource.y += controlOffset;
      break;
  }

  switch (targetPosition) {
    case 'right':
      controlPointTarget.x += controlOffset;
      break;
    case 'left':
      controlPointTarget.x -= controlOffset;
      break;
    case 'top':
      controlPointTarget.y -= controlOffset;
      break;
    case 'bottom':
      controlPointTarget.y += controlOffset;
      break;
  }

  return [controlPointSource, controlPointTarget];
}

/**
 * 平滑階梯路徑（n8n 主要使用）
 */
export function getSmoothStepPath({
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition = 'right',
  targetPosition = 'left',
  borderRadius = 8,
}: {
  sourceX: number;
  sourceY: number;
  targetX: number;
  targetY: number;
  sourcePosition?: 'left' | 'right' | 'top' | 'bottom';
  targetPosition?: 'left' | 'right' | 'top' | 'bottom';
  borderRadius?: number;
}): string {
  const [sourceOffsetX, sourceOffsetY] = getOffset(sourcePosition, borderRadius);
  const [targetOffsetX, targetOffsetY] = getOffset(targetPosition, borderRadius);

  const centerX = (sourceX + targetX) / 2;
  const centerY = (sourceY + targetY) / 2;

  // 水平優先路徑
  if (sourcePosition === 'right' && targetPosition === 'left') {
    return `
      M ${sourceX},${sourceY}
      L ${centerX - borderRadius},${sourceY}
      Q ${centerX},${sourceY} ${centerX},${sourceY + (targetY > sourceY ? borderRadius : -borderRadius)}
      L ${centerX},${targetY + (targetY > sourceY ? -borderRadius : borderRadius)}
      Q ${centerX},${targetY} ${centerX + borderRadius},${targetY}
      L ${targetX},${targetY}
    `.trim().replace(/\s+/g, ' ');
  }

  // 垂直優先路徑
  if (sourcePosition === 'bottom' && targetPosition === 'top') {
    return `
      M ${sourceX},${sourceY}
      L ${sourceX},${centerY - borderRadius}
      Q ${sourceX},${centerY} ${sourceX + (targetX > sourceX ? borderRadius : -borderRadius)},${centerY}
      L ${targetX + (targetX > sourceX ? -borderRadius : borderRadius)},${centerY}
      Q ${targetX},${centerY} ${targetX},${centerY + borderRadius}
      L ${targetX},${targetY}
    `.trim().replace(/\s+/g, ' ');
  }

  // 默認貝塞爾曲線
  return getBezierPath({
    sourceX,
    sourceY,
    targetX,
    targetY,
    sourcePosition,
    targetPosition,
  });
}

function getOffset(position: string, radius: number): [number, number] {
  switch (position) {
    case 'right':
      return [radius, 0];
    case 'left':
      return [-radius, 0];
    case 'top':
      return [0, -radius];
    case 'bottom':
      return [0, radius];
    default:
      return [0, 0];
  }
}
```

### 3.3 連接線樣式

```vue
<!-- packages/frontend/editor-ui/src/components/canvas/CanvasEdge.vue -->

<template>
  <g class="canvas-edge">
    <!-- 主路徑 -->
    <path
      :d="edgePath"
      :class="['edge-path', `status-${status}`]"
      :style="edgeStyle"
      :marker-end="markerEnd"
    />

    <!-- 數據計數標籤 -->
    <EdgeDataCount
      v-if="dataCount !== undefined"
      :data-count="dataCount"
      :edge-path="edgePath"
    />

    <!-- 懸停區域（更大的點擊區域） -->
    <path
      :d="edgePath"
      class="edge-interaction"
      :style="interactionStyle"
      @mouseenter="handleMouseEnter"
      @mouseleave="handleMouseLeave"
      @click="handleClick"
    />

    <!-- 刪除按鈕（懸停時顯示） -->
    <EdgeDeleteButton
      v-if="showDelete"
      :edge-path="edgePath"
      @delete="handleDelete"
    />
  </g>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { getSmoothStepPath, getBezierPath } from '@/utils/bezier';
import type { CanvasEdgeData } from '@/types/edges';

interface Props {
  id: string;
  source: string;
  target: string;
  sourceX: number;
  sourceY: number;
  targetX: number;
  targetY: number;
  sourcePosition?: 'left' | 'right' | 'top' | 'bottom';
  targetPosition?: 'left' | 'right' | 'top' | 'bottom';
  data?: CanvasEdgeData;
  selected?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  sourcePosition: 'right',
  targetPosition: 'left',
  selected: false,
});

// 懸停狀態
const isHovered = ref(false);
const showDelete = computed(() => isHovered.value || props.selected);

// 狀態
const status = computed(() => props.data?.status || 'default');
const dataCount = computed(() => props.data?.dataCount);

// 連接線路徑
const edgePath = computed(() => {
  if (props.data?.type === 'bezier') {
    return getBezierPath({
      sourceX: props.sourceX,
      sourceY: props.sourceY,
      targetX: props.targetX,
      targetY: props.targetY,
      sourcePosition: props.sourcePosition,
      targetPosition: props.targetPosition,
    });
  }

  // 默認使用平滑階梯
  return getSmoothStepPath({
    sourceX: props.sourceX,
    sourceY: props.sourceY,
    targetX: props.targetX,
    targetY: props.targetY,
    sourcePosition: props.sourcePosition,
    targetPosition: props.targetPosition,
    borderRadius: 8,
  });
});

// 連接線樣式
const edgeStyle = computed(() => {
  const baseStyle = {
    strokeWidth: props.selected ? '3px' : '2px',
    transition: 'all 0.2s ease',
  };

  // 根據狀態設置顏色
  const statusColors: Record<string, string> = {
    success: 'var(--color--success)',
    error: 'var(--color--danger)',
    running: 'var(--color--primary)',
    pinned: 'var(--color--secondary)',
    default: 'var(--color--foreground)',
  };

  const stroke = statusColors[status.value] || statusColors.default;

  // 合併自定義樣式
  return {
    ...baseStyle,
    stroke,
    ...props.data?.style,
  };
});

// 交互區域樣式（透明但更大）
const interactionStyle = computed(() => ({
  stroke: 'transparent',
  strokeWidth: '20px',
  fill: 'none',
  cursor: 'pointer',
}));

// 箭頭標記
const markerEnd = computed(() => {
  return props.selected ? 'url(#arrowhead-selected)' : 'url(#arrowhead)';
});

// 事件處理
const handleMouseEnter = () => {
  isHovered.value = true;
};

const handleMouseLeave = () => {
  isHovered.value = false;
};

const handleClick = (event: MouseEvent) => {
  event.stopPropagation();
  // 觸發選中事件
  emit('select', props.id);
};

const handleDelete = () => {
  emit('delete', props.id);
};

const emit = defineEmits<{
  select: [id: string];
  delete: [id: string];
}>();
</script>

<style scoped lang="scss">
.canvas-edge {
  .edge-path {
    fill: none;
    transition: all 0.2s ease;

    &.status-success {
      stroke: var(--color--success);
    }

    &.status-error {
      stroke: var(--color--danger);
      stroke-dasharray: 8 4;
    }

    &.status-running {
      stroke: var(--color--primary);
      animation: edge-pulse 2s ease-in-out infinite;
    }

    &.status-pinned {
      stroke: var(--color--secondary);
      stroke-width: 3px;
    }
  }

  .edge-interaction {
    &:hover + .edge-path {
      stroke-width: 3px;
      filter: brightness(1.2);
    }
  }
}

@keyframes edge-pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.6;
  }
}
</style>
```

### 3.4 連接線動畫

```scss
// 數據流動畫
@keyframes edge-flow {
  0% {
    stroke-dashoffset: 24;
  }
  100% {
    stroke-dashoffset: 0;
  }
}

.edge-path.animated {
  stroke-dasharray: 8 8;
  animation: edge-flow 0.5s linear infinite;
}

// 執行中的光點動畫
@keyframes edge-dot-flow {
  0% {
    offset-distance: 0%;
  }
  100% {
    offset-distance: 100%;
  }
}

.edge-execution-dot {
  offset-path: path(var(--edge-path));
  animation: edge-dot-flow 2s ease-in-out infinite;
}
```

---

## 4. 連接創建流程

### 4.1 拖拽端口連接

```vue
<!-- packages/frontend/editor-ui/src/components/canvas/NodeHandle.vue -->

<template>
  <Handle
    :id="id"
    :type="type"
    :position="position"
    :class="['node-handle', `type-${type}`, { connecting: isConnecting }]"
    :style="handleStyle"
    @mouseenter="handleMouseEnter"
    @mouseleave="handleMouseLeave"
  >
    <!-- 連接點圓圈 -->
    <div class="handle-circle" />

    <!-- 懸停提示 -->
    <Transition name="fade">
      <div v-if="showTooltip" class="handle-tooltip">
        {{ tooltipText }}
      </div>
    </Transition>
  </Handle>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { Handle, useVueFlow } from '@vue-flow/core';
import type { Position } from '@vue-flow/core';

interface Props {
  id: string;
  type: 'source' | 'target';
  position: Position;
  label?: string;
  connectionCount?: number;
  maxConnections?: number;
}

const props = withDefaults(defineProps<Props>(), {
  maxConnections: Infinity,
});

const { connectionStartHandle, connectionEndHandle } = useVueFlow();

// 懸停狀態
const isHovered = ref(false);
const showTooltip = computed(() => isHovered.value && !isConnecting.value);

// 是否正在連接
const isConnecting = computed(() => {
  return (
    connectionStartHandle.value?.handleId === props.id ||
    connectionEndHandle.value?.handleId === props.id
  );
});

// 是否可以連接
const canConnect = computed(() => {
  if (props.maxConnections === Infinity) return true;
  return (props.connectionCount || 0) < props.maxConnections;
});

// 端口樣式
const handleStyle = computed(() => {
  if (!canConnect.value) {
    return {
      cursor: 'not-allowed',
      opacity: 0.3,
    };
  }

  return {
    cursor: 'crosshair',
  };
});

// 提示文字
const tooltipText = computed(() => {
  if (!canConnect.value) {
    return `最多 ${props.maxConnections} 個連接`;
  }

  if (props.type === 'source') {
    return '拖拽創建連接';
  } else {
    return '連接到此節點';
  }
});

// 事件處理
const handleMouseEnter = () => {
  isHovered.value = true;
};

const handleMouseLeave = () => {
  isHovered.value = false;
};
</script>

<style scoped lang="scss">
.node-handle {
  width: 12px;
  height: 12px;
  background: var(--color--background--base);
  border: 2px solid var(--color--foreground);
  border-radius: 50%;
  transition: all 0.2s ease;
  z-index: 10;

  &.type-source {
    right: -6px;
  }

  &.type-target {
    left: -6px;
  }

  &:hover {
    width: 16px;
    height: 16px;
    border-width: 3px;
    border-color: var(--color--primary);

    .handle-circle {
      transform: scale(1.2);
      background: var(--color--primary);
    }
  }

  &.connecting {
    width: 16px;
    height: 16px;
    border-width: 3px;
    border-color: var(--color--primary);
    box-shadow: 0 0 0 4px var(--color--primary--tint-3);
  }

  .handle-circle {
    width: 100%;
    height: 100%;
    border-radius: 50%;
    background: var(--color--foreground);
    transition: all 0.2s ease;
  }

  .handle-tooltip {
    position: absolute;
    top: -36px;
    left: 50%;
    transform: translateX(-50%);
    padding: var(--spacing--2xs) var(--spacing--xs);
    background: var(--color--text--dark);
    color: var(--color--background--base);
    font-size: 12px;
    white-space: nowrap;
    border-radius: var(--border-radius--base);
    pointer-events: none;
    z-index: 100;

    &::after {
      content: '';
      position: absolute;
      bottom: -4px;
      left: 50%;
      transform: translateX(-50%);
      width: 0;
      height: 0;
      border-left: 4px solid transparent;
      border-right: 4px solid transparent;
      border-top: 4px solid var(--color--text--dark);
    }
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
```

### 4.2 連接驗證

```typescript
// packages/frontend/editor-ui/src/composables/useConnectionValidation.ts

import { computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Connection } from '@vue-flow/core';

export function useConnectionValidation() {
  const { getNode, getEdges } = useVueFlow();

  /**
   * 驗證連接是否有效
   */
  const isValidConnection = (connection: Connection): {
    valid: boolean;
    reason?: string;
  } => {
    // 1. 檢查源節點和目標節點是否存在
    const sourceNode = getNode(connection.source);
    const targetNode = getNode(connection.target);

    if (!sourceNode || !targetNode) {
      return {
        valid: false,
        reason: '節點不存在',
      };
    }

    // 2. 不能自連接
    if (connection.source === connection.target) {
      return {
        valid: false,
        reason: '不能連接到自己',
      };
    }

    // 3. 檢查是否已存在相同的連接
    const edges = getEdges();
    const duplicateEdge = edges.find(
      edge =>
        edge.source === connection.source &&
        edge.target === connection.target &&
        edge.sourceHandle === connection.sourceHandle &&
        edge.targetHandle === connection.targetHandle
    );

    if (duplicateEdge) {
      return {
        valid: false,
        reason: '連接已存在',
      };
    }

    // 4. 檢查是否會形成循環（可選，根據需求）
    if (wouldCreateCycle(connection)) {
      return {
        valid: false,
        reason: '不能創建循環連接',
      };
    }

    // 5. 檢查目標端口的最大連接數
    const targetHandle = targetNode.data?.inputs?.find(
      (input: any) => input.name === connection.targetHandle
    );

    if (targetHandle?.maxConnections) {
      const existingConnections = edges.filter(
        edge =>
          edge.target === connection.target &&
          edge.targetHandle === connection.targetHandle
      );

      if (existingConnections.length >= targetHandle.maxConnections) {
        return {
          valid: false,
          reason: `目標端口最多允許 ${targetHandle.maxConnections} 個連接`,
        };
      }
    }

    // 6. 檢查節點類型兼容性（可選）
    const sourceType = sourceNode.data?.type;
    const targetType = targetNode.data?.type;

    if (!areTypesCompatible(sourceType, targetType)) {
      return {
        valid: false,
        reason: '節點類型不兼容',
      };
    }

    return { valid: true };
  };

  /**
   * 檢查是否會創建循環
   */
  const wouldCreateCycle = (connection: Connection): boolean => {
    const edges = getEdges();

    // 使用 DFS 檢測循環
    const visited = new Set<string>();
    const recursionStack = new Set<string>();

    const hasCycle = (nodeId: string): boolean => {
      visited.add(nodeId);
      recursionStack.add(nodeId);

      // 獲取所有從當前節點出發的連接
      const outgoingEdges = edges.filter(edge => edge.source === nodeId);

      // 添加即將創建的連接
      if (nodeId === connection.source) {
        outgoingEdges.push({
          source: connection.source,
          target: connection.target,
        } as any);
      }

      for (const edge of outgoingEdges) {
        if (!visited.has(edge.target)) {
          if (hasCycle(edge.target)) {
            return true;
          }
        } else if (recursionStack.has(edge.target)) {
          // 找到循環
          return true;
        }
      }

      recursionStack.delete(nodeId);
      return false;
    };

    return hasCycle(connection.source);
  };

  /**
   * 檢查節點類型是否兼容
   */
  const areTypesCompatible = (sourceType: string, targetType: string): boolean => {
    // 這裡可以定義特定的類型兼容性規則
    // 例如：某些節點類型不能連接到特定類型

    // 簡單示例：所有類型都兼容
    return true;
  };

  return {
    isValidConnection,
  };
}
```

### 4.3 連接反饋

```vue
<!-- packages/frontend/editor-ui/src/components/canvas/ConnectionLine.vue -->

<template>
  <!-- 正在創建的連接線預覽 -->
  <g v-if="isConnecting" class="connection-line-preview">
    <path
      :d="connectionPath"
      class="connection-path"
      :class="{ valid: isValid, invalid: !isValid }"
    />

    <!-- 目標指示器 -->
    <circle
      v-if="targetPosition"
      :cx="targetPosition.x"
      :cy="targetPosition.y"
      r="8"
      class="connection-target-indicator"
      :class="{ valid: isValid, invalid: !isValid }"
    />

    <!-- 錯誤提示 -->
    <foreignObject
      v-if="!isValid && validationError"
      :x="targetPosition?.x || 0"
      :y="(targetPosition?.y || 0) + 20"
      width="200"
      height="40"
    >
      <div class="connection-error-tooltip">
        {{ validationError }}
      </div>
    </foreignObject>
  </g>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import { getSmoothStepPath } from '@/utils/bezier';
import { useConnectionValidation } from '@/composables/useConnectionValidation';

const {
  connectionStartHandle,
  connectionEndHandle,
  connectionPosition,
} = useVueFlow();

const { isValidConnection } = useConnectionValidation();

// 是否正在連接
const isConnecting = computed(() => !!connectionStartHandle.value);

// 起點位置
const sourcePosition = computed(() => {
  if (!connectionStartHandle.value) return null;

  const handle = document.querySelector(
    `[data-handleid="${connectionStartHandle.value.handleId}"]`
  );

  if (!handle) return null;

  const rect = handle.getBoundingClientRect();
  return {
    x: rect.left + rect.width / 2,
    y: rect.top + rect.height / 2,
  };
});

// 終點位置
const targetPosition = computed(() => {
  if (!connectionPosition.value) return null;

  return {
    x: connectionPosition.value.x,
    y: connectionPosition.value.y,
  };
});

// 連接路徑
const connectionPath = computed(() => {
  if (!sourcePosition.value || !targetPosition.value) return '';

  return getSmoothStepPath({
    sourceX: sourcePosition.value.x,
    sourceY: sourcePosition.value.y,
    targetX: targetPosition.value.x,
    targetY: targetPosition.value.y,
    sourcePosition: 'right',
    targetPosition: 'left',
  });
});

// 驗證連接
const connectionValidation = computed(() => {
  if (!connectionStartHandle.value || !connectionEndHandle.value) {
    return { valid: true };
  }

  return isValidConnection({
    source: connectionStartHandle.value.nodeId,
    sourceHandle: connectionStartHandle.value.handleId,
    target: connectionEndHandle.value.nodeId,
    targetHandle: connectionEndHandle.value.handleId,
  });
});

const isValid = computed(() => connectionValidation.value.valid);
const validationError = computed(() => connectionValidation.value.reason);
</script>

<style scoped lang="scss">
.connection-line-preview {
  pointer-events: none;

  .connection-path {
    fill: none;
    stroke-width: 3px;
    stroke-dasharray: 8 4;
    animation: dash 0.5s linear infinite;

    &.valid {
      stroke: var(--color--primary);
    }

    &.invalid {
      stroke: var(--color--danger);
    }
  }

  .connection-target-indicator {
    fill: none;
    stroke-width: 2px;

    &.valid {
      stroke: var(--color--primary);
      fill: var(--color--primary--tint-3);
    }

    &.invalid {
      stroke: var(--color--danger);
      fill: var(--color--danger--tint-4);
    }
  }

  .connection-error-tooltip {
    padding: var(--spacing--2xs) var(--spacing--xs);
    background: var(--color--danger);
    color: white;
    font-size: 12px;
    border-radius: var(--border-radius--base);
    white-space: nowrap;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
  }
}

@keyframes dash {
  to {
    stroke-dashoffset: -12;
  }
}
</style>
```

### 4.4 連接完成

```typescript
// packages/frontend/editor-ui/src/composables/useConnectionComplete.ts

import { useVueFlow } from '@vue-flow/core';
import { useCommandHistory } from '@/composables/useCommandHistory';
import type { Connection, Edge } from '@vue-flow/core';

export function useConnectionComplete() {
  const { addEdges } = useVueFlow();
  const { execute } = useCommandHistory();

  /**
   * 完成連接創建
   */
  const completeConnection = (connection: Connection) => {
    // 創建新連接線
    const newEdge: Edge = {
      id: `edge-${connection.source}-${connection.target}-${Date.now()}`,
      source: connection.source,
      sourceHandle: connection.sourceHandle,
      target: connection.target,
      targetHandle: connection.targetHandle,
      type: 'custom',
      animated: false,
      data: {
        sourceNodeId: connection.source,
        targetNodeId: connection.target,
        status: 'default',
      },
    };

    // 使用命令模式添加連接（支持撤銷）
    execute({
      execute: () => {
        addEdges([newEdge]);
        return newEdge;
      },
      undo: (edge: Edge) => {
        removeEdge(edge.id);
      },
      description: `創建連接: ${connection.source} → ${connection.target}`,
    });

    // 顯示成功提示
    showSuccessToast('連接已創建');

    // 自動保存工作流
    autoSaveWorkflow();
  };

  return {
    completeConnection,
  };
}
```

---

## 5. 節點對齊與吸附

### 5.1 16px 網格系統

```typescript
// packages/frontend/editor-ui/src/composables/useGridSnap.ts

import { ref } from 'vue';

export function useGridSnap() {
  const GRID_SIZE = 16;

  // 網格吸附開關
  const snapToGrid = ref(true);

  /**
   * 將坐標吸附到網格
   */
  const snapToGridPosition = (position: { x: number; y: number }) => {
    if (!snapToGrid.value) return position;

    return {
      x: Math.round(position.x / GRID_SIZE) * GRID_SIZE,
      y: Math.round(position.y / GRID_SIZE) * GRID_SIZE,
    };
  };

  /**
   * 切換網格吸附
   */
  const toggleSnapToGrid = () => {
    snapToGrid.value = !snapToGrid.value;
  };

  return {
    GRID_SIZE,
    snapToGrid,
    snapToGridPosition,
    toggleSnapToGrid,
  };
}
```

### 5.2 對齊線提示

```vue
<!-- packages/frontend/editor-ui/src/components/canvas/AlignmentGuides.vue -->

<template>
  <svg class="alignment-guides">
    <!-- 水平對齊線 -->
    <line
      v-for="(line, index) in horizontalLines"
      :key="`h-${index}`"
      :x1="0"
      :y1="line.y"
      :x2="canvasWidth"
      :y2="line.y"
      class="guide-line horizontal"
    />

    <!-- 垂直對齊線 -->
    <line
      v-for="(line, index) in verticalLines"
      :key="`v-${index}`"
      :x1="line.x"
      :y1="0"
      :x2="line.x"
      :y2="canvasHeight"
      class="guide-line vertical"
    />

    <!-- 距離標註 -->
    <g
      v-for="(distance, index) in distanceLabels"
      :key="`d-${index}`"
      class="distance-label"
    >
      <rect
        :x="distance.x - 20"
        :y="distance.y - 10"
        width="40"
        height="20"
        rx="4"
        fill="var(--color--primary)"
      />
      <text
        :x="distance.x"
        :y="distance.y + 4"
        text-anchor="middle"
        fill="white"
        font-size="12"
      >
        {{ distance.value }}px
      </text>
    </g>
  </svg>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

const { getNodes, getViewport } = useVueFlow();

const canvasWidth = ref(window.innerWidth);
const canvasHeight = ref(window.innerHeight);

// 對齊閾值（像素）
const ALIGNMENT_THRESHOLD = 8;

// 水平對齊線
const horizontalLines = computed(() => {
  const lines: Array<{ y: number }> = [];
  const selectedNodes = getNodes().filter(n => n.selected);
  const otherNodes = getNodes().filter(n => !n.selected);

  if (selectedNodes.length === 0) return lines;

  // 檢查每個選中節點與其他節點的對齊
  for (const selectedNode of selectedNodes) {
    const selectedY = selectedNode.position.y;
    const selectedCenterY = selectedY + (selectedNode.dimensions?.height || 96) / 2;

    for (const otherNode of otherNodes) {
      const otherY = otherNode.position.y;
      const otherCenterY = otherY + (otherNode.dimensions?.height || 96) / 2;

      // 頂部對齊
      if (Math.abs(selectedY - otherY) < ALIGNMENT_THRESHOLD) {
        lines.push({ y: otherY });
      }

      // 中心對齊
      if (Math.abs(selectedCenterY - otherCenterY) < ALIGNMENT_THRESHOLD) {
        lines.push({ y: otherCenterY });
      }

      // 底部對齊
      const selectedBottom = selectedY + (selectedNode.dimensions?.height || 96);
      const otherBottom = otherY + (otherNode.dimensions?.height || 96);
      if (Math.abs(selectedBottom - otherBottom) < ALIGNMENT_THRESHOLD) {
        lines.push({ y: otherBottom });
      }
    }
  }

  return lines;
});

// 垂直對齊線
const verticalLines = computed(() => {
  const lines: Array<{ x: number }> = [];
  const selectedNodes = getNodes().filter(n => n.selected);
  const otherNodes = getNodes().filter(n => !n.selected);

  if (selectedNodes.length === 0) return lines;

  for (const selectedNode of selectedNodes) {
    const selectedX = selectedNode.position.x;
    const selectedCenterX = selectedX + (selectedNode.dimensions?.width || 96) / 2;

    for (const otherNode of otherNodes) {
      const otherX = otherNode.position.x;
      const otherCenterX = otherX + (otherNode.dimensions?.width || 96) / 2;

      // 左側對齊
      if (Math.abs(selectedX - otherX) < ALIGNMENT_THRESHOLD) {
        lines.push({ x: otherX });
      }

      // 中心對齊
      if (Math.abs(selectedCenterX - otherCenterX) < ALIGNMENT_THRESHOLD) {
        lines.push({ x: otherCenterX });
      }

      // 右側對齊
      const selectedRight = selectedX + (selectedNode.dimensions?.width || 96);
      const otherRight = otherX + (otherNode.dimensions?.width || 96);
      if (Math.abs(selectedRight - otherRight) < ALIGNMENT_THRESHOLD) {
        lines.push({ x: otherRight });
      }
    }
  }

  return lines;
});

// 距離標註
const distanceLabels = computed(() => {
  // 顯示節點之間的距離
  const labels: Array<{ x: number; y: number; value: number }> = [];
  // TODO: 實現距離計算邏輯
  return labels;
});
</script>

<style scoped lang="scss">
.alignment-guides {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1000;

  .guide-line {
    stroke: var(--color--primary);
    stroke-width: 1px;
    stroke-dasharray: 4 4;
    opacity: 0.8;
    animation: fade-in 0.2s ease;
  }

  .distance-label {
    animation: fade-in 0.2s ease;
  }
}

@keyframes fade-in {
  from {
    opacity: 0;
  }
  to {
    opacity: 0.8;
  }
}
</style>
```

### 5.3 智能吸附

```typescript
// packages/frontend/editor-ui/src/composables/useSmartSnap.ts

import { computed } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

export function useSmartSnap() {
  const { getNodes } = useVueFlow();

  const SNAP_THRESHOLD = 16; // 吸附閾值（像素）

  /**
   * 計算節點的吸附位置
   */
  const calculateSnapPosition = (
    draggedNode: Node,
    proposedPosition: { x: number; y: number }
  ): { x: number; y: number; snapped: boolean } => {
    const otherNodes = getNodes().filter(n => n.id !== draggedNode.id);

    let snappedX = proposedPosition.x;
    let snappedY = proposedPosition.y;
    let snapped = false;

    const draggedWidth = draggedNode.dimensions?.width || 96;
    const draggedHeight = draggedNode.dimensions?.height || 96;

    // 計算拖拽節點的邊界
    const draggedLeft = proposedPosition.x;
    const draggedRight = proposedPosition.x + draggedWidth;
    const draggedTop = proposedPosition.y;
    const draggedBottom = proposedPosition.y + draggedHeight;
    const draggedCenterX = proposedPosition.x + draggedWidth / 2;
    const draggedCenterY = proposedPosition.y + draggedHeight / 2;

    // 檢查與其他節點的對齊
    for (const otherNode of otherNodes) {
      const otherWidth = otherNode.dimensions?.width || 96;
      const otherHeight = otherNode.dimensions?.height || 96;

      const otherLeft = otherNode.position.x;
      const otherRight = otherNode.position.x + otherWidth;
      const otherTop = otherNode.position.y;
      const otherBottom = otherNode.position.y + otherHeight;
      const otherCenterX = otherNode.position.x + otherWidth / 2;
      const otherCenterY = otherNode.position.y + otherHeight / 2;

      // 水平對齊檢查
      // 左對齊
      if (Math.abs(draggedLeft - otherLeft) < SNAP_THRESHOLD) {
        snappedX = otherLeft;
        snapped = true;
      }
      // 中心對齊
      else if (Math.abs(draggedCenterX - otherCenterX) < SNAP_THRESHOLD) {
        snappedX = otherCenterX - draggedWidth / 2;
        snapped = true;
      }
      // 右對齊
      else if (Math.abs(draggedRight - otherRight) < SNAP_THRESHOLD) {
        snappedX = otherRight - draggedWidth;
        snapped = true;
      }

      // 垂直對齊檢查
      // 頂部對齊
      if (Math.abs(draggedTop - otherTop) < SNAP_THRESHOLD) {
        snappedY = otherTop;
        snapped = true;
      }
      // 中心對齊
      else if (Math.abs(draggedCenterY - otherCenterY) < SNAP_THRESHOLD) {
        snappedY = otherCenterY - draggedHeight / 2;
        snapped = true;
      }
      // 底部對齊
      else if (Math.abs(draggedBottom - otherBottom) < SNAP_THRESHOLD) {
        snappedY = otherBottom - draggedHeight;
        snapped = true;
      }
    }

    return {
      x: snappedX,
      y: snappedY,
      snapped,
    };
  };

  return {
    calculateSnapPosition,
  };
}
```

### 5.4 分佈對齊

```typescript
// packages/frontend/editor-ui/src/composables/useNodeAlignment.ts

import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

export function useNodeAlignment() {
  const { getNodes, updateNode } = useVueFlow();

  /**
   * 水平對齊選中的節點
   */
  const alignHorizontally = (alignment: 'left' | 'center' | 'right') => {
    const selectedNodes = getNodes().filter(n => n.selected);
    if (selectedNodes.length < 2) return;

    let targetX: number;

    switch (alignment) {
      case 'left':
        // 對齊到最左邊的節點
        targetX = Math.min(...selectedNodes.map(n => n.position.x));
        break;

      case 'center':
        // 對齊到中心
        const minX = Math.min(...selectedNodes.map(n => n.position.x));
        const maxX = Math.max(...selectedNodes.map(n => n.position.x + (n.dimensions?.width || 96)));
        const centerX = (minX + maxX) / 2;

        selectedNodes.forEach(node => {
          const width = node.dimensions?.width || 96;
          updateNode(node.id, {
            position: {
              x: centerX - width / 2,
              y: node.position.y,
            },
          });
        });
        return;

      case 'right':
        // 對齊到最右邊的節點
        targetX = Math.max(...selectedNodes.map(n => n.position.x + (n.dimensions?.width || 96)));
        selectedNodes.forEach(node => {
          const width = node.dimensions?.width || 96;
          updateNode(node.id, {
            position: {
              x: targetX - width,
              y: node.position.y,
            },
          });
        });
        return;
    }

    selectedNodes.forEach(node => {
      updateNode(node.id, {
        position: {
          x: targetX,
          y: node.position.y,
        },
      });
    });
  };

  /**
   * 垂直對齊選中的節點
   */
  const alignVertically = (alignment: 'top' | 'middle' | 'bottom') => {
    const selectedNodes = getNodes().filter(n => n.selected);
    if (selectedNodes.length < 2) return;

    let targetY: number;

    switch (alignment) {
      case 'top':
        targetY = Math.min(...selectedNodes.map(n => n.position.y));
        break;

      case 'middle':
        const minY = Math.min(...selectedNodes.map(n => n.position.y));
        const maxY = Math.max(...selectedNodes.map(n => n.position.y + (n.dimensions?.height || 96)));
        const centerY = (minY + maxY) / 2;

        selectedNodes.forEach(node => {
          const height = node.dimensions?.height || 96;
          updateNode(node.id, {
            position: {
              x: node.position.x,
              y: centerY - height / 2,
            },
          });
        });
        return;

      case 'bottom':
        targetY = Math.max(...selectedNodes.map(n => n.position.y + (n.dimensions?.height || 96)));
        selectedNodes.forEach(node => {
          const height = node.dimensions?.height || 96;
          updateNode(node.id, {
            position: {
              x: node.position.x,
              y: targetY - height,
            },
          });
        });
        return;
    }

    selectedNodes.forEach(node => {
      updateNode(node.id, {
        position: {
          x: node.position.x,
          y: targetY,
        },
      });
    });
  };

  /**
   * 均勻分佈節點
   */
  const distributeEvenly = (direction: 'horizontal' | 'vertical') => {
    const selectedNodes = getNodes().filter(n => n.selected);
    if (selectedNodes.length < 3) return;

    // 按位置排序
    selectedNodes.sort((a, b) => {
      if (direction === 'horizontal') {
        return a.position.x - b.position.x;
      } else {
        return a.position.y - b.position.y;
      }
    });

    if (direction === 'horizontal') {
      const firstX = selectedNodes[0].position.x;
      const lastX = selectedNodes[selectedNodes.length - 1].position.x +
        (selectedNodes[selectedNodes.length - 1].dimensions?.width || 96);

      const totalGap = lastX - firstX;
      const gapSize = totalGap / (selectedNodes.length - 1);

      selectedNodes.forEach((node, index) => {
        if (index === 0 || index === selectedNodes.length - 1) return;

        updateNode(node.id, {
          position: {
            x: firstX + gapSize * index,
            y: node.position.y,
          },
        });
      });
    } else {
      const firstY = selectedNodes[0].position.y;
      const lastY = selectedNodes[selectedNodes.length - 1].position.y +
        (selectedNodes[selectedNodes.length - 1].dimensions?.height || 96);

      const totalGap = lastY - firstY;
      const gapSize = totalGap / (selectedNodes.length - 1);

      selectedNodes.forEach((node, index) => {
        if (index === 0 || index === selectedNodes.length - 1) return;

        updateNode(node.id, {
          position: {
            x: node.position.x,
            y: firstY + gapSize * index,
          },
        });
      });
    }
  };

  return {
    alignHorizontally,
    alignVertically,
    distributeEvenly,
  };
}
```

---

## 6. 批量操作

### 6.1 多選機制

已在 [2.4 選擇模式](#24-選擇模式) 中實現。

### 6.2 批量移動

已在 [2.1 拖拽模式](#21-拖拽模式) 中實現（自動支持多選拖拽）。

### 6.3 批量複製

```typescript
// packages/frontend/editor-ui/src/composables/useBatchCopy.ts

import { useVueFlow } from '@vue-flow/core';
import type { Node, Edge } from '@vue-flow/core';

export function useBatchCopy() {
  const { getNodes, getEdges, addNodes, addEdges } = useVueFlow();

  /**
   * 複製選中的節點和連接線
   */
  const copySelectedNodes = () => {
    const selectedNodes = getNodes().filter(n => n.selected);
    if (selectedNodes.length === 0) return;

    // 存儲到剪貼板
    const clipboard = {
      nodes: selectedNodes.map(node => ({
        ...node,
        id: undefined, // 複製時生成新 ID
      })),
      edges: getEdgesBetweenNodes(selectedNodes),
    };

    localStorage.setItem('workflow-clipboard', JSON.stringify(clipboard));

    showSuccessToast(`已複製 ${selectedNodes.length} 個節點`);
  };

  /**
   * 粘貼節點
   */
  const pasteNodes = () => {
    const clipboardData = localStorage.getItem('workflow-clipboard');
    if (!clipboardData) return;

    try {
      const clipboard = JSON.parse(clipboardData);

      // 生成新 ID 映射
      const idMap = new Map<string, string>();
      const newNodes: Node[] = [];

      clipboard.nodes.forEach((node: Node) => {
        const newId = `node-${Date.now()}-${Math.random()}`;
        idMap.set(node.id, newId);

        newNodes.push({
          ...node,
          id: newId,
          position: {
            x: node.position.x + 50, // 偏移 50px
            y: node.position.y + 50,
          },
          selected: true, // 選中新粘貼的節點
        });
      });

      // 更新連接線的 source 和 target
      const newEdges: Edge[] = clipboard.edges.map((edge: Edge) => ({
        ...edge,
        id: `edge-${Date.now()}-${Math.random()}`,
        source: idMap.get(edge.source) || edge.source,
        target: idMap.get(edge.target) || edge.target,
      }));

      // 取消其他節點的選中狀態
      getNodes().forEach(node => {
        updateNode(node.id, { selected: false });
      });

      // 添加新節點和連接線
      addNodes(newNodes);
      addEdges(newEdges);

      showSuccessToast(`已粘貼 ${newNodes.length} 個節點`);
    } catch (error) {
      showErrorToast('粘貼失敗');
    }
  };

  /**
   * 獲取節點之間的連接線
   */
  const getEdgesBetweenNodes = (nodes: Node[]): Edge[] => {
    const nodeIds = new Set(nodes.map(n => n.id));
    return getEdges().filter(
      edge => nodeIds.has(edge.source) && nodeIds.has(edge.target)
    );
  };

  return {
    copySelectedNodes,
    pasteNodes,
  };
}
```

### 6.4 批量刪除

```typescript
// packages/frontend/editor-ui/src/composables/useBatchDelete.ts

import { useVueFlow } from '@vue-flow/core';

export function useBatchDelete() {
  const { getNodes, getEdges, removeNodes, removeEdges } = useVueFlow();

  /**
   * 刪除選中的節點和連接線
   */
  const deleteSelectedNodes = () => {
    const selectedNodes = getNodes().filter(n => n.selected);
    const selectedEdges = getEdges().filter(e => e.selected);

    if (selectedNodes.length === 0 && selectedEdges.length === 0) return;

    // 確認刪除
    const confirmMessage =
      selectedNodes.length > 0
        ? `確定要刪除 ${selectedNodes.length} 個節點嗎？相關連接線也會被刪除。`
        : `確定要刪除 ${selectedEdges.length} 條連接線嗎？`;

    if (!confirm(confirmMessage)) return;

    // 刪除節點（會自動刪除相關連接線）
    if (selectedNodes.length > 0) {
      const nodeIds = selectedNodes.map(n => n.id);
      removeNodes(nodeIds);
    }

    // 刪除連接線
    if (selectedEdges.length > 0) {
      const edgeIds = selectedEdges.map(e => e.id);
      removeEdges(edgeIds);
    }

    showSuccessToast('已刪除');
  };

  return {
    deleteSelectedNodes,
  };
}
```

### 6.5 群組管理

```typescript
// packages/frontend/editor-ui/src/composables/useNodeGroup.ts

import { ref } from 'vue';
import { useVueFlow } from '@vue-flow/core';
import type { Node } from '@vue-flow/core';

export interface NodeGroup {
  id: string;
  name: string;
  nodeIds: string[];
  color: string;
  collapsed: boolean;
}

export function useNodeGroup() {
  const { getNodes, updateNode } = useVueFlow();

  // 群組列表
  const groups = ref<NodeGroup[]>([]);

  /**
   * 創建群組
   */
  const createGroup = (name: string, color: string = '#7B68EE') => {
    const selectedNodes = getNodes().filter(n => n.selected);
    if (selectedNodes.length < 2) {
      showErrorToast('請選擇至少 2 個節點');
      return;
    }

    const newGroup: NodeGroup = {
      id: `group-${Date.now()}`,
      name,
      nodeIds: selectedNodes.map(n => n.id),
      color,
      collapsed: false,
    };

    groups.value.push(newGroup);

    // 更新節點的群組信息
    selectedNodes.forEach(node => {
      updateNode(node.id, {
        data: {
          ...node.data,
          groupId: newGroup.id,
        },
      });
    });

    showSuccessToast(`已創建群組: ${name}`);
  };

  /**
   * 解散群組
   */
  const disbandGroup = (groupId: string) => {
    const group = groups.value.find(g => g.id === groupId);
    if (!group) return;

    // 移除節點的群組信息
    group.nodeIds.forEach(nodeId => {
      const node = getNodes().find(n => n.id === nodeId);
      if (node) {
        updateNode(nodeId, {
          data: {
            ...node.data,
            groupId: undefined,
          },
        });
      }
    });

    // 移除群組
    groups.value = groups.value.filter(g => g.id !== groupId);

    showSuccessToast('群組已解散');
  };

  /**
   * 折疊/展開群組
   */
  const toggleGroupCollapse = (groupId: string) => {
    const group = groups.value.find(g => g.id === groupId);
    if (!group) return;

    group.collapsed = !group.collapsed;

    // 隱藏/顯示群組內的節點
    group.nodeIds.forEach(nodeId => {
      updateNode(nodeId, {
        hidden: group.collapsed,
      });
    });
  };

  return {
    groups,
    createGroup,
    disbandGroup,
    toggleGroupCollapse,
  };
}
```

---

## 7. 撤銷/重做機制 (Command Pattern)

### 7.1 Command Pattern 架構

n8n 使用經典的 Command Pattern 實現撤銷/重做功能，確保所有可逆操作都能被追蹤和還原。

#### 7.1.1 Command 接口定義

```typescript
/**
 * Command 接口
 * 所有可撤銷操作都必須實現這個接口
 */
export interface ICommand {
  /** 命令唯一標識 */
  id: string;

  /** 命令類型 */
  type: CommandType;

  /** 命令描述（用於 UI 顯示） */
  description: string;

  /** 執行命令 */
  execute(): void;

  /** 撤銷命令 */
  undo(): void;

  /** 重做命令（默認等同於 execute） */
  redo?(): void;

  /** 是否可以與後續命令合併（用於優化連續操作） */
  canMergeWith?(command: ICommand): boolean;

  /** 合併命令 */
  merge?(command: ICommand): ICommand;
}

/**
 * 命令類型枚舉
 */
export enum CommandType {
  ADD_NODE = 'ADD_NODE',
  DELETE_NODE = 'DELETE_NODE',
  MOVE_NODE = 'MOVE_NODE',
  UPDATE_NODE = 'UPDATE_NODE',

  ADD_CONNECTION = 'ADD_CONNECTION',
  DELETE_CONNECTION = 'DELETE_CONNECTION',

  BATCH = 'BATCH',  // 批量操作

  UPDATE_WORKFLOW_SETTINGS = 'UPDATE_WORKFLOW_SETTINGS',
}

/**
 * 命令歷史管理器
 */
export class CommandHistory {
  /** 撤銷堆棧 */
  private undoStack: ICommand[] = [];

  /** 重做堆棧 */
  private redoStack: ICommand[] = [];

  /** 最大歷史記錄數 */
  private readonly maxHistorySize = 50;

  /** 是否正在執行命令（防止遞歸） */
  private isExecuting = false;

  /**
   * 執行命令並加入歷史
   */
  execute(command: ICommand): void {
    if (this.isExecuting) return;

    this.isExecuting = true;
    try {
      // 執行命令
      command.execute();

      // 嘗試與上一個命令合併
      const lastCommand = this.undoStack[this.undoStack.length - 1];
      if (lastCommand?.canMergeWith?.(command)) {
        const merged = lastCommand.merge!(command);
        this.undoStack[this.undoStack.length - 1] = merged;
      } else {
        // 加入撤銷堆棧
        this.undoStack.push(command);

        // 限制堆棧大小
        if (this.undoStack.length > this.maxHistorySize) {
          this.undoStack.shift();
        }
      }

      // 清空重做堆棧（新操作會使重做無效）
      this.redoStack = [];

    } finally {
      this.isExecuting = false;
    }
  }

  /**
   * 撤銷上一個命令
   */
  undo(): boolean {
    if (this.undoStack.length === 0 || this.isExecuting) return false;

    this.isExecuting = true;
    try {
      const command = this.undoStack.pop()!;
      command.undo();
      this.redoStack.push(command);
      return true;
    } finally {
      this.isExecuting = false;
    }
  }

  /**
   * 重做上一個撤銷的命令
   */
  redo(): boolean {
    if (this.redoStack.length === 0 || this.isExecuting) return false;

    this.isExecuting = true;
    try {
      const command = this.redoStack.pop()!;
      const redoFn = command.redo || command.execute.bind(command);
      redoFn();
      this.undoStack.push(command);
      return true;
    } finally {
      this.isExecuting = false;
    }
  }

  /**
   * 清空歷史
   */
  clear(): void {
    this.undoStack = [];
    this.redoStack = [];
  }

  /**
   * 獲取可撤銷的命令描述
   */
  getUndoDescription(): string | null {
    const command = this.undoStack[this.undoStack.length - 1];
    return command?.description || null;
  }

  /**
   * 獲取可重做的命令描述
   */
  getRedoDescription(): string | null {
    const command = this.redoStack[this.redoStack.length - 1];
    return command?.description || null;
  }

  /**
   * 是否可以撤銷
   */
  canUndo(): boolean {
    return this.undoStack.length > 0;
  }

  /**
   * 是否可以重做
   */
  canRedo(): boolean {
    return this.redoStack.length > 0;
  }
}
```

#### 7.1.2 具體命令實現

**AddNodeCommand（添加節點）**

```typescript
export class AddNodeCommand implements ICommand {
  id: string;
  type = CommandType.ADD_NODE;
  description: string;

  constructor(
    private node: Node,
    private vueFlowInstance: ReturnType<typeof useVueFlow>
  ) {
    this.id = `add-node-${Date.now()}`;
    this.description = `添加節點: ${node.data.name}`;
  }

  execute(): void {
    this.vueFlowInstance.addNodes([this.node]);
  }

  undo(): void {
    this.vueFlowInstance.removeNodes([this.node.id]);
  }
}
```

**DeleteNodeCommand（刪除節點）**

```typescript
export class DeleteNodeCommand implements ICommand {
  id: string;
  type = CommandType.DELETE_NODE;
  description: string;

  /** 被刪除的節點 */
  private deletedNodes: Node[];

  /** 被刪除的連接 */
  private deletedEdges: Edge[];

  constructor(
    nodeIds: string[],
    private vueFlowInstance: ReturnType<typeof useVueFlow>
  ) {
    this.id = `delete-node-${Date.now()}`;

    const { getNodes, getEdges } = vueFlowInstance;

    // 保存被刪除的節點
    this.deletedNodes = getNodes().filter(n => nodeIds.includes(n.id));

    // 保存相關的連接
    this.deletedEdges = getEdges().filter(e =>
      nodeIds.includes(e.source) || nodeIds.includes(e.target)
    );

    this.description = `刪除 ${this.deletedNodes.length} 個節點`;
  }

  execute(): void {
    const { removeNodes } = this.vueFlowInstance;
    removeNodes(this.deletedNodes.map(n => n.id));
  }

  undo(): void {
    const { addNodes, addEdges } = this.vueFlowInstance;

    // 恢復節點
    addNodes(this.deletedNodes);

    // 恢復連接
    addEdges(this.deletedEdges);
  }
}
```

**MoveNodeCommand（移動節點）**

```typescript
export class MoveNodeCommand implements ICommand {
  id: string;
  type = CommandType.MOVE_NODE;
  description: string;

  /** 節點的舊位置 */
  private oldPositions: Map<string, { x: number; y: number }>;

  /** 節點的新位置 */
  private newPositions: Map<string, { x: number; y: number }>;

  constructor(
    private nodeIds: string[],
    oldPositions: Map<string, { x: number; y: number }>,
    newPositions: Map<string, { x: number; y: number }>,
    private vueFlowInstance: ReturnType<typeof useVueFlow>
  ) {
    this.id = `move-node-${Date.now()}`;
    this.oldPositions = oldPositions;
    this.newPositions = newPositions;

    const count = nodeIds.length;
    this.description = count === 1 ? '移動節點' : `移動 ${count} 個節點`;
  }

  execute(): void {
    const { updateNode } = this.vueFlowInstance;

    this.nodeIds.forEach(nodeId => {
      const newPos = this.newPositions.get(nodeId);
      if (newPos) {
        updateNode(nodeId, { position: newPos });
      }
    });
  }

  undo(): void {
    const { updateNode } = this.vueFlowInstance;

    this.nodeIds.forEach(nodeId => {
      const oldPos = this.oldPositions.get(nodeId);
      if (oldPos) {
        updateNode(nodeId, { position: oldPos });
      }
    });
  }

  /**
   * 連續的移動操作可以合併
   */
  canMergeWith(command: ICommand): boolean {
    if (command.type !== CommandType.MOVE_NODE) return false;

    const moveCommand = command as MoveNodeCommand;

    // 只有相同的節點集合才能合併
    if (this.nodeIds.length !== moveCommand.nodeIds.length) return false;
    if (!this.nodeIds.every(id => moveCommand.nodeIds.includes(id))) return false;

    return true;
  }

  /**
   * 合併命令：保留初始位置，更新最終位置
   */
  merge(command: ICommand): ICommand {
    const moveCommand = command as MoveNodeCommand;

    return new MoveNodeCommand(
      this.nodeIds,
      this.oldPositions,  // 保留初始位置
      moveCommand.newPositions,  // 更新為最新位置
      this.vueFlowInstance
    );
  }
}
```

**AddConnectionCommand（添加連接）**

```typescript
export class AddConnectionCommand implements ICommand {
  id: string;
  type = CommandType.ADD_CONNECTION;
  description: string;

  constructor(
    private edge: Edge,
    private vueFlowInstance: ReturnType<typeof useVueFlow>
  ) {
    this.id = `add-connection-${Date.now()}`;
    this.description = '創建連接';
  }

  execute(): void {
    this.vueFlowInstance.addEdges([this.edge]);
  }

  undo(): void {
    this.vueFlowInstance.removeEdges([this.edge.id]);
  }
}
```

**BatchCommand（批量操作）**

```typescript
export class BatchCommand implements ICommand {
  id: string;
  type = CommandType.BATCH;
  description: string;

  constructor(
    private commands: ICommand[],
    description?: string
  ) {
    this.id = `batch-${Date.now()}`;
    this.description = description || `批量操作 (${commands.length} 個)`;
  }

  execute(): void {
    this.commands.forEach(cmd => cmd.execute());
  }

  undo(): void {
    // 逆序撤銷
    [...this.commands].reverse().forEach(cmd => cmd.undo());
  }

  redo(): void {
    this.commands.forEach(cmd => {
      const redoFn = cmd.redo || cmd.execute.bind(cmd);
      redoFn();
    });
  }
}
```

### 7.2 歷史管理 Composable

```typescript
/**
 * 歷史管理 Composable
 */
export function useCommandHistory() {
  const history = new CommandHistory();
  const vueFlow = useVueFlow();

  /** 當前可撤銷的操作描述 */
  const undoDescription = computed(() => history.getUndoDescription());

  /** 當前可重做的操作描述 */
  const redoDescription = computed(() => history.getRedoDescription());

  /** 是否可以撤銷 */
  const canUndo = computed(() => history.canUndo());

  /** 是否可以重做 */
  const canRedo = computed(() => history.canRedo());

  /**
   * 添加節點（帶歷史記錄）
   */
  const addNodeWithHistory = (node: Node) => {
    const command = new AddNodeCommand(node, vueFlow);
    history.execute(command);
  };

  /**
   * 刪除節點（帶歷史記錄）
   */
  const deleteNodesWithHistory = (nodeIds: string[]) => {
    const command = new DeleteNodeCommand(nodeIds, vueFlow);
    history.execute(command);
  };

  /**
   * 移動節點（帶歷史記錄）
   */
  const moveNodesWithHistory = (
    nodeIds: string[],
    oldPositions: Map<string, { x: number; y: number }>,
    newPositions: Map<string, { x: number; y: number }>
  ) => {
    const command = new MoveNodeCommand(nodeIds, oldPositions, newPositions, vueFlow);
    history.execute(command);
  };

  /**
   * 添加連接（帶歷史記錄）
   */
  const addConnectionWithHistory = (edge: Edge) => {
    const command = new AddConnectionCommand(edge, vueFlow);
    history.execute(command);
  };

  /**
   * 批量操作（帶歷史記錄）
   */
  const executeBatch = (commands: ICommand[], description?: string) => {
    const batchCommand = new BatchCommand(commands, description);
    history.execute(batchCommand);
  };

  /**
   * 撤銷
   */
  const undo = () => {
    if (history.undo()) {
      showSuccessToast(`已撤銷: ${undoDescription.value}`);
    }
  };

  /**
   * 重做
   */
  const redo = () => {
    if (history.redo()) {
      showSuccessToast(`已重做: ${redoDescription.value}`);
    }
  };

  /**
   * 清空歷史
   */
  const clearHistory = () => {
    history.clear();
  };

  return {
    // 狀態
    undoDescription,
    redoDescription,
    canUndo,
    canRedo,

    // 操作方法
    addNodeWithHistory,
    deleteNodesWithHistory,
    moveNodesWithHistory,
    addConnectionWithHistory,
    executeBatch,

    // 歷史控制
    undo,
    redo,
    clearHistory,
  };
}
```

### 7.3 使用示例

#### 7.3.1 在節點拖拽中使用

```typescript
export function useNodeDrag() {
  const { updateNode } = useVueFlow();
  const { moveNodesWithHistory } = useCommandHistory();

  /** 拖拽開始時的位置 */
  const dragStartPositions = ref<Map<string, { x: number; y: number }>>(new Map());

  /**
   * 開始拖拽
   */
  const onNodeDragStart = (event: any) => {
    const selectedNodes = getSelectedNodes();

    // 記錄起始位置
    dragStartPositions.value.clear();
    selectedNodes.forEach(node => {
      dragStartPositions.value.set(node.id, { ...node.position });
    });
  };

  /**
   * 拖拽中（實時更新，不記錄歷史）
   */
  const onNodeDrag = (event: any) => {
    // 實時更新位置（不使用命令模式）
    updateNode(event.node.id, {
      position: event.node.position,
    });
  };

  /**
   * 拖拽結束（記錄到歷史）
   */
  const onNodeDragStop = (event: any) => {
    const selectedNodes = getSelectedNodes();

    // 記錄結束位置
    const dragEndPositions = new Map<string, { x: number; y: number }>();
    selectedNodes.forEach(node => {
      dragEndPositions.set(node.id, { ...node.position });
    });

    // 創建移動命令並加入歷史
    moveNodesWithHistory(
      selectedNodes.map(n => n.id),
      dragStartPositions.value,
      dragEndPositions
    );
  };

  return {
    onNodeDragStart,
    onNodeDrag,
    onNodeDragStop,
  };
}
```

#### 7.3.2 在複製粘貼中使用批量命令

```typescript
export function useCopyPaste() {
  const { executeBatch } = useCommandHistory();

  const pasteNodes = () => {
    const clipboard = JSON.parse(localStorage.getItem('workflow-clipboard') || '{}');

    if (!clipboard.nodes || clipboard.nodes.length === 0) {
      showWarningToast('剪貼板為空');
      return;
    }

    const commands: ICommand[] = [];
    const idMap = new Map<string, string>();

    // 創建添加節點命令
    clipboard.nodes.forEach((node: Node) => {
      const newId = `node-${Date.now()}-${Math.random()}`;
      idMap.set(node.id, newId);

      const newNode = {
        ...node,
        id: newId,
        position: { x: node.position.x + 50, y: node.position.y + 50 },
        selected: true,
      };

      commands.push(new AddNodeCommand(newNode, vueFlow));
    });

    // 創建添加連接命令
    clipboard.edges?.forEach((edge: Edge) => {
      const newEdge = {
        ...edge,
        id: `edge-${Date.now()}-${Math.random()}`,
        source: idMap.get(edge.source)!,
        target: idMap.get(edge.target)!,
      };

      commands.push(new AddConnectionCommand(newEdge, vueFlow));
    });

    // 批量執行並加入歷史
    executeBatch(commands, `粘貼 ${clipboard.nodes.length} 個節點`);

    showSuccessToast(`已粘貼 ${clipboard.nodes.length} 個節點`);
  };

  return { pasteNodes };
}
```

### 7.4 UI 組件

#### 7.4.1 撤銷/重做工具欄

```vue
<template>
  <div class="history-toolbar">
    <!-- 撤銷按鈕 -->
    <button
      class="history-btn"
      :disabled="!canUndo"
      :title="undoDescription ? `撤銷: ${undoDescription}` : '撤銷'"
      @click="undo"
    >
      <i class="icon-undo"></i>
      <span v-if="undoDescription" class="btn-label">{{ undoDescription }}</span>
    </button>

    <!-- 重做按鈕 -->
    <button
      class="history-btn"
      :disabled="!canRedo"
      :title="redoDescription ? `重做: ${redoDescription}` : '重做'"
      @click="redo"
    >
      <i class="icon-redo"></i>
      <span v-if="redoDescription" class="btn-label">{{ redoDescription }}</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useCommandHistory } from '@/composables/useCommandHistory';

const {
  canUndo,
  canRedo,
  undoDescription,
  redoDescription,
  undo,
  redo,
} = useCommandHistory();
</script>

<style scoped>
.history-toolbar {
  display: flex;
  gap: 8px;
  padding: 8px;
  background: var(--color-background-xlight);
  border-radius: 8px;
}

.history-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  background: white;
  border: 1px solid var(--color-foreground-light);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 13px;
  color: var(--color-text-dark);
}

.history-btn:hover:not(:disabled) {
  background: var(--color-foreground-xlight);
  border-color: var(--color-primary);
}

.history-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.icon-undo::before {
  content: '↶';
  font-size: 16px;
}

.icon-redo::before {
  content: '↷';
  font-size: 16px;
}

.btn-label {
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
```

---

## 8. 鍵盤快捷鍵

### 8.1 快捷鍵系統架構

n8n 實現了完整的鍵盤快捷鍵系統，支持自定義映射、衝突檢測和優先級管理。

#### 8.1.1 快捷鍵定義

```typescript
/**
 * 修飾鍵枚舉
 */
export enum ModifierKey {
  CTRL = 'ctrl',
  CMD = 'cmd',
  ALT = 'alt',
  SHIFT = 'shift',
  CTRL_OR_CMD = 'ctrlOrCmd',  // 跨平台：Windows/Linux 用 Ctrl，Mac 用 Cmd
}

/**
 * 快捷鍵配置
 */
export interface ShortcutConfig {
  /** 快捷鍵 ID */
  id: string;

  /** 快捷鍵描述 */
  description: string;

  /** 快捷鍵組合 */
  keys: {
    modifiers: ModifierKey[];
    key: string;
  };

  /** 回調函數 */
  handler: (event: KeyboardEvent) => void;

  /** 是否全局快捷鍵（默認只在畫布聚焦時生效） */
  global?: boolean;

  /** 優先級（數字越大優先級越高，默認 0） */
  priority?: number;

  /** 是否阻止默認行為 */
  preventDefault?: boolean;

  /** 是否阻止事件冒泡 */
  stopPropagation?: boolean;

  /** 條件：是否啟用（動態控制） */
  enabled?: () => boolean;
}

/**
 * 快捷鍵分類
 */
export enum ShortcutCategory {
  GENERAL = 'general',           // 通用
  CANVAS = 'canvas',             // 畫布操作
  NODE = 'node',                 // 節點操作
  SELECTION = 'selection',       // 選擇操作
  CLIPBOARD = 'clipboard',       // 剪貼板
  HISTORY = 'history',           // 歷史記錄
  VIEW = 'view',                 // 視圖控制
}
```

#### 8.1.2 快捷鍵映射表

```typescript
/**
 * n8n 默認快捷鍵映射
 */
export const DEFAULT_SHORTCUTS: Record<ShortcutCategory, ShortcutConfig[]> = {
  // ========== 通用 ==========
  [ShortcutCategory.GENERAL]: [
    {
      id: 'save-workflow',
      description: '保存工作流',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 's' },
      handler: saveWorkflow,
      global: true,
      preventDefault: true,
    },
    {
      id: 'open-search',
      description: '打開搜索',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'k' },
      handler: openSearch,
      global: true,
      preventDefault: true,
    },
    {
      id: 'toggle-sidebar',
      description: '切換側邊欄',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'b' },
      handler: toggleSidebar,
      global: true,
    },
  ],

  // ========== 畫布操作 ==========
  [ShortcutCategory.CANVAS]: [
    {
      id: 'pan-mode',
      description: '切換平移模式（按住 Space）',
      keys: { modifiers: [], key: ' ' },
      handler: enablePanMode,
    },
    {
      id: 'fit-view',
      description: '適應視圖',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: '0' },
      handler: fitView,
    },
    {
      id: 'zoom-in',
      description: '放大',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: '=' },
      handler: zoomIn,
    },
    {
      id: 'zoom-out',
      description: '縮小',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: '-' },
      handler: zoomOut,
    },
    {
      id: 'reset-zoom',
      description: '重置縮放',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: '1' },
      handler: resetZoom,
    },
  ],

  // ========== 節點操作 ==========
  [ShortcutCategory.NODE]: [
    {
      id: 'delete-node',
      description: '刪除節點',
      keys: { modifiers: [], key: 'Delete' },
      handler: deleteSelectedNodes,
      enabled: () => hasSelectedNodes(),
    },
    {
      id: 'delete-node-backspace',
      description: '刪除節點（Backspace）',
      keys: { modifiers: [], key: 'Backspace' },
      handler: deleteSelectedNodes,
      enabled: () => hasSelectedNodes(),
    },
    {
      id: 'duplicate-node',
      description: '複製節點',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'd' },
      handler: duplicateSelectedNodes,
      enabled: () => hasSelectedNodes(),
      preventDefault: true,
    },
    {
      id: 'disable-node',
      description: '啟用/禁用節點',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: '/' },
      handler: toggleNodeDisabled,
      enabled: () => hasSelectedNodes(),
    },
    {
      id: 'rename-node',
      description: '重命名節點',
      keys: { modifiers: [], key: 'F2' },
      handler: renameNode,
      enabled: () => hasExactlyOneSelectedNode(),
    },
    {
      id: 'open-node-settings',
      description: '打開節點設置',
      keys: { modifiers: [], key: 'Enter' },
      handler: openNodeSettings,
      enabled: () => hasExactlyOneSelectedNode(),
    },
  ],

  // ========== 選擇操作 ==========
  [ShortcutCategory.SELECTION]: [
    {
      id: 'select-all',
      description: '全選',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'a' },
      handler: selectAllNodes,
      preventDefault: true,
    },
    {
      id: 'deselect-all',
      description: '取消全選',
      keys: { modifiers: [], key: 'Escape' },
      handler: deselectAllNodes,
    },
  ],

  // ========== 剪貼板 ==========
  [ShortcutCategory.CLIPBOARD]: [
    {
      id: 'copy',
      description: '複製',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'c' },
      handler: copySelectedNodes,
      enabled: () => hasSelectedNodes(),
    },
    {
      id: 'cut',
      description: '剪切',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'x' },
      handler: cutSelectedNodes,
      enabled: () => hasSelectedNodes(),
    },
    {
      id: 'paste',
      description: '粘貼',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'v' },
      handler: pasteNodes,
    },
  ],

  // ========== 歷史記錄 ==========
  [ShortcutCategory.HISTORY]: [
    {
      id: 'undo',
      description: '撤銷',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'z' },
      handler: undo,
      enabled: () => canUndo(),
    },
    {
      id: 'redo',
      description: '重做',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD, ModifierKey.SHIFT], key: 'z' },
      handler: redo,
      enabled: () => canRedo(),
    },
    {
      id: 'redo-alt',
      description: '重做（替代）',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD], key: 'y' },
      handler: redo,
      enabled: () => canRedo(),
    },
  ],

  // ========== 視圖控制 ==========
  [ShortcutCategory.VIEW]: [
    {
      id: 'toggle-grid',
      description: '切換網格',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD, ModifierKey.ALT], key: 'g' },
      handler: toggleGrid,
    },
    {
      id: 'toggle-minimap',
      description: '切換小地圖',
      keys: { modifiers: [ModifierKey.CTRL_OR_CMD, ModifierKey.ALT], key: 'm' },
      handler: toggleMinimap,
    },
  ],
};
```

### 8.2 快捷鍵管理器

```typescript
/**
 * 快捷鍵管理器
 */
export class ShortcutManager {
  /** 已註冊的快捷鍵 */
  private shortcuts = new Map<string, ShortcutConfig>();

  /** 當前按下的修飾鍵 */
  private pressedModifiers = new Set<ModifierKey>();

  /** 是否為 Mac 平台 */
  private readonly isMac = navigator.platform.toUpperCase().indexOf('MAC') >= 0;

  constructor() {
    this.setupEventListeners();
  }

  /**
   * 設置事件監聽器
   */
  private setupEventListeners(): void {
    // 監聽 keydown 事件
    document.addEventListener('keydown', this.handleKeyDown.bind(this), true);

    // 監聽 keyup 事件
    document.addEventListener('keyup', this.handleKeyUp.bind(this), true);

    // 失去焦點時清空修飾鍵狀態
    window.addEventListener('blur', () => {
      this.pressedModifiers.clear();
    });
  }

  /**
   * 註冊快捷鍵
   */
  register(config: ShortcutConfig): void {
    const key = this.getShortcutKey(config.keys);

    // 檢查衝突
    if (this.shortcuts.has(key)) {
      const existing = this.shortcuts.get(key)!;
      console.warn(
        `快捷鍵衝突: ${key}`,
        `\n  已存在: ${existing.description}`,
        `\n  新註冊: ${config.description}`
      );
    }

    this.shortcuts.set(key, config);
  }

  /**
   * 批量註冊快捷鍵
   */
  registerBatch(configs: ShortcutConfig[]): void {
    configs.forEach(config => this.register(config));
  }

  /**
   * 取消註冊快捷鍵
   */
  unregister(id: string): void {
    for (const [key, config] of this.shortcuts.entries()) {
      if (config.id === id) {
        this.shortcuts.delete(key);
        break;
      }
    }
  }

  /**
   * 獲取快捷鍵標識
   */
  private getShortcutKey(keys: { modifiers: ModifierKey[]; key: string }): string {
    const modifiers = [...keys.modifiers].sort();
    return `${modifiers.join('+')}+${keys.key.toLowerCase()}`;
  }

  /**
   * 處理 keydown 事件
   */
  private handleKeyDown(event: KeyboardEvent): void {
    // 更新修飾鍵狀態
    this.updateModifiers(event);

    // 忽略在輸入框中的按鍵
    const target = event.target as HTMLElement;
    if (
      target.tagName === 'INPUT' ||
      target.tagName === 'TEXTAREA' ||
      target.isContentEditable
    ) {
      // 只處理全局快捷鍵
      return this.handleGlobalShortcuts(event);
    }

    // 處理快捷鍵
    this.handleShortcut(event);
  }

  /**
   * 處理 keyup 事件
   */
  private handleKeyUp(event: KeyboardEvent): void {
    this.updateModifiers(event);
  }

  /**
   * 更新修飾鍵狀態
   */
  private updateModifiers(event: KeyboardEvent): void {
    this.pressedModifiers.clear();

    if (event.ctrlKey) this.pressedModifiers.add(ModifierKey.CTRL);
    if (event.metaKey) this.pressedModifiers.add(ModifierKey.CMD);
    if (event.altKey) this.pressedModifiers.add(ModifierKey.ALT);
    if (event.shiftKey) this.pressedModifiers.add(ModifierKey.SHIFT);
  }

  /**
   * 處理快捷鍵
   */
  private handleShortcut(event: KeyboardEvent): void {
    // 構建當前按鍵組合
    const currentKey = this.getCurrentKey(event);

    // 查找匹配的快捷鍵（優先級從高到低）
    const matchedShortcuts = Array.from(this.shortcuts.values())
      .filter(config => {
        const configKey = this.getShortcutKey(config.keys);
        return configKey === currentKey;
      })
      .sort((a, b) => (b.priority || 0) - (a.priority || 0));

    // 執行第一個匹配且啟用的快捷鍵
    for (const config of matchedShortcuts) {
      if (config.enabled && !config.enabled()) continue;

      if (config.preventDefault) event.preventDefault();
      if (config.stopPropagation) event.stopPropagation();

      config.handler(event);
      break;
    }
  }

  /**
   * 處理全局快捷鍵
   */
  private handleGlobalShortcuts(event: KeyboardEvent): void {
    const currentKey = this.getCurrentKey(event);

    const globalShortcuts = Array.from(this.shortcuts.values())
      .filter(config => config.global)
      .filter(config => {
        const configKey = this.getShortcutKey(config.keys);
        return configKey === currentKey;
      })
      .sort((a, b) => (b.priority || 0) - (a.priority || 0));

    for (const config of globalShortcuts) {
      if (config.enabled && !config.enabled()) continue;

      if (config.preventDefault) event.preventDefault();
      if (config.stopPropagation) event.stopPropagation();

      config.handler(event);
      break;
    }
  }

  /**
   * 獲取當前按鍵組合
   */
  private getCurrentKey(event: KeyboardEvent): string {
    const modifiers: ModifierKey[] = [];

    // 處理跨平台修飾鍵
    if (this.pressedModifiers.has(ModifierKey.CTRL) ||
        this.pressedModifiers.has(ModifierKey.CMD)) {
      modifiers.push(ModifierKey.CTRL_OR_CMD);
    }

    if (this.pressedModifiers.has(ModifierKey.ALT)) {
      modifiers.push(ModifierKey.ALT);
    }

    if (this.pressedModifiers.has(ModifierKey.SHIFT)) {
      modifiers.push(ModifierKey.SHIFT);
    }

    return this.getShortcutKey({
      modifiers,
      key: event.key,
    });
  }

  /**
   * 獲取快捷鍵的顯示文本
   */
  getShortcutLabel(config: ShortcutConfig): string {
    const parts: string[] = [];

    config.keys.modifiers.forEach(modifier => {
      switch (modifier) {
        case ModifierKey.CTRL:
          parts.push('Ctrl');
          break;
        case ModifierKey.CMD:
          parts.push('⌘');
          break;
        case ModifierKey.CTRL_OR_CMD:
          parts.push(this.isMac ? '⌘' : 'Ctrl');
          break;
        case ModifierKey.ALT:
          parts.push(this.isMac ? '⌥' : 'Alt');
          break;
        case ModifierKey.SHIFT:
          parts.push(this.isMac ? '⇧' : 'Shift');
          break;
      }
    });

    // 主鍵
    let key = config.keys.key;
    if (key === ' ') key = 'Space';
    else if (key.length === 1) key = key.toUpperCase();
    parts.push(key);

    return parts.join(this.isMac ? '' : '+');
  }

  /**
   * 獲取所有快捷鍵（按分類）
   */
  getAllShortcuts(): Map<ShortcutCategory, ShortcutConfig[]> {
    const categorized = new Map<ShortcutCategory, ShortcutConfig[]>();

    Object.entries(DEFAULT_SHORTCUTS).forEach(([category, shortcuts]) => {
      categorized.set(category as ShortcutCategory, shortcuts);
    });

    return categorized;
  }
}
```

### 8.3 快捷鍵 Composable

```typescript
/**
 * 快捷鍵 Composable
 */
export function useShortcuts() {
  const manager = new ShortcutManager();

  // 註冊所有默認快捷鍵
  Object.values(DEFAULT_SHORTCUTS).forEach(shortcuts => {
    manager.registerBatch(shortcuts);
  });

  /**
   * 註冊自定義快捷鍵
   */
  const registerShortcut = (config: ShortcutConfig) => {
    manager.register(config);
  };

  /**
   * 取消註冊快捷鍵
   */
  const unregisterShortcut = (id: string) => {
    manager.unregister(id);
  };

  /**
   * 獲取快捷鍵顯示文本
   */
  const getShortcutLabel = (config: ShortcutConfig): string => {
    return manager.getShortcutLabel(config);
  };

  /**
   * 獲取所有快捷鍵
   */
  const getAllShortcuts = (): Map<ShortcutCategory, ShortcutConfig[]> => {
    return manager.getAllShortcuts();
  };

  return {
    registerShortcut,
    unregisterShortcut,
    getShortcutLabel,
    getAllShortcuts,
  };
}
```

### 8.4 快捷鍵幫助面板

```vue
<template>
  <div v-if="isVisible" class="shortcut-help-overlay" @click="close">
    <div class="shortcut-help-panel" @click.stop>
      <!-- 標題 -->
      <div class="panel-header">
        <h2>鍵盤快捷鍵</h2>
        <button class="close-btn" @click="close">×</button>
      </div>

      <!-- 快捷鍵列表 -->
      <div class="panel-content">
        <div
          v-for="[category, shortcuts] in categorizedShortcuts"
          :key="category"
          class="shortcut-category"
        >
          <h3>{{ getCategoryName(category) }}</h3>

          <div class="shortcut-list">
            <div
              v-for="shortcut in shortcuts"
              :key="shortcut.id"
              class="shortcut-item"
            >
              <span class="shortcut-description">{{ shortcut.description }}</span>
              <kbd class="shortcut-keys">{{ getShortcutLabel(shortcut) }}</kbd>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useShortcuts } from '@/composables/useShortcuts';
import { ShortcutCategory } from '@/types/shortcuts';

const isVisible = ref(false);
const { getAllShortcuts, getShortcutLabel } = useShortcuts();

const categorizedShortcuts = computed(() => getAllShortcuts());

const open = () => {
  isVisible.value = true;
};

const close = () => {
  isVisible.value = false;
};

const getCategoryName = (category: ShortcutCategory): string => {
  const names: Record<ShortcutCategory, string> = {
    [ShortcutCategory.GENERAL]: '通用',
    [ShortcutCategory.CANVAS]: '畫布操作',
    [ShortcutCategory.NODE]: '節點操作',
    [ShortcutCategory.SELECTION]: '選擇操作',
    [ShortcutCategory.CLIPBOARD]: '剪貼板',
    [ShortcutCategory.HISTORY]: '歷史記錄',
    [ShortcutCategory.VIEW]: '視圖控制',
  };
  return names[category];
};

// 監聽 ? 鍵打開幫助面板
document.addEventListener('keydown', (event) => {
  if (event.key === '?' && !isVisible.value) {
    event.preventDefault();
    open();
  }
});

defineExpose({ open, close });
</script>

<style scoped>
.shortcut-help-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
  animation: fadeIn 0.2s;
}

.shortcut-help-panel {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
  animation: slideUp 0.3s;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid var(--color-foreground-light);
}

.panel-header h2 {
  margin: 0;
  font-size: 20px;
  color: var(--color-text-dark);
}

.close-btn {
  background: none;
  border: none;
  font-size: 32px;
  color: var(--color-text-light);
  cursor: pointer;
  line-height: 1;
  padding: 0;
  width: 32px;
  height: 32px;
}

.close-btn:hover {
  color: var(--color-text-dark);
}

.panel-content {
  overflow-y: auto;
  padding: 24px;
}

.shortcut-category {
  margin-bottom: 32px;
}

.shortcut-category:last-child {
  margin-bottom: 0;
}

.shortcut-category h3 {
  margin: 0 0 16px 0;
  font-size: 14px;
  font-weight: 600;
  color: var(--color-text-base);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.shortcut-list {
  display: grid;
  gap: 12px;
}

.shortcut-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 16px;
  background: var(--color-background-xlight);
  border-radius: 6px;
}

.shortcut-description {
  font-size: 14px;
  color: var(--color-text-dark);
}

.shortcut-keys {
  display: inline-flex;
  gap: 4px;
  padding: 4px 8px;
  background: white;
  border: 1px solid var(--color-foreground-base);
  border-radius: 4px;
  font-size: 12px;
  font-family: 'SF Mono', Monaco, 'Cascadia Code', monospace;
  color: var(--color-text-dark);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
```

---

## 總結

Part 5 完整涵蓋了 n8n Workflow Editor V2 的畫布互動與連接線系統：

### ✅ 已完成的 8 個章節：

1. **VueFlow 畫布系統**
   - 使用 @vue-flow/core 作為畫布引擎
   - 16px 網格系統配置
   - 縮放、平移、多選等基礎功能

2. **畫布交互模式**
   - 節點拖拽（單選/多選）
   - 視圖縮放（離散級別：0.1-2）
   - 平移控制（Space 鍵 + 拖拽）
   - 選擇模式（框選、Cmd/Ctrl 多選）

3. **貝塞爾曲線連接線**
   - `getBezierPath()` 和 `getSmoothStepPath()` 實現
   - SmoothStep 為 n8n 主要連接線風格
   - 連接線狀態樣式（success/error/running/pinned）
   - 動畫效果（pulse-border、shimmer、edge-flow）

4. **連接創建流程**
   - Handle 組件（12px 圓點，hover 效果）
   - 連接驗證（DFS 循環檢測、類型兼容性、最大連接數）
   - 視覺反饋（藍色虛線表示有效，紅色表示無效）

5. **節點對齊與吸附**
   - 智能吸附算法（SNAP_THRESHOLD: 16px）
   - 對齊指示線（ALIGNMENT_THRESHOLD: 8px）
   - 均勻分佈（3+ 節點）

6. **批量操作**
   - 多選（Cmd/Ctrl 切換，Shift 框選）
   - 複製/粘貼（localStorage 剪貼板，ID 映射，50px 偏移）
   - 刪除（確認對話框，級聯刪除連接）
   - 群組管理（創建、折疊/展開、解散）

7. **撤銷/重做機制**
   - Command Pattern 實現
   - 命令類型：AddNode、DeleteNode、MoveNode、AddConnection、Batch
   - 命令合併優化（連續移動操作）
   - 50 條歷史記錄限制
   - 歷史管理 UI 組件

8. **鍵盤快捷鍵**
   - 7 個分類：通用、畫布、節點、選擇、剪貼板、歷史、視圖
   - 跨平台支持（Ctrl/Cmd 自動適配）
   - 快捷鍵管理器（註冊、衝突檢測、優先級）
   - 快捷鍵幫助面板（按 ? 鍵打開）

### 🎯 核心特性：

- **VueFlow 集成**：基於成熟的 Vue 3 流程圖庫
- **16px 網格系統**：統一的對齊基礎
- **智能吸附**：8px 引導線 + 16px 磁性吸附
- **完整的撤銷/重做**：基於 Command Pattern
- **豐富的鍵盤快捷鍵**：涵蓋所有常用操作
- **跨平台支持**：Windows/Linux/Mac 快捷鍵自動適配

### 📊 代碼統計：

- 總行數：約 3,900+ 行
- TypeScript 代碼示例：2,500+ 行
- Vue 組件：4 個完整組件
- Composables：7 個可重用組合式函數

Part 5 現已完整，可以繼續進行 Part 6（協作、版本控制與技術架構）的創建。