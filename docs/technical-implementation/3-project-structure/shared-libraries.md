# Shared Libraries (共享庫設計)

## 版本信息
- **策略**: Monorepo 架構
- **包管理**: pnpm Workspaces
- **版本控制**: 語義化版本
- **最後更新**: 2025-01-01

## 整體結構

```
shared/
├── types/                                 # TypeScript 類型定義
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── api/
│       ├── workflow/
│       ├── agent/
│       └── index.ts
│
├── utils/                                 # 共享工具函數
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── validation/
│       ├── formatting/
│       ├── date/
│       └── index.ts
│
├── contracts/                             # API 契約定義
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── requests/
│       ├── responses/
│       ├── events/
│       └── index.ts
│
├── constants/                             # 共享常量
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── api-routes.ts
│       ├── error-codes.ts
│       ├── workflow-types.ts
│       └── index.ts
│
├── ui-components/                         # 共享 UI 組件 (可選)
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── Button/
│       ├── Input/
│       └── index.ts
│
├── pnpm-workspace.yaml                    # pnpm Workspace 配置
└── package.json                           # Root package.json
```

## 1. Types 包 (@aiworkflow/types)

### 目的
- 提供前後端共享的 TypeScript 類型定義
- 確保 API 契約的類型安全
- 減少類型重複定義

### 結構

```
shared/types/
├── package.json
├── tsconfig.json
└── src/
    ├── api/
    │   ├── common.ts
    │   ├── pagination.ts
    │   └── response.ts
    ├── workflow/
    │   ├── workflow.ts
    │   ├── node.ts
    │   └── edge.ts
    ├── agent/
    │   ├── agent.ts
    │   └── persona.ts
    ├── user/
    │   └── user.ts
    └── index.ts
```

### 核心類型定義

```typescript
// shared/types/src/workflow/workflow.ts
export interface Workflow {
  id: number
  name: string
  description: string
  ownerId: number
  nodes: WorkflowNode[]
  edges: WorkflowEdge[]
  status: WorkflowStatus
  createdAt: string
  lastModifiedAt?: string
}

export interface WorkflowNode {
  id: string
  type: WorkflowNodeType
  position: Position
  data: NodeData
}

export interface WorkflowEdge {
  id: string
  source: string
  target: string
  sourceHandle?: string
  targetHandle?: string
  type?: string
}

export type WorkflowStatus = 'draft' | 'published' | 'executing' | 'completed' | 'failed'

export type WorkflowNodeType = 'prompt' | 'agent' | 'function' | 'condition'

export interface Position {
  x: number
  y: number
}

export interface NodeData {
  label: string
  [key: string]: unknown
}

// shared/types/src/api/response.ts
export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
  timestamp: string
}

export interface PaginatedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
}

export interface ErrorResponse {
  error: string
  code: string
  details?: Record<string, string[]>
  timestamp: string
}

// shared/types/src/api/common.ts
export type ApiResult<T> = ApiResponse<T> | ErrorResponse

export interface RequestOptions {
  headers?: Record<string, string>
  params?: Record<string, string | number | boolean>
  timeout?: number
}
```

### package.json

```json
{
  "name": "@aiworkflow/types",
  "version": "1.0.0",
  "description": "Shared TypeScript types for AI Workflow Platform",
  "main": "./dist/index.js",
  "types": "./dist/index.d.ts",
  "exports": {
    ".": {
      "types": "./dist/index.d.ts",
      "import": "./dist/index.js",
      "require": "./dist/index.cjs"
    },
    "./workflow": {
      "types": "./dist/workflow/index.d.ts",
      "import": "./dist/workflow/index.js"
    },
    "./agent": {
      "types": "./dist/agent/index.d.ts",
      "import": "./dist/agent/index.js"
    }
  },
  "scripts": {
    "build": "tsup src/index.ts --format cjs,esm --dts",
    "dev": "tsup src/index.ts --format cjs,esm --dts --watch",
    "typecheck": "tsc --noEmit"
  },
  "devDependencies": {
    "tsup": "^8.0.0",
    "typescript": "^5.8.0"
  }
}
```

## 2. Utils 包 (@aiworkflow/utils)

### 目的
- 提供跨前後端的工具函數
- 統一驗證、格式化邏輯
- 減少代碼重複

### 結構

