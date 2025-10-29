# 模組 03: Code Interpreter

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 03
**User Stories**: US 3.1-3.3
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 3: Code Interpreter ⭐ 核心競爭力

#### Epic 3: 代碼執行與可視化

**目標**: 提供安全的 Python 代碼執行環境，支援自動生成可視化圖表

---

#### US 3.1 - Python 代碼執行（4 層安全沙箱）

**作為** Agent（系統內部）
**我想要** 在安全沙箱中執行 Python 代碼
**以便** 進行數據分析和複雜計算

**背景**:
這是本項目的**核心差異化能力之一**，彌補 Copilot Studio 無法執行代碼的限制。安全性是首要考量，必須實施 4 層防護。

**驗收標準**:

✅ 必須項（4 層安全防護）:

**Layer 1: Network Isolation（網絡隔離）**
- [ ] Docker Container 使用 `--network=none`（無外網訪問）
- [ ] 禁止訪問任何外部 API 或服務
- [ ] 僅允許與宿主機通訊（代碼輸入、結果輸出）

**Layer 2: Resource Limits（資源限制）**
- [ ] CPU 限制: 1 core（`--cpus=1`）
- [ ] Memory 限制: 512MB（`--memory=512m`）
- [ ] Disk 空間限制: 100MB（`--storage-opt size=100m`）
- [ ] Process 數量限制: 50（`--pids-limit=50`）
- [ ] 資源超限自動終止

**Layer 3: Execution Timeout（執行超時）**
- [ ] 硬性超時: 30 秒（強制終止）
- [ ] 軟性超時: 25 秒（警告提示）
- [ ] 超時後返回部分結果（如果有）
- [ ] 超時統計和告警

**Layer 4: Read-Only Filesystem（唯讀文件系統）**
- [ ] 文件系統唯讀掛載（`--read-only`）
- [ ] 僅 `/tmp` 可寫（100MB, `noexec`）
- [ ] 禁止寫入系統目錄
- [ ] 禁止執行二進制文件（`noexec` mount option）

✅ 必須項（Python 環境）:
- [ ] 預安裝套件: NumPy, Pandas, Matplotlib, Seaborn, SciPy
- [ ] Python 版本: 3.11+
- [ ] 支援中文字體（Matplotlib 顯示中文）
- [ ] 禁止安裝新套件（`pip install` 不可用）

✅ 必須項（執行功能）:
- [ ] 支援多行代碼執行
- [ ] 返回 stdout（標準輸出）
- [ ] 返回 stderr（錯誤輸出）
- [ ] 返回 return value（函數返回值）
- [ ] 返回生成的圖表（PNG/SVG）
- [ ] 返回生成的文件列表（/tmp 下）

✅ 必須項（錯誤處理）:
- [ ] 語法錯誤友好提示（行號、錯誤類型）
- [ ] 執行錯誤完整堆疊追蹤
- [ ] 超時錯誤明確提示
- [ ] 資源超限錯誤提示
- [ ] 安全違規錯誤提示（嘗試訪問網絡等）

✅ 必須項（Container Pool）:
- [ ] 預熱 5 個 Standby Containers
- [ ] 從 Pool 獲取 Container <2 秒
- [ ] 執行後銷毀 Container（不重用）
- [ ] 異步非阻塞補充 Pool
- [ ] Pool 使用率監控

**範例執行**:
```python
# 用戶代碼
import pandas as pd
import matplotlib.pyplot as plt

data = pd.DataFrame({
    'month': ['Jan', 'Feb', 'Mar', 'Apr'],
    'sales': [1000, 1200, 1100, 1300]
})

plt.figure(figsize=(10, 6))
plt.plot(data['month'], data['sales'], marker='o')
plt.title('月度銷售趨勢', fontsize=14)
plt.xlabel('月份')
plt.ylabel('銷售額')
plt.grid(True)
plt.savefig('/tmp/sales_trend.png', dpi=150)

print(data.describe())
```

**返回結果**:
```json
{
  "status": "success",
  "execution_time": 1.23,
  "stdout": "       sales\ncount    4.0\nmean  1150.0\nstd    129.1\nmin   1000.0\nmax   1300.0",
  "stderr": "",
  "return_value": null,
  "files": [
    {
      "path": "/tmp/sales_trend.png",
      "url": "https://blob.storage/executions/abc123/sales_trend.png",
      "mime_type": "image/png",
      "size_bytes": 45678
    }
  ]
}
```

