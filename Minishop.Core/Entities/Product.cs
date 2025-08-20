namespace Minishop.Core.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal PriceSEK { get; set; }
    public int Stock { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}