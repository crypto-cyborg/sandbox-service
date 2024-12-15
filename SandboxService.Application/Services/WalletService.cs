using LanguageExt.Common;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Core.Interfaces.Services;

public class WalletService(UnitOfWork unitOfWork) : IWalletService
{
    public async Task<Result<Account>> EnsureExists(Guid userId, string ticker)
    {
        throw new NotImplementedException();
    }
}