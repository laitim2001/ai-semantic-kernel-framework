/**
 * Chat Feature Type Definitions
 */

// Conversation Types
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

// Message Types
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
}
