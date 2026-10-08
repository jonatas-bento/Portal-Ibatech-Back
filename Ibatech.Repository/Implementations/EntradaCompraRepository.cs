using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class EntradaCompraRepository(
    IbatechDbContext context)
    : RepositoryBase<EntradaCompra>(context),
      IEntradaCompraRepository
{
    public async Task<EntradaCompra?> ObterComItensAsync(
        Guid id,
        CancellationToken ct = default) =>
        await DbSet
            .Include(x => x.Fornecedor)
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);

    public async Task<EntradaCompra?> ObterDetalheAsync(
        Guid id,
        CancellationToken ct = default) =>
        await DbSet
            .AsNoTracking()
            .Include(x => x.Fornecedor)
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);

    public async Task<IReadOnlyCollection<EntradaCompra>> ListarAsync(
        CancellationToken ct = default) =>
        await DbSet
            .AsNoTracking()
            .Include(x => x.Fornecedor)
            .OrderByDescending(x => x.DataEntrada)
            .ThenByDescending(x => x.CriadoEm)
            .ToListAsync(ct);

    public async Task<bool> ExisteDocumentoAsync(
        Guid fornecedorId,
        string numeroDocumento,
        CancellationToken ct = default) =>
        await DbSet
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.FornecedorId == fornecedorId &&
                    x.NumeroDocumento ==
                        numeroDocumento.Trim(),
                ct);

    public void AdicionarItem(
        EntradaCompraItem item) =>
        context
            .Set<EntradaCompraItem>()
            .Add(item);

    public void RemoverItem(
        EntradaCompraItem item) =>
        context
            .Set<EntradaCompraItem>()
            .Remove(item);
}
