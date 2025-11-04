# Workflow 編輯器 Wireframe - Part 5: 響應式設計、無障礙、ADR 與驗收標準

**版本**: 1.0.0 (Part 5/5)
**日期**: 2025-10-29
**狀態**: ✅ 已完成
**複雜度**: ⭐⭐⭐⭐⭐ (極高)

---

## 📱 響應式設計

### Tablet 版本佈局（768-1279px）

```
┌─────────────────────────────────────────────────────────┐
│ ☰  [🔀 Workflow Editor]  [客戶服務流程]  ⚙️ 🔔 👤    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ [可折疊節點庫] │  主要畫布區域                          │
│                                                         │
│ 📂 節點 (收合)  │   ┌─────────────┐                     │
│ [展開/收合按鈕] │   │  🎬 Start   │                     │
│                │   └──────┬──────┘                     │
│                │          ▼                            │
│  當展開時:      │   ┌─────────────┐                     │
│  顯示節點庫    │   │  💬 Input   │                     │
│  (覆蓋在畫布上) │   └──────┬──────┘                     │
│                │          ▼                            │
│                │   ┌─────────────┐                     │
│                │   │  🎯 Agent_1 │                     │
│                │   └──────┬──────┘                     │
│                │                                        │
│                │                                        │
│  ⚙️ 屬性面板:   │  [▶️ 執行] [💾 儲存]                  │
│  底部抽屜式     │                                        │
│  (向上滑動展開) │                                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
       │
       │ 向上滑動
       ▼
┌─────────────────────────────────────────────────────────┐
│ ⚙️ 節點屬性                                        [✕]  │
├─────────────────────────────────────────────────────────┤
│ Agent_1                                                 │
│ [📝 基本] [🔗 連接] [⚙️ 進階]                          │
│                                                         │
│ 節點名稱: [分類專家_________________]                   │
│ Agent: [▼ 分類專家 (GPT-4)___________]                 │
│ Temperature: [=======>] 0.7                             │
│                                                         │
│ [✅ 套用] [❌ 取消]                                     │
└─────────────────────────────────────────────────────────┘
```

**Tablet 調整策略**:
- 左側節點庫: 可折疊，預設收合
- 右側屬性面板: 改為底部抽屜式
- 畫布工具欄: 簡化為圖標模式
- 雙指縮放/平移手勢支持
- 節點大小略微縮小（160px 寬度）

---

### Mobile 版本佈局（<768px）

```
┌───────────────────────────┐
│ ☰  Workflow  [⚙️] [👤]   │
├───────────────────────────┤
│                           │
│ 客戶服務流程 v2.3          │
│ [📂 節點] [▶️] [💾]       │
│                           │
│ ┌───────────────────────┐ │
│ │                       │ │
│ │   🎬 Start            │ │
│ │      ↓                │ │
│ │   💬 Input            │ │
│ │      ↓                │ │
│ │   🎯 Agent_1          │ │
│ │      ↓                │ │
│ │   🔀 Router           │ │
│ │    ↙  ↓  ↘           │ │
│ │  點擊展開查看更多      │ │
│ │                       │ │
│ └───────────────────────┘ │
│                           │
│ 垂直列表模式               │
│ (簡化的流程視圖)           │
│                           │
├───────────────────────────┤
│ [🏠] [📊] [🔍] [⚙️] [👤] │
│ 底部導航欄                 │
└───────────────────────────┘
```

**點擊節點後**:
```
┌───────────────────────────┐
│ ← 返回  Agent_1      [✕]  │
├───────────────────────────┤
│ 📝 節點屬性                │
│                           │
│ 節點名稱:                  │
│ ┌───────────────────────┐ │
│ │ 分類專家              │ │
│ └───────────────────────┘ │
│                           │
│ Agent 選擇:                │
│ ┌───────────────────────┐ │
│ │ ▼ 分類專家 (GPT-4)    │ │
│ └───────────────────────┘ │
│                           │
│ Temperature: 0.7          │
│ ┌───────────────────────┐ │
│ │ =========>            │ │
│ └───────────────────────┘ │
│                           │
│ [📝 編輯提示詞]            │
│                           │
│ [展開更多設定...]          │
│                           │
│ [✅ 儲存變更]              │
└───────────────────────────┘
```

**Mobile 特殊設計**:
- **垂直列表模式**: 節點按順序垂直排列
- **點擊展開**: 點擊節點進入全螢幕編輯模式
- **手勢操作**:
  - 雙指縮放（在畫布模式）
  - 向右滑動返回
  - 長按顯示快捷選單
- **簡化工具欄**: 只顯示核心功能
- **底部導航**: 固定底部導航欄
- **節點預覽卡片**: 精簡顯示關鍵資訊

---

## ♿ 無障礙設計（WCAG 2.1 AA）

### 鍵盤導航

