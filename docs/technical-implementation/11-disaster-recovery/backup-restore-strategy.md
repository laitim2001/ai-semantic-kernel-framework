# 備份與恢復策略設計

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整備份與恢復策略設計,涵蓋:
- **RPO/RTO 定義** (Recovery Point Objective / Recovery Time Objective)
- **多層備份架構** (數據庫、應用配置、Kubernetes 狀態)
- **自動化備份流程** (PostgreSQL, Redis, MongoDB, Blob Storage)
- **備份驗證與測試** (完整性檢查、恢復演練)
- **加密與安全** (備份數據加密、訪問控制)
- **成本優化** (存儲分層、保留策略)

### 業務目標
確保系統在災難場景下能夠快速恢復,最小化數據丟失和業務中斷:
- **RPO ≤ 15 分鐘**: 最多丟失 15 分鐘內的數據
- **RTO ≤ 2 小時**: 在 2 小時內完成完整恢復
- **99.9% 可用性**: 年度計劃外停機時間 < 8.76 小時
- **數據完整性**: 100% 備份驗證通過率

**災難場景覆蓋**:
- 區域性故障 (Azure Region Outage)
- 數據損壞 (Database Corruption, Accidental Deletion)
- 安全事件 (Ransomware, Unauthorized Access)
- 人為錯誤 (Misconfiguration, Deployment Failure)

---

## RPO/RTO 定義

### 服務等級目標

| 數據類型 | RPO | RTO | 備份頻率 | 保留期 | 優先級 |
|---------|-----|-----|----------|--------|--------|
| **生產數據庫 (PostgreSQL)** | 15 分鐘 | 2 小時 | 連續 WAL + 每小時增量 | 30 天 + 90 天歸檔 | 🔴 Critical |
| **用戶工作流配置** | 15 分鐘 | 2 小時 | 連續 WAL | 30 天 + 90 天歸檔 | 🔴 Critical |
| **Redis 緩存數據** | 1 小時 | 30 分鐘 | 每小時 RDB + AOF | 7 天 | 🟡 High |
| **MongoDB 日誌** | 1 天 | 4 小時 | 每日全量 | 90 天 | 🟢 Medium |
| **Blob Storage (檔案)** | 1 小時 | 1 小時 | Azure Geo-Redundant | 永久 + Soft Delete 30 天 | 🔴 Critical |
| **Kubernetes etcd** | 12 小時 | 4 小時 | 每 6 小時 | 7 天 | 🟡 High |
| **應用配置 (Azure App Config)** | 1 天 | 1 小時 | 每日快照 | 30 天 | 🟢 Medium |

### RPO/RTO 計算邏輯

**RPO (Recovery Point Objective) - 可接受的數據丟失時間**:
```
RPO = 上次備份時間點 → 災難發生時間點
示例: 若 WAL 連續備份 + 每小時增量備份,最大數據丟失 = 15 分鐘
```

**RTO (Recovery Time Objective) - 可接受的業務中斷時間**:
```
RTO = 災難發現時間 + 決策時間 + 恢復執行時間 + 驗證時間
示例: 發現 (15min) + 決策 (15min) + 恢復 (1.5h) + 驗證 (15min) = 2h
```

---

## 備份架構設計

### 整體備份拓撲

