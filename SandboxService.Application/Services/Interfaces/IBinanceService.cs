using System;
using SandboxService.Application.Data.Dtos;

namespace SandboxService.Application.Services.Interfaces;

public interface IBinanceService
{
    Task<BinancePriceResponse> GetPrice(string symbol);
    Task ConnectToTickerStream(string symbol, Action<decimal> onPriceUpdate, CancellationToken cancellationToken);
}