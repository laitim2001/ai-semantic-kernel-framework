# 範圍變更申請 (Scope Change Request)

**文檔編號**: SCR-[YYYY-MM-DD]-[序號]
**申請日期**: [YYYY-MM-DD]
**申請人**: [姓名/角色]
**關聯 Sprint**: [Sprint X]
**關聯 User Story**: [US X.X - 標題]

---

## 1. 變更摘要 (Change Summary)

### 變更類型 (Change Type)
- [ ] 範圍變更 (Scope Change)
- [ ] 技術決策 (Technical Decision)
- [ ] 需求變更 (Requirement Change)
- [ ] 時程調整 (Schedule Adjustment)
- [ ] 資源調整 (Resource Adjustment)

### 影響等級 (Impact Level)
- [ ] 🟢 **低** (Low): 不影響 Sprint 時程和目標,僅內部技術調整
- [ ] 🟡 **中** (Medium): 輕微影響時程 (+1-2 days),但整體可控
- [ ] 🔴 **高** (High): 顯著影響時程 (+3+ days) 或範圍,需要調整 Sprint 目標
- [ ] 🔥 **緊急** (Critical): 嚴重影響交付,需要立即決策和行動

### 變更簡述 (Brief Description)
[用 1-2 句話描述變更內容]

---

## 2. 變更原因 (Rationale)

### 業務原因 (Business Justification)
[為什麼需要這個變更?對業務目標的影響?]

### 技術原因 (Technical Justification)
[技術架構、依賴關係、設計考慮等]

### 其他驅動因素 (Other Drivers)
- [ ] 客戶需求
- [ ] 市場變化
- [ ] 技術風險
- [ ] 團隊效率
- [ ] 架構優化
- [ ] 其他: [說明]

---

## 3. 詳細變更內容 (Detailed Change Description)

### 變更前狀態 (Current State)
```
[描述當前的計劃、範圍、實現方式]
```

### 變更後狀態 (Proposed State)
```
[描述變更後的計劃、範圍、實現方式]
```

### 具體變更項目 (Specific Changes)
1. **User Story 調整**:
   - 新增: [US X.X]
   - 修改: [US X.X]
   - 移除: [US X.X]

2. **功能範圍變更**:
   - 新增功能: [列表]
   - 修改功能: [列表]
   - 移除功能: [列表]

3. **技術實現變更**:
   - 架構調整: [說明]
   - 技術選型: [說明]
   - API 變更: [說明]

---

## 4. 影響分析 (Impact Analysis)

### 時程影響 (Schedule Impact)
- **預估時間變化**: +/- [X] days
- **關鍵里程碑影響**: [是/否]
- **Sprint 目標影響**: [說明]
- **交付日期影響**: [說明]

### 資源影響 (Resource Impact)
- **人力需求變化**: [增加/減少] [X] 人天
- **技術資源需求**: [新增/變更的工具、環境、服務]
- **預算影響**: [如有]

### Story Points 影響 (Story Points Impact)
- **原計劃 SP**: [X] SP
- **變更後 SP**: [Y] SP
- **SP 變化**: +/- [Z] SP
- **Sprint 總 SP 影響**: [說明]

### 相依性影響 (Dependency Impact)
- **影響的 User Stories**: [US X.X, US Y.Y]
- **影響的系統模組**: [列表]
- **影響的團隊成員**: [列表]
- **外部相依性**: [如有]

### 風險評估 (Risk Assessment)
| 風險項目 | 可能性 | 影響程度 | 緩解措施 |
|---------|--------|---------|---------|
| [風險 1] | 高/中/低 | 高/中/低 | [措施] |
| [風險 2] | 高/中/低 | 高/中/低 | [措施] |

### 正面影響 (Positive Impact)
1. [列出變更帶來的好處]
2. [...]

### 負面影響 (Negative Impact)
1. [列出變更帶來的挑戰]
2. [...]

### 中性影響 (Neutral Impact)
1. [列出需要注意但無明顯正負面的影響]
2. [...]

---

## 5. 替代方案 (Alternatives Considered)

