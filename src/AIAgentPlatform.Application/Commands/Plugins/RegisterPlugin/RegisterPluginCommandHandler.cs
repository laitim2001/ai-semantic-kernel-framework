using AIAgentPlatform.Application.Interfaces;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Commands.Plugins.RegisterPlugin;

/// <summary>
/// Handler for RegisterPluginCommand
/// </summary>
public sealed class RegisterPluginCommandHandler : IRequestHandler<RegisterPluginCommand, Plugin>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginMetadataExtractor _metadataExtractor;

    public RegisterPluginCommandHandler(
        IPluginRepository pluginRepository,
        IPluginMetadataExtractor metadataExtractor)
    {
        _pluginRepository = pluginRepository;
        _metadataExtractor = metadataExtractor;
    }

    public async Task<Plugin> Handle(RegisterPluginCommand request, CancellationToken cancellationToken)
    {
        PluginMetadataResult metadataResult;

        // Extract metadata from assembly or use provided metadata
        if (!string.IsNullOrWhiteSpace(request.AssemblyPath))
        {
            // Load from assembly
            metadataResult = await _metadataExtractor.ExtractFromAssemblyAsync(
                request.AssemblyPath,
                cancellationToken);

            if (!metadataResult.IsSuccess)
            {
                throw new InvalidOperationException(
                    $"Failed to extract plugin metadata from assembly: {metadataResult.ErrorMessage}");
            }
        }
        else
        {
            // Use provided metadata (manual registration)
            metadataResult = PluginMetadataResult.Success(
                request.PluginId!,
                request.Name!,
                request.Version!,
                PluginMetadata.Empty, // No functions for manual registration
                request.Description,
                request.Category);
        }

        // Check if plugin with same PluginId already exists
        var existingPlugin = await _pluginRepository.GetByPluginIdAsync(
            metadataResult.PluginId,
            cancellationToken);

        if (existingPlugin != null)
        {
            throw new InvalidOperationException(
                $"Plugin with ID '{metadataResult.PluginId}' is already registered.");
        }

        // Create plugin entity
        var plugin = Plugin.Create(
            userId: request.UserId,
            pluginId: metadataResult.PluginId,
            name: metadataResult.Name,
            version: metadataResult.Version,
            metadata: metadataResult.Metadata,
            description: metadataResult.Description,
            category: metadataResult.Category,
            assemblyPath: request.AssemblyPath,
            assemblyFullName: metadataResult.AssemblyFullName);

        // Auto-activate if requested
        if (request.AutoActivate)
        {
            plugin.Activate();
        }

        // Save to repository
        var savedPlugin = await _pluginRepository.AddAsync(plugin, cancellationToken);

        return savedPlugin;
    }
}
