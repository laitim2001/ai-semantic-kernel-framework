# 數據庫遷移策略 (Database Migration Strategy)

## 概述

本文檔定義 AI Workflow Platform 的數據庫遷移完整策略,包括 EF Core Migrations 工作流程、版本控制、環境管理、回滾策略和生產環境最佳實踐。

## 目標

- ✅ 建立標準化的數據庫遷移流程
- ✅ 確保開發、測試、生產環境的一致性
- ✅ 實現安全的數據庫版本演進
- ✅ 支持無停機遷移和回滾
- ✅ 集成 CI/CD 自動化部署

## EF Core Migrations 基礎

### 遷移命令概覽

**基礎命令**:

```bash
# 添加新遷移
dotnet ef migrations add <MigrationName> \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --context ApplicationDbContext

# 列出所有遷移
dotnet ef migrations list \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 移除最後一次遷移 (未應用到數據庫)
dotnet ef migrations remove \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 生成 SQL 腳本
dotnet ef migrations script \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --output migration.sql

# 應用遷移到數據庫
dotnet ef database update \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 回滾到特定遷移
dotnet ef database update <TargetMigration> \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 刪除數據庫
dotnet ef database drop \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --force
```

### 遷移文件結構

```
AIWorkflow.Infrastructure/
└── Persistence/
    └── Migrations/
        ├── 20250101000001_InitialCreate.cs
        ├── 20250101000001_InitialCreate.Designer.cs
        ├── 20250115120000_AddWorkflowTemplates.cs
        ├── 20250115120000_AddWorkflowTemplates.Designer.cs
        ├── 20250201093000_AddUserProfiles.cs
        ├── 20250201093000_AddUserProfiles.Designer.cs
        └── ApplicationDbContextModelSnapshot.cs
```

## 遷移命名規範

### 命名最佳實踐

**格式**: `Add{Feature}`, `Update{Feature}`, `Remove{Feature}`, `Fix{Issue}`

```bash
# ✅ 好的命名
dotnet ef migrations add InitialCreate
dotnet ef migrations add AddWorkflowTemplates
dotnet ef migrations add AddUserProfileTable
dotnet ef migrations add UpdateWorkflowStatusEnum
dotnet ef migrations add AddWorkflowNodeIndexes
dotnet ef migrations add RemoveObsoleteColumns
dotnet ef migrations add FixWorkflowForeignKeys

# ❌ 不好的命名
dotnet ef migrations add Migration1
dotnet ef migrations add Changes
dotnet ef migrations add Update
dotnet ef migrations add Fix
```

### 遷移描述文檔

在每個遷移文件中添加註釋:

```csharp
/// <summary>
/// 遷移: 添加工作流程模板功能
/// 日期: 2025-01-15
/// 作者: Database Team
///
/// 變更內容:
/// - 創建 workflow_templates 表
/// - 添加 workflows.template_id 外鍵
/// - 創建相關索引
///
/// 依賴: 需要先應用 InitialCreate 遷移
/// </summary>
public partial class AddWorkflowTemplates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ...
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // ...
    }
}
```

## 遷移開發流程

### 本地開發流程

**步驟 1: 修改實體模型**

```csharp
// 添加新屬性
public sealed class Workflow
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // 新增字段
    public string? Description { get; set; }
    public int Priority { get; set; } = 0;
}
```

**步驟 2: 創建遷移**

```bash
dotnet ef migrations add AddWorkflowDescriptionAndPriority \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API
```

**步驟 3: 檢查生成的遷移代碼**

```csharp
public partial class AddWorkflowDescriptionAndPriority : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "description",
            schema: "workflow",
            table: "workflows",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "priority",
            schema: "workflow",
            table: "workflows",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        // 為現有記錄設置默認值
        migrationBuilder.Sql(
            "UPDATE workflow.workflows SET priority = 0 WHERE priority IS NULL;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "description",
            schema: "workflow",
            table: "workflows");

        migrationBuilder.DropColumn(
            name: "priority",
            schema: "workflow",
            table: "workflows");
    }
}
```

**步驟 4: 測試遷移**

```bash
# 應用遷移到本地數據庫
dotnet ef database update --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 驗證數據庫結構
psql -U postgres -d aiworkflow -c "\d workflow.workflows"

# 測試回滾
dotnet ef database update PreviousMigration --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 重新應用
dotnet ef database update --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API
```

