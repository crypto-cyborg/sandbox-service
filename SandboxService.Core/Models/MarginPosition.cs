namespace SandboxService.Core.Models;

public class MarginPosition
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public decimal EntryPrice { get; set; }
    public bool IsLong { get; set; }
    public decimal Leverage { get; set; }
    public bool IsClosed { get; set; } = false;
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
}
