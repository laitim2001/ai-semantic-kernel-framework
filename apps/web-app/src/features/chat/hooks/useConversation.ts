/**
 * useConversation Hook
 * Manages conversation state and API calls
 */

import { useState, useEffect, useCallback } from 'react';
import { conversationService } from '../../../services';
import type { ConversationDto, MessageDto } from '../../../services';
import { MessageRole } from '../types';

interface UseConversationOptions {
  agentId: string;
  conversationId?: string;
}

interface UseConversationReturn {
  conversation: ConversationDto | null;
  messages: MessageDto[];
  isLoading: boolean;
  isSending: boolean;
  error: string | null;
  sendMessage: (content: string) => Promise<void>;
  createConversation: (title?: string) => Promise<void>;
  refreshConversation: () => Promise<void>;
}

export function useConversation(options: UseConversationOptions): UseConversationReturn {
  const { agentId, conversationId: initialConversationId } = options;

  const [conversation, setConversation] = useState<ConversationDto | null>(null);
  const [messages, setMessages] = useState<MessageDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [conversationId, setConversationId] = useState<string | undefined>(initialConversationId);

  /**
   * Load conversation data
   */
  const loadConversation = useCallback(async (id: string) => {
    try {
      setIsLoading(true);
      setError(null);

      const conversationData = await conversationService.getConversationById(id);
      setConversation(conversationData);

      // Load messages (assuming they're included in conversation response)
      // If messages are separate, we'd make another API call here
      // For now, we'll use an empty array and messages will be added as we send them
      setMessages([]);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load conversation');
      console.error('Failed to load conversation:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Create new conversation
   */
  const createConversation = useCallback(async (title?: string) => {
    try {
      setIsLoading(true);
      setError(null);

      // Generate a test userId (in production, this would come from auth)
      const userId = '00000000-0000-0000-0000-000000000001';

      const newConversation = await conversationService.createConversation({
        agentId,
        userId,
        title: title || 'New Conversation',
      });

      setConversation(newConversation);
      setConversationId(newConversation.id);
      setMessages([]);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create conversation');
      console.error('Failed to create conversation:', err);
    } finally {
      setIsLoading(false);
    }
  }, [agentId]);

  /**
   * Send message
   */
  const sendMessage = useCallback(async (content: string) => {
    if (!conversationId) {
      setError('No active conversation');
      return;
    }

    if (!content.trim()) {
      return;
    }

    try {
      setIsSending(true);
      setError(null);

      // Add user message optimistically
      const userMessage: MessageDto = {
        id: `temp-${Date.now()}`,
        conversationId,
        role: 'user',
        content,
        timestamp: new Date().toISOString(),
      };
      setMessages((prev) => [...prev, userMessage]);

      // Send message to API
      const sentMessage = await conversationService.addMessage(conversationId, {
        conversationId,
        role: 'user',
        content,
      });

      // Replace temp message with actual message
      setMessages((prev) =>
        prev.map((msg) => (msg.id === userMessage.id ? sentMessage : msg))
      );

      // In a real implementation with streaming or agent responses,
      // we would either:
      // 1. Wait for a streaming response
      // 2. Poll for new messages
      // 3. Use WebSocket/SignalR for real-time updates
      //
      // For now, we'll simulate agent response after a delay
      setTimeout(async () => {
        // Fetch updated conversation to get agent's response
        try {
          const updatedConversation = await conversationService.getConversationById(conversationId);
          setConversation(updatedConversation);

          // Add mock agent response for testing
          const agentMessage: MessageDto = {
            id: `agent-${Date.now()}`,
            conversationId,
            role: 'agent',
            content: 'This is a simulated agent response. Real agent integration coming soon!',
            timestamp: new Date().toISOString(),
          };
          setMessages((prev) => [...prev, agentMessage]);
        } catch (err) {
          console.error('Failed to fetch agent response:', err);
        }
      }, 1000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to send message');
      console.error('Failed to send message:', err);
      // Remove optimistic message on error
      setMessages((prev) => prev.filter((msg) => !msg.id.startsWith('temp-')));
    } finally {
      setIsSending(false);
    }
  }, [conversationId]);

  /**
   * Refresh conversation data
   */
  const refreshConversation = useCallback(async () => {
    if (conversationId) {
      await loadConversation(conversationId);
    }
  }, [conversationId, loadConversation]);

  /**
   * Load conversation on mount or when ID changes
   */
  useEffect(() => {
    if (conversationId) {
      loadConversation(conversationId);
    } else {
      // Auto-create conversation if no ID provided
      createConversation();
    }
  }, []); // Only run once on mount

  return {
    conversation,
    messages,
    isLoading,
    isSending,
    error,
    sendMessage,
    createConversation,
    refreshConversation,
  };
}

export default useConversation;
