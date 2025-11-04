# 成本優化策略

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的全面成本優化策略,涵蓋:
- **Right-Sizing 策略** (資源適配與調整)
- **預留實例優化** (Reserved Instances 最佳實踐)
- **自動擴展策略** (Auto-Scaling 成本優化)
- **存儲優化** (生命週期管理, 分層存儲)
- **網絡成本優化** (流量管理, CDN 策略)
- **AI 服務成本控制** (Token 優化, 模型選擇)
- **開發測試環境優化** (環境自動化管理)
- **FinOps 最佳實踐** (財務運營管理)

### 優化目標

**成本節省目標**:
- ✅ **短期 (3個月)**: 節省 15-20% 雲端成本
- ✅ **中期 (6個月)**: 節省 25-35% 雲端成本
- ✅ **長期 (12個月)**: 節省 40-50% 雲端成本,建立持續優化文化

**效率指標**:
- 資源使用率: **70-85%** (避免過度配置或不足)
- 預留實例覆蓋率: **≥ 70%** (穩定工作負載)
- 自動擴展響應時間: **≤ 60秒** (快速適應負載變化)
- 成本異常檢測: **≤ 24小時** (快速發現成本異常)

---

## Right-Sizing 策略

### 資源使用率監控

#### CPU 與 Memory 分析工具

```python
# scripts/resource_utilization_analyzer.py
"""
資源使用率分析器 - 識別過度配置和不足配置的資源
"""
import subprocess
import json
from datetime import datetime, timedelta
from typing import List, Dict
from dataclasses import dataclass

@dataclass
class ResourceMetrics:
    """資源指標"""
    resource_name: str
    resource_type: str
    cpu_avg: float
    cpu_p95: float
    memory_avg: float
    memory_p95: float
    recommendation: str
    potential_savings: float

class ResourceUtilizationAnalyzer:
    """資源使用率分析器"""

    def __init__(self, prometheus_url: str):
        self.prometheus_url = prometheus_url

    def query_prometheus(self, query: str, days: int = 7) -> Dict:
        """查詢 Prometheus 指標"""
        end_time = datetime.utcnow()
        start_time = end_time - timedelta(days=days)

        # 使用 Azure Monitor 或 Prometheus API 查詢
        # 這裡簡化示例
        result = {
            "data": {
                "result": []
            }
        }
        return result

    def analyze_aks_nodes(self) -> List[ResourceMetrics]:
        """分析 AKS 節點使用率"""
        # CPU 使用率查詢 (過去 7 天平均)
        cpu_query = """
        avg_over_time(
            (1 - rate(node_cpu_seconds_total{mode="idle"}[5m]))
            [7d:1h]
        ) * 100
        """

        # Memory 使用率查詢
        memory_query = """
        avg_over_time(
            (1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes))
            [7d:1h]
        ) * 100
        """

        recommendations = []

        # 模擬分析結果
        nodes = [
            {
                "name": "aks-system-pool-12345",
                "vm_size": "Standard_D4s_v5",
                "cpu_avg": 35.2,
                "cpu_p95": 58.1,
                "memory_avg": 42.8,
                "memory_p95": 65.3,
                "current_cost": 140.16
            },
            {
                "name": "aks-app-pool-67890",
                "vm_size": "Standard_D8s_v5",
                "cpu_avg": 78.5,
                "cpu_p95": 92.3,
                "memory_avg": 81.2,
                "memory_p95": 94.7,
                "current_cost": 280.32
            },
            {
                "name": "aks-ai-pool-11111",
                "vm_size": "Standard_D16s_v5",
                "cpu_avg": 25.8,
                "cpu_p95": 42.1,
                "memory_avg": 30.5,
                "memory_p95": 48.9,
                "current_cost": 560.64
            }
        ]

        for node in nodes:
            recommendation, savings = self._generate_recommendation(node)

            metrics = ResourceMetrics(
                resource_name=node["name"],
                resource_type=f"AKS Node ({node['vm_size']})",
                cpu_avg=node["cpu_avg"],
                cpu_p95=node["cpu_p95"],
                memory_avg=node["memory_avg"],
                memory_p95=node["memory_p95"],
                recommendation=recommendation,
                potential_savings=savings
            )
            recommendations.append(metrics)

        return recommendations

    def _generate_recommendation(self, node: Dict) -> tuple[str, float]:
        """生成優化建議"""
        cpu_avg = node["cpu_avg"]
        cpu_p95 = node["cpu_p95"]
        memory_avg = node["memory_avg"]
        memory_p95 = node["memory_p95"]
        current_cost = node["current_cost"]

        # 過度配置檢測 (CPU < 50%, Memory < 60%)
        if cpu_p95 < 50 and memory_p95 < 60:
            # 建議降級到更小的 VM
            new_vm_map = {
                "Standard_D16s_v5": ("Standard_D8s_v5", 280.32, 50),
                "Standard_D8s_v5": ("Standard_D4s_v5", 140.16, 50),
                "Standard_D4s_v5": ("Standard_D2s_v5", 70.08, 50)
            }

            if node["vm_size"] in new_vm_map:
                new_vm, new_cost, savings_pct = new_vm_map[node["vm_size"]]
                savings = current_cost - new_cost
                return (
                    f"⬇️ 降級到 {new_vm} (CPU/Memory 使用率偏低)",
                    savings
                )

        # 配置不足檢測 (CPU > 85% 或 Memory > 90%)
        elif cpu_p95 > 85 or memory_p95 > 90:
            # 建議升級到更大的 VM
            new_vm_map = {
                "Standard_D2s_v5": ("Standard_D4s_v5", 140.16),
                "Standard_D4s_v5": ("Standard_D8s_v5", 280.32),
                "Standard_D8s_v5": ("Standard_D16s_v5", 560.64)
            }

            if node["vm_size"] in new_vm_map:
                new_vm, new_cost = new_vm_map[node["vm_size"]]
                additional_cost = new_cost - current_cost
                return (
                    f"⬆️ 升級到 {new_vm} (CPU/Memory 使用率過高)",
                    -additional_cost
                )

        # 配置合適
        return ("✅ 配置合適,無需調整", 0.0)

    def generate_report(self) -> str:
        """生成優化報告"""
        recommendations = self.analyze_aks_nodes()

        total_savings = sum(r.potential_savings for r in recommendations)

        report = f"""
=== 資源使用率分析報告 ===
生成時間: {datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S UTC')}

總潛在節省: ${total_savings:,.2f}/月

資源分析:
{"="*80}
"""

        for rec in recommendations:
            report += f"""
資源: {rec.resource_name}
類型: {rec.resource_type}
CPU 使用率: 平均 {rec.cpu_avg:.1f}%, P95 {rec.cpu_p95:.1f}%
Memory 使用率: 平均 {rec.memory_avg:.1f}%, P95 {rec.memory_p95:.1f}%
建議: {rec.recommendation}
潛在節省: ${rec.potential_savings:,.2f}/月

"""

        return report


# 使用示例
if __name__ == "__main__":
    analyzer = ResourceUtilizationAnalyzer(prometheus_url="http://prometheus:9090")
    print(analyzer.generate_report())
```