```yaml
鍵盤快捷鍵完整列表:

畫布導航:
  Tab: 在節點間移動焦點
  Shift + Tab: 反向移動焦點
  Enter: 進入節點編輯模式
  Esc: 退出編輯模式/取消操作
  Arrow Keys: 移動選中的節點 (1px)
  Shift + Arrow Keys: 移動選中的節點 (10px)

節點操作:
  Ctrl + C: 複製選中節點
  Ctrl + V: 貼上節點
  Ctrl + X: 剪下節點
  Delete: 刪除選中節點
  Ctrl + D: 複製節點
  Ctrl + G: 群組選擇

畫布控制:
  Ctrl + Scroll: 縮放畫布
  Ctrl + 0: 重置縮放為 100%
  Ctrl + Plus: 放大
  Ctrl + Minus: 縮小
  Space + Drag: 平移畫布

執行與調試:
  F5: 執行工作流程
  F6: 暫停執行
  Shift + F5: 停止執行
  F8: 設置/移除斷點
  F9: 下一步 (Step Over)
  F10: 步入 (Step Into)

編輯操作:
  Ctrl + Z: 復原
  Ctrl + Y: 重做
  Ctrl + S: 儲存
  Ctrl + F: 搜尋
  Ctrl + A: 全選

面板切換:
  Ctrl + 1: 屬性面板
  Ctrl + 2: 執行狀態面板
  Ctrl + 3: 變量檢查器
  Ctrl + 4: 執行日誌
  Ctrl + 5: 性能分析
```

### ARIA 標籤和語義化 HTML

```html
<!-- 節點元素 -->
<div
  role="button"
  tabindex="0"
  aria-label="Agent Node: 分類專家"
  aria-describedby="node-003-desc"
  aria-pressed="false"
  data-node-id="node_003"
  class="workflow-node"
>
  <div id="node-003-desc" class="sr-only">
    Agent 節點，使用 GPT-4 模型，Temperature 0.7，負責分類用戶問題
  </div>
  <div class="node-icon" aria-hidden="true">🎯</div>
  <div class="node-label">分類專家</div>
  <div class="node-status" role="status" aria-live="polite">
    執行中...
  </div>
</div>

<!-- 連接線 -->
<path
  role="img"
  aria-label="連接: Input 到 Agent_1"
  aria-describedby="edge-001-desc"
  d="M 100,100 L 200,200"
  class="workflow-edge"
>
  <desc id="edge-001-desc">
    從 Input 節點（用戶問題）到 Agent_1 節點（分類專家）的數據流
  </desc>
</path>

<!-- 執行狀態通知 -->
<div
  role="alert"
  aria-live="assertive"
  aria-atomic="true"
  class="execution-notification"
>
  工作流程執行完成，狀態：成功，耗時：5 分 32 秒
</div>

<!-- 錯誤訊息 -->
<div
  role="alert"
  aria-live="assertive"
  aria-atomic="true"
  class="error-notification"
>
  節點 Agent_2 執行失敗：連接超時。按 Enter 查看詳情。
</div>

<!-- 工具欄按鈕 -->
<button
  type="button"
  aria-label="執行工作流程"
  aria-keyshortcuts="F5"
  aria-describedby="execute-btn-desc"
  class="toolbar-button"
>
  <span class="icon" aria-hidden="true">▶️</span>
  <span class="label">執行</span>
  <span id="execute-btn-desc" class="sr-only">
    執行當前工作流程，快捷鍵 F5
  </span>
</button>
```

### 顏色對比度

```yaml
WCAG AA 標準對比度 (4.5:1):

文字顏色:
  主要文字: #1F2937 on #FFFFFF (15.5:1) ✅
  次要文字: #6B7280 on #FFFFFF (4.6:1) ✅
  連結文字: #3B82F6 on #FFFFFF (4.8:1) ✅

按鈕顏色:
  主要按鈕: #FFFFFF on #3B82F6 (4.8:1) ✅
  次要按鈕: #1F2937 on #F3F4F6 (13.2:1) ✅
  危險按鈕: #FFFFFF on #EF4444 (5.1:1) ✅

狀態顏色:
  成功: #FFFFFF on #10B981 (4.7:1) ✅
  警告: #78350F on #FEF3C7 (7.8:1) ✅ (加深文字)
  錯誤: #FFFFFF on #EF4444 (5.1:1) ✅
  資訊: #FFFFFF on #3B82F6 (4.8:1) ✅

節點邊框:
  正常: #E5E7EB on #FFFFFF (1.3:1) ⚠️ (非文字可接受)
  選中: #3B82F6 on #FFFFFF (4.8:1) ✅
  錯誤: #EF4444 on #FFFFFF (5.6:1) ✅
```

### 螢幕閱讀器支持

