using Microsoft.EntityFrameworkCore;
using TestApi.Domain.Entities;

namespace TestApi.Dal.Repositories;

/// <summary>EF Core implementation of <see cref="IWeatherForecastRepository"/>.</summary>
public class WeatherForecastRepository(AppDbContext db) : IWeatherForecastRepository
{
    public async Task<IReadOnlyList<WeatherForecast>> GetAllAsync() =>
        await db.WeatherForecasts.ToListAsync();

    public async Task<WeatherForecast?> GetByIdAsync(int id) =>
        await db.WeatherForecasts.FindAsync(id);

    public async Task<WeatherForecast> CreateAsync(WeatherForecast forecast)
    {
        db.WeatherForecasts.Add(forecast);
        await db.SaveChangesAsync();
        return forecast;
    }

    public async Task<WeatherForecast> UpdateAsync(WeatherForecast forecast)
    {
        db.WeatherForecasts.Update(forecast);
        await db.SaveChangesAsync();
        return forecast;
    }

    public async Task DeleteAsync(WeatherForecast forecast)
    {
        db.WeatherForecasts.Remove(forecast);
        await db.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync() =>
        await db.WeatherForecasts.AnyAsync();

    public async Task AddRangeAsync(IEnumerable<WeatherForecast> forecasts)
    {
        db.WeatherForecasts.AddRange(forecasts);
        await db.SaveChangesAsync();
    }
}