```
shared/utils/
├── package.json
├── tsconfig.json
└── src/
    ├── validation/
    │   ├── email.ts
    │   ├── password.ts
    │   └── workflow.ts
    ├── formatting/
    │   ├── date.ts
    │   ├── number.ts
    │   └── string.ts
    ├── serialization/
    │   ├── json.ts
    │   └── workflow.ts
    ├── crypto/
    │   └── hash.ts
    └── index.ts
```

### 核心工具函數

```typescript
// shared/utils/src/validation/email.ts
export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export function normalizeEmail(email: string): string {
  return email.toLowerCase().trim()
}

// shared/utils/src/validation/password.ts
export interface PasswordStrength {
  isValid: boolean
  score: number // 0-4
  feedback: string[]
}

export function validatePassword(password: string): PasswordStrength {
  const feedback: string[] = []
  let score = 0

  if (password.length < 8) {
    feedback.push('密碼長度至少 8 個字符')
  } else {
    score++
  }

  if (!/[a-z]/.test(password)) {
    feedback.push('需包含小寫字母')
  } else {
    score++
  }

  if (!/[A-Z]/.test(password)) {
    feedback.push('需包含大寫字母')
  } else {
    score++
  }

  if (!/\d/.test(password)) {
    feedback.push('需包含數字')
  } else {
    score++
  }

  return {
    isValid: feedback.length === 0,
    score,
    feedback,
  }
}

// shared/utils/src/validation/workflow.ts
import type { Workflow, WorkflowNode } from '@aiworkflow/types'

export interface WorkflowValidationResult {
  isValid: boolean
  errors: string[]
}

export function validateWorkflow(workflow: Workflow): WorkflowValidationResult {
  const errors: string[] = []

  // 至少有一個節點
  if (!workflow.nodes || workflow.nodes.length === 0) {
    errors.push('工作流程至少需要一個節點')
  }

  // 檢查節點是否已配置
  workflow.nodes.forEach((node, index) => {
    if (!isNodeConfigured(node)) {
      errors.push(`節點 ${index + 1} (${node.type}) 未配置`)
    }
  })

  // 檢查邊的有效性
  workflow.edges.forEach((edge) => {
    const sourceExists = workflow.nodes.some((n) => n.id === edge.source)
    const targetExists = workflow.nodes.some((n) => n.id === edge.target)

    if (!sourceExists) {
      errors.push(`邊 ${edge.id} 的源節點 ${edge.source} 不存在`)
    }

    if (!targetExists) {
      errors.push(`邊 ${edge.id} 的目標節點 ${edge.target} 不存在`)
    }
  })

  return {
    isValid: errors.length === 0,
    errors,
  }
}

function isNodeConfigured(node: WorkflowNode): boolean {
  switch (node.type) {
    case 'prompt':
      return !!node.data.template
    case 'agent':
      return !!node.data.personaId && !!node.data.instruction
    case 'function':
      return !!node.data.functionName
    case 'condition':
      return !!node.data.condition
    default:
      return false
  }
}

// shared/utils/src/formatting/date.ts
export function formatDate(date: string | Date, locale = 'zh-TW'): string {
  return new Intl.DateTimeFormat(locale, {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(date))
}

export function formatRelativeTime(date: string | Date, locale = 'zh-TW'): string {
  const now = new Date()
  const target = new Date(date)
  const diffMs = now.getTime() - target.getTime()

  const diffSeconds = Math.floor(diffMs / 1000)
  const diffMinutes = Math.floor(diffSeconds / 60)
  const diffHours = Math.floor(diffMinutes / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffSeconds < 60) {
    return '剛剛'
  } else if (diffMinutes < 60) {
    return `${diffMinutes} 分鐘前`
  } else if (diffHours < 24) {
    return `${diffHours} 小時前`
  } else if (diffDays < 7) {
    return `${diffDays} 天前`
  } else {
    return formatDate(target, locale)
  }
}

// shared/utils/src/crypto/hash.ts
export async function hashString(input: string, algorithm = 'SHA-256'): Promise<string> {
  const encoder = new TextEncoder()
  const data = encoder.encode(input)
  const hashBuffer = await crypto.subtle.digest(algorithm, data)
  const hashArray = Array.from(new Uint8Array(hashBuffer))
  return hashArray.map((b) => b.toString(16).padStart(2, '0')).join('')
}

export function generateId(prefix = 'id'): string {
  const timestamp = Date.now().toString(36)
  const random = Math.random().toString(36).substring(2, 9)
  return `${prefix}-${timestamp}-${random}`
}
```

