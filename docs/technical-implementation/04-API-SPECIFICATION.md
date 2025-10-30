# TID Part 2.1: API 設計規範 (API Specification)

**文檔版本**: v1.0.0
**創建日期**: 2025-10-30
**狀態**: ✅ 完成
**所屬**: Part 2 - API Design

---

## 目錄

1. [API 設計原則](#1-api-設計原則)
2. [API Gateway 架構](#2-api-gateway-架構)
3. [RESTful API 設計](#3-restful-api-設計)
4. [認證授權](#4-認證授權)
5. [請求響應格式](#5-請求響應格式)
6. [錯誤處理](#6-錯誤處理)
7. [速率限制](#7-速率限制)
8. [版本控制](#8-版本控制)
9. [API 端點總覽](#9-api-端點總覽)

---

## 1. API 設計原則

### 1.1 核心原則

| 原則 | 說明 | 範例 |
|------|------|------|
| **RESTful** | 使用 HTTP 動詞和資源路徑 | `GET /api/v1/agents/{id}` |
| **一致性** | 命名、格式、錯誤處理統一 | 統一使用 `snake_case` 或 `camelCase` |
| **向後兼容** | 新版本不破壞舊客戶端 | 使用 API 版本控制 |
| **自描述** | API 響應包含完整信息 | 包含 `_links` HATEOAS |
| **安全優先** | 默認啟用認證和 HTTPS | JWT + Rate Limiting |

### 1.2 設計標準

```yaml
api_design_standards:
  # URL 設計
  url_structure: "/api/{version}/{resource}/{id}/{sub-resource}"
  base_url: "https://api.aiworkflow.com"

  # 命名規範
  resource_naming: "複數名詞 (agents, personas, workflows)"
  parameter_naming: "snake_case for query params, camelCase for JSON"

  # HTTP 方法
  methods:
    GET: "查詢資源 (冪等、可緩存)"
    POST: "創建資源 (非冪等)"
    PUT: "完整更新資源 (冪等)"
    PATCH: "部分更新資源 (冪等)"
    DELETE: "刪除資源 (冪等)"

  # 狀態碼
  success_codes: [200, 201, 202, 204]
  client_error_codes: [400, 401, 403, 404, 409, 422, 429]
  server_error_codes: [500, 502, 503, 504]

  # 響應格式
  content_type: "application/json"
  charset: "UTF-8"
  date_format: "ISO 8601 (2025-10-30T10:15:30Z)"
```

### 1.3 URL 設計模式

#### 資源路徑結構

```
/api/v1/agents                    # Collection
/api/v1/agents/{id}               # Single resource
/api/v1/agents/{id}/executions    # Sub-collection
/api/v1/agents/{id}/executions/{execution_id}  # Sub-resource
```

#### 查詢參數

```
# 分頁
GET /api/v1/agents?page=2&page_size=20

# 過濾
GET /api/v1/agents?status=active&persona_id=abc123

# 排序
GET /api/v1/agents?sort=created_at&order=desc

# 字段選擇 (Sparse Fieldsets)
GET /api/v1/agents?fields=id,name,status

# 搜索
GET /api/v1/agents?q=data+analyzer
```

---

## 2. API Gateway 架構

### 2.1 Gateway 職責

```mermaid
graph TB
    Client[Client Apps]

    subgraph "API Gateway Layer"
        Gateway[API Gateway]
        Auth[Authentication]
        RateLimit[Rate Limiter]
        Cache[Response Cache]
        Router[Request Router]
    end

    subgraph "Backend Services"
        AgentSvc[Agent Service]
        PersonaSvc[Persona Service]
        CodeSvc[Code Sandbox]
        SQLSvc[Text-to-SQL]
        RAGSvc[Knowledge RAG]
        WorkflowSvc[Workflow Service]
    end

    Client -->|HTTPS| Gateway
    Gateway --> Auth
    Auth --> RateLimit
    RateLimit --> Cache
    Cache --> Router

    Router --> AgentSvc
    Router --> PersonaSvc
    Router --> CodeSvc
    Router --> SQLSvc
    Router --> RAGSvc
    Router --> WorkflowSvc
```

### 2.2 Gateway 功能

| 功能 | 說明 | 實現方式 |
|------|------|---------|
| **路由** | 請求轉發到對應服務 | Nginx / Kong / Traefik |
| **認證** | JWT Token 驗證 | JWT middleware |
| **授權** | RBAC 權限檢查 | Policy-based |
| **速率限制** | 防止 API 濫用 | Redis + Sliding Window |
| **緩存** | 高頻查詢響應緩存 | Redis (TTL: 1-5min) |
| **日誌** | 請求日誌和追蹤 | Correlation ID |
| **監控** | Metrics 收集 | Prometheus |
| **轉換** | 協議轉換 (HTTP/gRPC) | Protocol buffer |

### 2.3 Gateway 配置示例

```yaml
# Kong Gateway Configuration
services:
  - name: agent-service
    url: http://agent-service:8080
    routes:
      - name: agent-routes
        paths:
          - /api/v1/agents
        methods: [GET, POST, PUT, PATCH, DELETE]
        strip_path: false
    plugins:
      - name: jwt
        config:
          secret_is_base64: false
      - name: rate-limiting
        config:
          minute: 100
          hour: 5000
      - name: response-ratelimiting
        config:
          limits:
            agent_create:
              minute: 10
      - name: cors
        config:
          origins: ["https://app.aiworkflow.com"]
          methods: [GET, POST, PUT, PATCH, DELETE, OPTIONS]
          headers: [Authorization, Content-Type]
          exposed_headers: [X-RateLimit-Remaining]
          credentials: true
```

---

## 3. RESTful API 設計

### 3.1 資源命名規範

#### 資源 URL 示例

```
✅ 正確:
GET    /api/v1/agents                   # 獲取 Agent 列表
POST   /api/v1/agents                   # 創建 Agent
GET    /api/v1/agents/{id}              # 獲取單個 Agent
PUT    /api/v1/agents/{id}              # 完整更新 Agent
PATCH  /api/v1/agents/{id}              # 部分更新 Agent
DELETE /api/v1/agents/{id}              # 刪除 Agent

❌ 錯誤:
GET    /api/v1/getAgents                # 不要在 URL 中使用動詞
POST   /api/v1/agent/create             # 不要使用單數 + 動作
GET    /api/v1/agents/list              # 列表操作用 GET /agents
DELETE /api/v1/deleteAgent/{id}         # 不要用動詞
```

### 3.2 HTTP 動詞和狀態碼

| 操作 | HTTP 方法 | 成功狀態碼 | 說明 |
|------|----------|-----------|------|
| 列表查詢 | GET | 200 OK | 返回資源列表 |
| 詳情查詢 | GET | 200 OK | 返回單個資源 |
| 創建資源 | POST | 201 Created | Location header 包含新資源 URL |
| 完整更新 | PUT | 200 OK | 返回更新後的資源 |
| 部分更新 | PATCH | 200 OK | 返回更新後的資源 |
| 刪除資源 | DELETE | 204 No Content | 無響應體 |
| 異步操作 | POST | 202 Accepted | 返回操作 ID 用於輪詢 |

### 3.3 CRUD 操作範例

#### 3.3.1 創建 Agent (POST)

**請求**:
```http
POST /api/v1/agents HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Data Analyzer",
  "description": "Analyzes data and generates insights",
  "persona_id": "550e8400-e29b-41d4-a716-446655440000",
  "plugins": [
    {
      "name": "code_executor",
      "config": {
        "timeout": 300,
        "memory_limit": "512MB"
      }
    }
  ],
  "settings": {
    "temperature": 0.7,
    "max_tokens": 4000
  }
}
```

**響應**:
```http
HTTP/1.1 201 Created
Location: /api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7
Content-Type: application/json

{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "name": "Data Analyzer",
  "description": "Analyzes data and generates insights",
  "persona_id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "active",
  "plugins": [
    {
      "id": "plugin-001",
      "name": "code_executor",
      "config": {
        "timeout": 300,
        "memory_limit": "512MB"
      }
    }
  ],
  "settings": {
    "temperature": 0.7,
    "max_tokens": 4000
  },
  "created_at": "2025-10-30T10:15:30Z",
  "updated_at": "2025-10-30T10:15:30Z",
  "_links": {
    "self": {
      "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7"
    },
    "execute": {
      "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7/execute"
    },
    "persona": {
      "href": "/api/v1/personas/550e8400-e29b-41d4-a716-446655440000"
    }
  }
}
```

#### 3.3.2 查詢 Agent 列表 (GET)

**請求**:
```http
GET /api/v1/agents?page=1&page_size=10&status=active&sort=created_at&order=desc HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**響應**:
```http
HTTP/1.1 200 OK
Content-Type: application/json
X-Total-Count: 45
X-Page: 1
X-Page-Size: 10
Link: </api/v1/agents?page=2&page_size=10>; rel="next",
      </api/v1/agents?page=5&page_size=10>; rel="last"

{
  "data": [
    {
      "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "name": "Data Analyzer",
      "description": "Analyzes data and generates insights",
      "status": "active",
      "created_at": "2025-10-30T10:15:30Z",
      "_links": {
        "self": {
          "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7"
        }
      }
    },
    {
      "id": "8d0e7780-e30c-42e5-b827-557766551111",
      "name": "Code Assistant",
      "description": "Helps with code generation",
      "status": "active",
      "created_at": "2025-10-29T15:20:10Z",
      "_links": {
        "self": {
          "href": "/api/v1/agents/8d0e7780-e30c-42e5-b827-557766551111"
        }
      }
    }
  ],
  "pagination": {
    "page": 1,
    "page_size": 10,
    "total_pages": 5,
    "total_count": 45
  },
  "_links": {
    "self": {
      "href": "/api/v1/agents?page=1&page_size=10"
    },
    "next": {
      "href": "/api/v1/agents?page=2&page_size=10"
    },
    "last": {
      "href": "/api/v1/agents?page=5&page_size=10"
    }
  }
}
```

#### 3.3.3 部分更新 Agent (PATCH)

**請求**:
```http
PATCH /api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7 HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "description": "Advanced data analyzer with ML capabilities",
  "settings": {
    "temperature": 0.8
  }
}
```

**響應**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "name": "Data Analyzer",
  "description": "Advanced data analyzer with ML capabilities",
  "persona_id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "active",
  "settings": {
    "temperature": 0.8,
    "max_tokens": 4000
  },
  "created_at": "2025-10-30T10:15:30Z",
  "updated_at": "2025-10-30T11:20:45Z",
  "_links": {
    "self": {
      "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7"
    }
  }
}
```

#### 3.3.4 刪除 Agent (DELETE)

**請求**:
```http
DELETE /api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7 HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**響應**:
```http
HTTP/1.1 204 No Content
```

### 3.4 異步操作模式

#### 3.4.1 執行 Agent (異步)

**請求**:
```http
POST /api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7/execute HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "input": {
    "query": "Analyze the sales data from Q3",
    "context": {
      "data_source": "postgres://sales_db"
    }
  },
  "config": {
    "timeout": 300,
    "priority": "high"
  }
}
```

**響應 (立即返回)**:
```http
HTTP/1.1 202 Accepted
Location: /api/v1/executions/9f1e8891-f41d-43f6-c938-668877662222
Content-Type: application/json

{
  "execution_id": "9f1e8891-f41d-43f6-c938-668877662222",
  "status": "pending",
  "created_at": "2025-10-30T11:25:00Z",
  "_links": {
    "self": {
      "href": "/api/v1/executions/9f1e8891-f41d-43f6-c938-668877662222"
    },
    "agent": {
      "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7"
    }
  }
}
```

#### 3.4.2 輪詢執行狀態

**請求**:
```http
GET /api/v1/executions/9f1e8891-f41d-43f6-c938-668877662222 HTTP/1.1
Host: api.aiworkflow.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**響應 (運行中)**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "execution_id": "9f1e8891-f41d-43f6-c938-668877662222",
  "agent_id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "status": "running",
  "progress": 45,
  "created_at": "2025-10-30T11:25:00Z",
  "started_at": "2025-10-30T11:25:02Z",
  "_links": {
    "self": {
      "href": "/api/v1/executions/9f1e8891-f41d-43f6-c938-668877662222"
    }
  }
}
```

**響應 (完成)**:
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "execution_id": "9f1e8891-f41d-43f6-c938-668877662222",
  "agent_id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "status": "completed",
  "progress": 100,
  "result": {
    "output": "Q3 sales analysis shows 15% growth...",
    "insights": [
      "Top performing region: North America",
      "Fastest growing product: AI Tools"
    ]
  },
  "metrics": {
    "execution_time_ms": 3500,
    "tokens_used": 1250,
    "cost": 0.05
  },
  "created_at": "2025-10-30T11:25:00Z",
  "started_at": "2025-10-30T11:25:02Z",
  "completed_at": "2025-10-30T11:25:05Z",
  "_links": {
    "self": {
      "href": "/api/v1/executions/9f1e8891-f41d-43f6-c938-668877662222"
    },
    "agent": {
      "href": "/api/v1/agents/7c9e6679-7425-40de-944b-e07fc1f90ae7"
    }
  }
}
```

---

## 4. 認證授權

### 4.1 認證流程

```mermaid
sequenceDiagram
    participant Client
    participant Gateway as API Gateway
    participant Auth as Auth Service
    participant Resource as Resource Service

    Client->>Gateway: POST /api/v1/auth/login
    Gateway->>Auth: Validate Credentials
    Auth->>Auth: Generate JWT Token
    Auth-->>Gateway: JWT Token
    Gateway-->>Client: 200 OK + Token

    Note over Client: Store token securely

    Client->>Gateway: GET /api/v1/agents<br/>Authorization: Bearer {token}
    Gateway->>Gateway: Validate JWT
    alt Token Valid
        Gateway->>Resource: Forward Request
        Resource-->>Gateway: Response
        Gateway-->>Client: 200 OK + Data
    else Token Invalid
        Gateway-->>Client: 401 Unauthorized
    end
```

### 4.2 JWT Token 結構

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-123",
    "email": "user@example.com",
    "roles": ["user", "admin"],
    "permissions": [
      "agent:read",
      "agent:write",
      "workflow:read"
    ],
    "iat": 1698652800,
    "exp": 1698739200
  },
  "signature": "..."
}
```

### 4.3 授權模型 (RBAC)

#### 角色定義

```yaml
roles:
  admin:
    permissions:
      - "*:*"  # All operations on all resources

  developer:
    permissions:
      - "agent:*"
      - "persona:*"
      - "workflow:*"
      - "execution:read"
      - "code:execute"

  analyst:
    permissions:
      - "agent:read"
      - "agent:execute"
      - "workflow:read"
      - "sql:query"
      - "rag:search"

  viewer:
    permissions:
      - "agent:read"
      - "persona:read"
      - "workflow:read"
      - "execution:read"
```

#### 權限檢查

```csharp
[Authorize(Policy = "RequireAgentWritePermission")]
[HttpPost]
public async Task<IActionResult> CreateAgent([FromBody] CreateAgentRequest request)
{
    // Only users with "agent:write" permission can access
    var agent = await _agentService.CreateAsync(request);
    return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, agent);
}

// Policy definition in Startup.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAgentWritePermission", policy =>
        policy.RequireClaim("permissions", "agent:write", "agent:*", "*:*"));
});
```

### 4.4 API Key 認證 (替代方案)

```http
GET /api/v1/agents HTTP/1.1
Host: api.aiworkflow.com
X-API-Key: ak_live_1234567890abcdef
```

**API Key 管理**:
```json
{
  "api_key_id": "key-001",
  "key": "ak_live_1234567890abcdef",
  "name": "Production API Key",
  "scopes": ["agent:read", "agent:execute"],
  "rate_limit": {
    "requests_per_minute": 100,
    "requests_per_day": 10000
  },
  "created_at": "2025-10-30T10:00:00Z",
  "expires_at": "2026-10-30T10:00:00Z",
  "last_used_at": "2025-10-30T11:30:00Z"
}
```

---

## 5. 請求響應格式

### 5.1 標準請求頭

```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
Accept: application/json
Accept-Language: en-US,zh-TW
X-Request-ID: 550e8400-e29b-41d4-a716-446655440000
User-Agent: AIWorkflow-Client/1.0
```

### 5.2 標準響應頭

```http
Content-Type: application/json; charset=utf-8
X-Request-ID: 550e8400-e29b-41d4-a716-446655440000
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1698652860
X-Response-Time: 45ms
Cache-Control: no-cache, no-store, must-revalidate
```

### 5.3 日期時間格式

統一使用 **ISO 8601** 格式:

```json
{
  "created_at": "2025-10-30T10:15:30Z",          // UTC
  "updated_at": "2025-10-30T10:15:30.123Z",      // UTC with milliseconds
  "scheduled_at": "2025-10-30T10:15:30+08:00"    // With timezone
}
```

### 5.4 響應包裝格式

#### 成功響應

```json
{
  "data": {
    "id": "agent-123",
    "name": "Data Analyzer"
  },
  "_links": {
    "self": {
      "href": "/api/v1/agents/agent-123"
    }
  }
}
```

#### 列表響應

```json
{
  "data": [
    { "id": "1", "name": "Agent 1" },
    { "id": "2", "name": "Agent 2" }
  ],
  "pagination": {
    "page": 1,
    "page_size": 10,
    "total_pages": 5,
    "total_count": 45
  },
  "_links": {
    "self": { "href": "/api/v1/agents?page=1" },
    "next": { "href": "/api/v1/agents?page=2" },
    "last": { "href": "/api/v1/agents?page=5" }
  }
}
```

---

## 6. 錯誤處理

### 6.1 錯誤響應格式

```json
{
  "error": {
    "code": "INVALID_REQUEST",
    "message": "The request is invalid",
    "details": [
      {
        "field": "name",
        "issue": "Name is required"
      },
      {
        "field": "persona_id",
        "issue": "Invalid UUID format"
      }
    ],
    "request_id": "550e8400-e29b-41d4-a716-446655440000",
    "timestamp": "2025-10-30T11:30:00Z",
    "documentation_url": "https://docs.aiworkflow.com/errors/INVALID_REQUEST"
  }
}
```

### 6.2 HTTP 狀態碼定義

#### 2xx 成功

| 狀態碼 | 說明 | 使用場景 |
|-------|------|---------|
| 200 OK | 請求成功 | GET, PUT, PATCH |
| 201 Created | 資源創建成功 | POST |
| 202 Accepted | 異步操作已接受 | POST (異步) |
| 204 No Content | 操作成功無返回內容 | DELETE |

#### 4xx 客戶端錯誤

| 狀態碼 | 錯誤碼 | 說明 |
|-------|--------|------|
| 400 Bad Request | INVALID_REQUEST | 請求格式錯誤 |
| 401 Unauthorized | UNAUTHORIZED | 未提供認證或認證無效 |
| 403 Forbidden | FORBIDDEN | 無權訪問資源 |
| 404 Not Found | NOT_FOUND | 資源不存在 |
| 409 Conflict | CONFLICT | 資源衝突 (如重複創建) |
| 422 Unprocessable Entity | VALIDATION_ERROR | 驗證失敗 |
| 429 Too Many Requests | RATE_LIMIT_EXCEEDED | 超過速率限制 |

#### 5xx 服務器錯誤

| 狀態碼 | 錯誤碼 | 說明 |
|-------|--------|------|
| 500 Internal Server Error | INTERNAL_ERROR | 服務器內部錯誤 |
| 502 Bad Gateway | BAD_GATEWAY | 上游服務錯誤 |
| 503 Service Unavailable | SERVICE_UNAVAILABLE | 服務暫時不可用 |
| 504 Gateway Timeout | GATEWAY_TIMEOUT | 上游服務超時 |

### 6.3 錯誤響應示例

#### 400 Bad Request

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "error": {
    "code": "INVALID_REQUEST",
    "message": "Request validation failed",
    "details": [
      {
        "field": "name",
        "issue": "Name must be between 1 and 100 characters"
      }
    ],
    "request_id": "req-123",
    "timestamp": "2025-10-30T11:30:00Z"
  }
}
```

