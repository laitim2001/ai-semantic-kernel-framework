using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// 建立 Agent 新版本
/// </summary>
public sealed class CreateAgentVersionCommand : IRequest<Guid>
{
    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// 變更描述
    /// </summary>
    public string ChangeDescription { get; set; } = string.Empty;

    /// <summary>
    /// 變更類型 (major, minor, patch, config)
    /// </summary>
    public string ChangeType { get; set; } = "config";

    /// <summary>
    /// 建立者 ID
    /// </summary>
    public Guid CreatedBy { get; set; }
}
