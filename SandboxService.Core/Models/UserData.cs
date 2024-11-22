namespace SandboxService.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string ApiKey { get; set; }
    public string SecretKey { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = [];
    public virtual ICollection<MarginPosition> MarginPositions { get; set; } = [];
}
