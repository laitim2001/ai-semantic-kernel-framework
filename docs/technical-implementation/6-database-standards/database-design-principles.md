# 數據庫設計原則 (Database Design Principles)

## 概述

本文檔定義 AI Workflow Platform 的數據庫設計核心原則、最佳實踐和設計模式,確保數據模型的可擴展性、性能和維護性。

## 目標

- ✅ 建立統一的數據庫設計標準
- ✅ 定義正規化與反正規化策略
- ✅ 確保數據完整性和一致性
- ✅ 優化查詢性能和索引設計
- ✅ 支持未來擴展和演進

## 數據庫架構設計

### Schema 組織策略

**多 Schema 架構設計**:

```sql
-- 創建數據庫
CREATE DATABASE aiworkflow
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Schema 劃分
CREATE SCHEMA workflow;     -- 工作流程核心數據
CREATE SCHEMA identity;     -- 身份認證與授權
CREATE SCHEMA execution;    -- 執行記錄與日誌
CREATE SCHEMA audit;        -- 審計追蹤
CREATE SCHEMA system;       -- 系統配置
```

**Schema 設計原則**:

1. **按業務領域劃分**: 每個 Schema 代表一個獨立的業務領域
2. **邏輯隔離**: Schema 提供邏輯隔離,便於權限管理
3. **命名清晰**: Schema 名稱直接反映業務功能
4. **避免過度劃分**: 不要為每個表創建獨立 Schema

### 表設計原則

#### 1. 主鍵設計

**整數主鍵 (推薦)**:

```sql
-- ✅ 推薦: 自增整數主鍵
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    -- 或使用 BIGSERIAL 處理大數據量
    -- id BIGSERIAL PRIMARY KEY,

    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

**UUID 主鍵 (特定場景)**:

```sql
-- ✅ 適用場景: 分佈式系統、需要全局唯一性
CREATE TABLE workflow.workflow_executions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    workflow_id INT NOT NULL REFERENCES workflow.workflows(id),
    status VARCHAR(50) NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

**主鍵選擇指南**:

| 類型 | 優點 | 缺點 | 適用場景 |
|------|------|------|----------|
| `SERIAL` | 性能高、索引小、排序快 | 可預測性、單數據庫限制 | 大部分場景 |
| `BIGSERIAL` | 超大容量 | 佔用空間更大 | 大數據量表 |
| `UUID` | 全局唯一、分佈式友好 | 索引大、性能略低 | 分佈式系統 |

#### 2. 外鍵設計

**外鍵命名規範**:

```sql
-- 格式: {referenced_table}_{id}
CREATE TABLE workflow.workflow_nodes (
    id SERIAL PRIMARY KEY,
    workflow_id INT NOT NULL,  -- 外鍵命名

    CONSTRAINT fk_workflow_nodes_workflows_workflow_id
        FOREIGN KEY (workflow_id)
        REFERENCES workflow.workflows(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
```

**外鍵刪除策略**:

| 策略 | 說明 | 使用場景 |
|------|------|----------|
| `CASCADE` | 自動刪除子記錄 | 強依賴關係 (工作流程→節點) |
| `RESTRICT` | 禁止刪除有引用的父記錄 | 保護性刪除 (用戶→工作流程) |
| `SET NULL` | 設置外鍵為 NULL | 可選關聯 (工作流程→模板) |
| `NO ACTION` | 默認行為,檢查延遲 | 一般場景 |

**示例 - 不同刪除策略**:

```sql
-- 1. CASCADE: 刪除工作流程時自動刪除所有節點
CREATE TABLE workflow.workflow_nodes (
    id SERIAL PRIMARY KEY,
    workflow_id INT NOT NULL,

    CONSTRAINT fk_workflow_nodes_workflows_workflow_id
        FOREIGN KEY (workflow_id)
        REFERENCES workflow.workflows(id)
        ON DELETE CASCADE  -- 級聯刪除
);

-- 2. RESTRICT: 禁止刪除有工作流程的用戶
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    owner_id INT NOT NULL,

    CONSTRAINT fk_workflows_users_owner_id
        FOREIGN KEY (owner_id)
        REFERENCES identity.users(id)
        ON DELETE RESTRICT  -- 保護性刪除
);

-- 3. SET NULL: 刪除模板時保留工作流程,設置為 NULL
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    template_id INT,

    CONSTRAINT fk_workflows_templates_template_id
        FOREIGN KEY (template_id)
        REFERENCES workflow.workflow_templates(id)
        ON DELETE SET NULL  -- 可選關聯
);
```