**步驟 5: 提交到版本控制**

```bash
git add AIWorkflow.Infrastructure/Persistence/Migrations/
git commit -m "feat: add workflow description and priority fields"
git push
```

### 團隊協作流程

**場景 1: 合併衝突處理**

當多個開發者同時創建遷移時:

```bash
# 開發者 A 創建了 20250115120000_AddFeatureA
# 開發者 B 創建了 20250115120100_AddFeatureB

# 合併後,移除衝突的遷移
dotnet ef migrations remove --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 重新創建遷移
dotnet ef migrations add AddFeatureB --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 或者手動合併遷移代碼
```

**場景 2: 基於主分支同步**

```bash
# 切換到主分支
git checkout main
git pull origin main

# 更新數據庫到最新遷移
dotnet ef database update --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 切回功能分支
git checkout feature/my-feature
git rebase main

# 移除舊遷移並重新創建
dotnet ef migrations remove --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API
dotnet ef migrations add MyFeature --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API
```

## 複雜遷移場景

### 數據遷移

**場景: 重命名列並保留數據**

```csharp
public partial class RenameOwnerIdToUserId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 添加新列
        migrationBuilder.AddColumn<int>(
            name: "user_id",
            schema: "workflow",
            table: "workflows",
            type: "integer",
            nullable: true);

        // 2. 複製數據
        migrationBuilder.Sql(@"
            UPDATE workflow.workflows
            SET user_id = owner_id;
        ");

        // 3. 設置為非空
        migrationBuilder.AlterColumn<int>(
            name: "user_id",
            schema: "workflow",
            table: "workflows",
            type: "integer",
            nullable: false);

        // 4. 添加外鍵
        migrationBuilder.CreateIndex(
            name: "ix_workflows_user_id",
            schema: "workflow",
            table: "workflows",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_workflows_users_user_id",
            schema: "workflow",
            table: "workflows",
            column: "user_id",
            principalSchema: "identity",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        // 5. 刪除舊列
        migrationBuilder.DropForeignKey(
            name: "fk_workflows_users_owner_id",
            schema: "workflow",
            table: "workflows");

        migrationBuilder.DropIndex(
            name: "ix_workflows_owner_id",
            schema: "workflow",
            table: "workflows");

        migrationBuilder.DropColumn(
            name: "owner_id",
            schema: "workflow",
            table: "workflows");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // 反向操作
        migrationBuilder.AddColumn<int>(
            name: "owner_id",
            schema: "workflow",
            table: "workflows",
            type: "integer",
            nullable: true);

        migrationBuilder.Sql(@"
            UPDATE workflow.workflows
            SET owner_id = user_id;
        ");

        migrationBuilder.AlterColumn<int>(
            name: "owner_id",
            schema: "workflow",
            table: "workflows",
            type: "integer",
            nullable: false);

        migrationBuilder.CreateIndex(
            name: "ix_workflows_owner_id",
            schema: "workflow",
            table: "workflows",
            column: "owner_id");

        migrationBuilder.AddForeignKey(
            name: "fk_workflows_users_owner_id",
            schema: "workflow",
            table: "workflows",
            column: "owner_id",
            principalSchema: "identity",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.DropForeignKey(
            name: "fk_workflows_users_user_id",
            schema: "workflow",
            table: "workflows");

        migrationBuilder.DropIndex(
            name: "ix_workflows_user_id",
            schema: "workflow",
            table: "workflows");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "workflow",
            table: "workflows");
    }
}
```

### 數據轉換遷移

**場景: 分割字段**

```csharp
public partial class SplitUserFullName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 添加新列
        migrationBuilder.AddColumn<string>(
            name: "first_name",
            schema: "identity",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "last_name",
            schema: "identity",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        // 2. 數據轉換 (PostgreSQL 函數)
        migrationBuilder.Sql(@"
            UPDATE identity.users
            SET
                first_name = SPLIT_PART(full_name, ' ', 1),
                last_name = CASE
                    WHEN POSITION(' ' IN full_name) > 0
                    THEN SUBSTRING(full_name FROM POSITION(' ' IN full_name) + 1)
                    ELSE ''
                END
            WHERE full_name IS NOT NULL;
        ");

        // 3. 設置為非空
        migrationBuilder.AlterColumn<string>(
            name: "first_name",
            schema: "identity",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false);

        migrationBuilder.AlterColumn<string>(
            name: "last_name",
            schema: "identity",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false);

        // 4. 刪除舊列
        migrationBuilder.DropColumn(
            name: "full_name",
            schema: "identity",
            table: "users");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "full_name",
            schema: "identity",
            table: "users",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.Sql(@"
            UPDATE identity.users
            SET full_name = first_name || ' ' || last_name;
        ");

        migrationBuilder.AlterColumn<string>(
            name: "full_name",
            schema: "identity",
            table: "users",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false);

        migrationBuilder.DropColumn(name: "first_name", schema: "identity", table: "users");
        migrationBuilder.DropColumn(name: "last_name", schema: "identity", table: "users");
    }
}
```

