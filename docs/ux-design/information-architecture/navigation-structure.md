# Navigation Structure - Semantic Kernel Agentic Framework

## 概覽

本文件定義了 Semantic Kernel Agentic Framework 的導航結構，包括主導航、次級導航、面包屑導航、以及移動端適配方案。

**設計目標**:
- **快速訪問**: 核心功能 ≤2 次點擊可達
- **清晰層次**: 導航結構反映用戶心智模型
- **一致性**: 全站統一的導航模式
- **可擴展性**: 支持未來功能添加

---

## 導航體系架構

```
全局導航體系
│
├─ 主導航（Primary Navigation）
│  └─ 左側垂直導航欄（10 個一級菜單）
│
├─ 次級導航（Secondary Navigation）
│  ├─ 頂部標籤頁（Tab Navigation）
│  └─ 左側子菜單（Sub-menu）
│
├─ 上下文導航（Contextual Navigation）
│  ├─ 面包屑（Breadcrumb）
│  └─ 頁內跳轉（Anchor Links）
│
└─ 工具導航（Utility Navigation）
   ├─ 頂部工具欄（User Menu, Notifications, Search）
   └─ 快捷操作（FAB, Shortcuts, Context Menu）
```

---

## 一、主導航 (Primary Navigation)

### 1.1 左側垂直導航欄

**位置**: 屏幕左側，固定
**寬度**: 展開 240px，折疊 64px
**高度**: 100vh（全屏高度）

**導航項目**:

```yaml
主導航結構:
  - id: home
    label: "首頁"
    icon: "🏠"
    route: "/"
    roles: [admin, developer, analyst, viewer]
    order: 1

  - id: agents
    label: "Agents"
    icon: "🤖"
    route: "/agents"
    roles: [admin, developer]
    order: 2
    badge: "New" # 可選: 顯示徽章（如新功能）
    children: # 可選: 子菜單
      - label: "Agent 列表"
        route: "/agents/list"
      - label: "創建 Agent"
        route: "/agents/new"
      - label: "模板庫"
        route: "/agents/templates"

  - id: conversations
    label: "對話"
    icon: "💬"
    route: "/conversations"
    roles: [admin, developer, analyst, viewer]
    order: 3

  - id: knowledge
    label: "知識庫"
    icon: "🧠"
    route: "/knowledge"
    roles: [admin, developer, analyst]
    order: 4

  - id: code-interpreter
    label: "Code Interpreter"
    icon: "🔬"
    route: "/code-interpreter"
    roles: [admin, analyst]
    order: 5

  - id: text-to-sql
    label: "Text-to-SQL"
    icon: "📊"
    route: "/text-to-sql"
    roles: [admin, analyst]
    order: 6

  - id: workflows
    label: "工作流"
    icon: "🔄"
    route: "/workflows"
    roles: [admin, developer]
    order: 7

  - id: personas
    label: "Persona"
    icon: "👤"
    route: "/personas"
    roles: [admin, developer]
    order: 8

  - id: monitoring
    label: "監控"
    icon: "📈"
    route: "/monitoring"
    roles: [admin]
    order: 9
    visibility: admin_only # 僅管理員可見

  - id: settings
    label: "設置"
    icon: "⚙️"
    route: "/settings"
    roles: [admin]
    order: 10
    visibility: admin_only

  - id: help
    label: "幫助"
    icon: "📚"
    route: "/help"
    roles: [admin, developer, analyst, viewer]
    order: 11
    position: bottom # 固定在導航欄底部
```

### 1.2 導航狀態設計

**視覺狀態**:
- **默認狀態**: 灰色圖標 + 白色文字（展開模式）
- **懸停狀態**: 背景色淺灰 + 高亮圖標
- **激活狀態**: 背景色主色調 + 高亮圖標 + 粗體文字 + 左側指示條
- **禁用狀態**: 灰色半透明（無權限時）

**交互行為**:
- 點擊: 導航到對應頁面
- 懸停: 顯示 Tooltip（折疊模式下）
- 展開/折疊: 點擊導航欄底部的折疊按鈕

**折疊模式**:
```
展開模式（240px）:
┌──────────────────┐
│ 🏠 首頁          │
│ 🤖 Agents        │
│ 💬 對話          │
│ ...              │
└──────────────────┘

折疊模式（64px）:
┌────┐
│ 🏠 │ ← Tooltip: "首頁"
│ 🤖 │ ← Tooltip: "Agents"
│ 💬 │ ← Tooltip: "對話"
│ ...│
└────┘
```

