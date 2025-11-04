# 財務預測分析

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的全面財務預測分析,涵蓋:
- **成本預測模型** (線性回歸, 時間序列, 機器學習)
- **收入預測** (基於用戶增長的收入模型)
- **現金流預測** (月度和季度現金流)
- **盈虧平衡分析** (Break-Even Point 計算)
- **財務比率分析** (毛利率, 淨利率, ROI)
- **情景分析** (樂觀, 基準, 悲觀情景)
- **敏感度分析** (關鍵變量影響)
- **投資決策支持** (NPV, IRR, Payback Period)

### 預測目標

**財務健康指標**:
- ✅ **預測準確性**: 實際 vs. 預測偏差 ≤ 15%
- ✅ **盈虧平衡**: 達到盈虧平衡點時間 ≤ 18 個月
- ✅ **毛利率目標**: ≥ 70% (SaaS 行業標準)
- ✅ **淨利率目標**: ≥ 20% (長期目標)
- ✅ **現金流健康**: 正向現金流 ≥ 6 個月運營成本

**預測時間範圍**:
- 短期預測 (1-3 個月): 高精度 (±5%)
- 中期預測 (3-12 個月): 中等精度 (±10%)
- 長期預測 (1-3 年): 趨勢分析 (±20%)

---

## 成本預測模型

### 基於歷史數據的線性回歸模型

```python
# scripts/cost_forecasting_models.py
"""
成本預測模型 - 使用多種統計和機器學習方法
"""
import numpy as np
import pandas as pd
from sklearn.linear_model import LinearRegression
from sklearn.ensemble import RandomForestRegressor
from datetime import datetime, timedelta
from typing import List, Tuple
from dataclasses import dataclass

@dataclass
class CostForecast:
    """成本預測結果"""
    date: datetime
    predicted_cost: float
    confidence_lower: float  # 95% 置信區間下限
    confidence_upper: float  # 95% 置信區間上限
    actual_cost: float = None  # 實際成本 (用於驗證)

class CostForecastingEngine:
    """成本預測引擎"""

    def __init__(self, historical_data: pd.DataFrame):
        """
        初始化預測引擎

        Args:
            historical_data: 包含 'date' 和 'cost' 列的 DataFrame
        """
        self.historical_data = historical_data
        self.linear_model = None
        self.rf_model = None

    def prepare_features(self, df: pd.DataFrame) -> Tuple[np.ndarray, np.ndarray]:
        """準備特徵工程"""
        # 添加時間特徵
        df['days_since_start'] = (df['date'] - df['date'].min()).dt.days
        df['month'] = df['date'].dt.month
        df['day_of_week'] = df['date'].dt.dayofweek
        df['week_of_year'] = df['date'].dt.isocalendar().week

        # 添加滯後特徵 (過去 7 天平均成本)
        df['cost_lag7'] = df['cost'].rolling(window=7).mean()
        df['cost_lag30'] = df['cost'].rolling(window=30).mean()

        # 移除 NaN 值
        df_clean = df.dropna()

        X = df_clean[['days_since_start', 'month', 'day_of_week', 'week_of_year', 'cost_lag7', 'cost_lag30']].values
        y = df_clean['cost'].values

        return X, y

    def train_linear_model(self):
        """訓練線性回歸模型"""
        X, y = self.prepare_features(self.historical_data.copy())

        self.linear_model = LinearRegression()
        self.linear_model.fit(X, y)

        # 計算訓練誤差
        y_pred = self.linear_model.predict(X)
        mae = np.mean(np.abs(y - y_pred))
        rmse = np.sqrt(np.mean((y - y_pred) ** 2))

        print(f"線性回歸模型訓練完成:")
        print(f"  MAE: ${mae:.2f}")
        print(f"  RMSE: ${rmse:.2f}")

    def train_random_forest_model(self):
        """訓練隨機森林模型 (捕捉非線性關係)"""
        X, y = self.prepare_features(self.historical_data.copy())

        self.rf_model = RandomForestRegressor(
            n_estimators=100,
            max_depth=10,
            random_state=42
        )
        self.rf_model.fit(X, y)

        # 計算訓練誤差
        y_pred = self.rf_model.predict(X)
        mae = np.mean(np.abs(y - y_pred))
        rmse = np.sqrt(np.mean((y - y_pred) ** 2))

        print(f"隨機森林模型訓練完成:")
        print(f"  MAE: ${mae:.2f}")
        print(f"  RMSE: ${rmse:.2f}")

    def forecast(
        self,
        days_ahead: int = 90,
        model_type: str = "linear"
    ) -> List[CostForecast]:
        """生成成本預測"""

        if model_type == "linear" and self.linear_model is None:
            raise ValueError("線性模型未訓練,請先調用 train_linear_model()")
        elif model_type == "rf" and self.rf_model is None:
            raise ValueError("隨機森林模型未訓練,請先調用 train_random_forest_model()")

        model = self.linear_model if model_type == "linear" else self.rf_model

        forecasts = []
        last_date = self.historical_data['date'].max()

        # 準備未來日期
        future_dates = [last_date + timedelta(days=i) for i in range(1, days_ahead + 1)]

        # 準備特徵
        for date in future_dates:
            days_since_start = (date - self.historical_data['date'].min()).days
            month = date.month
            day_of_week = date.weekday()
            week_of_year = date.isocalendar()[1]

            # 使用最近的歷史數據作為滯後特徵
            recent_costs = self.historical_data.tail(30)['cost'].values
            cost_lag7 = np.mean(recent_costs[-7:])
            cost_lag30 = np.mean(recent_costs)

            X_future = np.array([[days_since_start, month, day_of_week, week_of_year, cost_lag7, cost_lag30]])

            # 預測
            predicted_cost = model.predict(X_future)[0]

            # 計算置信區間 (簡化,實際應使用 prediction intervals)
            std_error = 50  # 假設標準誤差 $50
            confidence_lower = predicted_cost - (1.96 * std_error)
            confidence_upper = predicted_cost + (1.96 * std_error)

            forecasts.append(CostForecast(
                date=date,
                predicted_cost=predicted_cost,
                confidence_lower=max(0, confidence_lower),  # 成本不能為負
                confidence_upper=confidence_upper
            ))

        return forecasts

    def evaluate_accuracy(self, actual_costs: List[Tuple[datetime, float]]) -> dict:
        """評估預測準確性"""
        # 與實際成本比較
        errors = []
        percentage_errors = []

        for actual_date, actual_cost in actual_costs:
            # 找到對應的預測
            forecast = next((f for f in self.last_forecast if f.date == actual_date), None)

            if forecast:
                error = actual_cost - forecast.predicted_cost
                percentage_error = (error / actual_cost) * 100

                errors.append(abs(error))
                percentage_errors.append(abs(percentage_error))

        mae = np.mean(errors) if errors else 0
        mape = np.mean(percentage_errors) if percentage_errors else 0

        return {
            "mae": mae,
            "mape": mape,
            "accuracy": 100 - mape
        }

# 使用示例
# 準備歷史數據
dates = pd.date_range(start='2024-01-01', end='2024-12-31', freq='D')
np.random.seed(42)
costs = 400 + np.random.randn(len(dates)) * 50 + (np.arange(len(dates)) * 0.5)  # 模擬上升趨勢

historical_data = pd.DataFrame({
    'date': dates,
    'cost': costs
})

# 訓練模型
engine = CostForecastingEngine(historical_data)
engine.train_linear_model()
engine.train_random_forest_model()

# 生成 90 天預測
forecasts_linear = engine.forecast(days_ahead=90, model_type="linear")
forecasts_rf = engine.forecast(days_ahead=90, model_type="rf")

print(f"\n預測未來 90 天成本:")
print(f"線性模型平均預測: ${np.mean([f.predicted_cost for f in forecasts_linear]):,.2f}/天")
print(f"隨機森林平均預測: ${np.mean([f.predicted_cost for f in forecasts_rf]):,.2f}/天")
```

