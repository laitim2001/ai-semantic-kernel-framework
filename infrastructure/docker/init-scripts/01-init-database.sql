-- =============================================================================
-- Semantic Kernel Agentic Framework - Database Initialization
-- =============================================================================
-- 此腳本在 PostgreSQL 容器首次啟動時自動執行

-- 創建數據庫 (如果不存在)
-- 注意: POSTGRES_DB 環境變數已創建主數據庫,此處為備用

-- 創建擴展
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";      -- UUID 生成
CREATE EXTENSION IF NOT EXISTS "pg_trgm";        -- 文本搜索
CREATE EXTENSION IF NOT EXISTS "btree_gin";      -- GIN 索引優化
CREATE EXTENSION IF NOT EXISTS "btree_gist";     -- GIST 索引優化

-- 創建 Schema
CREATE SCHEMA IF NOT EXISTS agent;
CREATE SCHEMA IF NOT EXISTS knowledge;
CREATE SCHEMA IF NOT EXISTS workflow;
CREATE SCHEMA IF NOT EXISTS audit;

-- 設置搜索路徑
ALTER DATABASE semantic_kernel SET search_path TO public, agent, knowledge, workflow, audit;

-- 創建基本表結構註釋
COMMENT ON SCHEMA agent IS 'Agent 管理相關表';
COMMENT ON SCHEMA knowledge IS '知識庫管理相關表';
COMMENT ON SCHEMA workflow IS '工作流編排相關表';
COMMENT ON SCHEMA audit IS '審計日誌相關表';

-- 創建審計觸發器函數
CREATE OR REPLACE FUNCTION audit.audit_trigger()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        NEW.created_at = CURRENT_TIMESTAMP;
        NEW.updated_at = CURRENT_TIMESTAMP;
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        NEW.updated_at = CURRENT_TIMESTAMP;
        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- 輸出初始化完成信息
DO $$
BEGIN
    RAISE NOTICE '✅ Database initialization completed';
    RAISE NOTICE '   - Extensions: uuid-ossp, pg_trgm, btree_gin, btree_gist';
    RAISE NOTICE '   - Schemas: agent, knowledge, workflow, audit';
    RAISE NOTICE '   - Audit trigger function created';
END $$;
