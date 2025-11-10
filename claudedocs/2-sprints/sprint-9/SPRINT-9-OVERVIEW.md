# Sprint 9 概覽 - Structured Data 上傳

**Sprint**: Sprint 9 | **週次**: Week 25-27 | **日期**: 2026-04-27 ~ 2026-05-17 (21 days) | **狀態**: ⏳ 未開始

## 🎯 Sprint 目標
實現結構化數據 (Excel/CSV) 上傳和處理,為 Text-to-SQL 奠定基礎。同時穿插 P1 功能開發。

## 📊 User Stories (13 SP)

| User Story | SP | 模組 | 優先級 |
|-----------|----|----- |-------|
| US 5.4 - Excel/CSV 數據源上傳 ⭐ | 5 | Module 5 | P0 |
| US 6.4 - 代碼塊展示與複製 ⭐ | 3 | Module 6 | P0 |
| US 1.6 - Persona 模板庫 (P1) | 5 | Module 1 | P1 |

## 📋 核心技術任務

### US 5.4 - Excel/CSV 上傳
- Excel 解析: EPPlus 套件
- CSV 解析: CsvHelper 套件
- Schema 自動推斷 (Column Types, Stats)
- 數據預覽組件

### US 6.4 - 代碼塊展示
- react-syntax-highlighter 集成
- Copy to Clipboard 功能
- 多語言語法高亮

### US 1.6 - Persona 模板庫 (P1)
- 10+ 預定義模板
- 模板分類和標籤
- 模板搜索功能

## 🎯 驗收標準
- ✅ Excel/CSV 解析準確率 ≥ 98%
- ✅ Schema 推斷準確率 ≥ 95%
- ✅ 代碼複製功能正常
- ✅ 模板庫包含 10+ 模板

## 🔗 依賴
- **依賴**: Sprint 5 (知識庫基礎)
- **後續**: Sprint 10 (Text-to-SQL)

**狀態**: ⏳ 規劃中
