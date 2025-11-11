/**
 * ChatInput - 輸入區域
 * 支援多行輸入、Enter 發送、字數統計
 */

import React, { useRef, useEffect } from 'react';
import { Box, Button, Typography } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import { spacing, colors, typography, borderRadius, layoutSizes } from '../../../theme/tokens';
import { CHAT_CONFIG } from '../../../config/api';

interface ChatInputProps {
  value: string;
  charCount: number;
  maxChars?: number;
  isSending: boolean;
  onChange: (value: string) => void;
  onSend: () => void;
  onClear?: () => void;
}

export const ChatInput: React.FC<ChatInputProps> = ({
  value,
  charCount,
  maxChars = CHAT_CONFIG.maxInputLength,
  isSending,
  onChange,
  onSend
}) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  /**
   * 自動調整 textarea 高度
   */
  useEffect(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = 'auto';
      const scrollHeight = textareaRef.current.scrollHeight;
      const maxHeight = layoutSizes.chatInput.maxHeight;
      textareaRef.current.style.height = `${Math.min(scrollHeight, maxHeight)}px`;
    }
  }, [value]);

  /**
   * 處理鍵盤事件
   * Enter: 發送訊息
   * Shift+Enter: 插入換行
   */
  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault(); // 防止換行
      if (canSend) {
        onSend();
      }
    }
    // Shift+Enter 允許換行 (不需特殊處理)
  };

  /**
   * 是否可以發送
   */
  const canSend = value.trim().length > 0 && charCount <= maxChars && !isSending;

  /**
   * 字數狀態
   */
  const isWarning = charCount > CHAT_CONFIG.warningThreshold && charCount <= maxChars;
  const isError = charCount > maxChars;

  return (
    <Box
      sx={{
        padding: spacing.lg,
        borderTop: `1px solid ${colors.divider}`,
        backgroundColor: colors.background.default
      }}
    >
      {/* Textarea Container */}
      <Box
        sx={{
          position: 'relative',
          marginBottom: spacing.sm
        }}
      >
        <textarea
          ref={textareaRef}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="輸入訊息... (Enter 發送,Shift+Enter 換行)"
          disabled={isSending}
          aria-label="輸入訊息"
          aria-describedby="char-counter"
          aria-multiline="true"
          style={{
            width: '100%',
            minHeight: layoutSizes.chatInput.minHeight,
            maxHeight: layoutSizes.chatInput.maxHeight,
            padding: spacing.md,
            border: `1px solid ${isError ? colors.status.error : colors.border}`,
            borderRadius: borderRadius.md,
            fontSize: typography.fontSize.md,
            fontFamily: typography.fontFamily.base,
            lineHeight: typography.lineHeight.normal,
            resize: 'none',
            overflowY: 'auto',
            backgroundColor: isSending ? colors.background.paper : colors.background.input,
            color: colors.text.primary,
            outline: 'none',
            transition: 'all 150ms ease-in-out',
            boxSizing: 'border-box'
          }}
          onFocus={(e) => {
            e.target.style.borderColor = colors.primary.main;
            e.target.style.boxShadow = `0 0 0 3px rgba(25, 118, 210, 0.1)`;
            e.target.style.backgroundColor = colors.background.default;
          }}
          onBlur={(e) => {
            e.target.style.borderColor = isError ? colors.status.error : colors.border;
            e.target.style.boxShadow = 'none';
            e.target.style.backgroundColor = colors.background.input;
          }}
        />
      </Box>

      {/* Footer: Char Counter + Send Button */}
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}
      >
        {/* Character Counter */}
        <Typography
          id="char-counter"
          variant="caption"
          sx={{
            fontSize: typography.fontSize.xs,
            color: isError
              ? colors.status.error
              : isWarning
              ? colors.status.warning
              : colors.text.secondary,
            fontWeight: isError ? typography.fontWeight.medium : typography.fontWeight.regular
          }}
          aria-live="polite"
        >
          {charCount} / {maxChars}
          {isError && ' (超過字數限制)'}
          {isWarning && ' (接近限制)'}
        </Typography>

        {/* Send Button */}
        <Button
          variant="contained"
          color="primary"
          endIcon={<SendIcon />}
          onClick={onSend}
          disabled={!canSend}
          sx={{
            height: 40,
            padding: `0 ${spacing.lg}px`,
            borderRadius: borderRadius.sm,
            textTransform: 'none',
            fontWeight: typography.fontWeight.medium
          }}
          aria-label="發送訊息"
          aria-disabled={!canSend}
        >
          發送
        </Button>
      </Box>
    </Box>
  );
};

export default ChatInput;
