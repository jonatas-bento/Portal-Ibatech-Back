using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class FornecedorRepository(IbatechDbContext context)
    : RepositoryBase<Fornecedor>(context), IFornecedorRepository
{
    public async Task<IReadOnlyCollection<Fornecedor>> ListarAsync(
        CancellationToken ct = default) =>
        await DbSet
            .AsNoTracking()
            .OrderBy(x => x.Nome)
            .ToListAsync(ct);
}
