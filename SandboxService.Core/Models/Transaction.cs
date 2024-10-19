namespace SandboxService.Core.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Currency Currency { get; set; }

    public long Timestamp { get; set; }
    public Guid SenderId { get; set; }

    public decimal Amount { get; set; }
}
