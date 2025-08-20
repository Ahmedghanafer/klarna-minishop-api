namespace Minishop.Core.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalSEK { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}