# 字體系統

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成

---

## 概覽

本字體系統基於 **模組化尺度（Modular Scale）** 設計，確保字體大小的和諧與可讀性。

**設計原則**:
- 📖 **可讀性**: 優先考慮長時間閱讀的舒適度
- 📏 **層次清晰**: 明確的字級區分視覺層次
- ⚖️ **平衡**: 字重和大小的合理搭配
- ♿ **可訪問性**: 支持字體縮放和高對比度模式

---

## 字體家族

### 主要字體（Sans-serif）

**西文字體**:
```
font-family: 'Inter', 'Segoe UI', -apple-system, BlinkMacSystemFont,
             'Roboto', 'Helvetica Neue', Arial, sans-serif;
```

**中文字體**:
```
font-family: 'Noto Sans TC', 'Microsoft JhengHei', 'PingFang TC',
             '微軟正黑體', 'Apple LiGothic', sans-serif;
```

**組合字體（推薦）**:
```
font-family: 'Inter', 'Noto Sans TC', 'Segoe UI', 'Microsoft JhengHei',
             -apple-system, BlinkMacSystemFont, sans-serif;
```

### 等寬字體（Monospace）

**用途**: 代碼塊、JSON、SQL 查詢、終端輸出

```
font-family: 'JetBrains Mono', 'Fira Code', 'Consolas', 'Monaco',
             'Courier New', monospace;
```

---

## 字級系統（Type Scale）

### 標題層級（Headings）

| 層級 | 尺寸 | 行高 | 字重 | 使用場景 |
|------|------|------|------|---------|
| H1 | 32px / 2rem | 40px / 1.25 | 600 (SemiBold) | 頁面主標題 |
| H2 | 24px / 1.5rem | 32px / 1.33 | 600 (SemiBold) | 章節標題 |
| H3 | 20px / 1.25rem | 28px / 1.4 | 600 (SemiBold) | 子章節標題 |
| H4 | 16px / 1rem | 24px / 1.5 | 600 (SemiBold) | 小節標題 |
| H5 | 14px / 0.875rem | 20px / 1.43 | 600 (SemiBold) | 強調標題 |

### 正文層級（Body Text）

| 層級 | 尺寸 | 行高 | 字重 | 使用場景 |
|------|------|------|------|---------|
| Large | 16px / 1rem | 24px / 1.5 | 400 (Regular) | 重要正文、介紹段落 |
| Body | 14px / 0.875rem | 20px / 1.43 | 400 (Regular) | 標準正文 |
| Small | 12px / 0.75rem | 16px / 1.33 | 400 (Regular) | 輔助信息、說明文字 |
| Caption | 11px / 0.6875rem | 14px / 1.27 | 400 (Regular) | 圖表標註、極小文字 |

### 特殊層級

| 層級 | 尺寸 | 行高 | 字重 | 使用場景 |
|------|------|------|------|---------|
| Display | 48px / 3rem | 56px / 1.17 | 700 (Bold) | 著陸頁、營銷頁 |
| Code | 13px / 0.8125rem | 20px / 1.54 | 400 (Regular) | 代碼塊 |
| Label | 12px / 0.75rem | 16px / 1.33 | 500 (Medium) | 表單標籤、按鈕文字 |

---

## 字重（Font Weights）

| 字重 | 數值 | 名稱 | 使用場景 |
|------|------|------|---------|
| Regular | 400 | Normal | 正文、描述性文字 |
| Medium | 500 | Medium | 標籤、按鈕、強調 |
| SemiBold | 600 | SemiBold | 標題、重要信息 |
| Bold | 700 | Bold | 特殊強調、Display |

**注意**: 避免使用過輕（<400）或過重（>700）的字重，以確保可讀性。

---

## 行高和間距

### 行高（Line Height）

