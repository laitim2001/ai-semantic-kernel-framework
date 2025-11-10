# Sprint 3 概覽 - Persona Builder (核心差異化功能)

**Sprint 編號**: Sprint 3
**週次**: Week 7-9
**計劃日期**: 2025-12-22 ~ 2026-01-11 (21 days)
**實際日期**: TBD
**狀態**: ⏳ **未開始**

---

## 🎯 Sprint 目標

實現**核心差異化能力 #1 - 引導式 Persona Builder**,讓用戶通過直觀的引導式界面快速創建具有獨特個性的 AI Agent。

**關鍵交付物**:
1. ⏳ 引導式 Persona Builder UI (Stepper 向導)
2. ⏳ Persona Template Engine (模板引擎)
3. ⏳ 5 個預定義 Persona 模板
4. ⏳ Persona 實時預覽功能
5. ⏳ Persona 保存與應用機制

---

## 📊 User Stories

### 計劃 User Stories

| User Story | Story Points | 計劃天數 | 模組 | 狀態 | 優先級 |
|-----------|-------------|---------|------|------|-------|
| **US 1.5** - 引導式 Persona Builder ⭐ | 8 SP | 14 days | Module 1 | ⏳ 未開始 | P0 - 核心差異化 |

**總計**: **8 SP** (14 days 開發 + 7 days 測試/文檔)

---

## 🎯 核心差異化能力說明

### 為什麼 Persona Builder 是差異化能力?

**競品對比**:
- **OpenAI Custom GPTs**: 僅提供簡單的文字框輸入 system prompt
- **Microsoft Copilot Studio**: 需要技術背景配置 Topic 和 Action
- **我們的 Persona Builder**: 引導式向導 + 預定義模板 + 實時預覽

**差異化優勢**:
1. **零技術門檻**: 非技術用戶通過向導即可創建專業 Persona
2. **最佳實踐內建**: 5 個精心設計的預定義模板
3. **參數化配置**: 模板參數化,靈活調整
4. **實時預覽**: 立即查看 Persona 效果
5. **可複用**: Persona 可保存、分享、重用

---

## 📋 US 1.5 詳細技術任務

### Backend 任務

#### 1. Persona 數據模型設計
```yaml
Persona Entity:
  - Id (Guid)
  - Name (string)
  - Description (string)
  - TemplateId (Guid, nullable) - 基於哪個模板
  - Parameters (JSON) - 參數化配置
  - SystemPrompt (string) - 生成的 System Prompt
  - CreatedBy (string)
  - CreatedAt (DateTime)
  - UpdatedAt (DateTime)

PersonaTemplate Entity:
  - Id (Guid)
  - Name (string)
  - Description (string)
  - Category (string) - 客服/數據分析/助手/導師/創意
  - PromptTemplate (string) - 帶參數佔位符
  - ParameterSchema (JSON) - 參數定義 (name, type, default)
  - Icon (string)
  - IsActive (boolean)
```

#### 2. Persona Template Engine
```csharp
功能:
  - 模板參數解析
  - 參數驗證
  - System Prompt 生成
  - 模板應用邏輯

技術:
  - Handlebars.Net 或 Scriban 模板引擎
  - FluentValidation 參數驗證
```

#### 3. Persona API 端點
```yaml
POST /api/v1/personas:
  - 創建 Persona (基於模板或自定義)
  - Request: { name, description, templateId, parameters }
  - Response: Persona 對象 + 生成的 SystemPrompt

GET /api/v1/personas:
  - 查詢用戶的 Persona 列表
  - 支持篩選: templateId, category

GET /api/v1/personas/{id}:
  - 獲取 Persona 詳情

PUT /api/v1/personas/{id}:
  - 更新 Persona

DELETE /api/v1/personas/{id}:
  - 刪除 Persona (軟刪除)

GET /api/v1/persona-templates:
  - 獲取所有 Persona 模板
  - 預定義 5 個模板

POST /api/v1/personas/preview:
  - 預覽 Persona System Prompt (不保存)
  - Request: { templateId, parameters }
  - Response: { systemPrompt }
```

#### 4. Persona 應用到 Agent
```yaml
關聯邏輯:
  - Agent 可以選擇使用 Persona
  - Agent.PersonaId (Guid, nullable)
  - Agent 執行時,如果有 PersonaId,使用 Persona 的 SystemPrompt
```

