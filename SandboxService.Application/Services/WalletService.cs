using LanguageExt.Common;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Services;

public class WalletService(UnitOfWork unitOfWork) : IWalletService
{
    public async Task<Result<Account>> EnsureExists(Guid userId, string ticker)
    {
        throw new NotImplementedException();
    }
}