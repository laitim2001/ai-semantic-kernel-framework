-- IPA Platform Database Initialization Script
-- This script is automatically executed when PostgreSQL container starts

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";  -- For full-text search
CREATE EXTENSION IF NOT EXISTS "pgcrypto";  -- For encryption functions

-- Create schemas
CREATE SCHEMA IF NOT EXISTS workflow;
CREATE SCHEMA IF NOT EXISTS execution;
CREATE SCHEMA IF NOT EXISTS agent;
CREATE SCHEMA IF NOT EXISTS audit;

-- Set default search path
SET search_path TO public, workflow, execution, agent, audit;

-- Create initial tables (basic structure)
-- 詳細的 schema 將在 backend 代碼中通過 SQLAlchemy migrations 管理

-- Users table (basic structure)
CREATE TABLE IF NOT EXISTS public.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    azure_ad_object_id VARCHAR(255) UNIQUE,
    display_name VARCHAR(255),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_users_email ON public.users(email);
CREATE INDEX IF NOT EXISTS idx_users_azure_ad_object_id ON public.users(azure_ad_object_id);

-- Create updated_at trigger function
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply trigger to users table
DROP TRIGGER IF EXISTS update_users_updated_at ON public.users;
CREATE TRIGGER update_users_updated_at
    BEFORE UPDATE ON public.users
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Grant permissions
GRANT USAGE ON SCHEMA public, workflow, execution, agent, audit TO ipa_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public, workflow, execution, agent, audit TO ipa_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public, workflow, execution, agent, audit TO ipa_user;

-- Log initialization
DO $$
BEGIN
    RAISE NOTICE 'IPA Platform database initialized successfully';
END $$;
