# Design Tokens

**版本**: 1.0.0
**狀態**: ✅ 已完成
**日期**: 2025-10-29
**負責人**: UI/UX Designer (BMad Method)

[← 返回 Design System](./README.md) | [← 返回 UX Design](../README.md)

---

## 📋 概述

Design Tokens 是設計系統的原子級元素，定義了所有視覺設計的基本屬性。這些 tokens 確保整個應用的視覺一致性，並且可以輕鬆地跨平台和主題進行調整。

本文檔基於 **Material Design 3** 和 **Material-UI (MUI) v5** 的設計原則，針對 Semantic Kernel Agentic Framework 進行定制。

### 核心原則
- **一致性**: 所有 UI 元素使用統一的設計語言
- **可擴展性**: Token 結構支持主題切換（亮色/暗色模式）
- **語義化**: Token 名稱清晰表達其用途
- **開發者友好**: 易於在代碼中引用和使用

---

## 🎨 Color System - 顏色系統

### 1.1 Brand Colors - 品牌色

```typescript
// Primary - 主色（藍色系）
const primary = {
  50:  '#E3F2FD',  // 最淡
  100: '#BBDEFB',
  200: '#90CAF9',
  300: '#64B5F6',
  400: '#42A5F5',
  500: '#2196F3',  // 主色基準
  600: '#1E88E2',
  700: '#1976D2',
  800: '#1565C0',
  900: '#0D47A1',  // 最深
};

// Secondary - 輔色（紫色系）
const secondary = {
  50:  '#F3E5F5',
  100: '#E1BEE7',
  200: '#CE93D8',
  300: '#BA68C8',
  400: '#AB47BC',
  500: '#9C27B0',  // 輔色基準
  600: '#8E24AA',
  700: '#7B1FA2',
  800: '#6A1B9A',
  900: '#4A148C',
};
```

**使用場景**:
- **Primary 500**: 主要 CTA 按鈕、重要操作、主導航高亮
- **Primary 700**: 按鈕 hover 狀態、焦點狀態
- **Secondary 500**: 次要操作、標籤、圖標強調
- **Secondary 700**: 輔助交互狀態

---

### 1.2 Semantic Colors - 語義色

```typescript
// Success - 成功狀態（綠色）
const success = {
  light: '#81C784',   // success.light
  main:  '#4CAF50',   // success.main
  dark:  '#388E3C',   // success.dark
  contrastText: '#FFFFFF',
};

// Warning - 警告狀態（橙色）
const warning = {
  light: '#FFB74D',
  main:  '#FF9800',
  dark:  '#F57C00',
  contrastText: '#000000',
};

// Error - 錯誤狀態（紅色）
const error = {
  light: '#E57373',
  main:  '#F44336',
  dark:  '#D32F2F',
  contrastText: '#FFFFFF',
};

// Info - 信息狀態（藍色）
const info = {
  light: '#64B5F6',
  main:  '#2196F3',
  dark:  '#1976D2',
  contrastText: '#FFFFFF',
};
```

**使用場景**:
- **Success**: 操作成功提示、執行成功狀態、正面指標
- **Warning**: 警告信息、需要注意的操作、中等風險提示
- **Error**: 錯誤提示、失敗狀態、高風險警告、表單驗證錯誤
- **Info**: 中性信息提示、幫助說明、系統通知

---

### 1.3 Neutral Colors - 中性色

```typescript
// Grey Scale - 灰階（用於文字、背景、邊框）
const grey = {
  50:  '#FAFAFA',  // 最淡背景
  100: '#F5F5F5',  // 次級背景
  200: '#EEEEEE',  // 分隔線、禁用背景
  300: '#E0E0E0',  // 邊框
  400: '#BDBDBD',  // 輔助文字、圖標
  500: '#9E9E9E',  // 次要文字
  600: '#757575',  // 正文文字
  700: '#616161',  // 標題文字
  800: '#424242',  // 重要標題
  900: '#212121',  // 最深黑色
};
```