---

### Frontend 任務

#### 1. 引導式向導 UI (Stepper)
```yaml
Step 1 - 選擇模板:
  - 顯示 5 個 Persona 模板卡片
  - 每個卡片: Icon, Name, Description
  - 選擇模板或「自定義」

Step 2 - 配置參數:
  - 根據模板參數 Schema 動態生成表單
  - 表單驗證 (React Hook Form + Yup)
  - 參數說明和示例

Step 3 - 實時預覽:
  - 顯示生成的 System Prompt
  - 預覽對話範例 (模擬 Agent 回應)
  - 調整參數,實時更新預覽

Step 4 - 保存與應用:
  - 輸入 Persona Name 和 Description
  - 保存 Persona
  - 選擇應用到哪些 Agent
```

#### 2. Persona 管理界面
```yaml
Persona List View:
  - 顯示用戶創建的所有 Persona
  - 卡片式展示 (Name, Description, Template)
  - 操作: 編輯、刪除、複製、應用到 Agent

Persona Detail View:
  - Persona 詳情
  - 查看完整 System Prompt
  - 編輯參數
  - 查看使用此 Persona 的 Agent 列表
```

#### 3. UI 組件
```yaml
PersonaTemplateCard:
  - 模板卡片組件
  - Props: template, selected, onClick

PersonaParameterForm:
  - 動態參數表單
  - Props: parameterSchema, onChange

PersonaPreview:
  - System Prompt 預覽
  - Props: systemPrompt

PersonaStepper:
  - 向導主組件
  - Material-UI Stepper
```

---

### 5 個預定義 Persona 模板

#### 模板 1: 客戶服務專員 (Customer Support Specialist)
```yaml
Name: 客戶服務專員
Description: 專業、耐心、解決導向的客服 Agent
Category: 客服
Icon: 🎧

Parameters:
  - company_name (string, required): 公司名稱
  - product_name (string, required): 產品名稱
  - support_tone (select): 專業/友善/輕鬆 (default: 友善)
  - max_escalation_level (number): 最大升級層級 (1-3, default: 2)

Prompt Template:
  """
  You are a customer support specialist for {{company_name}}, helping users with {{product_name}}.

  Your tone is {{support_tone}}, and you always:
  - Listen carefully to customer concerns
  - Provide clear, step-by-step solutions
  - Escalate to level {{max_escalation_level}} when needed
  - End with "Is there anything else I can help with?"

  Remember: Customer satisfaction is the top priority.
  """
```

#### 模板 2: 數據分析師 (Data Analyst)
```yaml
Name: 數據分析師
Description: 精準、邏輯清晰、數據驅動的分析師 Agent
Category: 數據分析
Icon: 📊

Parameters:
  - domain_expertise (select): 財務/行銷/運營/產品 (default: 財務)
  - analysis_depth (select): 概覽/詳細/深度 (default: 詳細)
  - visualization_preference (select): 表格/圖表/混合 (default: 混合)

Prompt Template:
  """
  You are a data analyst specializing in {{domain_expertise}} analytics.

  When analyzing data:
  - Provide {{analysis_depth}} analysis with clear insights
  - Use {{visualization_preference}} to present findings
  - Always support conclusions with data evidence
  - Highlight trends, outliers, and actionable recommendations

  Format: Executive Summary → Key Findings → Detailed Analysis → Recommendations
  """
```

#### 模板 3: 個人助手 (Personal Assistant)
```yaml
Name: 個人助手
Description: 高效、細心、主動的個人助理 Agent
Category: 助手
Icon: 🤝

Parameters:
  - formality_level (select): 正式/半正式/輕鬆 (default: 半正式)
  - proactivity (select): 被動/平衡/主動 (default: 平衡)
  - reminder_style (select): 簡潔/詳細 (default: 簡潔)

Prompt Template:
  """
  You are a personal assistant with {{formality_level}} communication style.

  Your approach is {{proactivity}}:
  - Organize information clearly
  - Provide {{reminder_style}} reminders and summaries
  - Anticipate needs and suggest next steps
  - Manage tasks efficiently

  Always confirm understanding and next actions.
  """
```

