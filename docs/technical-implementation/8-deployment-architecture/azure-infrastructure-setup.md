# Azure 基礎設施設置指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台在 Azure 上的完整基礎設施設置方案,涵蓋:
- **Azure 服務架構設計** (AKS, Azure Database, Redis, Storage, OpenAI)
- **基礎設施即代碼 (IaC)** (Terraform, ARM Templates, Bicep)
- **網絡架構與安全** (VNet, NSG, Private Link, Firewall)
- **高可用性設計** (多區域部署, 自動擴展, 負載均衡)
- **成本優化策略** (資源定價, Reserved Instances, Spot Instances)
- **監控與告警** (Azure Monitor, Application Insights, Log Analytics)

### 技術背景
Azure 提供全面的雲端基礎設施服務,支持企業級應用部署:
- **計算服務**: Azure Kubernetes Service (AKS), Container Instances, App Service
- **數據庫服務**: Azure Database for PostgreSQL, Cosmos DB
- **緩存服務**: Azure Cache for Redis
- **存儲服務**: Blob Storage, File Storage
- **AI 服務**: Azure OpenAI Service, Cognitive Services
- **網絡服務**: Virtual Network, Load Balancer, Application Gateway, CDN
- **安全服務**: Key Vault, Security Center, Private Link

**基礎設施即代碼 (IaC) 優勢**:
- **可重複性**: 基礎設施配置版本化,可重複部署到多個環境
- **一致性**: 開發、測試、生產環境配置一致
- **自動化**: 通過 CI/CD Pipeline 自動創建和更新基礎設施
- **審計性**: 所有變更記錄在 Git 歷史中
- **協作性**: 團隊共同維護基礎設施代碼

---

## Azure 服務架構

### 整體架構圖

```
┌──────────────────────────────────────────────────────────────────┐
│ Azure Front Door (Global Load Balancer + CDN)                   │
│ - DDoS Protection                                                │
│ - Web Application Firewall (WAF)                                 │
└────────────────────┬─────────────────────────────────────────────┘
                     │
        ┌────────────┴────────────┐
        │                         │
┌───────▼─────────┐     ┌─────────▼────────┐
│ Region: East US │     │ Region: West US  │
│ (Primary)       │     │ (DR Failover)    │
└───────┬─────────┘     └─────────┬────────┘
        │                         │
┌───────▼──────────────────────────▼──────────────┐
│ Virtual Network (VNet): 10.0.0.0/16             │
├──────────────────────────────────────────────────┤
│ ┌─ AKS Subnet: 10.0.1.0/24                      │
│ │  ├─ AKS Cluster (3+ nodes)                    │
│ │  ├─ System Node Pool (monitoring, ingress)    │
│ │  └─ User Node Pool (application workloads)    │
│ │                                                │
│ ┌─ Database Subnet: 10.0.2.0/24                 │
│ │  ├─ Azure Database for PostgreSQL (HA)        │
│ │  └─ Private Endpoint                          │
│ │                                                │
│ ┌─ Redis Subnet: 10.0.3.0/24                    │
│ │  ├─ Azure Cache for Redis (Premium Cluster)   │
│ │  └─ Private Endpoint                          │
│ │                                                │
│ ┌─ Private Endpoint Subnet: 10.0.4.0/24         │
│ │  ├─ Storage Account Private Endpoint          │
│ │  ├─ Key Vault Private Endpoint                │
│ │  └─ Azure OpenAI Private Endpoint             │
└──────────────────────────────────────────────────┘

External Services (Managed by Azure):
├─ Azure OpenAI Service (GPT-4, GPT-3.5)
├─ Azure Blob Storage (GRS - Geo-Redundant)
├─ Azure Key Vault (Secrets, Keys, Certificates)
├─ Azure Container Registry (ACR) (Geo-Replication)
├─ Azure Monitor (Application Insights + Log Analytics)
└─ Azure AD B2C (User Authentication)
```

### 資源分組策略

