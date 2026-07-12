using Ibatech.Domain.Entities;
using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IClienteService
{
    Task<IEnumerable<ClienteResumoDto>> ListarAsync(string? texto, bool? ativo);
    Task<ClienteDetalheDto?> ObterPorIdAsync(Guid id);
    Task<ClienteResumoDto> CriarAsync(CriarClienteDto dto);
    Task<bool> AtualizarAsync(Guid id, AtualizarClienteDto dto);
    Task<bool> AlterarStatusAsync(Guid id, bool ativo);
}
