# React 19 編碼標準

## 版本信息
- **React 版本**: 19.0
- **TypeScript**: 5.8
- **最後更新**: 2025-01-01

## 組件結構

### 函數組件 (推薦)
```typescript
// ✅ 推薦: 函數組件 + TypeScript
interface UserProfileProps {
  user: User;
  onEdit?: (user: User) => void;
}

export function UserProfile({ user, onEdit }: UserProfileProps) {
  return (
    <div className="user-profile">
      <h2>{user.name}</h2>
      <p>{user.email}</p>
      {onEdit && <button onClick={() => onEdit(user)}>Edit</button>}
    </div>
  );
}

// ❌ 避免: 類組件 (舊式,除非必要)
class UserProfile extends React.Component<UserProfileProps> {
  render() {
    return <div>{this.props.user.name}</div>;
  }
}
```

## Hooks 最佳實踐

### useState
```typescript
// ✅ 基本使用
const [count, setCount] = useState<number>(0);
const [user, setUser] = useState<User | null>(null);

// ✅ 函數式更新 (基於前一個狀態)
setCount(prev => prev + 1);

// ✅ 惰性初始化 (昂貴計算)
const [data, setData] = useState(() => expensiveComputation());
```

### useEffect
```typescript
// ✅ 正確的依賴數組
useEffect(() => {
  fetchUser(userId).then(setUser);
}, [userId]);  // userId 變化時重新執行

// ✅ 清理函數
useEffect(() => {
  const subscription = dataSource.subscribe();
  return () => subscription.unsubscribe();
}, []);

// ❌ 避免: 缺少依賴
useEffect(() => {
  console.log(user.name);  // user 未在依賴數組中
}, []);
```

### useCallback 和 useMemo
```typescript
// ✅ useCallback - 記憶化回調函數
const handleClick = useCallback(() => {
  console.log(count);
}, [count]);

// ✅ useMemo - 記憶化昂貴計算
const sortedUsers = useMemo(() => {
  return users.sort((a, b) => a.name.localeCompare(b.name));
}, [users]);

// ❌ 過度使用 (簡單計算不需要)
const doubledCount = useMemo(() => count * 2, [count]);  // 不必要
```

### 自定義 Hooks
```typescript
// ✅ 自定義 Hook - 可重用邏輯
function useUser(userId: number) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    setLoading(true);
    fetchUser(userId)
      .then(setUser)
      .catch(setError)
      .finally(() => setLoading(false));
  }, [userId]);

  return { user, loading, error };
}

// 使用
function UserProfile({ userId }: { userId: number }) {
  const { user, loading, error } = useUser(userId);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!user) return null;

  return <div>{user.name}</div>;
}
```

## React 19 新特性

### useActionState (React 19)
```typescript
// ✅ 表單狀態管理
function TodoForm() {
  const [state, action, isPending] = useActionState(addTodo, null);

  return (
    <form action={action}>
      <input type="text" name="todo" />
      <button disabled={isPending}>
        {isPending ? "Adding..." : "Add Todo"}
      </button>
      {state?.error && <p>{state.error}</p>}
    </form>
  );
}
```

### useOptimistic (React 19)
```typescript
// ✅ 樂觀更新
function TodoList({ todos }: { todos: Todo[] }) {
  const [optimisticTodos, addOptimisticTodo] = useOptimistic(
    todos,
    (state, newTodo: Todo) => [...state, newTodo]
  );

  async function handleAdd(todo: Todo) {
    addOptimisticTodo(todo);
    await saveTodo(todo);
  }

  return (
    <ul>
      {optimisticTodos.map(todo => (
        <li key={todo.id}>{todo.text}</li>
      ))}
    </ul>
  );
}
```

## 組件組織

### 文件結構
```
components/
├── UserProfile/
│   ├── UserProfile.tsx
│   ├── UserProfile.test.tsx
│   ├── UserProfile.module.css
│   ├── useUserProfile.ts
│   └── index.ts
```

