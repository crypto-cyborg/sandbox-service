using SandboxService.Core.Models;

namespace SandboxService.Core.Interfaces;

public interface IMemoryCache
{
    public Dictionary<Guid, UserData> Data { get; }
}
