using LanguageExt.Common;
using MediatR;
using SandboxService.Core.Models;

namespace SandboxService.Application.Commands.GetOrders;

public class GetOrdersQuery(Guid userId) : IRequest<Result<IEnumerable<Order>>>
{
    public Guid UserId { get; } = userId;
}