using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class TransactionExtensions
{
    public record TransactionReadDto(
        Guid Id,
        CurrencyExtensions.CurrencyReadDto Currency,
        long Timestamp,
        Guid SenderId,
        decimal Amount);

    public static Transaction Create(Guid senderId, decimal amount, Currency currency)
    {
        return new Transaction
        {
            Currency = currency,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SenderId = senderId,
            Amount = amount,
        };
    }

    public static TransactionReadDto MapToResponse(this Transaction transaction)
        => new(transaction.Id, transaction.Currency.MapToResponse(), transaction.Timestamp, transaction.SenderId,
            transaction.Amount);
}