using MediatR;
using SandboxService.Persistence;
using static SandboxService.Core.Extensions.MarginPositionExtensions;


namespace SandboxService.Application.Commands
{
    internal class GetOpenPositionsQueryHandler(UnitOfWork unitOfWork)
        : IRequestHandler<GetOpenPositionsQuery, IEnumerable<MarginPositionRead>>
    {
        public async Task<IEnumerable<MarginPositionRead>> Handle(GetOpenPositionsQuery request,
            CancellationToken cancellationToken)
        {
            var user = (await unitOfWork.UserRepository.GetAsync(u => u.ApiKey == request.ApiKey)).FirstOrDefault();
            
            // TODO: null check

            var positions = user!.MarginPositions.Where(mp => !mp.IsClosed).MapToResponse();

            return positions;
        }
    }
}