## 3. Contracts 包 (@aiworkflow/contracts)

### 目的
- 定義前後端 API 契約
- 統一請求和響應格式
- 支持版本化

### 結構

```
shared/contracts/
├── package.json
├── tsconfig.json
└── src/
    ├── v1/
    │   ├── users/
    │   │   ├── requests.ts
    │   │   └── responses.ts
    │   ├── workflows/
    │   │   ├── requests.ts
    │   │   └── responses.ts
    │   └── agents/
    │       ├── requests.ts
    │       └── responses.ts
    ├── events/
    │   ├── workflow-events.ts
    │   └── collaboration-events.ts
    └── index.ts
```

### API 契約定義

```typescript
// shared/contracts/src/v1/workflows/requests.ts
import type { WorkflowNode, WorkflowEdge } from '@aiworkflow/types'

export interface CreateWorkflowRequest {
  name: string
  description: string
  nodes: WorkflowNode[]
  edges: WorkflowEdge[]
}

export interface UpdateWorkflowRequest {
  name?: string
  description?: string
  nodes?: WorkflowNode[]
  edges?: WorkflowEdge[]
}

export interface ExecuteWorkflowRequest {
  inputs: Record<string, unknown>
}

export interface GetWorkflowListRequest {
  page?: number
  pageSize?: number
  status?: string
  sortBy?: 'createdAt' | 'name' | 'lastModifiedAt'
  sortOrder?: 'asc' | 'desc'
}

// shared/contracts/src/v1/workflows/responses.ts
import type { Workflow, ApiResponse, PaginatedResponse } from '@aiworkflow/types'

export type CreateWorkflowResponse = ApiResponse<{ id: number }>

export type GetWorkflowResponse = ApiResponse<Workflow>

export type GetWorkflowListResponse = ApiResponse<PaginatedResponse<Workflow>>

export interface ExecutionResult {
  workflowId: number
  status: 'success' | 'failed'
  results: Record<string, unknown>
  executedAt: string
  duration: number
  error?: string
}

export type ExecuteWorkflowResponse = ApiResponse<ExecutionResult>

// shared/contracts/src/events/workflow-events.ts
export interface WorkflowCreatedEvent {
  type: 'workflow.created'
  workflowId: number
  userId: number
  timestamp: string
}

export interface WorkflowExecutedEvent {
  type: 'workflow.executed'
  workflowId: number
  executionId: string
  status: 'success' | 'failed'
  timestamp: string
}

export type WorkflowEvent = WorkflowCreatedEvent | WorkflowExecutedEvent

// shared/contracts/src/events/collaboration-events.ts
export interface UserJoinedEvent {
  type: 'user.joined'
  userId: string
  userName: string
  workflowId: number
  timestamp: string
}

export interface NodeChangedEvent {
  type: 'node.changed'
  nodeId: string
  workflowId: number
  changes: unknown
  userId: string
  timestamp: string
}

export type CollaborationEvent = UserJoinedEvent | NodeChangedEvent
```

## 4. Constants 包 (@aiworkflow/constants)

### 目的
- 集中管理常量定義
- 避免魔法字符串和數字
- 統一錯誤碼和路由

### 結構

```
shared/constants/
├── package.json
├── tsconfig.json
└── src/
    ├── api-routes.ts
    ├── error-codes.ts
    ├── workflow-types.ts
    ├── status-codes.ts
    └── index.ts
```

### 常量定義

