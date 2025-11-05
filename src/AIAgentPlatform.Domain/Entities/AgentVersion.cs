using AIAgentPlatform.Domain.Common;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Entities;

/// <summary>
/// Represents a version snapshot of an agent's configuration
/// </summary>
public sealed class AgentVersion : BaseEntity
{
    /// <summary>
    /// The agent this version belongs to
    /// </summary>
    public Guid AgentId { get; private set; }

    /// <summary>
    /// Version number (e.g., "1.0.0", "2.1.3")
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Description of changes in this version
    /// </summary>
    public string ChangeDescription { get; private set; }

    /// <summary>
    /// Type of change
    /// </summary>
    public VersionChangeType ChangeType { get; private set; }

    /// <summary>
    /// Complete configuration snapshot (JSON)
    /// </summary>
    public string ConfigurationSnapshot { get; private set; }

    /// <summary>
    /// User who created this version
    /// </summary>
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Whether this is the current active version
    /// </summary>
    public bool IsCurrent { get; private set; }

    /// <summary>
    /// When this version was rolled back from (if applicable)
    /// </summary>
    public DateTime? RolledBackAt { get; private set; }

    /// <summary>
    /// User who performed the rollback (if applicable)
    /// </summary>
    public Guid? RolledBackBy { get; private set; }

    // Navigation properties
    public Agent? Agent { get; private set; }

    // Private constructor for EF Core
    private AgentVersion()
    {
        Version = string.Empty;
        ChangeDescription = string.Empty;
        ConfigurationSnapshot = string.Empty;
        IsCurrent = false;
    }

    /// <summary>
    /// Creates a new agent version snapshot
    /// </summary>
    public static AgentVersion Create(
        Guid agentId,
        string version,
        string changeDescription,
        VersionChangeType changeType,
        string configurationSnapshot,
        Guid createdBy,
        bool isCurrent = false)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        ValidateVersion(version);
        ValidateChangeDescription(changeDescription);
        ValidateConfigurationSnapshot(configurationSnapshot);

        if (createdBy == Guid.Empty)
            throw new ArgumentException("CreatedBy user ID cannot be empty", nameof(createdBy));

        return new AgentVersion
        {
            AgentId = agentId,
            Version = version.Trim(),
            ChangeDescription = changeDescription.Trim(),
            ChangeType = changeType,
            ConfigurationSnapshot = configurationSnapshot,
            CreatedBy = createdBy,
            IsCurrent = isCurrent
        };
    }

    /// <summary>
    /// Marks this version as the current version
    /// </summary>
    public void MarkAsCurrent()
    {
        if (IsCurrent)
            return;

        IsCurrent = true;
        MarkAsModified();
    }

    /// <summary>
    /// Marks this version as not current
    /// </summary>
    public void MarkAsNotCurrent()
    {
        if (!IsCurrent)
            return;

        IsCurrent = false;
        MarkAsModified();
    }

    /// <summary>
    /// Records rollback information
    /// </summary>
    public void RecordRollback(Guid rolledBackBy)
    {
        if (rolledBackBy == Guid.Empty)
            throw new ArgumentException("RolledBackBy user ID cannot be empty", nameof(rolledBackBy));

        RolledBackAt = DateTime.UtcNow;
        RolledBackBy = rolledBackBy;
        MarkAsModified();
    }

    // Validation methods
    private static void ValidateVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be empty", nameof(version));

        if (version.Length > 20)
            throw new ArgumentException("Version cannot exceed 20 characters", nameof(version));
    }

    private static void ValidateChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Change description cannot be empty", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Change description cannot exceed 1000 characters", nameof(description));
    }

    private static void ValidateConfigurationSnapshot(string snapshot)
    {
        if (string.IsNullOrWhiteSpace(snapshot))
            throw new ArgumentException("Configuration snapshot cannot be empty", nameof(snapshot));
    }
}
