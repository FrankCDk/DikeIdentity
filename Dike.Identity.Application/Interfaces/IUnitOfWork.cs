namespace Dike.Identity.Application.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task CommitAsync(
            CancellationToken cancellationToken = default);

        Task RollbackAsync(
            CancellationToken cancellationToken = default);
    }
}