#### 401 Unauthorized

```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer realm="API", error="invalid_token"
Content-Type: application/json

{
  "error": {
    "code": "UNAUTHORIZED",
    "message": "Invalid or expired JWT token",
    "request_id": "req-124",
    "timestamp": "2025-10-30T11:31:00Z",
    "documentation_url": "https://docs.aiworkflow.com/auth"
  }
}
```

#### 404 Not Found

```http
HTTP/1.1 404 Not Found
Content-Type: application/json

{
  "error": {
    "code": "NOT_FOUND",
    "message": "Agent with id '7c9e6679-7425-40de-944b-e07fc1f90ae7' not found",
    "request_id": "req-125",
    "timestamp": "2025-10-30T11:32:00Z"
  }
}
```

#### 429 Too Many Requests

```http
HTTP/1.1 429 Too Many Requests
Retry-After: 60
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1698652920
Content-Type: application/json

{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded. Maximum 100 requests per minute.",
    "retry_after": 60,
    "request_id": "req-126",
    "timestamp": "2025-10-30T11:33:00Z",
    "documentation_url": "https://docs.aiworkflow.com/rate-limits"
  }
}
```

#### 500 Internal Server Error

```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
  "error": {
    "code": "INTERNAL_ERROR",
    "message": "An unexpected error occurred. Our team has been notified.",
    "request_id": "req-127",
    "timestamp": "2025-10-30T11:34:00Z",
    "documentation_url": "https://docs.aiworkflow.com/errors/500"
  }
}
```