**文字顏色應用**:
```typescript
const textColors = {
  primary:   grey[900],  // 主要文字（標題、正文）
  secondary: grey[600],  // 次要文字（說明、標籤）
  disabled:  grey[400],  // 禁用狀態文字
  hint:      grey[500],  // 提示文字
};
```

---

### 1.4 Background Colors - 背景色

```typescript
// Light Mode - 亮色模式
const lightBackground = {
  default:  '#FFFFFF',    // 主背景
  paper:    '#FFFFFF',    // 卡片背景
  elevated: '#F5F5F5',    // 懸浮元素背景
  hover:    'rgba(0, 0, 0, 0.04)',  // Hover 狀態背景
  selected: 'rgba(33, 150, 243, 0.08)',  // 選中狀態背景
};

// Dark Mode - 暗色模式
const darkBackground = {
  default:  '#121212',    // 主背景
  paper:    '#1E1E1E',    // 卡片背景
  elevated: '#2C2C2C',    // 懸浮元素背景
  hover:    'rgba(255, 255, 255, 0.08)',
  selected: 'rgba(33, 150, 243, 0.16)',
};
```

---

### 1.5 Overlay Colors - 遮罩色

```typescript
const overlayColors = {
  backdrop:       'rgba(0, 0, 0, 0.5)',    // Modal/Dialog 背景遮罩
  tooltipBg:      'rgba(97, 97, 97, 0.92)', // Tooltip 背景
  loadingOverlay: 'rgba(255, 255, 255, 0.8)', // Loading 遮罩
  focusRing:      'rgba(33, 150, 243, 0.25)', // 焦點環
};
```

---

## 📝 Typography System - 字體系統

### 2.1 Font Family - 字體族

```typescript
const fontFamily = {
  primary: [
    '-apple-system',
    'BlinkMacSystemFont',
    '"Segoe UI"',
    'Roboto',
    '"Helvetica Neue"',
    'Arial',
    'sans-serif',
    '"Apple Color Emoji"',
    '"Segoe UI Emoji"',
    '"Segoe UI Symbol"',
  ].join(','),

  monospace: [
    '"Fira Code"',
    '"Source Code Pro"',
    'Menlo',
    'Monaco',
    'Consolas',
    '"Courier New"',
    'monospace',
  ].join(','),
};
```

**使用場景**:
- **primary**: 所有 UI 文字（標題、正文、按鈕等）
- **monospace**: 代碼塊、終端輸出、日誌、配置文件

---

### 2.2 Font Sizes - 字體大小

基於 **4px 基礎單位**，使用 **1.25 倍數比例** 生成字體尺寸。

```typescript
const fontSize = {
  xs:   '0.75rem',   // 12px - 次要標籤、時間戳
  sm:   '0.875rem',  // 14px - 正文、表單輸入
  base: '1rem',      // 16px - 標準正文
  lg:   '1.125rem',  // 18px - 大正文、次標題
  xl:   '1.25rem',   // 20px - H5 標題
  '2xl': '1.5rem',   // 24px - H4 標題
  '3xl': '1.875rem', // 30px - H3 標題
  '4xl': '2.25rem',  // 36px - H2 標題
  '5xl': '3rem',     // 48px - H1 標題
  '6xl': '3.75rem',  // 60px - 超大標題（特殊場景）
};
```

---

### 2.3 Font Weights - 字重

```typescript
const fontWeight = {
  light:    300,  // 輕字重（不常用）
  regular:  400,  // 正常字重（正文）
  medium:   500,  // 中等字重（強調、按鈕）
  semibold: 600,  // 半粗體（小標題）
  bold:     700,  // 粗體（標題、重要信息）
};
```

**使用場景**:
- **regular (400)**: 正文、說明文字
- **medium (500)**: 按鈕文字、導航鏈接、標籤
- **semibold (600)**: 卡片標題、小節標題
- **bold (700)**: 頁面標題、章節標題、重要數據

---

### 2.4 Line Heights - 行高

```typescript
const lineHeight = {
  none:    1,      // 圖標、單行標題
  tight:   1.25,   // 大標題 (H1-H3)
  snug:    1.375,  // 小標題 (H4-H6)
  normal:  1.5,    // 正文（標準）
  relaxed: 1.625,  // 長文本、文章
  loose:   2,      // 特殊排版需求
};
```

