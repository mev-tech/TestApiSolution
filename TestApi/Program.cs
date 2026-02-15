using Elastic.Apm.NetCoreAll;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

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

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

    if (builder.Configuration.GetValue<bool>("ElasticApm:Enabled"))
    {
        builder.Services.AddAllElasticApm();
    }

    var app = builder.Build();

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

    app.MapGet("/weatherforecast", GetWeatherForecast)
        .WithName("GetWeatherForecast")
        .WithOpenApi();

    app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
    private static readonly string[] Summaries =
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

    public static WeatherForecast[] GetWeatherForecast()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index =>
                new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                )
            )
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
