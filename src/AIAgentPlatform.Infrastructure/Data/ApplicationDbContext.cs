using AIAgentPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AIAgentPlatform.Infrastructure.Data;

/// <summary>
/// Application database context
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<AgentExecution> AgentExecutions => Set<AgentExecution>();
    public DbSet<Plugin> Plugins => Set<Plugin>();
    public DbSet<AgentPlugin> AgentPlugins => Set<AgentPlugin>();
    public DbSet<AgentVersion> AgentVersions => Set<AgentVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // UpdatedAt is handled by BaseEntity.MarkAsModified()
        return base.SaveChangesAsync(cancellationToken);
    }
}
