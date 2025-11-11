/**
 * MessageList - 訊息列表容器
 * 可滾動的訊息顯示區域
 */

import React, { useEffect, useRef } from 'react';
import { Box, Typography } from '@mui/material';
import { spacing, colors } from '../../../theme/tokens';
import { Message } from '../types';

interface MessageListProps {
  messages: Message[];
  streamingContent?: string;
  isStreaming?: boolean;
  onStopStreaming?: () => void;
  children: React.ReactNode;
}

export const MessageList: React.FC<MessageListProps> = ({
  messages,
  streamingContent,
  isStreaming,
  children
}) => {
  const messagesEndRef = useRef<HTMLDivElement>(null);

  /**
   * 自動滾動到底部
   * - 當新訊息到達時滾動
   * - 當 Streaming 完成時滾動
   */
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: 'smooth', block: 'end' });
    }
  }, [messages.length, streamingContent, isStreaming]);

  return (
    <Box
      sx={{
        height: '100%',
        overflowY: 'auto',
        padding: spacing.xl,
        display: 'flex',
        flexDirection: 'column',
        gap: spacing.md,

        // Scrollbar styling
        '&::-webkit-scrollbar': {
          width: 8
        },
        '&::-webkit-scrollbar-thumb': {
          backgroundColor: colors.border,
          borderRadius: '4px',
          '&:hover': {
            backgroundColor: colors.text.disabled
          }
        },
        '&::-webkit-scrollbar-track': {
          backgroundColor: 'transparent'
        }
      }}
      role="log"
      aria-live="polite"
      aria-atomic="false"
      aria-label="訊息列表"
    >
      {/* Messages */}
      {children}

      {/* Empty State */}
      {messages.length === 0 && !streamingContent && (
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            height: '100%',
            textAlign: 'center',
            color: colors.text.secondary
          }}
        >
          <Typography variant="h3" sx={{ marginBottom: spacing.md }}>
            開始新對話
          </Typography>
          <Typography variant="body1">
            在下方輸入框輸入訊息,開始與 Agent 對話
          </Typography>
        </Box>
      )}

      {/* Scroll anchor */}
      <div ref={messagesEndRef} />
    </Box>
  );
};

export default MessageList;