---

## 7. 速率限制

### 7.1 限制策略

| 用戶類型 | 每分鐘請求數 | 每日請求數 | 並發連接數 |
|---------|------------|-----------|----------|
| Free | 60 | 1,000 | 5 |
| Basic | 100 | 10,000 | 10 |
| Pro | 300 | 50,000 | 20 |
| Enterprise | 1,000 | 無限制 | 100 |

### 7.2 速率限制響應頭

```http
X-RateLimit-Limit: 100           # 限制總數
X-RateLimit-Remaining: 95        # 剩餘請求數
X-RateLimit-Reset: 1698652920    # 重置時間 (Unix timestamp)
Retry-After: 60                  # 429 時建議重試時間 (秒)
```

### 7.3 Sliding Window 算法實現

```csharp
public class SlidingWindowRateLimiter
{
    private readonly IDistributedCache _cache;
    private const int WindowSizeSeconds = 60;
    private const int MaxRequests = 100;

    public async Task<RateLimitResult> CheckRateLimitAsync(string userId)
    {
        var key = $"ratelimit:{userId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - WindowSizeSeconds;

        // Get request timestamps from Redis (sorted set)
        var requests = await GetRequestTimestampsAsync(key, windowStart);

        if (requests.Count >= MaxRequests)
        {
            var oldestRequest = requests.First();
            var resetTime = oldestRequest + WindowSizeSeconds;

            return new RateLimitResult
            {
                IsAllowed = false,
                Remaining = 0,
                ResetAt = resetTime
            };
        }

        // Add current request timestamp
        await AddRequestTimestampAsync(key, now);

        return new RateLimitResult
        {
            IsAllowed = true,
            Remaining = MaxRequests - requests.Count - 1,
            ResetAt = now + WindowSizeSeconds
        };
    }
}
```

