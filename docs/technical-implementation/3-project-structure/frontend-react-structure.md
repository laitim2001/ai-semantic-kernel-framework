# Frontend React Structure (React 19)

## 版本信息
- **React 版本**: 19.0
- **TypeScript**: 5.8
- **構建工具**: Vite 5
- **狀態管理**: Zustand 4.5
- **最後更新**: 2025-01-01

## 整體結構

```
frontend/react-app/
├── public/                                 # 靜態資源
│   ├── favicon.ico
│   └── robots.txt
│
├── src/                                    # 源代碼
│   ├── api/                               # API 客戶端
│   ├── assets/                            # 資源文件
│   ├── components/                        # React 組件
│   ├── features/                          # 功能模塊
│   ├── hooks/                             # 自定義 Hooks
│   ├── layouts/                           # 佈局組件
│   ├── pages/                             # 頁面組件
│   ├── routes/                            # 路由配置
│   ├── stores/                            # Zustand Stores
│   ├── styles/                            # 全局樣式
│   ├── types/                             # TypeScript 類型
│   ├── utils/                             # 工具函數
│   ├── App.tsx                            # 根組件
│   ├── main.tsx                           # 應用入口
│   └── vite-env.d.ts                      # Vite 類型聲明
│
├── tests/                                  # 測試文件
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
├── .env.local                             # 環境變數 (本地)
├── .env.development                       # 開發環境
├── .env.production                        # 生產環境
├── .eslintrc.cjs                          # ESLint 配置
├── .prettierrc                            # Prettier 配置
├── index.html                             # HTML 模板
├── package.json
├── tsconfig.json                          # TypeScript 配置
├── tsconfig.node.json
└── vite.config.ts                         # Vite 配置
```

## 目錄結構詳解

### 1. API 客戶端層

```
src/api/
├── client.ts                              # Axios 實例配置
├── types.ts                               # API 類型定義
├── users/
│   ├── types.ts
│   ├── queries.ts                         # React Query queries
│   └── mutations.ts                       # React Query mutations
├── workflows/
│   ├── types.ts
│   ├── queries.ts
│   └── mutations.ts
└── agents/
    ├── types.ts
    ├── queries.ts
    └── mutations.ts
```

#### API Client 示例

```typescript
// src/api/client.ts
import axios, { type AxiosInstance, type AxiosRequestConfig } from 'axios'

class ApiClient {
  private client: AxiosInstance

  constructor() {
    this.client = axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    // 請求攔截器 - 添加 JWT Token
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('auth_token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // 響應攔截器 - 統一錯誤處理
    this.client.interceptors.response.use(
      (response) => response,
      async (error) => {
        if (error.response?.status === 401) {
          // 清除 token 並重定向到登錄頁
          localStorage.removeItem('auth_token')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.get<T>(url, config)
    return response.data
  }

  async post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.post<T>(url, data, config)
    return response.data
  }

  async put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.put<T>(url, data, config)
    return response.data
  }

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.delete<T>(url, config)
    return response.data
  }
}

export const apiClient = new ApiClient()
```

#### React Query Hooks 示例

