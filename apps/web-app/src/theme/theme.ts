/**
 * MUI Theme Configuration
 * 基於 Design Tokens 創建 Material-UI v7 主題
 */

import { createTheme } from '@mui/material/styles';
import { colors, spacing, typography, borderRadius, shadows, breakpoints } from './tokens';

// ============================================
// MUI Theme Options
// ============================================

const themeOptions = {
  // Color Palette
  palette: {
    mode: 'light',
    primary: {
      main: colors.primary.main,
      light: colors.primary.light,
      dark: colors.primary.dark,
      contrastText: colors.primary.contrastText
    },
    secondary: {
      main: colors.secondary.main,
      light: colors.secondary.light,
      dark: colors.secondary.dark,
      contrastText: colors.secondary.contrastText
    },
    background: {
      default: colors.background.default,
      paper: colors.background.paper
    },
    text: {
      primary: colors.text.primary,
      secondary: colors.text.secondary,
      disabled: colors.text.disabled
    },
    divider: colors.divider,
    success: {
      main: colors.status.success
    },
    warning: {
      main: colors.status.warning
    },
    error: {
      main: colors.status.error
    },
    info: {
      main: colors.status.info
    }
  },

  // Typography
  typography: {
    fontFamily: typography.fontFamily.base,
    fontSize: 16, // base font size in px
    fontWeightLight: 300,
    fontWeightRegular: typography.fontWeight.regular,
    fontWeightMedium: typography.fontWeight.medium,
    fontWeightBold: typography.fontWeight.bold,

    h1: {
      fontSize: typography.fontSize.xxl,
      fontWeight: typography.fontWeight.semibold,
      lineHeight: typography.lineHeight.tight
    },
    h2: {
      fontSize: typography.fontSize.xl,
      fontWeight: typography.fontWeight.semibold,
      lineHeight: typography.lineHeight.tight
    },
    h3: {
      fontSize: typography.fontSize.lg,
      fontWeight: typography.fontWeight.semibold,
      lineHeight: typography.lineHeight.tight
    },
    body1: {
      fontSize: typography.fontSize.md,
      lineHeight: typography.lineHeight.normal
    },
    body2: {
      fontSize: typography.fontSize.sm,
      lineHeight: typography.lineHeight.normal
    },
    caption: {
      fontSize: typography.fontSize.xs,
      lineHeight: typography.lineHeight.normal
    },
    button: {
      fontSize: typography.fontSize.md,
      fontWeight: typography.fontWeight.medium,
      textTransform: 'none' // 不自動轉大寫
    }
  },

  // Spacing
  spacing: spacing.sm, // 使用 8px 作為基礎單位

  // Shape (Border Radius)
  shape: {
    borderRadius: parseInt(borderRadius.md) // 8px
  },

  // Breakpoints
  breakpoints: {
    values: {
      xs: breakpoints.xs,
      sm: breakpoints.sm,
      md: breakpoints.md,
      lg: breakpoints.lg,
      xl: breakpoints.xl
    }
  },

  // Shadows
  shadows: [
    shadows.none,
    shadows.sm,
    shadows.sm,
    shadows.md,
    shadows.md,
    shadows.md,
    shadows.md,
    shadows.md,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.lg,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl,
    shadows.xl
  ],

  // Component Overrides
  components: {
    // Button
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: borderRadius.sm,
          padding: `${spacing.sm}px ${spacing.lg}px`,
          textTransform: 'none',
          fontWeight: typography.fontWeight.medium
        },
        contained: {
          boxShadow: shadows.sm,
          '&:hover': {
            boxShadow: shadows.md
          }
        }
      }
    },

    // TextField
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            borderRadius: borderRadius.md,
            backgroundColor: colors.background.input,
            '&:hover': {
              backgroundColor: colors.background.paper
            },
            '&.Mui-focused': {
              backgroundColor: colors.background.default
            }
          }
        }
      }
    },

    // Paper
    MuiPaper: {
      styleOverrides: {
        root: {
          borderRadius: borderRadius.md
        },
        elevation1: {
          boxShadow: shadows.sm
        },
        elevation2: {
          boxShadow: shadows.md
        },
        elevation3: {
          boxShadow: shadows.lg
        }
      }
    },

    // Card
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: borderRadius.md,
          boxShadow: shadows.md
        }
      }
    },

    // IconButton
    MuiIconButton: {
      styleOverrides: {
        root: {
          borderRadius: borderRadius.sm,
          transition: 'all 150ms cubic-bezier(0.4, 0, 0.2, 1)',
          '&:hover': {
            backgroundColor: colors.background.paper
          }
        }
      }
    },

    // Divider
    MuiDivider: {
      styleOverrides: {
        root: {
          borderColor: colors.divider
        }
      }
    }
  },

  // Transitions
  transitions: {
    duration: {
      shortest: 150,
      shorter: 200,
      short: 250,
      standard: 300,
      complex: 375,
      enteringScreen: 225,
      leavingScreen: 195
    },
    easing: {
      easeInOut: 'cubic-bezier(0.4, 0, 0.2, 1)',
      easeOut: 'cubic-bezier(0.0, 0, 0.2, 1)',
      easeIn: 'cubic-bezier(0.4, 0, 1, 1)',
      sharp: 'cubic-bezier(0.4, 0, 0.6, 1)'
    }
  }
};

// ============================================
// Create Theme Instance
// ============================================

export const theme = createTheme(themeOptions);

export default theme;
