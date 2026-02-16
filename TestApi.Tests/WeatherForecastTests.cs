using TestApi.Domain.Entities;

namespace TestApi.Tests;

/// <summary>
/// Unit tests for the WeatherForecast entity.
/// Tests verify temperature conversion and property assignment.
/// </summary>
public class WeatherForecastTests
{
    [Fact]
    public void WeatherForecast_ShouldCreateInstance_WithValidProperties()
    {
        var date = new DateOnly(2026, 2, 15);
        var forecast = new WeatherForecast { Date = date, TemperatureC = 20, Summary = "Mild" };

        Assert.Equal(date, forecast.Date);
        Assert.Equal(20, forecast.TemperatureC);
        Assert.Equal("Mild", forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForPositiveTemperature()
    {
        // 20°C: 32 + (int)(20 / 0.5556) = 32 + 35 = 67
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20 };
        Assert.Equal(67, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForZeroTemperature()
    {
        // 0°C = 32°F
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 0 };
        Assert.Equal(32, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForNegativeTemperature()
    {
        // -20°C: 32 + (int)(-20 / 0.5556) = 32 + (-35) = -3
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = -20 };
        Assert.Equal(-3, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldConvertFromCelsius_ForHighTemperature()
    {
        // 55°C: 32 + (int)(55 / 0.5556) = 32 + 98 = 130
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 55 };
        Assert.Equal(130, forecast.TemperatureF);
    }

    [Theory]
    [InlineData(0, 32)]
    [InlineData(10, 49)]
    [InlineData(20, 67)]
    [InlineData(30, 85)]
    [InlineData(100, 211)]
    [InlineData(-10, 15)]
    [InlineData(-40, -39)]
    public void WeatherForecast_TemperatureF_ShouldCorrectlyConvert_VariousTemperatures(int celsius, int expectedFahrenheit)
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = celsius };
        Assert.Equal(expectedFahrenheit, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_ShouldAllowNullSummary()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 15, Summary = null };
        Assert.Null(forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldUseTruncation_NotRounding()
    {
        // 11°C: 32 + (int)(11 / 0.5556) = 32 + 19 = 51
        var forecast1 = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 11 };
        // 12°C: 32 + (int)(12 / 0.5556) = 32 + 21 = 53
        var forecast2 = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 12 };

        Assert.Equal(51, forecast1.TemperatureF);
        Assert.Equal(53, forecast2.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldAcceptFutureDates()
    {
        var futureDate = new DateOnly(2030, 12, 31);
        var forecast = new WeatherForecast { Date = futureDate, TemperatureC = 20 };
        Assert.Equal(futureDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldAcceptPastDates()
    {
        var pastDate = new DateOnly(2020, 1, 1);
        var forecast = new WeatherForecast { Date = pastDate, TemperatureC = 20 };
        Assert.Equal(pastDate, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldAcceptEmptyString()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = string.Empty };
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
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = summary };
        Assert.Equal(summary, forecast.Summary);
    }
}
