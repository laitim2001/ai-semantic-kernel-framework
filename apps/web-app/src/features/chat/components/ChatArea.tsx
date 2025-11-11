/**
 * ChatArea - 對話區域
 * 包含 Header, MessageList, ChatInput
 */

import React from 'react';
import { Box } from '@mui/material';
import { colors } from '../../../theme/tokens';

interface ChatAreaProps {
  header: React.ReactNode;
  messageList: React.ReactNode;
  chatInput: React.ReactNode;
}

export const ChatArea: React.FC<ChatAreaProps> = ({
  header,
  messageList,
  chatInput
}) => {
  return (
    <Box
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: colors.background.chat
      }}
    >
      {/* Chat Header */}
      <Box sx={{ flexShrink: 0 }}>
        {header}
      </Box>

      {/* Message List - 可滾動區域 */}
      <Box
        sx={{
          flexGrow: 1,
          overflow: 'hidden'
        }}
      >
        {messageList}
      </Box>

      {/* Chat Input - 固定在底部 */}
      <Box sx={{ flexShrink: 0 }}>
        {chatInput}
      </Box>
    </Box>
  );
};

export default ChatArea;