### 時間序列 ARIMA 模型

```python
# scripts/arima_cost_forecasting.py
"""
ARIMA 時間序列成本預測
"""
from statsmodels.tsa.arima.model import ARIMA
from statsmodels.graphics.tsaplots import plot_acf, plot_pacf
import pandas as pd
import numpy as np

class ARIMACostForecaster:
    """ARIMA 成本預測器"""

    def __init__(self, historical_costs: pd.Series):
        """
        Args:
            historical_costs: 時間序列成本數據 (DatetimeIndex)
        """
        self.historical_costs = historical_costs
        self.model = None
        self.fitted_model = None

    def find_optimal_parameters(self) -> Tuple[int, int, int]:
        """尋找最佳 ARIMA (p, d, q) 參數"""
        # 使用 AIC (Akaike Information Criterion) 選擇最佳參數
        best_aic = np.inf
        best_params = None

        # 網格搜索
        for p in range(0, 4):
            for d in range(0, 2):
                for q in range(0, 4):
                    try:
                        model = ARIMA(self.historical_costs, order=(p, d, q))
                        fitted = model.fit()

                        if fitted.aic < best_aic:
                            best_aic = fitted.aic
                            best_params = (p, d, q)
                    except:
                        continue

        print(f"最佳 ARIMA 參數: {best_params}, AIC: {best_aic:.2f}")
        return best_params

    def train(self, order: Tuple[int, int, int] = None):
        """訓練 ARIMA 模型"""
        if order is None:
            order = self.find_optimal_parameters()

        self.model = ARIMA(self.historical_costs, order=order)
        self.fitted_model = self.model.fit()

        print(f"ARIMA{order} 模型訓練完成")
        print(self.fitted_model.summary())

    def forecast(self, steps: int = 90) -> pd.DataFrame:
        """生成預測"""
        if self.fitted_model is None:
            raise ValueError("模型未訓練,請先調用 train()")

        forecast = self.fitted_model.forecast(steps=steps)
        forecast_ci = self.fitted_model.get_forecast(steps=steps).conf_int()

        forecast_df = pd.DataFrame({
            'predicted_cost': forecast,
            'lower_ci': forecast_ci.iloc[:, 0],
            'upper_ci': forecast_ci.iloc[:, 1]
        })

        return forecast_df

# 使用示例
dates = pd.date_range(start='2024-01-01', end='2024-12-31', freq='D')
costs = pd.Series(
    400 + np.random.randn(len(dates)) * 50 + (np.arange(len(dates)) * 0.5),
    index=dates
)

forecaster = ARIMACostForecaster(costs)
forecaster.train()
forecast_df = forecaster.forecast(steps=90)

print(f"\n未來 90 天 ARIMA 預測:")
print(f"平均預測成本: ${forecast_df['predicted_cost'].mean():,.2f}/天")
print(f"95% 置信區間: ${forecast_df['lower_ci'].mean():,.2f} - ${forecast_df['upper_ci'].mean():,.2f}")
```