#### 3. 數據類型選擇

**字符串類型**:

```sql
CREATE TABLE examples (
    -- 固定長度字符串 (很少使用)
    code CHAR(10),                    -- ❌ 浪費空間

    -- 可變長度字符串 (推薦)
    name VARCHAR(200),                -- ✅ 有長度限制的字符串
    email VARCHAR(255),               -- ✅ Email 標準長度

    -- 無限長度文本
    description TEXT,                 -- ✅ 長文本內容
    content TEXT                      -- ✅ JSON、Markdown 等
);
```

**數值類型**:

```sql
CREATE TABLE examples (
    -- 整數類型
    id SERIAL PRIMARY KEY,            -- ✅ 自增主鍵 (INT)
    big_id BIGSERIAL,                 -- ✅ 大數據量 (BIGINT)
    small_count SMALLINT,             -- ✅ 小範圍計數 (-32768 ~ 32767)

    -- 小數類型
    price NUMERIC(10, 2),             -- ✅ 精確小數 (金額)
    percentage DECIMAL(5, 2),         -- ✅ 百分比 (0.00 ~ 999.99)

    -- 浮點數 (避免用於金額)
    latitude DOUBLE PRECISION,        -- ✅ 地理坐標
    longitude DOUBLE PRECISION        -- ✅ 地理坐標
);
```

**日期時間類型**:

```sql
CREATE TABLE examples (
    -- 帶時區的時間戳 (推薦)
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,

    -- 不帶時區的時間戳 (避免使用)
    legacy_date TIMESTAMP,            -- ❌ 時區問題

    -- 日期
    birth_date DATE,                  -- ✅ 只需要日期

    -- 時間
    daily_backup_time TIME            -- ✅ 每日時間
);
```

**布爾類型**:

```sql
CREATE TABLE examples (
    -- 布爾值
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_published BOOLEAN NOT NULL DEFAULT FALSE
);
```

**JSON 類型**:

```sql
CREATE TABLE examples (
    -- JSON (推薦用於動態結構)
    metadata JSONB,                   -- ✅ 二進制 JSON (性能更好)
    settings JSON,                    -- 普通 JSON

    -- 示例用法
    CHECK (metadata IS NULL OR jsonb_typeof(metadata) = 'object')
);
```

**數組類型**:

```sql
CREATE TABLE examples (
    -- PostgreSQL 數組
    tags TEXT[],                      -- ✅ 標籤列表
    scores INT[],                     -- ✅ 分數列表

    -- 示例用法
    CHECK (array_length(tags, 1) <= 20)  -- 限制數組大小
);
```

**數據類型選擇指南**:

| 數據類型 | PostgreSQL 類型 | 使用場景 |
|---------|----------------|----------|
| 短字符串 | `VARCHAR(n)` | 姓名、標題、Email |
| 長文本 | `TEXT` | 描述、內容、Markdown |
| 整數主鍵 | `SERIAL` / `BIGSERIAL` | 自增主鍵 |
| 外鍵 | `INT` / `BIGINT` | 引用其他表 |
| 金額 | `NUMERIC(10,2)` | 貨幣、精確計算 |
| 時間戳 | `TIMESTAMP WITH TIME ZONE` | 創建時間、更新時間 |
| 布爾值 | `BOOLEAN` | 是/否標記 |
| 動態數據 | `JSONB` | 配置、元數據 |

### 正規化與反正規化

#### 正規化策略

**第一正規化 (1NF)**: 消除重複組

```sql
-- ❌ 違反 1NF: 多值屬性
CREATE TABLE workflows_bad (
    id INT PRIMARY KEY,
    name VARCHAR(200),
    tags VARCHAR(500)  -- "tag1,tag2,tag3" 字符串拼接
);

-- ✅ 符合 1NF: 獨立表存儲標籤
CREATE TABLE workflows (
    id INT PRIMARY KEY,
    name VARCHAR(200)
);

CREATE TABLE workflow_tags (
    workflow_id INT REFERENCES workflows(id),
    tag VARCHAR(50),
    PRIMARY KEY (workflow_id, tag)
);
```