```typescript
// src/api/workflows/queries.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '../client'
import type { Workflow, WorkflowListResponse, CreateWorkflowDto } from './types'

// Query Keys
export const workflowKeys = {
  all: ['workflows'] as const,
  lists: () => [...workflowKeys.all, 'list'] as const,
  list: (filters: string) => [...workflowKeys.lists(), { filters }] as const,
  details: () => [...workflowKeys.all, 'detail'] as const,
  detail: (id: number) => [...workflowKeys.details(), id] as const,
}

// Queries
export function useWorkflows(page = 1, pageSize = 20) {
  return useQuery({
    queryKey: workflowKeys.list(`page=${page}&pageSize=${pageSize}`),
    queryFn: () =>
      apiClient.get<WorkflowListResponse>(
        `/api/workflows?page=${page}&pageSize=${pageSize}`
      ),
  })
}

export function useWorkflow(id: number) {
  return useQuery({
    queryKey: workflowKeys.detail(id),
    queryFn: () => apiClient.get<Workflow>(`/api/workflows/${id}`),
    enabled: id > 0,
  })
}

// Mutations
export function useCreateWorkflow() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateWorkflowDto) =>
      apiClient.post<number>('/api/workflows', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: workflowKeys.lists() })
    },
  })
}

export function useUpdateWorkflow() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: Partial<Workflow> }) =>
      apiClient.put<void>(`/api/workflows/${id}`, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: workflowKeys.detail(variables.id) })
      queryClient.invalidateQueries({ queryKey: workflowKeys.lists() })
    },
  })
}

export function useDeleteWorkflow() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => apiClient.delete<void>(`/api/workflows/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: workflowKeys.lists() })
    },
  })
}
```

### 2. 組件層

```
src/components/
├── common/                                # 通用組件
│   ├── Button/
│   │   ├── Button.tsx
│   │   ├── Button.test.tsx
│   │   └── Button.module.css
│   ├── Input/
│   ├── Modal/
│   ├── Loading/
│   └── ErrorBoundary/
│
├── forms/                                 # 表單組件
│   ├── WorkflowForm/
│   ├── UserProfileForm/
│   └── LoginForm/
│
├── layout/                                # 佈局組件
│   ├── Header/
│   ├── Sidebar/
│   ├── Footer/
│   └── Navigation/
│
└── workflow/                              # Workflow 特定組件
    ├── WorkflowCard/
    ├── WorkflowList/
    └── WorkflowDetail/
```

#### 組件示例

```typescript
// src/components/common/Button/Button.tsx
import { forwardRef, type ButtonHTMLAttributes } from 'react'
import styles from './Button.module.css'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  isLoading?: boolean
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      children,
      variant = 'primary',
      size = 'md',
      isLoading = false,
      disabled,
      className,
      ...props
    },
    ref
  ) => {
    return (
      <button
        ref={ref}
        className={`${styles.button} ${styles[variant]} ${styles[size]} ${className ?? ''}`}
        disabled={disabled || isLoading}
        {...props}
      >
        {isLoading ? (
          <>
            <span className={styles.spinner} />
            Loading...
          </>
        ) : (
          children
        )}
      </button>
    )
  }
)

Button.displayName = 'Button'
```

### 3. Features 功能模塊

```
src/features/
├── auth/                                  # 認證功能
│   ├── components/
│   │   ├── LoginForm.tsx
│   │   └── RegisterForm.tsx
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   └── useLogin.ts
│   ├── stores/
│   │   └── authStore.ts
│   └── types.ts
│
├── workflows/                             # Workflow 功能
│   ├── components/
│   │   ├── WorkflowEditor/
│   │   ├── WorkflowExecutor/
│   │   └── WorkflowHistory/
│   ├── hooks/
│   │   ├── useWorkflowExecution.ts
│   │   └── useWorkflowValidation.ts
│   ├── stores/
│   │   └── workflowStore.ts
│   └── types.ts
│
├── agents/                                # Agent 功能
│   ├── components/
│   │   ├── PersonaBuilder/
│   │   ├── PersonaList/
│   │   └── PersonaPreview/
│   ├── hooks/
│   │   └── usePersona.ts
│   └── types.ts
│
└── dashboard/                             # 儀表板
    ├── components/
    │   ├── StatsCard.tsx
    │   └── RecentActivity.tsx
    └── hooks/
        └── useDashboardData.ts
```

#### Feature Store 示例 (Zustand)

```typescript
// src/features/workflows/stores/workflowStore.ts
import { create } from 'zustand'
import { devtools, persist } from 'zustand/middleware'
import type { Workflow, WorkflowNode, WorkflowEdge } from '../types'

interface WorkflowState {
  // State
  currentWorkflow: Workflow | null
  nodes: WorkflowNode[]
  edges: WorkflowEdge[]
  selectedNodeId: string | null
  isExecuting: boolean

  // Actions
  setCurrentWorkflow: (workflow: Workflow) => void
  addNode: (node: WorkflowNode) => void
  updateNode: (id: string, updates: Partial<WorkflowNode>) => void
  removeNode: (id: string) => void
  addEdge: (edge: WorkflowEdge) => void
  removeEdge: (id: string) => void
  setSelectedNode: (id: string | null) => void
  clearWorkflow: () => void
  setIsExecuting: (isExecuting: boolean) => void
}

