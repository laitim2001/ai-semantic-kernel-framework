namespace AIAgentPlatform.Domain.ValueObjects;

/// <summary>
/// Represents plugin metadata including functions and parameters
/// </summary>
public sealed class PluginMetadata
{
    public IReadOnlyList<PluginFunction> Functions { get; }
    public IDictionary<string, string> Properties { get; }

    public static readonly PluginMetadata Empty = new(new List<PluginFunction>(), new Dictionary<string, string>());

    public PluginMetadata(IReadOnlyList<PluginFunction> functions, IDictionary<string, string> properties)
    {
        Functions = functions ?? throw new ArgumentNullException(nameof(functions));
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }

    /// <summary>
    /// Creates metadata from function list
    /// </summary>
    public static PluginMetadata Create(IReadOnlyList<PluginFunction> functions, IDictionary<string, string>? properties = null)
    {
        return new PluginMetadata(functions, properties ?? new Dictionary<string, string>());
    }

    /// <summary>
    /// Gets function by name
    /// </summary>
    public PluginFunction? GetFunction(string functionName)
    {
        return Functions.FirstOrDefault(f => f.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if function exists
    /// </summary>
    public bool HasFunction(string functionName)
    {
        return Functions.Any(f => f.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase));
    }

    public override bool Equals(object? obj)
    {
        return obj is PluginMetadata other &&
               Functions.SequenceEqual(other.Functions) &&
               Properties.SequenceEqual(other.Properties);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Functions, Properties);
    }
}

/// <summary>
/// Represents a plugin function with parameters
/// </summary>
public sealed class PluginFunction
{
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<PluginParameter> Parameters { get; }
    public string? ReturnType { get; }

    public PluginFunction(string name, string? description, IReadOnlyList<PluginParameter> parameters, string? returnType = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Function name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        Parameters = parameters ?? new List<PluginParameter>();
        ReturnType = returnType;
    }

    /// <summary>
    /// Creates a function with no parameters
    /// </summary>
    public static PluginFunction Create(string name, string? description = null, string? returnType = null)
    {
        return new PluginFunction(name, description, new List<PluginParameter>(), returnType);
    }

    /// <summary>
    /// Creates a function with parameters
    /// </summary>
    public static PluginFunction CreateWithParameters(
        string name,
        string? description,
        IReadOnlyList<PluginParameter> parameters,
        string? returnType = null)
    {
        return new PluginFunction(name, description, parameters, returnType);
    }

    /// <summary>
    /// Gets parameter by name
    /// </summary>
    public PluginParameter? GetParameter(string parameterName)
    {
        return Parameters.FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if parameter exists
    /// </summary>
    public bool HasParameter(string parameterName)
    {
        return Parameters.Any(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
    }

    public override bool Equals(object? obj)
    {
        return obj is PluginFunction other &&
               Name == other.Name &&
               Description == other.Description &&
               Parameters.SequenceEqual(other.Parameters) &&
               ReturnType == other.ReturnType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, Parameters, ReturnType);
    }

    public override string ToString() => $"{Name}({string.Join(", ", Parameters.Select(p => p.ToString()))})";
}

/// <summary>
/// Represents a function parameter
/// </summary>
public sealed class PluginParameter
{
    public string Name { get; }
    public string Type { get; }
    public string? Description { get; }
    public bool IsRequired { get; }
    public object? DefaultValue { get; }

    public PluginParameter(
        string name,
        string type,
        string? description = null,
        bool isRequired = true,
        object? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Parameter type cannot be empty", nameof(type));

        Name = name;
        Type = type;
        Description = description;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a required parameter
    /// </summary>
    public static PluginParameter Required(string name, string type, string? description = null)
    {
        return new PluginParameter(name, type, description, isRequired: true);
    }

    /// <summary>
    /// Creates an optional parameter
    /// </summary>
    public static PluginParameter Optional(string name, string type, string? description = null, object? defaultValue = null)
    {
        return new PluginParameter(name, type, description, isRequired: false, defaultValue);
    }

    public override bool Equals(object? obj)
    {
        return obj is PluginParameter other &&
               Name == other.Name &&
               Type == other.Type &&
               Description == other.Description &&
               IsRequired == other.IsRequired &&
               Equals(DefaultValue, other.DefaultValue);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type, Description, IsRequired, DefaultValue);
    }

    public override string ToString()
    {
        var requiredMarker = IsRequired ? "" : "?";
        var defaultValueStr = DefaultValue != null ? $" = {DefaultValue}" : "";
        return $"{Type}{requiredMarker} {Name}{defaultValueStr}";
    }
}
