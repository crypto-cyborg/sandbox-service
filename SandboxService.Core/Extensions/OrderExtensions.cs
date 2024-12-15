using SandboxService.Core.Models;

namespace SandboxService.Core.Extensions;

public static class OrderExtensions
{
    public record OrderReadDto(
        Guid Id,
        Guid? PositionId,
        string Status,
        string Type,
        string Symbol,
        decimal Amount,
        decimal Price,
        bool IsLong,
        DateTimeOffset CreatedAt,
        DateTimeOffset? CompletedAt);

    public static Order Create(OrderType orderType, string symbol, decimal amount, decimal price, bool isLong, int currencyId, Guid userId)
    {
        return new Order
        {
            Type = orderType,
            Symbol = symbol,
            PositionAmount = amount,
            Price = price,
            IsLong = isLong,
            CurrencyId = currencyId,
            UserId = userId
        };
    }

    public static OrderReadDto MapToResponse(this Order order)
        => new OrderReadDto(order.Id, order.PositionId, order.Status.ToString(), order.Type.ToString(),
            order.Symbol, order.PositionAmount, order.Price, order.IsLong, order.CreatedAt, order.CompletedAt);

    public static IEnumerable<OrderReadDto> MapToResponse(this IEnumerable<Order> orders)
        => orders.Select(o => o.MapToResponse());
}