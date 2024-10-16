namespace SandboxService.Core.Models;

public class Transaction
{
    public Currency Currency { get; set; }
    public DateTime Date { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public decimal Amount { get; set; }
}
