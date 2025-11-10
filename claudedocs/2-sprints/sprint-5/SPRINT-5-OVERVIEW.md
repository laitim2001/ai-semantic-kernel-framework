# Sprint 5 概覽 - Knowledge 管理基礎

**Sprint 編號**: Sprint 5
**週次**: Week 13-15
**計劃日期**: 2026-02-02 ~ 2026-02-22 (21 days)
**實際日期**: TBD
**狀態**: ⏳ **未開始**

---

## 🎯 Sprint 目標

建立 **Knowledge Base 知識庫管理基礎**,實現文件上傳、解析、Chunking 和管理功能,為後續 RAG (Retrieval-Augmented Generation) 奠定基礎。

**關鍵交付物**:
1. ⏳ 知識庫文件上傳與存儲 (Azure Blob Storage)
2. ⏳ 多格式文件解析 (PDF, DOCX, TXT, MD)
3. ⏳ 文本 Chunking 策略
4. ⏳ 知識庫文件 CRUD 管理
5. ⏳ 重新索引機制

---

## 📊 User Stories

### 計劃 User Stories

| User Story | Story Points | 計劃天數 | 模組 | 狀態 | 優先級 |
|-----------|-------------|---------|------|------|-------|
| **US 5.1** - 知識庫文件上傳與處理 | 5 SP | 9 days | Module 5 | ⏳ 未開始 | P0 |
| **US 5.3** - 知識庫管理與更新 | 5 SP | 9 days | Module 5 | ⏳ 未開始 | P0 |

**總計**: **10 SP** (14 days 開發 + 7 days 測試/文檔)

---

## 📋 詳細技術任務

### US 5.1 - 知識庫文件上傳與處理 (5 SP)

#### Azure Blob Storage 集成
```yaml
配置:
  - Azure Storage Account 創建
  - Blob Container 配置 (knowledge-base)
  - SAS Token 管理
  - 訪問策略設置

NuGet 套件:
  - Azure.Storage.Blobs (v12.x)
  - Azure.Identity (v1.x)
```

#### 文件上傳 API
```yaml
POST /api/v1/knowledge-base/files:
  - Multipart Form Upload
  - Request:
      file: IFormFile
      name: string (optional)
      description: string (optional)
      agentId: Guid (關聯到 Agent)
  - Response:
      fileId: Guid
      fileName: string
      fileSize: number
      uploadedAt: DateTime
      status: 'processing' | 'completed' | 'failed'

支持格式:
  - PDF (.pdf)
  - Word (.docx)
  - Text (.txt)
  - Markdown (.md)

文件大小限制:
  - 單個文件: 50MB
  - 總存儲: 10GB per Agent
```

#### 文件解析
```yaml
PDF 解析:
  - NuGet: PdfPig (v0.1.8)
  - 提取文本內容
  - 保留基本格式 (段落、列表)
  - 處理表格 (轉換為 Markdown)

DOCX 解析:
  - NuGet: DocumentFormat.OpenXml (v3.x)
  - 提取文本和格式
  - 保留標題層級
  - 處理嵌入圖片 (提取描述)

TXT/MD 解析:
  - 直接讀取
  - UTF-8 編碼支持
  - Markdown 保留格式
```

#### Chunking 策略
```yaml
Chunking 算法:
  - Sentence-based Chunking (基於句子)
  - Paragraph-based Chunking (基於段落)
  - Semantic Chunking (基於語義)

參數配置:
  - chunk_size: 512 tokens (default)
  - chunk_overlap: 64 tokens (default)
  - max_chunk_size: 1024 tokens

Metadata 保留:
  - source_file: 原始文件名
  - page_number: 頁碼 (PDF)
  - chunk_index: Chunk 序號
  - chunk_type: 'sentence' | 'paragraph' | 'semantic'
```

#### 數據模型
```csharp
KnowledgeBaseFile:
  - Id (Guid)
  - AgentId (Guid) - 關聯 Agent
  - FileName (string)
  - FileSize (long) - bytes
  - FileType (string) - 'pdf', 'docx', 'txt', 'md'
  - BlobUri (string) - Azure Blob URI
  - Status (string) - 'processing', 'completed', 'failed'
  - ProcessedAt (DateTime?)
  - CreatedAt (DateTime)
  - CreatedBy (string)

KnowledgeBaseChunk:
  - Id (Guid)
  - FileId (Guid)
  - Content (string) - Chunk 文本內容
  - ChunkIndex (int)
  - Metadata (JSON) - {page, source, type, etc.}
  - CreatedAt (DateTime)
```

---

### US 5.3 - 知識庫管理與更新 (5 SP)

#### 知識庫 CRUD API
```yaml
GET /api/v1/knowledge-base/files:
  - 查詢 Agent 的所有知識庫文件
  - 篩選: agentId, status, fileType
  - 排序: uploadedAt, fileSize, fileName
  - 分頁: pageIndex, pageSize

GET /api/v1/knowledge-base/files/{id}:
  - 獲取文件詳情
  - Response:
      file: File 對象
      chunks_count: number
      processed_chunks: number

PUT /api/v1/knowledge-base/files/{id}:
  - 更新文件元數據
  - Request: { name, description }

DELETE /api/v1/knowledge-base/files/{id}:
  - 刪除文件 (軟刪除)
  - 同時刪除相關 Chunks
  - 刪除 Azure Blob
```

