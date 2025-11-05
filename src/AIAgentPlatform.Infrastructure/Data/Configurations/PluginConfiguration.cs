using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Plugin entity
/// </summary>
public sealed class PluginConfiguration : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.ToTable("plugins");

        // Primary key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Name
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // Description
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        // Type - Convert Value Object to string
        builder.Property(p => p.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .HasConversion(
                v => v.Value,
                v => PluginType.From(v)
            )
            .IsRequired();

        // Version
        builder.Property(p => p.Version)
            .HasColumnName("version")
            .HasMaxLength(20)
            .IsRequired();

        // Configuration
        builder.Property(p => p.Configuration)
            .HasColumnName("configuration")
            .HasColumnType("jsonb");

        // IsEnabled
        builder.Property(p => p.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        // Author
        builder.Property(p => p.Author)
            .HasColumnName("author")
            .HasMaxLength(100);

        // Timestamps
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("ix_plugins_name");

        builder.HasIndex(p => p.Type)
            .HasDatabaseName("ix_plugins_type");

        builder.HasIndex(p => p.IsEnabled)
            .HasDatabaseName("ix_plugins_is_enabled");
    }
}
