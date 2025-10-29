# User Story 模板

**Semantic Kernel Agentic Framework - User Story Template**

**版本**: 1.0.0
**日期**: 2025-10-28
**用途**: 標準化 User Story 格式

[返回總覽](../README.md)

---

## 📝 User Story 格式

### 基本結構

```markdown
#### US X.Y - [User Story 標題] [⭐ 標記（如果適用）]

**作為** [用戶角色]
**我想要** [功能描述]
**以便** [業務價值]

**驗收標準**:

✅ 必須項:
- [ ] [具體可驗證的驗收條件 1]
- [ ] [具體可驗證的驗收條件 2]
- [ ] [具體可驗證的驗收條件 3]
- [ ] [性能要求：具體數字和閾值]
- [ ] [安全要求：具體防護措施]

📋 可選項:
- [ ] [可選功能 1]
- [ ] [可選功能 2]

**BDD 驗收標準** (Given-When-Then):

```gherkin
Scenario 1: [場景描述]
  Given [前置條件]
  When [用戶操作]
  Then [預期結果]
  And [額外驗證]
```

**技術要求**:
- [前端技術棧]
- [後端技術棧]
- [數據庫需求]
- [第三方集成]

**API Endpoint**:
```yaml
Endpoint: [HTTP Method] /api/v1/[resource]
Request Body:
  [JSON Schema 或簡要描述]
Response:
  Success (200/201):
    [Response Schema]
  Error (400/401/403/500):
    [Error Schema]
```

**Database**:
- 表: `[table_name]`
- 關鍵欄位: `field1`, `field2`, `field3`
- 索引: `index_name ON (field1, field2)`
- 關聯: `foreign_key REFERENCES other_table(id)`

**UI/UX**:
- 線框圖: [wireframe-XXX.png 或 Figma 連結]
- 組件: [ComponentName from Design System]
- 交互流程: [Step 1 → Step 2 → Step 3]
- 響應式設計: Mobile / Tablet / Desktop

**安全需求**:
- RBAC: `[Permission.Name]` 權限
- 輸入驗證: [XSS, SQL Injection, CSRF 防護]
- 敏感數據: [加密、遮罩、審計]

**性能標準**:
- API 響應時間: <[X]ms (P95)
- 頁面加載時間: <[X]ms
- 並發支持: [X] 個並發請求
- 數據處理: [X] 條記錄/秒

**測試場景**:
```yaml
Unit Tests:
  - Test 1: [描述]
  - Test 2: [描述]

Integration Tests:
  - Test 1: [描述]
  - Test 2: [描述]

E2E Tests:
  - Critical Path: [描述]
```

**Story Points**: [1-21]
**Sprint**: Sprint [N]
**Assigned To**: [Team Name]
**Dependencies**: [US X.Y, US Z.W]

**📊 優先級**: P0 / P1 / P2
**🎯 驗收方式**: [簡要描述如何驗收]
**🔗 相關**: [US X.Y, US Z.W]

**範例代碼** (如適用):
```csharp
// 示例代碼
```

---
```

---

## 📊 Story Points 參考

### Fibonacci 估算標準

| Story Points | 複雜度 | 工作量 | 參考示例 |
|-------------|--------|--------|---------|
| **1** | 非常簡單 | 1-2 小時 | 簡單 UI 調整，文字修改 |
| **2** | 簡單 | 半天 | 簡單表單，基礎 CRUD |
| **3** | 簡單偏中 | 1 天 | 標準 API Endpoint，簡單功能 |
| **5** | 中等 | 2-3 天 | 完整功能模組，帶測試 |
| **8** | 複雜 | 1 週 | 複雜業務邏輯，多組件集成 |
| **13** | 非常複雜 | 2 週 | 複雜系統集成，架構級變更 |
| **21** | 極度複雜 | 3-4 週 | 需要拆分，不建議單個 Story |

### 複雜度因素考量

**技術複雜度**:
- 新技術學習曲線
- 第三方集成複雜度
- 算法和數據處理複雜度

**業務複雜度**:
- 業務規則數量
- 異常情況處理
- 用戶流程分支

**不確定性**:
- 需求清晰度
- 技術風險
- 依賴第三方響應

**規模**:
- 涉及組件數量
- 代碼變更範圍
- 測試覆蓋要求

