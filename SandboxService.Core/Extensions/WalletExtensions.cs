using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class WalletExtensions
{
    public static Wallet Create(Guid userId)
        => new() { UserId = userId };

    public record WalletReadDto(
        Guid Id,
        Guid UserId,
        IEnumerable<AccountExtensions.AccountReadDto> Accounts,
        IEnumerable<TransactionExtensions.TransactionReadDto> Transactions);

    public static WalletReadDto MapToResponse(this Wallet wallet)
        => new(wallet.Id,
            wallet.UserId,
            wallet.Accounts.MapToResponse(),
            wallet.Transactions.MapToResponse());

    public static IEnumerable<WalletReadDto> MapToResponse(this IEnumerable<Wallet> wallets)
        => wallets.Select(w => w.MapToResponse());
}