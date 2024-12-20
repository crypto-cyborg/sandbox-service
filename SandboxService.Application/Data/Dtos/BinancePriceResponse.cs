namespace SandboxService.Application.Data.Dtos;

public record class BinancePriceResponse
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
}
