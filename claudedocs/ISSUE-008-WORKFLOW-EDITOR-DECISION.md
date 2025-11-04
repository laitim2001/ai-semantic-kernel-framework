# ISSUE-008: Workflow Editor 範圍決策

**決策日期**: 2025-11-03
**決策者**: Product Owner + Tech Lead (由 Claude Code 執行分析)
**狀態**: ✅ 已決策
**優先級**: 🔴 Critical

---

## 📋 問題摘要

**衝突內容**: Brief.md、UX Design、ADR-012 和 PoC 6 顯示 Workflow Editor 已完成詳細設計和驗證，但 User Stories (mvp-planning.md) 將其標記為「延後到 Phase 2」(US 4.3 高級路由)。

**影響範圍**:
- Epic 4: Multi-Agent Collaboration
- Sprint 規劃（Sprint 分配和時間表）
- 開發資源分配（10-12 個月計劃）
- PoC 6 成果的利用

---

## 📊 關鍵證據分析

### 1. Brief.md 的定義

從 `docs/brief.md` 搜索結果：
- **MVP 範圍**: 「不包含視覺化編排器（Phase 2 功能）」(Line 1305)
- **Phase 1**: 聚焦開發團隊（6 個月）
- **Phase 2+**: 規模化推廣與商業化（月 10+）

**結論**: Brief 明確將視覺化工作流編輯器定義為 **Phase 2** 功能。

---

### 2. MVP Planning (mvp-planning.md)

```yaml
Multi_Agent_Collaboration:
  description: "多 Agent 協作框架"
  stories: [US_4.1, US_4.2]
  note: "US_4.3（高級路由）延後到 Phase 2"
  deliverables:
    - Agent 間通訊機制
    - 基礎協作流程
```

**MVP Phase 1-3 (Month 4-6)** - Module 4:
- ✅ US 4.1: Agent 間通訊機制
- ✅ US 4.2: 基礎協作流程
- ❌ US 4.3: 高級路由（Phase 2）
- ❌ **Workflow Editor 未列入 MVP**

**結論**: MVP Planning 明確 **不包含** Workflow Editor，僅包含基礎協作 API。

---

### 3. ADR-012 工作流編輯器技術選型

**狀態**: ✅ 已接受
**日期**: 2025-11-01
**決策**: 微前端架構 (React Host + Vue Remote) + Module Federation

**技術棧**:
- 主應用 (Host): React 18 + Material-UI (95% 頁面)
- 工作流編輯器 (Remote): Vue 3 + VueFlow (獨立模組)

**實施計劃**: Week 1-10 (約 2.5 個月)
```yaml
Week 1-2: 基礎設施
Week 3-4: VueFlow 畫布核心
Week 5-6: React 主應用整合
Week 7-8: CRDT 協作
Week 9-10: 生產準備
```

**結論**: ADR-012 提供了 **完整的技術設計**，但未明確是否納入 MVP。

---

### 4. UX Design - Workflow Editor V2

**文檔範圍**: 7 個 Part，共 920+ 行
- Part 1: 核心設計理念與大卡片節點系統
- Part 2: 內聯配置與表達式系統
- Part 3: 執行視覺化與數據流
- Part 4: 節點類型庫（28 種節點）
- Part 5: 畫布互動與連接線
- Part 6: 協作、版本控制與技術架構
- Part 7: 響應式、無障礙與驗收

**設計完成度**: **100%** - 生產就緒設計

**開發檢查清單**:
- Phase 1: 基礎設施（2 週）
- Phase 2: 畫布系統（3 週）
- Phase 3: 節點系統（4 週）
- Phase 4: 執行引擎（3 週）
- Phase 5: 協作功能（3 週）
- Phase 6: 優化與測試（3 週）
- Phase 7: 部署與文檔（2 週）
- **總計**: 20 週（約 5 個月）

**結論**: UX Design 假設 Workflow Editor 會被實施，並提供 **完整的設計規範**。

---

### 5. PoC 6 驗證結果

**PoC 6: VueFlow CRDT 驗證報告**（來自 POC-1-6-COMPLETE-VALIDATION-REPORT.md）

| 測試項目 | 目標 | 實測結果 | 狀態 |
|---------|------|---------|------|
| 渲染性能 | >60 FPS | 60 FPS | ✅ 達標 |
| 同步延遲 | <500ms | <200ms | ✅ 超標 |
| 並發用戶 | 3+ | 3+ | ✅ 達標 |
| 衝突解決 | 自動 | Yjs CRDT | ✅ 達標 |

