using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class TransactionExtensions
{
    public record TransactionReadDto(
        Guid Id,
        Guid WalletId,
        int CurrencyId,
        CurrencyExtensions.CurrencyReadDto Currency,
        DateTimeOffset Time,
        Guid SenderId,
        Guid ReceiverId,
        decimal Amount);

    public static Transaction Create(Guid senderId, Guid receiverId, Guid walletId, decimal amount, int currencyId)
    {
        return new Transaction
        {
            CurrencyId = currencyId,
            SenderId = senderId,
            ReceiverId = receiverId,
            Amount = amount,
        };
    }

    public static TransactionReadDto MapToResponse(this Transaction transaction)
        => new(
            transaction.Id,
            transaction.WalletId,
            transaction.CurrencyId,
            transaction.Currency!.MapToResponse(),
            transaction.Timestamp,
            transaction.SenderId,
            transaction.ReceiverId,
            transaction.Amount);

    public static IEnumerable<TransactionReadDto> MapToResponse(this IEnumerable<Transaction> transactions)
        => transactions.Select(t => t.MapToResponse());
}