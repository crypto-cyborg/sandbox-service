namespace SandboxService.Core.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }

    public decimal Amount { get; set; }
    public decimal Price { get; set; }

    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; } = null;
}

public enum OrderType
{
    Buy,
    Sell,
}

public enum OrderStatus
{
    Pending,
    Completed,
    Canceled,
}
