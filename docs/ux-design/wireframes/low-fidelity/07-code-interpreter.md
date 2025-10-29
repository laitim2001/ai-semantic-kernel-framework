# Code Interpreter Wireframe

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成
**複雜度**: ⭐⭐⭐ (中等)

---

## 📋 頁面概覽

### 目的
提供類似 Jupyter Notebook 的代碼執行環境，讓 Agent 能夠執行 Python/JavaScript 代碼、分析數據、產生可視化圖表，並與用戶進行交互式數據探索。

### 目標用戶
- **Alex（IT 開發者）**: 執行複雜數據分析、測試演算法、除錯代碼
- **Sarah（業務分析師）**: 使用預建模板進行數據分析、產生報表和圖表
- **David（企業管理員）**: 監控代碼執行、管理資源使用、控制安全性

### 成功指標
- 代碼執行成功率 > 99%
- 執行響應時間 < 2s（簡單代碼）
- 圖表渲染時間 < 500ms
- Notebook 載入時間 < 1s
- 用戶滿意度評分 > 4.5/5

---

## 🖥️ Desktop 版本佈局（≥1280px）

### Notebook 主視圖（2-Column Layout）

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│ ← Back   [💻 Code Interpreter] [代碼解釋器]                 ⚙️ 🔔 👤 Alex          │
├──────────────────────────────────────────────────────┬──────────────────────────────┤
│                                                      │                              │
│  💻 Data Analysis Notebook                          │  📊 環境資訊                 │
│  最後執行：5 分鐘前 • Python 3.11                    │                              │
│                                                      │  ┌──────────────────────────┐│
│  ┌────────────────────────────────────────────────┐ │  │ 🐍 Python 3.11.5         ││
│  │ [▶️ Run All] [⏹️ Stop] [🔄 Restart Kernel]      │ │  │ 🟢 Kernel 運行中         ││
│  │ [+ Code] [+ Markdown]              [💾 Save]   │ │  │                          ││
│  └────────────────────────────────────────────────┘ │  │ 📦 已安裝套件 (45)       ││
│                                                      │  │ • numpy 1.24.3           ││
│  ┌────────────────────────────────────────────────┐ │  │ • pandas 2.0.2           ││
│  │ Cell 1: Code                        [⋮]        │ │  │ • matplotlib 3.7.1       ││
│  │ ┌────────────────────────────────────────────┐ │  │ • ...                    ││
│  │ │ import pandas as pd                       │ │  │                          ││
│  │ │ import numpy as np                        │ │  │ [管理套件]               ││
│  │ │ import matplotlib.pyplot as plt           │ │  └──────────────────────────┘│
│  │ │                                           │ │                              │
│  │ │ # Load data                               │ │  💾 資源使用                 │
│  │ │ df = pd.read_csv('sales_data.csv')       │ │  ┌──────────────────────────┐│
│  │ │ print(df.head())                          │ │  │ 🖥️ CPU                   ││
│  │ │▋                                          │ │  │ ███░░░░░░░ 25%           ││
│  │ └────────────────────────────────────────────┘ │  │                          ││
│  │                                                │ │  │ 💾 記憶體                ││
│  │ [▶️ Run] [+ 插入下方]    ⏱️ 執行時間：0.45s    │ │  │ ████░░░░░░ 34%           ││
│  └────────────────────────────────────────────────┘ │  │ 512 MB / 1.5 GB          ││
│                                                      │  │                          ││
│  ┌────────────────────────────────────────────────┐ │  │ 📀 磁碟                  ││
│  │ Output 1:                                      │ │  │ ██░░░░░░░░ 18%           ││
│  │ ┌────────────────────────────────────────────┐ │  │ 450 MB / 2.5 GB          ││
│  │ │      Date    Product  Quantity   Price    │ │  └──────────────────────────┘│
│  │ │ 0  2025-10-01  Widget      120   $1,234  │ │                              │
│  │ │ 1  2025-10-02  Gadget       85   $2,345  │ │  📂 文件瀏覽器               │
│  │ │ 2  2025-10-03  Widget      150   $1,567  │ │  ┌──────────────────────────┐│
│  │ │ 3  2025-10-04  Doohickey    92   $890    │ │  │ 📁 workspace/            ││
│  │ │ 4  2025-10-05  Widget      108   $1,123  │ │  │ ├─ 📄 sales_data.csv     ││
│  │ └────────────────────────────────────────────┘ │  │ ├─ 📄 analysis.py        ││
│  │                                                │ │  │ ├─ 📊 chart.png         ││
│  │ [展開] [複製] [下載]                           │ │  │ └─ 📁 results/          ││
│  └────────────────────────────────────────────────┘ │  │   └─ 📄 report.pdf      ││
│                                                      │  │                          ││
│  ┌────────────────────────────────────────────────┐ │  │ [📤 上傳] [📁 新建]      ││
│  │ Cell 2: Code                        [⋮]        │ │  └──────────────────────────┘│
│  │ ┌────────────────────────────────────────────┐ │                              │
│  │ │ # Calculate statistics                    │ │  🔍 變數檢視器               │
│  │ │ total_sales = df['Quantity'].sum()        │ │  ┌──────────────────────────┐│
│  │ │ avg_price = df['Price'].mean()            │ │  │ 變數名 | 類型 | 值       ││
│  │ │                                           │ │  ├──────────────────────────┤│
│  │ │ print(f"Total: {total_sales}")            │ │  │ df     | DataFrame | (5,4)││
│  │ │ print(f"Avg: ${avg_price:.2f}")           │ │  │ total  | int | 555       ││
│  │ │▋                                          │ │  │ avg    | float | 1431.8  ││
│  │ └────────────────────────────────────────────┘ │  │ plt    | module | ...    ││
│  │                                                │ │  │                          ││
│  │ [▶️ Run] [+ 插入下方]    🔄 執行中...          │ │  │ [刷新]                   ││
│  └────────────────────────────────────────────────┘ │  └──────────────────────────┘│
│                                                      │                              │
│  ┌────────────────────────────────────────────────┐ │  ⏱️ 執行歷史                 │
│  │ Cell 3: Code                        [⋮]        │ │  ┌──────────────────────────┐│
│  │ ┌────────────────────────────────────────────┐ │  │ Cell 1 ✅ 0.45s (5min前) ││
│  │ │ # Create visualization                    │ │  │ Cell 2 🔄 執行中...      ││
│  │ │ plt.figure(figsize=(10, 6))               │ │  │ Cell 1 ✅ 0.52s (10min前)││
│  │ │ plt.plot(df['Date'], df['Quantity'])      │ │  │ Cell 3 ❌ 錯誤 (15min前) ││
│  │ │ plt.title('Sales Trend')                  │ │  │ ...                      ││
│  │ │ plt.xlabel('Date')                        │ │  │                          ││
│  │ │ plt.ylabel('Quantity')                    │ │  │ [清除歷史]               ││
│  │ │ plt.show()                                │ │  └──────────────────────────┘│
│  │ └────────────────────────────────────────────┘ │                              │
│  │                                                │                              │
│  │ [▶️ Run] [+ 插入下方]    ⏳ 未執行             │                              │
│  └────────────────────────────────────────────────┘                              │
│                                                                                   │
│  [ + 添加 Cell ]                                                                 │
│                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────┘
```

---

## 📊 內容層級（Desktop）

### Level 1: 頁面結構（2-Column Layout）

```
┌──────────────────┬──────────────┐
│ 左側主區域        │ 右側面板     │
│ (60%)            │ (40%)        │
│                  │              │
│ • Toolbar        │ • 環境資訊   │
│ • Cell 列表      │ • 資源使用   │
│ • Code + Output  │ • 文件瀏覽   │
│                  │ • 變數檢視   │
│                  │ • 執行歷史   │
└──────────────────┴──────────────┘
```

### Level 2: Toolbar

```
┌──────────────────────────────────────────────────┐
│ [▶️ Run All] [⏹️ Stop] [🔄 Restart Kernel]      │ ← 執行控制
│ [+ Code] [+ Markdown]              [💾 Save]   │ ← Cell 操作 + 儲存
└──────────────────────────────────────────────────┘

