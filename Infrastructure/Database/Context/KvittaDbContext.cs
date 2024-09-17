using System.ComponentModel;
using Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Context;

public class KvittaDbContext(DbContextOptions<KvittaDbContext> options) : DbContext(options)
{
    public DbSet<Valuable> Valuables { get; set; }

    public DbSet<Warranty> Warranties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Valuable>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasMaxLength(25);
            entity.Property(x => x.Description).HasMaxLength(255);
        });
    }
}