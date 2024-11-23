namespace SandboxService.Core.Models;

public class Currency
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Ticker { get; set; }
}
