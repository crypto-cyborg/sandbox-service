﻿namespace SandboxService.Core.Models;

public class MarginPosition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
    public string Symbol { get; set; }
    public decimal Amount { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal Leverage { get; set; }
    public bool IsLong { get; set; }
    public bool IsClosed { get; set; }
    public DateTimeOffset OpenDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CloseDate { get; set; }

    public decimal? TakeProfit { get; set; }
    public decimal? StopLoss { get; set; }

    public User User { get; set; }
    public Currency Currency { get; set; }
}
