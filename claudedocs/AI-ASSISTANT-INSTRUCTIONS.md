# AI Assistant Instructions - 項目管理標準操作程序

本文件提供 AI 助手執行項目進度更新、文檔同步和 Git 操作的標準化指令。

---

## 📋 目錄

1. [進度報告更新指令](#進度報告更新指令)
2. [Git 提交和推送指令](#git-提交和推送指令)
3. [Session 摘要生成指令](#session-摘要生成指令)
4. [文檔同步檢查指令](#文檔同步檢查指令)
5. [完整工作流程指令](#完整工作流程指令)

---

## 🎯 進度報告更新指令

### Instruction 1: 更新 PROJECT-STATUS-REPORT.md

```
請更新項目狀態報告,包含以下內容:

1. 讀取當前的 claudedocs/PROJECT-STATUS-REPORT.md
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

3. 將報告儲存到: claudedocs/feature-reports/[YYYY-MM-DD]-[feature-name].md
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

**錯誤處理**:
- 如果測試失敗,停止流程並報告失敗測試
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

2. 生成摘要文件: claudedocs/sessions/SPRINT-[n]-SESSION-[m]-SUMMARY.md

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

4. 同時更新 claudedocs/PROJECT-STATUS-REPORT.md 中的 Session 記錄區塊
```

---

## 🔍 文檔同步檢查指令

### Instruction 6: 檢查文檔一致性

```
請執行文檔一致性檢查,確保所有文檔同步:

**檢查項目**:

1. **User Story 文檔**:
   - 檢查 docs/user-stories/US-*.md 的狀態欄位
   - 與 PROJECT-STATUS-REPORT.md 的進度比對
   - 報告不一致的地方

2. **Sprint 計劃文檔**:
   - claudedocs/SPRINT-*-ROADMAP.md
   - claudedocs/SPRINT-*-LAUNCH-CHECKLIST.md
   - 確認 checklist 項目與實際完成進度一致

3. **技術文檔**:
   - docs/technical-implementation/*.md
   - 檢查是否有新實作的功能需要更新文檔
   - 報告缺少文檔的新功能

4. **測試文檔**:
   - 檢查 tests/ 目錄下的測試檔案
   - 統計測試覆蓋的功能模組
   - 報告缺少測試的模組

5. **README 文檔**:
   - 檢查根目錄 README.md
   - 檢查 docs/brief-README.md
   - 確認功能清單是否為最新

**輸出格式**:
```markdown
## 📋 文檔一致性檢查報告

**檢查時間**: YYYY-MM-DD HH:MM UTC

### ✅ 一致的文檔
- [列出同步正確的文檔]

### ⚠️ 需要更新的文檔
- **[文件路徑]**: [需要更新的內容描述]
- **[文件路徑]**: [需要更新的內容描述]

### ❌ 缺失的文檔
- **[建議檔名]**: [應該包含的內容描述]

### 📝 更新建議
1. [具體的更新建議 1]
2. [具體的更新建議 2]
```

**執行時機**:
- 完成一個 Phase 後
- 創建 PR 之前
- Sprint 結束前
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
  status_report: "claudedocs/PROJECT-STATUS-REPORT.md"
  session_dir: "claudedocs/sessions/"
  feature_report_dir: "claudedocs/feature-reports/"
  user_stories_dir: "docs/user-stories/"

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

- **開發指南**: QUICK-START-GUIDE.md
- **Sprint 計劃**: claudedocs/SPRINT-*-ROADMAP.md
- **狀態報告**: claudedocs/PROJECT-STATUS-REPORT.md
- **User Stories**: docs/user-stories/US-*.md

---

**文檔版本**: 2.0.0
**創建日期**: 2025-11-05
**最後更新**: 2025-11-06
**維護者**: AI Assistant (Claude Code)
**更新頻率**: 根據項目需求更新

---

## 📝 版本歷史

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