```
Azure Subscription
├─ Resource Group: rg-aiagent-dev (Development)
│  ├─ AKS: aks-aiagent-dev
│  ├─ PostgreSQL: postgres-aiagent-dev
│  ├─ Redis: redis-aiagent-dev
│  ├─ Storage: staiagentdev
│  ├─ Key Vault: kv-aiagent-dev
│  └─ VNet: vnet-aiagent-dev
│
├─ Resource Group: rg-aiagent-staging (Staging)
│  ├─ AKS: aks-aiagent-staging
│  ├─ PostgreSQL: postgres-aiagent-staging
│  ├─ Redis: redis-aiagent-staging
│  ├─ Storage: staiagentstagig
│  ├─ Key Vault: kv-aiagent-staging
│  └─ VNet: vnet-aiagent-staging
│
├─ Resource Group: rg-aiagent-prod (Production)
│  ├─ AKS: aks-aiagent-prod (Multi-Zone HA)
│  ├─ PostgreSQL: postgres-aiagent-prod (HA + Read Replicas)
│  ├─ Redis: redis-aiagent-prod (Premium Cluster)
│  ├─ Storage: staiagentprod (GRS + CDN)
│  ├─ Key Vault: kv-aiagent-prod
│  ├─ VNet: vnet-aiagent-prod
│  ├─ Application Insights: appi-aiagent-prod
│  └─ Log Analytics: law-aiagent-prod
│
├─ Resource Group: rg-aiagent-shared (Shared Services)
│  ├─ Azure OpenAI Service: openai-aiagent-shared
│  ├─ Container Registry: acr-aiagent-shared (Geo-Replication)
│  ├─ Azure Front Door: afd-aiagent
│  └─ Azure AD B2C: b2c-aiagent
│
└─ Resource Group: rg-aiagent-network (Networking)
   ├─ VNet Peering Connections
   ├─ Network Security Groups (NSG)
   ├─ Application Gateway
   └─ Azure Firewall
```

---

## Terraform 基礎設施即代碼

### 項目結構

```
terraform/
├── environments/
│   ├── dev/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── terraform.tfvars
│   │   └── outputs.tf
│   ├── staging/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── terraform.tfvars
│   │   └── outputs.tf
│   └── production/
│       ├── main.tf
│       ├── variables.tf
│       ├── terraform.tfvars
│       └── outputs.tf
│
├── modules/
│   ├── aks/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── postgresql/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── redis/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── storage/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   ├── keyvault/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── outputs.tf
│   └── networking/
│       ├── main.tf
│       ├── variables.tf
│       └── outputs.tf
│
├── backend.tf         # Terraform State Backend (Azure Storage)
└── README.md
```

### Backend 配置: `terraform/backend.tf`

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "sttfstateaiagent"
    container_name       = "tfstate"
    key                  = "aiagent.tfstate"
  }

  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.45"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.5"
    }
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy = false  # 防止意外刪除
    }
    resource_group {
      prevent_deletion_if_contains_resources = true
    }
  }
}
```

### Production 主配置: `terraform/environments/production/main.tf`

```hcl
locals {
  environment         = "production"
  location           = "eastus"
  location_secondary = "westus"
  resource_prefix    = "aiagent"

  common_tags = {
    Environment = "Production"
    Project     = "AI Agent Platform"
    ManagedBy   = "Terraform"
    CostCenter  = "Engineering"
  }
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-${local.resource_prefix}-${local.environment}"
  location = local.location
  tags     = local.common_tags
}

# Virtual Network
module "networking" {
  source = "../../modules/networking"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  vnet_name           = "vnet-${local.resource_prefix}-${local.environment}"
  address_space       = ["10.0.0.0/16"]

  subnets = {
    aks = {
      name             = "snet-aks"
      address_prefixes = ["10.0.1.0/24"]
    }
    database = {
      name             = "snet-database"
      address_prefixes = ["10.0.2.0/24"]
      delegation = {
        name = "Microsoft.DBforPostgreSQL/flexibleServers"
        service_delegation = {
          name = "Microsoft.DBforPostgreSQL/flexibleServers"
          actions = [
            "Microsoft.Network/virtualNetworks/subnets/join/action"
          ]
        }
      }
    }
    redis = {
      name             = "snet-redis"
      address_prefixes = ["10.0.3.0/24"]
    }
    private_endpoints = {
      name             = "snet-private-endpoints"
      address_prefixes = ["10.0.4.0/24"]
    }
  }

