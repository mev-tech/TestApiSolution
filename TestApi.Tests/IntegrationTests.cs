namespace TestApi.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TestApi.Dal;
using TestApi.Domain.Entities;

// Match ASP.NET Core's camelCase JSON output when deserializing responses.
file static class Json
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}

/// <summary>
/// Integration tests for the TestApi endpoints using WebApplicationFactory with an in-memory database.
/// </summary>
public class IntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _factory = CreateFactory();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_factory is not null)
            await _factory.DisposeAsync();
    }

    private static WebApplicationFactory<Program> CreateFactory(string? dbName = null)
    {
        // Use an explicit InMemoryDatabaseRoot so the seeding scope and every request
        // scope share the exact same underlying in-memory data store.
        var resolvedName = dbName ?? Guid.NewGuid().ToString();
        var root = new InMemoryDatabaseRoot();

        // ConfigureTestServices runs AFTER the app's own service registrations,
        // so it can safely override the Npgsql DbContext with an InMemory one.
        return new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.ConfigureTestServices(services =>
            {
                var toRemove = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();
                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(resolvedName, root)
                           .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
                );
            })
        );
    }

    // --- GET /weatherforecast ---

    [Fact]
    public async Task GetWeatherForecast_ShouldReturn200Ok()
    {
        var response = await _client!.GetAsync("/weatherforecast");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnJsonArray()
    {
        var response = await _client!.GetAsync("/weatherforecast");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnSeededForecasts()
    {
        var response = await _client!.GetAsync("/weatherforecast");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(10, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnValidWeatherForecasts()
    {
        var response = await _client!.GetAsync("/weatherforecast");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        foreach (var forecast in doc.RootElement.EnumerateArray())
        {
            Assert.True(forecast.TryGetProperty("id", out var idElement));
            Assert.True(forecast.TryGetProperty("date", out var dateElement));
            Assert.True(forecast.TryGetProperty("temperatureC", out var tempCElement));
            Assert.True(forecast.TryGetProperty("summary", out var summaryElement));
            Assert.True(forecast.TryGetProperty("temperatureF", out var tempFElement));

            Assert.Equal(JsonValueKind.Number, idElement.ValueKind);
            Assert.Equal(JsonValueKind.String, dateElement.ValueKind);
            Assert.Equal(JsonValueKind.Number, tempCElement.ValueKind);
            Assert.True(summaryElement.ValueKind is JsonValueKind.String or JsonValueKind.Null);
            Assert.Equal(JsonValueKind.Number, tempFElement.ValueKind);
        }
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldHaveValidContentType()
    {
        var response = await _client!.GetAsync("/weatherforecast");

        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Contains("application/json", response.Content.Headers.ContentType.MediaType!);
    }

    [Fact]
    public async Task GetWeatherForecast_TemperatureRangeIsValid()
    {
        var response = await _client!.GetAsync("/weatherforecast");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        foreach (var forecast in doc.RootElement.EnumerateArray())
        {
            var tempC = forecast.GetProperty("temperatureC").GetInt32();
            Assert.InRange(tempC, -20, 55);
        }
    }

    // --- GET /weatherforecast/{id} ---

    [Fact]
    public async Task GetWeatherForecastById_ShouldReturn200Ok()
    {
        var listResponse = await _client!.GetAsync("/weatherforecast");
        listResponse.EnsureSuccessStatusCode();
        var items = await listResponse.Content.ReadFromJsonAsync<WeatherForecast[]>(Json.Options);
        var id = items![0].Id;

        var response = await _client!.GetAsync($"/weatherforecast/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecastById_ShouldReturn404ForUnknownId()
    {
        var response = await _client!.GetAsync("/weatherforecast/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- POST /weatherforecast ---

    [Fact]
    public async Task PostWeatherForecast_ShouldReturn201Created()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var newForecast = new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 22, Summary = "Warm" };

        var response = await client.PostAsJsonAsync("/weatherforecast", newForecast);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task PostWeatherForecast_ShouldPersistAndBeRetrievable()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var newForecast = new WeatherForecast { Date = new DateOnly(2030, 6, 15), TemperatureC = 30, Summary = "Hot" };

        var postResponse = await client.PostAsJsonAsync("/weatherforecast", newForecast);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        var getResponse = await client.GetAsync($"/weatherforecast/{created!.Id}");
        getResponse.EnsureSuccessStatusCode();
        var retrieved = await getResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        Assert.Equal(newForecast.Date, retrieved!.Date);
        Assert.Equal(newForecast.TemperatureC, retrieved.TemperatureC);
        Assert.Equal(newForecast.Summary, retrieved.Summary);
    }

    // --- PUT /weatherforecast/{id} ---

    [Fact]
    public async Task PutWeatherForecast_ShouldReturn200Ok()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/weatherforecast",
            new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 10, Summary = "Cool" }
        );
        var created = await postResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        var updated = new WeatherForecast { Date = created!.Date, TemperatureC = 25, Summary = "Warm" };
        var putResponse = await client.PutAsJsonAsync($"/weatherforecast/{created.Id}", updated);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
    }

    [Fact]
    public async Task PutWeatherForecast_ShouldReturn404ForUnknownId()
    {
        var updated = new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 25, Summary = "Warm" };
        var response = await _client!.PutAsJsonAsync("/weatherforecast/99999", updated);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutWeatherForecast_ShouldUpdateValues()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/weatherforecast",
            new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 10, Summary = "Cool" }
        );
        var created = await postResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        var updated = new WeatherForecast { Date = created!.Date, TemperatureC = 35, Summary = "Scorching" };
        await client.PutAsJsonAsync($"/weatherforecast/{created.Id}", updated);

        var getResponse = await client.GetAsync($"/weatherforecast/{created.Id}");
        var retrieved = await getResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        Assert.Equal(35, retrieved!.TemperatureC);
        Assert.Equal("Scorching", retrieved.Summary);
    }

    // --- DELETE /weatherforecast/{id} ---

    [Fact]
    public async Task DeleteWeatherForecast_ShouldReturn204NoContent()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/weatherforecast",
            new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 10, Summary = "Cool" }
        );
        var created = await postResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        var deleteResponse = await client.DeleteAsync($"/weatherforecast/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteWeatherForecast_ShouldReturn404ForUnknownId()
    {
        var response = await _client!.DeleteAsync("/weatherforecast/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWeatherForecast_ShouldRemoveItem()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/weatherforecast",
            new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 10, Summary = "Cool" }
        );
        var created = await postResponse.Content.ReadFromJsonAsync<WeatherForecast>(Json.Options);

        await client.DeleteAsync($"/weatherforecast/{created!.Id}");
        var getResponse = await client.GetAsync($"/weatherforecast/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    // --- /healthz ---

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
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);
        Assert.Equal("healthy", doc.RootElement.GetProperty("status").GetString());
    }

    // --- Logging sink / config tests ---

    [Theory]
    [InlineData("elk")]
    [InlineData("seq")]
    [InlineData("")]
    public async Task App_WithSink_StartsAndResponds(string loggingSink)
    {
        await using var factory = CreateFactory().WithWebHostBuilder(b =>
            b.UseSetting("LoggingSink", loggingSink)
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task App_WithElasticApmEnabled_StartsAndResponds()
    {
        await using var factory = CreateFactory().WithWebHostBuilder(b =>
            b.UseSetting("ElasticApm:Enabled", "true")
        );
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
