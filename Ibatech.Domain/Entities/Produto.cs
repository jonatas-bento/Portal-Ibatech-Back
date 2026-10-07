// Ibatech.Domain/Entities/Produto.cs
using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class Produto : EntityBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    // Identificação do produto
    public string? CodigoSku { get; private set; }
    public string? CodigoFornecedor { get; private set; }
    public string? CodigoBarras { get; private set; }

    // Informações comerciais/fiscais básicas
    public TipoProduto Tipo { get; private set; }
    public decimal PrecoCompra { get; private set; }
    public decimal PrecoVenda { get; private set; }
    public string? Marca { get; private set; }
    public string? Modelo { get; private set; }
    public string? Ncm { get; private set; }
    public string UnidadeComercial { get; private set; } = "UN";

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
        string? modelo = null,
        string? codigoFornecedor = null,
        string? codigoBarras = null,
        string? ncm = null,
        string? unidadeComercial = "UN")
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));

        ValidarPrecos(precoCompra, precoVenda);

        Nome = nome.Trim();
        Tipo = tipo;
        PrecoCompra = precoCompra;
        PrecoVenda = precoVenda;
        Descricao = Normalizar(descricao);
        CodigoSku = Normalizar(codigoSku);
        Marca = Normalizar(marca);
        Modelo = Normalizar(modelo);
        CodigoFornecedor = Normalizar(codigoFornecedor);
        CodigoBarras = Normalizar(codigoBarras);
        Ncm = Normalizar(ncm);
        UnidadeComercial = Normalizar(unidadeComercial) ?? "UN";
    }

    public void AtualizarPrecos(
        decimal novoPrecoCompra,
        decimal novoPrecoVenda)
    {
        ValidarPrecos(novoPrecoCompra, novoPrecoVenda);

        PrecoCompra = novoPrecoCompra;
        PrecoVenda = novoPrecoVenda;

        MarcarAtualizado();
    }

    private static void ValidarPrecos(
        decimal compra,
        decimal venda)
    {
        if (compra < 0)
            throw new ArgumentException(
                "Preço de compra não pode ser negativo.");

        if (venda < 0)
            throw new ArgumentException(
                "Preço de venda não pode ser negativo.");
    }

    private static string? Normalizar(string? valor)
    {
        var resultado = valor?.Trim();
        return string.IsNullOrWhiteSpace(resultado)
            ? null
            : resultado;
    }
}
