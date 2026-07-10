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
}