  tags = local.common_tags
}

# Azure Kubernetes Service (AKS)
module "aks" {
  source = "../../modules/aks"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  cluster_name        = "aks-${local.resource_prefix}-${local.environment}"

  kubernetes_version  = "1.28.3"
  dns_prefix          = "${local.resource_prefix}-${local.environment}"

  # System Node Pool (for system components like monitoring, ingress)
  default_node_pool = {
    name                = "system"
    node_count          = 3
    vm_size             = "Standard_D4s_v3"
    availability_zones  = ["1", "2", "3"]
    enable_auto_scaling = true
    min_count           = 3
    max_count           = 5
    max_pods            = 30
    vnet_subnet_id      = module.networking.subnet_ids["aks"]
  }

  # User Node Pool (for application workloads)
  user_node_pools = [
    {
      name                = "workload"
      node_count          = 5
      vm_size             = "Standard_D8s_v3"
      availability_zones  = ["1", "2", "3"]
      enable_auto_scaling = true
      min_count           = 5
      max_count           = 20
      max_pods            = 50
    }
  ]

  # Network Configuration
  network_plugin     = "azure"
  network_policy     = "azure"
  service_cidr       = "10.1.0.0/16"
  dns_service_ip     = "10.1.0.10"
  docker_bridge_cidr = "172.17.0.1/16"

  # Add-ons
  enable_azure_policy             = true
  enable_oms_agent                = true
  log_analytics_workspace_id      = module.monitoring.log_analytics_workspace_id
  enable_ingress_application_gateway = false  # 使用 NGINX Ingress

  # Azure AD Integration
  enable_azure_active_directory   = true
  azure_rbac_enabled              = true

  tags = local.common_tags
}

# Azure Database for PostgreSQL (Flexible Server)
module "postgresql" {
  source = "../../modules/postgresql"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  server_name         = "postgres-${local.resource_prefix}-${local.environment}"

  # High Availability Configuration
  sku_name            = "GP_Standard_D4ds_v4"  # General Purpose, 4 vCores
  storage_mb          = 524288  # 512 GB
  backup_retention_days = 35
  geo_redundant_backup_enabled = true

  # HA Configuration
  high_availability = {
    mode                      = "ZoneRedundant"
    standby_availability_zone = "2"
  }

  # PostgreSQL Version
  version = "16"

  # Network Configuration
  delegated_subnet_id = module.networking.subnet_ids["database"]
  private_dns_zone_id = azurerm_private_dns_zone.postgres.id

  # Admin Credentials (from Key Vault)
  administrator_login    = "aiagent_admin"
  administrator_password = data.azurerm_key_vault_secret.db_password.value

  # Read Replicas (for read-heavy workloads)
  read_replicas = [
    {
      name     = "replica-1"
      location = local.location_secondary
    }
  ]

  tags = local.common_tags
}

# Azure Cache for Redis (Premium Cluster)
module "redis" {
  source = "../../modules/redis"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  redis_name          = "redis-${local.resource_prefix}-${local.environment}"

  # Premium Tier with Clustering
  sku_name            = "Premium"
  capacity            = 2  # P2: 6 GB
  family              = "P"

  # Clustering Configuration
  shard_count         = 3  # 3 shards for horizontal scaling

  # Network Configuration
  subnet_id           = module.networking.subnet_ids["redis"]

  # Persistence (RDB + AOF for data durability)
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  rdb_backup_enabled              = true
  rdb_backup_frequency            = 60  # minutes
  rdb_backup_max_snapshot_count   = 1

  # Zone Redundancy (for high availability)
  zones = ["1", "2", "3"]

  tags = local.common_tags
}

