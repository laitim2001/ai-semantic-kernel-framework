# 20. 成本估算 (Cost Estimation)

## 文檔資訊

| 項目 | 內容 |
|------|------|
| **文檔版本** | 1.0.0 |
| **創建日期** | 2025-01-15 |
| **最後更新** | 2025-01-15 |
| **狀態** | Draft |
| **作者** | AI Workflow Platform Team |
| **關聯文檔** | 11-DEPLOYMENT-ARCHITECTURE.md, 18-DISASTER-RECOVERY.md |

---

## 目錄

- [20.1 成本估算總覽](#201-成本估算總覽)
- [20.2 Azure 基礎設施成本](#202-azure-基礎設施成本)
- [20.3 OpenAI API 成本](#203-openai-api-成本)
- [20.4 運維成本](#204-運維成本)
- [20.5 按用戶規模成本](#205-按用戶規模成本)
- [20.6 成本優化建議](#206-成本優化建議)
- [20.7 成本監控](#207-成本監控)

---

## 20.1 成本估算總覽

### 20.1.1 成本分類

```yaml
cost_categories:
  # 固定成本 (每月)
  fixed_costs:
    infrastructure: "Azure 基礎設施 (計算、存儲、網路)"
    monitoring: "監控和日誌系統"
    backup: "備份和災難恢復"
    networking: "CDN 和負載均衡"
    support: "Azure 技術支持"

  # 變動成本 (按使用量)
  variable_costs:
    openai_api: "OpenAI API 調用 (按 Token)"
    bandwidth: "出站流量 (按 GB)"
    storage_transactions: "存儲事務 (按操作數)"
    compute_scaling: "自動擴展計算資源"

  # 人力成本
  operational_costs:
    devops: "DevOps 工程師"
    sre: "SRE 工程師"
    dba: "數據庫管理員"
    on_call: "On-call 輪值"
```

### 20.1.2 月度成本估算 (生產環境)

**估算前提:**
- 用戶規模: 1000 個活躍用戶
- 每用戶平均: 100 次 Agent 執行/月
- 每次執行平均: 2000 Tokens (input + output)
- 數據增長: 100 GB/月
- 可用性要求: 99.9% (包含 DR)

**月度成本概覽:**

| 類別 | 月成本 (USD) | 年成本 (USD) | 占比 |
|------|-------------|-------------|------|
| **Azure 計算 (AKS)** | $2,500 | $30,000 | 30% |
| **Azure 數據庫 (PostgreSQL)** | $1,200 | $14,400 | 14% |
| **Azure 存儲 (Blob/Disk)** | $800 | $9,600 | 10% |
| **Azure 網路 (Front Door/CDN)** | $400 | $4,800 | 5% |
| **Azure 其他 (Redis/Key Vault)** | $300 | $3,600 | 4% |
| **OpenAI API** | $2,000 | $24,000 | 24% |
| **備份和 DR** | $600 | $7,200 | 7% |
| **監控 (第三方工具)** | $500 | $6,000 | 6% |
| **總計** | **$8,300** | **$99,600** | **100%** |

---

## 20.2 Azure 基礎設施成本

### 20.2.1 Azure Kubernetes Service (AKS)

**Node Pool 配置和成本:**

```yaml
production_aks_costs:
  # System Node Pool
  system_pool:
    vm_size: Standard_D4s_v5  # 4 vCPU, 16 GB RAM
    node_count: 3
    price_per_vm: $0.192/hour
    monthly_cost: $414.72
    annual_cost: $4,976.64

  # Application Node Pool
  application_pool:
    vm_size: Standard_D8s_v5  # 8 vCPU, 32 GB RAM
    average_node_count: 8  # 5-20 with auto-scaling
    price_per_vm: $0.384/hour
    monthly_cost: $2,211.84
    annual_cost: $26,542.08

  # Code Execution Node Pool
  code_execution_pool:
    vm_size: Standard_D8s_v5  # 8 vCPU, 32 GB RAM
    average_node_count: 5  # 3-15 with auto-scaling
    price_per_vm: $0.384/hour
    monthly_cost: $1,382.40
    annual_cost: $16,588.80

  # Managed Disk (OS Disk for each node)
  managed_disks:
    disk_type: Premium SSD
    disk_size: 128 GB per node
    total_nodes: 16
    price_per_disk: $19.71/month
    monthly_cost: $315.36
    annual_cost: $3,784.32

  total_aks_cost:
    monthly: $4,324.32
    annual: $51,891.84
```

**成本明細計算:**

```
System Pool:
  $0.192/hour × 3 nodes × 730 hours/month = $420.48

Application Pool (平均 8 nodes):
  $0.384/hour × 8 nodes × 730 hours/month = $2,243.52

Code Execution Pool (平均 5 nodes):
  $0.384/hour × 5 nodes × 730 hours/month = $1,401.60

Managed Disks:
  $19.71/month × 16 disks = $315.36

Total AKS: $4,381.0/month
```

### 20.2.2 Azure Database for PostgreSQL

```yaml
postgresql_costs:
  # Production Instance
  primary_instance:
    tier: GeneralPurpose
    compute: GP_Standard_D8ds_v4  # 8 vCPU, 32 GB RAM
    storage: 1 TB
    backup_storage: 1 TB (included)
    high_availability: Zone-Redundant

    pricing:
      compute: $0.468/hour × 730 = $341.64/month
      storage: $0.115/GB × 1024 GB = $117.76/month
      backup_storage_extra: $0  # First TB included
      high_availability: $341.64/month  # 100% of compute
      total_monthly: $800.04

  # DR Replica (West Europe)
  dr_replica:
    tier: GeneralPurpose
    compute: GP_Standard_D8ds_v4
    storage: 1 TB
    replication: Included in primary cost

    pricing:
      compute: $341.64/month
      storage: $117.76/month
      total_monthly: $459.40

  total_postgresql_cost:
    monthly: $1,259.44
    annual: $15,113.28
```

### 20.2.3 Azure Cache for Redis

```yaml
redis_costs:
  # Production Cache
  primary_cache:
    tier: Premium
    size: P3 (26 GB)
    price: $0.825/hour
    monthly_cost: $602.25
    annual_cost: $7,227.00

  # DR Cache (Geo-Replication)
  dr_cache:
    tier: Premium
    size: P3 (26 GB)
    geo_replication: Included
    additional_cost: $0  # Included in primary

  total_redis_cost:
    monthly: $602.25
    annual: $7,227.00
```

### 20.2.4 Azure Storage

```yaml
storage_costs:
  # Blob Storage (User Files)
  blob_storage:
    tier: Hot
    redundancy: GRS
    capacity: 10 TB
    price_per_gb: $0.0184/month
    monthly_cost: $188.42

    transactions:
      write_operations: 1,000,000/month
      read_operations: 10,000,000/month
      write_cost: $0.10
      read_cost: $0.40
      total_transaction_cost: $0.50

    total_blob_cost: $188.92/month

  # Managed Disk (Database + PVCs)
  managed_disks:
    premium_ssd_total: 5 TB
    price_per_gb: $0.153/month
    monthly_cost: $783.36

  # Backup Storage (GRS)
  backup_storage:
    capacity: 2 TB
    redundancy: GRS
    price_per_gb: $0.05/month
    monthly_cost: $102.40

  total_storage_cost:
    monthly: $1,074.68
    annual: $12,896.16
```

### 20.2.5 Azure Networking

```yaml
networking_costs:
  # Azure Front Door
  front_door:
    base_fee: $35/month
    routing_rules: 5 × $1 = $5/month
    outbound_data: 5 TB × $0.12/GB = $614.40/month
    total_front_door: $654.40/month

  # Load Balancer
  load_balancer:
    standard_lb: 2 instances
    price_per_lb: $0.025/hour
    monthly_cost: $36.50

  # Public IP Addresses
  public_ips:
    count: 5
    price_per_ip: $3.65/month
    monthly_cost: $18.25

  # VNet Peering (Primary ↔ DR)
  vnet_peering:
    data_transfer: 500 GB/month
    price_per_gb: $0.01
    monthly_cost: $5.00

  total_networking_cost:
    monthly: $714.15
    annual: $8,569.80
```

### 20.2.6 其他 Azure 服務

```yaml
other_azure_costs:
  # Azure Key Vault
  key_vault:
    secrets: 100
    operations: 100,000/month
    price_per_secret: $0.03/month
    price_per_10k_operations: $0.03
    monthly_cost: $3.30

  # Azure Monitor
  azure_monitor:
    log_ingestion: 50 GB/month
    price_per_gb: $2.76
    monthly_cost: $138.00

  # Azure Container Registry
  container_registry:
    tier: Premium
    storage: 500 GB
    base_price: $167/month
    storage_price: $0.10/GB × 500 = $50
    monthly_cost: $217.00

  # Azure Application Insights
  app_insights:
    data_ingestion: 10 GB/month
    price_per_gb: $2.30
    monthly_cost: $23.00

  total_other_costs:
    monthly: $381.30
    annual: $4,575.60
```

**Azure 總成本:**

| 服務 | 月成本 | 年成本 |
|------|--------|--------|
| AKS (計算) | $4,381.00 | $52,572.00 |
| PostgreSQL | $1,259.44 | $15,113.28 |
| Redis | $602.25 | $7,227.00 |
| Storage | $1,074.68 | $12,896.16 |
| Networking | $714.15 | $8,569.80 |
| 其他服務 | $381.30 | $4,575.60 |
| **Azure 總計** | **$8,412.82** | **$100,953.84** |

---

## 20.3 OpenAI API 成本

### 20.3.1 定價模型

**GPT-4 定價 (2025):**

| 模型 | Input Price | Output Price |
|------|-------------|--------------|
| **GPT-4** | $0.03 / 1K tokens | $0.06 / 1K tokens |
| **GPT-4-32k** | $0.06 / 1K tokens | $0.12 / 1K tokens |
| **GPT-3.5-turbo** | $0.0005 / 1K tokens | $0.0015 / 1K tokens |

**Embedding 定價:**

| 模型 | Price |
|------|-------|
| **text-embedding-3-small** | $0.00002 / 1K tokens |
| **text-embedding-3-large** | $0.00013 / 1K tokens |

### 20.3.2 使用量估算

**假設:**
- 1000 個活躍用戶
- 每用戶 100 次執行/月 = 100,000 次執行/月
- 每次執行平均: 1000 input tokens + 1000 output tokens
- 模型分佈: 80% GPT-4, 20% GPT-3.5-turbo

**成本計算:**

```yaml
gpt4_usage:
  executions: 80,000/month  # 80% of total
  input_tokens: 80,000,000  # 80k × 1000
  output_tokens: 80,000,000
  input_cost: 80,000 K tokens × $0.03 = $2,400
  output_cost: 80,000 K tokens × $0.06 = $4,800
  total_gpt4: $7,200/month

gpt35_turbo_usage:
  executions: 20,000/month  # 20% of total
  input_tokens: 20,000,000
  output_tokens: 20,000,000
  input_cost: 20,000 K tokens × $0.0005 = $10
  output_cost: 20,000 K tokens × $0.0015 = $30
  total_gpt35: $40/month

embedding_usage:
  # Knowledge Base 向量化
  documents: 10,000
  avg_tokens_per_doc: 500
  total_tokens: 5,000,000
  cost: 5,000 K tokens × $0.00002 = $0.10/month

total_openai_cost:
  monthly: $7,240.10
  annual: $86,881.20
```

**實際保守估算 (考慮失敗重試):**

```yaml
adjusted_openai_cost:
  base_cost: $7,240/month
  retry_overhead: 10%  # 失敗重試
  development_testing: 5%  # 開發測試用量
  total_monthly: $8,326/month
  total_annual: $99,912/month
```

### 20.3.3 按不同用量場景

| 用戶規模 | 月執行次數 | Input Tokens | Output Tokens | 月成本 (USD) | 年成本 (USD) |
|---------|-----------|-------------|--------------|-------------|-------------|
| **100 users** | 10,000 | 10M | 10M | $900 | $10,800 |
| **500 users** | 50,000 | 50M | 50M | $4,500 | $54,000 |
| **1,000 users** | 100,000 | 100M | 100M | $9,000 | $108,000 |
| **5,000 users** | 500,000 | 500M | 500M | $45,000 | $540,000 |
| **10,000 users** | 1,000,000 | 1B | 1B | $90,000 | $1,080,000 |

---

## 20.4 運維成本

### 20.4.1 第三方工具和服務

```yaml
third_party_costs:
  # Monitoring & Alerting
  pagerduty:
    plan: Business
    users: 5
    price_per_user: $41/month
    monthly_cost: $205

  datadog:
    plan: Pro
    hosts: 20
    price_per_host: $23/month
    monthly_cost: $460
    # 或使用開源方案 (Prometheus + Grafana) 降為 $0

  # CI/CD
  github_actions:
    plan: Team
    users: 10
    price_per_user: $4/month
    monthly_cost: $40
    compute_minutes: 3,000 minutes included
    additional_minutes: 1,000 × $0.008 = $8
    total_github: $48

  # Security
  snyk:
    plan: Team
    developers: 10
    price_per_developer: $52/month
    monthly_cost: $520

  # Error Tracking
  sentry:
    plan: Business
    events: 500K/month
    monthly_cost: $99

  total_third_party:
    monthly: $1,332
    annual: $15,984
```

### 20.4.2 人力成本 (僅供參考)

```yaml
team_costs_annual:
  # DevOps Engineer (2 FTE)
  devops:
    count: 2
    salary: $120,000/year each
    total: $240,000

  # SRE Engineer (1 FTE)
  sre:
    count: 1
    salary: $130,000/year
    total: $130,000

  # DBA (0.5 FTE, part-time)
  dba:
    count: 0.5
    salary: $110,000/year
    total: $55,000

  # Backend Developers (4 FTE)
  developers:
    count: 4
    salary: $110,000/year each
    total: $440,000

  # Engineering Manager (1 FTE)
  manager:
    count: 1
    salary: $150,000/year
    total: $150,000

  total_team_cost:
    annual: $1,015,000
    monthly: $84,583
```

---

## 20.5 按用戶規模成本

### 20.5.1 100 用戶 (MVP 階段)

```yaml
mvp_costs:
  # Azure Infrastructure (Scaled Down)
  aks_compute: $800/month  # 2 system + 3 app nodes
  postgresql: $300/month  # Smaller instance
  redis: $150/month  # Basic tier
  storage: $200/month
  networking: $150/month
  azure_subtotal: $1,600/month

  # OpenAI API
  openai: $900/month

  # Monitoring
  monitoring: $100/month  # Open source stack

  monthly_total: $2,600
  annual_total: $31,200
  cost_per_user: $26/month
```

### 20.5.2 1,000 用戶 (Production)

```yaml
production_costs:
  # Azure Infrastructure
  azure_infrastructure: $8,413/month

  # OpenAI API
  openai: $9,000/month

  # Third-party Tools
  monitoring: $1,332/month

  # Backup & DR
  backup_dr: $600/month

  monthly_total: $19,345
  annual_total: $232,140
  cost_per_user: $19.35/month
```

### 20.5.3 10,000 用戶 (Enterprise)

```yaml
enterprise_costs:
  # Azure Infrastructure (Auto-scaled)
  aks_compute: $15,000/month  # 3 + 20 + 15 nodes
  postgresql: $2,500/month  # Larger instance
  redis: $1,200/month  # Premium P4
  storage: $3,000/month
  networking: $2,500/month
  azure_subtotal: $24,200/month

  # OpenAI API
  openai: $90,000/month

  # Monitoring
  monitoring: $2,000/month

  # Backup & DR
  backup_dr: $1,500/month

  monthly_total: $117,700
  annual_total: $1,412,400
  cost_per_user: $11.77/month
```

**成本與規模關係:**

| 用戶規模 | 月成本 | 年成本 | 每用戶成本/月 | 規模經濟 |
|---------|-------|--------|-------------|----------|
| 100 | $2,600 | $31,200 | $26.00 | Baseline |
| 1,000 | $19,345 | $232,140 | $19.35 | -25% |
| 10,000 | $117,700 | $1,412,400 | $11.77 | -55% |

**規模經濟分析:**
- 基礎設施成本隨用戶增長亞線性增長 (auto-scaling)
- OpenAI API 成本線性增長 (主要成本驅動)
- 固定成本 (監控、備份) 分攤到更多用戶

---

## 20.6 成本優化建議

### 20.6.1 Azure 基礎設施優化

**1. 使用 Azure Reserved Instances (RI):**

```yaml
reserved_instances_savings:
  # 1-year RI commitment
  standard_d8s_v5:
    on_demand_price: $0.384/hour
    ri_price: $0.269/hour  # 30% discount
    monthly_savings_per_vm: $83.95
    annual_savings_per_vm: $1,007.40

  estimated_savings:
    application_pool: 8 VMs × $1,007 = $8,056/year
    code_execution_pool: 5 VMs × $1,007 = $5,035/year
    total_annual_savings: $13,091 (25% of compute cost)
```

**2. 使用 Azure Spot Instances (開發/測試環境):**

```yaml
spot_instances_savings:
  use_case: Development and Testing environments
  discount: Up to 90%
  risk: May be evicted with 30-second notice
  recommendation: Use for stateless workloads

  estimated_savings:
    dev_environment: $1,200/year
    staging_environment: $2,400/year
    total: $3,600/year
```

**3. 右鍵調整 (Right-sizing):**

```yaml
right_sizing_opportunities:
  # 監控實際使用率，降低過度配置
  persona_service:
    current: Standard_D8s_v5 (8 vCPU)
    actual_usage: 20% CPU average
    recommended: Standard_D4s_v5 (4 vCPU)
    savings: $110/month per node

  text_to_sql_service:
    current: 3 replicas
    actual_usage: Low traffic
    recommended: 2 replicas
    savings: $220/month
```

**4. 使用 Azure Hybrid Benefit (如果有 Windows Server 授權):**

```yaml
hybrid_benefit:
  applicable: If using Windows nodes
  discount: Up to 40%
  requirement: Existing Windows Server licenses
```

### 20.6.2 OpenAI API 成本優化

**1. 模型選擇優化:**

```yaml
model_optimization:
  strategy: Use cheaper models where possible

  # 簡單任務使用 GPT-3.5-turbo
  simple_tasks:
    - Quick questions
    - Simple code generation
    - Basic text completion
    cost_reduction: 90% (vs GPT-4)

  # 複雜任務才使用 GPT-4
  complex_tasks:
    - Advanced reasoning
    - Complex code generation
    - Detailed analysis

  estimated_savings:
    current_mix: 80% GPT-4, 20% GPT-3.5
    optimized_mix: 50% GPT-4, 50% GPT-3.5
    monthly_savings: $3,600 (40% reduction)
```

**2. Token 使用優化:**

```yaml
token_optimization:
  # Prompt Engineering
  prompt_efficiency:
    - Use concise prompts
    - Remove redundant context
    - Estimated reduction: 20% tokens

  # Response Length Control
  max_tokens_tuning:
    - Set appropriate max_tokens
    - Avoid over-generation
    - Estimated reduction: 15% tokens

  # Caching Frequent Responses
  caching_strategy:
    - Cache common persona responses
    - Cache frequent code snippets
    - Estimated reduction: 10% API calls

  total_estimated_savings:
    monthly: $1,800 (20% of OpenAI cost)
```

**3. 實施用量限制:**

```yaml
usage_limits:
  # Per-user rate limiting
  free_tier:
    max_executions: 10/day
    max_tokens: 10K/day

  basic_tier:
    max_executions: 50/day
    max_tokens: 50K/day
    price: $10/month

  pro_tier:
    max_executions: 200/day
    max_tokens: 200K/day
    price: $50/month

  enterprise_tier:
    max_executions: Unlimited
    max_tokens: Unlimited
    price: Custom

  cost_control:
    - Prevents runaway costs
    - Encourages paid tier adoption
    - Estimated savings: $2,000/month (prevented overuse)
```

### 20.6.3 存儲優化

```yaml
storage_optimization:
  # 使用生命週期策略
  blob_lifecycle:
    hot_to_cool: After 90 days
    cool_to_archive: After 365 days
    savings: 50% on aged data

  # 壓縮備份
  backup_compression:
    current_size: 2 TB
    compressed_size: 800 GB
    savings: $60/month

  # 刪除未使用的快照
  snapshot_cleanup:
    automated_deletion: After 7 days
    savings: $100/month

  total_storage_savings:
    monthly: $210
    annual: $2,520
```

### 20.6.4 網路優化

```yaml
networking_optimization:
  # 使用 Azure CDN 緩存靜態內容
  cdn_caching:
    cached_content: 80% of requests
    bandwidth_reduction: 4 TB/month
    savings: $480/month

  # VNet Peering 優化
  vnet_optimization:
    minimize_cross_region_traffic: true
    savings: $50/month

  total_networking_savings:
    monthly: $530
    annual: $6,360
```

**總優化潛力:**

| 優化項目 | 月節省 | 年節省 | 節省比例 |
|---------|-------|--------|---------|
| Azure RI | $1,091 | $13,091 | 13% |
| 右鍵調整 | $330 | $3,960 | 4% |
| OpenAI 模型優化 | $3,600 | $43,200 | 40% |
| Token 優化 | $1,800 | $21,600 | 20% |
| 存儲優化 | $210 | $2,520 | 2% |
| 網路優化 | $530 | $6,360 | 6% |
| **總計** | **$7,561** | **$90,731** | **39%** |

**優化後成本:**

| 項目 | 優化前 | 優化後 | 節省 |
|------|-------|--------|------|
| 月成本 | $19,345 | $11,784 | $7,561 (39%) |
| 年成本 | $232,140 | $141,409 | $90,731 (39%) |
| 每用戶/月 | $19.35 | $11.78 | $7.57 (39%) |

---

## 20.7 成本監控

### 20.7.1 Azure Cost Management

```yaml
azure_cost_management:
  # 設置預算告警
  budget_alerts:
    monthly_budget: $10,000
    alert_thresholds: [50%, 80%, 90%, 100%]
    notification: Email + Slack

  # 成本分析
  cost_analysis:
    group_by:
      - Resource Group
      - Service
      - Location
      - Tag (environment, team, project)

  # 成本建議
  advisor_recommendations:
    - Unused resources
    - Right-sizing opportunities
    - Reserved Instance recommendations
```

**設置預算腳本:**

```bash
# Azure CLI 設置預算
az consumption budget create \
  --budget-name "ai-workflow-prod-monthly" \
  --amount 10000 \
  --time-grain Monthly \
  --start-date 2025-01-01 \
  --end-date 2025-12-31 \
  --resource-group ai-workflow-rg \
  --notifications \
    threshold=50 \
    contact-emails="ops@aiworkflow.com" \
    contact-roles="Owner,Contributor" \
  --notifications \
    threshold=90 \
    contact-emails="cto@aiworkflow.com" \
    contact-roles="Owner"
```

### 20.7.2 OpenAI 使用監控

```csharp
// Cost Tracking Service
public class OpenAICostTrackingService
{
    private readonly IMetricsService _metrics;

    // GPT-4 定價
    private const decimal GPT4_INPUT_PRICE_PER_1K = 0.03m;
    private const decimal GPT4_OUTPUT_PRICE_PER_1K = 0.06m;

    public async Task TrackOpenAIUsageAsync(
        string userId,
        string model,
        int inputTokens,
        int outputTokens)
    {
        // 計算成本
        var inputCost = (inputTokens / 1000m) * GPT4_INPUT_PRICE_PER_1K;
        var outputCost = (outputTokens / 1000m) * GPT4_OUTPUT_PRICE_PER_1K;
        var totalCost = inputCost + outputCost;

        // 記錄 Prometheus 指標
        _metrics.RecordOpenAITokens(model, "input", inputTokens);
        _metrics.RecordOpenAITokens(model, "output", outputTokens);
        _metrics.RecordOpenAICost("openai", (double)totalCost);

        // 存儲到數據庫 (用於賬單)
        await _dbContext.OpenAIUsageLogs.AddAsync(new OpenAIUsageLog
        {
            UserId = userId,
            Model = model,
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            InputCost = inputCost,
            OutputCost = outputCost,
            TotalCost = totalCost,
            Timestamp = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }
}
```

**Grafana Dashboard (成本監控):**

```promql
# 每日 OpenAI 成本
sum(increase(cost_usd_total{service="openai"}[24h]))

# 每用戶平均成本
sum(increase(cost_usd_total{service="openai"}[24h])) / count(active_users)

# 成本趨勢 (過去 7 天)
sum(increase(cost_usd_total[24h]))

# 成本異常檢測
(sum(increase(cost_usd_total[24h])) - avg_over_time(sum(increase(cost_usd_total[24h]))[7d])) /
stddev_over_time(sum(increase(cost_usd_total[24h]))[7d]) > 2
```

### 20.7.3 告警規則

```yaml
# prometheus-cost-rules.yaml
groups:
- name: cost_alerts
  interval: 1h
  rules:

  # 每日成本超過預算
  - alert: DailyCostExceedsBudget
    expr: |
      sum(increase(cost_usd_total[24h])) > 500
    labels:
      severity: warning
    annotations:
      summary: "Daily cost exceeds $500"
      description: "Daily cost is ${{ $value }}, budget is $500"

  # OpenAI API 成本異常增長
  - alert: OpenAICostSpike
    expr: |
      (
        sum(increase(cost_usd_total{service="openai"}[1h]))
        /
        avg_over_time(sum(increase(cost_usd_total{service="openai"}[1h]))[7d])
      ) > 2
    for: 30m
    labels:
      severity: critical
    annotations:
      summary: "OpenAI API cost spike detected"
      description: "OpenAI cost is 2x higher than 7-day average"

  # 月度成本預測超標
  - alert: MonthlyProjectedCostExceedsBudget
    expr: |
      (
        sum(increase(cost_usd_total[7d])) * 30 / 7
      ) > 10000
    labels:
      severity: high
    annotations:
      summary: "Projected monthly cost exceeds budget"
      description: "Projected monthly cost: ${{ $value }}, budget: $10,000"
```

---

## 總結

本文檔提供了 AI Workflow Platform 的完整成本估算，涵蓋:

1. **成本總覽**: 月成本 $19,345, 年成本 $232,140 (1000 用戶)
2. **Azure 基礎設施**: $8,413/月 (43% 總成本)
3. **OpenAI API**: $9,000/月 (47% 總成本)
4. **運維成本**: $1,332/月 (7% 總成本)
5. **規模經濟**: 10,000 用戶時每用戶成本降至 $11.77/月 (55% 節省)
6. **成本優化**: 39% 潛在節省 ($90,731/年)
7. **成本監控**: Azure Cost Management + Prometheus 指標

**關鍵發現:**
- **OpenAI API 是最大成本驅動** (47%)，優化潛力最大
- **規模經濟明顯**: 用戶規模增長 100 倍，每用戶成本降低 55%
- **優化建議**: Reserved Instances、模型選擇、Token 優化可節省 39%

**成本對比 (1000 用戶):**

| 場景 | 月成本 | 年成本 | 每用戶/月 |
|------|-------|--------|----------|
| **基準** | $19,345 | $232,140 | $19.35 |
| **優化後** | $11,784 | $141,409 | $11.78 |
| **節省** | $7,561 | $90,731 | $7.57 (39%) |

**相關文檔:**
- 11-DEPLOYMENT-ARCHITECTURE.md - 基礎設施配置
- 18-DISASTER-RECOVERY.md - DR 成本
- 15-MONITORING.md - 監控成本指標

---

**版本歷史:**

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0.0 | 2025-01-15 | AI Workflow Team | 初始版本 |
