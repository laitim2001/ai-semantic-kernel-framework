# PROMPT-06: PROGRESS SAVE
# 進度保存與狀態同步

> **用途**: 保存開發進度、更新狀態文件、提交代碼
> **變數**: `{SPRINT_ID}` `{STORY_ID}`
> **預估時間**: 5-10 分鐘
> **版本**: v2.0.0

---

## 📋 執行目標

完成以下任務:
1. 更新 Sprint Status YAML 文件
2. 生成 Sprint Story 進度報告
3. 執行 Git 標準工作流程提交代碼
4. 生成 Session 工作摘要
5. (可選) 執行文檔一致性檢查

---

## 🔤 變數定義

```yaml
{SPRINT_ID}:
  描述: Sprint 標識符
  格式: "Sprint-{N}" 或 "sprint_{N}"
  範例: "Sprint-0", "sprint_0"

{STORY_ID}:
  描述: Story 標識符
  格式: "S{Sprint}-{Story}" 或 "{Sprint}-{Story}"
  範例: "S0-1", "S0-2"
```

---

## 🎯 執行步驟

### Step 1: 讀取當前狀態

```yaml
讀取文件:
  1. docs/03-implementation/sprint-status.yaml
  2. docs/bmm-workflow-status.yaml (可選)

提取信息:
  - 當前 Sprint 信息
  - Story 詳細信息 (標題、描述、點數、負責人)
  - Story 當前狀態
  - 已完成點數
```

### Step 2: 執行 Instruction 1 (更新專案狀態)

```yaml
任務: 更新 sprint-status.yaml

操作:
  1. 找到對應的 Story (使用 {STORY_ID})
  2. 更新以下字段:
     - status: "in-progress" → "completed" (或其他狀態)
     - completion_date: {TODAY}
  3. 更新 Sprint 統計:
     - updated: {TIMESTAMP}
     - completed_story_points: 計算新的總點數
  4. 保存文件

輸出格式:
---
✅ 狀態更新完成

Sprint: {SPRINT_ID}
Story: {STORY_ID} - {STORY_TITLE}
狀態變更: {OLD_STATUS} → {NEW_STATUS}
完成點數: {OLD_POINTS} → {NEW_POINTS}
總進度: {COMPLETED}/{TOTAL} ({PERCENTAGE}%)

更新時間: {TIMESTAMP}
---
```

### Step 3: 生成 Sprint Story 進度報告

```yaml
任務: 創建 Sprint Story 報告文件

文件路徑:
  claudedocs/sprint-reports/sprint-{N}-story-{ID}.md

報告內容:
  1. Story 基本信息 (ID, 標題, 點數, 負責人)
  2. 完成的功能清單
  3. 技術實現要點
  4. 測試覆蓋情況
  5. 遇到的問題和解決方案
  6. 修改的文件清單
  7. 下一步行動項

範本: 見下方 "Story 進度報告範本"
```

### Step 4: 執行 Instruction 3 (Git 工作流程)

```yaml
任務: Git 標準提交流程

步驟:
  1. 檢查 Git 狀態:
     → git status

  2. 查看未提交的更改:
     → git diff

  3. 添加文件:
     → git add .
     (或選擇性添加特定文件)

  4. 生成 Commit Message:
     格式: {TYPE}({SCOPE}): {DESCRIPTION}

     TYPE 選擇:
       - feat: 新功能實現
       - fix: Bug 修復
       - docs: 文檔更新
       - refactor: 代碼重構
       - test: 測試相關
       - chore: 構建/配置

     SCOPE: sprint-{N} 或 {component}

     DESCRIPTION 範例:
       - "complete S0-1 development environment setup"
       - "implement S1-3 agent CRUD API"
       - "update sprint status for S0-2"

  5. 提交:
     → git commit -m "{COMMIT_MESSAGE}"

  6. (可選) 推送:
     → git push origin {BRANCH}

輸出:
---
✅ Git 提交完成

Branch: {BRANCH_NAME}
Commit: {COMMIT_HASH}
Message: {COMMIT_MESSAGE}

Modified Files:
- {FILE_1}
- {FILE_2}
- ...

(可選) Pushed to: origin/{BRANCH}
---
```

