namespace TestApi.Domain.Entities;

/// <summary>Represents a weather forecast entry persisted in the database.</summary>
public class WeatherForecast
{
    /// <summary>Gets or sets the primary key.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the forecast date.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Gets or sets the temperature in Celsius.</summary>
    public int TemperatureC { get; set; }

    /// <summary>Gets or sets the weather condition summary.</summary>
    public string? Summary { get; set; }

    /// <summary>Gets the temperature in Fahrenheit.</summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
