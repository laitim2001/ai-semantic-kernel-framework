# 無障礙指南（Accessibility Guidelines）

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成
**標準**: WCAG 2.1 AA

[← 返回 Design System](./README.md)

---

## 📋 概述

本文檔基於 **WCAG 2.1 Level AA** 標準，提供完整的無障礙設計和實施指南，確保 Semantic Kernel Agentic Framework 能被所有用戶（包括殘障人士）順利使用。

### 為什麼無障礙很重要？

- **法律合規**: 許多國家和地區要求數字產品符合無障礙標準
- **用戶覆蓋**: 全球約 15% 人口有某種形式的殘障
- **更好的 UX**: 無障礙設計通常意味著更好的整體用戶體驗
- **SEO 優化**: 無障礙改進有助於搜索引擎優化
- **品質標誌**: 展現對用戶和品質的承諾

---

## WCAG 2.1 四大原則

### 1. 可感知 (Perceivable)

用戶必須能感知界面中的信息和組件。

**關鍵要求**:
- 提供替代文字（alt text）
- 提供字幕和文字稿
- 確保充足的顏色對比度
- 支持文字調整和縮放

### 2. 可操作 (Operable)

用戶必須能操作界面組件和導航。

**關鍵要求**:
- 鍵盤可訪問性
- 充足的時間完成任務
- 避免可能引發癲癇的內容
- 提供導航和尋找內容的方式

### 3. 可理解 (Understandable)

信息和界面操作必須可理解。

**關鍵要求**:
- 可讀且可理解的文字
- 可預測的界面行為
- 幫助用戶避免和更正錯誤

### 4. 穩健 (Robust)

內容必須能被各種用戶代理（包括輔助技術）可靠解析。

**關鍵要求**:
- 有效的 HTML 標記
- ARIA 標籤正確使用
- 兼容輔助技術

---

## 1. 可感知性 (Perceivable)

### 1.1 文字替代 (Text Alternatives)

#### 圖片替代文字

**原則**: 所有非文字內容必須有文字替代。

```tsx
// ✅ 正確：有意義的 alt text
<img src="agent-icon.png" alt="Customer Service Agent icon" />

// ✅ 正確：裝飾性圖片使用空 alt
<img src="decoration.png" alt="" role="presentation" />

// ❌ 錯誤：缺少 alt text
<img src="agent-icon.png" />

// ❌ 錯誤：無意義的 alt text
<img src="agent-icon.png" alt="image" />
```

**替代文字撰寫原則**:
- 描述圖片內容和功能
- 保持簡潔（<150 字符）
- 不使用 "image of" 或 "picture of"
- 裝飾性圖片使用空 alt (`alt=""`)

#### 圖標按鈕

```tsx
import { IconButton, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';

// ✅ 正確：有 aria-label
<IconButton aria-label="Delete agent" onClick={handleDelete}>
  <DeleteIcon />
</IconButton>

// ✅ 更好：有 Tooltip 提示
<Tooltip title="Delete agent">
  <IconButton aria-label="Delete agent" onClick={handleDelete}>
    <DeleteIcon />
  </IconButton>
</Tooltip>
```

### 1.2 色彩對比度 (Color Contrast)

#### WCAG AA 標準要求

| 內容類型 | 最小對比度 | 推薦對比度 |
|---------|-----------|-----------|
| 正常文字 (<18px) | 4.5:1 | 7:1 (AAA) |
| 大文字 (≥18px 或 ≥14px bold) | 3:1 | 4.5:1 (AAA) |
| UI 組件 | 3:1 | - |
| 圖形元素 | 3:1 | - |

#### 顏色對比度最佳實踐

```tsx
// ✅ 正確：高對比度文字
<Typography color="text.primary">  {/* Grey-900 on White: 16.10:1 */}
  Primary text
</Typography>

// ✅ 正確：次要文字
<Typography color="text.secondary"> {/* Grey-700 on White: 7.88:1 */}
  Secondary text
</Typography>

// ⚠️ 警告：對比度不足，僅用於大文字
<Typography variant="h1" color="primary.main"> {/* 3.96:1 */}
  Large Title
</Typography>

// ❌ 錯誤：對比度過低
<Typography sx={{ color: '#BDBDBD' }}> {/* 2.4:1 - 不符合 AA */}
  Low contrast text
</Typography>
```

#### 不依賴顏色傳達信息

