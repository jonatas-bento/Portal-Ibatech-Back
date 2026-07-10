using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IFinanceiroService
{
    Task<TransacaoResponseDto> CriarAsync(TransacaoCreateDto dto, CancellationToken ct = default);
    Task<IEnumerable<TransacaoResponseDto>> ListarAsync(CancellationToken ct = default);
    //Task LiquidarAsync(Guid id, CancellationToken ct = default);
    Task<ResumoFinanceiroDto> ObterResumoAsync(CancellationToken ct = default);
}
