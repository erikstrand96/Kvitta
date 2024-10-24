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

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddHttpLogging(x => { });

services.AddExceptionHandler<ExceptionHandler>();
services.AddProblemDetails();

bool isDevelopmentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development";

string? uriString = Environment.GetEnvironmentVariable("VaultUri");
if (!string.IsNullOrWhiteSpace(uriString))
{

    Uri keyVaultEndpoint = new Uri(uriString);
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new VisualStudioCredential());
}

IConfiguration config = builder.Configuration;

string serviceName = "Kvitta";

ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName).AddAttributes(new Dictionary<string, object>
{
    ["environment.name"] = builder.Environment.EnvironmentName,
    ["app"] = builder.Environment.ApplicationName
});

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resourceBuilder);

    if (isDevelopmentEnv)
    {
        options.AddConsoleExporter();
    }

    options.AddOtlpExporter();
});

string? otlpEndpoint = config.GetValue<string?>("OtlpEndpoint");
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

string aspnetcoreEnvironment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
    throw new Exception("No ASPNETCORE_ENVIRONMENT env variable set");

string? connectionString;

if (isDevelopmentEnv)
{
    connectionString = Environment.GetEnvironmentVariable("KvittaDbConnection") ??
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

app.UseHttpLogging();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapGet("/hello", string (ILogger<Program> logger) =>
{
    return "Hello NEW World!";
});

app.MapGet("/logtest", void (ILogger<Program> logger) =>
{
    logger.LogInformation("Log Test");
});

app.MapGet("/exception", void () =>
{
    throw new Exception("Testing Exceptions");
});

app.MapValuablesEndpoints();

await app.RunAsync();