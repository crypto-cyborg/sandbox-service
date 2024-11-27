namespace SandboxService.Core.Models;

public class Wallet
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid UserId { get; init; }
    public virtual User User { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = [];
    public virtual ICollection<Transaction> Transactions { get; set; } = [];
}