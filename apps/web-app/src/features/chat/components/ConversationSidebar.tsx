/**
 * ConversationSidebar - 對話側邊欄
 * 包含新對話按鈕和對話列表
 */

import React from 'react';
import { Box, Button, List } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { spacing, colors } from '../../../theme/tokens';
import { Conversation } from '../types';
import { ConversationItem } from './ConversationItem';

interface ConversationSidebarProps {
  conversations: Conversation[];
  currentConversationId: string | null;
  onSelectConversation: (conversationId: string) => void;
  onCreateConversation: () => void;
  onRenameConversation?: (conversationId: string, newTitle: string) => void;
  onDeleteConversation?: (conversationId: string) => void;
}

export const ConversationSidebar: React.FC<ConversationSidebarProps> = ({
  conversations,
  currentConversationId,
  onSelectConversation,
  onCreateConversation,
  onRenameConversation,
  onDeleteConversation
}) => {
  return (
    <Box
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: colors.background.paper,
        padding: `${spacing.lg}px`
      }}
      role="navigation"
      aria-label="對話列表"
    >
      {/* New Conversation Button */}
      <Button
        variant="contained"
        color="primary"
        startIcon={<AddIcon />}
        onClick={onCreateConversation}
        fullWidth
        sx={{
          height: 48,
          marginBottom: spacing.lg,
          textTransform: 'none',
          fontWeight: 500
        }}
        aria-label="新建對話"
      >
        新對話
      </Button>

      {/* Conversation List */}
      <Box
        sx={{
          flexGrow: 1,
          overflow: 'auto',
          '&::-webkit-scrollbar': {
            width: 8
          },
          '&::-webkit-scrollbar-thumb': {
            backgroundColor: colors.border,
            borderRadius: '4px'
          }
        }}
      >
        <List sx={{ padding: 0 }}>
          {conversations.map((conversation) => (
            <ConversationItem
              key={conversation.id}
              conversation={conversation}
              isActive={conversation.id === currentConversationId}
              onClick={() => onSelectConversation(conversation.id)}
              onRename={
                onRenameConversation
                  ? (newTitle) => onRenameConversation(conversation.id, newTitle)
                  : undefined
              }
              onDelete={
                onDeleteConversation
                  ? () => onDeleteConversation(conversation.id)
                  : undefined
              }
            />
          ))}
        </List>

        {/* Empty State */}
        {conversations.length === 0 && (
          <Box
            sx={{
              textAlign: 'center',
              padding: spacing.xl,
              color: colors.text.secondary
            }}
          >
            沒有對話記錄
            <br />
            點擊上方按鈕開始新對話
          </Box>
        )}
      </Box>
    </Box>
  );
};

export default ConversationSidebar;