---

## 收入預測模型

### 基於用戶增長的收入預測

```python
# scripts/revenue_forecasting.py
"""
收入預測模型 - 基於用戶增長和定價策略
"""
from dataclasses import dataclass
from typing import List
import numpy as np

@dataclass
class PricingTier:
    """定價層級"""
    name: str
    monthly_price: float
    features: List[str]

@dataclass
class RevenueForecast:
    """收入預測"""
    month: int
    users: int
    users_by_tier: dict  # tier_name -> user_count
    mrr: float  # Monthly Recurring Revenue
    arr: float  # Annual Recurring Revenue
    churn_rate: float
    net_new_mrr: float

class RevenueForecaster:
    """收入預測器"""

    def __init__(self, pricing_tiers: List[PricingTier]):
        self.pricing_tiers = {tier.name: tier for tier in pricing_tiers}

    def forecast_revenue(
        self,
        initial_users: int,
        monthly_growth_rate: float,
        user_distribution: dict,  # tier_name -> percentage
        churn_rate: float = 0.05,  # 5% 月流失率
        months: int = 24
    ) -> List[RevenueForecast]:
        """預測未來收入"""

        forecasts = []
        current_users = initial_users

        for month in range(months + 1):
            # 計算用戶分佈
            users_by_tier = {}
            for tier_name, percentage in user_distribution.items():
                users_by_tier[tier_name] = int(current_users * percentage)

            # 計算 MRR (Monthly Recurring Revenue)
            mrr = sum(
                users_by_tier.get(tier_name, 0) * self.pricing_tiers[tier_name].monthly_price
                for tier_name in self.pricing_tiers.keys()
            )

            # 計算 ARR (Annual Recurring Revenue)
            arr = mrr * 12

            # 計算 Net New MRR (考慮流失和新增)
            if month > 0:
                previous_mrr = forecasts[-1].mrr
                churned_mrr = previous_mrr * churn_rate
                new_user_mrr = mrr - (previous_mrr - churned_mrr)
                net_new_mrr = new_user_mrr
            else:
                net_new_mrr = mrr

            forecasts.append(RevenueForecast(
                month=month,
                users=current_users,
                users_by_tier=users_by_tier.copy(),
                mrr=mrr,
                arr=arr,
                churn_rate=churn_rate,
                net_new_mrr=net_new_mrr
            ))

            # 下個月用戶增長 (減去流失)
            new_users = int(current_users * monthly_growth_rate)
            churned_users = int(current_users * churn_rate)
            current_users = current_users + new_users - churned_users

        return forecasts

    def generate_report(self, forecasts: List[RevenueForecast]) -> str:
        """生成收入預測報告"""
        report = "=== 收入預測報告 ===\n\n"

        # 關鍵指標
        initial_mrr = forecasts[0].mrr
        final_mrr = forecasts[-1].mrr
        mrr_growth = ((final_mrr - initial_mrr) / initial_mrr) * 100

        report += f"初始 MRR: ${initial_mrr:,.2f}\n"
        report += f"{len(forecasts)-1} 個月後 MRR: ${final_mrr:,.2f}\n"
        report += f"MRR 增長: {mrr_growth:.1f}%\n"
        report += f"最終 ARR: ${forecasts[-1].arr:,.2f}\n\n"

        # 季度摘要
        report += "季度摘要:\n"
        report += "="*60 + "\n"

        for quarter in range(0, len(forecasts), 3):
            if quarter >= len(forecasts):
                break

            forecast = forecasts[quarter]
            report += f"\n第 {quarter//3 + 1} 季度 (月份 {forecast.month}):\n"
            report += f"  用戶數: {forecast.users:,}\n"
            report += f"  MRR: ${forecast.mrr:,.2f}\n"
            report += f"  ARR: ${forecast.arr:,.2f}\n"

            # 用戶分佈
            report += f"  用戶分佈:\n"
            for tier_name, count in forecast.users_by_tier.items():
                percentage = (count / forecast.users) * 100 if forecast.users > 0 else 0
                report += f"    {tier_name}: {count:,} ({percentage:.1f}%)\n"

        return report

# 使用示例
pricing_tiers = [
    PricingTier(name="Free", monthly_price=0, features=["基礎功能"]),
    PricingTier(name="Starter", monthly_price=29, features=["進階功能"]),
    PricingTier(name="Professional", monthly_price=99, features=["專業功能"]),
    PricingTier(name="Enterprise", monthly_price=499, features=["企業功能"])
]

forecaster = RevenueForecaster(pricing_tiers)

forecasts = forecaster.forecast_revenue(
    initial_users=5000,
    monthly_growth_rate=0.10,  # 10% 月增長率
    user_distribution={
        "Free": 0.50,
        "Starter": 0.30,
        "Professional": 0.15,
        "Enterprise": 0.05
    },
    churn_rate=0.05,  # 5% 流失率
    months=24
)

print(forecaster.generate_report(forecasts))
```

