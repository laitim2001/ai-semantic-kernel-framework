# Phase 11: 災難恢復計劃 (Disaster Recovery Plan)

## 階段概述

本階段定義 AI Workflow Platform 的完整災難恢復策略,包括備份策略、恢復流程、高可用性設計和業務連續性計劃。

## 目標

- ✅ 實現數據備份和恢復
- ✅ 確保高可用性 (99.9% uptime)
- ✅ 建立災難恢復流程
- ✅ 最小化 RTO 和 RPO
- ✅ 定期災難恢復演練

## 文檔結構

### 1. 備份策略
**文件**: `backup-strategy.md`

**內容**:
- 數據庫備份 (Full, Differential, Transaction Log)
- 文件存儲備份
- 配置備份
- 備份頻率和保留策略
- 備份加密
- 異地備份 (Geo-Redundant)
- 備份驗證

**技術重點**:
- PostgreSQL pg_dump / pg_basebackup
- Azure Backup
- Automated Backup Scripts
- Backup Verification Tests

### 2. 恢復流程
**文件**: `recovery-procedures.md`

**內容**:
- 數據庫恢復步驟
- Point-in-Time Recovery (PITR)
- 應用程序恢復
- 配置恢復
- 恢復時間測試
- 恢復優先級
- 恢復驗證檢查清單

**技術重點**:
- PostgreSQL WAL (Write-Ahead Log)
- Azure Site Recovery
- Kubernetes StatefulSet Recovery

### 3. 高可用性設計
**文件**: `high-availability-design.md`

**內容**:
- 多區域部署
- 數據庫複製 (Primary-Replica)
- 負載均衡
- 自動故障轉移
- 會話持久化
- 無狀態應用設計
- 健康檢查和自愈

**技術重點**:
- Azure Availability Zones
- PostgreSQL Replication
- Redis Sentinel
- Kubernetes Replica Sets

### 4. 業務連續性計劃
**文件**: `business-continuity-plan.md`

**內容**:
- RTO (Recovery Time Objective) 定義
- RPO (Recovery Point Objective) 定義
- 災難場景分析
- 應急響應流程
- 溝通計劃
- 災難恢復演練
- 事後分析 (Post-Mortem)

**技術重點**:
- RTO: < 4 hours
- RPO: < 15 minutes
- Incident Response

## RTO & RPO 目標

### 服務等級目標

| 服務類型 | RTO (恢復時間) | RPO (數據丟失) | 可用性目標 |
|---------|--------------|--------------|-----------|
| 核心 API | 1 小時 | 5 分鐘 | 99.95% |
| 數據庫 | 2 小時 | 15 分鐘 | 99.9% |
| 前端應用 | 30 分鐘 | N/A | 99.9% |
| 文件存儲 | 4 小時 | 1 小時 | 99.5% |
| 監控系統 | 1 小時 | 30 分鐘 | 99% |

## 備份架構

```
┌─────────────────────────────────────────────────┐
│           Production Environment                 │
│                                                  │
│  ┌──────────────┐         ┌──────────────┐     │
│  │  PostgreSQL  │────────>│  PostgreSQL  │     │
│  │   Primary    │ Replica │   Standby    │     │
│  └──────┬───────┘         └──────────────┘     │
│         │                                        │
│         │ Continuous Backup (WAL Archiving)     │
│         │                                        │
│         ▼                                        │
│  ┌──────────────────────────────────┐           │
│  │    Azure Blob Storage            │           │
│  │  (Geo-Redundant Backup)          │           │
│  │                                  │           │
│  │  - Full Backup: Daily            │           │
│  │  - Transaction Logs: 5 min       │           │
│  │  - Retention: 30 days            │           │
│  └──────────────────────────────────┘           │
└─────────────────────────────────────────────────┘
```

## 數據庫備份

### PostgreSQL 自動化備份腳本

