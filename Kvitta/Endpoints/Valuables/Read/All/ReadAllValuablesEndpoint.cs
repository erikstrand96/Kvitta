using System.Collections;
using Ardalis.ApiEndpoints;
using Infrastructure.Database.Context;
using Infrastructure.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Kvitta;

namespace Kvitta.Endpoints.Valuables.Read.All;

public class ReadAllValuablesEndpoint(KvittaDbContext context) : EndpointBaseAsync
    .WithoutRequest
    .WithResult<IResult>
{
    [HttpGet("/valuables")]
    [SwaggerOperation(Tags = ["ReadAllValuables"])]
    public  override async Task<IResult> HandleAsync(CancellationToken cancellationToken = new())
    {
       List<Valuable> result = await context.Valuables.ToListAsync(cancellationToken);

       IEnumerable<Response> mappedResponse = result.ToResponse();
       
        return  Results.Ok(mappedResponse);
    }
}

internal record Response
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public double Value { get; set; }

    public string? Description { get; set; }
}

internal static class Mapper
{
    internal static IEnumerable<Response> ToResponse(this IEnumerable<Valuable> values)
    {
        return values.Select(x => new Response()
        {
            Id = x.Id,
            Value = x.Value,
            Description = x.Description,
            Name = x.Name,
            PurchaseDate = x.PurchaseDate.ToDateOnly()
        });
    }  
} 