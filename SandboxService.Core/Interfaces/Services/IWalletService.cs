using LanguageExt.Common;
using SandboxService.Core.Models;

namespace SandboxService.Core.Interfaces.Services;

public interface IWalletService
{
    Task<Result<Account>> EnsureExists(Guid userId, string ticker);
}