**使用場景**:
- **tight (1.25)**: 大標題，減少行間距以保持緊湊
- **normal (1.5)**: 正文、說明文字，保證可讀性
- **relaxed (1.625)**: 長文本閱讀、文章內容

---

### 2.5 Letter Spacing - 字間距

```typescript
const letterSpacing = {
  tighter: '-0.05em',  // -0.8px
  tight:   '-0.025em', // -0.4px
  normal:  '0',
  wide:    '0.025em',  // 0.4px
  wider:   '0.05em',   // 0.8px
  widest:  '0.1em',    // 1.6px
};
```

**使用場景**:
- **tight**: 大標題（H1-H2），視覺更緊湊
- **normal**: 正文、標準文字
- **wide**: 全大寫文字（如按鈕文字、標籤）
- **wider**: 小號大寫文字（如分類標題）

---

### 2.6 Typography Variants - 排版變體

Material-UI 標準排版變體配置：

```typescript
const typography = {
  h1: {
    fontSize: fontSize['5xl'],     // 48px
    fontWeight: fontWeight.bold,   // 700
    lineHeight: lineHeight.tight,  // 1.25
    letterSpacing: letterSpacing.tight, // -0.025em
  },
  h2: {
    fontSize: fontSize['4xl'],
    fontWeight: fontWeight.bold,
    lineHeight: lineHeight.tight,
    letterSpacing: letterSpacing.tight,
  },
  h3: {
    fontSize: fontSize['3xl'],
    fontWeight: fontWeight.semibold,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.normal,
  },
  h4: {
    fontSize: fontSize['2xl'],
    fontWeight: fontWeight.semibold,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.normal,
  },
  h5: {
    fontSize: fontSize.xl,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.normal,
  },
  h6: {
    fontSize: fontSize.lg,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.normal,
    letterSpacing: letterSpacing.normal,
  },
  body1: {
    fontSize: fontSize.base,       // 16px - 標準正文
    fontWeight: fontWeight.regular,
    lineHeight: lineHeight.normal,
    letterSpacing: letterSpacing.normal,
  },
  body2: {
    fontSize: fontSize.sm,         // 14px - 次要正文
    fontWeight: fontWeight.regular,
    lineHeight: lineHeight.normal,
    letterSpacing: letterSpacing.normal,
  },
  subtitle1: {
    fontSize: fontSize.base,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.normal,
  },
  subtitle2: {
    fontSize: fontSize.sm,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.normal,
  },
  button: {
    fontSize: fontSize.sm,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.none,
    letterSpacing: letterSpacing.wide,
    textTransform: 'uppercase',
  },
  caption: {
    fontSize: fontSize.xs,
    fontWeight: fontWeight.regular,
    lineHeight: lineHeight.normal,
    letterSpacing: letterSpacing.normal,
  },
  overline: {
    fontSize: fontSize.xs,
    fontWeight: fontWeight.medium,
    lineHeight: lineHeight.snug,
    letterSpacing: letterSpacing.wider,
    textTransform: 'uppercase',
  },
};
```

---

## 📏 Spacing System - 間距系統

### 3.1 Base Unit - 基礎單位

使用 **8px** 作為基礎單位（Material Design 標準）。

```typescript
const spacing = (factor: number) => `${8 * factor}px`;

// 或使用 rem 單位（1rem = 16px）
const spacingRem = (factor: number) => `${0.5 * factor}rem`;
```

### 3.2 Spacing Scale - 間距刻度

```typescript
const space = {
  0:    '0',        // 0px
  0.5:  '4px',      // 0.25rem
  1:    '8px',      // 0.5rem
  1.5:  '12px',     // 0.75rem
  2:    '16px',     // 1rem
  2.5:  '20px',     // 1.25rem
  3:    '24px',     // 1.5rem
  3.5:  '28px',     // 1.75rem
  4:    '32px',     // 2rem
  5:    '40px',     // 2.5rem
  6:    '48px',     // 3rem
  7:    '56px',     // 3.5rem
  8:    '64px',     // 4rem
  9:    '72px',     // 4.5rem
  10:   '80px',     // 5rem
  12:   '96px',     // 6rem
  16:   '128px',    // 8rem
  20:   '160px',    // 10rem
  24:   '192px',    // 12rem
};
```

