namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Agent 與 Plugin 的關聯資訊
/// </summary>
public class AgentPluginDto
{
    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Plugin ID
    /// </summary>
    public Guid PluginId { get; set; }

    /// <summary>
    /// Plugin 詳細資訊
    /// </summary>
    public PluginDto Plugin { get; set; } = new();

    /// <summary>
    /// Plugin Name
    /// </summary>
    public string PluginName { get; set; } = string.Empty;

    /// <summary>
    /// Plugin Description
    /// </summary>
    public string? PluginDescription { get; set; }

    /// <summary>
    /// Plugin Type
    /// </summary>
    public string PluginType { get; set; } = string.Empty;

    /// <summary>
    /// Plugin Version
    /// </summary>
    public string PluginVersion { get; set; } = string.Empty;

    /// <summary>
    /// 是否已啟用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 執行順序 (數字越小越先執行)
    /// </summary>
    public int ExecutionOrder { get; set; }

    /// <summary>
    /// 自訂配置覆寫 (JSON, 可選)
    /// </summary>
    public string? CustomConfiguration { get; set; }

    /// <summary>
    /// 添加時間
    /// </summary>
    public DateTime AddedAt { get; set; }
}
