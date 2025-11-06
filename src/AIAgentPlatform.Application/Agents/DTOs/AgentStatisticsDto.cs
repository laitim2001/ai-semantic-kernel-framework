namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Agent 執行統計資訊
/// </summary>
public class AgentStatisticsDto
{
    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Agent 名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 總執行次數
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// 成功執行次數
    /// </summary>
    public int SuccessfulExecutions { get; set; }

    /// <summary>
    /// 失敗執行次數
    /// </summary>
    public int FailedExecutions { get; set; }

    /// <summary>
    /// 成功率 (0-100)
    /// </summary>
    public decimal SuccessRate { get; set; }

    /// <summary>
    /// 平均響應時間 (毫秒)
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// 最後執行時間
    /// </summary>
    public DateTime? LastExecutionTime { get; set; }

    /// <summary>
    /// 統計期間開始時間
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 統計期間結束時間
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}
