# PoC 5: Knowledge RAG 90%+ Accuracy 驗證

**優先級**: 🟡 P1 (高優先級)
**預計時間**: 2 days (Day 11-12)
**狀態**: ⏳ 待開始
**負責人**: AI/ML Tech Lead

**基於**: Azure AI Search + Cross-Encoder + BM25 (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Knowledge RAG 系統達到 **90%+ 準確率目標**：

1. ✅ **Hybrid Search**: Vector + BM25 融合
2. ✅ **Cross-Encoder Re-ranking**: 提升檢索準確度
3. ✅ **RRF (Reciprocal Rank Fusion)**: 結果融合策略
4. ✅ **準確率評估**: 達到 90%+ 目標
5. ✅ **性能基準**: 響應時間 <2 秒

---

## 📋 RAG 架構

```
┌──────────────────────────────────────────┐
│         Knowledge RAG System             │
│  ┌────────────────────────────────────┐  │
│  │  Document Processing               │  │
│  │  - Chunking (500-2000 tokens)      │  │
│  │  - Overlap (0-200 tokens)          │  │
│  │  - Metadata Extraction             │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Azure AI Search (Hybrid)          │  │
│  │  - Vector Search                   │  │
│  │  - BM25 Full-text Search           │  │
│  │  - RRF Fusion                      │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Cross-Encoder Re-ranking          │  │
│  │  - HuggingFace Model               │  │
│  │  - Query-Document Scoring          │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Context Generation                │  │
│  │  - Top-K Selection                 │  │
│  │  - LLM Answer Generation           │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
```

---

## 🔧 環境準備

### Azure AI Search 配置

```json
{
  "name": "knowledge-base-index",
  "fields": [
    {
      "name": "id",
      "type": "Edm.String",
      "key": true
    },
    {
      "name": "content",
      "type": "Edm.String",
      "searchable": true,
      "analyzer": "standard.lucene"
    },
    {
      "name": "content_vector",
      "type": "Collection(Edm.Single)",
      "searchable": true,
      "vectorSearchDimensions": 1536,
      "vectorSearchProfile": "my-vector-profile"
    },
    {
      "name": "metadata",
      "type": "Edm.ComplexType",
      "fields": [
        { "name": "source", "type": "Edm.String" },
        { "name": "page", "type": "Edm.Int32" }
      ]
    }
  ],
  "vectorSearch": {
    "algorithms": [
      {
        "name": "my-hnsw-algorithm",
        "kind": "hnsw",
        "hnswParameters": {
          "m": 4,
          "efConstruction": 400,
          "efSearch": 500
        }
      }
    ],
    "profiles": [
      {
        "name": "my-vector-profile",
        "algorithm": "my-hnsw-algorithm"
      }
    ]
  }
}
```

---

## 💻 實現步驟

### 步驟 1: Hybrid Search

```csharp
public class HybridSearchService
{
    private readonly SearchClient _searchClient;
    private readonly IEmbeddingGenerator _embeddingGenerator;

    public async Task<List<SearchResult>> HybridSearchAsync(
        string query,
        int topK = 10)
    {
        // 1. 生成查詢向量
        var queryVector = await _embeddingGenerator.GenerateAsync(query);

        // 2. Hybrid Search (Vector + BM25)
        var options = new SearchOptions
        {
            VectorSearch = new()
            {
                Queries =
                {
                    new VectorizedQuery(queryVector.ToArray())
                    {
                        KNearestNeighborsCount = topK,
                        Fields = { "content_vector" }
                    }
                }
            },
            QueryType = SearchQueryType.Semantic,
            Size = topK
        };

        var response = await _searchClient.SearchAsync<Document>(
            query,
            options
        );

        return response.Value.GetResults().ToList();
    }
}
```

### 步驟 2: Cross-Encoder Re-ranking

```csharp
public class CrossEncoderReranker
{
    private readonly HttpClient _httpClient;
    private const string HF_API_URL =
        "https://api-inference.huggingface.co/models/cross-encoder/ms-marco-MiniLM-L-6-v2";

    public async Task<List<RankedResult>> RerankAsync(
        string query,
        List<SearchResult> results)
    {
        var pairs = results.Select(r => new
        {
            query,
            document = r.Content
        }).ToList();

        // 調用 HuggingFace Cross-Encoder API
        var request = new
        {
            inputs = pairs
        };

        var response = await _httpClient.PostAsJsonAsync(HF_API_URL, request);
        var scores = await response.Content.ReadFromJsonAsync<List<double>>();

        // 根據分數重新排序
        var ranked = results
            .Zip(scores, (result, score) => new RankedResult
            {
                Result = result,
                Score = score
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        return ranked;
    }
}
```

### 步驟 3: RRF (Reciprocal Rank Fusion)

```csharp
public class RRFusion
{
    private const int K = 60; // RRF constant

    public List<SearchResult> FuseResults(
        List<SearchResult> vectorResults,
        List<SearchResult> bm25Results)
    {
        var scores = new Dictionary<string, double>();

        // RRF formula: score = 1 / (k + rank)
        for (int i = 0; i < vectorResults.Count; i++)
        {
            var docId = vectorResults[i].Id;
            scores[docId] = scores.GetValueOrDefault(docId) + 1.0 / (K + i + 1);
        }

        for (int i = 0; i < bm25Results.Count; i++)
        {
            var docId = bm25Results[i].Id;
            scores[docId] = scores.GetValueOrDefault(docId) + 1.0 / (K + i + 1);
        }

        return scores
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => vectorResults.First(r => r.Id == kvp.Key))
            .ToList();
    }
}
```

---

## 🧪 測試用例

### 測試數據集

**文檔集合**: 100 個技術文檔 (總計 ~10,000 段落)
**查詢集合**: 50 個測試查詢 (手動標註正確答案)

| 查詢 ID | 查詢 | 正確文檔 ID | 查詢類型 |
|---------|------|-------------|----------|
| Q1 | "How to create an agent?" | D12, D34 | 操作指南 |
| Q2 | "What is the maximum token limit?" | D8 | 事實查詢 |
| Q3 | "Difference between agents and plugins?" | D5, D19 | 比較查詢 |
| ... | ... | ... | ... |

### 評估指標

#### Precision@K

```csharp
public double CalculatePrecisionAtK(
    List<string> retrievedDocs,
    List<string> relevantDocs,
    int k)
{
    var topK = retrievedDocs.Take(k).ToList();
    var relevant = topK.Intersect(relevantDocs).Count();
    return (double)relevant / k;
}
```

#### Recall@K

```csharp
public double CalculateRecallAtK(
    List<string> retrievedDocs,
    List<string> relevantDocs,
    int k)
{
    var topK = retrievedDocs.Take(k).ToList();
    var relevant = topK.Intersect(relevantDocs).Count();
    return (double)relevant / relevantDocs.Count;
}
```

#### MRR (Mean Reciprocal Rank)

```csharp
public double CalculateMRR(List<QueryResult> results)
{
    double sumRR = 0;

    foreach (var result in results)
    {
        for (int i = 0; i < result.RetrievedDocs.Count; i++)
        {
            if (result.RelevantDocs.Contains(result.RetrievedDocs[i]))
            {
                sumRR += 1.0 / (i + 1);
                break;
            }
        }
    }

    return sumRR / results.Count;
}
```

---

## ✅ 成功標準驗證

### 1. Hybrid Search 準確率 >90% ✅ / ❌

**評估結果**:
- Precision@5: _____%
- Recall@10: _____%
- MRR: _____

**結果**: ✅ / ❌

### 2. Re-ranking 提升效果 >10% ✅ / ❌

**對比結果**:
- 無 Re-ranking: _____%
- 有 Re-ranking: _____%
- 提升: _____%

**結果**: ✅ / ❌

### 3. 響應時間 <2 秒 ✅ / ❌

**平均時間**: _____ 秒

**結果**: ✅ / ❌

### 4. 可擴展至 100,000+ 文檔 ✅ / ❌

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 準確率分析

| 方法 | Precision@5 | Recall@10 | MRR |
|------|-------------|-----------|-----|
| Vector Search 單獨 | ___% | ___% | ___ |
| BM25 單獨 | ___% | ___% | ___ |
| Hybrid Search (RRF) | ___% | ___% | ___ |
| + Cross-Encoder | ___% | ___% | ___ |

---

## ⚠️ 風險識別

| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| Cross-Encoder 成本高 | 中 | Re-rank Top-20 only |
| 長文檔處理 | 中 | 優化 Chunking 策略 |
| 冷啟動問題 | 低 | 預熱機制 |

---

**最後更新**: 2025-10-30
**PoC 負責人**: AI/ML Tech Lead

[← 上一個 PoC](./04-text-to-sql-engine.md) | [下一個 PoC: VueFlow CRDT →](./06-vueflow-crdt-collaboration.md)
