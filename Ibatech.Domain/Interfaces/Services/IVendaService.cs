using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IVendaService
{
    Task<IReadOnlyCollection<VendaResumoDto>> ListarAsync(
        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> CriarAsync(
        CriarVendaDto dto,
        Guid vendedorId,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarVendaDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AdicionarItemAsync(
        Guid vendaId,
        AdicionarVendaItemDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> AtualizarItemAsync(
        Guid vendaId,
        Guid itemId,
        AtualizarVendaItemDto dto,
        CancellationToken cancellationToken = default);

    Task<VendaDetalheDto> RemoverItemAsync(
        Guid vendaId,
        Guid itemId,
        CancellationToken cancellationToken = default);
}
