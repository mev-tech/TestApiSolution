namespace TestApi.Tests;

// WeatherForecast record for testing (matches Program.cs)
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class WeatherForecastTests
{
    [Fact]
    public void WeatherForecast_ShouldCreateInstance_WithValidParameters()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 20;
        var summary = "Mild";

        // Act
        var forecast = new WeatherForecast(date, temperatureC, summary);

        // Assert
        Assert.NotNull(forecast);
        Assert.Equal(date, forecast.Date);
        Assert.Equal(temperatureC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForPositiveTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 20; // 20°C should be approximately 68°F
        var forecast = new WeatherForecast(date, temperatureC, "Mild");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        // Formula: 32 + (int)(20 / 0.5556) = 32 + (int)(35.99...) = 32 + 35 = 67
        Assert.Equal(67, temperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForZeroTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 0; // 0°C should be 32°F
        var forecast = new WeatherForecast(date, temperatureC, "Freezing");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        // Formula: 32 + (int)(0 / 0.5556) = 32 + 0 = 32
        Assert.Equal(32, temperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForNegativeTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = -20; // -20°C
        var forecast = new WeatherForecast(date, temperatureC, "Freezing");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        // Formula: 32 + (int)(-20 / 0.5556) = 32 + (int)(-35.99...) = 32 + (-35) = -3
        Assert.Equal(-3, temperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForHighTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 55; // Maximum temperature from API range
        var forecast = new WeatherForecast(date, temperatureC, "Scorching");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        // Formula: 32 + (int)(55 / 0.5556) = 32 + (int)(98.98...) = 32 + 98 = 130
        Assert.Equal(130, temperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForLowTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = -20; // Minimum temperature from API range
        var forecast = new WeatherForecast(date, temperatureC, "Freezing");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        Assert.Equal(-3, temperatureF);
    }

    [Theory]
    [InlineData(0, 32)]
    [InlineData(10, 49)]
    [InlineData(20, 67)]
    [InlineData(30, 85)]
    [InlineData(100, 211)]
    [InlineData(-10, 14)]
    [InlineData(-40, -40)]
    public void WeatherForecast_TemperatureF_ShouldCorrectlyConvert_VariousTemperatures(int celsius, int expectedFahrenheit)
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecast(date, celsius, "Test");

        // Act
        var temperatureF = forecast.TemperatureF;

        // Assert
        Assert.Equal(expectedFahrenheit, temperatureF);
    }

    [Fact]
    public void WeatherForecast_ShouldAllowNullSummary()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 15;

        // Act
        var forecast = new WeatherForecast(date, temperatureC, null);

        // Assert
        Assert.Null(forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_ShouldSupportRecordEquality()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecast(date, 20, "Mild");
        var forecast2 = new WeatherForecast(date, 20, "Mild");

        // Act & Assert
        Assert.Equal(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_ShouldNotBeEqual_WhenPropertiesDiffer()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecast(date, 20, "Mild");
        var forecast2 = new WeatherForecast(date, 25, "Warm");

        // Act & Assert
        Assert.NotEqual(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_ShouldSupportWithExpression()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var original = new WeatherForecast(date, 20, "Mild");

        // Act
        var modified = original with { TemperatureC = 25 };

        // Assert
        Assert.Equal(20, original.TemperatureC);
        Assert.Equal(25, modified.TemperatureC);
        Assert.Equal(date, modified.Date);
        Assert.Equal("Mild", modified.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldUseTruncation_NotRounding()
    {
        // Arrange
        // Testing that conversion uses integer truncation, not rounding
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecast(date, 11, "Cool"); // Should truncate down
        var forecast2 = new WeatherForecast(date, 12, "Cool"); // Should truncate down

        // Act
        var tempF1 = forecast1.TemperatureF;
        var tempF2 = forecast2.TemperatureF;

        // Assert
        // 11°C: 32 + (int)(11 / 0.5556) = 32 + (int)(19.79...) = 32 + 19 = 51
        Assert.Equal(51, tempF1);
        // 12°C: 32 + (int)(12 / 0.5556) = 32 + (int)(21.59...) = 32 + 21 = 53
        Assert.Equal(53, tempF2);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldAcceptFutureDates()
    {
        // Arrange
        var futureDate = new DateOnly(2030, 12, 31);
        var forecast = new WeatherForecast(futureDate, 20, "Mild");

        // Act & Assert
        Assert.Equal(futureDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldAcceptPastDates()
    {
        // Arrange
        var pastDate = new DateOnly(2020, 1, 1);
        var forecast = new WeatherForecast(pastDate, 20, "Mild");

        // Act & Assert
        Assert.Equal(pastDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldAcceptEmptyString()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecast(date, 20, string.Empty);

        // Act & Assert
        Assert.Equal(string.Empty, forecast.Summary);
    }

    [Theory]
    [InlineData("Freezing")]
    [InlineData("Bracing")]
    [InlineData("Chilly")]
    [InlineData("Cool")]
    [InlineData("Mild")]
    [InlineData("Warm")]
    [InlineData("Balmy")]
    [InlineData("Hot")]
    [InlineData("Sweltering")]
    [InlineData("Scorching")]
    public void WeatherForecast_Summary_ShouldAccept_AllApiSummaryValues(string summary)
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecast(date, 20, summary);

        // Act & Assert
        Assert.Equal(summary, forecast.Summary);
    }
}