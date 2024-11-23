using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class CurrencyRepository(SandboxContext context) : RepositoryBase<Currency>(context)
{
    public async Task<Currency?> GetByTickerAsync(string ticker)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Ticker == ticker);
    }
}