### 方案 A: [方案名稱]
- **描述**: [說明]
- **優點**: [列表]
- **缺點**: [列表]
- **選擇/不選擇原因**: [說明]

### 方案 B: [方案名稱]
- **描述**: [說明]
- **優點**: [列表]
- **缺點**: [列表]
- **選擇/不選擇原因**: [說明]

### 不做任何變更 (Do Nothing)
- **風險**: [說明維持現狀的風險]
- **機會成本**: [說明錯過的機會]

---

## 6. 實施計劃 (Implementation Plan)

### 實施步驟 (Implementation Steps)
1. **Phase 1**: [階段名稱] - [預估時間]
   - [ ] [具體任務 1]
   - [ ] [具體任務 2]

2. **Phase 2**: [階段名稱] - [預估時間]
   - [ ] [具體任務 1]
   - [ ] [具體任務 2]

### 關鍵里程碑 (Key Milestones)
| 里程碑 | 日期 | 責任人 | 驗收標準 |
|-------|------|--------|---------|
| [里程碑 1] | [日期] | [姓名] | [標準] |
| [里程碑 2] | [日期] | [姓名] | [標準] |

### 資源分配 (Resource Allocation)
- **Tech Lead**: [工作內容] - [人天]
- **Backend Dev**: [工作內容] - [人天]
- **Frontend Dev**: [工作內容] - [人天]
- **QA**: [工作內容] - [人天]

### 驗收標準 (Acceptance Criteria)
- [ ] [標準 1]
- [ ] [標準 2]
- [ ] [標準 3]

---

## 7. 溝通計劃 (Communication Plan)

### Stakeholder 通知 (Stakeholder Notification)
| Stakeholder | 角色 | 通知方式 | 通知時間 |
|------------|------|---------|---------|
| Product Owner | PO | Email + Meeting | [日期] |
| Scrum Master | SM | Slack + Stand-up | [日期] |
| Tech Lead | TL | 1-on-1 Discussion | [日期] |
| Development Team | Dev | Team Meeting | [日期] |
| Project Manager | PM | Status Report | [日期] |

### 會議安排 (Meeting Schedule)
- **變更審查會議**: [日期/時間]
- **參與者**: [列表]
- **議程**: [討論重點]

### 文檔更新 (Documentation Updates)
- [ ] Sprint Backlog
- [ ] Sprint Overview
- [ ] User Story 文檔
- [ ] 技術設計文檔
- [ ] API 文檔
- [ ] 測試計劃
- [ ] PROJECT-STATUS-REPORT.md
- [ ] CHANGE-LOG.md

---

## 8. 審批流程 (Approval Workflow)

### 審批層級 (Approval Levels)

#### 🟢 低影響變更 (Low Impact)
- **必要審批**: Tech Lead
- **通知**: Scrum Master
- **流程**: 簡化審批,24小時內完成

#### 🟡 中影響變更 (Medium Impact)
- **必要審批**: Tech Lead → Product Owner → Scrum Master
- **通知**: Development Team, Project Manager
- **流程**: 正式審批,2-3 個工作天

#### 🔴 高影響變更 (High Impact)
- **必要審批**: Tech Lead → Product Owner → Scrum Master → Project Manager
- **會議**: Change Control Board (CCB) 會議
- **流程**: 完整評估,3-5 個工作天

#### 🔥 緊急變更 (Critical)
- **必要審批**: 所有 Stakeholders
- **會議**: 緊急 CCB 會議
- **流程**: 快速評估,24小時內完成

### 審批記錄 (Approval Record)

| 審批者 | 角色 | 決策 | 意見 | 日期 | 簽名 |
|-------|------|------|------|------|------|
| [姓名] | Tech Lead | ✅ 批准 / ❌ 拒絕 / ⏸️ 保留 | [意見] | [日期] | [簽名] |
| [姓名] | Product Owner | ✅ 批准 / ❌ 拒絕 / ⏸️ 保留 | [意見] | [日期] | [簽名] |
| [姓名] | Scrum Master | ✅ 批准 / ❌ 拒絕 / ⏸️ 保留 | [意見] | [日期] | [簽名] |
| [姓名] | Project Manager | ✅ 批准 / ❌ 拒絕 / ⏸️ 保留 | [意見] | [日期] | [簽名] |

