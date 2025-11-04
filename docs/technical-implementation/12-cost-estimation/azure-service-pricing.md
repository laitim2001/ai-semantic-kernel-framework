# Azure 服務定價分析

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台在 Azure 雲端環境的完整成本分析,涵蓋:
- **核心計算資源** (AKS, VM, Container Instances)
- **數據存儲服務** (PostgreSQL, Redis, Blob Storage)
- **網絡與負載均衡** (Application Gateway, Front Door, VPN Gateway)
- **AI 與認知服務** (Azure OpenAI Service, Cognitive Services)
- **監控與安全** (Azure Monitor, Key Vault, Security Center)
- **成本計算模型** (按需計費, 預留實例, Spot Instances)
- **區域定價差異** (East US vs. West Europe vs. Southeast Asia)

### 定價原則

**Azure 定價模式**:
- ✅ **按用量付費 (Pay-As-You-Go)**: 按實際使用量計費,無長期承諾
- ✅ **預留實例 (Reserved Instances)**: 1年/3年預付,節省 40-65%
- ✅ **Spot Instances**: 競價實例,節省高達 90%,適用於批處理
- ✅ **混合優惠 (Hybrid Benefit)**: 現有授權轉移,節省 40%
- ✅ **開發測試定價**: 開發/測試環境專屬折扣

**成本優化目標**:
- 生產環境總成本: **$3,500-$5,000/月** (中型部署)
- 單用戶月均成本: **$0.35-$0.50** (10,000 活躍用戶)
- 成本效率比: **≥ 85%** (有效使用率)

---

## 核心計算資源定價

### Azure Kubernetes Service (AKS)

#### 控制平面 (Control Plane)
- **免費層**: 控制平面本身免費,僅支付節點虛擬機成本
- **標準層 (SLA 99.95%)**: $73/月/cluster
- **建議配置**: 生產環境使用標準層,獲得 SLA 保障

#### 工作節點 (Worker Nodes)

**生產環境節點池配置**:

```yaml
# 節點池配置與成本計算
node_pools:
  - name: "system-pool"
    purpose: "系統組件 (kube-system, monitoring)"
    vm_size: "Standard_D4s_v5"
    specs:
      vcpus: 4
      ram_gb: 16
      storage_gb: 128 # SSD
    pricing:
      pay_as_you_go: "$140.16/月/node"
      reserved_1yr: "$91.10/月/node (35% 節省)"
      reserved_3yr: "$65.07/月/node (54% 節省)"
    node_count:
      min: 3  # Multi-zone HA
      max: 3  # 固定大小,系統池不自動擴展
    monthly_cost:
      pay_as_you_go: "$420.48"
      reserved_1yr: "$273.30"
      reserved_3yr: "$195.21"

  - name: "application-pool"
    purpose: "應用服務 (API, Worker, Frontend)"
    vm_size: "Standard_D8s_v5"
    specs:
      vcpus: 8
      ram_gb: 32
      storage_gb: 256 # SSD
    pricing:
      pay_as_you_go: "$280.32/月/node"
      reserved_1yr: "$182.21/月/node (35% 節省)"
      reserved_3yr: "$130.15/月/node (54% 節省)"
    node_count:
      min: 3  # 基礎容量
      avg: 5  # 平均負載
      max: 10 # 峰值容量
    monthly_cost:
      baseline: "$1,401.60 (5 nodes PAYG)"
      reserved_avg: "$911.05 (5 nodes 1yr)"
      peak_burst: "$2,803.20 (10 nodes PAYG)"

  - name: "ai-agent-pool"
    purpose: "AI Agent 執行 (CPU/Memory 密集)"
    vm_size: "Standard_D16s_v5"
    specs:
      vcpus: 16
      ram_gb: 64
      storage_gb: 512 # SSD
    pricing:
      pay_as_you_go: "$560.64/月/node"
      reserved_1yr: "$364.42/月/node (35% 節省)"
      reserved_3yr: "$260.30/月/node (54% 節省)"
    node_count:
      min: 2  # 基礎容量
      avg: 4  # 平均負載
      max: 8  # 峰值容量
    monthly_cost:
      baseline: "$2,242.56 (4 nodes PAYG)"
      reserved_avg: "$1,457.68 (4 nodes 1yr)"
      peak_burst: "$4,485.12 (8 nodes PAYG)"
```

**AKS 總成本估算 (生產環境)**:

| 配置 | 節點數 | 定價模式 | 月成本 |
|------|-------|---------|--------|
| System Pool | 3 nodes (D4s_v5) | 1年預留 | $273.30 |
| Application Pool | 5 nodes (D8s_v5) | 1年預留 | $911.05 |
| AI Agent Pool | 4 nodes (D16s_v5) | 1年預留 | $1,457.68 |
| AKS Standard SLA | 1 cluster | 標準層 | $73.00 |
| **總計** | **12 nodes** | **混合** | **$2,715.03** |