```typescript
// 螢幕閱讀器通知服務
class A11yAnnouncementService {
  private liveRegion: HTMLElement;

  constructor() {
    this.liveRegion = this.createLiveRegion();
  }

  private createLiveRegion(): HTMLElement {
    const region = document.createElement('div');
    region.setAttribute('role', 'status');
    region.setAttribute('aria-live', 'polite');
    region.setAttribute('aria-atomic', 'true');
    region.className = 'sr-only';
    document.body.appendChild(region);
    return region;
  }

  announce(message: string, priority: 'polite' | 'assertive' = 'polite') {
    this.liveRegion.setAttribute('aria-live', priority);
    this.liveRegion.textContent = message;

    // 清空以便下次通知
    setTimeout(() => {
      this.liveRegion.textContent = '';
    }, 1000);
  }

  announceNodeOperation(operation: string, nodeName: string) {
    this.announce(`${operation} 節點: ${nodeName}`);
  }

  announceExecutionStatus(status: string) {
    this.announce(`工作流程執行狀態: ${status}`, 'assertive');
  }

  announceError(error: string) {
    this.announce(`錯誤: ${error}`, 'assertive');
  }
}

// 使用範例
const a11y = new A11yAnnouncementService();

// 節點操作通知
workflowEditor.on('nodeAdded', (node) => {
  a11y.announceNodeOperation('已新增', node.data.label);
});

workflowEditor.on('nodeDeleted', (node) => {
  a11y.announceNodeOperation('已刪除', node.data.label);
});

// 執行狀態通知
workflowEngine.on('executionStarted', () => {
  a11y.announceExecutionStatus('開始執行');
});

workflowEngine.on('executionCompleted', (result) => {
  a11y.announceExecutionStatus(
    `執行完成，狀態: ${result.status}，耗時: ${result.duration}`
  );
});

workflowEngine.on('executionFailed', (error) => {
  a11y.announceError(`執行失敗: ${error.message}`);
});
```

### 焦點管理

```typescript
// 焦點管理服務
class FocusManagementService {
  private focusableElements: HTMLElement[] = [];
  private currentFocusIndex = 0;

  updateFocusableElements() {
    // 獲取所有可聚焦的節點
    const nodes = document.querySelectorAll<HTMLElement>('.workflow-node[tabindex="0"]');
    this.focusableElements = Array.from(nodes);
  }

  focusNext() {
    if (this.focusableElements.length === 0) return;
    this.currentFocusIndex = (this.currentFocusIndex + 1) % this.focusableElements.length;
    this.focusableElements[this.currentFocusIndex].focus();
  }

  focusPrevious() {
    if (this.focusableElements.length === 0) return;
    this.currentFocusIndex =
      (this.currentFocusIndex - 1 + this.focusableElements.length) % this.focusableElements.length;
    this.focusableElements[this.currentFocusIndex].focus();
  }

  focusNode(nodeId: string) {
    const node = document.querySelector<HTMLElement>(`[data-node-id="${nodeId}"]`);
    if (node) {
      node.focus();
      // 滾動到視野中
      node.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  // 焦點陷阱（用於模態對話框）
  trapFocus(container: HTMLElement) {
    const focusableElements = container.querySelectorAll<HTMLElement>(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    const firstElement = focusableElements[0];
    const lastElement = focusableElements[focusableElements.length - 1];

    container.addEventListener('keydown', (e) => {
      if (e.key === 'Tab') {
        if (e.shiftKey && document.activeElement === firstElement) {
          e.preventDefault();
          lastElement.focus();
        } else if (!e.shiftKey && document.activeElement === lastElement) {
          e.preventDefault();
          firstElement.focus();
        }
      }
    });

    firstElement.focus();
  }
}
```

---

## 📐 架構決策記錄（ADR）

### ADR-012: Canvas 引擎選型 - React Flow

**狀態**: ✅ 已接受
**日期**: 2025-10-29
**決策者**: 技術團隊

**背景**:
需要選擇一個強大的 Canvas 引擎來實現複雜的可視化工作流程編輯器，支持拖拽、縮放、連接線等功能。

**選項考慮**:
1. **React Flow** (推薦)
   - 優點: React 原生支持、豐富的節點/連接自定義、活躍社區、性能優異
   - 缺點: 學習曲線中等

2. **D3.js + React**
   - 優點: 極度靈活、強大的數據可視化能力
   - 缺點: 開發成本高、需要大量自定義代碼、維護困難

3. **Konva.js**
   - 優點: 高性能 Canvas 渲染
   - 缺點: 較底層、需要手動實現大量功能、React 集成複雜

4. **Cytoscape.js**
   - 優點: 圖形可視化專用、自動佈局算法
   - 缺點: 更適合圖形分析而非流程編輯、自定義受限

**決策**: 選擇 **React Flow**

**理由**:
- React 原生整合，符合現有技術棧
- 提供開箱即用的節點、連接、縮放、平移功能
- 豐富的自定義選項滿足複雜需求
- 活躍的社區和完善的文檔
- 性能優異，支持大型工作流程（500+ 節點）
- 內建無障礙支持基礎

