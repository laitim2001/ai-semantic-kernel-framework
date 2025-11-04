# Frontend Vue Structure (Vue 3 Workflow Editor)

## 版本信息
- **Vue 版本**: 3.5
- **TypeScript**: 5.8
- **構建工具**: Vite 5
- **核心庫**: VueFlow 1.33, Y.js (CRDT)
- **最後更新**: 2025-01-01

## 整體結構

```
frontend/vue-workflow-editor/
├── public/                                # 靜態資源
│   └── favicon.ico
│
├── src/                                   # 源代碼
│   ├── assets/                           # 資源文件
│   │   ├── icons/
│   │   └── images/
│   ├── components/                       # Vue 組件
│   │   ├── nodes/                        # VueFlow 節點組件
│   │   ├── edges/                        # VueFlow 邊組件
│   │   ├── panels/                       # 側邊面板組件
│   │   └── common/                       # 通用組件
│   ├── composables/                      # 組合式函數
│   │   ├── useVueFlow.ts
│   │   ├── useYjsSync.ts
│   │   ├── useWorkflowValidation.ts
│   │   └── useWebSocket.ts
│   ├── stores/                           # Pinia Stores
│   │   ├── workflow.ts
│   │   ├── collaboration.ts
│   │   └── ui.ts
│   ├── types/                            # TypeScript 類型
│   │   ├── workflow.ts
│   │   ├── node.ts
│   │   └── crdt.ts
│   ├── utils/                            # 工具函數
│   │   ├── nodeFactory.ts
│   │   ├── validation.ts
│   │   └── serialization.ts
│   ├── styles/                           # 全局樣式
│   │   ├── variables.css
│   │   ├── vueflow.css
│   │   └── main.css
│   ├── App.vue                           # 根組件
│   ├── main.ts                           # 應用入口
│   └── vite-env.d.ts
│
├── server/                                # WebSocket 服務器
│   ├── websocket-server.js               # Y.js WebSocket Provider
│   └── config.js
│
├── tests/                                 # 測試文件
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
├── .env.local
├── .eslintrc.cjs
├── .prettierrc
├── index.html
├── package.json
├── tsconfig.json
├── tsconfig.node.json
└── vite.config.ts
```

## 核心功能模塊

### 1. VueFlow 集成

#### 主編輯器組件