按鈕說明：
▶️ Run All    - 執行所有 Cells（從上到下）
⏹️ Stop       - 停止當前執行
🔄 Restart    - 重啟 Kernel（清空所有變數）
+ Code       - 新增代碼 Cell
+ Markdown   - 新增 Markdown Cell（文檔）
💾 Save      - 儲存 Notebook
```

### Level 3: Code Cell

```
┌────────────────────────────────────────────────┐
│ Cell 1: Code                        [⋮]        │ ← Cell 類型 + 操作選單
│ ┌────────────────────────────────────────────┐ │
│ │ import pandas as pd                       │ │ ← 代碼編輯器
│ │ import numpy as np                        │ │   (Monaco Editor)
│ │ import matplotlib.pyplot as plt           │ │
│ │                                           │ │
│ │ # Load data                               │ │
│ │ df = pd.read_csv('sales_data.csv')       │ │
│ │ print(df.head())                          │ │
│ │▋                                          │ │ ← 游標
│ └────────────────────────────────────────────┘ │
│                                                │
│ [▶️ Run] [+ 插入下方]    ⏱️ 執行時間：0.45s    │ ← 操作按鈕 + 執行狀態
└────────────────────────────────────────────────┘

Cell 狀態：
⏳ 未執行      - 灰色邊框
🔄 執行中      - 藍色邊框 + 動畫
✅ 成功        - 綠色邊框 + 執行時間
❌ 錯誤        - 紅色邊框 + 錯誤訊息
```

### Level 4: Output Cell

```
┌────────────────────────────────────────────────┐
│ Output 1:                                      │ ← Output 標籤
│ ┌────────────────────────────────────────────┐ │
│ │      Date    Product  Quantity   Price    │ │ ← 輸出內容
│ │ 0  2025-10-01  Widget      120   $1,234  │ │   (表格、文字、圖表)
│ │ 1  2025-10-02  Gadget       85   $2,345  │ │
│ │ 2  2025-10-03  Widget      150   $1,567  │ │
│ │ 3  2025-10-04  Doohickey    92   $890    │ │
│ │ 4  2025-10-05  Widget      108   $1,123  │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ [展開] [複製] [下載]                           │ ← 操作按鈕
└────────────────────────────────────────────────┘

Output 類型：
📝 文字輸出     - 純文字、print 結果
📊 表格         - DataFrame、數據表格
📈 圖表         - Matplotlib、Plotly 圖表
🖼️ 圖片         - PNG、JPG 圖片
⚠️ 錯誤         - Traceback、錯誤訊息
```

### Level 5: 右側面板

#### 環境資訊
```
┌──────────────────────────────────┐
│ 📊 環境資訊                      │
│                                  │
│ 🐍 Python 3.11.5                 │ ← Runtime 版本
│ 🟢 Kernel 運行中                 │ ← Kernel 狀態
│                                  │
│ 📦 已安裝套件 (45)               │ ← 套件列表
│ • numpy 1.24.3                   │
│ • pandas 2.0.2                   │
│ • matplotlib 3.7.1               │
│ • scikit-learn 1.2.2             │
│ • ...                            │
│                                  │
│ [管理套件]                       │ ← 套件管理按鈕
└──────────────────────────────────┘
```

#### 資源使用
```
┌──────────────────────────────────┐
│ 💾 資源使用                      │
│                                  │
│ 🖥️ CPU                           │
│ ███░░░░░░░ 25%                   │ ← 即時 CPU 使用率
│                                  │
│ 💾 記憶體                        │
│ ████░░░░░░ 34%                   │ ← 即時記憶體使用
│ 512 MB / 1.5 GB                  │
│                                  │
│ 📀 磁碟                          │
│ ██░░░░░░░░ 18%                   │ ← 磁碟使用
│ 450 MB / 2.5 GB                  │
└──────────────────────────────────┘
```

#### 文件瀏覽器
```
┌──────────────────────────────────┐
│ 📂 文件瀏覽器                    │
│                                  │
│ 📁 workspace/                    │ ← 工作目錄
│ ├─ 📄 sales_data.csv             │
│ ├─ 📄 analysis.py                │
│ ├─ 📊 chart.png                  │
│ └─ 📁 results/                   │
│    └─ 📄 report.pdf              │
│                                  │
│ [📤 上傳] [📁 新建]              │ ← 文件操作
└──────────────────────────────────┘

操作：
• 點擊文件：插入檔案路徑到當前 Cell
• 右鍵選單：下載、刪除、重命名
• 拖拽：上傳文件
```

#### 變數檢視器
```
┌──────────────────────────────────┐
│ 🔍 變數檢視器                    │
│                                  │
│ 變數名 | 類型      | 值          │
├──────────────────────────────────┤
│ df     | DataFrame | (5, 4)     │ ← 點擊展開詳情
│ total  | int       | 555        │
│ avg    | float     | 1431.8     │
│ plt    | module    | ...        │
│                                  │
│ [刷新]                           │ ← 手動刷新變數
└──────────────────────────────────┘

