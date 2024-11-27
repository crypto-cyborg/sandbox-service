using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class UserExtensions
{
    public record UserReadDto(
        Guid Id, 
        string ApiKey, 
        string SecretKey,
        WalletExtensions.WalletReadDto Wallet);

    public static UserReadDto MapToResponse(this User user) =>
        new(user.Id, user.ApiKey, user.SecretKey, user.Wallet.MapToResponse());
}