```vue
<!-- src/App.vue -->
<script setup lang="ts">
import { VueFlow, useVueFlow } from '@vue-flow/core'
import { Background } from '@vue-flow/background'
import { Controls } from '@vue-flow/controls'
import { MiniMap } from '@vue-flow/minimap'
import { onMounted } from 'vue'
import { useWorkflowStore } from './stores/workflow'
import { useCollaborationStore } from './stores/collaboration'
import { useYjsSync } from './composables/useYjsSync'

// Custom Nodes
import PromptNode from './components/nodes/PromptNode.vue'
import AgentNode from './components/nodes/AgentNode.vue'
import FunctionNode from './components/nodes/FunctionNode.vue'
import ConditionNode from './components/nodes/ConditionNode.vue'

// Panels
import NodePanel from './components/panels/NodePanel.vue'
import PropertiesPanel from './components/panels/PropertiesPanel.vue'
import CollaboratorsPanel from './components/panels/CollaboratorsPanel.vue'

const workflowStore = useWorkflowStore()
const collaborationStore = useCollaborationStore()

// VueFlow 實例
const { nodes, edges, addNodes, addEdges, onConnect, onNodesChange, onEdgesChange } =
  useVueFlow()

// Y.js CRDT 同步
const { isConnected, isSyncing } = useYjsSync({
  roomName: 'workflow-editor',
  nodes,
  edges,
})

// 連接新邊時的處理
onConnect((params) => {
  addEdges([
    {
      id: `edge-${params.source}-${params.target}`,
      source: params.source,
      target: params.target,
      type: 'smoothstep',
    },
  ])
})

// 節點變化處理
onNodesChange((changes) => {
  workflowStore.handleNodesChange(changes)
})

// 邊變化處理
onEdgesChange((changes) => {
  workflowStore.handleEdgesChange(changes)
})

onMounted(() => {
  // 初始化編輯器
  workflowStore.loadWorkflow()
})
</script>

<template>
  <div class="workflow-editor">
    <!-- 頂部工具欄 -->
    <header class="editor-header">
      <h1>Workflow Editor</h1>
      <div class="toolbar">
        <button @click="workflowStore.save" :disabled="isSyncing">
          {{ isSyncing ? '同步中...' : '保存' }}
        </button>
        <button @click="workflowStore.execute">執行</button>
        <div class="connection-status">
          <span :class="{ connected: isConnected }">
            {{ isConnected ? '已連接' : '未連接' }}
          </span>
        </div>
      </div>
    </header>

    <!-- 主編輯區 -->
    <div class="editor-container">
      <!-- 左側節點面板 -->
      <NodePanel />

      <!-- 中間 VueFlow 畫布 -->
      <div class="canvas-wrapper">
        <VueFlow
          v-model:nodes="nodes"
          v-model:edges="edges"
          :default-zoom="1"
          :min-zoom="0.2"
          :max-zoom="4"
          :snap-to-grid="true"
          :snap-grid="[15, 15]"
          fit-view-on-init
        >
          <!-- 自定義節點類型 -->
          <template #node-prompt="{ data }">
            <PromptNode :data="data" />
          </template>

          <template #node-agent="{ data }">
            <AgentNode :data="data" />
          </template>

          <template #node-function="{ data }">
            <FunctionNode :data="data" />
          </template>

          <template #node-condition="{ data }">
            <ConditionNode :data="data" />
          </template>

          <!-- 背景 -->
          <Background pattern-color="#aaa" :gap="16" />

          <!-- 控制器 -->
          <Controls />

          <!-- 小地圖 -->
          <MiniMap />
        </VueFlow>
      </div>

      <!-- 右側屬性面板 -->
      <PropertiesPanel />
    </div>

    <!-- 底部協作者面板 -->
    <CollaboratorsPanel />
  </div>
</template>

<style scoped>
.workflow-editor {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background: #fff;
  border-bottom: 1px solid #e0e0e0;
}

.toolbar {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.connection-status span {
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.875rem;
  background: #f0f0f0;
}

.connection-status .connected {
  background: #4caf50;
  color: white;
}

.editor-container {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.canvas-wrapper {
  flex: 1;
  position: relative;
}
</style>
```

### 2. 自定義節點組件

```
src/components/nodes/
├── PromptNode.vue                        # Prompt 節點
├── AgentNode.vue                         # Agent 節點
├── FunctionNode.vue                      # Function 節點
├── ConditionNode.vue                     # 條件節點
└── BaseNode.vue                          # 節點基類
```

#### Agent 節點示例

```vue
<!-- src/components/nodes/AgentNode.vue -->
<script setup lang="ts">
import { Handle, Position } from '@vue-flow/core'
import { computed } from 'vue'
import type { AgentNodeData } from '@/types/node'

interface Props {
  data: AgentNodeData
}

const props = defineProps<Props>()

const personaName = computed(() => props.data.persona?.name || 'Unnamed Agent')
const instruction = computed(
  () => props.data.instruction || 'No instruction provided'
)

const isConfigured = computed(() => {
  return props.data.persona && props.data.instruction
})
</script>

<template>
  <div class="agent-node" :class="{ configured: isConfigured }">
    <!-- 輸入 Handle -->
    <Handle type="target" :position="Position.Left" class="handle-input" />

    <!-- 節點內容 -->
    <div class="node-header">
      <div class="node-icon">🤖</div>
      <div class="node-title">{{ personaName }}</div>
    </div>

    <div class="node-body">
      <div class="instruction">{{ instruction }}</div>
    </div>

    <div class="node-footer">
      <span v-if="!isConfigured" class="warning">⚠️ 未配置</span>
      <span v-else class="status">✅ 已配置</span>
    </div>

    <!-- 輸出 Handle -->
    <Handle type="source" :position="Position.Right" class="handle-output" />
  </div>
</template>

<style scoped>
.agent-node {
  background: white;
  border: 2px solid #1a192b;
  border-radius: 8px;
  padding: 12px;
  min-width: 200px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: all 0.2s;
}

.agent-node:hover {
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.agent-node.configured {
  border-color: #4caf50;
}

.node-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}

.node-icon {
  font-size: 24px;
}

.node-title {
  font-weight: 600;
  font-size: 14px;
}

.node-body {
  font-size: 12px;
  color: #666;
  margin-bottom: 8px;
}

.instruction {
  max-width: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.node-footer {
  display: flex;
  justify-content: flex-end;
  font-size: 11px;
}

.warning {
  color: #ff9800;
}

.status {
  color: #4caf50;
}

.handle-input,
.handle-output {
  width: 12px;
  height: 12px;
  background: #555;
  border: 2px solid white;
}
</style>
```

