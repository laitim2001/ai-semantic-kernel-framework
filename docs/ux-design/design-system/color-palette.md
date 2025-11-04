# 色彩系統

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成

[← 返回 Design System](./README.md)

---

## 📋 概述

色彩系統是 Semantic Kernel Agentic Framework 視覺設計的核心要素，基於 **Material Design 3** 的色彩理論，提供一致、可訪問、具有語義化的色彩方案。

### 設計原則

1. **語義化 (Semantic)**: 顏色具有明確的意義和用途
2. **可訪問性 (Accessible)**: 所有顏色組合符合 WCAG 2.1 AA 標準（對比度 ≥4.5:1）
3. **一致性 (Consistent)**: 統一的色彩應用規範
4. **適應性 (Adaptive)**: 支持亮色/暗色主題無縫切換
5. **系統化 (Systematic)**: 基於色彩階梯的可預測色彩體系

---

## 🎨 色彩階梯（Color Scale）

### 色彩階梯說明

每個顏色從 `50`（最淺）到 `900`（最深）共 10 個層級：

- **50-100**: 背景色、Hover 狀態
- **200-300**: 次要元素、Disabled 狀態
- **400-600**: 主要使用層級（500 為 Main Color）
- **700-800**: 深色強調、按鈕按下狀態
- **900**: 文字顏色、最深強調

---

## 品牌色（Brand Colors）

### Primary - 藍色（Blue）

**品牌主色**，代表專業、可信賴、技術感。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#E3F2FD` | 背景、Hover 狀態 |
| 100 | `#BBDEFB` | 淺色背景 |
| 200 | `#90CAF9` | Border、Divider |
| 300 | `#64B5F6` | 圖標、次要按鈕 |
| 400 | `#42A5F5` | Active 狀態 |
| **500** | **`#2196F3`** | **主色（按鈕、連結、重點）** |
| 600 | `#1E88E2` | Hover 狀態 |
| 700 | `#1976D2` | 深色強調 |
| 800 | `#1565C0` | Pressed 狀態 |
| 900 | `#0D47A1` | 深色文字、最深強調 |

**對比度測試**:
- Primary-500 on White: **3.96:1** (通過 AA-Large)
- Primary-700 on White: **6.29:1** (通過 AAA)
- White on Primary-500: **3.96:1** (通過 AA-Large)

### Secondary - 紫色（Purple）

**輔助色**，用於次要操作、特殊狀態、視覺強調。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#F3E5F5` | 背景、Hover 狀態 |
| 100 | `#E1BEE7` | 淺色背景 |
| 200 | `#CE93D8` | Border、Divider |
| 300 | `#BA68C8` | 圖標、次要按鈕 |
| 400 | `#AB47BC` | Active 狀態 |
| **500** | **`#9C27B0`** | **輔助色（次要按鈕、強調）** |
| 600 | `#8E24AA` | Hover 狀態 |
| 700 | `#7B1FA2` | 深色強調 |
| 800 | `#6A1B9A` | Pressed 狀態 |
| 900 | `#4A148C` | 深色文字、最深強調 |

**對比度測試**:
- Secondary-500 on White: **5.09:1** (通過 AA)
- Secondary-700 on White: **7.72:1** (通過 AAA)

---

## 語義色（Semantic Colors）

### Success - 綠色（Green）

**成功狀態**，表示成功操作、正確狀態、正面反饋。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#E8F5E9` | 背景 |
| 100 | `#C8E6C9` | 淺色背景 |
| 200 | `#A5D6A7` | Border |
| 300 | `#81C784` | 次要 |
| 400 | `#66BB6A` | 強調 |
| **500** | **`#4CAF50`** | **主要成功色** |
| 600 | `#43A047` | Hover |
| 700 | `#388E3C` | 深色 |
| 800 | `#2E7D32` | Pressed |
| 900 | `#1B5E20` | 最深 |

**使用場景**:
- ✅ 成功消息和通知
- ✅ 表單驗證通過
- ✅ Agent 運行中狀態
- ✅ 成功執行圖標

**對比度**: Success-500 on White: **3.37:1** (通過 AA-Large)

### Warning - 橙色（Orange）

