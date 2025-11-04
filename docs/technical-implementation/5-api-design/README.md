# Phase 5: API 設計規範 (API Design Standards)

## 階段概述

本階段定義 AI Workflow Platform 的完整 API 設計標準和最佳實踐,確保前後端通信的一致性、可維護性和安全性。

## 目標

- ✅ 制定統一的 RESTful API 設計規範
- ✅ 定義清晰的 API 版本控制策略
- ✅ 建立標準化的錯誤處理機制
- ✅ 實現完整的 API 文檔系統 (OpenAPI/Swagger)
- ✅ 確保 API 安全性與認證授權機制

## 文檔結構

### 1. RESTful API 設計規範
**文件**: `restful-api-standards.md`

**內容**:
- HTTP 方法使用規範 (GET, POST, PUT, PATCH, DELETE)
- URL 路徑設計原則
- 查詢參數與分頁標準
- 請求/響應格式規範
- HTTP 狀態碼使用指南
- Content Negotiation
- HATEOAS 原則應用

**技術重點**:
- ASP.NET Core Minimal APIs
- Resource-oriented design
- RESTful maturity model (Richardson Maturity Model Level 2-3)

### 2. API 版本控制策略
**文件**: `api-versioning.md`

**內容**:
- 版本控制方法選擇 (URL path, Query string, Header, Media type)
- 版本生命週期管理
- 向後兼容性原則
- API 棄用策略
- 版本遷移指南

**技術重點**:
- ASP.NET Core API Versioning 8.0
- 語義化版本控制 (Semantic Versioning)
- API 版本共存策略

### 3. 錯誤處理與響應格式
**文件**: `error-handling.md`

**內容**:
- 標準化錯誤響應結構
- 錯誤代碼分類體系
- Problem Details (RFC 7807) 實現
- 驗證錯誤處理
- 多語言錯誤消息
- 日誌記錄與追蹤

**技術重點**:
- ASP.NET Core Exception Middleware
- FluentValidation 集成
- Structured logging with Serilog
- Correlation ID 追蹤

### 4. API 文檔標準 (Swagger/OpenAPI)
**文件**: `api-documentation.md`

**內容**:
- OpenAPI 3.0 規範實現
- Swagger UI 配置
- API 描述、示例和註解
- 請求/響應 Schema 定義
- 認證配置說明
- 交互式文檔測試

**技術重點**:
- Swashbuckle.AspNetCore 6.5+
- XML 註解文檔生成
- Swagger UI 自定義主題
- API 測試集成

### 5. API 安全性設計
**文件**: `api-security.md`

**內容**:
- JWT 認證實現
- OAuth 2.0 / OpenID Connect
- API Key 管理
- Rate Limiting 與節流
- CORS 配置
- Input 驗證與 Sanitization
- SQL Injection / XSS 防護
- 敏感數據處理

**技術重點**:
- ASP.NET Core Identity
- JWT Bearer Authentication
- AspNetCoreRateLimit 5.0+
- Data Protection APIs

## 技術棧

### 後端 API Framework
```
ASP.NET Core 8.0          # Web API framework
Minimal APIs              # Lightweight endpoint definitions
MediatR 12.2.0           # CQRS request handling
FluentValidation 11.9.0  # Input validation
```

### API 文檔
```
Swashbuckle.AspNetCore 6.5.0    # OpenAPI/Swagger generation
NSwag 14.0.3                    # Alternative OpenAPI tooling
```

### API 版本控制
```
Asp.Versioning.Http 8.0.0               # API versioning support
Asp.Versioning.Mvc.ApiExplorer 8.0.0   # API Explorer integration
```

### 安全與認證
```
Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0    # JWT authentication
AspNetCoreRateLimit 5.0.0                              # Rate limiting
```

### 監控與追蹤
```
Serilog.AspNetCore 8.0.0          # Structured logging
OpenTelemetry 1.7.0               # Distributed tracing
```

## API 設計原則

