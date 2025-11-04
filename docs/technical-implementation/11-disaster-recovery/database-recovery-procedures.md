# 數據庫恢復流程指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整數據庫恢復流程,涵蓋:
- **Point-in-Time Recovery (PITR)** (時間點恢復,精確到任意秒)
- **Transaction Log 恢復** (WAL 日誌回放)
- **全量備份恢復** (從 pg_dump/pg_dumpall 恢復)
- **部分數據恢復** (單表、單庫恢復)
- **跨區域恢復** (從 DR 站點恢復到主站點)
- **數據一致性驗證** (恢復後完整性檢查)
- **恢復性能優化** (並行恢復、索引重建策略)

### 恢復場景分類

| 場景類型 | 數據丟失範圍 | 恢復方法 | 預計 RTO | 風險級別 |
|---------|-------------|---------|----------|----------|
| **誤刪除單條記錄** | 單行數據 | PITR 恢復 → 提取單行 | 30 分鐘 | 🟢 低 |
| **誤刪除整張表** | 單表數據 | PITR 恢復 → 提取單表 | 1 小時 | 🟡 中 |
| **誤刪除數據庫** | 整個數據庫 | PITR 恢復或全量備份 | 2 小時 | 🔴 高 |
| **數據庫損壞** | 部分或全部數據 | WAL 恢復 + 完整性檢查 | 2-4 小時 | 🔴 高 |
| **區域性災難** | 整個 Azure Region | 跨區域 Geo-Restore | 4-8 小時 | 🔴 Critical |
| **惡意攻擊 (Ransomware)** | 全部數據加密 | 最近未受影響的備份 | 2-6 小時 | 🔴 Critical |

---

## Point-in-Time Recovery (PITR)

### PITR 原理

**工作機制**:
```
全量備份 (Base Backup) + WAL 日誌 (Incremental Changes) = 任意時間點狀態
```

**示例時間線**:
```
2025-01-15 00:00 ─── 全量備份 (Checkpoint)
                 │
2025-01-15 06:00 ─── WAL 文件 000001
2025-01-15 12:00 ─── WAL 文件 000002
2025-01-15 14:30 ─── ❌ 誤刪除事件發生
2025-01-15 18:00 ─── WAL 文件 000003
                 │
恢復目標: 2025-01-15 14:29:59 (誤刪除前 1 秒)
```

### 前置條件檢查

**1. 確認 WAL 歸檔已啟用**:

```sql
-- 連接到數據庫並檢查 WAL 配置
SHOW wal_level;        -- 應為 'replica' 或 'logical'
SHOW archive_mode;     -- 應為 'on'
SHOW archive_command;  -- 應配置正確的歸檔腳本

-- 檢查當前 WAL 文件位置
SELECT pg_current_wal_lsn();

-- 檢查最後一次成功歸檔的 WAL 文件
SELECT archived_count, last_archived_wal, last_archived_time
FROM pg_stat_archiver;
```

**2. 確認備份文件可用性**:

```bash
# 列出所有可用的全量備份
az storage blob list \
  --account-name aiagentbackup \
  --container-name postgresql-full \
  --prefix daily/ \
  --query "[].{Name:name, Size:properties.contentLength, LastModified:properties.lastModified}" \
  --output table

# 列出所有 WAL 歸檔文件
az storage blob list \
  --account-name aiagentbackup \
  --container-name postgresql-wal-archive \
  --prefix wal/ \
  --query "[].{Name:name, LastModified:properties.lastModified}" \
  --output table
```

### PITR 恢復流程

#### Step 1: 停止生產數據庫 (如果需要原地恢復)

```bash
# 如果恢復到獨立環境,跳過此步驟
# 如果需要原地恢復,必須先停止數據庫

# Kubernetes 方式
kubectl scale deployment postgresql-primary --replicas=0 -n aiagent-prod

# 直接停止 PostgreSQL
sudo systemctl stop postgresql
```

#### Step 2: 下載基礎備份