### Step 5: 執行 Instruction 5 (生成 Session 摘要)

```yaml
任務: 生成工作 Session 摘要

文件路徑:
  claudedocs/session-logs/session-{DATE}.md

摘要內容:
  1. 工作時段信息
  2. 完成的工作清單
  3. 修改的文件清單
  4. 遇到的問題和解決方案
  5. Git 提交記錄
  6. 下次工作待辦事項
  7. 備註

範本: 見下方 "Session 摘要範本"
```

### Step 6 (可選): 執行 Instruction 6 (文檔一致性檢查)

```yaml
任務: 檢查關鍵文檔同步狀態

檢查項目:
  1. sprint-status.yaml 更新時間
  2. bmm-workflow-status.yaml 是否需要更新
  3. Sprint 計劃文檔是否需要標記完成
  4. README.md 是否需要更新進度

輸出:
---
📋 文檔一致性檢查

✅ sprint-status.yaml (已更新)
✅ bmm-workflow-status.yaml (正常)
⚠️ 需要更新:
  - sprint-0-mvp-revised.md 需標記 {STORY_ID} 完成
  - README.md 可更新最新進度

建議操作:
1. 在 Sprint 計劃文檔中標記 Story 完成
2. (可選) 更新 README.md 進度說明
---
```

---

## 📤 輸出格式

### Story 進度報告範本

```markdown
# Sprint Story 進度報告: {STORY_ID}

**生成時間**: {TIMESTAMP}
**生成者**: AI Assistant (PROMPT-06)

---

## 📊 基本信息

| 項目 | 內容 |
|------|------|
| **Story ID** | {STORY_ID} |
| **標題** | {STORY_TITLE} |
| **Sprint** | {SPRINT_ID} |
| **Story Points** | {STORY_POINTS} |
| **負責人** | {ASSIGNEE} |
| **優先級** | {PRIORITY} |
| **完成日期** | {COMPLETION_DATE} |
| **狀態** | {STATUS} |

---

## ✅ 完成的功能

1. {FEATURE_1}
   - {DETAIL_1_1}
   - {DETAIL_1_2}

2. {FEATURE_2}
   - {DETAIL_2_1}
   - {DETAIL_2_2}

3. {FEATURE_3}

---

## 🔧 技術實現要點

### 核心實現

**{COMPONENT_1}**:
- 技術選型: {TECH_STACK}
- 實現方式: {IMPLEMENTATION_APPROACH}
- 關鍵代碼: `{FILE_PATH}:{LINE_NUMBER}`

**{COMPONENT_2}**:
- ...

### 技術決策

1. **決策**: {DECISION_1}
   - 原因: {REASON}
   - 影響: {IMPACT}

2. **決策**: {DECISION_2}
   - ...

---

## 🧪 測試覆蓋

### 單元測試
- [x] {TEST_CASE_1}
- [x] {TEST_CASE_2}
- [ ] {TEST_CASE_3} (待完成)

### 集成測試
- [x] {INTEGRATION_TEST_1}
- [x] {INTEGRATION_TEST_2}

### E2E 測試
- [ ] {E2E_TEST_1} (下個 Sprint)

**測試覆蓋率**: {COVERAGE_PERCENTAGE}%

---

## ⚠️ 遇到的問題

### 問題 1: {PROBLEM_1_TITLE}

**描述**: {PROBLEM_DESCRIPTION}

**原因**: {ROOT_CAUSE}

**解決方案**: {SOLUTION}

**相關文件**: `{RELATED_FILES}`

---

### 問題 2: {PROBLEM_2_TITLE}

...

---

## 📝 修改的文件

### 新增文件
```
{NEW_FILE_1}
{NEW_FILE_2}
```

### 修改文件
```
{MODIFIED_FILE_1}
{MODIFIED_FILE_2}
```

### 刪除文件
```
{DELETED_FILE_1} (如果有)
```

---

## 📋 下一步行動

- [ ] {ACTION_ITEM_1}
- [ ] {ACTION_ITEM_2}
- [ ] {ACTION_ITEM_3}

---

## 💡 經驗教訓

**做得好的地方**:
- {LESSON_LEARNED_POSITIVE_1}
- {LESSON_LEARNED_POSITIVE_2}

**需要改進的地方**:
- {LESSON_LEARNED_IMPROVEMENT_1}
- {LESSON_LEARNED_IMPROVEMENT_2}

---

## 📚 相關文檔

- [Sprint {N} 計劃](../../docs/03-implementation/sprint-planning/sprint-{N}-*.md)
- [技術架構](../../docs/02-architecture/technical-architecture.md)
- [Sprint Status](../../docs/03-implementation/sprint-status.yaml)

---

**報告生成**: PROMPT-06
**下次更新**: 下個 Story 完成時
```

