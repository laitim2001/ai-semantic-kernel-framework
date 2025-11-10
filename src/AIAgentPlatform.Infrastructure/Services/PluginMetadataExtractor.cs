using System.Reflection;
using System.Runtime.Loader;
using AIAgentPlatform.Application.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using AIAgentPlatform.Shared.Attributes;
using Microsoft.Extensions.Logging;

namespace AIAgentPlatform.Infrastructure.Services;

/// <summary>
/// Service for extracting plugin metadata from assemblies using reflection
/// </summary>
public sealed class PluginMetadataExtractor : IPluginMetadataExtractor
{
    private readonly ILogger<PluginMetadataExtractor> _logger;

    public PluginMetadataExtractor(ILogger<PluginMetadataExtractor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts plugin metadata from an assembly file
    /// </summary>
    public async Task<PluginMetadataResult> ExtractFromAssemblyAsync(
        string assemblyPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assemblyPath))
                return PluginMetadataResult.Failure("Assembly path cannot be empty");

            if (!File.Exists(assemblyPath))
                return PluginMetadataResult.Failure($"Assembly file not found: {assemblyPath}");

            _logger.LogInformation("Loading assembly from: {AssemblyPath}", assemblyPath);

            // Create isolated AssemblyLoadContext for plugin
            var loadContext = new PluginLoadContext(assemblyPath);

            try
            {
                // Load assembly in isolated context
                var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

                // Find plugin types
                var pluginTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && IsValidPlugin(t))
                    .ToList();

                if (pluginTypes.Count == 0)
                {
                    return PluginMetadataResult.Failure(
                        $"No valid plugin class found in assembly. Ensure class is marked with [Plugin] attribute.");
                }

                if (pluginTypes.Count > 1)
                {
                    _logger.LogWarning(
                        "Multiple plugin classes found in assembly. Using first: {PluginType}",
                        pluginTypes[0].FullName);
                }

                // Extract metadata from first plugin type
                var result = ExtractFromType(pluginTypes[0]);
                result.AssemblyFullName = assembly.FullName;

                await Task.CompletedTask; // Allow for async operations in future

