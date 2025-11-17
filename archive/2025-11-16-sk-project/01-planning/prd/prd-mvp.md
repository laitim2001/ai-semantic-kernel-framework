# PRD：Semantic Kernel Agent 管理平台 - MVP

**文件版本：** 1.0  
**日期：** 2025-11-15  
**狀態：** 草稿  
**作者：** 產品團隊  
**基於：** [Product Brief v2.0](../../00-discovery/product-brief/product-brief.md)

---

## 文件概述

### 目的
本文件詳細定義 **Semantic Kernel Agent 管理平台 MVP（第一階段，0-3個月）** 的產品需求，包括功能規格、使用者故事、驗收標準、API 設計、資料模型和非功能需求。

### 目標使用者
**主要：** 內部開發者團隊（4 人）  
**次要：** 未來的業務團隊使用者（Phase 2）

### MVP 範圍
**包含（In Scope）：**
- ✅ Agent 生命週期管理（CRUD）
- ✅ 兩種 Agent 定義模式（程式碼、配置檔）
- ✅ SK 核心功能完整包裝
- ✅ 基礎 RAG 知識庫（文件上傳、向量化、檢索）
- ✅ REST API（Agent 呼叫、管理）
- ✅ Web UI（Agent 管理、測試對話）
- ✅ 基礎可觀察性（日誌、執行歷史）

**不包含（Out of Scope - 延後到 Phase 2+）：**
- ❌ 視覺化拖拉式 Agent 編輯器
- ❌ n8n、Teams 等外部系統整合
- ❌ 進階 RAG（混合檢索、Reranking）
- ❌ RBAC 權限管理
- ❌ 多租戶架構
- ❌ OAuth/SSO 身份驗證
- ❌ 分散式追蹤（OpenTelemetry）

### 成功標準
1. ✅ 平台可在 30 分鐘內部署完成（Docker Compose）
2. ✅ 開發者可在 15 分鐘內創建第一個 Agent
3. ✅ 5+ 個不同用途的 Agents 成功運行
4. ✅ Agent 執行成功率 > 99%
5. ✅ API 回應時間 < 500ms（p95）
6. ✅ 基礎文件完整（部署、快速開始、API 參考）

---

## 功能需求詳細規格

### FR-1：Agent 管理核心

#### FR-1.1：Agent CRUD 操作

**需求描述：**  
使用者可以通過 Web UI 或 API 創建、讀取、更新、刪除 Agents。

**功能細節：**

**1. 創建 Agent（Create）**
- 支援兩種定義模式：
  - **程式碼模式**：上傳 C# Plugin DLL 或直接貼上程式碼
  - **配置模式**：填寫 YAML/JSON 配置檔
- 必填欄位：
  - `name`（Agent 名稱，唯一）
  - `description`（Agent 描述）
  - `type`（Agent 類型：`chat-completion`、`rag`、`function`）
  - `definition_mode`（定義模式：`code`、`config`）
- 選填欄位：
  - `system_prompt`（系統提示詞）
  - `model_config`（LLM 模型配置：`model_name`、`temperature`、`max_tokens`）
  - `skills`（關聯的 Skills 列表）
  - `knowledge_bases`（關聯的知識庫 ID 列表）
  - `tags`（標籤，用於分類）

**2. 讀取 Agent（Read）**
- 列表頁面：顯示所有 Agents，支援篩選和搜尋
  - 篩選條件：`type`、`tags`、`status`
  - 搜尋：`name`、`description` 模糊搜尋
  - 排序：`created_at`、`updated_at`、`name`
- 詳情頁面：顯示單一 Agent 完整資訊
  - 基本資訊（名稱、描述、類型、狀態）
  - 配置詳情（系統提示詞、模型設定、Skills）
  - 執行統計（總呼叫次數、成功率、平均回應時間）
  - 最近執行記錄（最新 10 筆）

**3. 更新 Agent（Update）**
- 允許修改欄位：
  - `description`、`system_prompt`、`model_config`
  - `skills`、`knowledge_bases`、`tags`
- **不可修改欄位**：
  - `name`（Agent 唯一識別符，用於 API 路徑）⚠️
  - `type`（Agent 類型）
  - `definition_mode`（定義模式）
- 更新後自動建立版本記錄

**設計說明：**
`name` 用於 API 端點路徑 (`/api/v1/agents/{name}/invoke`)，修改會中斷現有 API 呼叫。如需顯示名稱可使用 `description` 欄位。

**4. 刪除 Agent（Delete）**
- 軟刪除：標記為 `deleted`，保留執行歷史
- 確認對話框：防止誤刪
- 刪除後 Agent 無法執行，但歷史記錄可查詢

**驗收標準：**
- [ ] 可通過 UI 創建程式碼模式和配置模式 Agent
- [ ] Agent 列表頁面正確顯示所有 Agents
- [ ] 篩選和搜尋功能正常運作
- [ ] Agent 詳情頁面顯示完整資訊
- [ ] 可成功更新 Agent 配置
- [ ] 刪除 Agent 後無法執行，但歷史記錄保留

---

#### FR-1.2：Agent 狀態管理

**需求描述：**  
Agent 有明確的生命週期狀態，使用者可以啟動、停止、重啟 Agent。

**狀態定義：**
- `draft`（草稿）：剛創建，尚未啟動
- `active`（啟用）：正常運行，可接受請求
- `inactive`（停用）：暫停使用，不接受請求
- `error`（錯誤）：配置錯誤或執行失敗
- `deleted`（已刪除）：軟刪除狀態

**狀態轉換規則：**
```
draft → active（啟動）
active ↔ inactive（停用/啟用）
active/inactive/error → deleted（刪除）
任何狀態 → error（配置驗證失敗或執行錯誤）
```

**功能細節：**
- **啟動 Agent**：驗證配置 → 初始化 Kernel → 變更狀態為 `active`
- **停用 Agent**：停止接受新請求 → 等待當前請求完成 → 變更狀態為 `inactive`
- **重啟 Agent**：停用 → 重新初始化 → 啟動

**驗收標準：**
- [ ] Agent 狀態在 UI 正確顯示
- [ ] 可手動啟動/停用 Agent
- [ ] `inactive` 狀態的 Agent 拒絕新請求並返回錯誤
- [ ] 配置錯誤時 Agent 自動進入 `error` 狀態
- [ ] 狀態變更記錄在日誌中

---

#### FR-1.3：Agent 版本控制

**需求描述：**  
每次更新 Agent 配置時，自動建立版本快照，支援回滾到歷史版本。

**版本資訊：**
- `version_number`（版本號，自動遞增：v1, v2, v3...）
- `created_at`（建立時間）
- `created_by`（建立者，MVP 可簡化為 "admin"）
- `change_summary`（變更摘要，使用者填寫或自動生成）
- `config_snapshot`（完整配置快照，JSON 格式）

