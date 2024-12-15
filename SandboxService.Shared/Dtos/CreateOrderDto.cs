using SandboxService.Core.Models;

namespace SandboxService.Shared.Dtos;

public record CreateOrderDto(
    Guid UserId,
    OrderType Type,
    string Ticker,
    string Symbol,
    decimal Amount,
    bool IsLong,
    decimal Price,
    decimal? Leverage);