**準備度**: **95%** 🟢 優秀
**狀態**: ✅ 生產就緒

**多用戶並發問題**: ✅ **已修復** (2025-11-01)
- 客戶端前綴 ID 機制
- 雙層去重邏輯 (Sender + Receiver)
- 測試結果: 順序/並發測試全部通過 (30/60 nodes)

**結論**: PoC 6 **完全驗證了 Workflow Editor 的技術可行性**，並達到生產就緒狀態。

---

## 🤔 矛盾點分析

### 矛盾 1: Brief vs UX Design

| 文檔 | 立場 | 證據 |
|------|------|------|
| **Brief.md** | ❌ Phase 2 | "不包含視覺化編排器（Phase 2 功能）" |
| **UX Design** | ✅ 已設計 | 7 個 Part，920+ 行，完整設計 |

**分析**: Brief 說不做，但 UX 團隊已經完成了完整設計。這表明 **UX 團隊提前完成了 Phase 2 的設計工作**。

---

### 矛盾 2: MVP Planning vs ADR-012 + PoC 6

| 文檔 | 立場 | 證據 |
|------|------|------|
| **MVP Planning** | ❌ Phase 2 | US 4.1-4.2 只包含基礎協作 API |
| **ADR-012** | ✅ 技術決策完成 | 微前端架構，10 週實施計劃 |
| **PoC 6** | ✅ 已驗證 | 95% 準備度，生產就緒 |

**分析**: MVP Planning 說延後，但架構師已經完成了技術決策，QA 已經完成了 PoC 驗證。這表明 **技術團隊已經為 Workflow Editor 做好了準備**。

---

### 矛盾 3: 時間估算

| 估算來源 | Phase | 時間 | 說明 |
|---------|-------|------|------|
| **Brief.md** | Phase 2 | Month 10+ | 商業化階段 |
| **MVP Planning** | Phase 2 | 未定義 | 未提供具體時間表 |
| **ADR-012** | - | 10 週 (2.5 月) | 完整實施計劃 |
| **UX Design** | - | 20 週 (5 月) | 包含設計到部署 |

**分析**: 如果納入 MVP，ADR-012 的 10 週計劃可以在 **Month 7-9 (Phase 3)** 或 **Month 4-6 (Phase 2)** 執行。

---

## 💡 決策方案分析

### 方案 A: 納入 MVP (Phase 2 或 Phase 3) ✅ 推薦

#### 優點 ✅

1. **充分利用已完成工作**
   - ✅ UX Design 已完成 (920+ 行，7 個 Part)
   - ✅ ADR-012 技術決策已完成
   - ✅ PoC 6 已驗證 (95% 準備度)
   - **價值**: 避免浪費已投入的 **2-3 週設計 + 1 週 PoC** 工作量

2. **技術風險已降低**
   - ✅ VueFlow + CRDT 技術可行性已驗證
   - ✅ 多用戶並發問題已解決
   - ✅ 微前端架構已設計
   - **價值**: 從高風險創新功能降級為 **低風險實施任務**

3. **競爭優勢顯著**
   - ✅ n8n 風格視覺化編排器（行業標杆）
   - ✅ 實時多用戶協作（差異化能力）
   - ✅ 28 種節點類型（完整節點庫）
   - **價值**: 提升產品競爭力，吸引早期用戶

4. **符合 BMAD 原則**
   - 「如果 UX 和技術準備好了，為什麼要等？」
   - 「MVP 是最小 *可行* 產品，不是最小 *功能* 產品」
   - **價值**: 加速產品迭代，快速驗證市場

#### 缺點 ❌

1. **時間壓力**
   - 增加 **10 週 (2.5 個月)** 開發時間
   - MVP 從 10-12 個月延長至 **12-14 個月**

2. **資源需求**
   - 需要 Vue 3 技能（團隊可能不熟悉）
   - 需要 WebSocket / Yjs 專家（CRDT 實施）

3. **技術複雜度**
   - 微前端架構增加系統複雜度
   - React + Vue 混合維護成本

#### 建議納入時間點

| 時間點 | Phase | 月份 | 優點 | 缺點 |
|--------|-------|------|------|------|
| **Option A1: Phase 2** | Core Features | Month 4-6 | 早期交付，快速驗證 | 時間緊張，資源競爭 |
| **Option A2: Phase 3** | Differentiation | Month 7-9 | 時間充裕，專注開發 | 較晚交付，錯失早期反饋 |

**推薦**: **Option A2 - Phase 3 (Month 7-9)**
- Week 1-4: 基礎設施 + 畫布系統
- Week 5-8: 節點系統 + 執行引擎
- Week 9-10: 協作功能 + 生產準備

