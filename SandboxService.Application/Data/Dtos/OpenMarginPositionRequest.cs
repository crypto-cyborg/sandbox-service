namespace SandboxService.Application.Data.Dtos;

public record OpenMarginPositionRequest(
    Guid UserId,
    string Ticker,
    decimal Amount,
    string Symbol,
    bool IsLong
)
{
    private readonly decimal _leverage;
    public decimal Leverage
    {
        get => _leverage;
        init => _leverage = value == 0 ? 1 : value;
    }
};