```bash
#!/bin/bash
# backup_postgres.sh

set -e

# 配置
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-aiworkflow}"
DB_USER="${DB_USER:-postgres}"
BACKUP_DIR="/backups/postgres"
AZURE_STORAGE_ACCOUNT="aiworkflowbackup"
AZURE_CONTAINER="database-backups"
RETENTION_DAYS=30

# 創建備份目錄
mkdir -p "$BACKUP_DIR"

# 備份文件名
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/${DB_NAME}_${DATE}.dump"

echo "[$(date)] Starting PostgreSQL backup..."

# Full Backup (Custom Format)
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
BACKUP_FILE_GZ="${BACKUP_FILE}.gz"

echo "[$(date)] Backup created: $BACKUP_FILE_GZ"

# 上傳到 Azure Blob Storage
az storage blob upload \
    --account-name "$AZURE_STORAGE_ACCOUNT" \
    --container-name "$AZURE_CONTAINER" \
    --name "$(basename $BACKUP_FILE_GZ)" \
    --file "$BACKUP_FILE_GZ"

echo "[$(date)] Backup uploaded to Azure Blob Storage"

# 清理本地舊備份
find "$BACKUP_DIR" -name "*.dump.gz" -mtime +7 -delete

# 清理 Azure 舊備份
CUTOFF_DATE=$(date -d "$RETENTION_DAYS days ago" +%Y-%m-%d)
az storage blob list \
    --account-name "$AZURE_STORAGE_ACCOUNT" \
    --container-name "$AZURE_CONTAINER" \
    --query "[?properties.creationTime<'$CUTOFF_DATE'].name" \
    -o tsv | xargs -I {} az storage blob delete \
        --account-name "$AZURE_STORAGE_ACCOUNT" \
        --container-name "$AZURE_CONTAINER" \
        --name {}

echo "[$(date)] Backup completed successfully"
```

### WAL Archiving Configuration

```ini
# postgresql.conf
wal_level = replica
archive_mode = on
archive_command = 'az storage blob upload --account-name aiworkflowbackup --container-name wal-archive --name %f --file %p'
archive_timeout = 300  # 5 分鐘強制歸檔
```

## 恢復流程

### Point-in-Time Recovery (PITR)

```bash
#!/bin/bash
# restore_postgres_pitr.sh

set -e

BACKUP_FILE=$1
RECOVERY_TARGET_TIME=$2  # 格式: 2025-01-15 14:30:00

if [ -z "$BACKUP_FILE" ] || [ -z "$RECOVERY_TARGET_TIME" ]; then
    echo "Usage: $0 <backup_file> <recovery_target_time>"
    exit 1
fi

echo "[$(date)] Starting Point-in-Time Recovery..."
echo "Target Time: $RECOVERY_TARGET_TIME"

# 1. 停止 PostgreSQL
sudo systemctl stop postgresql

# 2. 清空數據目錄
sudo rm -rf /var/lib/postgresql/14/main/*

# 3. 恢復 Base Backup
sudo -u postgres pg_restore \
    -d postgres \
    -C \
    --clean \
    --if-exists \
    "$BACKUP_FILE"

# 4. 配置恢復參數
cat > /var/lib/postgresql/14/main/recovery.conf <<EOF
restore_command = 'az storage blob download --account-name aiworkflowbackup --container-name wal-archive --name %f --file %p'
recovery_target_time = '$RECOVERY_TARGET_TIME'
recovery_target_action = 'promote'
EOF

# 5. 啟動 PostgreSQL (進入恢復模式)
sudo systemctl start postgresql

echo "[$(date)] Recovery initiated. Monitoring progress..."

# 6. 等待恢復完成
while sudo -u postgres psql -c "SELECT pg_is_in_recovery();" | grep -q "t"; do
    echo "[$(date)] Still in recovery mode..."
    sleep 10
done

echo "[$(date)] Recovery completed successfully!"
echo "Database has been promoted to primary."

# 7. 驗證數據
sudo -u postgres psql -d aiworkflow -c "SELECT COUNT(*) FROM workflow.workflows;"
```

## 高可用性配置

### PostgreSQL Streaming Replication

**Primary 配置**:

```ini
# postgresql.conf (Primary)
listen_addresses = '*'
wal_level = replica
max_wal_senders = 10
max_replication_slots = 10
synchronous_commit = on
synchronous_standby_names = 'standby1'
```

```conf
# pg_hba.conf (Primary)
host replication replicator 10.0.0.0/8 md5
```

**Standby 配置**:

```ini
# postgresql.conf (Standby)
hot_standby = on
```