### 3. Y.js CRDT 協作

#### Y.js 同步 Composable

```typescript
// src/composables/useYjsSync.ts
import { ref, watch, onUnmounted } from 'vue'
import type { Ref } from 'vue'
import * as Y from 'yjs'
import { WebsocketProvider } from 'y-websocket'
import type { Node, Edge } from '@vue-flow/core'

interface UseYjsSyncOptions {
  roomName: string
  nodes: Ref<Node[]>
  edges: Ref<Edge[]>
}

export function useYjsSync({ roomName, nodes, edges }: UseYjsSyncOptions) {
  // Y.js 文檔
  const ydoc = new Y.Doc()

  // Y.js 共享類型
  const yNodes = ydoc.getArray<Node>('nodes')
  const yEdges = ydoc.getArray<Edge>('edges')

  // 連接狀態
  const isConnected = ref(false)
  const isSyncing = ref(false)

  // 客戶端 ID (用於避免循環更新)
  const clientId = ydoc.clientID.toString()

  // WebSocket Provider
  const wsUrl = import.meta.env.VITE_WS_URL || 'ws://localhost:3001'
  const provider = new WebsocketProvider(wsUrl, roomName, ydoc, {
    connect: true,
  })

  // 連接狀態監聽
  provider.on('status', (event: { status: string }) => {
    isConnected.value = event.status === 'connected'
  })

  provider.on('sync', (isSynced: boolean) => {
    if (isSynced) {
      // 首次同步完成,加載遠程數據
      nodes.value = yNodes.toArray()
      edges.value = yEdges.toArray()
    }
  })

  // 監聽 Y.js 變化 (來自其他客戶端)
  yNodes.observe((event) => {
    if (!isSyncing.value) {
      nodes.value = deduplicateNodes(yNodes.toArray())
    }
  })

  yEdges.observe((event) => {
    if (!isSyncing.value) {
      edges.value = deduplicateEdges(yEdges.toArray())
    }
  })

  // 監聽本地節點變化,同步到 Y.js
  watch(
    nodes,
    (newNodes) => {
      syncNodesToYjs(newNodes)
    },
    { deep: true }
  )

  // 監聽本地邊變化,同步到 Y.js
  watch(
    edges,
    (newEdges) => {
      syncEdgesToYjs(newEdges)
    },
    { deep: true }
  )

  // 同步節點到 Y.js
  function syncNodesToYjs(newNodes: Node[]) {
    isSyncing.value = true

    ydoc.transact(() => {
      // 清空舊數據
      yNodes.delete(0, yNodes.length)

      // 去重後插入新數據
      const deduplicated = deduplicateNodes(newNodes)
      deduplicated.forEach((node) => {
        // 添加客戶端標識
        const nodeWithClient = {
          ...node,
          _clientId: clientId,
          _timestamp: Date.now(),
        }
        yNodes.push([nodeWithClient])
      })
    })

    isSyncing.value = false
  }

  // 同步邊到 Y.js
  function syncEdgesToYjs(newEdges: Edge[]) {
    isSyncing.value = true

    ydoc.transact(() => {
      yEdges.delete(0, yEdges.length)

      const deduplicated = deduplicateEdges(newEdges)
      deduplicated.forEach((edge) => {
        const edgeWithClient = {
          ...edge,
          _clientId: clientId,
          _timestamp: Date.now(),
        }
        yEdges.push([edgeWithClient])
      })
    })

    isSyncing.value = false
  }

  // 去重節點 (保留最新的)
  function deduplicateNodes(nodeList: Node[]): Node[] {
    const nodeMap = new Map<string, Node>()

    nodeList.forEach((node) => {
      const existingNode = nodeMap.get(node.id)

      if (!existingNode) {
        nodeMap.set(node.id, node)
      } else {
        // 比較時間戳,保留最新的
        const existingTimestamp = (existingNode as any)._timestamp || 0
        const newTimestamp = (node as any)._timestamp || 0

        if (newTimestamp > existingTimestamp) {
          nodeMap.set(node.id, node)
        }
      }
    })

    return Array.from(nodeMap.values())
  }

  // 去重邊
  function deduplicateEdges(edgeList: Edge[]): Edge[] {
    const edgeMap = new Map<string, Edge>()

    edgeList.forEach((edge) => {
      const existingEdge = edgeMap.get(edge.id)

      if (!existingEdge) {
        edgeMap.set(edge.id, edge)
      } else {
        const existingTimestamp = (existingEdge as any)._timestamp || 0
        const newTimestamp = (edge as any)._timestamp || 0

        if (newTimestamp > existingTimestamp) {
          edgeMap.set(edge.id, edge)
        }
      }
    })

    return Array.from(edgeMap.values())
  }

  // 清理
  onUnmounted(() => {
    provider.destroy()
    ydoc.destroy()
  })

  return {
    ydoc,
    provider,
    isConnected,
    isSyncing,
    clientId,
  }
}
```