### 1. Resource-Oriented Design
✅ **DO**: 將 API 設計為資源的集合
```
GET    /api/v1/workflows           # 獲取工作流列表
POST   /api/v1/workflows           # 創建新工作流
GET    /api/v1/workflows/{id}      # 獲取特定工作流
PUT    /api/v1/workflows/{id}      # 更新工作流
DELETE /api/v1/workflows/{id}      # 刪除工作流
```

❌ **DON'T**: 使用 RPC 風格的命名
```
POST /api/v1/createWorkflow        # 避免動詞命名
POST /api/v1/getWorkflowById       # 避免在 URL 中使用動詞
```

### 2. Consistent Naming Conventions
✅ **DO**: 使用複數名詞和小寫 kebab-case
```
/api/v1/workflows
/api/v1/workflow-templates
/api/v1/agent-personas
```

❌ **DON'T**: 混用單數/複數或使用 camelCase
```
/api/v1/workflow           # 不一致的單數
/api/v1/workflowTemplates  # camelCase (應使用 kebab-case)
```

### 3. Idempotency
✅ **DO**: 確保 GET, PUT, DELETE 是冪等的
- GET 請求不應產生副作用
- PUT 請求多次執行結果相同
- DELETE 請求重複執行應返回 404 或 204

✅ **DO**: 為 POST 提供冪等性支持
```http
POST /api/v1/workflows
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
```

### 4. Pagination
✅ **DO**: 為列表資源提供分頁支持
```http
GET /api/v1/workflows?page=1&pageSize=20

Response:
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalCount": 156,
    "totalPages": 8
  },
  "links": {
    "self": "/api/v1/workflows?page=1&pageSize=20",
    "next": "/api/v1/workflows?page=2&pageSize=20",
    "last": "/api/v1/workflows?page=8&pageSize=20"
  }
}
```

### 5. Filtering, Sorting, Searching
✅ **DO**: 使用查詢參數進行過濾和排序
```http
GET /api/v1/workflows?status=published&sortBy=createdAt&sortOrder=desc
GET /api/v1/workflows?search=automation&tags=ai,ml
```

### 6. Versioning
✅ **DO**: 使用 URL path versioning (推薦) 或 Header versioning
```http
# URL Path Versioning (推薦)
GET /api/v1/workflows

# Header Versioning (替代方案)
GET /api/workflows
X-API-Version: 1
```

### 7. Error Handling
✅ **DO**: 使用標準的 Problem Details (RFC 7807)
```json
{
  "type": "https://api.aiworkflow.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Name": ["The Name field is required."],
    "Description": ["The Description must be at least 10 characters."]
  }
}
```

### 8. Security
✅ **DO**: 始終使用 HTTPS
✅ **DO**: 實現 JWT Bearer Authentication
✅ **DO**: 使用 Rate Limiting 防止濫用
✅ **DO**: 驗證和 Sanitize 所有輸入
✅ **DO**: 實現適當的 CORS 策略

## HTTP 狀態碼使用指南

### 2xx Success
- **200 OK**: 成功的 GET, PUT, PATCH 請求
- **201 Created**: 成功創建資源 (POST)
- **204 No Content**: 成功但無返回內容 (DELETE, PUT)

### 4xx Client Errors
- **400 Bad Request**: 請求格式錯誤或驗證失敗
- **401 Unauthorized**: 未認證或 Token 無效
- **403 Forbidden**: 已認證但無權限訪問
- **404 Not Found**: 資源不存在
- **409 Conflict**: 資源衝突 (如重複創建)
- **422 Unprocessable Entity**: 語義錯誤 (驗證失敗)
- **429 Too Many Requests**: 超過 Rate Limit

### 5xx Server Errors
- **500 Internal Server Error**: 伺服器內部錯誤
- **502 Bad Gateway**: 上游服務錯誤
- **503 Service Unavailable**: 服務暫時不可用
- **504 Gateway Timeout**: 上游服務超時

## Content Type 標準

### Request/Response
```
Content-Type: application/json; charset=utf-8
Accept: application/json
```

### 文件上傳
```
Content-Type: multipart/form-data
```

