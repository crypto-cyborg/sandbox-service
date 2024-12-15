namespace SandboxService.Shared.Dtos;

public record class CloseMarginPositionRequest
{
    public Guid UserId { get; set; }
    public Guid PositionId { get; set; }
}
