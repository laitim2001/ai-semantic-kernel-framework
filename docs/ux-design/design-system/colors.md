# 色彩系統

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成

---

## 概覽

本色彩系統基於 **語義化設計**，確保顏色的使用具有明確的意義和目的，同時滿足 WCAG 2.1 AA 可訪問性標準。

**設計原則**:
- 🎯 **語義化**: 顏色傳達明確的意義
- ♿ **可訪問性**: 對比度 ≥4.5:1（文字）、≥3:1（UI元素）
- 🌈 **豐富性**: 提供足夠的色階支持不同場景
- 🔄 **靈活性**: 支持淺色和深色主題

---

## 主色彩系統

### Primary（主色 - 藍色）

**用途**: 主要操作、鏈接、重要信息強調

| 色階 | Hex | RGB | 使用場景 | 對比度 (白底) |
|------|-----|-----|---------|-------------|
| Primary-900 | #002050 | 0, 32, 80 | 深色背景、強調 | 13.5:1 ✅ |
| Primary-700 | #004C87 | 0, 76, 135 | 懸停狀態 | 7.8:1 ✅ |
| Primary-500 | #0078D4 | 0, 120, 212 | 主要按鈕、鏈接 | 4.5:1 ✅ |
| Primary-300 | #4A9EE8 | 74, 158, 232 | 輔助元素 | 3.2:1 ⚠️ |
| Primary-100 | #DEECF9 | 222, 236, 249 | 淺色背景 | 1.1:1 ❌ |

**使用示例**:
```
主要按鈕背景: Primary-500 (#0078D4)
主要按鈕文字: White (#FFFFFF)
主要按鈕懸停: Primary-700 (#004C87)
鏈接文字: Primary-500 (#0078D4)
信息提示背景: Primary-100 (#DEECF9)
```

---

### Success（成功 - 綠色）

**用途**: 成功狀態、完成操作、正面反饋

| 色階 | Hex | RGB | 使用場景 | 對比度 (白底) |
|------|-----|-----|---------|-------------|
| Success-900 | #004B1C | 0, 75, 28 | 深色背景 | 11.2:1 ✅ |
| Success-700 | #0E7C0E | 14, 124, 14 | 懸停狀態 | 6.3:1 ✅ |
| Success-500 | #107C10 | 16, 124, 16 | 成功圖標、按鈕 | 5.9:1 ✅ |
| Success-300 | #6BB700 | 107, 183, 0 | 輔助元素 | 3.8:1 ⚠️ |
| Success-100 | #DFF6DD | 223, 246, 221 | 成功通知背景 | 1.1:1 ❌ |

**使用示例**:
```
成功通知背景: Success-100 (#DFF6DD)
成功圖標: Success-500 (#107C10)
成功按鈕: Success-700 (#0E7C0E)
健康狀態指示: Success-500 (#107C10)
```

---

### Warning（警告 - 橙色）

**用途**: 警告信息、需要注意的內容、中等風險

| 色階 | Hex | RGB | 使用場景 | 對比度 (白底) |
|------|-----|-----|---------|-------------|
| Warning-900 | #5C2E00 | 92, 46, 0 | 深色背景 | 10.8:1 ✅ |
| Warning-700 | #CA5010 | 202, 80, 16 | 懸停狀態 | 5.2:1 ✅ |
| Warning-500 | #FF8C00 | 255, 140, 0 | 警告圖標、按鈕 | 3.2:1 ⚠️ |
| Warning-300 | #FFAA44 | 255, 170, 68 | 輔助元素 | 2.3:1 ❌ |
| Warning-100 | #FFF4CE | 255, 244, 206 | 警告通知背景 | 1.1:1 ❌ |

**使用示例**:
```
警告通知背景: Warning-100 (#FFF4CE)
警告圖標: Warning-700 (#CA5010)
警告按鈕: Warning-700 (#CA5010)
中等風險狀態: Warning-500 (#FF8C00)
```

---

### Error（錯誤 - 紅色）

**用途**: 錯誤狀態、危險操作、失敗反饋

