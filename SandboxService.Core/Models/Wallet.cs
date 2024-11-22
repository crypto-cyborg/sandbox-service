namespace SandboxService.Core.Models;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    public decimal Balance { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = [];
}