### 最終決策 (Final Decision)
- **狀態**: [ ] ✅ 已批准 / [ ] ❌ 已拒絕 / [ ] ⏸️ 延期決定 / [ ] 🔄 需要修改後重審
- **決策日期**: [YYYY-MM-DD]
- **生效日期**: [YYYY-MM-DD]
- **決策說明**: [最終決策的理由和條件]

---

## 9. 追蹤與監控 (Tracking and Monitoring)

### 實施狀態追蹤 (Implementation Status)
| 階段 | 狀態 | 開始日期 | 完成日期 | 負責人 | 備註 |
|-----|------|---------|---------|--------|------|
| Phase 1 | 🔄 進行中 | [日期] | [日期] | [姓名] | [備註] |
| Phase 2 | ⏳ 待開始 | [日期] | [日期] | [姓名] | [備註] |

### 風險監控 (Risk Monitoring)
| 風險項目 | 當前狀態 | 緩解措施進度 | 負責人 | 更新日期 |
|---------|---------|-------------|--------|---------|
| [風險 1] | 🟢/🟡/🔴 | [進度] | [姓名] | [日期] |

### 里程碑達成 (Milestone Achievement)
- [ ] 里程碑 1: [名稱] - [預計日期]
- [ ] 里程碑 2: [名稱] - [預計日期]
- [ ] 里程碑 3: [名稱] - [預計日期]

---

## 10. 事後檢討 (Post-Implementation Review)

### 實際結果 (Actual Results)
- **實際完成時間**: [日期]
- **實際 SP 消耗**: [X] SP
- **實際資源消耗**: [人天]
- **交付成果**: [列表]

### 與預期比較 (Comparison with Estimates)
| 指標 | 預估 | 實際 | 差異 | 差異原因 |
|-----|------|------|------|---------|
| 時間 (days) | [X] | [Y] | [Z] | [說明] |
| Story Points | [X] | [Y] | [Z] | [說明] |
| 資源 (人天) | [X] | [Y] | [Z] | [說明] |

### 經驗教訓 (Lessons Learned)

#### 做得好的地方 (What Went Well)
1. [列出成功的地方]
2. [...]

#### 需要改進的地方 (What Could Be Improved)
1. [列出可改進的地方]
2. [...]

#### 未來建議 (Recommendations for Future)
1. [對類似變更的建議]
2. [對流程的改進建議]

### 知識沉澱 (Knowledge Capture)
- **技術學習**: [新技術、新方法的學習心得]
- **流程優化**: [對變更流程的改進建議]
- **最佳實踐**: [可複用的經驗和模式]

---

## 附錄 (Appendix)

### 相關文檔 (Related Documents)
- [CHANGE-LOG.md](./CHANGE-LOG.md): 變更記錄總表
- [Sprint Overview](../2-sprints/sprint-X/SPRINT-X-OVERVIEW.md): Sprint 概覽
- [User Story 文檔](../../docs/user-stories/modules/module-XX.md): 詳細需求
- [技術設計文檔](../../docs/technical-design/): 架構和設計

### 參考資料 (References)
1. [相關文獻、標準、最佳實踐]
2. [...]

### 附件 (Attachments)
- [ ] 技術設計圖
- [ ] 架構圖
- [ ] 資料流程圖
- [ ] API 規格
- [ ] 測試計劃
- [ ] 其他: [說明]

---

## 變更歷史 (Change History)

| 版本 | 日期 | 修改者 | 修改內容 |
|-----|------|--------|---------|
| 1.0 | [日期] | [姓名] | 初始版本 |
| 1.1 | [日期] | [姓名] | [修改說明] |

---

**備註**:
- 本模板根據變更影響等級,可以選擇性填寫部分章節
- 🟢 低影響變更: 可簡化填寫第 3, 4, 5, 7, 8 章節
- 🟡 中影響變更: 建議完整填寫所有章節
- 🔴🔥 高影響/緊急變更: 必須完整填寫所有章節,並附上詳細附件

**模板版本**: 1.0.0
**最後更新**: 2025-12-11
**維護者**: Scrum Master