```
┌─────────────────────────────────────────────────────────────────────┐
│ Production Environment (Primary Region: East US)                    │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌───────────────────┐  ┌───────────────────┐  ┌─────────────────┐ │
│  │ PostgreSQL 16     │  │ Redis 7           │  │ MongoDB 7       │ │
│  │ (Primary DB)      │  │ (Cache)           │  │ (Logs)          │ │
│  └─────────┬─────────┘  └─────────┬─────────┘  └────────┬────────┘ │
│            │ WAL                  │ RDB/AOF             │ Dump      │
│            ▼                      ▼                     ▼           │
│  ┌────────────────────────────────────────────────────────────────┐ │
│  │ Backup Orchestrator (Kubernetes CronJob)                       │ │
│  │ ├─ PostgreSQL Backup Job (Every Hour)                         │ │
│  │ ├─ Redis Backup Job (Every Hour)                              │ │
│  │ ├─ MongoDB Backup Job (Daily)                                 │ │
│  │ ├─ Kubernetes etcd Backup (Every 6 Hours)                     │ │
│  │ └─ Configuration Backup (Daily)                               │ │
│  └───────────────────────────┬────────────────────────────────────┘ │
│                              │                                       │
└──────────────────────────────┼───────────────────────────────────────┘
                               │ Upload
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│ Azure Backup Storage (Geo-Redundant)                               │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Blob Storage (Hot Tier) - 最近 7 天                          │  │
│  │ ├─ postgresql/hourly/YYYY-MM-DD-HH/                          │  │
│  │ ├─ redis/hourly/YYYY-MM-DD-HH/                               │  │
│  │ └─ mongodb/daily/YYYY-MM-DD/                                 │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Blob Storage (Cool Tier) - 8-30 天                           │  │
│  │ └─ Lifecycle Policy: Hot → Cool (7 days)                     │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Blob Storage (Archive Tier) - 31-90 天                       │  │
│  │ └─ Lifecycle Policy: Cool → Archive (30 days)                │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
                               │ Replication
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│ Disaster Recovery Region (West US 2)                               │
├─────────────────────────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Geo-Replicated Blob Storage (Read-Only)                      │  │
│  │ └─ Automatic Replication within 15 minutes                   │  │
│  └──────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## PostgreSQL 備份策略

### WAL 連續歸檔 (Continuous Archiving)

**目的**: 實現 RPO ≤ 15 分鐘的連續備份

#### 配置 WAL 歸檔

```bash
# PostgreSQL 配置: /var/lib/postgresql/data/postgresql.conf
wal_level = replica                  # 啟用 WAL 日誌記錄
archive_mode = on                    # 啟用歸檔模式
archive_command = '/usr/local/bin/pg_wal_archive.sh %p %f'  # 自定義歸檔腳本
archive_timeout = 900                # 15 分鐘強制切換 WAL 文件

max_wal_senders = 5                  # 最大 WAL 發送進程
wal_keep_size = 1GB                  # 保留 1GB WAL 文件
```

#### WAL 歸檔腳本

```bash
#!/bin/bash
# /usr/local/bin/pg_wal_archive.sh
# 參數: $1 = WAL 文件路徑, $2 = WAL 文件名

WAL_PATH=$1
WAL_FILE=$2
BACKUP_CONTAINER="postgresql-wal-archive"
STORAGE_ACCOUNT="aiagentbackup"

# 上傳到 Azure Blob Storage
az storage blob upload \
  --account-name "$STORAGE_ACCOUNT" \
  --container-name "$BACKUP_CONTAINER" \
  --name "wal/$WAL_FILE" \
  --file "$WAL_PATH" \
  --auth-mode login \
  --tier Hot \
  --metadata "timestamp=$(date -u +%Y-%m-%dT%H:%M:%SZ)" \
  --metadata "database=aiagent-prod" \
  || exit 1

# 日誌記錄
logger -t pg_wal_archive "Successfully archived WAL file: $WAL_FILE"
exit 0
```

### 增量備份 (Incremental Backup)

**目的**: 每小時執行增量備份,減少存儲成本

#### Kubernetes CronJob 配置

```yaml
# k8s/backup/postgresql-backup-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: postgresql-incremental-backup
  namespace: aiagent-prod
