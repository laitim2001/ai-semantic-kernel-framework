# Solution Architecture

## Visual Studio Solution 結構

```
AIWorkflow.sln
│
├── src/
│   ├── AIWorkflow.API/                    # Web API 項目
│   ├── AIWorkflow.Application/            # 應用層
│   ├── AIWorkflow.Domain/                 # 領域層
│   ├── AIWorkflow.Infrastructure/         # 基礎設施層
│   └── AIWorkflow.Shared/                 # 共享代碼
│
├── tests/
│   ├── AIWorkflow.UnitTests/              # 單元測試
│   ├── AIWorkflow.IntegrationTests/       # 集成測試
│   └── AIWorkflow.E2ETests/               # E2E 測試
│
└── docs/
    └── README.md
```

## Clean Architecture 分層

### 1. Domain Layer (核心層)
- **職責**: 業務邏輯和領域模型
- **依賴**: 無外部依賴
- **內容**:
  - Entities (實體)
  - Value Objects (值對象)
  - Domain Events (領域事件)
  - Interfaces (接口定義)

### 2. Application Layer (應用層)
- **職責**: 用例和應用邏輯
- **依賴**: Domain Layer
- **內容**:
  - Commands (命令)
  - Queries (查詢)
  - DTOs (數據傳輸對象)
  - Validators (驗證器)
  - Mappers (映射器)

### 3. Infrastructure Layer (基礎設施層)
- **職責**: 外部服務集成
- **依賴**: Application Layer, Domain Layer
- **內容**:
  - Data Access (EF Core)
  - External APIs
  - File Storage
  - Caching (Redis)
  - Messaging

### 4. Presentation Layer (表示層)
- **職責**: API 端點和中間件
- **依賴**: Application Layer
- **內容**:
  - Controllers
  - Middleware
  - Filters
  - SignalR Hubs

## 項目依賴圖

```
┌─────────────────┐
│   API Layer     │
└────────┬────────┘
         │ depends on
         ▼
┌─────────────────┐      ┌──────────────────┐
│ Application     │◄─────│ Infrastructure   │
└────────┬────────┘      └──────────────────┘
         │ depends on
         ▼
┌─────────────────┐
│   Domain        │  (No dependencies)
└─────────────────┘
```

## 核心項目說明

### AIWorkflow.API
```
AIWorkflow.API/
├── Controllers/
│   ├── UsersController.cs
│   ├── WorkflowsController.cs
│   └── AgentsController.cs
├── Middleware/
├── Filters/
├── Extensions/
├── Program.cs
└── appsettings.json
```

### AIWorkflow.Application
```
AIWorkflow.Application/
├── Features/
│   ├── Users/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   └── DTOs/
│   ├── Workflows/
│   └── Agents/
├── Common/
│   ├── Interfaces/
│   ├── Behaviors/
│   └── Exceptions/
└── DependencyInjection.cs
```

### AIWorkflow.Domain
```
AIWorkflow.Domain/
├── Entities/
│   ├── User.cs
│   ├── Workflow.cs
│   └── Agent.cs
├── ValueObjects/
├── Events/
├── Interfaces/
└── Exceptions/
```

### AIWorkflow.Infrastructure
```
AIWorkflow.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   └── Migrations/
├── Services/
│   ├── SemanticKernelService.cs
│   ├── CacheService.cs
│   └── FileStorageService.cs
├── ExternalApis/
└── DependencyInjection.cs
```

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
