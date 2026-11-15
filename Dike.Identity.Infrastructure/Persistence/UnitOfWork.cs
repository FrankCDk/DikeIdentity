using Dike.Identity.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dike.Identity.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly UsersDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(UsersDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException(
                    "A transaction is already active.");
            }

            _transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException(
                    "There is no active transaction.");
            }

            await _transaction.CommitAsync(cancellationToken);

            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(
            CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                return;

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}