### 二進制下載
```
Content-Type: application/octet-stream
Content-Disposition: attachment; filename="workflow-export.json"
```

## Rate Limiting 策略

### 限制層級
```
未認證用戶:    100 requests/hour
基礎用戶:      1000 requests/hour
企業用戶:      10000 requests/hour
管理員:        無限制
```

### Response Headers
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 998
X-RateLimit-Reset: 1640995200
Retry-After: 3600
```

## CORS 配置

### 允許的來源
```
開發環境: http://localhost:3000, http://localhost:5173
測試環境: https://test.aiworkflow.com
生產環境: https://aiworkflow.com, https://app.aiworkflow.com
```

### 允許的方法
```
GET, POST, PUT, PATCH, DELETE, OPTIONS
```

### 允許的 Headers
```
Authorization, Content-Type, X-API-Version, X-Correlation-ID
```

## API 測試策略

### 單元測試
- 控制器邏輯測試
- 驗證規則測試
- 錯誤處理測試

### 集成測試
- WebApplicationFactory 測試
- 完整 HTTP 請求/響應測試
- 認證授權測試

### Contract Testing
- Pact.io 消費者驅動契約測試
- OpenAPI Schema 驗證

### Load Testing
- Apache JMeter
- k6 性能測試

## 監控與可觀測性

### Metrics
- 請求速率 (requests/second)
- 響應時間 (latency percentiles: p50, p95, p99)
- 錯誤率 (error percentage)
- Rate limit 觸發次數

### Logging
- 結構化日誌 (JSON format)
- Correlation ID 追蹤
- 敏感數據脫敏

### Tracing
- OpenTelemetry 分佈式追蹤
- Span 追蹤跨服務調用

## 實現檢查清單

### API 設計階段
- [ ] 確認 API 資源模型和端點設計
- [ ] 定義請求/響應 Schema
- [ ] 設計錯誤響應結構
- [ ] 制定版本控制策略
- [ ] 規劃認證授權機制

### 開發階段
- [ ] 實現 Minimal APIs 或 Controller-based APIs
- [ ] 集成 FluentValidation 進行輸入驗證
- [ ] 配置 Swagger/OpenAPI 文檔生成
- [ ] 實現 JWT 認證中間件
- [ ] 添加 Rate Limiting 中間件
- [ ] 配置 CORS 策略
- [ ] 實現全局異常處理
- [ ] 添加結構化日誌記錄

### 測試階段
- [ ] 編寫單元測試 (>80% 覆蓋率)
- [ ] 編寫集成測試
- [ ] 執行 API Contract Testing
- [ ] 進行安全測試 (OWASP Top 10)
- [ ] 執行負載測試和性能測試

### 文檔階段
- [ ] 完成 Swagger UI 配置和自定義
- [ ] 添加 XML 註解文檔
- [ ] 編寫 API 使用指南
- [ ] 提供 Postman Collection
- [ ] 創建 API 變更日誌 (Changelog)

### 部署階段
- [ ] 配置生產環境 HTTPS
- [ ] 設置 API Gateway (如使用)
- [ ] 配置 CDN 和緩存策略
- [ ] 設置監控告警
- [ ] 實現健康檢查端點 (`/health`, `/ready`)

## 相關文檔

- [Phase 3: 項目結構](../3-project-structure/README.md)
- [Phase 4: 編碼標準](../4-coding-standards/README.md)
- [Phase 8: 安全標準](../8-security-standards/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 規範與標準
- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [OpenAPI Specification 3.1](https://spec.openapis.org/oas/v3.1.0)
- [Richardson Maturity Model](https://martinfowler.com/articles/richardsonMaturityModel.html)
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)

### ASP.NET Core 官方文檔
- [Web API Design Best Practices](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [ASP.NET Core API Versioning](https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/versioning)
- [Swagger/OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

### 書籍
- *RESTful Web APIs* by Leonard Richardson, Mike Amundsen
- *API Design Patterns* by JJ Geewax
- *Designing Web APIs* by Brenda Jin, Saurabh Sahni, Amir Shevat

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
