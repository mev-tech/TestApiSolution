namespace TestApi.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

/// <summary>
/// Integration tests for the TestApi endpoints using WebApplicationFactory.
/// These tests exercise the actual endpoint code paths to ensure proper coverage.
/// </summary>
public class IntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_client is not null)
        {
            _client.Dispose();
        }

        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturn200Ok()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnJsonArray()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnFiveDays()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(5, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnValidWeatherForecasts()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        using var doc = JsonDocument.Parse(content);
        var forecasts = doc.RootElement;

        foreach (var forecast in forecasts.EnumerateArray())
        {
            Assert.True(forecast.TryGetProperty("date", out var dateElement));
            Assert.True(forecast.TryGetProperty("temperatureC", out var tempCElement));
            Assert.True(forecast.TryGetProperty("summary", out var summaryElement));
            Assert.True(forecast.TryGetProperty("temperatureF", out var tempFElement));

            // Validate property types
            Assert.Equal(JsonValueKind.String, dateElement.ValueKind);
            Assert.Equal(JsonValueKind.Number, tempCElement.ValueKind);
            Assert.True(summaryElement.ValueKind is JsonValueKind.String or JsonValueKind.Null);
            Assert.Equal(JsonValueKind.Number, tempFElement.ValueKind);
        }
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldHaveValidContentType()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");

        // Assert
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Contains("application/json", response.Content.Headers.ContentType.MediaType!);
    }

    [Fact]
    public async Task GetWeatherForecast_MultipleRequests_ShouldReturnDifferentTemperatures()
    {
        // Act
        var response1 = await _client!.GetAsync("/weatherforecast");
        var content1 = await response1.Content.ReadAsStringAsync();

        var response2 = await _client!.GetAsync("/weatherforecast");
        var content2 = await response2.Content.ReadAsStringAsync();

        // Assert - Due to randomness, content should differ at least in temperatures
        using var doc1 = JsonDocument.Parse(content1);
        using var doc2 = JsonDocument.Parse(content2);

        var temps1 = doc1.RootElement
            .EnumerateArray()
            .Select(f => f.GetProperty("temperatureC").GetInt32())
            .ToList();

        var temps2 = doc2.RootElement
            .EnumerateArray()
            .Select(f => f.GetProperty("temperatureC").GetInt32())
            .ToList();

        // At least one temperature should differ (statistically almost certain with random data)
        Assert.NotEqual(temps1, temps2);
    }

    [Fact]
    public async Task GetWeatherForecast_TemperatureRangeIsValid()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        using var doc = JsonDocument.Parse(content);
        foreach (var forecast in doc.RootElement.EnumerateArray())
        {
            var tempC = forecast.GetProperty("temperatureC").GetInt32();
            Assert.InRange(tempC, -20, 55);
        }
    }
}