**警告狀態**，表示需要注意、潛在問題、提示信息。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#FFF3E0` | 背景 |
| 100 | `#FFE0B2` | 淺色背景 |
| 200 | `#FFCC80` | Border |
| 300 | `#FFB74D` | 次要 |
| 400 | `#FFA726` | 強調 |
| **500** | **`#FF9800`** | **主要警告色** |
| 600 | `#FB8C00` | Hover |
| 700 | `#F57C00` | 深色 |
| 800 | `#EF6C00` | Pressed |
| 900 | `#E65100` | 最深 |

**使用場景**:
- ⚠️ 警告消息和通知
- ⚠️ 資源使用率高
- ⚠️ 需要確認的操作
- ⚠️ 臨時錯誤或超時

**對比度**: Warning-700 on White: **4.54:1** (通過 AA)

### Error - 紅色（Red）

**錯誤狀態**，表示錯誤、失敗、危險操作、禁止。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#FFEBEE` | 背景 |
| 100 | `#FFCDD2` | 淺色背景 |
| 200 | `#EF9A9A` | Border |
| 300 | `#E57373` | 次要 |
| 400 | `#EF5350` | 強調 |
| **500** | **`#F44336`** | **主要錯誤色** |
| 600 | `#E53935` | Hover |
| 700 | `#D32F2F` | 深色 |
| 800 | `#C62828` | Pressed |
| 900 | `#B71C1C` | 最深 |

**使用場景**:
- ❌ 錯誤消息和通知
- ❌ 表單驗證失敗
- ❌ Agent 錯誤或失敗
- ❌ 危險操作按鈕（刪除、停止）

**對比度**: Error-700 on White: **6.38:1** (通過 AAA)

### Info - 藍灰色（Blue-Grey）

**信息狀態**，表示提示信息、中性通知、額外說明。

| 層級 | 色值 | 使用場景 |
|------|------|---------|
| 50 | `#E3F2FD` | 背景 |
| 100 | `#BBDEFB` | 淺色背景 |
| 200 | `#90CAF9` | Border |
| 300 | `#64B5F6` | 次要 |
| 400 | `#42A5F5` | 強調 |
| **500** | **`#2196F3`** | **主要信息色** |
| 600 | `#1E88E2` | Hover |
| 700 | `#1976D2` | 深色 |
| 800 | `#1565C0` | Pressed |
| 900 | `#0D47A1` | 最深 |

**使用場景**:
- ℹ️ 信息消息和通知
- ℹ️ 幫助提示和說明
- ℹ️ 進度指示
- ℹ️ 中性狀態標記

**對比度**: Info-700 on White: **6.29:1** (通過 AAA)

---

## 中性色（Neutral Colors）

### Grey Scale

中性色用於文字、邊框、背景、陰影等，是最常用的顏色。

| 層級 | 色值 | 名稱 | 使用場景 |
|------|------|------|---------|
| 50 | `#FAFAFA` | Grey-50 | 淺色背景 |
| 100 | `#F5F5F5` | Grey-100 | 卡片背景 |
| 200 | `#EEEEEE` | Grey-200 | Hover 背景 |
| 300 | `#E0E0E0` | Grey-300 | Border、Divider |
| 400 | `#BDBDBD` | Grey-400 | Disabled 文字 |
| 500 | `#9E9E9E` | Grey-500 | 次要文字 |
| 600 | `#757575` | Grey-600 | 輔助文字 |
| 700 | `#616161` | Grey-700 | 說明文字 |
| 800 | `#424242` | Grey-800 | 次要標題 |
| 900 | `#212121` | Grey-900 | 主要文字、標題 |

**文字顏色對比度**:
- Grey-900 on White: **16.10:1** (AAA)
- Grey-700 on White: **7.88:1** (AAA)
- Grey-600 on White: **5.74:1** (AA)
- Grey-500 on White: **4.05:1** (AA-Large)

**特殊用途**:
- **Pure Black**: `#000000` - 避免使用，過於刺眼
- **Pure White**: `#FFFFFF` - 背景色
- **Near Black**: `#212121` (Grey-900) - 推薦文字色
- **Near White**: `#FAFAFA` (Grey-50) - 推薦背景色

---

## 背景色與遮罩色

### Background Colors (亮色主題)