**第二正規化 (2NF)**: 消除部分依賴

```sql
-- ❌ 違反 2NF: order_date 只依賴 order_id
CREATE TABLE order_items_bad (
    order_id INT,
    product_id INT,
    order_date DATE,      -- 部分依賴於 order_id
    quantity INT,
    PRIMARY KEY (order_id, product_id)
);

-- ✅ 符合 2NF: 分離訂單信息
CREATE TABLE orders (
    id INT PRIMARY KEY,
    order_date DATE
);

CREATE TABLE order_items (
    order_id INT REFERENCES orders(id),
    product_id INT,
    quantity INT,
    PRIMARY KEY (order_id, product_id)
);
```

**第三正規化 (3NF)**: 消除傳遞依賴

```sql
-- ❌ 違反 3NF: city_name 依賴於 city_id
CREATE TABLE users_bad (
    id INT PRIMARY KEY,
    name VARCHAR(200),
    city_id INT,
    city_name VARCHAR(100)  -- 傳遞依賴
);

-- ✅ 符合 3NF: 分離城市信息
CREATE TABLE users (
    id INT PRIMARY KEY,
    name VARCHAR(200),
    city_id INT REFERENCES cities(id)
);

CREATE TABLE cities (
    id INT PRIMARY KEY,
    name VARCHAR(100)
);
```

#### 反正規化策略

**何時反正規化**:

1. **高頻讀取場景**: 查詢性能優先於存儲空間
2. **報表和統計**: 避免複雜 JOIN 操作
3. **歷史數據快照**: 保存時間點的完整狀態

**反正規化示例 1: 冗餘字段**

```sql
-- ✅ 反正規化: 冗餘 owner_email 避免 JOIN
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    owner_id INT NOT NULL REFERENCES identity.users(id),
    owner_email VARCHAR(255) NOT NULL,  -- 冗餘字段
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- 使用觸發器保持同步
CREATE OR REPLACE FUNCTION sync_owner_email()
RETURNS TRIGGER AS $$
BEGIN
    NEW.owner_email := (SELECT email FROM identity.users WHERE id = NEW.owner_id);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_owner_email
BEFORE INSERT OR UPDATE OF owner_id ON workflow.workflows
FOR EACH ROW
EXECUTE FUNCTION sync_owner_email();
```

**反正規化示例 2: 計算字段**

```sql
-- ✅ 反正規化: 存儲計算結果
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    node_count INT NOT NULL DEFAULT 0,      -- 冗餘計數
    edge_count INT NOT NULL DEFAULT 0,      -- 冗餘計數
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- 使用觸發器自動更新計數
CREATE OR REPLACE FUNCTION update_workflow_counts()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE workflow.workflows
        SET node_count = node_count + 1
        WHERE id = NEW.workflow_id;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE workflow.workflows
        SET node_count = node_count - 1
        WHERE id = OLD.workflow_id;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER workflow_node_count_trigger
AFTER INSERT OR DELETE ON workflow.workflow_nodes
FOR EACH ROW
EXECUTE FUNCTION update_workflow_counts();
```

**反正規化示例 3: 歷史快照表**

```sql
-- ✅ 反正規化: 保存完整歷史狀態
CREATE TABLE execution.workflow_execution_snapshots (
    id SERIAL PRIMARY KEY,
    execution_id UUID NOT NULL REFERENCES execution.workflow_executions(id),

    -- 完整快照 (反正規化)
    workflow_definition JSONB NOT NULL,  -- 整個工作流程定義
    input_data JSONB,
    output_data JSONB,

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

**正規化 vs 反正規化決策矩陣**:

| 場景 | 正規化 | 反正規化 | 理由 |
|------|--------|----------|------|
| 頻繁更新的數據 | ✅ | ❌ | 避免多處更新 |
| 頻繁讀取的數據 | ❌ | ✅ | 減少 JOIN 提升性能 |
| 報表和統計 | ❌ | ✅ | 預計算結果 |
| 事務性數據 | ✅ | ❌ | 確保數據一致性 |
| 歷史快照 | ❌ | ✅ | 保存時間點狀態 |

### 索引設計原則

#### 1. 索引類型選擇

**B-Tree 索引 (默認)**:

```sql
-- ✅ B-Tree: 大部分場景的默認選擇
CREATE INDEX ix_workflows_owner_id ON workflow.workflows (owner_id);

