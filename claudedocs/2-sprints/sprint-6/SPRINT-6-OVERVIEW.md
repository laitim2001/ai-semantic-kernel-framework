# Sprint 6 概覽 - 精確檢索策略 (核心差異化功能)

**Sprint 編號**: Sprint 6
**週次**: Week 16-18
**計劃日期**: 2026-02-23 ~ 2026-03-15 (21 days)
**實際日期**: TBD
**狀態**: ⏳ **未開始**

---

## 🎯 Sprint 目標

實現**核心差異化能力 #2 - 精確檢索策略 (90%+ 準確率)**,通過 Hybrid Search (Vector + Keyword) + Reranking 機制達到業界領先的檢索準確度。

**關鍵交付物**:
1. ⏳ Azure AI Search 集成
2. ⏳ Vector Embedding (OpenAI Embeddings API)
3. ⏳ Hybrid Search (Semantic + Keyword)
4. ⏳ Reranking 機制 (Cross-encoder)
5. ⏳ Query 重寫與優化策略
6. ⏳ 檢索準確率驗證 (目標 ≥ 90%)

---

## 📊 User Stories

### 計劃 User Stories

| User Story | Story Points | 計劃天數 | 模組 | 狀態 | 優先級 |
|-----------|-------------|---------|------|------|-------|
| **US 5.2** - 精確檢索策略配置 (90%+ 準確率) ⭐ | 8 SP | 14 days | Module 5 | ⏳ 未開始 | P0 - 核心差異化 |

**總計**: **8 SP** (14 days 開發 + 7 days 測試/驗證)

---

## 🎯 核心差異化能力說明

### 為什麼精確檢索是差異化能力?

**競品對比**:
- **傳統 RAG**: 僅使用 Vector Search,準確率 60-70%
- **Keyword Search**: 僅基於關鍵字,缺乏語義理解
- **我們的 Hybrid Search**: Vector + Keyword + Reranking,目標 **90%+ 準確率**

**差異化優勢**:
1. **Hybrid Search**: 結合語義 (Vector) 和精確匹配 (Keyword)
2. **Reranking**: Cross-encoder 重新排序,提升 Top-K 準確率
3. **Query 優化**: 自動查詢重寫和擴展
4. **Multi-query 策略**: 生成多個查詢角度,提升召回率
5. **動態 Top-K**: 根據查詢類型和置信度動態調整檢索數量

---

## 📋 詳細技術任務

### 1. Azure AI Search 集成

#### Azure 資源配置
```yaml
Azure AI Search Service:
  - SKU: Basic (開發) / Standard S1 (生產)
  - Region: East Asia (就近部署)
  - Index Schema:
      id: string (key)
      content: string (searchable)
      content_vector: Collection(Edm.Single) - 1536 維向量
      metadata: Complex (file_id, page, chunk_index)
      created_at: DateTimeOffset

NuGet 套件:
  - Azure.Search.Documents (v11.5+)
  - Azure.AI.OpenAI (v1.0+) - Embeddings
```

#### Index 創建
```yaml
POST /api/v1/knowledge-base/indexes:
  - 創建 AI Search Index
  - 配置 Vector Search Profile
  - 配置 Semantic Configuration

Vector Search Configuration:
  - Algorithm: HNSW (Hierarchical Navigable Small World)
  - Parameters:
      m: 4 (connections per node)
      efConstruction: 400
      efSearch: 500

Semantic Configuration:
  - Title Field: N/A
  - Content Fields: content
  - Keyword Fields: metadata.file_id
```

---

### 2. Vector Embedding

#### OpenAI Embeddings API 集成
```yaml
Model: text-embedding-3-small
  - Dimensions: 1536
  - Cost: $0.02 / 1M tokens
  - Speed: ~1000 embeddings/minute

批量 Embedding:
  - Batch Size: 100 chunks/request
  - 異步處理
  - 錯誤重試機制

POST /api/v1/embeddings/generate:
  - Request: { texts: string[] }
  - Response: { vectors: number[][] }
```

#### Embedding 存儲
```yaml
更新 KnowledgeBaseChunk:
  - Embedding (float[]) - 1536 維向量
  - EmbeddedAt (DateTime)

批量更新邏輯:
  - 文件上傳後,異步生成 Embedding
  - 批量寫入 Azure AI Search Index
```

---

### 3. Hybrid Search 實現

#### Search Query API
```yaml
POST /api/v1/knowledge-base/search:
  - Request:
      query: string
      agentId: Guid
      searchMode: 'vector' | 'keyword' | 'hybrid'
      topK: number (default: 5)
      filters:
        fileIds: Guid[]
        dateRange: { from, to }

  - Response:
      results:
        - chunk_id: Guid
          content: string
          score: number (0-1)
          metadata: object
          highlights: string[]

      metadata:
        total_results: number
        search_mode: string
        reranked: boolean
```