### 大數據量遷移

**場景: 批量處理避免鎖定**

```csharp
public partial class AddWorkflowNodeHash : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 添加新列
        migrationBuilder.AddColumn<string>(
            name: "content_hash",
            schema: "workflow",
            table: "workflow_nodes",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        // 2. 批量更新 (PostgreSQL DO 塊)
        migrationBuilder.Sql(@"
            DO $$
            DECLARE
                batch_size INT := 1000;
                last_id INT := 0;
                updated INT;
            BEGIN
                LOOP
                    UPDATE workflow.workflow_nodes
                    SET content_hash = MD5(
                        COALESCE(name, '') ||
                        COALESCE(type, '') ||
                        COALESCE(configuration::TEXT, '')
                    )
                    WHERE id > last_id
                      AND id <= last_id + batch_size
                      AND content_hash IS NULL;

                    GET DIAGNOSTICS updated = ROW_COUNT;

                    EXIT WHEN updated = 0;

                    last_id := last_id + batch_size;

                    -- 提交每個批次
                    COMMIT;

                    -- 暫停避免過載
                    PERFORM pg_sleep(0.1);
                END LOOP;
            END $$;
        ");

        // 3. 設置為非空
        migrationBuilder.AlterColumn<string>(
            name: "content_hash",
            schema: "workflow",
            table: "workflow_nodes",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false);

        // 4. 添加索引
        migrationBuilder.CreateIndex(
            name: "ix_workflow_nodes_content_hash",
            schema: "workflow",
            table: "workflow_nodes",
            column: "content_hash");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_workflow_nodes_content_hash",
            schema: "workflow",
            table: "workflow_nodes");

        migrationBuilder.DropColumn(
            name: "content_hash",
            schema: "workflow",
            table: "workflow_nodes");
    }
}
```

## 環境管理

### 連接字符串配置

**appsettings.Development.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=aiworkflow_dev;Username=postgres;Password=dev_password;Include Error Detail=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**appsettings.Staging.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=staging-db.example.com;Port=5432;Database=aiworkflow_staging;Username=staging_user;Password=${DB_PASSWORD};SSL Mode=Require"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

**appsettings.Production.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require;Trust Server Certificate=false;Connection Idle Lifetime=300;Maximum Pool Size=100"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Error",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  }
}
```

### 環境變數管理

**使用 Azure Key Vault / AWS Secrets Manager**:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

// 從環境變數覆蓋
builder.Configuration.AddEnvironmentVariables(prefix: "AIWORKFLOW_");
```

**Docker Compose 環境配置**:

```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: aiworkflow_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: dev_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: .
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=aiworkflow_dev;Username=postgres;Password=dev_password"
    depends_on:
      - postgres

volumes:
  postgres_data:
```

## 生產環境遷移策略

### 無停機遷移 (Blue-Green Deployment)

**階段 1: 添加新結構 (向後兼容)**

```csharp
public partial class AddNewColumnCompatible : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 添加新列 (可空)
        migrationBuilder.AddColumn<string>(
            name: "new_column",
            schema: "workflow",
            table: "workflows",
            type: "text",
            nullable: true);

        // 2. 創建觸發器同步舊列和新列
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION sync_new_column()
            RETURNS TRIGGER AS $$
            BEGIN
                IF NEW.old_column IS NOT NULL THEN
                    NEW.new_column := NEW.old_column;
                END IF;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            CREATE TRIGGER sync_new_column_trigger
            BEFORE INSERT OR UPDATE ON workflow.workflows
            FOR EACH ROW
            EXECUTE FUNCTION sync_new_column();
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS sync_new_column_trigger ON workflow.workflows");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS sync_new_column");
        migrationBuilder.DropColumn(name: "new_column", schema: "workflow", table: "workflows");
    }
}
```

