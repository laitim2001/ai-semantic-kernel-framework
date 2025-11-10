# PROMPT-09: Session 結束

**場景**: Session 結束前的強制檢查清單
**目標**: 確保文檔同步、代碼已提交、進度已保存
**預估時間**: 3-5 分鐘
**適用對象**: 每次 Session 結束前 (Mandatory 強制執行)

---

## 🎯 使用方式

```
開發者: @PROMPT-09-SESSION-END.md
AI: [執行 Session 結束檢查]
```

**無變數**: 此 Prompt 執行標準結束檢查,無需額外參數

**重要性**: 這是 **Instruction 10** (AI-ASSISTANT-INSTRUCTIONS.md) 的具體實施

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 檢查 1: PROJECT-STATUS-REPORT.md 是否最新? (Critical 🔴)
**執行命令**:
```bash
grep "報告日期" "C:\AI Semantic Kernel\claudedocs\5-status\PROJECT-STATUS-REPORT.md"
```

**檢查項目**:
- [ ] 報告日期 = 今天?
- [ ] 版本號正確遞增?
- [ ] 所有本次 Session 完成的工作都已記錄?
- [ ] 測試統計已更新?
- [ ] Session 歷史已添加?

**輸出**:
```markdown
## 🔍 檢查 1: 狀態報告檢查

### 報告日期檢查
- **當前報告日期**: [從文件讀取]
- **今天日期**: [系統日期]
- **狀態**: [✅ 最新 / ⚠️ 過期 X 天]

### 如果檢查失敗
⚠️ **Critical 問題**: 狀態報告過期 [X] 天

**建議**: 立即執行 `@PROMPT-06-PROGRESS-SAVE.md` 更新狀態報告

我可以立即執行 Instruction 1 更新狀態報告嗎? [Y/N]
```

**如果檢查失敗**:
- 詢問開發者是否要立即更新
- 如果同意,執行 Instruction 1 更新狀態報告

---

### 檢查 2: Git 是否有未提交的變更? (High Priority 🟡)
**執行命令**:
```bash
git status
git diff --stat
```

**檢查項目**:
- [ ] 是否有未暫存的變更?
- [ ] 是否有已暫存但未提交的變更?
- [ ] 是否有 untracked 文件?

**輸出**:
```markdown
## 🔍 檢查 2: Git 狀態檢查

### 未提交變更
- **Modified**: [X files]
- **Staged**: [X files]
- **Untracked**: [X files]

### 狀態
[✅ 所有變更已提交 / ⚠️ 有 X 個未提交變更]

### 如果有未提交變更
⚠️ **未提交的文件**:
[列出文件清單]

**建議**:
1. 使用 `@PROMPT-06-PROGRESS-SAVE.md` 提交變更
2. 或使用 `git stash` 暫存變更

我可以幫你提交這些變更嗎? [Y/N]
```

**如果有未提交變更**:
- 詢問開發者是否要提交
- 如果同意,執行 Instruction 3 Git 工作流程

---

### 檢查 3: Session 摘要是否需要生成? (High Priority 🟡)
**評估條件**:
- Session 時長 > 1 hour?
- 完成了重要工作 (US Phase, Bug 修復, 重大重構)?
- 做出了重要決策?

**輸出**:
```markdown
## 🔍 檢查 3: Session 摘要檢查

### Session 信息
- **Session 時長**: [估算時長]
- **主要工作**: [詢問開發者或從 Git 歷史推斷]
- **重要程度**: [High/Medium/Low]

### 評估結果
[✅ 需要生成 Session 摘要 / ℹ️ 本次 Session 較短,可不生成]

### 如果需要生成
**建議**: 執行 Instruction 5 生成 Session 摘要

我可以幫你生成 Session 摘要嗎? [Y/N]
```

**如果需要生成**:
- 詢問開發者是否要生成
- 如果同意,執行 Instruction 5 生成 Session 摘要

---

### 檢查 4: Todo 事項是否已記錄? (Medium Priority 🟢)
**檢查項目**:
- [ ] 是否有未完成的 User Story?
- [ ] 是否有已知問題需要追蹤?
- [ ] 是否有下次 Session 的待辦事項?

