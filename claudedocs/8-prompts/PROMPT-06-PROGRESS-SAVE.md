# PROMPT-06: 進度保存與同步

**場景**: 保存當前開發進度,同步文檔,上傳到 GitHub
**目標**: 文檔同步、索引更新、Git 操作、狀態報告更新
**預估時間**: 5-10 分鐘
**適用時機**: 每日結束、重要里程碑、功能完成、Session 中斷前

---

## 🎯 使用方式

```
開發者: @PROMPT-06-PROGRESS-SAVE.md US-1.5 Phase-2
AI: [保存 US-1.5 Phase 2 的進度並同步文檔]
```

**變數說明**:
- `[US-X.X]`: 當前 User Story ID
- `[Phase-N]`: 當前 Phase (可選)
- 如果未提供,請詢問開發者當前工作內容

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: 工作信息收集
**詢問開發者** (如果變數未提供):
- User Story ID
- Phase (如適用)
- 工作狀態 (開發中/測試中/已完成)
- 完成度 (X%)
- 今日主要工作 (簡述)

**輸出**:
```markdown
## 📝 當前工作信息

- **User Story**: US X.X - [名稱]
- **Phase**: Phase X (如適用)
- **狀態**: [開發中/測試中/已完成]
- **完成度**: [X%]
- **今日工作**: [簡述]
```

---

### 步驟 2: 文檔一致性檢查 (Critical ⭐)
**執行 Instruction 6 (AI-ASSISTANT-INSTRUCTIONS.md)**

**檢查以下文檔**:
- `claudedocs/5-status/PROJECT-STATUS-REPORT.md`
- `claudedocs/3-progress/USER-STORY-STATUS.md`
- `docs/user-stories/US-[X.X]-[name].md`

**輸出**:
```markdown
## 📋 文檔一致性檢查

### ✅ 一致的文檔 (X/3)
- [列出已同步的文檔]

### ⚠️ 需要更新的文檔 (X/3)
- [文檔名稱]: [具體不一致的地方]

### 自動修復計劃
- [列出 AI 將自動修復的問題]
```

---

### 步驟 3: 更新項目狀態報告 (Critical ⭐)
**執行 Instruction 1 (AI-ASSISTANT-INSTRUCTIONS.md)**

**更新 PROJECT-STATUS-REPORT.md**:
- 報告日期 (當前日期)
- Sprint 進度百分比
- User Story 狀態
- 完成的功能清單
- 測試統計 (如有)
- 已知問題 (如有)
- Session 歷史

**輸出**:
```markdown
## ✅ 狀態報告已更新

- **版本**: v[X.Y.Z] → v[X.Y.Z+1]
- **報告日期**: [更新為今日]
- **Sprint 進度**: [X%] → [Y%]
- **新增功能**: [列出]
- **測試統計**: [更新數量]
```

---

### 步驟 4: 更新 User Story 狀態
**更新 USER-STORY-STATUS.md**:

找到當前 User Story,更新:
- 狀態
- 實際 Story Points (如有變更)
- 實際時間
- 完成百分比
- 最後更新日期

**輸出**:
```markdown
## ✅ User Story 狀態已更新

- **US [X.X]**: [狀態] - [完成度 X%]
- **實際時間**: [X days] (計劃: [Y days])
- **Story Points**: [X SP] (計劃: [Y SP])
```

---

### 步驟 5: Git 狀態檢查 (Critical ⭐)
**執行命令**:
```bash
git status
git diff --stat
git log -3 --oneline
```

**分析並輸出**:
```markdown
## 🔍 Git 狀態檢查

### 未提交的變更
- **Modified**: [X files]
- **Added**: [X files]
- **Untracked**: [X files]

### 變更摘要
- **Domain Layer**: [X files changed]
- **Application Layer**: [X files changed]
- **Infrastructure Layer**: [X files changed]
- **API Layer**: [X files changed]
- **Tests**: [X files changed]
- **Docs**: [X files changed]

### 最近提交
1. [commit hash] - [message]
2. [commit hash] - [message]
3. [commit hash] - [message]

### 建議操作
- [ ] 需要提交代碼變更
- [ ] 需要推送到遠端
```

