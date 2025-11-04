# PoC 6: VueFlow + CRDT Collaboration 驗證

**優先級**: 🟡 P1 (高優先級)
**預計時間**: 2 days (Day 13-14)
**狀態**: ⏳ 待開始
**負責人**: Frontend Tech Lead

**基於**: Vue 3.5 + VueFlow 1.47 + Yjs + Socket.io (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Workflow Editor 的流暢性與多用戶協作能力：

1. ✅ **VueFlow 畫布引擎**: 流暢的視覺化編輯器
2. ✅ **Yjs CRDT 協作**: 無衝突多用戶編輯
3. ✅ **Socket.io 實時同步**: 低延遲同步
4. ✅ **衝突解決機制**: 自動合併編輯
5. ✅ **性能基準**: 60 FPS + <200ms 延遲

---

## 📋 VueFlow + CRDT 架構

```
┌─────────────────────────────────────────┐
│      Workflow Editor (Vue 3 SPA)        │
│  ┌───────────────────────────────────┐  │
│  │   VueFlow Canvas Engine           │  │
│  │  - Nodes Rendering                │  │
│  │  - Edges (Connections)            │  │
│  │  - Drag & Drop                    │  │
│  │  - Zoom & Pan                     │  │
│  └───────────────────────────────────┘  │
│  ┌───────────────────────────────────┐  │
│  │   Yjs CRDT (Conflict-Free)        │  │
│  │  - Y.Doc (Shared Document)        │  │
│  │  - Y.Array (Nodes)                │  │
│  │  - Y.Array (Edges)                │  │
│  │  - Automatic Merge                │  │
│  └───────────────────────────────────┘  │
│  ┌───────────────────────────────────┐  │
│  │   Socket.io WebSocket Provider    │  │
│  │  - Real-time Sync                 │  │
│  │  - Presence Awareness             │  │
│  │  - Cursor Sharing                 │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

---

## 🔧 環境準備

### Vue 3 + VueFlow 項目設置

```bash
# 創建 Vue 3 項目
npm create vite@latest workflow-editor -- --template vue-ts
cd workflow-editor

# 安裝 VueFlow
npm install @vue-flow/core @vue-flow/background @vue-flow/controls

# 安裝 Yjs CRDT
npm install yjs y-websocket

# 安裝 Socket.io
npm install socket.io-client

# 安裝 Pinia (狀態管理)
npm install pinia

# 安裝 Element Plus (UI)
npm install element-plus
```

---

## 💻 實現步驟

### 步驟 1: VueFlow 基礎畫布

**WorkflowEditor.vue**:

```vue
<script setup lang="ts">
import { ref } from 'vue';
import { VueFlow, useVueFlow } from '@vue-flow/core';
import { Background } from '@vue-flow/background';
import { Controls } from '@vue-flow/controls';
import type { Node, Edge } from '@vue-flow/core';

const nodes = ref<Node[]>([
  {
    id: '1',
    type: 'input',
    label: 'Start Node',
    position: { x: 250, y: 5 }
  },
  {
    id: '2',
    label: 'Process Node',
    position: { x: 100, y: 100 }
  },
  {
    id: '3',
    type: 'output',
    label: 'End Node',
    position: { x: 250, y: 200 }
  }
]);

const edges = ref<Edge[]>([
  { id: 'e1-2', source: '1', target: '2' },
  { id: 'e2-3', source: '2', target: '3' }
]);

const { onConnect, addEdges } = useVueFlow();

onConnect((params) => {
  addEdges([params]);
});
</script>

<template>
  <div class="workflow-editor">
    <VueFlow
      :nodes="nodes"
      :edges="edges"
      :fitViewOnInit="true"
    >
      <Background />
      <Controls />
    </VueFlow>
  </div>
</template>

<style scoped>
.workflow-editor {
  width: 100vw;
  height: 100vh;
}
</style>
```

### 步驟 2: Yjs CRDT 整合

**useYjsWorkflow.ts**:

```typescript
import { ref, watchEffect } from 'vue';
import * as Y from 'yjs';
import { WebsocketProvider } from 'y-websocket';
import type { Node, Edge } from '@vue-flow/core';

export function useYjsWorkflow(workflowId: string) {
  // 創建 Y.Doc
  const ydoc = new Y.Doc();

  // 連接 WebSocket Provider
  const provider = new WebsocketProvider(
    'ws://localhost:4444',
    `workflow-${workflowId}`,
    ydoc
  );

  // Shared Types
  const yNodes = ydoc.getArray<Node>('nodes');
  const yEdges = ydoc.getArray<Edge>('edges');

  // Local State
  const nodes = ref<Node[]>([]);
  const edges = ref<Edge[]>([]);

  // Sync Yjs → Vue
  yNodes.observe(() => {
    nodes.value = yNodes.toArray();
  });

  yEdges.observe(() => {
    edges.value = yEdges.toArray();
  });

  // Sync Vue → Yjs
  const addNode = (node: Node) => {
    ydoc.transact(() => {
      yNodes.push([node]);
    });
  };

  const updateNode = (nodeId: string, updates: Partial<Node>) => {
    ydoc.transact(() => {
      const index = yNodes.toArray().findIndex(n => n.id === nodeId);
      if (index !== -1) {
        const node = yNodes.get(index);
        Object.assign(node, updates);
      }
    });
  };

  const addEdge = (edge: Edge) => {
    ydoc.transact(() => {
      yEdges.push([edge]);
    });
  };

  return {
    nodes,
    edges,
    addNode,
    updateNode,
    addEdge,
    provider
  };
}
```

### 步驟 3: Socket.io 實時同步

**socket-provider.ts**:

```typescript
import { io, Socket } from 'socket.io-client';
import type { Node, Edge } from '@vue-flow/core';

export class SocketProvider {
  private socket: Socket;

  constructor(workflowId: string) {
    this.socket = io('ws://localhost:3000', {
      query: { workflowId }
    });

    this.socket.on('connect', () => {
      console.log('✅ Connected to collaboration server');
    });

    this.socket.on('disconnect', () => {
      console.log('⚠️ Disconnected from collaboration server');
    });
  }

  // 發送節點更新
  emitNodeUpdate(node: Node) {
    this.socket.emit('node:update', node);
  }

  // 接收節點更新
  onNodeUpdate(callback: (node: Node) => void) {
    this.socket.on('node:update', callback);
  }

  // 發送邊更新
  emitEdgeUpdate(edge: Edge) {
    this.socket.emit('edge:update', edge);
  }

  // 接收邊更新
  onEdgeUpdate(callback: (edge: Edge) => void) {
    this.socket.on('edge:update', callback);
  }

  // Presence Awareness (誰在線)
  onUserJoin(callback: (user: User) => void) {
    this.socket.on('user:join', callback);
  }

  onUserLeave(callback: (userId: string) => void) {
    this.socket.on('user:leave', callback);
  }

  // 遊標共享
  emitCursorMove(position: { x: number; y: number }) {
    this.socket.emit('cursor:move', position);
  }

  onCursorMove(callback: (userId: string, pos: { x: number; y: number }) => void) {
    this.socket.on('cursor:move', callback);
  }

  disconnect() {
    this.socket.disconnect();
  }
}
```

---

## 🧪 測試用例

### 測試套件 1: VueFlow 性能

| 測試場景 | 節點數 | FPS 目標 | 實際 FPS | 狀態 |
|---------|--------|---------|----------|------|
| TC-1.1 | 10 nodes | 60 FPS | _____ | ✅ / ❌ |
| TC-1.2 | 50 nodes | 60 FPS | _____ | ✅ / ❌ |
| TC-1.3 | 100 nodes | 50 FPS | _____ | ✅ / ❌ |
| TC-1.4 | 200 nodes | 40 FPS | _____ | ✅ / ❌ |

### 測試套件 2: CRDT 協作

| 測試場景 | 用戶數 | 操作 | 預期結果 | 狀態 |
|---------|--------|------|----------|------|
| TC-2.1 | 2 用戶 | 同時添加節點 | 無衝突 | ✅ / ❌ |
| TC-2.2 | 3 用戶 | 同時編輯節點 | 自動合併 | ✅ / ❌ |
| TC-2.3 | 5 用戶 | 同時添加連線 | 無衝突 | ✅ / ❌ |
| TC-2.4 | 2 用戶 | 同時刪除節點 | Last-Write-Wins | ✅ / ❌ |

### 測試套件 3: 實時同步

| 測試場景 | 操作 | 延遲目標 | 實際延遲 | 狀態 |
|---------|------|----------|----------|------|
| TC-3.1 | 節點移動 | <200ms | _____ ms | ✅ / ❌ |
| TC-3.2 | 節點添加 | <200ms | _____ ms | ✅ / ❌ |
| TC-3.3 | 連線創建 | <200ms | _____ ms | ✅ / ❌ |
| TC-3.4 | 批量操作 | <500ms | _____ ms | ✅ / ❌ |

### 測試套件 4: 離線支持

| 測試場景 | 操作 | 預期結果 | 狀態 |
|---------|------|----------|------|
| TC-4.1 | 離線編輯 | 本地保存 | ✅ / ❌ |
| TC-4.2 | 重新連線 | 自動同步 | ✅ / ❌ |
| TC-4.3 | 衝突解決 | 正確合併 | ✅ / ❌ |

---

## ✅ 成功標準驗證

### 1. VueFlow 畫布流暢運行 (60 FPS) ✅ / ❌

**測試結果**:
- 10 nodes: _____ FPS
- 50 nodes: _____ FPS
- 100 nodes: _____ FPS

**結果**: ✅ / ❌

### 2. 多用戶協作無衝突 (3+ 用戶) ✅ / ❌

**測試結果**:
- 2 用戶: ✅ / ❌
- 3 用戶: ✅ / ❌
- 5 用戶: ✅ / ❌

**結果**: ✅ / ❌

### 3. 實時同步延遲 <200ms ✅ / ❌

**平均延遲**: _____ ms

**結果**: ✅ / ❌

### 4. 離線編輯支持正常 ✅ / ❌

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 技術發現

1. **VueFlow 性能**:
   - 最大節點數: _____
   - FPS 下降閾值: _____
   - 優化建議: _________________________

2. **Yjs CRDT 效果**:
   - 衝突解決正確性: _____%
   - 記憶體佔用: _____ MB
   - 發現問題: _________________________

3. **Socket.io 同步**:
   - 平均延遲: _____ ms
   - 最大延遲: _____ ms
   - 連線穩定性: _________________________

4. **n8n 最佳實踐對比**:
   - 技術棧相似度: ✅ / ❌
   - 實現差異: _________________________

---

## ⚠️ 風險識別

| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| VueFlow 大型工作流性能 | 中 | 虛擬化渲染 / 分頁加載 |
| CRDT 記憶體佔用 | 中 | 定期 GC / 壓縮 |
| WebSocket 連線斷開 | 中 | 自動重連 + 離線支持 |
| 複雜節點渲染慢 | 低 | 延遲渲染 / 簡化視覺 |

---

## 💡 建議與下一步

### 技術建議

1. **性能優化**:
   - 實現節點虛擬化渲染
   - 優化大型工作流渲染
   - 實現分頁加載

2. **協作體驗**:
   - 實現遊標共享
   - 顯示其他用戶位置
   - 實現評論/註釋功能

3. **穩定性提升**:
   - 增強離線支持
   - 實現自動保存
   - 設計版本控制

### 下一步行動

- [ ] 完成所有測試用例
- [ ] 性能優化
- [ ] 撰寫驗證報告
- [ ] 準備 Demo
- [ ] 團隊評審

---

## 📊 驗證結果總結

**執行日期**: _____________
**執行人**: _____________
**狀態**: ⏳ 進行中 / ✅ 通過 / ❌ 未通過

### 成功標準達成情況

| 成功標準 | 要求 | 實際結果 | 狀態 |
|---------|------|----------|------|
| VueFlow 流暢性 | 60 FPS | _____ FPS | ✅ / ❌ |
| 多用戶協作 | 3+ 用戶無衝突 | _____ 用戶 | ✅ / ❌ |
| 實時同步延遲 | <200ms | _____ ms | ✅ / ❌ |
| 離線編輯 | 支持 | ✅ / ❌ | ✅ / ❌ |

### 總體評估

**結論**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**理由**:
___________________________________________________________________________
___________________________________________________________________________

---

## 🔗 相關文檔

- [PoC 驗證計劃總覽](./README.md)
- [TECH-STACK-ANALYSIS.md](../TECH-STACK-ANALYSIS.md)
- [Vue 3 官方文檔](https://vuejs.org/)
- [VueFlow 文檔](https://vueflow.dev/)
- [Yjs CRDT 文檔](https://docs.yjs.dev/)
- [n8n 技術棧參考](https://docs.n8n.io/)

---

**最後更新**: 2025-10-30
**PoC 負責人**: Frontend Tech Lead

[← 上一個 PoC](./05-knowledge-rag-accuracy.md) | [驗證報告 →](./poc-validation-report.md)
