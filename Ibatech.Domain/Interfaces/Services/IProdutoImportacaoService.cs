using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IProdutoImportacaoService
{
    Task<ProdutoImportacaoResultadoDto> ImportarAsync(
        Stream arquivo,
        string nomeArquivo,
        long tamanhoArquivo,
        CancellationToken cancellationToken = default);
}
