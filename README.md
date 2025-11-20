# IPA Platform - Intelligent Process Automation

<div align="center">

![Status](https://img.shields.io/badge/Status-In%20Planning-blue)
![Sprint](https://img.shields.io/badge/Sprint-0%20(Infrastructure)-orange)
![Version](https://img.shields.io/badge/Version-MVP%201.0-green)
![License](https://img.shields.io/badge/License-Proprietary-red)

**基於 Microsoft Semantic Kernel 的企業級智能流程自動化平台**

[English](#) | [繁體中文](#)

</div>

---

## 📋 目錄

- [項目概覽](#項目概覽)
- [核心特性](#核心特性)
- [架構設計](#架構設計)
- [技術棧](#技術棧)
- [快速開始](#快速開始)
- [文檔導航](#文檔導航)
- [開發計劃](#開發計劃)
- [團隊與貢獻](#團隊與貢獻)
- [許可協議](#許可協議)

---

## 🎯 項目概覽

### 什麼是 IPA Platform?

IPA (Intelligent Process Automation) Platform 是一個基於 **Microsoft Semantic Kernel** 構建的企業級智能流程自動化解決方案，專為中型企業（500-2000人）的 IT 運維和客戶服務團隊設計。

### 核心價值主張

與傳統 RPA 工具（如 UiPath）相比，IPA Platform 提供：

| 特性 | IPA Platform | 傳統 RPA |
|------|--------------|----------|
| **AI 驅動決策** | ✅ Azure OpenAI GPT-4o | ❌ 規則引擎 |
| **主動預防模式** | ✅ Agent 自動巡檢 | ❌ 被動觸發 |
| **跨系統智能關聯** | ✅ ServiceNow + Dynamics + SharePoint | ❌ 單系統操作 |
| **人機協作學習** | ✅ Few-shot Learning | ❌ 無學習能力 |
| **檢查點機制** | ✅ YAML 配置高風險操作 | ⚠️ 簡單審批 |

### 商業價值

- 💰 **成本節省**: 預計月節省 $10,000+ 人力成本（40-50% 效率提升）
- ⚡ **響應速度**: IT 工單處理時間縮短 40%+，CS 問題解決時間縮短 50%+
- 🎯 **準確率**: Agent 準確率從 60%（Month 1）提升至 90%+（Month 12）
- 📊 **可見性**: 完整審計追蹤，實時監控儀表板

---

## 🚀 核心特性

### 1️⃣ AI Agent 編排引擎
基於 Microsoft Semantic Kernel，支持複雜多步驟工作流：
- **順序編排**: Task A → Task B → Task C
- **並行執行**: Task A + B + C 同時運行
- **條件分支**: IF-THEN-ELSE 邏輯
- **循環處理**: WHILE 循環和迭代

### 2️⃣ 主動巡檢模式
Agent 自動定時執行預防性檢查：
- 🔍 **服務器健康巡檢**: 每天 9:00 檢查 CPU/Memory/Disk
- 📧 **智能告警**: 異常自動發送 Teams 通知
- 📊 **趨勢分析**: 預測潛在問題（如磁碟空間即將不足）

### 3️⃣ 跨系統智能關聯
打破企業數據孤島，提供 360° 統一視圖：
- **ServiceNow**: 獲取工單歷史和 SLA 狀態
- **Dynamics 365**: 查詢客戶資料和訂單信息
- **SharePoint**: 提取文檔和知識庫
- **AI 分析**: LLM 關聯分析生成洞察報告

### 4️⃣ 人機協作檢查點
高風險操作需要人工確認：
```yaml
checkpoints:
  - step: "delete_database"
    type: "manual"
    approvers: ["admin@company.com"]
    timeout: "2h"
```

### 5️⃣ Few-shot Learning
Agent 從人工修正中學習：
- 記錄人工修改的決策
- 生成 Few-shot Examples
- 下次執行時自動應用改進

### 6️⃣ 可視化工作流編輯器
基於 React Flow 的拖拽式編輯器：
- 📐 **節點連接**: 可視化定義步驟間依賴
- 🎨 **實時預覽**: 立即看到執行流程
- 💾 **版本管理**: 支持工作流版本回滾

---

## 🏗️ 架構設計

### 系統架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│                         Client Layer                            │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐      │
│  │  Web UI       │  │  Mobile App   │  │  CLI Tool     │      │
│  │  (React 18)   │  │  (Future)     │  │  (Future)     │      │
│  └───────┬───────┘  └───────┬───────┘  └───────┬───────┘      │
└──────────┼──────────────────┼──────────────────┼───────────────┘
           │                  │                  │
           ▼                  ▼                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                      API Gateway (Kong)                         │
│  - Authentication (OAuth 2.0 + JWT)                            │
│  - Rate Limiting (100 req/min)                                 │
│  - Request Routing                                             │
└──────────┬──────────────────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Microservices Layer                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │  Workflow   │  │  Execution  │  │  Agent      │            │
│  │  Service    │  │  Service    │  │  Service    │            │
│  │  (Python)   │  │  (Python)   │  │  (Python)   │            │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘            │
└─────────┼─────────────────┼─────────────────┼───────────────────┘
          │                 │                 │
          ▼                 ▼                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Message Queue (RabbitMQ)                     │
└─────────────────────────────────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Data Layer                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ PostgreSQL  │  │   Redis     │  │ Azure Blob  │            │
│  │     16      │  │      7      │  │   Storage   │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
```

### 核心組件

| 組件 | 職責 | 技術棧 |
|------|------|--------|
| **Workflow Service** | 工作流 CRUD、版本管理 | FastAPI 0.100+, SQLAlchemy |
| **Execution Service** | 執行狀態機、步驟編排 | Python-statemachine |
| **Agent Service** | Semantic Kernel 集成、Tool 管理 | Microsoft Semantic Kernel |
| **API Gateway** | 認證、限流、路由 | Kong 3.4 |
| **Message Queue** | 異步任務、事件驅動 | RabbitMQ 3.12 |

---

## 🛠️ 技術棧

### Backend

| 技術 | 版本 | 用途 |
|------|------|------|
| **Python** | 3.11+ | 主要開發語言 |
| **FastAPI** | 0.100+ | REST API 框架 |
| **Semantic Kernel** | 1.0+ | AI Agent 框架 |
| **SQLAlchemy** | 2.0+ | ORM |
| **Pydantic** | 2.0+ | 數據驗證 |
| **Celery** | 5.3+ | 異步任務隊列 |

### Frontend

| 技術 | 版本 | 用途 |
|------|------|------|
| **React** | 18.2+ | UI 框架 |
| **TypeScript** | 5.0+ | 類型安全 |
| **Vite** | 5.0+ | 構建工具 |
| **Shadcn UI** | Latest | 組件庫 |
| **React Flow** | 11.10+ | 工作流編輯器 |
| **TanStack Query** | 5.0+ | 數據獲取 |

### Infrastructure

| 技術 | 版本 | 用途 |
|------|------|------|
| **Kubernetes (AKS)** | 1.28+ | 容器編排 |
| **PostgreSQL** | 16+ | 主數據庫 |
| **Redis** | 7.0+ | 緩存/Session |
| **RabbitMQ** | 3.12+ | 消息隊列 |
| **Prometheus** | 2.45+ | 監控指標 |
| **Grafana** | 10.0+ | 儀表板 |
| **ELK Stack** | 8.10+ | 日誌分析 |

### DevOps

| 技術 | 用途 |
|------|------|
| **GitHub Actions** | CI/CD 流水線 |
| **Docker** | 容器化 |
| **Helm** | Kubernetes 包管理 |
| **Trivy** | 安全掃描 |
| **SonarQube** | 代碼質量 |

---

## 🚦 快速開始

### 前置要求

- **Python**: 3.11+
- **Node.js**: 18+
- **Docker**: 20.10+
- **kubectl**: 1.28+
- **Azure 訂閱**: 用於 OpenAI 和 AKS

### 本地開發環境

```bash
# 1. 克隆倉庫
git clone https://github.com/laitim2001/ai-semantic-kernel-framework-project.git
cd ai-semantic-kernel-framework-project

# 2. 啟動基礎設施（PostgreSQL, Redis, RabbitMQ）
docker-compose up -d

# 3. 安裝 Python 依賴
cd backend
python -m venv venv
source venv/bin/activate  # Windows: venv\Scripts\activate
pip install -r requirements.txt

# 4. 數據庫遷移
alembic upgrade head

# 5. 啟動後端服務
uvicorn main:app --reload --port 8000

# 6. 安裝前端依賴（新終端）
cd ../frontend
npm install

# 7. 啟動前端開發服務器
npm run dev
```

訪問 http://localhost:5173 查看應用。

### 環境變量配置

創建 `.env` 文件：

```bash
# Azure OpenAI
AZURE_OPENAI_ENDPOINT=https://your-instance.openai.azure.com/
AZURE_OPENAI_API_KEY=your-api-key
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o

# Database
DATABASE_URL=postgresql://user:pass@localhost:5432/ipa_db

# Redis
REDIS_URL=redis://localhost:6379/0

# RabbitMQ
RABBITMQ_URL=amqp://guest:guest@localhost:5672/

# JWT
JWT_SECRET_KEY=your-secret-key-change-in-production
JWT_ALGORITHM=HS256
JWT_EXPIRE_MINUTES=30
```

---

## 📚 文檔導航

### 發現階段 (Discovery)
- [Product Brief](./docs/00-discovery/product-brief/product-brief.md) - 產品願景和商業價值
- [SCAMPER 分析](./docs/00-discovery/brainstorming/02-scamper-method-overview.md) - 28 個核心決策

### 規劃階段 (Planning)
- [PRD 主文檔](./docs/01-planning/prd/prd-main.md) - 產品需求規格
- [PRD 附錄 A](./docs/01-planning/prd/prd-appendix-a-features-1-7.md) - 功能 1-7 詳細規格
- [PRD 附錄 B](./docs/01-planning/prd/prd-appendix-b-features-8-14.md) - 功能 8-14 詳細規格
- [PRD 附錄 C](./docs/01-planning/prd/prd-appendix-c-api-specs.md) - API 規格 (OpenAPI 3.0)
- [UI/UX 設計規範](./docs/01-planning/ui-ux/ui-ux-design-spec.md) - 設計系統和用戶流程

### 架構階段 (Architecture)
- [技術架構（主文檔）](./docs/02-architecture/technical-architecture.md) - 系統架構概覽
- [技術架構 Part 2](./docs/02-architecture/technical-architecture-part2.md) - 核心模塊設計
- [技術架構 Part 3](./docs/02-architecture/technical-architecture-part3.md) - 安全與監控
- [Solutioning Gate Check](./docs/02-architecture/gate-check/solutioning-gate-check.md) - 架構評審

### 實施階段 (Implementation)
- [MVP 實施計劃](./docs/03-implementation/mvp-implementation-plan.md) - 12 週執行計劃
- [Sprint Status](./docs/03-implementation/sprint-status.yaml) - Sprint 進度追蹤（242 Story Points）
- [Sprint 0: Infrastructure](./docs/03-implementation/sprint-planning/sprint-0-infrastructure-foundation.md) - 基礎設施搭建（42 點）
- [Sprint 1: Core Services](./docs/03-implementation/sprint-planning/sprint-1-core-services.md) - 核心服務開發（45 點）
- [Sprint 2: Integrations](./docs/03-implementation/sprint-planning/sprint-2-integrations.md) - 集成與擴展（40 點）
- [Sprint 3: Security](./docs/03-implementation/sprint-planning/sprint-3-security-observability.md) - 安全與可觀測性（38 點）
- [Sprint 4: UI/Frontend](./docs/03-implementation/sprint-planning/sprint-4-ui-frontend.md) - UI 與前端（42 點）
- [Sprint 5: Testing/Launch](./docs/03-implementation/sprint-planning/sprint-5-testing-launch.md) - 測試與上線（35 點）

---

## 📅 開發計劃

### 時間線概覽

```
2025-11-25              2025-12-20              2026-01-17              2026-02-14
    │                       │                       │                       │
    ├─ Sprint 0 ────────────┤                       │                       │
    │  Infrastructure       │                       │                       │
    │  (42 Points)          │                       │                       │
    │                       ├─ Sprint 1 ────────────┤                       │
    │                       │  Core Services        │                       │
    │                       │  (45 Points)          │                       │
    │                       │                       ├─ Sprint 2 ────────────┤
    │                       │                       │  Integrations         │
    │                       │                       │  (40 Points)          │
    │                       │                       │                       ├─ Sprint 3-5
    │                       │                       │                       │  Security/UI
    │                       │                       │                       │  Testing
    ▼                       ▼                       ▼                       ▼
  Week 1-2                Week 3-4                Week 5-6                Week 7-12
```

### 當前狀態

- **當前階段**: Sprint 0 準備階段
- **開始日期**: 2025-11-25（本週一）
- **下一里程碑**: 基礎設施就緒（2025-12-06）

### Sprint 0 關鍵任務

| 任務 ID | 描述 | 負責人 | Story Points | 狀態 |
|---------|------|--------|--------------|------|
| S0-1 | Development Environment Setup | DevOps | 5 | 🔜 Not Started |
| S0-2 | Kubernetes Cluster Setup (AKS) | DevOps | 8 | 🔜 Not Started |
| S0-3 | CI/CD Pipeline (GitHub Actions) | DevOps | 8 | 🔜 Not Started |
| S0-4 | Database Infrastructure (PostgreSQL) | Backend | 5 | 🔜 Not Started |
| S0-5 | Redis Cache Setup | Backend | 3 | 🔜 Not Started |
| S0-6 | RabbitMQ Message Queue | Backend | 3 | 🔜 Not Started |
| S0-7 | OAuth 2.0 Authentication | Backend | 8 | 🔜 Not Started |
| S0-8 | Monitoring Stack (Prometheus/Grafana) | DevOps | 5 | 🔜 Not Started |
| S0-9 | Logging Infrastructure (ELK) | DevOps | 5 | 🔜 Not Started |

---

## 👥 團隊與貢獻

### 核心團隊

| 角色 | 人數 | 職責 |
|------|------|------|
| **Backend Engineers** | 3 | Python 服務開發、Semantic Kernel 集成 |
| **Frontend Engineers** | 2 | React UI、工作流編輯器 |
| **DevOps Engineer** | 1 | K8s、CI/CD、監控 |
| **QA Engineer** | 1 | 測試策略、自動化測試 |
| **Product Owner** | 1 | 需求管理、Backlog 優先級 |

### 貢獻指南

我們歡迎貢獻！請閱讀 [CONTRIBUTING.md](./CONTRIBUTING.md) 了解：
- 代碼規範和風格指南
- Pull Request 流程
- 測試要求
- 文檔編寫標準

### 開發規範

- **Python**: 遵循 PEP 8，使用 `black` 格式化
- **TypeScript**: 使用 ESLint + Prettier
- **Commit Message**: 遵循 Conventional Commits
  - `feat:` 新功能
  - `fix:` 修復 Bug
  - `docs:` 文檔更新
  - `refactor:` 代碼重構
  - `test:` 測試相關
  - `chore:` 構建/工具變更

---

## 📊 項目狀態

### 里程碑完成度

- ✅ **Phase 0 - Discovery**: 已完成（SCAMPER 分析，28 個核心決策）
- ✅ **Phase 1 - Planning**: 已完成（PRD、UI/UX 設計）
- ✅ **Phase 2 - Architecture**: 已完成（技術架構、Solutioning Gate Check）
- 🔄 **Phase 3 - Implementation**: 進行中（Sprint 0 準備）

### 關鍵指標

| 指標 | 目標 | 當前 |
|------|------|------|
| **Total Story Points** | 242 | 0 (0%) |
| **Sprints Completed** | 6 | 0 |
| **Test Coverage** | > 80% | N/A |
| **API Endpoints** | 50+ | 0 |
| **UI Components** | 30+ | 0 |

---

## 🔒 安全性

### 安全特性

- **認證**: OAuth 2.0 + JWT with Azure AD
- **授權**: RBAC (4 角色: Admin, User, Viewer, Agent)
- **加密**: AES-256-GCM 靜態加密
- **密鑰管理**: Azure Key Vault with Managed Identity
- **API 安全**: Rate Limiting (100 req/min), CORS, Input Validation
- **審計**: 完整操作日誌（誰、何時、做了什麼）

### 漏洞報告

如發現安全漏洞，請通過以下方式報告：
- **Email**: security@company.com
- **Severity**: 標註 [CRITICAL] / [HIGH] / [MEDIUM] / [LOW]
- **Response Time**: 24 小時內確認，7 天內修復

---

## 📄 許可協議

本項目採用 **Proprietary License**。未經授權，禁止：
- 複製、分發或修改源代碼
- 用於商業目的
- 反向工程

© 2025 Company Name. All Rights Reserved.

---

## 📞 聯絡方式

- **Product Owner**: po@company.com
- **Technical Lead**: tech-lead@company.com
- **Support**: support@company.com
- **Documentation**: https://docs.ipa-platform.com

---

## 🙏 致謝

感謝以下開源項目和工具：
- [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel)
- [FastAPI](https://fastapi.tiangolo.com/)
- [React Flow](https://reactflow.dev/)
- [Shadcn UI](https://ui.shadcn.com/)

---

<div align="center">

**Built with ❤️ by IPA Platform Team**

[⬆ 回到頂部](#ipa-platform---intelligent-process-automation)

</div>
