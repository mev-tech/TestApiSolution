using TestApi.Domain.Entities;

namespace TestApi.Dal.Repositories;

/// <summary>Data access contract for weather forecasts.</summary>
public interface IWeatherForecastRepository
{
    Task<IReadOnlyList<WeatherForecast>> GetAllAsync();
    Task<WeatherForecast?> GetByIdAsync(int id);
    Task<WeatherForecast> CreateAsync(WeatherForecast forecast);
    Task<WeatherForecast> UpdateAsync(WeatherForecast forecast);
    Task DeleteAsync(WeatherForecast forecast);
    Task<bool> AnyAsync();
    Task AddRangeAsync(IEnumerable<WeatherForecast> forecasts);
}
