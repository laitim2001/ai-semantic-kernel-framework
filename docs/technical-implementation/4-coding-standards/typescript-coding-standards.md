# TypeScript 5.8 編碼標準

## 版本信息
- **TypeScript 版本**: 5.8
- **目標 ECMAScript**: ES2022
- **最後更新**: 2025-01-01

## 命名約定

### camelCase
- **變數**: `const userName = "Alice";`
- **函數**: `function fetchUserData() { }`
- **方法**: `getUserById(id: number) { }`
- **參數**: `function saveUser(userId: number) { }`

### PascalCase
- **類**: `class UserService { }`
- **接口**: `interface IUserRepository { }`
- **類型別名**: `type UserRole = "admin" | "user";`
- **枚舉**: `enum Status { Active, Inactive }`
- **React 組件**: `function UserProfile() { }`

### SCREAMING_SNAKE_CASE
- **全局常量**: `const MAX_RETRY_COUNT = 3;`
- **環境變數**: `const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;`

## TypeScript 配置

### tsconfig.json (嚴格模式)
```json
{
  "compilerOptions": {
    "target": "ES2022",
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "moduleResolution": "bundler",
    "resolveJsonModule": true,
    "allowImportingTsExtensions": true,
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "noUncheckedIndexedAccess": true,
    "exactOptionalPropertyTypes": true,
    "skipLibCheck": true,
    "esModuleInterop": true,
    "allowSyntheticDefaultImports": true,
    "forceConsistentCasingInFileNames": true,
    "isolatedModules": true,
    "jsx": "react-jsx",
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"],
      "@components/*": ["src/components/*"],
      "@services/*": ["src/services/*"]
    }
  },
  "include": ["src"],
  "exclude": ["node_modules", "dist"]
}
```

## 類型系統

### 基本類型
```typescript
// ✅ 明確類型註解
const userName: string = "Alice";
const userAge: number = 25;
const isActive: boolean = true;
const tags: string[] = ["developer", "designer"];

// ✅ 類型推斷 (簡單情況)
const count = 42;  // 推斷為 number
const name = "Bob";  // 推斷為 string

// ❌ 避免 any
const data: any = fetchData();  // 類型不安全

// ✅ 使用 unknown
const data: unknown = fetchData();
if (typeof data === "string") {
  console.log(data.toUpperCase());
}
```

### 接口 vs 類型別名
```typescript
// ✅ 接口 (對象形狀,可擴展)
interface User {
  id: number;
  name: string;
  email: string;
}

interface AdminUser extends User {
  permissions: string[];
}

// ✅ 類型別名 (聯合類型,交叉類型,實用類型)
type UserRole = "admin" | "user" | "guest";
type UserId = number | string;
type PartialUser = Partial<User>;

// 交叉類型
type Employee = User & {
  department: string;
  salary: number;
};
```

### 泛型
```typescript
// ✅ 泛型函數
function findById<T extends { id: number }>(items: T[], id: number): T | undefined {
  return items.find(item => item.id === id);
}

// ✅ 泛型接口
interface ApiResponse<T> {
  data: T;
  status: number;
  message: string;
}

// 使用
const userResponse: ApiResponse<User> = await fetchUser(1);

// ✅ 泛型約束
function getProperty<T, K extends keyof T>(obj: T, key: K): T[K] {
  return obj[key];
}
```

### Utility Types
```typescript
interface User {
  id: number;
  name: string;
  email: string;
  password: string;
}

// Partial - 所有屬性可選
type PartialUser = Partial<User>;

// Required - 所有屬性必需
type RequiredUser = Required<PartialUser>;

// Readonly - 所有屬性只讀
type ReadonlyUser = Readonly<User>;

// Pick - 選擇特定屬性
type UserPreview = Pick<User, "id" | "name">;

// Omit - 排除特定屬性
type UserWithoutPassword = Omit<User, "password">;

// Record - 創建對象類型
type UserMap = Record<string, User>;
```

## 函數

### 函數聲明
```typescript
// ✅ 顯式返回類型
function calculateTotal(items: number[]): number {
  return items.reduce((sum, item) => sum + item, 0);
}

// ✅ 箭頭函數
const multiply = (a: number, b: number): number => a * b;

// ✅ 可選參數和默認值
function greet(name: string, greeting: string = "Hello"): string {
  return `${greeting}, ${name}!`;
}

// ✅ Rest 參數
function sum(...numbers: number[]): number {
  return numbers.reduce((a, b) => a + b, 0);
}

// ✅ 函數重載
function formatDate(date: Date): string;
function formatDate(timestamp: number): string;
function formatDate(value: Date | number): string {
  const date = value instanceof Date ? value : new Date(value);
  return date.toISOString();
}
```

### Async/Await
```typescript
// ✅ 推薦: 明確 Promise 類型
async function fetchUser(id: number): Promise<User> {
  const response = await fetch(`/api/users/${id}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch user: ${response.statusText}`);
  }
  return response.json();
}

// ✅ 錯誤處理
async function getUserSafely(id: number): Promise<User | null> {
  try {
    return await fetchUser(id);
  } catch (error) {
    console.error("Error fetching user:", error);
    return null;
  }
}

// ✅ 並行執行
async function getUserWithOrders(userId: number): Promise<[User, Order[]]> {
  const [user, orders] = await Promise.all([
    fetchUser(userId),
    fetchOrders(userId)
  ]);
  return [user, orders];
}
```

