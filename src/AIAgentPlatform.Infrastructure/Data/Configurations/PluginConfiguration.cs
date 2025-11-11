using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace AIAgentPlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Plugin entity
/// </summary>
public sealed class PluginConfiguration : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.ToTable("Plugins");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.PluginId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Category)
            .HasMaxLength(200);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Configure PluginStatus as owned entity (Value Object pattern)
        builder.OwnsOne(p => p.Status, status =>
        {
            status.Property(s => s.Value)
                .HasColumnName("Status")
                .IsRequired()
                .HasMaxLength(50);
        });

        // Configure PluginMetadata as JSON column
        // Serialize entire object to JSON string
        builder.Property(p => p.Metadata)
            .HasColumnName("Metadata")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<PluginMetadata>(v, (JsonSerializerOptions?)null)
                     ?? PluginMetadata.Empty
            );

        // Indexes for better query performance
        builder.HasIndex(p => p.PluginId)
            .IsUnique();

        builder.HasIndex(p => p.UserId);

        builder.HasIndex(p => p.Category);

        builder.HasIndex(p => p.CreatedAt);
    }
}
