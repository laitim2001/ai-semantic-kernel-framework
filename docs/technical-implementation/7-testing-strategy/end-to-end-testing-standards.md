# 端到端測試標準 (End-to-End Testing Standards)

## 概述

本文檔定義 AI Workflow Platform 的端到端測試標準,使用 Playwright 框架測試完整的用戶場景,確保從前端到後端的整個系統正確運作。

## 目標

- ✅ 實現 5-10% 的端到端測試覆蓋率
- ✅ 測試關鍵用戶場景和業務流程
- ✅ 驗證跨瀏覽器兼容性
- ✅ 自動化視覺回歸測試
- ✅ 確保無障礙性 (Accessibility) 合規

## Playwright 框架配置

### 項目配置

```typescript
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test'

export default defineConfig({
  testDir: './tests/e2e',

  // 完整測試超時
  timeout: 60 * 1000,  // 60 秒

  // Expect 超時
  expect: {
    timeout: 10000
  },

  // 失敗時重試
  retries: process.env.CI ? 2 : 0,

  // 並行執行
  workers: process.env.CI ? 2 : 4,

  // 報告配置
  reporter: [
    ['html', { outputFolder: 'playwright-report' }],
    ['json', { outputFile: 'test-results/results.json' }],
    ['junit', { outputFile: 'test-results/junit.xml' }],
    ['list']
  ],

  // 全局設置
  use: {
    // 基礎 URL
    baseURL: process.env.BASE_URL || 'http://localhost:5173',

    // 截圖配置
    screenshot: 'only-on-failure',

    // 視頻配置
    video: 'retain-on-failure',

    // Trace 配置
    trace: 'retain-on-failure',

    // 視口大小
    viewport: { width: 1920, height: 1080 },

    // 操作超時
    actionTimeout: 15000,

    // 導航超時
    navigationTimeout: 30000
  },

  // 瀏覽器配置
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] }
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] }
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] }
    },
    // 移動端測試
    {
      name: 'mobile-chrome',
      use: { ...devices['Pixel 5'] }
    },
    {
      name: 'mobile-safari',
      use: { ...devices['iPhone 13'] }
    }
  ],

  // 開發服務器
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
    reuseExistingServer: !process.env.CI,
    timeout: 120000
  }
})
```

```json
// package.json
{
  "devDependencies": {
    "@playwright/test": "^1.40.1",
    "@axe-core/playwright": "^4.8.3"
  },
  "scripts": {
    "test:e2e": "playwright test",
    "test:e2e:ui": "playwright test --ui",
    "test:e2e:headed": "playwright test --headed",
    "test:e2e:debug": "playwright test --debug",
    "test:e2e:codegen": "playwright codegen http://localhost:5173",
    "test:e2e:report": "playwright show-report"
  }
}
```

## 測試組織結構

```
tests/
├── e2e/
│   ├── fixtures/
│   │   ├── auth.fixture.ts        # 認證 Fixture
│   │   └── workflow.fixture.ts    # 工作流程 Fixture
│   ├── pages/
│   │   ├── LoginPage.ts           # 登入頁面 POM
│   │   ├── WorkflowListPage.ts    # 工作流程列表 POM
│   │   └── WorkflowEditorPage.ts  # 編輯器 POM
│   ├── specs/
│   │   ├── auth.spec.ts           # 認證測試
│   │   ├── workflow-crud.spec.ts  # CRUD 測試
│   │   ├── workflow-editor.spec.ts # 編輯器測試
│   │   └── collaboration.spec.ts   # 協同編輯測試
│   └── utils/
│       ├── test-data.ts           # 測試數據
│       └── helpers.ts             # 輔助函數
```

## Page Object Model (POM)

### 登入頁面

```typescript
// tests/e2e/pages/LoginPage.ts
import { Page, Locator } from '@playwright/test'

export class LoginPage {
  readonly page: Page
  readonly emailInput: Locator
  readonly passwordInput: Locator
  readonly loginButton: Locator
  readonly errorMessage: Locator

  constructor(page: Page) {
    this.page = page
    this.emailInput = page.getByLabel('Email')
    this.passwordInput = page.getByLabel('Password')
    this.loginButton = page.getByRole('button', { name: 'Login' })
    this.errorMessage = page.getByRole('alert')
  }

  async goto() {
    await this.page.goto('/login')
  }

  async login(email: string, password: string) {
    await this.emailInput.fill(email)
    await this.passwordInput.fill(password)
    await this.loginButton.click()

    // 等待導航完成
    await this.page.waitForURL('/dashboard')
  }

  async loginWithError(email: string, password: string) {
    await this.emailInput.fill(email)
    await this.passwordInput.fill(password)
    await this.loginButton.click()

    // 等待錯誤消息出現
    await this.errorMessage.waitFor({ state: 'visible' })
  }

  async getErrorMessage(): Promise<string> {
    return await this.errorMessage.textContent() || ''
  }
}
```

