# Sprint 執行框架優化方案

**版本**: 2.0
**日期**: 2025-11-12
**狀態**: 提案待確認

---

## 🎯 目標

解決當前問題:
1. **規劃文件太多太詳細** - /docs 有 180+ 文件,AI助手無法有效引用
2. **Sprint文件太簡單** - 只有OVERVIEW,缺乏執行指引
3. **設計與實際脫節** - AI助手開發時沒有參考設計文檔

**核心原則**:
- ✅ **Just-in-Time Documentation** - 在需要時提供恰當的資訊
- ✅ **Progressive Disclosure** - 逐層展開細節,避免資訊過載
- ✅ **AI-Friendly Format** - 為AI助手優化的結構化格式
- ✅ **Single Source of Truth** - 每個Sprint的執行指引集中在一處

---

## 📂 新的Sprint文件結構

### 每個Sprint文件夾應包含:

```
/claudedocs/2-sprints/sprint-X/
├── 📄 SPRINT-X-OVERVIEW.md          # Sprint概覽 (保留)
├── 📋 SPRINT-X-PLAN.md              # 🆕 Sprint執行計劃
├── ✅ SPRINT-X-CHECKLIST.md         # 🆕 實施檢查清單 (可標記進度)
├── 📝 SPRINT-X-DEV-LOG.md           # 🆕 開發日誌
├── 📚 SPRINT-X-CONTEXT.md           # 🆕 上下文參考 (精簡引用)
├── 🐛 SPRINT-X-ISSUES.md            # 🆕 問題追蹤
└── 📊 SPRINT-X-RETROSPECTIVE.md     # 🆕 Sprint回顧 (完成後)
```

---

## 📄 文件格式詳細說明

### 1. SPRINT-X-OVERVIEW.md (保留,優化)

**目的**: Sprint總覽 - AI助手開始前的第一份文件

**結構**:
```markdown
# Sprint X Overview

## 🎯 Sprint目標
**一句話目標**: [簡潔描述本Sprint要達成什麼]

## 📅 時程
- 開始日期: YYYY-MM-DD
- 結束日期: YYYY-MM-DD
- 持續時間: X週

## 🎫 User Stories
| ID | 標題 | 優先級 | 預估點數 | 狀態 |
|----|------|--------|---------|------|
| US X.X | ... | P0 | 5 | 🔄 進行中 |

## 📊 成功指標
- [ ] 指標1: [具體可測量]
- [ ] 指標2: [具體可測量]

## ⚠️ 風險與依賴
- 風險1: [描述] - 緩解措施: [...]
- 依賴1: [描述] - 負責人: [...]

## 📚 相關文檔
- SPRINT-X-PLAN.md - 詳細執行計劃
- SPRINT-X-CHECKLIST.md - 實施檢查清單
- SPRINT-X-CONTEXT.md - 上下文參考
```

---

### 2. SPRINT-X-PLAN.md 🆕 (詳細執行計劃)

**目的**: 將 /docs 中的規劃文件**精簡提煉**為本Sprint的執行計劃

