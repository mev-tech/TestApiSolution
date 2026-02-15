var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

/// <summary>
/// Represents a weather forecast with date, temperature in Celsius, and summary description.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="TemperatureC">Temperature in Celsius.</param>
/// <param name="Summary">Weather condition summary (e.g., "Mild", "Warm").</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
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