**Cluster Autoscaler 動態成本**:
- 低峰時段 (夜間): 9 nodes → $2,150/月
- 平均負載 (工作日): 12 nodes → $2,715/月
- 高峰時段 (業務高峰): 18 nodes → $3,850/月

---

### Azure Container Instances (ACI)

**使用場景**: 短期任務, Batch Jobs, CI/CD Builds

```yaml
aci_pricing:
  cpu:
    price_per_vcpu: "$0.0000125/秒 ($0.045/小時)"
    monthly_1vcpu_fulltime: "$32.40"

  memory:
    price_per_gb: "$0.0000014/秒 ($0.00504/小時)"
    monthly_1gb_fulltime: "$3.63"

  example_workloads:
    - name: "Workflow Execution Job"
      specs: "2 vCPU, 4 GB RAM"
      duration: "平均 5 分鐘/job"
      frequency: "1000 jobs/月"
      cost_per_job: "$0.038"
      monthly_cost: "$38.00"

    - name: "Scheduled Data Processing"
      specs: "4 vCPU, 8 GB RAM"
      duration: "每天 2 小時"
      frequency: "每日執行"
      monthly_hours: "60 小時"
      monthly_cost: "$28.94"
```

---

## 數據存儲服務定價

### Azure Database for PostgreSQL - Flexible Server

#### 計算資源定價

```yaml
postgresql_compute:
  tier: "General Purpose"

  configurations:
    - name: "Primary Database (Production)"
      vm_size: "Standard_D4ds_v5"
      specs:
        vcpus: 4
        ram_gb: 16
      pricing:
        pay_as_you_go: "$206.40/月"
        reserved_1yr: "$134.16/月 (35% 節省)"
        reserved_3yr: "$95.83/月 (54% 節省)"
      high_availability:
        enabled: true
        cost_multiplier: "2x (主 + 待機)"
        monthly_cost_ha: "$268.32 (1年預留 HA)"

    - name: "Read Replica (Analytics)"
      vm_size: "Standard_D2ds_v5"
      specs:
        vcpus: 2
        ram_gb: 8
      pricing:
        pay_as_you_go: "$103.20/月"
        reserved_1yr: "$67.08/月 (35% 節省)"
      monthly_cost: "$67.08"

    - name: "DR Replica (West US 2)"
      vm_size: "Standard_D4ds_v5"
      specs:
        vcpus: 4
        ram_gb: 16
      pricing:
        pay_as_you_go: "$206.40/月"
        reserved_1yr: "$134.16/月 (35% 節省)"
      monthly_cost: "$134.16"
```

#### 存儲定價

```yaml
postgresql_storage:
  storage_type: "Premium SSD (P30)"

  pricing:
    base_storage: "$0.115/GB/月"
    backup_storage: "$0.095/GB/月 (GRS)"
    iops: "已包含在 Premium SSD 價格"

  configurations:
    - database: "Primary (HA)"
      storage_gb: 512
      backup_retention_days: 35
      backup_storage_gb: 1024  # 含 WAL 歸檔
      monthly_cost:
        data_storage: "$58.88 (512 GB × $0.115)"
        backup_storage: "$97.28 (1024 GB × $0.095)"
        total: "$156.16"

    - database: "Read Replica"
      storage_gb: 256
      backup_retention_days: 7
      backup_storage_gb: 256
      monthly_cost:
        data_storage: "$29.44"
        backup_storage: "$24.32"
        total: "$53.76"

    - database: "DR Replica"
      storage_gb: 512
      backup_retention_days: 35
      backup_storage_gb: 512
      monthly_cost:
        data_storage: "$58.88"
        backup_storage: "$48.64"
        total: "$107.52"
```

**PostgreSQL 總成本**:

| 組件 | 配置 | 月成本 |
|------|------|--------|
| Primary HA Compute | D4ds_v5 HA (1年預留) | $268.32 |
| Primary Storage | 512 GB + 1024 GB Backup | $156.16 |
| Read Replica Compute | D2ds_v5 (1年預留) | $67.08 |
| Read Replica Storage | 256 GB + 256 GB Backup | $53.76 |
| DR Replica Compute | D4ds_v5 (1年預留) | $134.16 |
| DR Replica Storage | 512 GB + 512 GB Backup | $107.52 |
| **總計** | **3 Instances** | **$787.00** |

---

### Azure Cache for Redis

#### 定價層級

