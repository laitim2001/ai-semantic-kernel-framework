/**
 * Chat Feature Type Definitions
 * TypeScript 類型定義 - Chat 功能相關
 */

// ============================================
// Conversation Types (對話類型)
// ============================================

export interface Conversation {
  id: string;
  agentId: string;
  title: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  lastMessage?: string;
  messageCount: number;
}

export interface CreateConversationRequest {
  agentId: string;
  title: string;
}

export interface UpdateConversationRequest {
  title: string;
}

export interface GetConversationsResponse {
  conversations: Conversation[];
  totalCount: number;
}

// ============================================
// Message Types (訊息類型)
// ============================================

export enum MessageRole {
  User = 'user',
  Agent = 'agent',
  System = 'system'
}

export interface Message {
  id: string;
  conversationId: string;
  role: MessageRole;
  content: string;
  timestamp: string;
  metadata?: {
    model?: string;
    tokens?: number;
    executionTime?: number;
    [key: string]: any;
  };
}

export interface GetMessagesResponse {
  messages: Message[];
  totalCount: number;
  hasMore: boolean;
}

// ============================================
// Agent Execution Types (Agent 執行類型)
// ============================================

export interface InvokeAgentRequest {
  conversationId: string;
  userInput: string;
  metadata?: {
    temperature?: number;
    maxTokens?: number;
    [key: string]: any;
  };
}

export interface InvokeAgentResponse {
  executionId: string;
  status: AgentExecutionStatus;
  message?: string;
}

export enum AgentExecutionStatus {
  Pending = 'pending',
  Running = 'running',
  Completed = 'completed',
  Failed = 'failed',
  Cancelled = 'cancelled'
}

// ============================================
// SignalR Streaming Types (SignalR 串流類型)
// ============================================

export interface StreamingChunk {
  executionId: string;
  conversationId: string;
  content: string;
  isComplete: boolean;
  timestamp: string;
}

export interface AgentExecutionStatusChanged {
  executionId: string;
  conversationId: string;
  status: AgentExecutionStatus;
  errorMessage?: string;
  timestamp: string;
}

// ============================================
// Chat UI State Types (Chat UI 狀態類型)
// ============================================

export interface ChatState {
  // Current conversation
  currentConversationId: string | null;
  conversations: Conversation[];

  // Messages
  messages: Message[];
  isLoadingMessages: boolean;

  // Streaming
  isStreaming: boolean;
  streamingContent: string;
  currentExecutionId: string | null;

  // Input
  inputContent: string;
  charCount: number;

  // UI State
  isSending: boolean;
  error: string | null;
}

export interface ConversationActions {
  // Conversation CRUD
  loadConversations: (agentId: string) => Promise<void>;
  createConversation: (agentId: string, title: string) => Promise<Conversation>;
  renameConversation: (conversationId: string, newTitle: string) => Promise<void>;
  deleteConversation: (conversationId: string) => Promise<void>;
  selectConversation: (conversationId: string) => void;

  // Message operations
  loadMessages: (conversationId: string) => Promise<void>;
  sendMessage: (agentId: string, conversationId: string, content: string) => Promise<void>;
  stopStreaming: () => void;

  // Input management
  setInputContent: (content: string) => void;
  clearInput: () => void;

  // Error handling
  setError: (error: string | null) => void;
  clearError: () => void;
}

// ============================================
// Export Configuration Types (匯出配置類型)
// ============================================

export enum ExportFormat {
  Markdown = 'markdown',
  PDF = 'pdf',
  JSON = 'json'
}

export interface ExportConversationRequest {
  conversationId: string;
  format: ExportFormat;
  includeTimestamps: boolean;
}

// ============================================
// Context Management Types (上下文管理類型)
// ============================================

export interface ContextInfo {
  totalMessages: number;
  totalTokens: number;
  maxTokens: number;
  percentage: number;
}

export interface ContextManagementOptions {
  maxMessages: number;     // 最大保留訊息數 (例: 20)
  maxTokens: number;        // 最大 token 數 (例: 8000)
  autoTruncate: boolean;    // 是否自動截斷
  truncateStrategy: 'oldest' | 'sliding-window';  // 截斷策略
}

// ============================================
// Component Props Types (組件 Props 類型)
// ============================================

export interface ChatPageProps {
  agentId: string;
}

export interface ConversationSidebarProps {
  conversations: Conversation[];
  currentConversationId: string | null;
  onSelectConversation: (conversationId: string) => void;
  onCreateConversation: () => void;
  onRenameConversation: (conversationId: string, newTitle: string) => void;
  onDeleteConversation: (conversationId: string) => void;
}

export interface ChatHeaderProps {
  conversationTitle: string;
  onRename: () => void;
  onExport: (format: ExportFormat) => void;
  onSettings: () => void;
}

export interface MessageListProps {
  messages: Message[];
  streamingContent: string;
  isStreaming: boolean;
  onStopStreaming: () => void;
}

export interface MessageProps {
  message: Message;
  isStreaming?: boolean;
  onStopStreaming?: () => void;
}

export interface ChatInputProps {
  value: string;
  charCount: number;
  maxChars: number;
  isSending: boolean;
  onChange: (value: string) => void;
  onSend: () => void;
  onClear: () => void;
}

// ============================================
// Utility Types
// ============================================

export type ChatStore = ChatState & ConversationActions;

export type Optional<T> = T | null | undefined;

export type Nullable<T> = T | null;
