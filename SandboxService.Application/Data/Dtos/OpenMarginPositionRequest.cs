using SandboxService.Core.Models;

namespace SandboxService.Application;

public record class OpenMarginPositionRequest
{
    public Guid UserId { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public string Symbol { get; set; }
    public bool IsLong { get; set; }
    public decimal Leverage { get; set; }
}
