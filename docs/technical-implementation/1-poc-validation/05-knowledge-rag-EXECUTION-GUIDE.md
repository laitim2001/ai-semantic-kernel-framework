# PoC 5: Knowledge RAG 90%+ Accuracy - 實際執行指導

**目的**: 驗證 Knowledge RAG 系統達到 90%+ 準確率目標

**預計時間**: 2 days (Day 11-12)
**執行人**: AI/ML Tech Lead
**執行日期**: _____________
**優先級**: 🟡 P1 (高優先級，但非阻擋性)

**前置條件**: PoC 1-4 (P0) 已通過 ✅

---

## 📋 執行前檢查清單

### 必備條件

- [ ] **P0 PoC 完成**: PoC 1-4 全部 PASSED
- [ ] **Azure AI Search 資源已創建** (或使用本地 Elasticsearch)
- [ ] **測試數據集準備**: 100 個文檔, 50 個查詢
- [ ] **.NET 8.0 + Azure SDK** 已安裝
- [ ] **時間安排**: Day 11 (4-6h), Day 12 (4-6h)

### 核心目標

1. ✅ **Hybrid Search**: Vector + BM25 融合 (Precision@5 >90%)
2. ✅ **Cross-Encoder Re-ranking**: 準確率提升 >10%
3. ✅ **響應時間 <2 秒**: 包含檢索和重排
4. ✅ **可擴展性**: 支持 100,000+ 文檔

---

## 🚀 Day 11: Hybrid Search 實現 (4-6 小時)

### Phase 11.1: Azure AI Search 設置 (1 小時)

#### 步驟 1: 創建 Search Index

```powershell
# 創建專案
mkdir C:\poc-knowledge-rag
cd C:\poc-knowledge-rag
dotnet new console -n KnowledgeRAGPoC

# 安裝套件
dotnet add package Azure.Search.Documents --version 11.5.1
dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.12
```

**創建 Index Schema** (`create-index.json`):

```json
{
  "name": "knowledge-base",
  "fields": [
    {"name": "id", "type": "Edm.String", "key": true},
    {"name": "content", "type": "Edm.String", "searchable": true},
    {"name": "content_vector", "type": "Collection(Edm.Single)",
     "searchable": true, "dimensions": 1536,
     "vectorSearchProfile": "vector-profile"},
    {"name": "title", "type": "Edm.String", "searchable": true},
    {"name": "source", "type": "Edm.String", "filterable": true}
  ],
  "vectorSearch": {
    "algorithms": [{"name": "hnsw-algo", "kind": "hnsw"}],
    "profiles": [{"name": "vector-profile", "algorithm": "hnsw-algo"}]
  }
}
```

**創建 HybridSearchService.cs**:

```csharp
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.AI.OpenAI;

public class HybridSearchService
{
    private readonly SearchClient _searchClient;
    private readonly OpenAIClient _openAIClient;

    public async Task<List<SearchResult>> HybridSearchAsync(
        string query, int topK = 10)
    {
        // 1. 生成查詢向量
        var embedding = await GenerateEmbeddingAsync(query);

        // 2. Hybrid Search (Vector + BM25)
        var options = new SearchOptions
        {
            VectorSearch = new()
            {
                Queries = { new VectorizedQuery(embedding)
                {
                    KNearestNeighborsCount = topK,
                    Fields = { "content_vector" }
                }}
            },
            Size = topK
        };

        var response = await _searchClient.SearchAsync<Document>(query, options);
        var results = new List<SearchResult>();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            results.Add(new SearchResult
            {
                Id = result.Document["id"].ToString(),
                Content = result.Document["content"].ToString(),
                Score = result.Score ?? 0
            });
        }

        return results;
    }

    private async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var response = await _openAIClient.GetEmbeddingsAsync(
            "text-embedding-ada-002", new EmbeddingsOptions(text));
        return response.Value.Data[0].Embedding.ToArray();
    }
}

public class SearchResult
{
    public string Id { get; set; }
    public string Content { get; set; }
    public double Score { get; set; }
}
```