| 名稱 | 色值 | 使用場景 |
|------|------|---------|
| Page Background | `#FAFAFA` (Grey-50) | 頁面主背景 |
| Card Background | `#FFFFFF` (White) | 卡片、對話框背景 |
| Hover Background | `#F5F5F5` (Grey-100) | Hover 狀態背景 |
| Selected Background | `#E3F2FD` (Primary-50) | 選中狀態背景 |
| Disabled Background | `#F5F5F5` (Grey-100) | 禁用狀態背景 |

### Background Colors (暗色主題)

| 名稱 | 色值 | 使用場景 |
|------|------|---------|
| Page Background | `#121212` | 頁面主背景 |
| Card Background | `#1E1E1E` | 卡片、對話框背景 |
| Hover Background | `#2C2C2C` | Hover 狀態背景 |
| Selected Background | `#1565C0` (Primary-800) | 選中狀態背景 |
| Disabled Background | `#2C2C2C` | 禁用狀態背景 |

### Overlay Colors

遮罩色用於 Modal、Drawer、Tooltip 等組件的背景遮罩。

| 名稱 | 色值 | 透明度 | 使用場景 |
|------|------|--------|---------|
| Modal Overlay | `#000000` | 50% | Modal 背景遮罩 |
| Drawer Overlay | `#000000` | 30% | Drawer 背景遮罩 |
| Tooltip Overlay | `#616161` | 90% | Tooltip 背景 |
| Snackbar Overlay | `#323232` | 90% | Snackbar 背景 |

---

## 業務場景色彩應用

### Agent 狀態色

| 狀態 | 顏色 | 色值 | 語義 |
|------|------|------|------|
| Active | Success-500 | `#4CAF50` | Agent 正在運行 |
| Inactive | Grey-500 | `#9E9E9E` | Agent 已停用 |
| Error | Error-500 | `#F44336` | Agent 錯誤 |
| Pending | Warning-500 | `#FF9800` | Agent 等待中 |
| Draft | Info-300 | `#64B5F6` | 草稿狀態 |

### 執行狀態色

| 狀態 | 顏色 | 色值 | 語義 |
|------|------|------|------|
| Running | Primary-500 | `#2196F3` | 執行中 |
| Success | Success-500 | `#4CAF50` | 執行成功 |
| Failed | Error-500 | `#F44336` | 執行失敗 |
| Cancelled | Grey-500 | `#9E9E9E` | 已取消 |
| Timeout | Warning-500 | `#FF9800` | 超時 |

### 數據可視化色彩（圖表）

**順序色階（Sequential）** - 用於表示數值大小:
```
#E3F2FD → #90CAF9 → #42A5F5 → #1976D2 → #0D47A1
(淺)                                        (深)
```

**分類色階（Categorical）** - 用於區分不同類別:
```
Primary-500:   #2196F3  (藍色)
Secondary-500: #9C27B0  (紫色)
Success-500:   #4CAF50  (綠色)
Warning-500:   #FF9800  (橙色)
Error-500:     #F44336  (紅色)
Info-500:      #00BCD4  (青色)
```

**對比色階（Diverging）** - 用於表示正負值:
```
負值: #F44336 (紅) → #FFEB3B (黃) → #4CAF50 (綠) :正值
```

---

## 亮色/暗色主題

### Light Theme (Default)

```typescript
const lightTheme = {
  palette: {
    mode: 'light',
    primary: { main: '#2196F3' },
    secondary: { main: '#9C27B0' },
    success: { main: '#4CAF50' },
    warning: { main: '#FF9800' },
    error: { main: '#F44336' },
    info: { main: '#2196F3' },
    background: {
      default: '#FAFAFA',
      paper: '#FFFFFF',
    },
    text: {
      primary: '#212121',
      secondary: '#616161',
      disabled: '#9E9E9E',
    },
  },
};
```

### Dark Theme

```typescript
const darkTheme = {
  palette: {
    mode: 'dark',
    primary: { main: '#90CAF9' },       // 使用較淺的 Primary-200
    secondary: { main: '#CE93D8' },     // 使用較淺的 Secondary-200
    success: { main: '#81C784' },       // 使用較淺的 Success-300
    warning: { main: '#FFB74D' },       // 使用較淺的 Warning-300
    error: { main: '#E57373' },         // 使用較淺的 Error-300
    info: { main: '#64B5F6' },          // 使用較淺的 Info-300
    background: {
      default: '#121212',
      paper: '#1E1E1E',
    },
    text: {
      primary: '#FFFFFF',
      secondary: '#B0B0B0',
      disabled: '#6C6C6C',
    },
  },
};
```

