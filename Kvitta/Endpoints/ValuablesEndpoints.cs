using Infrastructure.Database.Context;
using Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Kvitta.Endpoints;

public static class ValuablesEndpoints
{
    public static IEndpointRouteBuilder MapValuablesEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/valuables", async (KvittaDbContext context) =>
        {
           List<Valuable> valuables = await context.Valuables.ToListAsync();

           return Results.Ok(valuables);
        });
        
        routeBuilder.MapPost("/valuables", async (KvittaDbContext dbContext, Valuable valuable) =>
        {
            dbContext.Valuables.Add(valuable);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/valuables/{valuable.Id}", valuable);
        });

        routeBuilder.MapGet("/valuables/{id}", async (Guid id, KvittaDbContext dbContext) =>
        {
            Valuable? valuable = await dbContext.Valuables.FirstOrDefaultAsync(x => x.Id == id);

            if (valuable is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(valuable);
        });

        routeBuilder.MapDelete("/valuables/{id}", async (KvittaDbContext context, Guid id) =>
        {
            Valuable? valuable = await context.Valuables.FindAsync(id);

            if (valuable is null)
            {
                return Results.NotFound();
            }

            context.Valuables.Remove(valuable);
            await context.SaveChangesAsync();

            return Results.Ok();
        });

        return routeBuilder;
    }
}