-- 適用場景:
-- - 等值查詢: WHERE owner_id = 42
-- - 範圍查詢: WHERE created_at > '2025-01-01'
-- - 排序: ORDER BY created_at DESC
-- - LIKE 前綴: WHERE name LIKE 'test%'
```

**HASH 索引**:

```sql
-- ✅ HASH: 僅等值查詢
CREATE INDEX ix_users_email_hash ON identity.users USING HASH (email);

-- 適用場景:
-- - 等值查詢: WHERE email = 'user@example.com'
-- - 不支持範圍查詢
-- - 不支持排序
```

**GIN 索引 (通用倒排索引)**:

```sql
-- ✅ GIN: 全文檢索、JSONB、數組
CREATE INDEX ix_workflows_tags_gin ON workflow.workflows USING GIN (tags);
CREATE INDEX ix_workflows_metadata_gin ON workflow.workflows USING GIN (metadata);

-- 適用場景:
-- - 數組包含: WHERE tags @> ARRAY['important']
-- - JSONB 查詢: WHERE metadata @> '{"status": "active"}'
-- - 全文檢索: WHERE to_tsvector('english', description) @@ to_tsquery('workflow')
```

**GiST 索引 (通用搜索樹)**:

```sql
-- ✅ GiST: 地理空間、範圍類型
CREATE INDEX ix_locations_point_gist ON locations USING GIST (location);

-- 適用場景:
-- - 地理查詢: WHERE ST_DWithin(location, point, 1000)
-- - 範圍重疊: WHERE period && daterange('2025-01-01', '2025-12-31')
```

#### 2. 單列索引 vs 複合索引

**單列索引**:

```sql
-- ✅ 單列索引: 單一條件查詢
CREATE INDEX ix_workflows_status ON workflow.workflows (status);
CREATE INDEX ix_workflows_created_at ON workflow.workflows (created_at);

-- 適用查詢:
-- SELECT * FROM workflows WHERE status = 'Published';
-- SELECT * FROM workflows WHERE created_at > '2025-01-01';
```

**複合索引**:

```sql
-- ✅ 複合索引: 多條件查詢
CREATE INDEX ix_workflows_owner_id_status ON workflow.workflows (owner_id, status);
CREATE INDEX ix_workflows_owner_id_created_at ON workflow.workflows (owner_id, created_at DESC);

-- 適用查詢:
-- SELECT * FROM workflows WHERE owner_id = 42 AND status = 'Published';
-- SELECT * FROM workflows WHERE owner_id = 42 ORDER BY created_at DESC;

-- 複合索引可以覆蓋前綴查詢:
-- SELECT * FROM workflows WHERE owner_id = 42;  -- 使用 ix_workflows_owner_id_status
```

**複合索引順序規則**:

1. **等值查詢在前**: `(owner_id, status)` 而非 `(status, owner_id)`
2. **過濾性強的列在前**: 選擇性高的列放前面
3. **排序列在最後**: `(owner_id, created_at DESC)`

**示例 - 複合索引順序**:

```sql
-- ✅ 正確順序: 等值 → 範圍 → 排序
CREATE INDEX ix_workflows_complex
ON workflow.workflows (owner_id, status, created_at DESC);

-- 高效查詢:
SELECT * FROM workflows
WHERE owner_id = 42           -- 等值 (索引第一列)
  AND status = 'Published'    -- 等值 (索引第二列)
ORDER BY created_at DESC;     -- 排序 (索引第三列)

-- ❌ 錯誤順序: 排序列在前
CREATE INDEX ix_workflows_bad
ON workflow.workflows (created_at DESC, owner_id, status);
-- 效率低,無法有效利用 owner_id 過濾
```

#### 3. 部分索引 (Partial Index)

```sql
-- ✅ 部分索引: 只索引特定條件的行
CREATE INDEX ix_workflows_active_owner
ON workflow.workflows (owner_id)
WHERE status = 'Published' AND is_deleted = FALSE;