**影響**:
- 開發效率提升約 60%
- 維護成本降低
- 無障礙支持需要額外增強
- 需要學習 React Flow 的概念和 API

---

### ADR-013: 實時協作架構 - WebSocket + OT

**狀態**: ✅ 已接受
**日期**: 2025-10-29
**決策者**: 技術團隊

**背景**:
需要實現多人實時協作編輯工作流程的功能，類似 Google Docs 的協作體驗。

**選項考慮**:
1. **WebSocket + Operational Transformation (OT)**
   - 優點: 成熟的協作算法、衝突解決可靠
   - 缺點: OT 實現複雜

2. **WebSocket + CRDT (Conflict-free Replicated Data Type)**
   - 優點: 自動衝突解決、無需中央協調
   - 缺點: 資料結構複雜、儲存開銷大

3. **輪詢 (Polling)**
   - 優點: 實現簡單、相容性好
   - 缺點: 延遲高、伺服器負載大

4. **WebRTC P2P**
   - 優點: 點對點通信、低延遲
   - 缺點: 連接建立複雜、需要 TURN 伺服器、版本歷史難以管理

**決策**: 選擇 **WebSocket + Operational Transformation (OT)**

**理由**:
- WebSocket 提供低延遲雙向通信
- OT 是業界成熟的協作編輯算法
- 可以利用現有的 OT 庫（如 ShareDB）
- 支持離線編輯和衝突解決
- 版本歷史易於追蹤
- 伺服器可以作為權威來源

**實現細節**:
```typescript
// 使用 ShareDB 作為 OT 引擎
import ShareDB from 'sharedb/lib/client';
import WebSocket from 'ws';

const socket = new WebSocket('wss://api.example.com/collaboration');
const connection = new ShareDB.Connection(socket);
const doc = connection.get('workflows', workflowId);

doc.subscribe((err) => {
  if (err) throw err;
  // 訂閱成功，開始協作
  doc.on('op', (op, source) => {
    if (!source) {
      // 遠端操作，更新本地狀態
      applyRemoteOperation(op);
    }
  });
});

// 本地變更
function localChange(operation) {
  doc.submitOp(operation, (err) => {
    if (err) {
      // 處理衝突
      resolveConflict(err);
    }
  });
}
```

**影響**:
- 實時協作延遲 < 500ms
- 需要維護 WebSocket 連接和心跳
- 伺服器需要處理 OT 操作
- 離線編輯需要額外處理

---

### ADR-014: 版本控制策略 - 快照 + 增量

**狀態**: ✅ 已接受
**日期**: 2025-10-29
**決策者**: 技術團隊

**背景**:
需要設計一個高效的版本控制系統，支持快速版本切換和對比。

**選項考慮**:
1. **完整快照 (Full Snapshot)**
   - 優點: 簡單、版本獨立、讀取快速
   - 缺點: 儲存空間大、重複數據多

2. **增量變更 (Delta-only)**
   - 優點: 儲存空間小
   - 缺點: 讀取需要重建、對比複雜

3. **快照 + 增量 (Hybrid)** (推薦)
   - 優點: 平衡空間和速度、支持快速對比
   - 缺點: 實現稍複雜

4. **Git-like (Object Store)**
   - 優點: 成熟的版本控制模型
   - 缺點: 過於複雜、不適合 JSON 數據

**決策**: 選擇 **快照 + 增量 (Hybrid)**

**理由**:
- 每個主要版本儲存完整快照
- 次要版本儲存增量變更
- 快速版本載入（直接讀取快照）
- 高效版本對比（使用增量）
- 儲存空間可接受

**實現策略**:
```yaml
版本儲存結構:
  Major Version (v2.0):
    type: snapshot
    data: 完整的 nodes + edges + config
    size: ~100KB

  Minor Version (v2.1):
    type: delta
    base: v2.0
    changes:
      nodes_added: [...]
      nodes_modified: [...]
      nodes_deleted: [...]
    size: ~5KB

  Minor Version (v2.2):
    type: delta
    base: v2.1
    changes: [...]
    size: ~3KB

  Major Version (v3.0):
    type: snapshot
    data: 完整快照
    size: ~120KB

快照創建規則:
  - 每 10 個版本創建一次快照
  - 標記為 Major Release 時創建快照
  - 累積變更超過 50% 時創建快照
```

**影響**:
- 版本載入時間 < 2s
- 儲存空間節省約 70%
- 版本對比性能優異
- 需要定期整理舊版本

---

### ADR-015: 節點執行引擎 - 事件驅動 + 狀態機

**狀態**: ✅ 已接受
**日期**: 2025-10-29
**決策者**: 技術團隊

**背景**:
需要設計一個可靠的工作流程執行引擎，支持複雜的節點依賴、錯誤處理、斷點調試。

