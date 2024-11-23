using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class WalletExtensions
{
    public static Wallet Create(Guid userId, int currencyId, decimal balance = 0)
        => new() { UserId = userId, CurrencyId = currencyId, Balance = balance };

    public record WalletReadDto(
        Guid Id,
        Currency Currency,
        decimal Balance,
        ICollection<TransactionExtensions.TransactionReadDto> Transactions);
}