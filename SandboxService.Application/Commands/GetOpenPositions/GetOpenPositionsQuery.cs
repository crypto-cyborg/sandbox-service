using MediatR;
using static SandboxService.Core.Extensions.MarginPositionExtensions;

namespace SandboxService.Application.Commands.GetOpenPositions
{
    public class GetOpenPositionsQuery(Guid userId) : IRequest<IEnumerable<MarginPositionRead>>
    {
        public Guid UserId { get; set; } = userId;
    }
}
