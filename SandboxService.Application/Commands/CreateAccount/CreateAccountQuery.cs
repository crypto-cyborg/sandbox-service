using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Extensions;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.CreateAccount;

public class CreateAccountQuery(AccountExtensions.AccountCreateDto data) : IRequest<Result<Account>>
{
    public Guid WalletId { get; } = data.WalletId;
    public int CurrencyId { get; } = data.CurrencyId;
}