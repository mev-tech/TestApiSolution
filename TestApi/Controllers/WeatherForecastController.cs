using Microsoft.AspNetCore.Mvc;
using TestApi.Bll.Services;
using TestApi.Common.Exceptions;
using TestApi.Domain.Entities;

namespace TestApi.Controllers;

/// <summary>CRUD endpoints for weather forecasts.</summary>
[ApiController]
[Route("weatherforecast")]
public class WeatherForecastController(IWeatherForecastService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await service.GetByIdAsync(id));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WeatherForecast forecast)
    {
        var created = await service.CreateAsync(forecast);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WeatherForecast forecast)
    {
        try
        {
            return Ok(await service.UpdateAsync(id, forecast));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
