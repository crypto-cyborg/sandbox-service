namespace SandboxService.Core.Models;

public class MarginPosition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
    public string Symbol { get; set; }
    public decimal PositionAmount { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal? Leverage { get; set; }
    public bool IsLong { get; set; }
    public bool IsClosed { get; set; } = false;
    public DateTimeOffset OpenDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CloseDate { get; set; }

    public virtual User User { get; set; }
    public virtual Currency Currency { get; set; }
}