**使用場景**:
- **0.5 (4px)**: 圖標與文字間距、按鈕內邊距微調
- **1 (8px)**: 緊密元素間距（表單內元素）
- **2 (16px)**: 標準元素間距（卡片內部、按鈕內邊距）
- **3 (24px)**: 組件間距（表單組之間）
- **4 (32px)**: 區塊間距（卡片外邊距）
- **6 (48px)**: 章節間距（頁面區塊之間）
- **8+ (64px+)**: 大區塊間距（頁面布局）

---

### 3.3 Container Spacing - 容器間距

```typescript
const containerSpacing = {
  xs: space[2],   // 16px - 小容器內邊距
  sm: space[3],   // 24px - 標準容器內邊距
  md: space[4],   // 32px - 中型容器內邊距
  lg: space[6],   // 48px - 大容器內邊距
  xl: space[8],   // 64px - 超大容器內邊距
};
```

---

## 🎭 Shadows & Elevation - 陰影與高度

### 4.1 Shadow Scale - 陰影刻度

基於 Material Design 的 elevation 系統。

```typescript
const shadows = {
  none: 'none',

  // Level 1 - 輕微懸浮（卡片、按鈕）
  sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',

  // Level 2 - 標準懸浮（下拉菜單、提示框）
  base: '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',

  // Level 4 - 中等懸浮（浮動按鈕、卡片 hover）
  md: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',

  // Level 6 - 高懸浮（對話框、彈窗）
  lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',

  // Level 8 - 極高懸浮（Modal、Drawer）
  xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',

  // Level 12 - 最高懸浮（全屏對話框）
  '2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.25)',

  // Inner Shadow - 內陰影（輸入框、按下狀態）
  inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.06)',
};
```

**使用場景**:
- **sm**: 卡片、列表項、按鈕默認狀態
- **base**: 下拉菜單、Tooltip、懸浮提示
- **md**: 按鈕 hover、卡片 hover、浮動操作按鈕
- **lg**: 對話框、彈出面板、側邊欄
- **xl**: Modal、全屏 Drawer
- **2xl**: 全屏對話框、重要彈窗
- **inner**: 輸入框、文本域、按下狀態

---

### 4.2 Elevation Mapping - 高度映射

```typescript
const elevation = {
  0:  shadows.none,
  1:  shadows.sm,
  2:  shadows.base,
  4:  shadows.md,
  6:  shadows.lg,
  8:  shadows.xl,
  12: shadows['2xl'],
  16: shadows['2xl'],
  24: shadows['2xl'],
};
```

---

## 🔲 Border Radius - 圓角系統

### 5.1 Radius Scale - 圓角刻度

```typescript
const borderRadius = {
  none: '0',
  sm:   '4px',    // 0.25rem - 小元素（標籤、徽章）
  base: '6px',    // 0.375rem - 標準元素（按鈕、輸入框）
  md:   '8px',    // 0.5rem - 中型元素（卡片）
  lg:   '12px',   // 0.75rem - 大型元素（對話框）
  xl:   '16px',   // 1rem - 超大元素（Modal）
  '2xl': '24px',  // 1.5rem - 特大元素
  '3xl': '32px',  // 2rem - 極大元素
  full: '9999px', // 完全圓形（頭像、圓形按鈕）
};
```

**使用場景**:
- **sm (4px)**: Tag、Badge、Chip
- **base (6px)**: Button、Input、Select
- **md (8px)**: Card、Panel、Menu
- **lg (12px)**: Dialog、Popover
- **xl (16px)**: Modal、Drawer
- **full (9999px)**: Avatar、Circle Button、Status Indicator

---

## ⚡ Animation & Transition - 動畫與過渡

### 6.1 Duration - 持續時間

```typescript
const duration = {
  fastest: '100ms',   // 極快（微交互、ripple）
  fast:    '150ms',   // 快速（hover、focus）
  base:    '200ms',   // 標準（按鈕、輸入框）
  slow:    '300ms',   // 緩慢（對話框、抽屜）
  slower:  '500ms',   // 極慢（頁面過渡）
};
```