```tsx
// ❌ 錯誤：僅使用顏色
<Typography color="error">Error occurred</Typography>

// ✅ 正確：顏色 + 圖標 + 文字
<Box display="flex" alignItems="center" color="error.main">
  <ErrorIcon sx={{ mr: 1 }} />
  <Typography>Error: Operation failed</Typography>
</Box>

// ✅ 正確：表單驗證
<TextField
  error={hasError}
  helperText={hasError && "This field is required"}  {/* 文字說明 */}
  InputProps={{
    startAdornment: hasError && <ErrorIcon color="error" />  {/* 圖標 */}
  }}
/>
```

### 1.3 可調整文字 (Resizable Text)

**要求**: 用戶應能調整文字大小至 200% 而不損失內容或功能。

```css
/* ✅ 正確：使用 rem 單位 */
body {
  font-size: 0.875rem;  /* 14px，可縮放 */
}

/* ❌ 錯誤：使用固定 px */
body {
  font-size: 14px;  /* 不支持瀏覽器縮放 */
}

/* ✅ 正確：避免固定高度 */
.container {
  min-height: 3rem;  /* 使用 min-height 而非 height */
}
```

### 1.4 多媒體替代方案

#### 影片和音頻

- **字幕 (Captions)**: 為影片提供同步字幕
- **文字稿 (Transcripts)**: 為音頻提供文字版本
- **音頻描述**: 為視覺內容提供音頻說明

```tsx
// ✅ 正確：有字幕軌道的影片
<video controls>
  <source src="tutorial.mp4" type="video/mp4" />
  <track kind="captions" src="captions.vtt" srclang="zh" label="繁體中文" default />
  <track kind="captions" src="captions-en.vtt" srclang="en" label="English" />
</video>
```

---

## 2. 可操作性 (Operable)

### 2.1 鍵盤可訪問性 (Keyboard Accessibility)

**核心原則**: 所有功能必須能通過鍵盤操作。

#### 鍵盤導航標準

| 按鍵 | 功能 |
|------|------|
| `Tab` | 移到下一個可聚焦元素 |
| `Shift + Tab` | 移到上一個可聚焦元素 |
| `Enter` / `Space` | 激活按鈕或連結 |
| `Escape` | 關閉 Dialog、Menu |
| `Arrow Keys` | 在選項、標籤頁間移動 |
| `Home` / `End` | 跳到列表開頭/結尾 |

#### 焦點管理

```tsx
import { useRef, useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent } from '@mui/material';

function AccessibleDialog({ open, onClose }: Props) {
  const titleRef = useRef<HTMLElement>(null);

  // ✅ 正確：Dialog 打開時，焦點移到標題
  useEffect(() => {
    if (open && titleRef.current) {
      titleRef.current.focus();
    }
  }, [open]);

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle ref={titleRef} tabIndex={-1}>
        Delete Agent
      </DialogTitle>
      <DialogContent>
        Are you sure you want to delete this agent?
      </DialogContent>
    </Dialog>
  );
}
```

#### 焦點陷阱 (Focus Trap)

```tsx
// ✅ Material-UI Dialog 自動處理焦點陷阱
<Dialog open={open} onClose={onClose}>
  {/* 焦點會被限制在 Dialog 內 */}
  <DialogContent>...</DialogContent>
</Dialog>

// ⚠️ 自定義組件需要手動實現焦點陷阱
import { FocusTrap } from '@mui/base';

<FocusTrap open={isOpen}>
  <div>
    <button>First focusable</button>
    <button>Last focusable</button>
  </div>
</FocusTrap>
```

#### 跳過導航連結 (Skip Links)

```tsx
// ✅ 正確：提供跳過導航的連結
<Box
  component="a"
  href="#main-content"
  sx={{
    position: 'absolute',
    left: '-9999px',
    '&:focus': {
      position: 'static',
      left: 'auto',
    }
  }}
>
  Skip to main content
</Box>

<main id="main-content" tabIndex={-1}>
  {/* 主要內容 */}
</main>
```

### 2.2 充足的時間 (Enough Time)

#### 會話超時警告