spec:
  # 每小時執行一次
  schedule: "0 * * * *"
  successfulJobsHistoryLimit: 3
  failedJobsHistoryLimit: 1
  jobTemplate:
    spec:
      template:
        spec:
          serviceAccountName: backup-service-account
          containers:
          - name: pg-backup
            image: postgres:16
            env:
            - name: PGHOST
              value: "postgresql-primary.aiagent-prod.svc.cluster.local"
            - name: PGPORT
              value: "5432"
            - name: PGDATABASE
              value: "aiagent_prod"
            - name: PGUSER
              valueFrom:
                secretKeyRef:
                  name: postgresql-credentials
                  key: username
            - name: PGPASSWORD
              valueFrom:
                secretKeyRef:
                  name: postgresql-credentials
                  key: password
            - name: AZURE_STORAGE_ACCOUNT
              value: "aiagentbackup"
            - name: AZURE_STORAGE_SAS_TOKEN
              valueFrom:
                secretKeyRef:
                  name: backup-storage-credentials
                  key: sas-token
            command:
            - /bin/bash
            - -c
            - |
              set -e
              TIMESTAMP=$(date -u +%Y-%m-%d-%H)
              BACKUP_FILE="postgresql-incremental-$TIMESTAMP.dump"

              # 執行 pg_dump (自定義格式, 支持並行恢復)
              pg_dump \
                --format=custom \
                --compress=9 \
                --verbose \
                --file=/tmp/$BACKUP_FILE

              # 上傳到 Azure Blob Storage
              az storage blob upload \
                --account-name "$AZURE_STORAGE_ACCOUNT" \
                --container-name "postgresql-incremental" \
                --name "hourly/$TIMESTAMP/$BACKUP_FILE" \
                --file "/tmp/$BACKUP_FILE" \
                --sas-token "$AZURE_STORAGE_SAS_TOKEN" \
                --tier Hot \
                --metadata "backup_type=incremental" \
                --metadata "timestamp=$TIMESTAMP" \
                --metadata "database=aiagent_prod"

              # 驗證備份完整性
              pg_restore --list /tmp/$BACKUP_FILE > /dev/null

              echo "Incremental backup completed successfully: $BACKUP_FILE"
          restartPolicy: OnFailure
```

### 全量備份 (Full Backup)

**目的**: 每日執行全量備份,作為基線恢復點

```yaml
# k8s/backup/postgresql-full-backup-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: postgresql-full-backup
  namespace: aiagent-prod
spec:
  # 每日凌晨 2:00 執行
  schedule: "0 2 * * *"
  successfulJobsHistoryLimit: 7
  failedJobsHistoryLimit: 3
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: pg-full-backup
            image: postgres:16
            command:
            - /bin/bash
            - -c
            - |
              set -e
              TIMESTAMP=$(date -u +%Y-%m-%d)
              BACKUP_FILE="postgresql-full-$TIMESTAMP.dump"

              # 全量備份 (包含所有數據庫對象)
              pg_dumpall \
                --clean \
                --if-exists \
                --verbose \
                --file=/tmp/$BACKUP_FILE

              # 壓縮備份
              gzip -9 /tmp/$BACKUP_FILE

              # 上傳到 Azure Blob Storage
              az storage blob upload \
                --account-name "$AZURE_STORAGE_ACCOUNT" \
                --container-name "postgresql-full" \
                --name "daily/$TIMESTAMP/$BACKUP_FILE.gz" \
                --file "/tmp/$BACKUP_FILE.gz" \
                --sas-token "$AZURE_STORAGE_SAS_TOKEN" \
                --tier Hot

              echo "Full backup completed: $BACKUP_FILE.gz"
          restartPolicy: OnFailure
```

---

## Redis 備份策略

### RDB 快照備份

**配置 Redis 持久化**:

```conf
# redis.conf
save 900 1       # 900 秒內至少 1 次寫入,觸發保存
save 300 10      # 300 秒內至少 10 次寫入
save 60 10000    # 60 秒內至少 10000 次寫入

dbfilename dump.rdb
dir /data
rdbcompression yes
rdbchecksum yes
```

**自動備份 CronJob**:

```yaml
# k8s/backup/redis-backup-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: redis-backup
  namespace: aiagent-prod
