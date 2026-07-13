using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public class VendaRepository(IbatechDbContext context)
    : RepositoryBase<Venda>(context), IVendaRepository
{
    public async Task<Venda?> ObterComItensAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venda?> ObterDetalheAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Venda>> ListarAsync(
        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Include(v => v.Itens).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtros.Numero))
        {
            query = query.Where(v => v.Numero.Contains(filtros.Numero.Trim()));
        }

        if (filtros.ClienteId.HasValue)
        {
            query = query.Where(v => v.ClienteId == filtros.ClienteId);
        }

        if (filtros.VendedorId.HasValue)
        {
            query = query.Where(v => v.VendedorId == filtros.VendedorId);
        }

        if (filtros.Status.HasValue)
        {
            query = query.Where(v => v.Status == filtros.Status);
        }

        if (filtros.DataInicial.HasValue)
        {
            query = query.Where(v => v.DataVenda >= filtros.DataInicial.Value);
        }

        if (filtros.DataFinal.HasValue)
        {
            var limiteFinal = filtros.DataFinal.Value.Date.AddDays(1);
            query = query.Where(v => v.DataVenda < limiteFinal);
        }

        return await query
            .OrderByDescending(v => v.DataVenda)
            .ThenByDescending(v => v.Numero)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteNumeroAsync(
        string numero,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(v => v.Numero == numero.Trim(), cancellationToken);
    }

    public void AdicionarItem(VendaItem item)
    {
        context.Set<VendaItem>().Add(item);
    }
}
