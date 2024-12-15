namespace SandboxService.Core.Interfaces.Services;

public record BinancePriceResponse(string Symbol, decimal Price);

public interface IBinanceService
{
    Task<BinancePriceResponse> GetPrice(string symbol);
    Task ConnectToTickerStream(string symbol, Func<decimal, Task> onPriceUpdate, CancellationToken cancellationToken);
}
