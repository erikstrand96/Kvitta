using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Infrastructure.Database.Models;

public class Valuable
{
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTimeOffset PurchaseDate { get; set; }

    public double Value { get; set; }

    public string? Description { get; set; }

    public Warranty? Warranty { get; set; }
}