---

## 現金流預測

### 月度現金流模型

```csharp
// src/Financial/CashFlowForecaster.cs
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CashFlowForecaster
{
    public record CashFlowStatement(
        int Month,
        decimal Revenue,
        decimal CostOfRevenue,
        decimal OperatingExpenses,
        decimal CapitalExpenditures,
        decimal CashInflow,
        decimal CashOutflow,
        decimal NetCashFlow,
        decimal CashBalance
    );

    public List<CashFlowStatement> ForecastCashFlow(
        decimal initialCashBalance,
        List<decimal> monthlyRevenue,
        decimal costOfRevenuePercentage,
        decimal monthlyOperatingExpenses,
        decimal monthlyCapex,
        int months)
    {
        var statements = new List<CashFlowStatement>();
        var currentCashBalance = initialCashBalance;

        for (int month = 0; month < months; month++)
        {
            // 現金流入 (收入)
            var revenue = monthlyRevenue[month];
            var cashInflow = revenue;

            // 現金流出
            var costOfRevenue = revenue * costOfRevenuePercentage;
            var operatingExpenses = monthlyOperatingExpenses;
            var capex = monthlyCapex;
            var cashOutflow = costOfRevenue + operatingExpenses + capex;

            // 淨現金流
            var netCashFlow = cashInflow - cashOutflow;

            // 更新現金餘額
            currentCashBalance += netCashFlow;

            statements.Add(new CashFlowStatement(
                Month: month,
                Revenue: revenue,
                CostOfRevenue: costOfRevenue,
                OperatingExpenses: operatingExpenses,
                CapitalExpenditures: capex,
                CashInflow: cashInflow,
                CashOutflow: cashOutflow,
                NetCashFlow: netCashFlow,
                CashBalance: currentCashBalance
            ));
        }

        return statements;
    }

    public void GenerateReport(List<CashFlowStatement> statements)
    {
        Console.WriteLine("=== 現金流預測報告 ===\n");
        Console.WriteLine($"{"月份",-8} {"收入",-12} {"成本",-12} {"營運費用",-12} {"資本支出",-12} {"淨現金流",-12} {"現金餘額",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var statement in statements)
        {
            Console.WriteLine($"{statement.Month,-8} ${statement.Revenue,-11:N0} ${statement.CostOfRevenue,-11:N0} ${statement.OperatingExpenses,-11:N0} ${statement.CapitalExpenditures,-11:N0} ${statement.NetCashFlow,-11:N0} ${statement.CashBalance,-11:N0}");
        }

        // 摘要統計
        var totalRevenue = statements.Sum(s => s.Revenue);
        var totalCashOutflow = statements.Sum(s => s.CashOutflow);
        var totalNetCashFlow = statements.Sum(s => s.NetCashFlow);
        var finalCashBalance = statements.Last().CashBalance;

        var monthsOfRunway = statements.Count(s => s.CashBalance > 0);

        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"\n總收入: ${totalRevenue:N2}");
        Console.WriteLine($"總現金流出: ${totalCashOutflow:N2}");
        Console.WriteLine($"總淨現金流: ${totalNetCashFlow:N2}");
        Console.WriteLine($"最終現金餘額: ${finalCashBalance:N2}");
        Console.WriteLine($"現金跑道: {monthsOfRunway} 個月");

        // 現金流健康警告
        if (finalCashBalance < 0)
        {
            Console.WriteLine($"\n⚠️  警告: 預測現金流為負,需要額外融資");
        }
        else if (monthsOfRunway < 6)
        {
            Console.WriteLine($"\n⚠️  警告: 現金跑道少於 6 個月,建議開始融資準備");
        }
        else
        {
            Console.WriteLine($"\n✅ 現金流健康,現金跑道充足");
        }
    }
}

// 使用示例
var forecaster = new CashFlowForecaster();

// 模擬 24 個月收入增長
var monthlyRevenue = Enumerable.Range(0, 24)
    .Select(month => 10000m * (decimal)Math.Pow(1.15, month))  // 15% 月增長
    .ToList();

var statements = forecaster.ForecastCashFlow(
    initialCashBalance: 500000m,  // 初始現金 $500K
    monthlyRevenue: monthlyRevenue,
    costOfRevenuePercentage: 0.30m,  // 30% 收入成本
    monthlyOperatingExpenses: 50000m,  // 月營運費用 $50K
    monthlyCapex: 10000m,  // 月資本支出 $10K
    months: 24
);

forecaster.GenerateReport(statements);
```

---

## 盈虧平衡分析

### Break-Even Point 計算