---

## ✅ 驗收標準最佳實踐

### INVEST 原則

User Story 應該是:
- **Independent** (獨立的): 盡量減少依賴
- **Negotiable** (可協商的): 細節可以討論
- **Valuable** (有價值的): 對用戶有明確價值
- **Estimable** (可估算的): 團隊可以估算工作量
- **Small** (小的): 可在一個 Sprint 內完成
- **Testable** (可測試的): 有明確的驗收標準

### 驗收標準編寫準則

1. **具體可驗證**: 使用具體數字和閾值
   - ✅ "API 響應時間 <200ms (P95)"
   - ❌ "系統要快"

2. **完整覆蓋**: 涵蓋正常和異常情況
   - 正常流程（Happy Path）
   - 錯誤處理（Error Handling）
   - 邊界條件（Edge Cases）

3. **BDD 格式**: 使用 Given-When-Then
   - **Given**: 前置條件和初始狀態
   - **When**: 用戶操作或系統事件
   - **Then**: 預期結果和系統行為

4. **優先級區分**: 明確必須項和可選項
   - ✅ 必須項: MVP 必須實現
   - 📋 可選項: 可以延後或簡化

---

## 🎯 優先級標準

### P0 (MVP 必須)
- **定義**: 核心功能，MVP 必須交付，無此功能無法上線
- **標準**:
  - 業務關鍵流程必須功能
  - 差異化競爭力核心
  - 用戶無法繞過的功能
- **示例**: Agent 創建、Plugin 註冊、Code Interpreter

### P1 (MVP 高優先)
- **定義**: 重要功能，盡量在 MVP 中包含，顯著提升產品競爭力
- **標準**:
  - 提升用戶體驗
  - 增強系統穩定性
  - 重要但可降級的功能
- **示例**: Plugin 熱更新、Multi-Tenant、i18n

### P2 (Phase 2)
- **定義**: 增強功能，Phase 2 或更後期交付
- **標準**:
  - 錦上添花的功能
  - 用戶可以暫時忍受缺失
  - 有替代方案的功能
- **示例**: Plugin Marketplace、Persona 演化學習

---

## 📋 範例：完整 User Story

以下是一個完整的 User Story 示例，展示所有必要元素：

#### US 1.1 - 通過 Web UI 建立 Agent

**作為** IT 開發者或業務分析師
**我想要** 通過 Web UI 快速建立新的 Agent
**以便** 無需寫代碼即可部署 AI 助手

**驗收標準**:

✅ 必須項:
- [ ] 填寫 Agent 基本資訊（名稱、描述、角色類型）
- [ ] 從下拉選單選擇 LLM 模型（GPT-4, GPT-4o, GPT-4o-mini）
- [ ] 使用文字編輯器設定 System Prompt（支援模板）
- [ ] 設定模型參數（temperature: 0-2, max_tokens: 1-4096, top_p: 0-1）
- [ ] 建立操作 <5 秒完成，返回 Agent ID
- [ ] 自動驗證必填欄位和參數範圍
- [ ] 提供預覽功能（即時測試對話）

📋 可選項:
- [ ] 支援 Markdown 格式化 System Prompt
- [ ] 提供 Prompt 優化建議

**BDD 驗收標準**:

```gherkin
Scenario 1: 成功創建 Agent
  Given 用戶已登入且有 "Agent.Create" 權限
  And 用戶在 Agent Studio 頁面
  When 用戶填寫完整表單（名稱、描述、模型、Prompt）
  And 點擊 "創建" 按鈕
  Then 系統在 <5 秒內返回 Agent ID
  And 顯示成功通知 "Agent 創建成功"
  And 自動跳轉到 Agent 詳情頁

Scenario 2: 表單驗證錯誤
  Given 用戶在 Agent Studio 頁面
  When 用戶未填寫必填欄位
  And 點擊 "創建" 按鈕
  Then 表單顯示驗證錯誤提示
  And "創建" 按鈕保持禁用狀態
  And 錯誤欄位高亮顯示

Scenario 3: 即時測試預覽
  Given 用戶填寫完整表單
  When 用戶點擊 "測試預覽" 按鈕
  Then 打開對話預覽窗口
  And 用戶可以與 Agent 進行測試對話
  And 測試對話不會創建實際 Agent
```