**結構**:
```markdown
# Sprint X 執行計劃

## 📋 本Sprint要實現的功能 (What to Build)

### US X.1: [User Story標題]

#### MVP範圍定義 ⭐
**必須實現** (P0):
- [ ] 功能點1: [具體描述]
- [ ] 功能點2: [具體描述]

**可選實現** (P1):
- [ ] 功能點3: [如果時間允許]

**Phase 2功能** (不在本Sprint):
- ❌ 功能點4: [明確排除]

#### 技術規格摘要
**來源**: `/docs/user-stories/modules/module-XX.md` (精簡版)

**核心需求**:
- 輸入: [...]
- 輸出: [...]
- 性能標準: [P95 <500ms]

**技術實現要點**:
1. Backend: [ASP.NET Core + SK]
2. Frontend: [React組件]
3. Database: [新增Table: ...]

#### UI設計參考 (僅MVP範圍)
**來源**: `/docs/ux-design/wireframes/low-fidelity/XX.md` (精簡版)

**本Sprint實現的UI**:
- Layout: 2欄布局 (Sidebar + Chat Area)
- 核心組件: ChatWindow, MessageList, InputBox
- **不實現**: 3欄布局, Function Calling可視化 (Phase 2)

**UI Mock**:
```
+------------------+------------------------+
| Sidebar (280px)  | Chat Area (flex-grow)  |
| - Conversations  | - Header               |
| - New Chat       | - Message List         |
|                  | - Input Box            |
+------------------+------------------------+
```

#### 驗收標準 (Definition of Done)
- [ ] 單元測試覆蓋率 >80%
- [ ] 集成測試通過
- [ ] UI符合設計 (MVP範圍)
- [ ] 性能達標 (P95 <500ms)
- [ ] Code Review通過
- [ ] 文檔更新完成

---

### US X.2: [下一個User Story]
[重複相同結構]

---

## 🔧 技術實施指引

### 開發順序 (Recommended)
1️⃣ **Phase 1: Backend API** (Day 1-3)
   - 實現 API Endpoints
   - 數據庫Schema遷移
   - 單元測試

2️⃣ **Phase 2: Frontend UI** (Day 4-6)
   - 實現React組件
   - API集成
   - UI測試

3️⃣ **Phase 3: Integration & Testing** (Day 7-8)
   - E2E測試
   - 性能測試
   - Bug修復

4️⃣ **Phase 4: Documentation & Review** (Day 9-10)
   - 代碼審查
   - 文檔更新
   - Sprint回顧

### 關鍵技術決策
**決策1**: [技術選型]
- **選擇**: [...]
- **理由**: [...]
- **參考**: `/docs/architecture/ADR-XXX.md`

### 安全考量
- [ ] SQL Injection防護
- [ ] XSS防護
- [ ] CSRF Token驗證
- [ ] 參考: `/docs/technical-implementation/9-security-standards/`

### 性能優化
- [ ] API響應緩存 (Redis)
- [ ] Database Query優化
- [ ] 前端虛擬滾動 (如需要)

---

## 📦 Deliverables (交付物清單)

### Backend
- [ ] `AgentController.cs` - Agent CRUD API
- [ ] `AgentService.cs` - 業務邏輯
- [ ] `AgentRepository.cs` - 數據訪問
- [ ] `AgentDto.cs` - DTO定義
- [ ] Database Migration: `20251112_AddAgentTable.cs`

### Frontend
- [ ] `AgentList.tsx` - Agent列表組件
- [ ] `AgentCard.tsx` - Agent卡片組件
- [ ] `CreateAgentDialog.tsx` - 創建Agent對話框
- [ ] `useAgents.ts` - Agent數據Hook
- [ ] `agentService.ts` - API Client

### Tests
- [ ] `AgentService.Tests.cs` - 單元測試
- [ ] `AgentController.Tests.cs` - 控制器測試
- [ ] `AgentList.test.tsx` - 組件測試
- [ ] `agent.e2e.spec.ts` - E2E測試

### Documentation
- [ ] API文檔更新 (`/docs/technical-implementation/04-API-SPECIFICATION.md`)
- [ ] 開發日誌更新 (`SPRINT-X-DEV-LOG.md`)
- [ ] 回顧文檔 (`SPRINT-X-RETROSPECTIVE.md`)
```

---

### 3. SPRINT-X-CHECKLIST.md 🆕 (實施檢查清單)

**目的**:
- AI助手的執行清單 (可逐步標記完成)
- 同時作為進度追蹤文件

