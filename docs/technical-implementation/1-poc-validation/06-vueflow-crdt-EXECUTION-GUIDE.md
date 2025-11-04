# PoC 6: VueFlow + CRDT Collaboration - 實際執行指導

**目的**: 驗證 Workflow Editor 的流暢性與多用戶協作能力

**預計時間**: 2 days (Day 13-14)
**執行人**: Frontend Tech Lead
**執行日期**: _____________
**優先級**: 🟡 P1 (高優先級，但非阻擋性)

**前置條件**: PoC 1-4 (P0) 已通過 ✅

---

## 📋 執行前檢查清單

### 必備條件

- [ ] **P0 PoC 完成**: PoC 1-4 全部 PASSED
- [ ] **Node.js 18+ 已安裝**
- [ ] **Vue 3 開發環境準備就緒**
- [ ] **測試瀏覽器**: Chrome/Edge (3+ 實例)
- [ ] **時間安排**: Day 13 (4-6h), Day 14 (4-6h)

### 核心目標

1. ✅ **VueFlow 畫布流暢運行**: 60 FPS (100 nodes)
2. ✅ **多用戶協作無衝突**: 3+ 用戶同時編輯
3. ✅ **實時同步延遲 <200ms**: Socket.io + Yjs
4. ✅ **離線編輯支持**: 斷線重連自動同步

---

## 🚀 Day 13: VueFlow + Yjs CRDT (4-6 小時)

### Phase 13.1: Vue 3 專案設置 (1 小時)

#### 步驟 1: 創建 Vue 3 專案

```bash
# 創建 Vite + Vue 3 專案
npm create vite@latest workflow-editor -- --template vue-ts
cd workflow-editor

# 安裝 VueFlow
npm install @vue-flow/core @vue-flow/background @vue-flow/controls

# 安裝 Yjs CRDT
npm install yjs y-websocket

# 安裝 Socket.io
npm install socket.io-client

# 安裝 Pinia
npm install pinia

# 啟動開發伺服器
npm run dev
```

**預期輸出**:
```
  VITE v5.0.0  ready in 500 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

**✅ 通過標準**: 開發伺服器啟動成功

---

### Phase 13.2: VueFlow 基礎畫布 (2 小時)

#### 步驟 2: 實現基礎 VueFlow 編輯器

**創建 `WorkflowEditor.vue`**:

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

const { onConnect, addEdges, project } = useVueFlow();

onConnect((params) => {
  addEdges([params]);
});

// 測試: 動態添加節點
const addNode = () => {
  const newId = String(nodes.value.length + 1);
  nodes.value.push({
    id: newId,
    label: `Node ${newId}`,
    position: project({ x: Math.random() * 400, y: Math.random() * 400 })
  });
};

// 性能測試: 添加 100 個節點
const add100Nodes = () => {
  for (let i = 0; i < 100; i++) {
    addNode();
  }
  console.log(`Total nodes: ${nodes.value.length}`);
};
</script>

<template>
  <div class="workflow-container">
    <div class="toolbar">
      <button @click="addNode">Add Node</button>
      <button @click="add100Nodes">Add 100 Nodes (Performance Test)</button>
      <span>Nodes: {{ nodes.length }}</span>
    </div>

    <VueFlow
      :nodes="nodes"
      :edges="edges"
      :fit-view-on-init="true"
      class="workflow-canvas"
    >
      <Background />
      <Controls />
    </VueFlow>
  </div>
</template>

<style scoped>
.workflow-container {
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.toolbar {
  padding: 10px;
  background: #f0f0f0;
  display: flex;
  gap: 10px;
  align-items: center;
}

.workflow-canvas {
  flex: 1;
}
</style>
```

**測試 VueFlow 性能**:

1. 打開 `http://localhost:5173`
2. 點擊 "Add 100 Nodes"
3. 觀察 FPS (Chrome DevTools > Performance)

**✅ 通過標準**: 100 nodes 下 FPS >50

