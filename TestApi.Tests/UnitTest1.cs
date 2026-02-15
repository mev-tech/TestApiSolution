namespace TestApi.Tests;

public class WeatherForecastTests
{
    [Fact]
    public void WeatherForecast_CanBeCreatedWithValidData()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var tempC = 20;
        var summary = "Mild";

        // Act
        var forecast = new WeatherForecast(date, tempC, summary);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(tempC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_CalculatesCorrectly_ForZeroCelsius()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 0;
        var forecast = new WeatherForecast(date, tempC, "Freezing");

        // Act
        var tempF = forecast.TemperatureF;

        // Assert
        Assert.Equal(32, tempF); // 0°C = 32°F
    }

    [Fact]
    public void WeatherForecast_TemperatureF_CalculatesCorrectly_ForPositiveTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 20;
        var forecast = new WeatherForecast(date, tempC, "Mild");

        // Act
        var tempF = forecast.TemperatureF;

        // Assert
        // 20°C = 68°F (using formula: 32 + (int)(20 / 0.5556))
        Assert.Equal(68, tempF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_CalculatesCorrectly_ForNegativeTemperature()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = -20;
        var forecast = new WeatherForecast(date, tempC, "Freezing");

        // Act
        var tempF = forecast.TemperatureF;

        // Assert
        // -20°C = -4°F
        Assert.Equal(-4, tempF);
    }

    [Theory]
    [InlineData(-20, -4)]
    [InlineData(-10, 18)]
    [InlineData(0, 32)]
    [InlineData(10, 50)]
    [InlineData(20, 68)]
    [InlineData(25, 77)]
    [InlineData(30, 86)]
    [InlineData(37, 98)]
    [InlineData(50, 122)]
    [InlineData(100, 212)]
    public void WeatherForecast_TemperatureF_CalculatesCorrectly_ForVariousTemperatures(int tempC, int expectedTempF)
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var forecast = new WeatherForecast(date, tempC, "Test");

        // Act
        var actualTempF = forecast.TemperatureF;

