using Ibatech.Domain.Entities;
namespace Ibatech.Domain.Interfaces.Repositories;

public interface IEstoqueRepository : IRepositoryBase<Estoque>
{
    Task<Estoque?> ObterPorProdutoAsync(Guid produtoId, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Estoque> estoques, CancellationToken ct = default);

    /// <summary>
    /// Carrega, em uma única consulta e de forma rastreada, os estoques
    /// correspondentes aos ProdutoIds informados.
    /// </summary>
    Task<IReadOnlyCollection<Estoque>> ObterPorProdutosAsync(
        IReadOnlyCollection<Guid> produtoIds,
        CancellationToken ct = default);

    /// <summary>
    /// Adiciona um conjunto de movimentações de estoque ao contexto,
    /// sem executar SaveChanges.
    /// </summary>
    Task AdicionarMovimentacoesAsync(
        IEnumerable<MovimentacaoEstoque> movimentacoes,
        CancellationToken ct = default);
}
