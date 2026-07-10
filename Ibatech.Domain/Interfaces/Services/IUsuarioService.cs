using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioResponseDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct = default);
    Task<UsuarioResponseDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<UsuarioResponseDto>> ListarAsync(CancellationToken ct = default);
    Task DesativarAsync(Guid id, CancellationToken ct = default);
}