        // Assert
        Assert.Equal(expectedTempF, actualTempF);
    }

    [Fact]
    public void WeatherForecast_Summary_CanBeNull()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 15;

        // Act
        var forecast = new WeatherForecast(date, tempC, null);

        // Assert
        Assert.Null(forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Summary_CanBeEmptyString()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 15;

        // Act
        var forecast = new WeatherForecast(date, tempC, string.Empty);

        // Assert
        Assert.Equal(string.Empty, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Date_AcceptsValidDateOnly()
    {
        // Arrange
        var date = new DateOnly(2030, 12, 31);
        var tempC = 5;
        var summary = "Cool";

        // Act
        var forecast = new WeatherForecast(date, tempC, summary);

        // Assert
        Assert.Equal(2030, forecast.Date.Year);
        Assert.Equal(12, forecast.Date.Month);
        Assert.Equal(31, forecast.Date.Day);
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
    public void WeatherForecast_Summary_AcceptsAllValidSummaryValues(string summary)
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 15;

        // Act
        var forecast = new WeatherForecast(date, tempC, summary);

        // Assert
        Assert.Equal(summary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_RecordEquality_WorksCorrectly_ForEqualForecasts()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 15;
        var summary = "Mild";
        var forecast1 = new WeatherForecast(date, tempC, summary);
        var forecast2 = new WeatherForecast(date, tempC, summary);

        // Act & Assert
        Assert.Equal(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_RecordEquality_WorksCorrectly_ForDifferentDates()
    {
        // Arrange
        var date1 = new DateOnly(2026, 1, 1);
        var date2 = new DateOnly(2026, 1, 2);
        var tempC = 15;
        var summary = "Mild";
        var forecast1 = new WeatherForecast(date1, tempC, summary);
        var forecast2 = new WeatherForecast(date2, tempC, summary);

        // Act & Assert
        Assert.NotEqual(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_RecordEquality_WorksCorrectly_ForDifferentTemperatures()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var forecast1 = new WeatherForecast(date, 15, "Mild");
        var forecast2 = new WeatherForecast(date, 20, "Mild");

        // Act & Assert
        Assert.NotEqual(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_RecordEquality_WorksCorrectly_ForDifferentSummaries()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 15;
        var forecast1 = new WeatherForecast(date, tempC, "Mild");
        var forecast2 = new WeatherForecast(date, tempC, "Cool");

        // Act & Assert
        Assert.NotEqual(forecast1, forecast2);
    }

    [Fact]
    public void WeatherForecast_TemperatureC_AcceptsMinimumValue()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = -20; // Minimum from API

        // Act
        var forecast = new WeatherForecast(date, tempC, "Freezing");

        // Assert
        Assert.Equal(-20, forecast.TemperatureC);
    }

    [Fact]
    public void WeatherForecast_TemperatureC_AcceptsMaximumValue()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 54; // Maximum from API (55 is exclusive)

        // Act
        var forecast = new WeatherForecast(date, tempC, "Scorching");

        // Assert
        Assert.Equal(54, forecast.TemperatureC);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_RoundsCorrectly()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var tempC = 17; // Should produce non-integer Fahrenheit before casting
        var forecast = new WeatherForecast(date, tempC, "Mild");

        // Act
        var tempF = forecast.TemperatureF;

        // Assert
        // Verify that the result is an integer (due to (int) cast)
        Assert.IsType<int>(tempF);
        Assert.Equal(62, tempF);
    }

    [Fact]
    public void WeatherForecast_ToString_IncludesAllProperties()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 15);
        var tempC = 20;
        var summary = "Mild";
        var forecast = new WeatherForecast(date, tempC, summary);

        // Act
        var result = forecast.ToString();

        // Assert
        Assert.Contains("2026-02-15", result);
        Assert.Contains("20", result);
        Assert.Contains("Mild", result);
    }

    [Fact]
    public void WeatherForecast_WithExpression_CreatesModifiedCopy()
    {
        // Arrange
        var original = new WeatherForecast(new DateOnly(2026, 1, 1), 15, "Mild");

        // Act
        var modified = original with { TemperatureC = 20 };

        // Assert
        Assert.Equal(15, original.TemperatureC);
        Assert.Equal(20, modified.TemperatureC);
        Assert.Equal(original.Date, modified.Date);
        Assert.Equal(original.Summary, modified.Summary);
    }

    [Fact]
    public void WeatherForecast_WithExpression_CanModifyMultipleProperties()
    {
        // Arrange
        var original = new WeatherForecast(new DateOnly(2026, 1, 1), 15, "Mild");

        // Act
        var modified = original with { TemperatureC = 25, Summary = "Hot" };

        // Assert
        Assert.Equal(25, modified.TemperatureC);
        Assert.Equal("Hot", modified.Summary);
        Assert.Equal(original.Date, modified.Date);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_HandlesExtremeHeat()
    {
        // Arrange
        var date = new DateOnly(2026, 7, 15);
        var tempC = 50; // Extreme heat

        // Act
        var forecast = new WeatherForecast(date, tempC, "Scorching");

        // Assert
        Assert.Equal(122, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_HandlesExtremeCold()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 15);
        var tempC = -40; // Extreme cold

        // Act
        var forecast = new WeatherForecast(date, tempC, "Freezing");

        // Assert
        Assert.Equal(-40, forecast.TemperatureF); // -40°C = -40°F (same value!)
    }

    [Fact]
    public void WeatherForecast_Date_MinValue()
    {
        // Arrange & Act
        var forecast = new WeatherForecast(DateOnly.MinValue, 0, "Test");

        // Assert
        Assert.Equal(DateOnly.MinValue, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Date_MaxValue()
    {
        // Arrange & Act
        var forecast = new WeatherForecast(DateOnly.MaxValue, 0, "Test");

        // Assert
        Assert.Equal(DateOnly.MaxValue, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_GetHashCode_ConsistentForSameValues()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var forecast1 = new WeatherForecast(date, 15, "Mild");
        var forecast2 = new WeatherForecast(date, 15, "Mild");

        // Act
        var hash1 = forecast1.GetHashCode();
        var hash2 = forecast2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void WeatherForecast_GetHashCode_DifferentForDifferentValues()
    {
        // Arrange
        var forecast1 = new WeatherForecast(new DateOnly(2026, 1, 1), 15, "Mild");
        var forecast2 = new WeatherForecast(new DateOnly(2026, 1, 2), 15, "Mild");

        // Act
        var hash1 = forecast1.GetHashCode();
        var hash2 = forecast2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Theory]
    [InlineData(1, 33)]    // Small positive
    [InlineData(-1, 30)]   // Small negative
    [InlineData(5, 41)]
    [InlineData(-5, 23)]
    [InlineData(15, 59)]
    [InlineData(-15, 5)]
    public void WeatherForecast_TemperatureF_BoundaryValuesAroundFreezing(int tempC, int expectedTempF)
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var forecast = new WeatherForecast(date, tempC, "Test");

        // Act
        var actualTempF = forecast.TemperatureF;

        // Assert
        Assert.Equal(expectedTempF, actualTempF);
    }

    [Fact]
    public void WeatherForecast_Deconstruction_WorksCorrectly()
    {
        // Arrange
        var expectedDate = new DateOnly(2026, 2, 15);
        var expectedTempC = 20;
        var expectedSummary = "Mild";
        var forecast = new WeatherForecast(expectedDate, expectedTempC, expectedSummary);

        // Act
        var (actualDate, actualTempC, actualSummary) = forecast;

        // Assert
        Assert.Equal(expectedDate, actualDate);
        Assert.Equal(expectedTempC, actualTempC);
        Assert.Equal(expectedSummary, actualSummary);
    }

    [Fact]
    public void WeatherForecast_Summary_WithSpecialCharacters()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var specialSummary = "Mild & Sunny ☀️";

        // Act
        var forecast = new WeatherForecast(date, 20, specialSummary);

        // Assert
        Assert.Equal(specialSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Summary_WithVeryLongString()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 1);
        var longSummary = new string('A', 1000);

        // Act
        var forecast = new WeatherForecast(date, 20, longSummary);

        // Assert
        Assert.Equal(1000, forecast.Summary?.Length);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_TruncatesDecimalCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            (tempC: 3, expectedF: 37),  // 3 / 0.5556 = 5.4 -> 32 + 5 = 37
            (tempC: 7, expectedF: 44),  // 7 / 0.5556 = 12.6 -> 32 + 12 = 44
            (tempC: 13, expectedF: 55), // 13 / 0.5556 = 23.4 -> 32 + 23 = 55
        };

        foreach (var (tempC, expectedF) in testCases)
        {
            // Act
            var forecast = new WeatherForecast(new DateOnly(2026, 1, 1), tempC, "Test");

            // Assert
            Assert.Equal(expectedF, forecast.TemperatureF);
        }
    }
}

// Additional record to make the WeatherForecast type available for testing
// This is a copy from Program.cs for testing purposes
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}