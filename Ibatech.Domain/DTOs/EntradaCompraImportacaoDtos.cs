namespace Ibatech.Domain.DTOs;

public sealed class EntradaCompraImportacaoResultadoDto
{
    public bool Sucesso { get; init; }

    public string NomeArquivo { get; init; } = string.Empty;

    public int TotalLinhas { get; init; }

    public int ProdutosCriados { get; init; }

    public Guid? EntradaCompraId { get; init; }

    public EntradaCompraDetalheDto? Entrada { get; init; }

    public IReadOnlyCollection<ProdutoImportacaoErroDto> Erros { get; init; }
        = Array.Empty<ProdutoImportacaoErroDto>();
}
