using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public class OrderExtensions
{
    public static Order Create(Guid positionId, OrderType orderType, string symbol, decimal amount)
    {
        return new Order
        {
            PositionId = positionId,
            Type = orderType,
            Symbol = symbol,
            Amount = amount
        };
    }
}