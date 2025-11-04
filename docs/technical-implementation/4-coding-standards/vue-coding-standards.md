# Vue 3.5 編碼標準

## 版本信息
- **Vue 版本**: 3.5
- **TypeScript**: 5.8
- **最後更新**: 2025-01-01

## 組件結構

### Composition API (推薦)
```vue
<script setup lang="ts">
// ✅ 推薦: <script setup> + TypeScript
import { ref, computed, onMounted } from 'vue'
import type { User } from '@/types'

// Props 定義
interface Props {
  user: User
  editable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  editable: false
})

// Emits 定義
interface Emits {
  (e: 'update', user: User): void
  (e: 'delete', id: number): void
}

const emit = defineEmits<Emits>()

// Reactive state
const isEditing = ref(false)
const editedUser = ref<User>({ ...props.user })

// Computed
const displayName = computed(() => {
  return `${editedUser.value.firstName} ${editedUser.value.lastName}`
})

// Methods
function handleSave() {
  emit('update', editedUser.value)
  isEditing.value = false
}

// Lifecycle
onMounted(() => {
  console.log('Component mounted')
})
</script>

<template>
  <div class="user-profile">
    <h2>{{ displayName }}</h2>
    <button v-if="editable" @click="handleSave">
      Save
    </button>
  </div>
</template>

<style scoped>
.user-profile {
  padding: 1rem;
}
</style>
```

### ❌ 避免: Options API (舊式)
```vue
<script lang="ts">
// 僅在維護舊代碼時使用
export default {
  data() {
    return {
      count: 0
    }
  },
  methods: {
    increment() {
      this.count++
    }
  }
}
</script>
```

## Composition API 核心概念

### ref 和 reactive
```typescript
import { ref, reactive } from 'vue'

// ✅ ref - 基本類型和對象
const count = ref<number>(0)
const user = ref<User | null>(null)

// 訪問值
console.log(count.value)
count.value++

// ✅ reactive - 對象
const state = reactive({
  count: 0,
  users: [] as User[]
})

// 直接訪問
console.log(state.count)
state.count++

// ❌ 避免: reactive 用於基本類型
const count = reactive(0)  // 不起作用
```

### computed
```typescript
// ✅ 只讀 computed
const fullName = computed(() => {
  return `${firstName.value} ${lastName.value}`
})

// ✅ 可寫 computed
const fullName = computed({
  get: () => `${firstName.value} ${lastName.value}`,
  set: (value) => {
    const [first, last] = value.split(' ')
    firstName.value = first
    lastName.value = last
  }
})
```

### watch 和 watchEffect
```typescript
// ✅ watch - 監聽特定來源
watch(userId, (newId, oldId) => {
  console.log(`User ID changed from ${oldId} to ${newId}`)
  fetchUser(newId)
})

// ✅ watch 多個來源
watch([firstName, lastName], ([newFirst, newLast]) => {
  console.log(`Name: ${newFirst} ${newLast}`)
})

// ✅ watchEffect - 自動追蹤依賴
watchEffect(() => {
  console.log(`Count is: ${count.value}`)
})

// ✅ 停止監聽
const stopWatch = watch(count, () => {})
stopWatch()  // 停止監聽
```

## 組合式函數 (Composables)

### 創建可重用邏輯
```typescript
// useUser.ts
import { ref, onMounted } from 'vue'
import type { Ref } from 'vue'
import type { User } from '@/types'

export function useUser(userId: Ref<number>) {
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<Error | null>(null)

  async function fetchUser() {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`/api/users/${userId.value}`)
      user.value = await response.json()
    } catch (e) {
      error.value = e as Error
    } finally {
      loading.value = false
    }
  }

  // 自動重新獲取
  watch(userId, fetchUser, { immediate: true })

  return {
    user,
    loading,
    error,
    refetch: fetchUser
  }
}

// 使用
<script setup lang="ts">
const userId = ref(123)
const { user, loading, error, refetch } = useUser(userId)
</script>
```

## Props 和 Emits

### Props 定義
```typescript
// ✅ 基本 Props
interface Props {
  title: string
  count?: number
  user: User
  tags?: string[]
}

const props = defineProps<Props>()

// ✅ 默認值
const props = withDefaults(defineProps<Props>(), {
  count: 0,
  tags: () => []
})

// ✅ 運行時驗證 (可選)
const props = defineProps({
  title: {
    type: String,
    required: true
  },
  count: {
    type: Number,
    default: 0,
    validator: (value: number) => value >= 0
  }
})
```

### Emits 定義
```typescript
// ✅ TypeScript 類型
interface Emits {
  (e: 'update', value: string): void
  (e: 'delete', id: number): void
  (e: 'submit', data: FormData): void
}

const emit = defineEmits<Emits>()

// 使用
emit('update', 'new value')
emit('delete', 123)
```

## 模板語法

### 指令
```vue
<template>
  <!-- v-if / v-else-if / v-else -->
  <div v-if="loading">Loading...</div>
  <div v-else-if="error">Error: {{ error.message }}</div>
  <div v-else>{{ data }}</div>

  <!-- v-show (保留 DOM,切換 display) -->
  <div v-show="isVisible">Toggle me</div>

  <!-- v-for -->
  <ul>
    <li v-for="user in users" :key="user.id">
      {{ user.name }}
    </li>
  </ul>

  <!-- v-model -->
  <input v-model="searchQuery" type="text" />
  <input v-model.number="age" type="number" />
  <input v-model.trim="username" />

  <!-- v-bind (簡寫 :) -->
  <img :src="user.avatar" :alt="user.name" />

  <!-- v-on (簡寫 @) -->
  <button @click="handleClick">Click</button>
  <input @input="handleInput" @keyup.enter="submit" />

  <!-- 修飾符 -->
  <form @submit.prevent="onSubmit">
    <button @click.stop="handleClick">Click</button>
  </form>
</template>
```