展開 DataFrame：
┌──────────────────────────────────┐
│ 📊 df - DataFrame (5 rows × 4 columns)│
│                                  │
│      Date    Product  Quantity  │
│ 0  2025-10-01  Widget      120  │
│ 1  2025-10-02  Gadget       85  │
│ ...                              │
│                                  │
│ [匯出 CSV] [關閉]                │
└──────────────────────────────────┘
```

#### 執行歷史
```
┌──────────────────────────────────┐
│ ⏱️ 執行歷史                      │
│                                  │
│ Cell 1 ✅ 0.45s (5 分鐘前)       │ ← 成功執行
│ Cell 2 🔄 執行中...              │ ← 當前執行
│ Cell 1 ✅ 0.52s (10 分鐘前)      │ ← 歷史執行
│ Cell 3 ❌ 錯誤 (15 分鐘前)       │ ← 執行錯誤
│ ...                              │
│                                  │
│ [清除歷史]                       │
└──────────────────────────────────┘
```

---

## 📱 Tablet 版本佈局（768px - 1279px）

### 可折疊側邊欄

```
┌─────────────────────────────────────────────────────────┐
│ ☰  [💻 Data Analysis]              ⚙️ 🔔 👤 Alex       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [▶️ All] [⏹️] [🔄] [+ Code] [💾]    [📊 面板 ▾]        │
│                                                         │
│  ┌───────────────────────────────────────────────────┐ │
│  │ Cell 1: Code                        [⋮]           │ │
│  │ ┌───────────────────────────────────────────────┐ │ │
│  │ │ import pandas as pd                          │ │ │
│  │ │ df = pd.read_csv('data.csv')                 │ │ │
│  │ │ print(df.head())                             │ │ │
│  │ └───────────────────────────────────────────────┘ │ │
│  │ [▶️ Run]                      ⏱️ 0.45s            │ │
│  └───────────────────────────────────────────────────┘ │
│                                                         │
│  ┌───────────────────────────────────────────────────┐ │
│  │ Output 1:                                         │ │
│  │ [表格顯示...]                                     │ │
│  └───────────────────────────────────────────────────┘ │
│                                                         │
│  ⋮                                                      │
│                                                         │
└─────────────────────────────────────────────────────────┘

點擊「📊 面板」彈出側邊欄：
                    ┌──────────────────────┐
                    │ 📊 環境資訊          │
                    │ 🐍 Python 3.11       │
                    │ 🟢 運行中            │
                    │                      │
                    │ 💾 資源              │
                    │ CPU: 25%             │
                    │ RAM: 34%             │
                    │                      │
                    │ 📂 文件              │
                    │ [文件列表...]        │
                    │                      │
                    │ [關閉 ✕]             │
                    └──────────────────────┘
```

---

## 📱 Mobile 版本佈局（<768px）

### 全屏 Notebook 視圖

```
┌────────────────────────────────┐
│ ← [Data Analysis]         ⋮    │
├────────────────────────────────┤
│                                │
│  [▶️] [⏹️] [🔄] [+] [💾]        │
│                                │
│ ┌────────────────────────────┐│
│ │ Cell 1: Code          [⋮]  ││
│ │                            ││
│ │ import pandas as pd        ││
│ │ df = pd.read_csv(...)      ││
│ │ print(df.head())           ││
│ │                            ││
│ │ [▶️ Run]        ⏱️ 0.45s    ││
│ └────────────────────────────┘│
│                                │
│ ┌────────────────────────────┐│
│ │ Output 1:                  ││
│ │ [表格顯示...]              ││
│ │ [展開]                     ││
│ └────────────────────────────┘│
│                                │
│ ⋮                              │
│                                │
│ [ + 添加 Cell ]                │
│                                │
└────────────────────────────────┘

底部選單（點擊 ⋮）：
┌────────────────────────────────┐
│ 📊 查看環境資訊                │
│ 💾 查看資源使用                │
│ 📂 文件瀏覽器                  │
│ 🔍 變數檢視器                  │
│ ⏱️ 執行歷史                    │
│ ⚙️ Notebook 設定               │
│ ─────────────────────          │
│ ✕ 關閉                         │
└────────────────────────────────┘
```

---

## 🎯 互動元素詳細說明

### 1. Cell 操作

#### Cell 操作選單（點擊 [⋮]）
```
┌────────────────────────────┐
│ ▶️ 執行此 Cell              │
│ ▲ 執行此 Cell 及之前所有    │
│ ▼ 執行此 Cell 及之後所有    │
│ ─────────────────          │
│ ✂️ 剪下                     │
│ 📋 複製                     │
│ 📄 貼上                     │
│ ─────────────────          │
│ ⬆️ 向上移動                 │
│ ⬇️ 向下移動                 │
│ ─────────────────          │
│ 🔄 轉換為 Markdown         │
│ 🗑️ 刪除 Cell                │
└────────────────────────────┘
```

#### Cell 執行狀態
```
⏳ 未執行：
┌────────────────────────────────────────────────┐
│ Cell 1: Code                        [⋮]        │
│ [代碼...]                                      │
│ [▶️ Run] [+ 插入下方]    ⏳ 未執行             │
└────────────────────────────────────────────────┘

🔄 執行中：
┌────────────────────────────────────────────────┐
│ Cell 1: Code                        [⋮]        │ ← 藍色邊框
│ [代碼...]                                      │
│ [⏹️ Stop]                 🔄 執行中... (2.3s)  │ ← 動畫 + 計時
└────────────────────────────────────────────────┘

✅ 成功：
┌────────────────────────────────────────────────┐
│ Cell 1: Code                        [⋮]        │ ← 綠色邊框
│ [代碼...]                                      │
│ [▶️ Run] [+ 插入下方]    ✅ 完成 (0.45s)       │ ← 執行時間
└────────────────────────────────────────────────┘

❌ 錯誤：
┌────────────────────────────────────────────────┐
│ Cell 1: Code                        [⋮]        │ ← 紅色邊框
│ [代碼...]                                      │
│ [▶️ Run] [+ 插入下方]    ❌ 錯誤 (0.12s)       │
└────────────────────────────────────────────────┘

Output（錯誤訊息）：
┌────────────────────────────────────────────────┐
│ ❌ Error:                                      │
│ ┌────────────────────────────────────────────┐ │
│ │ Traceback (most recent call last):        │ │
│ │   File "<stdin>", line 1, in <module>     │ │
│ │ NameError: name 'df' is not defined       │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ 💡 建議：請先執行載入數據的 Cell               │
│                                                │
│ [複製錯誤] [查看文檔]                          │
└────────────────────────────────────────────────┘
```

### 2. 代碼編輯器功能

```
編輯器特性：
• 語法高亮（Python、JavaScript）
• 自動補全（Ctrl+Space）
• 代碼片段（Tab 展開）
• 行號顯示
• 括號匹配
• 多游標編輯（Ctrl+Click）
• 搜尋替換（Ctrl+F）

