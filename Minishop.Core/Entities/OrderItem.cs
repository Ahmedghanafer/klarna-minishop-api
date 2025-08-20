namespace Minishop.Core.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPriceSEK { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}