# Azure Blob Storage (Geo-Redundant)
module "storage" {
  source = "../../modules/storage"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  storage_account_name = "st${local.resource_prefix}${local.environment}"

  # Account Configuration
  account_tier             = "Standard"
  account_replication_type = "GRS"  # Geo-Redundant Storage
  account_kind             = "StorageV2"

  # Blob Configuration
  containers = [
    {
      name        = "agent-workflows"
      access_type = "private"
    },
    {
      name        = "user-uploads"
      access_type = "private"
    },
    {
      name        = "exports"
      access_type = "private"
    }
  ]

  # Network Rules
  network_rules = {
    default_action = "Deny"
    ip_rules       = []
    virtual_network_subnet_ids = [
      module.networking.subnet_ids["aks"],
      module.networking.subnet_ids["private_endpoints"]
    ]
  }

  # Enable Soft Delete
  blob_soft_delete_retention_days      = 7
  container_soft_delete_retention_days = 7

  # Enable Versioning
  enable_versioning = true

  # Enable Azure CDN (for static content)
  enable_cdn = true
  cdn_profile_name = "cdn-${local.resource_prefix}-${local.environment}"

  tags = local.common_tags
}

# Azure Key Vault
module "keyvault" {
  source = "../../modules/keyvault"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  key_vault_name      = "kv-${local.resource_prefix}-${local.environment}"

  # SKU
  sku_name = "premium"  # Hardware Security Module (HSM) support

  # Network Configuration
  network_acls = {
    default_action = "Deny"
    ip_rules       = []
    virtual_network_subnet_ids = [
      module.networking.subnet_ids["aks"]
    ]
  }

  # Enable Soft Delete and Purge Protection
  soft_delete_retention_days = 90
  purge_protection_enabled   = true

  # RBAC for AKS Managed Identity
  access_policies = [
    {
      tenant_id = data.azurerm_client_config.current.tenant_id
      object_id = module.aks.kubelet_identity_object_id

      secret_permissions = ["Get", "List"]
      key_permissions    = ["Get", "List", "Decrypt", "Encrypt"]
    }
  ]

  tags = local.common_tags
}

# Azure Monitor & Application Insights
module "monitoring" {
  source = "../../modules/monitoring"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  # Log Analytics Workspace
  log_analytics_name = "law-${local.resource_prefix}-${local.environment}"
  retention_in_days  = 90
  sku                = "PerGB2018"

  # Application Insights
  application_insights_name = "appi-${local.resource_prefix}-${local.environment}"
  application_type          = "web"

  # Sampling Configuration (10% to reduce cost)
  sampling_percentage = 10

  # Daily Cap (to prevent cost overrun)
  daily_data_cap_in_gb = 10

  tags = local.common_tags
}

# Private DNS Zones
resource "azurerm_private_dns_zone" "postgres" {
  name                = "privatelink.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.main.name
}

resource "azurerm_private_dns_zone_virtual_network_link" "postgres" {
  name                  = "postgres-vnet-link"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.postgres.name
  virtual_network_id    = module.networking.vnet_id
}
```

### Variables: `terraform/environments/production/variables.tf`

```hcl
variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "environment" {
  description = "Environment name (dev, staging, production)"
  type        = string
  default     = "production"
}

variable "aks_kubernetes_version" {
  description = "Kubernetes version for AKS"
  type        = string
  default     = "1.28.3"
}

variable "aks_system_node_count" {
  description = "Number of system nodes in AKS"
  type        = number
  default     = 3
}

variable "aks_workload_node_min_count" {
  description = "Minimum number of workload nodes"
  type        = number
  default     = 5
}

variable "aks_workload_node_max_count" {
  description = "Maximum number of workload nodes"
  type        = number
  default     = 20
}

variable "postgresql_sku_name" {
  description = "PostgreSQL SKU name"
  type        = string
  default     = "GP_Standard_D4ds_v4"
}

variable "postgresql_storage_mb" {
  description = "PostgreSQL storage in MB"
  type        = number
  default     = 524288  # 512 GB
}