快捷鍵：
Ctrl+Enter      - 執行當前 Cell
Shift+Enter     - 執行當前 Cell 並跳到下一個
Alt+Enter       - 執行當前 Cell 並在下方插入新 Cell
Ctrl+/          - 註解/取消註解
Tab             - 縮排或自動補全
Shift+Tab       - 取消縮排
Ctrl+D          - 刪除當前行
Ctrl+Z          - 復原
Ctrl+Y          - 重做
```

### 3. Output 類型與渲染

#### 文字輸出
```
┌────────────────────────────────────────────────┐
│ Output 1:                                      │
│ ┌────────────────────────────────────────────┐ │
│ │ Total sales: 555                          │ │ ← 純文字
│ │ Average price: $1,431.80                  │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ [複製]                                         │
└────────────────────────────────────────────────┘
```

#### 表格輸出（DataFrame）
```
┌────────────────────────────────────────────────┐
│ Output 1:                      [展開] [下載CSV] │
│ ┌────────────────────────────────────────────┐ │
│ │      Date    Product  Quantity   Price    │ │ ← 可捲動表格
│ │ 0  2025-10-01  Widget      120   $1,234  │ │
│ │ 1  2025-10-02  Gadget       85   $2,345  │ │
│ │ 2  2025-10-03  Widget      150   $1,567  │ │
│ │ 3  2025-10-04  Doohickey    92   $890    │ │
│ │ 4  2025-10-05  Widget      108   $1,123  │ │
│ │ ...                                       │ │
│ │ 顯示 5 / 100 行                            │ │ ← 分頁
│ └────────────────────────────────────────────┘ │
│                                                │
│ [展開全部] [複製] [下載 CSV]                   │
└────────────────────────────────────────────────┘
```

#### 圖表輸出（Matplotlib）
```
┌────────────────────────────────────────────────┐
│ Output 1:                      [展開] [下載PNG] │
│ ┌────────────────────────────────────────────┐ │
│ │          Sales Trend                      │ │
│ │                                           │ │
│ │   150 ┤    ╭─╮                            │ │ ← 圖表渲染
│ │       │   ╭╯ ╰╮                           │ │
│ │   100 ┤  ╭╯   ╰╮                          │ │
│ │       │ ╭╯     ╰╮                         │ │
│ │    50 ┼─╯       ╰─                        │ │
│ │       └┬────┬────┬────┬────┬──            │ │
│ │        10/01 10/02 10/03 10/04 10/05      │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ [放大] [下載 PNG] [下載 SVG]                   │
└────────────────────────────────────────────────┘

點擊「放大」：
┌──────────────────────────────────────────────┐
│ 📈 Sales Trend                         [✕]   │
├──────────────────────────────────────────────┤
│                                              │
│         [完整大小的圖表顯示]                 │
│                                              │
│                                              │
│                                              │
│                                              │
├──────────────────────────────────────────────┤
│ [🔍 放大] [🔍 縮小] [⬇️ 下載]        [關閉]   │
└──────────────────────────────────────────────┘
```

#### 錯誤輸出
```
┌────────────────────────────────────────────────┐
│ ❌ Error:                                      │
│ ┌────────────────────────────────────────────┐ │
│ │ Traceback (most recent call last):        │ │ ← Traceback
│ │   File "<stdin>", line 5, in <module>     │ │
│ │   File "pandas/core/frame.py", line 123   │ │
│ │ KeyError: 'Sales'                         │ │ ← 錯誤類型
│ └────────────────────────────────────────────┘ │
│                                                │
│ 💡 建議：                                      │
│ • 檢查欄位名稱是否正確（可能是 'sales'）       │
│ • 使用 df.columns 查看所有欄位                │
│                                                │
│ [複製錯誤] [查看 Pandas 文檔] [詢問 AI]        │
└────────────────────────────────────────────────┘
```

### 4. 文件操作

#### 上傳文件
```
點擊「📤 上傳」：
┌────────────────────────────────────┐
│ 📤 上傳文件到 workspace            │
│                                    │
│ ┌────────────────────────────────┐│
│ │                                ││
│ │   📁 拖曳文件到此處             ││
│ │      或點擊選擇文件             ││
│ │                                ││
│ │   支援格式：CSV, JSON, TXT,    ││
│ │   Excel, PNG, JPG, PDF...      ││
│ │   最大單檔：100 MB             ││
│ │                                ││
│ └────────────────────────────────┘│
│                                    │
│ [ 選擇文件 ]        [ 取消 ]       │
└────────────────────────────────────┘

上傳中：
┌────────────────────────────────────┐
│ ⬆️ 上傳中...                       │
│                                    │
│ ✅ sales_data.csv                  │
│    ████████████████████ 100%       │
│    2.4 MB / 2.4 MB                 │
│                                    │
│ [ 完成 ]                           │
└────────────────────────────────────┘
```

#### 文件右鍵選單
```
右鍵點擊文件：
┌────────────────────────────┐
│ 📄 sales_data.csv          │
├────────────────────────────┤
│ 📋 複製路徑                │
│ ✏️ 重命名                  │
│ 👁️ 預覽                    │
│ ⬇️ 下載                    │
│ 🗑️ 刪除                    │
└────────────────────────────┘
```

### 5. 套件管理

```
點擊「管理套件」：
┌──────────────────────────────────────────────┐
│ 📦 套件管理                            [✕]   │
├──────────────────────────────────────────────┤
│                                              │
│ 🔍 搜尋套件...                               │
│                                              │
│ ┌──────────────────────────────────────────┐│
│ │ 📦 已安裝 (45)          [🔄 更新全部]     ││
│ │                                          ││
│ │ ┌──────────────────────────────────────┐││
│ │ │ numpy          1.24.3      [⬆️] [🗑️] │││ ← 已安裝套件
│ │ │ pandas         2.0.2       [⬆️] [🗑️] │││
│ │ │ matplotlib     3.7.1       [⬆️] [🗑️] │││
│ │ │ scikit-learn   1.2.2       [⬆️] [🗑️] │││
│ │ │ ...                                  │││
│ │ └──────────────────────────────────────┘││
│ └──────────────────────────────────────────┘│
│                                              │
│ ┌──────────────────────────────────────────┐│
│ │ 🔍 搜尋結果                              ││
│ │                                          ││
│ │ seaborn        0.12.2       [➕ 安裝]    ││ ← 可安裝套件
│ │ plotly         5.14.1       [➕ 安裝]    ││
│ │ tensorflow     2.12.0       [➕ 安裝]    ││
│ │ ...                                      ││
│ └──────────────────────────────────────────┘│
│                                              │
│ 💡 提示：安裝套件需要重啟 Kernel            │
│                                              │
│ [ 關閉 ]                                     │
└──────────────────────────────────────────────┘

安裝套件：
┌──────────────────────────────────────────────┐
│ ⏳ 正在安裝 seaborn...                       │
│                                              │
│ ████████████████░░░░ 75%                     │
│                                              │
│ 📋 日誌：                                    │
│ Collecting seaborn==0.12.2                   │
│ Downloading seaborn-0.12.2-py3-none-any.whl  │
│ Installing...                                │
│                                              │
│ [ 取消 ]                                     │
└──────────────────────────────────────────────┘

安裝完成：
┌──────────────────────────────────────────────┐
│ ✅ seaborn 安裝成功                          │
│                                              │
│ 是否立即重啟 Kernel？                        │
│ (重啟後才能使用新套件)                       │
│                                              │
│ [ 立即重啟 ]          [ 稍後重啟 ]           │
└──────────────────────────────────────────────┘
```

### 6. Kernel 管理

```
點擊「🔄 Restart Kernel」：
┌────────────────────────────────────┐
│ ⚠️ 確認重啟 Kernel？               │
│                                    │
│ 重啟 Kernel 會：                   │
│ • 清空所有變數                     │
│ • 中斷當前執行                     │
│ • 釋放所有記憶體                   │
│                                    │
│ 是否繼續？                         │
│                                    │
│ [ 重啟 ]            [ 取消 ]       │
└────────────────────────────────────┘

