using Ibatech.Domain.Entities;
namespace Ibatech.Domain.Interfaces.Repositories;

public interface IEstoqueRepository : IRepositoryBase<Estoque>
{
    Task<Estoque?> ObterPorProdutoAsync(Guid produtoId, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Estoque> estoques, CancellationToken ct = default);
}