```tsx
import { useState, useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, Button } from '@mui/material';

function SessionTimeoutWarning() {
  const [showWarning, setShowWarning] = useState(false);
  const [timeLeft, setTimeLeft] = useState(60);

  // ✅ 正確：超時前 2 分鐘警告用戶
  useEffect(() => {
    const warningTimer = setTimeout(() => {
      setShowWarning(true);
    }, 18 * 60 * 1000); // 18 分鐘後警告（總共 20 分鐘會話）

    return () => clearTimeout(warningTimer);
  }, []);

  const handleExtendSession = () => {
    // 延長會話
    setShowWarning(false);
  };

  return (
    <Dialog open={showWarning}>
      <DialogTitle>Session Expiring</DialogTitle>
      <DialogContent>
        Your session will expire in {timeLeft} seconds.
        Would you like to continue?
      </DialogContent>
      <Button onClick={handleExtendSession}>Continue Session</Button>
    </Dialog>
  );
}
```

#### 自動更新內容

```tsx
// ✅ 正確：提供暫停/播放控制
import { useState } from 'react';
import { IconButton } from '@mui/material';
import PauseIcon from '@mui/icons-material/Pause';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';

function AutoRefreshingList() {
  const [isPaused, setIsPaused] = useState(false);

  return (
    <>
      <IconButton
        aria-label={isPaused ? "Resume auto-refresh" : "Pause auto-refresh"}
        onClick={() => setIsPaused(!isPaused)}
      >
        {isPaused ? <PlayArrowIcon /> : <PauseIcon />}
      </IconButton>
      {/* 列表內容 */}
    </>
  );
}
```

### 2.3 避免觸發癲癇 (Seizures and Physical Reactions)

**要求**: 避免每秒閃爍超過 3 次的內容。

```tsx
// ❌ 錯誤：快速閃爍動畫
<Box
  sx={{
    animation: 'blink 0.2s infinite',  // 每秒 5 次，超過安全閾值
  }}
/>

// ✅ 正確：緩慢動畫
<Box
  sx={{
    animation: 'pulse 2s ease-in-out infinite',  // 安全的動畫速度
  }}
/>
```

### 2.4 導航 (Navigable)

#### 頁面標題

```tsx
import { Helmet } from 'react-helmet-async';

// ✅ 正確：每個頁面有描述性標題
<Helmet>
  <title>Agent Detail - Customer Service Bot | SK Framework</title>
</Helmet>
```

#### 焦點順序

```tsx
// ✅ 正確：邏輯的 Tab 順序（從左到右，從上到下）
<form>
  <TextField label="Name" />      {/* Tab 1 */}
  <TextField label="Email" />     {/* Tab 2 */}
  <TextField label="Message" />   {/* Tab 3 */}
  <Button type="submit">Send</Button>  {/* Tab 4 */}
</form>

// ❌ 錯誤：使用 tabIndex 破壞自然順序
<Button tabIndex={5}>First Button</Button>
<Button tabIndex={1}>Second Button</Button>  {/* 不要這樣做！ */}
```

#### 連結目的

```tsx
// ❌ 錯誤：模糊的連結文字
<Link href="/agents/123">Click here</Link>

// ✅ 正確：描述性連結文字
<Link href="/agents/123">View Customer Service Agent details</Link>

// ✅ 正確：使用 aria-label 補充
<Link href="/agents/123" aria-label="View Customer Service Agent details">
  View details
</Link>
```

---

## 3. 可理解性 (Understandable)

### 3.1 可讀性 (Readable)

#### 語言聲明

```html
<!-- ✅ 正確：聲明頁面主要語言 -->
<html lang="zh-Hant">

<!-- ✅ 正確：標記不同語言的部分 -->
<p>歡迎使用 <span lang="en">Semantic Kernel</span></p>
```

#### 清晰的錯誤消息

```tsx
// ❌ 錯誤：技術性錯誤消息
<Alert severity="error">
  Error: ERR_CONNECTION_REFUSED
</Alert>

// ✅ 正確：用戶友好的錯誤消息
<Alert severity="error">
  <AlertTitle>Connection Failed</AlertTitle>
  Unable to connect to the server. Please check your internet connection and try again.
</Alert>
```

### 3.2 可預測性 (Predictable)

#### 一致的導航

```tsx
// ✅ 正確：所有頁面使用一致的導航結構
<AppBar position="static">
  <Toolbar>
    <Typography variant="h6">SK Framework</Typography>
    <Button color="inherit" href="/dashboard">Dashboard</Button>
    <Button color="inherit" href="/agents">Agents</Button>
    <Button color="inherit" href="/settings">Settings</Button>
  </Toolbar>
</AppBar>
```

