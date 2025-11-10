using AIAgentPlatform.Infrastructure.Services;
using AIAgentPlatform.UnitTests.TestPlugins;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AIAgentPlatform.UnitTests.Infrastructure.Services;

public sealed class PluginMetadataExtractorTests
{
    private readonly PluginMetadataExtractor _extractor;

    public PluginMetadataExtractorTests()
    {
        var logger = new Mock<ILogger<PluginMetadataExtractor>>();
        _extractor = new PluginMetadataExtractor(logger.Object);
    }

    [Fact]
    public void IsValidPlugin_WithValidPlugin_ShouldReturnTrue()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.IsValidPlugin(pluginType);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidPlugin_WithInvalidPlugin_ShouldReturnFalse()
    {
        // Arrange
        var pluginType = typeof(InvalidPlugin);

        // Act
        var result = _extractor.IsValidPlugin(pluginType);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidPlugin_WithAbstractClass_ShouldReturnFalse()
    {
        // Arrange
        var pluginType = typeof(AbstractPluginBase);

        // Act
        var result = _extractor.IsValidPlugin(pluginType);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ExtractFromType_WithValidPlugin_ShouldExtractMetadata()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.PluginId.Should().Be("WeatherPlugin");
        result.Name.Should().Be("Weather Information Plugin");
        result.Version.Should().Be("1.0.0");
        result.Description.Should().Be("Provides weather information for locations");
        result.Category.Should().Be("Utilities");
        result.TypeFullName.Should().Be(pluginType.FullName);
    }

    [Fact]
    public void ExtractFromType_WithInvalidPlugin_ShouldReturnFailure()
    {
        // Arrange
        var pluginType = typeof(InvalidPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not a valid plugin");
    }

    [Fact]
    public void ExtractFromType_ShouldExtractFunctions()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        result.Metadata.Functions.Should().HaveCount(3);

        var getCurrentWeather = result.Metadata.GetFunction("GetCurrentWeather");
        getCurrentWeather.Should().NotBeNull();
        getCurrentWeather!.Description.Should().Be("Gets current weather for a specific city");
    }

    [Fact]
    public void ExtractFromType_ShouldExtractFunctionParameters()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        var getCurrentWeather = result.Metadata.GetFunction("GetCurrentWeather");
        getCurrentWeather.Should().NotBeNull();

        // Should have 2 parameters (city and unit, CancellationToken is excluded)
        getCurrentWeather!.Parameters.Should().HaveCount(2);

        var cityParam = getCurrentWeather.GetParameter("city");
        cityParam.Should().NotBeNull();
        cityParam!.Name.Should().Be("city");
        cityParam.Type.Should().Be("string");
        cityParam.Description.Should().Be("City name (e.g., Taipei, Tokyo)");
        cityParam.IsRequired.Should().BeTrue();

        var unitParam = getCurrentWeather.GetParameter("unit");
        unitParam.Should().NotBeNull();
        unitParam!.Name.Should().Be("unit");
        unitParam.Description.Should().Be("Temperature unit");
        unitParam.IsRequired.Should().BeFalse();
        unitParam.DefaultValue.Should().Be(TemperatureUnit.Celsius);
    }

    [Fact]
    public void ExtractFromType_ShouldHandleTaskReturnType()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        var getCurrentWeather = result.Metadata.GetFunction("GetCurrentWeather");
        getCurrentWeather.Should().NotBeNull();
        getCurrentWeather!.ReturnType.Should().Be("WeatherResult");

        var getForecast = result.Metadata.GetFunction("GetForecastAsync");
        getForecast.Should().NotBeNull();
        getForecast!.ReturnType.Should().Contain("List");
    }

    [Fact]
    public void ExtractFromType_ShouldHandleOptionalParameters()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        var getForecast = result.Metadata.GetFunction("GetForecastAsync");
        getForecast.Should().NotBeNull();

        var daysParam = getForecast!.GetParameter("days");
        daysParam.Should().NotBeNull();
        daysParam!.IsRequired.Should().BeFalse();
        daysParam.DefaultValue.Should().Be(3);
    }

    [Fact]
    public void ExtractFromType_ShouldHandleFunctionWithoutDescription()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        var getAlert = result.Metadata.GetFunction("GetWeatherAlert");
        getAlert.Should().NotBeNull();
        getAlert!.Description.Should().BeNull();
    }

    [Fact]
    public void ExtractFromType_ShouldUseMethodNameWhenAttributeNameNotProvided()
    {
        // Arrange
        var pluginType = typeof(WeatherPlugin);

        // Act
        var result = _extractor.ExtractFromType(pluginType);

        // Assert
        result.Metadata.HasFunction("GetWeatherAlert").Should().BeTrue();
    }

    [Fact]
    public async Task ExtractFromAssemblyAsync_WithInvalidPath_ShouldReturnFailure()
    {
        // Arrange
        var assemblyPath = "non-existent-path.dll";

        // Act
        var result = await _extractor.ExtractFromAssemblyAsync(assemblyPath);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not found");
    }

    [Fact]
    public async Task ExtractFromAssemblyAsync_WithEmptyPath_ShouldReturnFailure()
    {
        // Arrange
        var assemblyPath = "";

        // Act
        var result = await _extractor.ExtractFromAssemblyAsync(assemblyPath);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("cannot be empty");
    }
}

// Test helper: Abstract plugin base class
public abstract class AbstractPluginBase
{
    public abstract string GetData();
}
