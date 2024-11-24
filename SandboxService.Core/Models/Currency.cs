namespace SandboxService.Core.Models;

public class Currency
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Ticker { get; init; }
}