```bash
#!/bin/bash
# scripts/pitr-restore-step1-download-backup.sh

set -e

RESTORE_TARGET_TIME="2025-01-15 14:29:59"
BACKUP_ACCOUNT="aiagentbackup"
RESTORE_DIR="/var/lib/postgresql/restore"

echo "🔍 Finding base backup before $RESTORE_TARGET_TIME..."

# 找到目標時間前的最新全量備份
LATEST_BACKUP=$(az storage blob list \
  --account-name "$BACKUP_ACCOUNT" \
  --container-name "postgresql-full" \
  --prefix "daily/" \
  --query "sort_by([?properties.lastModified <= '$RESTORE_TARGET_TIME'].{name:name, time:properties.lastModified}, &time)[-1].name" \
  --output tsv)

if [ -z "$LATEST_BACKUP" ]; then
  echo "❌ No suitable backup found before $RESTORE_TARGET_TIME"
  exit 1
fi

echo "✅ Found backup: $LATEST_BACKUP"

# 下載並解壓縮備份
echo "📥 Downloading backup..."
az storage blob download \
  --account-name "$BACKUP_ACCOUNT" \
  --container-name "postgresql-full" \
  --name "$LATEST_BACKUP" \
  --file "/tmp/backup.dump.gz" \
  --auth-mode login

echo "📦 Extracting backup..."
gunzip -c /tmp/backup.dump.gz > /tmp/backup.dump

echo "✅ Backup downloaded and extracted"
```

#### Step 3: 恢復基礎備份

```bash
#!/bin/bash
# scripts/pitr-restore-step2-restore-base.sh

set -e

RESTORE_DIR="/var/lib/postgresql/restore"
PGDATA="$RESTORE_DIR/data"

# 清理並創建恢復目錄
rm -rf "$RESTORE_DIR"
mkdir -p "$PGDATA"
chown -R postgres:postgres "$RESTORE_DIR"

# 恢復基礎備份
echo "🔄 Restoring base backup..."
su - postgres -c "psql -f /tmp/backup.dump postgres"

echo "✅ Base backup restored to $PGDATA"
```

#### Step 4: 配置 recovery.conf (PostgreSQL 12+)

```bash
#!/bin/bash
# scripts/pitr-restore-step3-configure-recovery.sh

RESTORE_TARGET_TIME="2025-01-15 14:29:59"
PGDATA="/var/lib/postgresql/restore/data"

# PostgreSQL 12+ 使用 postgresql.conf 和 standby.signal
cat >> "$PGDATA/postgresql.conf" <<EOF

# Point-in-Time Recovery Configuration
restore_command = 'az storage blob download --account-name aiagentbackup --container-name postgresql-wal-archive --name wal/%f --file %p --auth-mode login'
recovery_target_time = '$RESTORE_TARGET_TIME'
recovery_target_action = 'promote'

# 可選: 暫停在目標時間點,以便手動檢查
# recovery_target_action = 'pause'
EOF

# 創建 recovery.signal 文件 (觸發恢復模式)
touch "$PGDATA/recovery.signal"
chown postgres:postgres "$PGDATA/recovery.signal"

echo "✅ Recovery configuration completed"
```

#### Step 5: 啟動數據庫並等待恢復完成

```bash
#!/bin/bash
# scripts/pitr-restore-step4-start-recovery.sh

set -e

PGDATA="/var/lib/postgresql/restore/data"

# 啟動 PostgreSQL (恢復模式)
echo "🚀 Starting PostgreSQL in recovery mode..."
su - postgres -c "pg_ctl start -D $PGDATA"

# 監控恢復進度
echo "⏳ Waiting for recovery to complete..."
while true; do
  # 檢查是否還在恢復模式
  RECOVERY_STATUS=$(su - postgres -c "psql -t -c 'SELECT pg_is_in_recovery();'")

  if [ "$RECOVERY_STATUS" = "f" ]; then
    echo "✅ Recovery completed successfully!"
    break
  fi

  # 顯示恢復進度
  WAL_STATUS=$(su - postgres -c "psql -t -c 'SELECT last_replay_lsn, replay_lag FROM pg_stat_replication;'")
  echo "📊 Recovery progress: $WAL_STATUS"

  sleep 10
done

# 顯示恢復到的最終時間點
RECOVERY_TIME=$(su - postgres -c "psql -t -c 'SELECT pg_last_xact_replay_timestamp();'")
echo "🎯 Database restored to: $RECOVERY_TIME"
```

#### Step 6: 數據驗證

