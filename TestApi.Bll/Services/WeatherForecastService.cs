using TestApi.Common.Exceptions;
using TestApi.Dal.Repositories;
using TestApi.Domain.Entities;

namespace TestApi.Bll.Services;

/// <summary>Service layer implementation for weather forecast operations.</summary>
public class WeatherForecastService(IWeatherForecastRepository repository) : IWeatherForecastService
{
    public Task<IReadOnlyList<WeatherForecast>> GetAllAsync() =>
        repository.GetAllAsync();

    public async Task<WeatherForecast> GetByIdAsync(int id)
    {
        var forecast = await repository.GetByIdAsync(id);
        return forecast ?? throw NotFoundException.For<WeatherForecast>(id);
    }

    public Task<WeatherForecast> CreateAsync(WeatherForecast forecast) =>
        repository.CreateAsync(forecast);

    public async Task<WeatherForecast> UpdateAsync(int id, WeatherForecast forecast)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            throw NotFoundException.For<WeatherForecast>(id);

        existing.Date = forecast.Date;
        existing.TemperatureC = forecast.TemperatureC;
        existing.Summary = forecast.Summary;

        return await repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var forecast = await repository.GetByIdAsync(id);
        if (forecast is null)
            throw NotFoundException.For<WeatherForecast>(id);

        await repository.DeleteAsync(forecast);
    }
}