**理由**:
- Phase 2 已滿載（Code Interpreter, Knowledge RAG, Multi-Agent）
- Phase 3 原有任務（Persona Framework, Text-to-SQL）可與 Workflow Editor 並行
- 給予團隊充裕時間學習 Vue 3 和微前端架構

---

### 方案 B: 延後到 Phase 2 (Post-MVP) ❌ 不推薦

#### 優點 ✅

1. **MVP 範圍控制**
   - 遵循原始 Brief 定義
   - 避免 MVP 膨脹

2. **專注核心功能**
   - 確保 Phase 1-2 核心模組質量
   - 降低 MVP 失敗風險

#### 缺點 ❌

1. **浪費已完成工作**
   - ❌ UX Design (920+ 行) 延後使用
   - ❌ ADR-012 技術決策延後實施
   - ❌ PoC 6 驗證成果延後應用
   - **損失**: 2-3 週設計 + 1 週 PoC 工作量

2. **錯失競爭優勢**
   - ❌ 無視覺化編排器，競爭力下降
   - ❌ 僅提供 API，對非技術用戶不友好
   - **風險**: Phase 1 內部驗證效果受限

3. **技術風險累積**
   - 延後到 Phase 2 後，團隊已遺忘 Vue/VueFlow 技能
   - PoC 6 代碼可能過時，需要重新驗證

#### 決策結果: ❌ **不推薦**

---

## ✅ 最終決策

### 決策: **方案 A2 - 納入 MVP Phase 3 (Month 7-9)**

#### 決策理由

1. **充分利用已完成資產**
   - UX Design (100% 完成)
   - ADR-012 (技術決策完成)
   - PoC 6 (95% 準備度，生產就緒)

2. **技術風險可控**
   - PoC 6 已驗證核心技術可行性
   - 多用戶並發問題已解決
   - ADR-012 提供清晰實施路徑

3. **時間安排合理**
   - Phase 3 (Month 7-9) 時間充裕
   - 與 Persona Framework, Text-to-SQL 並行開發
   - 10 週開發時間符合 ADR-012 估算

4. **競爭優勢明顯**
   - n8n 風格視覺化編排器
   - 實時多用戶協作
   - 28 種節點類型完整庫

5. **符合 BMAD 原則**
   - 「設計和技術準備好了，就執行」
   - 「MVP 是可行產品，不是最小功能產品」

---

### 實施計劃

#### Phase 3 (Month 7-9) 調整

**原計劃**:
- Month 7: Persona Framework 基礎 (US 7.1-7.3) - 30-35 SP
- Month 8: Text-to-SQL Schema & 生成 (US 8.1-8.2) - 40-45 SP
- Month 9: Text-to-SQL 權限 & 結果展示 (US 8.3-8.4) - 35-40 SP

**調整後**:
- Month 7: Persona Framework (30-35 SP) + **Workflow Editor 基礎** (Week 1-4, ~20 SP)
- Month 8: Text-to-SQL (40-45 SP) + **Workflow Editor 核心** (Week 5-8, ~25 SP)
- Month 9: Text-to-SQL 完成 (35-40 SP) + **Workflow Editor 協作** (Week 9-10, ~15 SP)

**新增 User Story**:
- **US 4.3: Workflow Editor (Visual Orchestration)** - 60 SP
  - Sub-task 4.3.1: 微前端架構設置 (Module Federation) - 15 SP
  - Sub-task 4.3.2: VueFlow 畫布與節點系統 - 25 SP
  - Sub-task 4.3.3: CRDT 實時協作 - 15 SP
  - Sub-task 4.3.4: 集成測試與生產準備 - 5 SP

**Phase 3 總計**: 105-120 SP (原計劃) + 60 SP (Workflow Editor) = **165-180 SP**

**資源調整**:
- 增加 1 名 Vue 3 前端開發者（或培訓現有開發者）
- Tech Lead 監督微前端架構實施
- QA 負責 CRDT 協作測試

---

### MVP 時間表更新

| Phase | 原時間 | 新時間 | 變化 | Story Points |
|-------|--------|--------|------|-------------|
| Phase 1 | Month 1-3 | Month 1-3 | - | 90-105 SP |
| Phase 2 | Month 4-6 | Month 4-6 | - | 115-130 SP |
| Phase 3 | Month 7-9 | Month 7-9 | **+60 SP** | 165-180 SP |
| Phase 4 | Month 10-12 | Month 10-12 | - | 75-90 SP |
| **總計** | **10-12 月** | **11-13 月** | **+1 月** | **360-410 SP** |