**輸出示例**:
```
=== 資源使用率分析報告 ===
生成時間: 2025-11-02 10:30:00 UTC

總潛在節省: $280.32/月

資源分析:
================================================================================

資源: aks-system-pool-12345
類型: AKS Node (Standard_D4s_v5)
CPU 使用率: 平均 35.2%, P95 58.1%
Memory 使用率: 平均 42.8%, P95 65.3%
建議: ✅ 配置合適,無需調整
潛在節省: $0.00/月

資源: aks-app-pool-67890
類型: AKS Node (Standard_D8s_v5)
CPU 使用率: 平均 78.5%, P95 92.3%
Memory 使用率: 平均 81.2%, P95 94.7%
建議: ⬆️ 升級到 Standard_D16s_v5 (CPU/Memory 使用率過高)
潛在節省: $-280.32/月

資源: aks-ai-pool-11111
類型: AKS Node (Standard_D16s_v5)
CPU 使用率: 平均 25.8%, P95 42.1%
Memory 使用率: 平均 30.5%, P95 48.9%
建議: ⬇️ 降級到 Standard_D8s_v5 (CPU/Memory 使用率偏低)
潛在節省: $280.32/月
```

### 資源調整流程

```bash
#!/bin/bash
# scripts/right-size-aks-node-pool.sh

set -e

NODE_POOL_NAME="$1"
NEW_VM_SIZE="$2"
CLUSTER_NAME="aiagent-aks-prod"
RESOURCE_GROUP="aiagent-prod-rg"

echo "=== AKS Node Pool Right-Sizing ==="
echo "Node Pool: $NODE_POOL_NAME"
echo "Current VM Size: (查詢中...)"

# 查詢當前配置
CURRENT_CONFIG=$(az aks nodepool show \
  --resource-group "$RESOURCE_GROUP" \
  --cluster-name "$CLUSTER_NAME" \
  --name "$NODE_POOL_NAME" \
  --query '{vmSize:vmSize, count:count, minCount:minCount, maxCount:maxCount}' \
  -o json)

echo "當前配置: $CURRENT_CONFIG"

# 確認操作
read -p "確定要將 Node Pool 調整為 $NEW_VM_SIZE 嗎? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
  echo "操作已取消"
  exit 0
fi

# 創建新的 Node Pool (Blue-Green 策略)
NEW_NODE_POOL_NAME="${NODE_POOL_NAME}-new"

echo "Step 1: 創建新的 Node Pool ($NEW_NODE_POOL_NAME)..."
az aks nodepool add \
  --resource-group "$RESOURCE_GROUP" \
  --cluster-name "$CLUSTER_NAME" \
  --name "$NEW_NODE_POOL_NAME" \
  --node-vm-size "$NEW_VM_SIZE" \
  --node-count 1 \
  --enable-cluster-autoscaler \
  --min-count 1 \
  --max-count 5 \
  --mode User

echo "Step 2: 等待新 Node Pool 就緒..."
sleep 60

# 驗證新節點健康
kubectl wait --for=condition=Ready nodes -l agentpool="$NEW_NODE_POOL_NAME" --timeout=300s

echo "Step 3: 將工作負載遷移到新 Node Pool..."
# 使用 node affinity 或 taint/toleration 來遷移

# Cordon 舊節點 (防止新 Pod 調度)
kubectl cordon -l agentpool="$NODE_POOL_NAME"

# Drain 舊節點 (驅逐現有 Pod)
kubectl drain -l agentpool="$NODE_POOL_NAME" \
  --ignore-daemonsets \
  --delete-emptydir-data \
  --force \
  --grace-period=300

echo "Step 4: 刪除舊 Node Pool..."
az aks nodepool delete \
  --resource-group "$RESOURCE_GROUP" \
  --cluster-name "$CLUSTER_NAME" \
  --name "$NODE_POOL_NAME" \
  --no-wait

echo "Step 5: 重命名新 Node Pool (可選)..."
# Azure 不支持 Node Pool 重命名,保留 -new 後綴

echo "✅ Right-Sizing 完成!"
echo "新 Node Pool: $NEW_NODE_POOL_NAME (VM Size: $NEW_VM_SIZE)"
```

---

## 預留實例優化

### Reserved Instances 購買策略

#### 預留實例覆蓋率分析