重啟中：
┌────────────────────────────────────┐
│ 🔄 正在重啟 Kernel...              │
│                                    │
│ ⏳ 請稍候...                       │
│                                    │
└────────────────────────────────────┘

重啟完成：
┌────────────────────────────────────┐
│ ✅ Kernel 已重啟                   │
│                                    │
│ 🐍 Python 3.11.5                   │
│ 🟢 Kernel 運行中                   │
│                                    │
│ [ 確定 ]                           │
└────────────────────────────────────┘
```

### 7. Markdown Cell

```
┌────────────────────────────────────────────────┐
│ Cell 2: Markdown                    [⋮]        │
│ ┌────────────────────────────────────────────┐ │
│ │ # Data Analysis Report                    │ │ ← Markdown 編輯
│ │                                           │ │
│ │ ## Overview                               │ │
│ │ This notebook analyzes sales data...      │ │
│ │                                           │ │
│ │ ## Key Findings                           │ │
│ │ - Total sales: **555 units**              │ │
│ │ - Average price: **$1,431.80**            │ │
│ │▋                                          │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ [👁️ 預覽] [✏️ 編輯]                            │
└────────────────────────────────────────────────┘

預覽模式：
┌────────────────────────────────────────────────┐
│ Cell 2: Markdown                    [⋮]        │
│ ┌────────────────────────────────────────────┐ │
│ │ # Data Analysis Report                    │ │ ← Markdown 渲染
│ │                                           │ │
│ │ ## Overview                               │ │
│ │ This notebook analyzes sales data...      │ │
│ │                                           │ │
│ │ ## Key Findings                           │ │
│ │ • Total sales: 555 units                  │ │
│ │ • Average price: $1,431.80                │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ [👁️ 預覽] [✏️ 編輯]                            │
└────────────────────────────────────────────────┘
```

---

## ♿ 無障礙設計

### 鍵盤導航

```yaml
全局快捷鍵:
  Ctrl+S: 儲存 Notebook
  Ctrl+Enter: 執行當前 Cell
  Shift+Enter: 執行當前 Cell 並跳到下一個
  Alt+Enter: 執行當前 Cell 並在下方插入新 Cell
  Esc: 退出編輯模式（Command 模式）
  Enter: 進入編輯模式

Command 模式（Esc 後）:
  A: 在上方插入 Cell
  B: 在下方插入 Cell
  D+D: 刪除當前 Cell
  M: 轉換為 Markdown Cell
  Y: 轉換為 Code Cell
  Arrow Up/Down: 選擇上/下一個 Cell
  Shift+Arrow Up/Down: 擴展選擇

編輯模式:
  Ctrl+/: 註解/取消註解
  Tab: 縮排或自動補全
  Shift+Tab: 取消縮排或顯示函數幫助
  Ctrl+D: 刪除當前行
  Ctrl+Z: 復原
  Ctrl+Y: 重做
  Ctrl+F: 搜尋
  Ctrl+H: 替換
```

### ARIA 標籤

```html
<!-- Notebook 主容器 -->
<main aria-label="Code Interpreter Notebook" role="main">
  <!-- Toolbar -->
  <div role="toolbar" aria-label="Notebook 控制工具列">
    <button aria-label="執行所有 Cells">▶️ Run All</button>
    <button aria-label="停止執行">⏹️ Stop</button>
    <button aria-label="重啟 Kernel">🔄 Restart Kernel</button>
    <button aria-label="新增代碼 Cell">+ Code</button>
    <button aria-label="新增 Markdown Cell">+ Markdown</button>
    <button aria-label="儲存 Notebook">💾 Save</button>
  </div>

  <!-- Cell 列表 -->
  <div
    role="region"
    aria-label="Notebook Cells"
    tabindex="0"
  >
    <!-- Code Cell -->
    <article
      role="article"
      aria-labelledby="cell-1-label"
      aria-describedby="cell-1-status"
      data-cell-type="code"
      data-cell-status="completed"
    >
      <h3 id="cell-1-label" class="sr-only">Cell 1: Code</h3>

      <div role="toolbar" aria-label="Cell 操作">
        <button aria-label="執行此 Cell">▶️ Run</button>
        <button aria-label="Cell 選單" aria-haspopup="menu">⋮</button>
      </div>

      <!-- Monaco Editor -->
      <div
        role="textbox"
        aria-label="代碼編輯器"
        aria-multiline="true"
        contenteditable="true"
        spellcheck="false"
      >
        <!-- Monaco Editor content -->
      </div>

      <div
        id="cell-1-status"
        role="status"
        aria-live="polite"
        aria-atomic="true"
      >
        ✅ 完成 (0.45s)
      </div>
    </article>

    <!-- Output Cell -->
    <section
      role="region"
      aria-labelledby="output-1-label"
      aria-describedby="output-1-type"
    >
      <h4 id="output-1-label" class="sr-only">Output 1</h4>
      <p id="output-1-type" class="sr-only">表格輸出</p>

      <div role="table" aria-label="數據表格">
        <!-- Table content -->
      </div>

      <div role="toolbar" aria-label="輸出操作">
        <button aria-label="展開輸出">展開</button>
        <button aria-label="複製輸出">複製</button>
        <button aria-label="下載輸出">下載</button>
      </div>
    </section>
  </div>
</main>

<!-- 側邊面板 -->
<aside aria-label="環境與資源資訊" role="complementary">
  <!-- 環境資訊 -->
  <section aria-labelledby="env-heading">
    <h2 id="env-heading">環境資訊</h2>
    <dl>
      <dt>Runtime</dt>
      <dd>Python 3.11.5</dd>
      <dt>Kernel 狀態</dt>
      <dd>
        <span role="status" aria-live="polite">🟢 運行中</span>
      </dd>
    </dl>
  </section>

  <!-- 資源使用 -->
  <section aria-labelledby="resources-heading">
    <h2 id="resources-heading">資源使用</h2>

    <div role="group" aria-labelledby="cpu-label">
      <h3 id="cpu-label">CPU</h3>
      <div
        role="progressbar"
        aria-valuenow="25"
        aria-valuemin="0"
        aria-valuemax="100"
        aria-label="CPU 使用率 25%"
      >
        25%
      </div>
    </div>

    <div role="group" aria-labelledby="memory-label">
      <h3 id="memory-label">記憶體</h3>
      <div
        role="progressbar"
        aria-valuenow="34"
        aria-valuemin="0"
        aria-valuemax="100"
        aria-label="記憶體使用率 34%，512 MB / 1.5 GB"
      >
        34% (512 MB / 1.5 GB)
      </div>
    </div>
  </section>

  <!-- 文件瀏覽器 -->
  <section aria-labelledby="files-heading">
    <h2 id="files-heading">文件瀏覽器</h2>

    <nav aria-label="文件列表">
      <ul role="tree">
        <li role="treeitem" aria-expanded="true">
          <button aria-label="資料夾 workspace">📁 workspace/</button>
          <ul role="group">
            <li role="treeitem">
              <button aria-label="文件 sales_data.csv">📄 sales_data.csv</button>
            </li>
            <li role="treeitem">
              <button aria-label="文件 analysis.py">📄 analysis.py</button>
            </li>
          </ul>
        </li>
      </ul>
    </nav>
  </section>

  <!-- 變數檢視器 -->
  <section aria-labelledby="variables-heading">
    <h2 id="variables-heading">變數檢視器</h2>

    <table role="table" aria-label="當前變數列表">
      <thead>
        <tr>
          <th>變數名</th>
          <th>類型</th>
          <th>值</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>df</td>
          <td>DataFrame</td>
          <td>
            <button aria-label="展開 df 詳情">(5, 4)</button>
          </td>
        </tr>
        <tr>
          <td>total</td>
          <td>int</td>
          <td>555</td>
        </tr>
      </tbody>
    </table>
  </section>
