using LanguageExt.Common;
using SandboxService.Core.Models;

namespace SandboxService.Core.Interfaces.Services;

public interface IPositionService
{

    Task<MarginPosition> Open(Order order);
    Task<Result<MarginPosition>> Close(Guid? positionId);
}