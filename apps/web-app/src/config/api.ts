/**
 * API Configuration
 * API 基礎配置和常量
 */

// ============================================
// API Base URLs
// ============================================

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1';
export const SIGNALR_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5000/agentHub';

// ============================================
// API Endpoints
// ============================================

export const API_ENDPOINTS = {
  // Agent endpoints
  agents: {
    base: '/agents',
    getById: (id: string) => `/agents/${id}`,
    invoke: (id: string) => `/agents/${id}/invoke`
  },

  // Conversation endpoints
  conversations: {
    getAll: (agentId: string) => `/agents/${agentId}/conversations`,
    create: (agentId: string) => `/agents/${agentId}/conversations`,
    getById: (conversationId: string) => `/conversations/${conversationId}`,
    update: (conversationId: string) => `/conversations/${conversationId}`,
    delete: (conversationId: string) => `/conversations/${conversationId}`,
    export: (conversationId: string) => `/conversations/${conversationId}/export`
  },

  // Message endpoints
  messages: {
    getAll: (conversationId: string) => `/conversations/${conversationId}/messages`,
    getById: (conversationId: string, messageId: string) =>
      `/conversations/${conversationId}/messages/${messageId}`
  }
} as const;

// ============================================
// SignalR Hub Methods
// ============================================

export const SIGNALR_METHODS = {
  // Server -> Client events
  onAgentResponseChunk: 'AgentResponseChunk',
  onAgentExecutionStatusChanged: 'AgentExecutionStatusChanged',
  onAgentError: 'AgentError',

  // Client -> Server methods
  stopExecution: 'StopExecution',
  subscribeToConversation: 'SubscribeToConversation',
  unsubscribeFromConversation: 'UnsubscribeFromConversation'
} as const;

// ============================================
// Request Configuration
// ============================================

export const REQUEST_CONFIG = {
  timeout: 30000,              // 30 seconds
  headers: {
    'Content-Type': 'application/json'
  }
} as const;

// ============================================
// Chat Configuration
// ============================================

export const CHAT_CONFIG = {
  // Input limits
  maxInputLength: 2000,        // 最大輸入字數
  warningThreshold: 1800,      // 警告閾值 (90%)

  // Message pagination
  messagesPerPage: 50,         // 每頁訊息數
  initialLoadCount: 20,        // 初始載入訊息數

  // Streaming configuration
  streamingSpeed: 25,          // 字符/秒 (打字機效果速度)
  streamingChunkSize: 1,       // 每次顯示字符數

  // Context management
  maxContextMessages: 20,      // 最大上下文訊息數
  maxContextTokens: 8000,      // 最大上下文 token 數
  autoTruncate: true,          // 自動截斷

  // Auto-scroll
  autoScrollDelay: 100,        // 自動滾動延遲 (ms)
  scrollBehavior: 'smooth' as ScrollBehavior,  // 滾動行為

  // Retry configuration
  maxRetries: 3,               // 最大重試次數
  retryDelay: 1000,            // 重試延遲 (ms)

  // Conversation
  defaultConversationTitle: '新對話',  // 預設對話標題
  maxConversationTitleLength: 50      // 最大對話標題長度
} as const;

// ============================================
// Error Messages
// ============================================

export const ERROR_MESSAGES = {
  // Network errors
  NETWORK_ERROR: '網路連接失敗,請檢查網路設置',
  TIMEOUT_ERROR: '請求超時,請稍後重試',
  SERVER_ERROR: '伺服器錯誤,請稍後重試',

  // API errors
  AGENT_NOT_FOUND: 'Agent 不存在',
  CONVERSATION_NOT_FOUND: '對話不存在',
  MESSAGE_NOT_FOUND: '訊息不存在',
  INVALID_REQUEST: '無效的請求',

  // Validation errors
  INPUT_TOO_LONG: `輸入內容超過 ${CHAT_CONFIG.maxInputLength} 字限制`,
  INPUT_EMPTY: '請輸入訊息內容',
  TITLE_TOO_LONG: `對話標題超過 ${CHAT_CONFIG.maxConversationTitleLength} 字限制`,
  TITLE_EMPTY: '對話標題不能為空',

  // SignalR errors
  SIGNALR_CONNECTION_FAILED: 'SignalR 連接失敗',
  SIGNALR_DISCONNECTED: 'SignalR 連接已中斷',
  STREAMING_ERROR: 'Streaming 過程中發生錯誤',

  // Generic errors
  UNKNOWN_ERROR: '發生未知錯誤',
  OPERATION_FAILED: '操作失敗,請重試'
} as const;

// ============================================
// Success Messages
// ============================================

export const SUCCESS_MESSAGES = {
  CONVERSATION_CREATED: '對話已創建',
  CONVERSATION_RENAMED: '對話已重命名',
  CONVERSATION_DELETED: '對話已刪除',
  CONVERSATION_EXPORTED: '對話已匯出',
  MESSAGE_SENT: '訊息已發送',
  CONTEXT_CLEARED: '上下文已清除'
} as const;

// ============================================
// Type Exports
// ============================================

export type ApiEndpoints = typeof API_ENDPOINTS;
export type SignalRMethods = typeof SIGNALR_METHODS;
export type ChatConfig = typeof CHAT_CONFIG;
export type ErrorMessages = typeof ERROR_MESSAGES;
export type SuccessMessages = typeof SUCCESS_MESSAGES;
