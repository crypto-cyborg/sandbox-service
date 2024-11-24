using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class AccountExtensions
{
    public record AccountReadDto(
        Guid Id,
        Guid WalletId,
        int CurrencyId,
        Currency Currency,
        decimal Balance);

    public static Account Create(Guid walletId, int currencyId, decimal balance = 0)
        => new() { WalletId = walletId, CurrencyId = currencyId, Balance = balance };

    public static AccountReadDto MapToResponse(this Account account)
        => new(
            account.Id,
            account.WalletId,
            account.CurrencyId,
            account.Currency,
            account.Balance);

    public static IEnumerable<AccountReadDto> MapToResponse(this IEnumerable<Account> account)
        => account.Select(a => a.MapToResponse());
}