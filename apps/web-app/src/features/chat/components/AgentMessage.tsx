/**
 * AgentMessage - Agent 訊息組件
 * 支援 Markdown 渲染和代碼高亮
 */

import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import StopIcon from '@mui/icons-material/Stop';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { vscDarkPlus } from 'react-syntax-highlighter/dist/esm/styles/prism';
import { spacing, colors, typography, borderRadius, shadows, layoutSizes } from '../../../theme/tokens';
import { Message } from '../types';

interface AgentMessageProps {
  message: Message;
  isStreaming?: boolean;
  onStopStreaming?: () => void;
}

export const AgentMessage: React.FC<AgentMessageProps> = ({
  message,
  isStreaming = false,
  onStopStreaming
}) => {
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
        justifyContent: 'flex-start',
        marginBottom: spacing.md
      }}
      role="article"
      aria-label={`Agent 回覆: ${message.content.substring(0, 50)}...`}
      aria-busy={isStreaming}
    >
      <Box
        sx={{
          maxWidth: layoutSizes.message.maxWidth,
          padding: spacing.md,
          borderRadius: borderRadius.md,
          backgroundColor: colors.message.agent.background,
          color: colors.message.agent.text,
          boxShadow: shadows.sm,
          border: `1px solid ${colors.border}`
        }}
      >
        {/* Message Content with Markdown */}
        <Box
          sx={{
            '& p': {
              marginTop: 0,
              marginBottom: spacing.sm,
              '&:last-child': {
                marginBottom: 0
              }
            },
            '& h1, & h2, & h3, & h4, & h5, & h6': {
              marginTop: spacing.md,
              marginBottom: spacing.sm,
              fontWeight: typography.fontWeight.semibold,
              '&:first-child': {
                marginTop: 0
              }
            },
            '& h1': { fontSize: typography.fontSize.xxl },
            '& h2': { fontSize: typography.fontSize.xl },
            '& h3': { fontSize: typography.fontSize.lg },
            '& ul, & ol': {
              paddingLeft: spacing.lg,
              marginBottom: spacing.sm
            },
            '& li': {
              marginBottom: spacing.xs
            },
            '& code': {
              backgroundColor: 'rgba(0, 0, 0, 0.05)',
              padding: '2px 6px',
              borderRadius: borderRadius.sm,
              fontFamily: typography.fontFamily.code,
              fontSize: '0.9em'
            },
            '& pre': {
              marginBottom: spacing.sm,
              '& code': {
                backgroundColor: 'transparent',
                padding: 0
              }
            },
            '& blockquote': {
              borderLeft: `4px solid ${colors.primary.main}`,
              paddingLeft: spacing.md,
              marginLeft: 0,
              marginRight: 0,
              marginTop: spacing.sm,
              marginBottom: spacing.sm,
              fontStyle: 'italic',
              color: colors.text.secondary
            },
            '& table': {
              width: '100%',
              borderCollapse: 'collapse',
              marginBottom: spacing.sm,
              '& th, & td': {
                border: `1px solid ${colors.border}`,
                padding: spacing.sm,
                textAlign: 'left'
              },
              '& th': {
                backgroundColor: colors.background.paper,
                fontWeight: typography.fontWeight.semibold
              }
            },
            '& a': {
              color: colors.primary.main,
              textDecoration: 'none',
              '&:hover': {
                textDecoration: 'underline'
              }
            }
          }}
        >
          <ReactMarkdown
            remarkPlugins={[remarkGfm]}
            components={{
              // 自定義代碼塊渲染 (支援語法高亮)
              code({ node, inline, className, children, ...props }) {
                const match = /language-(\w+)/.exec(className || '');
                return !inline && match ? (
                  <SyntaxHighlighter
                    style={vscDarkPlus}
                    language={match[1]}
                    PreTag="div"
                    customStyle={{
                      borderRadius: borderRadius.md,
                      padding: spacing.md,
                      margin: 0,
                      fontSize: typography.fontSize.sm
                    }}
                    {...props}
                  >
                    {String(children).replace(/\n$/, '')}
                  </SyntaxHighlighter>
                ) : (
                  <code className={className} {...props}>
                    {children}
                  </code>
                );
              }
            }}
          >
            {message.content}
          </ReactMarkdown>

          {/* Streaming cursor (打字機游標) */}
          {isStreaming && (
            <Box
              component="span"
              sx={{
                display: 'inline-block',
                width: '2px',
                height: '1em',
                backgroundColor: colors.status.streaming,
                marginLeft: '2px',
                animation: 'blink 1s infinite',
                '@keyframes blink': {
                  '0%, 49%': { opacity: 1 },
                  '50%, 100%': { opacity: 0 }
                }
              }}
              aria-hidden="true"
            />
          )}
        </Box>

        {/* Footer: Timestamp + Stop Button */}
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            marginTop: spacing.sm
          }}
        >
          {/* Timestamp */}
          <Typography
            variant="caption"
            sx={{
              fontSize: typography.fontSize.xs,
              color: colors.message.agent.timestamp
            }}
          >
            {formatTimestamp(message.timestamp)}
          </Typography>

          {/* Stop Streaming Button */}
          {isStreaming && onStopStreaming && (
            <Button
              variant="contained"
              size="small"
              startIcon={<StopIcon />}
              onClick={onStopStreaming}
              sx={{
                fontSize: typography.fontSize.sm,
                padding: `${spacing.xs}px ${spacing.sm}px`,
                borderRadius: borderRadius.sm,
                backgroundColor: colors.status.warning,
                color: '#fff',
                textTransform: 'none',
                '&:hover': {
                  backgroundColor: '#f57c00'
                }
              }}
              aria-label="停止生成回覆"
            >
              停止生成
            </Button>
          )}
        </Box>
      </Box>
    </Box>
  );
};

export default AgentMessage;