</aside>
```

### 螢幕閱讀器優化

```yaml
執行狀態通知:
  實現: aria-live="polite"
  行為:
    - 執行開始：朗讀"Cell 1 開始執行"
    - 執行完成：朗讀"Cell 1 執行完成，用時 0.45 秒"
    - 執行錯誤：朗讀"Cell 1 執行錯誤：NameError"

焦點管理:
  執行 Cell 後:
    - 焦點移至 Output 區域（如有輸出）
    - 或焦點保持在 Cell 上（無輸出）

  新增 Cell 後:
    - 焦點移至新 Cell 的編輯器
    - 朗讀"已新增代碼 Cell"

  刪除 Cell 後:
    - 焦點移至上一個 Cell
    - 或焦點移至下一個 Cell（如無上一個）

錯誤處理:
  執行失敗:
    - 朗讀完整錯誤訊息
    - 焦點移至錯誤 Cell
    - 提供修復建議

  Kernel 崩潰:
    - 朗讀"Kernel 已停止響應"
    - 焦點移至重啟按鈕
```

---

## ⚡ 性能優化

### 1. Cell 虛擬化

```typescript
interface CellVirtualization {
  // 虛擬滾動
  virtualScroll: {
    enabled: true,
    itemHeight: 150,       // 平均 Cell 高度
    bufferSize: 3,         // 上下各渲染 3 個額外 Cell
    totalCells: 100,       // 總 Cell 數
    visibleCells: 10,      // 可視區域 10 個
    renderedCells: 16      // 實際渲染 16 個（10 + buffer）
  },

  // 性能收益
  memoryUsage: '~5MB',     // vs. 50MB 全量渲染
  initialRender: '< 200ms', // vs. 2000ms
  scrollFPS: '60fps'       // 流暢滾動
}
```

### 2. 代碼執行優化

```typescript
interface ExecutionOptimization {
  // WebSocket 連接
  connection: {
    protocol: 'WebSocket',
    keepAlive: true,
    reconnect: 'automatic',
    timeout: 300000        // 5 分鐘超時
  },

  // 輸出 Streaming
  outputStreaming: {
    enabled: true,
    chunkSize: 1024,       // 1KB chunks
    updateInterval: 100,   // 100ms 更新一次
    maxBufferSize: '10MB'  // 最大輸出緩衝
  },

  // 執行佇列
  executionQueue: {
    maxConcurrent: 1,      // 同時只執行 1 個 Cell
    queueSize: 10,         // 佇列大小
    priority: 'fifo'       // First-In-First-Out
  },

  // 性能指標
  executionLatency: '< 2s',   // 簡單代碼執行延遲
  outputLatency: '< 100ms'    // 輸出顯示延遲
}
```

### 3. Monaco Editor 優化

```typescript
interface EditorOptimization {
  // 延遲載入
  lazyLoad: {
    enabled: true,
    loadOnFocus: true,     // 聚焦時才載入 Editor
    unloadOnBlur: false    // 失焦後不卸載
  },

  // 語法高亮
  syntaxHighlighting: {
    enabled: true,
    debounce: 300,         // 300ms 防抖
    maxFileSize: '1MB'     // 大於 1MB 禁用
  },

  // 自動補全
  autoComplete: {
    enabled: true,
    debounce: 200,         // 200ms 防抖
    maxSuggestions: 50,    // 最多 50 個建議
    caching: true          // 快取建議
  },

  // 性能收益
  loadTime: '< 100ms',     // Editor 載入時間
  typingLatency: '< 16ms'  // 打字延遲（60fps）
}
```

### 4. 圖表渲染優化

```typescript
interface ChartOptimization {
  // Canvas vs SVG
  renderMode: 'canvas',    // 大數據量使用 Canvas
  svgThreshold: 1000,      // < 1000 點使用 SVG

  // 數據採樣
  dataSampling: {
    enabled: true,
    threshold: 10000,      // > 10000 點時採樣
    algorithm: 'lttb',     // Largest-Triangle-Three-Buckets
    targetPoints: 1000     // 採樣到 1000 點
  },

  // WebGL 加速
  webGL: {
    enabled: true,
    fallback: 'canvas',    // WebGL 不支援時降級
    maxPoints: 100000      // WebGL 最多渲染點數
  },

  // 性能收益
  renderTime: '< 500ms',   // 圖表渲染時間
  interactionFPS: '60fps'  // 互動流暢度
}
```

### 5. 資源監控優化

```typescript
interface ResourceMonitoring {
  // 監控間隔
  updateInterval: {
    cpu: 1000,             // 1 秒更新一次 CPU
    memory: 2000,          // 2 秒更新一次記憶體
    disk: 5000             // 5 秒更新一次磁碟
  },

  // 批次更新
  batchUpdate: {
    enabled: true,
    batchSize: 3,          // 批次更新 3 個指標
    debounce: 500          // 500ms 防抖
  },

  // 性能收益
  networkCalls: '-70%',    // 減少 70% 網路請求
  cpuUsage: '-50%'         // 減少 50% CPU 使用
}
```

---

## 🔧 技術實現建議

### 1. 狀態管理（Zustand）

```typescript
// stores/notebookStore.ts
import create from 'zustand';

type CellType = 'code' | 'markdown';
type CellStatus = 'idle' | 'running' | 'completed' | 'error';

interface Cell {
  id: string;
  type: CellType;
  content: string;
  output?: Output;
  executionTime?: number;
  executionCount?: number;
  status: CellStatus;
  error?: string;
}

interface Output {
  type: 'text' | 'table' | 'chart' | 'image' | 'error';
  data: any;
  metadata?: {
    mimeType?: string;
    dimensions?: { width: number; height: number };
    rows?: number;
    columns?: number;
  };
}

interface Notebook {
  id: string;
  name: string;
  cells: Cell[];
  kernel: {
    name: string;
    version: string;
    status: 'idle' | 'busy' | 'dead';
  };
  metadata: {
    language: string;
    createdAt: Date;
    updatedAt: Date;
    lastExecuted?: Date;
  };
}

interface NotebookStore {
  // 狀態
  notebooks: Notebook[];
  activeNotebookId: string | null;
  activeCellId: string | null;
  isExecuting: boolean;
  executionQueue: string[];

