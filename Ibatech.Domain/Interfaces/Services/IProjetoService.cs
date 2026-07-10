using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Interfaces.Services;

public interface IProjetoService
{
    Task<ProjetoResponseDto> CriarAsync(Guid usuarioId, ProjetoCreateDto dto, CancellationToken ct = default);
    Task<IEnumerable<ProjetoResponseDto>> ListarAsync(CancellationToken ct = default);
    Task<IEnumerable<ProjetoResponseDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task<ProjetoResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<ProjetoResponseDto> AtualizarStatusAsync(Guid id, AtualizarStatusDto dto, CancellationToken ct = default);
}
