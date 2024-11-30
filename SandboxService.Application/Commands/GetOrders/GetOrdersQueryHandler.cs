using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Models;
using SandboxService.Persistence;

namespace SandboxService.Application.Commands.GetOrders;

public class GetOrdersQueryHandler(UnitOfWork unitOfWork) : IRequestHandler<GetOrdersQuery, Result<IEnumerable<Order>>>
{
    public async Task<Result<IEnumerable<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);

        if (user is null)
        {
            return new Result<IEnumerable<Order>>(new SandboxException("User does not exist",
                SandboxExceptionType.ENTITY_NOT_FOUND));
        }

        return new Result<IEnumerable<Order>>(user.Orders);
    }
}