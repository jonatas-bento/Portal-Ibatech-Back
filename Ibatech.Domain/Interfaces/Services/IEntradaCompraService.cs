using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IEntradaCompraService
{
    Task<IReadOnlyCollection<EntradaCompraResumoDto>> ListarAsync(
        CancellationToken ct = default);

    Task<EntradaCompraDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<EntradaCompraDetalheDto> CriarAsync(
        CriarEntradaCompraDto dto,
        Guid usuarioId,
        CancellationToken ct = default);

    Task<EntradaCompraDetalheDto> AdicionarItemAsync(
        Guid entradaId,
        AdicionarEntradaCompraItemDto dto,
        Guid usuarioId,
        CancellationToken ct = default);

    Task<EntradaCompraDetalheDto> ConfirmarAsync(
        Guid entradaId,
        Guid usuarioId,
        CancellationToken ct = default);
}