#### 模板 4: 學習導師 (Learning Mentor)
```yaml
Name: 學習導師
Description: 耐心、啟發式、適應性強的教育 Agent
Category: 導師
Icon: 📚

Parameters:
  - subject_area (string): 學科領域
  - teaching_style (select): 蘇格拉底式/講解式/混合 (default: 混合)
  - difficulty_level (select): 初學/中級/進階 (default: 中級)

Prompt Template:
  """
  You are a learning mentor for {{subject_area}}, teaching at {{difficulty_level}} level.

  Your teaching style is {{teaching_style}}:
  - Break complex concepts into simple steps
  - Use examples and analogies
  - Ask guiding questions to promote thinking
  - Provide constructive feedback
  - Adapt to learner's pace

  Goal: Deep understanding, not just memorization.
  """
```

#### 模板 5: 創意夥伴 (Creative Partner)
```yaml
Name: 創意夥伴
Description: 創新、開放、激發靈感的創意 Agent
Category: 創意
Icon: 🎨

Parameters:
  - creativity_level (select): 保守/平衡/大膽 (default: 平衡)
  - brainstorm_approach (select): 發散/收斂/混合 (default: 混合)
  - domain (string): 創意領域 (寫作/設計/策略等)

Prompt Template:
  """
  You are a creative partner for {{domain}}, with {{creativity_level}} creativity.

  Your brainstorming approach is {{brainstorm_approach}}:
  - Generate diverse, innovative ideas
  - Build on and combine concepts
  - Challenge assumptions
  - Provide constructive critique
  - Inspire unconventional thinking

  Remember: No idea is too wild in the brainstorm phase.
  """
```

---

## 🧪 測試策略

### 單元測試

**Backend Tests** (目標 15 tests):
```yaml
PersonaTemplateEngineTests:
  - 模板參數解析測試
  - 參數驗證測試
  - System Prompt 生成測試
  - 5 個模板生成測試

PersonaServiceTests:
  - Persona CRUD 測試
  - 模板應用測試
  - 預覽功能測試

PersonaValidationTests:
  - 參數驗證規則測試
  - 必填欄位測試
```

**Frontend Tests** (目標 10 tests):
```yaml
PersonaStepperTests:
  - 向導流程測試
  - 步驟切換測試
  - 表單驗證測試

PersonaPreviewTests:
  - 預覽更新測試
  - 實時渲染測試
```

---

### 集成測試

**API Integration Tests** (目標 8 tests):
```yaml
PersonaApiTests:
  - POST /api/v1/personas - 創建 Persona
  - GET /api/v1/personas - 查詢列表
  - PUT /api/v1/personas/{id} - 更新
  - DELETE /api/v1/personas/{id} - 刪除
  - POST /api/v1/personas/preview - 預覽
  - GET /api/v1/persona-templates - 獲取模板

AgentPersonaIntegrationTests:
  - Agent 應用 Persona 測試
  - Agent 執行時使用 Persona SystemPrompt 測試
```

---

### E2E 測試

**E2E Test Scenarios** (目標 5 tests):
```yaml
Test 1 - 完整 Persona 創建流程:
  - 選擇「客戶服務專員」模板
  - 配置參數
  - 預覽 System Prompt
  - 保存 Persona
  - 驗證 Persona 存在於列表

Test 2 - Persona 應用到 Agent:
  - 創建 Persona
  - 創建 Agent
  - 將 Persona 應用到 Agent
  - 執行 Agent,驗證使用 Persona SystemPrompt

Test 3 - Persona 編輯流程:
  - 編輯現有 Persona
  - 修改參數
  - 實時預覽更新
  - 保存變更

Test 4 - 自定義 Persona:
  - 選擇「自定義」
  - 手動輸入 SystemPrompt
  - 保存並驗證

Test 5 - Persona 刪除:
  - 刪除 Persona
  - 驗證軟刪除
  - 驗證關聯 Agent 不受影響
```

---

## 📦 技術依賴

### 新增 NuGet 套件
```yaml
Backend:
  - Scriban (v5.10.0) - 模板引擎
  或
  - Handlebars.Net (v2.1.6) - 模板引擎替代方案
```

### 新增 npm 套件
```yaml
Frontend:
  - @mui/material (已有) - Stepper UI
  - react-hook-form (已有) - 表單管理
  - yup (已有) - 表單驗證
```

