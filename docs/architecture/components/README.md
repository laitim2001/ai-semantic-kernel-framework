# Architecture Components 導航

**Semantic Kernel Agentic Framework - Component Architecture**

本目錄包含系統各組件的詳細架構設計文檔,從總體架構文檔中提取。

---

## 📐 組件架構索引

### 核心組件

1. **[API Gateway](./api-gateway.md)**
   - API 路由和版本管理
   - 認證授權 (OAuth 2.0)
   - Rate Limiting
   - 請求驗證

2. **[Agent Service](./agent-service.md)**
   - Agent 執行引擎
   - Semantic Kernel 整合
   - Plugin 架構
   - 狀態管理

3. **[Code Interpreter Service](./code-interpreter-service.md)**
   - Python/R 執行環境
   - 4 層安全架構
   - Docker 沙箱設計
   - 資源限制

4. **[Knowledge Service](./knowledge-service.md)**
   - RAG 實作
   - Azure AI Search 整合
   - Embedding 管理
   - 檢索策略

5. **[Multi-Agent Orchestrator](./multi-agent-orchestrator.md)**
   - Agent 協作機制
   - 訊息路由
   - 任務分配
   - 狀態同步

6. **[Data Layer](./data-layer.md)**
   - PostgreSQL Schema
   - Redis 快取策略
   - 數據遷移
   - Multi-Tenant 設計

7. **[Frontend Architecture](./frontend-architecture.md)**
   - React + TypeScript
   - State Management
   - API 整合層
   - UI/UX 組件

---

## 🔗 文檔關聯

```
Architecture-Design-Document.md (總覽)
├── components/
│   ├── api-gateway.md
│   ├── agent-service.md
│   ├── code-interpreter-service.md
│   ├── knowledge-service.md
│   ├── multi-agent-orchestrator.md
│   ├── data-layer.md
│   └── frontend-architecture.md
├── ADR-006-agent-state-management.md
├── ADR-007-multi-agent-communication.md
├── ADR-008-code-interpreter-execution-model.md
├── database-schema.md
└── performance-scalability-strategy.md
```

---

## 📝 注意事項

**當前狀態**: 組件級文檔尚未從總體架構文檔中提取

**下一步行動**:
1. 從 Architecture-Design-Document.md 提取各組件詳細設計
2. 建立獨立的組件文檔
3. 保持總覽文檔的高層次視角

**提取原則**:
- 每個組件文檔獨立完整
- 包含 API 接口定義
- 包含數據模型
- 包含技術選型理由
- 包含與其他組件的整合點

---

**版本**: 1.0.0
**最後更新**: 2025-11-02
**狀態**: ⏳ 待提取
