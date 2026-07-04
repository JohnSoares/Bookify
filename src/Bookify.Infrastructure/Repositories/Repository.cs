using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal abstract class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<T?> GetByIdReadOnlyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entity => entity.Id == id, cancellationToken);
    }

    public virtual void Insert(T entity)
    {
        DbContext.Add(entity);
    }
}
