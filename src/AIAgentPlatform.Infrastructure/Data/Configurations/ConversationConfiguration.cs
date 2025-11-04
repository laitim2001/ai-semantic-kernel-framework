using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Conversation Entity Configuration
/// </summary>
public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.AgentId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        // ConversationStatus 為 Value Object，需要特別配置
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion(
                status => status.Value,
                value => ConversationStatus.Create(value)
            )
            .HasMaxLength(20);

        builder.Property(c => c.MessageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.LastMessageAt)
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(c => c.Agent)
            .WithMany()
            .HasForeignKey(c => c.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.AgentId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.UpdatedAt);
    }
}
