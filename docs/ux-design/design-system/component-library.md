# Component Library

**版本**: 1.0.0
**狀態**: ✅ 已完成
**日期**: 2025-10-29
**負責人**: UI/UX Designer (BMad Method)

[← 返回 Design System](./README.md) | [← 返回 UX Design](../README.md)

---

## 📋 概述

Component Library 定義了 Semantic Kernel Agentic Framework 中所有 UI 組件的使用規範，基於 **Material-UI (MUI) v5**，包括通用組件和自定義業務組件。

### 組件分類

1. **基礎輸入組件** (Inputs) - Button, TextField, Select, Checkbox, etc.
2. **導航組件** (Navigation) - AppBar, Drawer, Tabs, Breadcrumbs
3. **數據展示組件** (Data Display) - Table, List, Card, Chip, Avatar
4. **反饋組件** (Feedback) - Alert, Dialog, Snackbar, Progress
5. **佈局組件** (Layout) - Container, Grid, Box, Stack
6. **業務組件** (Business) - AgentCard, ExecutionLog, MetricCard

---

## 🔘 基礎輸入組件 (Inputs)

### 1.1 Button - 按鈕

**使用場景**: 觸發操作、提交表單、導航

```tsx
import Button from '@mui/material/Button';

// Contained Button（實心按鈕）- 主要操作
<Button variant="contained" color="primary">
  Create Agent
</Button>

// Outlined Button（邊框按鈕）- 次要操作
<Button variant="outlined" color="secondary">
  Cancel
</Button>

// Text Button（文字按鈕）- 低優先級操作
<Button variant="text">
  Learn More
</Button>

// With Icon（帶圖標）
<Button variant="contained" startIcon={<AddIcon />}>
  Add New
</Button>

// Loading State（加載狀態）
<Button variant="contained" disabled>
  <CircularProgress size={20} color="inherit" sx={{ mr: 1 }} />
  Loading...
</Button>

// Size Variants（尺寸變體）
<Button size="small">Small</Button>
<Button size="medium">Medium</Button>
<Button size="large">Large</Button>
```

**使用規範**:
- **Contained**: 主要操作（如"創建"、"保存"、"提交"）
- **Outlined**: 次要操作（如"取消"、"重置"）
- **Text**: 低優先級操作（如"了解更多"、"查看詳情"）
- 同一頁面最多 1 個 Contained Primary Button
- 危險操作使用 `color="error"`

---

### 1.2 TextField - 文本輸入框

**使用場景**: 單行文本輸入、多行文本輸入

```tsx
import TextField from '@mui/material/TextField';

// Standard TextField
<TextField
  label="Agent Name"
  variant="outlined"
  fullWidth
  required
/>

// With Helper Text
<TextField
  label="Description"
  helperText="Brief description of your agent"
  variant="outlined"
  fullWidth
/>

// With Error State
<TextField
  label="Email"
  error
  helperText="Invalid email format"
  variant="outlined"
  fullWidth
/>

// Multiline (Textarea)
<TextField
  label="System Prompt"
  multiline
  rows={4}
  variant="outlined"
  fullWidth
/>

// With Start Adornment
<TextField
  label="API URL"
  InputProps={{
    startAdornment: <InputAdornment position="start">https://</InputAdornment>,
  }}
  variant="outlined"
  fullWidth
/>

// Disabled State
<TextField
  label="Agent ID"
  value="agent_12345"
  disabled
  variant="outlined"
  fullWidth
/>
```

**使用規範**:
- 使用 `variant="outlined"` 作為標準樣式
- 必填字段添加 `required` 屬性
- 錯誤狀態配合 `helperText` 顯示錯誤信息
- 長文本輸入使用 `multiline` 並設置 `rows`

---

### 1.3 Select - 下拉選擇器

**使用場景**: 從預定義選項中選擇