**功能細節：**
- 創建 Agent 時自動建立 v1
- 每次更新時建立新版本
- 版本列表頁面顯示所有歷史版本
- 可查看任一版本的配置詳情
- 可回滾到指定版本（實際上是複製該版本配置並建立新版本）

**驗收標準：**
- [ ] 更新 Agent 後自動建立新版本
- [ ] 版本列表正確顯示歷史版本
- [ ] 可查看任一版本的完整配置
- [ ] 回滾功能正常運作

---

#### FR-1.4：Agent 測試介面

**需求描述：**  
提供對話測試介面，開發者可以快速測試 Agent 行為，無需通過 API。

**功能細節：**
- **測試對話窗口**：
  - 輸入框：使用者輸入測試訊息
  - 對話歷史：顯示完整對話過程
  - 清除按鈕：重置對話歷史
- **執行詳情面板**（可展開/收合）：
  - Function Calling 記錄（呼叫了哪些 Functions）
  - 知識庫檢索結果（RAG Agent）
  - Token 使用統計
  - 執行時間
- **配置快速調整**：
  - Temperature 滑桿（0.0 - 1.0）
  - Max Tokens 輸入框
  - System Prompt 編輯框
  - 即時生效，無需儲存

**驗收標準：**
- [ ] 測試對話介面可正常發送訊息並接收回應
- [ ] 對話歷史正確顯示
- [ ] 執行詳情面板顯示 Function Calling 和 RAG 檢索記錄
- [ ] 配置快速調整功能正常運作
- [ ] Token 使用和執行時間正確統計

---

### FR-2：Semantic Kernel 功能包裝

#### FR-2.1：Kernel 初始化與配置

**需求描述：**  
平台內部管理 Semantic Kernel 的 `Kernel` 實例，支援配置 LLM 服務。

**配置項目：**

**1. LLM 服務配置**
- **Azure OpenAI**：
  - `endpoint`（端點 URL）
  - `api_key`（API 金鑰，加密存儲）
  - `deployment_name`（部署名稱）
  - `api_version`（API 版本，預設 `2024-10-01-preview`）
  
- **OpenAI**：
  - `api_key`（API 金鑰，加密存儲）
  - `organization_id`（組織 ID，選填）

**2. 預設模型配置**
- `default_model`（預設模型：`gpt-4o`、`gpt-4o-mini`、`gpt-35-turbo`）
- `default_temperature`（預設溫度：0.7）
- `default_max_tokens`（預設最大 Tokens：2000）

**3. Kernel 服務配置**
- `timeout`（請求逾時，秒：30）
- `retry_policy`（重試策略：指數退避）
- `max_retries`（最大重試次數：3）

**功能細節：**
- 系統啟動時從配置檔或環境變數讀取 LLM 配置
- 為每個 Agent 建立獨立的 `Kernel` 實例
- 支援在 Agent 層級覆寫預設模型配置
- 配置變更後無需重啟服務（熱重載）

**驗收標準：**
- [ ] 可通過配置檔設定 Azure OpenAI 或 OpenAI
- [ ] Agent 可使用預設模型配置
- [ ] Agent 可覆寫模型配置（temperature、max_tokens）
- [ ] 配置變更後新 Agent 使用新配置

---

#### FR-2.2：ChatCompletionAgent 支援

**需求描述：**  
完整包裝 Semantic Kernel 的 `ChatCompletionAgent`，支援對話式 Agent。

**功能細節：**

**1. Agent 創建**
```csharp
// 平台內部實作
var agent = new ChatCompletionAgent
{
    Name = agentConfig.Name,
    Instructions = agentConfig.SystemPrompt,
    Kernel = kernel,
    Arguments = new KernelArguments
    {
        { "temperature", agentConfig.Temperature },
        { "max_tokens", agentConfig.MaxTokens }
    }
};
```

**2. 對話管理**
- 自動維護對話歷史（`ChatHistory`）
- 支援多輪對話（session 管理）
- 可設定對話歷史保留數量（預設最近 10 輪）

**3. System Prompt 支援**
- 允許動態設定 System Prompt
- 支援變數替換（例如：`{{user_name}}`、`{{current_date}}`）
- Prompt 模板管理（預設模板庫）

**驗收標準：**
- [ ] 可創建 `ChatCompletionAgent` 類型的 Agent
- [ ] Agent 可正確回應使用者訊息
- [ ] 多輪對話歷史正確維護
- [ ] System Prompt 可動態設定和變數替換

---

#### FR-2.3：Function Calling 支援

**需求描述：**  
支援 Semantic Kernel 的 Native Functions 和 Prompt Functions，Agent 可呼叫 Functions 執行任務。

**功能細節：**

**1. Native Function 定義**
- 開發者可撰寫 C# 方法作為 Function
- 使用 SK 的 `[KernelFunction]` 標註
- 支援參數定義和描述（讓 LLM 理解 Function 用途）

範例：
```csharp
public class WeatherPlugin
{
    [KernelFunction]
    [Description("Get current weather for a city")]
    public async Task<string> GetWeather(
        [Description("City name")] string city,
        [Description("Temperature unit: Celsius or Fahrenheit")] string unit = "Celsius"
    )
    {
        // 實作邏輯
        return $"Weather in {city}: 25°{unit[0]}";
    }
}
```

**2. Plugin 管理**
- Plugin 倉庫：存放可重用的 Plugins
- Plugin 註冊：將 Plugin 註冊到 Kernel
- Agent 關聯：在 Agent 配置中指定可用的 Plugins

**3. Function Calling 執行流程**
```
User Input → LLM 決定是否呼叫 Function
          ↓
      呼叫 Function（傳遞參數）
          ↓
      取得 Function 結果
          ↓
      LLM 根據結果生成最終回應
```

**驗收標準：**
- [ ] 可定義和註冊 Native Functions
- [ ] Agent 可正確呼叫 Functions
- [ ] Function 參數正確傳遞
- [ ] Function 執行記錄在日誌中

---

#### FR-2.4：Prompt Template 管理

**需求描述：**  
支援 Semantic Kernel 的 Prompt Functions，使用者可以創建和管理 Prompt 模板。

**功能細節：**

**1. Prompt 模板結構**
```yaml
name: summarize_text
description: Summarize long text into key points
template: |
  Summarize the following text in {{max_points}} bullet points:
  
  {{$input}}
parameters:
  - name: max_points
    description: Number of bullet points
    default: 5
```

**2. Prompt 模板管理**
- 模板列表：顯示所有可用模板
- 創建模板：通過 UI 或 YAML 定義
- 測試模板：輸入範例資料測試輸出
- 版本控制：模板變更記錄

**3. 模板使用**
- Agent 可引用模板作為 Function
- 支援變數替換和參數傳遞

**驗收標準：**
- [ ] 可創建和編輯 Prompt 模板
- [ ] 模板列表正確顯示
- [ ] 模板測試功能正常
- [ ] Agent 可呼叫 Prompt Functions

