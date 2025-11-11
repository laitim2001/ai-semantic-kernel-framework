/**
 * ChatHeader - 對話標題欄
 * 顯示當前對話標題和操作按鈕
 */

import React from 'react';
import { Box, Typography, IconButton, Tooltip } from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import SettingsIcon from '@mui/icons-material/Settings';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import { spacing, colors, typography, layoutSizes } from '../../../theme/tokens';

interface ChatHeaderProps {
  conversationTitle: string;
  onRename?: () => void;
  onExport?: () => void;
  onSettings?: () => void;
}

export const ChatHeader: React.FC<ChatHeaderProps> = ({
  conversationTitle,
  onRename,
  onExport,
  onSettings
}) => {
  return (
    <Box
      component="header"
      sx={{
        height: layoutSizes.chatHeader.height,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: `0 ${spacing.xl}px`,
        borderBottom: `1px solid ${colors.divider}`,
        backgroundColor: colors.background.default
      }}
    >
      {/* Title */}
      <Typography
        variant="h2"
        sx={{
          fontSize: typography.fontSize.xl,
          fontWeight: typography.fontWeight.semibold,
          color: colors.text.primary,
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap',
          flexGrow: 1,
          marginRight: spacing.lg
        }}
      >
        {conversationTitle}
      </Typography>

      {/* Action Buttons */}
      <Box
        sx={{
          display: 'flex',
          gap: spacing.sm
        }}
      >
        {/* Export Button */}
        {onExport && (
          <Tooltip title="匯出對話">
            <IconButton
              onClick={onExport}
              size="medium"
              sx={{
                width: 40,
                height: 40,
                color: colors.text.secondary,
                '&:hover': {
                  backgroundColor: colors.background.paper
                }
              }}
              aria-label="匯出對話"
            >
              <FileDownloadIcon />
            </IconButton>
          </Tooltip>
        )}

        {/* Settings Button */}
        {onSettings && (
          <Tooltip title="設置">
            <IconButton
              onClick={onSettings}
              size="medium"
              sx={{
                width: 40,
                height: 40,
                color: colors.text.secondary,
                '&:hover': {
                  backgroundColor: colors.background.paper
                }
              }}
              aria-label="設置"
            >
              <SettingsIcon />
            </IconButton>
          </Tooltip>
        )}

        {/* More Options */}
        <Tooltip title="更多選項">
          <IconButton
            onClick={onRename}
            size="medium"
            sx={{
              width: 40,
              height: 40,
              color: colors.text.secondary,
              '&:hover': {
                backgroundColor: colors.background.paper
              }
            }}
            aria-label="更多選項"
          >
            <MoreVertIcon />
          </IconButton>
        </Tooltip>
      </Box>
    </Box>
  );
};

export default ChatHeader;
