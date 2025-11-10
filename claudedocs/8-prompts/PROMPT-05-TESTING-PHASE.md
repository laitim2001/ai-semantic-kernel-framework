# PROMPT-05: 測試階段

**場景**: 功能開發完成,準備執行測試
**目標**: 測試執行、失敗分析、覆蓋率檢查
**預估時間**: 5-10 分鐘 (依測試數量)
**適用對象**: 單元測試、集成測試、測試失敗處理

---

## 🎯 使用方式

```
開發者: @PROMPT-05-TESTING-PHASE.md US-1.5
AI: [執行測試並分析結果]
```

**變數說明**:
- `[US-X.X]`: User Story ID (可選,用於關聯測試)
- 如果未提供,執行全部測試

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: 測試前檢查
**編譯檢查**:
```bash
# 編譯解決方案
dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --configuration Release
```

**輸出**:
```markdown
## 🔍 測試前檢查

### 編譯狀態
- **結果**: [成功/失敗]
- **警告**: [X 個]
- **錯誤**: [X 個]

### 編譯問題 (如果有)
- [列出錯誤和警告]
- **建議**: 先修復編譯錯誤再執行測試
```

---

### 步驟 2: 執行單元測試
**執行測試**:
```bash
# 執行單元測試
dotnet test "C:\AI Semantic Kernel\tests\AIAgentPlatform.UnitTests" --configuration Release --no-build --verbosity normal
```

**輸出**:
```markdown
## ✅ 單元測試結果

### 測試統計
- **總測試數**: [X]
- **通過**: [X]
- **失敗**: [X]
- **跳過**: [X]
- **成功率**: [X%]

### 測試執行時間
- **總時間**: [X seconds]
```

---

### 步驟 3: 執行集成測試
**執行測試**:
```bash
# 執行集成測試
dotnet test "C:\AI Semantic Kernel\tests\AIAgentPlatform.IntegrationTests" --configuration Release --no-build --verbosity normal
```

**輸出**:
```markdown
## ✅ 集成測試結果

### 測試統計
- **總測試數**: [X]
- **通過**: [X]
- **失敗**: [X]
- **跳過**: [X]
- **成功率**: [X%]

### 測試執行時間
- **總時間**: [X seconds]
```

---

### 步驟 4: 失敗分析 (如果有失敗)
**如果有測試失敗,執行 Instruction 4 (AI-ASSISTANT-INSTRUCTIONS.md)**:

```markdown
## 🚨 測試失敗分析

### 失敗測試清單
1. **測試名稱**: [FullyQualifiedName]
   - **錯誤訊息**: [Message]
   - **堆疊追蹤**: [StackTrace 關鍵行]

### 根本原因分析
**自動分析步驟**:
1. 讀取失敗測試的源代碼
2. 讀取被測試的功能代碼
3. 分析錯誤訊息和堆疊追蹤
4. 識別可能的原因

**分析結果**:
- **可能原因**: [列出 1-3 個最可能的原因]
- **建議修復**: [具體的修復建議]

### 重新執行失敗測試
```bash
# 重新執行特定失敗測試
dotnet test --filter "FullyQualifiedName~[TestName]" --configuration Release --verbosity detailed
```
```

---

### 步驟 5: 測試覆蓋率檢查 (可選)
**如果配置了測試覆蓋率工具**:
```bash
# 使用 coverlet 生成測試覆蓋率報告
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**輸出**:
```markdown
## 📊 測試覆蓋率

### 覆蓋率統計 (如果可用)
- **Line Coverage**: [X%]
- **Branch Coverage**: [X%]
- **Method Coverage**: [X%]

### 未覆蓋的關鍵代碼
- [列出重要但未測試的代碼]
```

---

### 步驟 6: 測試報告更新
**讀取並更新狀態報告**:
```markdown
## 📝 測試報告更新

### 需要更新的文檔
1. **PROJECT-STATUS-REPORT.md**:
   - 更新測試統計數字
   - 記錄測試執行日期

2. **User Story 文檔** (如果是 US 相關測試):
   - 更新測試完成狀態
   - 記錄測試結果

### 自動更新建議
- 使用 `@PROMPT-06-PROGRESS-SAVE.md` 統一更新所有文檔
```

---

### 步驟 7: 下一步建議
**根據測試結果提供建議**:

**如果所有測試通過**:
```markdown
## ✅ 測試全部通過

### 下一步行動
1. [ ] 執行代碼審查: `@PROMPT-08-CODE-REVIEW.md`
2. [ ] 更新文檔和進度: `@PROMPT-06-PROGRESS-SAVE.md`
3. [ ] 準備創建 PR (如果 US 完成)

### 可選檢查
- [ ] 執行性能測試 (如果是性能敏感功能)
- [ ] 執行手動驗證 (如果是 UI 功能)
```

**如果有測試失敗**:
```markdown
## ⚠️ 測試失敗處理

### 必須執行的步驟
1. [ ] 分析根本原因 (上述分析已提供)
2. [ ] 修復代碼
3. [ ] 重新執行測試
4. [ ] 記錄問題和解決方案

### 參考資源
- **Instruction 4**: 測試失敗處理指引
- **PROMPT-03**: Bug 修復準備 (如果是 Bug)
```

---

## 🎯 最終輸出摘要

**測試狀態總結**:
- 單元測試: [通過/失敗] ([X/Y])
- 集成測試: [通過/失敗] ([X/Y])
- 整體狀態: [✅ 全部通過 / ⚠️ 有失敗]

**關鍵指標**:
- 成功率: [X%]
- 執行時間: [X seconds]
- 覆蓋率: [X%] (如果可用)

---

## 📊 預期輸出

- **輸出長度**: ~1500-2500 字 (依測試結果)
- **測試執行**: 單元測試 + 集成測試
- **執行時間**: 5-10 分鐘 (依測試數量)

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
