// Ibatech.Repository/Implementations/EstoqueRepository.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class EstoqueRepository(IbatechDbContext ctx)
    : RepositoryBase<Estoque>(ctx), IEstoqueRepository
{
    public async Task<Estoque?> ObterPorProdutoAsync(
        Guid produtoId, CancellationToken ct = default) =>
        await DbSet
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId, ct);

    public async Task AddRangeAsync(
        IEnumerable<Estoque> estoques, CancellationToken ct = default) =>
        await DbSet.AddRangeAsync(estoques, ct);

    public async Task<IReadOnlyCollection<Estoque>> ObterPorProdutosAsync(
        IReadOnlyCollection<Guid> produtoIds,
        CancellationToken ct = default)
    {
        if (produtoIds.Count == 0)
            return Array.Empty<Estoque>();

        return await DbSet
            .Where(e => produtoIds.Contains(e.ProdutoId))
            .ToListAsync(ct);
    }

    public async Task AdicionarMovimentacoesAsync(
        IEnumerable<MovimentacaoEstoque> movimentacoes,
        CancellationToken ct = default) =>
        await ctx.Set<MovimentacaoEstoque>().AddRangeAsync(movimentacoes, ct);
}