**測試基礎檢索**:

```csharp
// Program.cs
var searchService = new HybridSearchService();
var results = await searchService.HybridSearchAsync("What is Semantic Kernel?", topK: 5);

Console.WriteLine($"Found {results.Count} results");
foreach (var result in results)
{
    Console.WriteLine($"[{result.Score:F3}] {result.Content.Substring(0, 100)}...");
}
```

**✅ 通過標準**: 檢索返回相關結果，Score 排序正確

---

### Phase 11.2: Cross-Encoder Re-ranking (2 小時)

#### 步驟 2: 實現 Re-ranking

**創建 CrossEncoderReranker.cs**:

```csharp
using System.Net.Http.Json;

public class CrossEncoderReranker
{
    private readonly HttpClient _httpClient;
    private const string HF_API = "https://api-inference.huggingface.co/models/cross-encoder/ms-marco-MiniLM-L-6-v2";

    public async Task<List<SearchResult>> RerankAsync(
        string query, List<SearchResult> results)
    {
        var pairs = results.Select(r => new { query, document = r.Content }).ToList();

        var response = await _httpClient.PostAsJsonAsync(HF_API, new { inputs = pairs });
        var scores = await response.Content.ReadFromJsonAsync<List<double>>();

        // 重新排序
        return results
            .Zip(scores, (r, s) => new { Result = r, Score = s })
            .OrderByDescending(x => x.Score)
            .Select(x => new SearchResult
            {
                Id = x.Result.Id,
                Content = x.Result.Content,
                Score = x.Score
            })
            .ToList();
    }
}
```

**測試 Re-ranking 效果**:

```csharp
var reranker = new CrossEncoderReranker();

// Before re-ranking
var hybridResults = await searchService.HybridSearchAsync("Semantic Kernel agents", 20);
Console.WriteLine("=== Before Re-ranking ===");
hybridResults.Take(5).ToList().ForEach(r =>
    Console.WriteLine($"[{r.Score:F3}] {r.Content.Substring(0, 80)}"));

// After re-ranking
var rerankedResults = await reranker.RerankAsync("Semantic Kernel agents", hybridResults);
Console.WriteLine("\n=== After Re-ranking ===");
rerankedResults.Take(5).ToList().ForEach(r =>
    Console.WriteLine($"[{r.Score:F3}] {r.Content.Substring(0, 80)}"));
```

**記錄點**:
```
[Day 11 - 14:00] Re-ranking 實現完成
- Re-ranking 前 Precision@5: _____%
- Re-ranking 後 Precision@5: _____%
- 提升: _____%
```

---

### Phase 11.3: 評估指標 (2 小時)

#### 步驟 3: 準確率評估

**創建測試數據集** (`test-queries.json`):

```json
{
  "queries": [
    {
      "query": "What is Semantic Kernel?",
      "relevant_docs": ["doc-1", "doc-5", "doc-12"]
    },
    {
      "query": "How to create an agent?",
      "relevant_docs": ["doc-3", "doc-7"]
    }
  ]
}
```

**計算評估指標**:

```csharp
public class RAGEvaluator
{
    public double CalculatePrecisionAtK(
        List<string> retrievedDocs,
        List<string> relevantDocs,
        int k)
    {
        var topK = retrievedDocs.Take(k).ToList();
        var relevant = topK.Intersect(relevantDocs).Count();
        return (double)relevant / k;
    }

    public double CalculateRecallAtK(
        List<string> retrievedDocs,
        List<string> relevantDocs,
        int k)
    {
        var topK = retrievedDocs.Take(k).ToList();
        var relevant = topK.Intersect(relevantDocs).Count();
        return (double)relevant / relevantDocs.Count;
    }

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
}
```

**執行評估**:

```csharp
var evaluator = new RAGEvaluator();
var testData = LoadTestQueries("test-queries.json");

foreach (var test in testData)
{
    var results = await searchService.HybridSearchAsync(test.Query);
    var reranked = await reranker.RerankAsync(test.Query, results);

    var precision = evaluator.CalculatePrecisionAtK(
        reranked.Select(r => r.Id).ToList(),
        test.RelevantDocs,
        k: 5);

    Console.WriteLine($"Query: {test.Query}");
    Console.WriteLine($"Precision@5: {precision:P}");
}
```

**✅ 通過標準**: Precision@5 > 90%, Re-ranking 提升 > 10%

---

## 🚀 Day 12: 性能優化與報告 (4-6 小時)

### Phase 12.1: 性能測試 (2 小時)

#### 步驟 4: 響應時間與擴展性

**性能測試**:

```csharp
public class PerformanceTests
{
    public static async Task MeasureLatency(HybridSearchService service)
    {
        var queries = new[] {
            "Semantic Kernel overview",
            "Agent creation tutorial",
            "Plugin system guide"
        };

        var times = new List<double>();

        foreach (var query in queries)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var results = await service.HybridSearchAsync(query, 10);
            sw.Stop();

            times.Add(sw.Elapsed.TotalMilliseconds);
            Console.WriteLine($"Query: {query} - {sw.ElapsedMilliseconds}ms");
        }

        var avgTime = times.Average();
        Console.WriteLine($"\nAverage Response Time: {avgTime:F0}ms");
        Console.WriteLine($"Result: {(avgTime < 2000 ? "✅ PASSED (<2s)" : "❌ FAILED (>2s)")}");
    }
}
```

**記錄點**:
```
[Day 12 - 12:00] 性能測試完成
- 平均響應時間: _____ ms (目標 <2000ms)
- 最快查詢: _____ ms
- 最慢查詢: _____ ms
```

---

### Phase 12.2: 填寫驗證報告 (2 小時)

#### 步驟 5: 整理結果

**填寫報告** (`poc-validation-report.md`):

```markdown
### PoC 5: Knowledge RAG 90%+ Accuracy (P1) 🟡

**狀態**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**成功標準達成情況**:
- [✅] Hybrid Search 準確率 >90% (Precision@5: 92%)
- [✅] Re-ranking 提升效果 >10% (提升 15%)
- [✅] 響應時間 <2 秒 (平均 1.2s)
- [✅] 可擴展至 100,000+ 文檔

**關鍵發現**:
- Precision@5: 92%
- Recall@10: 88%
- MRR: 0.85
- Re-ranking 提升: 15%

**風險與緩解**:
| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| Cross-Encoder 成本高 | 中 | Re-rank Top-20 only |
| 長文檔處理 | 中 | 優化 Chunking 策略 |

**建議**:
- Hybrid Search 效果優異，推薦採用
- 建立持續評估 pipeline
- 實現查詢緩存機制
```

---

## 📊 最終檢查清單

- [ ] **Hybrid Search**: Precision@5 >90%
- [ ] **Re-ranking**: 提升 >10%
- [ ] **響應時間**: <2 秒
- [ ] **報告填寫**: 完成

---

## 🎯 Go/No-Go 決策

### ✅ GO
- [x] Precision@5 >90%
- [x] Re-ranking 提升 >10%
- [x] 響應時間 <2s

### ⚠️ 有條件 GO
- [ ] Precision@5: 85-90% (可優化 Chunking)

### ❌ NO-GO
- [ ] Precision@5 <85%

---

**執行記錄**:

```
執行人: _________________
執行日期: _______________

Day 11 完成: _____________
Day 12 完成: _____________

最終狀態: ✅ PASSED / ❌ FAILED / ⚠️ CONDITIONAL

關鍵指標:
- Precision@5: _____% (目標 >90%)
- Re-ranking 提升: _____% (目標 >10%)
- 平均響應時間: _____ ms (目標 <2000ms)

Go/No-Go: _______________ (決策人: _______________)
下一步: 繼續 PoC 6 / 優化 / 重新評估
```

---

**最後更新**: 2025-10-30
**執行指導版本**: 1.0.0 (精簡版 - P1 PoC)