### 條件渲染最佳實踐
```vue
<!-- ✅ 優先使用 v-if (不渲染 DOM) -->
<div v-if="showContent">Content</div>

<!-- ✅ 頻繁切換使用 v-show (切換 CSS display) -->
<div v-show="isVisible">Toggle frequently</div>

<!-- ❌ 避免: v-if 和 v-for 同時使用 -->
<div v-for="item in items" v-if="item.isActive">Bad</div>

<!-- ✅ 推薦: 使用 computed -->
<script setup lang="ts">
const activeItems = computed(() => items.value.filter(item => item.isActive))
</script>
<div v-for="item in activeItems" :key="item.id">Good</div>
```

## 樣式

### Scoped CSS
```vue
<style scoped>
/* 只影響當前組件 */
.user-profile {
  padding: 1rem;
}

/* 深度選擇器 - 影響子組件 */
.user-profile :deep(.child-class) {
  color: red;
}

/* Slotted - 影響插槽內容 */
.user-profile :slotted(.slot-content) {
  margin: 0.5rem;
}
</style>
```

### CSS Modules
```vue
<template>
  <div :class="$style.container">
    <h1 :class="$style.title">Title</h1>
  </div>
</template>

<style module>
.container {
  padding: 1rem;
}

.title {
  font-size: 2rem;
}
</style>
```

## VueFlow 特定標準

### 節點組件
```vue
<script setup lang="ts">
import { Handle, Position } from '@vue-flow/core'
import type { NodeProps } from '@vue-flow/core'

interface CustomNodeData {
  label: string
  icon?: string
  description?: string
}

const props = defineProps<NodeProps<CustomNodeData>>()

const emit = defineEmits<{
  (e: 'nodeClick', data: CustomNodeData): void
}>()
</script>

<template>
  <div class="custom-node">
    <Handle type="target" :position="Position.Left" />

    <div class="node-content">
      <span v-if="data.icon" class="icon">{{ data.icon }}</span>
      <div class="label">{{ data.label }}</div>
      <div v-if="data.description" class="description">
        {{ data.description }}
      </div>
    </div>

    <Handle type="source" :position="Position.Right" />
  </div>
</template>

<style scoped>
.custom-node {
  background: white;
  border: 2px solid #1a192b;
  border-radius: 8px;
  padding: 10px;
}
</style>
```

### Y.js CRDT 集成
```typescript
// useYjsSync.ts
import { ref, watch } from 'vue'
import * as Y from 'yjs'
import { WebsocketProvider } from 'y-websocket'

export function useYjsSync<T>(roomName: string) {
  const ydoc = new Y.Doc()
  const yArray = ydoc.getArray<T>('items')
  const items = ref<T[]>([])

  // WebSocket provider
  const provider = new WebsocketProvider(
    'ws://localhost:3001',
    roomName,
    ydoc
  )

  // 同步標誌
  const isSyncing = ref(false)

  // 觀察 Y.js 變化
  yArray.observe(() => {
    if (!isSyncing.value) {
      items.value = yArray.toArray()
    }
  })

  // 同步本地變化到 Y.js
  function syncToYjs() {
    isSyncing.value = true
    yArray.delete(0, yArray.length)
    items.value.forEach(item => yArray.push([item]))
    isSyncing.value = false
  }

  // 清理
  onUnmounted(() => {
    provider.destroy()
    ydoc.destroy()
  })

  return {
    items,
    syncToYjs,
    provider,
    ydoc
  }
}
```

## 性能優化

### v-once 和 v-memo
```vue
<template>
  <!-- 只渲染一次 -->
  <div v-once>{{ staticContent }}</div>

  <!-- 記憶化渲染 (Vue 3.2+) -->
  <div v-memo="[user.id, user.name]">
    {{ user.name }} - {{ user.email }}
  </div>
</template>
```

### 異步組件
```typescript
import { defineAsyncComponent } from 'vue'

// ✅ 懶加載組件
const UserDashboard = defineAsyncComponent(() =>
  import('./UserDashboard.vue')
)

// ✅ 帶加載狀態
const UserDashboard = defineAsyncComponent({
  loader: () => import('./UserDashboard.vue'),
  loadingComponent: LoadingSpinner,
  errorComponent: ErrorDisplay,
  delay: 200,
  timeout: 3000
})
```

## 測試

### Vitest + Vue Test Utils
```typescript
import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import UserProfile from './UserProfile.vue'

describe('UserProfile', () => {
  it('renders user name', () => {
    const wrapper = mount(UserProfile, {
      props: {
        user: { id: 1, name: 'Alice' }
      }
    })

    expect(wrapper.text()).toContain('Alice')
  })

  it('emits update event on save', async () => {
    const wrapper = mount(UserProfile, {
      props: {
        user: { id: 1, name: 'Alice' },
        editable: true
      }
    })

    await wrapper.find('button').trigger('click')

    expect(wrapper.emitted('update')).toBeTruthy()
  })
})
```

## TypeScript 集成

### defineComponent (舊式,不推薦)
```typescript
// ❌ 不推薦 (除非維護舊代碼)
import { defineComponent } from 'vue'

export default defineComponent({
  props: {
    title: String
  },
  setup(props) {
    return {}
  }
})
```

### script setup (推薦)
```vue
<script setup lang="ts">
// ✅ 推薦: 自動類型推斷
import { ref } from 'vue'

const count = ref(0)  // 自動推斷為 Ref<number>
</script>
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