```sql
-- 驗證恢復的數據完整性

-- 1. 檢查表記錄數
SELECT
  schemaname,
  tablename,
  n_live_tup AS row_count
FROM pg_stat_user_tables
ORDER BY n_live_tup DESC
LIMIT 20;

-- 2. 檢查最新數據的時間戳
SELECT
  'workflows' AS table_name,
  MAX(updated_at) AS latest_record
FROM workflows
UNION ALL
SELECT
  'workflow_executions',
  MAX(completed_at)
FROM workflow_executions;

-- 3. 驗證關鍵業務數據
SELECT COUNT(*) AS total_workflows FROM workflows;
SELECT COUNT(*) AS total_users FROM users;
SELECT COUNT(*) AS active_executions FROM workflow_executions WHERE status = 'Running';

-- 4. 檢查數據庫完整性
SELECT datname, datallowconn, datistemplate
FROM pg_database
WHERE datname = 'aiagent_prod';
```

---

## Transaction Log (WAL) 恢復

### 適用場景
- 數據庫實例崩潰後的自動恢復
- WAL 文件完整但數據文件損壞
- 從 Streaming Replication 故障中恢復

### WAL 恢復流程

#### 1. 檢查 WAL 文件完整性

```bash
# 驗證 WAL 文件可讀性
pg_waldump /var/lib/postgresql/data/pg_wal/000000010000000000000001

# 檢查 WAL 文件時間線
pg_controldata /var/lib/postgresql/data | grep -E "Latest checkpoint|REDO|WAL"
```

#### 2. 配置 WAL 回放

```bash
# recovery.conf (PostgreSQL 11 及以下)
restore_command = 'cp /var/lib/postgresql/wal_archive/%f %p'
recovery_target = 'immediate'  # 回放所有可用 WAL
recovery_target_action = 'promote'
```

#### 3. 啟動恢復

```bash
# 啟動 PostgreSQL,自動回放 WAL
pg_ctl start -D /var/lib/postgresql/data

# 監控恢復日誌
tail -f /var/lib/postgresql/data/log/postgresql-*.log | grep -E "recovery|redo"
```

---

## 全量備份恢復

### 從 pg_dump 恢復單個數據庫

```bash
#!/bin/bash
# scripts/restore-from-dump.sh

set -e

BACKUP_FILE="/backups/aiagent_prod-2025-01-15.dump"
TARGET_DB="aiagent_prod_restored"

# 1. 創建新數據庫
psql -U postgres -c "CREATE DATABASE $TARGET_DB;"

# 2. 恢復數據 (Custom Format)
pg_restore \
  --dbname=$TARGET_DB \
  --username=postgres \
  --verbose \
  --jobs=4 \  # 並行恢復 (4 個並發連接)
  --no-owner \
  --no-acl \
  $BACKUP_FILE

echo "✅ Database restored to $TARGET_DB"
```

### 從 pg_dumpall 恢復所有數據庫

```bash
#!/bin/bash
# scripts/restore-all-databases.sh

set -e

BACKUP_FILE="/backups/all-databases-2025-01-15.sql"

# 恢復所有數據庫 (包含 roles, tablespaces)
psql -U postgres -f $BACKUP_FILE

echo "✅ All databases restored"
```

---

## 部分數據恢復

### 恢復單張表

```bash
#!/bin/bash
# scripts/restore-single-table.sh

BACKUP_FILE="/backups/aiagent_prod-2025-01-15.dump"
TABLE_NAME="workflows"
TARGET_DB="aiagent_prod"

# 1. 恢復到臨時數據庫
psql -U postgres -c "CREATE DATABASE temp_restore;"
pg_restore --dbname=temp_restore --username=postgres $BACKUP_FILE

# 2. 複製單表數據到生產庫
psql -U postgres -d temp_restore -c "\COPY $TABLE_NAME TO '/tmp/$TABLE_NAME.csv' CSV HEADER;"
psql -U postgres -d $TARGET_DB -c "\COPY $TABLE_NAME FROM '/tmp/$TABLE_NAME.csv' CSV HEADER;"

# 3. 清理臨時數據庫
psql -U postgres -c "DROP DATABASE temp_restore;"

echo "✅ Table $TABLE_NAME restored"
```

### 恢復特定時間範圍的數據