```yaml
redis_pricing:
  tier: "Premium (P1)"

  configurations:
    - name: "Production Redis Cluster"
      sku: "Premium P1"
      specs:
        cache_size_gb: 6
        shards: 3  # 分片數
        replicas_per_shard: 2  # 每分片副本數 (HA)
      pricing:
        base_cost: "$0.368/小時/分片"
        monthly_per_shard: "$265.92"
        total_shards: 3
        monthly_cost: "$797.76"

      features:
        - "Redis Persistence (RDB + AOF)"
        - "Geo-Replication"
        - "VNet Integration"
        - "99.9% SLA"

    - name: "Session Cache (Standard C3)"
      sku: "Standard C3"
      specs:
        cache_size_gb: 6
        replicas: 1  # Standard tier 含 1 副本
      pricing:
        base_cost: "$0.246/小時"
        monthly_cost: "$177.70"

      features:
        - "Redis Replication"
        - "99.9% SLA"
```

**Redis 總成本**: $797.76/月 (Premium P1 Cluster) 或 $177.70/月 (Standard C3)

**建議**: 生產環境使用 Premium P1,開發/測試使用 Standard C3

---

### Azure Blob Storage

#### 存儲層級定價

```yaml
blob_storage_pricing:
  storage_account_type: "StorageV2 (General Purpose v2)"
  redundancy: "GRS (Geo-Redundant Storage)"

  tiers:
    - name: "Hot Tier"
      use_case: "頻繁訪問數據 (應用日誌, 臨時文件)"
      pricing:
        storage: "$0.0184/GB/月 (GRS)"
        write_operations: "$0.055/10,000 operations"
        read_operations: "$0.0044/10,000 operations"
      monthly_usage:
        storage_gb: 500
        write_ops: 1000000  # 100萬次
        read_ops: 5000000   # 500萬次
      monthly_cost:
        storage: "$9.20"
        write: "$5.50"
        read: "$2.20"
        total: "$16.90"

    - name: "Cool Tier"
      use_case: "不常訪問數據 (備份, 歸檔日誌)"
      pricing:
        storage: "$0.01/GB/月 (GRS)"
        write_operations: "$0.10/10,000 operations"
        read_operations: "$0.01/10,000 operations"
        retrieval: "$0.01/GB"
      monthly_usage:
        storage_gb: 2000  # PostgreSQL 備份
        write_ops: 100000  # 10萬次
        read_ops: 50000    # 5萬次
        retrieval_gb: 50   # 恢復測試
      monthly_cost:
        storage: "$20.00"
        write: "$1.00"
        read: "$0.05"
        retrieval: "$0.50"
        total: "$21.55"

    - name: "Archive Tier"
      use_case: "長期歸檔 (法規遵循, 審計日誌)"
      pricing:
        storage: "$0.00099/GB/月 (GRS)"
        write_operations: "$0.11/10,000 operations"
        read_operations: "$5.50/10,000 operations"
        retrieval_standard: "$0.02/GB (標準, 15小時)"
        retrieval_high: "$0.03/GB (高優先級, 1小時)"
      monthly_usage:
        storage_gb: 5000  # 長期歸檔
        write_ops: 10000
        read_ops: 100  # 極少讀取
      monthly_cost:
        storage: "$4.95"
        write: "$0.11"
        read: "$0.055"
        total: "$5.12"
```

**Blob Storage 總成本**:
- Hot Tier: $16.90/月 (500 GB)
- Cool Tier: $21.55/月 (2000 GB)
- Archive Tier: $5.12/月 (5000 GB)
- **總計**: $43.57/月

---

## 網絡與負載均衡定價

### Azure Application Gateway (WAF v2)

```yaml
application_gateway_pricing:
  sku: "WAF_v2"
  tier: "Standard_v2 with WAF"

  fixed_costs:
    capacity_units_base: "$0.0144/小時/單位"
    minimum_units: 2
    monthly_base: "$20.74"  # 2 單位最低費用

  variable_costs:
    capacity_unit_hour: "$0.0144/小時"
    avg_capacity_units: 5  # 平均負載
    monthly_capacity: "$52.00"

  data_processing:
    price_per_gb: "$0.008/GB"
    monthly_traffic_gb: 1000  # 1 TB
    monthly_cost: "$8.00"

  total_monthly_cost: "$80.74"
```

### Azure Front Door (Standard)

```yaml
front_door_pricing:
  tier: "Standard"

  base_cost: "$35/月/域名"
  domains: 1  # aiagent.company.com
  base_monthly: "$35.00"

  data_transfer:
    outbound_internet:
      first_10tb: "$0.085/GB"
      next_40tb: "$0.065/GB"
    monthly_traffic_gb: 500  # 500 GB
    monthly_cost: "$42.50"

  requests:
    http_https: "$0.0075/10,000 requests"
    monthly_requests: 10000000  # 1000萬次
    monthly_cost: "$7.50"

  total_monthly_cost: "$85.00"
```

