using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// 更新 Agent 的 Plugin 設定
/// </summary>
public sealed class UpdateAgentPluginCommand : IRequest<bool>
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
    /// 是否啟用
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// 執行順序
    /// </summary>
    public int? ExecutionOrder { get; set; }

    /// <summary>
    /// 自訂配置 (JSON, 可選)
    /// </summary>
    public string? CustomConfiguration { get; set; }
}
