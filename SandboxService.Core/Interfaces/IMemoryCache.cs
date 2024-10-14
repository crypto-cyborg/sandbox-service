using SandboxService.Core.Models;

namespace SandboxService.Core;

public interface IMemoryCache
{
    public Dictionary<Guid, UserData> Data { get; }
}