---

### FR-3：知識庫與 RAG

#### FR-3.1：知識庫管理

**需求描述：**  
使用者可以創建多個知識庫，每個知識庫管理一組文件。

**技術架構說明：**  
為了支援開發環境（Qdrant）和生產環境（Azure AI Search）的切換，實作向量資料庫抽象層：

```csharp
public interface IVectorStore
{
    Task<string> CreateCollectionAsync(string collectionName, int vectorSize);
    Task<string> UpsertVectorAsync(string collectionName, VectorRecord record);
    Task<List<VectorRecord>> UpsertBatchAsync(string collectionName, List<VectorRecord> records);
    Task<List<SearchResult>> SearchAsync(string collectionName, float[] queryVector, int topK, float threshold);
    Task<bool> DeleteVectorAsync(string collectionName, string id);
    Task<bool> DeleteCollectionAsync(string collectionName);
}

// 實作類別
public class QdrantVectorStore : IVectorStore { /* Qdrant 實作 */ }
public class AzureAISearchVectorStore : IVectorStore { /* Azure AI Search 實作 */ }
```

通過配置檔切換：
```json
{
  "VectorStore": {
    "Provider": "Qdrant",  // 或 "AzureAISearch"
    "Qdrant": {
      "Endpoint": "http://localhost:6333"
    },
    "AzureAISearch": {
      "Endpoint": "https://your-search.search.windows.net",
      "ApiKey": "your-api-key"
    }
  }
}
```

**功能細節：**

**1. 知識庫 CRUD**
- **創建知識庫**：
  - `name`（知識庫名稱）
  - `description`（描述）
  - `embedding_model`（Embedding 模型：`text-embedding-ada-002`、`text-embedding-3-small`）
  - `chunk_size`（文件切塊大小，字元：1000）
  - `chunk_overlap`（切塊重疊，字元：200）
- **知識庫列表**：顯示所有知識庫及文件數量
- **知識庫詳情**：顯示配置和文件列表
- **更新知識庫**：修改配置（需重新向量化）
- **刪除知識庫**：軟刪除，保留向量資料

**2. 文件管理**
- **上傳文件**：
  - 支援格式：PDF、TXT、Markdown、Word（.docx）
  - 單檔大小限制：10MB
  - 批次上傳：最多 10 個檔案
- **文件列表**：顯示檔名、大小、上傳時間、向量化狀態
- **文件預覽**：顯示文件內容（純文字）
- **刪除文件**：刪除文件及其向量資料

**3. 向量化流程**
```
上傳文件 → 提取文字 → 文件切塊（Chunking）
        ↓
    生成 Embeddings（呼叫 LLM）
        ↓
    存入向量資料庫（Qdrant/Azure AI Search）
        ↓
    更新文件狀態為「已完成」
```

**驗收標準：**
- [ ] 可創建和管理多個知識庫
- [ ] 可上傳 PDF、TXT、Markdown、Word 文件
- [ ] 文件自動切塊和向量化
- [ ] 文件列表正確顯示狀態
- [ ] 可刪除文件和知識庫

---

#### FR-3.2：向量檢索

**需求描述：**  
實作基礎向量檢索，支援 RAG Agent 查詢知識庫。

**功能細節：**

**1. 檢索參數**
- `query`（查詢文字）
- `knowledge_base_id`（知識庫 ID）
- `top_k`（返回最相關的前 K 個結果，預設 5）
- `similarity_threshold`（相似度閾值，0.0-1.0，預設 0.7）

**2. 檢索流程**
```
使用者查詢 → 生成查詢 Embedding
          ↓
      向量資料庫檢索（餘弦相似度）
          ↓
      過濾低於閾值的結果
          ↓
      返回 Top-K 結果（包含原文和相似度分數）
```

**3. 檢索結果格式**
```json
{
  "results": [
    {
      "chunk_id": "doc1_chunk_3",
      "text": "Semantic Kernel 是一個開源框架...",
      "similarity_score": 0.89,
      "metadata": {
        "document_name": "sk-introduction.pdf",
        "page": 3
      }
    }
  ]
}
```

**驗收標準：**
- [ ] 向量檢索功能正常運作
- [ ] 檢索結果按相似度排序
- [ ] 相似度閾值過濾生效
- [ ] 檢索結果包含原文和 metadata

---

#### FR-3.3：RAG Agent 整合

**需求描述：**  
RAG Agent 類型的 Agent 自動整合知識庫檢索功能。

**功能細節：**

**1. RAG Agent 配置**
```yaml
type: rag
knowledge_bases:
  - kb_id_1
  - kb_id_2
retrieval_config:
  top_k: 5
  similarity_threshold: 0.7
  rerank: false  # MVP 不支援
```

**2. RAG 執行流程**
```
使用者提問 → 向量檢索相關文件
          ↓
      將檢索結果注入 Prompt（作為 Context）
          ↓
      LLM 根據 Context 生成回答
          ↓
      返回回答（可選：附上引用來源）
```

**3. Prompt 模板（RAG）**
```
You are a helpful assistant. Answer the user's question based on the following context.

Context:
{{retrieved_documents}}

User Question:
{{user_question}}

Answer:
```

**驗收標準：**
- [ ] RAG Agent 可關聯多個知識庫
- [ ] 提問時自動檢索相關文件
- [ ] LLM 根據檢索結果回答
- [ ] 測試介面顯示檢索到的文件片段

---

### FR-4：REST API

#### FR-4.1：Agent 呼叫 API

**需求描述：**  
提供標準 REST API 供外部系統呼叫 Agent。

**API 規格：**

**端點：** `POST /api/v1/agents/{agent_name}/invoke`

**請求格式：**
```json
{
  "message": "What is Semantic Kernel?",
  "session_id": "optional-session-id",
  "parameters": {
    "temperature": 0.7,
    "max_tokens": 500
  }
}
```

**回應格式（成功）：**
```json
{
  "agent_name": "sk-assistant",
  "message": "Semantic Kernel is an open-source SDK...",
  "session_id": "sess_12345",
  "execution_id": "exec_67890",
  "metadata": {
    "model": "gpt-4o",
    "tokens_used": 234,
    "execution_time_ms": 1234,
    "functions_called": ["get_documentation"]
  }
}
```

**回應格式（錯誤）：**
```json
{
  "error": {
    "code": "AGENT_NOT_ACTIVE",
    "message": "Agent is not in active state",
    "details": "Agent 'sk-assistant' is currently inactive"
  }
}
```

**錯誤碼：**
- `AGENT_NOT_FOUND` (404)
- `AGENT_NOT_ACTIVE` (400)
- `INVALID_REQUEST` (400)
- `RATE_LIMIT_EXCEEDED` (429)
- `INTERNAL_ERROR` (500)

