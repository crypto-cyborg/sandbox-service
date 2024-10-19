namespace SandboxService.Core.Models;

public class UserData
{
    public Guid UserId { get; set; }
    public string ApiKey { get; set; }
    public string SecretKey { get; set; }

    public ICollection<Wallet> Wallets { get; set; } = [];
}