### 1.3 子菜單設計

**觸發方式**:
- **展開模式**: 點擊父菜單項展開子菜單（accordion 模式）
- **折疊模式**: 懸停時彈出浮動子菜單

**子菜單示例**:
```
🤖 Agents ▼
  ├─ Agent 列表
  ├─ 創建 Agent
  └─ 模板庫
```

**視覺設計**:
- 子菜單項縮進 16px
- 子菜單背景色略深
- 子菜單圖標尺寸略小（16px vs 20px）

---

## 二、次級導航 (Secondary Navigation)

### 2.1 頂部標籤頁導航 (Tab Navigation)

**使用場景**: 同一實體的不同視圖（如 Agent 詳情頁的不同配置區）

**設計規範**:
```yaml
標籤頁配置:
  位置: 頁面頂部（頁面標題下方）
  樣式: 下劃線標籤（Material Design Tabs）
  最大數量: 5-7 個標籤
  響應式: 移動端轉為下拉選擇器
```

**示例: Agent 詳情頁**:
```
Agent 詳情 > GPT-4 客服助手
┌─────────────────────────────────────────┐
│ 📝 基本配置 │ 👤 Persona │ 🔧 能力 │ 🧪 測試 │ 🚀 部署 │
└─┬───────────────────────────────────────┘
  └─ 激活指示器（藍色下劃線）
```

**視覺狀態**:
- **默認**: 灰色文字
- **懸停**: 文字變深色
- **激活**: 主色調文字 + 底部藍色指示條
- **禁用**: 灰色半透明（某些狀態下不可用）

### 2.2 左側子菜單 (Sub-menu in Sidebar)

**使用場景**: 功能模組的細分導航（如系統設置的子頁面）

**示例: 系統設置**:
```
⚙️ 設置
├─ 🔧 基本設置
├─ 🛡️ 安全與權限
│  ├─ 用戶管理
│  ├─ 角色與權限
│  └─ 審計日誌
├─ 🔌 集成管理
└─ 🔨 系統維護
```

**設計規範**:
- 最多 3 層嵌套
- 使用縮進表示層級（每層 16px）
- 支持展開/折疊

---

## 三、上下文導航 (Contextual Navigation)

### 3.1 面包屑導航 (Breadcrumb)

**位置**: 頁面頂部，主導航下方
**格式**: `首頁 > 一級 > 二級 > 三級 > 當前頁`

**設計規範**:
```yaml
面包屑配置:
  最大層級: 4 層
  分隔符: ">"
  樣式: 文字鏈接 + 分隔符
  當前頁: 灰色不可點擊
  移動端: 收起為 "< 返回" 按鈕
```

**示例**:
```
首頁 > Agents 管理 > Agent 詳情 > Persona 配置
^可點擊 ^可點擊    ^可點擊      ^當前頁（不可點擊）
```

**移動端適配**:
```
桌面端: 首頁 > Agents 管理 > Agent 詳情 > Persona 配置
移動端: < 返回
```

### 3.2 頁內跳轉 (Anchor Navigation)

**使用場景**: 長頁面內的不同區域跳轉

**設計模式**: 右側固定目錄（Table of Contents）

**示例: 幫助文檔頁**:
```
┌─────────────────────────┬─────────────┐
│ 文檔內容                 │ 目錄        │
│                         │ • 快速開始  │
│ ## 快速開始              │ • 創建 Agent│
│ ...                     │ • 配置能力  │
│                         │ • 測試部署  │
│ ## 創建 Agent            │             │
│ ...                     │             │
└─────────────────────────┴─────────────┘
```

**交互行為**:
- 點擊目錄項: 平滑滾動到對應區域
- 滾動頁面: 目錄自動高亮當前區域
- 移動端: 目錄收起為下拉菜單

---

## 四、工具導航 (Utility Navigation)

### 4.1 頂部工具欄

**位置**: 屏幕頂部，固定
**高度**: 64px

**組件佈局**:
```
┌──────────────────────────────────────────────────────────┐
│ [Logo]  [全局搜索框]            [🔔通知] [👤用戶菜單]      │
└──────────────────────────────────────────────────────────┘
  ↑        ↑                        ↑        ↑
  Logo    Search                  Bell    User Menu
```

#### 4.1.1 全局搜索 (Global Search)

**觸發方式**:
- 點擊搜索框
- 快捷鍵: `Ctrl/Cmd + K`