### 7.4 速率限制中間件

```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _rateLimiter;

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            await _next(context);
            return;
        }

        var result = await _rateLimiter.CheckRateLimitAsync(userId);

        context.Response.Headers["X-RateLimit-Limit"] = "100";
        context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = result.ResetAt.ToString();

        if (!result.IsAllowed)
        {
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = "60";
            await context.Response.WriteAsJsonAsync(new
            {
                error = new
                {
                    code = "RATE_LIMIT_EXCEEDED",
                    message = "Rate limit exceeded. Maximum 100 requests per minute."
                }
            });
            return;
        }

        await _next(context);
    }
}
```

---

## 8. 版本控制

### 8.1 版本策略

使用 **URI Versioning** (URL 路徑版本):

```
✅ 推薦:
/api/v1/agents
/api/v2/agents

❌ 不推薦:
/api/agents?version=1         # Query parameter
Accept: application/vnd.api.v1+json  # Header versioning
```

### 8.2 版本演進規則

| 變更類型 | 版本影響 | 範例 |
|---------|---------|------|
| 新增字段 (可選) | 無需變更版本 | 新增 `description` 字段 |
| 新增端點 | 無需變更版本 | 新增 `GET /agents/{id}/metrics` |
| 修改字段類型 | 需要新版本 | `status: string` → `status: enum` |
| 刪除字段 | 需要新版本 | 移除 `deprecated_field` |
| 修改端點行為 | 需要新版本 | 改變排序邏輯 |

