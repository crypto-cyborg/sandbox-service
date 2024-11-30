using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

    public async Task ConnectToTickerStream(string symbol, Func<decimal, Task> onPriceUpdate, CancellationToken cancellationToken)
    {
        var url = $"wss://stream.binance.com:9443/ws/{symbol.ToLower()}@ticker";

        using var webSocket = new ClientWebSocket();

        try
        {
            await webSocket.ConnectAsync(new Uri(url), cancellationToken);

            Console.WriteLine($"Connected to {url}");

            var buffer = new byte[4096];
            while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(buffer, cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Connection closed.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await ProcessMessageAsync(message, onPriceUpdate);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in WebSocket connection: {e.Message}");
            throw;
        }
        finally
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", cancellationToken);
            }
        }
    }

    private async Task ProcessMessageAsync(string message, Func<decimal, Task> onPriceUpdate)
    {
        try
        {
            var json = JsonDocument.Parse(message);
            var priceString = json.RootElement.GetProperty("c").GetString();
            if (decimal.TryParse(priceString, out var price))
            {
                await onPriceUpdate(price);
            }
            else
            {
                Console.WriteLine("Failed to parse price.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }
}