#### 避免自動更改焦點

```tsx
// ❌ 錯誤：輸入時自動跳到下一個欄位
<TextField
  onChange={(e) => {
    if (e.target.value.length === 4) {
      nextInputRef.current?.focus();  // 不要自動移動焦點
    }
  }}
/>

// ✅ 正確：讓用戶控制焦點移動
<TextField
  onKeyDown={(e) => {
    if (e.key === 'Enter' && e.target.value.length === 4) {
      nextInputRef.current?.focus();  // 只在 Enter 時移動
    }
  }}
/>
```

### 3.3 輸入協助 (Input Assistance)

#### 表單標籤和說明

```tsx
// ✅ 正確：清晰的標籤和說明
<TextField
  id="agent-name"
  label="Agent Name"
  required
  helperText="Enter a unique name for your agent (3-50 characters)"
  error={hasError}
  aria-describedby="name-helper-text"
/>

// ✅ 正確：必填欄位標記
<TextField
  required
  label="Email Address"
  InputProps={{
    'aria-required': true,
  }}
/>
```

#### 錯誤識別和建議

```tsx
import { Alert, TextField } from '@mui/material';

// ✅ 正確：清晰的錯誤識別和修正建議
<>
  {submitError && (
    <Alert severity="error" role="alert">
      <AlertTitle>Validation Error</AlertTitle>
      <ul>
        <li>Email address is invalid. Please use format: user@example.com</li>
        <li>Password must be at least 8 characters long</li>
      </ul>
    </Alert>
  )}

  <TextField
    error={emailError}
    label="Email"
    helperText={emailError && "Please enter a valid email address"}
    aria-invalid={emailError}
    aria-describedby={emailError ? "email-error" : undefined}
  />
</>
```

#### 確認對話框

```tsx
// ✅ 正確：危險操作前確認
<Dialog open={confirmDelete} onClose={handleCancel}>
  <DialogTitle>Confirm Delete</DialogTitle>
  <DialogContent>
    <Alert severity="warning">
      This action cannot be undone. Are you sure you want to delete the agent "Customer Service Bot"?
    </Alert>
  </DialogContent>
  <DialogActions>
    <Button onClick={handleCancel}>Cancel</Button>
    <Button onClick={handleDelete} color="error" variant="contained">
      Delete Agent
    </Button>
  </DialogActions>
</Dialog>
```

---

## 4. 穩健性 (Robust)

### 4.1 ARIA 標籤和角色 (ARIA)

#### ARIA 基本原則

1. **第一規則**: 如果可以使用原生 HTML 元素，不要使用 ARIA
2. **不改變語義**: ARIA 不改變元素行為，只改變輔助技術的解釋
3. **鍵盤支持**: 使用 ARIA 角色的元素必須支持鍵盤操作

#### 常用 ARIA 屬性

```tsx
// ✅ aria-label: 提供元素標籤
<IconButton aria-label="Delete agent">
  <DeleteIcon />
</IconButton>

// ✅ aria-labelledby: 引用其他元素作為標籤
<Dialog aria-labelledby="dialog-title">
  <DialogTitle id="dialog-title">Delete Agent</DialogTitle>
</Dialog>

// ✅ aria-describedby: 提供額外描述
<TextField
  aria-describedby="password-helper"
  helperText={<span id="password-helper">Must be 8+ characters</span>}
/>

// ✅ aria-live: 動態內容更新通知
<Box aria-live="polite" aria-atomic="true">
  {successMessage}
</Box>

// ✅ aria-expanded: 展開/收起狀態
<Button
  aria-expanded={isExpanded}
  aria-controls="panel-content"
  onClick={togglePanel}
>
  Show Details
</Button>

// ✅ aria-current: 當前導航項
<Button
  href="/dashboard"
  aria-current={currentPage === 'dashboard' ? 'page' : undefined}
>
  Dashboard
</Button>
```

#### ARIA 角色

```tsx
// ✅ role="alert": 重要消息
<Box role="alert" aria-live="assertive">
  Error: Connection lost
</Box>

// ✅ role="status": 狀態更新
<Box role="status" aria-live="polite">
  Saving...
</Box>

// ✅ role="navigation": 導航區域
<Box component="nav" role="navigation" aria-label="Main navigation">
  {/* 導航連結 */}
</Box>

// ✅ role="search": 搜索區域
<Box component="form" role="search">
  <TextField label="Search agents" />
</Box>
```

