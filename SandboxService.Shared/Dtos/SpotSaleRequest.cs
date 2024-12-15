namespace SandboxService.Shared.Dtos;

public record class SpotSaleRequest
{
    public Guid UserId { get; set; }
    public string Symbol { get; set; }
    public string BaseAsset { get; set; }
    public string QuoteAsset { get; set; }
    public decimal Quantity { get; set; }
}
