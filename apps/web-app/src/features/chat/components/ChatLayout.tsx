/**
 * ChatLayout - Chat 頁面主佈局
 * 左側 Sidebar (280px) + 右側 ChatArea (flex-grow)
 */

import React from 'react';
import { Box, useMediaQuery, useTheme } from '@mui/material';
import { layoutSizes } from '../../../theme/tokens';

interface ChatLayoutProps {
  sidebar: React.ReactNode;
  chatArea: React.ReactNode;
}

export const ChatLayout: React.FC<ChatLayoutProps> = ({ sidebar, chatArea }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm')); // < 600px

  return (
    <Box
      sx={{
        display: 'flex',
        height: '100vh',
        width: '100vw',
        overflow: 'hidden',
        backgroundColor: theme.palette.background.default
      }}
    >
      {/* Sidebar - 在 Mobile 時隱藏,切換為 Drawer (後續實現) */}
      {!isMobile && (
        <Box
          component="aside"
          sx={{
            width: layoutSizes.sidebar.width,
            minWidth: layoutSizes.sidebar.minWidth,
            maxWidth: layoutSizes.sidebar.maxWidth,
            height: '100%',
            borderRight: `1px solid ${theme.palette.divider}`,
            overflow: 'hidden',
            flexShrink: 0
          }}
        >
          {sidebar}
        </Box>
      )}

      {/* Chat Area - 主要對話區域 */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          height: '100%',
          overflow: 'hidden',
          display: 'flex',
          flexDirection: 'column'
        }}
      >
        {chatArea}
      </Box>
    </Box>
  );
};

export default ChatLayout;