### 4. Pinia Stores

```
src/stores/
├── workflow.ts                           # Workflow 狀態
├── collaboration.ts                      # 協作狀態
└── ui.ts                                # UI 狀態
```

#### Workflow Store 示例

```typescript
// src/stores/workflow.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Node, Edge, NodeChange, EdgeChange } from '@vue-flow/core'
import { applyNodeChanges, applyEdgeChanges } from '@vue-flow/core'
import type { Workflow } from '@/types/workflow'
import { apiClient } from '@/api/client'

export const useWorkflowStore = defineStore('workflow', () => {
  // State
  const currentWorkflow = ref<Workflow | null>(null)
  const nodes = ref<Node[]>([])
  const edges = ref<Edge[]>([])
  const selectedNodeId = ref<string | null>(null)
  const isExecuting = ref(false)

  // Getters
  const selectedNode = computed(() => {
    if (!selectedNodeId.value) return null
    return nodes.value.find((n) => n.id === selectedNodeId.value)
  })

  const isWorkflowValid = computed(() => {
    // 至少有一個節點
    if (nodes.value.length === 0) return false

    // 所有節點都已配置
    return nodes.value.every((node) => {
      return (node.data as any).isConfigured !== false
    })
  })

  // Actions
  function loadWorkflow(workflowId?: number) {
    if (workflowId) {
      // 從 API 加載
      apiClient.get<Workflow>(`/api/workflows/${workflowId}`).then((workflow) => {
        currentWorkflow.value = workflow
        nodes.value = workflow.nodes || []
        edges.value = workflow.edges || []
      })
    } else {
      // 新建空白 Workflow
      currentWorkflow.value = null
      nodes.value = []
      edges.value = []
    }
  }

  async function saveWorkflow() {
    if (!currentWorkflow.value) {
      // 創建新 Workflow
      const newWorkflow = await apiClient.post<Workflow>('/api/workflows', {
        name: 'Untitled Workflow',
        description: '',
        nodes: nodes.value,
        edges: edges.value,
      })
      currentWorkflow.value = newWorkflow
    } else {
      // 更新現有 Workflow
      await apiClient.put(`/api/workflows/${currentWorkflow.value.id}`, {
        nodes: nodes.value,
        edges: edges.value,
      })
    }
  }

  async function executeWorkflow(inputs: Record<string, unknown>) {
    if (!currentWorkflow.value || !isWorkflowValid.value) {
      throw new Error('Workflow is not valid for execution')
    }

    isExecuting.value = true

    try {
      const result = await apiClient.post(
        `/api/workflows/${currentWorkflow.value.id}/execute`,
        inputs
      )
      return result
    } finally {
      isExecuting.value = false
    }
  }

  function handleNodesChange(changes: NodeChange[]) {
    nodes.value = applyNodeChanges(changes, nodes.value)
  }

  function handleEdgesChange(changes: EdgeChange[]) {
    edges.value = applyEdgeChanges(changes, edges.value)
  }

  function setSelectedNode(nodeId: string | null) {
    selectedNodeId.value = nodeId
  }

  function addNode(node: Node) {
    nodes.value.push(node)
  }

  function updateNode(nodeId: string, data: Partial<Node['data']>) {
    const node = nodes.value.find((n) => n.id === nodeId)
    if (node) {
      node.data = { ...node.data, ...data }
    }
  }

  function removeNode(nodeId: string) {
    nodes.value = nodes.value.filter((n) => n.id !== nodeId)
    edges.value = edges.value.filter(
      (e) => e.source !== nodeId && e.target !== nodeId
    )
  }

  return {
    // State
    currentWorkflow,
    nodes,
    edges,
    selectedNodeId,
    isExecuting,

    // Getters
    selectedNode,
    isWorkflowValid,

    // Actions
    loadWorkflow,
    saveWorkflow,
    executeWorkflow,
    handleNodesChange,
    handleEdgesChange,
    setSelectedNode,
    addNode,
    updateNode,
    removeNode,
  }
})
```