**技術實現**:
- Docker Engine API
- Container Orchestration
- Azure Blob Storage（圖片儲存）

**📊 優先級**: P0 (MVP 必須，核心競爭力)
**🎯 性能目標**: P95 執行時間 <8 秒
**🔒 安全要求**: 通過安全審計，無 Container 逃逸風險
**⏱️ 開發工期**: 3-4 週
**🔗 相關**: US 3.2 (自動生成代碼), ADR-008 (執行模型)

---

#### US 3.2 - Agent 自動生成可視化代碼 ⭐ 關鍵能力

**作為** 業務用戶
**我想要** Agent 自動生成數據分析代碼並執行
**以便** 獲得視覺化的分析結果

**背景**:
這是 Code Interpreter 的核心價值體現。Agent 不只執行用戶提供的代碼，更重要的是能夠**自主生成代碼**來完成分析任務。

**驗收標準**:

✅ 必須項（代碼生成能力）:
- [ ] Agent 可根據數據和問題自動生成 Python 代碼
- [ ] 支援常見數據分析任務（統計、聚合、過濾、排序）
- [ ] 支援常見圖表類型：
  - 折線圖（Line Chart）
  - 柱狀圖（Bar Chart）
  - 散點圖（Scatter Plot）
  - 圓餅圖（Pie Chart）
  - 熱力圖（Heatmap）
  - 箱型圖（Box Plot）

✅ 必須項（代碼質量）:
- [ ] 生成的代碼可讀性良好（變量命名清晰）
- [ ] 包含註解說明關鍵步驟
- [ ] 自動處理中文顯示（設定字體）
- [ ] 圖表包含標題、軸標籤、圖例
- [ ] 錯誤處理（數據為空、類型錯誤等）

✅ 必須項（執行流程）:
- [ ] Agent 生成代碼 → 展示代碼給用戶（可選）
- [ ] 執行代碼 → 獲取結果
- [ ] 返回：圖片 URL + 文字說明
- [ ] 如果執行失敗，Agent 修正代碼並重試（最多 2 次）

✅ 必須項（多步驟分析）:
- [ ] 支援多步驟代碼執行（Step 1 加載數據 → Step 2 清理 → Step 3 可視化）
- [ ] 步驟間數據傳遞（通過文件或變量）
- [ ] 執行進度展示（當前步驟、總步驟數）

**互動範例**:

**場景 1: 銷售趨勢分析**
```
用戶: "分析這個銷售數據的趨勢"
[上傳 sales.csv]

Agent:
"我來幫您分析銷售趨勢。首先載入數據..."

[Agent 生成並執行代碼:]
```python
import pandas as pd
import matplotlib.pyplot as plt

# 載入數據
data = pd.read_csv('/tmp/sales.csv')
print(f"數據概覽:\n{data.head()}")

# 繪製趨勢圖
plt.figure(figsize=(12, 6))
plt.plot(data['date'], data['revenue'], marker='o', linewidth=2)
plt.title('銷售收入趨勢', fontsize=16)
plt.xlabel('日期')
plt.ylabel('收入（元）')
plt.grid(True, alpha=0.3)
plt.xticks(rotation=45)
plt.tight_layout()
plt.savefig('/tmp/sales_trend.png', dpi=150)
```

Agent:
"從趨勢圖可以看出：
📈 銷售收入呈現穩定上升趨勢
💡 Q3 增長最快，增長率達 25%
⚠️ 10月有小幅下滑，建議關注

[圖片: sales_trend.png]"
```

**場景 2: 多維度數據分析**
```
用戶: "比較不同地區的銷售表現"

Agent:
"我來分析各地區的銷售表現，包括總銷售額和平均客單價。"

[Agent 生成並執行代碼:]
```python
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# 載入數據
data = pd.read_csv('/tmp/sales.csv')

# 按地區聚合
region_stats = data.groupby('region').agg({
    'revenue': ['sum', 'mean', 'count']
}).round(2)

print(region_stats)

# 繪製對比圖
fig, axes = plt.subplots(1, 2, figsize=(14, 6))

# 左圖：總銷售額
axes[0].bar(region_stats.index, region_stats[('revenue', 'sum')])
axes[0].set_title('各地區總銷售額')
axes[0].set_ylabel('銷售額（元）')

# 右圖：平均客單價
axes[1].bar(region_stats.index, region_stats[('revenue', 'mean')])
axes[1].set_title('各地區平均客單價')
axes[1].set_ylabel('平均金額（元）')