**驗收標準：**
- [ ] API 可成功呼叫 active 狀態的 Agent
- [ ] 回應格式符合規格
- [ ] 錯誤處理正確（Agent 不存在、非 active 狀態）
- [ ] Session ID 管理正確（多輪對話）

---

#### FR-4.2：Agent 管理 API

**需求描述：**  
提供 CRUD API 管理 Agents。

**API 列表：**

| 方法 | 端點 | 描述 |
|------|------|------|
| GET | `/api/v1/agents` | 列出所有 Agents |
| GET | `/api/v1/agents/{name}` | 取得 Agent 詳情 |
| POST | `/api/v1/agents` | 創建 Agent |
| PUT | `/api/v1/agents/{name}` | 更新 Agent |
| DELETE | `/api/v1/agents/{name}` | 刪除 Agent |
| POST | `/api/v1/agents/{name}/activate` | 啟動 Agent |
| POST | `/api/v1/agents/{name}/deactivate` | 停用 Agent |

**範例（創建 Agent）：**

**請求：** `POST /api/v1/agents`
```json
{
  "name": "customer-support-bot",
  "description": "Customer support assistant",
  "type": "chat-completion",
  "definition_mode": "config",
  "config": {
    "system_prompt": "You are a friendly customer support agent.",
    "model_config": {
      "model": "gpt-4o-mini",
      "temperature": 0.7,
      "max_tokens": 1000
    },
    "skills": ["email-plugin", "ticket-plugin"]
  }
}
```

**回應：** `201 Created`
```json
{
  "name": "customer-support-bot",
  "status": "draft",
  "created_at": "2025-11-15T10:30:00Z",
  "version": "v1"
}
```

**驗收標準：**
- [ ] 所有 CRUD API 正常運作
- [ ] 列表 API 支援篩選和分頁
- [ ] 創建 API 驗證必填欄位
- [ ] 啟動/停用 API 正確變更狀態

---

#### FR-4.3：知識庫 API

**需求描述：**  
提供 API 管理知識庫和文件。

**API 列表：**

| 方法 | 端點 | 描述 |
|------|------|------|
| GET | `/api/v1/knowledge-bases` | 列出所有知識庫 |
| POST | `/api/v1/knowledge-bases` | 創建知識庫 |
| POST | `/api/v1/knowledge-bases/{id}/documents` | 上傳文件 |
| GET | `/api/v1/knowledge-bases/{id}/documents` | 列出知識庫文件 |
| DELETE | `/api/v1/knowledge-bases/{id}/documents/{doc_id}` | 刪除文件 |
| POST | `/api/v1/knowledge-bases/{id}/search` | 檢索知識庫 |

**範例（檢索知識庫）：**

**請求：** `POST /api/v1/knowledge-bases/kb_123/search`
```json
{
  "query": "How to create an agent?",
  "top_k": 5,
  "similarity_threshold": 0.7
}
```

**回應：**
```json
{
  "results": [
    {
      "chunk_id": "doc5_chunk_2",
      "text": "To create an agent in Semantic Kernel...",
      "similarity_score": 0.92,
      "metadata": {
        "document_name": "agent-tutorial.md",
        "document_id": "doc_5"
      }
    }
  ],
  "query_time_ms": 234
}
```

**驗收標準：**
- [ ] 知識庫 CRUD API 正常運作
- [ ] 文件上傳 API 支援多檔案
- [ ] 檢索 API 返回正確結果
- [ ] API 文件（OpenAPI/Swagger）自動生成

---

### FR-5：Web UI

#### FR-5.1：使用者身份驗證

**需求描述：**  
簡單的使用者登入機制（MVP 簡化版）。

**功能細節：**
- **登入頁面**：使用者名稱 + 密碼
- **預設帳號**：`admin` / `admin123`（可通過環境變數修改）
- **Session 管理**：JWT Token，有效期 24 小時
- **登出功能**：清除 Token

**MVP 簡化：**
- 單一使用者（admin）
- 無註冊功能
- 無忘記密碼
- 無角色權限（Phase 2 再實作 RBAC）

**驗收標準：**
- [ ] 登入頁面正常運作
- [ ] 使用正確帳密可登入
- [ ] 登入後可存取所有頁面
- [ ] 未登入時自動導向登入頁面
- [ ] 登出功能正常

---

#### FR-5.2：Agent 管理頁面

**需求描述：**  
Web UI 提供完整的 Agent 管理功能。

**頁面結構：**

**1. Agent 列表頁面（`/agents`）**
- **表格欄位**：
  - Agent 名稱（可點擊進入詳情）
  - 類型（chat-completion / rag / function）
  - 狀態（draft / active / inactive / error / deleted）
  - 最後更新時間
  - 操作按鈕（編輯、刪除、啟動/停用、測試）
- **篩選器**：
  - 類型（多選）
  - 狀態（多選）
  - 標籤（多選）
- **搜尋框**：模糊搜尋名稱和描述
- **排序**：依名稱、更新時間、狀態排序
- **新增按鈕**：導向創建頁面

**2. Agent 創建頁面（`/agents/new`）**
- **基本資訊區塊**：
  - Agent 名稱（必填）
  - 描述（選填，多行文字）
  - 類型（下拉選單）
  - 定義模式（Code / Config，單選）
  - 標籤（多選或自行輸入）
- **配置區塊（若選擇 Config 模式）**：
  - System Prompt（多行文字編輯器）
  - 模型配置：
    - Model（下拉選單：gpt-4o, gpt-4o-mini, gpt-35-turbo）
    - Temperature（滑桿：0.0 - 1.0）
    - Max Tokens（數字輸入：100 - 4000）
  - Skills（多選清單，從 Plugin 倉庫選擇）
  - Knowledge Bases（多選清單，若類型為 RAG）
- **程式碼區塊（若選擇 Code 模式）**：
  - 上傳 DLL 檔案
  - 或貼上 C# 程式碼（語法高亮編輯器）
- **按鈕**：儲存草稿、儲存並啟動、取消

**3. Agent 詳情頁面（`/agents/{name}`）**
- **基本資訊卡片**：
  - 名稱、描述、類型、狀態
  - 建立時間、最後更新時間
  - 當前版本號
- **配置卡片**：
  - System Prompt（可摺疊）
  - 模型配置
  - Skills 列表
  - Knowledge Bases 列表
- **統計卡片**：
  - 總呼叫次數
  - 成功率
  - 平均回應時間
  - 總 Token 使用量
- **最近執行記錄**：
  - 表格顯示最新 10 筆
  - 欄位：時間、使用者輸入（截取前 50 字元）、狀態、執行時間
  - 可點擊查看完整對話
- **操作按鈕**：
  - 編輯、刪除、啟動/停用、測試
  - 版本歷史

**4. Agent 編輯頁面（`/agents/{name}/edit`）**
- 與創建頁面相同，但不可修改名稱和類型
- 儲存後自動建立新版本