```yaml
break_even_analysis:
  assumptions:
    fixed_costs_monthly: "$50,000"  # 人員工資、租金、固定軟件許可
    variable_cost_per_user: "$0.35"  # 雲端成本、客戶支持
    revenue_per_user_monthly: "$10"  # ARPU (Average Revenue Per User)

  calculation:
    # Break-Even Point (用戶數) = Fixed Costs / (Revenue Per User - Variable Cost Per User)
    formula: "BEP = FC / (R - VC)"
    break_even_users: 5181  # $50,000 / ($10 - $0.35) = 5,181 用戶
    break_even_mrr: "$51,810"  # 5,181 × $10

  timeline_projection:
    - month: 0
      users: 5000
      status: "未達到盈虧平衡"
      gap_users: -181

    - month: 3
      users: 6655
      status: "已達到盈虧平衡"
      profit_monthly: "$14,394"

    - month: 6
      users: 8863
      status: "盈利模式"
      profit_monthly: "$35,572"

    - month: 12
      users: 15573
      status: "規模盈利"
      profit_monthly: "$100,388"

  sensitivity_analysis:
    - scenario: "提高 ARPU 到 $12"
      new_break_even_users: 4292
      impact: "降低盈虧平衡點 17%"

    - scenario: "降低變動成本到 $0.25"
      new_break_even_users: 5128
      impact: "降低盈虧平衡點 1%"

    - scenario: "降低固定成本到 $40K"
      new_break_even_users: 4145
      impact: "降低盈虧平衡點 20%"
```

---

## 財務比率分析

### 關鍵財務比率計算

```python
# scripts/financial_ratios.py
"""
財務比率分析 - SaaS 行業關鍵指標
"""
from dataclasses import dataclass
from typing import List

@dataclass
class FinancialRatios:
    """財務比率"""
    month: int
    # 盈利能力比率
    gross_margin: float  # 毛利率
    operating_margin: float  # 營運利潤率
    net_margin: float  # 淨利率

    # 增長比率
    revenue_growth_rate: float  # 收入增長率
    user_growth_rate: float  # 用戶增長率

    # SaaS 專用指標
    ltv_cac_ratio: float  # LTV/CAC 比率 (應 > 3)
    magic_number: float  # Magic Number (應 > 0.75)
    net_dollar_retention: float  # NDR (應 > 100%)
    rule_of_40: float  # Rule of 40 (增長率 + 利潤率, 應 > 40)

class FinancialRatiosCalculator:
    """財務比率計算器"""

    @staticmethod
    def calculate_ratios(
        revenue: float,
        cost_of_revenue: float,
        operating_expenses: float,
        users: int,
        previous_revenue: float = None,
        previous_users: int = None,
        ltv: float = None,
        cac: float = None
    ) -> FinancialRatios:
        """計算財務比率"""

        # 盈利能力比率
        gross_profit = revenue - cost_of_revenue
        gross_margin = (gross_profit / revenue) * 100 if revenue > 0 else 0

        operating_profit = gross_profit - operating_expenses
        operating_margin = (operating_profit / revenue) * 100 if revenue > 0 else 0

        net_profit = operating_profit  # 簡化,未考慮稅和利息
        net_margin = (net_profit / revenue) * 100 if revenue > 0 else 0

        # 增長比率
        revenue_growth_rate = 0
        if previous_revenue:
            revenue_growth_rate = ((revenue - previous_revenue) / previous_revenue) * 100

        user_growth_rate = 0
        if previous_users:
            user_growth_rate = ((users - previous_users) / previous_users) * 100

        # SaaS 專用指標
        ltv_cac_ratio = ltv / cac if ltv and cac and cac > 0 else 0

        # Magic Number = (本季新增 ARR) / (上季銷售和市場費用)
        # 簡化計算
        magic_number = 0.85  # 假設值

        # Net Dollar Retention (NDR)
        # NDR = (期初 ARR + 擴張收入 - 流失收入) / 期初 ARR
        ndr = 110  # 假設 110%

        # Rule of 40 = 收入增長率 + 利潤率
        rule_of_40 = revenue_growth_rate + net_margin

        return FinancialRatios(
            month=0,
            gross_margin=gross_margin,
            operating_margin=operating_margin,
            net_margin=net_margin,
            revenue_growth_rate=revenue_growth_rate,
            user_growth_rate=user_growth_rate,
            ltv_cac_ratio=ltv_cac_ratio,
            magic_number=magic_number,
            net_dollar_retention=ndr,
            rule_of_40=rule_of_40
        )

    @staticmethod
    def generate_report(ratios: FinancialRatios) -> str:
        """生成財務比率報告"""
        report = f"""
=== 財務比率分析報告 ===

盈利能力指標:
  毛利率 (Gross Margin): {ratios.gross_margin:.1f}%
    目標: ≥ 70% (SaaS 標準)
    狀態: {'✅ 達標' if ratios.gross_margin >= 70 else '❌ 未達標'}

  營運利潤率 (Operating Margin): {ratios.operating_margin:.1f}%
    目標: ≥ 15%
    狀態: {'✅ 達標' if ratios.operating_margin >= 15 else '❌ 未達標'}

  淨利率 (Net Margin): {ratios.net_margin:.1f}%
    目標: ≥ 20%
    狀態: {'✅ 達標' if ratios.net_margin >= 20 else '❌ 未達標'}

增長指標:
  收入增長率: {ratios.revenue_growth_rate:.1f}%
  用戶增長率: {ratios.user_growth_rate:.1f}%

SaaS 專用指標:
  LTV/CAC 比率: {ratios.ltv_cac_ratio:.2f}
    目標: ≥ 3.0
    狀態: {'✅ 達標' if ratios.ltv_cac_ratio >= 3.0 else '❌ 未達標'}

  Magic Number: {ratios.magic_number:.2f}
    目標: ≥ 0.75
    狀態: {'✅ 達標' if ratios.magic_number >= 0.75 else '❌ 未達標'}

  Net Dollar Retention: {ratios.net_dollar_retention:.1f}%
    目標: ≥ 100%
    狀態: {'✅ 達標' if ratios.net_dollar_retention >= 100 else '❌ 未達標'}

  Rule of 40: {ratios.rule_of_40:.1f}
    目標: ≥ 40
    狀態: {'✅ 達標' if ratios.rule_of_40 >= 40 else '❌ 未達標'}

總體健康度: {'✅ 優秀' if ratios.rule_of_40 >= 50 and ratios.gross_margin >= 70 else '⚠️  需改進'}
"""
        return report

# 使用示例
calculator = FinancialRatiosCalculator()

ratios = calculator.calculate_ratios(
    revenue=100000,
    cost_of_revenue=30000,
    operating_expenses=50000,
    users=10000,
    previous_revenue=85000,
    previous_users=8500,
    ltv=500,  # 客戶生命週期價值 $500
    cac=150   # 客戶獲取成本 $150
)

print(calculator.generate_report(ratios))
```