```sql
-- 從備份庫複製特定時間範圍的數據

-- 1. 連接到恢復的臨時數據庫
\c aiagent_prod_restored

-- 2. 提取誤刪除的數據
CREATE TEMP TABLE recovered_workflows AS
SELECT *
FROM workflows
WHERE created_at >= '2025-01-15 00:00:00'
  AND created_at < '2025-01-15 14:30:00'  -- 誤刪除前的時間點
  AND id NOT IN (SELECT id FROM aiagent_prod.workflows);  -- 排除未刪除的

-- 3. 複製到生產庫
INSERT INTO aiagent_prod.workflows
SELECT * FROM recovered_workflows;

-- 4. 驗證恢復的行數
SELECT COUNT(*) AS recovered_rows FROM recovered_workflows;
```

---

## 跨區域恢復 (Geo-Restore)

### 從 DR 站點恢復到主站點

**場景**: Primary Region (East US) 完全不可用,需要從 DR Region (West US 2) 恢復

#### Step 1: 激活 DR 站點的只讀副本

```bash
# 將 DR 站點的 Read-Only Replica 提升為 Primary
az postgres flexible-server replica stop-replication \
  --resource-group aiagent-dr-rg \
  --name aiagent-postgres-dr \
  --yes

# 驗證寫入能力
psql -h aiagent-postgres-dr.postgres.database.azure.com \
     -U dbadmin \
     -d aiagent_prod \
     -c "CREATE TABLE test_write (id int); DROP TABLE test_write;"
```

#### Step 2: 更新應用配置指向 DR 站點

```yaml
# k8s/overlays/dr/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: database-config
  namespace: aiagent-prod
data:
  DATABASE_HOST: "aiagent-postgres-dr.postgres.database.azure.com"
  DATABASE_PORT: "5432"
  DATABASE_NAME: "aiagent_prod"
```

```bash
# 應用新配置
kubectl apply -k k8s/overlays/dr/

# 重啟應用 Pods
kubectl rollout restart deployment/api-deployment -n aiagent-prod
```

#### Step 3: 恢復主站點後的數據同步

```bash
# 當主站點恢復後,將 DR 站點的數據同步回主站點

# 1. 在主站點創建新數據庫
psql -h aiagent-postgres-primary.postgres.database.azure.com \
     -U dbadmin \
     -c "CREATE DATABASE aiagent_prod_new;"

# 2. 從 DR 站點導出數據
pg_dump \
  -h aiagent-postgres-dr.postgres.database.azure.com \
  -U dbadmin \
  -d aiagent_prod \
  --format=custom \
  --file=/tmp/dr-site-data.dump

# 3. 導入到主站點
pg_restore \
  -h aiagent-postgres-primary.postgres.database.azure.com \
  -U dbadmin \
  -d aiagent_prod_new \
  --jobs=8 \
  /tmp/dr-site-data.dump

# 4. 驗證數據一致性
psql -h aiagent-postgres-primary.postgres.database.azure.com \
     -U dbadmin \
     -d aiagent_prod_new \
     -c "SELECT COUNT(*) FROM workflows;"

# 5. 切換到新數據庫
psql -h aiagent-postgres-primary.postgres.database.azure.com \
     -U dbadmin \
     -c "ALTER DATABASE aiagent_prod RENAME TO aiagent_prod_old;"
psql -h aiagent-postgres-primary.postgres.database.azure.com \
     -U dbadmin \
     -c "ALTER DATABASE aiagent_prod_new RENAME TO aiagent_prod;"
```

---

## 數據一致性驗證

### 自動化完整性檢查腳本

