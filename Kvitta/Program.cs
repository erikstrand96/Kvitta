using Infrastructure.Database.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IServiceCollection services = builder.Services;
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

string connectionString = config.GetConnectionString("POSTGRES_CONNECTION") ??
                          throw new InvalidOperationException("POSTGRES_CONNECTION is not set.");

services.AddKvittaDbContext(connectionString);
services.ApplyMigrations();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();