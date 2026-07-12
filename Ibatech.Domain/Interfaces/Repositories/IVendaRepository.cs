using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;

namespace Ibatech.Domain.Interfaces.Repositories;

public interface IVendaRepository : IRepositoryBase<Venda>
{
    Task<Venda?> ObterComItensAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Venda?> ObterDetalheAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Venda>> ListarAsync(
        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteNumeroAsync(
        string numero,
        CancellationToken cancellationToken = default);
}
