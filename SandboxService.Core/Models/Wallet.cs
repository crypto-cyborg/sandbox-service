namespace SandboxService.Core.Models;

public class Wallet
{
    public Guid Id { get; set; }
    public Currency Currency { get; set; }
    public decimal Value { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = [];
}