**記錄點**:
```
[Day 13 - 12:00] VueFlow 基礎實現完成
- 10 nodes FPS: _____
- 100 nodes FPS: _____
- 畫布操作流暢度: 優秀 / 良好 / 需優化
```

---

### Phase 13.3: Yjs CRDT 整合 (2 小時)

#### 步驟 3: 實現 CRDT 協作

**創建 `useYjsWorkflow.ts`**:

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

  const deleteNode = (nodeId: string) => {
    ydoc.transact(() => {
      const index = yNodes.toArray().findIndex(n => n.id === nodeId);
      if (index !== -1) {
        yNodes.delete(index, 1);
      }
    });
  };

  const addEdge = (edge: Edge) => {
    ydoc.transact(() => {
      yEdges.push([edge]);
    });
  };

  // Presence Awareness (誰在線)
  const awareness = provider.awareness;
  const users = ref<Map<number, any>>(new Map());

  awareness.on('change', () => {
    users.value = new Map(awareness.getStates());
  });

  return {
    nodes,
    edges,
    users,
    addNode,
    updateNode,
    deleteNode,
    addEdge,
    provider
  };
}
```

**啟動 y-websocket 伺服器**:

```bash
# 安裝全局 y-websocket 伺服器
npm install -g y-websocket

# 啟動伺服器
HOST=localhost PORT=4444 y-websocket
```

**更新 WorkflowEditor.vue 使用 Yjs**:

```vue
<script setup lang="ts">
import { useYjsWorkflow } from './useYjsWorkflow';

// 使用 Yjs 共享狀態
const workflowId = 'test-workflow-1';
const {
  nodes,
  edges,
  users,
  addNode,
  updateNode,
  deleteNode,
  addEdge
} = useYjsWorkflow(workflowId);

// 顯示在線用戶
const onlineUsers = computed(() => users.value.size);
</script>

<template>
  <div class="toolbar">
    <span>Online Users: {{ onlineUsers }}</span>
    <button @click="addNode({ id: Date.now().toString(), label: 'New', position: { x: 100, y: 100 } })">
      Add Node
    </button>
  </div>

  <VueFlow :nodes="nodes" :edges="edges">
    <Background />
    <Controls />
  </VueFlow>
</template>
```

**測試多用戶協作**:

1. 在 3 個瀏覽器視窗打開應用
2. 在不同視窗添加/移動節點
3. 觀察實時同步

**✅ 通過標準**: 3 個用戶同時操作無衝突

---

## 🚀 Day 14: 性能測試與報告 (4-6 小時)

### Phase 14.1: 同步延遲測試 (2 小時)

#### 步驟 4: 測量實時同步延遲

**創建延遲測試工具**:

```typescript
// latency-test.ts
export async function measureSyncLatency(
  addNode: (node: Node) => void,
  nodes: Ref<Node[]>
): Promise<number> {
  const testNodeId = `latency-test-${Date.now()}`;
  const startTime = performance.now();

  // 添加節點
  addNode({
    id: testNodeId,
    label: 'Latency Test',
    position: { x: 0, y: 0 }
  });

  // 等待節點出現在 nodes 數組
  return new Promise((resolve) => {
    const checkInterval = setInterval(() => {
      if (nodes.value.some(n => n.id === testNodeId)) {
        const latency = performance.now() - startTime;
        clearInterval(checkInterval);
        resolve(latency);
      }
    }, 10);
  });
}
```

**執行 10 次延遲測試**:

```typescript
const latencies: number[] = [];
for (let i = 0; i < 10; i++) {
  const latency = await measureSyncLatency(addNode, nodes);
  latencies.push(latency);
  console.log(`Test ${i + 1}: ${latency.toFixed(2)}ms`);
}

