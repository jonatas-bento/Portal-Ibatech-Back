// Ibatech.Repository/Base/RepositoryBase.cs
using System.Linq.Expressions;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Base;

public abstract class RepositoryBase<TEntity>(IbatechDbContext context)
    : IRepositoryBase<TEntity>
    where TEntity : class
{
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> ObterPorIdAsync(
        Guid id, CancellationToken ct = default) =>
        await DbSet.FindAsync([id], ct);

    public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync(
        CancellationToken ct = default) =>
        await DbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IEnumerable<TEntity>> BuscarAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken ct = default) =>
        await DbSet.AsNoTracking().Where(predicado).ToListAsync(ct);

    public virtual async Task AdicionarAsync(
        TEntity entidade, CancellationToken ct = default) =>
        await DbSet.AddAsync(entidade, ct);

    public virtual void Atualizar(TEntity entidade) =>
        DbSet.Update(entidade);

    public virtual void Remover(TEntity entidade) =>
        DbSet.Remove(entidade);

    public virtual async Task<bool> ExisteAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken ct = default) =>
        await DbSet.AnyAsync(predicado, ct);
}