**5. Agent 測試頁面（`/agents/{name}/test`）**
- **對話區域（左側，60% 寬度）**：
  - 對話歷史（訊息氣泡，使用者右側、Agent 左側）
  - 輸入框（多行文字，支援 Enter 送出）
  - 清除對話按鈕
- **配置區域（右側，40% 寬度）**：
  - 快速調整：
    - Temperature 滑桿
    - Max Tokens 輸入框
    - System Prompt 編輯框
  - 執行詳情（可摺疊）：
    - Token 使用統計
    - 執行時間
    - Function Calling 記錄
    - RAG 檢索結果（若為 RAG Agent）

**驗收標準：**
- [ ] Agent 列表頁面正確顯示所有 Agents
- [ ] 篩選、搜尋、排序功能正常
- [ ] 可通過 UI 創建 Config 模式 Agent
- [ ] Agent 詳情頁面顯示完整資訊
- [ ] 編輯頁面可更新配置
- [ ] 測試頁面可正常對話
- [ ] 執行詳情顯示正確資訊

---

#### FR-5.3：知識庫管理頁面

**需求描述：**  
Web UI 提供知識庫和文件管理功能。

**頁面結構：**

**1. 知識庫列表頁面（`/knowledge-bases`）**
- **卡片式佈局**：
  - 知識庫名稱
  - 描述
  - 文件數量
  - 最後更新時間
  - 操作按鈕（查看、編輯、刪除）
- **新增按鈕**：創建知識庫

**2. 知識庫詳情頁面（`/knowledge-bases/{id}`）**
- **知識庫資訊卡片**：
  - 名稱、描述
  - Embedding 模型
  - Chunk Size、Chunk Overlap
  - 建立時間
- **文件列表表格**：
  - 檔名、大小、上傳時間
  - 向量化狀態（處理中 / 已完成 / 失敗）
  - 操作按鈕（預覽、刪除）
- **上傳文件按鈕**：
  - 拖放區域或選擇檔案
  - 支援批次上傳
  - 上傳進度條
- **測試檢索**：
  - 輸入查詢文字
  - 顯示檢索結果（相似度分數、原文片段）

**驗收標準：**
- [ ] 知識庫列表正確顯示
- [ ] 可創建知識庫
- [ ] 可上傳文件（PDF、TXT、Markdown、Word）
- [ ] 文件上傳後自動向量化
- [ ] 文件列表顯示向量化狀態
- [ ] 測試檢索功能正常

---

### FR-6：可觀察性

#### FR-6.1：日誌記錄

**需求描述：**  
記錄所有重要操作和錯誤，方便除錯。

**日誌類型：**

**1. 應用日誌**
- **INFO**：正常操作（Agent 創建、啟動、停用）
- **WARN**：警告（配置可能有問題、檢索結果較少）
- **ERROR**：錯誤（Agent 執行失敗、API 呼叫失敗）
- **DEBUG**：除錯資訊（詳細執行流程、變數值）

**2. Agent 執行日誌**
- 每次 Agent 執行記錄：
  - `execution_id`（唯一識別符）
  - `agent_name`
  - `user_input`（使用者輸入）
  - `agent_response`（Agent 回應）
  - `status`（success / error）
  - `execution_time_ms`（執行時間）
  - `tokens_used`（Token 使用量）
  - `functions_called`（呼叫的 Functions，JSON）
  - `rag_results`（RAG 檢索結果，若適用）
  - `error_message`（若失敗）
  - `created_at`（時間戳記）

**日誌輸出：**
- **Console**：開發環境
- **File**：生產環境（logs/app.log，每日輪替）
- **資料庫**：Agent 執行日誌存入 PostgreSQL

**驗收標準：**
- [ ] 應用日誌正確記錄所有操作
- [ ] Agent 執行日誌存入資料庫
- [ ] 日誌包含必要資訊（execution_id、時間、狀態）
- [ ] 錯誤日誌包含完整堆疊追蹤

---

#### FR-6.2：執行歷史查詢

**需求描述：**  
使用者可以查詢 Agent 的執行歷史。

**資料保留策略：**
- 執行記錄預設保留 **30 天**
- 超過 30 天的記錄自動歸檔或刪除
- 可在系統設定中調整保留天數
- 未來 Phase 2 可實作「標記保留」功能

**功能細節：**

**1. 執行歷史列表頁面（`/agents/{name}/executions`）**
- **表格欄位**：
  - Execution ID（可點擊查看詳情）
  - 使用者輸入（截取前 100 字元）
  - 狀態（成功 / 失敗）
  - 執行時間
  - Token 使用量
  - 時間戳記
- **篩選器**：
  - 狀態（成功 / 失敗）
  - 時間範圍（今天 / 本週 / 本月 / 自訂）
- **分頁**：每頁 50 筆

**2. 執行詳情頁面（`/executions/{execution_id}`）**
- **執行資訊**：
  - Execution ID、Agent 名稱、時間戳記
  - 狀態、執行時間、Token 使用量
- **對話內容**：
  - 使用者輸入（完整）
  - Agent 回應（完整）
- **執行詳情**：
  - Function Calling 記錄（JSON 格式化顯示）
  - RAG 檢索結果（若適用）
  - 錯誤訊息（若失敗）

**驗收標準：**
- [ ] 執行歷史列表正確顯示
- [ ] 篩選和分頁功能正常
- [ ] 執行詳情頁面顯示完整資訊
- [ ] 可查詢特定時間範圍的執行記錄

---

## 非功能需求

### NFR-1：效能

**需求：**
- **API 回應時間**：< 500ms（p95）
- **Agent 執行時間**：< 3 秒（p95，不含 LLM 呼叫時間）
- **RAG 檢索時間**：< 300ms（p95）
- **並發請求**：支援 10+ 並發 Agent 執行

**測試方法：**
- 使用負載測試工具（k6、Locust）
- 模擬 10 並發使用者
- 測試 100 次 Agent 呼叫

**驗收標準：**
- [ ] API 回應時間符合要求
- [ ] 10 並發下系統穩定運行
- [ ] 無記憶體洩漏或資源耗盡

---

### NFR-2：可靠性

**需求：**
- **正常運行時間**：> 95%（MVP 目標）
- **錯誤處理**：所有錯誤有清晰的錯誤訊息
- **自動重試**：LLM API 呼叫失敗時自動重試（最多 3 次，指數退避）
- **資料一致性**：資料庫操作使用交易（Transaction）

**驗收標準：**
- [ ] 系統連續運行 7 天無崩潰
- [ ] LLM API 暫時失敗時自動重試
- [ ] 資料庫操作失敗時正確回滾
- [ ] 所有錯誤返回有意義的錯誤訊息

---

### NFR-3：安全性