**結構**:
```markdown
# Sprint X 實施檢查清單

**更新時間**: 自動更新
**完成度**: 45/100 (45%)

---

## 📊 進度總覽

| 類別 | 完成 / 總數 | 百分比 |
|------|------------|--------|
| Backend | 15/30 | 50% |
| Frontend | 12/25 | 48% |
| Testing | 8/20 | 40% |
| Documentation | 5/10 | 50% |
| **總計** | **45/100** | **45%** |

---

## 🔧 Backend 開發

### US X.1: Agent CRUD API

#### Database Schema
- [x] 設計Agent Table Schema
- [x] 創建EF Core Entity (`Agent.cs`)
- [x] 創建Migration文件
- [ ] 執行Migration
- [ ] 驗證Schema正確性

#### API Endpoints
- [x] `GET /api/agents` - 列表
- [x] `GET /api/agents/{id}` - 詳情
- [ ] `POST /api/agents` - 創建
- [ ] `PUT /api/agents/{id}` - 更新
- [ ] `DELETE /api/agents/{id}` - 刪除

#### Business Logic
- [x] `AgentService.cs` - GetAll
- [x] `AgentService.cs` - GetById
- [ ] `AgentService.cs` - Create
- [ ] `AgentService.cs` - Update
- [ ] `AgentService.cs` - Delete
- [ ] Input Validation
- [ ] Error Handling

#### Data Access
- [x] `AgentRepository.cs` - GetAll
- [x] `AgentRepository.cs` - GetById
- [ ] `AgentRepository.cs` - Create
- [ ] `AgentRepository.cs` - Update
- [ ] `AgentRepository.cs` - Delete

---

## 🎨 Frontend 開發

### US X.1: Agent列表頁面

#### 組件開發
- [x] `AgentList.tsx` - 基礎結構
- [x] `AgentCard.tsx` - 卡片組件
- [ ] `CreateAgentDialog.tsx` - 創建對話框
- [ ] `EditAgentDialog.tsx` - 編輯對話框
- [ ] `DeleteConfirmDialog.tsx` - 刪除確認

#### State Management
- [x] `useAgents.ts` - 數據Hook
- [ ] Zustand Store (如需要)
- [ ] Loading State處理
- [ ] Error State處理

#### API Integration
- [x] `agentService.ts` - API Client基礎
- [ ] `getAgents()` - GET列表
- [ ] `createAgent()` - POST創建
- [ ] `updateAgent()` - PUT更新
- [ ] `deleteAgent()` - DELETE刪除

#### UI/UX
- [x] Layout實現 (2欄)
- [ ] Responsive設計
- [ ] Loading指示器
- [ ] Error提示
- [ ] 空狀態設計

---

## 🧪 Testing

### Unit Tests
- [x] `AgentService.Tests.cs` - GetAll
- [x] `AgentService.Tests.cs` - GetById
- [ ] `AgentService.Tests.cs` - Create
- [ ] `AgentService.Tests.cs` - Update
- [ ] `AgentService.Tests.cs` - Delete
- [ ] 測試覆蓋率 >80%

### Integration Tests
- [ ] `AgentController.Tests.cs` - API集成測試
- [ ] Database集成測試

### Frontend Tests
- [x] `AgentList.test.tsx` - 組件渲染
- [ ] `AgentList.test.tsx` - 用戶交互
- [ ] `useAgents.test.ts` - Hook測試

### E2E Tests
- [ ] `agent.e2e.spec.ts` - 創建Agent流程
- [ ] `agent.e2e.spec.ts` - 編輯Agent流程
- [ ] `agent.e2e.spec.ts` - 刪除Agent流程

---

## 📚 Documentation

### API Documentation
- [x] API Endpoints文檔
- [ ] 請求/響應示例
- [ ] 錯誤代碼說明

### Code Documentation
- [x] Backend XML註釋
- [ ] Frontend JSDoc註釋
- [ ] README更新

### Process Documentation
- [ ] 開發日誌更新 (`SPRINT-X-DEV-LOG.md`)
- [ ] 問題追蹤 (`SPRINT-X-ISSUES.md`)
- [ ] 回顧文檔 (`SPRINT-X-RETROSPECTIVE.md`)

---

## ✅ Definition of Done

### 代碼質量
- [ ] 單元測試覆蓋率 >80%
- [ ] 集成測試通過
- [ ] E2E測試通過
- [ ] ESLint無錯誤
- [ ] 無Console.log殘留

### 性能標準
- [ ] API響應時間 P95 <500ms
- [ ] 前端首屏渲染 <2s
- [ ] 無記憶體洩漏

### 安全標準
- [ ] SQL Injection測試通過
- [ ] XSS測試通過
- [ ] CSRF Token驗證
- [ ] 輸入驗證完整

### Code Review
- [ ] Code Review完成
- [ ] 所有Comment已解決
- [ ] 無Critical問題

### Documentation
- [ ] API文檔更新
- [ ] 代碼註釋完整
- [ ] README更新

---

## 📝 Notes

### Blockers
- 無

### Deferred Items
- 3欄布局 (Phase 2)
- Function Calling可視化 (Phase 2)

### Learnings
- [記錄開發過程中的發現]
```