variable "redis_capacity" {
  description = "Redis cache capacity (P1, P2, P3, P4, P5)"
  type        = number
  default     = 2  # P2: 6 GB
}

variable "redis_shard_count" {
  description = "Number of Redis shards for clustering"
  type        = number
  default     = 3
}

variable "tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default = {
    Environment = "Production"
    Project     = "AI Agent Platform"
    ManagedBy   = "Terraform"
  }
}
```

### Outputs: `terraform/environments/production/outputs.tf`

```hcl
output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "aks_cluster_name" {
  description = "Name of the AKS cluster"
  value       = module.aks.cluster_name
}

output "aks_cluster_id" {
  description = "ID of the AKS cluster"
  value       = module.aks.cluster_id
}

output "aks_kube_config" {
  description = "Kubernetes config for AKS cluster"
  value       = module.aks.kube_config
  sensitive   = true
}

output "postgresql_fqdn" {
  description = "Fully qualified domain name of PostgreSQL server"
  value       = module.postgresql.fqdn
}

output "postgresql_connection_string" {
  description = "PostgreSQL connection string"
  value       = module.postgresql.connection_string
  sensitive   = true
}

output "redis_hostname" {
  description = "Redis cache hostname"
  value       = module.redis.hostname
}

output "redis_primary_access_key" {
  description = "Redis primary access key"
  value       = module.redis.primary_access_key
  sensitive   = true
}

output "storage_account_name" {
  description = "Name of the storage account"
  value       = module.storage.storage_account_name
}

output "storage_primary_connection_string" {
  description = "Primary connection string for storage account"
  value       = module.storage.primary_connection_string
  sensitive   = true
}

output "key_vault_name" {
  description = "Name of the Key Vault"
  value       = module.keyvault.key_vault_name
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = module.keyvault.key_vault_uri
}

output "application_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = module.monitoring.instrumentation_key
  sensitive   = true
}

output "application_insights_connection_string" {
  description = "Application Insights connection string"
  value       = module.monitoring.connection_string
  sensitive   = true
}
```

---

## 部署步驟

### 1. 初始化 Terraform State Backend

```bash
# 創建 Resource Group 用於存儲 Terraform State
az group create \
  --name rg-terraform-state \
  --location eastus

# 創建 Storage Account
az storage account create \
  --name sttfstateaiagent \
  --resource-group rg-terraform-state \
  --location eastus \
  --sku Standard_LRS \
  --encryption-services blob

# 創建 Blob Container
az storage container create \
  --name tfstate \
  --account-name sttfstateaiagent

# 啟用 Blob Versioning (用於狀態文件版本控制)
az storage account blob-service-properties update \
  --account-name sttfstateaiagent \
  --enable-versioning true
```

### 2. 初始化 Terraform

```bash
cd terraform/environments/production

# 初始化 Terraform (下載 Provider 插件)
terraform init

# 驗證配置文件語法
terraform validate

# 格式化配置文件
terraform fmt -recursive
```

### 3. 規劃基礎設施變更

```bash
# 生成執行計劃
terraform plan -out=tfplan

# 查看詳細計劃
terraform show tfplan
```

### 4. 應用基礎設施變更

```bash
# 應用變更 (創建/更新/刪除資源)
terraform apply tfplan

# 或直接應用 (需要手動確認)
terraform apply

# 自動批准 (用於 CI/CD Pipeline)
terraform apply -auto-approve
```

### 5. 驗證部署

```bash
# 獲取 AKS Credentials
az aks get-credentials \
  --resource-group rg-aiagent-production \
  --name aks-aiagent-production \
  --overwrite-existing

# 驗證 Kubernetes 連接
kubectl get nodes
kubectl get namespaces

# 查看 Terraform Outputs
terraform output

