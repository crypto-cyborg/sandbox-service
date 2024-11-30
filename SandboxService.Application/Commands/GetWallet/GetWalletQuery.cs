using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.GetWallet;

public class GetWalletQuery(Guid userId) : IRequest<Result<Wallet>>
{
    public Guid UserId { get; set; } = userId;
}