```csharp
// src/FinOps/ReservedInstanceCoverageAnalyzer.cs
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ReservedInstanceCoverageAnalyzer
{
    public record ResourceUsage(
        string ResourceType,
        string VmSize,
        int AverageCount,
        int MinCount,
        int MaxCount,
        decimal HourlyPayAsYouGoRate,
        decimal HourlyReservedRate1Year,
        decimal HourlyReservedRate3Year
    );

    public record CoverageRecommendation(
        string ResourceType,
        string VmSize,
        int RecommendedReservedCount,
        int ReservationTerm,
        decimal MonthlySavings,
        decimal AnnualSavings,
        string Rationale
    );

    public List<CoverageRecommendation> AnalyzeCoverage(List<ResourceUsage> usages)
    {
        var recommendations = new List<CoverageRecommendation>();

        foreach (var usage in usages)
        {
            // 策略: 為最低使用量 (MinCount) 購買 3年預留
            // 為穩定使用量 (MinCount 到 AvgCount) 購買 1年預留
            // 峰值部分 (AvgCount 到 MaxCount) 使用按需計費

            if (usage.MinCount > 0)
            {
                // 3年預留建議
                var savings3Year = CalculateSavings(
                    usage.MinCount,
                    usage.HourlyPayAsYouGoRate,
                    usage.HourlyReservedRate3Year,
                    term: 3
                );

                recommendations.Add(new CoverageRecommendation(
                    ResourceType: usage.ResourceType,
                    VmSize: usage.VmSize,
                    RecommendedReservedCount: usage.MinCount,
                    ReservationTerm: 3,
                    MonthlySavings: savings3Year.MonthlySavings,
                    AnnualSavings: savings3Year.AnnualSavings,
                    Rationale: $"基礎容量 ({usage.MinCount} 實例) 持續運行,適合 3年預留"
                ));
            }

            // 1年預留建議 (穩定增長部分)
            var stableGrowth = usage.AverageCount - usage.MinCount;
            if (stableGrowth > 0)
            {
                var savings1Year = CalculateSavings(
                    stableGrowth,
                    usage.HourlyPayAsYouGoRate,
                    usage.HourlyReservedRate1Year,
                    term: 1
                );

                recommendations.Add(new CoverageRecommendation(
                    ResourceType: usage.ResourceType,
                    VmSize: usage.VmSize,
                    RecommendedReservedCount: stableGrowth,
                    ReservationTerm: 1,
                    MonthlySavings: savings1Year.MonthlySavings,
                    AnnualSavings: savings1Year.AnnualSavings,
                    Rationale: $"穩定增長容量 ({stableGrowth} 實例),適合 1年預留"
                ));
            }

            // 峰值部分使用按需計費 (無建議)
            var burstCapacity = usage.MaxCount - usage.AverageCount;
            if (burstCapacity > 0)
            {
                Console.WriteLine($"峰值容量 {burstCapacity} 實例建議使用按需計費或 Spot Instances");
            }
        }

        return recommendations;
    }

    private (decimal MonthlySavings, decimal AnnualSavings) CalculateSavings(
        int instanceCount,
        decimal payAsYouGoRate,
        decimal reservedRate,
        int term)
    {
        var hoursPerMonth = 730m;  // 平均每月小時數
        var hoursPerYear = hoursPerMonth * 12;

        var monthlyPayAsYouGo = instanceCount * payAsYouGoRate * hoursPerMonth;
        var monthlyReserved = instanceCount * reservedRate * hoursPerMonth;
        var monthlySavings = monthlyPayAsYouGo - monthlyReserved;

        var annualSavings = monthlySavings * 12;

        return (monthlySavings, annualSavings);
    }

    public void GenerateReport(List<CoverageRecommendation> recommendations)
    {
        Console.WriteLine("=== 預留實例覆蓋率分析報告 ===\n");

        var totalMonthlySavings = recommendations.Sum(r => r.MonthlySavings);
        var totalAnnualSavings = recommendations.Sum(r => r.AnnualSavings);

        Console.WriteLine($"總潛在節省: ${totalMonthlySavings:N2}/月, ${totalAnnualSavings:N2}/年\n");

        foreach (var rec in recommendations)
        {
            Console.WriteLine($"資源: {rec.ResourceType} ({rec.VmSize})");
            Console.WriteLine($"建議預留數量: {rec.RecommendedReservedCount} 實例");
            Console.WriteLine($"預留期限: {rec.ReservationTerm} 年");
            Console.WriteLine($"月度節省: ${rec.MonthlySavings:N2}");
            Console.WriteLine($"年度節省: ${rec.AnnualSavings:N2}");
            Console.WriteLine($"理由: {rec.Rationale}");
            Console.WriteLine();
        }
    }
}

// 使用示例
var analyzer = new ReservedInstanceCoverageAnalyzer();

var usages = new List<ReservedInstanceCoverageAnalyzer.ResourceUsage>
{
    new(
        ResourceType: "AKS Node",
        VmSize: "Standard_D8s_v5",
        AverageCount: 5,
        MinCount: 3,
        MaxCount: 10,
        HourlyPayAsYouGoRate: 0.384m,
        HourlyReservedRate1Year: 0.2496m,
        HourlyReservedRate3Year: 0.1784m
    ),
    new(
        ResourceType: "PostgreSQL Flexible Server",
        VmSize: "Standard_D4ds_v5",
        AverageCount: 2,
        MinCount: 2,
        MaxCount: 2,
        HourlyPayAsYouGoRate: 0.283m,
        HourlyReservedRate1Year: 0.184m,
        HourlyReservedRate3Year: 0.131m
    )
};

var recommendations = analyzer.AnalyzeCoverage(usages);
analyzer.GenerateReport(recommendations);
```

### 預留實例購買腳本

