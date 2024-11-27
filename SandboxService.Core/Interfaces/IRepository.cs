using System.Linq.Expressions;

namespace SandboxService.Core.Interfaces;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    Task<TEntity?> GetByIdAsync(object id, string includeProperties = "");
    Task InsertAsync(TEntity entity);
    void Delete(object id);
    void Delete(TEntity entityToDelete);
    void Update(TEntity entityToUpdate);
}