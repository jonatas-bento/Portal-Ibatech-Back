using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IFornecedorService
{
    Task<IReadOnlyCollection<FornecedorDto>> ListarAsync(
        CancellationToken ct = default);

    Task<FornecedorDto> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<FornecedorDto> CriarAsync(
        CriarFornecedorDto dto,
        CancellationToken ct = default);

    Task<FornecedorDto> AtualizarAsync(
        Guid id,
        AtualizarFornecedorDto dto,
        CancellationToken ct = default);
}
