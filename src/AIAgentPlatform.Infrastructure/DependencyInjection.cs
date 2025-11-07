using AIAgentPlatform.Application.AgentExecutions.Services;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Infrastructure.Data;
using AIAgentPlatform.Infrastructure.Data.Repositories;
using AIAgentPlatform.Infrastructure.Services;
using AIAgentPlatform.Infrastructure.Services.AgentExecution;
using AIAgentPlatform.Infrastructure.Services.SemanticKernel;
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
        services.AddScoped<IAgentExecutionRepository, AgentExecutionRepository>();
        services.AddScoped<IPluginRepository, PluginRepository>();
        services.AddScoped<IAgentPluginRepository, AgentPluginRepository>();
        services.AddScoped<IAgentVersionRepository, AgentVersionRepository>();

        // Register services
        services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        services.AddScoped<IAgentExecutionService, AgentExecutionService>();
        services.AddScoped<IExecutionExportService, ExecutionExportService>();

        return services;
    }
}