### VPN Gateway (Site-to-Site)

```yaml
vpn_gateway_pricing:
  sku: "VpnGw1"

  gateway_hours: "$0.19/小時"
  monthly_cost: "$137.20"

  data_transfer:
    outbound_gb: 100
    price_per_gb: "$0.087/GB (前 5 TB)"
    monthly_cost: "$8.70"

  total_monthly_cost: "$145.90"
```

**網絡總成本**:
- Application Gateway: $80.74/月
- Front Door: $85.00/月
- VPN Gateway: $145.90/月 (可選)
- **總計**: $311.64/月

---

## AI 與認知服務定價

### Azure OpenAI Service

#### GPT-4 模型定價

```yaml
openai_pricing:
  model: "GPT-4 (8K context)"

  pricing:
    input_tokens: "$0.03/1K tokens"
    output_tokens: "$0.06/1K tokens"

  usage_estimation:
    - scenario: "Workflow Planning Agent"
      avg_input_tokens: 2000
      avg_output_tokens: 1000
      executions_per_month: 50000
      cost_per_execution: "$0.12"
      monthly_cost: "$6,000"

    - scenario: "Code Generation Agent"
      avg_input_tokens: 1500
      avg_output_tokens: 800
      executions_per_month: 30000
      cost_per_execution: "$0.093"
      monthly_cost: "$2,790"

    - scenario: "Data Analysis Agent"
      avg_input_tokens: 3000
      avg_output_tokens: 1500
      executions_per_month: 20000
      cost_per_execution: "$0.18"
      monthly_cost: "$3,600"

  total_monthly_cost: "$12,390"

  optimization_strategies:
    - "使用 GPT-3.5 Turbo 替代簡單任務 (節省 90%)"
    - "Token 優化 (減少 prompt 長度, 使用 function calling)"
    - "結果緩存 (Redis cache 相同查詢)"
    - "批處理請求 (減少 API 調用次數)"

  optimized_monthly_cost: "$6,200 (50% 節省)"
```

#### GPT-3.5 Turbo 定價

```yaml
gpt35_pricing:
  model: "GPT-3.5 Turbo (4K context)"

  pricing:
    input_tokens: "$0.0015/1K tokens"
    output_tokens: "$0.002/1K tokens"

  usage_estimation:
    - scenario: "Simple Q&A Agent"
      avg_input_tokens: 500
      avg_output_tokens: 300
      executions_per_month: 100000
      cost_per_execution: "$0.00135"
      monthly_cost: "$135"

    - scenario: "Text Classification"
      avg_input_tokens: 300
      avg_output_tokens: 50
      executions_per_month: 200000
      cost_per_execution: "$0.00055"
      monthly_cost: "$110"

  total_monthly_cost: "$245"
```

**Azure OpenAI 總成本**: $6,445/月 (優化後)

---

### Azure Cognitive Services

```yaml
cognitive_services:
  - service: "Computer Vision API"
    tier: "S1"
    pricing:
      transactions: "$1.50/1,000 transactions"
    usage:
      monthly_transactions: 50000
      monthly_cost: "$75.00"

  - service: "Text Analytics API"
    tier: "S"
    pricing:
      text_records: "$2.00/1,000 records"
    usage:
      monthly_records: 100000
      monthly_cost: "$200.00"

  - service: "Speech Service"
    tier: "S0"
    pricing:
      standard_hours: "$1.00/hour"
    usage:
      monthly_hours: 50
      monthly_cost: "$50.00"

total_cognitive_cost: "$325.00/月"
```

---

## 監控與安全定價

### Azure Monitor

```yaml
azure_monitor_pricing:
  components:
    - name: "Log Analytics Workspace"
      pricing:
        data_ingestion: "$2.99/GB (前 5 GB/天 免費)"
        data_retention: "$0.12/GB/月 (90天後)"
      usage:
        daily_ingestion_gb: 10  # 應用日誌 + 系統日誌
        monthly_ingestion_gb: 300
        billable_gb: 150  # 扣除免費額度 (5 GB/天 × 30天 = 150 GB)
        retention_beyond_90days_gb: 500
      monthly_cost:
        ingestion: "$448.50 (150 GB)"
        retention: "$60.00 (500 GB)"
        total: "$508.50"

    - name: "Application Insights"
      pricing:
        data_ingestion: "$2.99/GB"
      usage:
        monthly_ingestion_gb: 50  # 應用遙測數據
      monthly_cost: "$149.50"

    - name: "Metrics (Custom Metrics)"
      pricing:
        metric_samples: "$0.25/百萬個樣本"
      usage:
        monthly_samples_million: 100
      monthly_cost: "$25.00"

    - name: "Alerts"
      pricing:
        metric_alerts: "$0.10/alert/月"
        log_alerts: "$1.50/alert/月"
      usage:
        metric_alerts_count: 50
        log_alerts_count: 20
      monthly_cost:
        metric: "$5.00"
        log: "$30.00"
        total: "$35.00"

total_monitor_cost: "$718.00/月"
```

