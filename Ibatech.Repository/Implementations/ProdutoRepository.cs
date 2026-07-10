// Ibatech.Repository/Implementations/ProdutoRepository.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class ProdutoRepository(IbatechDbContext ctx)
    : RepositoryBase<Produto>(ctx), IProdutoRepository
{
    public async Task<Produto?> ObterComEstoqueAsync(
        Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Estoque)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Produto>> ListarComEstoqueAsync(
        CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Estoque)
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);

    public async Task AdicionarMovimentacaoAsync(
        MovimentacaoEstoque mov, CancellationToken ct = default) =>
        await ctx.Movimentacoes.AddAsync(mov, ct);

    public async Task AddRangeAsync(
        IEnumerable<Produto> produtos, CancellationToken ct = default) =>
        await DbSet.AddRangeAsync(produtos, ct);

    public async Task<IReadOnlyCollection<string>> ObterSkusExistentesAsync(
        IEnumerable<string> skus, CancellationToken ct = default)
    {
        var skusNormalizados = skus
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (skusNormalizados.Count == 0)
            return Array.Empty<string>();

        return await DbSet
            .AsNoTracking()
            .Where(p => p.CodigoSku != null && skusNormalizados.Contains(p.CodigoSku))
            .Select(p => p.CodigoSku!)
            .ToListAsync(ct);
    }
}
