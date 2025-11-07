using Microsoft.AspNetCore.SignalR;

namespace AIAgentPlatform.API.Hubs;

/// <summary>
/// SignalR Hub for real-time agent execution monitoring
/// </summary>
public sealed class ExecutionMonitorHub : Hub
{
    /// <summary>
    /// Subscribe to execution updates for a specific agent
    /// </summary>
    /// <param name="agentId">The agent ID to monitor</param>
    public async Task SubscribeToAgent(string agentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }

    /// <summary>
    /// Unsubscribe from execution updates for a specific agent
    /// </summary>
    /// <param name="agentId">The agent ID to stop monitoring</param>
    public async Task UnsubscribeFromAgent(string agentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }

    /// <summary>
    /// Subscribe to all execution updates
    /// </summary>
    public async Task SubscribeToAll()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all-executions");
    }

    /// <summary>
    /// Unsubscribe from all execution updates
    /// </summary>
    public async Task UnsubscribeFromAll()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all-executions");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        // Optionally log connection
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        // Optionally log disconnection
    }
}
