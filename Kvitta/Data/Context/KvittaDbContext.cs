using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Kvitta.Data.Context;

public class KvittaDbContext(DbContextOptions<KvittaDbContext> options) : DbContext(options)
{
    public DbSet<Test> Tests { get; set; }
}

public class Test
{
    [Key] 
    public int Id { get; set; }
    
    [StringLength(20)]
    public string FistName { get; set; }
    
    [StringLength(20)]
    public string LastName { get; set; }
    
    [StringLength(255)] 
    public string? Description { get; set; }
    
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;
}