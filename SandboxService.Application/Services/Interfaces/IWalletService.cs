using LanguageExt.Common;
using SandboxService.Core.Models;

namespace SandboxService.Application.Services.Interfaces;

public interface IWalletService
{
    Task<Result<Account>> EnsureExists(Guid userId, string ticker);
}