```conf
# standby.signal (Standby)
primary_conninfo = 'host=primary-db.example.com port=5432 user=replicator password=secret application_name=standby1'
primary_slot_name = 'standby1_slot'
```

### Kubernetes StatefulSet (High Availability)

```yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: postgres
  namespace: aiworkflow
spec:
  serviceName: postgres
  replicas: 3  # 1 Primary + 2 Replicas
  selector:
    matchLabels:
      app: postgres
  template:
    metadata:
      labels:
        app: postgres
    spec:
      containers:
      - name: postgres
        image: postgres:16
        ports:
        - containerPort: 5432
        env:
        - name: POSTGRES_DB
          value: aiworkflow
        - name: POSTGRES_USER
          valueFrom:
            secretKeyRef:
              name: postgres-secret
              key: username
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: postgres-secret
              key: password
        volumeMounts:
        - name: postgres-storage
          mountPath: /var/lib/postgresql/data
        livenessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 5
          periodSeconds: 5
  volumeClaimTemplates:
  - metadata:
      name: postgres-storage
    spec:
      accessModes: ["ReadWriteOnce"]
      storageClassName: "premium-rwo"
      resources:
        requests:
          storage: 100Gi
```

## 災難恢復演練

### 演練計劃模板

```yaml
disaster_recovery_drill:
  name: "Q1 2025 DR Drill"
  date: "2025-01-15"
  duration: "4 hours"
  participants:
    - DevOps Team
    - Backend Team
    - DBA Team
    - Management

  scenarios:
    - name: "Database Corruption"
      steps:
        - "1. Simulate database corruption"
        - "2. Detect corruption via monitoring"
        - "3. Initiate recovery procedure"
        - "4. Restore from latest backup"
        - "5. Apply WAL to target time"
        - "6. Validate data integrity"
        - "7. Promote to primary"
        - "8. Resume application traffic"
      expected_rto: "2 hours"
      expected_rpo: "15 minutes"

    - name: "Data Center Outage"
      steps:
        - "1. Simulate AZ failure"
        - "2. Automatic failover to standby region"
        - "3. DNS切換到 DR 環境"
        - "4. Validate service availability"
        - "5. Monitor application health"
      expected_rto: "30 minutes"
      expected_rpo: "5 minutes"

  success_criteria:
    - "RTO < Target"
    - "RPO < Target"
    - "Data integrity validated"
    - "All services operational"
    - "Documentation updated"

  post_drill:
    - "Document actual RTO/RPO"
    - "Identify improvement areas"
    - "Update runbooks"
    - "Schedule next drill"
```

## 災難恢復檢查清單

### 備份驗證
- [ ] 每日備份成功完成
- [ ] 備份文件完整性驗證
- [ ] 備份已上傳到異地存儲
- [ ] WAL 歸檔正常運行
- [ ] 備份恢復測試 (每月)

### 高可用性
- [ ] PostgreSQL 複製同步正常
- [ ] Redis Sentinel 運行正常
- [ ] Kubernetes 多副本部署
- [ ] 健康檢查配置正確
- [ ] 自動故障轉移測試

### 災難恢復準備
- [ ] DR 文檔完整且最新
- [ ] 恢復腳本已測試
- [ ] 團隊成員熟悉 DR 流程
- [ ] 定期 DR 演練 (每季度)
- [ ] 監控和告警配置

### 事件響應
- [ ] 事件響應計劃已建立
- [ ] 溝通渠道已設置
- [ ] On-Call 輪值已配置
- [ ] Runbook 已更新
- [ ] Post-Mortem 流程已定義

## 相關文檔

- [Phase 6: 數據庫標準](../6-database-standards/README.md)
- [Phase 8: 部署架構](../8-deployment-architecture/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [PostgreSQL Backup and Restore](https://www.postgresql.org/docs/current/backup.html)
- [PostgreSQL High Availability](https://www.postgresql.org/docs/current/high-availability.html)
- [Azure Backup](https://learn.microsoft.com/en-us/azure/backup/)

### 最佳實踐
- [Disaster Recovery Best Practices](https://cloud.google.com/architecture/dr-scenarios-planning-guide)
- [Database HA Patterns](https://aws.amazon.com/rds/ha/)

---

**文檔維護**: SRE Team / DBA Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