**需求：**
- **API 身份驗證**：JWT Token 驗證
- **API Key 加密**：LLM API Key 加密存儲（AES-256）
- **輸入驗證**：所有 API 輸入驗證和清理
- **SQL 注入防護**：使用參數化查詢
- **HTTPS**：生產環境強制使用 HTTPS

**驗收標準：**
- [ ] API 無 Token 時返回 401 Unauthorized
- [ ] LLM API Key 加密存儲
- [ ] 輸入驗證防止常見攻擊（XSS、SQL 注入）
- [ ] 安全掃描工具無高風險漏洞

---

### NFR-4：可維護性

**需求：**
- **程式碼覆蓋率**：> 70%（單元測試）
- **文件完整性**：
  - 部署文件（Docker Compose 使用說明）
  - 快速開始指南
  - API 參考文件（OpenAPI/Swagger）
  - 架構設計文件
- **日誌清晰度**：所有錯誤有堆疊追蹤和 Context
- **配置外部化**：所有配置通過環境變數或配置檔

**驗收標準：**
- [ ] 單元測試覆蓋率 > 70%
- [ ] 部署文件完整，新人可在 30 分鐘內部署成功
- [ ] API 文件自動生成並可測試
- [ ] 日誌可追蹤完整請求流程

---

### NFR-5：可擴展性

**需求：**
- **水平擴展**：API 服務無狀態，可部署多實例
- **資料庫連線池**：連線池管理，避免連線耗盡
- **快取機制**：Redis 快取熱門資料（Agent 配置、Prompt 模板）
- **異步處理**：文件向量化使用背景工作處理

**驗收標準：**
- [ ] 可部署多個 API 實例負載平衡
- [ ] 資料庫連線池正常運作
- [ ] Redis 快取命中率 > 80%（Agent 配置查詢）
- [ ] 文件向量化不阻塞主執行緒

---

## 技術架構概要

### 系統架構圖

#### 開發環境架構（Docker Compose）
```
┌─────────────────────────────┐
│   Docker Compose Stack      │
│                             │
│  ┌─────────────┐            │
│  │  React UI   │ :3000      │
│  │  (Dev Server)            │
│  └──────┬──────┘            │
│         │ HTTP              │
│  ┌──────▼──────────────┐   │
│  │   ASP.NET Core API  │   │
│  │   (.NET 8)     :5000│   │
│  │  - JWT 驗證          │   │
│  │  - SK Integration   │   │
│  └──────┬──────────────┘   │
│         │                   │
│  ┌──────▼──────────────┐   │
│  │   PostgreSQL  :5432 │   │
│  │   (元資料/使用者)    │   │
│  └─────────────────────┘   │
│                             │
│  ┌─────────────────────┐   │
│  │   Qdrant      :6333 │   │
│  │   (向量資料庫)       │   │
│  └─────────────────────┘   │
│                             │
│  ┌─────────────────────┐   │
│  │   Redis       :6379 │   │
│  │   (快取)             │   │
│  └─────────────────────┘   │
└─────────────────────────────┘
         │
         ▼
┌─────────────────────────────┐
│   External Services         │
│  - Azure OpenAI (雲端)      │
└─────────────────────────────┘
```

#### 生產環境架構（Azure）
```
┌─────────────────────────────────────────────┐
│           Azure Container Apps              │
│                                             │
│  ┌─────────────────┐  ┌─────────────────┐ │
│  │ React UI        │  │ ASP.NET API     │ │
│  │ (Nginx Container)  │ (.NET 8 Container)│
│  │ Port: 80/443    │  │ Port: 5000      │ │
│  └────────┬────────┘  └────────┬────────┘ │
│           │                     │          │
│           └─────────┬───────────┘          │
└─────────────────────┼──────────────────────┘
                      │
          ┌───────────┴───────────┐
          │                       │
┌─────────▼─────────┐  ┌─────────▼─────────┐
│ Azure Database    │  │ Azure Cache       │
│ for PostgreSQL    │  │ for Redis         │
└───────────────────┘  └───────────────────┘
          │                       │
          └───────────┬───────────┘
                      │
          ┌───────────┴───────────┐
          │                       │
┌─────────▼─────────┐  ┌─────────▼─────────┐
│ Azure AI Search   │  │ Azure OpenAI      │
│ (向量資料庫)       │  │ (LLM Service)     │
└───────────────────┘  └───────────────────┘
          │
┌─────────▼─────────┐
│ Azure Key Vault   │
│ (Secrets 管理)    │
└───────────────────┘
```

### 技術棧總結

#### 核心技術
**後端：**
- .NET 8 / C#
- ASP.NET Core Web API
- Semantic Kernel SDK (最新穩定版)
**資料層：**
- PostgreSQL（元資料、使用者、執行記錄）
- **向量資料庫**：
  - **開發/測試**：Qdrant（本地 Docker）
  - **生產環境**：Azure AI Search（雲端託管、混合檢索）
- Redis（快取、Session）

**LLM 服務：**
- **主要**：Azure OpenAI
- **擴展支援**：OpenAI、Anthropic、Ollama 等（通過抽象層）
- Vite (開發伺服器)
**前端：**
- React + TypeScript ✅
- Nginx（生產環境容器化）

**基礎設施：**
- **開發環境**：Docker + Docker Compose (Docker Desktop)
- **生產環境**：Azure Container Apps
- **CI/CD**：GitHub Actions
- **開發/測試**：Qdrant (Docker)
- **生產**：Azure AI Search
- **抽象層**：`IVectorStore` 介面（支援切換）

**快取：**
- Redis 7+

**LLM 服務：**
- **主要**：Azure OpenAI (GPT-4o, GPT-4o-mini)
- **可擴展**：OpenAI、Anthropic、Ollama (通過 SK 抽象)

#### 基礎設施
**開發環境：**
- Docker Desktop
- Docker Compose
- Visual Studio 2022 / VS Code

**生產環境（Phase 2）：**
- Azure Container Apps (應用託管)
- Azure Database for PostgreSQL (資料庫)
- Azure Cache for Redis (快取)
- Azure AI Search (向量資料庫)
- Azure OpenAI (LLM)
- Azure Key Vault (Secrets)
- Azure Monitor / Application Insights (監控)

**CI/CD：**
- GitHub Actions
- Azure DevOps (可選)

**監控與日誌：**
- Serilog (結構化日誌)
- 內建執行歷史查詢
- Azure Application Insights (Phase 2)

---

## 資料模型

### 核心實體