**階段 2: 部署新代碼 (使用新列)**

應用程序更新後開始讀寫新列,但仍保持舊列。

**階段 3: 移除舊結構**

```csharp
public partial class RemoveOldColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. 移除觸發器
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS sync_new_column_trigger ON workflow.workflows");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS sync_new_column");

        // 2. 刪除舊列
        migrationBuilder.DropColumn(
            name: "old_column",
            schema: "workflow",
            table: "workflows");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "old_column",
            schema: "workflow",
            table: "workflows",
            type: "text",
            nullable: true);
    }
}
```

### 生產環境遷移檢查清單

**遷移前檢查**:

- [ ] 在 Staging 環境完整測試遷移
- [ ] 備份生產數據庫
- [ ] 驗證回滾腳本可用
- [ ] 評估遷移執行時間
- [ ] 確認維護窗口或無停機策略
- [ ] 通知相關團隊和用戶

**遷移腳本準備**:

```bash
# 生成幂等性 SQL 腳本
dotnet ef migrations script \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --idempotent \
    --output production_migration.sql

# 生成從特定版本到最新的腳本
dotnet ef migrations script <FromMigration> \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --idempotent \
    --output incremental_migration.sql
```

**執行遷移**:

```bash
# 方式 1: 使用 dotnet ef (開發/測試環境)
dotnet ef database update \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --connection "Host=prod-db;Database=aiworkflow;..."

# 方式 2: 執行 SQL 腳本 (生產環境推薦)
psql -h prod-db.example.com \
     -U aiworkflow_admin \
     -d aiworkflow \
     -f production_migration.sql

# 方式 3: 應用程序啟動時自動遷移 (小型應用)
# Program.cs
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();  // 自動應用遷移
}
```

**遷移後驗證**:

```sql
-- 1. 檢查遷移歷史
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- 2. 驗證表結構
\d workflow.workflows

-- 3. 檢查數據完整性
SELECT COUNT(*) FROM workflow.workflows WHERE created_at IS NULL;

-- 4. 驗證索引
SELECT schemaname, tablename, indexname
FROM pg_indexes
WHERE schemaname = 'workflow';

-- 5. 檢查外鍵約束
SELECT
    tc.constraint_name,
    tc.table_schema,
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_schema = 'workflow';
```

## 回滾策略

### 回滾遷移

**場景 1: 遷移剛應用且無數據變更**

```bash
# 回滾到上一個遷移
dotnet ef database update PreviousMigration \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 或使用 SQL 腳本
dotnet ef migrations script CurrentMigration PreviousMigration \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API \
    --output rollback.sql
```

**場景 2: 遷移已應用且有數據變更**

```bash
# 1. 停止應用程序
# 2. 備份當前數據庫
pg_dump -h localhost -U postgres -d aiworkflow > backup_before_rollback.sql

# 3. 執行回滾
dotnet ef database update PreviousMigration \
    --project AIWorkflow.Infrastructure \
    --startup-project AIWorkflow.API

# 4. 驗證數據完整性
# 5. 重啟應用程序
```

**場景 3: 無法自動回滾,需要手動介入**

```sql
-- 手動回滾腳本示例
BEGIN;

-- 1. 刪除新添加的列
ALTER TABLE workflow.workflows DROP COLUMN IF EXISTS new_column;

-- 2. 恢復舊列
ALTER TABLE workflow.workflows ADD COLUMN old_column TEXT;

-- 3. 恢復數據 (從備份或其他源)
UPDATE workflow.workflows w
SET old_column = b.old_column_value
FROM backup_table b
WHERE w.id = b.id;

-- 4. 更新遷移歷史
DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250115120000_ProblematicMigration';

COMMIT;
```

### 數據庫備份策略

**自動化備份腳本**:

```bash
#!/bin/bash
# backup_database.sh

# 配置
DB_HOST="localhost"
DB_PORT="5432"
DB_NAME="aiworkflow"
DB_USER="postgres"
BACKUP_DIR="/backups/postgresql"
DATE=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="$BACKUP_DIR/${DB_NAME}_${DATE}.sql"

# 創建備份目錄
mkdir -p "$BACKUP_DIR"

# 執行備份
pg_dump -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        -F c \
        -b \
        -v \
        -f "$BACKUP_FILE"

# 壓縮備份
gzip "$BACKUP_FILE"

# 刪除 30 天前的備份
find "$BACKUP_DIR" -name "*.sql.gz" -mtime +30 -delete

echo "Backup completed: ${BACKUP_FILE}.gz"
```