```tsx
import { FormControl, InputLabel, Select, MenuItem } from '@mui/material';

// Basic Select
<FormControl fullWidth>
  <InputLabel id="model-select-label">Model</InputLabel>
  <Select
    labelId="model-select-label"
    id="model-select"
    value={model}
    label="Model"
    onChange={handleChange}
  >
    <MenuItem value="gpt-4">GPT-4</MenuItem>
    <MenuItem value="gpt-3.5-turbo">GPT-3.5 Turbo</MenuItem>
    <MenuItem value="claude-3-opus">Claude 3 Opus</MenuItem>
    <MenuItem value="claude-3-sonnet">Claude 3 Sonnet</MenuItem>
  </Select>
</FormControl>

// Multiple Select
<FormControl fullWidth>
  <InputLabel>Plugins</InputLabel>
  <Select
    multiple
    value={selectedPlugins}
    onChange={handleChange}
    renderValue={(selected) => selected.join(', ')}
  >
    <MenuItem value="knowledge-base">Knowledge Base</MenuItem>
    <MenuItem value="code-interpreter">Code Interpreter</MenuItem>
    <MenuItem value="text-to-sql">Text-to-SQL</MenuItem>
  </Select>
</FormControl>

// With Groups
<FormControl fullWidth>
  <InputLabel>Category</InputLabel>
  <Select value={category} label="Category">
    <ListSubheader>OpenAI Models</ListSubheader>
    <MenuItem value="gpt-4">GPT-4</MenuItem>
    <MenuItem value="gpt-3.5">GPT-3.5</MenuItem>

    <ListSubheader>Anthropic Models</ListSubheader>
    <MenuItem value="claude-3">Claude 3</MenuItem>
  </Select>
</FormControl>
```

**使用規範**:
- 選項數量 < 5 時考慮使用 Radio Buttons
- 選項數量 > 10 時考慮使用 Autocomplete
- 多選場景使用 `multiple` 屬性
- 使用 `ListSubheader` 進行選項分組

---

### 1.4 Checkbox - 複選框

**使用場景**: 多選、開關設置

```tsx
import { Checkbox, FormControlLabel, FormGroup } from '@mui/material';

// Basic Checkbox
<FormControlLabel
  control={<Checkbox checked={checked} onChange={handleChange} />}
  label="Enable auto-save"
/>

// Checkbox Group
<FormGroup>
  <FormControlLabel
    control={<Checkbox checked={plugins.knowledgeBase} />}
    label="Knowledge Base"
  />
  <FormControlLabel
    control={<Checkbox checked={plugins.codeInterpreter} />}
    label="Code Interpreter"
  />
  <FormControlLabel
    control={<Checkbox checked={plugins.textToSql} />}
    label="Text-to-SQL"
  />
</FormGroup>

// Indeterminate State（部分選中）
<Checkbox
  checked={allChecked}
  indeterminate={someChecked}
  onChange={handleSelectAll}
/>

// With Custom Color
<Checkbox
  checked={checked}
  onChange={handleChange}
  color="success"
/>
```

**使用規範**:
- 用於非互斥的多選場景
- 互斥選擇使用 Radio Buttons
- 單個開關設置考慮使用 Switch
- 使用 `indeterminate` 表示部分選中狀態

---

### 1.5 Switch - 開關

**使用場景**: 開啟/關閉功能

```tsx
import { Switch, FormControlLabel } from '@mui/material';

// Basic Switch
<FormControlLabel
  control={<Switch checked={enabled} onChange={handleChange} />}
  label="Enable notifications"
/>

// Different Colors
<Switch checked={true} color="primary" />
<Switch checked={true} color="secondary" />
<Switch checked={true} color="success" />

// Size Variants
<Switch size="small" />
<Switch size="medium" />
```

**使用規範**:
- 用於即時生效的開關設置
- 需要確認的設置使用 Checkbox
- 標籤應明確說明開啟狀態的含義

---

## 🗂️ 導航組件 (Navigation)

### 2.1 AppBar - 應用欄

**使用場景**: 頁面頂部導航欄

```tsx
import { AppBar, Toolbar, IconButton, Typography, Button } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';

<AppBar position="fixed">
  <Toolbar>
    <IconButton
      edge="start"
      color="inherit"
      aria-label="menu"
      onClick={handleDrawerToggle}
    >
      <MenuIcon />
    </IconButton>

    <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
      Semantic Kernel Agentic Framework
    </Typography>

    <Button color="inherit">Dashboard</Button>
    <Button color="inherit">Agents</Button>
    <Button color="inherit">Workflows</Button>

    <IconButton color="inherit">
      <NotificationsIcon />
    </IconButton>

    <Avatar sx={{ ml: 2 }}>DC</Avatar>
  </Toolbar>
</AppBar>
```