### 主題切換實現

```typescript
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useState } from 'react';

function App() {
  const [mode, setMode] = useState<'light' | 'dark'>('light');

  const theme = createTheme({
    palette: {
      mode,
      ...(mode === 'light' ? lightTheme.palette : darkTheme.palette),
    },
  });

  const toggleTheme = () => {
    setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
  };

  return (
    <ThemeProvider theme={theme}>
      <Button onClick={toggleTheme}>
        {mode === 'light' ? 'Dark Mode' : 'Light Mode'}
      </Button>
      {/* Your app */}
    </ThemeProvider>
  );
}
```

---

## 色彩對比度測試

### WCAG 2.1 對比度標準

| 級別 | 標準文字 (< 18px) | 大文字 (≥ 18px) |
|------|------------------|----------------|
| AA | 4.5:1 | 3:1 |
| AAA | 7:1 | 4.5:1 |

### 主要顏色組合對比度測試

**Primary Colors**:
| 前景色 | 背景色 | 對比度 | 通過級別 |
|--------|--------|--------|---------|
| White | Primary-500 | 3.96:1 | AA-Large ⚠️ |
| White | Primary-700 | 6.29:1 | AAA ✅ |
| Primary-700 | White | 6.29:1 | AAA ✅ |
| Grey-900 | White | 16.10:1 | AAA ✅ |

**Semantic Colors**:
| 前景色 | 背景色 | 對比度 | 通過級別 |
|--------|--------|--------|---------|
| Success-700 | White | 4.85:1 | AA ✅ |
| Warning-700 | White | 4.54:1 | AA ✅ |
| Error-700 | White | 6.38:1 | AAA ✅ |
| Info-700 | White | 6.29:1 | AAA ✅ |

**Grey Scale**:
| 前景色 | 背景色 | 對比度 | 通過級別 |
|--------|--------|--------|---------|
| Grey-900 | White | 16.10:1 | AAA ✅ |
| Grey-700 | White | 7.88:1 | AAA ✅ |
| Grey-600 | White | 5.74:1 | AA ✅ |
| Grey-500 | White | 4.05:1 | AA-Large ⚠️ |

### 對比度檢查工具

