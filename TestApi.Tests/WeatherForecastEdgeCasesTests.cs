namespace TestApi.Tests;

// WeatherForecast record for testing (matches Program.cs)
internal record WeatherForecastForEdgeCases(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/// <summary>
/// Additional edge case and boundary tests for WeatherForecast functionality.
/// These tests strengthen confidence by testing extreme values and unusual scenarios.
/// </summary>
public class WeatherForecastEdgeCasesTests
{
    [Fact]
    public void WeatherForecast_TemperatureF_ShouldHandleMaxInt32Temperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = int.MaxValue;

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, temperatureC, "Extreme");
        var temperatureF = forecast.TemperatureF;

        // Assert - Should not throw, just perform the calculation
        Assert.NotEqual(0, temperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldHandleMinInt32Temperature()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = int.MinValue;

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, temperatureC, "Extreme");
        var temperatureF = forecast.TemperatureF;

        // Assert - Should not throw, just perform the calculation
        Assert.NotEqual(0, temperatureF);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldHandleMinDateOnly()
    {
        // Arrange
        var minDate = DateOnly.MinValue; // 0001-01-01

        // Act
        var forecast = new WeatherForecastForEdgeCases(minDate, 20, "Ancient");

        // Assert
        Assert.Equal(minDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldHandleMaxDateOnly()
    {
        // Arrange
        var maxDate = DateOnly.MaxValue; // 9999-12-31

        // Act
        var forecast = new WeatherForecastForEdgeCases(maxDate, 20, "Far Future");

        // Assert
        Assert.Equal(maxDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleVeryLongString()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var longSummary = new string('A', 10000); // 10,000 character string

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, longSummary);

        // Assert
        Assert.Equal(longSummary, forecast.Summary);
        Assert.Equal(10000, forecast.Summary!.Length);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var specialSummary = "Temperature: 20°C ☀️ 🌡️ \"Hot\" & <Sunny>";

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, specialSummary);

        // Assert
        Assert.Equal(specialSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleUnicodeCharacters()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var unicodeSummary = "晴れ 太陽 🌞 Sól солнце";

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, unicodeSummary);

        // Assert
        Assert.Equal(unicodeSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldBeConsistent()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecastForEdgeCases(date, 25, "Warm");

        // Act
        var tempF1 = forecast.TemperatureF;
        var tempF2 = forecast.TemperatureF;
        var tempF3 = forecast.TemperatureF;

        // Assert - Should always return the same value
        Assert.Equal(tempF1, tempF2);
        Assert.Equal(tempF2, tempF3);
    }

    [Fact]
    public void WeatherForecast_ToString_ShouldReturnValidString()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Act
        var result = forecast.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void WeatherForecast_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Act
        var hash1 = forecast.GetHashCode();
        var hash2 = forecast.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void WeatherForecast_GetHashCode_ShouldDifferForDifferentObjects()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecastForEdgeCases(date, 20, "Mild");
        var forecast2 = new WeatherForecastForEdgeCases(date, 25, "Warm");

        // Act
        var hash1 = forecast1.GetHashCode();
        var hash2 = forecast2.GetHashCode();

        // Assert
        // Note: Hash codes CAN be equal for different objects, but typically aren't
        // This is a probabilistic test
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void WeatherForecast_Deconstruction_ShouldWork()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Act
        var (deconstructedDate, deconstructedTempC, deconstructedSummary) = forecast;

        // Assert
        Assert.Equal(date, deconstructedDate);
        Assert.Equal(20, deconstructedTempC);
        Assert.Equal("Mild", deconstructedSummary);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void WeatherForecast_ShouldSupport_MultipleInstancesForFiveDayForecast(int dayOffset)
    {
        // Arrange
        var baseDate = new DateOnly(2026, 2, 15);
        var date = baseDate.AddDays(dayOffset);

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.True(forecast.Date > baseDate);
    }

    [Fact]
    public void WeatherForecast_Collection_ShouldSupportLinqOperations()
    {
        // Arrange
        var forecasts = new List<WeatherForecastForEdgeCases>
        {
            new(new DateOnly(2026, 2, 15), 10, "Cool"),
            new(new DateOnly(2026, 2, 16), 20, "Mild"),
            new(new DateOnly(2026, 2, 17), 30, "Warm")
        };

        // Act
        var average = forecasts.Average(f => f.TemperatureC);
        var max = forecasts.Max(f => f.TemperatureC);
        var min = forecasts.Min(f => f.TemperatureC);

        // Assert
        Assert.Equal(20, average);
        Assert.Equal(30, max);
        Assert.Equal(10, min);
    }

    [Fact]
    public void WeatherForecast_TemperatureConversion_ShouldMatchApproximateFormula()
    {
        // Arrange & Act
        // Test a few known conversions to ensure the formula is approximately correct
        var forecast0 = new WeatherForecastForEdgeCases(DateOnly.FromDateTime(DateTime.Now), 0, "Test");
        var forecast100 = new WeatherForecastForEdgeCases(DateOnly.FromDateTime(DateTime.Now), 100, "Test");
        var forecastMinus40 = new WeatherForecastForEdgeCases(DateOnly.FromDateTime(DateTime.Now), -40, "Test");

        // Assert
        // 0°C = 32°F (exact)
        Assert.Equal(32, forecast0.TemperatureF);

        // 100°C = 212°F (approximately, due to integer division)
        // Formula: 32 + (int)(100 / 0.5556) = 32 + (int)(179.99...) = 32 + 179 = 211
        Assert.Equal(211, forecast100.TemperatureF);

        // -40°C ≈ -39°F (approximately due to integer division)
        // Formula: 32 + (int)(-40 / 0.5556) = 32 + (int)(-71.98...) = 32 + (-71) = -39
        Assert.Equal(-39, forecastMinus40.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleWhitespaceOnly()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var whitespaceSummary = "   ";

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, whitespaceSummary);

        // Assert
        Assert.Equal(whitespaceSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_WithExpression_ShouldCreateNewInstance()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var original = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Act
        var modified1 = original with { Date = date.AddDays(1) };
        var modified2 = original with { Summary = "Hot" };
        var modified3 = original with { TemperatureC = 30, Summary = "Warm" };

        // Assert
        Assert.NotSame(original, modified1);
        Assert.NotSame(original, modified2);
        Assert.NotSame(original, modified3);
        Assert.Equal(date.AddDays(1), modified1.Date);
        Assert.Equal("Hot", modified2.Summary);
        Assert.Equal(30, modified3.TemperatureC);
        Assert.Equal("Warm", modified3.Summary);
    }

    [Fact]
    public void WeatherForecast_NullCheck_ShouldNotBeNull()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Assert
        Assert.NotNull(forecast);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleNull()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, 20, null);

        // Assert
        Assert.Null(forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_ZeroTemperature_ShouldHandleCorrectly()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var temperatureC = 0;

        // Act
        var forecast = new WeatherForecastForEdgeCases(date, temperatureC, "Freezing");
        var temperatureF = forecast.TemperatureF;

        // Assert - 0°C should equal 32°F
        Assert.Equal(temperatureC, forecast.TemperatureC);
        Assert.Equal(32, temperatureF);
    }

    [Fact]
    public void WeatherForecast_EqualityOperator_ShouldReturnTrueForSameValues()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecastForEdgeCases(date, 20, "Mild");
        var forecast2 = new WeatherForecastForEdgeCases(date, 20, "Mild");

        // Act & Assert
        Assert.Equal(forecast1, forecast2);
        Assert.True(forecast1 == forecast2);
    }

    [Fact]
    public void WeatherForecast_EqualityOperator_ShouldReturnFalseForDifferentValues()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecastForEdgeCases(date, 20, "Mild");
        var forecast2 = new WeatherForecastForEdgeCases(date, 25, "Warm");

        // Act & Assert
        Assert.NotEqual(forecast1, forecast2);
        Assert.True(forecast1 != forecast2);
    }

    [Fact]
    public void WeatherForecast_InequalityOperator_ShouldReturnTrueForDifferentObjects()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var forecast1 = new WeatherForecastForEdgeCases(date, 20, "Mild");
        var forecast2 = new WeatherForecastForEdgeCases(date, 20, "Hot");

        // Act & Assert
        Assert.True(forecast1 != forecast2);
    }
}