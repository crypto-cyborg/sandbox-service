using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services.Interfaces;

namespace SandboxService.Application.Services;

public class BinanceService : IBinanceService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient _httpClient = new();

    public BinanceService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<BinancePriceResponse> GetPrice(string symbol)
    {
        var baseUrl = configuration.GetSection("Binance:BaseUrl").Value;
        var url = $"{baseUrl}ticker/price?symbol={symbol}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to get price data from Binance.");

        var json = await response.Content.ReadAsStringAsync();
        var priceData =
            JsonConvert.DeserializeObject<BinancePriceResponse>(json)
            ?? throw new Exception("Failed to deserialize price data from Binance.");

        return priceData;
    }
}
