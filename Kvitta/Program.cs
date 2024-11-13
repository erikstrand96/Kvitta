using HealthChecks.UI.Client;
using Infrastructure.Database.Extensions;
using Kvitta.Endpoints;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

string aspnetcoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                       throw new ApplicationException("No ASPNETCORE_ENVIRONMENT set!");

bool isDevelopmentEnv = aspnetcoreEnv is "Development";

builder.Logging.ClearProviders();

string serviceName = builder.Environment.ApplicationName;

builder.Host.UseSerilog((_, logConfig) =>
{
    logConfig.ReadFrom.Configuration(config);

    logConfig.WriteTo.OpenTelemetry(otelConfig =>
    {
        otelConfig.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "Kvitta",
            ["environment.name"] = builder.Environment.EnvironmentName
        };
    });

    if (isDevelopmentEnv)
    {
        logConfig.WriteTo.Console();
    }
});

var services = builder.Services;

ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName).AddAttributes(
    new Dictionary<string, object>
    {
        ["environment.name"] = builder.Environment.EnvironmentName
    });

builder.Services.AddOpenTelemetry().WithMetrics(metrics =>
{
    metrics.SetResourceBuilder(resourceBuilder)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation();

    bool enableConsoleMetrics = config.GetValue<bool>("EnableConsoleMetrics");

    if (isDevelopmentEnv && enableConsoleMetrics)
    {
        metrics.AddConsoleExporter();
    }

    metrics.AddOtlpExporter();
});

services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

string connectionString = config.GetValue<string>("KvittaDbConnection") ??
                          throw new ApplicationException("No database connection set!");

services.AddKvittaDbContext(connectionString);

if (isDevelopmentEnv)
{
    services.ApplyMigrations();
}

services.AddHealthChecks().AddNpgSql(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDevelopmentEnv)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/logtest", void (ILogger logger) => { logger.Warning("Log Test"); });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/hello", string () => "Hello NEW World!\n");

app.MapValuablesEndpoints();

await app.RunAsync();

public abstract partial class Program
{
}