**使用場景**:
- **fastest (100ms)**: Ripple 效果、按鈕按下反饋
- **fast (150ms)**: Hover 狀態變化、Focus 狀態
- **base (200ms)**: 按鈕點擊、輸入框展開、下拉菜單
- **slow (300ms)**: 對話框打開/關閉、抽屜滑動
- **slower (500ms)**: 頁面過渡、大型動畫

---

### 6.2 Easing - 緩動函數

```typescript
const easing = {
  linear:     'cubic-bezier(0, 0, 1, 1)',
  easeIn:     'cubic-bezier(0.4, 0, 1, 1)',        // 加速
  easeOut:    'cubic-bezier(0, 0, 0.2, 1)',        // 減速
  easeInOut:  'cubic-bezier(0.4, 0, 0.2, 1)',      // 標準（推薦）
  sharp:      'cubic-bezier(0.4, 0, 0.6, 1)',      // 快速過渡

  // Material Design 標準緩動
  standard:   'cubic-bezier(0.4, 0, 0.2, 1)',
  emphasized: 'cubic-bezier(0.0, 0, 0.2, 1)',
  decelerated:'cubic-bezier(0.0, 0, 0.2, 1)',
  accelerated:'cubic-bezier(0.4, 0, 1, 1)',
};
```

**使用場景**:
- **easeInOut (standard)**: 大部分場景（按鈕、輸入框、卡片）
- **easeOut (decelerated)**: 元素進入視圖（對話框打開、下拉菜單展開）
- **easeIn (accelerated)**: 元素離開視圖（對話框關閉、元素隱藏）
- **sharp**: 快速過渡（Tooltip、Snackbar）

---

### 6.3 Transition Presets - 過渡預設

```typescript
const transition = {
  // 標準過渡（推薦用於大多數場景）
  standard: `all ${duration.base} ${easing.standard}`,

  // 快速過渡（hover 狀態）
  fast: `all ${duration.fast} ${easing.standard}`,

  // 緩慢過渡（對話框、抽屜）
  slow: `all ${duration.slow} ${easing.emphasized}`,

  // 顏色過渡
  color: `color ${duration.fast} ${easing.standard},
          background-color ${duration.fast} ${easing.standard},
          border-color ${duration.fast} ${easing.standard}`,

  // 透明度過渡
  opacity: `opacity ${duration.base} ${easing.standard}`,

  // 變換過渡（位移、縮放、旋轉）
  transform: `transform ${duration.base} ${easing.emphasized}`,

  // 陰影過渡
  shadow: `box-shadow ${duration.base} ${easing.standard}`,
};
```

---

## 📱 Responsive Breakpoints - 響應式斷點

### 7.1 Breakpoint Values - 斷點值

```typescript
const breakpoints = {
  xs: 0,       // 手機豎屏
  sm: 600px,   // 手機橫屏、小平板
  md: 960px,   // 平板
  lg: 1280px,  // 桌面
  xl: 1920px,  // 大屏桌面
};
```

**使用場景**:
- **xs (0-599px)**: 手機豎屏
- **sm (600-959px)**: 手機橫屏、小平板
- **md (960-1279px)**: 平板、小型筆記本
- **lg (1280-1919px)**: 標準桌面、筆記本
- **xl (1920px+)**: 大屏桌面、外接顯示器

---

### 7.2 Media Queries - 媒體查詢

```typescript
const mediaQueries = {
  xs: '@media (min-width: 0px)',
  sm: '@media (min-width: 600px)',
  md: '@media (min-width: 960px)',
  lg: '@media (min-width: 1280px)',
  xl: '@media (min-width: 1920px)',

  // Mobile First
  smUp: '@media (min-width: 600px)',
  mdUp: '@media (min-width: 960px)',
  lgUp: '@media (min-width: 1280px)',
  xlUp: '@media (min-width: 1920px)',

  // Desktop First
  smDown: '@media (max-width: 599px)',
  mdDown: '@media (max-width: 959px)',
  lgDown: '@media (max-width: 1279px)',
  xlDown: '@media (max-width: 1919px)',
};
```

