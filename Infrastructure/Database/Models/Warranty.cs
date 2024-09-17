using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Database.Models;

public class Warranty
{
    [Key]
    public Guid Id { get; set; }

    public DateOnly ExpirationDate { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}