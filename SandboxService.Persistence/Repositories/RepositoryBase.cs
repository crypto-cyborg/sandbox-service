using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Interfaces;
using SandboxService.Persistence.Contexts;

namespace SandboxService.Persistence.Repositories;

public class RepositoryBase<TEntity>(SandboxContext context) : IRepository<TEntity>
    where TEntity : class
{
    internal readonly SandboxContext Context = context;
    internal readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = ""
    )
    {
        IQueryable<TEntity> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        query = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task InsertAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual void Delete(object id)
    {
        var entityToDelete = DbSet.Find(id);
        Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (Context.Entry(entityToDelete).State == EntityState.Detached)
        {
            DbSet.Attach(entityToDelete);
        }

        DbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        DbSet.Attach(entityToUpdate);
        Context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}