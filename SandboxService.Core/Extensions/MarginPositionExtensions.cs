﻿using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class MarginPositionExtensions
{
    public record MarginPositionRead(
        Guid Id,
        Guid UserId,
        CurrencyExtensions.CurrencyReadDto Currency,
        string Symbol,
        decimal Amount,
        decimal EntryPrice,
        decimal Leverage,
        bool IsLong,
        bool IsClosed,
        DateTimeOffset OpenDate,
        DateTimeOffset? CloseDate);

    public static MarginPositionRead MapToResponse(this MarginPosition mp)
        => new(mp.Id, mp.UserId, mp.Currency.MapToResponse(), mp.Symbol, mp.Amount, mp.EntryPrice, mp.Leverage,
            mp.IsLong, mp.IsClosed, mp.OpenDate, mp.CloseDate);

    public static IEnumerable<MarginPositionRead> MapToResponse(this IEnumerable<MarginPosition> positions)
        => positions.Select(p => p.MapToResponse());
}