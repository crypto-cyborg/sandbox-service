namespace SandboxService.Core.Models;

public class User
{
    public Guid Id { get; init; }
    public required string ApiKey { get; init; } = null!;
    public required string SecretKey { get; init; } = null!;

    public Guid WalletId { get; init; } 
    public virtual Wallet Wallet { get; init; }
}