### 4.2 語義化 HTML (Semantic HTML)

```tsx
// ✅ 正確：使用語義化標籤
<header>
  <nav aria-label="Main navigation">
    <ul>
      <li><a href="/">Home</a></li>
    </ul>
  </nav>
</header>

<main>
  <article>
    <h1>Agent Details</h1>
    <section>
      <h2>Configuration</h2>
      {/* 內容 */}
    </section>
  </article>
</main>

<footer>
  <p>&copy; 2025 SK Framework</p>
</footer>

// ❌ 錯誤：全部使用 div
<div class="header">
  <div class="nav">...</div>
</div>
<div class="main">
  <div class="article">...</div>
</div>
```

### 4.3 表單可訪問性

```tsx
// ✅ 正確：完整的表單可訪問性
<form onSubmit={handleSubmit} aria-label="Create new agent">
  <FormControl fullWidth margin="normal">
    <InputLabel htmlFor="agent-name">Agent Name *</InputLabel>
    <TextField
      id="agent-name"
      name="name"
      required
      aria-required="true"
      aria-invalid={errors.name ? 'true' : 'false'}
      aria-describedby="name-helper"
      error={!!errors.name}
    />
    <FormHelperText id="name-helper" error={!!errors.name}>
      {errors.name || "Enter a unique name (3-50 characters)"}
    </FormHelperText>
  </FormControl>

  <FormControl fullWidth margin="normal">
    <InputLabel id="model-label">Model *</InputLabel>
    <Select
      labelId="model-label"
      id="model"
      value={model}
      onChange={handleModelChange}
      aria-labelledby="model-label"
      required
    >
      <MenuItem value="gpt-4">GPT-4</MenuItem>
      <MenuItem value="gpt-3.5">GPT-3.5</MenuItem>
    </Select>
  </FormControl>

  <Button type="submit" variant="contained">
    Create Agent
  </Button>
</form>
```

---

## 測試清單

### 自動化測試工具

| 工具 | 用途 | 使用方式 |
|------|------|---------|
| **axe DevTools** | 瀏覽器擴展 | 自動檢測可訪問性問題 |
| **WAVE** | 瀏覽器擴展 | 視覺化可訪問性報告 |
| **Lighthouse** | Chrome DevTools | 可訪問性評分和建議 |
| **Jest + jest-axe** | 單元測試 | 組件可訪問性測試 |
| **Pa11y** | CI/CD | 自動化可訪問性測試 |

### 手動測試清單

#### ✅ 鍵盤導航測試

- [ ] 所有互動元素可用 Tab 訪問
- [ ] Tab 順序符合邏輯
- [ ] 焦點狀態清晰可見
- [ ] Enter/Space 可激活按鈕
- [ ] Escape 可關閉 Dialog/Menu
- [ ] Arrow 鍵可在選項間移動

#### ✅ 屏幕閱讀器測試

推薦工具:
- **NVDA** (Windows, 免費)
- **JAWS** (Windows, 付費)
- **VoiceOver** (macOS/iOS, 內建)
- **TalkBack** (Android, 內建)

測試清單:
- [ ] 圖片有替代文字
- [ ] 表單欄位有標籤
- [ ] 錯誤消息被正確讀出
- [ ] 動態內容更新被通知
- [ ] 頁面結構清晰（標題層級）

#### ✅ 視覺測試

- [ ] 顏色對比度 ≥ 4.5:1（正常文字）
- [ ] 顏色對比度 ≥ 3:1（大文字、UI 組件）
- [ ] 不僅依賴顏色傳達信息
- [ ] 焦點指示器清晰可見
- [ ] 文字可縮放至 200%

#### ✅ 移動設備測試

- [ ] 觸控目標至少 44×44px
- [ ] 支持螢幕旋轉
- [ ] 縮放功能正常
- [ ] 觸控操作無需精確點擊

### 自動化測試範例

```typescript
// Jest + jest-axe
import { render } from '@testing-library/react';
import { axe, toHaveNoViolations } from 'jest-axe';
import AgentCard from './AgentCard';

expect.extend(toHaveNoViolations);

test('AgentCard should have no accessibility violations', async () => {
  const { container } = render(<AgentCard agent={mockAgent} />);
  const results = await axe(container);
  expect(results).toHaveNoViolations();
});

// Lighthouse CI
// lighthouserc.js
module.exports = {
  ci: {
    collect: {
      url: ['http://localhost:3000'],
    },
    assert: {
      assertions: {
        'categories:accessibility': ['error', { minScore: 0.9 }],
      },
    },
  },
};
```