---

### 步驟 6: 執行 Git 工作流程 (如需要)
**如果有未提交的變更,執行 Instruction 3**:

1. 檢查是否在正確的 feature 分支
2. 執行測試 (如果是代碼變更)
3. 暫存變更
4. 生成提交訊息
5. 提交
6. 推送到遠端

**提交訊息格式**:
```
feat(US-X.X): [簡要描述]

- [詳細變更 1]
- [詳細變更 2]

Phase X completed / In progress

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>
```

**輸出**:
```markdown
## ✅ Git 操作完成

- **分支**: [branch-name]
- **提交**: [commit-hash]
- **提交訊息**: [message]
- **推送**: ✅ 成功推送到 origin/[branch-name]

### 變更統計
- **Files changed**: [X]
- **Insertions**: +[X]
- **Deletions**: -[X]
```

---

### 步驟 7: 生成 Session 摘要 (可選)
**如果 Session 時長 >1h 或完成重要工作**:

**執行 Instruction 5 (AI-ASSISTANT-INSTRUCTIONS.md)**

生成 Session 摘要並保存到:
`claudedocs/2-sprints/sprint-[N]/SPRINT-[N]-SESSION-[M]-SUMMARY.md`

**輸出**:
```markdown
## ✅ Session 摘要已生成

- **檔案**: claudedocs/2-sprints/sprint-[N]/SPRINT-[N]-SESSION-[M]-SUMMARY.md
- **Session 時長**: [X hours]
- **主要成果**: [列出 3-5 項]
```

---

### 步驟 8: 索引維護 (可選)
**檢查並更新以下索引文件**:

1. **FEATURE-COMPLETION-MATRIX.md**:
   - 更新模組完成度

2. **MASTER-DEVELOPMENT-SCHEDULE.md**:
   - 更新實際進度日期 (如適用)

3. **CHANGE-LOG.md**:
   - 記錄重要變更或決策 (如有)

**輸出**:
```markdown
## ✅ 索引文件已更新

- ✅ FEATURE-COMPLETION-MATRIX.md: Module [X] 完成度 [Y%] → [Z%]
- ✅ MASTER-DEVELOPMENT-SCHEDULE.md: [更新內容]
- ✅ CHANGE-LOG.md: [新增記錄] (如適用)
```

---

### 步驟 9: 最終驗證
**再次執行文檔一致性檢查**:

**輸出**:
```markdown
## ✅ 最終驗證通過

- ✅ 所有文檔已同步
- ✅ Git 變更已提交並推送
- ✅ 狀態報告已更新
- ✅ Session 摘要已生成 (如適用)

### 一致性得分
- **得分**: [X%] (目標: ≥95%)
- **Critical 問題**: 0 個
- **Warning 問題**: [X] 個
```

---

## 🎯 最終輸出摘要

**完整的進度保存總結**:
```markdown
# 進度保存與同步完成報告

## 📊 工作摘要
- **User Story**: US [X.X] - [名稱]
- **完成度**: [X%]
- **今日工作**: [簡述]

## ✅ 文檔同步狀態
- ✅ PROJECT-STATUS-REPORT.md (v[X.Y.Z])
- ✅ USER-STORY-STATUS.md
- ✅ Session 摘要已生成 (如適用)

## ✅ Git 狀態
- ✅ 分支: [branch-name]
- ✅ 提交: [commit-hash]
- ✅ 推送: 成功

## 📋 下次 Session 計劃
1. [待辦事項 1]
2. [待辦事項 2]
3. [待辦事項 3]

---

**保存時間**: [當前日期時間] UTC
**一致性得分**: [X%]
**Status**: ✅ 全部完成
```

---

## 📊 預期輸出

- **輸出長度**: ~2000-3000 字
- **文檔更新數量**: 4-6 個文件
- **Git 操作**: 提交 + 推送
- **執行時間**: 5-10 分鐘

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
