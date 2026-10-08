using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IEntradaCompraImportacaoService
{
    Task<EntradaCompraImportacaoResultadoDto> ImportarAsync(
        Stream arquivo,
        string nomeArquivo,
        long tamanhoArquivo,
        Guid fornecedorId,
        string numeroDocumento,
        DateTime dataEntrada,
        decimal valorFrete,
        decimal valorDesconto,
        decimal outrasDespesas,
        string? observacao,
        Guid usuarioId,
        CancellationToken ct = default);
}