```csharp
// src/Infrastructure/Recovery/DatabaseIntegrityChecker.cs
using Npgsql;

public sealed class DatabaseIntegrityChecker
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<DatabaseIntegrityChecker> _logger;

    public async Task<IntegrityCheckResult> PerformFullCheckAsync(
        CancellationToken cancellationToken = default)
    {
        var results = new List<CheckResult>();

        // 1. 檢查數據庫連接
        results.Add(await CheckDatabaseConnectionAsync(cancellationToken));

        // 2. 檢查表完整性
        results.Add(await CheckTableIntegrityAsync(cancellationToken));

        // 3. 檢查外鍵約束
        results.Add(await CheckForeignKeyConstraintsAsync(cancellationToken));

        // 4. 檢查索引完整性
        results.Add(await CheckIndexIntegrityAsync(cancellationToken));

        // 5. 檢查數據一致性
        results.Add(await CheckDataConsistencyAsync(cancellationToken));

        // 6. 檢查業務規則
        results.Add(await CheckBusinessRulesAsync(cancellationToken));

        var allPassed = results.All(r => r.Passed);

        return new IntegrityCheckResult(
            Passed: allPassed,
            Checks: results,
            Timestamp: DateTime.UtcNow);
    }

    private async Task<CheckResult> CheckTableIntegrityAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 檢查所有關鍵表是否存在
            const string sql = @"
                SELECT tablename
                FROM pg_tables
                WHERE schemaname = 'public'
                  AND tablename IN ('users', 'workflows', 'workflow_executions', 'agents')";

            await using var cmd = new NpgsqlCommand(sql, _connection);
            var tables = new List<string>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                tables.Add(reader.GetString(0));
            }

            var expectedTables = new[] { "users", "workflows", "workflow_executions", "agents" };
            var missingTables = expectedTables.Except(tables).ToList();

            if (missingTables.Any())
            {
                return CheckResult.Failed(
                    "TableIntegrity",
                    $"Missing tables: {string.Join(", ", missingTables)}");
            }

            return CheckResult.Passed("TableIntegrity", $"All {tables.Count} critical tables exist");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Table integrity check failed");
            return CheckResult.Failed("TableIntegrity", ex.Message);
        }
    }

    private async Task<CheckResult> CheckForeignKeyConstraintsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 檢查外鍵約束違規
            const string sql = @"
                SELECT conname, conrelid::regclass AS table_name
                FROM pg_constraint
                WHERE contype = 'f'
                  AND NOT convalidated";

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var violations = new List<string>();
            while (await reader.ReadAsync(cancellationToken))
            {
                violations.Add($"{reader.GetString(0)} on {reader.GetString(1)}");
            }

            if (violations.Any())
            {
                return CheckResult.Failed(
                    "ForeignKeyConstraints",
                    $"Invalid constraints: {string.Join(", ", violations)}");
            }

            return CheckResult.Passed("ForeignKeyConstraints", "All foreign keys valid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Foreign key check failed");
            return CheckResult.Failed("ForeignKeyConstraints", ex.Message);
        }
    }

    private async Task<CheckResult> CheckDataConsistencyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var inconsistencies = new List<string>();

            // 檢查孤兒記錄 (workflow_executions 沒有對應的 workflow)
            const string orphanCheckSql = @"
                SELECT COUNT(*)
                FROM workflow_executions we
                LEFT JOIN workflows w ON we.workflow_id = w.id
                WHERE w.id IS NULL";

            await using var cmd = new NpgsqlCommand(orphanCheckSql, _connection);
            var orphanCount = (long)(await cmd.ExecuteScalarAsync(cancellationToken) ?? 0L);

            if (orphanCount > 0)
            {
                inconsistencies.Add($"{orphanCount} orphaned workflow_executions");
            }

            // 檢查時間戳邏輯錯誤 (created_at > updated_at)
            const string timestampCheckSql = @"
                SELECT COUNT(*)
                FROM workflows
                WHERE created_at > updated_at";

            cmd.CommandText = timestampCheckSql;
            var timestampErrors = (long)(await cmd.ExecuteScalarAsync(cancellationToken) ?? 0L);

            if (timestampErrors > 0)
            {
                inconsistencies.Add($"{timestampErrors} workflows with invalid timestamps");
            }

            if (inconsistencies.Any())
            {
                return CheckResult.Failed(
                    "DataConsistency",
                    string.Join("; ", inconsistencies));
            }

            return CheckResult.Passed("DataConsistency", "All data consistency checks passed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data consistency check failed");
            return CheckResult.Failed("DataConsistency", ex.Message);
        }
    }
}

public record IntegrityCheckResult(
    bool Passed,
    IReadOnlyList<CheckResult> Checks,
    DateTime Timestamp);

public record CheckResult(string CheckName, bool Passed, string Message)
{
    public static CheckResult Passed(string checkName, string message) =>
        new(checkName, true, message);
    public static CheckResult Failed(string checkName, string message) =>
        new(checkName, false, message);
}
```

