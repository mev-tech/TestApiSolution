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
    public async Task GetWeatherForecast_MultipleRequests_ShouldAllBeValid()
    {
        // Act - Verify contract-level behavior across multiple requests
        var response1 = await _client!.GetAsync("/weatherforecast");
        var response2 = await _client!.GetAsync("/weatherforecast");

        // Assert - Both requests return valid responses
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        using var doc1 = JsonDocument.Parse(content1);
        using var doc2 = JsonDocument.Parse(content2);

        // Verify both responses have the same structure and data types
        Assert.Equal(5, doc1.RootElement.GetArrayLength());
        Assert.Equal(5, doc2.RootElement.GetArrayLength());

        // Verify all items have required properties
        foreach (var doc in new[] { doc1, doc2 })
        {
            foreach (var forecast in doc.RootElement.EnumerateArray())
            {
                Assert.True(forecast.TryGetProperty("date", out var dateElement));
                Assert.True(forecast.TryGetProperty("temperatureC", out var tempCElement));
                Assert.True(forecast.TryGetProperty("temperatureF", out var tempFElement));
                Assert.Equal(JsonValueKind.String, dateElement.ValueKind);
                Assert.Equal(JsonValueKind.Number, tempCElement.ValueKind);
                Assert.Equal(JsonValueKind.Number, tempFElement.ValueKind);
            }
        }
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

    [Fact]
    public async Task GetHealthz_ShouldReturn200Ok()
    {
        var response = await _client!.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealthz_ShouldReturnHealthyStatus()
    {
        var response = await _client!.GetAsync("/healthz");
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);
        Assert.Equal("healthy", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task App_WithElkSink_StartsAndResponds()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("LoggingSink", "elk")
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task App_WithSeqSink_StartsAndResponds()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("LoggingSink", "seq")
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task App_WithNoSink_StartsAndResponds()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("LoggingSink", "")
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task App_WithElasticApmEnabled_StartsAndResponds()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ElasticApm:Enabled", "true")
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