**技術要求**:
- React 18 + TypeScript 5.x (前端)
- Material-UI (組件庫)
- React Query (數據獲取)
- Yup / Zod (表單驗證)
- ASP.NET Core Minimal API (後端)
- PostgreSQL (數據持久化)

**API Endpoint**:
```yaml
Endpoint: POST /api/v1/agents
Authentication: Bearer Token (JWT)
Authorization: "Agent.Create" permission

Request Body:
  {
    "name": "string (required, max 100)",
    "description": "string (optional, max 500)",
    "roleType": "enum (CustomerService, DataAnalyst, Custom)",
    "model": "enum (gpt-4, gpt-4o, gpt-4o-mini)",
    "systemPrompt": "string (required, max 4000)",
    "temperature": "number (0-2)",
    "maxTokens": "number (1-4096)",
    "topP": "number (0-1)"
  }

Response:
  Success (201 Created):
    {
      "agentId": "uuid",
      "name": "string",
      "status": "Active",
      "createdAt": "datetime"
    }

  Error (400 Bad Request):
    {
      "error": "ValidationError",
      "message": "詳細錯誤信息",
      "fields": {
        "name": "名稱不能為空"
      }
    }
```

**Database**:
- 表: `agents`
- 關鍵欄位: `id`, `name`, `description`, `role_type`, `model`, `system_prompt`, `temperature`, `max_tokens`, `top_p`, `created_by`, `created_at`
- 索引: `idx_agents_created_by ON (created_by, created_at DESC)`
- 關聯: `created_by REFERENCES users(id)`

**UI/UX**:
- 線框圖: wireframe-001-agent-studio.fig (Figma)
- 組件: `AgentCreateForm`, `ModelSelector`, `PromptEditor`, `TestPreviewDialog`
- 交互流程:
  1. 填寫表單 →
  2. （可選）測試預覽 →
  3. 創建 Agent →
  4. 跳轉詳情頁
- 響應式設計: Desktop (1280px+), Tablet (768-1279px)

**安全需求**:
- RBAC: `Agent.Create` 權限檢查
- XSS 防護: 輸入內容自動轉義
- CSRF Token: 表單提交包含 CSRF Token
- Rate Limiting: 同一用戶 10 次/分鐘創建限制

**性能標準**:
- API 響應時間: <200ms (P95)
- 表單渲染時間: <100ms
- 測試預覽啟動: <1s
- 數據庫寫入: <50ms

**測試場景**:
```yaml
Unit Tests:
  - FormValidation: 測試表單驗證邏輯
  - ModelSelector: 測試模型選擇組件
  - PromptEditor: 測試 Prompt 編輯器

Integration Tests:
  - AgentCreationAPI: 測試完整創建流程
  - AuthorizationCheck: 測試權限檢查

E2E Tests:
  - CriticalPath: 用戶登入 → 創建 Agent → 驗證成功
```

**Story Points**: 5
**Sprint**: Sprint 1
**Assigned To**: Frontend Team + Backend Team
**Dependencies**:
- US 9.1 (RBAC 系統必須先實現)
- Database Schema 初始化完成

**📊 優先級**: P0 (MVP 必須)
**🎯 驗收方式**: 開發者可在 2 分鐘內通過 Web UI 建立一個可用的 Agent
**🔗 相關**: US 1.2 (SDK 創建), US 1.5 (Persona Builder)

**範例代碼**:
```typescript
// React Component 示例
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object({
  name: yup.string().required('名稱不能為空').max(100),
  model: yup.string().oneOf(['gpt-4', 'gpt-4o', 'gpt-4o-mini']),
  systemPrompt: yup.string().required().max(4000),
  temperature: yup.number().min(0).max(2),
}).required();

export function AgentCreateForm() {
  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema)
  });

  const onSubmit = async (data) => {
    const response = await fetch('/api/v1/agents', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    // Handle response...
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {/* Form fields... */}
    </form>
  );
}
```

---

## 📚 相關資源

- [Definition of Done](./definition-of-done.md) - DoD 標準
- [Sprint Backlog](../sprints/sprint-backlog.md) - Sprint 規劃
- [返回總覽](../README.md) - User Stories 總覽

---

**最後更新**: 2025-10-28