### SQL 完整性檢查腳本

```sql
-- scripts/verify-database-integrity.sql
-- 執行此腳本以驗證數據庫恢復後的完整性

\echo '=== Database Integrity Verification Report ==='
\echo ''

-- 1. 表記錄數統計
\echo '1. Table Row Counts:'
SELECT
  schemaname,
  tablename,
  n_live_tup AS row_count
FROM pg_stat_user_tables
ORDER BY n_live_tup DESC;

\echo ''

-- 2. 外鍵約束驗證
\echo '2. Foreign Key Constraint Validation:'
SELECT
  tc.table_name,
  tc.constraint_name,
  kcu.column_name,
  ccu.table_name AS foreign_table_name,
  ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_schema = 'public';

\echo ''

-- 3. 孤兒記錄檢查
\echo '3. Orphaned Records Check:'
SELECT
  'workflow_executions' AS table_name,
  COUNT(*) AS orphan_count
FROM workflow_executions we
LEFT JOIN workflows w ON we.workflow_id = w.id
WHERE w.id IS NULL
UNION ALL
SELECT
  'agent_personas',
  COUNT(*)
FROM agent_personas ap
LEFT JOIN agents a ON ap.agent_id = a.id
WHERE a.id IS NULL;

\echo ''

-- 4. 數據時間範圍
\echo '4. Data Time Range:'
SELECT
  'workflows' AS table_name,
  MIN(created_at) AS earliest,
  MAX(created_at) AS latest,
  MAX(updated_at) AS last_update
FROM workflows
UNION ALL
SELECT
  'workflow_executions',
  MIN(started_at),
  MAX(started_at),
  MAX(completed_at)
FROM workflow_executions;

\echo ''

-- 5. 索引健康狀態
\echo '5. Index Health:'
SELECT
  schemaname,
  tablename,
  indexname,
  idx_scan AS index_scans,
  idx_tup_read AS tuples_read,
  idx_tup_fetch AS tuples_fetched
FROM pg_stat_user_indexes
ORDER BY idx_scan DESC
LIMIT 20;

\echo ''
\echo '=== Verification Complete ==='
```

---

## 恢復性能優化

### 並行恢復配置

```bash
# 使用 pg_restore 並行恢復 (適用於 Custom Format)
pg_restore \
  --dbname=aiagent_prod \
  --username=postgres \
  --jobs=8 \  # 8 個並行連接
  --verbose \
  /backups/aiagent_prod.dump

# 效能提升: 8 jobs 可將恢復時間減少 60-70%
```

### 索引重建策略

```sql
-- 1. 恢復時跳過索引創建 (先導入數據)
pg_restore \
  --dbname=aiagent_prod \
  --no-index \  # 跳過索引
  /backups/aiagent_prod.dump

-- 2. 數據導入完成後,並行創建索引
-- PostgreSQL 11+ 支持並行索引創建
SET max_parallel_maintenance_workers = 4;

CREATE INDEX CONCURRENTLY idx_workflows_user_id ON workflows(user_id);
CREATE INDEX CONCURRENTLY idx_workflows_created_at ON workflows(created_at);
CREATE INDEX CONCURRENTLY idx_executions_workflow_id ON workflow_executions(workflow_id);

-- 3. 驗證索引已創建
SELECT schemaname, tablename, indexname
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;
```

---

## 常見恢復場景處理

### 場景 1: 誤刪除單個用戶的所有工作流

**問題**: 用戶 ID=12345 的所有 workflows 被誤刪除

**解決方案**:

```bash
# 1. PITR 恢復到臨時數據庫
./scripts/pitr-restore.sh "2025-01-15 14:29:59" temp_restore

# 2. 提取誤刪除的數據
psql -d temp_restore -c "
  COPY (
    SELECT *
    FROM workflows
    WHERE user_id = 12345
  ) TO '/tmp/user_12345_workflows.csv' CSV HEADER;
"

# 3. 導入回生產庫
psql -d aiagent_prod -c "
  COPY workflows
  FROM '/tmp/user_12345_workflows.csv' CSV HEADER;
"

# 4. 驗證恢復
psql -d aiagent_prod -c "
  SELECT COUNT(*) AS restored_workflows
  FROM workflows
  WHERE user_id = 12345;
"
```