### 組件內部順序
```typescript
// 1. Import
import { useState, useEffect } from "react";
import { User } from "@/types";

// 2. Types/Interfaces
interface UserProfileProps {
  userId: number;
}

// 3. Constants
const DEFAULT_AVATAR = "/images/default-avatar.png";

// 4. Component
export function UserProfile({ userId }: UserProfileProps) {
  // 5. Hooks (按順序: useState, useEffect, custom hooks)
  const [user, setUser] = useState<User | null>(null);
  const { settings } = useSettings();

  useEffect(() => {
    fetchUser(userId).then(setUser);
  }, [userId]);

  // 6. Event handlers
  const handleEdit = () => {
    // ...
  };

  // 7. Render helpers
  const renderAvatar = () => {
    return <img src={user?.avatar ?? DEFAULT_AVATAR} />;
  };

  // 8. Early returns
  if (!user) return null;

  // 9. Main render
  return (
    <div>
      {renderAvatar()}
      <h2>{user.name}</h2>
    </div>
  );
}
```

## Props 最佳實踐

### Props 定義
```typescript
// ✅ 接口定義 Props
interface ButtonProps {
  children: React.ReactNode;
  variant?: "primary" | "secondary";
  disabled?: boolean;
  onClick?: () => void;
}

// ✅ 默認值
function Button({
  children,
  variant = "primary",
  disabled = false,
  onClick
}: ButtonProps) {
  return (
    <button
      className={`btn btn-${variant}`}
      disabled={disabled}
      onClick={onClick}
    >
      {children}
    </button>
  );
}

// ✅ 擴展 HTML 屬性
interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}
```

### Props 傳遞
```typescript
// ✅ 展開 props (謹慎使用)
const inputProps = { type: "text", value, onChange };
<input {...inputProps} />

// ✅ 子組件傳遞
interface ParentProps {
  children: React.ReactNode;
}

function Parent({ children }: ParentProps) {
  return <div className="parent">{children}</div>;
}
```

## 狀態管理

### Context API
```typescript
// ✅ 創建 Context
interface AuthContextType {
  user: User | null;
  login: (credentials: Credentials) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Provider
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  const login = async (credentials: Credentials) => {
    const user = await loginApi(credentials);
    setUser(user);
  };

  const logout = () => setUser(null);

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// 自定義 Hook
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
```

## 性能優化

### React.memo
```typescript
// ✅ 記憶化組件 (避免不必要的重新渲染)
export const UserCard = React.memo(function UserCard({ user }: { user: User }) {
  return <div>{user.name}</div>;
});

// ✅ 自定義比較函數
export const UserCard = React.memo(
  function UserCard({ user }: { user: User }) {
    return <div>{user.name}</div>;
  },
  (prevProps, nextProps) => prevProps.user.id === nextProps.user.id
);
```

### 列表渲染
```typescript
// ✅ 使用穩定的 key
users.map(user => <UserCard key={user.id} user={user} />)

// ❌ 避免: 使用 index 作為 key (可能導致問題)
users.map((user, index) => <UserCard key={index} user={user} />)
```

### 代碼分割
```typescript
// ✅ 懶加載組件
const UserDashboard = lazy(() => import("./UserDashboard"));

function App() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <UserDashboard />
    </Suspense>
  );
}
```

## 錯誤邊界

```typescript
// ✅ 錯誤邊界類組件
class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  { hasError: boolean; error: Error | null }
> {
  state = { hasError: false, error: null };

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    console.error("Error caught:", error, info);
  }

  render() {
    if (this.state.hasError) {
      return <div>Something went wrong: {this.state.error?.message}</div>;
    }
    return this.props.children;
  }
}

// 使用
<ErrorBoundary>
  <UserDashboard />
</ErrorBoundary>
```

## 測試

### React Testing Library
```typescript
import { render, screen, fireEvent } from "@testing-library/react";
import { Button } from "./Button";

describe("Button", () => {
  it("renders children correctly", () => {
    render(<Button>Click me</Button>);
    expect(screen.getByText("Click me")).toBeInTheDocument();
  });

  it("calls onClick when clicked", () => {
    const handleClick = jest.fn();
    render(<Button onClick={handleClick}>Click</Button>);

    fireEvent.click(screen.getByText("Click"));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("is disabled when disabled prop is true", () => {
    render(<Button disabled>Disabled</Button>);
    expect(screen.getByRole("button")).toBeDisabled();
  });
});
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
