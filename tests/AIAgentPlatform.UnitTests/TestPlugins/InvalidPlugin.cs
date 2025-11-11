namespace AIAgentPlatform.UnitTests.TestPlugins;

/// <summary>
/// Invalid plugin class without [Plugin] attribute for testing
/// </summary>
public class InvalidPlugin
{
    public string GetData()
    {
        return "Invalid";
    }
}