| 文字類型 | 行高比例 | 說明 |
|---------|---------|------|
| 標題 | 1.2 - 1.4 | 較緊湊，增強視覺衝擊 |
| 正文 | 1.4 - 1.6 | 舒適閱讀，適合長文本 |
| 標籤/按鈕 | 1.0 - 1.2 | 緊湊，節省空間 |

### 段落間距

| 元素 | 間距 | 說明 |
|------|------|------|
| 標題後 | 16px | H2/H3 之後的間距 |
| 段落間 | 12px | 正文段落之間 |
| 列表項 | 8px | 列表項之間 |
| 組塊間 | 24px | 不同內容組塊之間 |

---

## 使用示例

### 頁面標題區域

```
┌────────────────────────────────────────┐
│ Dashboard                      [H1: 32px / SemiBold]
│ 歡迎回來，Alex Chen             [Body: 14px / Regular, Neutral-700]
└────────────────────────────────────────┘
```

### 卡片標題和內容

```
┌────────────────────────────────────────┐
│ 活躍 Agents                    [H3: 20px / SemiBold]
│
│ 當前有 8 個 Agent 正在運行      [Body: 14px / Regular]
│ 總共創建了 23 個 Agent          [Small: 12px / Regular, Neutral-700]
└────────────────────────────────────────┘
```

### 表單標籤和輸入

```
Agent 名稱 *                         [Label: 12px / Medium]
┌────────────────────────────────────────┐
│ My Customer Service Agent     [Body: 14px / Regular]
└────────────────────────────────────────┘
最多 50 個字符                        [Caption: 11px / Regular, Neutral-600]
```

### 按鈕文字

```
[ 創建 Agent ]                   [Label: 12px / Medium]
[ 取消 ]                         [Label: 12px / Medium]
```

### 代碼塊

````
```json                          [Code: 13px / Regular, Monospace]
{
  "agent_name": "Customer Service Bot",
  "model": "gpt-4",
  "temperature": 0.7
}
```
````

---

## 響應式字體

### 移動端調整（<768px）

| 桌面尺寸 | 移動尺寸 | 調整理由 |
|---------|---------|---------|
| H1: 32px | 24px | 減少標題尺寸以適應小屏幕 |
| H2: 24px | 20px | 保持層次但縮小 |
| Body: 14px | 16px | 提升可讀性（移動端適合稍大字體）|

### CSS 實現

```css
/* 桌面默認 */
h1 { font-size: 32px; }
body { font-size: 14px; }

/* 移動端適配 */
@media (max-width: 767px) {
  h1 { font-size: 24px; }
  body { font-size: 16px; }
}
```

---

## 可訪問性

### 最小字體尺寸

- ✅ **正文最小**: 14px（移動端 16px）
- ✅ **輔助文字最小**: 12px
- ❌ **避免**: <11px 的字體（除特殊情況）

### 對比度

| 文字類型 | 最小對比度 | 推薦顏色組合 |
|---------|-----------|-------------|
| 正文 | 4.5:1 | Neutral-900 on White |
| 大文字（≥18px） | 3:1 | Neutral-700 on White |
| 輔助文字 | 4.5:1 | Neutral-700 on White |

### 用戶字體縮放

- 支持瀏覽器字體縮放（使用 `rem` 而非 `px`）
- 測試 200% 縮放下的佈局
- 避免固定高度導致內容被裁剪

---

## 特殊場景

### 數據表格

| 元素 | 字級 | 字重 | 顏色 |
|------|------|------|------|
| 表頭 | 12px | 600 (SemiBold) | Neutral-800 |
| 單元格 | 13px | 400 (Regular) | Neutral-900 |
| 排序指示 | 11px | 400 (Regular) | Neutral-600 |

### 數字顯示（KPI）

| 元素 | 字級 | 字重 | 說明 |
|------|------|------|------|
| 主要數字 | 32px | 600 (SemiBold) | 醒目的 KPI 數字 |
| 單位 | 14px | 400 (Regular) | 數字單位（次、個、元）|
| 變化趨勢 | 12px | 500 (Medium) | +15% ↑ |