spec:
  schedule: "0 * * * *"  # 每小時
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: redis-backup
            image: redis:7
            command:
            - /bin/bash
            - -c
            - |
              set -e
              TIMESTAMP=$(date -u +%Y-%m-%d-%H)

              # 觸發 BGSAVE (後台保存)
              redis-cli -h redis-master.aiagent-prod.svc.cluster.local BGSAVE

              # 等待 BGSAVE 完成
              while [ $(redis-cli -h redis-master.aiagent-prod.svc.cluster.local LASTSAVE) -eq $LAST_SAVE ]; do
                sleep 5
              done

              # 複製 RDB 文件
              kubectl cp redis-master-0:/data/dump.rdb /tmp/redis-$TIMESTAMP.rdb

              # 上傳到 Blob Storage
              az storage blob upload \
                --account-name "$AZURE_STORAGE_ACCOUNT" \
                --container-name "redis-backup" \
                --name "hourly/$TIMESTAMP/dump.rdb" \
                --file "/tmp/redis-$TIMESTAMP.rdb"
          restartPolicy: OnFailure
```

### AOF 持久化備份

**配置 AOF (Append-Only File)**:

```conf
# redis.conf
appendonly yes
appendfilename "appendonly.aof"
appendfsync everysec   # 每秒同步一次 (平衡性能與持久性)

# AOF 重寫配置
auto-aof-rewrite-percentage 100
auto-aof-rewrite-min-size 64mb
```

---

## MongoDB 日誌備份策略

### 每日全量備份

```yaml
# k8s/backup/mongodb-backup-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: mongodb-backup
  namespace: aiagent-prod
spec:
  schedule: "0 3 * * *"  # 每日凌晨 3:00
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: mongodb-backup
            image: mongo:7
            command:
            - /bin/bash
            - -c
            - |
              set -e
              TIMESTAMP=$(date -u +%Y-%m-%d)
              BACKUP_FILE="mongodb-logs-$TIMESTAMP"

              # 執行 mongodump
              mongodump \
                --host=mongodb.aiagent-prod.svc.cluster.local \
                --port=27017 \
                --username=$MONGO_USER \
                --password=$MONGO_PASSWORD \
                --authenticationDatabase=admin \
                --db=aiagent_logs \
                --gzip \
                --archive=/tmp/$BACKUP_FILE.archive

              # 上傳到 Blob Storage
              az storage blob upload \
                --account-name "$AZURE_STORAGE_ACCOUNT" \
                --container-name "mongodb-backup" \
                --name "daily/$TIMESTAMP/$BACKUP_FILE.archive" \
                --file "/tmp/$BACKUP_FILE.archive" \
                --tier Cool  # 日誌備份使用 Cool Tier
          restartPolicy: OnFailure
```

---

## Kubernetes etcd 備份

### 定期 etcd 快照

```yaml
# k8s/backup/etcd-backup-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: etcd-backup
  namespace: kube-system
spec:
  schedule: "0 */6 * * *"  # 每 6 小時
  jobTemplate:
    spec:
      template:
        spec:
          hostNetwork: true
          containers:
          - name: etcd-backup
            image: k8s.gcr.io/etcd:3.5.10
            env:
            - name: ETCDCTL_API
              value: "3"
            command:
            - /bin/sh
            - -c
            - |
              TIMESTAMP=$(date -u +%Y-%m-%d-%H)
              SNAPSHOT_FILE="/backup/etcd-snapshot-$TIMESTAMP.db"

              # 創建 etcd 快照
              etcdctl snapshot save $SNAPSHOT_FILE \
                --endpoints=https://127.0.0.1:2379 \
                --cacert=/etc/kubernetes/pki/etcd/ca.crt \
                --cert=/etc/kubernetes/pki/etcd/server.crt \
                --key=/etc/kubernetes/pki/etcd/server.key

              # 驗證快照
              etcdctl snapshot status $SNAPSHOT_FILE

              # 上傳到 Blob Storage
              az storage blob upload \
                --account-name "$AZURE_STORAGE_ACCOUNT" \
                --container-name "etcd-backup" \
                --name "snapshots/$TIMESTAMP/etcd-snapshot.db" \
                --file "$SNAPSHOT_FILE"
            volumeMounts:
            - name: etcd-certs
              mountPath: /etc/kubernetes/pki/etcd
              readOnly: true
            - name: backup
              mountPath: /backup
          volumes:
          - name: etcd-certs
            hostPath:
              path: /etc/kubernetes/pki/etcd
          - name: backup
            emptyDir: {}
          restartPolicy: OnFailure
