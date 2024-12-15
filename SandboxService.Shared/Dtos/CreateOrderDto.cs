using SandboxService.Core.Models;

namespace SandboxService.Shared.Dtos;

public record CreateOrderDto(
    Guid UserId,
    OrderType Type,
    string Ticker,
    string Symbol,
    decimal Amount,
    bool IsLong,
    decimal Price)
{
    private readonly decimal _leverage;

    public decimal Leverage
    {
        get => _leverage;
        init => _leverage = value == 0 ? 1 : value;
    }
}