**使用規範**:
- 固定在頂部使用 `position="fixed"`
- Logo 或標題靠左對齊
- 主要導航居中或靠左
- 用戶相關操作靠右對齊

---

### 2.2 Drawer - 側邊欄

**使用場景**: 側邊導航、側邊面板

```tsx
import { Drawer, List, ListItem, ListItemIcon, ListItemText } from '@mui/material';

// Permanent Drawer（永久側邊欄）
<Drawer
  variant="permanent"
  sx={{
    width: 240,
    flexShrink: 0,
    '& .MuiDrawer-paper': {
      width: 240,
      boxSizing: 'border-box',
    },
  }}
>
  <List>
    <ListItem button>
      <ListItemIcon><DashboardIcon /></ListItemIcon>
      <ListItemText primary="Dashboard" />
    </ListItem>
    <ListItem button>
      <ListItemIcon><AgentIcon /></ListItemIcon>
      <ListItemText primary="Agents" />
    </ListItem>
    <ListItem button>
      <ListItemIcon><WorkflowIcon /></ListItemIcon>
      <ListItemText primary="Workflows" />
    </ListItem>
  </List>
</Drawer>

// Temporary Drawer（臨時側邊欄）
<Drawer
  anchor="left"
  open={drawerOpen}
  onClose={handleDrawerClose}
>
  {/* 側邊欄內容 */}
</Drawer>
```

**使用規範**:
- Desktop 使用 `variant="permanent"`
- Mobile 使用 `variant="temporary"` 配合 `open` 屬性
- 寬度建議 240-280px
- 使用 ListItemIcon + ListItemText 組合

---

### 2.3 Tabs - 標籤頁

**使用場景**: 同級內容切換

```tsx
import { Tabs, Tab, Box } from '@mui/material';

function TabPanel(props) {
  const { children, value, index, ...other } = props;
  return (
    <div hidden={value !== index} {...other}>
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

<Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
  <Tabs value={value} onChange={handleChange}>
    <Tab label="Configuration" />
    <Tab label="Execution History" />
    <Tab label="Metrics" />
  </Tabs>
</Box>

<TabPanel value={value} index={0}>
  Configuration content...
</TabPanel>
<TabPanel value={value} index={1}>
  Execution History content...
</TabPanel>
<TabPanel value={value} index={2}>
  Metrics content...
</TabPanel>
```

**使用規範**:
- 用於同級內容的切換（不是頁面導航）
- 標籤數量建議 2-7 個
- 超過 7 個考慮使用下拉菜單
- 配合 TabPanel 管理內容顯示

---

### 2.4 Breadcrumbs - 面包屑

**使用場景**: 顯示當前位置層級

```tsx
import { Breadcrumbs, Link, Typography } from '@mui/material';
import NavigateNextIcon from '@mui/icons-material/NavigateNext';

<Breadcrumbs separator={<NavigateNextIcon fontSize="small" />}>
  <Link underline="hover" color="inherit" href="/">
    Home
  </Link>
  <Link underline="hover" color="inherit" href="/agents">
    Agents
  </Link>
  <Typography color="text.primary">Customer Support Agent</Typography>
</Breadcrumbs>
```

**使用規範**:
- 顯示層級不超過 5 層
- 當前頁面使用 Typography 不可點擊
- 其他層級使用 Link 可點擊
- 移動端考慮只顯示上一級

---

## 📋 數據展示組件 (Data Display)

### 3.1 Table - 表格

**使用場景**: 展示結構化數據

```tsx
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
} from '@mui/material';

<TableContainer component={Paper}>
  <Table>
    <TableHead>
      <TableRow>
        <TableCell>Agent Name</TableCell>
        <TableCell>Status</TableCell>
        <TableCell>Model</TableCell>
        <TableCell align="right">Executions</TableCell>
        <TableCell>Actions</TableCell>
      </TableRow>
    </TableHead>
    <TableBody>
      {rows.map((row) => (
        <TableRow key={row.id} hover>
          <TableCell>{row.name}</TableCell>
          <TableCell>
            <Chip label={row.status} color="success" size="small" />
          </TableCell>
          <TableCell>{row.model}</TableCell>
          <TableCell align="right">{row.executions}</TableCell>
          <TableCell>
            <IconButton size="small"><EditIcon /></IconButton>
            <IconButton size="small"><DeleteIcon /></IconButton>
          </TableCell>
        </TableRow>
      ))}
    </TableBody>
  </Table>

  <TablePagination
    rowsPerPageOptions={[10, 25, 50]}
    component="div"
    count={totalRows}
    rowsPerPage={rowsPerPage}
    page={page}
    onPageChange={handleChangePage}
    onRowsPerPageChange={handleChangeRowsPerPage}
  />
</TableContainer>
```

