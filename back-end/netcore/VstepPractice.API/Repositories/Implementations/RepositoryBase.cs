using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public abstract class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>, IDisposable
    where TEntity : class, IEntity<TKey>
{
    protected readonly ApplicationDbContext _context;

    protected RepositoryBase(ApplicationDbContext context)
        => _context = context;

    public void Dispose()
        => _context?.Dispose();

    public virtual IQueryable<TEntity> FindAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking();

        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public virtual async Task<TEntity?> FindByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties)
            .AsTracking()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public virtual async Task<TEntity?> FindSingleAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties)
            .AsTracking()
            .SingleOrDefaultAsync(predicate, cancellationToken);

    public virtual void Add(TEntity entity)
        => _context.Add(entity);

    public virtual void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);

    public virtual void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public virtual void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);
}