### 工作流程列表頁面

```typescript
// tests/e2e/pages/WorkflowListPage.ts
import { Page, Locator } from '@playwright/test'

export class WorkflowListPage {
  readonly page: Page
  readonly createButton: Locator
  readonly searchInput: Locator
  readonly workflowCards: Locator

  constructor(page: Page) {
    this.page = page
    this.createButton = page.getByRole('button', { name: 'Create Workflow' })
    this.searchInput = page.getByPlaceholder('Search workflows...')
    this.workflowCards = page.getByTestId('workflow-card')
  }

  async goto() {
    await this.page.goto('/workflows')
  }

  async createWorkflow() {
    await this.createButton.click()
    await this.page.waitForURL('/workflows/new')
  }

  async searchWorkflow(query: string) {
    await this.searchInput.fill(query)
    await this.searchInput.press('Enter')

    // 等待搜索結果加載
    await this.page.waitForResponse(
      (response) => response.url().includes('/api/v1/workflows') && response.status() === 200
    )
  }

  async getWorkflowCount(): Promise<number> {
    return await this.workflowCards.count()
  }

  async openWorkflow(workflowName: string) {
    await this.page.getByRole('link', { name: workflowName }).click()
  }

  async deleteWorkflow(workflowName: string) {
    const card = this.page.getByTestId('workflow-card').filter({ hasText: workflowName })
    await card.getByRole('button', { name: 'Delete' }).click()

    // 確認刪除
    await this.page.getByRole('button', { name: 'Confirm' }).click()

    // 等待刪除完成
    await this.page.waitForResponse(
      (response) => response.url().includes('/api/v1/workflows') &&
                    response.request().method() === 'DELETE'
    )
  }
}
```

### 工作流程編輯器頁面

```typescript
// tests/e2e/pages/WorkflowEditorPage.ts
import { Page, Locator } from '@playwright/test'

export class WorkflowEditorPage {
  readonly page: Page
  readonly canvas: Locator
  readonly nodePanel: Locator
  readonly saveButton: Locator
  readonly publishButton: Locator

  constructor(page: Page) {
    this.page = page
    this.canvas = page.getByTestId('workflow-canvas')
    this.nodePanel = page.getByTestId('node-panel')
    this.saveButton = page.getByRole('button', { name: 'Save' })
    this.publishButton = page.getByRole('button', { name: 'Publish' })
  }

  async addNode(nodeType: string, position: { x: number; y: number }) {
    // 從節點面板拖動節點到畫布
    const nodeElement = this.nodePanel.getByText(nodeType)
    await nodeElement.dragTo(this.canvas, {
      targetPosition: position
    })
  }

  async connectNodes(sourceNodeId: string, targetNodeId: string) {
    const sourceHandle = this.page.getByTestId(`node-${sourceNodeId}-output`)
    const targetHandle = this.page.getByTestId(`node-${targetNodeId}-input`)

    await sourceHandle.hover()
    await this.page.mouse.down()
    await targetHandle.hover()
    await this.page.mouse.up()
  }

  async save() {
    await this.saveButton.click()

    // 等待保存完成
    await this.page.waitForResponse(
      (response) => response.url().includes('/api/v1/workflows') &&
                    response.request().method() === 'PUT'
    )
  }

  async publish() {
    await this.publishButton.click()

    // 等待發布完成
    await this.page.getByText('Workflow published successfully').waitFor()
  }

  async getNodeCount(): Promise<number> {
    return await this.page.getByTestId('workflow-node').count()
  }
}
```

## 用戶場景測試

### 認證流程測試

