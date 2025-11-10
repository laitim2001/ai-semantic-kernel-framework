using AIAgentPlatform.Domain.ValueObjects;
using FluentAssertions;

namespace AIAgentPlatform.UnitTests.Domain.ValueObjects;

public sealed class PluginMetadataTests
{
    [Fact]
    public void Empty_ShouldReturnEmptyMetadata()
    {
        // Act
        var metadata = PluginMetadata.Empty;

        // Assert
        metadata.Should().NotBeNull();
        metadata.Functions.Should().BeEmpty();
        metadata.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithFunctions_ShouldCreateMetadata()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather", "Get current weather");
        var functions = new List<PluginFunction> { function };

        // Act
        var metadata = PluginMetadata.Create(functions);

        // Assert
        metadata.Functions.Should().HaveCount(1);
        metadata.Functions[0].Should().Be(function);
    }

    [Fact]
    public void Create_WithFunctionsAndProperties_ShouldCreateMetadata()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather");
        var functions = new List<PluginFunction> { function };
        var properties = new Dictionary<string, string> { { "Version", "1.0.0" } };

        // Act
        var metadata = PluginMetadata.Create(functions, properties);

        // Assert
        metadata.Functions.Should().HaveCount(1);
        metadata.Properties.Should().HaveCount(1);
        metadata.Properties["Version"].Should().Be("1.0.0");
    }

    [Fact]
    public void GetFunction_WithExistingFunction_ShouldReturnFunction()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather", "Get current weather");
        var functions = new List<PluginFunction> { function };
        var metadata = PluginMetadata.Create(functions);

        // Act
        var result = metadata.GetFunction("GetWeather");

        // Assert
        result.Should().Be(function);
    }

    [Fact]
    public void GetFunction_WithNonExistingFunction_ShouldReturnNull()
    {
        // Arrange
        var metadata = PluginMetadata.Empty;

        // Act
        var result = metadata.GetFunction("NonExisting");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void HasFunction_WithExistingFunction_ShouldReturnTrue()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather");
        var functions = new List<PluginFunction> { function };
        var metadata = PluginMetadata.Create(functions);

        // Act
        var result = metadata.HasFunction("GetWeather");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasFunction_WithNonExistingFunction_ShouldReturnFalse()
    {
        // Arrange
        var metadata = PluginMetadata.Empty;

        // Act
        var result = metadata.HasFunction("NonExisting");

        // Assert
        result.Should().BeFalse();
    }
}

public sealed class PluginFunctionTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateFunction()
    {
        // Arrange
        var name = "GetWeather";
        var description = "Get current weather";

        // Act
        var function = PluginFunction.Create(name, description);

        // Assert
        function.Name.Should().Be(name);
        function.Description.Should().Be(description);
        function.Parameters.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "";

        // Act & Assert
        var action = () => PluginFunction.Create(name);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void CreateWithParameters_WithValidParameters_ShouldCreateFunction()
    {
        // Arrange
        var name = "GetWeather";
        var description = "Get current weather";
        var parameters = new List<PluginParameter>
        {
            PluginParameter.Required("city", "string", "City name")
        };

        // Act
        var function = PluginFunction.CreateWithParameters(name, description, parameters, "WeatherResult");

        // Assert
        function.Name.Should().Be(name);
        function.Description.Should().Be(description);
        function.Parameters.Should().HaveCount(1);
        function.ReturnType.Should().Be("WeatherResult");
    }

    [Fact]
    public void GetParameter_WithExistingParameter_ShouldReturnParameter()
    {
        // Arrange
        var parameter = PluginParameter.Required("city", "string");
        var parameters = new List<PluginParameter> { parameter };
        var function = PluginFunction.CreateWithParameters("GetWeather", null, parameters);

        // Act
        var result = function.GetParameter("city");

        // Assert
        result.Should().Be(parameter);
    }

    [Fact]
    public void GetParameter_WithNonExistingParameter_ShouldReturnNull()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather");

        // Act
        var result = function.GetParameter("city");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void HasParameter_WithExistingParameter_ShouldReturnTrue()
    {
        // Arrange
        var parameter = PluginParameter.Required("city", "string");
        var parameters = new List<PluginParameter> { parameter };
        var function = PluginFunction.CreateWithParameters("GetWeather", null, parameters);

        // Act
        var result = function.HasParameter("city");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParameter_WithNonExistingParameter_ShouldReturnFalse()
    {
        // Arrange
        var function = PluginFunction.Create("GetWeather");

        // Act
        var result = function.HasParameter("city");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var parameters = new List<PluginParameter>
        {
            PluginParameter.Required("city", "string"),
            PluginParameter.Optional("unit", "string", defaultValue: "Celsius")
        };
        var function = PluginFunction.CreateWithParameters("GetWeather", null, parameters);

        // Act
        var result = function.ToString();

        // Assert
        result.Should().Contain("GetWeather");
        result.Should().Contain("city");
        result.Should().Contain("unit");
    }
}

public sealed class PluginParameterTests
{
    [Fact]
    public void Required_WithValidParameters_ShouldCreateRequiredParameter()
    {
        // Arrange
        var name = "city";
        var type = "string";
        var description = "City name";

        // Act
        var parameter = PluginParameter.Required(name, type, description);

        // Assert
        parameter.Name.Should().Be(name);
        parameter.Type.Should().Be(type);
        parameter.Description.Should().Be(description);
        parameter.IsRequired.Should().BeTrue();
        parameter.DefaultValue.Should().BeNull();
    }

    [Fact]
    public void Optional_WithValidParameters_ShouldCreateOptionalParameter()
    {
        // Arrange
        var name = "unit";
        var type = "string";
        var description = "Temperature unit";
        var defaultValue = "Celsius";

        // Act
        var parameter = PluginParameter.Optional(name, type, description, defaultValue);

        // Assert
        parameter.Name.Should().Be(name);
        parameter.Type.Should().Be(type);
        parameter.Description.Should().Be(description);
        parameter.IsRequired.Should().BeFalse();
        parameter.DefaultValue.Should().Be(defaultValue);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "";
        var type = "string";

        // Act & Assert
        var action = () => new PluginParameter(name, type);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Constructor_WithEmptyType_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "city";
        var type = "";

        // Act & Assert
        var action = () => new PluginParameter(name, type);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*type cannot be empty*");
    }

    [Fact]
    public void ToString_ForRequiredParameter_ShouldReturnFormattedString()
    {
        // Arrange
        var parameter = PluginParameter.Required("city", "string");

        // Act
        var result = parameter.ToString();

        // Assert
        result.Should().Contain("string");
        result.Should().Contain("city");
        result.Should().NotContain("?");
    }

    [Fact]
    public void ToString_ForOptionalParameter_ShouldReturnFormattedStringWithQuestionMark()
    {
        // Arrange
        var parameter = PluginParameter.Optional("unit", "string", defaultValue: "Celsius");

        // Act
        var result = parameter.ToString();

        // Assert
        result.Should().Contain("string?");
        result.Should().Contain("unit");
        result.Should().Contain("Celsius");
    }
}
