# 19. 備份恢復流程 (Backup and Restore Procedures)

## 文檔資訊

| 項目 | 內容 |
|------|------|
| **文檔版本** | 1.0.0 |
| **創建日期** | 2025-01-15 |
| **最後更新** | 2025-01-15 |
| **狀態** | Draft |
| **作者** | AI Workflow Platform Team |
| **關聯文檔** | 18-DISASTER-RECOVERY.md, 06-DATABASE-SCHEMA.md |

---

## 目錄

- [19.1 備份策略總覽](#191-備份策略總覽)
- [19.2 數據庫備份](#192-數據庫備份)
- [19.3 應用數據備份](#193-應用數據備份)
- [19.4 配置備份](#194-配置備份)
- [19.5 恢復流程](#195-恢復流程)
- [19.6 備份驗證](#196-備份驗證)
- [19.7 自動化腳本](#197-自動化腳本)

---

## 19.1 備份策略總覽

### 19.1.1 3-2-1 備份原則

**3-2-1 規則:**
- **3 份副本**: 原始數據 + 2 份備份
- **2 種介質**: 本地存儲 + 雲端存儲
- **1 份異地**: 至少 1 份異地備份

**應用到 AI Workflow Platform:**

```yaml
backup_copies:
  copy_1: 生產數據 (East US)
  copy_2: 本地備份 (East US Azure Blob)
  copy_3: 異地備份 (West Europe GRS)

storage_media:
  media_1: Azure Managed Disk (生產環境)
  media_2: Azure Blob Storage (Hot tier)

geographic_separation:
  primary: East US
  secondary: West Europe (自動異地複製)
```

### 19.1.2 備份頻率和保留

| 數據類型 | 備份頻率 | 保留期限 | 存儲位置 | 備份類型 |
|---------|---------|---------|---------|---------|
| **PostgreSQL** | 每日 | 30 days | Azure Blob (GRS) | Full |
| **PostgreSQL WAL** | 持續 | 7 days | Azure Blob | Incremental |
| **Redis RDB** | 每 15 分鐘 | 24 hours | Local + Azure Blob | Snapshot |
| **Redis AOF** | 持續 | 24 hours | Local | Incremental |
| **Blob Storage** | 實時 | N/A | GRS 自動複製 | Continuous |
| **Kubernetes Config** | 每日 | 90 days | Git + Azure Blob | Full |
| **Secrets** | 每日 | 90 days | Azure Key Vault | Full |
| **Application Logs** | 實時 | 30 days | Loki + Azure Blob | Continuous |

### 19.1.3 備份優先級

**P0 (Critical) - 必須備份:**
- PostgreSQL 數據庫
- Kubernetes ConfigMaps 和 Secrets
- Azure Key Vault 密鑰

**P1 (High) - 應該備份:**
- Redis 持久化數據
- RabbitMQ 隊列配置
- 應用日誌 (最近 30 天)

**P2 (Medium) - 可選備份:**
- Prometheus 監控數據
- Grafana 儀表板配置
- 臨時文件和緩存

---

## 19.2 數據庫備份

### 19.2.1 PostgreSQL 自動備份

**Azure Database for PostgreSQL 內建備份:**

```yaml
azure_postgresql_backup:
  automated_backup:
    enabled: true
    frequency: Daily
    time: "02:00 UTC"
    retention_days: 30
    geo_redundant: true

  point_in_time_restore:
    enabled: true
    retention_days: 7
    wal_archiving: Continuous
    restore_granularity: "Any point within 7 days"

  backup_storage:
    type: GRS (Geo-Redundant Storage)
    encryption: AES-256
    redundancy: "3 copies in primary + 3 copies in secondary region"
```

**手動備份腳本:**

```bash
#!/bin/bash
# backup-postgresql.sh - PostgreSQL 手動備份

set -e

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/postgresql"
AZURE_CONTAINER="postgresql-backups"

echo "=== PostgreSQL Backup Started at $(date) ==="

# 創建備份目錄
mkdir -p $BACKUP_DIR

# 備份所有數據庫
DATABASES=(auth_db agent_db persona_db code_db sql_db knowledge_db workflow_db)

for DB in "${DATABASES[@]}"; do
    echo "Backing up database: $DB"

    # 執行 pg_dump
    pg_dump \
        -h postgresql.postgres.database.azure.com \
        -U postgres \
        -d $DB \
        -F c \  # Custom format (compressed)
        -f $BACKUP_DIR/${DB}_${TIMESTAMP}.dump

    # 壓縮備份 (雖然 -F c 已經壓縮，但再用 gzip 可以更小)
    gzip $BACKUP_DIR/${DB}_${TIMESTAMP}.dump

    echo "✓ $DB backup completed: ${DB}_${TIMESTAMP}.dump.gz"
done

# 備份全局對象 (roles, tablespaces, etc.)
echo "Backing up global objects..."
pg_dumpall \
    -h postgresql.postgres.database.azure.com \
    -U postgres \
    --globals-only \
    -f $BACKUP_DIR/globals_${TIMESTAMP}.sql

gzip $BACKUP_DIR/globals_${TIMESTAMP}.sql

# 上傳到 Azure Blob Storage
echo "Uploading backups to Azure Blob Storage..."
for file in $BACKUP_DIR/*_${TIMESTAMP}.*; do
    az storage blob upload \
        --account-name aiworkflowbackup \
        --container-name $AZURE_CONTAINER \
        --name "$(basename $file)" \
        --file "$file" \
        --tier Hot

    echo "✓ Uploaded: $(basename $file)"
done

# 計算備份大小
BACKUP_SIZE=$(du -sh $BACKUP_DIR | cut -f1)
echo "Total backup size: $BACKUP_SIZE"

# 清理 30 天前的本地備份
echo "Cleaning up old local backups..."
find $BACKUP_DIR -name "*.dump.gz" -mtime +30 -delete
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete

# 清理 Azure 中 90 天前的備份
echo "Cleaning up old Azure backups..."
CUTOFF_DATE=$(date -d "90 days ago" +%Y%m%d)
az storage blob list \
    --account-name aiworkflowbackup \
    --container-name $AZURE_CONTAINER \
    --query "[?properties.creationTime<'$CUTOFF_DATE'].name" \
    -o tsv | while read blob; do
    az storage blob delete \
        --account-name aiworkflowbackup \
        --container-name $AZURE_CONTAINER \
        --name "$blob"
    echo "✓ Deleted old backup: $blob"
done

echo "=== PostgreSQL Backup Completed at $(date) ==="

# 發送通知
curl -X POST https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK \
    -H 'Content-Type: application/json' \
    -d "{\"text\":\"✅ PostgreSQL backup completed successfully.\nTimestamp: $TIMESTAMP\nSize: $BACKUP_SIZE\"}"
```

**Kubernetes CronJob 配置:**

```yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: postgresql-backup
  namespace: ai-workflow-prod
spec:
  schedule: "0 2 * * *"  # 每天 02:00 UTC
  successfulJobsHistoryLimit: 3
  failedJobsHistoryLimit: 1
  jobTemplate:
    spec:
      template:
        spec:
          restartPolicy: OnFailure
          containers:
          - name: backup
            image: postgres:16-alpine
            command:
            - /bin/bash
            - -c
            - |
              # 安裝 Azure CLI
              apk add --no-cache python3 py3-pip
              pip3 install azure-cli

              # 執行備份腳本
              /scripts/backup-postgresql.sh

            env:
            - name: PGPASSWORD
              valueFrom:
                secretKeyRef:
                  name: postgresql-secrets
                  key: postgres-password

            - name: AZURE_STORAGE_ACCOUNT
              value: aiworkflowbackup

            - name: AZURE_STORAGE_KEY
              valueFrom:
                secretKeyRef:
                  name: azure-storage-secrets
                  key: storage-key

            volumeMounts:
            - name: backup-scripts
              mountPath: /scripts
            - name: backup-storage
              mountPath: /backups

          volumes:
          - name: backup-scripts
            configMap:
              name: backup-scripts
              defaultMode: 0755

          - name: backup-storage
            persistentVolumeClaim:
              claimName: backup-pvc
```

### 19.2.2 PostgreSQL 時間點恢復 (PITR)

**WAL 歸檔配置:**

```sql
-- postgresql.conf 配置
wal_level = replica
archive_mode = on
archive_command = 'az storage blob upload --account-name aiworkflowbackup --container-name wal-archive --name %f --file %p'
archive_timeout = 300  -- 5 minutes
```

**PITR 恢復腳本:**

```bash
#!/bin/bash
# restore-pitr.sh - 時間點恢復

TARGET_TIME="$1"  # 例如: "2025-01-15 10:30:00"

if [ -z "$TARGET_TIME" ]; then
    echo "Usage: ./restore-pitr.sh 'YYYY-MM-DD HH:MM:SS'"
    exit 1
fi

echo "=== Point-in-Time Recovery ==="
echo "Target time: $TARGET_TIME"

# 使用 Azure CLI 創建 PITR 恢復
az postgres flexible-server restore \
    --resource-group ai-workflow-rg \
    --name ai-workflow-db-restored \
    --source-server ai-workflow-db-prod \
    --restore-time "$TARGET_TIME" \
    --location eastus

echo "Waiting for restore to complete..."
az postgres flexible-server wait \
    --resource-group ai-workflow-rg \
    --name ai-workflow-db-restored \
    --created \
    --timeout 3600

# 驗證恢復的數據
echo "Verifying restored data..."
psql -h ai-workflow-db-restored.postgres.database.azure.com \
     -U postgres \
     -d agent_db \
     -c "SELECT 'Agents', COUNT(*) FROM agents UNION ALL
         SELECT 'Personas', COUNT(*) FROM personas UNION ALL
         SELECT 'Executions', COUNT(*) FROM executions;"

echo ""
echo "✅ PITR restore completed successfully"
echo "Restored server: ai-workflow-db-restored.postgres.database.azure.com"
echo ""
echo "Next steps:"
echo "1. Verify data integrity"
echo "2. Update application connection strings"
echo "3. Test application functionality"
echo "4. Delete old server after confirmation"
```

### 19.2.3 Redis 備份

**Redis RDB Snapshot:**

```bash
#!/bin/bash
# backup-redis.sh - Redis 備份

set -e

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/redis"
AZURE_CONTAINER="redis-backups"

echo "=== Redis Backup Started at $(date) ==="

mkdir -p $BACKUP_DIR

# 觸發 BGSAVE (後台保存)
redis-cli -h redis -a "$REDIS_PASSWORD" BGSAVE

# 等待 BGSAVE 完成
while [ "$(redis-cli -h redis -a "$REDIS_PASSWORD" LASTSAVE)" -eq "$LAST_SAVE" ]; do
    sleep 1
done

# 複製 RDB 文件
cp /data/dump.rdb $BACKUP_DIR/redis_${TIMESTAMP}.rdb

# 壓縮備份
gzip $BACKUP_DIR/redis_${TIMESTAMP}.rdb

# 上傳到 Azure
az storage blob upload \
    --account-name aiworkflowbackup \
    --container-name $AZURE_CONTAINER \
    --name "redis_${TIMESTAMP}.rdb.gz" \
    --file "$BACKUP_DIR/redis_${TIMESTAMP}.rdb.gz"

echo "✓ Redis backup uploaded"

# 清理 24 小時前的備份
find $BACKUP_DIR -name "*.rdb.gz" -mtime +1 -delete

echo "=== Redis Backup Completed at $(date) ==="
```

**Redis AOF 持久化:**

```conf
# redis.conf
appendonly yes
appendfilename "appendonly.aof"
appendfsync everysec  # 每秒 fsync 一次
auto-aof-rewrite-percentage 100
auto-aof-rewrite-min-size 64mb
```

---

## 19.3 應用數據備份

### 19.3.1 Kubernetes 資源備份 (Velero)

**安裝 Velero:**

```bash
# 安裝 Velero CLI
wget https://github.com/vmware-tanzu/velero/releases/download/v1.12.0/velero-v1.12.0-linux-amd64.tar.gz
tar -xvf velero-v1.12.0-linux-amd64.tar.gz
sudo mv velero-v1.12.0-linux-amd64/velero /usr/local/bin/

# 安裝 Velero 到 Kubernetes
velero install \
    --provider azure \
    --plugins velero/velero-plugin-for-microsoft-azure:v1.8.0 \
    --bucket velero-backups \
    --secret-file ./credentials-velero \
    --backup-location-config \
        resourceGroup=ai-workflow-backups-rg,storageAccount=aiworkflowvelero,subscriptionId=<subscription-id> \
    --snapshot-location-config \
        apiTimeout=5m,resourceGroup=ai-workflow-backups-rg,subscriptionId=<subscription-id>
```

**創建定期備份:**

```yaml
# velero-schedule.yaml
apiVersion: velero.io/v1
kind: Schedule
metadata:
  name: daily-backup
  namespace: velero
spec:
  schedule: "0 2 * * *"  # 每天 02:00
  template:
    includedNamespaces:
    - ai-workflow-prod
    - monitoring

    includedResources:
    - '*'

    excludedResources:
    - events
    - events.events.k8s.io

    storageLocation: default
    volumeSnapshotLocations:
    - default

    ttl: 720h  # 30 days

    hooks:
      resources:
      - name: postgresql-backup-hook
        includedNamespaces:
        - ai-workflow-prod
        labelSelector:
          matchLabels:
            app: postgresql
        pre:
        - exec:
            container: postgresql
            command:
            - /bin/bash
            - -c
            - "psql -U postgres -c 'SELECT pg_start_backup(\"velero-backup\");'"
        post:
        - exec:
            container: postgresql
            command:
            - /bin/bash
            - -c
            - "psql -U postgres -c 'SELECT pg_stop_backup();'"
```

**手動備份命令:**

```bash
# 備份整個 namespace
velero backup create manual-backup-$(date +%Y%m%d) \
    --include-namespaces ai-workflow-prod \
    --wait

# 備份特定資源
velero backup create config-backup-$(date +%Y%m%d) \
    --include-namespaces ai-workflow-prod \
    --include-resources configmap,secret \
    --wait

# 查看備份狀態
velero backup get

# 查看備份詳情
velero backup describe manual-backup-20250115
```

### 19.3.2 Blob Storage 備份

**Azure Blob Storage 內建功能:**

```yaml
blob_storage_protection:
  # 軟刪除 (14 天內可恢復)
  soft_delete:
    enabled: true
    retention_days: 14

  # 版本控制 (保留所有版本)
  versioning:
    enabled: true
    retention: 30 versions per blob

  # 異地冗餘 (自動複製到配對區域)
  geo_redundancy:
    type: GRS
    read_access: true  # RA-GRS
    secondary_region: West Europe

  # 不可變存儲 (合規)
  immutable_storage:
    enabled: false  # 可選，用於法規遵循
    retention_days: 2555  # 7 years
```

---

## 19.4 配置備份

### 19.4.1 Kubernetes ConfigMaps 和 Secrets

**備份腳本:**

```bash
#!/bin/bash
# backup-k8s-configs.sh - Kubernetes 配置備份

set -e

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/k8s-configs"
NAMESPACE="ai-workflow-prod"

echo "=== Kubernetes Configs Backup Started ==="

mkdir -p $BACKUP_DIR

# 備份 ConfigMaps
echo "Backing up ConfigMaps..."
kubectl get configmaps -n $NAMESPACE -o yaml > $BACKUP_DIR/configmaps_${TIMESTAMP}.yaml

# 備份 Secrets
echo "Backing up Secrets..."
kubectl get secrets -n $NAMESPACE -o yaml > $BACKUP_DIR/secrets_${TIMESTAMP}.yaml

# 備份 Deployments
echo "Backing up Deployments..."
kubectl get deployments -n $NAMESPACE -o yaml > $BACKUP_DIR/deployments_${TIMESTAMP}.yaml

# 備份 Services
echo "Backing up Services..."
kubectl get services -n $NAMESPACE -o yaml > $BACKUP_DIR/services_${TIMESTAMP}.yaml

# 備份 Ingress
echo "Backing up Ingress..."
kubectl get ingress -n $NAMESPACE -o yaml > $BACKUP_DIR/ingress_${TIMESTAMP}.yaml

# 備份 StatefulSets
echo "Backing up StatefulSets..."
kubectl get statefulsets -n $NAMESPACE -o yaml > $BACKUP_DIR/statefulsets_${TIMESTAMP}.yaml

# 備份 PVCs
echo "Backing up PersistentVolumeClaims..."
kubectl get pvc -n $NAMESPACE -o yaml > $BACKUP_DIR/pvcs_${TIMESTAMP}.yaml

# 創建壓縮包
tar -czf $BACKUP_DIR/k8s-configs_${TIMESTAMP}.tar.gz -C $BACKUP_DIR \
    configmaps_${TIMESTAMP}.yaml \
    secrets_${TIMESTAMP}.yaml \
    deployments_${TIMESTAMP}.yaml \
    services_${TIMESTAMP}.yaml \
    ingress_${TIMESTAMP}.yaml \
    statefulsets_${TIMESTAMP}.yaml \
    pvcs_${TIMESTAMP}.yaml

# 上傳到 Azure
az storage blob upload \
    --account-name aiworkflowbackup \
    --container-name k8s-configs \
    --name "k8s-configs_${TIMESTAMP}.tar.gz" \
    --file "$BACKUP_DIR/k8s-configs_${TIMESTAMP}.tar.gz"

# 也提交到 Git (去除敏感信息)
echo "Committing to Git..."
git clone https://github.com/yourorg/k8s-configs.git /tmp/k8s-configs-repo
cp $BACKUP_DIR/*.yaml /tmp/k8s-configs-repo/backups/
cd /tmp/k8s-configs-repo
git add .
git commit -m "Backup: $TIMESTAMP"
git push

echo "=== Kubernetes Configs Backup Completed ==="
```

### 19.4.2 Azure Key Vault 備份

```bash
#!/bin/bash
# backup-keyvault.sh - Azure Key Vault 備份

KEYVAULT_NAME="ai-workflow-keyvault"
BACKUP_DIR="/backups/keyvault"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "=== Azure Key Vault Backup Started ==="

mkdir -p $BACKUP_DIR

# 備份所有 Secrets
az keyvault secret list --vault-name $KEYVAULT_NAME --query "[].name" -o tsv | while read secret; do
    echo "Backing up secret: $secret"
    az keyvault secret backup \
        --vault-name $KEYVAULT_NAME \
        --name $secret \
        --file "$BACKUP_DIR/${secret}_${TIMESTAMP}.backup"
done

# 備份所有 Keys
az keyvault key list --vault-name $KEYVAULT_NAME --query "[].name" -o tsv | while read key; do
    echo "Backing up key: $key"
    az keyvault key backup \
        --vault-name $KEYVAULT_NAME \
        --name $key \
        --file "$BACKUP_DIR/${key}_${TIMESTAMP}.backup"
done

# 創建壓縮包
tar -czf $BACKUP_DIR/keyvault_${TIMESTAMP}.tar.gz -C $BACKUP_DIR *.backup

# 上傳到 Azure Blob
az storage blob upload \
    --account-name aiworkflowbackup \
    --container-name keyvault-backups \
    --name "keyvault_${TIMESTAMP}.tar.gz" \
    --file "$BACKUP_DIR/keyvault_${TIMESTAMP}.tar.gz"

echo "=== Azure Key Vault Backup Completed ==="
```

---

## 19.5 恢復流程

### 19.5.1 PostgreSQL 完全恢復

```bash
#!/bin/bash
# restore-postgresql.sh - PostgreSQL 完全恢復

BACKUP_DATE="$1"  # 例如: 20250115

if [ -z "$BACKUP_DATE" ]; then
    echo "Usage: ./restore-postgresql.sh YYYYMMDD"
    exit 1
fi

echo "=== PostgreSQL Full Restore ==="
echo "Backup date: $BACKUP_DATE"

# Step 1: 停止所有應用服務
echo "Step 1: Stopping application services..."
kubectl scale deployment --all --replicas=0 -n ai-workflow-prod

# Step 2: 下載備份
echo "Step 2: Downloading backups from Azure..."
BACKUP_DIR="/tmp/restore-$BACKUP_DATE"
mkdir -p $BACKUP_DIR

DATABASES=(auth_db agent_db persona_db code_db sql_db knowledge_db workflow_db)

for DB in "${DATABASES[@]}"; do
    az storage blob download \
        --account-name aiworkflowbackup \
        --container-name postgresql-backups \
        --name "${DB}_${BACKUP_DATE}_020000.dump.gz" \
        --file "$BACKUP_DIR/${DB}.dump.gz"

    gunzip "$BACKUP_DIR/${DB}.dump.gz"
done

# 下載全局對象備份
az storage blob download \
    --account-name aiworkflowbackup \
    --container-name postgresql-backups \
    --name "globals_${BACKUP_DATE}_020000.sql.gz" \
    --file "$BACKUP_DIR/globals.sql.gz"

gunzip "$BACKUP_DIR/globals.sql.gz"

# Step 3: 恢復全局對象
echo "Step 3: Restoring global objects..."
psql -h postgresql -U postgres -f "$BACKUP_DIR/globals.sql"

# Step 4: 恢復各個數據庫
for DB in "${DATABASES[@]}"; do
    echo "Step 4.$i: Restoring database: $DB"

    # 刪除現有數據庫 (小心!)
    psql -h postgresql -U postgres -c "DROP DATABASE IF EXISTS $DB;"
    psql -h postgresql -U postgres -c "CREATE DATABASE $DB;"

    # 恢復數據
    pg_restore \
        -h postgresql \
        -U postgres \
        -d $DB \
        -v \
        "$BACKUP_DIR/${DB}.dump"

    echo "✓ $DB restored successfully"
done

# Step 5: 驗證數據完整性
echo "Step 5: Verifying data integrity..."
for DB in "${DATABASES[@]}"; do
    COUNT=$(psql -h postgresql -U postgres -d $DB -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public';")
    echo "$DB: $COUNT tables"
done

# Step 6: 重啟應用服務
echo "Step 6: Restarting application services..."
kubectl scale deployment api-gateway --replicas=3 -n ai-workflow-prod
kubectl scale deployment auth-service --replicas=3 -n ai-workflow-prod
kubectl scale deployment agent-service --replicas=5 -n ai-workflow-prod
kubectl scale deployment persona-service --replicas=3 -n ai-workflow-prod
kubectl scale deployment code-execution-service --replicas=5 -n ai-workflow-prod
kubectl scale deployment text-to-sql-service --replicas=3 -n ai-workflow-prod
kubectl scale deployment knowledge-base-service --replicas=3 -n ai-workflow-prod
kubectl scale deployment workflow-service --replicas=5 -n ai-workflow-prod

# 等待 Pods Ready
echo "Waiting for pods to be ready..."
kubectl wait --for=condition=ready pod --all -n ai-workflow-prod --timeout=300s

# Step 7: 健康檢查
echo "Step 7: Running health checks..."
for service in api-gateway auth-service agent-service; do
    response=$(kubectl exec -it $(kubectl get pod -l app=$service -n ai-workflow-prod -o jsonpath='{.items[0].metadata.name}') -n ai-workflow-prod -- curl -s -o /dev/null -w "%{http_code}" http://localhost:8080/health)
    if [ "$response" -eq 200 ]; then
        echo "✓ $service is healthy"
    else
        echo "✗ $service returned HTTP $response"
    fi
done

echo ""
echo "=== PostgreSQL Restore Completed ==="
echo "Restored from backup: $BACKUP_DATE"
echo "Next steps:"
echo "1. Verify application functionality"
echo "2. Check data consistency"
echo "3. Monitor error logs"
```

### 19.5.2 Redis 恢復

```bash
#!/bin/bash
# restore-redis.sh - Redis 恢復

BACKUP_DATE="$1"

if [ -z "$BACKUP_DATE" ]; then
    echo "Usage: ./restore-redis.sh YYYYMMDD_HHMMSS"
    exit 1
fi

echo "=== Redis Restore ==="

# 下載備份
az storage blob download \
    --account-name aiworkflowbackup \
    --container-name redis-backups \
    --name "redis_${BACKUP_DATE}.rdb.gz" \
    --file "/tmp/redis_backup.rdb.gz"

gunzip /tmp/redis_backup.rdb.gz

# 停止 Redis
kubectl exec -it redis-0 -n ai-workflow-prod -- redis-cli -a $REDIS_PASSWORD SHUTDOWN SAVE

# 複製備份文件
kubectl cp /tmp/redis_backup.rdb ai-workflow-prod/redis-0:/data/dump.rdb

# 啟動 Redis
kubectl delete pod redis-0 -n ai-workflow-prod

# 等待 Redis 啟動
kubectl wait --for=condition=ready pod redis-0 -n ai-workflow-prod --timeout=120s

# 驗證數據
KEYS_COUNT=$(kubectl exec -it redis-0 -n ai-workflow-prod -- redis-cli -a $REDIS_PASSWORD DBSIZE)
echo "Redis restored. Total keys: $KEYS_COUNT"

echo "=== Redis Restore Completed ==="
```

### 19.5.3 Kubernetes 資源恢復 (Velero)

```bash
# 列出可用備份
velero backup get

# 恢復整個 namespace
velero restore create --from-backup daily-backup-20250115 --wait

# 恢復特定資源
velero restore create --from-backup daily-backup-20250115 \
    --include-resources deployment,service,configmap \
    --namespace-mappings ai-workflow-prod:ai-workflow-restored \
    --wait

# 查看恢復狀態
velero restore describe <restore-name>

# 查看恢復日誌
velero restore logs <restore-name>
```

---

## 19.6 備份驗證

### 19.6.1 自動驗證測試

```bash
#!/bin/bash
# verify-backup.sh - 備份驗證

BACKUP_DATE="$1"

echo "=== Backup Verification Started ==="

# 1. 驗證備份文件存在
echo "1. Checking backup files existence..."
DATABASES=(auth_db agent_db persona_db code_db sql_db knowledge_db workflow_db)

for DB in "${DATABASES[@]}"; do
    FILE="${DB}_${BACKUP_DATE}_020000.dump.gz"
    if az storage blob exists \
        --account-name aiworkflowbackup \
        --container-name postgresql-backups \
        --name "$FILE" \
        --query "exists" -o tsv | grep -q "true"; then
        echo "✓ $FILE exists"
    else
        echo "✗ $FILE missing"
        exit 1
    fi
done

# 2. 驗證備份完整性
echo "2. Verifying backup integrity..."
for DB in "${DATABASES[@]}"; do
    # 下載備份
    az storage blob download \
        --account-name aiworkflowbackup \
        --container-name postgresql-backups \
        --name "${DB}_${BACKUP_DATE}_020000.dump.gz" \
        --file "/tmp/${DB}.dump.gz"

    # 測試解壓
    if gunzip -t "/tmp/${DB}.dump.gz" 2>/dev/null; then
        echo "✓ $DB backup file is valid"
    else
        echo "✗ $DB backup file is corrupted"
        exit 1
    fi

    rm "/tmp/${DB}.dump.gz"
done

# 3. 測試恢復 (恢復到臨時數據庫)
echo "3. Testing restore to temporary database..."
TEST_DB="test_restore_$BACKUP_DATE"

# 下載 agent_db 備份進行測試
az storage blob download \
    --account-name aiworkflowbackup \
    --container-name postgresql-backups \
    --name "agent_db_${BACKUP_DATE}_020000.dump.gz" \
    --file "/tmp/test.dump.gz"

gunzip "/tmp/test.dump.gz"

# 創建測試數據庫
psql -h postgresql -U postgres -c "DROP DATABASE IF EXISTS $TEST_DB;"
psql -h postgresql -U postgres -c "CREATE DATABASE $TEST_DB;"

# 嘗試恢復
if pg_restore \
    -h postgresql \
    -U postgres \
    -d $TEST_DB \
    -v \
    "/tmp/test.dump" 2>&1 | tee /tmp/restore.log; then
    echo "✓ Test restore successful"

    # 驗證表數量
    TABLE_COUNT=$(psql -h postgresql -U postgres -d $TEST_DB -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public';")
    echo "Restored tables count: $TABLE_COUNT"

    # 清理測試數據庫
    psql -h postgresql -U postgres -c "DROP DATABASE $TEST_DB;"
else
    echo "✗ Test restore failed"
    cat /tmp/restore.log
    exit 1
fi

# 4. 驗證備份大小合理性
echo "4. Verifying backup size..."
for DB in "${DATABASES[@]}"; do
    SIZE=$(az storage blob show \
        --account-name aiworkflowbackup \
        --container-name postgresql-backups \
        --name "${DB}_${BACKUP_DATE}_020000.dump.gz" \
        --query "properties.contentLength" -o tsv)

    SIZE_MB=$((SIZE / 1024 / 1024))

    if [ $SIZE_MB -lt 1 ]; then
        echo "⚠ Warning: $DB backup is suspiciously small ($SIZE_MB MB)"
    else
        echo "✓ $DB backup size: $SIZE_MB MB"
    fi
done

echo ""
echo "=== Backup Verification Completed Successfully ==="
```

### 19.6.2 月度恢復演練

```yaml
monthly_restore_drill:
  frequency: Monthly
  date: "First Saturday of each month"
  duration: 4 hours

  objectives:
    - Verify backup integrity
    - Test restore procedures
    - Measure RTO/RPO
    - Train team members

  procedure:
    1. Select random backup from last month
    2. Create isolated test environment
    3. Restore backup to test environment
    4. Verify data integrity and completeness
    5. Test application functionality
    6. Measure restore time
    7. Document results and issues
    8. Clean up test environment

  success_criteria:
    - Backup restores without errors
    - Data integrity checks pass
    - Applications function correctly
    - RTO within target (< 2 hours)
    - Zero data loss confirmed

  participants:
    - DBA Team (lead)
    - DevOps Team
    - QA Team (verification)

  reporting:
    - Create restore drill report
    - Update runbooks if needed
    - Share learnings with team
    - Track improvements over time
```

---

## 19.7 自動化腳本

### 19.7.1 完整備份自動化

```yaml
# k8s/cronjobs/backup-all.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: backup-all
  namespace: ai-workflow-prod
spec:
  schedule: "0 2 * * *"  # 每天 02:00 UTC
  concurrencyPolicy: Forbid
  successfulJobsHistoryLimit: 7
  failedJobsHistoryLimit: 3

  jobTemplate:
    spec:
      template:
        spec:
          restartPolicy: OnFailure
          serviceAccountName: backup-sa

          containers:
          - name: backup
            image: aiworkflow.azurecr.io/backup-tool:latest
            command:
            - /bin/bash
            - -c
            - |
              set -e

              echo "=== Starting Full Backup ==="

              # 1. PostgreSQL Backup
              echo "Backing up PostgreSQL..."
              /scripts/backup-postgresql.sh

              # 2. Redis Backup
              echo "Backing up Redis..."
              /scripts/backup-redis.sh

              # 3. Kubernetes Configs
              echo "Backing up Kubernetes configs..."
              /scripts/backup-k8s-configs.sh

              # 4. Velero Backup
              echo "Triggering Velero backup..."
              velero backup create auto-backup-$(date +%Y%m%d) \
                --include-namespaces ai-workflow-prod \
                --wait

              # 5. Verify Backups
              echo "Verifying backups..."
              /scripts/verify-backup.sh $(date +%Y%m%d)

              echo "=== Full Backup Completed ==="

            env:
            - name: PGPASSWORD
              valueFrom:
                secretKeyRef:
                  name: postgresql-secrets
                  key: postgres-password

            - name: REDIS_PASSWORD
              valueFrom:
                secretKeyRef:
                  name: redis-secrets
                  key: redis-password

            - name: AZURE_STORAGE_ACCOUNT
              value: aiworkflowbackup

            - name: AZURE_STORAGE_KEY
              valueFrom:
                secretKeyRef:
                  name: azure-storage-secrets
                  key: storage-key

            volumeMounts:
            - name: backup-scripts
              mountPath: /scripts
            - name: backup-storage
              mountPath: /backups

          volumes:
          - name: backup-scripts
            configMap:
              name: backup-scripts
              defaultMode: 0755
          - name: backup-storage
            persistentVolumeClaim:
              claimName: backup-pvc
```

### 19.7.2 備份監控告警

```yaml
# prometheus-backup-rules.yaml
groups:
- name: backup_alerts
  interval: 1h
  rules:

  # 備份失敗告警
  - alert: BackupFailed
    expr: |
      kube_job_status_failed{job_name=~"backup-.*"} > 0
    for: 5m
    labels:
      severity: critical
    annotations:
      summary: "Backup job failed"
      description: "Backup job {{ $labels.job_name }} has failed"
      runbook_url: "https://wiki.aiworkflow.com/runbooks/backup-failed"

  # 備份超時告警
  - alert: BackupTookTooLong
    expr: |
      time() - kube_job_status_start_time{job_name=~"backup-.*"} > 7200
    labels:
      severity: warning
    annotations:
      summary: "Backup taking too long"
      description: "Backup job {{ $labels.job_name }} has been running for more than 2 hours"

  # 備份未執行告警
  - alert: BackupNotRun
    expr: |
      time() - max(kube_job_status_completion_time{job_name=~"backup-.*"}) > 90000
    labels:
      severity: critical
    annotations:
      summary: "Backup has not run in 25 hours"
      description: "No successful backup in the last 25 hours"

  # 備份空間不足告警
  - alert: BackupStorageAlmostFull
    expr: |
      (1 - azure_storage_account_used_capacity / azure_storage_account_capacity) < 0.1
    labels:
      severity: warning
    annotations:
      summary: "Backup storage almost full"
      description: "Backup storage account has less than 10% free space"
```

---

## 總結

本文檔提供了 AI Workflow Platform 的完整備份恢復流程，涵蓋:

1. **備份策略**: 3-2-1 原則、備份頻率、保留期限
2. **數據庫備份**: PostgreSQL (Full + PITR), Redis (RDB + AOF)
3. **應用數據備份**: Velero Kubernetes 備份、Blob Storage 保護
4. **配置備份**: ConfigMaps/Secrets、Azure Key Vault
5. **恢復流程**: PostgreSQL/Redis/Kubernetes 完整恢復腳本
6. **備份驗證**: 自動驗證測試、月度恢復演練
7. **自動化**: CronJob 自動備份、備份監控告警

**關鍵配置:**
- **PostgreSQL**: 每日 Full + 持續 WAL (30 天保留)
- **Redis**: 每 15 分鐘 RDB + 持續 AOF
- **Kubernetes**: 每日 Velero 備份 (30 天保留)
- **異地複製**: Azure GRS (East US ↔ West Europe)

**相關文檔:**
- 18-DISASTER-RECOVERY.md - 災難恢復計劃
- 06-DATABASE-SCHEMA.md - 數據庫架構
- 20-COST-ESTIMATION.md - 備份成本估算

---

**版本歷史:**

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0.0 | 2025-01-15 | AI Workflow Team | 初始版本 |
