using AIAgentPlatform.Application.Interfaces;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Infrastructure.Data;
using AIAgentPlatform.Infrastructure.Data.Repositories;
using AIAgentPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIAgentPlatform.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );

        // Register repositories
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Register services
        services.AddSingleton<IPluginMetadataExtractor, PluginMetadataExtractor>();

        return services;
    }
}
