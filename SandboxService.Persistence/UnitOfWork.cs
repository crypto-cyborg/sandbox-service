using Microsoft.EntityFrameworkCore.Storage;
using SandboxService.Persistence.Contexts;
using SandboxService.Persistence.Repositories;

namespace SandboxService.Persistence;

public class UnitOfWork(SandboxContext context)
{
    private readonly SandboxContext _context = context;

    private UserRepository? _userRepository;
    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);


    private CurrencyRepository? _currencyRepository;
    public CurrencyRepository CurrencyRepository => _currencyRepository ??= new CurrencyRepository(_context);


    private AccountRepository? _accountRepository;
    public AccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_context);


    private WalletRepository? _walletRepository;
    public WalletRepository WalletRepository => _walletRepository ??= new WalletRepository(_context);


    private TransactionRepository? _transactionRepository;
    public TransactionRepository TransactionRepository =>
        _transactionRepository ??= new TransactionRepository(_context);

    private MarginPositionRepository? _marginPositionRepository;
    public MarginPositionRepository MarginPositionRepository =>
        _marginPositionRepository ??= new MarginPositionRepository(_context);

    private OrderRepository? _orderRepository;
    public OrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);

    public async Task SaveAsync() => await _context.SaveChangesAsync();
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}