using Ibatech.Infra.Context;
using Ibatech.Repository.UnitOfWork;

namespace Ibatech.Repository.UnitOfWork;

public sealed class UnitOfWork(IbatechDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);

    public Task RollbackAsync(CancellationToken ct = default)
    {
        foreach (var entry in context.ChangeTracker.Entries())
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        return Task.CompletedTask;
    }

    public void Dispose() => context.Dispose();
}