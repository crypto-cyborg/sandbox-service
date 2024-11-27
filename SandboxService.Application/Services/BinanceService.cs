using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;

namespace SandboxService.Application.Services;

public class BinanceService(HttpClient httpClient) : IBinanceService
{
    public async Task<BinancePriceResponse> GetPrice(string symbol)
    {
        var url = $"ticker/price?symbol={symbol}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to get price data from Binance.");

        var json = await response.Content.ReadAsStringAsync();
        var priceData =
            JsonConvert.DeserializeObject<BinancePriceResponse>(json)
            ?? throw new Exception("Failed to deserialize price data from Binance.");

        return priceData;
    }
}
