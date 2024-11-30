using MediatR;
using static SandboxService.Core.Extensions.MarginPositionExtensions;

namespace SandboxService.Application.Commands
{
    public class GetAllPositionsQuery(string apiKey) : IRequest<IEnumerable<MarginPositionRead>>
    {
        public string ApiKey { get; set; } = apiKey;
    }
}