```

---

## Azure Blob Storage 備份

### Geo-Redundant Storage (GRS) 配置

```bash
# 創建 GRS 存儲賬戶
az storage account create \
  --name aiagentbackup \
  --resource-group aiagent-prod-rg \
  --location eastus \
  --sku Standard_GRS \
  --kind StorageV2 \
  --access-tier Hot \
  --enable-hierarchical-namespace true \
  --min-tls-version TLS1_2

# 啟用 Soft Delete (防止意外刪除)
az storage blob service-properties delete-policy update \
  --account-name aiagentbackup \
  --enable true \
  --days-retained 30

# 啟用 Versioning (版本控制)
az storage account blob-service-properties update \
  --account-name aiagentbackup \
  --enable-versioning true

# 啟用 Point-in-Time Restore
az storage account blob-service-properties update \
  --account-name aiagentbackup \
  --enable-restore-policy true \
  --restore-days 7
```

### Lifecycle Management Policy

```json
{
  "rules": [
    {
      "enabled": true,
      "name": "MoveToCoolAfter7Days",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["postgresql-incremental/", "redis-backup/"]
        },
        "actions": {
          "baseBlob": {
            "tierToCool": {
              "daysAfterModificationGreaterThan": 7
            },
            "tierToArchive": {
              "daysAfterModificationGreaterThan": 30
            },
            "delete": {
              "daysAfterModificationGreaterThan": 90
            }
          }
        }
      }
    },
    {
      "enabled": true,
      "name": "DeleteOldLogs",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["mongodb-backup/"]
        },
        "actions": {
          "baseBlob": {
            "delete": {
              "daysAfterModificationGreaterThan": 90
            }
          }
        }
      }
    }
  ]
}
```

---

## 備份驗證與測試

### 自動化備份驗證

```csharp
// src/Infrastructure/BackupValidation/BackupValidator.cs
using Azure.Storage.Blobs;
using Npgsql;

public sealed class BackupValidator
{
    private readonly BlobServiceClient _blobClient;
    private readonly ILogger<BackupValidator> _logger;

    public async Task<BackupValidationResult> ValidatePostgreSQLBackupAsync(
        string backupBlobName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. 下載備份文件
            var containerClient = _blobClient.GetBlobContainerClient("postgresql-incremental");
            var blobClient = containerClient.GetBlobClient(backupBlobName);

            var tempFile = Path.GetTempFileName();
            await blobClient.DownloadToAsync(tempFile, cancellationToken);

            // 2. 驗證備份完整性 (pg_restore --list)
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "pg_restore",
                Arguments = $"--list {tempFile}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync(cancellationToken);
                return BackupValidationResult.Failed($"Backup corrupted: {error}");
            }

            // 3. 檢查備份大小 (不應小於最小閾值)
            var fileInfo = new FileInfo(tempFile);
            const long MinBackupSizeMB = 100; // 100 MB

            if (fileInfo.Length < MinBackupSizeMB * 1024 * 1024)
            {
                return BackupValidationResult.Failed(
                    $"Backup size too small: {fileInfo.Length / 1024 / 1024} MB (expected > {MinBackupSizeMB} MB)");
            }

