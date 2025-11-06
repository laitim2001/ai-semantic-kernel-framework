using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AgentVersion entity
/// </summary>
public sealed class AgentVersionConfiguration : IEntityTypeConfiguration<AgentVersion>
{
    public void Configure(EntityTypeBuilder<AgentVersion> builder)
    {
        builder.ToTable("agent_versions");

        // Primary key
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // AgentId
        builder.Property(v => v.AgentId)
            .HasColumnName("agent_id")
            .IsRequired();

        // Version
        builder.Property(v => v.Version)
            .HasColumnName("version")
            .HasMaxLength(20)
            .IsRequired();

        // ChangeDescription
        builder.Property(v => v.ChangeDescription)
            .HasColumnName("change_description")
            .HasMaxLength(1000)
            .IsRequired();

        // ChangeType - Convert Value Object to string
        builder.Property(v => v.ChangeType)
            .HasColumnName("change_type")
            .HasMaxLength(20)
            .HasConversion(
                v => v.Value,
                v => VersionChangeType.From(v)
            )
            .IsRequired();

        // ConfigurationSnapshot
        builder.Property(v => v.ConfigurationSnapshot)
            .HasColumnName("configuration_snapshot")
            .HasColumnType("jsonb")
            .IsRequired();

        // CreatedBy
        builder.Property(v => v.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        // IsCurrent
        builder.Property(v => v.IsCurrent)
            .HasColumnName("is_current")
            .IsRequired();

        // RolledBackAt
        builder.Property(v => v.RolledBackAt)
            .HasColumnName("rolled_back_at");

        // RolledBackBy
        builder.Property(v => v.RolledBackBy)
            .HasColumnName("rolled_back_by");

        // Timestamps
        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(v => v.Agent)
            .WithMany()
            .HasForeignKey(v => v.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.AgentId)
            .HasDatabaseName("ix_agent_versions_agent_id");

        builder.HasIndex(v => new { v.AgentId, v.Version })
            .IsUnique()
            .HasDatabaseName("ix_agent_versions_agent_version");

        builder.HasIndex(v => v.IsCurrent)
            .HasDatabaseName("ix_agent_versions_is_current");

        builder.HasIndex(v => v.CreatedAt)
            .HasDatabaseName("ix_agent_versions_created_at");
    }
}
