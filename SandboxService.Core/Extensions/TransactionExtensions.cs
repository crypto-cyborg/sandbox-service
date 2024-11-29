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
        decimal Amount,
        string TransactionType,
        string TradeType);

    public static Transaction Create(Guid senderId, Guid receiverId, Guid walletId, decimal amount, int currencyId, TransactionType transactionType, TradeType tradeType)
    {
        return new Transaction
        {
            CurrencyId = currencyId,
            SenderId = senderId,
            ReceiverId = receiverId,
            Amount = amount,
            TransactionType = transactionType,
            TradeType = tradeType
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
            transaction.Amount,
            transaction.TransactionType.ToString(),
            transaction.TradeType.ToString());

    public static IEnumerable<TransactionReadDto> MapToResponse(this IEnumerable<Transaction> transactions)
        => transactions.Select(t => t.MapToResponse());
}