using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class OrderRepository(SandboxContext context) : RepositoryBase<Order>(context)
{
    public async Task<Order?> GetByTickerAsync(string symbol)
    {
        // Do I need this?
        // var query = DbSet.Include(o => o.Position);

        return await DbSet.FirstOrDefaultAsync(o => o.Symbol == symbol);
    }

    public override async Task<Order?> GetByIdAsync(object id, string includeProperties = "")
    {
        IQueryable<Order> query = DbSet;

        query = query.Include(o => o.Position.Currency);

        return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id") == id);
    }
}