#### Hybrid Search 實現邏輯
```yaml
Step 1 - Vector Search:
  - 查詢 Embedding 生成
  - Vector 相似度搜索 (Cosine Similarity)
  - Top-K: 20 (初步召回)

Step 2 - Keyword Search:
  - 全文搜索 (Azure AI Search)
  - BM25 算法
  - Top-K: 20

Step 3 - 結果合併:
  - Reciprocal Rank Fusion (RRF)
  - 公式: score = Σ(1 / (k + rank_i))
  - k = 60 (常數)

Step 4 - Reranking (可選):
  - Cross-encoder 模型
  - 重新計算 Query-Chunk 相似度
  - 最終 Top-K: 5
```

---

### 4. Reranking 機制

#### Cross-encoder Model
```yaml
Model 選擇:
  - Option 1: ms-marco-MiniLM-L-6-v2 (輕量, 快速)
  - Option 2: ms-marco-electra-base (更準確)

Hugging Face Integration:
  - Python Service (FastAPI)
  - 部署為 Azure Container Instance
  - API Endpoint: POST /rerank

Request/Response:
  Request:
    query: string
    passages: string[]

  Response:
    scores: number[]
    ranked_indices: number[]
```

#### Reranking 觸發條件
```yaml
觸發條件:
  - topK > 5 (需要精細排序)
  - 初步分數差異小 (top score - 5th score < 0.1)
  - 用戶明確要求高準確率

性能考量:
  - Reranking 時間: ~200ms (5 passages)
  - 僅對 Top-20 進行 Reranking
  - 緩存熱門查詢結果
```

---

### 5. Query 優化策略

#### Query 重寫
```yaml
Rewrite Strategies:
  - Spelling Correction (拼寫修正)
  - Synonym Expansion (同義詞擴展)
  - Acronym Expansion (縮寫展開)

實現:
  - LLM-based Rewrite (GPT-4)
  - Prompt: "Rewrite query for better search: {query}"
  - 緩存常見查詢重寫

POST /api/v1/query/rewrite:
  Request: { query: string }
  Response: { rewritten_query: string }
```

#### Multi-query 策略
```yaml
生成多角度查詢:
  - Original Query
  - Paraphrased Query (改寫)
  - Expanded Query (擴展)

示例:
  Original: "如何使用 Agent?"

  Paraphrased: "Agent 的使用方法是什麼?"

  Expanded: "如何創建和配置 AI Agent 進行任務自動化?"

合併策略:
  - 對 3 個查詢分別檢索
  - Reciprocal Rank Fusion 合併
```

#### 動態 Top-K 調整
```yaml
調整邏輯:
  - 高置信度查詢 (top score > 0.8): topK = 3
  - 中等置信度 (0.5 - 0.8): topK = 5
  - 低置信度 (< 0.5): topK = 10 + Reranking

查詢類型識別:
  - Factual (事實類): topK = 3
  - Exploratory (探索類): topK = 10
  - Procedural (步驟類): topK = 5
```

---

## 🧪 測試策略

### 檢索準確率評估框架

#### Golden Dataset 準備
```yaml
Dataset 結構:
  - 50 個標準查詢
  - 每個查詢標註 3-5 個相關 Chunks (Ground Truth)
  - 查詢類型覆蓋: Factual, Exploratory, Procedural

示例:
  Query: "如何創建 Agent?"
  Ground Truth Chunk IDs: [123, 456, 789]
  Expected Top-3 Recall: 100%
```

#### 評估指標
```yaml
主要指標:
  - Precision@K: 前 K 個結果的準確率
  - Recall@K: 召回率
  - MRR (Mean Reciprocal Rank): 平均倒數排名
  - NDCG@K (Normalized Discounted Cumulative Gain)

目標:
  - Precision@5 ≥ 90%
  - Recall@5 ≥ 85%
  - MRR ≥ 0.85
  - NDCG@5 ≥ 0.90
```

#### A/B Testing 框架
```yaml
對比策略:
  - Baseline: Vector Search Only
  - Strategy A: Hybrid Search (Vector + Keyword)
  - Strategy B: Hybrid + Reranking
  - Strategy C: Hybrid + Reranking + Query Rewrite

評估流程:
  1. 對 50 個查詢運行所有策略
  2. 計算各策略的 Precision, Recall, MRR, NDCG
  3. 統計顯著性測試
  4. 選擇最優策略
```

---

### 單元測試 (目標 15 tests)
```yaml
EmbeddingServiceTests:
  - Embedding 生成測試
  - 批量 Embedding 測試
  - 錯誤處理測試

SearchServiceTests:
  - Vector Search 測試
  - Keyword Search 測試
  - Hybrid Search 測試
  - RRF 合併邏輯測試

QueryOptimizerTests:
  - Query 重寫測試
  - Multi-query 生成測試
  - Top-K 動態調整測試
```