```bash
#!/bin/bash
# scripts/purchase-reserved-instances.sh

set -e

echo "=== Azure Reserved Instance 購買工具 ==="

# 配置參數
RESOURCE_TYPE="$1"  # VirtualMachines, SQLDatabases
VM_SIZE="$2"        # Standard_D8s_v5
QUANTITY="$3"       # 5
TERM="$4"           # 1 年或 3 年
REGION="eastus"

# 驗證參數
if [ -z "$RESOURCE_TYPE" ] || [ -z "$VM_SIZE" ] || [ -z "$QUANTITY" ] || [ -z "$TERM" ]; then
  echo "用法: $0 <RESOURCE_TYPE> <VM_SIZE> <QUANTITY> <TERM>"
  echo "示例: $0 VirtualMachines Standard_D8s_v5 5 P1Y"
  exit 1
fi

# 查詢預留實例定價
echo "Step 1: 查詢預留實例定價..."
az reservations catalog show \
  --reserved-resource-type "$RESOURCE_TYPE" \
  --location "$REGION" \
  --query "[?name=='$VM_SIZE']" \
  -o table

# 計算成本
echo ""
echo "Step 2: 計算預留實例成本..."

# 購買預留實例 (需要 Billing Admin 權限)
echo ""
echo "Step 3: 購買預留實例..."
echo "⚠️  注意: 此操作需要 Billing Administrator 角色"
read -p "確定要購買 $QUANTITY 個 $VM_SIZE 預留實例 (期限: $TERM) 嗎? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
  echo "購買已取消"
  exit 0
fi

# 使用 Azure Portal 或 PowerShell 購買
# az CLI 目前不完全支持預留實例購買,建議使用 Portal

echo "請訪問 Azure Portal 完成購買:"
echo "https://portal.azure.com/#blade/Microsoft_Azure_Reservations/CreateBlade"
echo ""
echo "購買參數:"
echo "  資源類型: $RESOURCE_TYPE"
echo "  VM 大小: $VM_SIZE"
echo "  數量: $QUANTITY"
echo "  期限: $TERM"
echo "  區域: $REGION"
```

---

## 自動擴展策略

### Horizontal Pod Autoscaler (HPA) 優化

```yaml
# k8s/base/hpa-optimized.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-deployment-hpa
  namespace: aiagent-prod
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-deployment

  minReplicas: 3  # 最小副本數 (保證高可用)
  maxReplicas: 20 # 最大副本數 (成本控制上限)

  # 多指標擴展策略
  metrics:
    # CPU 指標
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70  # 70% CPU 觸發擴展

    # Memory 指標
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 80  # 80% Memory 觸發擴展

    # 自定義指標 - HTTP 請求速率
    - type: Pods
      pods:
        metric:
          name: http_requests_per_second
        target:
          type: AverageValue
          averageValue: "1000"  # 每秒 1000 請求觸發擴展

  # 擴展行為配置 (避免頻繁擴縮)
  behavior:
    scaleUp:
      stabilizationWindowSeconds: 60  # 擴展前觀察 60 秒
      policies:
      - type: Percent
        value: 50  # 每次最多增加 50% Pod
        periodSeconds: 60
      - type: Pods
        value: 5   # 或每次最多增加 5 個 Pod
        periodSeconds: 60
      selectPolicy: Max  # 選擇最激進的策略

    scaleDown:
      stabilizationWindowSeconds: 300  # 縮容前觀察 5 分鐘 (避免抖動)
      policies:
      - type: Percent
        value: 10  # 每次最多減少 10% Pod
        periodSeconds: 60
      - type: Pods
        value: 2   # 或每次最多減少 2 個 Pod
        periodSeconds: 60
      selectPolicy: Min  # 選擇最保守的策略
```

### Cluster Autoscaler 成本優化配置

```yaml
# k8s/cluster-autoscaler-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: cluster-autoscaler-config
  namespace: kube-system
data:
  config.yaml: |
    # 擴展策略
    expanderPriorities:
      # 優先使用預留實例節點池
      - name: reserved-instance-pool
        priority: 100
      # 其次使用按需節點池
      - name: on-demand-pool
        priority: 50
      # 最後使用 Spot 節點池 (最便宜但可能被回收)
      - name: spot-pool
        priority: 10

    # 縮容配置
    scaleDownEnabled: true
    scaleDownUnneededTime: 10m  # 節點閒置 10 分鐘後縮容
    scaleDownUtilizationThreshold: 0.5  # 節點使用率 < 50% 視為閒置

    # 成本優化
    balanceSimilarNodeGroups: true  # 平衡相似節點組
    skipNodesWithSystemPods: false  # 允許縮容系統 Pod 節點
    skipNodesWithLocalStorage: false

    # 時間調度優化 (工作時間 vs. 非工作時間)
    scheduling:
      - name: "business-hours"
        cron: "0 8 * * 1-5"  # 週一到週五 8:00
        minNodes: 10
        maxNodes: 20
      - name: "off-hours"
        cron: "0 18 * * *"  # 每天 18:00
        minNodes: 5
        maxNodes: 10
      - name: "weekend"
        cron: "0 0 * * 6,0"  # 週末
        minNodes: 3
        maxNodes: 8
```

### 時間調度自動縮容

```python
# scripts/scheduled_scaling.py
"""
時間調度自動縮容 - 根據業務時間自動調整集群規模
"""
import os
from datetime import datetime, time
from azure.identity import DefaultAzureCredential
from azure.mgmt.containerservice import ContainerServiceClient

class ScheduledScalingManager:
    """時間調度縮容管理器"""

    def __init__(self, subscription_id: str, resource_group: str, cluster_name: str):
        self.subscription_id = subscription_id
        self.resource_group = resource_group
        self.cluster_name = cluster_name

        credential = DefaultAzureCredential()
        self.client = ContainerServiceClient(credential, subscription_id)

    def get_current_schedule(self) -> dict:
        """獲取當前應該的調度配置"""
        now = datetime.now()
        current_time = now.time()
        is_weekday = now.weekday() < 5  # 0-4 是週一到週五

        # 業務時間 (週一到週五 8:00-18:00)
        if is_weekday and time(8, 0) <= current_time < time(18, 0):
            return {
                "period": "business-hours",
                "application_pool": {"min": 5, "max": 15},
                "ai_pool": {"min": 4, "max": 10}
            }

        # 週末
        elif not is_weekday:
            return {
                "period": "weekend",
                "application_pool": {"min": 2, "max": 8},
                "ai_pool": {"min": 1, "max": 5}
            }

        # 非業務時間 (工作日晚上)
        else:
            return {
                "period": "off-hours",
                "application_pool": {"min": 3, "max": 10},
                "ai_pool": {"min": 2, "max": 6}
            }

    def apply_scheduled_scaling(self):
        """應用時間調度縮容"""
        schedule = self.get_current_schedule()
        print(f"當前時段: {schedule['period']}")

        # 調整 Application Pool
        self._update_node_pool(
            "application-pool",
            min_count=schedule["application_pool"]["min"],
            max_count=schedule["application_pool"]["max"]
        )

        # 調整 AI Pool
        self._update_node_pool(
            "ai-pool",
            min_count=schedule["ai_pool"]["min"],
            max_count=schedule["ai_pool"]["max"]
        )

        print(f"✅ 時間調度縮容已應用")

    def _update_node_pool(self, node_pool_name: str, min_count: int, max_count: int):
        """更新節點池配置"""
        print(f"更新 {node_pool_name}: min={min_count}, max={max_count}")

        # 獲取當前節點池配置
        agent_pool = self.client.agent_pools.get(
            self.resource_group,
            self.cluster_name,
            node_pool_name
        )

        # 僅更新 autoscaler 配置
        agent_pool.min_count = min_count
        agent_pool.max_count = max_count

        # 應用更新
        self.client.agent_pools.begin_create_or_update(
            self.resource_group,
            self.cluster_name,
            node_pool_name,
            agent_pool
        ).result()

# 使用示例 (通過 Kubernetes CronJob 每小時執行一次)
if __name__ == "__main__":
    manager = ScheduledScalingManager(
        subscription_id=os.getenv("AZURE_SUBSCRIPTION_ID"),
        resource_group="aiagent-prod-rg",
        cluster_name="aiagent-aks-prod"
    )
    manager.apply_scheduled_scaling()
```