**結論**: MVP 從 10-12 個月延長至 **11-13 個月**，增加約 1 個月。

---

### 風險與緩解措施

#### 風險 1: Vue 3 技能差距 🟡 中等風險

**風險描述**: 團隊可能不熟悉 Vue 3 Composition API

**緩解措施**:
- ✅ Week 1-2: Vue 3 + VueFlow 培訓（Tech Lead 或外部講師）
- ✅ Pair Programming: Vue 專家 + React 開發者配對
- ✅ PoC 6 代碼作為參考範本
- ✅ ADR-012 提供詳細技術指導

**監控指標**:
- 培訓後評估（Week 2 結束）
- 第一個節點組件完成時間（Week 4）

---

#### 風險 2: 微前端架構複雜度 🟡 中等風險

**風險描述**: React + Vue 混合架構增加維護成本

**緩解措施**:
- ✅ 使用 ADR-012 的 Module Federation 配置
- ✅ 標準化通信接口（TypeScript 定義）
- ✅ Event Bus 封裝（mitt 庫）
- ✅ 統一設計令牌（CSS Variables）

**監控指標**:
- Event Bus 延遲 <50ms
- 狀態同步錯誤率 <0.1%

---

#### 風險 3: 時間壓力 🟢 低風險

**風險描述**: Phase 3 增加 60 SP，可能延遲交付

**緩解措施**:
- ✅ Persona Framework 和 Text-to-SQL 優先級不變
- ✅ Workflow Editor 可獨立開發（微前端架構）
- ✅ 如需要，可延後 Workflow Editor 協作功能到 Phase 4

**監控指標**:
- 每週 Story Points 完成率
- Sprint Burndown Chart

---

## 📋 行動計劃

### 立即行動 (Week 0 - 本週)

- [x] **決策完成**: Workflow Editor 納入 MVP Phase 3
- [ ] **更新文檔**:
  - [ ] 更新 `mvp-planning.md` (增加 US 4.3)
  - [ ] 更新 `sprint-allocation.md` (調整 Phase 3)
  - [ ] 更新 `COMPLETE-DEVELOPMENT-PLAN.md` (調整時間表)
- [ ] **團隊溝通**:
  - [ ] Product Owner 確認決策
  - [ ] Tech Lead 確認技術方案
  - [ ] 開發團隊確認資源分配

### Week 1-2 (Brief 切分期間)

- [ ] **技術準備**:
  - [ ] Vue 3 + VueFlow 培訓材料準備
  - [ ] 微前端架構 Demo 搭建
  - [ ] PoC 6 代碼遷移到 Monorepo

### Phase 3 Week 1-4 (Month 7)

- [ ] **基礎設施**:
  - [ ] Monorepo 結構設置 (pnpm + Turborepo)
  - [ ] Module Federation 配置 (Vite + Webpack)
  - [ ] 共享設計令牌和 Event Bus
  - [ ] VueFlow 畫布基礎 (網格、縮放、平移)

### Phase 3 Week 5-8 (Month 8)

- [ ] **節點系統**:
  - [ ] 28 種節點類型實現
  - [ ] 內聯配置面板
  - [ ] 表達式系統 (`{{$json}}` 語法)
  - [ ] 節點拖放和連接

### Phase 3 Week 9-10 (Month 9)

- [ ] **協作功能**:
  - [ ] Yjs CRDT 整合
  - [ ] WebSocket 實時通信
  - [ ] 多用戶光標顯示
  - [ ] 集成測試與生產準備

---

## 📊 成功指標

### 技術指標

| 指標 | 目標 | 測試方法 |
|------|------|---------|
| **渲染性能** | 60 FPS (1000+ nodes) | Chrome DevTools Performance |
| **首屏加載** | <2s (3G 網絡) | Lighthouse |
| **模塊加載** | <500ms | Network Tab |
| **同步延遲** | <200ms | WebSocket Metrics |
| **並發用戶** | 3+ 無衝突 | Playwright 多用戶測試 |

### 質量指標

| 指標 | 目標 | 測試方法 |
|------|------|---------|
| **單元測試** | 覆蓋率 >80% | Vitest Coverage |
| **E2E 測試** | 核心流程 100% | Playwright |
| **TypeScript** | 無 `any` 類型 | TSC Strict Mode |
| **無障礙** | WCAG 2.1 AA | Axe DevTools |
| **瀏覽器兼容** | Chrome 90+, Firefox 88+, Safari 14+ | BrowserStack |

