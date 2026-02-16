using Microsoft.EntityFrameworkCore;
using TestApi.Domain.Entities;

namespace TestApi.Dal;

/// <summary>Database context for the TestApi application.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>Gets or sets the weather forecasts table.</summary>
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Summary).HasMaxLength(100);
            entity.Ignore(e => e.TemperatureF);
        });
    }
}