**使用規範**:
- 使用 `Paper` 包裹提供陰影
- 添加 `hover` 屬性改善交互
- 數字列使用 `align="right"`
- 大量數據必須使用分頁
- 操作列固定在最右側

---

### 3.2 Card - 卡片

**使用場景**: 展示內容單元

```tsx
import {
  Card,
  CardHeader,
  CardContent,
  CardActions,
  Avatar,
  IconButton,
  Typography,
  Button,
} from '@mui/material';

// Basic Card
<Card>
  <CardHeader
    avatar={<Avatar>A</Avatar>}
    action={<IconButton><MoreVertIcon /></IconButton>}
    title="Customer Support Agent"
    subheader="Last updated: 2 hours ago"
  />
  <CardContent>
    <Typography variant="body2" color="text.secondary">
      AI-powered customer support agent with knowledge base integration.
    </Typography>
  </CardContent>
  <CardActions>
    <Button size="small">Edit</Button>
    <Button size="small">View Details</Button>
  </CardActions>
</Card>

// Card with Media
<Card>
  <CardMedia
    component="img"
    height="140"
    image="/agent-thumbnail.jpg"
    alt="Agent thumbnail"
  />
  <CardContent>
    <Typography gutterBottom variant="h5">
      Data Analysis Agent
    </Typography>
    <Typography variant="body2" color="text.secondary">
      Analyze data and generate insights automatically.
    </Typography>
  </CardContent>
</Card>
```

**使用規範**:
- 卡片高度盡量一致（Grid 佈局時）
- 使用 `elevation` 控制陰影深度
- 交互卡片添加 hover 效果
- 標題使用 h5 或 h6

---

### 3.3 Chip - 標籤

**使用場景**: 標籤、狀態標識

```tsx
import Chip from '@mui/material/Chip';

// Basic Chips
<Chip label="Active" color="success" />
<Chip label="Pending" color="warning" />
<Chip label="Failed" color="error" />
<Chip label="Draft" color="default" />

// Outlined Variant
<Chip label="Tag" variant="outlined" />

// With Icon
<Chip label="Knowledge Base" icon={<StorageIcon />} />

// Clickable Chip
<Chip label="Click me" onClick={handleClick} />

// Deletable Chip
<Chip label="Python" onDelete={handleDelete} />

// Size Variants
<Chip label="Small" size="small" />
<Chip label="Medium" size="medium" />
```

**使用規範**:
- 狀態使用對應語義色彩
- 可刪除標籤添加 `onDelete`
- 可點擊標籤添加 `onClick`
- 小空間使用 `size="small"`

---

### 3.4 Avatar - 頭像

**使用場景**: 用戶頭像、圖標占位

```tsx
import { Avatar, AvatarGroup } from '@mui/material';

// Text Avatar
<Avatar>DC</Avatar>

// Image Avatar
<Avatar alt="David Chen" src="/avatars/david.jpg" />

// Icon Avatar
<Avatar><PersonIcon /></Avatar>

// Colored Avatar
<Avatar sx={{ bgcolor: deepOrange[500] }}>DC</Avatar>

// Size Variants
<Avatar sx={{ width: 24, height: 24 }}>S</Avatar>
<Avatar>M</Avatar>
<Avatar sx={{ width: 56, height: 56 }}>L</Avatar>

// Avatar Group
<AvatarGroup max={4}>
  <Avatar alt="User 1" src="/avatar1.jpg" />
  <Avatar alt="User 2" src="/avatar2.jpg" />
  <Avatar alt="User 3" src="/avatar3.jpg" />
  <Avatar alt="User 4" src="/avatar4.jpg" />
  <Avatar alt="User 5" src="/avatar5.jpg" />
</AvatarGroup>
```

**使用規範**:
- 優先使用圖片頭像
- 無圖片時顯示首字母
- 使用 AvatarGroup 展示多個用戶
- 設置合適的 `alt` 文字