-- 適用查詢:
SELECT * FROM workflows
WHERE owner_id = 42 AND status = 'Published' AND is_deleted = FALSE;

-- 優勢:
-- - 索引更小,效率更高
-- - 只索引需要的數據
-- - 特別適合查詢特定狀態的場景
```

**部分索引使用場景**:

```sql
-- 1. 只索引活躍用戶
CREATE INDEX ix_users_active_email
ON identity.users (email)
WHERE is_active = TRUE AND is_deleted = FALSE;

-- 2. 只索引未完成的執行
CREATE INDEX ix_executions_pending
ON execution.workflow_executions (workflow_id, started_at)
WHERE status IN ('Pending', 'Running');

-- 3. 只索引最近的記錄
CREATE INDEX ix_logs_recent
ON audit.audit_logs (entity_id, created_at)
WHERE created_at > NOW() - INTERVAL '30 days';
```

#### 4. 覆蓋索引 (Covering Index)

```sql
-- ✅ 覆蓋索引: 包含所有查詢列
CREATE INDEX ix_workflows_covering
ON workflow.workflows (owner_id, status)
INCLUDE (name, created_at);

-- Index-Only Scan (無需回表):
SELECT name, status, created_at
FROM workflows
WHERE owner_id = 42 AND status = 'Published';

-- 優勢:
-- - 查詢直接從索引獲取所有數據
-- - 避免回表 (Heap Fetch)
-- - 顯著提升性能
```

#### 5. 索引維護策略

**重建索引**:

```sql
-- 在線重建索引 (不鎖表)
REINDEX INDEX CONCURRENTLY ix_workflows_owner_id;

-- 重建表的所有索引
REINDEX TABLE CONCURRENTLY workflow.workflows;

-- 重建整個 Schema
REINDEX SCHEMA CONCURRENTLY workflow;
```

**分析索引使用情況**:

```sql
-- 查看未使用的索引
SELECT
    schemaname,
    tablename,
    indexname,
    idx_scan,
    idx_tup_read,
    idx_tup_fetch,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
WHERE idx_scan = 0
  AND schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY pg_relation_size(indexrelid) DESC;

-- 查看索引膨脹
SELECT
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size,
    round(100 * pg_relation_size(indexrelid) / pg_relation_size(indrelid), 2) AS ratio
FROM pg_stat_user_indexes
ORDER BY pg_relation_size(indexrelid) DESC;
```

### 約束設計

#### 1. 主鍵約束

```sql
-- ✅ 主鍵約束
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    -- 等價於:
    -- id SERIAL,
    -- CONSTRAINT pk_workflows PRIMARY KEY (id)
);

-- 複合主鍵
CREATE TABLE workflow.workflow_node_connections (
    source_node_id INT NOT NULL,
    target_node_id INT NOT NULL,
    PRIMARY KEY (source_node_id, target_node_id)
);
```

#### 2. 唯一約束

```sql
-- ✅ 單列唯一約束
CREATE TABLE identity.users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    CONSTRAINT uq_users_email UNIQUE (email)
);

-- ✅ 複合唯一約束
CREATE TABLE workflow.workflow_versions (
    workflow_id INT NOT NULL,
    version INT NOT NULL,
    CONSTRAINT uq_workflow_versions UNIQUE (workflow_id, version)
);

-- ✅ 部分唯一約束 (PostgreSQL 特性)
CREATE UNIQUE INDEX uq_users_email_active
ON identity.users (email)
WHERE is_deleted = FALSE;
```

#### 3. 檢查約束

```sql
-- ✅ 檢查約束
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    status VARCHAR(50) NOT NULL,
    priority INT NOT NULL DEFAULT 0,

    -- 狀態值約束
    CONSTRAINT ck_workflows_status_valid
        CHECK (status IN ('Draft', 'Published', 'Archived', 'Executing', 'Completed', 'Failed')),

    -- 優先級範圍約束
    CONSTRAINT ck_workflows_priority_range
        CHECK (priority BETWEEN 0 AND 10),

    -- 名稱長度約束
    CONSTRAINT ck_workflows_name_length
        CHECK (LENGTH(name) >= 3)
);