const avgLatency = latencies.reduce((a, b) => a + b) / latencies.length;
console.log(`Average Latency: ${avgLatency.toFixed(2)}ms`);
console.log(`Result: ${avgLatency < 200 ? '✅ PASSED' : '❌ FAILED'}`);
```

**記錄點**:
```
[Day 14 - 12:00] 同步延遲測試完成
- 平均延遲: _____ ms (目標 <200ms)
- 最小延遲: _____ ms
- 最大延遲: _____ ms
```

---

### Phase 14.2: 離線支持測試 (1 小時)

#### 步驟 5: 測試斷線重連

**測試步驟**:

1. 打開應用，添加一些節點
2. 停止 y-websocket 伺服器 (模擬斷線)
3. 繼續添加/編輯節點 (應該本地保存)
4. 重啟 y-websocket 伺服器
5. 觀察是否自動同步

**實現離線檢測**:

```typescript
const isOnline = ref(true);

provider.on('status', (event: { status: string }) => {
  isOnline.value = event.status === 'connected';
  console.log(`Connection status: ${event.status}`);
});
```

**✅ 通過標準**: 斷線後重連，離線編輯自動同步

---

### Phase 14.3: 填寫驗證報告 (1 小時)

#### 步驟 6: 整理結果

**填寫報告** (`poc-validation-report.md`):

```markdown
### PoC 6: VueFlow + CRDT Collaboration (P1) 🟡

**狀態**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**成功標準達成情況**:
- [✅] VueFlow 畫布流暢運行 (60 FPS)
- [✅] 多用戶協作無衝突 (3+ 用戶)
- [✅] 實時同步延遲 <200ms (平均 120ms)
- [✅] 離線編輯支持正常

**關鍵發現**:
- VueFlow 性能: 65 FPS (100 nodes)
- 最大並發用戶: 5 用戶測試成功
- 平均同步延遲: 120 ms
- n8n 最佳實踐對比: 技術棧相似，實現方案可行

**風險與緩解**:
| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| VueFlow 大型工作流性能 | 中 | 虛擬化渲染 / 分頁加載 |
| CRDT 記憶體佔用 | 中 | 定期 GC / 壓縮 |
| WebSocket 連線斷開 | 中 | 自動重連 + 離線支持 |

**建議**:
- VueFlow 表現優異，推薦採用
- 實現節點虛擬化渲染（大型工作流）
- 增強離線支持（IndexedDB 持久化）
- 實現遊標共享和評論功能
```

---

## 📊 最終檢查清單

- [ ] **VueFlow 性能**: 100 nodes >50 FPS
- [ ] **多用戶協作**: 3+ 用戶無衝突
- [ ] **同步延遲**: <200ms
- [ ] **離線支持**: 斷線重連正常
- [ ] **報告填寫**: 完成

---

## 🎯 Go/No-Go 決策

### ✅ GO
- [x] VueFlow 流暢 (60 FPS)
- [x] 3+ 用戶協作無衝突
- [x] 同步延遲 <200ms
- [x] 離線支持正常

### ⚠️ 有條件 GO
- [ ] FPS: 50-60 (可優化虛擬化)
- [ ] 延遲: 200-300ms (可優化 WebSocket)

### ❌ NO-GO
- [ ] FPS <50 (大型工作流卡頓)
- [ ] 多用戶頻繁衝突
- [ ] 延遲 >300ms

---

**執行記錄**:

```
執行人: _________________
執行日期: _______________

Day 13 完成: _____________
Day 14 完成: _____________

最終狀態: ✅ PASSED / ❌ FAILED / ⚠️ CONDITIONAL

關鍵指標:
- VueFlow FPS (100 nodes): _____ (目標 >50)
- 最大並發用戶: _____ 用戶
- 平均同步延遲: _____ ms (目標 <200ms)
- 離線支持: 正常 / 異常

技術評估:
- VueFlow 性能: 優秀 / 良好 / 需優化
- Yjs CRDT 效果: 穩定 / 基本 / 不穩定
- Socket.io 連線: 穩定 / 偶爾斷線 / 頻繁斷線

Go/No-Go: _______________ (決策人: _______________)
```

---

**最後更新**: 2025-10-30
**執行指導版本**: 1.0.0 (精簡版 - P1 PoC)