---

### 4. SPRINT-X-DEV-LOG.md 🆕 (開發日誌)

**目的**: 記錄開發過程中的決策、問題、解決方案

**結構**:
```markdown
# Sprint X 開發日誌

---

## 📅 Day 1 (2025-11-12)

### 🎯 今日目標
- [ ] 完成Agent Table Schema設計
- [ ] 實現GET /api/agents API
- [ ] 創建AgentList前端組件

### ✅ 完成內容
- [x] Agent Table Schema設計完成
  - 文件: `src/Infrastructure/Configurations/AgentConfiguration.cs`
  - Migration: `20251112_AddAgentTable.cs`
- [x] GET /api/agents API實現
  - Endpoint: `AgentController.GetAgents()`
  - Service: `AgentService.GetAllAsync()`

### 🐛 遇到的問題

#### 問題1: Entity Framework Migration失敗
**現象**: `dotnet ef migrations add` 報錯 "DbContext not found"

**原因**: Startup.cs中未註冊DbContext

**解決方案**:
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
```

**參考**: `/docs/technical-implementation/6-database-standards/entity-framework-core-configuration.md`

#### 問題2: API返回null
**現象**: GET /api/agents 返回 null

**原因**: Repository未註冊到DI容器

**解決方案**:
```csharp
services.AddScoped<IAgentRepository, AgentRepository>();
```

### 💡 學到的知識
- EF Core Migration需要在Startup project中執行
- Repository Pattern的依賴注入順序很重要

### 📊 今日進度
- Backend: 10/30 (33%)
- Frontend: 5/25 (20%)
- Testing: 0/20 (0%)

### 🔜 明日計劃
- [ ] 實現POST /api/agents
- [ ] 完成AgentCard組件
- [ ] 開始單元測試

---

## 📅 Day 2 (2025-11-13)

[繼續記錄...]

---

## 📊 Sprint總結 (Sprint結束後更新)

### 成功
- ✅ 完成所有P0功能
- ✅ 測試覆蓋率達到85%
- ✅ 性能達標

### 挑戰
- ⚠️ 數據庫遷移花費比預期多1天
- ⚠️ UI細節調整需求變更

### 改進建議
- 💡 下次Sprint提前做數據庫設計Review
- 💡 UI設計需要更早確認
```

---

### 5. SPRINT-X-CONTEXT.md 🆕 (上下文參考)

**目的**: 精簡引用 /docs 中的關鍵內容,避免AI助手查找困難

**結構**:
```markdown
# Sprint X 上下文參考

## 📚 本Sprint相關的規劃文檔快速參考

---

## US X.1: [User Story]

### 完整需求文檔位置
- Module定義: `/docs/user-stories/modules/module-XX.md`
- Epic詳細: `/docs/user-stories/epics/epic-XX.md`
- UI設計: `/docs/ux-design/wireframes/low-fidelity/XX.md`

### MVP範圍摘要 (精簡版)

**必須實現的核心功能**:
1. 功能點1: [從Epic文檔精簡提取]
2. 功能點2: [從Epic文檔精簡提取]

**明確排除的功能** (Phase 2):
- ❌ 高級功能1
- ❌ 高級功能2