**恢復腳本**:

```bash
#!/bin/bash
# restore_database.sh

BACKUP_FILE=$1

if [ -z "$BACKUP_FILE" ]; then
    echo "Usage: $0 <backup_file>"
    exit 1
fi

# 解壓備份
if [[ "$BACKUP_FILE" == *.gz ]]; then
    gunzip -c "$BACKUP_FILE" > /tmp/restore.sql
    RESTORE_FILE="/tmp/restore.sql"
else
    RESTORE_FILE="$BACKUP_FILE"
fi

# 恢復數據庫
pg_restore -h localhost \
           -p 5432 \
           -U postgres \
           -d aiworkflow \
           -v \
           --clean \
           --if-exists \
           "$RESTORE_FILE"

# 清理臨時文件
rm -f /tmp/restore.sql

echo "Database restored from: $BACKUP_FILE"
```

## CI/CD 集成

### GitHub Actions 工作流程

```yaml
# .github/workflows/database-migration.yml
name: Database Migration

on:
  push:
    branches: [main, develop]
    paths:
      - 'AIWorkflow.Infrastructure/Persistence/Migrations/**'
  pull_request:
    branches: [main, develop]
    paths:
      - 'AIWorkflow.Infrastructure/Persistence/Migrations/**'

jobs:
  validate-migrations:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_DB: aiworkflow_test
          POSTGRES_USER: postgres
          POSTGRES_PASSWORD: test_password
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Install EF Core tools
        run: dotnet tool install --global dotnet-ef

      - name: Apply migrations
        run: |
          dotnet ef database update \
            --project AIWorkflow.Infrastructure \
            --startup-project AIWorkflow.API \
            --connection "Host=localhost;Port=5432;Database=aiworkflow_test;Username=postgres;Password=test_password"

      - name: Generate migration script
        run: |
          dotnet ef migrations script \
            --project AIWorkflow.Infrastructure \
            --startup-project AIWorkflow.API \
            --idempotent \
            --output migration.sql

      - name: Upload migration script
        uses: actions/upload-artifact@v4
        with:
          name: migration-script
          path: migration.sql

  deploy-staging:
    needs: validate-migrations
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/develop'

    steps:
      - uses: actions/checkout@v4

      - name: Download migration script
        uses: actions/download-artifact@v4
        with:
          name: migration-script

      - name: Apply migration to staging
        run: |
          psql "${{ secrets.STAGING_DB_CONNECTION }}" -f migration.sql

  deploy-production:
    needs: validate-migrations
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    environment: production

    steps:
      - uses: actions/checkout@v4

      - name: Download migration script
        uses: actions/download-artifact@v4
        with:
          name: migration-script

      - name: Backup production database
        run: |
          pg_dump "${{ secrets.PROD_DB_CONNECTION }}" \
            -F c \
            -f "backup_$(date +%Y%m%d_%H%M%S).dump"

      - name: Apply migration to production
        run: |
          psql "${{ secrets.PROD_DB_CONNECTION }}" -f migration.sql
```

