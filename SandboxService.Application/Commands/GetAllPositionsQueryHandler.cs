using MediatR;
using SandboxService.Persistence;
using static SandboxService.Core.Extensions.MarginPositionExtensions;


namespace SandboxService.Application.Commands
{
    internal class GetAllPositionsQueryHandler(UnitOfWork unitOfWork)
        : IRequestHandler<GetAllPositionsQuery, IEnumerable<MarginPositionRead>>
    {
        public async Task<IEnumerable<MarginPositionRead>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
        {
            var positions = await unitOfWork.MarginPositionRepository.GetAsync(mp => mp.UserId == request.UserId);

            return positions.MapToResponse();
        }
    }
}
