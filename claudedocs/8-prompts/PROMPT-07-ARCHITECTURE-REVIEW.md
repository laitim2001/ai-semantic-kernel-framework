# PROMPT-07: 架構審查

**場景**: 架構設計驗證、系統擴展性評估、技術債務識別
**目標**: 架構一致性檢查、擴展性評估、重構建議
**預估時間**: 10-15 分鐘
**適用對象**: Sprint 開始、重要功能前、定期架構審查

---

## 🎯 使用方式

```
開發者: @PROMPT-07-ARCHITECTURE-REVIEW.md
AI: [執行全面架構審查]
```

**無變數**: 此 Prompt 執行完整架構審查,無需額外參數

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: 架構文檔審查
**讀取文件**:
- `docs/architecture/architecture-overview.md`
- `docs/architecture/backend-architecture.md`
- `docs/architecture/frontend-architecture.md`
- `docs/architecture/database-schema.md`

**輸出**:
```markdown
## 📚 架構文檔審查

### 文檔完整性
- ✅ **架構概覽**: [狀態,最後更新日期]
- ✅ **Backend 架構**: [狀態,最後更新日期]
- ✅ **Frontend 架構**: [狀態,最後更新日期]
- ✅ **數據庫 Schema**: [狀態,最後更新日期]

### 文檔一致性
- [文檔之間是否一致]
- [是否有過時的描述]
```

---

### 步驟 2: 代碼架構一致性檢查
**執行代碼結構檢查**:
```bash
# Domain Layer 結構
find "C:\AI Semantic Kernel\src\AIAgentPlatform.Domain" -type d -maxdepth 2

# Application Layer 結構
find "C:\AI Semantic Kernel\src\AIAgentPlatform.Application" -type d -maxdepth 3

# Infrastructure Layer 結構
find "C:\AI Semantic Kernel\src\AIAgentPlatform.Infrastructure" -type d -maxdepth 3

# API Layer 結構
find "C:\AI Semantic Kernel\src\AIAgentPlatform.API" -type d -maxdepth 2
```

**分析並輸出**:
```markdown
## 🏗️ 代碼架構一致性

### Domain Layer
- **Entities**: [X 個] - [檢查是否符合 DDD 原則]
- **ValueObjects**: [X 個] - [檢查不可變性]
- **Events**: [X 個] - [檢查事件設計]

### Application Layer
- **Commands**: [X 個] - [檢查 CQRS 分離]
- **Queries**: [X 個] - [檢查查詢優化]
- **Handlers**: [X 個] - [檢查單一職責]

### Infrastructure Layer
- **Repositories**: [X 個] - [檢查接口實作]
- **Configurations**: [X 個] - [檢查 EF 配置]
- **Migrations**: [X 個] - [檢查遷移策略]

### API Layer
- **Controllers**: [X 個] - [檢查 RESTful 設計]
- **Middlewares**: [X 個] - [檢查中間件設計]

### 一致性評估
- ✅ **符合架構**: [列出]
- ⚠️ **需要改進**: [列出]
- ❌ **違反架構**: [列出]
```

---

### 步驟 3: 依賴關係分析
**分析層級依賴**:
```bash
# 檢查依賴方向 (不應該有反向依賴)
grep -r "using.*Infrastructure" "C:\AI Semantic Kernel\src\AIAgentPlatform.Domain" --include="*.cs"
grep -r "using.*API" "C:\AI Semantic Kernel\src\AIAgentPlatform.Application" --include="*.cs"
```

**輸出**:
```markdown
## 🔗 依賴關係分析

### 依賴方向檢查
- **Domain → Application**: [是否有反向依賴]
- **Application → Infrastructure**: [是否有反向依賴]
- **API → All Layers**: [是否正確依賴]

### 循環依賴檢查
- [是否存在循環依賴]

### NuGet 套件依賴
- **Domain Layer**: [只應有基礎套件]
- **Application Layer**: [只應有應用級套件]
- **Infrastructure Layer**: [可有第三方套件]

### 依賴健康度
- ✅ **健康**: [X 個依賴]
- ⚠️ **需要優化**: [X 個依賴]
- ❌ **違反原則**: [X 個依賴]
```

---

### 步驟 4: 可擴展性評估
**評估系統擴展能力**:

**輸出**:
```markdown
## 📈 可擴展性評估

### 水平擴展能力
- **無狀態設計**: [是否支持]
- **Session 管理**: [如何處理]
- **數據庫連接**: [連接池配置]

### 垂直擴展能力
- **異步處理**: [是否廣泛使用]
- **緩存策略**: [是否實現]
- **查詢優化**: [N+1 問題檢查]

### 模組化設計
- **Feature 隔離**: [是否良好隔離]
- **Plugin 架構**: [是否支持擴展]

### 可擴展性評分
- **總體評分**: [X/10]
- **主要限制**: [列出]
- **改進建議**: [列出]
```

---

