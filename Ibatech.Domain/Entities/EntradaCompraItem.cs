using Ibatech.Domain.Entities.Base;

namespace Ibatech.Domain.Entities;

public sealed class EntradaCompraItem : EntityBase
{
    public Guid EntradaCompraId { get; private set; }
    public Guid ProdutoId { get; private set; }

    // Snapshot histórico
    public string? CodigoSku { get; private set; }
    public string? CodigoFornecedor { get; private set; }
    public string NomeProduto { get; private set; } = null!;

    public int Quantidade { get; private set; }

    // Valor efetivamente cobrado pelo fornecedor
    public decimal PrecoUnitarioCompra { get; private set; }
    public decimal ValorTotalProduto { get; private set; }

    // Rateios da entrada
    public decimal ValorFreteRateado { get; private set; }
    public decimal ValorDescontoRateado { get; private set; }
    public decimal ValorOutrasDespesasRateado { get; private set; }

    // Custo final colocado na loja
    public decimal CustoEfetivoUnitario { get; private set; }

    public Produto? Produto { get; private set; }

    private EntradaCompraItem() { }

    public EntradaCompraItem(
        Guid entradaCompraId,
        Guid produtoId,
        string? codigoSku,
        string? codigoFornecedor,
        string nomeProduto,
        int quantidade,
        decimal precoUnitarioCompra)
    {
        if (entradaCompraId == Guid.Empty)
            throw new ArgumentException("EntradaCompraId é obrigatório.");

        if (produtoId == Guid.Empty)
            throw new ArgumentException("ProdutoId é obrigatório.");

        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new ArgumentException("Nome do produto é obrigatório.");

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        if (precoUnitarioCompra < 0)
            throw new ArgumentException("Preço de compra não pode ser negativo.");

        EntradaCompraId = entradaCompraId;
        ProdutoId = produtoId;

        CodigoSku = Normalizar(codigoSku);
        CodigoFornecedor = Normalizar(codigoFornecedor);
        NomeProduto = nomeProduto.Trim();

        Quantidade = quantidade;
        PrecoUnitarioCompra = precoUnitarioCompra;

        ValorTotalProduto =
            Math.Round(
                quantidade * precoUnitarioCompra,
                2,
                MidpointRounding.AwayFromZero);

        AplicarRateio(0, 0, 0);
    }

    internal void AplicarRateio(
        decimal valorFrete,
        decimal valorDesconto,
        decimal outrasDespesas)
    {
        if (valorFrete < 0)
            throw new ArgumentException("Frete rateado não pode ser negativo.");

        if (valorDesconto < 0)
            throw new ArgumentException("Desconto rateado não pode ser negativo.");

        if (outrasDespesas < 0)
            throw new ArgumentException("Outras despesas rateadas não podem ser negativas.");

        var custoTotalEfetivo =
            ValorTotalProduto
            + valorFrete
            + outrasDespesas
            - valorDesconto;

        if (custoTotalEfetivo < 0)
            throw new InvalidOperationException(
                "O custo efetivo do item não pode ser negativo.");

        ValorFreteRateado = valorFrete;
        ValorDescontoRateado = valorDesconto;
        ValorOutrasDespesasRateado = outrasDespesas;

        CustoEfetivoUnitario =
            Math.Round(
                custoTotalEfetivo / Quantidade,
                4,
                MidpointRounding.AwayFromZero);

        MarcarAtualizado();
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
}