### 8.3 版本並存

```yaml
# v1: Original API
GET /api/v1/agents/{id}
Response:
  id: "agent-123"
  name: "Data Analyzer"
  status: "active"  # String type

# v2: Enhanced API
GET /api/v2/agents/{id}
Response:
  id: "agent-123"
  name: "Data Analyzer"
  status:           # Object type
    state: "active"
    health: "healthy"
    last_check: "2025-10-30T11:00:00Z"
  metadata:         # New field
    version: "1.2.0"
    created_by: "user-456"
```

### 8.4 廢棄策略

```http
# v1 API 標記為廢棄
GET /api/v1/agents HTTP/1.1

HTTP/1.1 200 OK
Sunset: Wed, 31 Dec 2025 23:59:59 GMT
Deprecation: true
Link: </api/v2/agents>; rel="successor-version"
Warning: 299 - "API v1 is deprecated. Please migrate to v2 by 2025-12-31"

{
  "data": { ... },
  "_deprecation": {
    "sunset_date": "2025-12-31",
    "successor_version": "/api/v2/agents",
    "migration_guide": "https://docs.aiworkflow.com/migration/v1-to-v2"
  }
}
```

---

## 9. API 端點總覽

### 9.1 Agent Service API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/agents` | GET | 獲取 Agent 列表 |
| `/api/v1/agents` | POST | 創建 Agent |
| `/api/v1/agents/{id}` | GET | 獲取 Agent 詳情 |
| `/api/v1/agents/{id}` | PUT | 完整更新 Agent |
| `/api/v1/agents/{id}` | PATCH | 部分更新 Agent |
| `/api/v1/agents/{id}` | DELETE | 刪除 Agent |
| `/api/v1/agents/{id}/execute` | POST | 執行 Agent (異步) |
| `/api/v1/agents/{id}/plugins` | GET | 獲取 Agent 插件列表 |
| `/api/v1/agents/{id}/plugins` | POST | 添加插件 |

