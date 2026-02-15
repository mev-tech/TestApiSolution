var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Skip HTTPS redirect on Lambda — API Gateway handles TLS termination
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
{
    app.UseHttpsRedirection();
}

app.MapGet("/weatherforecast", GetWeatherForecast)
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

app.Run();

public partial class Program
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public static WeatherForecast[] GetWeatherForecast()
    {
        return Enumerable.Range(1, 5)
            .Select(index =>
                new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray();
    }
}

/// <summary>
/// Represents a weather forecast with date, temperature in Celsius, and summary description.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="TemperatureC">Temperature in Celsius.</param>
/// <param name="Summary">Weather condition summary (e.g., "Mild", "Warm").</param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Gets the temperature in Fahrenheit, converted from TemperatureC.
    /// </summary>
    /// <remarks>
    /// Uses the formula: F = 32 + (C / 0.5556)
    /// Integer division may cause slight inaccuracies.
    /// </remarks>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