plt.tight_layout()
plt.savefig('/tmp/region_comparison.png', dpi=150)
```

Agent:
"各地區銷售表現分析：

🏆 台北：總銷售額第一（$500,000），但客單價中等
💼 新竹：客單價最高（$1,200），科技園區消費力強
📊 台中：訂單量最多，適合薄利多銷策略

[圖片: region_comparison.png]

建議：
1. 加強台北地區的高端產品推廣
2. 複製新竹的高單價銷售模式到其他地區"
```

✅ 必須項（質量控制）:
- [ ] 圖表生成成功率 >95%
- [ ] 代碼執行成功率 >90%（首次嘗試）
- [ ] 修正後成功率 >98%（含重試）
- [ ] 用戶滿意度（UAT 測試）>85%

**技術實現**:
- GPT-4o 或 GPT-4 生成代碼
- Prompt Engineering（Few-Shot 範例）
- Code Validation（語法檢查）

**📊 優先級**: P0 (MVP 必須)
**🎯 準確率目標**: 圖表生成成功率 >95%
**⏱️ 開發工期**: 2-3 週
**🔗 相關**: US 3.1 (代碼執行), US 6.3 (圖表展示)

---

#### US 3.3 - Code Interpreter 結果展示（UI 增強）

**作為** 業務用戶
**我想要** 在 Chat 介面中清晰展示代碼執行結果
**以便** 理解 Agent 的分析過程和結果

**驗收標準**:

✅ 必須項（代碼塊展示）:
- [ ] 代碼塊語法高亮（Python 語法）
- [ ] 顯示語言標籤（```python）
- [ ] 一鍵複製代碼按鈕
- [ ] 行號顯示（可選開關）
- [ ] 代碼折疊/展開（超過 20 行自動折疊）
- [ ] 深色/淺色主題適配

✅ 必須項（執行結果展示）:
- [ ] 執行結果與代碼區分展示（不同背景色）
- [ ] stdout 輸出格式化（保持空白和縮排）
- [ ] stderr 錯誤訊息紅色標註
- [ ] 執行時間顯示（1.23s）
- [ ] 執行狀態圖示（成功✅、失敗❌、執行中⏳）

✅ 必須項（圖片展示）:
- [ ] 圖片內嵌顯示（自動載入）
- [ ] 圖片點擊放大預覽（Lightbox）
- [ ] 圖片下載按鈕
- [ ] 支援格式: PNG, SVG
- [ ] 圖片載入失敗顯示佔位符

✅ 必須項（互動功能）:
- [ ] 代碼可編輯並重新執行（進階功能）
- [ ] 執行歷史記錄（查看之前的代碼版本）
- [ ] 分享代碼和結果（生成分享連結）

**UI 設計參考**:
- Jupyter Notebook 代碼單元格樣式
- Claude/ChatGPT Code Interpreter UI
- GitHub 代碼塊樣式

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ Agent 回應:                              │
│ 我來幫您分析銷售數據的趨勢。             │
│                                          │
│ [代碼塊]                                 │
│ ┌───────────────────────────────────┐   │
│ │ ```python            [複製] [▼]   │   │
│ │  1  import pandas as pd           │   │
│ │  2  import matplotlib.pyplot as...│   │
│ │  3                                │   │
│ │  4  data = pd.read_csv(...)       │   │
│ │  ...                              │   │
│ └───────────────────────────────────┘   │
│                                          │
│ [執行結果] ✅ 1.23s                      │
│ ┌───────────────────────────────────┐   │
│ │ 數據概覽:                          │   │
│ │    date     revenue  region       │   │
│ │ 0  2024-01  50000    台北         │   │
│ │ 1  2024-02  52000    台北         │   │
│ └───────────────────────────────────┘   │
│                                          │
│ [圖片] 📊 sales_trend.png [下載] [🔍]   │
│ [圖片內嵌顯示]                           │
│                                          │
│ 從趨勢圖可以看出...                      │
└─────────────────────────────────────────┘
```

**技術要求**:
- Syntax Highlighter: Prism.js 或 Highlight.js
- Lightbox: React Image Lightbox
- Code Editor: CodeMirror 或 Monaco Editor（進階功能）

**📊 優先級**: P0 (MVP 必須)
**🎯 UX 目標**: 用戶可清晰理解代碼執行過程
**🔗 相關**: US 3.1 (代碼執行), US 6.4 (代碼塊通用展示)

---