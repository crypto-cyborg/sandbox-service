namespace SandboxService.Core.Models;

public class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid WalletId { get; set; }
    public virtual Wallet Wallet { get; set; }

    public int CurrencyId { get; init; }
    public virtual Currency Currency { get; set; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public Guid SenderId { get; init; }
    public Guid ReceiverId { get; init; }

    public required TransactionType TransactionType { get; set; }
    public required TradeType TradeType { get; set; }

    public decimal Amount { get; set; }
}

public enum TransactionType
{
    BUY,
    SELL
}

public enum TradeType
{ 
    SPOT,
    MARGIN
}
