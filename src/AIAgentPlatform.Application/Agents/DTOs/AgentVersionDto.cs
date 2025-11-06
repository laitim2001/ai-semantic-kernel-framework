namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Agent 版本資訊
/// </summary>
public class AgentVersionDto
{
    /// <summary>
    /// 版本 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// 版本號 (例如: 1.0.0, 1.1.0)
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 變更描述
    /// </summary>
    public string ChangeDescription { get; set; } = string.Empty;

    /// <summary>
    /// 變更類型 (major, minor, patch, config)
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Agent 配置快照 (JSON)
    /// </summary>
    public string ConfigurationSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// 建立者 ID
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 是否為當前版本
    /// </summary>
    public bool IsCurrent { get; set; }
}
