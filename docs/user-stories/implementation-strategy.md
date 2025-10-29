# 技術難點與實施策略

**Semantic Kernel Agentic Framework - Implementation Strategy**

**版本**: 1.0.0
**日期**: 2025-10-28
**狀態**: 策略規劃
**作者**: Product Owner + System Architect (BMad Method)

[📚 主索引](../README.md) | [📋 User Stories](./README.md) | [🎯 MVP 規劃](./mvp-planning.md) | [🏗️ 架構設計](../architecture/Architecture-Design-Document.md)

---

## 文檔目的

本文檔詳細說明 Semantic Kernel Agentic Framework 的 **4 大核心差異化能力** 的技術難點和實施策略，為開發團隊提供清晰的技術路線圖和成功標準。

**核心差異化能力**:
1. **Persona Framework** - 個性化 Agent 系統
2. **Text-to-SQL** - 安全的結構化數據查詢
3. **Code Interpreter** - 4 層安全沙箱
4. **Knowledge Management** - 90%+ 準確率檢索

---

## 1. Persona Framework

### 核心價值主張

**與競品差異**: Microsoft Copilot Studio 僅提供簡單的 System Prompt 設定，無法保證對話風格一致性和個性穩定性。我們的 Persona Framework 通過結構化配置和 Few-Shot 範例，確保 Agent 個性真實且一致。

### 技術難點

#### 難點 1: Prompt Engineering 複雜度高
**問題描述**:
- Persona 配置需要轉換為精確的 System Prompt
- 多個 Persona 維度（正式度、熱情度、專業度）需要協調
- Few-Shot 範例的選擇和注入時機影響效果

**影響範圍**: US 1.5 (Persona Builder), US 7.1 (模板配置), US 7.2 (Prompt Engineering)

#### 難點 2: 風格一致性驗證缺乏標準化指標
**問題描述**:
- 如何量化評估 Persona 是否一致？
- 多輪對話中 Persona 可能漂移（風格退化）
- 缺乏自動化的一致性檢測機制

**影響範圍**: US 7.3 (一致性驗證)

#### 難點 3: 多輪對話中 Persona 可能漂移
**問題描述**:
- LLM 在長對話中可能"忘記" Persona 設定
- 用戶引導可能改變 Agent 風格
- Context Window 限制導致 Persona 資訊被截斷

**影響範圍**: US 7.2 (Prompt Engineering), US 7.4 (Persona 演化)

---

### 實施策略

#### Phase 1 (MVP) - 基礎 Persona 配置和 Prompt 生成
**時間**: 第 7 個月 (Sprint 3)
**User Stories**: US 1.5, US 7.1, US 7.2

**實施步驟**:
1. **Persona 配置結構化**:
   ```yaml
   persona:
     identity:
       name: "張明華"
       role: "資深產品經理"
       background: "10年互聯網產品經驗..."

     personality_traits:
       formality: 7/10
       enthusiasm: 8/10
       empathy: 9/10
       decision_style: ["數據驅動", "快速決策"]

     expertise:
       primary: ["產品管理", "用戶研究"]
       secondary: ["數據分析", "UX 設計"]
       limitations: ["後端架構", "硬體開發"]

     example_conversations:
       - user: "我們要不要加這個功能？"
         assistant: "先問幾個問題：1) 解決什麼用戶痛點？..."
   ```

2. **Prompt Template Engine**:
   - 將 Persona 配置轉換為結構化 System Prompt
   - Few-Shot 範例動態注入
   - 保留 Context Window 的 20% 給 Persona 資訊

3. **Persona Builder UI**:
   - 6 步引導式介面（參考 US 1.5）
   - 即時預覽 Prompt 生成結果
   - 個性一致性評分（初步版本）

**技術選型**:
- Prompt Template: Jinja2 或 Liquid
- Few-Shot 範例管理: PostgreSQL JSON 欄位
- 一致性評分: GPT-4 as Judge（初步實現）

