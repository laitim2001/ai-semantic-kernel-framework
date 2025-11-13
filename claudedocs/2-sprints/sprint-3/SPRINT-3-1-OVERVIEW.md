# Sprint 3 概覽 (OVERVIEW)

**版本**: v2.0
**創建日期**: 2025-11-13
**最後更新**: 2025-11-13
**Sprint 週期**: Week 7-9 (3 週)
**Sprint 狀態**: 📋 計劃階段 (Planned)
**Phase**: Phase 1A - 基礎平台 (Foundation Platform)

---

## 📋 目錄

1. [Sprint 目標](#sprint-目標)
2. [User Stories 分配](#user-stories-分配)
3. [技術範圍](#技術範圍)
4. [預期交付](#預期交付)
5. [風險與依賴](#風險與依賴)
6. [成功指標](#成功指標)
7. [相關文檔](#相關文檔)
8. [完整參考文獻索引](#完整參考文獻索引)

---

## Sprint 目標

### 核心目標

完成 **Plugin 系統基礎架構 (US 1.3)**，建立企業級 AI Agent 平台的核心擴展能力，讓 Agent 可以動態載入和執行外部功能模組。

#### 主要目標 (Primary Goals)

1. **Plugin 生命週期管理** - 實現 Plugin 的 Create, Activate, Deactivate, Delete 操作
2. **Plugin Registry** - 建立 Plugin 註冊表與元數據管理
3. **安全隔離** - Plugin 執行在獨立 AppDomain，防止系統被惡意 Plugin 影響
4. **MCP 協議支援** - 整合 Model Context Protocol，支援標準化的 Plugin 介面

#### 次要目標 (Secondary Goals)

5. **Plugin 驗證機制** - 驗證 Plugin 簽名和權限
6. **Plugin 依賴管理** - 基礎的 NuGet 依賴解析
7. **Plugin 文檔生成** - 自動生成 Plugin API 文檔

### 業務價值 (Business Value)

```yaml
對開發者 (IT Developer):
  價值: 可以開發和部署自定義 Plugin，擴展 Agent 功能
  影響: 大幅提升平台靈活性，避免所有功能都需要平台團隊開發
  參考: docs/ux-design/user-research/personas.md (Alex - IT Developer)

對業務分析師 (Business Analyst):
  價值: 可以選擇和配置現成的 Plugin，快速組裝業務流程
  影響: 減少對技術團隊的依賴，提升業務敏捷性
  參考: docs/ux-design/user-research/personas.md (Emma - Business Analyst)

對平台管理員 (Enterprise Admin):
  價值: 可以管理 Plugin 權限和使用情況，確保系統安全
  影響: 降低安全風險，符合企業 IT 治理要求
  參考: docs/ux-design/user-research/personas.md (Chris - Enterprise Admin)
```

### Sprint 在 MVP 中的定位

```
Phase 1A: 基礎平台 (Sprint 1-6, Week 1-18)
├─ Sprint 1: US 1.1 基礎設施 ✅ 已完成
├─ Sprint 2: US 1.2 Agent 引擎 + US 2.1 Plugin 基礎 🔄 進行中 (70%)
├─ Sprint 3: US 1.3 Plugin 系統 ← 當前 Sprint
├─ Sprint 4: US 2.2 Plugin 熱加載 + US 2.1 Persona
├─ Sprint 5: US 3.1 Knowledge 檢索
└─ Sprint 6: US 5.1 Code Interpreter (簡化版)

關鍵里程碑:
  M3: 核心功能完成 (Week 12) - Sprint 3 完成後距離此里程碑還有 3 週
  參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (Section 時間與資源限制)
```

---

## User Stories 分配

### US 1.3: Plugin 系統基礎架構

**Story Points**: 13 SP
**優先級**: P0 (Must Have)
**負責團隊**: Backend Team (3 名開發者)
**完整 User Story**: [docs/user-stories/us-2-plugin-system.md](../../../docs/user-stories/us-2-plugin-system.md)

#### User Story 描述

> **As a** IT Developer
> **I want to** 開發和部署自定義 Plugin
> **So that** 我可以擴展 Agent 的功能而無需修改核心平台程式碼

#### 驗收標準 (Acceptance Criteria)

```yaml
✅ AC1: Plugin CRUD API
  Given: 開發者使用 RESTful API
  When: 呼叫 POST /api/plugins 建立 Plugin
  Then: 系統應該驗證 Plugin 元數據並建立 Plugin 記錄
  參考: docs/api/plugin-api-design.md

✅ AC2: Plugin 類型支援
  Given: 系統支援兩種 Plugin 類型
  When: 開發者上傳 Native Plugin (C# DLL) 或 Semantic Plugin (Prompt YAML)
  Then: 系統應該正確識別並載入對應類型的 Plugin
  參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md

✅ AC3: 安全隔離
  Given: Plugin 可能包含惡意程式碼
  When: 系統執行 Plugin
  Then: Plugin 應該在獨立 AppDomain 中執行，無法訪問系統敏感資源
  參考: docs/security/plugin-security.md

✅ AC4: MCP 協議支援
  Given: 系統支援 Model Context Protocol
  When: Plugin 實現 MCP 介面
  Then: 系統應該能夠透過標準協議與 Plugin 通訊
  參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md (Section MCP Integration)

✅ AC5: Plugin Registry
  Given: 系統維護 Plugin 註冊表
  When: Plugin 被載入
  Then: 系統應該記錄 Plugin 的元數據、版本、依賴、權限
  參考: docs/database/plugin-schema.md
```

#### 任務分解 (Task Breakdown)

| Task ID | 任務描述 | 負責人 | SP | 狀態 | 參考文檔 |
|---------|---------|--------|----|----|---------|
| T3.1 | Plugin Entity 設計與實作 (Domain Layer) | Backend Dev 1 | 2 SP | 📋 Planned | [docs/architecture/layered-architecture/Domain-Layer.md](../../../docs/architecture/layered-architecture/Domain-Layer.md) |
| T3.2 | Plugin Repository 實作 (Infrastructure Layer) | Backend Dev 1 | 2 SP | 📋 Planned | [docs/architecture/layered-architecture/Infrastructure-Layer.md](../../../docs/architecture/layered-architecture/Infrastructure-Layer.md) |
| T3.3 | Plugin CRUD Commands/Queries (Application Layer) | Backend Dev 2 | 3 SP | 📋 Planned | [docs/architecture/layered-architecture/Application-Layer.md](../../../docs/architecture/layered-architecture/Application-Layer.md) |
| T3.4 | IPluginLoader 介面設計與實作 | Backend Dev 3 | 2 SP | 📋 Planned | [docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md) |
| T3.5 | AppDomain 隔離機制實作 | Backend Dev 3 | 2 SP | 📋 Planned | [docs/security/plugin-security.md](../../../docs/security/plugin-security.md) |
| T3.6 | MCP 協議整合 | Backend Dev 2 | 1 SP | 📋 Planned | [docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md) |
| T3.7 | Plugin API Controller 實作 | Backend Dev 1 | 1 SP | 📋 Planned | [docs/api/plugin-api-design.md](../../../docs/api/plugin-api-design.md) |

**總計**: 13 SP

---

## 技術範圍

### 後端開發 (Backend Development)

#### Domain Layer (領域層)

```yaml
新增 Entities:
  - Plugin:
      屬性: PluginId, Name, Description, Type (Native/Semantic), Status, Metadata
      業務規則:
        - Plugin Name 必須唯一
        - 只有 Active 狀態的 Plugin 才能被 Agent 載入
      參考: docs/architecture/layered-architecture/Domain-Layer.md (Section Entities)

  - PluginMetadata:
      屬性: PluginId, Version, Author, Dependencies, Permissions
      業務規則:
        - Version 必須遵循 SemVer 2.0 格式
        - Dependencies 必須在系統中已註冊
      參考: docs/database/plugin-schema.md

Value Objects:
  - PluginType (enum): Native, Semantic
  - PluginStatus (enum): Draft, Active, Inactive, Archived
  - SemanticVersion: Major.Minor.Patch
    參考: docs/architecture/layered-architecture/Domain-Layer.md (Section Value Objects)
```

#### Application Layer (應用層 - CQRS)

```yaml
Commands:
  - CreatePluginCommand:
      Handler: CreatePluginCommandHandler
      驗證: FluentValidation (Name, Type, Metadata)
      參考: docs/architecture/layered-architecture/Application-Layer.md (Section Commands)

  - ActivatePluginCommand:
      Handler: ActivatePluginCommandHandler
      業務邏輯: 驗證 Plugin 簽名 → 載入依賴 → 啟用 Plugin
      參考: docs/architecture/layered-architecture/Application-Layer.md

  - DeactivatePluginCommand / DeletePluginCommand

Queries:
  - GetPluginByIdQuery
  - GetPluginsQuery (with filtering by Type, Status)
  - GetPluginMetadataQuery
    參考: docs/architecture/layered-architecture/Application-Layer.md (Section Queries)
```

#### Infrastructure Layer (基礎設施層)

```yaml
Repositories:
  - IPluginRepository:
      方法: AddAsync, UpdateAsync, DeleteAsync, GetByIdAsync, GetAllAsync
      實作: PluginRepository (EF Core)
      參考: docs/architecture/layered-architecture/Infrastructure-Layer.md (Section Repositories)

Services:
  - IPluginLoader:
      方法: LoadPlugin(pluginPath), UnloadPlugin(pluginId), GetLoadedPlugins()
      實作: PluginLoader
      參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md (Section Plugin Loader)

  - IPluginValidator:
      方法: ValidateSignature(plugin), ValidatePermissions(plugin)
      實作: PluginValidator
      參考: docs/security/plugin-security.md (Section Plugin Validation)

Database:
  - Plugins 表 (新增)
  - PluginMetadata 表 (新增)
  - Migration: 20XX_AddPluginTables
    參考: docs/database/plugin-schema.md
```

### API 設計

```yaml
Plugin API Endpoints:
  POST /api/plugins:
    描述: 建立新 Plugin
    Request Body: { name, description, type, file (binary) }
    Response: 201 Created + Plugin DTO
    參考: docs/api/plugin-api-design.md

  GET /api/plugins/{id}:
    描述: 取得單一 Plugin 詳情
    Response: 200 OK + Plugin DTO
    參考: docs/api/plugin-api-design.md

  GET /api/plugins:
    描述: 取得 Plugin 列表（支援篩選和分頁）
    Query Params: type, status, page, pageSize
    Response: 200 OK + PagedResult<PluginDTO>
    參考: docs/api/plugin-api-design.md

  PUT /api/plugins/{id}/activate:
    描述: 啟用 Plugin
    Response: 200 OK
    參考: docs/api/plugin-api-design.md

  PUT /api/plugins/{id}/deactivate:
    描述: 停用 Plugin
    Response: 200 OK

  DELETE /api/plugins/{id}:
    描述: 刪除 Plugin
    Response: 204 No Content
    參考: docs/api/plugin-api-design.md
```

### 安全機制

```yaml
AppDomain 隔離:
  目的: 防止 Plugin 訪問系統敏感資源或影響其他 Plugin
  實作:
    - 每個 Native Plugin 在獨立 AppDomain 中執行
    - AppDomain 配置限制:
        - 無檔案系統訪問 (除 Plugin 目錄)
        - 無網路訪問 (除白名單 API)
        - 記憶體限制: 256MB / Plugin
  參考: docs/security/plugin-security.md (Section AppDomain Isolation)

Plugin 簽名驗證:
  目的: 確保 Plugin 來源可信
  實作:
    - Plugin DLL 必須使用強式名稱簽名 (Strong Name)
    - 系統驗證簽名憑證是否在白名單中
  參考: docs/security/plugin-security.md (Section Code Signing)

權限管理:
  目的: 限制 Plugin 可訪問的系統功能
  實作:
    - Plugin Manifest 宣告所需權限 (Permissions)
    - 系統執行前檢查權限 (File, Network, Database, API)
  參考: docs/security/plugin-security.md (Section Permission Model)
```

---

## 預期交付

### Deliverables (交付物)

#### 1. 後端 API (Backend API)

```yaml
✅ Plugin CRUD API:
  - 6 個 RESTful 端點 (Create, Read, List, Activate, Deactivate, Delete)
  - Swagger 文檔完整
  - Request/Response 驗證
  參考: docs/api/plugin-api-design.md

✅ Plugin 生命週期管理:
  - Plugin 狀態轉換邏輯 (Draft → Active → Inactive → Archived)
  - Plugin 版本記錄 (基礎版，完整版在 Sprint 4)
  參考: docs/architecture/layered-architecture/Domain-Layer.md

✅ 安全隔離機制:
  - AppDomain 隔離實作
  - Plugin 簽名驗證
  - 權限檢查機制
  參考: docs/security/plugin-security.md
```

#### 2. 資料庫 Schema (Database Schema)

```yaml
✅ Plugins 表:
  欄位: PluginId, Name, Description, Type, Status, FilePath, CreatedAt, UpdatedAt
  索引: Name (Unique), Type, Status
  參考: docs/database/plugin-schema.md

✅ PluginMetadata 表:
  欄位: MetadataId, PluginId, Version, Author, Dependencies, Permissions
  外鍵: PluginId → Plugins.PluginId
  參考: docs/database/plugin-schema.md

✅ EF Core Migration:
  檔案: 20XX_AddPluginTables.cs
  操作: CreateTable, CreateIndex, AddForeignKey
  參考: docs/database/migration-guide.md
```

#### 3. 單元測試 (Unit Tests)

```yaml
✅ Domain Layer Tests:
  測試範圍: Plugin Entity 業務規則、Value Objects 驗證
  覆蓋率目標: ≥85%
  參考: docs/testing/unit-testing-guidelines.md

✅ Application Layer Tests:
  測試範圍: Command/Query Handlers, FluentValidation Rules
  覆蓋率目標: ≥80%
  參考: docs/testing/unit-testing-guidelines.md

✅ Infrastructure Layer Tests:
  測試範圍: Repository CRUD 操作, PluginLoader 邏輯
  測試工具: In-Memory Database, Moq
  參考: docs/testing/integration-testing-guidelines.md
```

#### 4. 技術文檔 (Technical Documentation)

```yaml
✅ API 文檔:
  - Swagger UI (自動生成)
  - API 設計文檔 (docs/api/plugin-api-design.md)
  - 範例 Request/Response

✅ Plugin 開發指南:
  - 如何開發 Native Plugin (C# 範例)
  - 如何開發 Semantic Plugin (YAML 範例)
  - Plugin Manifest 格式說明
  參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md (Section Developer Guide)

✅ 安全性文檔:
  - AppDomain 隔離機制說明
  - Plugin 簽名流程
  - 權限模型設計
  參考: docs/security/plugin-security.md
```

### 不交付 (Out of Scope for Sprint 3)

```yaml
❌ Plugin Marketplace:
  理由: MVP Phase 2 功能
  參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (Section MVP 排除功能)

❌ Plugin 熱重載 (Hot Reload):
  理由: 將在 Sprint 4 (US 2.2) 實作
  參考: claudedocs/1-planning/SPRINT-ALLOCATION-ANALYSIS.md (Sprint 4)

❌ Plugin 版本控制 (完整):
  理由: Sprint 3 僅實作基礎版本記錄，完整版本管理在 Sprint 4
  參考: docs/user-stories/us-2-plugin-system.md (US 2.2)

❌ Plugin 相依性自動解析:
  理由: Sprint 3 僅支援手動宣告依賴，自動解析留待 Phase 2
  參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md
```

---

## 風險與依賴

### 高風險項目 (High Risks)

#### RISK-003: AppDomain 隔離技術複雜度 ⚠️ 高風險

```yaml
風險描述:
  .NET 9 中 AppDomain 隔離的最佳實踐尚未完全確立
  可能遇到跨 AppDomain 通訊的效能問題

影響:
  - Plugin 載入時間可能超過預期 (目標 <2 秒)
  - 記憶體使用可能較高

發生概率: 40%

緩解措施:
  ✅ PoC 驗證: 在 Sprint 3 Week 1 進行 AppDomain 隔離 PoC
  ✅ 參考實作: 研究 LangChain.NET, AutoGen.NET 的 Plugin 隔離方案
  ✅ 效能基準: 建立 Plugin 載入效能測試，目標 P95 <2 秒
  ✅ Fallback 方案: 如 AppDomain 效能不佳，考慮使用 AssemblyLoadContext

殘餘風險: 15%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-003 完整分析)
  - docs/security/plugin-security.md (Section Isolation Alternatives)
```

#### RISK-019: MCP 協議整合複雜度 🟡 中風險

```yaml
風險描述:
  Model Context Protocol (MCP) 是相對新的標準
  .NET 生態系統的 MCP 實作可能不成熟

影響:
  - MCP Plugin 可能無法與 Semantic Kernel 無縫整合
  - 需要額外開發 MCP Adapter

發生概率: 30%

緩解措施:
  ✅ 研究現有實作: 調研 anthropic/mcp 和 .NET 社群的 MCP 實作
  ✅ 簡化 MVP 範圍: Sprint 3 僅支援基礎 MCP 功能，高級功能留待 Phase 2
  ✅ Adapter Pattern: 設計 IMCPAdapter 介面，允許未來替換實作

殘餘風險: 10%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-019)
  - docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md (Section MCP Integration)
```

### 中風險項目 (Medium Risks)

#### RISK-020: Plugin 安全性漏洞 🟡 中風險

```yaml
風險描述:
  惡意 Plugin 可能繞過安全隔離機制
  例如: 反射、序列化漏洞

影響:
  - 系統敏感資料可能被竊取
  - 系統穩定性可能受影響

發生概率: 25%

緩解措施:
  ✅ 安全測試: 進行 Plugin 安全滲透測試
  ✅ Code Review: 安全團隊審查 AppDomain 隔離實作
  ✅ 限制反射: 禁止 Plugin 使用 System.Reflection (透過 AppDomain 權限)

殘餘風險: 10%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-020)
  - docs/security/plugin-security.md (Section Security Testing)
```

### 依賴項目 (Dependencies)

#### 對其他 Sprint 的依賴

```yaml
依賴 Sprint 2 (US 1.2 Agent 引擎):
  理由: Plugin 需要載入到 Agent 執行引擎中
  狀態: 🔄 Sprint 2 進行中 (70% 完成)
  風險: 如 Sprint 2 延遲，Sprint 3 可能受影響
  緩解: Sprint 2 Week 3 完成 Agent 引擎核心即可開始 Sprint 3
  參考: claudedocs/1-planning/DEPENDENCY-MATRIX.md (DEP-012)

依賴 Sprint 1 (US 1.1 基礎設施):
  理由: Plugin 需要資料庫和 Redis 基礎設施
  狀態: ✅ 已完成
  風險: 無
  參考: claudedocs/1-planning/DEPENDENCY-MATRIX.md (DEP-001)
```

#### 外部依賴 (External Dependencies)

```yaml
.NET 9 SDK:
  版本: >= 9.0.0
  風險: 低 (已穩定)
  參考: docs/technical-implementation/01-backend-net9/01-monorepo-setup.md

Semantic Kernel:
  版本: >= 1.0.0
  風險: 中 (Plugin 整合 API 可能變動)
  緩解: 使用穩定版本，避免使用實驗性 API
  參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md

Entity Framework Core 9:
  版本: >= 9.0.0
  風險: 低 (成熟技術)
  參考: docs/architecture/layered-architecture/Infrastructure-Layer.md
```

---

## 成功指標

### 功能指標 (Functional Metrics)

```yaml
Plugin CRUD API:
  目標: 所有 6 個 API 端點正常運作
  測量: Postman / Swagger UI 手動測試 + 自動化測試
  成功標準: 100% 端點通過測試
  參考: docs/api/plugin-api-design.md

Plugin 載入成功率:
  目標: ≥95%
  測量: 載入 10 個測試 Plugin (5 Native + 5 Semantic)
  成功標準: ≥9.5 個成功載入
  參考: docs/testing/integration-testing-guidelines.md

安全隔離有效性:
  目標: 0 次安全漏洞
  測量: 執行 5 個惡意 Plugin 測試案例
  成功標準: 所有惡意操作被阻擋，系統無受損
  參考: docs/security/plugin-security.md (Section Security Testing)
```

### 非功能指標 (Non-Functional Metrics)

```yaml
Plugin 載入效能:
  目標: P95 <2 秒
  測量: 載入 100 次測試 Plugin，記錄時間
  成功標準: 95% 的載入時間 <2 秒
  參考: docs/testing/performance-testing.md

API 響應時間:
  目標: P95 <500ms (GET), P95 <1s (POST)
  測量: 使用 k6 負載測試工具
  成功標準: 95% 請求符合目標
  參考: docs/testing/performance-testing.md

記憶體使用:
  目標: 每個 Plugin AppDomain ≤256MB
  測量: 使用 .NET Memory Profiler
  成功標準: 10 個 Plugin 同時載入，總記憶體 ≤2.5GB
  參考: docs/testing/performance-testing.md
```

### 質量指標 (Quality Metrics)

```yaml
單元測試覆蓋率:
  目標: ≥80% (Domain + Application Layer)
  測量: dotnet test --collect:"XPlat Code Coverage"
  成功標準: 覆蓋率報告顯示 ≥80%
  參考: docs/testing/unit-testing-guidelines.md

Code Review 通過率:
  目標: 100% PR 經過 Code Review
  測量: GitHub PR Review 記錄
  成功標準: 所有 PR 至少 1 個 Approve
  參考: docs/development-standards/code-review-guidelines.md

Bug Density:
  目標: ≤2 bugs / 100 LOC (critical + high)
  測量: Sprint Retrospective 統計
  成功標準: 符合目標
  參考: claudedocs/2-sprints/sprint-1/SPRINT-1-7-RETROSPECTIVE.md (Quality Metrics)
```

---

## 相關文檔

### Sprint 文檔

- [SPRINT-3-2-PLAN.md](./SPRINT-3-2-PLAN.md) - Sprint 3 詳細執行計劃 (3 週分工)
- [SPRINT-3-3-CONTEXT.md](./SPRINT-3-3-CONTEXT.md) - Sprint 3 技術上下文與架構設計
- [SPRINT-3-4-CHECKLIST.md](./SPRINT-3-4-CHECKLIST.md) - Sprint 3 任務檢查清單
- [SPRINT-3-5-DEV-LOG.md](./SPRINT-3-5-DEV-LOG.md) - Sprint 3 開發日誌 (每日更新)
- [SPRINT-3-6-ISSUES.md](./SPRINT-3-6-ISSUES.md) - Sprint 3 問題追蹤
- [SPRINT-3-7-RETROSPECTIVE.md](./SPRINT-3-7-RETROSPECTIVE.md) - Sprint 3 回顧 (Sprint 結束後)

### 前置 Sprint

- [Sprint 1 Overview](../sprint-1/SPRINT-1-1-OVERVIEW.md) - US 1.1 基礎設施
- [Sprint 2 Overview](../sprint-2/SPRINT-2-1-OVERVIEW.md) - US 1.2 Agent 引擎 + US 2.1 Plugin 基礎

### 規劃文檔 (Planning Documents)

- [MVP-SCOPE-DEFINITION.md](../../1-planning/MVP-SCOPE-DEFINITION.md) - Sprint 3 在 MVP 中的範圍定義
- [SPRINT-ALLOCATION-ANALYSIS.md](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細分配與依賴分析
- [DEPENDENCY-MATRIX.md](../../1-planning/DEPENDENCY-MATRIX.md) - Sprint 3 依賴關係 (DEP-012)
- [RISK-REGISTER.md](../../1-planning/RISK-REGISTER.md) - Sprint 3 相關風險 (RISK-003, RISK-019, RISK-020)

---

## 📚 完整參考文獻索引

本文檔的技術設計、驗收標準與風險評估基於以下文檔，按類別組織以便快速定位相關資訊：

### Planning 文檔（濃縮版，優先查閱）

> 📌 **重要**：優先查閱此區域文檔，它們是 /docs 的濃縮版，提供更全面的項目背景、架構設計與技術棧資訊

- [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Sprint 3 在 MVP 中的範圍與邊界定義 (Plugin 系統基礎架構)
- [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細分配、Story Points 估算 (13 SP) 與依賴關係 (DEP-012)
- [Dependency Matrix](../../1-planning/DEPENDENCY-MATRIX.md) - Sprint 3 依賴分析 (依賴 Sprint 2 Agent 引擎)
- [Risk Register](../../1-planning/RISK-REGISTER.md) - Sprint 3 風險管理 (RISK-003, RISK-019, RISK-020)
- [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Plugin 系統在整體開發策略中的定位
- [Technical Decisions Log](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - Plugin 系統相關技術決策記錄

### User Stories（原始需求）

- [US 2: Plugin System](../../../docs/user-stories/us-2-plugin-system.md) - Plugin 系統完整 User Story (US 2.1 基礎架構 in Sprint 3)
- [US 1: Agent CRUD](../../../docs/user-stories/us-1-agent-crud.md) - Agent 引擎基礎 (Sprint 3 依賴項)

### ADR（架構決策記錄）

- [ADR-011: Framework Abstraction Layer](../../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) - IToolRegistry 介面設計 (Plugin 載入抽象)
- [ADR-007: Communication Architecture](../../../docs/architecture/adr/ADR-007-internal-communication-strategy.md) - Plugin 與 Agent 通訊機制 (Phase 1: MediatR)
- [ADR-006: State Management](../../../docs/architecture/adr/ADR-006-state-management-strategy.md) - Plugin 元數據快取策略 (Redis + PostgreSQL)

### 技術實施文檔（Backend）

- [04: Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md) - Plugin 系統完整技術實作指南 (Native + Semantic Plugin, MCP 整合)
- [02: Semantic Kernel Integration](../../../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md) - SK Plugin 整合機制
- [01: Monorepo Setup](../../../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md) - 項目結構與 CI/CD (Plugin 建置流程)

### 架構設計文檔（Clean Architecture）

- [Architecture Design Document](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture 完整設計 (Plugin 系統的 4-Layer 架構)
- [Domain Layer](../../../docs/architecture/layered-architecture/Domain-Layer.md) - Plugin Entity, PluginType, PluginStatus 設計
- [Application Layer](../../../docs/architecture/layered-architecture/Application-Layer.md) - Plugin CQRS Commands/Queries (CreatePluginCommand, ActivatePluginCommand)
- [Infrastructure Layer](../../../docs/architecture/layered-architecture/Infrastructure-Layer.md) - IPluginRepository, IPluginLoader 實作
- [Presentation Layer (API)](../../../docs/architecture/layered-architecture/Presentation-Layer.md) - Plugin API Controller 設計

### API 設計文檔

- [Plugin API Design](../../../docs/api/plugin-api-design.md) - Plugin RESTful API 設計 (6 個端點: CRUD, Activate, Deactivate)
- [API Design Guidelines](../../../docs/api/API-Design-Guidelines.md) - RESTful API 設計規範 (適用於所有 API)

### Database 設計文檔

- [Plugin Schema](../../../docs/database/plugin-schema.md) - Plugins, PluginMetadata 資料表設計
- [Migration Guide](../../../docs/database/migration-guide.md) - EF Core Migration 最佳實踐
- [Redis Design](../../../docs/database/redis-design.md) - Plugin 元數據快取設計

### 安全性文檔

- [Plugin Security](../../../docs/security/plugin-security.md) - AppDomain 隔離、Plugin 簽名驗證、權限模型 (4-Layer Security)
- [Code Execution Security](../../../docs/security/code-execution-security.md) - 代碼執行安全最佳實踐 (適用於 Plugin)
- [Authentication & Authorization](../../../docs/security/Authentication-Authorization.md) - Plugin API 權限控制

### 測試文檔

- [Unit Testing Guidelines](../../../docs/testing/unit-testing-guidelines.md) - Plugin Domain/Application Layer 單元測試規範 (目標覆蓋率 ≥80%)
- [Integration Testing Guidelines](../../../docs/testing/integration-testing-guidelines.md) - Plugin Repository, PluginLoader 整合測試
- [Performance Testing](../../../docs/testing/performance-testing.md) - Plugin 載入效能測試 (目標 P95 <2 秒)

### UX 設計文檔

- [User Personas](../../../docs/ux-design/user-research/personas.md) - 三大用戶群 (IT Developer, Business Analyst, Enterprise Admin) Plugin 使用場景
- [User Journey Maps](../../../docs/ux-design/user-research/user-journey-maps.md) - Plugin 開發與部署用戶旅程

### 開發標準文檔

- [Code Review Guidelines](../../../docs/development-standards/code-review-guidelines.md) - Plugin 代碼審查標準
- [Non-Functional Requirements](../../../docs/development-standards/Non-Functional-Requirements.md) - Plugin 系統非功能性需求 (效能、安全、可靠性)

### 前置 Sprint 文檔（經驗傳承）

- [Sprint 1 Overview](../sprint-1/SPRINT-1-1-OVERVIEW.md) - 基礎設施搭建經驗 (Docker, PostgreSQL, Redis)
- [Sprint 1 Retrospective](../sprint-1/SPRINT-1-7-RETROSPECTIVE.md) - Sprint 1 回顧與改進措施
- [Sprint 2 Overview](../sprint-2/SPRINT-2-1-OVERVIEW.md) - Agent 引擎與 SK 整合經驗
- [Sprint 2 Context](../sprint-2/SPRINT-2-3-CONTEXT.md) - Agent 引擎技術上下文 (Plugin 依賴項)

### Change Management

- [Change Log](../../4-changes/CHANGE-LOG.md) - Sprint 3 相關變更記錄

---

**文檔建立日期**: 2025-11-13
**文檔維護者**: Project Manager
**Sprint 負責人**: Backend Team Lead
**最後審查日期**: 2025-11-13

---

**變更日誌 (Change Log)**:

- **v2.0** (2025-11-13): 初始版本建立
  - ✅ 添加完整參考文獻索引 (50+ 文檔，11 個類別)
  - ✅ 基於 Sprint 1/2 模板結構
  - ✅ 遵循 v2.0 標準 (完整可追溯性)
  - ✅ 明確 Sprint 3 目標、範圍、風險、依賴
