using SandboxService.Core.Models;

namespace SandboxService.Core.Interfaces;

public interface ICacheService
{
    UserData Get(Guid key);
    void Set(Guid key, UserData value);
    void Delete(Guid key);
}
