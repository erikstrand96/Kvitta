using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Context;

public class KvittaDbContext(DbContextOptions<KvittaDbContext> options) : DbContext(options)
{
    
}