---

## 情景分析

### 樂觀、基準、悲觀情景

```yaml
scenario_analysis:
  base_case:
    name: "基準情景"
    assumptions:
      user_growth_rate: "10% 月增長"
      churn_rate: "5% 月流失"
      arpu: "$10/月"
      gross_margin: "70%"

    12_month_projection:
      users: 15573
      mrr: "$155,730"
      arr: "$1,868,760"
      net_margin: "20%"

  optimistic_case:
    name: "樂觀情景"
    assumptions:
      user_growth_rate: "15% 月增長"
      churn_rate: "3% 月流失"
      arpu: "$12/月"
      gross_margin: "75%"

    12_month_projection:
      users: 24780
      mrr: "$297,360"
      arr: "$3,568,320"
      net_margin: "30%"

  pessimistic_case:
    name: "悲觀情景"
    assumptions:
      user_growth_rate: "5% 月增長"
      churn_rate: "8% 月流失"
      arpu: "$8/月"
      gross_margin: "60%"

    12_month_projection:
      users: 8830
      mrr: "$70,640"
      arr: "$847,680"
      net_margin: "5%"

  probability_weighted_forecast:
    optimistic_probability: 0.25
    base_case_probability: 0.50
    pessimistic_probability: 0.25

    weighted_arr:
      calculation: "(0.25 × $3.57M) + (0.50 × $1.87M) + (0.25 × $0.85M)"
      result: "$1,838,380"
```

---

## 敏感度分析

### 關鍵變量影響分析