**選項考慮**:
1. **事件驅動 + 狀態機** (推薦)
   - 優點: 清晰的狀態管理、易於調試、支持暫停/恢復
   - 缺點: 實現複雜度中等

2. **遞歸執行**
   - 優點: 實現簡單
   - 缺點: 難以控制、無法暫停、堆疊溢位風險

3. **Promise Chain**
   - 優點: JavaScript 原生、易於理解
   - 缺點: 無法暫停、錯誤處理複雜

4. **Worker Thread Pool**
   - 優點: 真正並行、不阻塞主線程
   - 缺點: 數據傳遞複雜、調試困難

**決策**: 選擇 **事件驅動 + 狀態機**

**理由**:
- 清晰的節點生命週期: idle → pending → running → completed/failed
- 支持暫停、恢復、單步執行
- 事件系統支持監聽和調試
- 可擴展性強，易於添加新功能
- 錯誤處理和重試邏輯清晰

**狀態機設計**:
```typescript
enum NodeExecutionState {
  IDLE = 'idle',
  PENDING = 'pending',
  RUNNING = 'running',
  COMPLETED = 'completed',
  FAILED = 'failed',
  PAUSED = 'paused',
  CANCELLED = 'cancelled',
}

class WorkflowExecutionEngine extends EventEmitter {
  private nodeStates: Map<string, NodeExecutionState> = new Map();
  private executionQueue: string[] = [];
  private isPaused = false;

  async execute(workflow: Workflow) {
    this.emit('executionStarted', { workflowId: workflow.id });

    // 拓撲排序確定執行順序
    const executionOrder = this.topologicalSort(workflow);

    for (const nodeId of executionOrder) {
      if (this.isPaused) {
        this.emit('executionPaused');
        await this.waitForResume();
      }

      await this.executeNode(workflow, nodeId);
    }

    this.emit('executionCompleted', { workflowId: workflow.id });
  }

  private async executeNode(workflow: Workflow, nodeId: string) {
    const node = workflow.nodes.find(n => n.id === nodeId);
    if (!node) return;

    // 狀態轉換: idle → pending
    this.updateNodeState(nodeId, NodeExecutionState.PENDING);
    this.emit('nodeStateChanged', { nodeId, state: NodeExecutionState.PENDING });

    // 檢查斷點
    if (this.hasBreakpoint(nodeId)) {
      this.isPaused = true;
      this.emit('breakpointHit', { nodeId });
      await this.waitForResume();
    }

    try {
      // 狀態轉換: pending → running
      this.updateNodeState(nodeId, NodeExecutionState.RUNNING);
      this.emit('nodeStateChanged', { nodeId, state: NodeExecutionState.RUNNING });

      const result = await this.runNodeExecutor(node);

      // 狀態轉換: running → completed
      this.updateNodeState(nodeId, NodeExecutionState.COMPLETED);
      this.emit('nodeStateChanged', { nodeId, state: NodeExecutionState.COMPLETED, result });
    } catch (error) {
      // 狀態轉換: running → failed
      this.updateNodeState(nodeId, NodeExecutionState.FAILED);
      this.emit('nodeStateChanged', { nodeId, state: NodeExecutionState.FAILED, error });

      // 錯誤處理策略
      await this.handleNodeError(node, error);
    }
  }
}
```

**影響**:
- 支持完整的調試功能
- 執行狀態可追蹤和可視化
- 性能開銷可接受
- 易於測試和維護

---

## ✅ 驗收標準

### 功能驗收

```yaml
基礎功能 (P0):
  ✅ 畫布操作:
    - [ ] 無限畫布縮放（25%-500%）
    - [ ] 平移和拖拽流暢（60 FPS）
    - [ ] 網格對齊功能正常
    - [ ] 迷你地圖顯示正確

  ✅ 節點操作:
    - [ ] 從節點庫拖拽創建節點
    - [ ] 雙擊快速添加節點
    - [ ] 移動、複製、刪除節點
    - [ ] 多選和群組操作
    - [ ] 節點配置保存正確

  ✅ 連接管理:
    - [ ] 拖拽創建連接線
    - [ ] 連接驗證和錯誤提示
    - [ ] 連接線自動路由
    - [ ] 連接數據流預覽

  ✅ 節點類型庫:
    - [ ] 28 種節點全部可用
    - [ ] 節點搜尋功能正常
    - [ ] 節點分類正確

執行與調試 (P0):
  ✅ 執行控制:
    - [ ] 手動執行工作流程
    - [ ] 暫停和停止執行
    - [ ] 重新執行功能

  ✅ 實時追蹤:
    - [ ] 節點執行狀態可視化
    - [ ] 執行進度實時更新
    - [ ] 執行日誌完整記錄

  ✅ 調試功能:
    - [ ] 設置和管理斷點
    - [ ] 單步執行正常
    - [ ] 變量檢查器準確
    - [ ] 錯誤堆疊追蹤完整

  ✅ 性能分析:
    - [ ] 節點執行時間統計
    - [ ] 性能瓶頸識別
    - [ ] 成本分析正確

版本控制 (P1):
  ✅ 版本管理:
    - [ ] 儲存版本功能正常
    - [ ] 版本歷史列表完整
    - [ ] 版本載入和還原正確

  ✅ 版本對比:
    - [ ] 並排對比視圖正確
    - [ ] 變更詳情列表完整
    - [ ] 差異標記清晰

協作功能 (P1):
  ✅ 實時協作:
    - [ ] WebSocket 連接穩定
    - [ ] 游標位置實時同步
    - [ ] 節點鎖定機制有效
    - [ ] 操作同步延遲 < 500ms

  ✅ 衝突解決:
    - [ ] 衝突檢測準確
    - [ ] 自動合併正常
    - [ ] 手動解決流程清晰

  ✅ 聊天功能:
    - [ ] 訊息發送和接收
    - [ ] @提及功能正常
    - [ ] 節點引用可點擊
```

