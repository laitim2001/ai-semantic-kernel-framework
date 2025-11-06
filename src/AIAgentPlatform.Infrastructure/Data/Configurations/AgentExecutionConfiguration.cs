using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AgentExecution entity
/// </summary>
public sealed class AgentExecutionConfiguration : IEntityTypeConfiguration<AgentExecution>
{
    public void Configure(EntityTypeBuilder<AgentExecution> builder)
    {
        builder.ToTable("agent_executions");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // AgentId
        builder.Property(e => e.AgentId)
            .HasColumnName("agent_id")
            .IsRequired();

        // ConversationId
        builder.Property(e => e.ConversationId)
            .HasColumnName("conversation_id")
            .IsRequired();

        // StartTime
        builder.Property(e => e.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        // EndTime
        builder.Property(e => e.EndTime)
            .HasColumnName("end_time");

        // Status - Convert Value Object to string
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.Value,
                v => ExecutionStatus.From(v)
            )
            .IsRequired();

        // ResponseTimeMs
        builder.Property(e => e.ResponseTimeMs)
            .HasColumnName("response_time_ms")
            .HasPrecision(18, 2);

        // TokensUsed
        builder.Property(e => e.TokensUsed)
            .HasColumnName("tokens_used");

        // ErrorMessage
        builder.Property(e => e.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2000);

        // Metadata
        builder.Property(e => e.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb");

        // Timestamps
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Agent)
            .WithMany()
            .HasForeignKey(e => e.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Conversation)
            .WithMany()
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.AgentId)
            .HasDatabaseName("ix_agent_executions_agent_id");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("ix_agent_executions_conversation_id");

        builder.HasIndex(e => e.StartTime)
            .HasDatabaseName("ix_agent_executions_start_time");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_agent_executions_status");
    }
}
