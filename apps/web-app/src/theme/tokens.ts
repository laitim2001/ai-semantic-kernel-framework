/**
 * Design Tokens - 設計變量系統
 * 基於 US-6.1-UI-DESIGN-SPECIFICATION.md
 * 參考: ChatGPT, Claude AI, Material Design 3
 */

// ============================================
// Color Palette
// ============================================

export const colors = {
  // Primary Colors (主色調)
  primary: {
    main: '#1976d2',      // MUI Blue 700 - 主要操作按鈕
    light: '#42a5f5',     // MUI Blue 400 - Hover 狀態
    dark: '#1565c0',      // MUI Blue 800 - Active 狀態
    contrastText: '#fff'  // 按鈕文字顏色
  },

  // Secondary Colors (次要色)
  secondary: {
    main: '#9c27b0',      // MUI Purple 500 - 次要操作
    light: '#ba68c8',     // MUI Purple 300 - Hover
    dark: '#7b1fa2',      // MUI Purple 700 - Active
    contrastText: '#fff'
  },

  // Message Colors (訊息顏色)
  message: {
    user: {
      background: '#1976d2',              // 用戶訊息背景 (藍色)
      text: '#ffffff',                    // 用戶訊息文字 (白色)
      timestamp: 'rgba(255, 255, 255, 0.7)'  // 時間戳 (半透明白)
    },
    agent: {
      background: '#f5f5f5',              // Agent 訊息背景 (淺灰)
      text: '#212121',                    // Agent 訊息文字 (深灰)
      timestamp: 'rgba(0, 0, 0, 0.5)'     // 時間戳 (半透明黑)
    }
  },

  // Neutral Colors (中性色)
  background: {
    default: '#ffffff',     // 頁面背景 (白色)
    paper: '#f9f9f9',       // Sidebar 背景 (極淺灰)
    chat: '#ffffff',        // 對話區域背景 (白色)
    input: '#f5f5f5'        // 輸入框背景 (淺灰)
  },

  text: {
    primary: '#212121',     // 主要文字 (深灰)
    secondary: '#757575',   // 次要文字 (中灰)
    disabled: '#bdbdbd',    // 禁用文字 (淺灰)
    hint: '#9e9e9e'         // 提示文字 (淺灰)
  },

  divider: '#e0e0e0',       // 分隔線 (淺灰)
  border: '#e0e0e0',        // 邊框 (淺灰)

  // Status Colors (狀態色)
  status: {
    success: '#4caf50',     // 成功 (綠色)
    warning: '#ff9800',     // 警告 (橙色)
    error: '#f44336',       // 錯誤 (紅色)
    info: '#2196f3',        // 資訊 (藍色)
    streaming: '#1976d2'    // Streaming 狀態 (藍色)
  }
} as const;

// ============================================
// Spacing Scale (間距系統)
// ============================================

export const spacing = {
  xs: 4,    // 0.25rem - 極小間距 (icon padding)
  sm: 8,    // 0.5rem  - 小間距 (button padding)
  md: 16,   // 1rem    - 中間距 (component margin)
  lg: 24,   // 1.5rem  - 大間距 (section padding)
  xl: 32,   // 2rem    - 超大間距 (page padding)
  xxl: 48   // 3rem    - 極大間距 (header height)
} as const;

// ============================================
// Typography (字體系統)
// ============================================

export const typography = {
  fontFamily: {
    base: '"Roboto", "Noto Sans TC", "Microsoft JhengHei", sans-serif',
    code: '"Fira Code", "Consolas", "Monaco", monospace'
  },

  fontSize: {
    xs: '0.75rem',    // 12px - 時間戳、字數統計
    sm: '0.875rem',   // 14px - 次要文字
    md: '1rem',       // 16px - 主要文字 (訊息內容)
    lg: '1.125rem',   // 18px - 標題
    xl: '1.25rem',    // 20px - 大標題
    xxl: '1.5rem'     // 24px - 頁面標題
  },

  fontWeight: {
    regular: 400,     // 一般文字
    medium: 500,      // 按鈕、強調文字
    semibold: 600,    // 標題
    bold: 700         // 重要強調
  },

  lineHeight: {
    tight: 1.2,       // 標題行高
    normal: 1.5,      // 正文行高
    relaxed: 1.75     // 段落行高
  }
} as const;

