using Ibatech.Domain.Entities;
namespace Ibatech.Domain.Interfaces.Repositories;

public interface IProdutoRepository : IRepositoryBase<Produto>
{
    Task<Produto?> ObterComEstoqueAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Produto>> ListarComEstoqueAsync(CancellationToken ct = default);
    Task AdicionarMovimentacaoAsync(MovimentacaoEstoque mov, CancellationToken ct = default);
}