### Azure Key Vault

```yaml
key_vault_pricing:
  operations:
    secret_operations: "$0.03/10,000 operations"
    certificate_operations: "$3.00/renewal"

  usage:
    secret_operations_monthly: 1000000  # 100萬次
    certificate_renewals: 5

  monthly_cost:
    operations: "$3.00"
    certificates: "$15.00"
    total: "$18.00"
```

### Azure Security Center (Defender for Cloud)

```yaml
security_center_pricing:
  plans:
    - name: "Defender for Servers"
      pricing: "$15/server/月"
      servers: 12  # AKS nodes
      monthly_cost: "$180.00"

    - name: "Defender for Containers"
      pricing: "$7/vCore/月"
      vcores: 96  # 總 vCore 數
      monthly_cost: "$672.00"

    - name: "Defender for Databases"
      pricing: "$15/server/月"
      servers: 3  # PostgreSQL instances
      monthly_cost: "$45.00"

total_security_cost: "$897.00/月"
```

**監控與安全總成本**: $1,633.00/月

---

## 數據傳輸定價

### 出站數據傳輸 (Egress)

```yaml
data_transfer_pricing:
  zones:
    zone1:  # North America, Europe
      first_10tb: "$0.087/GB"
      next_40tb: "$0.083/GB"
      next_100tb: "$0.070/GB"
      over_150tb: "$0.050/GB"

  usage_estimation:
    - scenario: "API 響應數據"
      monthly_gb: 500
      rate: "$0.087/GB"
      monthly_cost: "$43.50"

    - scenario: "文件下載 (Blob Storage)"
      monthly_gb: 300
      rate: "$0.087/GB"
      monthly_cost: "$26.10"

    - scenario: "監控數據導出"
      monthly_gb: 100
      rate: "$0.087/GB"
      monthly_cost: "$8.70"

  total_egress_cost: "$78.30/月"

  free_tier:
    first_100gb: "免費"
    between_regions: "有費用 ($0.02/GB)"
    within_region: "免費"
```

---

## 區域定價差異

### 主要區域成本比較

```yaml
regional_pricing_comparison:
  baseline_config:
    description: "相同配置在不同區域的成本"
    specs: "12 AKS nodes (D8s_v5), PostgreSQL HA, Redis Premium"

  regions:
    - name: "East US"
      location_code: "eastus"
      relative_cost: "100% (基準)"
      aks_d8s_v5_monthly: "$280.32"
      postgresql_d4ds_v5_monthly: "$206.40"
      redis_p1_monthly: "$265.92"
      total_monthly: "$2,850.00"

    - name: "West US 2"
      location_code: "westus2"
      relative_cost: "100%"
      aks_d8s_v5_monthly: "$280.32"
      postgresql_d4ds_v5_monthly: "$206.40"
      redis_p1_monthly: "$265.92"
      total_monthly: "$2,850.00"

    - name: "West Europe"
      location_code: "westeurope"
      relative_cost: "110%"
      aks_d8s_v5_monthly: "$308.35"
      postgresql_d4ds_v5_monthly: "$227.04"
      redis_p1_monthly: "$292.51"
      total_monthly: "$3,135.00"

    - name: "Southeast Asia"
      location_code: "southeastasia"
      relative_cost: "115%"
      aks_d8s_v5_monthly: "$322.37"
      postgresql_d4ds_v5_monthly: "$237.36"
      redis_p1_monthly: "$305.81"
      total_monthly: "$3,277.50"

    - name: "Japan East"
      location_code: "japaneast"
      relative_cost: "120%"
      aks_d8s_v5_monthly: "$336.38"
      postgresql_d4ds_v5_monthly: "$247.68"
      redis_p1_monthly: "$319.10"
      total_monthly: "$3,420.00"

  recommendations:
    production_primary: "East US (最低成本, 高可用性)"
    dr_secondary: "West US 2 (同等成本, 地理冗餘)"
    europe_expansion: "West Europe (符合 GDPR)"
    asia_expansion: "Southeast Asia (亞洲用戶延遲最低)"
```

---

## 預留實例節省計算器

### 預留實例 (Reserved Instances) 投資回報

