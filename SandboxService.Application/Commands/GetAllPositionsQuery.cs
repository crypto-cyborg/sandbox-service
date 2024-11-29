using MediatR;
using static SandboxService.Core.Extensions.MarginPositionExtensions;

namespace SandboxService.Application.Commands
{
    public class GetAllPositionsQuery(Guid userId) : IRequest<IEnumerable<MarginPositionRead>>
    {
        public Guid UserId { get; set; } = userId;
    }
}