```typescript
// tests/e2e/specs/auth.spec.ts
import { test, expect } from '@playwright/test'
import { LoginPage } from '../pages/LoginPage'

test.describe('Authentication', () => {
  let loginPage: LoginPage

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page)
    await loginPage.goto()
  })

  test('should login successfully with valid credentials', async ({ page }) => {
    // Arrange
    const email = 'test@example.com'
    const password = 'Test@123'

    // Act
    await loginPage.login(email, password)

    // Assert
    await expect(page).toHaveURL('/dashboard')
    await expect(page.getByText('Welcome back')).toBeVisible()
  })

  test('should show error with invalid credentials', async () => {
    // Arrange
    const email = 'wrong@example.com'
    const password = 'wrongpassword'

    // Act
    await loginPage.loginWithError(email, password)

    // Assert
    const errorMessage = await loginPage.getErrorMessage()
    expect(errorMessage).toContain('Invalid email or password')
  })

  test('should show validation errors for empty fields', async ({ page }) => {
    // Act - 點擊登入按鈕而不填寫
    await loginPage.loginButton.click()

    // Assert
    await expect(page.getByText('Email is required')).toBeVisible()
    await expect(page.getByText('Password is required')).toBeVisible()
  })

  test('should navigate to registration page', async ({ page }) => {
    // Act
    await page.getByRole('link', { name: 'Create an account' }).click()

    // Assert
    await expect(page).toHaveURL('/register')
  })
})
```

### 工作流程 CRUD 測試

```typescript
// tests/e2e/specs/workflow-crud.spec.ts
import { test, expect } from '@playwright/test'
import { LoginPage } from '../pages/LoginPage'
import { WorkflowListPage } from '../pages/WorkflowListPage'

test.describe('Workflow CRUD Operations', () => {
  let loginPage: LoginPage
  let workflowListPage: WorkflowListPage

  test.beforeEach(async ({ page }) => {
    // 登入
    loginPage = new LoginPage(page)
    await loginPage.goto()
    await loginPage.login('test@example.com', 'Test@123')

    // 導航到工作流程列表
    workflowListPage = new WorkflowListPage(page)
    await workflowListPage.goto()
  })

  test('should create new workflow', async ({ page }) => {
    // Arrange
    const workflowName = `Test Workflow ${Date.now()}`

    // Act
    await workflowListPage.createButton.click()
    await page.getByLabel('Workflow Name').fill(workflowName)
    await page.getByLabel('Description').fill('E2E Test Workflow')
    await page.getByRole('button', { name: 'Create' }).click()

    // Assert
    await expect(page).toHaveURL(/\/workflows\/\d+/)
    await expect(page.getByText(workflowName)).toBeVisible()
  })

  test('should search workflows', async () => {
    // Arrange
    const searchQuery = 'Test'

    // Act
    await workflowListPage.searchWorkflow(searchQuery)

    // Assert
    const count = await workflowListPage.getWorkflowCount()
    expect(count).toBeGreaterThan(0)
  })

  test('should open workflow editor', async ({ page }) => {
    // Act
    const firstWorkflow = workflowListPage.workflowCards.first()
    const workflowName = await firstWorkflow.getByTestId('workflow-name').textContent()
    await firstWorkflow.click()

    // Assert
    await expect(page).toHaveURL(/\/workflows\/\d+\/edit/)
    await expect(page.getByText(workflowName!)).toBeVisible()
  })

  test('should delete workflow', async ({ page }) => {
    // Arrange - 創建測試工作流程
    const workflowName = `Temp Workflow ${Date.now()}`
    await workflowListPage.createButton.click()
    await page.getByLabel('Workflow Name').fill(workflowName)
    await page.getByRole('button', { name: 'Create' }).click()
    await workflowListPage.goto()

    // Act
    await workflowListPage.deleteWorkflow(workflowName)

    // Assert
    await expect(page.getByText(workflowName)).not.toBeVisible()
  })
})
```

### 工作流程編輯器測試