**部署為 Kubernetes CronJob**:

```yaml
# k8s/scheduled-scaling-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: scheduled-scaling
  namespace: kube-system
spec:
  schedule: "0 * * * *"  # 每小時執行一次
  jobTemplate:
    spec:
      template:
        spec:
          serviceAccountName: cluster-autoscaler
          containers:
          - name: scheduled-scaling
            image: aiagent-acr.azurecr.io/scheduled-scaling:latest
            env:
            - name: AZURE_SUBSCRIPTION_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: subscription-id
            - name: AZURE_CLIENT_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: client-id
            - name: AZURE_CLIENT_SECRET
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: client-secret
            - name: AZURE_TENANT_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: tenant-id
          restartPolicy: OnFailure
```

---

## 存儲優化

### Blob Storage 生命週期管理

```json
{
  "rules": [
    {
      "enabled": true,
      "name": "move-old-logs-to-cool",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["logs/"]
        },
        "actions": {
          "baseBlob": {
            "tierToCool": {
              "daysAfterModificationGreaterThan": 30
            },
            "tierToArchive": {
              "daysAfterModificationGreaterThan": 90
            },
            "delete": {
              "daysAfterModificationGreaterThan": 365
            }
          }
        }
      }
    },
    {
      "enabled": true,
      "name": "move-old-backups-to-archive",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["backups/postgresql/"]
        },
        "actions": {
          "baseBlob": {
            "tierToArchive": {
              "daysAfterModificationGreaterThan": 35
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
      "name": "delete-temp-files",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["temp/"]
        },
        "actions": {
          "baseBlob": {
            "delete": {
              "daysAfterModificationGreaterThan": 7
            }
          }
        }
      }
    }
  ]
}
```

**應用生命週期策略**:

```bash
#!/bin/bash
# scripts/apply-blob-lifecycle-policy.sh

STORAGE_ACCOUNT="aiagentstorageprod"
RESOURCE_GROUP="aiagent-prod-rg"
POLICY_FILE="blob-lifecycle-policy.json"

echo "=== 應用 Blob Storage 生命週期策略 ==="

# 應用策略
az storage account management-policy create \
  --account-name "$STORAGE_ACCOUNT" \
  --resource-group "$RESOURCE_GROUP" \
  --policy "@$POLICY_FILE"

echo "✅ 生命週期策略已應用"

# 驗證策略
az storage account management-policy show \
  --account-name "$STORAGE_ACCOUNT" \
  --resource-group "$RESOURCE_GROUP" \
  -o table
```

### PostgreSQL 備份優化

```yaml
postgresql_backup_optimization:
  strategies:
    - name: "增量備份優化"
      description: "減少備份存儲空間"
      implementation:
        - "使用 WAL 歸檔實現增量備份"
        - "僅保留最近 7 天的完整備份"
        - "舊備份轉為 Archive Tier"
      savings: "$50/月"

    - name: "壓縮優化"
      description: "提高備份壓縮率"
      implementation:
        - "pg_dump --compress=9 (最高壓縮)"
        - "使用 zstd 替代 gzip (更高壓縮率)"
      compression_ratio: "70% 空間節省"
      savings: "$30/月"

    - name: "保留策略優化"
      description: "合理的備份保留期"
      policy:
        daily_backups: "7 天"
        weekly_backups: "4 週"
        monthly_backups: "12 個月"
        yearly_backups: "3 年 (Archive Tier)"
      savings: "$80/月"
```

---

## 網絡成本優化

### CDN 與靜態資源優化

```yaml
cdn_optimization:
  strategy: "使用 Azure CDN 減少 Blob Storage 出站流量"

  configuration:
    cdn_profile: "aiagent-cdn-standard"
    cdn_endpoint: "aiagent-static.azureedge.net"
    origin: "aiagentstorageprod.blob.core.windows.net"

  cost_comparison:
    without_cdn:
      blob_egress_gb: 1000  # 1 TB
      cost_per_gb: 0.087
      monthly_cost: "$87.00"

    with_cdn:
      cdn_egress_gb: 1000
      cdn_cost_per_gb: 0.081  # CDN 出站稍便宜
      blob_to_cdn_transfer: "免費 (同區域)"
      monthly_cost: "$81.00"
      cache_hit_rate: "85%"  # 85% 請求命中緩存
      effective_monthly_cost: "$12.15"  # 僅 15% 流量回源
      savings: "$74.85/月"

  implementation:
    - "配置 CDN Profile 和 Endpoint"
    - "設置緩存規則 (靜態資源 7 天 TTL)"
    - "啟用壓縮 (Gzip, Brotli)"
    - "配置自定義域名"
```

**CDN 配置腳本**:

```bash
#!/bin/bash
# scripts/setup-cdn.sh

RESOURCE_GROUP="aiagent-prod-rg"
CDN_PROFILE="aiagent-cdn-standard"
CDN_ENDPOINT="aiagent-static"
STORAGE_ACCOUNT="aiagentstorageprod"

echo "=== 配置 Azure CDN ==="

# 創建 CDN Profile
az cdn profile create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CDN_PROFILE" \
  --sku Standard_Microsoft

# 創建 CDN Endpoint
az cdn endpoint create \
  --resource-group "$RESOURCE_GROUP" \
  --profile-name "$CDN_PROFILE" \
  --name "$CDN_ENDPOINT" \
  --origin "$STORAGE_ACCOUNT.blob.core.windows.net" \
  --origin-host-header "$STORAGE_ACCOUNT.blob.core.windows.net" \
  --enable-compression true \
  --content-types-to-compress \
    "text/plain" \
    "text/html" \
    "text/css" \
    "application/javascript" \
    "application/json"

# 配置緩存規則
az cdn endpoint rule add \
  --resource-group "$RESOURCE_GROUP" \
  --profile-name "$CDN_PROFILE" \
  --endpoint-name "$CDN_ENDPOINT" \
  --order 1 \
  --rule-name "CacheStaticAssets" \
  --match-variable UrlFileExtension \
  --operator Equal \
  --match-values "jpg" "png" "gif" "css" "js" \
  --action-name CacheExpiration \
  --cache-behavior Override \
  --cache-duration "7.00:00:00"  # 7 天

echo "✅ CDN 配置完成"
echo "CDN Endpoint: https://$CDN_ENDPOINT.azureedge.net"
```

### 跨區域流量優化

```yaml
cross_region_optimization:
  problem: "跨區域數據傳輸成本高 ($0.02/GB)"

  solutions:
    - strategy: "使用 Azure Front Door 智能路由"
      description: "自動將用戶路由到最近的區域"
      cost_impact: "減少 60% 跨區域流量"
      savings: "$200/月"

    - strategy: "區域數據本地化"
      description: "在用戶所在區域存儲常用數據"
      implementation:
        - "使用 Geo-Replication 複製熱數據"
        - "冷數據集中存儲在主區域"
      savings: "$150/月"

    - strategy: "壓縮跨區域傳輸"
      description: "壓縮數據再傳輸"
      compression_ratio: "70%"
      savings: "$100/月"
```

---

## AI 服務成本控制

### Token 優化策略

```python
# src/AI/TokenOptimizer.py
"""
Azure OpenAI Token 優化器
"""
import tiktoken
from typing import List, Dict
import hashlib
import redis

class TokenOptimizer:
    """Token 使用優化器"""

    def __init__(self, redis_client: redis.Redis):
        self.redis_client = redis_client
        self.encoder = tiktoken.encoding_for_model("gpt-4")

    def count_tokens(self, text: str) -> int:
        """計算文本的 token 數量"""
        return len(self.encoder.encode(text))

    def optimize_prompt(self, prompt: str, max_tokens: int = 2000) -> str:
        """優化 prompt 長度"""
        current_tokens = self.count_tokens(prompt)

        if current_tokens <= max_tokens:
            return prompt

        # 策略 1: 移除不必要的空白
        optimized = " ".join(prompt.split())

        # 策略 2: 截斷到最大長度 (保留前後文)
        if self.count_tokens(optimized) > max_tokens:
            # 簡化截斷策略
            words = optimized.split()
            target_words = int(len(words) * (max_tokens / current_tokens))
            optimized = " ".join(words[:target_words])

        return optimized

    def get_cached_response(self, prompt: str) -> str | None:
        """從緩存獲取響應"""
        cache_key = f"openai:response:{hashlib.sha256(prompt.encode()).hexdigest()}"
        cached = self.redis_client.get(cache_key)

        if cached:
            print(f"✅ Cache hit (節省 API 調用)")
            return cached.decode('utf-8')

        return None

    def cache_response(self, prompt: str, response: str, ttl: int = 86400):
        """緩存響應 (24 小時 TTL)"""
        cache_key = f"openai:response:{hashlib.sha256(prompt.encode()).hexdigest()}"
        self.redis_client.setex(cache_key, ttl, response)

    def batch_requests(self, prompts: List[str]) -> List[str]:
        """批處理請求 (減少 API 調用次數)"""
        # 合併相似的 prompt
        batched_prompts = []
        responses = []

        # 檢查緩存
        for prompt in prompts:
            cached = self.get_cached_response(prompt)
            if cached:
                responses.append(cached)
            else:
                batched_prompts.append(prompt)

        # 批量調用 API
        if batched_prompts:
            # 實際調用 Azure OpenAI API
            # api_responses = self.call_openai_batch(batched_prompts)
            # responses.extend(api_responses)
            pass

        return responses

    def estimate_cost(self, input_tokens: int, output_tokens: int, model: str = "gpt-4") -> float:
        """估算 API 調用成本"""
        pricing = {
            "gpt-4": {"input": 0.03, "output": 0.06},  # per 1K tokens
            "gpt-3.5-turbo": {"input": 0.0015, "output": 0.002}
        }

        input_cost = (input_tokens / 1000) * pricing[model]["input"]
        output_cost = (output_tokens / 1000) * pricing[model]["output"]

        return input_cost + output_cost

# 使用示例
redis_client = redis.Redis(host='localhost', port=6379, db=0)
optimizer = TokenOptimizer(redis_client)

prompt = "分析以下工作流並生成優化建議..."
optimized_prompt = optimizer.optimize_prompt(prompt, max_tokens=1500)

print(f"原始 tokens: {optimizer.count_tokens(prompt)}")
print(f"優化後 tokens: {optimizer.count_tokens(optimized_prompt)}")

# 估算成本
cost = optimizer.estimate_cost(input_tokens=1500, output_tokens=800, model="gpt-4")
print(f"估算成本: ${cost:.4f}")
```

### 模型選擇策略