  // Notebook 操作
  createNotebook: (name: string) => void;
  loadNotebook: (id: string) => Promise<void>;
  saveNotebook: (id: string) => Promise<void>;
  deleteNotebook: (id: string) => void;

  // Cell 操作
  addCell: (notebookId: string, type: CellType, index?: number) => void;
  updateCell: (notebookId: string, cellId: string, content: string) => void;
  deleteCell: (notebookId: string, cellId: string) => void;
  moveCell: (notebookId: string, cellId: string, direction: 'up' | 'down') => void;

  // 執行操作
  executeCell: (notebookId: string, cellId: string) => Promise<void>;
  executeCells: (notebookId: string, cellIds: string[]) => Promise<void>;
  executeAll: (notebookId: string) => Promise<void>;
  stopExecution: () => void;

  // Kernel 操作
  restartKernel: (notebookId: string) => Promise<void>;
  interruptKernel: (notebookId: string) => Promise<void>;
}

const useNotebookStore = create<NotebookStore>((set, get) => ({
  notebooks: [],
  activeNotebookId: null,
  activeCellId: null,
  isExecuting: false,
  executionQueue: [],

  createNotebook: (name) => {
    const newNotebook: Notebook = {
      id: generateId(),
      name,
      cells: [
        {
          id: generateId(),
          type: 'code',
          content: '',
          status: 'idle'
        }
      ],
      kernel: {
        name: 'python3',
        version: '3.11.5',
        status: 'idle'
      },
      metadata: {
        language: 'python',
        createdAt: new Date(),
        updatedAt: new Date()
      }
    };

    set((state) => ({
      notebooks: [...state.notebooks, newNotebook],
      activeNotebookId: newNotebook.id
    }));
  },

  addCell: (notebookId, type, index) => {
    const newCell: Cell = {
      id: generateId(),
      type,
      content: '',
      status: 'idle'
    };

    set((state) => ({
      notebooks: state.notebooks.map(nb =>
        nb.id === notebookId
          ? {
              ...nb,
              cells: index !== undefined
                ? [
                    ...nb.cells.slice(0, index + 1),
                    newCell,
                    ...nb.cells.slice(index + 1)
                  ]
                : [...nb.cells, newCell],
              metadata: {
                ...nb.metadata,
                updatedAt: new Date()
              }
            }
          : nb
      ),
      activeCellId: newCell.id
    }));
  },

  executeCell: async (notebookId, cellId) => {
    const notebook = get().notebooks.find(nb => nb.id === notebookId);
    if (!notebook) return;

    const cell = notebook.cells.find(c => c.id === cellId);
    if (!cell || cell.type !== 'code') return;

    // 更新狀態為執行中
    set((state) => ({
      notebooks: state.notebooks.map(nb =>
        nb.id === notebookId
          ? {
              ...nb,
              cells: nb.cells.map(c =>
                c.id === cellId
                  ? { ...c, status: 'running' as const }
                  : c
              ),
              kernel: { ...nb.kernel, status: 'busy' as const }
            }
          : nb
      ),
      isExecuting: true
    }));

    try {
      const startTime = performance.now();

      // 執行代碼（WebSocket 連接到 Kernel）
      const result = await executeCode({
        notebookId,
        cellId,
        code: cell.content,
        onOutput: (output) => {
          // Streaming 輸出更新
          set((state) => ({
            notebooks: state.notebooks.map(nb =>
              nb.id === notebookId
                ? {
                    ...nb,
                    cells: nb.cells.map(c =>
                      c.id === cellId
                        ? { ...c, output }
                        : c
                    )
                  }
                : nb
            )
          }));
        }
      });

      const executionTime = performance.now() - startTime;

      // 更新狀態為完成
      set((state) => ({
        notebooks: state.notebooks.map(nb =>
          nb.id === notebookId
            ? {
                ...nb,
                cells: nb.cells.map(c =>
                  c.id === cellId
                    ? {
                        ...c,
                        status: 'completed' as const,
                        output: result.output,
                        executionTime,
                        executionCount: (c.executionCount || 0) + 1
                      }
                    : c
                ),
                kernel: { ...nb.kernel, status: 'idle' as const },
                metadata: {
                  ...nb.metadata,
                  lastExecuted: new Date()
                }
              }
            : nb
        ),
        isExecuting: false
      }));

    } catch (error) {
      // 更新狀態為錯誤
      set((state) => ({
        notebooks: state.notebooks.map(nb =>
          nb.id === notebookId
            ? {
                ...nb,
                cells: nb.cells.map(c =>
                  c.id === cellId
                    ? {
                        ...c,
                        status: 'error' as const,
                        error: error.message,
                        output: {
                          type: 'error' as const,
                          data: {
                            traceback: error.traceback,
                            message: error.message
                          }
                        }
                      }
                    : c
                ),
                kernel: { ...nb.kernel, status: 'idle' as const }
              }
            : nb
        ),
        isExecuting: false
      }));
    }
  },

  restartKernel: async (notebookId) => {
    set((state) => ({
      notebooks: state.notebooks.map(nb =>
        nb.id === notebookId
          ? {
              ...nb,
              kernel: { ...nb.kernel, status: 'dead' as const },
              cells: nb.cells.map(c => ({
                ...c,
                status: 'idle' as const,
                executionCount: undefined
              }))
            }
          : nb
      )
    }));

    await api.restartKernel(notebookId);

    set((state) => ({
      notebooks: state.notebooks.map(nb =>
        nb.id === notebookId
          ? {
              ...nb,
              kernel: { ...nb.kernel, status: 'idle' as const }
            }
          : nb
      )
    }));
  }
}));
```

### 2. WebSocket 代碼執行

```typescript
// services/kernelService.ts
interface ExecuteOptions {
  notebookId: string;
  cellId: string;
  code: string;
  onOutput?: (output: Output) => void;
  onError?: (error: Error) => void;
}

class KernelService {
  private ws: WebSocket | null = null;
  private messageHandlers: Map<string, (data: any) => void> = new Map();

  async connect(notebookId: string): Promise<void> {
    this.ws = new WebSocket(`ws://localhost:8888/kernel/${notebookId}`);

    this.ws.onmessage = (event) => {
      const message = JSON.parse(event.data);
      const handler = this.messageHandlers.get(message.msgId);

      if (handler) {
        handler(message);
      }
    };

    return new Promise((resolve, reject) => {
      this.ws!.onopen = () => resolve();
      this.ws!.onerror = (error) => reject(error);
    });
  }