// ============================================
// Border Radius (圓角系統)
// ============================================

export const borderRadius = {
  none: 0,           // 無圓角
  sm: '4px',         // 小圓角 (按鈕、輸入框)
  md: '8px',         // 中圓角 (Card、訊息氣泡)
  lg: '12px',        // 大圓角 (對話容器)
  xl: '16px',        // 超大圓角 (特殊容器)
  full: '9999px'     // 完全圓角 (圓形按鈕)
} as const;

// ============================================
// Shadows (陰影系統)
// ============================================

export const shadows = {
  none: 'none',
  sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',           // 微陰影 (輸入框)
  md: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',         // 中陰影 (Card)
  lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',       // 大陰影 (Modal)
  xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1)',       // 超大陰影 (Dropdown)
  inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.06)'   // 內陰影 (Active input)
} as const;

// ============================================
// Animation (動畫系統)
// ============================================

export const animation = {
  duration: {
    fast: '150ms',      // 快速動畫 (Hover 效果)
    normal: '250ms',    // 標準動畫 (Transition)
    slow: '350ms',      // 慢速動畫 (Modal 進出)
    typing: '20ms'      // 打字機效果 (每個字符)
  },

  easing: {
    easeInOut: 'cubic-bezier(0.4, 0, 0.2, 1)',     // 標準緩動
    easeOut: 'cubic-bezier(0.0, 0, 0.2, 1)',       // 出場緩動
    easeIn: 'cubic-bezier(0.4, 0, 1, 1)',          // 入場緩動
    sharp: 'cubic-bezier(0.4, 0, 0.6, 1)'          // 銳利緩動
  }
} as const;

// ============================================
// Layout Sizes (佈局尺寸)
// ============================================

export const layoutSizes = {
  sidebar: {
    width: 280,         // Sidebar 固定寬度
    minWidth: 240,      // 最小寬度 (響應式收縮)
    maxWidth: 320       // 最大寬度 (響應式放大)
  },

  chatHeader: {
    height: 64          // Header 固定高度
  },

  chatInput: {
    minHeight: 56,      // 輸入框最小高度 (單行)
    maxHeight: 200,     // 輸入框最大高度 (多行滾動)
    padding: 16         // 輸入框內邊距
  },

  message: {
    maxWidth: 720,      // 訊息氣泡最大寬度
    minHeight: 48,      // 訊息氣泡最小高度
    padding: 16,        // 訊息氣泡內邊距
    gap: 12             // 訊息間距
  }
} as const;

// ============================================
// Breakpoints (響應式斷點)
// ============================================

export const breakpoints = {
  xs: 0,        // 0-599px    - Mobile (320-599px)
  sm: 600,      // 600-959px  - Tablet (600-959px)
  md: 960,      // 960-1279px - Desktop Small (960-1279px)
  lg: 1280,     // 1280-1919px - Desktop Medium (1280-1919px)
  xl: 1920      // 1920px+    - Desktop Large (1920px+)
} as const;

// ============================================
// TypeScript Types
// ============================================

export type ColorPalette = typeof colors;
export type Spacing = typeof spacing;
export type Typography = typeof typography;
export type BorderRadius = typeof borderRadius;
export type Shadows = typeof shadows;
export type Animation = typeof animation;
export type LayoutSizes = typeof layoutSizes;
export type Breakpoints = typeof breakpoints;

// ============================================
// Default Export
// ============================================

export const designTokens = {
  colors,
  spacing,
  typography,
  borderRadius,
  shadows,
  animation,
  layoutSizes,
  breakpoints
} as const;

export type DesignTokens = typeof designTokens;

export default designTokens;