---

## 💬 反饋組件 (Feedback)

### 4.1 Alert - 警告提示

**使用場景**: 信息提示、警告、錯誤

```tsx
import { Alert, AlertTitle } from '@mui/material';

// Severity Variants
<Alert severity="success">This is a success alert!</Alert>
<Alert severity="info">This is an info alert!</Alert>
<Alert severity="warning">This is a warning alert!</Alert>
<Alert severity="error">This is an error alert!</Alert>

// With Title
<Alert severity="error">
  <AlertTitle>Error</AlertTitle>
  This is an error alert — <strong>check it out!</strong>
</Alert>

// Outlined Variant
<Alert variant="outlined" severity="warning">
  This is an outlined warning alert!
</Alert>

// Filled Variant
<Alert variant="filled" severity="success">
  This is a filled success alert!
</Alert>

// With Action
<Alert
  severity="warning"
  action={
    <Button color="inherit" size="small">
      UNDO
    </Button>
  }
>
  This action can be undone.
</Alert>

// Closeable
<Alert
  severity="info"
  onClose={() => {}}
>
  You can close this alert!
</Alert>
```

**使用規範**:
- 根據信息重要性選擇 severity
- 重要信息使用 `variant="filled"`
- 可關閉的提示添加 `onClose`
- 長時間顯示的提示使用 `variant="outlined"`

---

### 4.2 Dialog - 對話框

**使用場景**: 確認操作、表單輸入、信息展示

```tsx
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from '@mui/material';

// Basic Dialog
<Dialog open={open} onClose={handleClose}>
  <DialogTitle>Delete Agent?</DialogTitle>
  <DialogContent>
    <DialogContentText>
      Are you sure you want to delete this agent? This action cannot be undone.
    </DialogContentText>
  </DialogContent>
  <DialogActions>
    <Button onClick={handleClose}>Cancel</Button>
    <Button onClick={handleDelete} color="error" autoFocus>
      Delete
    </Button>
  </DialogActions>
</Dialog>

// Form Dialog
<Dialog open={open} onClose={handleClose}>
  <DialogTitle>Create New Agent</DialogTitle>
  <DialogContent>
    <TextField
      autoFocus
      margin="dense"
      label="Agent Name"
      fullWidth
      variant="outlined"
    />
    <TextField
      margin="dense"
      label="Description"
      fullWidth
      variant="outlined"
      multiline
      rows={3}
    />
  </DialogContent>
  <DialogActions>
    <Button onClick={handleClose}>Cancel</Button>
    <Button onClick={handleCreate} variant="contained">
      Create
    </Button>
  </DialogActions>
</Dialog>

// Full Screen Dialog
<Dialog fullScreen open={open} onClose={handleClose}>
  <AppBar sx={{ position: 'relative' }}>
    <Toolbar>
      <IconButton edge="start" color="inherit" onClick={handleClose}>
        <CloseIcon />
      </IconButton>
      <Typography sx={{ ml: 2, flex: 1 }} variant="h6">
        Workflow Editor
      </Typography>
      <Button autoFocus color="inherit" onClick={handleSave}>
        Save
      </Button>
    </Toolbar>
  </AppBar>
  {/* Dialog Content */}
</Dialog>
```

**使用規範**:
- 簡單確認使用基礎 Dialog
- 表單輸入使用 Form Dialog
- 複雜內容使用 Full Screen Dialog
- 危險操作按鈕使用 `color="error"`
- 主要操作按鈕添加 `autoFocus`

---

### 4.3 Snackbar - 消息通知

**使用場景**: 短暫消息提示、操作反饋

```tsx
import { Snackbar, Alert } from '@mui/material';

// Basic Snackbar
<Snackbar
  open={open}
  autoHideDuration={6000}
  onClose={handleClose}
  message="Agent created successfully"
/>

// Snackbar with Alert
<Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
  <Alert onClose={handleClose} severity="success" sx={{ width: '100%' }}>
    Agent created successfully!
  </Alert>
</Snackbar>

// With Action
<Snackbar
  open={open}
  autoHideDuration={6000}
  onClose={handleClose}
  message="Agent deleted"
  action={
    <Button color="secondary" size="small" onClick={handleUndo}>
      UNDO
    </Button>
  }
/>

// Position Variants
<Snackbar
  anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
  open={open}
  onClose={handleClose}
  message="Message"
/>
```

