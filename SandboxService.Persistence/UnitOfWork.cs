using SandboxService.Persistence.Contexts;
using SandboxService.Persistence.Repositories;

namespace SandboxService.Persistence;

public class UnitOfWork(SandboxContext context) : IDisposable
{
    private readonly SandboxContext _context = context;

    private UserRepository? _userRepository;
    public UserRepository UserRepository => _userRepository ??= new UserRepository(context);

    public async Task<bool> SaveAsync() => await context.SaveChangesAsync() > 0; 

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}