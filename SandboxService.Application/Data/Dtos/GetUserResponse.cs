namespace SandboxService.Application.Data.Dtos;

public record GetUserResponse(Guid Id, string ApiKey, string SecretKey);
