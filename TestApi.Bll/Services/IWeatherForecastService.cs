using TestApi.Domain.Entities;

namespace TestApi.Bll.Services;

/// <summary>Business logic contract for weather forecast operations.</summary>
public interface IWeatherForecastService
{
    Task<IReadOnlyList<WeatherForecast>> GetAllAsync();
    Task<WeatherForecast> GetByIdAsync(int id);
    Task<WeatherForecast> CreateAsync(WeatherForecast forecast);
    Task<WeatherForecast> UpdateAsync(int id, WeatherForecast forecast);
    Task DeleteAsync(int id);
}
