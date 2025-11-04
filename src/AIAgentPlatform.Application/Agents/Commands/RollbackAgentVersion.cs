using MediatR;

namespace AIAgentPlatform.Application.Agents.Commands;

/// <summary>
/// 回滾 Agent 到指定版本
/// </summary>
public sealed class RollbackAgentVersionCommand : IRequest<bool>
{
    /// <summary>
    /// Agent ID
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// 目標版本 ID
    /// </summary>
    public Guid VersionId { get; set; }

    /// <summary>
    /// 執行回滾的使用者 ID
    /// </summary>
    public Guid UserId { get; set; }
}