### 技術規格摘要

#### Backend API
**Endpoint**: `GET /api/agents`
**Request**:
```json
{
  "page": 1,
  "pageSize": 20,
  "filter": "active"
}
```

**Response**:
```json
{
  "agents": [...],
  "total": 100,
  "page": 1
}
```

**參考**: `/docs/technical-implementation/04-API-SPECIFICATION.md` (Line 123-145)

#### Database Schema
**Table**: `Agents`
```sql
CREATE TABLE Agents (
  Id UUID PRIMARY KEY,
  Name VARCHAR(255) NOT NULL,
  Description TEXT,
  CreatedAt TIMESTAMP,
  UpdatedAt TIMESTAMP
);
```

**參考**: `/docs/architecture/database-schema.md` (Line 234-267)

#### UI組件結構
**Layout**: 2欄布局
```
+------------------+------------------------+
| Sidebar (280px)  | Main Area              |
+------------------+------------------------+
```

**主要組件**:
- `AgentList.tsx`
- `AgentCard.tsx`
- `CreateAgentDialog.tsx`

**參考**: `/docs/ux-design/wireframes/low-fidelity/02-agent-list.md` (Line 45-120)

### 相關架構決策
- **ADR-006**: Agent狀態管理策略
  - 決策: 使用Database持久化 + Redis緩存
  - 理由: [...]
  - 文件: `/docs/architecture/ADR-006-agent-state-management.md`

### 編碼標準參考
- C#: `/docs/technical-implementation/4-coding-standards/csharp-coding-standards.md`
- TypeScript: `/docs/technical-implementation/4-coding-standards/typescript-coding-standards.md`
- React: `/docs/technical-implementation/4-coding-standards/react-coding-standards.md`

**關鍵規範**:
- 命名: PascalCase for C# classes, camelCase for TS/JS
- 註釋: XML comments for public APIs
- Error Handling: Try-catch with logging

---

## US X.2: [下一個User Story]
[重複相同結構]
```

---

### 6. SPRINT-X-ISSUES.md 🆕 (問題追蹤)

**目的**: 追蹤Sprint中遇到的問題和解決狀態

**結構**:
```markdown
# Sprint X 問題追蹤

## 🐛 Open Issues

### Issue #1: [問題標題]
**發現時間**: 2025-11-12 14:30
**嚴重性**: 🔴 Critical / 🟡 Medium / 🟢 Low
**負責人**: AI Assistant / Developer Name

**現象**:
[詳細描述問題]

**重現步驟**:
1. Step 1
2. Step 2
3. Step 3

**預期行為**:
[應該如何運作]

**實際行為**:
[實際發生了什麼]

**臨時解決方案** (Workaround):
[如果有]

**根本原因分析**:
[分析問題根源]

**解決方案**:
[提出的解決方案]

**狀態**: 🔄 進行中 / ✅ 已解決 / ❌ 無法解決

---

## ✅ Resolved Issues

### Issue #2: [已解決的問題]
**解決時間**: 2025-11-13 10:00
**解決方案**: [...]
**相關Commit**: `abc123`
**學到的教訓**: [...]
```

---

### 7. SPRINT-X-RETROSPECTIVE.md 🆕 (Sprint回顧)

**目的**: Sprint結束後的回顧總結

**結構**:
```markdown
# Sprint X 回顧

**Sprint結束日期**: 2025-11-22
**參與人員**: Team Members

---

## 📊 Sprint指標

| 指標 | 計劃 | 實際 | 達成率 |
|------|------|------|--------|
| User Stories | 5 | 4 | 80% |
| Story Points | 25 | 20 | 80% |
| 測試覆蓋率 | >80% | 85% | ✅ |
| Bug Count | <5 | 3 | ✅ |

---

## ✅ 做得好的地方 (Keep)

1. **Backend開發速度快**
   - 提前1天完成所有API
   - 測試覆蓋率達到85%

