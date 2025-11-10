# Sprint 4 概覽 - Persona Framework 基礎

**Sprint 編號**: Sprint 4
**週次**: Week 10-12
**計劃日期**: 2026-01-12 ~ 2026-02-01 (21 days)
**實際日期**: TBD
**狀態**: ⏳ **未開始**

---

## 🎯 Sprint 目標

建立完整的 **Persona Framework**,實現 Persona 模板系統和 Persona-Driven Prompt Engineering,為後續 Persona 功能奠定基礎。

**關鍵交付物**:
1. ⏳ Persona 模板 CRUD 管理系統
2. ⏳ 模板參數系統 (參數定義、驗證、版本控制)
3. ⏳ Persona-Driven Prompt Engineering 引擎
4. ⏳ Persona-Context Merger (上下文注入)
5. ⏳ Dynamic Prompt 生成機制

---

## 📊 User Stories

### 計劃 User Stories

| User Story | Story Points | 計劃天數 | 模組 | 狀態 | 優先級 |
|-----------|-------------|---------|------|------|-------|
| **US 7.1** - Persona 模板配置 | 5 SP | 9 days | Module 7 | ⏳ 未開始 | P0 |
| **US 7.2** - Persona-Driven Prompt Engineering | 8 SP | 12 days | Module 7 | ⏳ 未開始 | P0 |

**總計**: **13 SP** (14 days 開發 + 7 days 測試/文檔)

---

## 📋 詳細技術任務

### US 7.1 - Persona 模板配置 (5 SP)

#### Backend 任務
```yaml
PersonaTemplate CRUD API:
  - POST /api/v1/persona-templates (創建模板)
  - GET /api/v1/persona-templates (列表)
  - GET /api/v1/persona-templates/{id} (詳情)
  - PUT /api/v1/persona-templates/{id} (更新)
  - DELETE /api/v1/persona-templates/{id} (刪除)

模板版本控制:
  - PersonaTemplateVersion Entity
  - 版本號管理 (Major.Minor.Patch)
  - 版本歷史記錄
  - 版本回滾功能
  - GET /api/v1/persona-templates/{id}/versions

參數系統:
  - Parameter Schema 定義 (JSON Schema)
  - 參數類型: string, number, boolean, select, multi-select
  - 參數驗證規則: required, min, max, regex, enum
  - 默認值設置
```

#### Frontend 任務
```yaml
模板編輯器:
  - Template Editor UI (Monaco Editor 或 CodeMirror)
  - Handlebars/Scriban 語法高亮
  - 參數定義界面 (拖拽式參數配置)
  - 模板驗證提示

參數配置界面:
  - Parameter Schema Builder
  - 參數類型選擇器
  - 驗證規則配置
  - 默認值設置

模板預覽:
  - 實時模板渲染預覽
  - 參數測試功能
  - 生成結果展示

版本管理界面:
  - 版本歷史列表
  - 版本對比 (Diff View)
  - 版本回滾功能
```

#### 數據模型擴展
```csharp
PersonaTemplate:
  - Version (string) - 版本號
  - ChangeLog (string) - 版本變更記錄
  - IsPublished (bool) - 是否發布
  - PublishedAt (DateTime?)

PersonaTemplateVersion:
  - Id (Guid)
  - TemplateId (Guid)
  - Version (string)
  - PromptTemplate (string)
  - ParameterSchema (JSON)
  - CreatedAt (DateTime)
  - CreatedBy (string)
```

---

### US 7.2 - Persona-Driven Prompt Engineering (8 SP)

#### Prompt Template Engine 增強
```yaml
功能:
  - Persona 特性提取
  - Context 注入邏輯
  - Dynamic Prompt 生成
  - Prompt 優化策略

Persona 特性提取:
  - 從 Persona Parameters 提取關鍵特性
  - 特性分類: Tone, Style, Expertise, Constraints
  - 特性權重計算

Context 注入:
  - 用戶上下文 (對話歷史、用戶資料)
  - 任務上下文 (當前任務、目標、限制)
  - 知識上下文 (RAG 檢索結果)
  - 注入位置策略 (開頭/中間/結尾)

Dynamic Prompt 生成:
  - 基礎 Persona Prompt + Context
  - Few-shot Examples 注入
  - 動態調整 Prompt 長度 (token 限制)
```

