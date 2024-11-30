using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Commands.CreateAccount;

public class CreateAccountQueryHandler(UnitOfWork unitOfWork) : IRequestHandler<CreateAccountQuery, Result<Account>>
{
    public async Task<Result<Account>> Handle(CreateAccountQuery request, CancellationToken cancellationToken)
    {
        var wallet = await unitOfWork.WalletRepository.GetByIdAsync(request.WalletId, includeProperties: "Accounts");
        var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(request.CurrencyId);

        if (wallet is null)
        {
            return new Result<Account>(new SandboxException("Wallet does not exist",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }
        
        if (currency is null)
        {
            return new Result<Account>(new SandboxException("Currency does not exist",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        var account = AccountExtensions.Create(wallet.Id, currency.Id, 0);
        
        wallet.Accounts.Add(account);

        return account;
    }
}