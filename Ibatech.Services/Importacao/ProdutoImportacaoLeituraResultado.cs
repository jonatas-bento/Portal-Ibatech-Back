using Ibatech.Domain.DTOs;
using Ibatech.Services.DTOs.Produto;

namespace Ibatech.Services.Importacao;

internal sealed class ProdutoImportacaoLeituraResultado
{
    public bool Sucesso { get; init; }
    public int TotalLinhas { get; init; }
    public IReadOnlyList<ProdutoImportacaoLinhaDto> Linhas { get; init; } = new List<ProdutoImportacaoLinhaDto>();
    public IReadOnlyList<ProdutoImportacaoErroDto> Erros { get; init; } = new List<ProdutoImportacaoErroDto>();
}