---

#### Phase 2 (Post-MVP) - 風格一致性驗證和告警
**時間**: Phase 2 (第 13-15 個月)
**User Stories**: US 7.3

**實施步驟**:
1. **一致性評分系統**:
   - 使用 GPT-4 評估每次對話與 Persona 的一致性
   - 計算 Persona Drift Score (0-100)
   - 當分數 <70 時觸發告警

2. **自動化檢測機制**:
   - 每 10 輪對話進行一次一致性檢查
   - 關鍵詞和語氣分析（輔助指標）
   - 用戶回饋收集（主觀評分）

3. **Persona 修正機制**:
   - 自動注入 Persona 提醒（Re-injection）
   - 用戶可手動觸發 Persona Reset
   - 記錄漂移事件以供分析

**技術選型**:
- 一致性評估: GPT-4 API (Judge Model)
- 語氣分析: Sentiment Analysis API
- 告警通知: SignalR WebSocket

---

#### Phase 3 (Future) - AI 驅動的 Persona 優化
**時間**: Phase 3 (第 16+ 個月)
**User Stories**: US 7.4

**實施步驟**:
1. **學習和演化**:
   - 收集用戶與 Agent 的對話歷史
   - 使用 Fine-tuning 優化 Persona 表現
   - A/B Testing 不同 Persona 版本

2. **自動化建議**:
   - 分析用戶滿意度和對話品質
   - 提供 Persona 調整建議
   - 自動優化 Few-Shot 範例選擇

---

### 成功標準

**Phase 1 (MVP)**:
- ✅ Persona Builder 可在 20 分鐘內創建高質量 Persona
- ✅ Persona 一致性分數 >70%（初步實現）
- ✅ 用戶滿意度比無 Persona 版本提升 10%+

**Phase 2**:
- ✅ Persona 一致性分數 >85%
- ✅ 自動檢測準確率 >80%
- ✅ 用戶滿意度比無 Persona 版本提升 15%+

**Phase 3**:
- ✅ AI 驅動優化使一致性分數提升 5-10%
- ✅ Fine-tuned Persona 表現優於 Prompt-based 版本
- ✅ 用戶滿意度 >85%

---

## 2. Text-to-SQL 安全策略

### 核心價值主張

**與競品差異**: 多數產品將 Text-to-SQL 視為"實驗性功能"，安全性和準確率不足。我們採用 **4 層安全防護** 和 **語義增強策略**，確保 SQL 生成既安全又準確。

### 技術難點

#### 難點 1: SQL Injection 防禦必須 100% 有效
**問題描述**:
- LLM 生成的 SQL 可能包含惡意代碼
- 傳統黑名單過濾不足以防禦所有攻擊
- 需要多層防護機制

**影響範圍**: US 8.2 (安全生成), US 8.3 (權限控制)

#### 難點 2: 複雜 SQL 生成準確率不足
**問題描述**:
- JOIN、子查詢、聚合函數的組合複雜
- 業務術語與數據庫 Schema 的語義鴻溝
- 缺乏上下文理解導致生成錯誤

**影響範圍**: US 8.1 (Schema 理解), US 8.2 (SQL 生成)

#### 難點 3: Row-Level Security 實施複雜
**問題描述**:
- 動態注入 WHERE 子句過濾用戶權限
- 不同數據庫的 RLS 實現方式不同
- 性能影響需要優化

**影響範圍**: US 8.3 (權限控制)

---

### 實施策略

#### 多層安全防護