### 9.2 Persona Service API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/personas` | GET | 獲取 Persona 列表 |
| `/api/v1/personas` | POST | 創建 Persona |
| `/api/v1/personas/{id}` | GET | 獲取 Persona 詳情 |
| `/api/v1/personas/{id}` | PUT | 更新 Persona |
| `/api/v1/personas/{id}` | DELETE | 刪除 Persona |
| `/api/v1/personas/{id}/render` | POST | 渲染 Persona 模板 |
| `/api/v1/personas/{id}/validate` | POST | 驗證 Persona |

### 9.3 Code Sandbox API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/code/execute` | POST | 執行代碼 (異步) |
| `/api/v1/code/executions/{id}` | GET | 獲取執行結果 |
| `/api/v1/code/executions/{id}` | DELETE | 取消執行 |

### 9.4 Text-to-SQL API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/text-to-sql/query` | POST | 自然語言查詢 |
| `/api/v1/text-to-sql/schemas` | GET | 獲取數據庫 Schema |
| `/api/v1/text-to-sql/schemas/{id}/enhance` | POST | 增強 Schema 描述 |
| `/api/v1/text-to-sql/validate` | POST | 驗證 SQL 安全性 |

### 9.5 Knowledge RAG API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/knowledge/documents` | GET | 獲取文檔列表 |
| `/api/v1/knowledge/documents` | POST | 上傳文檔 |
| `/api/v1/knowledge/documents/{id}` | DELETE | 刪除文檔 |
| `/api/v1/knowledge/search` | POST | 向量搜索 |
| `/api/v1/knowledge/index` | POST | 索引文檔 (異步) |

