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
            .Include(u => u.Wallets)
            .ThenInclude(w => w.Currency);

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return orderBy is null ? await query.ToListAsync() : await orderBy(query).ToListAsync();
    }
}