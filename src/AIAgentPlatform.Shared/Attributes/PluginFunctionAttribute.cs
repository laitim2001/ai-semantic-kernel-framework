namespace AIAgentPlatform.Shared.Attributes;

/// <summary>
/// Marks a method as a plugin function that can be invoked by agents
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PluginFunctionAttribute : Attribute
{
    /// <summary>
    /// Function name (optional, defaults to method name)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Function description for AI context
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Creates a new PluginFunctionAttribute
    /// </summary>
    public PluginFunctionAttribute()
    {
    }

    /// <summary>
    /// Creates a new PluginFunctionAttribute with a name
    /// </summary>
    /// <param name="name">Function name</param>
    public PluginFunctionAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Provides a description for a parameter
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class DescriptionAttribute : Attribute
{
    /// <summary>
    /// Parameter description
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Creates a new DescriptionAttribute
    /// </summary>
    /// <param name="description">Parameter description</param>
    public DescriptionAttribute(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Description = description;
    }
}