---

## 實施優先級

### 🔴 高優先級（必須實施）

1. **鍵盤可訪問性**
   - 所有功能可用鍵盤操作
   - 焦點管理正確
   - 焦點指示器清晰

2. **替代文字**
   - 所有圖片有 alt text
   - 圖標按鈕有 aria-label

3. **顏色對比度**
   - 文字顏色對比度 ≥ 4.5:1
   - UI 組件對比度 ≥ 3:1

4. **表單標籤**
   - 所有表單欄位有標籤
   - 錯誤消息清晰

### 🟡 中優先級（應該實施）

5. **語義化 HTML**
   - 使用適當的 HTML 標籤
   - 標題層級正確

6. **ARIA 標籤**
   - 動態內容有 aria-live
   - 複雜組件有適當 ARIA

7. **響應式設計**
   - 文字可縮放
   - 觸控目標足夠大

### 🟢 低優先級（建議實施）

8. **屏幕閱讀器優化**
   - 跳過導航連結
   - landmark 角色

9. **高級 ARIA**
   - aria-describedby
   - aria-controls

---

## 常見錯誤和解決方案

### 錯誤 1: 缺少替代文字

```tsx
// ❌ 錯誤
<img src="logo.png" />

// ✅ 正確
<img src="logo.png" alt="SK Framework Logo" />
```

### 錯誤 2: 對比度不足

```tsx
// ❌ 錯誤：對比度 2.5:1
<Typography sx={{ color: '#999' }}>  {/* Grey-500 */}
  Low contrast text
</Typography>

// ✅ 正確：對比度 7.88:1
<Typography color="text.secondary">  {/* Grey-700 */}
  Sufficient contrast text
</Typography>
```

### 錯誤 3: 僅使用顏色

```tsx
// ❌ 錯誤
<span style={{ color: 'red' }}>Error</span>

// ✅ 正確
<Box display="flex" alignItems="center" color="error.main">
  <ErrorIcon sx={{ mr: 0.5 }} />
  <Typography>Error</Typography>
</Box>
```

### 錯誤 4: 鍵盤無法訪問

```tsx
// ❌ 錯誤：div 沒有鍵盤支持
<div onClick={handleClick}>Click me</div>

// ✅ 正確：使用按鈕
<Button onClick={handleClick}>Click me</Button>

// ✅ 正確：如果必須使用 div，添加鍵盤支持
<div
  role="button"
  tabIndex={0}
  onClick={handleClick}
  onKeyDown={(e) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      handleClick();
    }
  }}
>
  Click me
</div>
```

### 錯誤 5: 缺少表單標籤

```tsx
// ❌ 錯誤：沒有標籤
<input type="text" placeholder="Name" />

// ✅ 正確：有標籤
<TextField
  id="name"
  label="Name"
  placeholder="Enter your name"
/>
```

---

## 持續改進

### 定期審查

- **每季度**: 進行完整可訪問性審核
- **每次 Release**: 運行自動化測試
- **新功能**: 完成前必須通過可訪問性測試

### 培訓和文化

- 定期團隊培訓
- 可訪問性作為代碼審查標準
- 分享最佳實踐和案例

### 用戶反饋

- 提供可訪問性反饋渠道
- 記錄和追蹤用戶報告的問題
- 優先修復影響用戶的問題

---

## 參考資料

### 標準和指南
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [WAI-ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)
- [Material Design Accessibility](https://m3.material.io/foundations/accessible-design/overview)

### 工具
- [axe DevTools](https://www.deque.com/axe/devtools/)
- [WAVE](https://wave.webaim.org/)
- [Lighthouse](https://developers.google.com/web/tools/lighthouse)
- [Color Contrast Checker](https://webaim.org/resources/contrastchecker/)

### 學習資源
- [WebAIM](https://webaim.org/)
- [A11y Project](https://www.a11yproject.com/)
- [Inclusive Components](https://inclusive-components.design/)
- [Deque University](https://dequeuniversity.com/)

---

**最後更新**: 2025-10-29
**負責人**: UI/UX Designer
**下一步**: 更新 Design System README 總覽

---

[← 返回 Design System](./README.md)