```typescript
// tests/e2e/specs/workflow-editor.spec.ts
import { test, expect } from '@playwright/test'
import { LoginPage } from '../pages/LoginPage'
import { WorkflowEditorPage } from '../pages/WorkflowEditorPage'

test.describe('Workflow Editor', () => {
  let loginPage: LoginPage
  let editorPage: WorkflowEditorPage

  test.beforeEach(async ({ page }) => {
    // 登入並創建新工作流程
    loginPage = new LoginPage(page)
    await loginPage.goto()
    await loginPage.login('test@example.com', 'Test@123')

    await page.goto('/workflows/new')
    await page.getByLabel('Workflow Name').fill(`Editor Test ${Date.now()}`)
    await page.getByRole('button', { name: 'Create' }).click()

    editorPage = new WorkflowEditorPage(page)
  })

  test('should add node to canvas', async () => {
    // Act
    await editorPage.addNode('Start', { x: 100, y: 100 })
    await editorPage.addNode('AI Agent', { x: 300, y: 100 })

    // Assert
    const nodeCount = await editorPage.getNodeCount()
    expect(nodeCount).toBe(2)
  })

  test('should connect two nodes', async ({ page }) => {
    // Arrange
    await editorPage.addNode('Start', { x: 100, y: 100 })
    await editorPage.addNode('End', { x: 300, y: 100 })

    // Act
    await editorPage.connectNodes('node-1', 'node-2')

    // Assert
    await expect(page.getByTestId('edge-node-1-node-2')).toBeVisible()
  })

  test('should save workflow', async ({ page }) => {
    // Arrange
    await editorPage.addNode('Start', { x: 100, y: 100 })

    // Act
    await editorPage.save()

    // Assert
    await expect(page.getByText('Workflow saved')).toBeVisible()
  })

  test('should publish workflow', async ({ page }) => {
    // Arrange
    await editorPage.addNode('Start', { x: 100, y: 100 })
    await editorPage.addNode('End', { x: 300, y: 100 })
    await editorPage.save()

    // Act
    await editorPage.publish()

    // Assert
    await expect(page.getByText('Workflow published successfully')).toBeVisible()
    await expect(page.getByTestId('status-badge')).toHaveText('Published')
  })

  test('should undo/redo actions', async ({ page }) => {
    // Arrange
    await editorPage.addNode('Start', { x: 100, y: 100 })
    const initialCount = await editorPage.getNodeCount()

    // Act - Undo
    await page.keyboard.press('Control+Z')
    const afterUndoCount = await editorPage.getNodeCount()

    // Act - Redo
    await page.keyboard.press('Control+Y')
    const afterRedoCount = await editorPage.getNodeCount()

    // Assert
    expect(initialCount).toBe(1)
    expect(afterUndoCount).toBe(0)
    expect(afterRedoCount).toBe(1)
  })
})
```

## 視覺回歸測試

```typescript
// tests/e2e/specs/visual-regression.spec.ts
import { test, expect } from '@playwright/test'

test.describe('Visual Regression', () => {
  test('workflow list page should match snapshot', async ({ page }) => {
    await page.goto('/workflows')

    // 等待內容加載
    await page.waitForLoadState('networkidle')

    // 截圖比對
    await expect(page).toHaveScreenshot('workflow-list.png', {
      fullPage: true,
      maxDiffPixels: 100
    })
  })

  test('workflow editor should match snapshot', async ({ page }) => {
    await page.goto('/workflows/1/edit')
    await page.waitForLoadState('networkidle')

    await expect(page).toHaveScreenshot('workflow-editor.png', {
      fullPage: true,
      maxDiffPixels: 100
    })
  })
})
```

## 無障礙性測試

```typescript
// tests/e2e/specs/accessibility.spec.ts
import { test, expect } from '@playwright/test'
import AxeBuilder from '@axe-core/playwright'

test.describe('Accessibility', () => {
  test('login page should not have accessibility violations', async ({ page }) => {
    await page.goto('/login')

    const accessibilityScanResults = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
      .analyze()

    expect(accessibilityScanResults.violations).toEqual([])
  })

  test('workflow list page should be keyboard navigable', async ({ page }) => {
    await page.goto('/workflows')

    // Tab 鍵導航
    await page.keyboard.press('Tab')
    await expect(page.getByRole('button', { name: 'Create Workflow' })).toBeFocused()

    await page.keyboard.press('Tab')
    await expect(page.getByPlaceholder('Search workflows...')).toBeFocused()
  })

  test('all interactive elements should have aria-labels', async ({ page }) => {
    await page.goto('/workflows')

    const buttons = await page.getByRole('button').all()
    for (const button of buttons) {
      const ariaLabel = await button.getAttribute('aria-label')
      const text = await button.textContent()
      expect(ariaLabel || text).toBeTruthy()
    }
  })
})
```

## 檢查清單

### 測試覆蓋
- [ ] 所有關鍵用戶場景都有 E2E 測試
- [ ] 測試覆蓋主要業務流程
- [ ] 測試跨瀏覽器兼容性
- [ ] 測試移動端響應式設計
- [ ] 測試無障礙性合規

### 測試質量
- [ ] 使用 Page Object Model
- [ ] 測試獨立且可重複執行
- [ ] 使用有意義的等待策略
- [ ] 截圖和視頻記錄失敗
- [ ] 測試數據清理

### CI/CD 集成
- [ ] E2E 測試集成到 CI 流程
- [ ] 測試失敗時自動通知
- [ ] 測試報告自動生成
- [ ] 視覺回歸測試自動化
- [ ] 性能指標監控

## 參考資源

### 官方文檔
- [Playwright Documentation](https://playwright.dev/)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Axe Accessibility Testing](https://www.deque.com/axe/)

---

**文檔維護**: QA Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
