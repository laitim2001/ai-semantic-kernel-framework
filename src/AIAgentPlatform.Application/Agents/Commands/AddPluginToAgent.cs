using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// 將 Plugin 添加到 Agent
/// </summary>
public sealed class AddPluginToAgentCommand : IRequest<bool>
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
    /// 是否立即啟用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 執行順序
    /// </summary>
    public int ExecutionOrder { get; set; } = 0;

    /// <summary>
    /// 自訂配置 (JSON, 可選)
    /// </summary>
    public string? CustomConfiguration { get; set; }
}
