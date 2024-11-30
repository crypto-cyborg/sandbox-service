using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.CreateAccount;

public class CreateAccountQuery(Guid walletId, string ticker) : IRequest<Result<Account>>
{
    public Guid WalletId { get; } = walletId;
    public string Ticker { get; } = ticker;
}