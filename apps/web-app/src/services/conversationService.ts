/**
 * Conversation Service
 * API calls for conversation management
 */

import apiClient, { getApiErrorMessage } from './apiClient';
import type {
  ConversationDto,
  MessageDto,
  CreateConversationRequest,
  AddMessageRequest,
  GetConversationsParams,
} from './types';

const BASE_PATH = '/v1/conversations';

export const conversationService = {
  /**
   * Get list of conversations
   */
  async getConversations(params?: GetConversationsParams): Promise<ConversationDto[]> {
    try {
      const response = await apiClient.get<ConversationDto[]>(BASE_PATH, { params });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch conversations:', getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Get conversation by ID
   */
  async getConversationById(id: string): Promise<ConversationDto> {
    try {
      const response = await apiClient.get<ConversationDto>(`${BASE_PATH}/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch conversation ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Create new conversation
   */
  async createConversation(request: CreateConversationRequest): Promise<ConversationDto> {
    try {
      const response = await apiClient.post<ConversationDto>(BASE_PATH, request);
      return response.data;
    } catch (error) {
      console.error('Failed to create conversation:', getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Add message to conversation
   */
  async addMessage(conversationId: string, request: AddMessageRequest): Promise<MessageDto> {
    try {
      const response = await apiClient.post<MessageDto>(
        `${BASE_PATH}/${conversationId}/messages`,
        request
      );
      return response.data;
    } catch (error) {
      console.error(`Failed to add message to conversation ${conversationId}:`, getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Delete conversation
   */
  async deleteConversation(id: string): Promise<void> {
    try {
      await apiClient.delete(`${BASE_PATH}/${id}`);
    } catch (error) {
      console.error(`Failed to delete conversation ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Update conversation (rename, etc.)
   */
  async updateConversation(id: string, updates: Partial<ConversationDto>): Promise<ConversationDto> {
    try {
      const response = await apiClient.put<ConversationDto>(`${BASE_PATH}/${id}`, updates);
      return response.data;
    } catch (error) {
      console.error(`Failed to update conversation ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },
};

export default conversationService;
