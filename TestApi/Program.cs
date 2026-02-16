using System.Diagnostics.CodeAnalysis;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using TestApi.Bll.Services;
using TestApi.Dal;
using TestApi.Dal.Repositories;
using TestApi.Domain.Entities;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(
        (context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);

            var sink = context.Configuration["LoggingSink"];

            if (string.Equals(sink, "seq", StringComparison.OrdinalIgnoreCase))
            {
                var seqUrl = context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341";
                configuration.WriteTo.Seq(seqUrl);
            }
            else if (
                string.Equals(sink, "elk", StringComparison.OrdinalIgnoreCase)
                || string.Equals(sink, "elasticsearch", StringComparison.OrdinalIgnoreCase)
            )
            {
                var esUrl = context.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
                configuration.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(esUrl))
                    {
                        AutoRegisterTemplate = true,
                        IndexFormat = "testapi-{0:yyyy.MM.dd}",
                    }
                );
            }
        }
    );

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    );

    builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
    builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

    if (builder.Configuration.GetValue<bool>("ElasticApm:Enabled"))
    {
        builder.Services.AddAllElasticApm();
    }

    var app = builder.Build();

    // Apply migrations and seed on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (db.Database.IsRelational())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();

        if (!await db.WeatherForecasts.AnyAsync())
        {
            var summaries = Program.Summaries;
            db.WeatherForecasts.AddRange(
                Enumerable.Range(1, 10).Select(i => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)],
                })
            );
            await db.SaveChangesAsync();
        }
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set(
                "UserAgent",
                httpContext.Request.Headers.UserAgent.ToString()
            );
        };
    });

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

    app.MapControllers();
    app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

[ExcludeFromCodeCoverage]
public partial class Program
{
    internal static readonly string[] Summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    ];
}
