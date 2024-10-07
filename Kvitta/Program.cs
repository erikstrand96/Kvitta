using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infrastructure.Database.Extensions;
using Kvitta.Endpoints;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

IServiceCollection services = builder.Services;
// Add services to the container.

builder.Logging.ClearProviders();
services.AddSerilog();
builder.Host.UseSerilog((context, serilogConfig) =>
    {
        serilogConfig.ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName).
            Enrich.FromLogContext();

        string? grafanaLoki = config.GetValue<string>("GrafanaLoki");

        if (!string.IsNullOrWhiteSpace(grafanaLoki))
        {
            serilogConfig.WriteTo.GrafanaLoki(grafanaLoki);
        }
    },
    writeToProviders: true);

builder.Services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

services.AddControllers();

string aspnetcoreEnvironment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new Exception("No aspnetcoreEnvironment");

string? connectionString;

if (aspnetcoreEnvironment == "Development")
{
    connectionString = config.GetConnectionString("KvittaDbConnection") ??
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapGet("/hello", string () =>
{
    Log.Warning("Hello world!");

    return "Hello NEW World!";
});

app.MapValuablesEndpoints();

app.MapControllers();

await app.RunAsync();