2. **代碼質量高**
   - Code Review無Critical問題
   - 技術債務少

---

## ⚠️ 需要改進的地方 (Improve)

1. **UI設計變更頻繁**
   - 問題: 設計文檔與實際需求不一致
   - 改進: 下次Sprint前做UI設計Review

2. **測試環境不穩定**
   - 問題: Docker容器頻繁崩潰
   - 改進: 升級Docker版本

---

## 💡 學到的知識 (Insights)

1. **技術**:
   - Entity Framework Core Migration最佳實踐
   - React Hook的性能優化技巧

2. **流程**:
   - 每日Stand-up很有幫助
   - Code Review應該更早開始

---

## 🔜 下個Sprint的行動計劃

1. [ ] UI設計Review (Sprint開始前完成)
2. [ ] 升級Docker版本
3. [ ] 建立自動化測試環境
```

---

## 🚀 使用流程

### Sprint開始前 (Day -1)

1. **Product Owner / Scrum Master**:
   ```bash
   # 創建Sprint文件夾和基礎文件
   cd claudedocs/2-sprints/
   mkdir sprint-X
   cd sprint-X

   # 創建所有必需文件
   touch SPRINT-X-OVERVIEW.md
   touch SPRINT-X-PLAN.md
   touch SPRINT-X-CHECKLIST.md
   touch SPRINT-X-DEV-LOG.md
   touch SPRINT-X-CONTEXT.md
   touch SPRINT-X-ISSUES.md
   ```

2. **填寫規劃文件**:
   - `SPRINT-X-OVERVIEW.md` - Sprint目標、User Stories
   - `SPRINT-X-PLAN.md` - 從 /docs 提取精簡的執行計劃
   - `SPRINT-X-CONTEXT.md` - 精簡引用關鍵規劃文檔
   - `SPRINT-X-CHECKLIST.md` - 創建詳細的任務清單

### Sprint進行中 (Day 1-10)

3. **AI助手開始工作**:
   ```
   1. 閱讀 SPRINT-X-OVERVIEW.md - 了解Sprint目標
   2. 閱讀 SPRINT-X-PLAN.md - 了解要實現什麼
   3. 閱讀 SPRINT-X-CHECKLIST.md - 逐步執行任務
   4. 參考 SPRINT-X-CONTEXT.md - 查找技術規格
   5. 更新 SPRINT-X-CHECKLIST.md - 標記完成進度
   6. 記錄 SPRINT-X-DEV-LOG.md - 記錄開發過程
   7. 記錄 SPRINT-X-ISSUES.md - 追蹤問題
   ```

4. **每日更新**:
   - ✅ 更新 `SPRINT-X-CHECKLIST.md` 進度
   - 📝 記錄 `SPRINT-X-DEV-LOG.md` 當日工作
   - 🐛 更新 `SPRINT-X-ISSUES.md` 問題狀態

### Sprint結束後 (Day 11)

5. **Sprint回顧**:
   - 填寫 `SPRINT-X-RETROSPECTIVE.md`
   - 更新 `/claudedocs/3-progress/USER-STORY-STATUS.md`
   - 更新 `/claudedocs/PROJECT-STATUS-REPORT.md`

---

## 📋 文件模板

### 提供標準模板

為了加快Sprint準備速度,提供標準模板:

```
/claudedocs/templates/
├── SPRINT-OVERVIEW-TEMPLATE.md
├── SPRINT-PLAN-TEMPLATE.md
├── SPRINT-CHECKLIST-TEMPLATE.md
├── SPRINT-DEV-LOG-TEMPLATE.md
├── SPRINT-CONTEXT-TEMPLATE.md
├── SPRINT-ISSUES-TEMPLATE.md
└── SPRINT-RETROSPECTIVE-TEMPLATE.md
```

**自動化腳本** (可選):
```bash
#!/bin/bash
# create-sprint.sh

SPRINT_NUM=$1
SPRINT_DIR="claudedocs/2-sprints/sprint-$SPRINT_NUM"

