using AIAgentPlatform.Shared.Attributes;

namespace AIAgentPlatform.UnitTests.TestPlugins;

/// <summary>
/// Sample Weather Plugin for testing metadata extraction
/// </summary>
[Plugin("WeatherPlugin", "1.0.0", Name = "Weather Information Plugin", Description = "Provides weather information for locations", Category = "Utilities")]
public class WeatherPlugin
{
    [PluginFunction(Name = "GetCurrentWeather", Description = "Gets current weather for a specific city")]
    public async Task<WeatherResult> GetCurrentWeatherAsync(
        [Description("City name (e.g., Taipei, Tokyo)")] string city,
        [Description("Temperature unit")] TemperatureUnit unit = TemperatureUnit.Celsius,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);

        return new WeatherResult
        {
            City = city,
            Temperature = 25.5,
            Unit = unit,
            Condition = "Sunny"
        };
    }

    [PluginFunction(Description = "Gets weather forecast for multiple days")]
    public Task<List<WeatherResult>> GetForecastAsync(
        [Description("City name")] string city,
        [Description("Number of days (1-7)")] int days = 3)
    {
        var forecast = new List<WeatherResult>();
        for (int i = 0; i < days; i++)
        {
            forecast.Add(new WeatherResult
            {
                City = city,
                Temperature = 20 + i,
                Unit = TemperatureUnit.Celsius,
                Condition = "Partly Cloudy"
            });
        }
        return Task.FromResult(forecast);
    }

    [PluginFunction]
    public string GetWeatherAlert(string city)
    {
        return $"No weather alerts for {city}";
    }
}

public class WeatherResult
{
    public string City { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public TemperatureUnit Unit { get; set; }
    public string Condition { get; set; } = string.Empty;
}

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}
