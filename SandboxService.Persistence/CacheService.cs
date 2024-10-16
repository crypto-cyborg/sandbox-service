using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;

namespace SandboxService.Persistence;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public UserData Get(Guid key)
    {
        if (_cache.Data.TryGetValue(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException("");
    }

    public void Set(Guid key, UserData value)
    {
        if (!_cache.Data.TryAdd(key, value))
        {
            throw new ArgumentException("");
        }
    }

    public void Delete(Guid key)
    {
        if (!_cache.Data.Remove(key))
        {
            throw new KeyNotFoundException("");
        }
    }

}
