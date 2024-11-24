namespace SandboxService.Core.Models;

public class Account
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid WalletId { get; init; }
    
    public required int CurrencyId { get; init; }
    public virtual Currency Currency { get; set; }

    public required decimal Balance { get; set; }
}
