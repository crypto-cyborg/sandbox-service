using System;
using SandboxService.Application.Data.Dtos;

namespace SandboxService.Application.Services.Interfaces;

public interface IBinanceService
{
    Task<BinancePriceResponse> GetPrice(string symbol);
}