---

## 🔧 Z-Index Scale - 層級系統

### 8.1 Z-Index Values - 層級值

```typescript
const zIndex = {
  // 內容層級
  base:      0,      // 正常內容
  dropdown:  1000,   // 下拉菜單
  sticky:    1100,   // 粘性元素（Sticky Header）
  fixed:     1200,   // 固定元素（Fixed Footer）

  // 浮層層級
  backdrop:  1300,   // 遮罩層
  drawer:    1400,   // 側邊欄 Drawer
  modal:     1500,   // Modal 對話框
  snackbar:  1600,   // Snackbar 通知
  tooltip:   1700,   // Tooltip 提示框
};
```

**使用場景**:
- **dropdown (1000)**: Select 下拉菜單、Autocomplete
- **sticky (1100)**: 頁面頭部、吸頂導航
- **fixed (1200)**: 頁腳、返回頂部按鈕
- **backdrop (1300)**: Modal/Drawer 的半透明遮罩
- **drawer (1400)**: 側邊欄、抽屜面板
- **modal (1500)**: 對話框、彈窗
- **snackbar (1600)**: Toast 通知、消息提示
- **tooltip (1700)**: 提示框、氣泡提示（最高層級）

---

## 📐 Grid System - 網格系統

### 9.1 Container Widths - 容器寬度

```typescript
const containerMaxWidth = {
  xs: '100%',      // 手機全寬
  sm: '600px',     // 小屏最大寬度
  md: '960px',     // 平板最大寬度
  lg: '1280px',    // 桌面最大寬度
  xl: '1920px',    // 大屏最大寬度
};
```

---

### 9.2 Grid Columns - 網格列數

```typescript
const gridColumns = {
  xs: 4,   // 手機 4 列
  sm: 8,   // 平板 8 列
  md: 12,  // 桌面 12 列
  lg: 12,
  xl: 12,
};
```

---

### 9.3 Gutter Spacing - 間距設置

```typescript
const gutterWidth = {
  xs: space[2],  // 16px
  sm: space[3],  // 24px
  md: space[4],  // 32px
  lg: space[4],  // 32px
  xl: space[4],  // 32px
};
```

---

## 🎯 Usage Examples - 使用範例

### 10.1 在 Material-UI 中使用

```typescript
import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#2196F3',
      light: '#64B5F6',
      dark: '#1976D2',
    },
    secondary: {
      main: '#9C27B0',
      light: '#BA68C8',
      dark: '#7B1FA2',
    },
    success: {
      main: '#4CAF50',
    },
    error: {
      main: '#F44336',
    },
    warning: {
      main: '#FF9800',
    },
    info: {
      main: '#2196F3',
    },
    grey: {
      50: '#FAFAFA',
      100: '#F5F5F5',
      // ... 其他灰階
    },
  },
  typography: {
    fontFamily: [
      '-apple-system',
      'BlinkMacSystemFont',
      '"Segoe UI"',
      'Roboto',
      // ...
    ].join(','),
    h1: {
      fontSize: '3rem',
      fontWeight: 700,
      lineHeight: 1.25,
    },
    // ... 其他排版變體
  },
  spacing: 8, // 基礎單位 8px
  shape: {
    borderRadius: 6,
  },
  shadows: [
    'none',
    '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
    // ... 其他陰影級別
  ],
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 960,
      lg: 1280,
      xl: 1920,
    },
  },
  zIndex: {
    mobileStepper: 1000,
    appBar: 1100,
    drawer: 1200,
    modal: 1300,
    snackbar: 1400,
    tooltip: 1500,
  },
  transitions: {
    easing: {
      easeInOut: 'cubic-bezier(0.4, 0, 0.2, 1)',
      easeOut: 'cubic-bezier(0.0, 0, 0.2, 1)',
      easeIn: 'cubic-bezier(0.4, 0, 1, 1)',
      sharp: 'cubic-bezier(0.4, 0, 0.6, 1)',
    },
    duration: {
      shortest: 150,
      shorter: 200,
      short: 250,
      standard: 300,
      complex: 375,
      enteringScreen: 225,
      leavingScreen: 195,
    },
  },
});

export default theme;
```

