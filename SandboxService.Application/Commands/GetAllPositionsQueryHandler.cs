using MediatR;
using SandboxService.Persistence;
using static SandboxService.Core.Extensions.MarginPositionExtensions;


namespace SandboxService.Application.Commands
{
    internal class GetAllPositionsQueryHandler(UnitOfWork unitOfWork)
        : IRequestHandler<GetAllPositionsQuery, IEnumerable<MarginPositionRead>>
    {
        public async Task<IEnumerable<MarginPositionRead>> Handle(GetAllPositionsQuery request,
            CancellationToken cancellationToken)
        {
            var user = (await unitOfWork.UserRepository.GetAsync(u => u.ApiKey == request.ApiKey)).FirstOrDefault();
            
            // TODO: null check

            return user.MarginPositions.MapToResponse();
        }
    }
}