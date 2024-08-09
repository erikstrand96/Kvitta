namespace Infrastructure.Database.Models;

public class Valuable
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public DateTimeOffset PurchaseDate { get; set; }

    public double Value { get; set; }

    public string? Description { get; set; }
}