  async executeCode(options: ExecuteOptions): Promise<{ output: Output }> {
    const msgId = generateId();

    return new Promise((resolve, reject) => {
      const outputs: any[] = [];

      // 註冊訊息處理器
      this.messageHandlers.set(msgId, (message) => {
        switch (message.msgType) {
          case 'stream':
            // Streaming 輸出（print）
            const streamOutput: Output = {
              type: 'text',
              data: message.content.text
            };
            outputs.push(streamOutput);
            options.onOutput?.(streamOutput);
            break;

          case 'display_data':
            // DataFrame、圖表等
            const displayOutput: Output = this.parseDisplayData(message.content);
            outputs.push(displayOutput);
            options.onOutput?.(displayOutput);
            break;

          case 'execute_result':
            // 執行結果
            const resultOutput: Output = this.parseExecuteResult(message.content);
            outputs.push(resultOutput);
            options.onOutput?.(resultOutput);
            break;

          case 'error':
            // 執行錯誤
            const error = new Error(message.content.ename);
            error.traceback = message.content.traceback;
            options.onError?.(error);
            reject(error);
            break;

          case 'execute_reply':
            // 執行完成
            this.messageHandlers.delete(msgId);

            // 合併所有輸出
            const finalOutput: Output = this.mergeOutputs(outputs);
            resolve({ output: finalOutput });
            break;
        }
      });

      // 發送執行請求
      this.ws!.send(JSON.stringify({
        msgId,
        msgType: 'execute_request',
        content: {
          code: options.code,
          silent: false,
          storeHistory: true
        }
      }));
    });
  }

  private parseDisplayData(content: any): Output {
    // DataFrame
    if (content['text/html']?.includes('<table>')) {
      return {
        type: 'table',
        data: this.parseHTMLTable(content['text/html']),
        metadata: {
          mimeType: 'text/html'
        }
      };
    }

    // Matplotlib/Plotly 圖表
    if (content['image/png']) {
      return {
        type: 'image',
        data: content['image/png'],
        metadata: {
          mimeType: 'image/png'
        }
      };
    }

    // 純文字
    return {
      type: 'text',
      data: content['text/plain']
    };
  }

  private parseExecuteResult(content: any): Output {
    // 返回值
    return {
      type: 'text',
      data: content['text/plain']
    };
  }

  private mergeOutputs(outputs: Output[]): Output {
    if (outputs.length === 0) {
      return { type: 'text', data: '' };
    }

    if (outputs.length === 1) {
      return outputs[0];
    }

    // 多個輸出：組合為單一輸出
    return {
      type: 'text',
      data: outputs.map(o => o.data).join('\n')
    };
  }

  async restartKernel(notebookId: string): Promise<void> {
    await api.post(`/kernels/${notebookId}/restart`);

    // 重新連接
    this.ws?.close();
    await this.connect(notebookId);
  }

  disconnect(): void {
    this.ws?.close();
    this.ws = null;
    this.messageHandlers.clear();
  }
}

const kernelService = new KernelService();
export default kernelService;
```

---

## 📐 設計決策記錄（ADR）

### ADR-001: Jupyter-like vs Custom UI

**決策**: 採用類似 Jupyter Notebook 的 UI 設計

**理由**:
1. **熟悉度**: 數據科學家和開發者已熟悉 Jupyter UI
2. **學習曲線**: 降低學習成本，快速上手
3. **功能完整**: Jupyter 模型經過驗證，支援所有核心功能
4. **社區支援**: 大量參考資料和最佳實踐

**權衡**:
- ✅ 優點: 熟悉、成熟、功能完整
- ❌ 缺點: 缺乏創新、UI 較傳統

**替代方案**:
- Google Colab 風格: 更現代但學習曲線略高
- Observable 風格: 創新但與主流不同

---

### ADR-002: WebSocket vs HTTP Polling

**決策**: 使用 WebSocket 進行代碼執行和輸出 Streaming

**理由**:
1. **即時性**: 輸出即時顯示，無延遲
2. **效率**: 減少 HTTP 請求開銷
3. **雙向通信**: 支援 Kernel 中斷和即時控制
4. **行業標準**: Jupyter、VS Code 均使用 WebSocket

**權衡**:
- ✅ 優點: 即時、高效、互動性強
- ❌ 缺點: 實現複雜度略高

**實現**:
```typescript
WebSocket 訊息類型:
- execute_request: 執行代碼
- execute_reply: 執行完成
- stream: print 輸出
- display_data: DataFrame、圖表
- error: 執行錯誤
- kernel_info: Kernel 資訊
```

---

### ADR-003: Monaco Editor vs CodeMirror

**決策**: 使用 Monaco Editor 作為代碼編輯器

**理由**:
1. **功能強大**: VS Code 同款，功能完整
2. **智能提示**: 內建 IntelliSense，支援語法檢查
3. **性能優秀**: 處理大文件表現優異
4. **TypeScript 支援**: 官方 TypeScript 支援

**權衡**:
- ✅ 優點: 功能強、性能好、體驗佳
- ❌ 缺點: Bundle 體積較大（~5MB）

**優化策略**:
```typescript
// 延遲載入 Monaco
import('monaco-editor').then(monaco => {
  // 初始化 Editor
});

// Code Splitting
const MonacoEditor = lazy(() => import('./MonacoEditor'));
```

---

### ADR-004: Cell 輸出格式

**決策**: 支援多種輸出格式（文字、表格、圖表、圖片）

**理由**:
1. **靈活性**: 不同數據類型適合不同展示方式
2. **可視化**: 圖表和表格提升數據理解
3. **互動性**: 可展開/收起、下載輸出
4. **標準支援**: 與 Jupyter Output 格式兼容

**支援格式**:
```typescript
OutputType:
- text/plain: 純文字
- text/html: HTML 表格
- image/png: PNG 圖片
- image/svg: SVG 向量圖
- application/json: JSON 數據
- application/vnd.plotly.v1+json: Plotly 互動圖表
```

---

## ✅ 驗收標準

### 功能完整性
- [ ] Notebook CRUD 操作
- [ ] Cell 增刪改查與移動
- [ ] 代碼執行（Python、JavaScript）
- [ ] 輸出 Streaming 即時顯示
- [ ] 多種輸出格式支援（文字、表格、圖表、圖片）
- [ ] Markdown Cell 支援
- [ ] Kernel 管理（啟動、重啟、中斷）
- [ ] 變數檢視器
- [ ] 文件瀏覽器（上傳、下載、管理）
- [ ] 套件管理（安裝、更新、刪除）
- [ ] 執行歷史記錄
- [ ] 資源監控（CPU、記憶體、磁碟）

### 性能指標
- [ ] 代碼執行成功率 > 99%
- [ ] 執行響應時間 < 2s（簡單代碼）
- [ ] 圖表渲染時間 < 500ms
- [ ] Notebook 載入時間 < 1s
- [ ] 支援 >100 個 Cells 的 Notebook

### 無障礙合規
- [ ] WCAG 2.1 AA 合規
- [ ] 完整鍵盤導航支援（Command 模式 + 編輯模式）
- [ ] 螢幕閱讀器優化
- [ ] ARIA 標籤完整
- [ ] 執行狀態即時通知

### 響應式設計
- [ ] Desktop (≥1280px) 2-Column 佈局
- [ ] Tablet (768-1279px) 可折疊側邊欄
- [ ] Mobile (<768px) 全屏佈局
- [ ] 所有斷點下功能完整可用

---

**文檔版本**: 1.0.0
**最後更新**: 2025-10-29
**審核狀態**: ✅ 已完成
**下一步**: 開始創建 Text-to-SQL Wireframe (08-text-to-sql.md)