            // 4. 記錄驗證成功
            _logger.LogInformation(
                "Backup validation passed: {BackupName}, Size: {SizeMB} MB",
                backupBlobName,
                fileInfo.Length / 1024 / 1024);

            File.Delete(tempFile);
            return BackupValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup validation failed for {BackupName}", backupBlobName);
            return BackupValidationResult.Failed(ex.Message);
        }
    }
}

public record BackupValidationResult(bool IsValid, string? ErrorMessage = null)
{
    public static BackupValidationResult Success() => new(true);
    public static BackupValidationResult Failed(string error) => new(false, error);
}
```

### 定期恢復演練

```yaml
# k8s/backup/restore-drill-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: backup-restore-drill
  namespace: aiagent-test
spec:
  schedule: "0 4 * * 0"  # 每週日凌晨 4:00
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: restore-drill
            image: postgres:16
            command:
            - /bin/bash
            - -c
            - |
              set -e

              # 1. 下載最新備份
              LATEST_BACKUP=$(az storage blob list \
                --account-name aiagentbackup \
                --container-name postgresql-full \
                --prefix daily/ \
                --query "sort_by([].{name:name, lastModified:properties.lastModified}, &lastModified)[-1].name" \
                --output tsv)

              az storage blob download \
                --account-name aiagentbackup \
                --container-name postgresql-full \
                --name "$LATEST_BACKUP" \
                --file /tmp/backup.dump.gz

              # 2. 解壓縮
              gunzip /tmp/backup.dump.gz

              # 3. 恢復到測試數據庫
              psql -h postgresql-test.aiagent-test.svc.cluster.local \
                -U postgres \
                -d aiagent_test \
                -f /tmp/backup.dump

              # 4. 驗證恢復結果
              RECORD_COUNT=$(psql -h postgresql-test.aiagent-test.svc.cluster.local \
                -U postgres \
                -d aiagent_test \
                -t -c "SELECT COUNT(*) FROM workflows;")

              if [ "$RECORD_COUNT" -lt 100 ]; then
                echo "❌ Restore drill failed: Too few records ($RECORD_COUNT)"
                exit 1
              fi

              echo "✅ Restore drill passed: $RECORD_COUNT workflows restored"
          restartPolicy: OnFailure
```

---

## 備份加密與安全

### 數據加密

**傳輸中加密** (Encryption in Transit):
- 所有備份上傳使用 HTTPS (TLS 1.3)
- Azure Storage SDK 強制使用加密連接

**靜態加密** (Encryption at Rest):
```bash
# Azure Blob Storage 默認啟用 256-bit AES 加密
az storage account update \
  --name aiagentbackup \
  --resource-group aiagent-prod-rg \
  --encryption-services blob \
  --encryption-key-source Microsoft.Storage
```

### 訪問控制

**使用 Managed Identity 進行身份驗證**:

```yaml
# k8s/backup/backup-service-account.yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: backup-service-account
  namespace: aiagent-prod
  annotations:
    azure.workload.identity/client-id: "12345678-1234-1234-1234-123456789012"
---
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: backup-role
  namespace: aiagent-prod
rules:
- apiGroups: [""]
  resources: ["pods", "pods/log"]
  verbs: ["get", "list"]
