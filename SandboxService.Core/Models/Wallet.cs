namespace SandboxService.Core.Models;

public class Wallet
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid UserId { get; init; }

    public virtual ICollection<Account> Accounts { get; set; } = [];
    public virtual ICollection<Transaction> Transactions { get; set; } = [];
}