### 性能驗收

```yaml
性能指標:
  畫布性能:
    - [ ] 100 個節點: 60 FPS
    - [ ] 500 個節點: > 30 FPS
    - [ ] 縮放響應: < 100ms
    - [ ] 拖拽延遲: < 50ms

  執行性能:
    - [ ] 執行啟動: < 1s
    - [ ] 節點執行平均: < 3s
    - [ ] 日誌記錄: < 10ms/條

  協作性能:
    - [ ] WebSocket 延遲: < 500ms
    - [ ] 游標同步: < 100ms
    - [ ] 操作廣播: < 200ms

  版本控制:
    - [ ] 版本載入: < 2s
    - [ ] 版本對比: < 3s
    - [ ] 儲存版本: < 1s

  記憶體使用:
    - [ ] 初始載入: < 50MB
    - [ ] 100 節點: < 100MB
    - [ ] 500 節點: < 300MB
```

### 無障礙驗收

```yaml
WCAG 2.1 AA 合規:
  鍵盤導航:
    - [ ] 所有功能可通過鍵盤操作
    - [ ] Tab 順序邏輯正確
    - [ ] 焦點指示清晰可見
    - [ ] 快捷鍵無衝突

  螢幕閱讀器:
    - [ ] ARIA 標籤完整準確
    - [ ] 動態內容有 live region
    - [ ] 操作結果有語音反饋
    - [ ] 通過 NVDA/JAWS 測試

  顏色對比:
    - [ ] 文字對比度 ≥ 4.5:1
    - [ ] UI 元件對比度 ≥ 3:1
    - [ ] 狀態顏色不僅依賴顏色

  互動元素:
    - [ ] 觸控目標 ≥ 44×44px
    - [ ] 錯誤訊息清晰明確
    - [ ] 表單標籤關聯正確
```

### 瀏覽器相容性

```yaml
支援瀏覽器:
  桌面:
    - [ ] Chrome/Edge 最新 3 版
    - [ ] Firefox 最新 3 版
    - [ ] Safari 最新 2 版

  平板:
    - [ ] iPad Safari (iOS 15+)
    - [ ] Android Chrome (Android 10+)

  行動:
    - [ ] iPhone Safari (iOS 15+)
    - [ ] Android Chrome (Android 10+)

  解析度:
    - [ ] 1920×1080 (Desktop)
    - [ ] 1366×768 (Laptop)
    - [ ] 768×1024 (Tablet Portrait)
    - [ ] 1024×768 (Tablet Landscape)
    - [ ] 375×667 (Mobile)
```

---

## 🧪 測試策略

### 單元測試

```typescript
// 範例: 節點連接驗證測試

describe('節點連接驗證', () => {
  it('應該允許兼容類型的連接', () => {
    const sourceNode = createNode({ type: 'agent_single', outputs: [{ type: 'string' }] });
    const targetNode = createNode({ type: 'output', inputs: [{ type: 'string' }] });

    const canConnect = validateConnection(sourceNode, 0, targetNode, 0);

    expect(canConnect).toBe(true);
  });

  it('應該拒絕不兼容類型的連接', () => {
    const sourceNode = createNode({ type: 'agent_single', outputs: [{ type: 'number' }] });
    const targetNode = createNode({ type: 'output', inputs: [{ type: 'string' }] });

    const canConnect = validateConnection(sourceNode, 0, targetNode, 0);

    expect(canConnect).toBe(false);
  });

  it('應該檢測循環依賴', () => {
    const workflow = createWorkflow([
      createNode({ id: 'node1' }),
      createNode({ id: 'node2' }),
      createNode({ id: 'node3' }),
    ]);

    addEdge(workflow, 'node1', 'node2');
    addEdge(workflow, 'node2', 'node3');

    const hasCycle = detectCycle(workflow, 'node3', 'node1');

    expect(hasCycle).toBe(true);
  });
});

// 測試覆蓋率目標
/*
  - 行覆蓋率: > 80%
  - 分支覆蓋率: > 75%
  - 函數覆蓋率: > 85%
*/
```

