namespace Ibatech.Domain.DTOs;

public sealed class ProdutoImportacaoResultadoDto
{
    public bool Sucesso { get; init; }
    public string NomeArquivo { get; init; } = string.Empty;
    public int TotalLinhas { get; init; }
    public int LinhasValidas { get; init; }
    public int ProdutosImportados { get; init; }
    public int LinhasComErro { get; init; }
    public List<ProdutoImportacaoErroDto> Erros { get; init; } = new();
}