```python
# scripts/sensitivity_analysis.py
"""
敏感度分析 - 評估關鍵變量對財務結果的影響
"""
import numpy as np
import pandas as pd

class SensitivityAnalyzer:
    """敏感度分析器"""

    @staticmethod
    def analyze_revenue_sensitivity(
        base_users: int,
        base_arpu: float,
        base_growth_rate: float,
        base_churn_rate: float,
        months: int = 12
    ) -> pd.DataFrame:
        """分析收入對各變量的敏感度"""

        results = []

        # 測試變量範圍
        arpu_variations = np.arange(base_arpu * 0.8, base_arpu * 1.2, base_arpu * 0.1)
        growth_variations = np.arange(base_growth_rate * 0.5, base_growth_rate * 1.5, base_growth_rate * 0.25)
        churn_variations = np.arange(base_churn_rate * 0.5, base_churn_rate * 1.5, base_churn_rate * 0.25)

        # ARPU 敏感度
        for arpu in arpu_variations:
            final_revenue = SensitivityAnalyzer._calculate_final_revenue(
                base_users, arpu, base_growth_rate, base_churn_rate, months
            )
            results.append({
                'variable': 'ARPU',
                'value': arpu,
                'change_pct': ((arpu - base_arpu) / base_arpu) * 100,
                'final_revenue': final_revenue
            })

        # 增長率敏感度
        for growth in growth_variations:
            final_revenue = SensitivityAnalyzer._calculate_final_revenue(
                base_users, base_arpu, growth, base_churn_rate, months
            )
            results.append({
                'variable': 'Growth Rate',
                'value': growth,
                'change_pct': ((growth - base_growth_rate) / base_growth_rate) * 100,
                'final_revenue': final_revenue
            })

        # 流失率敏感度
        for churn in churn_variations:
            final_revenue = SensitivityAnalyzer._calculate_final_revenue(
                base_users, base_arpu, base_growth_rate, churn, months
            )
            results.append({
                'variable': 'Churn Rate',
                'value': churn,
                'change_pct': ((churn - base_churn_rate) / base_churn_rate) * 100,
                'final_revenue': final_revenue
            })

        df = pd.DataFrame(results)
        return df

    @staticmethod
    def _calculate_final_revenue(users, arpu, growth_rate, churn_rate, months):
        """計算最終收入"""
        current_users = users

        for month in range(months):
            new_users = int(current_users * growth_rate)
            churned_users = int(current_users * churn_rate)
            current_users = current_users + new_users - churned_users

        return current_users * arpu

    @staticmethod
    def generate_tornado_chart_data(df: pd.DataFrame) -> dict:
        """生成龍捲風圖數據"""
        base_revenue = df[df['change_pct'] == 0]['final_revenue'].values[0] if len(df[df['change_pct'] == 0]) > 0 else df['final_revenue'].median()

        tornado_data = {}

        for variable in df['variable'].unique():
            var_df = df[df['variable'] == variable].copy()
            var_df['revenue_change'] = var_df['final_revenue'] - base_revenue
            var_df['revenue_change_pct'] = (var_df['revenue_change'] / base_revenue) * 100

            max_impact = var_df.loc[var_df['revenue_change_pct'].abs().idxmax()]

            tornado_data[variable] = {
                'max_impact_pct': float(max_impact['revenue_change_pct']),
                'max_impact_value': float(max_impact['revenue_change'])
            }

        # 按影響大小排序
        sorted_data = dict(sorted(tornado_data.items(), key=lambda x: abs(x[1]['max_impact_pct']), reverse=True))

        return sorted_data

# 使用示例
analyzer = SensitivityAnalyzer()

sensitivity_df = analyzer.analyze_revenue_sensitivity(
    base_users=5000,
    base_arpu=10.0,
    base_growth_rate=0.10,
    base_churn_rate=0.05,
    months=12
)

tornado_data = analyzer.generate_tornado_chart_data(sensitivity_df)

print("=== 敏感度分析 - 龍捲風圖數據 ===\n")
print(f"{'變量':<20} {'最大影響 (%)':<15} {'最大影響 ($)':<15}")
print("-" * 50)

for variable, impact in tornado_data.items():
    print(f"{variable:<20} {impact['max_impact_pct']:>14.1f}% {impact['max_impact_value']:>14,.0f}")
```

---

## 投資決策支持

### NPV, IRR, Payback Period 計算

```csharp
// src/Financial/InvestmentAnalyzer.cs
public sealed class InvestmentAnalyzer
{
    public record InvestmentScenario(
        string Name,
        decimal InitialInvestment,
        List<decimal> CashFlows,  // 每期現金流
        decimal DiscountRate
    );

    public record InvestmentMetrics(
        decimal NPV,
        double IRR,
        double PaybackPeriod,
        string Recommendation
    );

    public InvestmentMetrics AnalyzeInvestment(InvestmentScenario scenario)
    {
        // 計算 NPV (Net Present Value)
        var npv = CalculateNPV(scenario.CashFlows, scenario.DiscountRate, scenario.InitialInvestment);

        // 計算 IRR (Internal Rate of Return)
        var irr = CalculateIRR(scenario.CashFlows, scenario.InitialInvestment);

        // 計算 Payback Period
        var paybackPeriod = CalculatePaybackPeriod(scenario.CashFlows, scenario.InitialInvestment);

        // 生成投資建議
        var recommendation = GenerateRecommendation(npv, irr, paybackPeriod, scenario.DiscountRate);

        return new InvestmentMetrics(npv, irr, paybackPeriod, recommendation);
    }

    private decimal CalculateNPV(List<decimal> cashFlows, decimal discountRate, decimal initialInvestment)
    {
        decimal npv = -initialInvestment;

        for (int period = 0; period < cashFlows.Count; period++)
        {
            var discountFactor = (decimal)Math.Pow((double)(1 + discountRate), period + 1);
            npv += cashFlows[period] / discountFactor;
        }

        return npv;
    }

    private double CalculateIRR(List<decimal> cashFlows, decimal initialInvestment)
    {
        // 使用 Newton-Raphson 方法求 IRR
        // 簡化實現,實際應使用數值優化算法

        double irr = 0.10;  // 初始猜測 10%
        double tolerance = 0.0001;
        int maxIterations = 100;

        for (int i = 0; i < maxIterations; i++)
        {
            double npv = (double)-initialInvestment;
            double derivative = 0;

            for (int period = 0; period < cashFlows.Count; period++)
            {
                double factor = Math.Pow(1 + irr, period + 1);
                npv += (double)cashFlows[period] / factor;
                derivative -= (double)cashFlows[period] * (period + 1) / Math.Pow(1 + irr, period + 2);
            }

            double irrNext = irr - (npv / derivative);

            if (Math.Abs(irrNext - irr) < tolerance)
            {
                return irrNext;
            }

            irr = irrNext;
        }

        return irr;
    }

    private double CalculatePaybackPeriod(List<decimal> cashFlows, decimal initialInvestment)
    {
        decimal cumulativeCashFlow = 0;

        for (int period = 0; period < cashFlows.Count; period++)
        {
            cumulativeCashFlow += cashFlows[period];

            if (cumulativeCashFlow >= initialInvestment)
            {
                // 計算精確的回收期 (含小數)
                var remainingInvestment = initialInvestment - (cumulativeCashFlow - cashFlows[period]);
                var fractionOfPeriod = (double)(remainingInvestment / cashFlows[period]);

                return period + fractionOfPeriod;
            }
        }

        return double.PositiveInfinity;  // 無法回收
    }

    private string GenerateRecommendation(decimal npv, double irr, double paybackPeriod, decimal discountRate)
    {
        if (npv > 0 && irr > (double)discountRate && paybackPeriod < 3)
        {
            return "✅ 強烈推薦投資 - NPV 為正, IRR 高於折現率, 回收期短";
        }
        else if (npv > 0 && irr > (double)discountRate)
        {
            return "✅ 推薦投資 - NPV 為正, IRR 高於折現率";
        }
        else if (npv > 0)
        {
            return "⚠️  謹慎考慮 - NPV 為正但 IRR 較低";
        }
        else
        {
            return "❌ 不推薦投資 - NPV 為負";
        }
    }

    public void GenerateReport(InvestmentScenario scenario, InvestmentMetrics metrics)
    {
        Console.WriteLine($"=== 投資分析報告: {scenario.Name} ===\n");
        Console.WriteLine($"初始投資: ${scenario.InitialInvestment:N0}");
        Console.WriteLine($"折現率: {scenario.DiscountRate:P1}\n");

        Console.WriteLine("投資指標:");
        Console.WriteLine($"  NPV (淨現值): ${metrics.NPV:N2}");
        Console.WriteLine($"  IRR (內部報酬率): {metrics.IRR:P2}");
        Console.WriteLine($"  Payback Period (回收期): {metrics.PaybackPeriod:F2} 年\n");

        Console.WriteLine($"投資建議: {metrics.Recommendation}");
    }
}

// 使用示例
var analyzer = new InvestmentAnalyzer();

// 投資場景: 購買 3 年預留實例
var scenario = new InvestmentAnalyzer.InvestmentScenario(
    Name: "3 年預留實例投資",
    InitialInvestment: 0m,  // 無預付款
    CashFlows: new List<decimal>
    {
        17071.92m,  // 第 1 年節省
        17071.92m,  // 第 2 年節省
        17071.92m   // 第 3 年節省
    },
    DiscountRate: 0.10m  // 10% 折現率
);

var metrics = analyzer.AnalyzeInvestment(scenario);
analyzer.GenerateReport(scenario, metrics);
```

