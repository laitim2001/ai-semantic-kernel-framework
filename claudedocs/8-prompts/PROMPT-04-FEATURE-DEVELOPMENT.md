# PROMPT-04: 功能開發中

**場景**: 功能開發過程中的指引和檢查
**目標**: 實作指引、代碼質量檢查、進度追蹤
**預估時間**: 依 Phase 而定
**適用對象**: Feature 開發中、Phase 實作中

---

## 🎯 使用方式

```
開發者: @PROMPT-04-FEATURE-DEVELOPMENT.md US-1.5 Phase-2
AI: [執行以下指令,提供 US-1.5 Phase 2 的開發指引]
```

**變數說明**:
- `[US-X.X]`: User Story ID
- `[Phase-N]`: Phase 編號 (可選)
- 如果未提供變數,請詢問開發者當前工作的 US 和 Phase

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: 當前進度檢查
**讀取文件**:
- `docs/user-stories/US-[X.X]-[name].md` (User Story 規格)
- `claudedocs/3-progress/USER-STORY-STATUS.md` (當前狀態)

**輸出**:
```markdown
## 📊 當前進度

### User Story 信息
- **ID**: US X.X
- **標題**: [標題]
- **當前狀態**: [狀態]
- **完成度**: [X%]

### Phase 信息 (如適用)
- **當前 Phase**: Phase X
- **Phase 目標**: [描述]
- **預估時間**: [X days]
```

---

### 步驟 2: 實作指引
**根據 Phase 提供具體指引**:

**如果是 Phase 1 (通常是 Domain/Application Layer)**:
```markdown
## 🏗️ Phase 1 實作指引

### Domain Layer 開發
**檢查清單**:
- [ ] Entity 設計符合 DDD 原則
- [ ] Value Object 正確使用
- [ ] Domain Event 適當定義

**最佳實踐**:
- Entity 包含業務邏輯,不只是數據容器
- Value Object 用於不可變的概念
- Domain Event 用於跨 Aggregate 通訊

### Application Layer 開發
**檢查清單**:
- [ ] Command/Query 遵循 CQRS 模式
- [ ] Handler 保持單一職責
- [ ] Validator 使用 FluentValidation

**代碼質量檢查**:
```bash
# 檢查新增的 Entity
find "C:\AI Semantic Kernel\src\AIAgentPlatform.Domain\Entities" -name "*.cs" -mtime -1

# 檢查新增的 Command/Query
find "C:\AI Semantic Kernel\src\AIAgentPlatform.Application" -name "*Command.cs" -o -name "*Query.cs" -mtime -1
```
```

**如果是 Phase 2 (通常是 Infrastructure/API Layer)**:
```markdown
## 🏗️ Phase 2 實作指引

### Infrastructure Layer 開發
**檢查清單**:
- [ ] Repository 實作遵循 Interface
- [ ] EF Configuration 使用 Fluent API
- [ ] Migration 已創建並測試

**Migration 檢查**:
```bash
# 檢查最新 Migration
ls -lt "C:\AI Semantic Kernel\src\AIAgentPlatform.Infrastructure\Persistence\Migrations" | head -5
```

### API Layer 開發
**檢查清單**:
- [ ] Controller 使用 RESTful 原則
- [ ] Endpoint 有適當的 HTTP 方法和狀態碼
- [ ] API 文檔 (Swagger) 已更新

**API 測試**:
```bash
# 啟動 API (如果未運行)
dotnet run --project "C:\AI Semantic Kernel\src\AIAgentPlatform.API"

# 使用 curl 測試新端點 (範例)
curl -X GET http://localhost:5000/api/[endpoint]
```
```

**如果是 Phase 3 (通常是 Testing)**:
- 使用 `@PROMPT-05-TESTING-PHASE.md` 獲取詳細測試指引

---

### 步驟 3: 代碼質量檢查
**執行靜態檢查**:
```bash
# 編譯檢查
dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --no-restore

# 代碼風格檢查 (如有配置)
dotnet format "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --verify-no-changes
```

**輸出**:
```markdown
## ✅ 代碼質量檢查

### 編譯結果
- **狀態**: [成功/失敗]
- **警告數量**: [X]
- **錯誤數量**: [X]

### 需要處理的問題
- [列出警告和錯誤]
```

---

### 步驟 4: 與 AI-ASSISTANT-INSTRUCTIONS 整合
**提醒開發者相關指引**:
```markdown
## 📋 相關 AI 助手指引

根據當前開發階段,請參考:
- **Instruction 2**: Feature 開發報告 (完成 Phase 時)
- **Instruction 3**: Git 工作流程 (提交代碼前)
- **Instruction 4**: 測試執行與失敗處理 (執行測試時)

**測試提醒**:
- 開發完成後,執行測試: `@PROMPT-05-TESTING-PHASE.md`
- 測試失敗時,參考 AI-ASSISTANT-INSTRUCTIONS.md Instruction 4
```

---

### 步驟 5: 進度追蹤建議
**輸出**:
```markdown
## 📈 進度追蹤

### 建議的檢查點
- [ ] 每完成一個主要組件,提交一次代碼
- [ ] 每完成一個 Phase,更新 User Story 狀態
- [ ] 每日結束前,使用 `@PROMPT-06-PROGRESS-SAVE.md` 保存進度

### Git 提交建議
**提交頻率**: 每完成一個邏輯單元就提交
**提交訊息格式**:
```
feat(US-X.X): [簡要描述]

- [詳細變更 1]
- [詳細變更 2]

Phase X of Y completed
```

### 下一步
- 如果 Phase 完成: 使用 `@PROMPT-06-PROGRESS-SAVE.md` 保存進度
- 如果準備測試: 使用 `@PROMPT-05-TESTING-PHASE.md` 執行測試
```

---

## 🎯 最終輸出摘要

**當前狀態總結**:
- Phase 進度: [X/Y]
- 代碼質量: [良好/需改進]
- 阻塞問題: [列出,如果有]

**下一步行動**:
1. [具體的下一步]
2. [下一個檢查點]

---

## 📊 預期輸出

- **輸出長度**: ~1000-1500 字 (依 Phase 複雜度)
- **文檔讀取數量**: ~3-5 個文件
- **代碼檢查**: 2-3 次
- **執行時間**: 3-5 分鐘

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