**搜索範圍**:
- Agents
- 對話
- 知識庫
- 工作流
- Personas
- 幫助文檔

**搜索結果分組**:
```yaml
搜索結果結構:
  - category: "Agents"
    items:
      - title: "客服助手 Agent"
        route: "/agents/123"
        description: "智能客服解決方案"
        icon: "🤖"

  - category: "工作流"
    items:
      - title: "客服流程"
        route: "/workflows/456"
        description: "多 Agent 客服工作流"
        icon: "🔄"

  - category: "幫助文檔"
    items:
      - title: "如何創建客服 Agent"
        route: "/help/create-agent"
        description: "詳細步驟教學"
        icon: "📚"
```

**視覺設計**:
- 全屏覆蓋模式（Modal）
- 結果分組顯示
- 高亮匹配關鍵詞
- 鍵盤導航支持（↑↓選擇，Enter打開）

#### 4.1.2 通知中心 (Notification Center)

**觸發方式**: 點擊鈴鐺圖標

**通知類型**:
- 系統通知（系統維護、更新）
- Agent 狀態變更（啟動/停止/錯誤）
- 任務完成（工作流執行完成、數據分析完成）
- 告警通知（錯誤率過高、成本超標）

**通知面板結構**:
```
通知中心
├─ 標題欄
│  ├─ "通知" (左)
│  └─ "全部標為已讀" (右)
│
├─ 通知列表
│  ├─ 通知 1 (未讀 • 高亮背景)
│  │  ├─ [圖標] 標題
│  │  ├─ 內容摘要
│  │  └─ 時間
│  ├─ 通知 2 (已讀)
│  └─ ...
│
└─ 底部鏈接
   └─ "查看全部通知"
```

**徽章數量**: 顯示未讀通知數量（最多 99+）

#### 4.1.3 用戶菜單 (User Menu)

**觸發方式**: 點擊用戶頭像

**菜單項**:
```yaml
用戶菜單:
  - label: "個人資料"
    icon: "👤"
    route: "/profile"

  - label: "API Keys"
    icon: "🔑"
    route: "/settings/api-keys"

  - label: "偏好設置"
    icon: "⚙️"
    route: "/settings/preferences"

  - separator: true

  - label: "幫助與支持"
    icon: "❓"
    route: "/help"

  - label: "反饋"
    icon: "💬"
    action: "openFeedbackModal"

  - separator: true

  - label: "登出"
    icon: "🚪"
    action: "logout"
```

### 4.2 快捷操作 (Quick Actions)

#### 4.2.1 懸浮操作按鈕 (FAB - Floating Action Button)

**位置**: 屏幕右下角，固定
**尺寸**: 56x56px (圓形按鈕)

**主要操作**:
```yaml
主 FAB 操作（根據當前頁面上下文）:
  - 首頁: "創建 Agent" (+)
  - Agent 列表: "創建 Agent" (+)
  - 對話列表: "新建對話" (+)
  - 知識庫列表: "創建知識庫" (+)
  - 工作流列表: "創建工作流" (+)
```

**擴展 FAB（可選）**:
```
點擊主 FAB → 展開多個子操作

         [創建 Agent] ←
      [新建對話] ←
   [數據分析] ←
[+] ← 主按鈕
```

#### 4.2.2 全局快捷鍵 (Keyboard Shortcuts)

**快捷鍵列表**:
```yaml
全局快捷鍵:
  - key: "Ctrl/Cmd + K"
    action: "打開全局搜索"

  - key: "Ctrl/Cmd + N"
    action: "創建新 Agent"

  - key: "Ctrl/Cmd + E"
    action: "打開 Code Interpreter"

  - key: "Ctrl/Cmd + Q"
    action: "打開 Text-to-SQL"

  - key: "Ctrl/Cmd + /"
    action: "打開快捷鍵幫助"

  - key: "Ctrl/Cmd + B"
    action: "折疊/展開左側導航"

  - key: "Esc"
    action: "關閉當前彈窗/Modal"

頁面內快捷鍵:
  - key: "Tab"
    action: "在可聚焦元素間跳轉"

  - key: "Enter"
    action: "確認/提交"

  - key: "Ctrl/Cmd + S"
    action: "保存當前編輯"

  - key: "Ctrl/Cmd + Z"
    action: "撤銷"

  - key: "Ctrl/Cmd + Shift + Z"
    action: "重做"
```

**快捷鍵幫助面板**:
- 觸發: `Ctrl/Cmd + /`
- 顯示所有可用快捷鍵
- 按功能分組顯示