#### Collaboration Store 示例

```typescript
// src/stores/collaboration.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

interface Collaborator {
  clientId: string
  name: string
  color: string
  cursor?: { x: number; y: number }
  selectedNodeId?: string
}

export const useCollaborationStore = defineStore('collaboration', () => {
  // State
  const collaborators = ref<Map<string, Collaborator>>(new Map())
  const currentUser = ref<Collaborator | null>(null)

  // Getters
  const activeCollaborators = computed(() => {
    return Array.from(collaborators.value.values()).filter(
      (c) => c.clientId !== currentUser.value?.clientId
    )
  })

  const collaboratorCount = computed(() => activeCollaborators.value.length)

  // Actions
  function setCurrentUser(clientId: string, name: string) {
    const color = generateColor(clientId)
    currentUser.value = { clientId, name, color }
  }

  function addCollaborator(collaborator: Collaborator) {
    collaborators.value.set(collaborator.clientId, collaborator)
  }

  function removeCollaborator(clientId: string) {
    collaborators.value.delete(clientId)
  }

  function updateCollaboratorCursor(clientId: string, cursor: { x: number; y: number }) {
    const collaborator = collaborators.value.get(clientId)
    if (collaborator) {
      collaborator.cursor = cursor
    }
  }

  function updateCollaboratorSelection(clientId: string, nodeId: string | undefined) {
    const collaborator = collaborators.value.get(clientId)
    if (collaborator) {
      collaborator.selectedNodeId = nodeId
    }
  }

  // Helper: 根據 clientId 生成固定顏色
  function generateColor(clientId: string): string {
    const colors = [
      '#FF6B6B',
      '#4ECDC4',
      '#45B7D1',
      '#FFA07A',
      '#98D8C8',
      '#F7DC6F',
      '#BB8FCE',
      '#85C1E2',
    ]

    let hash = 0
    for (let i = 0; i < clientId.length; i++) {
      hash = clientId.charCodeAt(i) + ((hash << 5) - hash)
    }

    return colors[Math.abs(hash) % colors.length]
  }

  return {
    // State
    collaborators,
    currentUser,

    // Getters
    activeCollaborators,
    collaboratorCount,

    // Actions
    setCurrentUser,
    addCollaborator,
    removeCollaborator,
    updateCollaboratorCursor,
    updateCollaboratorSelection,
  }
})
```

### 5. 側邊面板組件

#### Node Panel (節點列表面板)

