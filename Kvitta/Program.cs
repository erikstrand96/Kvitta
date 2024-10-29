using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infrastructure.Database.Extensions;
using Kvitta;
using Kvitta.Endpoints;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;
bool isDevelopmentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development";

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, logConfig) =>
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

string serviceName = "Kvitta";

ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName).AddAttributes(new Dictionary<string, object>
{
    ["environment.name"] = builder.Environment.EnvironmentName,
    ["app"] = builder.Environment.ApplicationName
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

services.AddControllers();

string? connectionString;

if (isDevelopmentEnv)
{
    connectionString = config.GetValue<string>("KvittaDbConnection") ??
                       throw new ApplicationException("No database connection set!");

    ;
    services.AddKvittaDbContext(connectionString);
    services.ApplyMigrations();
}
else
{
    SecretClientOptions secretClientOptions = new()
    {
        Retry =
        {
            Delay = TimeSpan.FromSeconds(2),
            MaxRetries = 5,
        }
    };

    var client = new SecretClient(new Uri("https://kvitta-keyvault.vault.azure.net/"), new DefaultAzureCredential(),
        secretClientOptions);

    KeyVaultSecret keyVaultSecret = client.GetSecret("KvittaDbConnection");
    connectionString = keyVaultSecret.Value ?? throw new ApplicationException("No database connection set!");

    services.AddKvittaDbContext(connectionString);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDevelopmentEnv)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();


app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapGet("/hello", string (ILogger<Program> logger) => "Hello NEW World!");

app.MapGet("/logtest", void (ILogger logger) =>
{
    logger.Warning("Log Test");
});

app.MapValuablesEndpoints();

await app.RunAsync();