**輸出**:
```markdown
## 🔍 檢查 4: Todo 事項檢查

### 未完成工作
- **User Story**: [當前 US 狀態和完成度]
- **已知問題**: [列出從 PROJECT-STATUS-REPORT.md 讀取]
- **阻塞項**: [是否有阻塞]

### 下次 Session 計劃
[建議開發者記錄以下內容]:
1. [ ] 下次繼續的工作
2. [ ] 需要解決的問題
3. [ ] 需要決策的事項

### 記錄建議
**建議**: 在 PROJECT-STATUS-REPORT.md 或 User Story 文檔中記錄待辦事項
```

---

### 最終輸出: Session 結束總結
**綜合所有檢查結果**:

**理想場景**:
```markdown
# ✅ Session 結束檢查完成

## 檢查結果總覽
- ✅ **檢查 1**: 狀態報告最新 (報告日期: 2025-XX-XX)
- ✅ **檢查 2**: 所有變更已提交 (最後提交: [commit-hash])
- ✅ **檢查 3**: Session 摘要已生成 (SPRINT-X-SESSION-Y-SUMMARY.md)
- ✅ **檢查 4**: Todo 事項已記錄

## 📊 本次 Session 摘要
- **工作時長**: [X hours]
- **User Story**: US X.X - [完成度 X%]
- **主要成果**: [3-5 項]
- **提交數量**: [X commits]

## 📋 下次 Session 計劃
1. [待辦事項 1]
2. [待辦事項 2]

---

**Session 結束時間**: 2025-XX-XX HH:MM UTC
**Status**: ✅ 可以安全結束 Session
**建議**: 休息一下,下次見! 🎉
```

**問題場景**:
```markdown
# ⚠️ Session 結束檢查 - 發現問題

## 檢查結果總覽
- ❌ **檢查 1**: 狀態報告過期 3 天 (最後更新: 2025-XX-XX)
- ⚠️ **檢查 2**: 有 5 個未提交變更
- ℹ️ **檢查 3**: Session 時長 <1h,無需生成摘要
- ⚠️ **檢查 4**: 待辦事項未記錄

## 🚨 Critical 問題 (必須處理)
1. **狀態報告過期**: 需要立即更新
   - **操作**: `@PROMPT-06-PROGRESS-SAVE.md`

## ⚠️ 警告 (建議處理)
1. **未提交變更**: 5 個文件未提交
   - **操作**: 使用 `@PROMPT-06-PROGRESS-SAVE.md` 或 `git stash`

2. **待辦事項未記錄**: 需要記錄下次 Session 計劃
   - **操作**: 在 PROJECT-STATUS-REPORT.md 記錄

---

**Status**: ⚠️ 建議處理問題後再結束 Session
**建議操作順序**:
1. 執行 `@PROMPT-06-PROGRESS-SAVE.md` (處理檢查 1 和 2)
2. 記錄下次 Session 待辦事項
3. 重新執行 `@PROMPT-09-SESSION-END.md` 確認
```

---

## 🎯 最終檢查清單

**在結束 Session 前,確保**:
- ✅ PROJECT-STATUS-REPORT.md 是最新的
- ✅ 所有代碼變更已提交並推送
- ✅ 重要 Session 已生成摘要
- ✅ 下次 Session 計劃已記錄

**如果全部 ✅**: 可以安全結束 Session
**如果有 ❌ 或 ⚠️**: 建議處理後再結束

---

## 📊 預期輸出

- **輸出長度**: ~1000-1500 字
- **文檔讀取數量**: ~2-3 個文件
- **命令執行**: 2-3 次
- **執行時間**: 3-5 分鐘

---

## 💡 使用建議

**每次 Session 結束前都應執行此檢查**:
- 即使只是簡短的 Session
- 即使沒有寫代碼,只是閱讀或規劃
- 即使認為已經保存了所有內容

**這是防止進度遺失的最後一道防線**

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