```vue
<!-- src/components/panels/NodePanel.vue -->
<script setup lang="ts">
import { useWorkflowStore } from '@/stores/workflow'
import { createNode } from '@/utils/nodeFactory'

const workflowStore = useWorkflowStore()

const nodeTypes = [
  { type: 'prompt', label: 'Prompt', icon: '💬', description: '自定義 AI Prompt' },
  { type: 'agent', label: 'Agent', icon: '🤖', description: '智能代理節點' },
  {
    type: 'function',
    label: 'Function',
    icon: '⚙️',
    description: '自定義函數',
  },
  {
    type: 'condition',
    label: 'Condition',
    icon: '🔀',
    description: '條件分支',
  },
]

function handleDragStart(event: DragEvent, nodeType: string) {
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
    event.dataTransfer.setData('application/vueflow', nodeType)
  }
}

function handleClick(nodeType: string) {
  // 在畫布中心創建節點
  const newNode = createNode(nodeType, {
    x: window.innerWidth / 2,
    y: window.innerHeight / 2,
  })

  workflowStore.addNode(newNode)
}
</script>

<template>
  <aside class="node-panel">
    <h3>節點類型</h3>
    <div class="node-list">
      <div
        v-for="nodeType in nodeTypes"
        :key="nodeType.type"
        class="node-item"
        draggable="true"
        @dragstart="handleDragStart($event, nodeType.type)"
        @click="handleClick(nodeType.type)"
      >
        <div class="node-icon">{{ nodeType.icon }}</div>
        <div class="node-info">
          <div class="node-label">{{ nodeType.label }}</div>
          <div class="node-description">{{ nodeType.description }}</div>
        </div>
      </div>
    </div>
  </aside>
</template>

<style scoped>
.node-panel {
  width: 250px;
  background: #f5f5f5;
  padding: 1rem;
  border-right: 1px solid #e0e0e0;
  overflow-y: auto;
}

.node-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.node-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  cursor: grab;
  transition: all 0.2s;
}

.node-item:hover {
  border-color: #2196f3;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.node-item:active {
  cursor: grabbing;
}

.node-icon {
  font-size: 24px;
}

.node-info {
  flex: 1;
}

.node-label {
  font-weight: 600;
  font-size: 14px;
  margin-bottom: 2px;
}

.node-description {
  font-size: 12px;
  color: #666;
}
</style>
```

#### Properties Panel (屬性編輯面板)

```vue
<!-- src/components/panels/PropertiesPanel.vue -->
<script setup lang="ts">
import { computed } from 'vue'
import { useWorkflowStore } from '@/stores/workflow'

const workflowStore = useWorkflowStore()

const selectedNode = computed(() => workflowStore.selectedNode)

function updateNodeData(key: string, value: unknown) {
  if (selectedNode.value) {
    workflowStore.updateNode(selectedNode.value.id, {
      [key]: value,
    })
  }
}

function deleteNode() {
  if (selectedNode.value) {
    workflowStore.removeNode(selectedNode.value.id)
  }
}
</script>

<template>
  <aside class="properties-panel">
    <div v-if="!selectedNode" class="empty-state">
      <p>選擇一個節點以編輯屬性</p>
    </div>

    <div v-else class="properties-content">
      <div class="panel-header">
        <h3>節點屬性</h3>
        <button @click="deleteNode" class="delete-btn">🗑️ 刪除</button>
      </div>

      <!-- Agent Node Properties -->
      <div v-if="selectedNode.type === 'agent'" class="properties-form">
        <div class="form-group">
          <label>Persona</label>
          <select
            :value="selectedNode.data.personaId"
            @change="updateNodeData('personaId', ($event.target as HTMLSelectElement).value)"
          >
            <option value="">選擇 Persona</option>
            <option value="1">助手</option>
            <option value="2">專家</option>
          </select>
        </div>

        <div class="form-group">
          <label>指令</label>
          <textarea
            :value="selectedNode.data.instruction"
            @input="updateNodeData('instruction', ($event.target as HTMLTextAreaElement).value)"
            rows="4"
          />
        </div>
      </div>

      <!-- Prompt Node Properties -->
      <div v-else-if="selectedNode.type === 'prompt'" class="properties-form">
        <div class="form-group">
          <label>Prompt 模板</label>
          <textarea
            :value="selectedNode.data.template"
            @input="updateNodeData('template', ($event.target as HTMLTextAreaElement).value)"
            rows="6"
          />
        </div>
      </div>
    </div>
  </aside>
</template>

<style scoped>
.properties-panel {
  width: 300px;
  background: #f5f5f5;
  border-left: 1px solid #e0e0e0;
  padding: 1rem;
  overflow-y: auto;
}

.empty-state {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.delete-btn {
  padding: 0.5rem 1rem;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.properties-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  font-size: 14px;
}

.form-group input,
.form-group textarea,
.form-group select {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
}
</style>
```

### 6. WebSocket 服務器

