using Ibatech.Domain.Entities.Base;

namespace Ibatech.Domain.Entities;

public sealed class VendaItem : EntityBase
{
    public Guid VendaId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string? CodigoSku { get; private set; }
    public string NomeProduto { get; private set; } = null!;
    public string? DescricaoProduto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }

    private VendaItem() { }

    public VendaItem(
        Guid vendaId,
        Guid produtoId,
        string? codigoSku,
        string nomeProduto,
        string? descricaoProduto,
        int quantidade,
        decimal precoUnitario,
        decimal desconto)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("VendaId é obrigatório.");
        if (produtoId == Guid.Empty) throw new ArgumentException("ProdutoId é obrigatório.");
        if (string.IsNullOrWhiteSpace(nomeProduto)) throw new ArgumentException("Nome do produto é obrigatório.");
        if (quantidade < 1) throw new ArgumentException("Quantidade deve ser maior ou igual a 1.");
        if (precoUnitario < 0) throw new ArgumentException("Preço unitário não pode ser negativo.");
        if (desconto < 0) throw new ArgumentException("Desconto não pode ser negativo.");
        if (desconto > (quantidade * precoUnitario)) throw new ArgumentException("Desconto não pode ser superior ao valor total do item.");

        VendaId = vendaId;
        ProdutoId = produtoId;
        CodigoSku = codigoSku;
        NomeProduto = nomeProduto;
        DescricaoProduto = descricaoProduto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        Desconto = desconto;
        CalcularValorTotal();
    }

    public void Atualizar(int quantidade, decimal desconto)
    {
        if (quantidade < 1) throw new ArgumentException("Quantidade deve ser maior ou igual a 1.");
        if (desconto < 0) throw new ArgumentException("Desconto não pode ser negativo.");
        if (desconto > (quantidade * PrecoUnitario)) throw new ArgumentException("Desconto não pode ser superior ao valor total do item.");

        Quantidade = quantidade;
        Desconto = desconto;
        CalcularValorTotal();
        MarcarAtualizado();
    }

    private void CalcularValorTotal()
    {
        ValorTotal = (Quantidade * PrecoUnitario) - Desconto;
    }
}