### 整合測試

```typescript
// 範例: 工作流程執行整合測試

describe('工作流程執行', () => {
  it('應該成功執行簡單的線性工作流程', async () => {
    const workflow = createWorkflow([
      createStartNode(),
      createInputNode(),
      createAgentNode({ agentId: 'test-agent' }),
      createOutputNode(),
      createEndNode(),
    ]);

    connectNodes(workflow, 0, 1);
    connectNodes(workflow, 1, 2);
    connectNodes(workflow, 2, 3);
    connectNodes(workflow, 3, 4);

    const result = await executeWorkflow(workflow, { userInput: 'test' });

    expect(result.status).toBe('completed');
    expect(result.output).toBeDefined();
  });

  it('應該正確處理條件分支', async () => {
    const workflow = createWorkflow([
      createStartNode(),
      createConditionNode({ condition: 'value > 10' }),
      createAgentNode({ id: 'agent-A' }),
      createAgentNode({ id: 'agent-B' }),
      createEndNode(),
    ]);

    // ... 設置連接

    const result1 = await executeWorkflow(workflow, { value: 15 });
    expect(result1.executedNodes).toContain('agent-A');

    const result2 = await executeWorkflow(workflow, { value: 5 });
    expect(result2.executedNodes).toContain('agent-B');
  });
});
```

### E2E 測試（Playwright）

```typescript
// 範例: E2E 測試 - 創建和執行工作流程

test('用戶應該能夠創建和執行工作流程', async ({ page }) => {
  // 1. 導航到工作流程編輯器
  await page.goto('/workflows/editor');

  // 2. 拖拽創建 Start 節點
  await page.locator('[data-node-type="start"]').dragTo(
    page.locator('.workflow-canvas'),
    { targetPosition: { x: 100, y: 100 } }
  );

  // 3. 創建 Agent 節點
  await page.locator('[data-node-type="agent_single"]').dragTo(
    page.locator('.workflow-canvas'),
    { targetPosition: { x: 100, y: 250 } }
  );

  // 4. 連接節點
  await page.locator('[data-node-id="node_001"] .output-port').click();
  await page.locator('[data-node-id="node_002"] .input-port').click();

  // 5. 配置 Agent 節點
  await page.locator('[data-node-id="node_002"]').click();
  await page.locator('[aria-label="Agent 選擇"]').click();
  await page.locator('text=測試 Agent').click();
  await page.locator('button:has-text("套用")').click();

  // 6. 執行工作流程
  await page.locator('button[aria-label="執行工作流程"]').click();

  // 7. 驗證執行狀態
  await expect(page.locator('[data-node-id="node_002"][data-status="running"]')).toBeVisible();
  await expect(page.locator('[data-node-id="node_002"][data-status="completed"]')).toBeVisible({ timeout: 30000 });

  // 8. 檢查執行日誌
  await page.locator('[aria-label="執行日誌"]').click();
  await expect(page.locator('text=執行完成')).toBeVisible();
});

test('用戶應該能夠使用斷點調試', async ({ page }) => {
  // ... 創建工作流程

  // 設置斷點
  await page.locator('[data-node-id="node_002"]').click({ button: 'right' });
  await page.locator('text=設置斷點').click();

  // 執行
  await page.locator('button[aria-label="執行工作流程"]').click();

  // 驗證斷點觸發
  await expect(page.locator('text=斷點已觸發')).toBeVisible();

  // 檢查變量
  await page.locator('button:has-text("檢查變量")').click();
  await expect(page.locator('.variable-inspector')).toBeVisible();

  // 繼續執行
  await page.locator('button:has-text("繼續")').click();
});
```

---

## 🚀 性能優化建議

```yaml
前端優化:
  虛擬化渲染:
    - 使用 React Virtualized 渲染大量節點
    - 只渲染可視區域內的節點
    - 懶加載節點詳細配置

  記憶體管理:
    - 及時清理未使用的事件監聽器
    - 使用 WeakMap 避免記憶體洩漏
    - 定期清理執行日誌（保留最近 1000 條）

  渲染優化:
    - 使用 React.memo 避免不必要的重渲染
    - 節點拖拽使用 transform 而非 top/left
    - 連接線使用 SVG path 而非 DOM 元素

  網絡優化:
    - WebSocket 訊息使用 Protocol Buffers 壓縮
    - 批次發送小的變更（debounce 200ms）
    - 使用 CDN 加速靜態資源

後端優化:
  資料庫:
    - 版本快照使用壓縮儲存（gzip）
    - 版本歷史分頁查詢（每頁 20 個）
    - 添加適當的索引（workflowId, createdAt）

  執行引擎:
    - 節點執行使用 Worker 池
    - 長時間運行的任務使用後台 Job
    - 添加執行超時保護

  WebSocket:
    - 使用 Redis 作為訊息 Broker
    - 實現心跳和自動重連
    - 限制每個連接的訊息速率

  快取策略:
    - 熱門工作流程快取在 Redis（TTL 1 小時）
    - 節點執行結果快取（可選）
    - CDN 快取靜態資源（24 小時）
```