# 查看敏感 Outputs
terraform output -json | jq '.postgresql_connection_string.value' -r
```

---

## 網絡安全配置

### Network Security Groups (NSG)

```hcl
# terraform/modules/networking/nsg.tf
resource "azurerm_network_security_group" "aks" {
  name                = "nsg-aks-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name

  # Allow HTTPS Ingress
  security_rule {
    name                       = "AllowHTTPS"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "Internet"
    destination_address_prefix = "*"
  }

  # Allow HTTP Ingress (for redirect to HTTPS)
  security_rule {
    name                       = "AllowHTTP"
    priority                   = 110
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80"
    source_address_prefix      = "Internet"
    destination_address_prefix = "*"
  }

  # Deny all other Inbound
  security_rule {
    name                       = "DenyAllInbound"
    priority                   = 4096
    direction                  = "Inbound"
    access                     = "Deny"
    protocol                   = "*"
    source_port_range          = "*"
    destination_port_range     = "*"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

# Associate NSG with AKS Subnet
resource "azurerm_subnet_network_security_group_association" "aks" {
  subnet_id                 = azurerm_subnet.aks.id
  network_security_group_id = azurerm_network_security_group.aks.id
}
```

### Private Endpoint 配置

```hcl
# terraform/modules/storage/private_endpoint.tf
resource "azurerm_private_endpoint" "storage_blob" {
  name                = "pe-${var.storage_account_name}-blob"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_endpoint_subnet_id

  private_service_connection {
    name                           = "psc-${var.storage_account_name}-blob"
    private_connection_resource_id = azurerm_storage_account.main.id
    is_manual_connection           = false
    subresource_names              = ["blob"]
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [var.private_dns_zone_blob_id]
  }
}

# Private DNS Zone for Blob Storage
resource "azurerm_private_dns_zone" "blob" {
  name                = "privatelink.blob.core.windows.net"
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "blob" {
  name                  = "blob-vnet-link"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.blob.name
  virtual_network_id    = var.vnet_id
}
```

---

## 高可用性與災難恢復

### Multi-Region 部署

```hcl
# terraform/environments/production/main.tf
# Primary Region (East US)
module "primary_region" {
  source = "../../modules/regional_deployment"

  location            = "eastus"
  environment         = "production"
  is_primary_region   = true

  # ... other configuration
}

# Secondary Region (West US) - DR Failover
module "secondary_region" {
  source = "../../modules/regional_deployment"

  location            = "westus"
  environment         = "production"
  is_primary_region   = false

  # Read-only replicas
  postgresql_primary_server_id = module.primary_region.postgresql_server_id

  # ... other configuration
}

# Azure Traffic Manager (Global Load Balancing)
resource "azurerm_traffic_manager_profile" "main" {
  name                   = "tm-aiagent-production"
  resource_group_name    = azurerm_resource_group.main.name
  traffic_routing_method = "Priority"  # Primary-Secondary failover

  dns_config {
    relative_name = "aiagent-prod"
    ttl           = 30
  }

  monitor_config {
    protocol = "HTTPS"
    port     = 443
    path     = "/health"
  }
}

# Primary Endpoint
resource "azurerm_traffic_manager_azure_endpoint" "primary" {
  name               = "primary-eastus"
  profile_id         = azurerm_traffic_manager_profile.main.id
  target_resource_id = module.primary_region.public_ip_id
  priority           = 1
}

# Secondary Endpoint
resource "azurerm_traffic_manager_azure_endpoint" "secondary" {
  name               = "secondary-westus"
  profile_id         = azurerm_traffic_manager_profile.main.id
  target_resource_id = module.secondary_region.public_ip_id
  priority           = 2
}
```

### Auto-Scaling 配置

```hcl
# AKS Cluster Autoscaler
resource "azurerm_kubernetes_cluster_node_pool" "workload" {
  name                  = "workload"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = "Standard_D8s_v3"

  enable_auto_scaling = true
  min_count           = 5
  max_count           = 20

  # Node Labels for workload scheduling
  node_labels = {
    "workload-type" = "general"
  }

  # Node Taints (for dedicated workloads)
  node_taints = []
}

# Horizontal Pod Autoscaler (HPA) - Kubernetes Level
# Defined in Kubernetes manifests (k8s/base/hpa.yaml)
```

---

## 成本優化

### Reserved Instances

```bash
# 購買 1 年期 Reserved Instances (節省 30-40%)
az reservations reservation-order purchase \
  --reserved-resource-type VirtualMachines \
  --sku Standard_D8s_v3 \
  --location eastus \
  --quantity 10 \
  --term P1Y \  # 1 Year
  --billing-scope /subscriptions/<subscription-id>

# 購買 3 年期 Reserved Instances (節省 50-60%)
az reservations reservation-order purchase \
  --reserved-resource-type VirtualMachines \
  --sku Standard_D8s_v3 \
  --location eastus \
  --quantity 5 \
  --term P3Y \  # 3 Years
  --billing-scope /subscriptions/<subscription-id>
```

### Spot Instances (Dev/Staging)

```hcl
# 僅用於開發/測試環境,不適用於生產環境
resource "azurerm_kubernetes_cluster_node_pool" "spot" {
  name                  = "spot"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = "Standard_D8s_v3"

  # Spot Configuration (節省 70-90% 成本)
  priority        = "Spot"
  eviction_policy = "Delete"
  spot_max_price  = -1  # Pay up to on-demand price

  enable_auto_scaling = true
  min_count           = 0
  max_count           = 10

  node_labels = {
    "kubernetes.azure.com/scalesetpriority" = "spot"
  }

  node_taints = [
    "kubernetes.azure.com/scalesetpriority=spot:NoSchedule"
  ]
}
```

### 成本監控與告警

```hcl
# Azure Cost Management Alert
resource "azurerm_consumption_budget_subscription" "main" {
  name            = "budget-aiagent-production"
  subscription_id = data.azurerm_subscription.current.id

  amount     = 5000  # USD per month
  time_grain = "Monthly"

  time_period {
    start_date = "2024-01-01T00:00:00Z"
  }

  notification {
    enabled        = true
    threshold      = 80  # Alert at 80% of budget
    operator       = "GreaterThan"
    contact_emails = ["engineering@example.com", "finance@example.com"]
  }

  notification {
    enabled        = true
    threshold      = 100  # Alert at 100% of budget
    operator       = "GreaterThan"
    contact_emails = ["engineering@example.com", "finance@example.com"]
  }
}
```

---

## 監控與告警

### Application Insights 配置

```hcl
# terraform/modules/monitoring/application_insights.tf
resource "azurerm_application_insights" "main" {
  name                = "appi-${var.application_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.main.id

  # Sampling Configuration (reduce cost)
  sampling_percentage = var.sampling_percentage  # 10% for production

  # Daily Cap (prevent cost overrun)
  daily_data_cap_in_gb = var.daily_data_cap_in_gb  # 10 GB per day

  tags = var.tags
}

# Smart Detection Rules
resource "azurerm_application_insights_smart_detection_rule" "failure_anomalies" {
  name                    = "Failure Anomalies"
  application_insights_id = azurerm_application_insights.main.id
  enabled                 = true
  send_emails_to_subscription_owners = false
  additional_email_recipients        = ["engineering@example.com"]
}
```

### Metric Alerts

```hcl
# CPU Usage Alert
resource "azurerm_monitor_metric_alert" "aks_cpu_high" {
  name                = "alert-aks-cpu-high"
  resource_group_name = var.resource_group_name
  scopes              = [azurerm_kubernetes_cluster.main.id]
  description         = "Alert when AKS CPU usage exceeds 80%"

  criteria {
    metric_namespace = "Microsoft.ContainerService/managedClusters"
    metric_name      = "node_cpu_usage_percentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }

  action {
    action_group_id = azurerm_monitor_action_group.engineering.id
  }
}

# Memory Usage Alert
resource "azurerm_monitor_metric_alert" "aks_memory_high" {
  name                = "alert-aks-memory-high"
  resource_group_name = var.resource_group_name
  scopes              = [azurerm_kubernetes_cluster.main.id]
  description         = "Alert when AKS memory usage exceeds 85%"

  criteria {
    metric_namespace = "Microsoft.ContainerService/managedClusters"
    metric_name      = "node_memory_working_set_percentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 85
  }

  action {
    action_group_id = azurerm_monitor_action_group.engineering.id
  }
}

# Action Group (Notification Channel)
resource "azurerm_monitor_action_group" "engineering" {
  name                = "ag-engineering"
  resource_group_name = var.resource_group_name
  short_name          = "eng"

  email_receiver {
    name          = "Engineering Team"
    email_address = "engineering@example.com"
  }

  sms_receiver {
    name         = "On-Call Engineer"
    country_code = "1"
    phone_number = "5551234567"
  }

  webhook_receiver {
    name        = "Slack Webhook"
    service_uri = "https://hooks.slack.com/services/..."
  }
}
```

---

## 故障排查

### Terraform 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **State Lock 錯誤** | 多個 Terraform 進程同時訪問 State | 手動解鎖: `terraform force-unlock <lock-id>` |
| **Provider Authentication 失敗** | Azure 憑證過期或權限不足 | 重新登錄: `az login` 或檢查 Service Principal 權限 |
| **Resource Already Exists** | 資源已存在但不在 State 中 | 導入資源: `terraform import <resource_type>.<name> <resource_id>` |
| **Timeout 錯誤** | 資源創建時間超過默認超時 | 增加超時: `timeouts { create = "60m" }` |

### 調試技巧

```bash
# 啟用詳細日誌
export TF_LOG=DEBUG
export TF_LOG_PATH=./terraform-debug.log
terraform apply

# 查看 State
terraform state list
terraform state show azurerm_kubernetes_cluster.main

# 刷新 State (同步實際狀態)
terraform refresh

# 導入現有資源
terraform import azurerm_resource_group.main /subscriptions/<sub-id>/resourceGroups/rg-aiagent-production

# 清理孤立資源 (謹慎使用)
terraform state rm <resource_address>
```

---

## 安全性檢查清單

- [ ] **網絡隔離**
  - [ ] 所有子網配置 NSG
  - [ ] 數據庫、Redis 使用 Private Endpoint
  - [ ] 禁用公共訪問,僅允許 VNet 內部通信

- [ ] **身份與訪問管理**
  - [ ] AKS 啟用 Azure AD 集成
  - [ ] 使用 Managed Identity 訪問 Key Vault
  - [ ] Service Principal 權限最小化

- [ ] **數據加密**
  - [ ] 啟用 Storage Account 加密 (默認)
  - [ ] PostgreSQL 啟用 SSL/TLS
  - [ ] Redis 禁用非 SSL 端口

- [ ] **監控與審計**
  - [ ] 啟用 Azure Monitor
  - [ ] 配置 Metric Alerts (CPU, Memory, Disk)
  - [ ] 啟用 Azure Activity Log

- [ ] **備份與災難恢復**
  - [ ] PostgreSQL 自動備份 (35 天保留)
  - [ ] Blob Storage 啟用 Soft Delete (7 天)
  - [ ] Terraform State 啟用 Versioning

---

## 總結

本文檔提供了完整的 Azure 基礎設施設置方案,涵蓋:

✅ **完整的 Azure 服務架構** (AKS, PostgreSQL, Redis, Blob Storage, Key Vault)
✅ **Terraform 基礎設施即代碼** (模塊化設計,多環境支持)
✅ **網絡安全配置** (VNet, NSG, Private Endpoint, Private DNS)
✅ **高可用性設計** (Multi-Zone, Read Replicas, Traffic Manager)
✅ **成本優化策略** (Reserved Instances, Spot Instances, Budget Alerts)
✅ **監控與告警** (Application Insights, Metric Alerts, Action Groups)

透過本指南,DevOps 團隊可以實現:
- **可重複部署**: 基礎設施配置代碼化,一鍵部署到多個環境
- **高可用性**: Multi-Zone 部署,99.95% SLA
- **安全合規**: 網絡隔離、加密傳輸、最小權限原則
- **成本可控**: 資源優化、預算告警、Reserved Instances
