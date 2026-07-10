using Ibatech.Domain.Enums;

namespace Ibatech.Services.DTOs.Produto;

internal sealed class ProdutoImportacaoLinhaValidada
{
    public int NumeroLinha { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoProduto Tipo { get; init; }
    public decimal PrecoCompra { get; init; }
    public decimal PrecoVenda { get; init; }
    public int QuantidadeInicial { get; init; }
    public int QuantidadeMinima { get; init; }
    public string? CodigoSku { get; init; }
    public string? Descricao { get; init; }
    public string? Marca { get; init; }
    public string? Modelo { get; init; }
}
