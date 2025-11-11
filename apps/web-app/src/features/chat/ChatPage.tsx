/**
 * ChatPage - Chat 頁面 (Demo 版本)
 * 用於測試靜態 UI,含 Mock 資料
 */

import React, { useState } from 'react';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
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
import { Conversation, Message as MessageType, MessageRole } from './types';

// ============================================
// Mock Data (測試資料)
// ============================================

const mockConversations: Conversation[] = [
  {
    id: '1',
    agentId: 'agent-1',
    title: 'React Hooks 介紹',
    createdAt: '2025-12-12T10:00:00Z',
    updatedAt: '2025-12-12T10:30:00Z',
    isDeleted: false,
    lastMessage: '太好了!如果還有其他問題...',
    messageCount: 8
  },
  {
    id: '2',
    agentId: 'agent-1',
    title: 'TypeScript 泛型',
    createdAt: '2025-12-12T09:00:00Z',
    updatedAt: '2025-12-12T09:45:00Z',
    isDeleted: false,
    lastMessage: '泛型提供了強大的類型約束...',
    messageCount: 5
  },
  {
    id: '3',
    agentId: 'agent-1',
    title: 'Material-UI 使用技巧',
    createdAt: '2025-12-11T14:00:00Z',
    updatedAt: '2025-12-11T15:20:00Z',
    isDeleted: false,
    lastMessage: 'MUI v6 帶來了很多改進...',
    messageCount: 12
  }
];

const mockMessages: MessageType[] = [
  {
    id: 'msg-1',
    conversationId: '1',
    role: MessageRole.User,
    content: '你好,請介紹一下 React Hooks',
    timestamp: '2025-12-12T10:00:00Z'
  },
  {
    id: 'msg-2',
    conversationId: '1',
    role: MessageRole.Agent,
    content: `# React Hooks 簡介

React Hooks 是 React 16.8 引入的新功能,讓你可以在**函數組件**中使用狀態和其他 React 特性。

## 常用的 Hooks

### 1. useState
用於在函數組件中添加狀態:

\`\`\`javascript
const [count, setCount] = useState(0);
\`\`\`

### 2. useEffect
用於處理副作用 (side effects):

\`\`\`javascript
useEffect(() => {
  document.title = \`You clicked \${count} times\`;
}, [count]);
\`\`\`

### 3. useContext
用於消費 Context:

\`\`\`javascript
const value = useContext(MyContext);
\`\`\`

## Hooks 的優點

1. **更簡潔的代碼** - 不需要 class 組件
2. **更好的邏輯複用** - 通過自定義 Hooks
3. **更容易理解** - 不需要理解 \`this\`

> **注意**: Hooks 只能在函數組件的頂層調用,不能在循環、條件或嵌套函數中調用。`,
    timestamp: '2025-12-12T10:01:00Z'
  },
  {
    id: 'msg-3',
    conversationId: '1',
    role: MessageRole.User,
    content: '能舉個 useState 的實際例子嗎?',
    timestamp: '2025-12-12T10:25:00Z'
  },
  {
    id: 'msg-4',
    conversationId: '1',
    role: MessageRole.Agent,
    content: `當然!這裡是一個實際的計數器例子:

\`\`\`typescript
import React, { useState } from 'react';

function Counter() {
  // 宣告狀態變量 count,初始值為 0
  const [count, setCount] = useState(0);

  return (
    <div>
      <p>你點擊了 {count} 次</p>
      <button onClick={() => setCount(count + 1)}>
        點擊我
      </button>
      <button onClick={() => setCount(0)}>
        重置
      </button>
    </div>
  );
}
\`\`\`

**解釋**:
- \`useState(0)\` 創建一個初始值為 0 的狀態
- \`count\` 是當前的狀態值
- \`setCount\` 是更新狀態的函數
- 每次調用 \`setCount\` 都會觸發組件重新渲染`,
    timestamp: '2025-12-12T10:26:00Z'
  },
  {
    id: 'msg-5',
    conversationId: '1',
    role: MessageRole.User,
    content: '太好了!如果還有其他問題,我會再問你的',
    timestamp: '2025-12-12T10:30:00Z'
  }
];

// ============================================
// ChatPage Component
// ============================================

export const ChatPage: React.FC = () => {
  const [currentConversationId, setCurrentConversationId] = useState<string | null>('1');
  const [inputValue, setInputValue] = useState('');
  const [charCount, setCharCount] = useState(0);

  const handleInputChange = (value: string) => {
    setInputValue(value);
    setCharCount(value.length);
  };

  const handleSend = () => {
    console.log('Send message:', inputValue);
    // TODO: 實際發送邏輯
    setInputValue('');
    setCharCount(0);
  };

  const handleSelectConversation = (conversationId: string) => {
    setCurrentConversationId(conversationId);
  };

  const handleCreateConversation = () => {
    console.log('Create new conversation');
    // TODO: 實際創建邏輯
  };

  const currentConversation = mockConversations.find((c) => c.id === currentConversationId);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ChatLayout
        sidebar={
          <ConversationSidebar
            conversations={mockConversations}
            currentConversationId={currentConversationId}
            onSelectConversation={handleSelectConversation}
            onCreateConversation={handleCreateConversation}
          />
        }
        chatArea={
          <ChatArea
            header={
              <ChatHeader
                conversationTitle={currentConversation?.title || '新對話'}
                onExport={() => console.log('Export conversation')}
                onSettings={() => console.log('Open settings')}
                onRename={() => console.log('Rename conversation')}
              />
            }
            messageList={
              <MessageList messages={mockMessages}>
                {mockMessages.map((message) => (
                  <Message key={message.id} message={message} />
                ))}
              </MessageList>
            }
            chatInput={
              <ChatInput
                value={inputValue}
                charCount={charCount}
                isSending={false}
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

export default ChatPage;
