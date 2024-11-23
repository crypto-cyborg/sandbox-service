using SandboxService.Persistence.Contexts;
using SandboxService.Persistence.Repositories;

namespace SandboxService.Persistence;

public class UnitOfWork(SandboxContext context) : IDisposable
{
    private readonly SandboxContext _context = context;

    private UserRepository? _userRepository;
    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    private CurrencyRepository? _currencyRepository;
    public CurrencyRepository CurrencyRepository => _currencyRepository ??= new CurrencyRepository(_context);

    public async Task<bool> SaveAsync() => await _context.SaveChangesAsync() > 0; 

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}