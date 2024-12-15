using SandboxService.Core.Exceptions;
using SandboxService.Core.Models;

namespace SandboxService.Application.Utilities;

public static class MarginUtilities
{
    public static void ValidatePosition(MarginPosition position)
    {
        if (position.IsClosed)
            throw new SandboxException("Position already closed", SandboxExceptionType.INVALID_OPERATION);
    }

    public static decimal CalculateMargin(decimal amount, decimal entryPrice, decimal leverage)
    {
        return amount * entryPrice / leverage;
    }

    public static decimal CalculatePnl(MarginPosition position, decimal currentPrice)
    {
        return position.IsLong
            ? (currentPrice - position.EntryPrice) * position.PositionAmount
            : (position.EntryPrice - currentPrice) * position.PositionAmount;
    }
}