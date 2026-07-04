namespace Bookify.Domain.Abstractions;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdReadOnlyAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    void Insert(TEntity entity);
}