### 9.6 Workflow Service API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/workflows` | GET | 獲取 Workflow 列表 |
| `/api/v1/workflows` | POST | 創建 Workflow |
| `/api/v1/workflows/{id}` | GET | 獲取 Workflow 詳情 |
| `/api/v1/workflows/{id}` | PUT | 更新 Workflow |
| `/api/v1/workflows/{id}` | DELETE | 刪除 Workflow |
| `/api/v1/workflows/{id}/execute` | POST | 執行 Workflow |

### 9.7 Execution Service API (共用)

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/executions` | GET | 獲取執行歷史 |
| `/api/v1/executions/{id}` | GET | 獲取執行詳情 |
| `/api/v1/executions/{id}/cancel` | POST | 取消執行 |
| `/api/v1/executions/{id}/retry` | POST | 重試執行 |

### 9.8 Auth Service API

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/v1/auth/login` | POST | 用戶登錄 |
| `/api/v1/auth/logout` | POST | 用戶登出 |
| `/api/v1/auth/refresh` | POST | 刷新 Token |
| `/api/v1/auth/me` | GET | 獲取當前用戶信息 |

---

## 總結

### API 設計核心要點

1. **RESTful 設計**: 資源導向、HTTP 動詞語義化
2. **統一規範**: URL、命名、響應格式一致
3. **安全優先**: JWT 認證、RBAC 授權、速率限制
4. **錯誤友好**: 清晰的錯誤碼和錯誤信息
5. **版本控制**: URI 版本化、向後兼容
6. **異步支持**: 長時間操作返回 202 Accepted
7. **HATEOAS**: 響應包含相關鏈接
8. **監控追蹤**: Request ID、Rate Limit Headers

### 下一步

✅ **Part 2.1 完成**: API 設計規範
📋 **接下來**: 05-OPENAPI-SPEC.yaml - 完整 OpenAPI 3.0 規格定義

---

**文檔版本**: v1.0.0
**最後更新**: 2025-10-30
**作者**: AI Workflow Platform Team