### 對話氣泡

| 元素 | 字級 | 行高 | 說明 |
|------|------|------|------|
| 用戶消息 | 14px | 20px (1.43) | 標準對話文字 |
| Agent 回覆 | 14px | 22px (1.57) | 略大行高提升可讀性 |
| 時間戳 | 11px | 14px | 消息時間 |

---

## CSS 變量定義

```css
:root {
  /* Font Families */
  --font-sans: 'Inter', 'Noto Sans TC', 'Segoe UI', 'Microsoft JhengHei',
               -apple-system, BlinkMacSystemFont, sans-serif;
  --font-mono: 'JetBrains Mono', 'Fira Code', 'Consolas', monospace;

  /* Font Sizes */
  --font-size-display: 3rem;      /* 48px */
  --font-size-h1: 2rem;           /* 32px */
  --font-size-h2: 1.5rem;         /* 24px */
  --font-size-h3: 1.25rem;        /* 20px */
  --font-size-h4: 1rem;           /* 16px */
  --font-size-h5: 0.875rem;       /* 14px */
  --font-size-large: 1rem;        /* 16px */
  --font-size-body: 0.875rem;     /* 14px */
  --font-size-small: 0.75rem;     /* 12px */
  --font-size-caption: 0.6875rem; /* 11px */
  --font-size-code: 0.8125rem;    /* 13px */

  /* Font Weights */
  --font-weight-regular: 400;
  --font-weight-medium: 500;
  --font-weight-semibold: 600;
  --font-weight-bold: 700;

  /* Line Heights */
  --line-height-tight: 1.2;
  --line-height-normal: 1.43;
  --line-height-relaxed: 1.6;
}

/* Mobile Adjustments */
@media (max-width: 767px) {
  :root {
    --font-size-h1: 1.5rem;      /* 24px */
    --font-size-h2: 1.25rem;     /* 20px */
    --font-size-body: 1rem;      /* 16px */
  }
}
```

---

## Tailwind CSS 配置

```javascript
// tailwind.config.js
module.exports = {
  theme: {
    fontSize: {
      'display': ['48px', { lineHeight: '56px', fontWeight: '700' }],
      'h1': ['32px', { lineHeight: '40px', fontWeight: '600' }],
      'h2': ['24px', { lineHeight: '32px', fontWeight: '600' }],
      'h3': ['20px', { lineHeight: '28px', fontWeight: '600' }],
      'h4': ['16px', { lineHeight: '24px', fontWeight: '600' }],
      'h5': ['14px', { lineHeight: '20px', fontWeight: '600' }],
      'large': ['16px', { lineHeight: '24px', fontWeight: '400' }],
      'body': ['14px', { lineHeight: '20px', fontWeight: '400' }],
      'small': ['12px', { lineHeight: '16px', fontWeight: '400' }],
      'caption': ['11px', { lineHeight: '14px', fontWeight: '400' }],
      'code': ['13px', { lineHeight: '20px', fontWeight: '400' }],
    },
    fontFamily: {
      'sans': ['Inter', 'Noto Sans TC', 'Segoe UI', 'Microsoft JhengHei', 'sans-serif'],
      'mono': ['JetBrains Mono', 'Fira Code', 'Consolas', 'monospace'],
    },
    fontWeight: {
      'regular': '400',
      'medium': '500',
      'semibold': '600',
      'bold': '700',
    }
  }
}
```

---

## 參考資料

- [Inter Font](https://rsms.me/inter/)
- [Noto Sans TC](https://fonts.google.com/noto/specimen/Noto+Sans+TC)
- [Material Design Typography](https://m3.material.io/styles/typography/overview)
- [Modular Scale Calculator](https://www.modularscale.com/)

---

**下一步**: 開始創建第一個 Wireframe - Dashboard
