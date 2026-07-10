using Ibatech.Domain.DTOs;
using Ibatech.Services.DTOs.Produto;

namespace Ibatech.Services.Importacao;

internal sealed class ProdutoImportacaoValidacaoResultado
{
    public bool Sucesso { get; init; }
    public int TotalLinhas { get; init; }
    public int LinhasValidas { get; init; }
    public int LinhasComErro { get; init; }
    public IReadOnlyList<ProdutoImportacaoLinhaValidada> Linhas { get; init; } = new List<ProdutoImportacaoLinhaValidada>();
    public IReadOnlyList<ProdutoImportacaoErroDto> Erros { get; init; } = new List<ProdutoImportacaoErroDto>();
}
