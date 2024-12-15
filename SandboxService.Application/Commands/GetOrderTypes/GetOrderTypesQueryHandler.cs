using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Exceptions;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.GetOrderTypes;

public class GetOrderTypesQueryHandler() : IRequestHandler<GetOrderTypesQuery, Result<IEnumerable<object>>>
{
    public Task<Result<IEnumerable<object>>> Handle(GetOrderTypesQuery request,
        CancellationToken cancellationToken)
    {
        var types = Enum.GetValues(typeof(OrderType))
            .Cast<OrderType>()
            .Select(ot => new { Name = ot.ToString(), Value = (int)ot })
            .ToArray();

        if (types.Length == 0)
        {
            return Task.FromResult(new Result<IEnumerable<object>>(
                new SandboxException("No types were found", SandboxExceptionType.ENTITY_NOT_FOUND)));
        }

        return Task.FromResult(new Result<IEnumerable<object>>(types));
    }
}