---

### 10.2 在 CSS/SCSS 中使用

```scss
// _tokens.scss

// Colors
$primary-500: #2196F3;
$secondary-500: #9C27B0;
$grey-900: #212121;

// Spacing
$space-1: 8px;
$space-2: 16px;
$space-3: 24px;
$space-4: 32px;

// Typography
$font-base: 1rem;
$font-lg: 1.125rem;
$font-2xl: 1.5rem;

// Shadows
$shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
$shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

// Border Radius
$radius-base: 6px;
$radius-md: 8px;

// Transitions
$transition-fast: all 150ms cubic-bezier(0.4, 0, 0.2, 1);
$transition-base: all 200ms cubic-bezier(0.4, 0, 0.2, 1);

// Usage Example
.button {
  padding: $space-1 $space-2;
  background-color: $primary-500;
  border-radius: $radius-base;
  box-shadow: $shadow-sm;
  transition: $transition-fast;

  &:hover {
    box-shadow: $shadow-md;
  }
}
```

---

### 10.3 在 CSS Variables 中使用

```css
:root {
  /* Colors */
  --color-primary-500: #2196F3;
  --color-secondary-500: #9C27B0;
  --color-grey-900: #212121;

  /* Spacing */
  --space-1: 8px;
  --space-2: 16px;
  --space-3: 24px;

  /* Typography */
  --font-base: 1rem;
  --font-weight-medium: 500;

  /* Shadows */
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);

  /* Border Radius */
  --radius-base: 6px;

  /* Transitions */
  --transition-base: all 200ms cubic-bezier(0.4, 0, 0.2, 1);
}

/* Usage */
.card {
  padding: var(--space-3);
  background-color: white;
  border-radius: var(--radius-base);
  box-shadow: var(--shadow-sm);
  transition: var(--transition-base);
}
```

---

## 📚 參考資料

### Design Systems
- [Material Design 3](https://m3.material.io/)
- [Material-UI (MUI) Documentation](https://mui.com/)
- [Tailwind CSS Design System](https://tailwindcss.com/docs)

### Design Tokens
- [Style Dictionary](https://amzn.github.io/style-dictionary/)
- [Design Tokens W3C Community Group](https://www.w3.org/community/design-tokens/)

### Accessibility
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)

---

## ✅ 驗收標準

### Design Token 完成標準

**顏色系統**:
- [ ] 定義完整的品牌色（Primary/Secondary）
- [ ] 定義語義色（Success/Warning/Error/Info）
- [ ] 定義中性色（Grey Scale）
- [ ] 定義背景色和遮罩色
- [ ] 確保色彩對比度符合 WCAG 2.1 AA（≥4.5:1）

**字體系統**:
- [ ] 定義字體族（Primary/Monospace）
- [ ] 定義完整的字體尺寸刻度（10 個級別）
- [ ] 定義字重系統（5 個級別）
- [ ] 定義行高和字間距
- [ ] 配置 Material-UI Typography 變體

**間距系統**:
- [ ] 基於 8px 網格的間距刻度
- [ ] 定義容器間距規範
- [ ] 定義網格系統配置

**視覺效果**:
- [ ] 定義陰影系統（7 個級別）
- [ ] 定義圓角系統（9 個級別）
- [ ] 定義動畫時長和緩動函數
- [ ] 定義過渡預設

**響應式**:
- [ ] 定義斷點值（5 個）
- [ ] 定義媒體查詢工具
- [ ] 定義容器最大寬度

**其他**:
- [ ] 定義 Z-Index 層級系統
- [ ] 提供 Material-UI 配置範例
- [ ] 提供 CSS/SCSS 使用範例
- [ ] 提供 CSS Variables 使用範例

---

**文檔狀態**: ✅ 已完成
**審查人**: UI/UX Designer
**審查日期**: 2025-10-29
**版本**: 1.0.0

---

[← 返回 Design System](./README.md) | [← 返回 UX Design](../README.md)