#### API 端點
```yaml
POST /api/v1/prompts/generate:
  - 生成最終 Prompt
  - Request:
      personaId: Guid
      context:
        conversationHistory: Message[]
        userProfile: object
        task: object
        knowledgeContext: string[]
  - Response:
      generatedPrompt: string
      metadata:
        promptLength: number
        includedContext: string[]

POST /api/v1/prompts/optimize:
  - Prompt 優化建議
  - Request: { prompt: string }
  - Response:
      optimizedPrompt: string
      suggestions: string[]
      metrics: { clarity, specificity, token_count }
```

#### Prompt Engineering 最佳實踐
```yaml
Prompt 結構:
  1. Role Definition (from Persona)
  2. Core Instructions (from Template)
  3. Context Integration (dynamic)
  4. Examples (Few-shot, if applicable)
  5. Output Format Specification
  6. Constraints and Guardrails

優化策略:
  - 明確性優化 (Clarity)
  - 具體性優化 (Specificity)
  - Token 效率優化
  - 一致性優化 (Consistency)
```

#### Testing
```yaml
單元測試:
  - Persona 特性提取測試
  - Context 注入測試
  - Prompt 生成測試
  - 不同 Persona + Context 組合測試

A/B Testing Framework:
  - 對比不同 Prompt 生成策略
  - 測量 Agent 回應質量
  - 收集優化數據
```

---

## 🧪 測試策略

### 單元測試 (目標 20 tests)
```yaml
PersonaTemplateServiceTests:
  - 模板 CRUD 測試
  - 版本控制測試
  - 參數驗證測試

PromptEngineTests:
  - Persona 特性提取測試
  - Context 注入測試
  - Prompt 生成測試
  - 優化邏輯測試
```

### 集成測試 (目標 10 tests)
```yaml
PersonaTemplateApiTests:
  - API 端點測試
  - 版本管理測試

PromptGenerationApiTests:
  - Prompt 生成 API 測試
  - 優化 API 測試
```

### E2E 測試 (目標 5 tests)
```yaml
Test 1 - 創建模板流程:
  - 創建新 Persona 模板
  - 定義參數
  - 保存並發布

Test 2 - 版本管理流程:
  - 修改模板
  - 創建新版本
  - 回滾到舊版本

Test 3 - Prompt 生成流程:
  - 選擇 Persona
  - 提供 Context
  - 生成 Prompt
  - 驗證 Prompt 內容

Test 4 - A/B Testing:
  - 生成兩個不同策略的 Prompt
  - 對比結果質量

Test 5 - 端到端集成:
  - Persona → Prompt → Agent 執行
  - 驗證完整流程
```

---

## 🎯 驗收標準 (DoD)

### 功能驗收
- ✅ Persona 模板 CRUD 功能完整
- ✅ 模板版本控制正常運作
- ✅ Prompt 生成邏輯正確
- ✅ Context 注入功能完整
- ✅ 優化建議有效

### 性能驗收
- ✅ 模板渲染時間 < 100ms
- ✅ Prompt 生成時間 < 200ms
- ✅ API 響應時間 < 300ms

### 質量驗收
- ✅ 單元測試通過率 100%
- ✅ 集成測試通過率 100%
- ✅ E2E 測試通過率 100%
- ✅ 測試覆蓋率 ≥ 85%
- ✅ Code Review 通過
- ✅ PO 驗收通過

---

## 🎓 關鍵風險

### 風險 1: Prompt Engineering 策略複雜度
**影響**: 高 🔴
**緩解**:
- 從簡單策略開始,逐步增強
- 參考 OpenAI/Anthropic 最佳實踐
- 建立 Prompt 質量評估框架

### 風險 2: Context 注入影響 Prompt 質量
**影響**: 中 🟡
**緩解**:
- 設計 Context 注入位置實驗
- A/B Testing 驗證策略
- 提供手動調整選項

---

## 🔗 相關文檔

- **User Story 詳情**: [docs/user-stories/sprints/sprint-allocation.md](../../../docs/user-stories/sprints/sprint-allocation.md#sprint-4)
- **依賴 Sprint**: Sprint 3 (US 1.5 必須完成)
- **後續 Sprint**: Sprint 9 (US 1.6 - Persona 模板庫)

---

**維護說明**: 規劃文檔,Sprint 4 開始後更新實際進度。

**狀態**: ⏳ 規劃中
