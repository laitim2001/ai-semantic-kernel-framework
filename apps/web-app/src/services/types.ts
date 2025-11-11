/**
 * API Service Types
 * Type definitions for API requests and responses
 */

// ============================================
// Conversation Types
// ============================================

export interface ConversationDto {
  id: string;
  agentId: string;
  userId?: string;
  title: string;
  createdAt: string;
  updatedAt: string;
  status?: 'active' | 'archived' | 'deleted';
  metadata?: Record<string, unknown>;
}

export interface MessageDto {
  id: string;
  conversationId: string;
  role: 'user' | 'agent' | 'system';
  content: string;
  timestamp: string;
  metadata?: Record<string, unknown>;
}

export interface CreateConversationRequest {
  agentId: string;
  userId: string;
  title: string;
  metadata?: Record<string, unknown>;
}

export interface AddMessageRequest {
  conversationId: string;
  role: 'user' | 'agent' | 'system';
  content: string;
  metadata?: Record<string, unknown>;
}

export interface GetConversationsParams {
  userId?: string;
  agentId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

// ============================================
// Agent Types
// ============================================

export interface AgentDto {
  id: string;
  userId: string;
  name: string;
  description?: string;
  instructions?: string;
  model: string;
  temperature?: number;
  maxTokens?: number;
  status: 'active' | 'paused' | 'archived';
  createdAt: string;
  updatedAt: string;
  metadata?: Record<string, unknown>;
}

export interface GetAgentsParams {
  userId?: string;
  status?: string;
  searchTerm?: string;
  model?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  skip?: number;
  take?: number;
}

export interface GetAgentsResponse {
  agents: AgentDto[];
  totalCount: number;
  skip: number;
  take: number;
}

// ============================================
// Pagination & Common Types
// ============================================

export interface PaginationParams {
  page: number;
  pageSize: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiResponse<T> {
  data?: T;
  error?: {
    message: string;
    code?: string;
  };
  success: boolean;
}
