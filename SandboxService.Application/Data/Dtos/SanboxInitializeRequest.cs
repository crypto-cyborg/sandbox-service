namespace SandboxService.Application.Data.Dtos;

public record class SanboxInitializeRequest
{
    public Guid UserId { get; set; }
    public required string ApiKey { get; set; }
    public required string SecretKey { get; set; }
}
