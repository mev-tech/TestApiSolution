using TestApi.Domain.Entities;

namespace TestApi.Tests;

/// <summary>
/// Edge case and boundary tests for WeatherForecast entity.
/// </summary>
public class WeatherForecastEdgeCasesTests
{
    [Fact]
    public void WeatherForecast_TemperatureF_ShouldHandleMaxInt32Temperature()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = int.MaxValue };
        // Should not throw
        _ = forecast.TemperatureF;
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldHandleMinInt32Temperature()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = int.MinValue };
        // Should not throw
        _ = forecast.TemperatureF;
    }

    [Fact]
    public void WeatherForecast_Date_ShouldHandleMinDateOnly()
    {
        var forecast = new WeatherForecast { Date = DateOnly.MinValue, TemperatureC = 20 };
        Assert.Equal(DateOnly.MinValue, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Date_ShouldHandleMaxDateOnly()
    {
        var forecast = new WeatherForecast { Date = DateOnly.MaxValue, TemperatureC = 20, Summary = "Far Future" };
        Assert.Equal(DateOnly.MaxValue, forecast.Date);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleVeryLongString()
    {
        var longSummary = new string('A', 10000);
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = longSummary };

        Assert.Equal(longSummary, forecast.Summary);
        Assert.Equal(10000, forecast.Summary!.Length);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleSpecialCharacters()
    {
        var specialSummary = "Temperature: 20°C ☀️ 🌡️ \"Hot\" & <Sunny>";
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = specialSummary };
        Assert.Equal(specialSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleUnicodeCharacters()
    {
        var unicodeSummary = "晴れ 太陽 🌞 Sól солнце";
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = unicodeSummary };
        Assert.Equal(unicodeSummary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_TemperatureF_ShouldBeConsistent()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 25 };
        Assert.Equal(forecast.TemperatureF, forecast.TemperatureF);
        Assert.Equal(forecast.TemperatureF, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_ToString_ShouldReturnValidString()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = "Mild" };
        var result = forecast.ToString();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void WeatherForecast_GetHashCode_ShouldBeConsistent()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = "Mild" };
        Assert.Equal(forecast.GetHashCode(), forecast.GetHashCode());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void WeatherForecast_ShouldSupport_MultipleInstancesForFiveDayForecast(int dayOffset)
    {
        var baseDate = new DateOnly(2026, 2, 15);
        var date = baseDate.AddDays(dayOffset);
        var forecast = new WeatherForecast { Date = date, TemperatureC = 20 };

        Assert.Equal(date, forecast.Date);
        Assert.True(forecast.Date > baseDate);
    }

    [Fact]
    public void WeatherForecast_Collection_ShouldSupportLinqOperations()
    {
        var forecasts = new List<WeatherForecast>
        {
            new() { Date = new DateOnly(2026, 2, 15), TemperatureC = 10, Summary = "Cool" },
            new() { Date = new DateOnly(2026, 2, 16), TemperatureC = 20, Summary = "Mild" },
            new() { Date = new DateOnly(2026, 2, 17), TemperatureC = 30, Summary = "Warm" }
        };

        Assert.Equal(20, forecasts.Average(f => f.TemperatureC));
        Assert.Equal(30, forecasts.Max(f => f.TemperatureC));
        Assert.Equal(10, forecasts.Min(f => f.TemperatureC));
    }

    [Fact]
    public void WeatherForecast_TemperatureConversion_ShouldMatchApproximateFormula()
    {
        // 0°C = 32°F (exact)
        Assert.Equal(32, new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 0 }.TemperatureF);
        // 100°C → 32 + (int)(100 / 0.5556) = 32 + 179 = 211
        Assert.Equal(211, new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 100 }.TemperatureF);
        // -40°C → 32 + (int)(-40 / 0.5556) = 32 + (-71) = -39
        Assert.Equal(-39, new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = -40 }.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleWhitespaceOnly()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = "   " };
        Assert.Equal("   ", forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_Summary_ShouldHandleNull()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 20, Summary = null };
        Assert.Null(forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_ZeroTemperature_ShouldHandleCorrectly()
    {
        var forecast = new WeatherForecast { Date = new DateOnly(2026, 2, 15), TemperatureC = 0 };
        Assert.Equal(0, forecast.TemperatureC);
        Assert.Equal(32, forecast.TemperatureF);
    }
}