#### 4.2.3 右鍵上下文菜單 (Context Menu)

**使用場景**: 列表項、卡片、節點等元素

**示例: Agent 列表項右鍵菜單**:
```yaml
Agent 右鍵菜單:
  - label: "啟動 Agent"
    icon: "▶️"
    action: "startAgent"
    condition: "status === 'stopped'"

  - label: "停止 Agent"
    icon: "⏸️"
    action: "stopAgent"
    condition: "status === 'running'"

  - label: "測試對話"
    icon: "💬"
    action: "openTestChat"

  - label: "查看詳情"
    icon: "👁️"
    action: "viewDetails"

  - separator: true

  - label: "複製 Agent"
    icon: "📋"
    action: "cloneAgent"

  - label: "導出配置"
    icon: "📥"
    action: "exportConfig"

  - separator: true

  - label: "刪除"
    icon: "🗑️"
    action: "deleteAgent"
    color: "red"
    confirm: true
```

---

## 五、移動端導航適配

### 5.1 響應式斷點

```yaml
斷點定義:
  mobile: "< 768px"
  tablet: "768px - 1024px"
  desktop: "> 1024px"
```

### 5.2 移動端導航模式

#### 5.2.1 頂部導航欄（移動端）

```
┌────────────────────────────┐
│ [☰] Logo    [🔔] [👤]       │
└────────────────────────────┘
  ↑             ↑    ↑
 Menu          Bell User
```

**組件變更**:
- 漢堡菜單（☰）: 打開左側抽屜導航
- 移除全局搜索框 → 移至抽屜導航頂部
- 保留通知和用戶菜單

#### 5.2.2 抽屜導航 (Drawer Navigation)

**觸發方式**: 點擊漢堡菜單圖標 / 從左側邊緣滑入

**抽屜內容**:
```
┌─────────────────────┐
│ [搜索框]             │
├─────────────────────┤
│ 🏠 首頁              │
│ 🤖 Agents            │
│   ├─ Agent 列表      │
│   ├─ 創建 Agent      │
│   └─ 模板庫          │
│ 💬 對話              │
│ 🧠 知識庫            │
│ ...                 │
├─────────────────────┤
│ 📚 幫助              │
│ ⚙️ 設置              │
└─────────────────────┘
```

**交互行為**:
- 點擊菜單項 → 導航到頁面 + 自動關閉抽屜
- 點擊遮罩 → 關閉抽屜
- 從左側滑入 → 打開抽屜
- 從右側滑出 → 關閉抽屜

#### 5.2.3 底部導航欄 (Bottom Navigation)

**可選方案**: 對於高頻使用的 App 場景

```
┌────────────────────────────────────┐
│        頁面內容                     │
└────────────────────────────────────┘
┌────────────────────────────────────┐
│ 🏠   🤖    💬    🔬    👤          │
│ 首頁 Agents 對話  分析  我的        │
└────────────────────────────────────┘
```

**設計規範**:
- 最多 5 個主要功能
- 圖標 + 文字標籤
- 當前激活項高亮

### 5.3 標籤頁適配

**桌面端**: 水平標籤頁（Tab Navigation）
```
┌──────────────────────────────────┐
│ 基本配置 │ Persona │ 能力 │ 測試 │
└──────────────────────────────────┘
```

**移動端**: 下拉選擇器
```
┌──────────────────────────────────┐
│ [▼] 基本配置                      │
└──────────────────────────────────┘
    ↓ 點擊展開
┌──────────────────────────────────┐
│ ✓ 基本配置                        │
│   Persona                        │
│   能力配置                        │
│   測試與調試                      │
│   部署與發布                      │
└──────────────────────────────────┘
```

### 5.4 面包屑適配

**桌面端**: 完整面包屑
```
首頁 > Agents 管理 > Agent 詳情 > Persona 配置
```

**移動端**: 收起為返回按鈕
```
< 返回
```
- 點擊返回到上一級頁面
- 長按顯示完整面包屑路徑（可選）

---

## 六、導航狀態管理

### 6.1 激活狀態邏輯

**主導航激活規則**:
```javascript
// 示例: React Router 實現
function isActive(route, currentPath) {
  // 精確匹配
  if (route === currentPath) return true;

  // 前綴匹配（子路由也激活父導航）
  if (currentPath.startsWith(route) && route !== '/') return true;

  return false;
}
```

