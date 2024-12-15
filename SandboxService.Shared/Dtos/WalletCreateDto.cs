using SandboxService.Core.Models;

namespace SandboxService.Shared.Dtos;

public class WalletCreateDto
{
    public Guid UserId { get; set; }

    public Currency Currency { get; set; }
}
