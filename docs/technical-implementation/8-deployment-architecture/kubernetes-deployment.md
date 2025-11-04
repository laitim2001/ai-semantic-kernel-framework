# Kubernetes 部署完整配置

**Semantic Kernel Agentic Framework - Kubernetes 部署架構**

**版本**: 1.0.0
**日期**: 2025-11-01
**狀態**: ✅ 完整
**負責角色**: Tech Lead + DevOps Architect

[返回 Technical Implementation](../README.md) | [Docker 容器化](./docker-containerization.md) | [CI/CD Pipeline](./cicd-pipeline.md)

---

## 目錄

1. [概述](#概述)
2. [Kubernetes 集群架構](#kubernetes-集群架構)
3. [Azure Kubernetes Service (AKS) 配置](#azure-kubernetes-service-aks-配置)
4. [命名空間設計](#命名空間設計)
5. [ConfigMap 和 Secret 管理](#configmap-和-secret-管理)
6. [Deployment 配置](#deployment-配置)
7. [StatefulSet 配置](#statefulset-配置)
8. [Service 配置](#service-配置)
9. [Ingress 配置](#ingress-配置)
10. [持久化存儲 (PV/PVC)](#持久化存儲-pvpvc)
11. [自動擴展 (HPA)](#自動擴展-hpa)
12. [網絡策略 (NetworkPolicy)](#網絡策略-networkpolicy)
13. [RBAC 權限配置](#rbac-權限配置)
14. [Helm Charts 完整配置](#helm-charts-完整配置)
15. [監控集成 (Prometheus Operator)](#監控集成-prometheus-operator)
16. [滾動更新和回滾策略](#滾動更新和回滾策略)
17. [災難恢復和高可用性](#災難恢復和高可用性)
18. [部署命令和故障排查](#部署命令和故障排查)

---

## 概述

### 部署目標

本文檔提供 Semantic Kernel Agentic Framework 在 Kubernetes (K8s) 上的完整部署配置，包括：

- ✅ **生產級配置**: 13 個微服務的完整 K8s 資源定義
- ✅ **高可用性**: 多副本、健康檢查、自動恢復
- ✅ **自動擴展**: HPA (Horizontal Pod Autoscaler) 配置
- ✅ **安全加固**: NetworkPolicy、RBAC、Secret 管理
- ✅ **監控集成**: Prometheus Operator + Grafana
- ✅ **Helm 管理**: 完整 Helm Charts 配置
- ✅ **滾動更新**: 零停機部署策略

### 技術棧

| 組件 | 版本 | 說明 |
|------|------|------|
| **Kubernetes** | 1.28+ | 容器編排平台 |
| **Azure Kubernetes Service** | AKS 1.28+ | 托管 K8s 服務 |
| **Helm** | 3.13+ | K8s 包管理器 |
| **NGINX Ingress Controller** | 1.9+ | Ingress 控制器 |
| **cert-manager** | 1.13+ | 自動 HTTPS 證書管理 |
| **Prometheus Operator** | 0.68+ | 監控和告警 |
| **Grafana** | 10.1+ | 可視化儀表板 |

### 架構概覽

```
┌─────────────────────────────────────────────────────────────────┐
│                         Internet                                 │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Azure Load Balancer                           │
│                    (Public IP + HTTPS)                          │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│              NGINX Ingress Controller                            │
│              (SSL Termination + Routing)                        │
└─────────────────┬───────────────────────────────────────────────┘
                  │
     ┌────────────┼────────────┐
     │            │            │
     ▼            ▼            ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│ Frontend │ │   API    │ │  Grafana │
│   Host   │ │ Gateway  │ │          │
│ (React)  │ │          │ │          │
└──────────┘ └────┬─────┘ └──────────┘
                   │
     ┌─────────────┼─────────────┐
     │             │             │
     ▼             ▼             ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│  Agent   │ │ Workflow │ │   LLM    │
│  Service │ │  Service │ │  Service │
└──────────┘ └──────────┘ └──────────┘
     │             │             │
     └─────────────┼─────────────┘
                   │
     ┌─────────────┼─────────────┐
     │             │             │
     ▼             ▼             ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│PostgreSQL│ │  Redis   │ │RabbitMQ  │
│(StatefulS│ │(StatefulS│ │(StatefulS│
│    et)   │ │    et)   │ │    et)   │
└──────────┘ └──────────┘ └──────────┘
     │             │             │
     ▼             ▼             ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│ Azure    │ │ Azure    │ │ Azure    │
│  Disk    │ │  Disk    │ │  Disk    │
└──────────┘ └──────────┘ └──────────┘
```

---

## Kubernetes 集群架構

### 集群規格設計

#### 開發環境 (Dev)

```yaml
cluster_name: sk-agentic-dev
region: East Asia (台北)
kubernetes_version: 1.28.3

node_pools:
  system_pool:
    vm_size: Standard_D2s_v3
    node_count: 2
    os_disk_size_gb: 128
    os_type: Linux
    max_pods_per_node: 30

  application_pool:
    vm_size: Standard_D4s_v3
    node_count: 3
    auto_scaling:
      enabled: true
      min_count: 2
      max_count: 5
    os_disk_size_gb: 256

  stateful_pool:
    vm_size: Standard_D8s_v3
    node_count: 3
    os_disk_size_gb: 512
    labels:
      workload: stateful

networking:
  network_plugin: azure
  network_policy: calico
  service_cidr: 10.0.0.0/16
  dns_service_ip: 10.0.0.10
  pod_cidr: 10.244.0.0/16

addons:
  monitoring: true
  azure_policy: true
  http_application_routing: false
```

#### 生產環境 (Prod)

```yaml
cluster_name: sk-agentic-prod
region: East Asia (台北) + Southeast Asia (新加坡)
kubernetes_version: 1.28.3

node_pools:
  system_pool:
    vm_size: Standard_D4s_v3
    node_count: 3
    availability_zones: [1, 2, 3]

  application_pool:
    vm_size: Standard_D8s_v3
    node_count: 6
    auto_scaling:
      enabled: true
      min_count: 3
      max_count: 15
    availability_zones: [1, 2, 3]

  stateful_pool:
    vm_size: Standard_D16s_v3
    node_count: 5
    availability_zones: [1, 2, 3]
    labels:
      workload: stateful

networking:
  network_plugin: azure
  network_policy: calico
  service_cidr: 10.0.0.0/16
  dns_service_ip: 10.0.0.10
  pod_cidr: 10.244.0.0/16

high_availability:
  multi_region: true
  secondary_region: Southeast Asia
  replication_type: active-passive
  failover_rto: 5min
  failover_rpo: 1min

addons:
  monitoring: true
  azure_policy: true
  azure_defender: true
  backup: true
```

### Terraform 配置 (AKS 集群創建)

```hcl
# terraform/aks/main.tf

terraform {
  required_version = ">= 1.6.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.45.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "terraform-state-rg"
    storage_account_name = "tfstateskagentic"
    container_name       = "tfstate"
    key                  = "aks-cluster.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = true
    }
  }
}

# Resource Group
resource "azurerm_resource_group" "aks" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    Environment = var.environment
    Project     = "Semantic-Kernel-Agentic"
    ManagedBy   = "Terraform"
  }
}

# Virtual Network
resource "azurerm_virtual_network" "aks" {
  name                = "${var.cluster_name}-vnet"
  location            = azurerm_resource_group.aks.location
  resource_group_name = azurerm_resource_group.aks.name
  address_space       = ["10.0.0.0/8"]

  tags = azurerm_resource_group.aks.tags
}

# Subnet for AKS
resource "azurerm_subnet" "aks" {
  name                 = "${var.cluster_name}-subnet"
  resource_group_name  = azurerm_resource_group.aks.name
  virtual_network_name = azurerm_virtual_network.aks.name
  address_prefixes     = ["10.240.0.0/16"]
}

# AKS Cluster
resource "azurerm_kubernetes_cluster" "main" {
  name                = var.cluster_name
  location            = azurerm_resource_group.aks.location
  resource_group_name = azurerm_resource_group.aks.name
  dns_prefix          = var.cluster_name
  kubernetes_version  = var.kubernetes_version

  # System Node Pool
  default_node_pool {
    name                = "system"
    vm_size             = var.system_node_vm_size
    node_count          = var.system_node_count
    availability_zones  = var.availability_zones
    vnet_subnet_id      = azurerm_subnet.aks.id
    os_disk_size_gb     = 128
    max_pods            = 30

    node_labels = {
      "nodepool-type" = "system"
      "environment"   = var.environment
    }

    tags = azurerm_resource_group.aks.tags
  }

  # Identity
  identity {
    type = "SystemAssigned"
  }

  # Network Profile
  network_profile {
    network_plugin     = "azure"
    network_policy     = "calico"
    service_cidr       = "10.0.0.0/16"
    dns_service_ip     = "10.0.0.10"
    outbound_type      = "loadBalancer"
    load_balancer_sku  = "standard"
  }

  # Azure Active Directory Integration
  azure_active_directory_role_based_access_control {
    managed            = true
    azure_rbac_enabled = true
  }

  # Add-ons
  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.aks.id
  }

  azure_policy_enabled = true

  # Auto-upgrade
  automatic_channel_upgrade = "stable"

  # Maintenance Window
  maintenance_window {
    allowed {
      day   = "Sunday"
      hours = [2, 3, 4]
    }
  }

  tags = azurerm_resource_group.aks.tags
}

# Application Node Pool
resource "azurerm_kubernetes_cluster_node_pool" "application" {
  name                  = "application"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = var.app_node_vm_size
  node_count            = var.app_node_min_count
  availability_zones    = var.availability_zones
  vnet_subnet_id        = azurerm_subnet.aks.id

  enable_auto_scaling = true
  min_count           = var.app_node_min_count
  max_count           = var.app_node_max_count

  os_disk_size_gb = 256
  max_pods        = 50

  node_labels = {
    "nodepool-type" = "application"
    "environment"   = var.environment
  }

  node_taints = []

  tags = azurerm_resource_group.aks.tags
}

# Stateful Node Pool
resource "azurerm_kubernetes_cluster_node_pool" "stateful" {
  name                  = "stateful"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = var.stateful_node_vm_size
  node_count            = var.stateful_node_count
  availability_zones    = var.availability_zones
  vnet_subnet_id        = azurerm_subnet.aks.id

  enable_auto_scaling = false

  os_disk_size_gb = 512
  max_pods        = 30

  node_labels = {
    "nodepool-type" = "stateful"
    "workload"      = "database"
    "environment"   = var.environment
  }

  node_taints = [
    "workload=stateful:NoSchedule"
  ]

  tags = azurerm_resource_group.aks.tags
}

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "aks" {
  name                = "${var.cluster_name}-logs"
  location            = azurerm_resource_group.aks.location
  resource_group_name = azurerm_resource_group.aks.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = azurerm_resource_group.aks.tags
}

# Container Insights Solution
resource "azurerm_log_analytics_solution" "container_insights" {
  solution_name         = "ContainerInsights"
  location              = azurerm_resource_group.aks.location
  resource_group_name   = azurerm_resource_group.aks.name
  workspace_resource_id = azurerm_log_analytics_workspace.aks.id
  workspace_name        = azurerm_log_analytics_workspace.aks.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }
}

# Outputs
output "cluster_name" {
  value = azurerm_kubernetes_cluster.main.name
}

output "kube_config" {
  value     = azurerm_kubernetes_cluster.main.kube_config_raw
  sensitive = true
}

output "cluster_identity_principal_id" {
  value = azurerm_kubernetes_cluster.main.identity[0].principal_id
}
```

### 變數定義

```hcl
# terraform/aks/variables.tf

variable "environment" {
  description = "Environment name (dev/staging/prod)"
  type        = string

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastasia"
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "cluster_name" {
  description = "AKS cluster name"
  type        = string
}

variable "kubernetes_version" {
  description = "Kubernetes version"
  type        = string
  default     = "1.28.3"
}

variable "availability_zones" {
  description = "Availability zones for node pools"
  type        = list(string)
  default     = ["1", "2", "3"]
}

# System Node Pool
variable "system_node_vm_size" {
  description = "VM size for system node pool"
  type        = string
  default     = "Standard_D2s_v3"
}

variable "system_node_count" {
  description = "Node count for system node pool"
  type        = number
  default     = 3
}

# Application Node Pool
variable "app_node_vm_size" {
  description = "VM size for application node pool"
  type        = string
  default     = "Standard_D4s_v3"
}

variable "app_node_min_count" {
  description = "Minimum node count for application node pool"
  type        = number
  default     = 3
}

variable "app_node_max_count" {
  description = "Maximum node count for application node pool"
  type        = number
  default     = 10
}

# Stateful Node Pool
variable "stateful_node_vm_size" {
  description = "VM size for stateful node pool"
  type        = string
  default     = "Standard_D8s_v3"
}

variable "stateful_node_count" {
  description = "Node count for stateful node pool"
  type        = number
  default     = 3
}
```

### 環境配置文件

**開發環境 (dev.tfvars)**:

```hcl
# terraform/aks/environments/dev.tfvars

environment         = "dev"
location            = "eastasia"
resource_group_name = "sk-agentic-dev-rg"
cluster_name        = "sk-agentic-dev"
kubernetes_version  = "1.28.3"

# System Node Pool
system_node_vm_size = "Standard_D2s_v3"
system_node_count   = 2

# Application Node Pool
app_node_vm_size    = "Standard_D4s_v3"
app_node_min_count  = 2
app_node_max_count  = 5

# Stateful Node Pool
stateful_node_vm_size = "Standard_D8s_v3"
stateful_node_count   = 3

availability_zones = ["1", "2", "3"]
```

**生產環境 (prod.tfvars)**:

```hcl
# terraform/aks/environments/prod.tfvars

environment         = "prod"
location            = "eastasia"
resource_group_name = "sk-agentic-prod-rg"
cluster_name        = "sk-agentic-prod"
kubernetes_version  = "1.28.3"

# System Node Pool
system_node_vm_size = "Standard_D4s_v3"
system_node_count   = 3

# Application Node Pool
app_node_vm_size    = "Standard_D8s_v3"
app_node_min_count  = 3
app_node_max_count  = 15

# Stateful Node Pool
stateful_node_vm_size = "Standard_D16s_v3"
stateful_node_count   = 5

availability_zones = ["1", "2", "3"]
```

---

## 命名空間設計

### 命名空間架構

```yaml
# k8s/base/namespaces.yaml

---
# 系統命名空間 (Ingress, Cert-Manager, Monitoring)
apiVersion: v1
kind: Namespace
metadata:
  name: ingress-nginx
  labels:
    name: ingress-nginx
    environment: shared

---
apiVersion: v1
kind: Namespace
metadata:
  name: cert-manager
  labels:
    name: cert-manager
    environment: shared

---
apiVersion: v1
kind: Namespace
metadata:
  name: monitoring
  labels:
    name: monitoring
    environment: shared

---
# 應用命名空間 (Dev)
apiVersion: v1
kind: Namespace
metadata:
  name: sk-agentic-dev
  labels:
    name: sk-agentic-dev
    environment: dev
    istio-injection: enabled

---
# 應用命名空間 (Staging)
apiVersion: v1
kind: Namespace
metadata:
  name: sk-agentic-staging
  labels:
    name: sk-agentic-staging
    environment: staging
    istio-injection: enabled

---
# 應用命名空間 (Prod)
apiVersion: v1
kind: Namespace
metadata:
  name: sk-agentic-prod
  labels:
    name: sk-agentic-prod
    environment: prod
    istio-injection: enabled

---
# 數據命名空間 (Dev)
apiVersion: v1
kind: Namespace
metadata:
  name: sk-agentic-data-dev
  labels:
    name: sk-agentic-data-dev
    environment: dev
    workload: stateful

---
# 數據命名空間 (Prod)
apiVersion: v1
kind: Namespace
metadata:
  name: sk-agentic-data-prod
  labels:
    name: sk-agentic-data-prod
    environment: prod
    workload: stateful
```

### 資源配額 (ResourceQuota)

**開發環境配額**:

```yaml
# k8s/overlays/dev/resource-quota.yaml

apiVersion: v1
kind: ResourceQuota
metadata:
  name: dev-quota
  namespace: sk-agentic-dev
spec:
  hard:
    requests.cpu: "20"
    requests.memory: 40Gi
    limits.cpu: "40"
    limits.memory: 80Gi
    persistentvolumeclaims: "10"
    services.loadbalancers: "2"
```

**生產環境配額**:

```yaml
# k8s/overlays/prod/resource-quota.yaml

apiVersion: v1
kind: ResourceQuota
metadata:
  name: prod-quota
  namespace: sk-agentic-prod
spec:
  hard:
    requests.cpu: "100"
    requests.memory: 200Gi
    limits.cpu: "200"
    limits.memory: 400Gi
    persistentvolumeclaims: "50"
    services.loadbalancers: "5"
```

### 限制範圍 (LimitRange)

```yaml
# k8s/base/limit-range.yaml

apiVersion: v1
kind: LimitRange
metadata:
  name: resource-limits
spec:
  limits:
  # Container Limits
  - type: Container
    default:
      cpu: 500m
      memory: 512Mi
    defaultRequest:
      cpu: 100m
      memory: 128Mi
    max:
      cpu: "2"
      memory: 4Gi
    min:
      cpu: 50m
      memory: 64Mi

  # Pod Limits
  - type: Pod
    max:
      cpu: "4"
      memory: 8Gi
    min:
      cpu: 100m
      memory: 128Mi

  # PVC Limits
  - type: PersistentVolumeClaim
    max:
      storage: 500Gi
    min:
      storage: 1Gi
```

---

## ConfigMap 和 Secret 管理

### ConfigMap 配置

**API Gateway ConfigMap**:

```yaml
# k8s/base/configmaps/api-gateway-config.yaml

apiVersion: v1
kind: ConfigMap
metadata:
  name: api-gateway-config
  namespace: sk-agentic-prod
data:
  appsettings.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "Kestrel": {
        "Endpoints": {
          "Http": {
            "Url": "http://0.0.0.0:5000"
          }
        }
      },
      "ReverseProxy": {
        "Routes": {
          "agent-route": {
            "ClusterId": "agent-cluster",
            "Match": {
              "Path": "/api/agents/{**catch-all}"
            }
          },
          "workflow-route": {
            "ClusterId": "workflow-cluster",
            "Match": {
              "Path": "/api/workflows/{**catch-all}"
            }
          }
        },
        "Clusters": {
          "agent-cluster": {
            "Destinations": {
              "destination1": {
                "Address": "http://agent-service:8001"
              }
            }
          },
          "workflow-cluster": {
            "Destinations": {
              "destination1": {
                "Address": "http://workflow-service:8002"
              }
            }
          }
        }
      }
    }
```

**PostgreSQL ConfigMap**:

```yaml
# k8s/base/configmaps/postgres-config.yaml

apiVersion: v1
kind: ConfigMap
metadata:
  name: postgres-config
  namespace: sk-agentic-data-prod
data:
  POSTGRES_DB: "sk_agentic_db"
  POSTGRES_MAX_CONNECTIONS: "200"
  POSTGRES_SHARED_BUFFERS: "2GB"
  POSTGRES_EFFECTIVE_CACHE_SIZE: "6GB"
  POSTGRES_WORK_MEM: "16MB"

  postgresql.conf: |
    # Connection Settings
    max_connections = 200
    superuser_reserved_connections = 3

    # Memory Settings
    shared_buffers = 2GB
    effective_cache_size = 6GB
    work_mem = 16MB
    maintenance_work_mem = 512MB

    # Write-Ahead Log
    wal_level = replica
    max_wal_senders = 5
    wal_keep_size = 1GB

    # Query Tuning
    random_page_cost = 1.1
    effective_io_concurrency = 200

    # Logging
    log_destination = 'stderr'
    logging_collector = on
    log_directory = 'log'
    log_filename = 'postgresql-%Y-%m-%d.log'
    log_rotation_age = 1d
    log_rotation_size = 100MB
    log_line_prefix = '%m [%p] %q%u@%d '
    log_timezone = 'Asia/Taipei'

  pg_hba.conf: |
    # TYPE  DATABASE        USER            ADDRESS                 METHOD
    local   all             all                                     trust
    host    all             all             127.0.0.1/32            trust
    host    all             all             ::1/128                 trust
    host    all             all             10.0.0.0/8              md5
    host    replication     replicator      10.0.0.0/8              md5
```

### Secret 管理

**數據庫密碼 Secret**:

```yaml
# k8s/base/secrets/database-secrets.yaml

apiVersion: v1
kind: Secret
metadata:
  name: database-secrets
  namespace: sk-agentic-prod
type: Opaque
stringData:
  POSTGRES_PASSWORD: "your-super-secure-password-here"
  POSTGRES_REPLICATION_PASSWORD: "replication-password-here"
  REDIS_PASSWORD: "redis-password-here"
  RABBITMQ_PASSWORD: "rabbitmq-password-here"
```

**Azure OpenAI Secret**:

```yaml
# k8s/base/secrets/azure-openai-secrets.yaml

apiVersion: v1
kind: Secret
metadata:
  name: azure-openai-secrets
  namespace: sk-agentic-prod
type: Opaque
stringData:
  AZURE_OPENAI_API_KEY: "your-azure-openai-api-key"
  AZURE_OPENAI_ENDPOINT: "https://your-resource.openai.azure.com/"
  AZURE_OPENAI_DEPLOYMENT_NAME: "gpt-4"
  AZURE_OPENAI_EMBEDDING_DEPLOYMENT: "text-embedding-ada-002"
```

**JWT Secret**:

```yaml
# k8s/base/secrets/jwt-secrets.yaml

apiVersion: v1
kind: Secret
metadata:
  name: jwt-secrets
  namespace: sk-agentic-prod
type: Opaque
stringData:
  JWT_SECRET_KEY: "your-256-bit-secret-key-here-change-in-production"
  JWT_ISSUER: "https://api.sk-agentic.com"
  JWT_AUDIENCE: "https://app.sk-agentic.com"
```

### 使用 Sealed Secrets (推薦)

**安裝 Sealed Secrets Controller**:

```bash
# 安裝 Sealed Secrets Controller
kubectl apply -f https://github.com/bitnami-labs/sealed-secrets/releases/download/v0.24.0/controller.yaml

# 安裝 kubeseal CLI
wget https://github.com/bitnami-labs/sealed-secrets/releases/download/v0.24.0/kubeseal-0.24.0-linux-amd64.tar.gz
tar -xvzf kubeseal-0.24.0-linux-amd64.tar.gz
sudo install -m 755 kubeseal /usr/local/bin/kubeseal
```

**創建 Sealed Secret**:

```bash
# 創建 Secret YAML (不提交到 Git)
kubectl create secret generic database-secrets \
  --namespace=sk-agentic-prod \
  --from-literal=POSTGRES_PASSWORD='your-password' \
  --dry-run=client -o yaml > database-secrets.yaml

# 使用 kubeseal 加密
kubeseal --format=yaml < database-secrets.yaml > database-sealed-secrets.yaml

# 現在可以安全地提交 database-sealed-secrets.yaml 到 Git
git add database-sealed-secrets.yaml
git commit -m "feat: add sealed database secrets"
```

**Sealed Secret 示例**:

```yaml
# k8s/base/secrets/database-sealed-secrets.yaml

apiVersion: bitnami.com/v1alpha1
kind: SealedSecret
metadata:
  name: database-secrets
  namespace: sk-agentic-prod
spec:
  encryptedData:
    POSTGRES_PASSWORD: AgBQ7Vn... (加密內容)
    REDIS_PASSWORD: AgCK9Xm... (加密內容)
  template:
    metadata:
      name: database-secrets
      namespace: sk-agentic-prod
    type: Opaque
```

---

## Deployment 配置

### API Gateway Deployment

```yaml
# k8s/base/deployments/api-gateway.yaml

apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-gateway
  namespace: sk-agentic-prod
  labels:
    app: api-gateway
    version: v1.0.0
    component: gateway
spec:
  replicas: 3
  revisionHistoryLimit: 5
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  selector:
    matchLabels:
      app: api-gateway
  template:
    metadata:
      labels:
        app: api-gateway
        version: v1.0.0
      annotations:
        prometheus.io/scrape: "true"
        prometheus.io/port: "5000"
        prometheus.io/path: "/metrics"
    spec:
      # Pod Anti-Affinity (不同節點)
      affinity:
        podAntiAffinity:
          preferredDuringSchedulingIgnoredDuringExecution:
          - weight: 100
            podAffinityTerm:
              labelSelector:
                matchExpressions:
                - key: app
                  operator: In
                  values:
                  - api-gateway
              topologyKey: kubernetes.io/hostname

      # Node Selector
      nodeSelector:
        nodepool-type: application

      # Tolerations
      tolerations:
      - key: "workload"
        operator: "Equal"
        value: "application"
        effect: "NoSchedule"

      # Security Context
      securityContext:
        runAsNonRoot: true
        runAsUser: 1000
        fsGroup: 1000

      # Init Container (等待依賴)
      initContainers:
      - name: wait-for-postgres
        image: busybox:1.36
        command:
        - sh
        - -c
        - |
          until nc -z postgres-service.sk-agentic-data-prod.svc.cluster.local 5432; do
            echo "Waiting for PostgreSQL..."
            sleep 2
          done

      # Application Container
      containers:
      - name: api-gateway
        image: skagenticacr.azurecr.io/api-gateway:1.0.0
        imagePullPolicy: IfNotPresent

        ports:
        - name: http
          containerPort: 5000
          protocol: TCP

        # Environment Variables
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:5000"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: database-connection-strings
              key: POSTGRES_CONNECTION_STRING
        - name: Redis__Configuration
          valueFrom:
            secretKeyRef:
              name: database-connection-strings
              key: REDIS_CONNECTION_STRING

        # Volume Mounts
        volumeMounts:
        - name: config
          mountPath: /app/appsettings.json
          subPath: appsettings.json
        - name: temp
          mountPath: /tmp

        # Resource Limits
        resources:
          requests:
            cpu: 200m
            memory: 256Mi
          limits:
            cpu: 1000m
            memory: 1Gi

        # Liveness Probe
        livenessProbe:
          httpGet:
            path: /health/live
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

        # Readiness Probe
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3

        # Startup Probe
        startupProbe:
          httpGet:
            path: /health/startup
            port: 5000
          initialDelaySeconds: 0
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 30

      # Volumes
      volumes:
      - name: config
        configMap:
          name: api-gateway-config
      - name: temp
        emptyDir: {}

      # Image Pull Secrets
      imagePullSecrets:
      - name: acr-secret
```

### Agent Service Deployment

```yaml
# k8s/base/deployments/agent-service.yaml

apiVersion: apps/v1
kind: Deployment
metadata:
  name: agent-service
  namespace: sk-agentic-prod
  labels:
    app: agent-service
    version: v1.0.0
    component: backend
spec:
  replicas: 5
  revisionHistoryLimit: 5
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 2
      maxUnavailable: 1
  selector:
    matchLabels:
      app: agent-service
  template:
    metadata:
      labels:
        app: agent-service
        version: v1.0.0
      annotations:
        prometheus.io/scrape: "true"
        prometheus.io/port: "8001"
        prometheus.io/path: "/metrics"
    spec:
      affinity:
        podAntiAffinity:
          preferredDuringSchedulingIgnoredDuringExecution:
          - weight: 100
            podAffinityTerm:
              labelSelector:
                matchExpressions:
                - key: app
                  operator: In
                  values:
                  - agent-service
              topologyKey: kubernetes.io/hostname

      nodeSelector:
        nodepool-type: application

      securityContext:
        runAsNonRoot: true
        runAsUser: 1000
        fsGroup: 1000

      containers:
      - name: agent-service
        image: skagenticacr.azurecr.io/agent-service:1.0.0
        imagePullPolicy: IfNotPresent

        ports:
        - name: http
          containerPort: 8001
          protocol: TCP

        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:8001"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: database-connection-strings
              key: POSTGRES_CONNECTION_STRING
        - name: SemanticKernel__AzureOpenAI__ApiKey
          valueFrom:
            secretKeyRef:
              name: azure-openai-secrets
              key: AZURE_OPENAI_API_KEY
        - name: SemanticKernel__AzureOpenAI__Endpoint
          valueFrom:
            secretKeyRef:
              name: azure-openai-secrets
              key: AZURE_OPENAI_ENDPOINT

        resources:
          requests:
            cpu: 500m
            memory: 1Gi
          limits:
            cpu: 2000m
            memory: 4Gi

        livenessProbe:
          httpGet:
            path: /health/live
            port: 8001
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8001
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3

      imagePullSecrets:
      - name: acr-secret
```

### Frontend Host Deployment (React)

```yaml
# k8s/base/deployments/frontend-host.yaml

apiVersion: apps/v1
kind: Deployment
metadata:
  name: frontend-host
  namespace: sk-agentic-prod
  labels:
    app: frontend-host
    version: v1.0.0
    component: frontend
spec:
  replicas: 3
  revisionHistoryLimit: 5
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  selector:
    matchLabels:
      app: frontend-host
  template:
    metadata:
      labels:
        app: frontend-host
        version: v1.0.0
    spec:
      nodeSelector:
        nodepool-type: application

      containers:
      - name: frontend-host
        image: skagenticacr.azurecr.io/frontend-host:1.0.0
        imagePullPolicy: IfNotPresent

        ports:
        - name: http
          containerPort: 3000
          protocol: TCP

        env:
        - name: VITE_API_BASE_URL
          value: "https://api.sk-agentic.com"
        - name: VITE_WORKFLOW_EDITOR_URL
          value: "https://app.sk-agentic.com/workflow-editor"

        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi

        livenessProbe:
          httpGet:
            path: /health
            port: 3000
          initialDelaySeconds: 10
          periodSeconds: 10
          timeoutSeconds: 3
          failureThreshold: 3

        readinessProbe:
          httpGet:
            path: /health
            port: 3000
          initialDelaySeconds: 5
          periodSeconds: 5
          timeoutSeconds: 2
          failureThreshold: 3

      imagePullSecrets:
      - name: acr-secret
```

### Frontend Remote Deployment (Vue)

```yaml
# k8s/base/deployments/frontend-remote.yaml

apiVersion: apps/v1
kind: Deployment
metadata:
  name: frontend-remote
  namespace: sk-agentic-prod
  labels:
    app: frontend-remote
    version: v1.0.0
    component: frontend
spec:
  replicas: 3
  revisionHistoryLimit: 5
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  selector:
    matchLabels:
      app: frontend-remote
  template:
    metadata:
      labels:
        app: frontend-remote
        version: v1.0.0
    spec:
      nodeSelector:
        nodepool-type: application

      containers:
      - name: frontend-remote
        image: skagenticacr.azurecr.io/frontend-remote:1.0.0
        imagePullPolicy: IfNotPresent

        ports:
        - name: http
          containerPort: 3001
          protocol: TCP

        env:
        - name: VITE_API_BASE_URL
          value: "https://api.sk-agentic.com"
        - name: VITE_WEBSOCKET_URL
          value: "wss://api.sk-agentic.com/ws"

        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi

        livenessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 10
          periodSeconds: 10
          timeoutSeconds: 3
          failureThreshold: 3

        readinessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 5
          periodSeconds: 5
          timeoutSeconds: 2
          failureThreshold: 3

      imagePullSecrets:
      - name: acr-secret
```

---

## StatefulSet 配置

### PostgreSQL StatefulSet

```yaml
# k8s/base/statefulsets/postgres.yaml

apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: postgres
  namespace: sk-agentic-data-prod
  labels:
    app: postgres
    version: "16"
spec:
  serviceName: postgres-headless
  replicas: 3
  selector:
    matchLabels:
      app: postgres
  template:
    metadata:
      labels:
        app: postgres
        version: "16"
    spec:
      # Node Selector for Stateful Workload
      nodeSelector:
        nodepool-type: stateful
        workload: database

      # Tolerations
      tolerations:
      - key: "workload"
        operator: "Equal"
        value: "stateful"
        effect: "NoSchedule"

      # Pod Anti-Affinity (強制不同可用區)
      affinity:
        podAntiAffinity:
          requiredDuringSchedulingIgnoredDuringExecution:
          - labelSelector:
              matchExpressions:
              - key: app
                operator: In
                values:
                - postgres
            topologyKey: topology.kubernetes.io/zone

      # Security Context
      securityContext:
        fsGroup: 999
        runAsUser: 999
        runAsNonRoot: true

      # Init Container (初始化數據目錄權限)
      initContainers:
      - name: init-chmod-data
        image: postgres:16-alpine
        command:
        - sh
        - -c
        - |
          chown -R 999:999 /var/lib/postgresql/data
          chmod 700 /var/lib/postgresql/data
        volumeMounts:
        - name: postgres-data
          mountPath: /var/lib/postgresql/data

      # PostgreSQL Container
      containers:
      - name: postgres
        image: postgres:16-alpine
        imagePullPolicy: IfNotPresent

        ports:
        - name: postgres
          containerPort: 5432
          protocol: TCP

        # Environment Variables
        env:
        - name: POSTGRES_DB
          valueFrom:
            configMapKeyRef:
              name: postgres-config
              key: POSTGRES_DB
        - name: POSTGRES_USER
          value: "postgres"
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: POSTGRES_PASSWORD
        - name: PGDATA
          value: /var/lib/postgresql/data/pgdata
        - name: POD_IP
          valueFrom:
            fieldRef:
              fieldPath: status.podIP

        # Volume Mounts
        volumeMounts:
        - name: postgres-data
          mountPath: /var/lib/postgresql/data
        - name: postgres-config-volume
          mountPath: /etc/postgresql/postgresql.conf
          subPath: postgresql.conf
        - name: postgres-config-volume
          mountPath: /etc/postgresql/pg_hba.conf
          subPath: pg_hba.conf
        - name: init-scripts
          mountPath: /docker-entrypoint-initdb.d

        # Resource Limits
        resources:
          requests:
            cpu: 1000m
            memory: 2Gi
          limits:
            cpu: 4000m
            memory: 8Gi

        # Liveness Probe
        livenessProbe:
          exec:
            command:
            - /bin/sh
            - -c
            - pg_isready -U postgres -h 127.0.0.1
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

        # Readiness Probe
        readinessProbe:
          exec:
            command:
            - /bin/sh
            - -c
            - pg_isready -U postgres -h 127.0.0.1
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3

      # Volumes
      volumes:
      - name: postgres-config-volume
        configMap:
          name: postgres-config
      - name: init-scripts
        configMap:
          name: postgres-init-scripts

  # Volume Claim Templates
  volumeClaimTemplates:
  - metadata:
      name: postgres-data
    spec:
      accessModes: [ "ReadWriteOnce" ]
      storageClassName: managed-premium-retain
      resources:
        requests:
          storage: 100Gi
```

### Redis StatefulSet

```yaml
# k8s/base/statefulsets/redis.yaml

apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: redis
  namespace: sk-agentic-data-prod
  labels:
    app: redis
    version: "7"
spec:
  serviceName: redis-headless
  replicas: 3
  selector:
    matchLabels:
      app: redis
  template:
    metadata:
      labels:
        app: redis
        version: "7"
    spec:
      nodeSelector:
        nodepool-type: stateful
        workload: database

      tolerations:
      - key: "workload"
        operator: "Equal"
        value: "stateful"
        effect: "NoSchedule"

      affinity:
        podAntiAffinity:
          requiredDuringSchedulingIgnoredDuringExecution:
          - labelSelector:
              matchExpressions:
              - key: app
                operator: In
                values:
                - redis
            topologyKey: topology.kubernetes.io/zone

      securityContext:
        fsGroup: 999
        runAsUser: 999
        runAsNonRoot: true

      containers:
      - name: redis
        image: redis:7-alpine
        imagePullPolicy: IfNotPresent

        command:
        - redis-server
        - /etc/redis/redis.conf
        - --requirepass
        - $(REDIS_PASSWORD)

        ports:
        - name: redis
          containerPort: 6379
          protocol: TCP

        env:
        - name: REDIS_PASSWORD
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: REDIS_PASSWORD

        volumeMounts:
        - name: redis-data
          mountPath: /data
        - name: redis-config
          mountPath: /etc/redis/redis.conf
          subPath: redis.conf

        resources:
          requests:
            cpu: 500m
            memory: 1Gi
          limits:
            cpu: 2000m
            memory: 4Gi

        livenessProbe:
          exec:
            command:
            - redis-cli
            - --no-auth-warning
            - -a
            - $(REDIS_PASSWORD)
            - ping
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

        readinessProbe:
          exec:
            command:
            - redis-cli
            - --no-auth-warning
            - -a
            - $(REDIS_PASSWORD)
            - ping
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3

      volumes:
      - name: redis-config
        configMap:
          name: redis-config

  volumeClaimTemplates:
  - metadata:
      name: redis-data
    spec:
      accessModes: [ "ReadWriteOnce" ]
      storageClassName: managed-premium-retain
      resources:
        requests:
          storage: 20Gi
```

### RabbitMQ StatefulSet

```yaml
# k8s/base/statefulsets/rabbitmq.yaml

apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: rabbitmq
  namespace: sk-agentic-data-prod
  labels:
    app: rabbitmq
    version: "3.12"
spec:
  serviceName: rabbitmq-headless
  replicas: 3
  selector:
    matchLabels:
      app: rabbitmq
  template:
    metadata:
      labels:
        app: rabbitmq
        version: "3.12"
    spec:
      nodeSelector:
        nodepool-type: stateful
        workload: database

      tolerations:
      - key: "workload"
        operator: "Equal"
        value: "stateful"
        effect: "NoSchedule"

      affinity:
        podAntiAffinity:
          requiredDuringSchedulingIgnoredDuringExecution:
          - labelSelector:
              matchExpressions:
              - key: app
                operator: In
                values:
                - rabbitmq
            topologyKey: topology.kubernetes.io/zone

      securityContext:
        fsGroup: 999
        runAsUser: 999
        runAsNonRoot: true

      containers:
      - name: rabbitmq
        image: rabbitmq:3.12-management-alpine
        imagePullPolicy: IfNotPresent

        ports:
        - name: amqp
          containerPort: 5672
          protocol: TCP
        - name: management
          containerPort: 15672
          protocol: TCP
        - name: prometheus
          containerPort: 15692
          protocol: TCP

        env:
        - name: RABBITMQ_DEFAULT_USER
          value: "admin"
        - name: RABBITMQ_DEFAULT_PASS
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: RABBITMQ_PASSWORD
        - name: RABBITMQ_ERLANG_COOKIE
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: RABBITMQ_ERLANG_COOKIE
        - name: RABBITMQ_NODENAME
          value: "rabbit@$(POD_NAME).rabbitmq-headless.sk-agentic-data-prod.svc.cluster.local"
        - name: POD_NAME
          valueFrom:
            fieldRef:
              fieldPath: metadata.name

        volumeMounts:
        - name: rabbitmq-data
          mountPath: /var/lib/rabbitmq
        - name: rabbitmq-config
          mountPath: /etc/rabbitmq/rabbitmq.conf
          subPath: rabbitmq.conf

        resources:
          requests:
            cpu: 500m
            memory: 1Gi
          limits:
            cpu: 2000m
            memory: 4Gi

        livenessProbe:
          exec:
            command:
            - rabbitmq-diagnostics
            - -q
            - ping
          initialDelaySeconds: 60
          periodSeconds: 30
          timeoutSeconds: 10
          failureThreshold: 3

        readinessProbe:
          exec:
            command:
            - rabbitmq-diagnostics
            - -q
            - check_running
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

      volumes:
      - name: rabbitmq-config
        configMap:
          name: rabbitmq-config

  volumeClaimTemplates:
  - metadata:
      name: rabbitmq-data
    spec:
      accessModes: [ "ReadWriteOnce" ]
      storageClassName: managed-premium-retain
      resources:
        requests:
          storage: 20Gi
```

---

## Service 配置

### API Gateway Service (LoadBalancer)

```yaml
# k8s/base/services/api-gateway-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: api-gateway-service
  namespace: sk-agentic-prod
  labels:
    app: api-gateway
  annotations:
    service.beta.kubernetes.io/azure-load-balancer-internal: "false"
    service.beta.kubernetes.io/azure-dns-label-name: "sk-agentic-api"
spec:
  type: LoadBalancer
  sessionAffinity: ClientIP
  sessionAffinityConfig:
    clientIP:
      timeoutSeconds: 10800
  ports:
  - name: http
    port: 80
    targetPort: 5000
    protocol: TCP
  - name: https
    port: 443
    targetPort: 5000
    protocol: TCP
  selector:
    app: api-gateway
```

### Agent Service (ClusterIP)

```yaml
# k8s/base/services/agent-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: agent-service
  namespace: sk-agentic-prod
  labels:
    app: agent-service
spec:
  type: ClusterIP
  ports:
  - name: http
    port: 8001
    targetPort: 8001
    protocol: TCP
  selector:
    app: agent-service
```

### PostgreSQL Service (ClusterIP)

```yaml
# k8s/base/services/postgres-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: postgres-service
  namespace: sk-agentic-data-prod
  labels:
    app: postgres
spec:
  type: ClusterIP
  ports:
  - name: postgres
    port: 5432
    targetPort: 5432
    protocol: TCP
  selector:
    app: postgres
```

### PostgreSQL Headless Service (StatefulSet)

```yaml
# k8s/base/services/postgres-headless.yaml

apiVersion: v1
kind: Service
metadata:
  name: postgres-headless
  namespace: sk-agentic-data-prod
  labels:
    app: postgres
spec:
  type: ClusterIP
  clusterIP: None
  ports:
  - name: postgres
    port: 5432
    targetPort: 5432
    protocol: TCP
  selector:
    app: postgres
```

### Redis Service (ClusterIP)

```yaml
# k8s/base/services/redis-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: redis-service
  namespace: sk-agentic-data-prod
  labels:
    app: redis
spec:
  type: ClusterIP
  ports:
  - name: redis
    port: 6379
    targetPort: 6379
    protocol: TCP
  selector:
    app: redis
```

### Frontend Host Service (ClusterIP)

```yaml
# k8s/base/services/frontend-host-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: frontend-host-service
  namespace: sk-agentic-prod
  labels:
    app: frontend-host
spec:
  type: ClusterIP
  ports:
  - name: http
    port: 3000
    targetPort: 3000
    protocol: TCP
  selector:
    app: frontend-host
```

### Frontend Remote Service (ClusterIP)

```yaml
# k8s/base/services/frontend-remote-service.yaml

apiVersion: v1
kind: Service
metadata:
  name: frontend-remote-service
  namespace: sk-agentic-prod
  labels:
    app: frontend-remote
spec:
  type: ClusterIP
  ports:
  - name: http
    port: 3001
    targetPort: 3001
    protocol: TCP
  selector:
    app: frontend-remote
```

---

## Ingress 配置

### 安裝 NGINX Ingress Controller

```bash
# 添加 Helm Repository
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

# 安裝 NGINX Ingress Controller
helm install ingress-nginx ingress-nginx/ingress-nginx \
  --namespace ingress-nginx \
  --create-namespace \
  --set controller.replicaCount=3 \
  --set controller.nodeSelector."nodepool-type"=application \
  --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz \
  --set controller.metrics.enabled=true \
  --set controller.metrics.serviceMonitor.enabled=true
```

### 安裝 cert-manager (自動 HTTPS 證書)

```bash
# 添加 Helm Repository
helm repo add jetstack https://charts.jetstack.io
helm repo update

# 安裝 cert-manager
helm install cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --create-namespace \
  --version v1.13.2 \
  --set installCRDs=true \
  --set nodeSelector."nodepool-type"=system
```

### ClusterIssuer (Let's Encrypt)

```yaml
# k8s/base/cert-manager/cluster-issuer.yaml

---
# Let's Encrypt Staging (測試用)
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-staging
spec:
  acme:
    server: https://acme-staging-v02.api.letsencrypt.org/directory
    email: admin@sk-agentic.com
    privateKeySecretRef:
      name: letsencrypt-staging
    solvers:
    - http01:
        ingress:
          class: nginx

---
# Let's Encrypt Production
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: admin@sk-agentic.com
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx
```

### Ingress 配置 (主應用)

```yaml
# k8s/base/ingress/main-ingress.yaml

apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: main-ingress
  namespace: sk-agentic-prod
  annotations:
    # NGINX Ingress Controller
    kubernetes.io/ingress.class: nginx

    # cert-manager (自動 HTTPS)
    cert-manager.io/cluster-issuer: letsencrypt-prod

    # NGINX 配置
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/force-ssl-redirect: "true"
    nginx.ingress.kubernetes.io/proxy-body-size: "50m"
    nginx.ingress.kubernetes.io/proxy-read-timeout: "300"
    nginx.ingress.kubernetes.io/proxy-send-timeout: "300"

    # CORS
    nginx.ingress.kubernetes.io/enable-cors: "true"
    nginx.ingress.kubernetes.io/cors-allow-origin: "https://app.sk-agentic.com"
    nginx.ingress.kubernetes.io/cors-allow-methods: "GET, POST, PUT, DELETE, OPTIONS"
    nginx.ingress.kubernetes.io/cors-allow-headers: "DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Authorization"

    # Rate Limiting
    nginx.ingress.kubernetes.io/limit-rps: "100"

    # Connection Limits
    nginx.ingress.kubernetes.io/limit-connections: "50"

spec:
  tls:
  - hosts:
    - app.sk-agentic.com
    - api.sk-agentic.com
    secretName: sk-agentic-tls

  rules:
  # Frontend Host (React)
  - host: app.sk-agentic.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: frontend-host-service
            port:
              number: 3000

  # API Gateway
  - host: api.sk-agentic.com
    http:
      paths:
      - path: /api
        pathType: Prefix
        backend:
          service:
            name: api-gateway-service
            port:
              number: 5000
      - path: /health
        pathType: Prefix
        backend:
          service:
            name: api-gateway-service
            port:
              number: 5000
```

### Ingress 配置 (工作流編輯器)

```yaml
# k8s/base/ingress/workflow-editor-ingress.yaml

apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: workflow-editor-ingress
  namespace: sk-agentic-prod
  annotations:
    kubernetes.io/ingress.class: nginx
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/force-ssl-redirect: "true"

    # WebSocket Support
    nginx.ingress.kubernetes.io/websocket-services: "frontend-remote-service"
    nginx.ingress.kubernetes.io/proxy-read-timeout: "3600"
    nginx.ingress.kubernetes.io/proxy-send-timeout: "3600"

    # CORS
    nginx.ingress.kubernetes.io/enable-cors: "true"
    nginx.ingress.kubernetes.io/cors-allow-origin: "https://app.sk-agentic.com"

spec:
  tls:
  - hosts:
    - app.sk-agentic.com
    secretName: sk-agentic-tls

  rules:
  - host: app.sk-agentic.com
    http:
      paths:
      # Workflow Editor Remote Module
      - path: /workflow-editor
        pathType: Prefix
        backend:
          service:
            name: frontend-remote-service
            port:
              number: 3001

      # WebSocket for CRDT Collaboration
      - path: /ws
        pathType: Prefix
        backend:
          service:
            name: frontend-remote-service
            port:
              number: 3001
```

---

## 持久化存儲 (PV/PVC)

### StorageClass 配置

**Azure Disk (Premium SSD)**:

```yaml
# k8s/base/storage/storageclass-premium.yaml

apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: managed-premium-retain
provisioner: disk.csi.azure.com
parameters:
  skuName: Premium_LRS
  kind: Managed
  cachingMode: ReadWrite
reclaimPolicy: Retain
volumeBindingMode: WaitForFirstConsumer
allowVolumeExpansion: true
```

**Azure Disk (Standard SSD)**:

```yaml
# k8s/base/storage/storageclass-standard.yaml

apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: managed-standard-retain
provisioner: disk.csi.azure.com
parameters:
  skuName: StandardSSD_LRS
  kind: Managed
  cachingMode: ReadWrite
reclaimPolicy: Retain
volumeBindingMode: WaitForFirstConsumer
allowVolumeExpansion: true
```

**Azure Files (共享存儲)**:

```yaml
# k8s/base/storage/storageclass-azurefile.yaml

apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: azurefile-csi-premium
provisioner: file.csi.azure.com
parameters:
  skuName: Premium_LRS
  protocol: nfs
reclaimPolicy: Retain
volumeBindingMode: Immediate
allowVolumeExpansion: true
mountOptions:
  - dir_mode=0777
  - file_mode=0777
  - uid=1000
  - gid=1000
```

### PersistentVolume 配置 (手動創建)

**PostgreSQL Backup Volume**:

```yaml
# k8s/base/storage/postgres-backup-pv.yaml

apiVersion: v1
kind: PersistentVolume
metadata:
  name: postgres-backup-pv
  labels:
    app: postgres
    type: backup
spec:
  capacity:
    storage: 500Gi
  volumeMode: Filesystem
  accessModes:
  - ReadWriteMany
  persistentVolumeReclaimPolicy: Retain
  storageClassName: azurefile-csi-premium
  mountOptions:
  - dir_mode=0777
  - file_mode=0777
  csi:
    driver: file.csi.azure.com
    volumeHandle: postgres-backup-volume-id
    volumeAttributes:
      resourceGroup: sk-agentic-prod-rg
      shareName: postgres-backups
```

### PersistentVolumeClaim 配置

**應用日誌 PVC**:

```yaml
# k8s/base/storage/app-logs-pvc.yaml

apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: app-logs-pvc
  namespace: sk-agentic-prod
spec:
  accessModes:
  - ReadWriteMany
  storageClassName: azurefile-csi-premium
  resources:
    requests:
      storage: 100Gi
```

---

## 自動擴展 (HPA)

### API Gateway HPA

```yaml
# k8s/base/hpa/api-gateway-hpa.yaml

apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-gateway-hpa
  namespace: sk-agentic-prod
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-gateway
  minReplicas: 3
  maxReplicas: 10
  metrics:
  # CPU 使用率
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  # Memory 使用率
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
  # 自定義指標 (每秒請求數)
  - type: Pods
    pods:
      metric:
        name: http_requests_per_second
      target:
        type: AverageValue
        averageValue: "1000"
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Percent
        value: 50
        periodSeconds: 60
      - type: Pods
        value: 2
        periodSeconds: 60
      selectPolicy: Min
    scaleUp:
      stabilizationWindowSeconds: 0
      policies:
      - type: Percent
        value: 100
        periodSeconds: 30
      - type: Pods
        value: 4
        periodSeconds: 30
      selectPolicy: Max
```

### Agent Service HPA

```yaml
# k8s/base/hpa/agent-service-hpa.yaml

apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: agent-service-hpa
  namespace: sk-agentic-prod
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: agent-service
  minReplicas: 5
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 75
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 85
  - type: Pods
    pods:
      metric:
        name: active_agent_sessions
      target:
        type: AverageValue
        averageValue: "50"
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 600
      policies:
      - type: Percent
        value: 25
        periodSeconds: 120
      selectPolicy: Min
    scaleUp:
      stabilizationWindowSeconds: 0
      policies:
      - type: Percent
        value: 200
        periodSeconds: 60
      selectPolicy: Max
```

### Frontend Host HPA

```yaml
# k8s/base/hpa/frontend-host-hpa.yaml

apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: frontend-host-hpa
  namespace: sk-agentic-prod
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: frontend-host
  minReplicas: 3
  maxReplicas: 15
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 60
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 70
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Pods
        value: 1
        periodSeconds: 60
      selectPolicy: Min
    scaleUp:
      stabilizationWindowSeconds: 0
      policies:
      - type: Pods
        value: 2
        periodSeconds: 30
      selectPolicy: Max
```

---

## 網絡策略 (NetworkPolicy)

### 默認拒絕所有流量

```yaml
# k8s/base/network-policies/default-deny-all.yaml

apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny-all
  namespace: sk-agentic-prod
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  - Egress
```

### 允許 API Gateway 訪問後端服務

```yaml
# k8s/base/network-policies/api-gateway-policy.yaml

apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: api-gateway-policy
  namespace: sk-agentic-prod
spec:
  podSelector:
    matchLabels:
      app: api-gateway
  policyTypes:
  - Ingress
  - Egress
  ingress:
  # 允許從 Ingress Controller 進入
  - from:
    - namespaceSelector:
        matchLabels:
          name: ingress-nginx
    ports:
    - protocol: TCP
      port: 5000
  egress:
  # 允許訪問 Agent Service
  - to:
    - podSelector:
        matchLabels:
          app: agent-service
    ports:
    - protocol: TCP
      port: 8001
  # 允許訪問 Workflow Service
  - to:
    - podSelector:
        matchLabels:
          app: workflow-service
    ports:
    - protocol: TCP
      port: 8002
  # 允許訪問數據層 (PostgreSQL, Redis)
  - to:
    - namespaceSelector:
        matchLabels:
          name: sk-agentic-data-prod
    - podSelector:
        matchLabels:
          app: postgres
    ports:
    - protocol: TCP
      port: 5432
  - to:
    - namespaceSelector:
        matchLabels:
          name: sk-agentic-data-prod
    - podSelector:
        matchLabels:
          app: redis
    ports:
    - protocol: TCP
      port: 6379
  # 允許 DNS 查詢
  - to:
    - namespaceSelector: {}
      podSelector:
        matchLabels:
          k8s-app: kube-dns
    ports:
    - protocol: UDP
      port: 53
```

### 允許 Agent Service 訪問數據庫

```yaml
# k8s/base/network-policies/agent-service-policy.yaml

apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: agent-service-policy
  namespace: sk-agentic-prod
spec:
  podSelector:
    matchLabels:
      app: agent-service
  policyTypes:
  - Ingress
  - Egress
  ingress:
  # 只允許從 API Gateway 進入
  - from:
    - podSelector:
        matchLabels:
          app: api-gateway
    ports:
    - protocol: TCP
      port: 8001
  egress:
  # 允許訪問 PostgreSQL
  - to:
    - namespaceSelector:
        matchLabels:
          name: sk-agentic-data-prod
    - podSelector:
        matchLabels:
          app: postgres
    ports:
    - protocol: TCP
      port: 5432
  # 允許訪問 Redis
  - to:
    - namespaceSelector:
        matchLabels:
          name: sk-agentic-data-prod
    - podSelector:
        matchLabels:
          app: redis
    ports:
    - protocol: TCP
      port: 6379
  # 允許訪問 LLM Service
  - to:
    - podSelector:
        matchLabels:
          app: llm-service
    ports:
    - protocol: TCP
      port: 8004
  # 允許 DNS
  - to:
    - namespaceSelector: {}
      podSelector:
        matchLabels:
          k8s-app: kube-dns
    ports:
    - protocol: UDP
      port: 53
  # 允許 HTTPS 出站 (Azure OpenAI API)
  - to:
    - namespaceSelector: {}
    ports:
    - protocol: TCP
      port: 443
```

### 數據庫隔離策略

```yaml
# k8s/base/network-policies/database-isolation-policy.yaml

apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: database-isolation-policy
  namespace: sk-agentic-data-prod
spec:
  podSelector:
    matchLabels:
      app: postgres
  policyTypes:
  - Ingress
  - Egress
  ingress:
  # 只允許應用命名空間的特定服務訪問
  - from:
    - namespaceSelector:
        matchLabels:
          name: sk-agentic-prod
    - podSelector:
        matchLabels:
          component: backend
    ports:
    - protocol: TCP
      port: 5432
  # 允許同一 StatefulSet 內的 Pod 互相通信 (複製)
  - from:
    - podSelector:
        matchLabels:
          app: postgres
    ports:
    - protocol: TCP
      port: 5432
  egress:
  # 允許同一 StatefulSet 內的 Pod 互相通信
  - to:
    - podSelector:
        matchLabels:
          app: postgres
    ports:
    - protocol: TCP
      port: 5432
  # 允許 DNS
  - to:
    - namespaceSelector: {}
      podSelector:
        matchLabels:
          k8s-app: kube-dns
    ports:
    - protocol: UDP
      port: 53
```

---

## RBAC 權限配置

### ServiceAccount 創建

```yaml
# k8s/base/rbac/service-accounts.yaml

---
# API Gateway ServiceAccount
apiVersion: v1
kind: ServiceAccount
metadata:
  name: api-gateway-sa
  namespace: sk-agentic-prod

---
# Agent Service ServiceAccount
apiVersion: v1
kind: ServiceAccount
metadata:
  name: agent-service-sa
  namespace: sk-agentic-prod

---
# Monitoring ServiceAccount
apiVersion: v1
kind: ServiceAccount
metadata:
  name: monitoring-sa
  namespace: monitoring
```

### Role 和 RoleBinding (命名空間內權限)

**Agent Service Role**:

```yaml
# k8s/base/rbac/agent-service-role.yaml

apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: agent-service-role
  namespace: sk-agentic-prod
rules:
# 允許讀取 ConfigMap 和 Secret
- apiGroups: [""]
  resources: ["configmaps", "secrets"]
  verbs: ["get", "list", "watch"]

# 允許讀取 Pod 信息 (用於服務發現)
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "list"]

# 允許創建 Event (日誌記錄)
- apiGroups: [""]
  resources: ["events"]
  verbs: ["create", "patch"]
```

**Agent Service RoleBinding**:

```yaml
# k8s/base/rbac/agent-service-rolebinding.yaml

apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: agent-service-rolebinding
  namespace: sk-agentic-prod
subjects:
- kind: ServiceAccount
  name: agent-service-sa
  namespace: sk-agentic-prod
roleRef:
  kind: Role
  name: agent-service-role
  apiGroup: rbac.authorization.k8s.io
```

### ClusterRole 和 ClusterRoleBinding (集群級別權限)

**Monitoring ClusterRole**:

```yaml
# k8s/base/rbac/monitoring-clusterrole.yaml

apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: monitoring-clusterrole
rules:
# 允許讀取所有命名空間的 Pods
- apiGroups: [""]
  resources: ["pods", "nodes", "services", "endpoints"]
  verbs: ["get", "list", "watch"]

# 允許讀取 Metrics
- apiGroups: [""]
  resources: ["pods/log"]
  verbs: ["get"]

# 允許訪問 Metrics API
- nonResourceURLs: ["/metrics"]
  verbs: ["get"]
```

**Monitoring ClusterRoleBinding**:

```yaml
# k8s/base/rbac/monitoring-clusterrolebinding.yaml

apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: monitoring-clusterrolebinding
subjects:
- kind: ServiceAccount
  name: monitoring-sa
  namespace: monitoring
roleRef:
  kind: ClusterRole
  name: monitoring-clusterrole
  apiGroup: rbac.authorization.k8s.io
```

### 在 Deployment 中使用 ServiceAccount

```yaml
# 更新 API Gateway Deployment 使用 ServiceAccount
spec:
  template:
    spec:
      serviceAccountName: api-gateway-sa
      automountServiceAccountToken: true
      # ... rest of the spec
```

---

## Helm Charts 完整配置

### Helm Chart 結構

```
helm/sk-agentic/
├── Chart.yaml
├── values.yaml
├── values-dev.yaml
├── values-staging.yaml
├── values-prod.yaml
├── templates/
│   ├── NOTES.txt
│   ├── _helpers.tpl
│   ├── namespace.yaml
│   ├── configmaps/
│   │   ├── api-gateway-config.yaml
│   │   ├── postgres-config.yaml
│   │   └── redis-config.yaml
│   ├── secrets/
│   │   ├── database-secrets.yaml
│   │   └── azure-openai-secrets.yaml
│   ├── deployments/
│   │   ├── api-gateway.yaml
│   │   ├── agent-service.yaml
│   │   ├── workflow-service.yaml
│   │   ├── frontend-host.yaml
│   │   └── frontend-remote.yaml
│   ├── statefulsets/
│   │   ├── postgres.yaml
│   │   ├── redis.yaml
│   │   └── rabbitmq.yaml
│   ├── services/
│   │   ├── api-gateway-service.yaml
│   │   ├── agent-service.yaml
│   │   ├── postgres-service.yaml
│   │   └── frontend-host-service.yaml
│   ├── ingress/
│   │   └── main-ingress.yaml
│   ├── hpa/
│   │   ├── api-gateway-hpa.yaml
│   │   └── agent-service-hpa.yaml
│   ├── network-policies/
│   │   ├── default-deny-all.yaml
│   │   └── api-gateway-policy.yaml
│   └── rbac/
│       ├── service-accounts.yaml
│       └── roles.yaml
└── charts/
    └── postgresql/ (可選：使用外部 Helm Chart)
```

### Chart.yaml

```yaml
# helm/sk-agentic/Chart.yaml

apiVersion: v2
name: sk-agentic
description: Semantic Kernel Agentic Framework - Complete K8s Deployment
type: application
version: 1.0.0
appVersion: "1.0.0"
keywords:
  - semantic-kernel
  - ai
  - multi-agent
  - llm
maintainers:
  - name: SK Agentic Team
    email: team@sk-agentic.com
dependencies: []
```

### values.yaml (默認值)

```yaml
# helm/sk-agentic/values.yaml

# Global Configuration
global:
  environment: production
  domain: sk-agentic.com
  tlsSecret: sk-agentic-tls
  imagePullSecrets:
    - name: acr-secret

# Namespace
namespace:
  name: sk-agentic-prod
  labels:
    environment: production
    istio-injection: enabled

# Image Registry
imageRegistry: skagenticacr.azurecr.io
imageTag: "1.0.0"
imagePullPolicy: IfNotPresent

# API Gateway
apiGateway:
  enabled: true
  replicaCount: 3
  image:
    repository: api-gateway
    tag: "1.0.0"
  service:
    type: ClusterIP
    port: 5000
  resources:
    requests:
      cpu: 200m
      memory: 256Mi
    limits:
      cpu: 1000m
      memory: 1Gi
  hpa:
    enabled: true
    minReplicas: 3
    maxReplicas: 10
    targetCPUUtilizationPercentage: 70
    targetMemoryUtilizationPercentage: 80

# Agent Service
agentService:
  enabled: true
  replicaCount: 5
  image:
    repository: agent-service
    tag: "1.0.0"
  service:
    type: ClusterIP
    port: 8001
  resources:
    requests:
      cpu: 500m
      memory: 1Gi
    limits:
      cpu: 2000m
      memory: 4Gi
  hpa:
    enabled: true
    minReplicas: 5
    maxReplicas: 20
    targetCPUUtilizationPercentage: 75

# Frontend Host (React)
frontendHost:
  enabled: true
  replicaCount: 3
  image:
    repository: frontend-host
    tag: "1.0.0"
  service:
    type: ClusterIP
    port: 3000
  resources:
    requests:
      cpu: 100m
      memory: 128Mi
    limits:
      cpu: 500m
      memory: 512Mi
  hpa:
    enabled: true
    minReplicas: 3
    maxReplicas: 15
    targetCPUUtilizationPercentage: 60

# Frontend Remote (Vue)
frontendRemote:
  enabled: true
  replicaCount: 3
  image:
    repository: frontend-remote
    tag: "1.0.0"
  service:
    type: ClusterIP
    port: 3001
  resources:
    requests:
      cpu: 100m
      memory: 128Mi
    limits:
      cpu: 500m
      memory: 512Mi

# PostgreSQL
postgres:
  enabled: true
  replicaCount: 3
  image:
    repository: postgres
    tag: "16-alpine"
  service:
    type: ClusterIP
    port: 5432
  storage:
    storageClass: managed-premium-retain
    size: 100Gi
  resources:
    requests:
      cpu: 1000m
      memory: 2Gi
    limits:
      cpu: 4000m
      memory: 8Gi

# Redis
redis:
  enabled: true
  replicaCount: 3
  image:
    repository: redis
    tag: "7-alpine"
  service:
    type: ClusterIP
    port: 6379
  storage:
    storageClass: managed-premium-retain
    size: 20Gi
  resources:
    requests:
      cpu: 500m
      memory: 1Gi
    limits:
      cpu: 2000m
      memory: 4Gi

# RabbitMQ
rabbitmq:
  enabled: true
  replicaCount: 3
  image:
    repository: rabbitmq
    tag: "3.12-management-alpine"
  service:
    type: ClusterIP
    amqpPort: 5672
    managementPort: 15672
  storage:
    storageClass: managed-premium-retain
    size: 20Gi
  resources:
    requests:
      cpu: 500m
      memory: 1Gi
    limits:
      cpu: 2000m
      memory: 4Gi

# Ingress
ingress:
  enabled: true
  className: nginx
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/proxy-body-size: "50m"
  hosts:
    - host: app.sk-agentic.com
      paths:
        - path: /
          pathType: Prefix
          service: frontend-host-service
          port: 3000
    - host: api.sk-agentic.com
      paths:
        - path: /api
          pathType: Prefix
          service: api-gateway-service
          port: 5000
  tls:
    - secretName: sk-agentic-tls
      hosts:
        - app.sk-agentic.com
        - api.sk-agentic.com

# Network Policies
networkPolicies:
  enabled: true
  defaultDenyAll: true

# RBAC
rbac:
  create: true
  serviceAccounts:
    apiGateway: api-gateway-sa
    agentService: agent-service-sa

# Monitoring
monitoring:
  enabled: true
  prometheus:
    enabled: true
  grafana:
    enabled: true
```

### values-dev.yaml (開發環境覆蓋)

```yaml
# helm/sk-agentic/values-dev.yaml

global:
  environment: development
  domain: dev.sk-agentic.com

imageTag: "dev-latest"

apiGateway:
  replicaCount: 1
  hpa:
    enabled: false

agentService:
  replicaCount: 2
  hpa:
    minReplicas: 2
    maxReplicas: 5

postgres:
  replicaCount: 1
  storage:
    size: 20Gi

redis:
  replicaCount: 1
  storage:
    size: 5Gi

ingress:
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-staging
```

### values-prod.yaml (生產環境覆蓋)

```yaml
# helm/sk-agentic/values-prod.yaml

global:
  environment: production
  domain: sk-agentic.com

imageTag: "1.0.0"

apiGateway:
  replicaCount: 5
  hpa:
    minReplicas: 5
    maxReplicas: 20

agentService:
  replicaCount: 10
  hpa:
    minReplicas: 10
    maxReplicas: 50

postgres:
  replicaCount: 5
  storage:
    size: 500Gi

redis:
  replicaCount: 5
  storage:
    size: 100Gi

ingress:
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/rate-limit: "1000"
```

### _helpers.tpl (模板輔助函數)

```yaml
# helm/sk-agentic/templates/_helpers.tpl

{{/* Generate basic labels */}}
{{- define "sk-agentic.labels" -}}
app.kubernetes.io/name: {{ .Chart.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
environment: {{ .Values.global.environment }}
{{- end }}

{{/* Generate selector labels */}}
{{- define "sk-agentic.selectorLabels" -}}
app.kubernetes.io/name: {{ .Chart.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/* Full image name */}}
{{- define "sk-agentic.imageName" -}}
{{- $registry := .Values.imageRegistry }}
{{- $repository := .image.repository }}
{{- $tag := .image.tag | default .Values.imageTag }}
{{- printf "%s/%s:%s" $registry $repository $tag }}
{{- end }}

{{/* Common annotations */}}
{{- define "sk-agentic.annotations" -}}
prometheus.io/scrape: "true"
prometheus.io/port: {{ .port | quote }}
prometheus.io/path: "/metrics"
{{- end }}
```

### 部署命令

```bash
# 安裝 Chart (開發環境)
helm install sk-agentic ./helm/sk-agentic \
  --namespace sk-agentic-dev \
  --create-namespace \
  --values ./helm/sk-agentic/values-dev.yaml

# 升級 Chart
helm upgrade sk-agentic ./helm/sk-agentic \
  --namespace sk-agentic-dev \
  --values ./helm/sk-agentic/values-dev.yaml

# 卸載 Chart
helm uninstall sk-agentic --namespace sk-agentic-dev

# Dry-run (測試模板渲染)
helm install sk-agentic ./helm/sk-agentic \
  --namespace sk-agentic-prod \
  --values ./helm/sk-agentic/values-prod.yaml \
  --dry-run --debug

# 生成 K8s YAML 文件 (不安裝)
helm template sk-agentic ./helm/sk-agentic \
  --namespace sk-agentic-prod \
  --values ./helm/sk-agentic/values-prod.yaml \
  --output-dir ./generated-manifests
```

---

由於文檔非常龐大（已超過 3,500 行），我將在下一次回應中繼續完成剩餘部分，包括：

- **監控集成 (Prometheus Operator)**
- **滾動更新和回滾策略**
- **災難恢復和高可用性**
- **部署命令和故障排查**

請確認是否需要我繼續完成剩餘內容？
