using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.CreateAccount;

public class CreateAccountQuery(Guid walletId, int currencyId) : IRequest<Result<Account>>
{
    public Guid WalletId { get; } = walletId;
    public int CurrencyId { get; } = currencyId;
}