using System.Collections;
using Ardalis.ApiEndpoints;
using Infrastructure.Database.Context;
using Infrastructure.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

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

        return  Results.Ok<IEnumerable<Valuable>>(result);
    }
}