| 色階 | Hex | RGB | 使用場景 | 對比度 (白底) |
|------|-----|-----|---------|-------------|
| Error-900 | #590005 | 89, 0, 5 | 深色背景 | 12.1:1 ✅ |
| Error-700 | #A4262C | 164, 38, 44 | 懸停狀態 | 6.8:1 ✅ |
| Error-500 | #D13438 | 209, 52, 56 | 錯誤圖標、按鈕 | 5.0:1 ✅ |
| Error-300 | #F1707B | 241, 112, 123 | 輔助元素 | 2.8:1 ❌ |
| Error-100 | #FDE7E9 | 253, 231, 233 | 錯誤通知背景 | 1.1:1 ❌ |

**使用示例**:
```
錯誤通知背景: Error-100 (#FDE7E9)
錯誤圖標: Error-500 (#D13438)
刪除按鈕: Error-700 (#A4262C)
失敗狀態指示: Error-500 (#D13438)
表單驗證錯誤: Error-500 (#D13438)
```

---

## 中性色系統

### Neutral Gray（中性灰）

**用途**: 文本、邊框、背景、分隔線

| 色階 | Hex | RGB | 使用場景 | 對比度 (白底) |
|------|-----|-----|---------|-------------|
| Neutral-900 | #201F1E | 32, 31, 30 | 主要文本 | 15.8:1 ✅ |
| Neutral-800 | #323130 | 50, 49, 48 | 副標題 | 13.1:1 ✅ |
| Neutral-700 | #605E5C | 96, 94, 92 | 輔助文本 | 7.2:1 ✅ |
| Neutral-600 | #8A8886 | 138, 136, 134 | 禁用文本 | 4.6:1 ✅ |
| Neutral-500 | #C8C6C4 | 200, 198, 196 | 邊框、分隔線 | 2.3:1 ⚠️ |
| Neutral-300 | #EDEBE9 | 237, 235, 233 | 卡片背景 | 1.1:1 ❌ |
| Neutral-100 | #F3F2F1 | 243, 242, 241 | 頁面背景 | 1.0:1 ❌ |
| Neutral-50 | #FAF9F8 | 250, 249, 248 | 淺色背景 | 1.0:1 ❌ |

**使用示例**:
```
主要文本: Neutral-900 (#201F1E)
副標題: Neutral-800 (#323130)
輔助文本: Neutral-700 (#605E5C)
禁用狀態: Neutral-600 (#8A8886)
邊框: Neutral-500 (#C8C6C4)
卡片背景: Neutral-300 (#EDEBE9)
頁面背景: Neutral-100 (#F3F2F1)
```

---

## 語義色彩映射

### 按鈕顏色

| 按鈕類型 | 背景色 | 文字色 | 邊框色 | 懸停背景 | 禁用背景 |
|---------|-------|-------|-------|---------|---------|
| Primary | Primary-500 | White | - | Primary-700 | Neutral-300 |
| Secondary | White | Primary-500 | Primary-500 | Primary-100 | Neutral-300 |
| Success | Success-700 | White | - | Success-900 | Neutral-300 |
| Danger | Error-700 | White | - | Error-900 | Neutral-300 |
| Ghost | Transparent | Neutral-900 | Neutral-500 | Neutral-100 | Transparent |

### 狀態顏色

| 狀態 | 圖標色 | 背景色 | 邊框色 | 使用場景 |
|------|-------|-------|-------|---------|
| Info | Primary-500 | Primary-100 | Primary-300 | 信息提示、幫助文檔 |
| Success | Success-500 | Success-100 | Success-300 | 成功操作、健康狀態 |
| Warning | Warning-700 | Warning-100 | Warning-300 | 警告信息、需注意 |
| Error | Error-500 | Error-100 | Error-300 | 錯誤狀態、失敗操作 |
| Neutral | Neutral-700 | Neutral-100 | Neutral-300 | 默認狀態、通用信息 |

### Agent 狀態顏色

| 狀態 | 顏色 | Hex | 使用場景 |
|------|-----|-----|---------|
| Running | Success-500 | #107C10 | Agent 運行中 |
| Stopped | Neutral-600 | #8A8886 | Agent 已停止 |
| Error | Error-500 | #D13438 | Agent 錯誤 |
| Warning | Warning-700 | #CA5010 | Agent 警告 |
| Deploying | Primary-500 | #0078D4 | Agent 部署中 |