#### 重新索引 API
```yaml
POST /api/v1/knowledge-base/files/{id}/reindex:
  - 重新處理文件
  - 重新生成 Chunks
  - 場景:
      - Chunking 策略更新
      - 文件內容修復
      - Embedding 模型升級 (Sprint 6)

POST /api/v1/knowledge-base/agents/{agentId}/reindex-all:
  - 重新索引 Agent 所有文件
  - 批量處理
  - 異步執行
```

#### Frontend 任務
```yaml
文件上傳界面:
  - Drag & Drop 上傳組件
  - 文件類型驗證
  - 上傳進度條
  - 多文件批量上傳

文件列表界面:
  - 文件列表展示 (表格或卡片)
  - 篩選和排序
  - 狀態指示 (processing/completed/failed)
  - 操作: 查看詳情、重新索引、刪除

文件詳情界面:
  - 文件元數據展示
  - Chunks 統計
  - Chunks 預覽 (可選)
  - 重新索引功能
```

---

## 🧪 測試策略

### 單元測試 (目標 20 tests)
```yaml
FileParserTests:
  - PDF 解析測試 (各種格式 PDF)
  - DOCX 解析測試
  - TXT/MD 解析測試
  - 錯誤處理測試 (損壞文件)

ChunkingTests:
  - Sentence Chunking 測試
  - Paragraph Chunking 測試
  - Chunk Size 限制測試
  - Chunk Overlap 測試

KnowledgeBaseServiceTests:
  - 文件上傳邏輯測試
  - Metadata 提取測試
  - CRUD 操作測試
```

### 集成測試 (目標 12 tests)
```yaml
BlobStorageIntegrationTests:
  - Azure Blob 上傳測試
  - Blob 下載測試
  - Blob 刪除測試

KnowledgeBaseApiTests:
  - POST /files - 上傳測試
  - GET /files - 列表測試
  - GET /files/{id} - 詳情測試
  - DELETE /files/{id} - 刪除測試
  - POST /files/{id}/reindex - 重新索引測試

文件處理 Pipeline 測試:
  - 完整流程測試 (上傳 → 解析 → Chunking → 存儲)
```

### E2E 測試 (目標 5 tests)
```yaml
Test 1 - 完整上傳流程:
  - 上傳 PDF 文件
  - 驗證解析完成
  - 查看 Chunks

Test 2 - 批量上傳:
  - 上傳 3 個不同格式文件
  - 驗證全部處理完成

Test 3 - 重新索引:
  - 上傳文件
  - 觸發重新索引
  - 驗證 Chunks 更新

Test 4 - 文件刪除:
  - 刪除文件
  - 驗證 Blob 和 Chunks 被刪除

Test 5 - 大文件處理:
  - 上傳 20MB PDF
  - 驗證處理成功
  - 性能測試
```

---

## 🎯 驗收標準 (DoD)

### 功能驗收
- ✅ 支持 PDF, DOCX, TXT, MD 文件上傳
- ✅ 文件解析準確率 ≥ 95%
- ✅ Chunking 邏輯正確
- ✅ CRUD 功能完整
- ✅ 重新索引功能正常

### 性能驗收
- ✅ 文件上傳速度 ≥ 1MB/s
- ✅ PDF 解析速度: 10 頁/秒
- ✅ Chunking 速度: 1000 tokens/秒
- ✅ API 響應時間 < 500ms (除上傳)

### 質量驗收
- ✅ 單元測試通過率 100%
- ✅ 集成測試通過率 100%
- ✅ E2E 測試通過率 100%
- ✅ 測試覆蓋率 ≥ 85%
- ✅ Code Review 通過
- ✅ PO 驗收通過

---

## 🎓 關鍵風險

### 風險 1: 文件解析準確率不足
**影響**: 高 🔴
**緩解**:
- 選用成熟的解析庫 (PdfPig, OpenXml)
- 測試多種格式 PDF (掃描版/文字版/混合)
- 提供手動校正機制

### 風險 2: 大文件處理超時
**影響**: 中 🟡
**緩解**:
- 異步處理機制
- 文件大小限制 (50MB)
- 進度追蹤和通知

### 風險 3: Azure Blob 成本超預算
**影響**: 低 🟢
**緩解**:
- 設置存儲配額 (10GB/Agent)
- 冷存儲策略 (90 天未用)
- 監控存儲使用量

---

## 📦 技術依賴

### 新增 NuGet 套件
```yaml
Azure.Storage.Blobs: v12.x
Azure.Identity: v1.x
PdfPig: v0.1.8
DocumentFormat.OpenXml: v3.x
```

### Azure 資源
```yaml
Azure Storage Account:
  - SKU: Standard_LRS
  - Container: knowledge-base
  - 訪問級別: Private
```

---

## 🔗 相關文檔

- **User Story 詳情**: [docs/user-stories/sprints/sprint-allocation.md](../../../docs/user-stories/sprints/sprint-allocation.md#sprint-5)
- **後續 Sprint**: Sprint 6 (US 5.2 - 精確檢索, 核心差異化)

---

**維護說明**: 規劃文檔,Sprint 5 開始後更新實際進度。

**狀態**: ⏳ 規劃中
