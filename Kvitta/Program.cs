using Infrastructure.Database.Context;
using Infrastructure.Database.Extensions;
using Infrastructure.Database.Models;using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IServiceCollection services = builder.Services;
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

string connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION") ??
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

app.MapGet("/", string () => "Hello World!");

app.MapPost("/valuables", async Task<IResult> (KvittaDbContext dbContext, Valuable valuable) =>
{
    dbContext.Valuables.Add(valuable);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/valuables/{valuable.Id}", valuable);
});

app.MapGet("/valuables/{id}", async (Guid id, KvittaDbContext dbContext) =>
{
    Valuable? valuable = await dbContext.Valuables.FirstOrDefaultAsync(x => x.Id == id);

    if (valuable is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(valuable);
});

app.MapGet("/valuables", async (KvittaDbContext dbContext) =>
{
    List<Valuable> valuables = await dbContext.Valuables.ToListAsync();

    return Results.Ok(valuables);
});

await app.RunAsync();