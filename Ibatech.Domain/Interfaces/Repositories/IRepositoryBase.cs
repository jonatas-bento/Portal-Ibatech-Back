// Ibatech.Domain/Interfaces/Repositories/IRepositoryBase.cs
using System.Linq.Expressions;

namespace Ibatech.Domain.Interfaces.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : class
{
    Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> ObterTodosAsync(CancellationToken ct = default);
    Task<IEnumerable<TEntity>> BuscarAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken ct = default);
    Task AdicionarAsync(TEntity entidade, CancellationToken ct = default);
    void Atualizar(TEntity entidade);
    void Remover(TEntity entidade);
    Task<bool> ExisteAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken ct = default);
    IQueryable<TEntity> ObterQueryable();
}