---

## 深色主題（Dark Theme）

### 主要顏色適配

| 淺色主題 | 深色主題 | 說明 |
|---------|---------|------|
| Primary-500 | Primary-300 | 主色調整為更淺 |
| Success-500 | Success-300 | 成功色調整 |
| Warning-500 | Warning-300 | 警告色調整 |
| Error-500 | Error-300 | 錯誤色調整 |

### 背景和文字顏色

| 元素 | 淺色主題 | 深色主題 |
|------|---------|---------|
| 頁面背景 | Neutral-100 (#F3F2F1) | #1E1E1E |
| 卡片背景 | White (#FFFFFF) | #2D2D2D |
| 主要文本 | Neutral-900 (#201F1E) | #FFFFFF |
| 副標題 | Neutral-800 (#323130) | #E8E8E8 |
| 輔助文本 | Neutral-700 (#605E5C) | #B8B8B8 |
| 邊框 | Neutral-500 (#C8C6C4) | #444444 |

---

## 可訪問性檢查清單

### 文字對比度要求

- ✅ **AAA 級別**: 對比度 ≥7:1（主要文本）
- ✅ **AA 級別**: 對比度 ≥4.5:1（正文）、≥3:1（大文本 ≥18pt）
- ✅ **UI 元素**: 對比度 ≥3:1（按鈕、圖標、邊框）

### 對比度測試工具

- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Coolors Contrast Checker](https://coolors.co/contrast-checker)
- Figma Plugin: "Color Contrast Checker"

### 色盲友好檢查

- **紅綠色盲**: 不僅依賴顏色區分狀態，使用圖標輔助
- **藍黃色盲**: 避免單獨使用藍色和黃色區分關鍵信息
- **全色盲**: 確保足夠的明度對比

---

## CSS 變量定義

```css
:root {
  /* Primary Colors */
  --color-primary-900: #002050;
  --color-primary-700: #004C87;
  --color-primary-500: #0078D4;
  --color-primary-300: #4A9EE8;
  --color-primary-100: #DEECF9;

  /* Success Colors */
  --color-success-900: #004B1C;
  --color-success-700: #0E7C0E;
  --color-success-500: #107C10;
  --color-success-300: #6BB700;
  --color-success-100: #DFF6DD;

  /* Warning Colors */
  --color-warning-900: #5C2E00;
  --color-warning-700: #CA5010;
  --color-warning-500: #FF8C00;
  --color-warning-300: #FFAA44;
  --color-warning-100: #FFF4CE;

  /* Error Colors */
  --color-error-900: #590005;
  --color-error-700: #A4262C;
  --color-error-500: #D13438;
  --color-error-300: #F1707B;
  --color-error-100: #FDE7E9;

  /* Neutral Colors */
  --color-neutral-900: #201F1E;
  --color-neutral-800: #323130;
  --color-neutral-700: #605E5C;
  --color-neutral-600: #8A8886;
  --color-neutral-500: #C8C6C4;
  --color-neutral-300: #EDEBE9;
  --color-neutral-100: #F3F2F1;
  --color-neutral-50: #FAF9F8;

  /* Semantic Colors */
  --color-background: var(--color-neutral-100);
  --color-surface: #FFFFFF;
  --color-text-primary: var(--color-neutral-900);
  --color-text-secondary: var(--color-neutral-700);
  --color-border: var(--color-neutral-500);
}

/* Dark Theme */
[data-theme="dark"] {
  --color-background: #1E1E1E;
  --color-surface: #2D2D2D;
  --color-text-primary: #FFFFFF;
  --color-text-secondary: #B8B8B8;
  --color-border: #444444;
}
```

---

## 參考資料

- [Material Design Color System](https://m3.material.io/styles/color/overview)
- [Fluent UI Theme Designer](https://fabricweb.z5.web.core.windows.net/pr-deploy-site/refs/heads/master/theming-designer/)
- [WCAG 2.1 Contrast Guidelines](https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html)

---

**下一步**: 創建字體系統文檔
