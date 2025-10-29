# Definition of Done (DoD)

**Semantic Kernel Agentic Framework - Definition of Done**

**版本**: 1.0.0
**日期**: 2025-10-28
**適用範圍**: 所有 User Stories

[返回總覽](../README.md)

---

## 📋 什麼是 Definition of Done (DoD)？

**Definition of Done** 是一個 checklist，定義了什麼時候可以認為一個 User Story 已經「完成」。只有當 User Story 滿足所有 DoD 條件時，才能被標記為 "Done" 並在 Sprint Review 中演示。

---

## ✅ 通用 DoD (所有 User Stories)

### 1. 功能完整性

- [ ] **所有驗收標準已滿足**: User Story 中列出的所有「必須項」驗收標準均已實現並驗證
- [ ] **BDD 場景通過**: 所有 Given-When-Then 場景均可演示並驗證
- [ ] **邊界條件處理**: 正常流程、錯誤處理、邊界條件均已實現
- [ ] **業務邏輯正確**: 業務規則按照需求正確實現
- [ ] **數據完整性**: 數據驗證、約束、關聯關係正確

### 2. 代碼質量

- [ ] **代碼審查通過**: 至少 1 名團隊成員進行 Code Review 並批准
- [ ] **編碼規範遵循**: 符合項目 Coding Standards (C# 12, TypeScript 5)
- [ ] **無明顯技術債**: 沒有 TODO 或 FIXME 注釋（除非有 JIRA ticket）
- [ ] **可讀性良好**: 代碼邏輯清晰，適當的註解和文檔
- [ ] **重複代碼重構**: 遵循 DRY 原則，無明顯重複代碼

### 3. 測試覆蓋

- [ ] **單元測試**: 覆蓋率 ≥80%，核心業務邏輯 100%
- [ ] **集成測試**: 關鍵 API Endpoint 和數據庫操作有集成測試
- [ ] **E2E 測試**: 關鍵用戶流程有端到端測試（P0 stories 必須）
- [ ] **測試通過**: 所有測試（Unit, Integration, E2E）均通過
- [ ] **測試可維護**: 測試代碼遵循 AAA (Arrange-Act-Assert) 模式

### 4. 安全性

- [ ] **RBAC 權限檢查**: 實現並測試角色權限控制
- [ ] **輸入驗證**: 所有用戶輸入經過驗證和淨化
- [ ] **XSS/CSRF 防護**: 前端和後端均實現相應防護
- [ ] **敏感數據保護**: 密碼加密、PII 遮罩、審計日誌
- [ ] **SQL Injection 防護**: 使用參數化查詢或 ORM

### 5. 性能

- [ ] **性能標準達成**: 滿足 User Story 中定義的性能指標
- [ ] **無明顯性能問題**: 無明顯的內存洩漏、N+1 查詢、阻塞操作
- [ ] **資源使用合理**: CPU、內存、網絡使用在可接受範圍
- [ ] **緩存策略**: 合理使用緩存（Redis, Browser Cache）
- [ ] **數據庫優化**: 適當的索引、查詢優化

### 6. 文檔

- [ ] **API 文檔**: API Endpoint 有 OpenAPI/Swagger 文檔
- [ ] **代碼註解**: 複雜邏輯有清晰的註解說明
- [ ] **README 更新**: 如有新功能或配置，更新 README
- [ ] **變更日誌**: 更新 CHANGELOG.md (重要變更)
- [ ] **用戶文檔**: 面向用戶的功能有用戶手冊更新

### 7. 部署準備

- [ ] **CI/CD 通過**: GitHub Actions Pipeline 構建成功
- [ ] **環境配置**: Development, Staging 環境配置正確
- [ ] **數據庫遷移**: 如有 Schema 變更，提供遷移腳本
- [ ] **依賴管理**: NuGet/npm 依賴正確聲明
- [ ] **無破壞性變更**: 不破壞現有功能或向後兼容

### 8. 驗收

- [ ] **Product Owner 驗收**: PO 確認功能符合預期
- [ ] **UI/UX 驗收**: UI/UX Designer 確認界面符合設計
- [ ] **QA 驗收**: QA 測試通過（如有專職 QA）
- [ ] **演示準備**: 可以在 Sprint Review 中演示

---

## 🎯 特定類型 DoD

### 前端 User Stories 額外 DoD

- [ ] **響應式設計**: 在 Desktop, Tablet, Mobile 上顯示正常
- [ ] **瀏覽器兼容**: Chrome, Firefox, Safari, Edge 最新版本測試通過
- [ ] **可訪問性**: WCAG 2.1 AA 標準（鍵盤導航、屏幕閱讀器）
- [ ] **UI 組件一致性**: 使用 Design System 組件庫
- [ ] **Loading 狀態**: 異步操作有 Loading 指示器
- [ ] **錯誤處理 UI**: 友好的錯誤提示和恢復引導

### 後端 User Stories 額外 DoD

- [ ] **API 版本控制**: 使用 /api/v1/ 格式
- [ ] **HTTP 狀態碼正確**: 2xx, 4xx, 5xx 使用正確
- [ ] **錯誤響應標準化**: 統一的錯誤響應格式
- [ ] **日誌記錄**: 關鍵操作有結構化日誌（Serilog）
- [ ] **OpenTelemetry 集成**: 分布式追蹤和 metrics 收集
- [ ] **健康檢查**: Health Check Endpoint (/health)

### Database User Stories 額外 DoD

- [ ] **Schema 文檔**: 表結構和關係圖更新
- [ ] **索引優化**: 關鍵查詢有適當索引
- [ ] **遷移腳本**: Up/Down Migration 腳本可執行
- [ ] **數據備份**: 關鍵數據有備份策略
- [ ] **約束和觸發器**: 數據完整性約束正確

### 安全相關 User Stories 額外 DoD

- [ ] **威脅建模**: 完成 STRIDE 威脅建模
- [ ] **滲透測試**: 關鍵功能通過滲透測試（P0 stories）
- [ ] **安全審計**: 代碼無明顯安全漏洞（SonarQube）
- [ ] **合規性**: 符合 GDPR, SOC 2 要求（如適用）
- [ ] **審計日誌**: 敏感操作有審計日誌記錄

### Performance-Critical User Stories 額外 DoD

- [ ] **負載測試**: 通過 k6 負載測試（指定並發和 RPS）
- [ ] **性能基準**: 性能指標記錄到監控系統
- [ ] **緩存策略**: 明確緩存策略和失效機制
- [ ] **數據庫查詢優化**: EXPLAIN ANALYZE 驗證查詢計劃
- [ ] **CDN 配置**: 靜態資源通過 CDN 分發

---

## 🚫 常見「未完成」的情況

以下情況 **不能** 標記為 Done:

❌ **功能未完全實現**: 「90% 完成」不是 Done，100% 才是
❌ **測試失敗**: 任何測試失敗都不能標記為 Done
❌ **代碼未合併**: 代碼在 feature branch 未合併到 develop
❌ **PO 未驗收**: Product Owner 未確認功能符合預期
❌ **有 Blocker Bug**: 發現 P0/P1 Bug 未修復
❌ **文檔缺失**: 缺少必要的 API 文檔或用戶手冊
❌ **性能不達標**: 未滿足性能指標
❌ **安全漏洞**: 存在已知安全漏洞未修復

---

## 📊 DoD Checklist 使用流程

### Sprint 開始

1. **Review DoD**: 團隊回顧 DoD 標準，確保理解一致
2. **Story 分解**: 將 User Story 分解為可驗證的 Tasks
3. **DoD 確認**: 確認特定 Story 是否有額外 DoD 要求

### 開發過程中

1. **持續驗證**: 開發過程中持續檢查 DoD 條件
2. **自我檢查**: 開發者完成後自我檢查 DoD Checklist
3. **同行評審**: Code Review 時檢查 DoD 條件

### Sprint 結束前

1. **DoD 驗證**: Sprint 結束前驗證所有 Story 的 DoD
2. **PO 驗收**: Product Owner 驗收所有 Done Stories
3. **Sprint Review**: 演示符合 DoD 的功能

---

## 🔄 DoD 演進

DoD 不是固定不變的，應該隨著團隊成熟度和項目需求演進：

### Phase 1 (Sprint 1-6)

- 基礎 DoD: 功能完整、測試通過、代碼審查
- 逐步引入更多質量標準

### Phase 2 (Sprint 7-12)

- 增強 DoD: 性能測試、安全審計、文檔完整性
- 提升測試覆蓋率要求 (70% → 80%)

### Phase 3 (Sprint 13-18)

- 嚴格 DoD: E2E 測試、負載測試、合規性驗證
- 引入自動化質量 Gate

### 持續改進

- **Retrospective**: 每個 Sprint 回顧 DoD 是否合適
- **團隊共識**: DoD 變更需要團隊共識
- **文檔更新**: DoD 變更及時更新本文檔

---

## 📚 DoD 與其他概念的關係

### DoD vs Acceptance Criteria

| 概念 | 範圍 | 定義者 | 示例 |
|------|------|--------|------|
| **Acceptance Criteria** | 特定 User Story | Product Owner | "Agent 創建 <5 秒完成" |
| **Definition of Done** | 所有 User Stories | 團隊共同定義 | "測試覆蓋率 ≥80%" |

### DoD vs Definition of Ready (DoR)

| 概念 | 時間點 | 目的 |
|------|--------|------|
| **Definition of Ready** | Sprint Planning 前 | 確保 Story 可以開始開發 |
| **Definition of Done** | Sprint 結束前 | 確保 Story 完成並可交付 |

---

## 🎯 質量標準總結

```yaml
代碼質量:
  測試覆蓋率: ≥80%
  Code Review: 必須
  編碼規範: 遵循
  技術債: 無明顯遺留

安全性:
  RBAC: 已實現
  輸入驗證: 所有輸入
  XSS/CSRF: 已防護
  敏感數據: 已保護

性能:
  API響應: <200ms (P95)
  頁面加載: <1s
  數據庫查詢: 已優化
  資源使用: 合理

文檔:
  API文檔: OpenAPI/Swagger
  代碼註解: 複雜邏輯
  用戶手冊: 面向用戶功能
  變更日誌: 重要變更

部署:
  CI/CD: 通過
  環境配置: 正確
  遷移腳本: 可執行
  向後兼容: 無破壞

驗收:
  PO驗收: 通過
  UI/UX驗收: 通過
  QA測試: 通過
  演示準備: 可演示
```

---

## 📋 DoD Checklist Template

複製以下模板到 JIRA/Azure DevOps Ticket 中使用：

```markdown
## Definition of Done Checklist

### 功能完整性
- [ ] 所有驗收標準已滿足
- [ ] BDD 場景通過
- [ ] 邊界條件處理
- [ ] 業務邏輯正確

### 代碼質量
- [ ] Code Review 通過 (Reviewer: ______)
- [ ] 編碼規範遵循
- [ ] 無技術債遺留
- [ ] 代碼可讀性良好

### 測試覆蓋
- [ ] 單元測試覆蓋率 ≥80% (實際: ___%)
- [ ] 集成測試通過
- [ ] E2E 測試通過 (如適用)

### 安全性
- [ ] RBAC 權限檢查
- [ ] 輸入驗證
- [ ] XSS/CSRF 防護

### 性能
- [ ] 性能標準達成 (API <200ms: ___ms)
- [ ] 無明顯性能問題

### 文檔
- [ ] API 文檔更新
- [ ] 代碼註解完整
- [ ] 用戶文檔更新 (如需要)

### 部署
- [ ] CI/CD Pipeline 通過
- [ ] 環境配置正確
- [ ] 數據庫遷移 (如需要)

### 驗收
- [ ] PO 驗收通過 (簽名: ______)
- [ ] UI/UX 驗收通過 (如需要)
- [ ] 演示準備完成

**完成日期**: ________
**完成者**: ________
```

---

## 📚 相關資源

- [User Story Template](./user-story-template.md) - User Story 模板
- [Sprint Backlog](../sprints/sprint-backlog.md) - Sprint 規劃
- [返回總覽](../README.md) - User Stories 總覽

---

**最後更新**: 2025-10-28
