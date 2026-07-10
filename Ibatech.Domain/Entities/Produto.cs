// Ibatech.Domain/Entities/Produto.cs
using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class Produto : EntityBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? CodigoSku { get; private set; }
    public TipoProduto Tipo { get; private set; }
    public decimal PrecoCompra { get; private set; }
    public decimal PrecoVenda { get; private set; }
    public string? Marca { get; private set; }
    public string? Modelo { get; private set; }

    // Navegação
    public Estoque? Estoque { get; private set; }
    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes =>
        _movimentacoes.AsReadOnly();
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    protected Produto() { }

    public Produto(
        string nome,
        TipoProduto tipo,
        decimal precoCompra,
        decimal precoVenda,
        string? descricao = null,
        string? codigoSku = null,
        string? marca = null,
        string? modelo = null)
    {
        ValidarPrecos(precoCompra, precoVenda);
        Nome = nome;
        Tipo = tipo;
        PrecoCompra = precoCompra;
        PrecoVenda = precoVenda;
        Descricao = descricao;
        CodigoSku = codigoSku;
        Marca = marca;
        Modelo = modelo;
    }

    public void AtualizarPrecos(decimal novoPrecoCompra, decimal novoPrecoVenda)
    {
        ValidarPrecos(novoPrecoCompra, novoPrecoVenda);
        PrecoCompra = novoPrecoCompra;
        PrecoVenda = novoPrecoVenda;
        MarcarAtualizado();
    }

    private static void ValidarPrecos(decimal compra, decimal venda)
    {
        if (compra < 0) throw new ArgumentException("Preço de compra não pode ser negativo.");
        if (venda < 0) throw new ArgumentException("Preço de venda não pode ser negativo.");
    }
}