**使用規範**:
- 成功操作使用 Success Alert
- 錯誤操作使用 Error Alert
- 自動關閉時間 4-6 秒
- 可撤銷操作添加 UNDO 按鈕
- 默認位置左下角，避免遮擋主要內容

---

### 4.4 Progress - 進度指示

**使用場景**: 加載狀態、進度展示

```tsx
import { CircularProgress, LinearProgress, Box, Typography } from '@mui/material';

// Circular Progress (不確定)
<CircularProgress />

// Circular Progress (確定)
<Box sx={{ position: 'relative', display: 'inline-flex' }}>
  <CircularProgress variant="determinate" value={progress} />
  <Box
    sx={{
      top: 0,
      left: 0,
      bottom: 0,
      right: 0,
      position: 'absolute',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
    }}
  >
    <Typography variant="caption" component="div" color="text.secondary">
      {`${Math.round(progress)}%`}
    </Typography>
  </Box>
</Box>

// Linear Progress (不確定)
<LinearProgress />

// Linear Progress (確定)
<LinearProgress variant="determinate" value={progress} />

// With Buffer
<LinearProgress variant="buffer" value={progress} valueBuffer={buffer} />

// Color Variants
<CircularProgress color="secondary" />
<CircularProgress color="success" />
<CircularProgress color="inherit" />
```

**使用規範**:
- 短時間加載（<3秒）使用 Circular
- 長時間加載（>3秒）使用 Linear 並顯示百分比
- 頁面加載使用居中的 Circular
- 按鈕加載使用小尺寸 Circular

---

## 📐 佈局組件 (Layout)

### 5.1 Container - 容器

**使用場景**: 頁面內容容器

```tsx
import Container from '@mui/material/Container';

// Fixed Max Width
<Container maxWidth="sm">{/* 600px */}</Container>
<Container maxWidth="md">{/* 960px */}</Container>
<Container maxWidth="lg">{/* 1280px */}</Container>
<Container maxWidth="xl">{/* 1920px */}</Container>

// Full Width
<Container maxWidth={false}>{/* 100% */}</Container>

// With Padding
<Container maxWidth="lg" sx={{ py: 4 }}>
  {/* Content */}
</Container>
```

**使用規範**:
- 頁面主內容使用 `maxWidth="lg"` 或 `maxWidth="xl"`
- 表單頁面使用 `maxWidth="sm"` 或 `maxWidth="md"`
- 添加垂直 padding (`py: 4`)

---

### 5.2 Grid - 網格系統

**使用場景**: 響應式佈局

```tsx
import { Grid } from '@mui/material';

// Basic Grid
<Grid container spacing={3}>
  <Grid item xs={12} sm={6} md={4}>
    <Card>Card 1</Card>
  </Grid>
  <Grid item xs={12} sm={6} md={4}>
    <Card>Card 2</Card>
  </Grid>
  <Grid item xs={12} sm={6} md={4}>
    <Card>Card 3</Card>
  </Grid>
</Grid>

// Nested Grid
<Grid container spacing={2}>
  <Grid item xs={12} md={8}>
    <Paper>Main Content</Paper>
  </Grid>
  <Grid item xs={12} md={4}>
    <Grid container spacing={2}>
      <Grid item xs={12}>
        <Paper>Sidebar Item 1</Paper>
      </Grid>
      <Grid item xs={12}>
        <Paper>Sidebar Item 2</Paper>
      </Grid>
    </Grid>
  </Grid>
</Grid>

// Auto Layout
<Grid container spacing={2}>
  <Grid item xs>
    <Paper>Auto</Paper>
  </Grid>
  <Grid item xs={6}>
    <Paper>xs=6</Paper>
  </Grid>
  <Grid item xs>
    <Paper>Auto</Paper>
  </Grid>
</Grid>
```

**使用規範**:
- 使用 12 列網格系統
- `spacing` 值建議 2-4
- 響應式設計使用 xs/sm/md/lg/xl
- 移動端優先（從 xs 開始定義）

---

### 5.3 Stack - 堆疊佈局

**使用場景**: 單向排列元素

