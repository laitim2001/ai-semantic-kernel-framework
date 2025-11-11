/**
 * Message - 統一訊息組件
 * 根據 role 自動選擇 UserMessage 或 AgentMessage
 */

import React from 'react';
import { Message as MessageType, MessageRole } from '../types';
import { UserMessage } from './UserMessage';
import { AgentMessage } from './AgentMessage';

interface MessageProps {
  message: MessageType;
  isStreaming?: boolean;
  onStopStreaming?: () => void;
}

export const Message: React.FC<MessageProps> = ({
  message,
  isStreaming,
  onStopStreaming
}) => {
  switch (message.role) {
    case MessageRole.User:
      return <UserMessage message={message} />;

    case MessageRole.Agent:
      return (
        <AgentMessage
          message={message}
          isStreaming={isStreaming}
          onStopStreaming={onStopStreaming}
        />
      );

    case MessageRole.System:
      // System messages (暫不顯示或使用特殊樣式)
      return null;

    default:
      return null;
  }
};

export default Message;
