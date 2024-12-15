using LanguageExt.Common;
using MediatR;

namespace SandboxService.Application.Commands.GetOrderTypes;

public class GetOrderTypesQuery : IRequest<Result<IEnumerable<object>>> { }