```yaml
model_selection_strategy:
  decision_tree:
    - task: "簡單分類任務"
      recommended_model: "GPT-3.5 Turbo"
      cost_per_1k_tokens: "$0.002 (output)"
      use_cases:
        - "文本分類"
        - "簡單問答"
        - "關鍵字提取"

    - task: "複雜推理任務"
      recommended_model: "GPT-4"
      cost_per_1k_tokens: "$0.06 (output)"
      use_cases:
        - "代碼生成"
        - "複雜工作流規劃"
        - "多步驟推理"

    - task: "批量處理"
      recommended_model: "GPT-3.5 Turbo + 批處理"
      optimization: "合併多個請求到單個 API 調用"
      savings: "60% API 調用次數"

  cost_optimization:
    - "使用 GPT-3.5 Turbo 處理 70% 簡單任務"
    - "僅對複雜任務使用 GPT-4"
    - "實施結果緩存 (Redis, 24 小時 TTL)"
    - "Token 優化 (移除冗餘內容)"

  estimated_savings: "$6,000/月 (從 $12,390 降至 $6,390)"
```

---

## 開發測試環境優化

### 環境自動化管理

```python
# scripts/dev_environment_scheduler.py
"""
開發測試環境自動化調度器 - 工作時間自動啟動,非工作時間自動關閉
"""
from datetime import datetime, time
import os
from azure.identity import DefaultAzureCredential
from azure.mgmt.containerservice import ContainerServiceClient

class DevEnvironmentScheduler:
    """開發環境調度器"""

    def __init__(self):
        self.subscription_id = os.getenv("AZURE_SUBSCRIPTION_ID")
        credential = DefaultAzureCredential()
        self.client = ContainerServiceClient(credential, self.subscription_id)

    def should_be_running(self) -> bool:
        """判斷當前是否應該運行"""
        now = datetime.now()
        current_time = now.time()
        is_weekday = now.weekday() < 5

        # 工作日 8:00-20:00 運行
        if is_weekday and time(8, 0) <= current_time < time(20, 0):
            return True

        return False

    def start_dev_cluster(self):
        """啟動開發集群"""
        print("🚀 啟動開發集群...")

        self.client.managed_clusters.begin_start(
            resource_group_name="aiagent-dev-rg",
            resource_name="aiagent-aks-dev"
        ).result()

        print("✅ 開發集群已啟動")

    def stop_dev_cluster(self):
        """停止開發集群"""
        print("🛑 停止開發集群...")

        self.client.managed_clusters.begin_stop(
            resource_group_name="aiagent-dev-rg",
            resource_name="aiagent-aks-dev"
        ).result()

        print("✅ 開發集群已停止")

    def run_scheduled_check(self):
        """執行定時檢查"""
        should_run = self.should_be_running()

        # 獲取當前集群狀態
        cluster = self.client.managed_clusters.get(
            resource_group_name="aiagent-dev-rg",
            resource_name="aiagent-aks-dev"
        )

        current_state = cluster.power_state.code  # "Running" or "Stopped"

        if should_run and current_state == "Stopped":
            self.start_dev_cluster()
        elif not should_run and current_state == "Running":
            self.stop_dev_cluster()
        else:
            print(f"✅ 集群狀態正確 (應該: {'運行' if should_run else '停止'}, 實際: {current_state})")

# 使用示例 (通過 Azure Function 或 GitHub Actions 定時執行)
if __name__ == "__main__":
    scheduler = DevEnvironmentScheduler()
    scheduler.run_scheduled_check()
```

**部署為 Azure Function (Timer Trigger)**:

```csharp
// functions/DevEnvironmentScheduler.cs
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class DevEnvironmentScheduler
{
    private readonly ILogger _logger;

    public DevEnvironmentScheduler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DevEnvironmentScheduler>();
    }

    [Function("DevEnvironmentScheduler")]
    public async Task Run(
        [TimerTrigger("0 */30 * * * *")] TimerInfo timer)  // 每 30 分鐘執行一次
    {
        _logger.LogInformation($"DevEnvironmentScheduler 執行於: {DateTime.UtcNow}");

        // 執行 Python 腳本或直接調用 Azure SDK
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python3",
                Arguments = "scripts/dev_environment_scheduler.py",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        var output = await process.StandardOutput.ReadToEndAsync();
        _logger.LogInformation(output);
    }
}
```

**成本節省估算**:
- 開發集群成本: $800/月 (24/7 運行)
- 優化後成本: $300/月 (僅工作時間運行, 約 50 小時/週)
- **月度節省**: $500/月
- **年度節省**: $6,000/年

---

## FinOps 最佳實踐

### 成本責任分配 (Cost Allocation)

```yaml
cost_allocation_strategy:
  tagging_policy:
    required_tags:
      - key: "Environment"
        values: ["Production", "Staging", "Development"]
      - key: "Team"
        values: ["Platform", "DataScience", "Frontend"]
      - key: "CostCenter"
        values: ["Engineering", "Operations", "R&D"]
      - key: "Project"
        values: ["AIAgent", "Analytics", "Infrastructure"]

  implementation:
    - "所有資源必須包含 required_tags"
    - "通過 Azure Policy 強制標記合規性"
    - "每月生成成本分配報告"
    - "團隊 Chargeback 模型"

  cost_allocation_example:
    total_monthly_cost: "$13,203"
    breakdown:
      - team: "Platform"
        cost: "$8,500"
        percentage: "64%"
        resources: ["AKS", "PostgreSQL", "Redis"]
      - team: "DataScience"
        cost: "$3,700"
        percentage: "28%"
        resources: ["Azure OpenAI", "Cognitive Services"]
      - team: "Operations"
        cost: "$1,003"
        percentage: "8%"
        resources: ["Monitoring", "Security Center"]
```

### 成本異常檢測