**多層級激活**:
- 父導航: 當任何子路由激活時，父導航也激活
- 子菜單: 僅當精確匹配時激活

**示例**:
```
當前路徑: /agents/123/persona

激活狀態:
  🤖 Agents ← 激活（父級）
    ├─ Agent 列表
    ├─ 創建 Agent
    └─ 模板庫
```

### 6.2 導航歷史管理

**瀏覽器歷史**:
- 使用瀏覽器 History API
- 支持前進/後退按鈕
- 保留滾動位置

**返回行為**:
- 移動端返回按鈕 → 瀏覽器 `history.back()`
- 面包屑導航 → 精確跳轉到指定層級

### 6.3 導航權限控制

**權限檢查**:
```javascript
// 示例: 角色權限檢查
const navigationConfig = [
  {
    id: 'monitoring',
    label: '監控',
    route: '/monitoring',
    roles: ['admin'], // 僅管理員可見
  },
  // ...
];

function filterNavigationByRole(config, userRole) {
  return config.filter(item =>
    !item.roles || item.roles.includes(userRole)
  );
}
```

**無權限處理**:
- 隱藏導航項（推薦）
- 顯示為禁用狀態（灰色）
- 點擊顯示權限提示

---

## 七、導航性能優化

### 7.1 懶加載（Lazy Loading）

**路由懶加載**:
```javascript
// React 示例
const AgentList = lazy(() => import('./pages/agents/AgentList'));
const AgentDetail = lazy(() => import('./pages/agents/AgentDetail'));

<Routes>
  <Route path="/agents" element={
    <Suspense fallback={<LoadingSpinner />}>
      <AgentList />
    </Suspense>
  } />
</Routes>
```

**子菜單懶加載**:
- 僅在展開時加載子菜單數據
- 緩存已加載的子菜單

### 7.2 預加載（Prefetch）

**鼠標懸停預加載**:
```javascript
// 鼠標懸停時預加載下一個頁面
<Link
  to="/agents"
  onMouseEnter={() => prefetch('/agents')}
>
  Agents
</Link>
```

**導航欄預加載策略**:
- 首次加載: 加載當前激活頁面
- 懸停預加載: 懸停 200ms 後預加載頁面資源
- 離開時取消: 如果用戶離開懸停，取消預加載

### 7.3 緩存策略

**導航狀態緩存**:
- LocalStorage: 保存用戶的導航展開/折疊狀態
- SessionStorage: 保存當前會話的導航歷史

**頁面數據緩存**:
- 列表頁: 緩存 5 分鐘
- 詳情頁: 緩存 2 分鐘
- 實時數據: 不緩存

---

## 八、可訪問性 (Accessibility)

### 8.1 鍵盤導航

**Tab 順序**:
1. 跳過導航鏈接（Skip to main content）
2. 主導航項（從上到下）
3. 頁面主要內容
4. 次級導航
5. 工具導航

**焦點管理**:
- 焦點可見（Focus Visible）: 藍色輪廓
- 焦點陷阱（Focus Trap）: Modal 內循環聚焦
- 焦點恢復（Focus Restore）: 關閉 Modal 後恢復焦點

### 8.2 語義化標記

**HTML 語義**:
```html
<!-- 主導航 -->
<nav aria-label="主導航">
  <ul>
    <li><a href="/" aria-current="page">首頁</a></li>
    <li><a href="/agents">Agents</a></li>
  </ul>
</nav>

<!-- 面包屑 -->
<nav aria-label="面包屑">
  <ol>
    <li><a href="/">首頁</a></li>
    <li><a href="/agents">Agents</a></li>
    <li aria-current="page">Agent 詳情</li>
  </ol>
</nav>

<!-- 標籤頁 -->
<div role="tablist" aria-label="Agent 配置">
  <button role="tab" aria-selected="true">基本配置</button>
  <button role="tab" aria-selected="false">Persona</button>
</div>
```

### 8.3 ARIA 屬性

**導航 ARIA**:
- `aria-label`: 導航區域標籤
- `aria-current`: 當前激活項
- `aria-expanded`: 子菜單展開狀態
- `aria-haspopup`: 指示有彈出菜單
- `role="navigation"`: 導航區域角色

**示例**:
```html
<button
  aria-expanded="true"
  aria-controls="agents-submenu"
  aria-haspopup="true"
>
  Agents
</button>
<ul id="agents-submenu" aria-labelledby="agents-button">
  <li><a href="/agents/list">Agent 列表</a></li>
</ul>
```

