using AIAgentPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AgentPlugin entity
/// </summary>
public sealed class AgentPluginConfiguration : IEntityTypeConfiguration<AgentPlugin>
{
    public void Configure(EntityTypeBuilder<AgentPlugin> builder)
    {
        builder.ToTable("agent_plugins");

        // Primary key
        builder.HasKey(ap => ap.Id);
        builder.Property(ap => ap.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // AgentId
        builder.Property(ap => ap.AgentId)
            .HasColumnName("agent_id")
            .IsRequired();

        // PluginId
        builder.Property(ap => ap.PluginId)
            .HasColumnName("plugin_id")
            .IsRequired();

        // IsEnabled
        builder.Property(ap => ap.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        // ExecutionOrder
        builder.Property(ap => ap.ExecutionOrder)
            .HasColumnName("execution_order")
            .IsRequired();

        // CustomConfiguration
        builder.Property(ap => ap.CustomConfiguration)
            .HasColumnName("custom_configuration")
            .HasColumnType("jsonb");

        // AddedAt
        builder.Property(ap => ap.AddedAt)
            .HasColumnName("added_at")
            .IsRequired();

        // AddedBy
        builder.Property(ap => ap.AddedBy)
            .HasColumnName("added_by")
            .IsRequired();

        // Timestamps
        builder.Property(ap => ap.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ap => ap.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(ap => ap.Agent)
            .WithMany()
            .HasForeignKey(ap => ap.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ap => ap.Plugin)
            .WithMany(p => p.AgentPlugins)
            .HasForeignKey(ap => ap.PluginId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ap => ap.AgentId)
            .HasDatabaseName("ix_agent_plugins_agent_id");

        builder.HasIndex(ap => ap.PluginId)
            .HasDatabaseName("ix_agent_plugins_plugin_id");

        builder.HasIndex(ap => new { ap.AgentId, ap.PluginId })
            .IsUnique()
            .HasDatabaseName("ix_agent_plugins_agent_plugin");

        builder.HasIndex(ap => ap.ExecutionOrder)
            .HasDatabaseName("ix_agent_plugins_execution_order");
    }
}
