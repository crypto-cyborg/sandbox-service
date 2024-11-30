using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Commands.GetWallet;

public class GetWalletQueryHandler(UnitOfWork unitOfWork) : IRequestHandler<GetWalletQuery, Result<Wallet>>
{
    public async Task<Result<Wallet>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);

        if (user is null)
        {
            return new Result<Wallet>(
                new SandboxException("User does not exist", SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        return user.Wallet;
    }
}