## 類

### 類定義
```typescript
// ✅ 現代類語法
class UserService {
  // 私有字段 (TypeScript 5.0+)
  #apiClient: ApiClient;

  // 只讀屬性
  readonly baseUrl: string;

  // 構造函數
  constructor(apiClient: ApiClient, baseUrl: string) {
    this.#apiClient = apiClient;
    this.baseUrl = baseUrl;
  }

  // 公共方法
  async getUser(id: number): Promise<User> {
    return this.#apiClient.get<User>(`${this.baseUrl}/users/${id}`);
  }

  // 私有方法
  #validateUser(user: User): boolean {
    return user.name.length > 0 && user.email.includes("@");
  }

  // 靜態方法
  static createDefault(): UserService {
    return new UserService(new ApiClient(), "/api");
  }
}
```

## 類型守衛

### 自定義類型守衛
```typescript
// ✅ 類型謂詞
function isUser(value: unknown): value is User {
  return (
    typeof value === "object" &&
    value !== null &&
    "id" in value &&
    "name" in value &&
    typeof (value as User).id === "number" &&
    typeof (value as User).name === "string"
  );
}

// 使用
function processData(data: unknown): void {
  if (isUser(data)) {
    console.log(data.name);  // TypeScript 知道 data 是 User
  }
}

// ✅ instanceof 守衛
class ApiError extends Error {
  constructor(public statusCode: number, message: string) {
    super(message);
  }
}

function handleError(error: unknown): void {
  if (error instanceof ApiError) {
    console.error(`API Error ${error.statusCode}: ${error.message}`);
  } else if (error instanceof Error) {
    console.error(`Error: ${error.message}`);
  }
}
```

## 模塊組織

### 導入/導出
```typescript
// ✅ 命名導出 (推薦用於多個導出)
// user-service.ts
export class UserService { }
export interface UserServiceConfig { }

// 導入
import { UserService, UserServiceConfig } from "./user-service";

// ✅ 默認導出 (僅單一主要導出)
// user.ts
export default class User { }

// 導入
import User from "./user";

// ✅ 重新導出
export { UserService } from "./user-service";
export type { User, UserRole } from "./types";

// ❌ 避免: 混合 import * as
import * as UserModule from "./user";  // 不推薦
```

### 文件結構
```
src/
├── types/
│   ├── user.types.ts
│   ├── api.types.ts
│   └── index.ts
├── services/
│   ├── user.service.ts
│   ├── auth.service.ts
│   └── index.ts
├── utils/
│   ├── validation.ts
│   ├── formatting.ts
│   └── index.ts
└── index.ts
```

## 錯誤處理

### 自定義錯誤類
```typescript
// ✅ 自定義錯誤
class ValidationError extends Error {
  constructor(
    public field: string,
    message: string
  ) {
    super(message);
    this.name = "ValidationError";
  }
}

class ApiError extends Error {
  constructor(
    public statusCode: number,
    message: string,
    public details?: unknown
  ) {
    super(message);
    this.name = "ApiError";
  }
}

// 使用
function validateUser(user: User): void {
  if (!user.name) {
    throw new ValidationError("name", "Name is required");
  }
  if (!user.email.includes("@")) {
    throw new ValidationError("email", "Invalid email format");
  }
}
```

## 最佳實踐

### 避免類型斷言
```typescript
// ❌ 避免: 過度使用 as
const user = data as User;

// ✅ 推薦: 使用類型守衛
if (isUser(data)) {
  const user = data;  // TypeScript 自動推斷類型
}

// ❌ 避免: 雙重斷言
const value = (data as unknown) as string;

// ✅ 推薦: 正確的類型設計
```

### 嚴格的 null 檢查
```typescript
// ✅ 處理可能為 null 的值
function getUserName(user: User | null): string {
  return user?.name ?? "Unknown";
}

// ✅ 非空斷言 (確定不為 null 時)
function getUser(id: number): User {
  const user = cache.get(id);
  // 只在確定 user 存在時使用
  return user!;
}
```

### 避免魔法數字/字符串
```typescript
// ❌ 避免
if (user.role === "admin") { }

// ✅ 推薦: 使用常量或枚舉
const USER_ROLES = {
  ADMIN: "admin",
  USER: "user",
  GUEST: "guest"
} as const;

type UserRole = typeof USER_ROLES[keyof typeof USER_ROLES];

if (user.role === USER_ROLES.ADMIN) { }

// 或使用枚舉
enum UserRole {
  Admin = "admin",
  User = "user",
  Guest = "guest"
}
```

## 註釋規範

### JSDoc 註釋
```typescript
/**
 * 根據用戶 ID 獲取用戶信息
 * @param id - 用戶的唯一標識符
 * @returns 包含用戶信息的 Promise
 * @throws {ApiError} 當 API 請求失敗時
 * @example
 * ```typescript
 * const user = await getUserById(123);
 * console.log(user.name);
 * ```
 */
async function getUserById(id: number): Promise<User> {
  // 實現...
}
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
