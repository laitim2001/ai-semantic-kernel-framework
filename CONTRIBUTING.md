# Contributing to IPA Platform

感謝您對 IPA Platform 的貢獻！本文檔提供了開發規範、工作流程和最佳實踐指南。

## 📋 目錄

- [開發環境設置](#開發環境設置)
- [代碼規範](#代碼規範)
- [Git 工作流](#git-工作流)
- [提交規範](#提交規範)
- [Pull Request 流程](#pull-request-流程)
- [測試要求](#測試要求)
- [文檔規範](#文檔規範)

---

## 🚀 開發環境設置

### 前置要求

- **Docker Desktop** 20.10+
- **Python** 3.11+ (後端開發)
- **Node.js** 18+ (前端開發)
- **Git** 2.30+
- **Azure CLI** 2.50+ (部署相關)
- **VS Code** 或其他 IDE

### 本地環境啟動

1. **克隆倉庫**
```bash
git clone https://github.com/laitim2001/ai-semantic-kernel-framework-project.git
cd ai-semantic-kernel-framework-project
```

2. **配置環境變量**
```bash
cp .env.example .env
# 編輯 .env 文件，填入必要的 API keys 和連接字符串
```

3. **啟動開發環境**
```bash
docker-compose up -d
```

4. **驗證服務**
```bash
# 檢查所有容器狀態
docker-compose ps

# 檢查後端 API
curl http://localhost:8000/health

# 檢查數據庫連接
docker-compose exec postgres psql -U ipa_user -d ipa_platform -c "\l"
```

5. **停止環境**
```bash
docker-compose down
# 如需清除數據
docker-compose down -v
```

---

## 📝 代碼規範

### Python (後端)

#### 代碼風格
- 使用 **PEP 8** 作為基礎標準
- 使用 **Black** 進行代碼格式化
- 使用 **isort** 進行 import 排序
- 使用 **flake8** 進行 linting
- 使用 **mypy** 進行類型檢查

#### 配置文件
項目根目錄已包含 `pyproject.toml` 配置：

```toml
[tool.black]
line-length = 100
target-version = ['py311']

[tool.isort]
profile = "black"
line_length = 100

[tool.mypy]
python_version = "3.11"
strict = true
```

#### 執行檢查
```bash
# 格式化代碼
black backend/

# 排序 imports
isort backend/

# Linting
flake8 backend/

# 類型檢查
mypy backend/
```

#### 命名規範
- **類名**: PascalCase (例: `WorkflowService`)
- **函數/方法**: snake_case (例: `execute_workflow`)
- **常量**: UPPER_SNAKE_CASE (例: `MAX_RETRY_COUNT`)
- **私有成員**: 前綴下劃線 (例: `_internal_method`)

#### 文檔字符串
使用 Google 風格的 docstring：

```python
def execute_workflow(
    workflow_id: str,
    input_data: dict,
    timeout: int = 300
) -> WorkflowResult:
    """執行指定的工作流。

    Args:
        workflow_id: 工作流的唯一標識符
        input_data: 工作流輸入數據
        timeout: 執行超時時間（秒），默認 300 秒

    Returns:
        WorkflowResult: 包含執行結果和狀態的對象

    Raises:
        WorkflowNotFoundError: 當工作流不存在時
        TimeoutError: 當執行超時時

    Example:
        >>> result = execute_workflow("wf-123", {"param": "value"})
        >>> print(result.status)
        'completed'
    """
    pass
```

### TypeScript/JavaScript (前端)

#### 代碼風格
- 使用 **ESLint** + **Prettier**
- 使用 **TypeScript** strict 模式
- React 組件使用 **函數組件** + **Hooks**

#### 配置文件
```json
{
  "extends": [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react/recommended",
    "prettier"
  ]
}
```

#### 命名規範
- **組件名**: PascalCase (例: `WorkflowEditor`)
- **文件名**: kebab-case (例: `workflow-editor.tsx`)
- **函數/變量**: camelCase (例: `handleSubmit`)
- **常量**: UPPER_SNAKE_CASE (例: `API_BASE_URL`)

---

## 🌿 Git 工作流

我們使用 **Git Flow** 的簡化版本：

### 分支策略

```
main (生產環境)
  └── develop (開發環境)
       ├── feature/S1-workflow-engine (功能分支)
       ├── feature/S2-agent-system (功能分支)
       ├── bugfix/fix-auth-issue (修復分支)
       └── hotfix/critical-bug (緊急修復)
```

### 分支命名規範

- **功能分支**: `feature/{sprint}-{feature-name}`
  - 例: `feature/S1-workflow-engine`
- **修復分支**: `bugfix/{issue-number}-{description}`
  - 例: `bugfix/123-fix-login-error`
- **緊急修復**: `hotfix/{description}`
  - 例: `hotfix/security-patch`

### 分支操作流程

1. **創建功能分支**
```bash
git checkout develop
git pull origin develop
git checkout -b feature/S1-workflow-engine
```

2. **開發過程中定期同步**
```bash
git fetch origin develop
git merge origin/develop
```

3. **完成開發後推送**
```bash
git push origin feature/S1-workflow-engine
```

4. **合併回 develop**
- 通過 Pull Request 進行 Code Review
- 至少 1 人 approve
- 通過所有 CI 測試
- 解決所有衝突

---

## 💬 提交規範

我們使用 **Conventional Commits** 規範：

### 提交格式

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 類型

- `feat`: 新功能
- `fix`: Bug 修復
- `docs`: 文檔更新
- `style`: 代碼格式（不影響功能）
- `refactor`: 重構（既不是新功能也不是修復）
- `perf`: 性能優化
- `test`: 測試相關
- `chore`: 構建/工具配置

### 示例

```bash
# 功能開發
git commit -m "feat(workflow): add workflow execution engine"

# Bug 修復
git commit -m "fix(auth): resolve token expiration issue"

# 文檔更新
git commit -m "docs: update API documentation for workflow endpoints"

# 重構
git commit -m "refactor(agent): simplify agent selection logic"

# 多行提交
git commit -m "feat(workflow): add parallel task execution

- Implement task dependency graph
- Add parallel execution scheduler
- Update workflow schema to support parallel tasks

Closes #42"
```

### Commit Message 規則

- **Subject**: 不超過 50 字符，使用祈使句
- **Body**: 72 字符換行，說明改動原因和內容
- **Footer**: 關聯 Issue 或 Breaking Changes

---

## 🔍 Pull Request 流程

### 創建 PR 前檢查

- [ ] 代碼通過所有 lint 檢查
- [ ] 新增代碼有對應的單元測試
- [ ] 所有測試通過（單元測試 + 集成測試）
- [ ] 更新相關文檔
- [ ] 本地環境驗證功能正常

### PR 標題格式

使用與 commit 相同的格式：
```
feat(workflow): Add workflow execution engine
```

### PR 描述模板

```markdown
## 📝 變更說明
簡要描述此 PR 的目的和實現內容。

## 🎯 相關 Issue
Closes #42

## 🧪 測試方法
1. 啟動本地環境
2. 執行以下 API 請求...
3. 驗證返回結果...

## 📸 截圖（如適用）
附上相關截圖或演示。

## ✅ 檢查清單
- [x] 代碼符合規範
- [x] 添加/更新了單元測試
- [x] 更新了相關文檔
- [x] 本地環境測試通過
- [x] CI 測試通過
```

### Code Review 準則

#### 對於 Reviewer（審查者）
- 在 **24小時內** 完成 review
- 檢查：代碼質量、測試覆蓋、安全性、性能
- 提供 **建設性** 的反饋
- 使用標籤：
  - 🚨 **Critical**: 必須修改
  - 💡 **Suggestion**: 建議優化
  - ❓ **Question**: 需要澄清

#### 對於 Author（提交者）
- **及時回應** review 意見
- 對爭議點進行討論，達成共識
- 修改後 **重新請求 review**

### 合併要求

- ✅ 至少 **1 人 approve**（複雜功能需要 2 人）
- ✅ 所有 **CI 測試通過**
- ✅ 無未解決的 **衝突**
- ✅ 無未解決的 **review 意見**

---

## 🧪 測試要求

### 測試類型

1. **單元測試** (Unit Tests)
   - 測試單個函數/類的行為
   - 覆蓋率目標: **80%+**

2. **集成測試** (Integration Tests)
   - 測試多個組件協作
   - 測試數據庫交互
   - 測試外部 API 調用（使用 mock）

3. **端到端測試** (E2E Tests)
   - 測試完整用戶場景
   - 使用 Playwright 或 Cypress

### Python 測試 (pytest)

```bash
# 運行所有測試
pytest

# 運行特定測試文件
pytest tests/test_workflow_service.py

# 運行特定測試
pytest tests/test_workflow_service.py::test_execute_workflow

# 查看覆蓋率
pytest --cov=backend --cov-report=html

# 運行標記的測試
pytest -m "not slow"  # 跳過慢速測試
```

### 測試文件結構

```
backend/
├── src/
│   └── workflow/
│       ├── __init__.py
│       └── service.py
└── tests/
    ├── unit/
    │   └── workflow/
    │       └── test_service.py
    ├── integration/
    │   └── test_workflow_api.py
    └── conftest.py  # pytest fixtures
```

### 測試示例

```python
# tests/unit/workflow/test_service.py
import pytest
from workflow.service import WorkflowService

@pytest.fixture
def workflow_service():
    """創建測試用的 WorkflowService 實例"""
    return WorkflowService()

def test_execute_workflow_success(workflow_service):
    """測試成功執行工作流"""
    result = workflow_service.execute_workflow(
        workflow_id="test-wf",
        input_data={"param": "value"}
    )
    assert result.status == "completed"
    assert result.output is not None

def test_execute_workflow_not_found(workflow_service):
    """測試工作流不存在的情況"""
    with pytest.raises(WorkflowNotFoundError):
        workflow_service.execute_workflow(
            workflow_id="non-existent",
            input_data={}
        )
```

### 前端測試 (Jest + React Testing Library)

```bash
# 運行測試
npm test

# 查看覆蓋率
npm test -- --coverage
```

---

## 📚 文檔規範

### API 文檔

- 使用 **OpenAPI 3.0** (Swagger)
- FastAPI 自動生成文檔: `http://localhost:8000/docs`
- 每個 endpoint 必須包含：
  - 功能描述
  - 請求參數
  - 響應格式
  - 錯誤代碼
  - 示例

### README 更新

當添加新功能或修改配置時，及時更新：
- 項目 README.md
- 相關模塊的 README
- 環境變量說明

### 架構決策記錄 (ADR)

重大技術決策需要記錄在 `docs/architecture/decisions/` 目錄：

```markdown
# ADR-001: 選擇 FastAPI 作為後端框架

## 狀態
已接受

## 背景
需要選擇一個高性能的 Python Web 框架...

## 決策
選擇 FastAPI

## 後果
優點：...
缺點：...
```

---

## 🔒 安全規範

### 敏感信息處理

- ❌ **絕不提交**：
  - API keys
  - 密碼
  - 證書/私鑰
  - `.env` 文件

- ✅ **使用**：
  - 環境變量
  - Azure Key Vault（生產環境）
  - `.env.example` 模板

### 依賴管理

```bash
# 檢查安全漏洞
pip-audit  # Python
npm audit  # Node.js

# 更新依賴
pip-compile --upgrade
npm update
```

---

## 🎯 Sprint 開發流程

### Sprint 週期：2週

1. **Sprint Planning** (週一)
   - 確認 Sprint Goal
   - 分解 User Stories
   - 估算 Story Points

2. **Daily Standup** (每日 10:00)
   - 昨天完成了什麼
   - 今天計劃做什麼
   - 遇到什麼阻礙

3. **Sprint Review** (週五下午)
   - Demo 完成的功能
   - 收集反饋

4. **Sprint Retrospective** (週五下午)
   - 討論改進點
   - 更新工作流程

---

## 🆘 獲取幫助

- **技術問題**: 在 GitHub Issues 中提問
- **緊急問題**: 聯繫 DevOps 團隊
- **文檔問題**: 提交 PR 改進文檔

---

## 📜 許可證

本項目採用 MIT 許可證。詳見 [LICENSE](LICENSE) 文件。

---

**感謝您的貢獻！讓我們一起打造優秀的 IPA Platform！** 🚀