### Azure DevOps Pipeline

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop
  paths:
    include:
      - AIWorkflow.Infrastructure/Persistence/Migrations/*

pool:
  vmImage: 'ubuntu-latest'

variables:
  dotnetVersion: '8.0.x'

stages:
  - stage: Validate
    jobs:
      - job: ValidateMigrations
        services:
          postgres:
            image: postgres:16
            ports:
              - 5432:5432
            env:
              POSTGRES_DB: aiworkflow_test
              POSTGRES_USER: postgres
              POSTGRES_PASSWORD: test_password

        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          - script: dotnet tool install --global dotnet-ef
            displayName: 'Install EF Core tools'

          - script: |
              dotnet ef database update \
                --project AIWorkflow.Infrastructure \
                --startup-project AIWorkflow.API \
                --connection "Host=localhost;Database=aiworkflow_test;Username=postgres;Password=test_password"
            displayName: 'Apply migrations'

          - script: |
              dotnet ef migrations script \
                --project AIWorkflow.Infrastructure \
                --startup-project AIWorkflow.API \
                --idempotent \
                --output $(Build.ArtifactStagingDirectory)/migration.sql
            displayName: 'Generate migration script'

          - task: PublishBuildArtifacts@1
            inputs:
              PathtoPublish: '$(Build.ArtifactStagingDirectory)'
              ArtifactName: 'migration-scripts'

  - stage: DeployStaging
    dependsOn: Validate
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
    jobs:
      - deployment: DeployToStaging
        environment: 'Staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - download: current
                  artifact: migration-scripts

                - task: PostgreSQL@1
                  inputs:
                    connectionString: $(StagingDbConnection)
                    sqlFile: '$(Pipeline.Workspace)/migration-scripts/migration.sql'

  - stage: DeployProduction
    dependsOn: Validate
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
    jobs:
      - deployment: DeployToProduction
        environment: 'Production'
        strategy:
          runOnce:
            deploy:
              steps:
                - download: current
                  artifact: migration-scripts

                - script: |
                    pg_dump "$(ProductionDbConnection)" \
                      -F c \
                      -f "backup_$(date +%Y%m%d_%H%M%S).dump"
                  displayName: 'Backup production database'

                - task: PostgreSQL@1
                  inputs:
                    connectionString: $(ProductionDbConnection)
                    sqlFile: '$(Pipeline.Workspace)/migration-scripts/migration.sql'
```

## 遷移最佳實踐

### 設計原則

1. **小而頻繁**: 小型遷移易於理解和回滾
2. **向後兼容**: 確保遷移不破壞現有功能
3. **幂等性**: 遷移可以多次執行而不出錯
4. **可回滾**: 始終提供 Down() 實現
5. **數據保護**: 重要變更前備份數據

### 常見陷阱

**❌ 避免**:

```csharp
// 1. 在遷移中執行業務邏輯
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ❌ 不要在遷移中計算業務數據
    migrationBuilder.Sql(@"
        UPDATE workflow.workflows
        SET priority = (SELECT COUNT(*) FROM workflow.workflow_nodes WHERE workflow_id = workflows.id)
    ");
}

// 2. 硬編碼數據
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ❌ 避免硬編碼業務數據
    migrationBuilder.InsertData(
        table: "users",
        columns: new[] { "id", "email", "password" },
        values: new object[] { 1, "admin@example.com", "hardcoded_password" }
    );
}

// 3. 忽略 Down() 方法
protected override void Down(MigrationBuilder migrationBuilder)
{
    // ❌ 空的 Down() 方法
}
```

**✅ 推薦**:

```csharp
// 1. 簡單的結構變更
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "description",
        schema: "workflow",
        table: "workflows",
        type: "text",
        nullable: true);
}

// 2. 使用種子數據配置
public sealed class RoleSeedData : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role { Id = 1, Name = "Admin", CreatedAt = DateTime.UtcNow }
        );
    }
}

// 3. 完整的 Down() 實現
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "description",
        schema: "workflow",
        table: "workflows");
}
```

## 檢查清單

### 開發階段
- [ ] 遷移命名清晰描述變更內容
- [ ] 遷移文件添加詳細註釋
- [ ] 實現完整的 Up() 和 Down() 方法
- [ ] 本地測試遷移和回滾
- [ ] 檢查生成的 SQL 是否符合預期

### 測試階段
- [ ] 在測試數據庫驗證遷移
- [ ] 測試數據遷移邏輯正確性
- [ ] 驗證索引和約束創建成功
- [ ] 測試回滾功能
- [ ] 性能測試 (大數據量場景)

### 部署階段
- [ ] Staging 環境完整測試
- [ ] 生成生產環境遷移腳本
- [ ] 備份生產數據庫
- [ ] 準備回滾腳本
- [ ] 評估維護窗口需求
- [ ] 執行遷移並驗證
- [ ] 監控應用程序運行狀況

## 相關文檔

- [數據庫設計原則](./database-design-principles.md)
- [Entity Framework Core 配置](./entity-framework-core-configuration.md)
- [查詢性能優化](./query-performance-optimization.md)
- [Phase 8: 部署架構](../8-deployment-architecture/README.md)

## 參考資源

### 官方文檔
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL Backup and Restore](https://www.postgresql.org/docs/current/backup.html)

### 最佳實踐
- [Database Migration Best Practices](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing)
- [Zero-Downtime Database Migrations](https://www.braintreepayments.com/blog/safe-operations-for-high-volume-postgresql/)

---

**文檔維護**: Database Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