-- JSONB 結構約束
CREATE TABLE workflow.workflow_nodes (
    id SERIAL PRIMARY KEY,
    configuration JSONB NOT NULL,

    CONSTRAINT ck_workflow_nodes_configuration_valid
        CHECK (jsonb_typeof(configuration) = 'object'),

    CONSTRAINT ck_workflow_nodes_configuration_required
        CHECK (configuration ? 'type' AND configuration ? 'name')
);
```

#### 4. 默認值

```sql
-- ✅ 默認值設計
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Draft',
    priority INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

    -- 時間戳默認值
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,

    -- UUID 默認值
    execution_id UUID DEFAULT gen_random_uuid()
);
```

### 審計字段標準

#### 標準審計字段

```sql
-- ✅ 完整審計字段集
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,

    -- 審計字段
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by INT NOT NULL REFERENCES identity.users(id),

    updated_at TIMESTAMP WITH TIME ZONE,
    updated_by INT REFERENCES identity.users(id),

    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by INT REFERENCES identity.users(id),

    -- 並發控制
    version INT NOT NULL DEFAULT 1
);
```

#### 審計字段觸發器

```sql
-- 自動更新 updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    NEW.version = OLD.version + 1;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_workflows_updated_at
BEFORE UPDATE ON workflow.workflows
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
```

### 軟刪除設計

#### 軟刪除實現

```sql
-- ✅ 軟刪除表設計
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,

    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by INT REFERENCES identity.users(id),

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    -- 部分唯一索引 (排除已刪除記錄)
    CONSTRAINT uq_workflows_name_active UNIQUE (name)
        WHERE is_deleted = FALSE
);

-- 軟刪除索引
CREATE INDEX ix_workflows_is_deleted ON workflow.workflows (is_deleted)
WHERE is_deleted = FALSE;
```

#### 軟刪除函數

```sql
-- 軟刪除函數
CREATE OR REPLACE FUNCTION soft_delete_workflow(
    p_workflow_id INT,
    p_user_id INT
)
RETURNS BOOLEAN AS $$
BEGIN
    UPDATE workflow.workflows
    SET is_deleted = TRUE,
        deleted_at = NOW(),
        deleted_by = p_user_id
    WHERE id = p_workflow_id
      AND is_deleted = FALSE;

    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;

-- 使用示例
SELECT soft_delete_workflow(42, 1);
```

#### 永久刪除策略

```sql
-- 定期清理已刪除記錄 (超過 90 天)
CREATE OR REPLACE FUNCTION cleanup_deleted_workflows()
RETURNS INT AS $$
DECLARE
    deleted_count INT;
BEGIN
    DELETE FROM workflow.workflows
    WHERE is_deleted = TRUE
      AND deleted_at < NOW() - INTERVAL '90 days';

    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- 定時任務 (使用 pg_cron 擴展)