---

## 檢查清單

### 財務預測檢查清單

- [ ] **成本預測模型**
  - [ ] 歷史數據收集完整 (≥ 6 個月)
  - [ ] 線性回歸模型已訓練
  - [ ] ARIMA 時間序列模型已建立
  - [ ] 預測準確性定期驗證 (±15%)

- [ ] **收入預測模型**
  - [ ] 定價策略已確定
  - [ ] 用戶增長模型已建立
  - [ ] 流失率假設合理 (5-8%)
  - [ ] MRR/ARR 預測已生成

- [ ] **現金流預測**
  - [ ] 月度現金流模型已建立
  - [ ] 現金跑道計算 (≥ 6 個月)
  - [ ] 負現金流風險已評估
  - [ ] 融資需求已識別

- [ ] **盈虧平衡分析**
  - [ ] 固定成本和變動成本已分離
  - [ ] 盈虧平衡點已計算
  - [ ] 達到盈虧平衡時間已預測
  - [ ] 敏感度分析已完成

- [ ] **財務比率分析**
  - [ ] 毛利率目標 ≥ 70%
  - [ ] LTV/CAC 比率 ≥ 3.0
  - [ ] Rule of 40 ≥ 40
  - [ ] NDR ≥ 100%

- [ ] **情景分析**
  - [ ] 樂觀、基準、悲觀情景已建立
  - [ ] 概率加權預測已計算
  - [ ] 風險緩解計劃已制定

- [ ] **投資決策**
  - [ ] NPV, IRR, Payback Period 已計算
  - [ ] 投資建議已生成
  - [ ] 融資策略已制定

---

## 總結

本文檔提供了完整的財務預測分析體系,涵蓋:

✅ **成本預測模型** (線性回歸, ARIMA, 機器學習)
✅ **收入預測模型** (基於用戶增長, 定價策略)
✅ **現金流預測** (月度現金流, 現金跑道分析)
✅ **盈虧平衡分析** (Break-Even Point, 敏感度分析)
✅ **財務比率分析** (毛利率, LTV/CAC, Rule of 40)
✅ **情景分析** (樂觀/基準/悲觀情景)
✅ **投資決策支持** (NPV, IRR, Payback Period)

**關鍵財務目標**:
- **盈虧平衡**: 5,181 用戶 (約 3 個月達成)
- **毛利率**: ≥ 70% (SaaS 標準)
- **Rule of 40**: ≥ 40 (增長 + 利潤)
- **現金跑道**: ≥ 6 個月

透過本指南,財務和管理團隊可以建立科學的財務預測體系,支持戰略決策和融資規劃。
