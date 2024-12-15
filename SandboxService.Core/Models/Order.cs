namespace SandboxService.Core.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? PositionId { get; set; }
    public virtual MarginPosition? Position { get; set; }

    public Guid UserId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.PENDING;
    public OrderType Type { get; set; }

    public int CurrencyId { get; set; }
    public bool IsLong { get; set; }
    public string Symbol { get; set; }
    public decimal PositionAmount { get; set; }
    public decimal Price { get; set; }
    public decimal? Leverage { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; } = null;
}


public enum OrderStatus
{
    PENDING,
    COMPLETED,
    CANCELED
}

public enum OrderType
{
    SPOT_LOSS,
    TAKE_PROFIT,
    LIMIT,
    MARKET
}