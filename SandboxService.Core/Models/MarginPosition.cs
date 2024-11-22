namespace SandboxService.Core.Models;

public class MarginPosition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public decimal EntryPrice { get; set; }
    public string Symbol { get; set; }
    public bool IsLong { get; set; }
    public decimal Leverage { get; set; }
    public bool IsClosed { get; set; } = false;
    public DateTime OpenDate { get; set; } = DateTime.UtcNow;
    public DateTime? CloseDate { get; set; } = null;
}