- apiGroups: [""]
  resources: ["pods/exec"]
  verbs: ["create"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: backup-role-binding
  namespace: aiagent-prod
subjects:
- kind: ServiceAccount
  name: backup-service-account
roleRef:
  kind: Role
  name: backup-role
  apiGroup: rbac.authorization.k8s.io
```

**Azure RBAC 權限配置**:

```bash
# 授予 Managed Identity 對 Blob Storage 的寫入權限
az role assignment create \
  --assignee "12345678-1234-1234-1234-123456789012" \
  --role "Storage Blob Data Contributor" \
  --scope "/subscriptions/{subscription-id}/resourceGroups/aiagent-prod-rg/providers/Microsoft.Storage/storageAccounts/aiagentbackup"
```

---

## 成本優化策略

### 存儲分層成本比較

| 存儲層級 | 存儲成本 ($/GB/月) | 訪問成本 | 使用場景 |
|---------|-------------------|---------|----------|
| **Hot Tier** | $0.0184 | 最低 | 最近 7 天備份 (頻繁訪問) |
| **Cool Tier** | $0.01 | 中等 | 8-30 天備份 (偶爾訪問) |
| **Archive Tier** | $0.002 | 最高 | 31-90 天備份 (極少訪問,需提前 15 小時解凍) |

### 成本計算示例

**假設每日備份大小**:
- PostgreSQL 全量備份: 50 GB (壓縮後)
- PostgreSQL 增量備份: 5 GB/小時 × 24 = 120 GB/天
- Redis 備份: 2 GB/小時 × 24 = 48 GB/天
- MongoDB 備份: 10 GB/天
- **總計**: 228 GB/天

**月度存儲成本** (30 天):
```
Hot Tier (0-7 days):   228 GB/天 × 7 天 × $0.0184 = $29.37
Cool Tier (8-30 days): 228 GB/天 × 23 天 × $0.01 = $52.44
Archive Tier (31-90天): 228 GB/天 × 60 天 × $0.002 = $27.36

總月度成本: $109.17
年度成本: $1,310
```

### 優化建議

1. **增量備份替代全量備份**: 降低存儲大小 70%
2. **壓縮備份文件**: gzip -9 可減少 60-80% 存儲空間
3. **定期清理舊備份**: 僅保留 90 天數據 (法規要求)
4. **使用 Lifecycle Policy**: 自動遷移至低成本存儲層級

---

## 監控與告警

### Prometheus 備份監控指標

```csharp
// src/Infrastructure/Metrics/BackupMetrics.cs
using Prometheus;

public static class BackupMetrics
{
    public static readonly Counter BackupExecutionsTotal = Metrics.CreateCounter(
        "backup_executions_total",
        "Total number of backup executions",
        new CounterConfiguration
        {
            LabelNames = new[] { "backup_type", "status" }
        });

    public static readonly Histogram BackupDuration = Metrics.CreateHistogram(
        "backup_duration_seconds",
        "Backup execution duration in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "backup_type" },
            Buckets = new[] { 30, 60, 120, 300, 600, 1800, 3600 }
        });

    public static readonly Gauge BackupSizeBytes = Metrics.CreateGauge(
        "backup_size_bytes",
        "Backup file size in bytes",
        new GaugeConfiguration
        {
            LabelNames = new[] { "backup_type" }
        });

    public static readonly Gauge LastSuccessfulBackupTimestamp = Metrics.CreateGauge(
        "backup_last_success_timestamp_seconds",
        "Timestamp of last successful backup",
        new GaugeConfiguration
        {
            LabelNames = new[] { "backup_type" }
        });
}
```

### Grafana Dashboard

```json
{
  "dashboard": {
    "title": "Backup Monitoring Dashboard",
    "panels": [
      {
        "id": 1,
        "title": "Backup Success Rate (Last 24h)",
        "type": "stat",
        "targets": [
          {
            "expr": "sum(rate(backup_executions_total{status=\"success\"}[24h])) / sum(rate(backup_executions_total[24h])) * 100",
            "legendFormat": "Success Rate %"
          }
        ]
      },
      {
        "id": 2,
        "title": "Last Successful Backup",
        "type": "stat",
        "targets": [
          {
            "expr": "time() - backup_last_success_timestamp_seconds",
            "legendFormat": "{{backup_type}} - Age (seconds)"
          }
        ]
      },
      {
        "id": 3,
        "title": "Backup Duration Trends",
        "type": "graph",
        "targets": [
          {
            "expr": "backup_duration_seconds",
            "legendFormat": "{{backup_type}}"
          }
        ]
      }
    ]
  }
}
```

### 告警規則

```yaml
# prometheus/backup-alerts.yml
groups:
- name: backup_alerts
  interval: 1m
  rules:
  - alert: BackupFailure
    expr: |
      increase(backup_executions_total{status="failure"}[1h]) > 0
    for: 5m
    labels:
      severity: critical
    annotations:
      summary: "Backup failed for {{ $labels.backup_type }}"
      description: "Backup execution failed in the last hour for {{ $labels.backup_type }}"

  - alert: BackupTooOld
    expr: |
      (time() - backup_last_success_timestamp_seconds) > 7200
    for: 10m
    labels:
      severity: warning
    annotations:
      summary: "No successful backup for {{ $labels.backup_type }} in 2 hours"
      description: "Last successful backup was {{ $value | humanizeDuration }} ago"

  - alert: BackupDurationHigh
    expr: |
      backup_duration_seconds > 3600
    for: 5m
    labels:
      severity: warning
    annotations:
      summary: "Backup duration is too high for {{ $labels.backup_type }}"
      description: "Backup took {{ $value | humanizeDuration }} to complete"
