namespace SandboxService.Application.Data.Dtos;

public record class SanboxInitializeRequest
{
    public Guid UserId { get; set; }
}