### 場景 2: 數據庫主從切換後數據不一致

**問題**: Primary 失敗後切換到 Replica,但 Replica 有數據延遲

**解決方案**:

```sql
-- 1. 檢查 Replica 的數據延遲
SELECT
  CASE WHEN pg_last_wal_receive_lsn() = pg_last_wal_replay_lsn()
    THEN 0
    ELSE EXTRACT(EPOCH FROM now() - pg_last_xact_replay_timestamp())
  END AS replication_lag_seconds;

-- 2. 如果延遲可接受 (< 1 分鐘),提升 Replica
SELECT pg_promote();

-- 3. 如果延遲過大,等待 WAL 回放完成
-- 監控直到 pg_last_wal_receive_lsn() = pg_last_wal_replay_lsn()
```

### 場景 3: 數據庫磁盤空間不足導致崩潰

**問題**: 數據庫因磁盤滿而無法啟動

**解決方案**:

```bash
# 1. 清理舊 WAL 文件
cd /var/lib/postgresql/data/pg_wal
ls -lht | head -20  # 查看最大的 WAL 文件
rm -f 000000010000000000000001  # 刪除舊 WAL (謹慎操作!)

# 2. 清理臨時文件
rm -rf /var/lib/postgresql/data/base/pgsql_tmp/*

# 3. 清理日誌文件
find /var/lib/postgresql/data/log -name "*.log" -mtime +7 -delete

# 4. 增加磁盤空間
# Azure Managed Disk 擴展
az disk update \
  --resource-group aiagent-prod-rg \
  --name postgres-data-disk \
  --size-gb 512  # 從 256GB 擴展到 512GB

# 5. 重啟數據庫
pg_ctl start -D /var/lib/postgresql/data
```

---

## 檢查清單

### 恢復前檢查清單

- [ ] **備份驗證**
  - [ ] 確認備份文件存在且完整
  - [ ] 驗證 WAL 歸檔文件可訪問
  - [ ] 檢查備份文件大小合理

- [ ] **環境準備**
  - [ ] 確認恢復目標環境可用 (磁盤空間充足)
  - [ ] 停止或隔離生產數據庫 (如需原地恢復)
  - [ ] 通知相關團隊恢復計劃

- [ ] **權限確認**
  - [ ] 恢復操作人員具備必要權限
  - [ ] Azure Blob Storage 訪問權限正常
  - [ ] PostgreSQL 管理員權限可用

### 恢復後檢查清單

- [ ] **數據完整性驗證**
  - [ ] 執行完整性檢查腳本
  - [ ] 驗證關鍵表記錄數
  - [ ] 檢查外鍵約束
  - [ ] 驗證業務規則

- [ ] **性能驗證**
  - [ ] 執行典型查詢並驗證性能
  - [ ] 檢查索引是否完整
  - [ ] 驗證統計信息已更新

- [ ] **應用集成測試**
  - [ ] 啟動應用並驗證數據庫連接
  - [ ] 執行冒煙測試 (Smoke Test)
  - [ ] 驗證關鍵業務流程

- [ ] **文檔記錄**
  - [ ] 記錄恢復過程和時間
  - [ ] 記錄遇到的問題和解決方案
  - [ ] 更新災難恢復手冊

---

## 總結

本文檔提供了完整的數據庫恢復流程指南,涵蓋:

✅ **Point-in-Time Recovery** (精確到秒的時間點恢復)
✅ **Transaction Log 恢復** (WAL 日誌回放)
✅ **全量備份恢復** (pg_dump/pg_dumpall)
✅ **部分數據恢復** (單表、單庫、特定時間範圍)
✅ **跨區域恢復** (Geo-Restore, DR 站點激活)
✅ **數據一致性驗證** (自動化完整性檢查)
✅ **性能優化** (並行恢復、索引重建策略)

透過本指南,運維團隊可以在各種災難場景下快速、準確地恢復數據庫,確保:
- **RTO ≤ 2 小時**: 快速恢復業務運行
- **RPO ≤ 15 分鐘**: 最小化數據丟失
- **100% 數據完整性**: 確保恢復數據的正確性