```tsx
import Stack from '@mui/material/Stack';

// Vertical Stack (default)
<Stack spacing={2}>
  <Button variant="contained">Button 1</Button>
  <Button variant="contained">Button 2</Button>
  <Button variant="contained">Button 3</Button>
</Stack>

// Horizontal Stack
<Stack direction="row" spacing={2}>
  <Button variant="outlined">Cancel</Button>
  <Button variant="contained">Save</Button>
</Stack>

// With Divider
<Stack
  direction="row"
  divider={<Divider orientation="vertical" flexItem />}
  spacing={2}
>
  <Item>Item 1</Item>
  <Item>Item 2</Item>
  <Item>Item 3</Item>
</Stack>

// Responsive Direction
<Stack
  direction={{ xs: 'column', sm: 'row' }}
  spacing={{ xs: 1, sm: 2, md: 4 }}
>
  <Item>Item 1</Item>
  <Item>Item 2</Item>
</Stack>
```

**使用規範**:
- 簡單的單向排列優先使用 Stack
- 複雜佈局使用 Grid
- 表單操作按鈕使用水平 Stack
- 卡片內容使用垂直 Stack

---

## 🎨 業務組件 (Business Components)

### 6.1 AgentCard - Agent 卡片

**自定義業務組件** - 展示 Agent 信息

```tsx
import { Card, CardHeader, CardContent, CardActions, Avatar, Chip, Button } from '@mui/material';

interface AgentCardProps {
  agent: {
    id: string;
    name: string;
    description: string;
    model: string;
    status: 'active' | 'inactive' | 'error';
    executions: number;
    lastRun: Date;
  };
  onEdit: (id: string) => void;
  onDelete: (id: string) => void;
}

function AgentCard({ agent, onEdit, onDelete }: AgentCardProps) {
  const statusColors = {
    active: 'success',
    inactive: 'default',
    error: 'error',
  };

  return (
    <Card>
      <CardHeader
        avatar={
          <Avatar sx={{ bgcolor: 'primary.main' }}>
            {agent.name.charAt(0).toUpperCase()}
          </Avatar>
        }
        action={
          <Chip
            label={agent.status}
            color={statusColors[agent.status]}
            size="small"
          />
        }
        title={agent.name}
        subheader={`Model: ${agent.model} | Executions: ${agent.executions}`}
      />
      <CardContent>
        <Typography variant="body2" color="text.secondary">
          {agent.description}
        </Typography>
        <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
          Last run: {format(agent.lastRun, 'MMM dd, yyyy HH:mm')}
        </Typography>
      </CardContent>
      <CardActions>
        <Button size="small" onClick={() => onEdit(agent.id)}>
          Edit
        </Button>
        <Button size="small" onClick={() => onDelete(agent.id)}>
          Delete
        </Button>
        <Button size="small">View Details</Button>
      </CardActions>
    </Card>
  );
}
```

**使用場景**: Agent 列表頁、Dashboard

---

### 6.2 ExecutionLog - 執行日誌

**自定義業務組件** - 展示執行日誌

```tsx
import { Paper, List, ListItem, ListItemText, Typography, Chip } from '@mui/material';

interface ExecutionLogProps {
  logs: {
    timestamp: Date;
    level: 'info' | 'warning' | 'error';
    message: string;
  }[];
}

function ExecutionLog({ logs }: ExecutionLogProps) {
  const levelColors = {
    info: 'info',
    warning: 'warning',
    error: 'error',
  };

  return (
    <Paper sx={{ maxHeight: 400, overflow: 'auto' }}>
      <List dense>
        {logs.map((log, index) => (
          <ListItem key={index}>
            <ListItemText
              primary={
                <>
                  <Chip
                    label={log.level.toUpperCase()}
                    color={levelColors[log.level]}
                    size="small"
                    sx={{ mr: 1 }}
                  />
                  <Typography component="span" variant="body2">
                    {format(log.timestamp, 'HH:mm:ss')}
                  </Typography>
                </>
              }
              secondary={log.message}
            />
          </ListItem>
        ))}
      </List>
    </Paper>
  );
}
```

**使用場景**: Agent 執行詳情頁、Monitoring Dashboard

---

### 6.3 MetricCard - 指標卡片

**自定義業務組件** - 展示關鍵指標

