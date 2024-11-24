namespace SandboxService.Core.Models;

public class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public required int CurrencyId { get; init; }
    public virtual Currency? Currency { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public required Guid SenderId { get; init; }
    public required Guid ReceiverId { get; init; }

    public decimal Amount { get; set; }
}