```typescript
// shared/constants/src/api-routes.ts
export const API_ROUTES = {
  // Users
  USERS: '/api/users',
  USER_BY_ID: (id: number) => `/api/users/${id}`,
  USER_PROFILE: '/api/users/profile',

  // Workflows
  WORKFLOWS: '/api/workflows',
  WORKFLOW_BY_ID: (id: number) => `/api/workflows/${id}`,
  WORKFLOW_EXECUTE: (id: number) => `/api/workflows/${id}/execute`,
  WORKFLOW_HISTORY: (id: number) => `/api/workflows/${id}/history`,

  // Agents
  AGENTS: '/api/agents',
  AGENT_PERSONAS: '/api/agents/personas',
  PERSONA_BY_ID: (id: number) => `/api/agents/personas/${id}`,

  // Authentication
  AUTH_LOGIN: '/api/auth/login',
  AUTH_REGISTER: '/api/auth/register',
  AUTH_LOGOUT: '/api/auth/logout',
  AUTH_REFRESH: '/api/auth/refresh',
} as const

// shared/constants/src/error-codes.ts
export const ERROR_CODES = {
  // Validation Errors (1000-1999)
  VALIDATION_FAILED: 'ERR_1000',
  INVALID_EMAIL: 'ERR_1001',
  INVALID_PASSWORD: 'ERR_1002',
  WORKFLOW_INVALID: 'ERR_1100',
  NODE_NOT_CONFIGURED: 'ERR_1101',

  // Authentication Errors (2000-2999)
  UNAUTHORIZED: 'ERR_2000',
  TOKEN_EXPIRED: 'ERR_2001',
  INVALID_CREDENTIALS: 'ERR_2002',
  FORBIDDEN: 'ERR_2003',

  // Resource Errors (3000-3999)
  NOT_FOUND: 'ERR_3000',
  WORKFLOW_NOT_FOUND: 'ERR_3001',
  USER_NOT_FOUND: 'ERR_3002',
  PERSONA_NOT_FOUND: 'ERR_3003',

  // Execution Errors (4000-4999)
  EXECUTION_FAILED: 'ERR_4000',
  WORKFLOW_ALREADY_EXECUTING: 'ERR_4001',
  NODE_EXECUTION_FAILED: 'ERR_4002',

  // Server Errors (5000-5999)
  INTERNAL_ERROR: 'ERR_5000',
  DATABASE_ERROR: 'ERR_5001',
  EXTERNAL_API_ERROR: 'ERR_5002',
} as const

// shared/constants/src/workflow-types.ts
export const WORKFLOW_NODE_TYPES = {
  PROMPT: 'prompt',
  AGENT: 'agent',
  FUNCTION: 'function',
  CONDITION: 'condition',
} as const

export const WORKFLOW_STATUS = {
  DRAFT: 'draft',
  PUBLISHED: 'published',
  EXECUTING: 'executing',
  COMPLETED: 'completed',
  FAILED: 'failed',
} as const

export const NODE_ICONS = {
  [WORKFLOW_NODE_TYPES.PROMPT]: '💬',
  [WORKFLOW_NODE_TYPES.AGENT]: '🤖',
  [WORKFLOW_NODE_TYPES.FUNCTION]: '⚙️',
  [WORKFLOW_NODE_TYPES.CONDITION]: '🔀',
} as const

// shared/constants/src/status-codes.ts
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500,
} as const
```

## 5. UI Components 包 (@aiworkflow/ui-components)

### 目的 (可選)
- 跨前端框架的通用 UI 組件
- 統一設計系統
- 減少重複開發

### 結構

```
shared/ui-components/
├── package.json
├── tsconfig.json
└── src/
    ├── Button/
    │   ├── Button.tsx
    │   ├── Button.module.css
    │   └── index.ts
    ├── Input/
    ├── Modal/
    └── index.ts
```

## Monorepo 配置

### pnpm-workspace.yaml

```yaml
packages:
  - 'shared/types'
  - 'shared/utils'
  - 'shared/contracts'
  - 'shared/constants'
  - 'shared/ui-components'
  - 'backend/src/*'
  - 'frontend/react-app'
  - 'frontend/vue-workflow-editor'
```

### Root package.json

```json
{
  "name": "ai-workflow-platform",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "build": "pnpm -r build",
    "dev": "pnpm -r --parallel dev",
    "typecheck": "pnpm -r typecheck",
    "test": "pnpm -r test"
  },
  "devDependencies": {
    "typescript": "^5.8.0",
    "prettier": "^3.0.0",
    "eslint": "^9.0.0"
  },
  "packageManager": "pnpm@9.0.0"
}
```

## 使用示例

### 前端 React 使用共享庫

