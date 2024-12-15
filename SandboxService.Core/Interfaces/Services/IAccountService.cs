using SandboxService.Core.Extensions;

namespace SandboxService.Core.Interfaces.Services;

public record SandboxInitializeRequest(Guid UserId);

public interface IAccountService
{
    Task<UserExtensions.UserReadDto> CreateSandboxUser(SandboxInitializeRequest request);

}
