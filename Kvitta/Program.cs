using Infrastructure.Database.Extensions;
using Kvitta.Endpoints;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IServiceCollection services = builder.Services;
// Add services to the container.

builder.Logging.ClearProviders();
services.AddSerilog();
builder.Host.UseSerilog((context, serilogConfig) =>
    {
        serilogConfig.ReadFrom.Configuration(context.Configuration);
    },
    writeToProviders: true);

builder.Services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

services.AddControllers();

string connectionString = Environment.GetEnvironmentVariable("KVITTA_DB_CONNECTION") ??
                          throw new InvalidOperationException("KVITTA_DB_CONNECTION is not set.");

services.AddKvittaDbContext(connectionString);

string aspnetcoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                               ?? throw new Exception("No aspnetcoreEnvironment");

if (aspnetcoreEnvironment == "Development")
{
    services.ApplyMigrations();
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

app.MapGet("/", string () => "Hello NEW World!");

app.MapValuablesEndpoints();

app.MapControllers();

await app.RunAsync();