/**
 * ConversationItem - 對話列表項目
 * 顯示對話標題、預覽和時間戳
 */

import React from 'react';
import { ListItem, ListItemButton, Box, Typography } from '@mui/material';
import { spacing, colors, typography, borderRadius } from '../../../theme/tokens';
import { Conversation } from '../types';

interface ConversationItemProps {
  conversation: Conversation;
  isActive: boolean;
  onClick: () => void;
  onRename?: (newTitle: string) => void;
  onDelete?: () => void;
}

/**
 * 格式化相對時間
 */
const formatRelativeTime = (timestamp: string): string => {
  const now = new Date();
  const date = new Date(timestamp);
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (diffInSeconds < 60) {
    return '剛剛';
  } else if (diffInSeconds < 3600) {
    const minutes = Math.floor(diffInSeconds / 60);
    return `${minutes} 分鐘前`;
  } else if (diffInSeconds < 86400) {
    const hours = Math.floor(diffInSeconds / 3600);
    return `${hours} 小時前`;
  } else if (diffInSeconds < 604800) {
    const days = Math.floor(diffInSeconds / 86400);
    return `${days} 天前`;
  } else {
    // 超過 7 天顯示日期
    return date.toLocaleDateString('zh-TW', {
      month: 'numeric',
      day: 'numeric'
    });
  }
};

export const ConversationItem: React.FC<ConversationItemProps> = ({
  conversation,
  isActive,
  onClick
}) => {
  return (
    <ListItem
      disablePadding
      sx={{
        marginBottom: spacing.sm
      }}
    >
      <ListItemButton
        onClick={onClick}
        selected={isActive}
        sx={{
          padding: spacing.md,
          borderRadius: borderRadius.md,
          transition: 'all 150ms ease-in-out',
          border: 'none',

          // Active state
          ...(isActive && {
            backgroundColor: 'rgba(25, 118, 210, 0.08)',
            borderLeft: `3px solid ${colors.primary.main}`,
            paddingLeft: `${spacing.md - 3}px`
          }),

          // Hover state (only for inactive items)
          ...(!isActive && {
            '&:hover': {
              backgroundColor: colors.background.paper
            }
          }),

          // Selected state override
          '&.Mui-selected': {
            backgroundColor: 'rgba(25, 118, 210, 0.08)',
            '&:hover': {
              backgroundColor: 'rgba(25, 118, 210, 0.12)'
            }
          }
        }}
        role="button"
        aria-label={`切換到對話: ${conversation.title}`}
        aria-current={isActive ? 'page' : undefined}
      >
        <Box sx={{ width: '100%', overflow: 'hidden' }}>
          {/* Title */}
          <Typography
            sx={{
              fontSize: typography.fontSize.md,
              fontWeight: typography.fontWeight.medium,
              color: colors.text.primary,
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
              marginBottom: spacing.xs
            }}
          >
            💬 {conversation.title}
          </Typography>

          {/* Last Message Preview */}
          {conversation.lastMessage && (
            <Typography
              variant="body2"
              sx={{
                fontSize: typography.fontSize.sm,
                color: colors.text.secondary,
                overflow: 'hidden',
                textOverflow: 'ellipsis',
                whiteSpace: 'nowrap',
                marginBottom: spacing.xs
              }}
            >
              {conversation.lastMessage}
            </Typography>
          )}

          {/* Timestamp */}
          <Typography
            variant="caption"
            sx={{
              fontSize: typography.fontSize.xs,
              color: colors.text.hint
            }}
          >
            {formatRelativeTime(conversation.updatedAt)}
          </Typography>
        </Box>
      </ListItemButton>
    </ListItem>
  );
};

export default ConversationItem;
