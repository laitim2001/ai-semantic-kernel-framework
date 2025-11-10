# Sprint 8 概覽 - Code Interpreter 可視化 (核心差異化功能)

**Sprint 編號**: Sprint 8
**週次**: Week 22-24
**計劃日期**: 2026-04-06 ~ 2026-04-26 (21 days)
**狀態**: ⏳ **未開始**

---

## 🎯 Sprint 目標

實現**核心差異化能力 #3 - Agent 自動生成可視化代碼**,讓 Agent 根據數據自動生成 Matplotlib/Seaborn 可視化代碼並執行。

**關鍵交付物**:
1. ⏳ LLM 生成 Python 可視化代碼
2. ⏳ Matplotlib/Seaborn 集成
3. ⏳ 圖表生成和保存機制
4. ⏳ 代碼執行結果展示 UI
5. ⏳ 多類型圖表支持 (折線/柱狀/散點/熱圖)

---

## 📊 User Stories

| User Story | SP | 計劃天數 | 模組 | 優先級 |
|-----------|----|---------|----- |-------|
| **US 3.2** - Agent 自動生成可視化代碼 ⭐ | 8 | 12 days | Module 3 | P0 - 核心差異化 |
| **US 3.3** - Code Interpreter 結果展示 (UI 增強) | 5 | 9 days | Module 3 | P0 |

**總計**: 13 SP

---

## 📋 核心技術任務

### US 3.2 - 自動可視化代碼生成

#### LLM Prompt Engineering
```yaml
Prompt Template:
  """
  Given this data: {data_summary}
  User request: {user_query}

  Generate Python code using matplotlib/seaborn to visualize this data.
  Requirements:
  - Use pandas for data manipulation
  - Save figure as PNG to /tmp/chart.png
  - Include proper labels and title
  - Handle missing values

  Output only executable Python code.
  """

示例:
  Data: [sales data with date, revenue, category]
  User: "Show revenue trend over time by category"

  Generated Code:
    import matplotlib.pyplot as plt
    import pandas as pd
    df = pd.read_json('data.json')
    df.pivot_table(values='revenue', index='date', columns='category').plot()
    plt.savefig('/tmp/chart.png')
```

#### 錯誤修復機制
```yaml
Self-Healing:
  1. 執行生成的代碼
  2. 如果錯誤,將錯誤信息發送給 LLM
  3. LLM 修復代碼
  4. 重新執行 (最多 3 次)
```

### US 3.3 - 結果展示 UI

#### Frontend 組件
```yaml
CodeBlock Component:
  - Syntax Highlighting (Prism.js)
  - Copy to Clipboard
  - Line Numbers

ChartDisplay Component:
  - Image Rendering
  - Zoom/Pan 功能
  - Download 按鈕

ResultsPanel:
  - Tabs: Code, Output, Chart
  - Error Display
```

---

## 🧪 測試策略

### 代碼生成質量測試
```yaml
測試場景 (20 cases):
  - 折線圖: 時間序列數據
  - 柱狀圖: 分類比較
  - 散點圖: 相關性分析
  - 熱圖: 矩陣數據
  - 餅圖: 比例分布

評估標準:
  - 代碼可執行率: ≥ 95%
  - 圖表正確性: ≥ 90%
  - 自動修復成功率: ≥ 80%
```

---

## 🎯 驗收標準

### 功能驗收
- ✅ 支持 5 種圖表類型
- ✅ 代碼可執行率 ≥ 95%
- ✅ 圖表正確性 ≥ 90%
- ✅ UI 展示完整

### 性能驗收
- ✅ 代碼生成 < 3 seconds
- ✅ 圖表渲染 < 2 seconds

---

## 🎓 關鍵風險

### 風險 1: 生成代碼質量不穩定
**影響**: 高 🔴
**緩解**: Few-shot Examples, 錯誤修復機制, Prompt 優化

---

## 🔗 相關文檔

- **依賴 Sprint**: Sprint 7 (US 3.1 必須完成)
- **後續 Sprint**: Sprint 9 (US 6.4 - 代碼塊展示增強)

---

**狀態**: ⏳ 規劃中
