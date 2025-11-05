using AIAgentPlatform.Domain.Common;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents the association between an agent and a plugin
/// </summary>
public sealed class AgentPlugin : BaseEntity
{
    /// <summary>
    /// The agent this plugin is associated with
    /// </summary>
    public Guid AgentId { get; private set; }

    /// <summary>
    /// The plugin associated with the agent
    /// </summary>
    public Guid PluginId { get; private set; }

    /// <summary>
    /// Whether this plugin is enabled for this agent
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Execution order (lower number = higher priority)
    /// </summary>
    public int ExecutionOrder { get; private set; }

    /// <summary>
    /// Custom configuration for this agent-plugin combination (JSON)
    /// </summary>
    public string? CustomConfiguration { get; private set; }

    /// <summary>
    /// When this plugin was added to the agent
    /// </summary>
    public DateTime AddedAt { get; private set; }

    /// <summary>
    /// User who added this plugin
    /// </summary>
    public Guid AddedBy { get; private set; }

    // Navigation properties
    public Agent? Agent { get; private set; }
    public Plugin? Plugin { get; private set; }

    // Private constructor for EF Core
    private AgentPlugin()
    {
        IsEnabled = true;
        ExecutionOrder = 0;
    }

    /// <summary>
    /// Creates a new agent-plugin association
    /// </summary>
    public static AgentPlugin Create(
        Guid agentId,
        Guid pluginId,
        Guid addedBy,
        int executionOrder = 0,
        string? customConfiguration = null)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        if (pluginId == Guid.Empty)
            throw new ArgumentException("Plugin ID cannot be empty", nameof(pluginId));

        if (addedBy == Guid.Empty)
            throw new ArgumentException("AddedBy user ID cannot be empty", nameof(addedBy));

        if (executionOrder < 0)
            throw new ArgumentException("Execution order cannot be negative", nameof(executionOrder));

        return new AgentPlugin
        {
            AgentId = agentId,
            PluginId = pluginId,
            AddedBy = addedBy,
            ExecutionOrder = executionOrder,
            CustomConfiguration = customConfiguration,
            IsEnabled = true,
            AddedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates plugin configuration
    /// </summary>
    public void UpdateConfiguration(
        bool isEnabled,
        int executionOrder,
        string? customConfiguration)
    {
        if (executionOrder < 0)
            throw new ArgumentException("Execution order cannot be negative", nameof(executionOrder));

        IsEnabled = isEnabled;
        ExecutionOrder = executionOrder;
        CustomConfiguration = customConfiguration;
        MarkAsModified();
    }

    /// <summary>
    /// Enables the plugin for this agent
    /// </summary>
    public void Enable()
    {
        if (IsEnabled)
            return;

        IsEnabled = true;
        MarkAsModified();
    }

    /// <summary>
    /// Disables the plugin for this agent
    /// </summary>
    public void Disable()
    {
        if (!IsEnabled)
            return;

        IsEnabled = false;
        MarkAsModified();
    }

    /// <summary>
    /// Updates execution order
    /// </summary>
    public void UpdateExecutionOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Execution order cannot be negative", nameof(order));

        if (ExecutionOrder == order)
            return;

        ExecutionOrder = order;
        MarkAsModified();
    }
}
