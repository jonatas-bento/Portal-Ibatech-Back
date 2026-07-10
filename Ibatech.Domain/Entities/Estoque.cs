using Ibatech.Domain.Entities.Base;

namespace Ibatech.Domain.Entities;

public class Estoque : EntityBase
{
    public Guid ProdutoId { get; private set; }
    public int QuantidadeAtual { get; private set; }
    public int QuantidadeMinima { get; private set; }
    public Produto? Produto { get; private set; }

    public bool EstaBaixoDoMinimo => QuantidadeAtual <= QuantidadeMinima;

    protected Estoque() { }

    public Estoque(Guid produtoId, int quantidadeInicial, int quantidadeMinima = 5)
    {
        if (quantidadeInicial < 0)
            throw new ArgumentException("Quantidade inicial não pode ser negativa.");
        ProdutoId = produtoId;
        QuantidadeAtual = quantidadeInicial;
        QuantidadeMinima = quantidadeMinima;
    }

    public void Entrada(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade de entrada deve ser positiva.");
        QuantidadeAtual += quantidade;
        MarcarAtualizado();
    }

    public void Saida(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade de saída deve ser positiva.");
        if (quantidade > QuantidadeAtual) throw new InvalidOperationException("Estoque insuficiente.");
        QuantidadeAtual -= quantidade;
        MarcarAtualizado();
    }

    public void AjustarMinimo(int novoMinimo)
    {
        if (novoMinimo < 0) throw new ArgumentException("Mínimo não pode ser negativo.");
        QuantidadeMinima = novoMinimo;
        MarcarAtualizado();
    }
}