---

### 集成測試 (目標 10 tests)
```yaml
AzureAISearchIntegrationTests:
  - Index 創建測試
  - Document 索引測試
  - Vector Search API 測試

RerankingIntegrationTests:
  - Cross-encoder API 測試
  - Reranking 性能測試

端到端檢索測試:
  - 上傳文件 → Embedding → 檢索
  - 驗證檢索準確率
```

---

### 準確率驗證測試 (目標 50 queries)
```yaml
Golden Dataset 測試:
  - 運行 50 個標準查詢
  - 計算 Precision@5, Recall@5, MRR, NDCG
  - 驗證 ≥ 90% 準確率目標

邊界案例測試:
  - 短查詢 (2-3 字)
  - 長查詢 (>20 字)
  - 模糊查詢
  - 拼寫錯誤查詢
  - 同義詞查詢

性能測試:
  - 單次查詢延遲 < 500ms
  - 並發 100 查詢/秒
```

---

## 🎯 驗收標準 (DoD)

### 功能驗收
- ✅ Hybrid Search 功能完整
- ✅ Reranking 機制正常運作
- ✅ Query 優化策略有效
- ✅ Multi-query 策略實現

### 準確率驗收
- ✅ **Precision@5 ≥ 90%** (核心指標)
- ✅ Recall@5 ≥ 85%
- ✅ MRR ≥ 0.85
- ✅ NDCG@5 ≥ 0.90

### 性能驗收
- ✅ Vector Search 延遲 < 200ms
- ✅ Hybrid Search 延遲 < 500ms
- ✅ Reranking 延遲 < 200ms
- ✅ 端到端檢索延遲 < 800ms

### 質量驗收
- ✅ 單元測試通過率 100%
- ✅ 集成測試通過率 100%
- ✅ Golden Dataset 測試通過率 ≥ 90%
- ✅ 測試覆蓋率 ≥ 85%
- ✅ Code Review 通過
- ✅ PO 驗收通過

---

## 🎓 關鍵風險

### 風險 1: 準確率未達 90% 目標
**影響**: 關鍵 🔴
**緩解**:
- Sprint 中期 (Day 7) 初步驗證準確率
- 如未達標,調整 Reranking 模型或 Hybrid 權重
- 準備 Plan B: 手動調優或引入更強模型

### 風險 2: Reranking 延遲過高
**影響**: 高 🔴
**緩解**:
- 選用輕量級 Cross-encoder (MiniLM)
- 實施結果緩存
- 異步 Reranking (用戶可選)

### 風險 3: Azure AI Search 成本超預算
**影響**: 中 🟡
**緩解**:
- 使用 Basic SKU 開發和測試
- 監控查詢量和索引大小
- 實施查詢緩存策略

---

## 📦 技術依賴

### 新增 NuGet 套件
```yaml
Azure.Search.Documents: v11.5+
Azure.AI.OpenAI: v1.0+
```

### Azure 資源
```yaml
Azure AI Search:
  - SKU: Basic (開發) / Standard S1 (生產)
  - Pricing: ~$75/month (Basic), ~$250/month (S1)

Azure Container Instance (Reranker):
  - vCPU: 1
  - Memory: 2GB
  - Pricing: ~$30/month
```

### Python Reranker Service
```yaml
Framework: FastAPI
Libraries:
  - sentence-transformers
  - ms-marco-MiniLM-L-6-v2

部署:
  - Docker Container
  - Azure Container Instance
```

---

## 📅 里程碑

| 里程碑 | 預估日期 | 交付物 |
|-------|---------|--------|
| **M1** | 2026-02-25 | Azure AI Search Index 創建完成 |
| **M2** | 2026-03-01 | Vector Embedding 批量生成完成 |
| **M3** | 2026-03-05 | Hybrid Search 實現完成 |
| **M4** | 2026-03-08 | Reranking 機制集成完成 |
| **M5** | 2026-03-12 | 準確率驗證 ≥ 90% |
| **M6** | 2026-03-15 | PO 驗收通過 |

---

## 🔗 相關文檔

- **User Story 詳情**: [docs/user-stories/sprints/sprint-allocation.md](../../../docs/user-stories/sprints/sprint-allocation.md#sprint-6)
- **依賴 Sprint**: Sprint 5 (US 5.1, 5.3 必須完成)
- **後續 Sprint**: Sprint 9, 10 (RAG 應用和 Text-to-SQL)

---

**維護說明**: 規劃文檔,Sprint 6 開始後更新實際進度。

**狀態**: ⏳ 規劃中
