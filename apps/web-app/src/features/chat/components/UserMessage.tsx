/**
 * UserMessage - 用戶訊息組件
 * 右對齊,藍色氣泡
 */

import React from 'react';
import { Box, Typography } from '@mui/material';
import { spacing, colors, typography, borderRadius, shadows, layoutSizes } from '../../../theme/tokens';
import { Message } from '../types';

interface UserMessageProps {
  message: Message;
}

export const UserMessage: React.FC<UserMessageProps> = ({ message }) => {
  /**
   * 格式化時間戳
   */
  const formatTimestamp = (timestamp: string): string => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('zh-TW', {
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'flex-end',
        marginBottom: spacing.md
      }}
      role="article"
      aria-label={`您的訊息: ${message.content}`}
    >
      <Box
        sx={{
          maxWidth: layoutSizes.message.maxWidth,
          padding: spacing.md,
          borderRadius: borderRadius.md,
          backgroundColor: colors.message.user.background,
          color: colors.message.user.text,
          boxShadow: shadows.sm
        }}
      >
        {/* Message Content */}
        <Typography
          sx={{
            fontSize: typography.fontSize.md,
            lineHeight: typography.lineHeight.normal,
            whiteSpace: 'pre-wrap',
            wordBreak: 'break-word',
            marginBottom: spacing.xs
          }}
        >
          {message.content}
        </Typography>

        {/* Timestamp */}
        <Typography
          variant="caption"
          sx={{
            fontSize: typography.fontSize.xs,
            color: colors.message.user.timestamp,
            textAlign: 'right',
            display: 'block'
          }}
        >
          {formatTimestamp(message.timestamp)}
        </Typography>
      </Box>
    </Box>
  );
};

export default UserMessage;
