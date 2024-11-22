namespace SandboxService.Core.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    public long Timestamp { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }

    public decimal Amount { get; set; }
}
