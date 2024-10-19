namespace SandboxService.Application.Data.Dtos;

public record class SpotPurchaseRequest
{
    public Guid UserId { get; set; }
    public string BaseAsset { get; set; }
    public string QuoteAsset { get; set; }
    public string Symbol { get; init; }
    public decimal Quantity { get; init; }
}