mkdir -p $SPRINT_DIR
cp claudedocs/templates/* $SPRINT_DIR/

# 替換模板中的變量
sed -i "s/{{SPRINT_NUM}}/$SPRINT_NUM/g" $SPRINT_DIR/*.md

echo "Sprint $SPRINT_NUM 文件創建完成!"
```

---

## 🎯 優勢

### 相比現狀的改進

| 問題 | 現狀 | 改進後 |
|------|------|--------|
| **規劃文件太多** | 180+文件,AI助手不知道看哪個 | CONTEXT.md精簡引用關鍵內容 |
| **Sprint文件太簡單** | 只有OVERVIEW,缺乏執行指引 | 7個結構化文件,逐層展開細節 |
| **設計與實際脫節** | AI助手不知道MVP範圍 | PLAN.md明確定義MVP vs Phase 2 |
| **進度難追蹤** | 沒有統一的進度文件 | CHECKLIST.md可標記進度 |
| **問題易遺忘** | 開發過程中的問題沒記錄 | DEV-LOG.md和ISSUES.md系統追蹤 |
| **缺乏回顧** | Sprint結束後無總結 | RETROSPECTIVE.md標準化回顧 |

### 對AI助手的好處

1. **清晰的執行路徑**:
   - OVERVIEW → PLAN → CHECKLIST → 執行
   - 每步都知道下一步該做什麼

2. **Just-in-Time信息**:
   - 不需要在180+文件中搜索
   - CONTEXT.md提供精簡的關鍵信息引用

3. **進度可視化**:
   - CHECKLIST.md提供清晰的任務列表
   - 完成度一目了然

4. **問題可追溯**:
   - DEV-LOG.md記錄開發過程
   - ISSUES.md追蹤問題解決

---

## 🔄 與現有文檔的關係

### /docs (Reference Layer) - 保持不變
- 詳細的規劃文檔
- 架構設計文檔
- 技術規範文檔

**作用**: 作為"知識庫",需要時查閱

### /claudedocs (Execution Layer) - 新結構
- Sprint執行文件 (新的7文件結構)
- 進度追蹤文件
- AI助手指引

**作用**: 作為"執行指南",每日使用

**關係**:
- SPRINT-X-CONTEXT.md **精簡引用** /docs 內容
- SPRINT-X-PLAN.md **提煉** /docs 中的執行要點
- 避免重複,但提供快速訪問路徑

---

## ✅ 下一步行動

### 立即行動 (本週)

1. **創建模板文件夾**:
   ```bash
   mkdir -p claudedocs/templates
   ```

2. **創建標準模板** (7個模板文件):
   - 基於本文檔的結構創建模板

3. **試點Sprint 2**:
   - 使用新框架重新組織Sprint 2文件
   - 驗證是否有效改善AI助手執行效率

### 漸進式推廣 (下週起)

4. **Sprint 3開始使用新框架**

5. **收集反饋**:
   - AI助手使用體驗
   - 團隊使用體驗
   - 持續優化

6. **自動化** (可選):
   - 創建Sprint初始化腳本
   - 自動生成CHECKLIST進度報告

---

## 📝 備註

### 與其他工具的整合 (可選)

- **Jira / Linear**: CHECKLIST.md可以同步到任務管理工具
- **Slack / Teams**: DEV-LOG.md可以生成每日更新
- **GitHub**: ISSUES.md可以關聯GitHub Issues

### 文件大小控制

每個文件建議大小:
- OVERVIEW: <500行
- PLAN: <1000行
- CHECKLIST: <500行
- DEV-LOG: 無限制 (每日追加)
- CONTEXT: <800行
- ISSUES: 無限制
- RETROSPECTIVE: <300行

**總計**: 每個Sprint約3000-4000行文檔,遠少於當前的18萬行

---

**是否採納此方案?**
- [ ] ✅ 採納 - 開始實施
- [ ] 🔄 修改 - 需要調整
- [ ] ❌ 拒絕 - 提出替代方案