                return result;
            }
            finally
            {
                // Unload the isolated context to free resources
                loadContext.Unload();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract metadata from assembly: {AssemblyPath}", assemblyPath);
            return PluginMetadataResult.Failure($"Failed to load assembly: {ex.Message}");
        }
    }

    /// <summary>
    /// Extracts plugin metadata from a loaded type
    /// </summary>
    public PluginMetadataResult ExtractFromType(Type pluginType)
    {
        try
        {
            if (!IsValidPlugin(pluginType))
            {
                return PluginMetadataResult.Failure(
                    $"Type {pluginType.FullName} is not a valid plugin. Must be a class with [Plugin] attribute.");
            }

            // Extract Plugin attribute
            var pluginAttr = pluginType.GetCustomAttribute<PluginAttribute>();
            if (pluginAttr == null)
            {
                return PluginMetadataResult.Failure(
                    $"Type {pluginType.FullName} does not have [Plugin] attribute.");
            }

            // Extract functions
            var functions = ExtractFunctions(pluginType);

            // Create metadata
            var metadata = PluginMetadata.Create(functions);

            // Determine plugin name (use attribute Name or type name)
            var name = pluginAttr.Name ?? pluginType.Name;

            return PluginMetadataResult.Success(
                pluginAttr.PluginId,
                name,
                pluginAttr.Version,
                metadata,
                pluginAttr.Description,
                pluginAttr.Category,
                typeFullName: pluginType.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract metadata from type: {TypeName}", pluginType.FullName);
            return PluginMetadataResult.Failure($"Failed to extract metadata: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates that a type is a valid plugin
    /// </summary>
    public bool IsValidPlugin(Type pluginType)
    {
        return pluginType.IsClass &&
               !pluginType.IsAbstract &&
               pluginType.GetCustomAttribute<PluginAttribute>() != null;
    }

    /// <summary>
    /// Extracts functions from plugin type
    /// </summary>
    private List<PluginFunction> ExtractFunctions(Type pluginType)
    {
        var functions = new List<PluginFunction>();

        // Get all public methods with [PluginFunction] attribute
        var methods = pluginType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<PluginFunctionAttribute>() != null)
            .ToList();

        foreach (var method in methods)
        {
            try
            {
                var function = ExtractFunction(method);
                functions.Add(function);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract function: {MethodName}", method.Name);
            }
        }

        return functions;
    }

    /// <summary>
    /// Extracts function metadata from method
    /// </summary>
    private PluginFunction ExtractFunction(MethodInfo method)
    {
        var functionAttr = method.GetCustomAttribute<PluginFunctionAttribute>();
        if (functionAttr == null)
            throw new InvalidOperationException($"Method {method.Name} does not have [PluginFunction] attribute");

        // Determine function name (use attribute Name or method name)
        var functionName = functionAttr.Name ?? method.Name;

        // Get description from attribute or method's Description attribute
        var description = functionAttr.Description;

        // Extract parameters
        var parameters = ExtractParameters(method);

        // Get return type
        var returnType = GetFriendlyTypeName(method.ReturnType);

        return PluginFunction.CreateWithParameters(
            functionName,
            description,
            parameters,
            returnType);
    }

    /// <summary>
    /// Extracts parameters from method
    /// </summary>
    private List<PluginParameter> ExtractParameters(MethodInfo method)
    {
        var parameters = new List<PluginParameter>();

        var methodParams = method.GetParameters();

        foreach (var param in methodParams)
        {
            // Skip CancellationToken parameters (not user-facing)
            if (param.ParameterType == typeof(CancellationToken))
                continue;

            try
            {
                var parameter = ExtractParameter(param);
                parameters.Add(parameter);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract parameter: {ParameterName}", param.Name);
            }
        }

        return parameters;
    }

    /// <summary>
    /// Extracts parameter metadata
    /// </summary>
    private PluginParameter ExtractParameter(ParameterInfo param)
    {
        var name = param.Name ?? "unknown";
        var type = GetFriendlyTypeName(param.ParameterType);

        // Get description from [Description] attribute
        var descriptionAttr = param.GetCustomAttribute<DescriptionAttribute>();
        var description = descriptionAttr?.Description;

        // Check if parameter is optional (has default value)
        var isRequired = !param.IsOptional;
        var defaultValue = param.HasDefaultValue ? param.DefaultValue : null;

        if (isRequired)
        {
            return PluginParameter.Required(name, type, description);
        }
        else
        {
            return PluginParameter.Optional(name, type, description, defaultValue);
        }
    }

    /// <summary>
    /// Gets friendly type name for display
    /// </summary>
    private string GetFriendlyTypeName(Type type)
    {
        // Handle Task<T> and Task
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = type.GetGenericArguments()[0];
            return GetFriendlyTypeName(innerType);
        }

        if (type == typeof(Task))
        {
            return "void";
        }

        // Handle Nullable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var innerType = type.GetGenericArguments()[0];
            return $"{GetFriendlyTypeName(innerType)}?";
        }

        // Handle common types
        if (type == typeof(string)) return "string";
        if (type == typeof(int)) return "int";
        if (type == typeof(long)) return "long";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(double)) return "double";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(DateTime)) return "DateTime";
        if (type == typeof(Guid)) return "Guid";
        if (type == typeof(void)) return "void";

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            return $"{GetFriendlyTypeName(elementType)}[]";
        }

        // Handle List<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var itemType = type.GetGenericArguments()[0];
            return $"List<{GetFriendlyTypeName(itemType)}>";
        }

        // Handle IEnumerable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var itemType = type.GetGenericArguments()[0];
            return $"IEnumerable<{GetFriendlyTypeName(itemType)}>";
        }

        // Default: use type name
        return type.Name;
    }

    /// <summary>
    /// Custom AssemblyLoadContext for plugin isolation
    /// </summary>
    private sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // Try to resolve assembly path
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            // Fall back to default loading
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