```javascript
// server/websocket-server.js
const WebSocket = require('ws')
const http = require('http')

const PORT = process.env.WS_PORT || 3001

// 創建 HTTP 服務器
const server = http.createServer((req, res) => {
  res.writeHead(200, { 'Content-Type': 'text/plain' })
  res.end('Y.js WebSocket Server')
})

// 創建 WebSocket 服務器
const wss = new WebSocket.Server({ server })

// 存儲房間連接
const rooms = new Map()

wss.on('connection', (ws, req) => {
  // 從 URL 提取房間名稱
  const url = new URL(req.url, `http://${req.headers.host}`)
  const roomName = url.pathname.slice(1) // 移除開頭的 '/'

  console.log(`Client connected to room: ${roomName}`)

  // 初始化房間
  if (!rooms.has(roomName)) {
    rooms.set(roomName, new Set())
  }

  const room = rooms.get(roomName)
  room.add(ws)

  // 轉發消息到同一房間的其他客戶端
  ws.on('message', (data) => {
    room.forEach((client) => {
      if (client !== ws && client.readyState === WebSocket.OPEN) {
        client.send(data)
      }
    })
  })

  // 客戶端斷開連接
  ws.on('close', () => {
    room.delete(ws)

    // 如果房間為空,清理房間
    if (room.size === 0) {
      rooms.delete(roomName)
      console.log(`Room ${roomName} deleted (no clients)`)
    }

    console.log(`Client disconnected from room: ${roomName}`)
  })
})

server.listen(PORT, () => {
  console.log(`Y.js WebSocket Server listening on port ${PORT}`)
})
```

### 7. 工具函數

```typescript
// src/utils/nodeFactory.ts
import type { Node } from '@vue-flow/core'

export function createNode(
  type: string,
  position: { x: number; y: number }
): Node {
  const id = `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`

  const baseNode = {
    id,
    type,
    position,
  }

  switch (type) {
    case 'agent':
      return {
        ...baseNode,
        data: {
          label: 'Agent Node',
          personaId: null,
          instruction: '',
          isConfigured: false,
        },
      }

    case 'prompt':
      return {
        ...baseNode,
        data: {
          label: 'Prompt Node',
          template: '',
          isConfigured: false,
        },
      }

    case 'function':
      return {
        ...baseNode,
        data: {
          label: 'Function Node',
          functionName: '',
          parameters: {},
          isConfigured: false,
        },
      }

    case 'condition':
      return {
        ...baseNode,
        data: {
          label: 'Condition Node',
          condition: '',
          isConfigured: false,
        },
      }

    default:
      return {
        ...baseNode,
        data: { label: 'Unknown Node' },
      }
  }
}
```

## 配置文件

### Vite 配置

```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 5174,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
      },
    },
  },
  optimizeDeps: {
    include: ['yjs', 'y-websocket', '@vue-flow/core'],
  },
})
```

### Package.json

```json
{
  "name": "vue-workflow-editor",
  "version": "1.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "preview": "vite preview",
    "ws-server": "node server/websocket-server.js"
  },
  "dependencies": {
    "vue": "^3.5.0",
    "@vue-flow/core": "^1.33.0",
    "@vue-flow/background": "^1.3.0",
    "@vue-flow/controls": "^1.1.0",
    "@vue-flow/minimap": "^1.4.0",
    "pinia": "^2.2.0",
    "yjs": "^13.6.0",
    "y-websocket": "^2.0.0",
    "axios": "^1.7.0"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^5.0.0",
    "typescript": "^5.8.0",
    "vue-tsc": "^2.0.0",
    "vite": "^5.0.0",
    "ws": "^8.18.0"
  }
}
```

## 最佳實踐

### 1. VueFlow 性能優化
- ✅ 使用 `node-drag-threshold` 減少不必要的渲染
- ✅ 啟用 `snapToGrid` 提升對齊體驗
- ✅ 使用 `fitViewOnInit` 自動居中顯示

### 2. Y.js CRDT 同步
- ✅ 使用事務 (`ydoc.transact`) 批量更新
- ✅ 實現去重邏輯避免節點重複
- ✅ 添加時間戳和客戶端 ID 追蹤變更來源

### 3. 組件設計
- ✅ 使用 Composition API 和 `<script setup>`
- ✅ 自定義節點繼承自 BaseNode
- ✅ Props 和 Emits 類型定義完整

### 4. 狀態管理
- ✅ Pinia 管理全局狀態
- ✅ Composables 封裝可重用邏輯
- ✅ 響應式數據流清晰

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
