namespace SandboxService.Shared.Dtos;

public record GetUserResponse(Guid Id, string ApiKey, string SecretKey);