### 8.4 屏幕閱讀器支持

**導航通告**:
- 導航變更: "已導航到 Agents 頁面"
- 子菜單展開: "Agents 子菜單已展開"
- 頁面加載: "頁面加載完成"

**跳過鏈接**:
```html
<a href="#main-content" class="skip-link">
  跳過導航，直接到主要內容
</a>
```

---

## 九、導航分析與監控

### 9.1 導航追蹤

**追蹤指標**:
- 導航點擊率（Click Rate）
- 導航路徑分析（Navigation Path）
- 退出率（Exit Rate）
- 時間到達（Time to Reach）

**追蹤實現**:
```javascript
// Google Analytics 示例
function trackNavigation(label, route) {
  gtag('event', 'navigation_click', {
    'event_category': 'Navigation',
    'event_label': label,
    'value': route
  });
}
```

### 9.2 A/B 測試

**測試場景**:
- 導航項順序優化
- 圖標 vs 純文字
- 主色調對比測試

**測試指標**:
- 目標完成率（Goal Completion Rate）
- 任務完成時間（Time on Task）
- 錯誤率（Error Rate）

---

## 十、導航最佳實踐總結

### 10.1 設計原則

1. **一致性**: 全站統一的導航模式和視覺風格
2. **可預測性**: 用戶能預期點擊後的結果
3. **反饋性**: 明確的激活狀態和懸停反饋
4. **效率性**: 核心功能 ≤2 次點擊可達
5. **可訪問性**: 支持鍵盤導航和屏幕閱讀器

### 10.2 常見問題解決

**問題 1: 導航層級過深**
- 解決: 扁平化導航，最多 3 層
- 替代: 使用全局搜索快速訪問

**問題 2: 移動端導航空間不足**
- 解決: 抽屜導航 + 底部導航欄組合
- 優先: 高頻功能放在底部導航

**問題 3: 導航項過多**
- 解決: 按角色過濾，僅顯示相關功能
- 分組: 使用分隔線和標題分組

**問題 4: 導航狀態不明確**
- 解決: 明確的視覺指示器（顏色、下劃線、粗體）
- 反饋: 懸停和激活狀態明顯區分

### 10.3 實施檢查清單

**開發階段**:
- [ ] 實現主導航（展開/折疊）
- [ ] 實現次級導航（標籤頁、子菜單）
- [ ] 實現面包屑導航
- [ ] 實現全局搜索
- [ ] 實現快捷鍵
- [ ] 實現移動端適配
- [ ] 權限控制邏輯

**可訪問性檢查**:
- [ ] 鍵盤導航完整支持
- [ ] ARIA 屬性正確使用
- [ ] 焦點管理合理
- [ ] 屏幕閱讀器測試通過
- [ ] 顏色對比度符合 WCAG 2.1 AA

**性能檢查**:
- [ ] 路由懶加載
- [ ] 導航預加載
- [ ] 狀態緩存
- [ ] 移動端優化

**測試階段**:
- [ ] 不同角色導航權限測試
- [ ] 不同設備響應式測試
- [ ] 瀏覽器兼容性測試
- [ ] A/B 測試數據收集

---

## 附錄: 導航配置文件示例

```typescript
// navigation.config.ts
export const navigationConfig = [
  {
    id: 'home',
    label: '首頁',
    icon: 'HomeIcon',
    route: '/',
    roles: ['admin', 'developer', 'analyst', 'viewer'],
    order: 1,
  },
  {
    id: 'agents',
    label: 'Agents',
    icon: 'RobotIcon',
    route: '/agents',
    roles: ['admin', 'developer'],
    order: 2,
    children: [
      {
        id: 'agents-list',
        label: 'Agent 列表',
        route: '/agents/list',
      },
      {
        id: 'agents-new',
        label: '創建 Agent',
        route: '/agents/new',
      },
      {
        id: 'agents-templates',
        label: '模板庫',
        route: '/agents/templates',
      },
    ],
  },
  // ...
];

export const shortcuts = [
  { key: 'mod+k', action: 'openSearch', label: '打開搜索' },
  { key: 'mod+n', action: 'createAgent', label: '創建 Agent' },
  { key: 'mod+e', action: 'openCodeInterpreter', label: '打開 Code Interpreter' },
  // ...
];
```

---

**文檔版本**: 1.0
**最後更新**: 2025-10-29
**維護者**: UI/UX Designer
**相關文檔**: [Site Map](sitemap.md), [Content Hierarchy](content-hierarchy.md)