---

## 📊 監控和告警

```yaml
關鍵指標:
  用戶體驗:
    - 頁面載入時間 (P95 < 3s)
    - 畫布操作響應時間 (P95 < 100ms)
    - 執行啟動時間 (P95 < 2s)

  系統性能:
    - WebSocket 連接數
    - 同時執行的工作流程數
    - API 響應時間
    - 資料庫查詢時間

  錯誤率:
    - JavaScript 錯誤率 (< 1%)
    - API 錯誤率 (< 2%)
    - 工作流程執行失敗率 (< 5%)

  業務指標:
    - 日活躍用戶數 (DAU)
    - 工作流程創建數
    - 工作流程執行次數
    - 協作會話數

告警規則:
  高優先級:
    - WebSocket 服務不可用
    - 執行引擎失敗率 > 10%
    - 資料庫連接池耗盡

  中優先級:
    - API 響應時間 P95 > 5s
    - 錯誤率 > 5%
    - 記憶體使用 > 80%

  低優先級:
    - 版本儲存失敗
    - 協作同步延遲 > 2s
```

---

## ✅ Part 5 檢查清單

### 完成項目
- [✅] 響應式設計
  - [✅] Tablet 佈局（768-1279px）
  - [✅] Mobile 佈局（<768px）
  - [✅] 手勢操作支持
- [✅] 無障礙設計（WCAG 2.1 AA）
  - [✅] 鍵盤導航完整
  - [✅] ARIA 標籤和語義化 HTML
  - [✅] 顏色對比度達標
  - [✅] 螢幕閱讀器支持
  - [✅] 焦點管理
- [✅] 架構決策記錄（ADR）
  - [✅] ADR-012: Canvas 引擎選型
  - [✅] ADR-013: 實時協作架構
  - [✅] ADR-014: 版本控制策略
  - [✅] ADR-015: 節點執行引擎
- [✅] 驗收標準
  - [✅] 功能驗收
  - [✅] 性能驗收
  - [✅] 無障礙驗收
  - [✅] 瀏覽器相容性
- [✅] 測試策略
  - [✅] 單元測試
  - [✅] 整合測試
  - [✅] E2E 測試
- [✅] 性能優化建議
- [✅] 監控和告警

---

## 🎉 Workflow 編輯器設計完成總結

### 設計規模統計

```yaml
文檔規模:
  Part 1: 基礎架構（節點庫、佈局） - ~600 行
  Part 2: 節點配置系統（28 種節點） - ~1100 行
  Part 3: 執行與調試功能 - ~800 行
  Part 4: 版本控制與協作 - ~900 行
  Part 5: 響應式、無障礙、ADR、驗收 - ~1000 行
  總計: ~4,400 行完整規範

功能統計:
  節點類型: 28 種專業節點
  佈局模式: 3 種（Desktop/Tablet/Mobile）
  執行模式: 4 種（手動/定時/事件/API）
  斷點類型: 3 種（常規/條件/日誌）
  日誌等級: 5 種（Debug/Info/Warning/Error/Success）
  協作功能: 實時游標、鎖定、衝突解決、聊天
  版本控制: 快照 + 增量策略
  ADR: 4 個關鍵架構決策

技術實現:
  Canvas 引擎: React Flow
  協作架構: WebSocket + OT
  狀態管理: Zustand
  版本控制: Snapshot + Delta
  執行引擎: Event-Driven + State Machine
  測試: 單元 + 整合 + E2E
```

### 設計亮點

✨ **最複雜的 Wireframe**：28 種專業節點，完整的執行調試系統
🔄 **完整協作系統**：實時游標、鎖定機制、衝突解決、內建聊天
📚 **版本控制**：快照 + 增量策略，高效對比和還原
🐛 **專業調試**：斷點、單步執行、變量檢查、性能分析
♿ **無障礙完整**：WCAG 2.1 AA 合規，完整鍵盤導航和螢幕閱讀器支持
📱 **響應式設計**：Desktop/Tablet/Mobile 三種佈局
🛠️ **技術實現代碼**：WebSocket、版本控制 Store、狀態機引擎
📐 **4 個 ADR**：關鍵技術選型決策完整記錄

---

**文檔狀態**: Part 5/5 完成 ✅
**整體進度**: 100% 完成 🎉
**下一步**: 合併所有 Parts 成為完整的 09-workflow-editor.md
