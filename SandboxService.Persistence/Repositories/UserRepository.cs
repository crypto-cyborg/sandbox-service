using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class UserRepository(SandboxContext context) : RepositoryBase<User>(context)
{
    public override async Task<IEnumerable<User>> GetAsync(
        Expression<Func<User, bool>>? filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<User> query = DbSet
            .Include(u => u.Wallet.Accounts).ThenInclude(a => a.Currency);

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return orderBy is null ? await query.ToListAsync() : await orderBy(query).ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(object id, string includeProperties = "")
    {
        return await DbSet
            .Include(u => u.Wallet.Accounts).ThenInclude(a => a.Currency)
            .Include(u => u.Wallet.Transactions)
            .FirstOrDefaultAsync(u => u.Id == (Guid)id);
    }
}