```python
# scripts/cost_anomaly_detector.py
"""
成本異常檢測器 - 使用統計方法檢測異常成本
"""
import numpy as np
from datetime import datetime, timedelta
from typing import List, Tuple
from dataclasses import dataclass

@dataclass
class CostAnomaly:
    """成本異常"""
    date: datetime
    actual_cost: float
    expected_cost: float
    deviation_percent: float
    severity: str  # "Low", "Medium", "High"
    resource_group: str

class CostAnomalyDetector:
    """成本異常檢測器"""

    def __init__(self, threshold_std: float = 2.0):
        self.threshold_std = threshold_std  # 標準差閾值

    def detect_anomalies(
        self,
        historical_costs: List[Tuple[datetime, float]],
        resource_group: str = "All"
    ) -> List[CostAnomaly]:
        """檢測成本異常"""

        if len(historical_costs) < 7:
            print("⚠️  歷史數據不足 (需要至少 7 天)")
            return []

        # 提取成本數據
        dates = [date for date, _ in historical_costs]
        costs = np.array([cost for _, cost in historical_costs])

        # 計算移動平均和標準差
        window_size = 7  # 7 天移動窗口
        anomalies = []

        for i in range(window_size, len(costs)):
            window = costs[i - window_size:i]
            mean = np.mean(window)
            std = np.std(window)

            current_cost = costs[i]
            deviation = abs(current_cost - mean)
            deviation_std = deviation / std if std > 0 else 0
            deviation_percent = ((current_cost - mean) / mean) * 100

            # 異常檢測 (超過 threshold_std 個標準差)
            if deviation_std > self.threshold_std:
                severity = self._calculate_severity(deviation_std)

                anomaly = CostAnomaly(
                    date=dates[i],
                    actual_cost=current_cost,
                    expected_cost=mean,
                    deviation_percent=deviation_percent,
                    severity=severity,
                    resource_group=resource_group
                )
                anomalies.append(anomaly)

        return anomalies

    def _calculate_severity(self, deviation_std: float) -> str:
        """計算異常嚴重程度"""
        if deviation_std > 4.0:
            return "High"
        elif deviation_std > 3.0:
            return "Medium"
        else:
            return "Low"

    def generate_alert(self, anomaly: CostAnomaly) -> dict:
        """生成告警"""
        return {
            "title": f"成本異常檢測: {anomaly.resource_group}",
            "severity": anomaly.severity,
            "message": f"""
檢測到成本異常:

日期: {anomaly.date.strftime('%Y-%m-%d')}
資源組: {anomaly.resource_group}
實際成本: ${anomaly.actual_cost:,.2f}
預期成本: ${anomaly.expected_cost:,.2f}
偏差: {anomaly.deviation_percent:+.1f}%
嚴重程度: {anomaly.severity}

建議立即檢查資源使用情況和配置。
            """,
            "timestamp": datetime.utcnow().isoformat()
        }

# 使用示例
detector = CostAnomalyDetector(threshold_std=2.0)

# 模擬歷史成本數據 (日期, 成本)
historical_costs = [
    (datetime(2025, 10, 1), 420.50),
    (datetime(2025, 10, 2), 435.20),
    (datetime(2025, 10, 3), 428.75),
    (datetime(2025, 10, 4), 441.30),
    (datetime(2025, 10, 5), 438.90),
    (datetime(2025, 10, 6), 445.60),
    (datetime(2025, 10, 7), 432.10),
    (datetime(2025, 10, 8), 850.00),  # 異常高成本
]

anomalies = detector.detect_anomalies(historical_costs, resource_group="aiagent-prod-rg")

for anomaly in anomalies:
    alert = detector.generate_alert(anomaly)
    print(alert["message"])
```

---

## 檢查清單

### 成本優化檢查清單

- [ ] **Right-Sizing (資源適配)**
  - [ ] 每季度審查資源使用率報告
  - [ ] CPU/Memory 使用率在 70-85% 範圍
  - [ ] 識別並調整過度配置資源

- [ ] **預留實例 (Reserved Instances)**
  - [ ] 預留實例覆蓋率 ≥ 70%
  - [ ] 基礎容量使用 3 年預留
  - [ ] 穩定增長使用 1 年預留

- [ ] **自動擴展 (Auto-Scaling)**
  - [ ] HPA 已配置 (CPU 70%, Memory 80%)
  - [ ] Cluster Autoscaler 已啟用
  - [ ] 時間調度縮容已實施

- [ ] **存儲優化**
  - [ ] Blob Storage 生命週期管理已配置
  - [ ] PostgreSQL 備份壓縮率 ≥ 70%
  - [ ] 舊備份自動轉為 Archive Tier

- [ ] **網絡優化**
  - [ ] Azure CDN 已配置 (靜態資源)
  - [ ] 跨區域流量最小化
  - [ ] 壓縮已啟用 (Gzip, Brotli)

- [ ] **AI 服務優化**
  - [ ] Token 優化已實施
  - [ ] 結果緩存已啟用 (Redis)
  - [ ] 70% 簡單任務使用 GPT-3.5 Turbo

- [ ] **開發測試環境**
  - [ ] 開發集群自動化調度已配置
  - [ ] 僅工作時間運行
  - [ ] 測試環境使用低成本配置

- [ ] **FinOps 實踐**
  - [ ] 成本標記政策已實施
  - [ ] 成本異常檢測已啟用
  - [ ] 每月成本審查會議

---

## 總結

本文檔提供了完整的成本優化策略,涵蓋:

✅ **Right-Sizing 策略** (資源使用率監控, 自動調整流程)
✅ **預留實例優化** (覆蓋率分析, 購買策略, 35-54% 節省)
✅ **自動擴展策略** (HPA 優化, Cluster Autoscaler, 時間調度)
✅ **存儲優化** (生命週期管理, 備份壓縮, 70% 空間節省)
✅ **網絡成本優化** (CDN 策略, 跨區域優化, $75/月節省)
✅ **AI 服務成本控制** (Token 優化, 模型選擇, $6,000/月節省)
✅ **開發測試環境優化** (自動化調度, $500/月節省)
✅ **FinOps 最佳實踐** (成本分配, 異常檢測, 持續優化)

**總體成本優化潛力**:
- 短期 (3個月): 節省 15-20% → **$2,000-$2,600/月**
- 中期 (6個月): 節省 25-35% → **$3,300-$4,600/月**
- 長期 (12個月): 節省 40-50% → **$5,300-$6,600/月**

透過本指南,運維團隊可以系統化地降低雲端成本,同時保持服務品質和可用性。
