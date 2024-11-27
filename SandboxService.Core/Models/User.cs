namespace SandboxService.Core.Models;

public class User
{
    public Guid Id { get; init; }
    public required string ApiKey { get; init; } = null!;
    public required string SecretKey { get; init; } = null!;

    public Guid WalletId { get; set; }
    public virtual Wallet Wallet { get; set; }
    
    public byte[] RowVersion { get; set; }
}
