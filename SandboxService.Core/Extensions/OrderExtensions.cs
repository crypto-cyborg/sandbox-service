using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class OrderExtensions
{
    public record OrderReadDto(
        Guid Id,
        Guid PositionId,
        string Status,
        string Type,
        string Symbol,
        decimal Amount,
        DateTimeOffset CreatedAt,
        DateTimeOffset? CompletedAt);

    public static Order Create(Guid positionId, OrderType orderType, string symbol, decimal amount, Guid userId)
    {
        return new Order
        {
            PositionId = positionId,
            Type = orderType,
            Symbol = symbol,
            Amount = amount,
            UserId = userId
        };
    }

    public static OrderReadDto MapToResponse(this Order order)
        => new OrderReadDto(order.Id, order.PositionId, order.Status.ToString(), order.Type.ToString(),
            order.Symbol, order.Amount, order.CreatedAt, order.CompletedAt);

    public static IEnumerable<OrderReadDto> MapToResponse(this IEnumerable<Order> orders)
        => orders.Select(o => o.MapToResponse());
}