export const useWorkflowStore = create<WorkflowState>()(
  devtools(
    persist(
      (set, get) => ({
        // Initial state
        currentWorkflow: null,
        nodes: [],
        edges: [],
        selectedNodeId: null,
        isExecuting: false,

        // Actions
        setCurrentWorkflow: (workflow) =>
          set({
            currentWorkflow: workflow,
            nodes: workflow.nodes || [],
            edges: workflow.edges || [],
          }),

        addNode: (node) =>
          set((state) => ({
            nodes: [...state.nodes, node],
          })),

        updateNode: (id, updates) =>
          set((state) => ({
            nodes: state.nodes.map((node) =>
              node.id === id ? { ...node, ...updates } : node
            ),
          })),

        removeNode: (id) =>
          set((state) => ({
            nodes: state.nodes.filter((node) => node.id !== id),
            edges: state.edges.filter(
              (edge) => edge.source !== id && edge.target !== id
            ),
            selectedNodeId: state.selectedNodeId === id ? null : state.selectedNodeId,
          })),

        addEdge: (edge) =>
          set((state) => ({
            edges: [...state.edges, edge],
          })),

        removeEdge: (id) =>
          set((state) => ({
            edges: state.edges.filter((edge) => edge.id !== id),
          })),

        setSelectedNode: (id) =>
          set({
            selectedNodeId: id,
          }),

        clearWorkflow: () =>
          set({
            currentWorkflow: null,
            nodes: [],
            edges: [],
            selectedNodeId: null,
            isExecuting: false,
          }),

        setIsExecuting: (isExecuting) =>
          set({ isExecuting }),
      }),
      {
        name: 'workflow-storage',
        partialize: (state) => ({
          currentWorkflow: state.currentWorkflow,
          nodes: state.nodes,
          edges: state.edges,
        }),
      }
    )
  )
)
```

#### Custom Hook 示例

```typescript
// src/features/workflows/hooks/useWorkflowExecution.ts
import { useState, useCallback } from 'react'
import { useMutation } from '@tanstack/react-query'
import { apiClient } from '@/api/client'
import { useWorkflowStore } from '../stores/workflowStore'
import type { ExecutionResult } from '../types'

interface ExecutionInput {
  [key: string]: unknown
}

export function useWorkflowExecution(workflowId: number) {
  const [result, setResult] = useState<ExecutionResult | null>(null)
  const setIsExecuting = useWorkflowStore((state) => state.setIsExecuting)

  const executeMutation = useMutation({
    mutationFn: (inputs: ExecutionInput) =>
      apiClient.post<ExecutionResult>(`/api/workflows/${workflowId}/execute`, inputs),
    onMutate: () => {
      setIsExecuting(true)
      setResult(null)
    },
    onSuccess: (data) => {
      setResult(data)
      setIsExecuting(false)
    },
    onError: () => {
      setIsExecuting(false)
    },
  })

  const execute = useCallback(
    (inputs: ExecutionInput) => {
      executeMutation.mutate(inputs)
    },
    [executeMutation]
  )

  return {
    execute,
    result,
    isExecuting: executeMutation.isPending,
    error: executeMutation.error,
  }
}
```

### 4. Pages 頁面層

```
src/pages/
├── Home/
│   └── Home.tsx
├── Login/
│   └── Login.tsx
├── Dashboard/
│   └── Dashboard.tsx
├── Workflows/
│   ├── WorkflowList.tsx
│   ├── WorkflowDetail.tsx
│   ├── WorkflowCreate.tsx
│   └── WorkflowEdit.tsx
├── Agents/
│   ├── AgentList.tsx
│   └── PersonaBuilder.tsx
├── Settings/
│   └── Settings.tsx
└── NotFound/
    └── NotFound.tsx
```

#### Page 組件示例

```typescript
// src/pages/Workflows/WorkflowList.tsx
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useWorkflows, useDeleteWorkflow } from '@/api/workflows/queries'
import { Button } from '@/components/common/Button'
import { WorkflowCard } from '@/features/workflows/components/WorkflowCard'
import { Loading } from '@/components/common/Loading'
import { ErrorMessage } from '@/components/common/ErrorMessage'