### 步驟 5: 技術債務識別
**搜索常見技術債務指標**:
```bash
# 搜索 TODO 和 FIXME
grep -r "TODO\|FIXME" "C:\AI Semantic Kernel\src" --include="*.cs" | wc -l

# 搜索複雜度高的方法 (行數 >50)
find "C:\AI Semantic Kernel\src" -name "*.cs" -exec awk '/^.*\{$/,/^.*\}$/ { count++ } END { if (count > 50) print FILENAME }' {} \;

# 搜索重複代碼模式
# (簡化版,實際需要更複雜的分析)
```

**輸出**:
```markdown
## 💳 技術債務識別

### 代碼質量指標
- **TODO/FIXME**: [X 個] - [優先級分類]
- **長方法 (>50 行)**: [X 個] - [需要重構]
- **重複代碼**: [檢測結果]

### 架構債務
- **緊耦合組件**: [列出]
- **上帝類 (God Object)**: [列出]
- **缺少抽象**: [列出]

### 性能債務
- **N+1 查詢**: [列出可疑位置]
- **缺少索引**: [數據庫檢查]
- **內存洩漏風險**: [列出可疑位置]

### 測試債務
- **未測試代碼**: [關鍵功能缺少測試]
- **測試覆蓋率低**: [<80% 的模組]

### 債務總覽
- **Critical**: [X 項]
- **High**: [X 項]
- **Medium**: [X 項]
- **Low**: [X 項]
```

---

### 步驟 6: 安全性檢查
**檢查常見安全問題**:
```bash
# 搜索硬編碼密碼或 API Key
grep -r "password\|apikey\|secret" "C:\AI Semantic Kernel\src" --include="*.cs" -i

# 檢查 SQL 注入風險 (檢查字串拼接 SQL)
grep -r "\"SELECT.*\+.*\"" "C:\AI Semantic Kernel\src" --include="*.cs"
```

**輸出**:
```markdown
## 🔒 安全性檢查

### 認證授權
- **JWT 實作**: [是否正確]
- **角色權限**: [是否完整]
- **API 安全**: [是否有適當保護]

### 數據安全
- **SQL 注入防護**: [是否使用參數化查詢]
- **XSS 防護**: [是否有輸入驗證]
- **敏感數據加密**: [是否加密]

### 配置安全
- **密碼管理**: [是否使用 Secrets Manager]
- **連接字串**: [是否安全儲存]

### 安全性評分
- **總體評分**: [X/10]
- **Critical 風險**: [X 個]
- **建議修復**: [列出]
```

---

### 步驟 7: 性能檢查
**分析性能相關設計**:

**輸出**:
```markdown
## ⚡ 性能檢查

### 數據庫性能
- **查詢優化**: [是否使用 Include/Projection]
- **索引策略**: [是否有適當索引]
- **連接管理**: [是否正確使用連接池]

### 應用性能
- **異步編程**: [async/await 使用情況]
- **緩存使用**: [是否實現緩存]
- **批量操作**: [是否優化批量處理]

### API 性能
- **響應時間**: [是否有超時控制]
- **並發處理**: [是否支持高並發]
- **限流機制**: [是否實現]

### 性能評分
- **總體評分**: [X/10]
- **瓶頸識別**: [列出]
- **優化建議**: [列出優先級]
```

---

### 步驟 8: 改進建議總結
**綜合所有分析,提供優先級建議**:

**輸出**:
```markdown
## 🎯 架構改進建議

### Critical 優先級 (必須立即處理)
1. [建議 1] - [原因] - [預估時間]
2. [建議 2] - [原因] - [預估時間]

### High 優先級 (本 Sprint 內處理)
1. [建議 1] - [原因] - [預估時間]
2. [建議 2] - [原因] - [預估時間]

### Medium 優先級 (下個 Sprint 考慮)
1. [建議 1] - [原因] - [預估時間]

### Low 優先級 (技術債務積壓)
1. [建議 1] - [原因] - [預估時間]

### 長期架構規劃
- [建議 1]: [描述和理由]
- [建議 2]: [描述和理由]
```

---

## 🎯 最終輸出摘要

**架構健康度總結**:
```markdown
# 架構審查報告

## 📊 總體評分
- **架構一致性**: [X/10]
- **可擴展性**: [X/10]
- **安全性**: [X/10]
- **性能**: [X/10]
- **代碼質量**: [X/10]
- **整體健康度**: [X/10]

## 🚨 Critical 問題
- [列出需要立即處理的問題]

## ✅ 優勢
- [列出架構設計良好的部分]

## 📋 行動項目
1. [優先級 1 行動項目]
2. [優先級 2 行動項目]
3. [優先級 3 行動項目]

---

**審查日期**: [當前日期]
**下次審查建議**: [2-4 weeks 後]
```

---

## 📊 預期輸出

- **輸出長度**: ~3000-4000 字
- **文檔讀取數量**: ~5-8 個文件
- **代碼檢查**: 10+ 次
- **執行時間**: 10-15 分鐘

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
