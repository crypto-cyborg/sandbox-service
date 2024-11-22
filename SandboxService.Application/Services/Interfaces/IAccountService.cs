using SandboxService.Application.Data.Dtos;
using SandboxService.Core.Models;

namespace SandboxService.Application.Services.Interfaces;

public interface IAccountService
{
    Task<User> CreateSandboxUser(SanboxInitializeRequest request);

}
