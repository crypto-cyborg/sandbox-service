namespace SandboxService.Application.Data.Dtos;

public record OpenMarginPositionRequest(
    Guid UserId,
    string Ticker,
    decimal Amount,
    string Symbol,
    bool IsLong,
    decimal Leverage,
    decimal? StopLoss,
    decimal? TakeProfit
);