#### 1. Agent
```csharp
public class Agent
{
    public Guid Id { get; set; }
    public string Name { get; set; } // 唯一
    public string Description { get; set; }
    public AgentType Type { get; set; } // ChatCompletion, RAG, Function
    public DefinitionMode DefinitionMode { get; set; } // Code, Config
    public AgentStatus Status { get; set; } // Draft, Active, Inactive, Error, Deleted
    public string SystemPrompt { get; set; }
    public ModelConfig ModelConfig { get; set; } // JSON
    public List<string> Skills { get; set; } // JSON array
    public List<Guid> KnowledgeBaseIds { get; set; } // JSON array
    public List<string> Tags { get; set; } // JSON array
    public int CurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

#### 2. AgentVersion
```csharp
public class AgentVersion
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public int VersionNumber { get; set; }
    public string ConfigSnapshot { get; set; } // JSON
    public string ChangeSummary { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } // MVP: "admin"
}
```

#### 3. KnowledgeBase
```csharp
public class KnowledgeBase
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string EmbeddingModel { get; set; } // text-embedding-ada-002
    public int ChunkSize { get; set; } // 1000
    public int ChunkOverlap { get; set; } // 200
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### 4. Document
```csharp
public class Document
{
    public Guid Id { get; set; }
    public Guid KnowledgeBaseId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; } // pdf, txt, md, docx
    public long FileSizeBytes { get; set; }
    public DocumentStatus Status { get; set; } // Uploading, Processing, Completed, Failed
    public string ErrorMessage { get; set; }
    public int ChunkCount { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
```

#### 5. Execution
```csharp
public class Execution
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public string AgentName { get; set; }
    public string UserInput { get; set; }
    public string AgentResponse { get; set; }
    public ExecutionStatus Status { get; set; } // Success, Error
    public int ExecutionTimeMs { get; set; }
    public int TokensUsed { get; set; }
    public string FunctionsCalled { get; set; } // JSON array
    public string RagResults { get; set; } // JSON array
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## 開發計畫

### Sprint 規劃（每 Sprint 2 週）

#### Sprint 1-2（Week 1-4）：基礎設施與 Agent 管理
**後端：**
- [ ] 專案架構設定（.NET 8 Solution、專案結構）
- [ ] Docker Compose 配置（PostgreSQL、Redis、Qdrant）
- [ ] Entity Framework Core 設定與 Migrations
- [ ] Agent CRUD API 實作
- [ ] SK Kernel 初始化與配置
- [ ] Agent 狀態管理
- [ ] JWT 身份驗證

**前端：**
- [ ] React 專案建立（Vite + TypeScript）
- [ ] 專案結構與路由設定
- [ ] UI 組件庫整合（Ant Design / Material-UI）
- [ ] API Client 封裝（Axios）
- [ ] 登入頁面

**基礎設施：**
- [ ] GitHub Repository 設定
- [ ] Docker Compose 檔案完成
- [ ] 開發環境啟動腳本

#### Sprint 3-4（Week 5-8）：SK 功能包裝與 RAG
**後端：**
- [ ] ChatCompletionAgent 支援
- [ ] Function Calling 支援
- [ ] Prompt Template 管理
- [ ] 向量資料庫抽象層（IVectorStore）
- [ ] Qdrant 整合實作
- [ ] 知識庫 CRUD API
- [ ] 文件上傳與處理（PDF、TXT、Markdown、Word）
- [ ] 文件切塊（Chunking）邏輯
- [ ] Embedding 生成與向量化
- [ ] 基礎向量檢索
- [ ] RAG Agent 整合

**前端：**
- [ ] Agent 列表頁面
- [ ] Agent 創建頁面（表單驗證）
- [ ] Agent 詳情頁面
- [ ] 知識庫管理頁面
- [ ] 文件上傳組件（拖放、進度條）

#### Sprint 5-6（Week 9-12）：UI 完善與測試
**後端：**
- [ ] 執行歷史記錄完善
- [ ] 錯誤處理和日誌優化
- [ ] API 文件生成（Swagger/OpenAPI）
- [ ] 單元測試（核心邏輯）
- [ ] 整合測試（API 端到端）

**前端：**
- [ ] Agent 測試對話介面
- [ ] 執行歷史查詢頁面（含日期篩選器）
- [ ] Agent 編輯頁面
- [ ] 配置快速調整功能
- [ ] 執行詳情展示（Function Calling、RAG 結果）
- [ ] Material-UI 主題客製化
- [ ] 響應式設計優化（手機/平板/桌面）
- [ ] 錯誤處理和用戶反饋（Snackbar、Alert）

**文件與部署：**
- [ ] README.md（快速開始指南）
- [ ] 部署文件（Docker Compose 使用說明）
- [ ] API 使用文件
- [ ] 架構文件
- [ ] Nginx 配置（生產環境準備）
- [ ] 環境變數管理指南

**測試：**
- [ ] 端到端測試場景
- [ ] 效能測試（負載測試）
- [ ] 安全性檢查
- [ ] MVP 驗收測試

---

## 驗收測試計畫

### 測試場景

#### 場景 1：創建並測試聊天 Agent
1. 使用者登入平台
2. 創建 Chat Completion Agent（Config 模式）
3. 設定 System Prompt："You are a helpful AI assistant"
4. 啟動 Agent
5. 進入測試頁面，發送訊息："Hello, who are you?"
6. Agent 正確回應
7. 查看執行歷史，記錄正確顯示

**預期結果：** Agent 創建成功、測試對話正常、執行記錄完整

---

#### 場景 2：創建 RAG Agent 並查詢知識庫
1. 創建知識庫 "SK Documentation"
2. 上傳 3 個 Markdown 文件（Semantic Kernel 文件）
3. 等待向量化完成
4. 創建 RAG Agent，關聯該知識庫
5. 啟動 Agent
6. 測試提問："What is Semantic Kernel?"
7. Agent 根據知識庫回答
8. 執行詳情顯示檢索到的文件片段

**預期結果：** 知識庫創建成功、文件向量化完成、RAG Agent 正確檢索並回答

---

#### 場景 3：通過 API 呼叫 Agent
1. 創建並啟動 Agent
2. 使用 Postman/curl 呼叫 API：
   ```bash
   curl -X POST http://localhost:5000/api/v1/agents/my-agent/invoke \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json" \
     -d '{"message": "Hello"}'
   ```
3. API 返回正確回應
4. 執行記錄在資料庫中

**預期結果：** API 呼叫成功、回應格式正確、執行記錄完整

---

## 附錄

### 技術決策記錄

**已確認的技術決策：**
- ✅ **前端框架**：React + TypeScript
- ✅ **後端框架**：.NET 8 / C#
- ✅ **關聯式資料庫**：PostgreSQL
- ✅ **快取**：Redis
- ✅ **向量資料庫**：
  - 開發/測試：Qdrant (Docker)
  - 生產：Azure AI Search
- ✅ **LLM 供應商**：
  - 主要：Azure OpenAI
  - 擴展：支援 OpenAI、Anthropic、Ollama（通過抽象層）
- ✅ **部署策略**：
  - MVP：Docker Compose (Docker Desktop)
  - Phase 2：Azure Container Apps
- ✅ **前端部署**：打包進 Nginx 容器（一體化）
- ✅ **環境策略**：本地開發 → Azure 測試 → Azure 生產

**已確認的功能細節：**
- ✅ **Agent 名稱是否可修改？**
  - **決策**：❌ 不可修改
  - **理由**：名稱作為 API 路徑和唯一識別符，修改會中斷現有 API 呼叫
  - **實作**：`name` 欄位在資料庫層面設為不可更新

- ✅ **Agent 匯出/匯入功能？**
  - **決策**：❌ MVP 不包含，Phase 2 實作
  - **理由**：專注核心功能，Phase 2 可提升團隊協作能力
  - **格式**：計畫支援 JSON/YAML 格式匯出入

- ✅ **執行歷史保留時間？**
  - **決策**：30 天
  - **實作**：定時任務每日清理超過 30 天的記錄
  - **擴展**：系統設定可調整保留天數

- ✅ **UI 組件庫選擇？**
  - **決策**：Material-UI (MUI) v5+
  - **理由**：社群最大、生態豐富、組件完整、文件優秀
  - **主題**：客製化 Material Design 主題

**Phase 2 考慮的功能：**
- ⏸️ 本地 LLM 支援（Ollama）
- ⏸️ Kubernetes 部署選項
- ⏸️ Agent 匯出/匯入
- ⏸️ Azure AI Search 切換實作

---

### 參考資源

**內部文件：**
- [Product Brief v2.0](../../00-discovery/product-brief/product-brief.md)
- [腦力激盪結果](../../00-discovery/brainstorming/)

**外部資源：**
- [Semantic Kernel 官方文件](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Semantic Kernel GitHub](https://github.com/microsoft/semantic-kernel)
- [Qdrant 文件](https://qdrant.tech/documentation/)
- [Azure OpenAI 文件](https://learn.microsoft.com/en-us/azure/ai-services/openai/)

---

## 附錄 B：前端技術細節

### Material-UI 套件清單
```json
{
  "dependencies": {
    "@mui/material": "^5.15.0",
    "@mui/icons-material": "^5.15.0",
    "@emotion/react": "^11.11.0",
    "@emotion/styled": "^11.11.0",
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.20.0",
    "axios": "^1.6.0",
    "@tanstack/react-query": "^5.0.0"
  }
}
```

### 推薦的 MUI 組件使用
- **表單**：TextField, Select, Autocomplete, Switch, Checkbox
- **數據展示**：DataGrid, Table, Card, Chip
- **導航**：AppBar, Drawer, Tabs, Breadcrumbs
- **反饋**：Dialog, Snackbar, Alert, CircularProgress
- **佈局**：Container, Grid, Stack, Box

---

## 附錄 C：部署指南

### 本地開發環境部署（MVP）

#### 前置需求
- Docker Desktop 安裝
- .NET 8 SDK
- Node.js 18+ (for React)
- Visual Studio 2022 或 VS Code

#### 快速啟動
```bash
# 1. Clone Repository
git clone <repository-url>
cd ai-semantic-kernel-framework-project