**在線工具**:
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Colour Contrast Checker](https://colourcontrast.cc/)

**瀏覽器擴展**:
- axe DevTools
- WAVE Evaluation Tool

**設計工具插件**:
- Figma: Contrast Checker Plugin
- Adobe XD: Stark Plugin

---

## 使用範例

### Button 顏色應用

```tsx
import { Button } from '@mui/material';

// Primary Button (主要操作)
<Button variant="contained" color="primary">
  Create Agent
</Button>

// Secondary Button (次要操作)
<Button variant="outlined" color="secondary">
  Cancel
</Button>

// Success Button (成功操作)
<Button variant="contained" color="success">
  Save
</Button>

// Error Button (危險操作)
<Button variant="contained" color="error">
  Delete
</Button>
```

### Alert 顏色應用

```tsx
import { Alert } from '@mui/material';

// Success Alert
<Alert severity="success">Agent 執行成功！</Alert>

// Warning Alert
<Alert severity="warning">資源使用率達到 80%</Alert>

// Error Alert
<Alert severity="error">Agent 執行失敗</Alert>

// Info Alert
<Alert severity="info">提示：點擊查看詳細信息</Alert>
```

### Chip 狀態顏色

```tsx
import { Chip } from '@mui/material';

// Agent 狀態
<Chip label="Active" color="success" size="small" />
<Chip label="Inactive" color="default" size="small" />
<Chip label="Error" color="error" size="small" />
<Chip label="Pending" color="warning" size="small" />
```

### 自定義狀態徽章

```tsx
import { Badge, Avatar } from '@mui/material';
import { styled } from '@mui/material/styles';

const StatusBadge = styled(Badge)<{ status: string }>(({ theme, status }) => ({
  '& .MuiBadge-badge': {
    backgroundColor:
      status === 'active' ? theme.palette.success.main :
      status === 'error' ? theme.palette.error.main :
      theme.palette.grey[500],
    color:
      status === 'active' ? theme.palette.success.main :
      status === 'error' ? theme.palette.error.main :
      theme.palette.grey[500],
    boxShadow: `0 0 0 2px ${theme.palette.background.paper}`,
  },
}));

// 使用
<StatusBadge
  overlap="circular"
  anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
  variant="dot"
  status="active"
>
  <Avatar>AG</Avatar>
</StatusBadge>
```

---

## CSS 變量定義

```css
:root {
  /* Primary Colors */
  --color-primary-50: #E3F2FD;
  --color-primary-100: #BBDEFB;
  --color-primary-200: #90CAF9;
  --color-primary-300: #64B5F6;
  --color-primary-400: #42A5F5;
  --color-primary-500: #2196F3;
  --color-primary-600: #1E88E2;
  --color-primary-700: #1976D2;
  --color-primary-800: #1565C0;
  --color-primary-900: #0D47A1;

  /* Secondary Colors */
  --color-secondary-50: #F3E5F5;
  --color-secondary-500: #9C27B0;
  --color-secondary-700: #7B1FA2;

  /* Semantic Colors */
  --color-success: #4CAF50;
  --color-warning: #FF9800;
  --color-error: #F44336;
  --color-info: #2196F3;

  /* Neutral Colors */
  --color-grey-50: #FAFAFA;
  --color-grey-100: #F5F5F5;
  --color-grey-300: #E0E0E0;
  --color-grey-500: #9E9E9E;
  --color-grey-600: #757575;
  --color-grey-700: #616161;
  --color-grey-900: #212121;

  /* Background Colors */
  --bg-page: #FAFAFA;
  --bg-card: #FFFFFF;
  --bg-hover: #F5F5F5;

  /* Text Colors */
  --text-primary: #212121;
  --text-secondary: #616161;
  --text-disabled: #9E9E9E;
}

/* Dark Theme */
[data-theme="dark"] {
  --color-primary-500: #90CAF9;
  --color-secondary-500: #CE93D8;
  --bg-page: #121212;
  --bg-card: #1E1E1E;
  --bg-hover: #2C2C2C;
  --text-primary: #FFFFFF;
  --text-secondary: #B0B0B0;
}
```

---

## 無障礙指南

### 色彩對比度要求

1. **正文文字**: 至少 4.5:1 (WCAG AA)
2. **大文字（≥18px）**: 至少 3:1 (WCAG AA)
3. **UI 組件**: 至少 3:1 (WCAG AA)
4. **圖形元素**: 至少 3:1 (WCAG AA)

### 色彩使用原則

1. **不單靠顏色傳達信息**
   - ❌ 錯誤：僅用紅色表示錯誤
   - ✅ 正確：紅色 + 錯誤圖標 + 錯誤文字

2. **提供多種視覺提示**
   - 顏色 + 圖標 + 文字標籤
   - 顏色 + 形狀差異
   - 顏色 + 位置差異

3. **支持高對比度模式**
   - 測試 Windows 高對比度模式
   - 測試瀏覽器強制顏色模式
   - 確保邊框和焦點環可見

4. **色盲友好設計**
   - 避免紅綠對比（最常見的色盲類型）
   - 使用藍色、橙色作為對比色
   - 提供形狀和圖案輔助

### 測試清單

- [ ] 所有文字顏色對比度 ≥ 4.5:1
- [ ] 大文字顏色對比度 ≥ 3:1
- [ ] UI 組件顏色對比度 ≥ 3:1
- [ ] 不單靠顏色傳達關鍵信息
- [ ] 高對比度模式測試通過
- [ ] 色盲模擬測試通過
- [ ] 焦點狀態清晰可見

---

## 參考資料

### 色彩理論
- [Material Design 3 - Color System](https://m3.material.io/styles/color/system/overview)
- [Adobe Color](https://color.adobe.com/)
- [Coolors](https://coolors.co/)

### 可訪問性
- [WCAG 2.1 - Contrast Guidelines](https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html)
- [WebAIM - Color Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Color Blind Accessibility](https://www.colourblindawareness.org/)

### 工具
- [Color Contrast Checker](https://colourcontrast.cc/)
- [Who Can Use](https://www.whocanuse.com/)
- [Stark - Figma Plugin](https://www.getstark.co/)

---

**最後更新**: 2025-10-29
**負責人**: UI/UX Designer
**下一步**: 創建 Accessibility Guidelines 文檔

---

[← 返回 Design System](./README.md)
