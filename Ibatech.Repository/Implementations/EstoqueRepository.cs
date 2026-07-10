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
}