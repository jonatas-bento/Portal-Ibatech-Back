using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioResponseDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct = default);
    Task<UsuarioResponseDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<UsuarioResumoDto>> ListarAsync(string? nome, string? email, RoleUsuario? role, bool? ativo, CancellationToken ct = default);
    Task AtualizarAsync(Guid id, AtualizarUsuarioDto dto, CancellationToken ct = default);
    Task AlterarStatusAsync(Guid id, bool ativo, CancellationToken ct = default);
    Task RedefinirSenhaAsync(Guid id, string novaSenha, CancellationToken ct = default);
    Task AlterarMinhaSenhaAsync(Guid id, string senhaAtual, string novaSenha, CancellationToken ct = default);
}