-- SELECT cron.schedule('cleanup-deleted-workflows', '0 2 * * *', 'SELECT cleanup_deleted_workflows()');
```

## 數據完整性保證

### 1. 參照完整性

```sql
-- ✅ 外鍵級聯規則
CREATE TABLE workflow.workflow_nodes (
    id SERIAL PRIMARY KEY,
    workflow_id INT NOT NULL,

    -- 刪除工作流程時級聯刪除節點
    CONSTRAINT fk_workflow_nodes_workflows_workflow_id
        FOREIGN KEY (workflow_id)
        REFERENCES workflow.workflows(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

-- ✅ 防止孤兒記錄
CREATE TABLE execution.workflow_execution_logs (
    id BIGSERIAL PRIMARY KEY,
    execution_id UUID NOT NULL,

    -- 刪除執行記錄時禁止有未處理日誌
    CONSTRAINT fk_execution_logs_executions_execution_id
        FOREIGN KEY (execution_id)
        REFERENCES execution.workflow_executions(id)
        ON DELETE RESTRICT
);
```

### 2. 域完整性

```sql
-- ✅ 檢查約束確保域完整性
CREATE TABLE workflow.workflow_nodes (
    id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    timeout_seconds INT,
    retry_count INT,

    -- 類型枚舉
    CONSTRAINT ck_workflow_nodes_type_valid
        CHECK (type IN ('Trigger', 'Action', 'Condition', 'Loop', 'SubWorkflow')),

    -- 超時範圍
    CONSTRAINT ck_workflow_nodes_timeout_range
        CHECK (timeout_seconds IS NULL OR timeout_seconds BETWEEN 1 AND 3600),

    -- 重試次數
    CONSTRAINT ck_workflow_nodes_retry_count_range
        CHECK (retry_count IS NULL OR retry_count BETWEEN 0 AND 5)
);
```

### 3. 實體完整性

```sql
-- ✅ NOT NULL 約束確保必填字段
CREATE TABLE workflow.workflows (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,              -- 必填
    description TEXT,                        -- 可選
    owner_id INT NOT NULL,                   -- 必填
    status VARCHAR(50) NOT NULL DEFAULT 'Draft',  -- 必填有默認值
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
```

## 性能優化設計

### 1. 分區表設計

**按時間分區 (Range Partitioning)**:

```sql
-- 創建分區主表
CREATE TABLE audit.audit_logs (
    id BIGSERIAL,
    entity_type VARCHAR(100) NOT NULL,
    entity_id INT NOT NULL,
    action VARCHAR(50) NOT NULL,
    user_id INT NOT NULL,
    changes JSONB,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
) PARTITION BY RANGE (created_at);

-- 創建分區
CREATE TABLE audit.audit_logs_2025_01 PARTITION OF audit.audit_logs
    FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');

CREATE TABLE audit.audit_logs_2025_02 PARTITION OF audit.audit_logs
    FOR VALUES FROM ('2025-02-01') TO ('2025-03-01');

-- 自動創建未來分區的函數
CREATE OR REPLACE FUNCTION create_audit_log_partition()
RETURNS void AS $$
DECLARE
    partition_date DATE;
    partition_name TEXT;
    start_date DATE;
    end_date DATE;
BEGIN
    partition_date := DATE_TRUNC('month', NOW() + INTERVAL '1 month');
    partition_name := 'audit_logs_' || TO_CHAR(partition_date, 'YYYY_MM');
    start_date := partition_date;
    end_date := partition_date + INTERVAL '1 month';

    EXECUTE FORMAT(
        'CREATE TABLE IF NOT EXISTS audit.%I PARTITION OF audit.audit_logs
         FOR VALUES FROM (%L) TO (%L)',
        partition_name, start_date, end_date
    );
END;
$$ LANGUAGE plpgsql;
```

**按列表分區 (List Partitioning)**:

```sql
-- 按狀態分區
CREATE TABLE execution.workflow_executions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    workflow_id INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
) PARTITION BY LIST (status);

CREATE TABLE execution.workflow_executions_active PARTITION OF execution.workflow_executions
    FOR VALUES IN ('Pending', 'Running');

CREATE TABLE execution.workflow_executions_completed PARTITION OF execution.workflow_executions
    FOR VALUES IN ('Completed', 'Failed', 'Cancelled');
```

### 2. 物化視圖

```sql
-- 創建物化視圖
CREATE MATERIALIZED VIEW workflow.workflow_statistics AS
SELECT
    w.owner_id,
    COUNT(*) AS total_workflows,
    COUNT(*) FILTER (WHERE w.status = 'Published') AS published_count,
    COUNT(*) FILTER (WHERE w.status = 'Draft') AS draft_count,
    AVG(w.node_count) AS avg_node_count,
    MAX(w.created_at) AS last_created_at
FROM workflow.workflows w
WHERE w.is_deleted = FALSE
GROUP BY w.owner_id;

-- 創建索引
CREATE UNIQUE INDEX ix_workflow_statistics_owner_id
ON workflow.workflow_statistics (owner_id);

-- 刷新物化視圖
REFRESH MATERIALIZED VIEW CONCURRENTLY workflow.workflow_statistics;

-- 定時刷新 (使用 pg_cron)
-- SELECT cron.schedule('refresh-workflow-stats', '*/15 * * * *',
--   'REFRESH MATERIALIZED VIEW CONCURRENTLY workflow.workflow_statistics');
```

### 3. 表繼承 (謹慎使用)

```sql
-- 基表
CREATE TABLE workflow.base_workflow_node (
    id SERIAL PRIMARY KEY,
    workflow_id INT NOT NULL,
    name VARCHAR(200) NOT NULL,
    type VARCHAR(50) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- 繼承表
CREATE TABLE workflow.action_nodes (
    action_type VARCHAR(50) NOT NULL,
    endpoint_url VARCHAR(500),
    http_method VARCHAR(10)
) INHERITS (workflow.base_workflow_node);

CREATE TABLE workflow.condition_nodes (
    condition_expression TEXT NOT NULL,
    true_path INT,
    false_path INT
) INHERITS (workflow.base_workflow_node);
```

**注意**: PostgreSQL 表繼承有限制,推薦使用 JSONB 或獨立表代替。

## 命名規範總結

### 數據庫對象命名

| 對象類型 | 命名格式 | 示例 |
|---------|---------|------|
| Schema | `lowercase` | `workflow`, `identity` |
| Table | `lowercase_plural` | `workflows`, `workflow_nodes` |
| Column | `lowercase_underscore` | `owner_id`, `created_at` |
| Primary Key | `pk_{table}` | `pk_workflows` |
| Foreign Key | `fk_{table}_{ref_table}_{column}` | `fk_workflows_users_owner_id` |
| Unique Constraint | `uq_{table}_{column(s)}` | `uq_users_email` |
| Check Constraint | `ck_{table}_{column}_{condition}` | `ck_workflows_status_valid` |
| Index | `ix_{table}_{column(s)}` | `ix_workflows_owner_id_status` |
| Function | `lowercase_underscore` | `update_updated_at_column` |
| Trigger | `{action}_{table}_{event}` | `update_workflows_updated_at` |

## 檢查清單

### 設計階段
- [ ] 確定 Schema 劃分策略
- [ ] 設計表結構和關係
- [ ] 選擇主鍵類型 (SERIAL vs UUID)
- [ ] 定義外鍵關聯和刪除策略
- [ ] 評估正規化 vs 反正規化
- [ ] 規劃索引策略
- [ ] 設計約束和默認值
- [ ] 定義審計字段標準
- [ ] 實現軟刪除機制

### 實現階段
- [ ] 創建 Schema 和表
- [ ] 實現所有約束
- [ ] 創建必要索引
- [ ] 實現觸發器和函數
- [ ] 配置分區表 (如需要)
- [ ] 創建物化視圖 (如需要)

### 驗證階段
- [ ] 測試數據完整性約束
- [ ] 驗證外鍵級聯行為
- [ ] 分析查詢計劃
- [ ] 檢查索引使用情況
- [ ] 測試並發場景
- [ ] 驗證軟刪除邏輯

### 優化階段
- [ ] 識別慢查詢
- [ ] 優化索引策略
- [ ] 考慮分區表
- [ ] 創建必要的物化視圖
- [ ] 配置定期維護任務

## 相關文檔

- [Entity Framework Core 配置](./entity-framework-core-configuration.md)
- [數據庫遷移策略](./database-migration-strategy.md)
- [查詢性能優化](./query-performance-optimization.md)
- [Phase 3: 項目結構 - 後端項目結構](../3-project-structure/backend-project-structure.md)

## 參考資源

### 官方文檔
- [PostgreSQL Documentation](https://www.postgresql.org/docs/16/index.html)
- [PostgreSQL Data Types](https://www.postgresql.org/docs/16/datatype.html)
- [PostgreSQL Indexes](https://www.postgresql.org/docs/16/indexes.html)

### 最佳實踐
- [Database Design Best Practices](https://www.postgresql.org/docs/current/ddl.html)
- [PostgreSQL Performance Tips](https://wiki.postgresql.org/wiki/Performance_Optimization)
- [Normalization in Database Design](https://en.wikipedia.org/wiki/Database_normalization)

### 書籍
- *PostgreSQL: Up and Running* by Regina Obe, Leo Hsu
- *Database Design for Mere Mortals* by Michael J. Hernandez
- *SQL Antipatterns* by Bill Karwin

---

**文檔維護**: Database Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