**Layer 1: 參數化查詢（Prepared Statements）**
- 所有用戶輸入參數化處理
- 使用 SqlParameter (C#) 或類似機制
- 避免字串拼接 SQL

**Layer 2: SQL 黑名單過濾（DDL/DML/系統命令）**
```csharp
// 禁止的 SQL 關鍵字
var blacklist = new[] {
    "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "CREATE",
    "TRUNCATE", "EXEC", "EXECUTE", "xp_", "sp_",
    "GRANT", "REVOKE", "SHUTDOWN"
};

// AST 分析（語法樹檢查）
var sqlTree = SqlParser.Parse(generatedSql);
if (sqlTree.ContainsAny(blacklist)) {
    throw new SecurityException("Unsafe SQL detected");
}
```

**Layer 3: AST 分析（語法樹檢查）**
- 使用 SQL Parser 生成抽象語法樹
- 驗證只包含 SELECT 語句
- 檢查嵌套深度（防止複雜攻擊）

**Layer 4: 沙箱執行（隔離環境）**
- 使用只讀數據庫副本（Read Replica）
- 設定 Query Timeout（30 秒）
- 限制返回行數（1000 行）

---

#### 準確率提升策略

**1. Few-shot Learning（提供範例查詢）**
```yaml
schema_examples:
  - question: "過去一週的銷售總額"
    sql: "SELECT SUM(amount) FROM sales WHERE date >= NOW() - INTERVAL '7 days'"

  - question: "前 10 名暢銷產品"
    sql: "SELECT product_name, SUM(quantity) as total FROM sales GROUP BY product_name ORDER BY total DESC LIMIT 10"
```

**2. Schema 語義增強（業務術語對應）**
```yaml
semantic_mapping:
  tables:
    sales:
      synonyms: ["訂單", "銷售", "交易"]
      description: "銷售交易記錄表"

  columns:
    sales.amount:
      synonyms: ["金額", "價格", "總額"]
      business_term: "交易金額"
```

**3. 查詢驗證和自動修正**
- LLM 生成 SQL 後，先在測試環境驗證
- 如果語法錯誤，自動重新生成（最多 3 次）
- 記錄失敗案例以供學習

**4. 用戶確認機制（執行前預覽 SQL）**
- 顯示生成的 SQL 給用戶審核
- 用戶可選擇執行或修改
- 提供 "保存為範例" 功能

---

### 成功標準

**安全性**:
- ✅ 0 SQL Injection 漏洞（滲透測試驗證）
- ✅ 通過 OWASP Top 10 安全檢查
- ✅ Row-Level Security 100% 生效

**準確率**:
- ✅ 簡單查詢（單表、基礎聚合）: >85%
- ✅ 中等查詢（JOIN、子查詢）: >75%
- ✅ 複雜查詢（多重 JOIN、複雜邏輯）: >70%

**性能**:
- ✅ SQL 生成時間 <3 秒
- ✅ 查詢執行時間 <10 秒（大部分情況）
- ✅ 並發支援 >100 用戶

---

## 3. Code Interpreter 安全沙箱

### 核心價值主張

**與競品差異**: 其他產品的 Code Interpreter 通常安全性不足或功能受限。我們的 4 層安全沙箱設計確保 **既安全又靈活**，支援數據分析和可視化場景。

### 技術難點

#### 難點 1: 沙箱逃逸風險
**問題描述**:
- 惡意代碼可能嘗試訪問宿主機檔案系統
- 利用系統漏洞提權
- 繞過網路隔離

**影響範圍**: US 3.1 (沙箱實現), US 3.2 (代碼執行)

#### 難點 2: 資源濫用（無限迴圈、記憶體炸彈）
**問題描述**:
- 用戶代碼可能故意或無意造成資源耗盡
- CPU、記憶體、磁碟空間需要嚴格限制
- 超時機制需要準確觸發

**影響範圍**: US 3.1 (資源限制)

#### 難點 3: 檔案系統隔離
**問題描述**:
- 需要提供臨時檔案存儲
- 防止訪問其他用戶的檔案
- 執行後自動清理

**影響範圍**: US 3.1 (沙箱設計), US 3.3 (結果展示)

---

### 實施策略

#### 4 層安全防護（參考 US 3.1, 3.2）

**Layer 1: 程式碼靜態分析（禁止危險 API）**
```python
# 黑名單檢查
FORBIDDEN_APIS = [
    'os.system', 'subprocess', 'eval', 'exec',
    '__import__', 'open', 'file',
    'socket', 'urllib', 'requests'
]

def validate_code(code: str) -> bool:
    ast_tree = ast.parse(code)
    for node in ast.walk(ast_tree):
        if isinstance(node, ast.Call):
            if any(api in ast.unparse(node.func) for api in FORBIDDEN_APIS):
                raise SecurityError(f"Forbidden API: {ast.unparse(node.func)}")
```

**Layer 2: Docker 容器隔離**
```yaml
docker_config:
  image: python:3.11-slim
  network_mode: none  # 無網路訪問
  read_only: true     # 唯讀檔案系統（除 /tmp）
  security_opt:
    - no-new-privileges:true
    - seccomp:unconfined
  cap_drop:
    - ALL  # 移除所有 Linux Capabilities
```

**Layer 3: 資源限制（CPU、記憶體、時間）**
```yaml
resource_limits:
  cpu: 0.5          # 0.5 個 CPU 核心
  memory: 256MB     # 256 MB 記憶體
  disk: 100MB       # 100 MB 磁碟空間
  timeout: 30s      # 30 秒超時
```

**Layer 4: 網路隔離（無外部連線）**
- Docker `--network=none`
- 防火牆規則阻止所有出站流量
- 無法訪問內部網路和外部 API

---

#### 測試驗證

**1. 沙箱逃逸測試**
```python
# 測試案例：嘗試讀取宿主機檔案
test_cases = [
    "open('/etc/passwd', 'r').read()",  # 應被阻止
    "os.system('ls -la /')",            # 應被阻止
    "__import__('subprocess').run('whoami')",  # 應被阻止
]

for test in test_cases:
    try:
        execute_in_sandbox(test)
        assert False, "沙箱逃逸測試失敗：惡意代碼未被阻止"
    except SecurityError:
        pass  # 預期行為
```

**2. 資源濫用測試**
```python
# 測試案例：無限迴圈
test_cases = [
    "while True: pass",  # 應在 30 秒後超時
    "a = [1] * (10**9)",  # 應因記憶體不足而失敗
]
```

**3. 性能壓測（大量並發執行）**
- 模擬 100 個並發用戶同時執行代碼
- 驗證容器池管理機制
- 確保資源隔離有效

---

### 成功標準

**安全性**:
- ✅ 0 沙箱逃逸漏洞（滲透測試驗證）
- ✅ 資源限制 100% 生效
- ✅ 通過 OWASP Container Security 檢查

**可用性**:
- ✅ 執行成功率 >95%（正常代碼）
- ✅ 平均執行時間 <5 秒
- ✅ 支援常見數據分析庫（pandas, numpy, matplotlib）

**可擴展性**:
- ✅ 容器池支援 >100 並發執行
- ✅ 容器啟動時間 <2 秒
- ✅ 容器回收時間 <1 秒

---

## 4. Knowledge 準確率優化

### 核心價值主張

**與競品差異**: 傳統 RAG 系統檢索準確率通常 60-70%，導致 Agent 回答不準確或無法回答。我們的 **Hybrid Search + Reranking** 策略將準確率提升到 **90%+**。

### 技術難點

#### 難點 1: RAG 檢索不準確（找不到相關文件）
**問題描述**:
- 向量檢索可能錯過關鍵字匹配
- 語義相似但內容不相關的文件
- Query 表達方式與文件內容差異大

**影響範圍**: US 5.2 (檢索策略), US 5.5 (數據庫連接)

#### 難點 2: Chunk 切分策略影響準確率
**問題描述**:
- 固定長度切分可能割裂語義
- Chunk 太小導致上下文不足
- Chunk 太大導致相關性下降

**影響範圍**: US 5.1 (文件處理), US 5.2 (檢索策略)

#### 難點 3: 多模態（圖片、表格）識別困難
**問題描述**:
- PDF 中的圖表無法直接向量化
- 表格結構需要特殊處理
- 圖片中的文字需要 OCR

**影響範圍**: US 5.1 (文件處理)

---

### 實施策略

#### 1. Chunking 優化

**語義切分（基於段落和句子邊界）**
```python
def semantic_chunking(text: str, max_tokens: int = 1000) -> List[str]:
    # 按段落切分
    paragraphs = text.split('\n\n')

    chunks = []
    current_chunk = []
    current_tokens = 0

    for para in paragraphs:
        para_tokens = count_tokens(para)

        if current_tokens + para_tokens > max_tokens:
            # 完成當前 chunk
            chunks.append('\n\n'.join(current_chunk))
            current_chunk = [para]
            current_tokens = para_tokens
        else:
            current_chunk.append(para)
            current_tokens += para_tokens

    if current_chunk:
        chunks.append('\n\n'.join(current_chunk))

    return chunks
```

**保留上下文（Chunk 之間有 overlap）**
- 前一個 Chunk 的最後 100 tokens
- 後一個 Chunk 的前 100 tokens
- 避免語義割裂

**元數據增強（標題、來源、日期）**
```yaml
chunk_metadata:
  source: "產品需求文檔 v2.3"
  section: "3.2 用戶登入流程"
  page: 15
  date: "2025-10-28"
  keywords: ["登入", "身份驗證", "Session"]
```

---

#### 2. 檢索增強

**Hybrid Search（向量 + 關鍵字）**
```python
# 向量檢索（語義相似）
vector_results = azure_ai_search.vector_search(
    query_embedding=embedding,
    top_k=20
)

# 關鍵字檢索（精確匹配）
keyword_results = azure_ai_search.keyword_search(
    query=query,
    top_k=20
)

# 混合評分（0.7 向量 + 0.3 關鍵字）
hybrid_results = merge_and_rerank(
    vector_results,
    keyword_results,
    vector_weight=0.7,
    keyword_weight=0.3
)
```

**Reranking（二次排序提升準確率）**
```python
# 使用 Cross-Encoder 模型重新排序
from sentence_transformers import CrossEncoder

reranker = CrossEncoder('cross-encoder/ms-marco-MiniLM-L-6-v2')

# 對 Top 20 結果重新評分
scores = reranker.predict([(query, doc.text) for doc in hybrid_results])

# 按新分數重新排序
reranked_results = sorted(
    zip(hybrid_results, scores),
    key=lambda x: x[1],
    reverse=True
)[:5]  # 返回 Top 5
```

**Query Expansion（查詢改寫）**
```python
# 使用 LLM 擴展查詢
expanded_queries = llm.generate([
    f"改寫以下問題，生成 3 個語義相似的查詢：\n{original_query}"
])

# 對每個擴展查詢執行檢索
all_results = []
for query in [original_query] + expanded_queries:
    results = hybrid_search(query)
    all_results.extend(results)

# 去重和聚合
final_results = deduplicate_and_rank(all_results)
```

---

#### 3. 多模態處理

**圖片 OCR（Azure Computer Vision）**
```python
from azure.cognitiveservices.vision.computervision import ComputerVisionClient

# 提取圖片中的文字
ocr_result = vision_client.read(image_url)
extracted_text = '\n'.join([line.text for line in ocr_result.lines])

# 加入 Chunk
chunks.append({
    'type': 'image_ocr',
    'text': extracted_text,
    'source': 'Figure 3.2'
})
```

**表格識別（Azure Form Recognizer）**
```python
from azure.ai.formrecognizer import DocumentAnalysisClient

# 識別表格結構
table_result = form_client.analyze_document(pdf_file)

for table in table_result.tables:
    # 轉換為 Markdown 表格
    markdown_table = convert_to_markdown(table)
    chunks.append({
        'type': 'table',
        'text': markdown_table,
        'source': 'Table 2.1'
    })
```

**圖表理解（GPT-4 Vision）**
```python
# 使用 GPT-4 Vision 理解圖表
chart_description = openai.ChatCompletion.create(
    model="gpt-4-vision-preview",
    messages=[{
        "role": "user",
        "content": [
            {"type": "text", "text": "描述這個圖表的內容和趨勢"},
            {"type": "image_url", "image_url": chart_url}
        ]
    }]
)

chunks.append({
    'type': 'chart_description',
    'text': chart_description,
    'source': 'Chart 4.3'
})
```

---

### 成功標準

**檢索準確率**:
- ✅ Recall@10（前 10 個結果包含答案）: >90%
- ✅ MRR（Mean Reciprocal Rank）: >0.8
- ✅ NDCG@5: >0.85

**回應相關性**:
- ✅ 回應相關性（Relevance）: >85%
- ✅ 回應準確性（Accuracy）: >80%
- ✅ 用戶滿意度: >80%

**性能**:
- ✅ 檢索時間 <500ms
- ✅ 端到端響應時間 <2s
- ✅ 支援文檔數量: >10,000 個

---

## 5. 實施優先級總結

### Phase 1 (MVP) - 月 1-9

| 能力 | 優先級 | 實施時間 | 關鍵 User Stories |
|------|-------|---------|------------------|
| **Knowledge 準確率** | 最高 | 月 4-5 | US 5.1, 5.2, 5.3 |
| **Persona Framework** | 高 | 月 7 | US 1.5, 7.1, 7.2 |
| **Code Interpreter** | 高 | 月 4 | US 3.1, 3.2 |
| **Text-to-SQL** | 中 | 月 8-9 | US 8.1, 8.2, 8.3 |

### Phase 2 (Post-MVP) - 月 10-15

| 能力 | 優先級 | 實施時間 | 關鍵 User Stories |
|------|-------|---------|------------------|
| **Persona 一致性驗證** | 中 | 月 13-14 | US 7.3 |
| **Text-to-SQL 進階** | 中 | 月 12-13 | US 8.4 |
| **Knowledge 自動化** | 低 | 月 14-15 | US 5.6, 5.7 |

---

## 6. 風險與緩解

### 高風險項目

**Risk 1: Persona 一致性分數無法達到 85%**
- **緩解措施**: 降低目標到 70%（MVP），延後高標準到 Phase 2
- **Plan B**: 使用 Fine-tuning 代替 Prompt Engineering

**Risk 2: Text-to-SQL 準確率不足 85%**
- **緩解措施**: 限制支援的 SQL 複雜度，提供人工審核選項
- **Plan B**: 改為"SQL 建議"而非"自動生成"

**Risk 3: Code Interpreter 沙箱被繞過**
- **緩解措施**: 聘請安全顧問進行滲透測試
- **Plan B**: 延遲發布直到通過安全審計

---

## 7. 相關文檔

### 核心文檔
- [📚 主索引](../README.md) - 完整文檔導航和進度追蹤
- [📋 User Stories README](./README.md) - User Stories 總覽
- [🎯 MVP 規劃](./mvp-planning.md) - MVP 範圍與時程規劃

### 架構與規劃
- [🏗️ Architecture Design Document](../architecture/Architecture-Design-Document.md) - 系統架構設計
- [📊 Project Management Plan](../project-management/Project-Management-Plan.md) - 項目管理計劃
- [📋 Sprint Backlog](./sprints/sprint-backlog.md) - Sprint 待辦清單
- [📅 Sprint Allocation](./sprints/sprint-allocation.md) - Sprint 分配詳情

### 未來階段
- [🎨 UI/UX 設計](../ux-design/README.md) - Stage 3.3 規劃
- [🔬 技術實施](../technical-implementation/README.md) - Stage 3.4 規劃

---

**最後更新**: 2025-10-28
**狀態**: 策略規劃完成，待 Stage 3.4 (Tech Lead) 驗證
**下一步**: PoC 驗證關鍵技術（Stage 3.4）