```typescript
// frontend/react-app/package.json
{
  "dependencies": {
    "@aiworkflow/types": "workspace:*",
    "@aiworkflow/utils": "workspace:*",
    "@aiworkflow/contracts": "workspace:*",
    "@aiworkflow/constants": "workspace:*"
  }
}

// frontend/react-app/src/api/workflows.ts
import type { CreateWorkflowRequest, GetWorkflowResponse } from '@aiworkflow/contracts'
import { API_ROUTES } from '@aiworkflow/constants'
import { validateWorkflow } from '@aiworkflow/utils'
import { apiClient } from './client'

export async function createWorkflow(
  request: CreateWorkflowRequest
): Promise<GetWorkflowResponse> {
  // 客戶端驗證
  const validation = validateWorkflow({
    id: 0,
    ...request,
    ownerId: 0,
    status: 'draft',
    createdAt: new Date().toISOString(),
  })

  if (!validation.isValid) {
    throw new Error(validation.errors.join(', '))
  }

  return apiClient.post(API_ROUTES.WORKFLOWS, request)
}
```

### 後端 .NET 使用類型定義 (通過代碼生成)

```csharp
// backend/src/AIWorkflow.Application/Workflows/Commands/CreateWorkflowCommand.cs
namespace AIWorkflow.Application.Workflows.Commands;

/// <summary>
/// 創建 Workflow 命令 - 對應 @aiworkflow/contracts CreateWorkflowRequest
/// </summary>
public sealed record CreateWorkflowCommand : IRequest<Result<int>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public List<WorkflowNodeDto> Nodes { get; init; } = [];
    public List<WorkflowEdgeDto> Edges { get; init; } = [];
}

// 可以使用工具從 TypeScript 類型生成 C# DTO
// 例如: ts-to-csharp, TypeScriptToCSModels
```

### 前端 Vue 使用共享庫

```typescript
// frontend/vue-workflow-editor/package.json
{
  "dependencies": {
    "@aiworkflow/types": "workspace:*",
    "@aiworkflow/utils": "workspace:*",
    "@aiworkflow/constants": "workspace:*"
  }
}

// frontend/vue-workflow-editor/src/utils/nodeFactory.ts
import type { WorkflowNode, WorkflowNodeType } from '@aiworkflow/types'
import { WORKFLOW_NODE_TYPES, NODE_ICONS } from '@aiworkflow/constants'
import { generateId } from '@aiworkflow/utils'

export function createNode(
  type: WorkflowNodeType,
  position: { x: number; y: number }
): WorkflowNode {
  const id = generateId('node')

  return {
    id,
    type,
    position,
    data: {
      label: `${NODE_ICONS[type]} ${type.toUpperCase()}`,
    },
  }
}
```

## 版本管理

### 語義化版本 (Semantic Versioning)

```
MAJOR.MINOR.PATCH

例如: 1.2.3
- MAJOR (1): 不兼容的 API 變更
- MINOR (2): 向後兼容的功能新增
- PATCH (3): 向後兼容的 Bug 修復
```

### 發布策略

```json
// shared/types/package.json
{
  "name": "@aiworkflow/types",
  "version": "1.0.0",
  "scripts": {
    "release:patch": "npm version patch && npm publish",
    "release:minor": "npm version minor && npm publish",
    "release:major": "npm version major && npm publish"
  }
}
```

## 代碼生成 (可選)

### TypeScript to C# DTO

使用工具自動生成 C# DTO 從 TypeScript 類型:

```bash
# 安裝代碼生成工具
npm install -g ts-to-csharp

# 生成 C# DTOs
ts-to-csharp --input shared/types/src --output backend/src/AIWorkflow.Contracts
```

### OpenAPI / Swagger

從 .NET API 生成 TypeScript 客戶端:

```bash
# 使用 NSwag 生成 TypeScript 客戶端
nswag openapi2tsclient \
  /input:http://localhost:5001/swagger/v1/swagger.json \
  /output:frontend/react-app/src/api/generated-client.ts
```

## 最佳實踐

### 1. 依賴管理
- ✅ 使用 pnpm Workspaces 統一管理依賴
- ✅ 共享庫只依賴必要的外部庫
- ✅ 避免循環依賴

### 2. 類型安全
- ✅ 所有共享代碼使用 TypeScript
- ✅ 嚴格的 tsconfig.json 配置
- ✅ 導出完整的類型定義

### 3. 文檔
- ✅ 每個函數添加 JSDoc 註釋
- ✅ 提供使用示例
- ✅ 維護 CHANGELOG

### 4. 測試
- ✅ 共享工具函數必須有單元測試
- ✅ 驗證函數覆蓋率 > 80%
- ✅ 使用 Vitest 或 Jest

### 5. 發布
- ✅ 使用語義化版本
- ✅ 自動化發布流程
- ✅ 發布前運行完整測試

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