---

### Session 摘要範本

```markdown
# Work Session 摘要: {DATE}

**生成時間**: {TIMESTAMP}
**生成者**: AI Assistant (PROMPT-06)

---

## ⏱️ 工作時段

| 項目 | 時間 |
|------|------|
| **開始時間** | {START_TIME} |
| **結束時間** | {END_TIME} |
| **工作時長** | {DURATION} 小時 |
| **Sprint** | {SPRINT_ID} |

---

## ✅ 完成的工作

1. ✅ {TASK_1}
   - {SUBTASK_1_1}
   - {SUBTASK_1_2}

2. ✅ {TASK_2}
   - {SUBTASK_2_1}

3. ✅ {TASK_3}

---

## 📝 Story 進度更新

| Story ID | 標題 | 原狀態 | 新狀態 | 進度 |
|----------|------|--------|--------|------|
| {STORY_ID_1} | {TITLE_1} | {OLD_STATUS} | {NEW_STATUS} | {PROGRESS}% |
| {STORY_ID_2} | {TITLE_2} | {OLD_STATUS} | {NEW_STATUS} | {PROGRESS}% |

**Sprint 總進度**: {COMPLETED_POINTS}/{TOTAL_POINTS} ({PERCENTAGE}%)

---

## 📁 修改的文件

### 代碼文件
```
{CODE_FILE_1}
{CODE_FILE_2}
```

### 配置文件
```
{CONFIG_FILE_1}
{CONFIG_FILE_2}
```

### 文檔文件
```
docs/03-implementation/sprint-status.yaml
claudedocs/sprint-reports/sprint-{N}-story-{ID}.md
```

---

## 💾 Git 提交記錄

```
{COMMIT_HASH_1} - {COMMIT_MESSAGE_1}
{COMMIT_HASH_2} - {COMMIT_MESSAGE_2}
```

**Branch**: {BRANCH_NAME}
**Pushed**: {YES/NO}

---

## ⚠️ 遇到的問題

### 問題 1: {PROBLEM_TITLE}

**現象**: {SYMPTOM}
**原因**: {ROOT_CAUSE}
**解決**: {SOLUTION}
**耗時**: {TIME_SPENT}

---

## 🔄 下次工作待辦

優先級排序:

**P0 - 緊急**:
- [ ] {TODO_P0_1}
- [ ] {TODO_P0_2}

**P1 - 高**:
- [ ] {TODO_P1_1}
- [ ] {TODO_P1_2}

**P2 - 中**:
- [ ] {TODO_P2_1}

**P3 - 低**:
- [ ] {TODO_P3_1}

---

## 💭 備註

### 技術決策
- {TECH_DECISION_NOTE}

### 團隊溝通
- {TEAM_COMMUNICATION_NOTE}

### 其他
- {OTHER_NOTES}

---

## 📊 時間分配

| 活動 | 時間 (小時) | 百分比 |
|------|------------|--------|
| 編碼 | {CODING_TIME} | {CODING_PERCENTAGE}% |
| 測試 | {TESTING_TIME} | {TESTING_PERCENTAGE}% |
| 調試 | {DEBUGGING_TIME} | {DEBUGGING_PERCENTAGE}% |
| 文檔 | {DOCUMENTATION_TIME} | {DOCUMENTATION_PERCENTAGE}% |
| 會議 | {MEETING_TIME} | {MEETING_PERCENTAGE}% |
| 其他 | {OTHER_TIME} | {OTHER_PERCENTAGE}% |

---

**生成工具**: PROMPT-06
**下次 Session**: {NEXT_SESSION_DATE}
```

---

## 💡 使用範例