```csharp
// scripts/ReservedInstanceROICalculator.cs
using System;

public sealed class ReservedInstanceROICalculator
{
    public record ReservedInstanceOption(
        string Name,
        decimal PayAsYouGoCostMonthly,
        decimal ReservedCost1YearMonthly,
        decimal ReservedCost3YearMonthly,
        decimal UpfrontCost1Year,
        decimal UpfrontCost3Year
    );

    public static void CalculateROI()
    {
        var options = new[]
        {
            new ReservedInstanceOption(
                Name: "AKS D8s_v5 Node (×5)",
                PayAsYouGoCostMonthly: 1401.60m,
                ReservedCost1YearMonthly: 911.05m,
                ReservedCost3YearMonthly: 650.75m,
                UpfrontCost1Year: 0m,  // 按月付款
                UpfrontCost3Year: 0m
            ),
            new ReservedInstanceOption(
                Name: "PostgreSQL D4ds_v5 HA",
                PayAsYouGoCostMonthly: 412.80m,
                ReservedCost1YearMonthly: 268.32m,
                ReservedCost3YearMonthly: 191.66m,
                UpfrontCost1Year: 0m,
                UpfrontCost3Year: 0m
            )
        };

        Console.WriteLine("=== Reserved Instance ROI Analysis ===\n");

        foreach (var option in options)
        {
            Console.WriteLine($"Resource: {option.Name}");
            Console.WriteLine($"Pay-As-You-Go: ${option.PayAsYouGoCostMonthly:N2}/月\n");

            // 1 年預留分析
            var savings1Year = option.PayAsYouGoCostMonthly - option.ReservedCost1YearMonthly;
            var savingsPercent1Year = (savings1Year / option.PayAsYouGoCostMonthly) * 100;
            var totalSavings1Year = savings1Year * 12;

            Console.WriteLine("1 年預留實例:");
            Console.WriteLine($"  月成本: ${option.ReservedCost1YearMonthly:N2}");
            Console.WriteLine($"  月節省: ${savings1Year:N2} ({savingsPercent1Year:F1}%)");
            Console.WriteLine($"  年總節省: ${totalSavings1Year:N2}");
            Console.WriteLine($"  投資回報週期: 立即 (無預付款)\n");

            // 3 年預留分析
            var savings3Year = option.PayAsYouGoCostMonthly - option.ReservedCost3YearMonthly;
            var savingsPercent3Year = (savings3Year / option.PayAsYouGoCostMonthly) * 100;
            var totalSavings3Year = savings3Year * 36;

            Console.WriteLine("3 年預留實例:");
            Console.WriteLine($"  月成本: ${option.ReservedCost3YearMonthly:N2}");
            Console.WriteLine($"  月節省: ${savings3Year:N2} ({savingsPercent3Year:F1}%)");
            Console.WriteLine($"  3年總節省: ${totalSavings3Year:N2}");
            Console.WriteLine($"  投資回報週期: 立即 (無預付款)\n");

            Console.WriteLine("---\n");
        }
    }
}

// 輸出示例:
// === Reserved Instance ROI Analysis ===
//
// Resource: AKS D8s_v5 Node (×5)
// Pay-As-You-Go: $1,401.60/月
//
// 1 年預留實例:
//   月成本: $911.05
//   月節省: $490.55 (35.0%)
//   年總節省: $5,886.60
//   投資回報週期: 立即 (無預付款)
//
// 3 年預留實例:
//   月成本: $650.75
//   月節省: $750.85 (53.6%)
//   3年總節省: $27,030.60
//   投資回報週期: 立即 (無預付款)
```

---

## 成本估算工具

### Azure Pricing Calculator 整合

