namespace SandboxService.Core.Models;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public Currency Currency { get; set; }

    public decimal Balance { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];
}