---

## 🎯 驗收標準 (DoD)

### 功能驗收
- ✅ 5 個 Persona 模板可用且參數化完整
- ✅ 引導式向導流程順暢 (4 步驟)
- ✅ 實時預覽功能正常運作
- ✅ Persona 可成功應用到 Agent
- ✅ Agent 執行時使用 Persona SystemPrompt

### 性能驗收
- ✅ Persona 創建 API 響應時間 < 200ms
- ✅ 預覽 API 響應時間 < 150ms
- ✅ 模板參數渲染時間 < 100ms

### 質量驗收
- ✅ 單元測試通過率 100% (目標 25 tests)
- ✅ 集成測試通過率 100% (目標 8 tests)
- ✅ E2E 測試通過率 100% (目標 5 tests)
- ✅ 測試覆蓋率 ≥ 85%
- ✅ Code Review 通過

### 用戶體驗驗收
- ✅ 非技術用戶可在 5 分鐘內創建 Persona
- ✅ 向導界面清晰易懂
- ✅ 預覽功能實時響應 (< 500ms)
- ✅ PO 驗收通過

---

## 🎓 關鍵風險與緩解策略

### 風險 1: 模板設計不符合用戶期望
**影響**: 高 🔴
**緩解策略**:
- Sprint Planning 時與 PO 確認 5 個模板選擇
- Sprint 中期與 Stakeholder 驗證模板效果
- 保留調整模板的彈性

### 風險 2: 模板引擎性能不佳
**影響**: 中 🟡
**緩解策略**:
- Sprint 初期進行 Scriban vs Handlebars.Net 技術評估
- 實施參數緩存機制
- 性能測試提前至 Day 7

### 風險 3: 實時預覽延遲
**影響**: 中 🟡
**緩解策略**:
- 前端 Debounce 參數變更 (300ms)
- 後端優化模板渲染邏輯
- 考慮前端本地渲染 (將模板引擎移至前端)

---

## 📊 Sprint Metrics (預估)

### 開發速度
- **計劃 Story Points**: 8 SP
- **計劃天數**: 21 days (14 days 開發 + 7 days 測試)
- **預估速度**: 0.57 SP/day

### 工作量分配
```yaml
Backend (40%):
  - 數據模型: 2 days
  - Template Engine: 3 days
  - API 端點: 3 days
  - 測試: 3 days

Frontend (50%):
  - Stepper UI: 4 days
  - 參數表單: 3 days
  - 預覽功能: 2 days
  - 測試: 2 days

模板設計 (10%):
  - 5 個模板設計: 2 days
```

---

## 🔗 相關文檔

### 技術文檔
- **User Story 詳情**: [docs/user-stories/sprints/sprint-allocation.md](../../../docs/user-stories/sprints/sprint-allocation.md#sprint-3)
- **Persona Framework 設計**: TBD (Sprint 3 產出)
- **模板引擎技術評估**: TBD (Sprint 3 Day 1-2)

### Sprint 管理文檔
- **Sprint Planning**: TBD (Sprint 3 開始前)
- **Daily Standups**: TBD (Sprint 3 執行中)
- **Sprint Retrospective**: TBD (Sprint 3 完成後)

### 相關 Sprint
- **依賴 Sprint**: Sprint 1, Sprint 2 (US 1.1-1.4 必須完成)
- **後續 Sprint**: Sprint 4 (US 7.1, 7.2 - Persona Framework 擴展)

---

## 📅 里程碑

| 里程碑 | 預估日期 | 交付物 |
|-------|---------|--------|
| **M0** | 2025-12-22 | Sprint 3 Planning 完成 |
| **M1** | 2025-12-26 | 模板引擎技術選型完成 |
| **M2** | 2026-01-02 | Backend API 完成 + 5 個模板設計完成 |
| **M3** | 2026-01-07 | Frontend Stepper UI 完成 |
| **M4** | 2026-01-09 | 集成測試完成 |
| **M5** | 2026-01-11 | E2E 測試 + PO 驗收通過 |

---

**維護說明**: 此文檔在 Sprint 3 開始前為規劃狀態,Sprint 執行中更新進度,Sprint 完成後轉為回顧文檔。

**狀態**: ⏳ 規劃中 (等待 Sprint 2 完成)