export function WorkflowList() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)

  const { data, isLoading, error } = useWorkflows(page, 20)
  const deleteMutation = useDeleteWorkflow()

  const handleCreate = () => {
    navigate('/workflows/create')
  }

  const handleEdit = (id: number) => {
    navigate(`/workflows/${id}/edit`)
  }

  const handleDelete = async (id: number) => {
    if (confirm('確定要刪除此 Workflow 嗎?')) {
      await deleteMutation.mutateAsync(id)
    }
  }

  const handleExecute = (id: number) => {
    navigate(`/workflows/${id}/execute`)
  }

  if (isLoading) {
    return <Loading />
  }

  if (error) {
    return <ErrorMessage message="載入 Workflow 列表失敗" />
  }

  return (
    <div className="workflow-list-page">
      <div className="page-header">
        <h1>My Workflows</h1>
        <Button onClick={handleCreate} variant="primary">
          + 創建新 Workflow
        </Button>
      </div>

      <div className="workflow-grid">
        {data?.workflows.map((workflow) => (
          <WorkflowCard
            key={workflow.id}
            workflow={workflow}
            onEdit={() => handleEdit(workflow.id)}
            onDelete={() => handleDelete(workflow.id)}
            onExecute={() => handleExecute(workflow.id)}
          />
        ))}
      </div>

      {data && data.totalPages > 1 && (
        <div className="pagination">
          <Button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
          >
            上一頁
          </Button>
          <span>
            第 {page} 頁 / 共 {data.totalPages} 頁
          </span>
          <Button
            onClick={() => setPage((p) => p + 1)}
            disabled={page >= data.totalPages}
          >
            下一頁
          </Button>
        </div>
      )}
    </div>
  )
}
```

### 5. 路由配置

```
src/routes/
├── index.tsx                              # 路由配置入口
├── ProtectedRoute.tsx                     # 受保護路由組件
└── routes.ts                              # 路由定義
```

#### 路由配置示例

```typescript
// src/routes/index.tsx
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import { MainLayout } from '@/layouts/MainLayout'
import { AuthLayout } from '@/layouts/AuthLayout'
import { ProtectedRoute } from './ProtectedRoute'

// Pages
import { Home } from '@/pages/Home'
import { Login } from '@/pages/Login'
import { Dashboard } from '@/pages/Dashboard'
import { WorkflowList } from '@/pages/Workflows/WorkflowList'
import { WorkflowDetail } from '@/pages/Workflows/WorkflowDetail'
import { WorkflowCreate } from '@/pages/Workflows/WorkflowCreate'
import { WorkflowEdit } from '@/pages/Workflows/WorkflowEdit'
import { AgentList } from '@/pages/Agents/AgentList'
import { PersonaBuilder } from '@/pages/Agents/PersonaBuilder'
import { Settings } from '@/pages/Settings'
import { NotFound } from '@/pages/NotFound'