# 2. 啟動基礎設施（PostgreSQL, Redis, Qdrant）
docker-compose up -d

# 3. 設定環境變數
cp .env.example .env
# 編輯 .env 填入 Azure OpenAI API Key

# 4. 啟動後端 API
cd src/Backend
dotnet restore
dotnet ef database update
dotnet run

# 5. 啟動前端 (另一個終端)
cd src/Frontend
npm install
npm run dev

# 6. 訪問應用
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Azure 生產環境部署（Phase 2）

#### 前置準備
```bash
# 安裝 Azure CLI
az login
az account set --subscription <subscription-id>

# 建立資源群組
az group create --name rg-sk-agent-platform --location eastus
```

#### 部署步驟
```bash
# 1. 建立 Azure Container Apps Environment
az containerapp env create \
  --name sk-agent-env \
  --resource-group rg-sk-agent-platform \
  --location eastus

# 2. 建立 PostgreSQL
az postgres flexible-server create \
  --name sk-agent-db \
  --resource-group rg-sk-agent-platform \
  --location eastus \
  --admin-user dbadmin \
  --admin-password <secure-password> \
  --sku-name Standard_B2s

# 3. 建立 Redis Cache
az redis create \
  --name sk-agent-cache \
  --resource-group rg-sk-agent-platform \
  --location eastus \
  --sku Basic \
  --vm-size c0

# 4. 建立 Azure AI Search
az search service create \
  --name sk-agent-search \
  --resource-group rg-sk-agent-platform \
  --location eastus \
  --sku basic

# 5. 部署 Container Apps (透過 GitHub Actions 或 Azure CLI)
# 詳細步驟見 Phase 2 部署文件
```

---

**文件狀態：** ✅ v1.0 最終版 - 所有決策已確認  
**下一步驟：** 
1. ✅ 技術決策確認（已完成）
2. ✅ 功能細節確認（已完成）
3. ⏭️ **進入架構設計階段**（Phase 2: Solutioning）
4. ⏭️ 建立 GitHub Repository 和專案結構
5. ⏭️ Sprint Planning - 分解 User Stories
6. ⏭️ 開始 Sprint 1 開發

**最後更新：** 2025-11-16  
**版本：** 1.0 (Final)  
**負責人：** 產品團隊  
**完成日期：** 2025-11-16

---

## ✅ PRD 決策總結

### 核心技術棧
- **後端**：.NET 8 / C# + ASP.NET Core
- **前端**：React 18+ + TypeScript + Vite + Material-UI v5+
- **資料庫**：PostgreSQL 16+ (關聯式) + Redis (快取)
- **向量資料庫**：Qdrant (開發) → Azure AI Search (生產)
- **LLM**：Azure OpenAI (主) + 可擴展支援
- **部署**：Docker Compose (開發) → Azure Container Apps (生產)

### 關鍵設計決策
- ✅ Agent 名稱不可修改（API 穩定性）
- ✅ 執行歷史保留 30 天
- ✅ Agent 匯出/匯入功能延後至 Phase 2
- ✅ UI 組件庫使用 Material-UI
- ✅ 向量資料庫抽象層設計（支援切換）
- ✅ 三階段環境策略（本地開發 → Azure 測試 → Azure 生產）

### MVP 交付範圍
- ✅ Agent CRUD + 狀態管理 + 版本控制
- ✅ SK 完整功能包裝（ChatCompletion, Functions, Prompts）
- ✅ 基礎 RAG（文件上傳、向量化、檢索）
- ✅ REST API + OpenAPI 文件
- ✅ Web UI（React + Material-UI）
- ✅ 基礎可觀察性（日誌、執行歷史）

### 時程規劃
- **總時長**：12 週（3 個月）
- **Sprint 1-2** (Week 1-4)：基礎設施 + Agent 管理
- **Sprint 3-4** (Week 5-8)：SK 功能 + RAG
- **Sprint 5-6** (Week 9-12)：UI 完善 + 測試 + 文件
