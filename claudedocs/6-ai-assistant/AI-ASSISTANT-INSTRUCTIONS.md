# AI Assistant Instructions - 項目管理標準操作程序

本文件提供 AI 助手執行項目進度更新、文檔同步和 Git 操作的標準化指令。

---

## 📋 目錄

1. [Instructions 優先級矩陣](#instructions-優先級矩陣) ⭐ NEW
2. [進度報告更新指令](#進度報告更新指令)
3. [Git 提交和推送指令](#git-提交和推送指令)
4. [Session 摘要生成指令](#session-摘要生成指令)
5. [文檔同步檢查指令](#文檔同步檢查指令)
6. [完整工作流程指令](#完整工作流程指令)
7. [重要事件自動觸發規則](#重要事件自動觸發規則) ⭐ NEW
8. [Session 結束強制檢查清單](#session-結束強制檢查清單) ⭐ NEW

---

## 🎯 Instructions 優先級矩陣

本章節定義所有 Instructions 的優先級和執行規則,幫助 AI 判斷哪些操作是關鍵的,哪些可以延後。

### 優先級定義

| 優先級 | 符號 | 定義 | AI 行為 |
|-------|------|------|---------|
| **Critical** | 🔴 | 必須執行,不可跳過 | AI 主動檢測並自動執行,無需用戶確認 (除非用戶明確要求跳過) |
| **High Priority** | 🟡 | 強烈建議,可協商跳過 | AI 主動建議並詢問用戶,尊重用戶決定 |
| **Medium Priority** | 🟢 | 建議執行,可延後 | AI 可以提及,但不強制建議,完全由用戶決定 |

---

### Instructions 優先級表

| Instruction | 名稱 | 優先級 | 觸發條件 | 執行規則 |
|------------|------|--------|---------|---------|
| **Instruction 1** | 更新 PROJECT-STATUS-REPORT.md | 🔴 Critical | User Story 完成、集成測試完成、Phase 完成 | AI 主動執行 |
| **Instruction 2** | 生成 Feature 完成報告 | 🟢 Medium | User Story 完成、Feature 實作完成 | 用戶請求時執行 |
| **Instruction 3** | Git 工作流程 (提交和推送) | 🔴 Critical | 代碼變更完成、測試通過 | 用戶請求時執行,但必須嚴格驗證 |
| **Instruction 4** | 創建 Pull Request | 🟢 Medium | Feature 完成、準備 Code Review | 用戶請求時執行 |
| **Instruction 5** | 生成 Session 摘要 | 🟡 High | Session 時長 >1h、重要工作完成 | AI 建議執行,詢問用戶 |
| **Instruction 6** | 文檔一致性檢查 | 🟡 High | Instruction 1 執行後、Session 結束、PR 創建前 | AI 在強制時機自動執行 |
| **Instruction 7** | 完整結束流程 | 🟡 High | Feature 開發完整結束 | 用戶請求時執行 |
| **Instruction 8** | 快速同步 | 🟢 Medium | 緊急情況、時間緊迫 | 用戶明確請求時執行 |
| **Instruction 9** | 重要事件自動觸發規則 | 🔴 Critical | 檢測到 Critical/High Events | AI 自動監測和執行 |
| **Instruction 10** | Session 結束檢查清單 | 🔴 Critical | Session 結束信號檢測 | AI 自動執行檢查 |

---

### 執行規則詳解

#### Rule 1: Critical Priority Instructions (🔴)

**適用 Instructions**: 1, 3, 9, 10

**AI 行為**:
- 主動監測觸發條件
- 檢測到條件滿足時,立即告知用戶
- 直接執行操作 (無需用戶確認),除非用戶明確要求暫緩
- 記錄執行結果並驗證成功

**範例**:
```
🔴 檢測到 Critical Event: 集成測試完成 (25/26 passed)
執行 Instruction 1 更新狀態報告...
✅ PROJECT-STATUS-REPORT.md 已更新到 v6.0.0
執行 Instruction 6 驗證文檔一致性...
✅ 驗證通過,文檔同步正確
```

**用戶覆蓋**:
用戶可以說 "跳過文檔更新,先提交代碼",AI 會記錄並在下次 Session 提醒補充。

---

#### Rule 2: High Priority Instructions (🟡)

**適用 Instructions**: 5, 6 (特定時機), 7

**AI 行為**:
- 監測觸發條件或時機
- 檢測到時機時,告知用戶並說明理由
- 列出建議執行的操作
- 詢問用戶是否執行
- 根據用戶回應執行或記錄延後原因

**範例**:
```
🟡 建議執行 Session 摘要
檢測到: Session 時長 3 小時,完成重要工作 (US 1.2 集成測試)
建議操作: 執行 Instruction 5 創建 Session 摘要

是否現在執行? (y/n)
如果暫不執行,我會在 Session 結束檢查時再次提醒。
```

---

#### Rule 3: Medium Priority Instructions (🟢)

**適用 Instructions**: 2, 4, 8

**AI 行為**:
- 僅在用戶明確請求時執行
- 可以在適當時機簡單提及可選操作 (不強制)
- 完全尊重用戶決定
- 不主動監測或建議

**範例**:
```
✅ 變更已提交並推送到 GitHub。

(可選) 你可以執行 Instruction 4 創建 PR 進行 Code Review,
或稍後手動創建。
```

---

### 優先級覆蓋規則

用戶可以臨時調整優先級,AI 應該尊重並記錄:

#### 用戶降低優先級:
- **用戶**: "跳過文檔更新,先提交代碼"
- **AI 行為**:
  - 允許跳過 Instruction 1 (通常為 Critical)
  - 記錄跳過原因和時間
  - 在下次 Session 或 Session 結束時提醒補充

#### 用戶提升優先級:
- **用戶**: "完整檢查,不要遺漏任何步驟"
- **AI 行為**:
  - 將所有 Medium 提升為 High
  - 將所有 High 提升為 Critical
  - 執行所有可選的驗證和檢查

#### 用戶加速模式:
- **用戶**: "這是緊急修復,快速流程"
- **AI 行為**:
  - 使用 Instruction 8 (快速同步) 而非 Instruction 7
  - 僅執行 Critical 操作
  - 記錄跳過的 High/Medium 操作供後續補充

---

### 優先級決策流程

```
AI 檢測到事件/完成任務
    ↓
查詢 Instructions 優先級表
    ↓
判斷優先級
    ↓
    ├─ 🔴 Critical → 立即執行,告知用戶結果
    ├─ 🟡 High → 建議執行,詢問用戶
    └─ 🟢 Medium → 僅提及,不強制

用戶覆蓋請求?
    ├─ 是 → 記錄覆蓋,按用戶指示執行
    └─ 否 → 按優先級規則執行

執行完成 → 記錄結果 → 繼續下一個任務
```

---

### 優先級記錄與追蹤

AI 應該記錄以下資訊:
- 跳過的 Critical 操作 (記錄原因和補救計劃)
- 延後的 High 操作 (記錄在 Session 摘要或 Todo List)
- 用戶覆蓋的優先級決策 (記錄原因)

**記錄範例**:
```markdown
## Session X 優先級覆蓋記錄

1. **跳過 Instruction 1 (Critical)**
   - 時間: 2025-11-06 14:30 UTC
   - 原因: 用戶要求 "先提交代碼,稍後更新文檔"
   - 補救計劃: 在 Session 結束前補充執行

2. **延後 Instruction 5 (High)**
   - 時間: 2025-11-06 16:00 UTC
   - 原因: 用戶時間緊迫
   - 補救計劃: 下次 Session 開始時補充
```

---

## 🎯 進度報告更新指令

### Instruction 1: 更新 PROJECT-STATUS-REPORT.md

```
請更新項目狀態報告,包含以下內容:

1. 讀取當前的 claudedocs/5-status/PROJECT-STATUS-REPORT.md
2. 分析最近完成的工作 (檢查 git log 最近 5 筆提交)
3. 更新以下區塊:
   - 報告日期 (使用當前 UTC 時間)
   - Sprint 進度百分比
   - User Story 狀態 (🟢 已完成 / 🟡 進行中 / ⏳ 待開始)
   - 完成的功能清單
   - 測試覆蓋率統計
   - 已知問題和待辦事項

4. 保持以下格式:
   - 使用繁體中文
   - 保留所有 emoji 狀態標記
   - 維持 Markdown 表格格式
   - 更新檔案變更統計 (files changed, lines added/removed)

5. 在報告末尾添加更新記錄:
   ```
   ---
   **最後更新**: YYYY-MM-DD HH:MM UTC
   **更新者**: AI Assistant (Claude Code)
   **更新範圍**: [簡述更新內容]
   ```

範例使用:
- 當完成一個 Phase 或 User Story 時
- 當執行完整的功能實作後
- 每日工作結束時
```

### Instruction 2: 生成 Feature 完成報告

```
請為剛完成的功能生成詳細報告:

1. 分析以下資訊:
   - git diff --stat main..HEAD (檔案變更統計)
   - git log --oneline main..HEAD (提交歷史)
   - 執行測試結果 (dotnet test --verbosity normal)

2. 生成報告包含:
   ## 功能完成報告: [Feature Name]

   ### 📊 變更摘要
   - 分支: [branch-name]
   - 提交數: [count]
   - 檔案變更: [files changed]
   - 程式碼行數: +[additions] -[deletions]

   ### ✅ 完成項目
   #### Domain Layer
   - [列出新增/修改的實體和 ValueObjects]

   #### Application Layer
   - [列出新增/修改的 Commands, Queries, Handlers]

   #### Infrastructure Layer
   - [列出新增/修改的 Repositories, Configurations]

   #### Database
   - [列出 Migrations 和表格變更]

   ### 🧪 測試狀態
   - 單元測試: [passed/total]
   - 整合測試: [passed/total]
   - 覆蓋率: [percentage]% (如果有)

   ### ⚠️ 已知問題
   - [列出測試失敗或待改進項目]

   ### 📝 後續建議
   - [列出建議的下一步工作]

3. 將報告儲存到: claudedocs/2-sprints/sprint-[N]/[YYYY-MM-DD]-[feature-name]-report.md
```

---

## 🔄 Git 提交和推送指令

### Instruction 3: 執行標準 Git 工作流程

```
請執行完整的 Git 提交和推送流程:

**前置檢查**:
1. 執行 git status 確認當前分支和變更
2. 執行 git branch 確認不在 main/master 分支
3. 如果在 main/master,請先創建 feature 分支:
   - 格式: feature/us-[story-number]-[brief-description]
   - 範例: feature/us-1.3-phase2-4-advanced-features

**執行流程**:
1. 檢查未追蹤的臨時檔案,詢問是否需要刪除:
   - *.log, *.tmp, debug.*, test-*.ps1 等

2. 查看變更內容:
   ```bash
   git status
   git diff --stat
   git log -3 --oneline  # 參考最近的提交訊息風格
   ```

3. 執行測試驗證:
   ```bash
   dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --no-restore
   dotnet test "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --no-build --verbosity normal
   ```

   **如果測試失敗,執行測試分類流程** (詳見下方 "測試失敗處理流程")

4. 暫存變更:
   ```bash
   git add .
   ```

5. 生成提交訊息 (遵循以下格式):
   ```
   [type]: [User Story] - [簡短描述]

   [詳細說明段落,包含:]
   - 完成的功能
   - 變更的檔案類型
   - 測試狀態

   [影響範圍:]
   - Domain: [變更摘要]
   - Application: [變更摘要]
   - Infrastructure: [變更摘要]
   - Tests: [變更摘要]

   [測試結果:]
   - Build: ✅ Success
   - Tests: [passed/total]

   🤖 Generated with [Claude Code](https://claude.com/claude-code)

   Co-Authored-By: Claude <noreply@anthropic.com>
   ```

   Type 類型:
   - feat: 新功能
   - fix: 錯誤修復
   - refactor: 重構
   - test: 測試相關
   - docs: 文檔更新
   - chore: 雜項變更

6. 提交:
   ```bash
   git commit -m "$(cat <<'EOF'
   [commit message here]
   EOF
   )"
   ```

7. 推送到遠端:
   ```bash
   git push -u origin [branch-name]
   ```

8. 顯示推送結果和分支 URL

---

### 測試失敗處理流程 (更新)

當步驟 3 的測試執行失敗時,AI 必須按照以下流程進行分類和處理:

#### 步驟 1: 分析測試結果

```bash
# AI 檢查測試輸出,識別失敗的測試名稱和數量
# 範例輸出:
#   Failed: CreateAgent_WithValidData_ShouldSucceed
#   Failed: CreateConversation_WithValidData_ShouldReturnCreatedConversation
#   Passed: 95/97
```

#### 步驟 2: 測試分類

AI 必須將失敗測試分為以下三類:

##### 🔴 Critical Failures (核心功能失敗)

**定義**:
- 基本 CRUD 操作測試失敗 (Create, Read, Update, Delete)
- 新增功能的核心測試失敗
- 導致系統無法運行的測試失敗
- 迴歸測試失敗 (之前通過,本次失敗)

**範例**:
- CreateAgent_WithValidData_ShouldSucceed
- GetAgentById_WithValidId_ShouldReturnAgent
- 新增的 AgentExecutionService 核心功能測試失敗

**AI 處理**:
```
❌ Critical 測試失敗: CreateAgent_WithValidData_ShouldSucceed

這是核心功能測試,必須停止流程,不允許提交代碼。

建議操作:
1. 檢查測試失敗原因 (查看錯誤訊息)
2. 修復問題
3. 重新執行測試
4. 確認通過後再繼續

是否要查看詳細錯誤訊息? (y/n)
```

**決策**: STOP - 不允許繼續提交

---

##### 🟡 Known Issue Failures (已知問題)

**定義**:
- PROJECT-STATUS-REPORT.md 中已記錄的已知測試失敗
- 非本次 Session 引入的問題
- 不影響新功能的失敗測試
- 有明確記錄的待修復問題

**範例**:
- CreateConversation_WithValidData (Session 5 發現,已記錄,優先級: 中)

**AI 處理**:
```
⚠️ 測試失敗: CreateConversation_WithValidData_ShouldSucceed

檢查結果: 這是已知問題 ✅
- 發現時間: Session 5 (2025-11-06)
- 記錄位置: PROJECT-STATUS-REPORT.md
- 狀態: 待修復
- 優先級: 中
- 影響範圍: Conversation API 創建操作

本次變更: US 1.3 Phase 4 (AgentPlugin API)
影響分析: 此失敗不影響本次變更的功能

是否繼續提交? (建議: 是,但需在提交訊息中標注已知問題)
```

**決策**: CONTINUE WITH ANNOTATION - 可以繼續,但提交訊息必須包含:
```
測試結果:
- Build: ✅ Success
- Tests: 96/97 (99%)
- Known Issue: CreateConversation_WithValidData (US 1.2, 待修復)
```

---

##### 🟢 Edge Case Failures (邊界情況失敗)

**定義**:
- 邊界條件或異常情況測試失敗
- 不影響主要用戶流程的測試
- 可以後續改進的非關鍵測試
- 極端數據量或極端參數的測試

**範例**:
- GetAgent_WithExtremelyLongDescription (描述長度 >10000 字元)
- CreateAgent_With1000Plugins (極端數量測試)
- GetAgent_WithUnicodeEmojis (特殊字元處理)

**AI 處理**:
```
⚠️ Edge Case 測試失敗: GetAgent_WithExtremelyLongDescription

分析: 這是邊界情況測試,不影響正常使用場景。

測試詳情:
- 測試場景: Description 長度 >10000 字元
- 一般使用: Description 通常 <1000 字元
- 影響: 極少數極端情況

建議操作:
1. 記錄為已知改進項 (不是 Bug)
2. 可以繼續提交
3. 創建 Issue 或 TODO 追蹤改進

是否繼續提交? (建議: 是,記錄改進項)
```

**決策**: CONTINUE WITH NOTE - 可以繼續,記錄為改進項

---

#### 步驟 3: 執行決策

**決策矩陣**:

```yaml
Critical Failures:
  action: STOP
  behavior: 不允許提交,必須修復
  exception: 無 (Critical 必須修復)

Known Issue Failures:
  action: CONTINUE WITH ANNOTATION
  behavior: 詢問用戶,建議繼續但標注已知問題
  requirement: 提交訊息必須標注已知問題

Edge Case Failures:
  action: CONTINUE WITH NOTE
  behavior: 詢問用戶,建議繼續並記錄改進項
  requirement: 記錄到 PROJECT-STATUS-REPORT.md 的改進清單或創建 Issue
```

---

#### 步驟 4: 文檔更新規則

**重要**: 無論測試是否通過,AI 都必須更新 PROJECT-STATUS-REPORT.md

**測試通過時**:
- 記錄測試統計 (passed/total)
- 更新測試覆蓋率

**測試失敗時** (Known Issue 或 Edge Case):
- 記錄失敗測試名稱和詳情
- 記錄失敗原因和分類
- 添加到已知問題清單或改進清單
- 標注優先級

**範例**:
```markdown
### 已知問題

#### 🟡 US 1.2 已知問題 (1 個)
1. **CreateConversation_WithValidData_ShouldSucceed**
   - 狀態: 測試失敗
   - 發現時間: Session 5 (2025-11-06)
   - 優先級: 中
   - 影響: Conversation API 創建操作
   - 計劃: Sprint 2 修復

#### 🟢 改進清單 (2 個)
1. **GetAgent_WithExtremelyLongDescription**
   - 類型: 邊界情況改進
   - 優先級: 低
   - 計劃: Sprint 3 或後續 Sprint
```

---

#### 步驟 5: 提交訊息標注

**測試通過**:
```
測試結果:
- Build: ✅ Success
- Tests: 97/97 (100%) ✅
```

**Known Issue存在**:
```
測試結果:
- Build: ✅ Success
- Tests: 96/97 (99%)
- Known Issue: CreateConversation_WithValidData (US 1.2, 待修復)
```

**Edge Case失敗**:
```
測試結果:
- Build: ✅ Success
- Tests: 96/97 (99%)
- Edge Case: GetAgent_WithExtremelyLongDescription (記錄為改進項)
```

---

**錯誤處理**:
- 如果 push 失敗,檢查是否需要 pull --rebase
- 如果有衝突,報告衝突檔案並等待用戶處理
```

### Instruction 4: 創建 Pull Request

```
請為當前分支創建 Pull Request:

**前置條件**:
1. 確認分支已推送到遠端
2. 確認所有測試通過
3. 確認 PROJECT-STATUS-REPORT.md 已更新

**執行步驟**:
1. 分析分支變更:
   ```bash
   git log main..HEAD --oneline
   git diff main...HEAD --stat
   ```

2. 生成 PR 標題和內容:

   **標題格式**:
   ```
   [User Story ID] - [功能簡述]
   ```
   範例: `US 1.3 Phase 2-4 - Agent Advanced Features (Statistics, Versioning, Plugins)`

   **內容格式**:
   ```markdown
   ## 📋 Summary
   [3-5 句話描述此 PR 的目的和完成的功能]

   ## 🎯 User Story
   - **ID**: US [number]
   - **Phase**: Phase [number]
   - **Title**: [user story title]

   ## ✅ Changes
   ### Domain Layer
   - [列出新增/修改的實體]

   ### Application Layer
   - [列出新增/修改的 Commands/Queries/Handlers]

   ### Infrastructure Layer
   - [列出新增/修改的 Repositories/Configurations]

   ### Database
   - [列出 Migrations]

   ### Tests
   - [列出測試檔案和覆蓋率]

   ## 📊 Statistics
   - Files changed: [count]
   - Lines added: +[count]
   - Lines removed: -[count]
   - Commits: [count]

   ## 🧪 Test Results
   - Build: ✅ Success
   - Unit Tests: [passed]/[total]
   - Integration Tests: [passed]/[total]
   - Known Issues: [list or "None"]

   ## 📝 Review Checklist
   - [ ] Code follows project conventions
   - [ ] All tests passing
   - [ ] Documentation updated
   - [ ] Migration tested on local DB
   - [ ] No sensitive data in commits

   ## 🔗 Related
   - User Story: docs/user-stories/US-[number]-[name].md
   - Status Report: claudedocs/PROJECT-STATUS-REPORT.md

   🤖 Generated with [Claude Code](https://claude.com/claude-code)
   ```

3. 執行 gh pr create:
   ```bash
   gh pr create --title "[PR title]" --body "$(cat <<'EOF'
   [PR body content]
   EOF
   )"
   ```

4. 顯示 PR URL 給用戶

**選項參數**:
- 如果是草稿: 添加 --draft 標記
- 如果要指定 reviewer: 添加 --reviewer [username]
- 如果要自動合併: 添加 --auto-merge (僅在測試通過時)
```

---

## 📝 Session 摘要生成指令

### Instruction 5: 生成 Session 工作摘要

```
請為本次工作 Session 生成詳細摘要:

1. 分析 Session 內容:
   - 查看 git log 本次 session 的所有提交
   - 統計檔案變更和程式碼行數
   - 收集測試執行結果
   - 記錄遇到的錯誤和解決方案

2. 生成摘要文件: claudedocs/2-sprints/sprint-[n]/SPRINT-[N]-SESSION-[M]-SUMMARY.md

3. 摘要格式:
   ```markdown
   # Sprint [N] - Session [M] 工作摘要

   **日期**: YYYY-MM-DD
   **時長**: [estimated hours]
   **狀態**: ✅ 已完成 / 🟡 進行中

   ---

   ## 🎯 Session 目標
   [本次 Session 的主要目標]

   ## ✅ 完成項目

   ### 1. [主要任務 1]
   - 子任務 1
   - 子任務 2

   ### 2. [主要任務 2]
   - 子任務 1
   - 子任務 2

   ## 📊 變更統計
   - **提交數**: [count]
   - **檔案變更**: [count] files
   - **程式碼行數**: +[additions] -[deletions]
   - **測試數**: [count] tests

   ## 🏗️ 技術實作細節

   ### Domain Layer
   [列出新增/修改的實體、ValueObjects]

   ### Application Layer
   [列出新增/修改的 Commands、Queries、Handlers]

   ### Infrastructure Layer
   [列出新增/修改的 Repositories、Configurations]

   ### Database
   [列出 Migrations 和 schema 變更]

   ## 🧪 測試狀態
   - **建置**: ✅ Success
   - **單元測試**: [passed]/[total]
   - **整合測試**: [passed]/[total]
   - **失敗測試**: [list or "None"]

   ## ⚠️ 問題和解決方案

   ### 問題 1: [問題描述]
   - **錯誤**: [error message or description]
   - **根本原因**: [root cause analysis]
   - **解決方案**: [solution applied]
   - **影響**: [impact assessment]

   ### 問題 2: [問題描述]
   [重複相同格式]

   ## 📚 學習要點
   - [技術學習點 1]
   - [技術學習點 2]
   - [最佳實踐或模式]

   ## 🔄 Git 操作記錄
   - **分支**: [branch-name]
   - **提交**: [commit-hash-list]
   - **PR**: [PR URL if created]

   ## 📝 待辦事項
   - [ ] [遺留任務 1]
   - [ ] [遺留任務 2]
   - [ ] [下次 Session 建議]

   ## 💡 後續建議
   1. [建議 1]
   2. [建議 2]
   3. [建議 3]

   ---

   **生成時間**: YYYY-MM-DD HH:MM UTC
   **生成者**: AI Assistant (Claude Code)
   ```

4. 同時更新 claudedocs/5-status/PROJECT-STATUS-REPORT.md 中的 Session 記錄區塊
```

---

## 🔍 文檔同步檢查指令

### Instruction 6: 檢查文檔一致性 (強制驗證)

```
請執行文檔一致性檢查,確保所有文檔同步:

**執行時機** (更新):

#### 🔴 強制執行時機 (必須執行,不可跳過):
1. **執行 Instruction 1 之後**
   - 驗證 PROJECT-STATUS-REPORT.md 更新正確
   - 確認所有關鍵資訊已同步

2. **執行 Instruction 7 的 Phase 2 結束時**
   - 驗證所有文檔已更新
   - 確認無遺漏

3. **執行 Instruction 10 (Session 結束檢查) 時**
   - 最終驗證文檔一致性
   - 確保 Session 結束時文檔同步

4. **創建 PR 之前 (Instruction 4)**
   - 確保 PR 包含的變更已反映在文檔中
   - 避免 PR 中代碼與文檔不一致

#### 🟡 建議執行時機:
- Sprint 結束前 (總體檢查)
- 每日工作結束時 (可選)
- 重要里程碑完成後 (Phase 完成, US 完成)

---

**檢查項目**:

### 驗證 1: PROJECT-STATUS-REPORT.md 完整性 (Critical)

```yaml
檢查項目:
  - [ ] 報告日期 = 當前日期 (UTC)
  - [ ] 版本號正確遞增 (按規則: major.minor.patch)
  - [ ] 所有完成的 User Stories 狀態為 ✅
  - [ ] 所有進行中的 User Stories 狀態為 🟡
  - [ ] 測試統計完整 (unit + integration)
  - [ ] API 端點清單最新
  - [ ] Session 歷史包含本次 Session (如適用)
  - [ ] 已知問題清單最新

驗證方法:
  1. 讀取 5-status/PROJECT-STATUS-REPORT.md
  2. 讀取 User Stories 文檔 (docs/user-stories/US-*.md)
  3. 執行 git log 檢查最近提交 (可選)
  4. 比對一致性
```

### 驗證 2: User Story 文檔一致性

```yaml
檢查項目:
  - [ ] US 狀態與 PROJECT-STATUS-REPORT.md 一致
  - [ ] Acceptance Criteria 完成度與實際實作一致
  - [ ] 測試覆蓋記錄與實際測試一致

報告格式:
  - ✅ US 1.1: 狀態一致 (✅ 已完成)
  - ✅ US 1.2: 狀態一致 (✅ 已完成)
  - ⚠️ US 1.3 Phase 4: 缺少集成測試結果記錄
```

### 驗證 3: 測試文檔一致性

```yaml
檢查項目:
  - [ ] 測試統計與實際測試文件一致
  - [ ] 失敗測試已記錄在已知問題中
  - [ ] 測試覆蓋率記錄準確

執行方法 (可選,如時間允許):
  1. 統計 tests/ 目錄下的測試文件 (使用 find 或 ls)
  2. 比對 PROJECT-STATUS-REPORT.md 中的測試統計
  3. 報告不一致項目
```

### 驗證 4: Sprint 文檔 (可選)

```yaml
檢查項目:
  - claudedocs/1-planning/SPRINT-ROADMAP.md
  - claudedocs/2-sprints/sprint-[N]/SPRINT-[N]-OVERVIEW.md
  - claudedocs/2-sprints/sprint-[N]/SPRINT-[N]-PROGRESS.md
  - 確認文檔內容與實際完成進度一致
```

### 驗證 5: README 文檔 (可選)

```yaml
檢查項目:
  - 根目錄 README.md
  - docs/brief-README.md
  - 確認功能清單是否為最新
```

---

**輸出格式** (更新):

```markdown
## 📋 文檔一致性檢查報告

**檢查時間**: YYYY-MM-DD HH:MM UTC
**檢查範圍**: [列出檢查的文檔]
**觸發原因**: [執行 Instruction 1 後 / Session 結束檢查 / PR 創建前]

---

### ✅ 一致的文檔 (X/Y)
- ✅ PROJECT-STATUS-REPORT.md: 版本號、報告日期
- ✅ US 1.1: 狀態、測試覆蓋
- ✅ US 1.2: 狀態 (但缺少集成測試詳情)
- ✅ SPRINT-1-ROADMAP.md: Checklist 與進度一致
- ✅ README.md: 功能清單最新

---

### ⚠️ 需要更新的文檔 (X/Y)
- ⚠️ **PROJECT-STATUS-REPORT.md**:
  - 缺少 US 1.2 集成測試結果 (8 tests)
  - 缺少 US 1.3 Phase 4 集成測試結果 (8 tests)
  - 測試統計不完整 (只有 97 unit, 缺少 26 integration)
  - 缺少 Session 5 歷史記錄

- ⚠️ **US-1.2-conversation-crud.md**:
  - Acceptance Criteria 狀態未更新 (測試覆蓋完成)

---

### ❌ 缺失的文檔 (X/Y)
(如果有缺失的文檔)
- **[建議檔名]**: [應該包含的內容描述]

---

### 🔧 自動修復建議
AI 可以自動修復以下問題:
1. 更新 PROJECT-STATUS-REPORT.md (執行 Instruction 1)
2. 更新 US 1.2 文檔的測試狀態

是否執行自動修復? (y/n)

---

### 📝 手動修復建議
以下需要用戶手動確認或決定:
(如果有需要手動處理的項目)

---

### ✅ 驗證結論
- **一致性得分**: XX% (X/Y 完全一致)
- **Critical 問題**: X 個 (必須立即修復)
- **Warning 問題**: X 個 (建議修復,可延後)
- **建議操作**: [列出建議的下一步行動]
```

---

### 驗證失敗處理 (新增)

#### 如果驗證發現 Critical 問題:

**Critical 問題定義**:
- PROJECT-STATUS-REPORT.md 報告日期過期 >1 天
- 測試統計與實際結果差異 >10%
- 有 User Story 完成但狀態未更新
- 有重要功能實作但文檔缺失

**處理邏輯**:
1. AI 必須立即報告問題
2. AI 必須建議修復方案
3. 在用戶確認前,不允許執行後續操作 (如 git commit, PR creation)

**範例**:
```
⚠️ 驗證失敗: 發現 Critical 問題
- PROJECT-STATUS-REPORT.md 過期 2 天
- 測試統計差異 21% (97 unit 缺少 26 integration)

必須先修復這些問題,才能繼續提交或創建 PR。
是否現在執行 Instruction 1 修復? (強烈建議: 是)
```

---

#### 如果驗證發現 Warning 問題:

**Warning 問題定義**:
- 有小型文檔更新遺漏
- 有非關鍵統計數據差異 <10%
- 有可選文檔未更新 (如 README)

**處理邏輯**:
1. AI 報告問題
2. AI 建議修復,但不強制
3. 用戶可以選擇延後修復

**範例**:
```
⚠️ 驗證警告: 發現小問題
- US 1.2 文檔的測試狀態未更新

建議在下次 Session 修復。是否現在修復? (可選)
```
```

---

## 🚀 完整工作流程指令

### Instruction 7: 完整的功能開發結束流程

```
請執行完整的功能開發結束流程,包含測試、提交、推送、文檔更新:

**Phase 1: 驗證和測試**
1. 執行完整建置:
   ```bash
   dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --configuration Release
   ```

2. 執行所有測試:
   ```bash
   dotnet test "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --configuration Release --verbosity normal
   ```

3. 如果測試失敗:
   - 報告失敗的測試清單
   - 停止流程,等待用戶決定是否繼續

**Phase 2: 文檔更新**
1. 更新 PROJECT-STATUS-REPORT.md (使用 Instruction 1)
2. 生成 Feature 完成報告 (使用 Instruction 2)
3. 生成 Session 摘要 (使用 Instruction 5)
4. 執行文檔一致性檢查 (使用 Instruction 6)

**Phase 3: Git 操作**
1. 執行標準 Git 工作流程 (使用 Instruction 3)
2. 等待推送成功確認

**Phase 4: Pull Request**
1. 詢問用戶是否要創建 PR
2. 如果是,執行 Instruction 4
3. 顯示 PR URL

**Phase 5: 清理和總結**
1. 檢查是否有臨時檔案需要刪除
2. 顯示完整的工作總結:
   ```
   ✅ 功能開發完成流程已完成

   📊 統計:
   - 檔案變更: [count]
   - 程式碼行數: +[additions] -[deletions]
   - 測試: [passed]/[total]

   📝 文檔更新:
   - ✅ PROJECT-STATUS-REPORT.md
   - ✅ Feature Report
   - ✅ Session Summary

   🔄 Git:
   - ✅ 已提交: [commit-hash]
   - ✅ 已推送: [branch-name]
   - ✅ PR 已創建: [PR-URL] (如適用)

   🎯 下一步建議:
   - [建議 1]
   - [建議 2]
   ```

**使用時機**: 當完成一個完整的功能模組或 Phase 時使用
```

### Instruction 8: 快速進度同步 (日常使用)

```
請執行快速的進度同步和提交:

**簡化流程** (適用於小型變更或日常提交):

1. 快速檢查:
   ```bash
   git status
   dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --no-restore
   ```

2. 如果建置成功,執行提交:
   ```bash
   git add .
   git commit -m "[type]: [簡短描述]

[詳細說明 1-2 句]

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
   git push
   ```

3. 更新 PROJECT-STATUS-REPORT.md 的最後更新時間

4. 顯示簡短總結

**使用時機**:
- 小型錯誤修復
- 文檔更新
- 配置調整
- 不需要完整測試流程的變更
```

---

## 🎯 Instruction 9: 重要事件自動觸發規則

AI Assistant 必須主動監測以下重要事件,並在事件發生時自動觸發相應操作。

### 執行原則

**AI 主動性要求**:
- AI 必須在任務完成時主動檢測是否觸發重要事件
- 檢測到 Critical Event 時,AI 直接執行無需用戶確認
- 檢測到 High Priority Event 時,AI 建議並詢問用戶
- 檢測到 Medium Priority Event 時,AI 僅提及不強制

---

### 🔴 Critical Events (必須立即觸發)

#### Event 1: User Story 完成
**檢測條件**:
- 用戶明確表示 "US X.X 完成"
- 或 AI 檢測到某個 US 的所有 Acceptance Criteria 都已滿足
- 或 所有相關測試都已通過

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
   - 更新 User Story 狀態為 ✅ 已完成
   - 更新完成時間和統計數據
   - 記錄測試結果

2. 🟡 **Instruction 2**: 生成 Feature 完成報告
   - 創建詳細的功能完成報告
   - 記錄變更統計和技術細節

3. 🟡 **Instruction 5**: 生成 Session 摘要
   - 如果此 Session 主要在實作此 US

**AI 輸出範例**:
```
🎉 我注意到 User Story 1.2 已完成!這是重要里程碑。
我將執行以下操作:
1. ✅ 更新 PROJECT-STATUS-REPORT.md (必須)
2. ✅ 生成 Feature 完成報告 (建議)
3. ✅ 生成 Session 摘要 (如適用)
開始執行...
```

---

#### Event 2: 集成測試完成 ⭐ NEW
**檢測條件**:
- 創建了 ≥5 個集成測試
- 或 執行了集成測試並得到結果 (passed/failed)
- 或 用戶明確表示 "集成測試完成"

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
   - 更新集成測試統計 (total, passed, failed)
   - 添加測試覆蓋的 API 端點表格
   - 記錄測試通過率和已知問題
   - 更新版本號 (如果是重要里程碑)

2. 🟡 **Instruction 6**: 文檔一致性檢查
   - 驗證測試結果已正確記錄

**AI 輸出範例**:
```
📊 集成測試已完成 (25/26 通過, 96%)!
我將更新 PROJECT-STATUS-REPORT.md 記錄測試結果。
執行中...

✅ 狀態報告已更新到 v6.0.0
- 新增集成測試結果區塊
- 更新測試統計 (122 total tests)
- 更新版本號和報告日期
```

**重要性**: 🔴 Critical (此事件在 Session 5 中被遺漏,導致文檔不同步)

---

#### Event 3: Sprint 結束
**檢測條件**:
- 用戶明確表示 "Sprint X 結束"
- 或 Sprint 計劃中的所有 User Stories 都已完成
- 或 用戶請求 "Sprint Review" 或 "Sprint Retrospective"

**自動觸發操作**:
1. 🔴 **Instruction 7**: 完整結束流程
2. 🔴 **Instruction 1**: 最終狀態報告更新
3. 🟡 **Instruction 4**: 建議創建 PR (如有未合併分支)
4. 🟡 生成 Sprint Retrospective 報告

**AI 輸出範例**:
```
🏁 Sprint 1 已結束!
我將執行完整結束流程...
[執行 Instruction 7]
```

---

#### Event 4: Phase 完成
**檢測條件**:
- 用戶明確表示 "Phase X 完成"
- 或 Phase 的所有任務都已完成 (根據 Roadmap 或 Todo List)

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
2. 🟡 **Instruction 5**: 生成 Session 摘要

**AI 輸出範例**:
```
✅ Phase 2 已完成!
更新狀態報告...
```

---

### 🟡 High Priority Events (Session 結束前必須觸發)

#### Event 5: 多個測試文件創建
**檢測條件**:
- 創建了 ≥3 個測試文件
- 或 新增了 ≥10 個測試案例

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (測試統計)

**AI 輸出範例**:
```
📝 檢測到創建了 3 個測試文件 (新增 15 個測試)
建議更新狀態報告記錄測試成果。是否執行?
```

---

#### Event 6: 多個後端問題修復
**檢測條件**:
- 修復了 ≥3 個 Bug 或問題
- 或 用戶明確表示 "修復了多個問題"

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (問題修復記錄)

**AI 輸出範例**:
```
🔧 檢測到修復了 5 個後端問題
建議更新狀態報告記錄修復詳情。是否執行?
```

---

#### Event 7: API 端點新增/修改
**檢測條件**:
- 新增或修改了 ≥2 個 API Controller 端點
- 或 用戶明確表示 "API 端點完成"

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (API 端點清單)

**AI 輸出範例**:
```
🌐 檢測到新增了 4 個 API 端點
建議更新狀態報告的 API 端點表格。是否執行?
```

---

### 🟢 Medium Priority Events (可以下次更新)

#### Event 8: 單個文件修改
**處理**: 不觸發自動更新,可以累積到下次 Session

#### Event 9: 配置調整
**處理**: 使用 Instruction 8 (快速同步) 即可

---

### AI 行為規則

#### Rule 1: 主動檢測時機
AI 必須在以下時機主動檢測重要事件:
- 用戶任務完成時
- Session 即將結束時 (用戶說 "結束"、"先到這裡")
- 執行 git commit 之前
- 創建 PR 之前

#### Rule 2: 主動建議策略
**檢測到 Critical Event 時**:
1. 明確告知用戶檢測到的事件
2. 列出將要執行的操作
3. 立即開始執行 (不需要等待用戶確認)
4. 執行完畢後報告結果

**檢測到 High Priority Event 時**:
1. 告知用戶檢測到的事件
2. 建議執行的操作
3. 詢問用戶是否現在執行
4. 根據用戶回應執行或延後

**檢測到 Medium Priority Event 時**:
1. 簡單提及可選操作
2. 不強制建議
3. 完全尊重用戶決定

#### Rule 3: 強制執行 vs 建議執行
- 🔴 Critical Operations: 直接執行,無需詢問
- 🟡 High Priority Operations: 建議並詢問
- 🟢 Medium Priority Operations: 僅提及,不強制

---

### 執行範例

**範例 1: Session 5 理想執行流程** (如果有 Instruction 9):
```
User: "我完成了 US 1.2 和 US 1.3 Phase 4 的集成測試"

AI: 🎯 檢測到重要事件: 集成測試完成 (Event 2 - Critical)
    - 創建了 16 個集成測試
    - 測試通過率: 96% (25/26)

    我將執行以下操作:
    1. 🔴 更新 PROJECT-STATUS-REPORT.md (記錄測試結果)
    2. 🟡 執行文檔一致性檢查 (驗證更新正確)

    開始執行 Instruction 1 (更新狀態報告)...
    [執行更新]

    ✅ PROJECT-STATUS-REPORT.md 已更新到 v6.0.0
    - 新增 US 1.2 集成測試結果區塊
    - 新增 US 1.3 Phase 4 集成測試結果區塊
    - 更新測試統計 (122 total tests)
    - 更新版本號和報告日期

    開始執行 Instruction 6 (文檔一致性檢查)...
    [執行檢查]

    ✅ 文檔一致性檢查完成 - 所有關鍵資訊已同步
```

**範例 2: US 完成時**:
```
User: "US 1.4 完成了!"

AI: 🎉 檢測到重要事件: User Story 完成 (Event 1 - Critical)

    我將執行以下操作:
    1. ✅ 更新 PROJECT-STATUS-REPORT.md
    2. ✅ 生成 Feature 完成報告

    開始執行...
    [執行 Instruction 1 和 Instruction 2]

    ✅ 完成!
    - 狀態報告已更新
    - Feature 報告已生成: claudedocs/feature-reports/2025-11-06-us-1.4.md
```

---

## 🔍 Instruction 10: Session 結束強制檢查清單

### 觸發時機
當用戶表示 Session 即將結束時,AI 必須自動執行此檢查清單:
- 用戶說: "今天結束了"、"先到這裡"、"下次再繼續"、"暫停一下"
- 或 用戶詢問 "還有什麼要做的嗎?"
- 或 AI 檢測到用戶即將離開的信號

**重要**: 這是強制檢查,不是可選操作!

---

### 強制檢查項目

#### 1. 🔴 PROJECT-STATUS-REPORT.md 是否最新?
```yaml
AI 執行檢查:
  1. 讀取 PROJECT-STATUS-REPORT.md 的 "報告日期"
  2. 比較與當前日期 (UTC)
  3. 如果不是今天的日期 → 必須更新
  4. 檢查版本號是否正確遞增

檢查內容:
  - [ ] 報告日期 = 今天?
  - [ ] 版本號正確遞增?
  - [ ] 所有本次 Session 完成的工作都已記錄?
  - [ ] 測試統計已更新?
  - [ ] Session 歷史已添加?

如果有任何 [ ] 未勾選:
  → 執行 Instruction 1 (更新狀態報告)
```

---

#### 2. 🟡 是否有未提交的重要變更?
```yaml
AI 執行:
  git status

檢查:
  - 是否有 unstaged 或 staged 的變更?
  - 是否是重要文件 (src/, tests/, docs/)?

如果是:
  → 建議執行 Instruction 3 (Git 工作流程)
  → 詢問用戶: "檢測到未提交的變更,是否現在提交?"
```

---

#### 3. 🟡 是否需要創建 Session 摘要?
```yaml
檢查條件:
  - Session 持續時間 >1 小時?
  - 或 完成了重要功能/測試 (≥3 tasks)?
  - 或 修復了問題 (≥2 fixes)?

如果是:
  → 建議執行 Instruction 5 (生成 Session 摘要)
  → 詢問用戶: "建議創建 Session X 摘要記錄本次工作,是否執行?"
```

---

#### 4. 🟢 是否有待辦事項需要記錄?
```yaml
檢查:
  - 是否有未完成的任務?
  - 是否有計劃的後續工作?
  - 是否有發現的問題待解決?

如果是:
  → 記錄到 Todo List 或下次 Session 提醒事項
```

---

### AI 輸出格式

```
🔍 Session 結束檢查

執行時間: YYYY-MM-DD HH:MM UTC

📋 檢查結果:
1. 🔴 狀態報告檢查:
   - 報告日期: 2025-11-05 (過期 1 天) ❌
   - 版本號: v5.0.0 (需要遞增) ❌
   - 測試統計: 缺少集成測試結果 ❌

   → 必須執行 Instruction 1 更新狀態報告

2. 🟡 Git 變更檢查:
   - 未提交變更: 5 files ⚠️
   - 重要文件: tests/IntegrationTests (2 files)

   → 建議執行 Instruction 3 提交變更

3. 🟡 Session 摘要檢查:
   - Session 時長: ~3 小時 ⚠️
   - 重要工作: 創建 16 個集成測試,修復 9 個問題

   → 建議創建 Session 5 摘要

4. 🟢 待辦事項:
   - [ ] 修復 CreateConversation 測試失敗
   - [ ] 統一 API 路由版本控制

---

🎯 建議操作:
1. 🔴 立即更新 PROJECT-STATUS-REPORT.md (必須)
2. 🟡 提交未提交的變更 (建議)
3. 🟡 創建 Session 5 摘要 (建議)

是否執行以上操作? (建議: 全部執行)
```

---

### 自動執行規則

#### Rule 1: 強制執行 (無需詢問)
如果檢測到以下情況,AI 直接執行,無需用戶確認:
- PROJECT-STATUS-REPORT.md 報告日期過期 ≥2 天
- 有 Critical Event 未記錄 (如 US 完成、集成測試完成)
- 有未提交的 .md 文檔變更 (文檔應該立即提交)

**AI 輸出範例**:
```
⚠️ 檢測到 Critical 問題: 狀態報告過期 2 天
我將立即更新 PROJECT-STATUS-REPORT.md...
[執行 Instruction 1]
✅ 完成
```

---

#### Rule 2: 建議執行 (詢問用戶)
如果檢測到以下情況,AI 建議並詢問:
- 有未提交的代碼變更
- Session 時長 >1 小時且完成重要工作
- 有多個待辦事項需要記錄

**AI 輸出範例**:
```
📊 建議執行以下操作:
1. 提交代碼變更 (5 files)
2. 創建 Session 摘要 (Session 時長 3h)

是否現在執行? (y/n)
```

---

#### Rule 3: 僅提及 (不強制)
如果檢測到以下情況,AI 僅提及:
- 有小型文檔更新可延後
- 有非關鍵的配置調整

**AI 輸出範例**:
```
💡 可選操作:
- 有 1 個小型配置調整可以下次提交

Session 結束檢查完成 ✅
```

---

### 執行範例

**範例: Session 5 結束時** (理想流程):
```
User: "今天先到這裡"

AI: 🔍 執行 Session 結束檢查...

    檢查結果:
    1. ❌ 狀態報告過期 (2025-11-05 vs 2025-11-06)
    2. ❌ 集成測試結果未記錄 (16 tests, 25/26 passed)
    3. ⚠️ 有未提交變更 (測試文件)

    我將立即更新 PROJECT-STATUS-REPORT.md (必須操作)...

    [執行 Instruction 1]

    ✅ 狀態報告已更新到 v6.0.0
    - 新增集成測試結果區塊
    - 更新測試統計
    - 更新版本號和日期

    另外,建議:
    - 提交測試文件變更
    - 創建 Session 5 摘要

    是否現在執行? (y/n)

User: "y"

AI: [執行 Instruction 3 和 Instruction 5]

    ✅ 全部完成!
    - 變更已提交
    - Session 5 摘要已創建

    下次 Session 建議:
    1. 開始 Sprint 2 (US 1.4 - Agent 執行引擎)
    2. 修復 CreateConversation 測試失敗

    工作愉快! 👋
```

---

### 特殊情況處理

#### 情況 1: 用戶拒絕執行建議操作
```
User: "n, 下次再做"

AI: 好的,我會記錄待辦事項:
    - [ ] 更新 PROJECT-STATUS-REPORT.md (v5.0.0 → v6.0.0)
    - [ ] 提交測試文件變更
    - [ ] 創建 Session 5 摘要

    ⚠️ 重要提醒: 狀態報告已過期,下次 Session 開始時我會優先提醒更新。

    Session 結束 ✅
```

---

#### 情況 2: 所有檢查都通過
```
User: "今天結束了"

AI: 🔍 執行 Session 結束檢查...

    ✅ 所有檢查通過:
    - ✅ 狀態報告最新 (v6.0.0, 2025-11-06)
    - ✅ 無未提交變更
    - ✅ Session 摘要已創建
    - ✅ 無待辦事項

    Session 結束 ✅ 文檔完全同步!

    下次 Session 建議:
    - 開始 Sprint 2 實施

    工作愉快! 👋
```

---

#### 情況 3: 檢測到 Critical 問題但用戶堅持不修復
```
User: "先不管,直接結束"

AI: ⚠️ 警告: 狀態報告過期 2 天,包含未記錄的重要工作 (集成測試結果)

    這會導致:
    - 項目狀態不透明
    - 團隊溝通問題
    - Sprint 進度追蹤困難

    強烈建議現在更新 (只需 2-3 分鐘)。

    確定要跳過? (yes/no)

User: "yes"

AI: 好的,我會記錄 Critical 待辦事項:
    - 🔴 [Critical] 更新 PROJECT-STATUS-REPORT.md (過期 2 天)

    ⚠️ 下次 Session 我會在開始時立即提醒並執行更新。

    Session 結束
```

---

## 🎨 使用範例

### 範例 1: 完成 Phase 2-4 功能後

**用戶輸入**:
```
我剛完成 US 1.3 Phase 2-4 的實作,請執行完整的結束流程
```

**AI 執行** (使用 Instruction 7):
1. ✅ 執行測試驗證
2. ✅ 更新 PROJECT-STATUS-REPORT.md
3. ✅ 生成 feature-reports/2025-11-05-us-1.3-phase2-4.md
4. ✅ 生成 sessions/SPRINT-1-SESSION-3-SUMMARY.md
5. ✅ 執行文檔一致性檢查
6. ✅ Git commit 和 push
7. ✅ 創建 Pull Request
8. ✅ 顯示完整總結

---

### 範例 2: 修復小錯誤後

**用戶輸入**:
```
我修復了一個 typo,請快速同步
```

**AI 執行** (使用 Instruction 8):
1. ✅ 快速建置檢查
2. ✅ 提交變更
3. ✅ 推送到遠端
4. ✅ 更新狀態報告時間戳

---

### 範例 3: 每日工作結束前

**用戶輸入**:
```
今天工作結束了,請幫我同步進度和文檔
```

**AI 執行** (組合多個 Instructions):
1. ✅ 執行 Instruction 6 (文檔一致性檢查)
2. ✅ 執行 Instruction 1 (更新狀態報告)
3. ✅ 執行 Instruction 5 (生成 Session 摘要)
4. ✅ 執行 Instruction 3 (Git 工作流程)
5. ✅ 詢問是否需要創建 PR

---

## 📌 快速參考卡

| 任務 | 使用指令 | 預估時間 |
|------|---------|---------|
| 更新狀態報告 | Instruction 1 | 2-3 分鐘 |
| 生成功能完成報告 | Instruction 2 | 3-5 分鐘 |
| Git 提交推送 | Instruction 3 | 5-10 分鐘 |
| 創建 Pull Request | Instruction 4 | 3-5 分鐘 |
| 生成 Session 摘要 | Instruction 5 | 5-8 分鐘 |
| 文檔一致性檢查 | Instruction 6 | 3-5 分鐘 |
| 完整結束流程 | Instruction 7 | 15-20 分鐘 |
| 快速同步 | Instruction 8 | 1-2 分鐘 |

---

## 🔧 自定義參數

某些 Instructions 支援自定義參數:

**Instruction 3 (Git 工作流程)**:
```
請執行標準 Git 工作流程,使用以下參數:
- 跳過測試: yes (緊急修復時)
- 提交類型: hotfix
- 訊息前綴: [HOTFIX]
```

**Instruction 7 (完整流程)**:
```
請執行完整的功能開發結束流程,使用以下參數:
- 跳過 PR 創建: yes (稍後手動創建)
- 測試失敗時繼續: no (必須測試通過)
- 生成詳細報告: yes
```

---

## ⚙️ 環境變數和設定

AI 助手應該記住的項目設定:

```yaml
project:
  name: "AI Agent Platform"
  root: "C:\\AI Semantic Kernel"
  solution: "src\\AIAgentPlatform.sln"

git:
  main_branch: "master"
  feature_prefix: "feature/us-"
  remote: "origin"

documentation:
  status_report: "claudedocs/5-status/PROJECT-STATUS-REPORT.md"
  session_dir: "claudedocs/2-sprints/sprint-[N]/"
  feature_report_dir: "claudedocs/2-sprints/sprint-[N]/"
  user_stories_dir: "docs/user-stories/"
  planning_dir: "claudedocs/1-planning/"
  progress_dir: "claudedocs/3-progress/"
  changes_dir: "claudedocs/4-changes/"
  archive_dir: "claudedocs/7-archive/"

testing:
  build_config: "Release"
  test_verbosity: "normal"
  minimum_coverage: 80  # 目標覆蓋率

commits:
  types: ["feat", "fix", "refactor", "test", "docs", "chore"]
  require_tests: true
  require_build: true
```

---

## 📖 相關文檔

- **文檔中心**: claudedocs/README.md
- **開發日程**: claudedocs/1-planning/MASTER-DEVELOPMENT-SCHEDULE.md
- **Sprint 規劃**: claudedocs/2-sprints/sprint-[N]/SPRINT-[N]-OVERVIEW.md
- **進度追蹤**: claudedocs/3-progress/USER-STORY-STATUS.md
- **變更記錄**: claudedocs/4-changes/CHANGE-LOG.md
- **狀態報告**: claudedocs/5-status/PROJECT-STATUS-REPORT.md
- **User Stories**: docs/user-stories/US-*.md

---

**文檔版本**: 2.3.0
**創建日期**: 2025-11-05
**最後更新**: 2025-12-08
**維護者**: AI Assistant (Claude Code)
**更新頻率**: 根據項目需求更新

---

## 📂 文檔結構說明

本項目採用新的 7 文件夾文檔結構 (v2.0.0, 2025-12-08):

### 文檔架構
```
claudedocs/
├── 1-planning/          # 總體規劃 (MASTER-DEVELOPMENT-SCHEDULE, SPRINT-ROADMAP)
├── 2-sprints/           # Sprint 詳細文檔 (OVERVIEW, PROGRESS, RETROSPECTIVE)
│   ├── sprint-1/
│   ├── sprint-2/
│   └── ...
├── 3-progress/          # 進度追蹤 (USER-STORY-STATUS, FEATURE-COMPLETION-MATRIX)
├── 4-changes/           # 變更記錄 (CHANGE-LOG, SCOPE-CHANGE-REQUESTS)
├── 5-status/            # 狀態報告 (PROJECT-STATUS-REPORT)
├── 6-ai-assistant/      # AI 助手指引 (本文件)
└── 7-archive/           # 歷史歸檔 (已完成的 Sprint 文檔, 決策記錄)
```

### 雙層架構
- **/docs**: Reference Layer (靜態規劃參考 - BMAD Method, 架構設計, 原始需求)
- **/claudedocs**: Execution Layer (動態執行追蹤 - 實際進度, 變更記錄, 狀態報告)

### AI 維護規則
1. **狀態報告**: 始終更新 `5-status/PROJECT-STATUS-REPORT.md`
2. **Sprint 文檔**: Session 摘要和進度記錄存放在 `2-sprints/sprint-[N]/`
3. **進度追蹤**: User Story 完成時更新 `3-progress/USER-STORY-STATUS.md`
4. **變更記錄**: 重要變更記錄到 `4-changes/CHANGE-LOG.md`
5. **歸檔原則**: 完成的 Sprint 文檔、決策記錄移至 `7-archive/`

詳細說明請參考: `claudedocs/README.md`

---

## 📝 版本歷史

### v2.3.0 (2025-12-08) - 文檔結構重組
**更新類型**: 結構性更新 (配合 claudedocs 7-folder 架構)

**更新內容**:
- ✅ 更新所有文檔路徑引用:
  - `claudedocs/PROJECT-STATUS-REPORT.md` → `claudedocs/5-status/PROJECT-STATUS-REPORT.md`
  - `claudedocs/sessions/` → `claudedocs/2-sprints/sprint-[N]/`
  - `claudedocs/feature-reports/` → `claudedocs/2-sprints/sprint-[N]/`
- ✅ 新增 7 文件夾結構說明區塊
- ✅ 更新環境變數設定區塊 (新增 planning_dir, progress_dir, changes_dir, archive_dir)
- ✅ 更新相關文檔連結區塊 (指向新結構)

**影響範圍**:
- Instruction 1: PROJECT-STATUS-REPORT.md 路徑更新
- Instruction 2: Feature 報告存放位置更新
- Instruction 5: Session 摘要存放位置更新
- Instruction 6: 文檔驗證路徑更新
- 環境變數設定: 新增 4 個文檔目錄路徑

**預期效果**:
- AI 能正確操作新的 7-folder 文檔結構
- 所有文檔路徑引用準確無誤
- 維護新結構的清晰度和一致性

---

### v2.2.0 (2025-11-06) - 完善測試失敗處理
**更新類型**: Phase 3 完善 (Sprint 2 Week 2 Day 17)

**新增內容**:
- ✅ **Instruction 3 測試失敗處理流程** (~200 lines)
  - 5 個步驟的完整測試失敗處理流程
  - 3 個測試分類 (Critical 🔴, Known Issue 🟡, Edge Case 🟢)
  - 決策矩陣 (STOP, CONTINUE WITH ANNOTATION, CONTINUE WITH NOTE)
  - 文檔更新規則 (測試失敗時也要記錄)
  - 提交訊息標注規範 (3 種場景)

**預期效果**:
- AI 在測試失敗時做出更智能的決策
- 避免過度阻塞 (已知問題不應阻止提交)
- 避免過度寬鬆 (Critical 失敗必須修復)
- 確保所有測試失敗都有文檔記錄

---

### v2.1.0 (2025-11-06) - 增強驗證與優先級系統
**更新類型**: Phase 2 增強 (Sprint 2 Week 1 Day 14)

**新增內容**:
- ✅ **Instructions 優先級矩陣** (~180 lines)
  - 定義 3 個優先級 (Critical 🔴, High 🟡, Medium 🟢)
  - 為所有 10 個 Instructions 添加優先級標記
  - 3 個執行規則詳解 (Rule 1-3)
  - 優先級覆蓋規則 (降低、提升、加速模式)
  - 優先級決策流程圖
  - 優先級記錄與追蹤機制

- ✅ **Instruction 6 強化** (~210 lines 新增)
  - 添加 4 個強制執行時機 (Critical)
  - 5 個詳細驗證項目 (PROJECT-STATUS-REPORT, US 文檔, 測試文檔等)
  - 驗證失敗處理邏輯 (Critical vs Warning)
  - 更新 AI 輸出格式 (包含自動修復建議)
  - 強制執行邏輯和範例

**預期效果**:
- 提供主動錯誤檢測,減少文檔不一致風險
- 幫助 AI 做出更好的決策,減少用戶干預需求
- 文檔驗證從"可選"變為"強制" (在關鍵時機)

---

### v2.0.0 (2025-11-06) - 主動觸發機制
**重大更新**: 從"被動響應式"升級為"主動監測式"設計

**新增內容**:
- ✅ **Instruction 9: 重要事件自動觸發規則**
  - 定義 9 個重要事件 (4 Critical + 3 High + 2 Medium)
  - Event 2 "集成測試完成" 為新增的關鍵事件
  - AI 行為規則 (主動檢測、主動建議、強制執行)
  - 詳細的執行範例和輸出格式

- ✅ **Instruction 10: Session 結束強制檢查清單**
  - 4 個強制檢查項目
  - 自動執行規則 (強制 vs 建議 vs 僅提及)
  - 3 種特殊情況處理
  - 詳細的執行範例和輸出格式

**預期效果**:
- 文檔同步率從 ~50% 提升到 ≥95%
- 自動觸發準確率 ≥90%
- Session 結束檢查執行率 100%
- 用戶干預次數從 ~3次/session 降低到 ≤1次/session

**根本原因**: 解決 Sprint 1 Session 5 中發現的文檔不同步問題

---

### v1.0.0 (2025-11-05) - 初始版本
**首次發布**: 8 個基礎 Instructions

**包含內容**:
- Instruction 1: 更新 PROJECT-STATUS-REPORT.md
- Instruction 2: 生成 Feature 完成報告
- Instruction 3: Git 提交和推送指令
- Instruction 4: 創建 Pull Request
- Instruction 5: 生成 Session 摘要
- Instruction 6: 文檔同步檢查指令
- Instruction 7: 完整工作流程指令
- Instruction 8: 快速進度同步

**設計模式**: 被動響應式 (依賴用戶明確請求)
