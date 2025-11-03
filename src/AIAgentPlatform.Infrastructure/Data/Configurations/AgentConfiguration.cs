using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Agent entity
/// </summary>
public sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");

        // Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // UUID generated in domain

        // UserId
        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Name
        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // Description
        builder.Property(a => a.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        // Instructions
        builder.Property(a => a.Instructions)
            .HasColumnName("instructions")
            .HasMaxLength(10000)
            .IsRequired();

        // Model - Convert Value Object to string
        builder.Property(a => a.Model)
            .HasColumnName("model")
            .HasMaxLength(50)
            .HasConversion(
                v => v.Value,
                v => LLMModel.From(v)
            )
            .IsRequired();

        // Temperature
        builder.Property(a => a.Temperature)
            .HasColumnName("temperature")
            .HasPrecision(3, 2)
            .IsRequired();

        // MaxTokens
        builder.Property(a => a.MaxTokens)
            .HasColumnName("max_tokens")
            .IsRequired();

        // Status - Convert Value Object to string
        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.Value,
                v => AgentStatus.From(v)
            )
            .IsRequired();

        // Timestamps
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("ix_agents_user_id");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("ix_agents_status");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("ix_agents_created_at");
    }
}