```python
# scripts/azure_pricing_calculator.py
"""
Azure 定價計算器 - 自動化成本估算
"""
import requests
import json
from typing import Dict, List
from dataclasses import dataclass

@dataclass
class AzureResource:
    """Azure 資源定義"""
    service_name: str
    region: str
    tier: str
    quantity: int
    unit_price: float
    billing_period: str = "Monthly"

class AzurePricingCalculator:
    """Azure 定價計算器"""

    def __init__(self, currency: str = "USD", region: str = "eastus"):
        self.currency = currency
        self.region = region
        self.resources: List[AzureResource] = []

    def add_aks_node_pool(
        self,
        vm_size: str,
        node_count: int,
        reserved_instance: bool = False,
        term_years: int = 1
    ) -> None:
        """添加 AKS 節點池"""
        # 定價數據 (簡化示例)
        pricing = {
            "Standard_D4s_v5": {"payg": 140.16, "1yr": 91.10, "3yr": 65.07},
            "Standard_D8s_v5": {"payg": 280.32, "1yr": 182.21, "3yr": 130.15},
            "Standard_D16s_v5": {"payg": 560.64, "1yr": 364.42, "3yr": 260.30}
        }

        if reserved_instance:
            price_key = f"{term_years}yr"
            unit_price = pricing[vm_size][price_key]
        else:
            unit_price = pricing[vm_size]["payg"]

        resource = AzureResource(
            service_name=f"AKS Node ({vm_size})",
            region=self.region,
            tier="Standard",
            quantity=node_count,
            unit_price=unit_price
        )
        self.resources.append(resource)

    def add_postgresql_flexible_server(
        self,
        vm_size: str,
        storage_gb: int,
        backup_storage_gb: int,
        high_availability: bool = False,
        reserved_instance: bool = False,
        term_years: int = 1
    ) -> None:
        """添加 PostgreSQL Flexible Server"""
        # 計算定價
        compute_pricing = {
            "Standard_D4ds_v5": {"payg": 206.40, "1yr": 134.16, "3yr": 95.83},
            "Standard_D2ds_v5": {"payg": 103.20, "1yr": 67.08, "3yr": 47.92}
        }

        if reserved_instance:
            price_key = f"{term_years}yr"
            compute_cost = compute_pricing[vm_size][price_key]
        else:
            compute_cost = compute_pricing[vm_size]["payg"]

        if high_availability:
            compute_cost *= 2

        storage_cost = storage_gb * 0.115
        backup_cost = backup_storage_gb * 0.095

        total_cost = compute_cost + storage_cost + backup_cost

        resource = AzureResource(
            service_name=f"PostgreSQL Flexible Server ({vm_size})",
            region=self.region,
            tier="GeneralPurpose",
            quantity=1,
            unit_price=total_cost
        )
        self.resources.append(resource)

    def add_redis_cache(self, sku: str, shards: int = 1) -> None:
        """添加 Redis Cache"""
        pricing = {
            "Premium_P1": 265.92,
            "Standard_C3": 177.70,
            "Basic_C2": 68.00
        }

        unit_price = pricing[sku] * shards

        resource = AzureResource(
            service_name=f"Redis Cache ({sku})",
            region=self.region,
            tier=sku.split("_")[0],
            quantity=1,
            unit_price=unit_price
        )
        self.resources.append(resource)

    def add_blob_storage(
        self,
        tier: str,
        storage_gb: int,
        redundancy: str = "GRS"
    ) -> None:
        """添加 Blob Storage"""
        pricing = {
            "Hot": {"LRS": 0.0184, "GRS": 0.0230},
            "Cool": {"LRS": 0.01, "GRS": 0.0125},
            "Archive": {"LRS": 0.00099, "GRS": 0.00248}
        }

        unit_price = storage_gb * pricing[tier][redundancy]

        resource = AzureResource(
            service_name=f"Blob Storage ({tier}, {redundancy})",
            region=self.region,
            tier=tier,
            quantity=storage_gb,
            unit_price=unit_price
        )
        self.resources.append(resource)

    def calculate_total(self) -> Dict:
        """計算總成本"""
        total_monthly = sum(r.unit_price * r.quantity for r in self.resources)
        total_yearly = total_monthly * 12

        breakdown = []
        for resource in self.resources:
            breakdown.append({
                "service": resource.service_name,
                "region": resource.region,
                "quantity": resource.quantity,
                "unit_price": resource.unit_price,
                "monthly_cost": resource.unit_price * resource.quantity
            })

        return {
            "currency": self.currency,
            "total_monthly": round(total_monthly, 2),
            "total_yearly": round(total_yearly, 2),
            "breakdown": breakdown
        }

    def generate_report(self) -> str:
        """生成成本報告"""
        result = self.calculate_total()

        report = f"""
=== Azure 成本估算報告 ===

區域: {self.region}
貨幣: {result['currency']}

月度成本: ${result['total_monthly']:,.2f}
年度成本: ${result['total_yearly']:,.2f}

成本明細:
{"="*60}
"""

        for item in result['breakdown']:
            report += f"\n{item['service']}"
            report += f"\n  數量: {item['quantity']}"
            report += f"\n  單價: ${item['unit_price']:,.2f}"
            report += f"\n  月成本: ${item['monthly_cost']:,.2f}"
            report += f"\n"

        return report


# 使用示例
if __name__ == "__main__":
    calculator = AzurePricingCalculator(region="eastus")

    # 添加 AKS 節點
    calculator.add_aks_node_pool("Standard_D4s_v5", node_count=3, reserved_instance=True)
    calculator.add_aks_node_pool("Standard_D8s_v5", node_count=5, reserved_instance=True)

    # 添加 PostgreSQL
    calculator.add_postgresql_flexible_server(
        vm_size="Standard_D4ds_v5",
        storage_gb=512,
        backup_storage_gb=1024,
        high_availability=True,
        reserved_instance=True
    )

    # 添加 Redis
    calculator.add_redis_cache("Premium_P1", shards=3)

    # 添加 Blob Storage
    calculator.add_blob_storage("Hot", storage_gb=500)
    calculator.add_blob_storage("Cool", storage_gb=2000)

    # 生成報告
    print(calculator.generate_report())
```