const router = createBrowserRouter([
  {
    path: '/',
    element: <MainLayout />,
    children: [
      {
        index: true,
        element: <Home />,
      },
      {
        path: 'dashboard',
        element: (
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: 'workflows',
        children: [
          {
            index: true,
            element: (
              <ProtectedRoute>
                <WorkflowList />
              </ProtectedRoute>
            ),
          },
          {
            path: 'create',
            element: (
              <ProtectedRoute>
                <WorkflowCreate />
              </ProtectedRoute>
            ),
          },
          {
            path: ':id',
            element: (
              <ProtectedRoute>
                <WorkflowDetail />
              </ProtectedRoute>
            ),
          },
          {
            path: ':id/edit',
            element: (
              <ProtectedRoute>
                <WorkflowEdit />
              </ProtectedRoute>
            ),
          },
        ],
      },
      {
        path: 'agents',
        children: [
          {
            index: true,
            element: (
              <ProtectedRoute>
                <AgentList />
              </ProtectedRoute>
            ),
          },
          {
            path: 'persona-builder',
            element: (
              <ProtectedRoute>
                <PersonaBuilder />
              </ProtectedRoute>
            ),
          },
        ],
      },
      {
        path: 'settings',
        element: (
          <ProtectedRoute>
            <Settings />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    path: '/auth',
    element: <AuthLayout />,
    children: [
      {
        path: 'login',
        element: <Login />,
      },
    ],
  },
  {
    path: '*',
    element: <NotFound />,
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}
```

#### Protected Route 示例

```typescript
// src/routes/ProtectedRoute.tsx
import { Navigate } from 'react-router-dom'
import { useAuthStore } from '@/features/auth/stores/authStore'

interface ProtectedRouteProps {
  children: React.ReactNode
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)

  if (!isAuthenticated) {
    return <Navigate to="/auth/login" replace />
  }

  return <>{children}</>
}
```

### 6. Layouts 佈局層

```
src/layouts/
├── MainLayout/
│   ├── MainLayout.tsx
│   └── MainLayout.module.css
└── AuthLayout/
    ├── AuthLayout.tsx
    └── AuthLayout.module.css
```

#### Layout 組件示例

```typescript
// src/layouts/MainLayout/MainLayout.tsx
import { Outlet } from 'react-router-dom'
import { Header } from '@/components/layout/Header'
import { Sidebar } from '@/components/layout/Sidebar'
import { Footer } from '@/components/layout/Footer'
import styles from './MainLayout.module.css'

export function MainLayout() {
  return (
    <div className={styles.layout}>
      <Header />
      <div className={styles.container}>
        <Sidebar />
        <main className={styles.main}>
          <Outlet />
        </main>
      </div>
      <Footer />
    </div>
  )
}
```

### 7. Types 類型定義

```
src/types/
├── api.ts                                 # API 相關類型
├── common.ts                              # 通用類型
├── workflow.ts                            # Workflow 類型
├── agent.ts                               # Agent 類型
└── user.ts                                # User 類型
```

#### 類型定義示例

```typescript
// src/types/workflow.ts
export interface Workflow {
  id: number
  name: string
  description: string
  ownerId: number
  owner: User
  nodes: WorkflowNode[]
  edges: WorkflowEdge[]
  status: WorkflowStatus
  createdAt: string
  lastModifiedAt?: string
}

export interface WorkflowNode {
  id: string
  type: 'prompt' | 'agent' | 'function' | 'condition'
  position: { x: number; y: number }
  data: {
    label: string
    config: Record<string, unknown>
  }
}

export interface WorkflowEdge {
  id: string
  source: string
  target: string
  type?: string
}

export type WorkflowStatus = 'draft' | 'published' | 'executing' | 'completed' | 'failed'

export interface CreateWorkflowDto {
  name: string
  description: string
  nodes: WorkflowNode[]
  edges: WorkflowEdge[]
}

export interface ExecutionResult {
  workflowId: number
  status: 'success' | 'failed'
  results: Record<string, unknown>
  executedAt: string
  duration: number
}
```

### 8. Utilities 工具函數

```
src/utils/
├── format.ts                              # 格式化工具
├── validation.ts                          # 驗證工具
├── date.ts                                # 日期處理
├── storage.ts                             # 本地存儲
└── constants.ts                           # 常量定義
```

#### 工具函數示例

```typescript
// src/utils/format.ts
export function formatDate(date: string | Date): string {
  return new Intl.DateTimeFormat('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(date))
}

export function formatDuration(milliseconds: number): string {
  const seconds = Math.floor(milliseconds / 1000)
  const minutes = Math.floor(seconds / 60)
  const hours = Math.floor(minutes / 60)

  if (hours > 0) {
    return `${hours}h ${minutes % 60}m`
  }
  if (minutes > 0) {
    return `${minutes}m ${seconds % 60}s`
  }
  return `${seconds}s`
}

// src/utils/validation.ts
export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export function isValidPassword(password: string): boolean {
  // 至少 8 個字符,包含大小寫字母和數字
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/
  return passwordRegex.test(password)
}
```

## 應用配置

### 1. Vite 配置

```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@components': path.resolve(__dirname, './src/components'),
      '@features': path.resolve(__dirname, './src/features'),
      '@api': path.resolve(__dirname, './src/api'),
      '@utils': path.resolve(__dirname, './src/utils'),
      '@types': path.resolve(__dirname, './src/types'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          react: ['react', 'react-dom'],
          router: ['react-router-dom'],
          query: ['@tanstack/react-query'],
        },
      },
    },
  },
})
```

### 2. TypeScript 配置

```json
// tsconfig.json
{
  "compilerOptions": {
    "target": "ES2022",
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"],
      "@components/*": ["src/components/*"],
      "@features/*": ["src/features/*"],
      "@api/*": ["src/api/*"],
      "@utils/*": ["src/utils/*"],
      "@types/*": ["src/types/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

### 3. React Query 配置

```typescript
// src/main.tsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import App from './App'
import './styles/index.css'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 10, // 10 minutes (renamed from cacheTime)
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
})

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  </React.StrictMode>
)
```

### 4. App 根組件

```typescript
// src/App.tsx
import { AppRouter } from './routes'
import { ErrorBoundary } from './components/common/ErrorBoundary'
import { Toaster } from 'react-hot-toast'

export default function App() {
  return (
    <ErrorBoundary>
      <AppRouter />
      <Toaster position="top-right" />
    </ErrorBoundary>
  )
}
```

## 狀態管理策略

### Zustand vs React Query

- **Zustand**: 客戶端 UI 狀態 (主題, 側邊欄狀態, 表單數據)
- **React Query**: 服務端數據 (API 請求, 緩存, 同步)

```typescript
// 客戶端 UI 狀態 - Zustand
interface UIState {
  theme: 'light' | 'dark'
  sidebarOpen: boolean
  toggleSidebar: () => void
}

export const useUIStore = create<UIState>((set) => ({
  theme: 'light',
  sidebarOpen: true,
  toggleSidebar: () => set((state) => ({ sidebarOpen: !state.sidebarOpen })),
}))

// 服務端數據 - React Query
const { data, isLoading } = useQuery({
  queryKey: ['workflows'],
  queryFn: fetchWorkflows,
})
```

## 性能優化

### 1. 代碼分割

```typescript
// 路由級代碼分割
import { lazy, Suspense } from 'react'

const WorkflowList = lazy(() => import('@/pages/Workflows/WorkflowList'))

function App() {
  return (
    <Suspense fallback={<Loading />}>
      <WorkflowList />
    </Suspense>
  )
}
```

### 2. React.memo 優化

```typescript
import { memo } from 'react'

interface WorkflowCardProps {
  workflow: Workflow
  onEdit: (id: number) => void
}

export const WorkflowCard = memo(
  function WorkflowCard({ workflow, onEdit }: WorkflowCardProps) {
    return <div onClick={() => onEdit(workflow.id)}>{workflow.name}</div>
  },
  (prevProps, nextProps) => prevProps.workflow.id === nextProps.workflow.id
)
```

### 3. useMemo 和 useCallback

```typescript
function WorkflowList() {
  const { data } = useWorkflows()

  // 緩存計算結果
  const sortedWorkflows = useMemo(() => {
    return data?.workflows.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  }, [data?.workflows])

  // 緩存回調函數
  const handleEdit = useCallback((id: number) => {
    navigate(`/workflows/${id}/edit`)
  }, [navigate])

  return <div>{/* ... */}</div>
}
```

## 測試策略

### Unit Tests (Vitest)

```typescript
// src/components/common/Button/Button.test.tsx
import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { Button } from './Button'

describe('Button', () => {
  it('renders children correctly', () => {
    render(<Button>Click me</Button>)
    expect(screen.getByText('Click me')).toBeInTheDocument()
  })

  it('calls onClick when clicked', () => {
    const handleClick = vi.fn()
    render(<Button onClick={handleClick}>Click</Button>)

    fireEvent.click(screen.getByText('Click'))
    expect(handleClick).toHaveBeenCalledTimes(1)
  })

  it('shows loading state', () => {
    render(<Button isLoading>Submit</Button>)
    expect(screen.getByText('Loading...')).toBeInTheDocument()
  })
})
```

## 最佳實踐

### 1. 組件組織
- ✅ 每個組件獨立文件夾 (組件 + 樣式 + 測試)
- ✅ 按功能分組組件,不按類型
- ✅ 使用 index.ts 導出

### 2. 狀態管理
- ✅ 服務端數據用 React Query
- ✅ 客戶端 UI 狀態用 Zustand
- ✅ 表單狀態用 React Hook Form

### 3. 性能
- ✅ 路由級代碼分割
- ✅ 合理使用 memo, useMemo, useCallback
- ✅ 虛擬滾動處理長列表

### 4. 類型安全
- ✅ 嚴格 TypeScript 配置
- ✅ API 響應類型定義
- ✅ Props 接口定義

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
