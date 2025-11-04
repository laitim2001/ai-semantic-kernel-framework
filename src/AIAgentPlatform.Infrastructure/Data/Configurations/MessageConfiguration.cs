using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Message Entity Configuration
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ConversationId)
            .IsRequired();

        // MessageRole 為 Value Object，需要特別配置
        builder.Property(m => m.Role)
            .IsRequired()
            .HasConversion(
                role => role.Value,
                value => MessageRole.Create(value)
            )
            .HasMaxLength(20);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(32000);

        builder.Property(m => m.TokenCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => m.Role);
    }
}
