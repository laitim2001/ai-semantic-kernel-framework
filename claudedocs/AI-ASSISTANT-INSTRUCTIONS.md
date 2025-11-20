# AI Assistant Instructions for IPA Platform
# 智能流程自動化平台 - AI 助手操作指令手冊

> **版本**: v2.0.0
> **專案**: Microsoft Agent Framework Platform (IPA)
> **更新日期**: 2025-11-20
> **適用 AI**: Claude Code, GitHub Copilot, 其他 AI 助手

---

## 📋 目錄

1. [核心指令清單](#核心指令清單)
2. [快速參考卡](#快速參考卡)
3. [環境變數設定](#環境變數設定)
4. [詳細指令說明](#詳細指令說明)
5. [使用範例](#使用範例)
6. [錯誤處理](#錯誤處理)

---

## 核心指令清單

### 專案管理指令

| 指令 ID | 指令名稱 | 用途 | 預估時間 |
|---------|----------|------|----------|
| **Instruction 1** | 更新專案狀態報告 | 更新 Sprint Status YAML | 3-5 分鐘 |
| **Instruction 2** | 生成 Sprint 完成報告 | 記錄 Sprint 完成情況 | 5-8 分鐘 |
| **Instruction 3** | Git 標準工作流程 | 提交代碼到 Git | 2-3 分鐘 |
| **Instruction 4** | 創建 Pull Request | 創建並推送 PR | 3-5 分鐘 |
| **Instruction 5** | 生成 Session 摘要 | 記錄工作 Session | 2-3 分鐘 |

### 質量保證指令

| 指令 ID | 指令名稱 | 用途 | 預估時間 |
|---------|----------|------|----------|
| **Instruction 6** | 文檔一致性檢查 | 檢查文檔同步狀態 | 3-5 分鐘 |
| **Instruction 7** | 完整 Sprint 結束流程 | Sprint 完成所有步驟 | 15-20 分鐘 |
| **Instruction 8** | 快速進度同步 | 快速提交小改動 | 1-2 分鐘 |

### 審查與分析指令

| 指令 ID | 指令名稱 | 用途 | 預估時間 |
|---------|----------|------|----------|
| **Instruction 9** | 架構審查 | 審查技術架構決策 | 10-15 分鐘 |
| **Instruction 10** | 代碼審查 | 審查代碼質量 | 5-10 分鐘 |

---

## 快速參考卡

### 使用場景決策樹

```
問：我該用哪個指令?

├─ 📝 日常快速提交 (小改動, <30分鐘工作)
│  └─ → 使用 Instruction 8 (快速進度同步)
│
├─ 🎯 完成一個 Sprint Story
│  └─ → 使用 Instruction 2 + Instruction 3
│
├─ ✅ Sprint 全部完成
│  └─ → 使用 Instruction 7 (完整結束流程)
│
├─ 🔍 檢查文檔是否同步
│  └─ → 使用 Instruction 6 (文檔一致性檢查)
│
├─ 🚀 準備發 PR
│  └─ → 使用 Instruction 4 (創建 Pull Request)
│
└─ 📊 每日工作結束
   └─ → 使用 Instruction 5 (生成 Session 摘要)
```

### 組合使用指南

```yaml
日常開發流程:
  1. 開始工作: @PROMPT-04 (Sprint Development)
  2. 完成 Story: Instruction 2 (生成完成報告)
  3. 提交代碼: Instruction 3 (Git 工作流程)
  4. 結束工作: Instruction 5 (Session 摘要)

Sprint 結束流程:
  1. 檢查文檔: Instruction 6 (一致性檢查)
  2. 完整結束: Instruction 7 (完整結束流程)
  3. 創建 PR: Instruction 4 (Pull Request)
```

---

## 環境變數設定

在執行指令前,AI 應自動讀取以下專案配置:

```yaml
# 專案基本信息
PROJECT_NAME: "IPA - Intelligent Process Automation Platform"
PROJECT_PATH: "C:\ai-semantic-kernel-framework-project"
DOCS_PATH: "docs/"
CLAUDEDOCS_PATH: "claudedocs/"

# 工作流程追蹤文件
WORKFLOW_STATUS_FILE: "docs/bmm-workflow-status.yaml"
SPRINT_STATUS_FILE: "docs/03-implementation/sprint-status.yaml"

# Sprint 配置
CURRENT_SPRINT: "Sprint 0"
SPRINT_DURATION_WEEKS: 2
TEAM_SIZE: 8
VELOCITY_TARGET: 40

# Git 配置
GIT_BRANCH_PREFIX: "feature/"
GIT_MAIN_BRANCH: "main"
GIT_REMOTE: "origin"
GITHUB_REPO: "https://github.com/laitim2001/ai-semantic-kernel-framework-project.git"

# 文檔標準
COMMIT_MESSAGE_FORMAT: "type(scope): description"
COMMIT_TYPES: ["feat", "fix", "docs", "refactor", "test", "chore"]
```

---

## 詳細指令說明

### Instruction 1: 更新專案狀態報告

**用途**: 更新 `sprint-status.yaml` 文件,記錄當前 Sprint 的進度

**執行步驟**:
1. 讀取 `docs/03-implementation/sprint-status.yaml`
2. 確認當前 Sprint ID (例如: sprint_0, sprint_1)
3. 更新以下字段:
   - `updated`: 當前日期時間
   - `completed_story_points`: 已完成的故事點
   - 更新 backlog 中每個 Story 的 `status`
4. 計算 Sprint 完成百分比
5. 保存文件

**參數**:
- `sprint_id`: Sprint 標識符 (例如: "sprint_0")
- `story_id`: Story 標識符 (例如: "S0-1")
- `new_status`: 新狀態 ("in-progress", "completed", "blocked")

**使用範例**:
```
用戶: "請使用 Instruction 1 更新狀態,Story S0-1 已完成"
AI: 執行指令,更新 sprint-status.yaml
```

**輸出格式**:
```yaml
✅ 狀態更新完成

Sprint: Sprint 0
Story: S0-1 - Development Environment Setup
狀態: not-started → completed
完成點數: 0 → 5
總進度: 0/38 → 5/38 (13%)

更新時間: 2025-11-20 14:30:00
```

---

### Instruction 2: 生成 Sprint 完成報告

**用途**: 當完成一個 Sprint Story 時,生成完成報告

**執行步驟**:
1. 讀取 `sprint-status.yaml` 確認 Story 詳情
2. 生成完成報告,包括:
   - Story 基本信息
   - 完成的功能清單
   - 技術實現要點
   - 測試覆蓋情況
   - 遇到的問題和解決方案
3. 將報告保存到 `claudedocs/sprint-reports/sprint-{N}-story-{ID}.md`
4. 更新 `sprint-status.yaml` (調用 Instruction 1)

**參數**:
- `story_id`: Story 標識符 (必需)

**使用範例**:
```
用戶: "請使用 Instruction 2 生成 S0-1 的完成報告"
```

**輸出模板**:
```markdown
# Sprint Story 完成報告: {Story ID}

## 基本信息
- **Story ID**: S0-1
- **標題**: Development Environment Setup
- **Story Points**: 5
- **負責人**: DevOps
- **完成日期**: 2025-11-20

## 完成的功能
1. Docker Compose 配置完成
2. 本地開發環境啟動腳本
3. ...

## 技術實現要點
- 使用 Docker Compose v2.x
- PostgreSQL 14 容器配置
- ...

## 測試覆蓋
- [x] 環境啟動測試
- [x] 數據庫連接測試
- ...

## 問題與解決
### 問題 1: Docker 網絡配置
**解決**: ...

## 下一步行動
- [ ] 團隊培訓 Docker 環境使用
```

---

### Instruction 3: Git 標準工作流程

**用途**: 標準化的 Git commit 流程

**執行步驟**:
1. 檢查 Git 狀態: `git status`
2. 查看未提交的更改: `git diff`
3. 添加文件: `git add .` 或指定文件
4. 生成 commit message (遵循 Conventional Commits)
5. 提交: `git commit -m "message"`
6. (可選) 推送: `git push origin <branch>`

**Commit Message 格式**:
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Type 類型**:
- `feat`: 新功能
- `fix`: Bug 修復
- `docs`: 文檔更新
- `refactor`: 代碼重構
- `test`: 測試相關
- `chore`: 構建/工具配置

**使用範例**:
```
用戶: "請使用 Instruction 3 提交代碼,Sprint 0 Story 1 完成"
AI: 生成 commit: "feat(sprint-0): complete S0-1 development environment setup"
```

---

### Instruction 4: 創建 Pull Request

**用途**: 創建並推送 Pull Request

**執行步驟**:
1. 確認當前分支
2. 確保所有更改已提交
3. 推送到遠端: `git push origin <branch>`
4. 生成 PR 標題和描述
5. 使用 GitHub CLI 或提示用戶手動創建 PR

**PR 標題格式**:
```
[Sprint {N}] {Story ID}: {簡短描述}
```

**PR 描述模板**:
```markdown
## Sprint 信息
- **Sprint**: Sprint 0
- **Story ID**: S0-1
- **Story Points**: 5

## 更改摘要
- 配置 Docker Compose 開發環境
- 添加 PostgreSQL 和 Redis 容器
- 創建啟動腳本

## 測試清單
- [x] 本地環境啟動測試
- [x] 數據庫連接測試
- [x] Redis 連接測試

## 相關文檔
- [Sprint 0 計劃](docs/03-implementation/sprint-planning/sprint-0-mvp-revised.md)
- [開發環境指南](docs/03-implementation/local-development-guide.md)

## Review 注意事項
- 確認 Docker Compose 版本
- 檢查環境變數配置
```

**使用範例**:
```
用戶: "請使用 Instruction 4 創建 PR,Story S0-1"
```

---

### Instruction 5: 生成 Session 摘要

**用途**: 記錄每個工作 Session 的內容

**執行步驟**:
1. 總結本次 Session 完成的工作
2. 記錄修改的文件清單
3. 記錄遇到的問題和解決方案
4. 列出下次工作的待辦事項
5. 保存到 `claudedocs/session-logs/session-{date}.md`

**使用範例**:
```
用戶: "請使用 Instruction 5 生成 Session 摘要"
```

**輸出模板**:
```markdown
# Work Session 摘要: 2025-11-20

## 工作時段
- **開始時間**: 14:00
- **結束時間**: 17:30
- **工作時長**: 3.5 小時

## 完成的工作
1. ✅ 完成 Sprint 0 Story 1 (開發環境配置)
2. ✅ 創建 Docker Compose 配置
3. ✅ 編寫本地開發指南

## 修改的文件
- `docker-compose.yml` (新增)
- `backend/Dockerfile` (新增)
- `docs/03-implementation/local-development-guide.md` (更新)
- `docs/03-implementation/sprint-status.yaml` (更新)

## 遇到的問題
### 問題 1: PostgreSQL 容器啟動慢
**原因**: 初始化腳本執行時間長
**解決**: 優化 init-db.sql,減少初始數據

## Git 提交記錄
- `feat(sprint-0): complete S0-1 development environment setup`
- `docs: update local development guide`

## 下次工作待辦
- [ ] 開始 Story S0-2: Azure App Service Setup
- [ ] 創建 Azure Service Principal
- [ ] 配置 GitHub Actions workflow

## 備註
- Docker Compose 版本要求 >= 2.0
- 團隊需要培訓 Docker 基礎知識
```

---

### Instruction 6: 文檔一致性檢查

**用途**: 檢查關鍵文檔是否保持同步

**執行步驟**:
1. 檢查以下文檔:
   - `bmm-workflow-status.yaml`
   - `sprint-status.yaml`
   - Sprint 計劃文檔
   - README.md
2. 驗證數據一致性:
   - Sprint 狀態是否匹配
   - Story 狀態是否同步
   - 完成點數是否正確
3. 生成檢查報告

**使用範例**:
```
用戶: "請使用 Instruction 6 檢查文檔一致性"
```

**輸出格式**:
```yaml
📋 文檔一致性檢查報告

✅ bmm-workflow-status.yaml
  - 更新時間: 2025-11-19
  - 當前階段: Phase 3 - Implementation
  - 狀態: 正常

✅ sprint-status.yaml
  - 更新時間: 2025-11-20
  - 當前 Sprint: Sprint 0
  - 完成度: 5/38 (13%)
  - 狀態: 正常

⚠️ 需要更新
  - README.md 未反映最新 Sprint 狀態
  - Sprint 0 計劃文檔需要更新完成情況

建議操作:
1. 更新 README.md 添加 Sprint 0 進度
2. 在 sprint-0-mvp-revised.md 標記 S0-1 完成
```

---

### Instruction 7: 完整 Sprint 結束流程

**用途**: Sprint 完成時執行所有必要步驟

**執行步驟**:
1. **文檔一致性檢查** (Instruction 6)
2. **生成 Sprint 完成報告**:
   - 總結所有完成的 Stories
   - 計算實際 vs 計劃點數
   - 記錄 Sprint 統計數據
3. **更新狀態文件**:
   - 更新 `sprint-status.yaml` Sprint 狀態為 "completed"
   - 更新 `bmm-workflow-status.yaml`
4. **Git 提交** (Instruction 3)
5. **創建 PR** (Instruction 4) (可選)
6. **生成 Session 摘要** (Instruction 5)

**使用範例**:
```
用戶: "Sprint 0 全部完成,請執行 Instruction 7"
```

**預估時間**: 15-20 分鐘

---

### Instruction 8: 快速進度同步

**用途**: 快速提交小改動,不需要完整流程

**執行步驟**:
1. 檢查 Git 狀態
2. 生成簡短的 commit message
3. 提交並推送
4. (可選) 更新 sprint-status.yaml 的 updated 時間

**使用範例**:
```
用戶: "修復了一個小 bug,請快速同步"
AI: 執行 Instruction 8
→ git add .
→ git commit -m "fix: resolve Docker network issue"
→ git push
```

**預估時間**: 1-2 分鐘

---

### Instruction 9: 架構審查

**用途**: 審查技術架構文檔和決策

**執行步驟**:
1. 讀取 `docs/02-architecture/technical-architecture.md`
2. 審查架構決策:
   - 技術選型合理性
   - 架構模式適用性
   - 可擴展性考慮
   - 安全性考慮
3. 對照 PRD 需求檢查覆蓋度
4. 生成審查報告

**使用範例**:
```
用戶: "請使用 Instruction 9 審查當前架構"
```

**輸出格式**:
```markdown
# 架構審查報告

## 審查範圍
- Technical Architecture v1.0
- 審查日期: 2025-11-20

## 架構優勢
✅ 使用 Azure App Service 簡化部署
✅ Agent Framework 原生支持多 Agent 協作
✅ 混合監控方案平衡成本和功能

## 潛在風險
⚠️ Redis 單點故障風險
⚠️ Service Bus 成本可能較高
⚠️ Agent Framework Preview 版本穩定性

## 改進建議
1. 考慮 Redis Cluster 提高可用性
2. 評估 Service Bus 替代方案
3. 準備 Agent Framework 升級計劃

## PRD 需求覆蓋度
- Orchestration: ✅ 100%
- Checkpointing: ✅ 100%
- Correlation: ✅ 100%
- Collaboration: ✅ 100%
```

---

### Instruction 10: 代碼審查

**用途**: 審查代碼質量和最佳實踐

**執行步驟**:
1. 讀取指定路徑的代碼文件
2. 審查代碼質量:
   - 代碼風格和規範
   - 錯誤處理
   - 安全性問題
   - 性能考慮
   - 測試覆蓋
3. 生成審查建議

**參數**:
- `file_path`: 要審查的文件或目錄路徑

**使用範例**:
```
用戶: "請使用 Instruction 10 審查 backend/src/ 目錄"
```

---

## 使用範例

### 範例 1: 日常開發流程

```
# 場景: 開發者完成了一個小功能

用戶: "我完成了 Docker Compose 配置,請幫我提交"

AI 執行:
1. Instruction 8 (快速進度同步)
   → git add docker-compose.yml
   → git commit -m "feat(docker): add Docker Compose configuration"
   → git push origin feature/sprint-0-setup

輸出:
✅ 快速同步完成
提交: feat(docker): add Docker Compose configuration
推送: feature/sprint-0-setup → origin
```

---

### 範例 2: Sprint Story 完成

```
# 場景: 完成一個完整的 Sprint Story

用戶: "Story S0-1 完成了,請執行完整流程"

AI 執行:
1. Instruction 2 (生成完成報告)
   → 創建 sprint-reports/sprint-0-story-s0-1.md

2. Instruction 1 (更新狀態)
   → 更新 sprint-status.yaml: S0-1 status → completed

3. Instruction 3 (Git 提交)
   → git commit -m "feat(sprint-0): complete S0-1 development environment"

4. Instruction 5 (Session 摘要)
   → 創建 session-logs/session-2025-11-20.md

輸出:
✅ Story S0-1 完成流程執行完畢
📄 生成報告: sprint-reports/sprint-0-story-s0-1.md
📊 更新狀態: Sprint 0 進度 5/38 (13%)
💾 Git 提交: feat(sprint-0): complete S0-1
📋 Session 摘要: session-logs/session-2025-11-20.md
```

---

### 範例 3: Sprint 全部完成

```
# 場景: Sprint 0 的所有 Stories 都完成了

用戶: "Sprint 0 全部完成,請執行完整結束流程"

AI 執行 Instruction 7:
1. Instruction 6 (文檔一致性檢查)
2. 生成 Sprint 完成報告
3. 更新 sprint-status.yaml: sprint_0 status → completed
4. Instruction 3 (Git 提交)
5. Instruction 4 (創建 PR)
6. Instruction 5 (Session 摘要)

預估時間: 15-20 分鐘

輸出:
✅ Sprint 0 完整結束流程完成
📊 完成統計: 38/38 points (100%)
📄 Sprint 報告: sprint-reports/sprint-0-final.md
🔄 PR 創建: [Sprint 0] Complete infrastructure foundation
📋 下一步: 準備 Sprint 1
```

---

## 錯誤處理

### 常見錯誤和解決方案

#### 錯誤 1: Git 衝突

**錯誤訊息**:
```
error: Your local changes to the following files would be overwritten by merge
```

**解決步驟**:
1. 檢查衝突文件: `git status`
2. 選擇處理方式:
   - Stash 本地更改: `git stash`
   - Commit 本地更改: `git add . && git commit`
3. 拉取遠端更新: `git pull`
4. 解決衝突後重新執行指令

---

#### 錯誤 2: Sprint Status 文件格式錯誤

**錯誤訊息**:
```
YAML parsing error: Invalid YAML format
```

**解決步驟**:
1. 使用 YAML 驗證器檢查語法
2. 檢查縮進是否正確 (使用空格,不用 Tab)
3. 檢查特殊字符是否需要引號
4. 恢復到上一個有效版本: `git checkout HEAD -- sprint-status.yaml`

---

#### 錯誤 3: 文檔路徑不存在

**錯誤訊息**:
```
FileNotFoundError: No such file or directory
```

**解決步驟**:
1. 檢查環境變數配置中的路徑
2. 確認當前工作目錄: `pwd` (Linux/Mac) 或 `cd` (Windows)
3. 使用絕對路徑重新執行

---

## 附錄

### A. Commit Message 範例

```bash
# 新功能
feat(sprint-0): add Docker Compose configuration
feat(backend): implement agent CRUD API
feat(frontend): create agent list component

# Bug 修復
fix(docker): resolve network connection issue
fix(api): handle null reference in agent service

# 文檔更新
docs(readme): update installation instructions
docs(sprint-0): add completion notes

# 重構
refactor(backend): extract database connection logic
refactor(frontend): improve component structure

# 測試
test(backend): add unit tests for agent service
test(e2e): add end-to-end workflow tests

# 構建/配置
chore(ci): update GitHub Actions workflow
chore(deps): upgrade FastAPI to 0.104.0
```

---

### B. 快捷鍵對照表

| 操作 | 快捷指令 |
|------|----------|
| 更新狀態 | `!ins1 <story_id> <status>` |
| 完成報告 | `!ins2 <story_id>` |
| Git 提交 | `!ins3 <message>` |
| 快速同步 | `!ins8` |
| 文檔檢查 | `!ins6` |

---

### C. 相關文檔鏈接

- [BMAD Workflow 文檔](../docs/bmm-workflow-status.yaml)
- [Sprint Status 追蹤](../docs/03-implementation/sprint-status.yaml)
- [Sprint 計劃文檔](../docs/03-implementation/sprint-planning/)
- [技術架構文檔](../docs/02-architecture/technical-architecture.md)
- [PRD 文檔](../docs/01-planning/prd/prd-main.md)

---

## 更新日誌

### v2.0.0 (2025-11-20)
- ✅ 初始版本發布
- ✅ 10 個核心指令完成
- ✅ 整合 BMAD 工作流程
- ✅ 適配 IPA 平台專案結構

---

**文檔維護者**: AI Assistant Team
**反饋渠道**: GitHub Issues