```

---

## 檢查清單

### 備份實施檢查清單

- [ ] **PostgreSQL 備份**
  - [ ] WAL 連續歸檔已啟用並測試
  - [ ] 每小時增量備份 CronJob 正常運行
  - [ ] 每日全量備份 CronJob 正常運行
  - [ ] 備份文件成功上傳到 Azure Blob Storage

- [ ] **Redis 備份**
  - [ ] RDB 快照配置正確
  - [ ] AOF 持久化已啟用
  - [ ] 每小時備份 CronJob 正常運行

- [ ] **MongoDB 備份**
  - [ ] 每日備份 CronJob 正常運行
  - [ ] 備份文件完整性已驗證

- [ ] **Kubernetes etcd 備份**
  - [ ] 每 6 小時快照 CronJob 正常運行
  - [ ] etcd 快照驗證通過

- [ ] **Azure Blob Storage 配置**
  - [ ] Geo-Redundant Storage (GRS) 已啟用
  - [ ] Soft Delete (30 天) 已啟用
  - [ ] Versioning 已啟用
  - [ ] Lifecycle Policy 已配置並生效

- [ ] **備份驗證**
  - [ ] 自動化備份驗證腳本正常運行
  - [ ] 每週恢復演練 CronJob 正常執行
  - [ ] 恢復演練日誌審查通過

- [ ] **監控與告警**
  - [ ] Prometheus 備份指標正常收集
  - [ ] Grafana Dashboard 顯示正常
  - [ ] 告警規則已配置並測試

- [ ] **安全與合規**
  - [ ] 備份數據已加密 (傳輸中 + 靜態)
  - [ ] 訪問控制 (RBAC) 已配置
  - [ ] Audit Log 已啟用

---

## 總結

本文檔提供了完整的備份與恢復策略設計,涵蓋:

✅ **RPO/RTO 定義** (RPO ≤ 15 分鐘, RTO ≤ 2 小時)
✅ **多層備份架構** (PostgreSQL WAL + 增量 + 全量, Redis RDB/AOF, MongoDB, Kubernetes etcd)
✅ **自動化備份流程** (Kubernetes CronJob, Azure Blob Storage)
✅ **備份驗證與測試** (自動化驗證, 定期恢復演練)
✅ **成本優化** (存儲分層, Lifecycle Policy, 月度成本 $109)
✅ **監控與告警** (Prometheus + Grafana, 備份失敗告警)

透過本指南,運維團隊可以實現:
- **數據安全**: 99.9% 備份成功率
- **快速恢復**: 2 小時內完整恢復
- **成本控制**: 年度備份成本 < $1,500
- **合規性**: 符合數據保留和加密要求