```tsx
import { Card, CardContent, Typography, Box } from '@mui/material';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';

interface MetricCardProps {
  title: string;
  value: string | number;
  change?: {
    value: number;
    direction: 'up' | 'down';
  };
  icon?: React.ReactNode;
}

function MetricCard({ title, value, change, icon }: MetricCardProps) {
  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box>
            <Typography color="text.secondary" gutterBottom variant="overline">
              {title}
            </Typography>
            <Typography variant="h4" component="div">
              {value}
            </Typography>
            {change && (
              <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
                {change.direction === 'up' ? (
                  <TrendingUpIcon color="success" fontSize="small" />
                ) : (
                  <TrendingDownIcon color="error" fontSize="small" />
                )}
                <Typography
                  variant="body2"
                  color={change.direction === 'up' ? 'success.main' : 'error.main'}
                  sx={{ ml: 0.5 }}
                >
                  {change.value}%
                </Typography>
              </Box>
            )}
          </Box>
          {icon && (
            <Box
              sx={{
                bgcolor: 'primary.main',
                color: 'primary.contrastText',
                borderRadius: 2,
                p: 1,
              }}
            >
              {icon}
            </Box>
          )}
        </Box>
      </CardContent>
    </Card>
  );
}
```

**使用場景**: Dashboard、Monitoring 頁面

---

### 6.4 WorkflowNode - 工作流節點

**自定義業務組件** - 工作流編輯器節點

```tsx
import { Paper, Typography, Box, IconButton } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import SettingsIcon from '@mui/icons-material/Settings';

interface WorkflowNodeProps {
  node: {
    id: string;
    type: string;
    label: string;
    icon: React.ReactNode;
  };
  onDelete: (id: string) => void;
  onConfigure: (id: string) => void;
}

function WorkflowNode({ node, onDelete, onConfigure }: WorkflowNodeProps) {
  return (
    <Paper
      elevation={2}
      sx={{
        p: 2,
        minWidth: 200,
        cursor: 'pointer',
        '&:hover': {
          elevation: 4,
          bgcolor: 'action.hover',
        },
      }}
    >
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          {node.icon}
          <Typography variant="subtitle1">{node.label}</Typography>
        </Box>
        <Box>
          <IconButton size="small" onClick={() => onConfigure(node.id)}>
            <SettingsIcon fontSize="small" />
          </IconButton>
          <IconButton size="small" onClick={() => onDelete(node.id)}>
            <DeleteIcon fontSize="small" />
          </IconButton>
        </Box>
      </Box>
      <Typography variant="caption" color="text.secondary">
        Type: {node.type}
      </Typography>
    </Paper>
  );
}
```

**使用場景**: Workflow Editor

---

## ✅ 驗收標準

### Component Library 完成標準

**基礎輸入組件**:
- [x] Button 組件（3 種變體、尺寸、狀態）
- [x] TextField 組件（標準、錯誤、多行、禁用）
- [x] Select 組件（單選、多選、分組）
- [x] Checkbox 組件（基礎、分組、部分選中）
- [x] Switch 組件（基礎、顏色、尺寸）

**導航組件**:
- [x] AppBar 組件（固定、帶菜單、用戶信息）
- [x] Drawer 組件（永久、臨時）
- [x] Tabs 組件（基礎、配合 TabPanel）
- [x] Breadcrumbs 組件（層級導航）

**數據展示組件**:
- [x] Table 組件（基礎、分頁、排序）
- [x] Card 組件（基礎、帶媒體）
- [x] Chip 組件（狀態、標籤、可刪除）
- [x] Avatar 組件（文字、圖片、分組）

**反饋組件**:
- [x] Alert 組件（4 種嚴重性、變體）
- [x] Dialog 組件（確認、表單、全屏）
- [x] Snackbar 組件（消息、操作）
- [x] Progress 組件（循環、線性）

**佈局組件**:
- [x] Container 組件（最大寬度、響應式）
- [x] Grid 組件（12 列系統、嵌套）
- [x] Stack 組件（垂直、水平、響應式）

**業務組件**:
- [x] AgentCard 組件（完整實現）
- [x] ExecutionLog 組件（完整實現）
- [x] MetricCard 組件（完整實現）
- [x] WorkflowNode 組件（完整實現）

---

**文檔狀態**: ✅ 已完成
**審查人**: UI/UX Designer
**審查日期**: 2025-10-29
**版本**: 1.0.0

---

[← 返回 Design System](./README.md) | [← 返回 UX Design](../README.md)