### 範例 1: 完成一個 Story

```bash
場景: Story S0-1 完成,需要保存進度

用戶輸入:
"@PROMPT-06-PROGRESS-SAVE.md Sprint-0 S0-1"

AI 執行步驟:
1. 讀取 sprint-status.yaml
2. 找到 Story S0-1: "Development Environment Setup"
3. 更新狀態: not-started → completed
4. 更新完成點數: 0 → 5
5. 生成 Story 報告: sprint-reports/sprint-0-story-s0-1.md
6. Git 提交: "feat(sprint-0): complete S0-1 development environment"
7. 生成 Session 摘要: session-logs/session-2025-11-20.md
8. 文檔一致性檢查

輸出:
---
✅ 進度保存完成

Sprint: Sprint-0
Story: S0-1 - Development Environment Setup
狀態: not-started → completed
完成點數: 0 → 5
總進度: 5/38 (13%)

📄 生成報告:
- Sprint 報告: claudedocs/sprint-reports/sprint-0-story-s0-1.md
- Session 摘要: claudedocs/session-logs/session-2025-11-20.md

💾 Git 提交:
- Commit: feat(sprint-0): complete S0-1 development environment
- Branch: feature/sprint-0-setup
- Pushed: Yes

📋 文檔檢查:
✅ sprint-status.yaml 已更新
✅ bmm-workflow-status.yaml 正常
⚠️ 建議更新 sprint-0-mvp-revised.md 標記 S0-1 完成

⏭️ 下一步建議:
- 開始 Story S0-2: Azure App Service Setup
- 或執行 @PROMPT-09-SESSION-END.md 結束工作
---
```

---

### 範例 2: Story 進行中保存

```bash
場景: Story S0-2 開發中,需要臨時保存進度

用戶輸入:
"@PROMPT-06-PROGRESS-SAVE.md Sprint-0 S0-2"

AI 注意:
- Story 狀態保持 "in-progress"
- 只更新 updated 時間戳
- 提交 Git (WIP commit)
- 生成簡化的 Session 摘要

輸出:
---
✅ 進度保存完成 (進行中)

Sprint: Sprint-0
Story: S0-2 - Azure App Service Setup
狀態: in-progress (保持不變)
完成度: 部分完成

💾 Git 提交:
- Commit: wip(sprint-0): S0-2 Azure App Service config in progress
- Branch: feature/sprint-0-app-service

📋 Session 摘要: session-logs/session-2025-11-20-wip.md

⏭️ 下次繼續:
- 完成 App Service 環境變數配置
- 測試 staging 環境部署
---
```

---

## 🔗 整合的 Instructions

此 Prompt 整合並執行以下 Instructions:

- **Instruction 1**: 更新專案狀態報告
- **Instruction 2**: 生成 Sprint 完成報告 (如果 Story 完成)
- **Instruction 3**: Git 標準工作流程
- **Instruction 5**: 生成 Session 摘要
- **Instruction 6**: 文檔一致性檢查 (可選)

---

## ⚠️ 注意事項

### Git 衝突處理

如果遇到 Git 衝突:
1. 暫停 Prompt 執行
2. 提示用戶解決衝突
3. 等待用戶確認後繼續

### Story 狀態判斷

```yaml
如果 Story 100% 完成:
  - status: "completed"
  - completion_date: {TODAY}
  - 生成完整 Story 報告

如果 Story 部分完成:
  - status: "in-progress"
  - 生成簡化的進度摘要
  - Git commit 使用 "wip" 前綴

如果 Story 遇到阻塞:
  - status: "blocked"
  - 記錄阻塞原因
  - 建議下一步行動
```

---

## 📚 相關文檔

- [AI Assistant Instructions](../AI-ASSISTANT-INSTRUCTIONS.md)
- [Prompts README](./README.md)
- [PROMPT-04: Sprint Development](./PROMPT-04-SPRINT-DEVELOPMENT.md)
- [PROMPT-09: Session End](./PROMPT-09-SESSION-END.md)
- [Sprint Status YAML](../../docs/03-implementation/sprint-status.yaml)

---

**版本**: v2.0.0
**更新日期**: 2025-11-20
**維護者**: AI Assistant Team