---

## 總成本彙總

### 生產環境總成本 (中型部署)

```yaml
production_cost_summary:
  compute:
    aks_cluster: "$2,715.03"
    aci_batch_jobs: "$66.94"
    subtotal: "$2,781.97"

  storage:
    postgresql: "$787.00"
    redis: "$797.76"
    blob_storage: "$43.57"
    subtotal: "$1,628.33"

  networking:
    application_gateway: "$80.74"
    front_door: "$85.00"
    vpn_gateway: "$145.90"
    data_egress: "$78.30"
    subtotal: "$389.94"

  ai_services:
    azure_openai: "$6,445.00"
    cognitive_services: "$325.00"
    subtotal: "$6,770.00"

  monitoring_security:
    azure_monitor: "$718.00"
    key_vault: "$18.00"
    security_center: "$897.00"
    subtotal: "$1,633.00"

  monthly_total: "$13,203.24"
  yearly_total: "$158,438.88"

  cost_per_user:
    active_users: 10000
    cost_per_user_monthly: "$1.32"

  optimization_potential:
    reserved_instances: "節省 $5,000/年"
    right_sizing: "節省 $3,000/年"
    spot_instances: "節省 $2,000/年"
    optimized_yearly: "$148,438.88"
```

### 開發/測試環境成本

```yaml
dev_test_cost_summary:
  compute:
    aks_cluster_small: "$800.00"
    subtotal: "$800.00"

  storage:
    postgresql_standard: "$150.00"
    redis_standard: "$177.70"
    blob_storage: "$20.00"
    subtotal: "$347.70"

  networking:
    basic_load_balancer: "$20.00"
    subtotal: "$20.00"

  ai_services:
    azure_openai_limited: "$500.00"
    subtotal: "$500.00"

  monitoring:
    basic_monitoring: "$100.00"
    subtotal: "$100.00"

  monthly_total: "$1,767.70"
  yearly_total: "$21,212.40"
```

---

## 檢查清單

### 成本優化檢查清單

- [ ] **預留實例 (Reserved Instances)**
  - [ ] AKS 節點池已使用 1年/3年 預留
  - [ ] PostgreSQL 計算已使用 1年 預留
  - [ ] 預留實例覆蓋率 ≥ 70%

- [ ] **Right-Sizing (資源適配)**
  - [ ] 監控 CPU/Memory 使用率 (目標: 70-85%)
  - [ ] 識別過度配置的資源
  - [ ] 季度審查資源規格

- [ ] **自動擴展 (Auto-Scaling)**
  - [ ] HPA 已配置 (CPU 75%, Memory 80%)
  - [ ] Cluster Autoscaler 已啟用
  - [ ] 夜間/週末自動縮容

- [ ] **存儲優化**
  - [ ] Blob Storage 生命週期管理已配置
  - [ ] 舊備份自動移至 Archive Tier
  - [ ] PostgreSQL 備份保留期合理 (35天)

- [ ] **網絡優化**
  - [ ] 使用 Azure CDN 減少出站流量
  - [ ] 跨區域流量最小化
  - [ ] 靜態資源使用 Blob Storage + CDN

- [ ] **AI 服務優化**
  - [ ] 使用 GPT-3.5 替代簡單任務
  - [ ] Token 優化和結果緩存
  - [ ] 批處理 API 請求

---

## 總結

本文檔提供了完整的 Azure 服務定價分析,涵蓋:

✅ **核心計算資源** (AKS $2,715/月, ACI $67/月)
✅ **數據存儲服務** (PostgreSQL $787/月, Redis $798/月, Blob $44/月)
✅ **網絡與負載均衡** (總計 $390/月)
✅ **AI 與認知服務** (Azure OpenAI $6,445/月優化後)
✅ **監控與安全** (總計 $1,633/月)
✅ **預留實例節省** (35-54% 節省, 無預付款)
✅ **區域定價差異** (East US 最優, 其他區域 +10-20%)
✅ **成本計算工具** (Python 自動化定價計算器)

**生產環境總成本**: $13,203/月 (未優化) → $12,370/月 (優化後)

透過本指南,財務團隊可以準確預測雲端成本,並制定有效的成本控制策略。
