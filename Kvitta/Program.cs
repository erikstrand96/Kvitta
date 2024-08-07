using Kvitta;
using Kvitta.Data.Context;
using Kvitta.Data.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IServiceCollection services = builder.Services;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

string connectionString = config.GetConnectionString("POSTGRES_CONNECTION") ??
                          throw new InvalidOperationException("POSTGRES_CONNECTION environment variable is not set.");

services.AddKvittaDbContext(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

services.ApplyMigrations();

app.UseHttpsRedirection();

app.MapGet("/tests", async (KvittaDbContext dbContext) =>
{
    var tests = await dbContext.Tests.ToListAsync();
    return tests;
}).WithName("GetAllTests").WithOpenApi();

app.Run();