### 用戶體驗指標

| 指標 | 目標 | 測試方法 |
|------|------|---------|
| **節點創建** | <3 次點擊 | 用戶測試 |
| **工作流保存** | 自動保存 <30s | 計時器測試 |
| **錯誤恢復** | 無數據丟失 | 崩潰恢復測試 |
| **學習曲線** | <30 分鐘上手 | 新用戶測試 |

---

## 🎯 驗收標準

### MVP Phase 3 完成標準

- [ ] **功能完整性**:
  - [ ] 28 種節點類型全部實現
  - [ ] 內聯配置面板運作正常
  - [ ] 表達式系統支持 `{{$json}}` 語法
  - [ ] 實時多用戶協作無衝突

- [ ] **性能達標**:
  - [ ] 60 FPS 渲染 (1000+ 節點)
  - [ ] <2s 首屏加載
  - [ ] <200ms 同步延遲

- [ ] **質量保證**:
  - [ ] 單元測試覆蓋率 >80%
  - [ ] E2E 測試核心流程 100%
  - [ ] WCAG 2.1 AA 合規

- [ ] **文檔完整**:
  - [ ] 用戶操作手冊
  - [ ] 開發者 API 文檔
  - [ ] 部署指南

---

## 📝 相關文檔更新清單

| 文檔 | 更新內容 | 負責人 | 狀態 |
|------|---------|--------|------|
| `mvp-planning.md` | 增加 US 4.3 (Workflow Editor) - 60 SP | PO | ⏳ 待更新 |
| `sprint-allocation.md` | 調整 Phase 3 Story Points (165-180 SP) | PO | ⏳ 待更新 |
| `COMPLETE-DEVELOPMENT-PLAN.md` | 更新時間表 (11-13 個月) | PO | ⏳ 待更新 |
| `modules/module-04-multi-agent.md` | 增加 US 4.3 詳細描述 | Tech Lead | ⏳ 待更新 |
| `brief.md` | 更新 MVP 範圍定義 | PO | ⏳ 待切分後更新 |

---

## 🔗 參考文檔

### 已分析文檔

1. **`docs/brief.md`** (206KB)
   - MVP 範圍定義: Phase 1-2 不包含視覺化編排器
   - Phase 2+ 商業化階段

2. **`docs/user-stories/mvp-planning.md`** (890 行)
   - US 4.1-4.2 基礎協作 API
   - US 4.3 高級路由標記為 Phase 2

3. **`docs/architecture/ADR-012-workflow-editor-technology.md`** (900+ 行)
   - 技術決策: 微前端架構 + Module Federation
   - 實施計劃: 10 週 (Week 1-10)

4. **`docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md`** (920+ 行)
   - 完整 UX 設計 (Part 1-7)
   - 28 種節點類型、協作功能、無障礙支持

5. **`poc-projects/POC-1-6-COMPLETE-VALIDATION-REPORT.md`** (796 行)
   - PoC 6 驗證結果: 95% 準備度，生產就緒
   - 多用戶並發問題已解決

### 需要更新的文檔

- `docs/user-stories/mvp-planning.md`
- `docs/user-stories/sprints/sprint-allocation.md`
- `docs/COMPLETE-DEVELOPMENT-PLAN.md`
- `docs/user-stories/modules/module-04-multi-agent.md`
- `docs/brief.md` (切分後)

---

## 🎉 總結

### 決策摘要

✅ **Workflow Editor 納入 MVP Phase 3 (Month 7-9)**

**核心理由**:
1. 充分利用已完成的 UX 設計、技術決策和 PoC 驗證
2. 技術風險已降低（PoC 6 達 95% 準備度）
3. 競爭優勢明顯（n8n 風格視覺化編排器）
4. 符合 BMAD 原則（設計和技術準備好了，就執行）

**時間成本**: 增加 **1 個月** (從 10-12 月延長至 11-13 月)

**資源需求**: 增加 1 名 Vue 3 前端開發者或培訓現有開發者

**風險級別**: 🟡 中等（Vue 3 技能差距、微前端複雜度）

**預期價值**: 🟢 高（視覺化編排器、實時協作、28 種節點類型）

---

**決策者簽署**:
- [ ] Product Owner: _______________  日期: _______________
- [ ] Tech Lead: _______________  日期: _______________
- [ ] Scrum Master: _______________  日期: _______________

---

**文檔版本**: 1.0.0
**創建日期**: 2025-11-03
**最後更新**: 2025-11-03
**維護者**: Claude Code (AI Assistant)
**狀態**: ✅ 決策完成，待 PO 確認
