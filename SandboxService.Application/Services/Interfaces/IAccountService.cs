using SandboxService.Application.Data.Dtos;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;

namespace SandboxService.Application.Services.Interfaces;

public interface IAccountService
{
    Task<UserExtensions.UserReadDto> CreateSandboxUser(SanboxInitializeRequest request);

}
