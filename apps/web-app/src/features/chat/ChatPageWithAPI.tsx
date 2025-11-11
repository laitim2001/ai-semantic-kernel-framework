/**
 * ChatPage - Chat 頁面 (API 集成版本)
 * 使用真實 API 的對話界面
 */

import React, { useState, useEffect } from 'react';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Alert, CircularProgress, Box } from '@mui/material';
import theme from '../../theme/theme';
import {
  ChatLayout,
  ConversationSidebar,
  ChatArea,
  ChatHeader,
  MessageList,
  Message,
  ChatInput
} from './components';
import { Message as MessageType, MessageRole } from './types';
import { useConversation } from './hooks/useConversation';
import { agentService } from '../../services';
import type { MessageDto } from '../../services';

// ============================================
// Constants
// ============================================

const DEFAULT_AGENT_ID = 'e73dd2a8-39ab-440a-a43c-b9bf57e7267d'; // From API test

// ============================================
// ChatPageWithAPI Component
// ============================================

export const ChatPageWithAPI: React.FC = () => {
  const [agentId, setAgentId] = useState<string>(DEFAULT_AGENT_ID);
  const [agentName, setAgentName] = useState<string>('AI Assistant');
  const [isLoadingAgent, setIsLoadingAgent] = useState(true);
  const [inputValue, setInputValue] = useState('');
  const [charCount, setCharCount] = useState(0);

  const {
    conversation,
    messages: apiMessages,
    isLoading,
    isSending,
    error: apiError,
    sendMessage,
    createConversation,
  } = useConversation({ agentId });

  /**
   * Load agent information
   */
  useEffect(() => {
    const loadAgent = async () => {
      try {
        setIsLoadingAgent(true);

        // Try to load specific agent or get first available
        try {
          const agent = await agentService.getAgentById(agentId);
          setAgentName(agent.name);
        } catch {
          // If specific agent not found, get first available agent
          const agentsResponse = await agentService.getAgents({ take: 1, status: 'active' });
          if (agentsResponse.agents.length > 0) {
            const firstAgent = agentsResponse.agents[0];
            setAgentId(firstAgent.id);
            setAgentName(firstAgent.name);
          }
        }
      } catch (err) {
        console.error('Failed to load agent:', err);
        setAgentName('AI Assistant (Offline)');
      } finally {
        setIsLoadingAgent(false);
      }
    };

    loadAgent();
  }, []);

  /**
   * Convert API messages to UI message format
   */
  const uiMessages: MessageType[] = apiMessages.map((msg: MessageDto) => ({
    id: msg.id,
    conversationId: msg.conversationId,
    role: msg.role as MessageRole,
    content: msg.content,
    timestamp: msg.timestamp,
  }));

  /**
   * Handle input change
   */
  const handleInputChange = (value: string) => {
    setInputValue(value);
    setCharCount(value.length);
  };

  /**
   * Handle send message
   */
  const handleSend = async () => {
    if (!inputValue.trim() || isSending) {
      return;
    }

    await sendMessage(inputValue);
    setInputValue('');
    setCharCount(0);
  };

  /**
   * Handle create new conversation
   */
  const handleCreateConversation = async () => {
    await createConversation();
    setInputValue('');
    setCharCount(0);
  };

  /**
   * Loading state
   */
  if (isLoadingAgent || isLoading) {
    return (
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            height: '100vh',
            gap: 2,
          }}
        >
          <CircularProgress />
          <span>Loading...</span>
        </Box>
      </ThemeProvider>
    );
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      {/* Error Banner */}
      {apiError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {apiError}
        </Alert>
      )}

      <ChatLayout
        sidebar={
          <ConversationSidebar
            conversations={[]}
            currentConversationId={conversation?.id || null}
            onSelectConversation={(id) => console.log('Select conversation:', id)}
            onCreateConversation={handleCreateConversation}
          />
        }
        chatArea={
          <ChatArea
            header={
              <ChatHeader
                conversationTitle={conversation?.title || `Chat with ${agentName}`}
                onExport={() => console.log('Export conversation')}
                onSettings={() => console.log('Open settings')}
                onRename={() => console.log('Rename conversation')}
              />
            }
            messageList={
              <MessageList messages={uiMessages}>
                {uiMessages.map((message) => (
                  <Message key={message.id} message={message} />
                ))}
              </MessageList>
            }
            chatInput={
              <ChatInput
                value={inputValue}
                charCount={charCount}
                isSending={isSending}
                onChange={handleInputChange}
                onSend={handleSend}
              />
            }
          />
        }
      />
    </ThemeProvider>
  );
};

export default ChatPageWithAPI;
