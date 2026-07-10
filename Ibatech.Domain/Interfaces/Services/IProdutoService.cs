using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Interfaces.Services;

public interface IProdutoService
{
    Task<ProdutoResponseDto> CriarAsync(ProdutoCreateDto dto, CancellationToken ct = default);
    Task<IEnumerable<ProdutoResponseDto>> ListarAsync(CancellationToken ct = default);
    Task<IEnumerable<ProdutoResponseDto>> ListarAlertasReposicaoAsync(CancellationToken ct = default);
    Task RegistrarMovimentacaoAsync(Guid produtoId, TipoMovimentacao tipo, int quantidade,
        Guid? usuarioId, string? motivo, CancellationToken ct = default);
}
