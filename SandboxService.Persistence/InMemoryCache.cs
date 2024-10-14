using SandboxService.Core;
using SandboxService.Core.Models;

namespace SandboxService.Persistence;

public class InMemoryCache : IMemoryCache
{
    public Dictionary<Guid, UserData> Data { get; } = [];
}
