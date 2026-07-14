using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;
using FormaPagamentoEnum = Ibatech.Domain.Enums.FormaPagamento;

namespace Ibatech.Domain.Entities;

public sealed class Venda : EntityBase
{
    private readonly List<VendaItem> _itens = [];

    public string Numero { get; private set; } = null!;
    public Guid? ClienteId { get; private set; }
    public string? ClienteNomeSnapshot { get; private set; }
    public string? ClienteCpfCnpjSnapshot { get; private set; }
    public Guid VendedorId { get; private set; }
    public string VendedorNomeSnapshot { get; private set; } = null!;
    public StatusVenda Status { get; private set; }
    public DateTime DataVenda { get; private set; }
    public decimal ValorBruto { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public FormaPagamento? FormaPagamento { get; private set; }
    public decimal? ValorRecebido { get; private set; }
    public decimal? Troco { get; private set; }

    public TransacaoFinanceira? TransacaoFinanceira { get; private set; }
    public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();
    public IReadOnlyCollection<MovimentacaoEstoque> MovimentacoesEstoque => _movimentacoesEstoque.AsReadOnly();

    private readonly List<MovimentacaoEstoque> _movimentacoesEstoque = [];

    private Venda() { }

    public Venda(
        string numero,
        Guid? clienteId,
        string? clienteNomeSnapshot,
        string? clienteCpfCnpjSnapshot,
        Guid vendedorId,
        string vendedorNomeSnapshot,
        string? observacao)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número é obrigatório.");
        if (vendedorId == Guid.Empty) throw new ArgumentException("VendedorId inválido.");
        if (string.IsNullOrWhiteSpace(vendedorNomeSnapshot)) throw new ArgumentException("VendedorNomeSnapshot é obrigatório.");

        Numero = numero;
        ClienteId = clienteId;
        ClienteNomeSnapshot = clienteNomeSnapshot;
        ClienteCpfCnpjSnapshot = clienteCpfCnpjSnapshot;
        VendedorId = vendedorId;
        VendedorNomeSnapshot = vendedorNomeSnapshot;
        Observacao = observacao?.Trim();
        Status = StatusVenda.Rascunho;
        DataVenda = DateTime.UtcNow;
        ValorBruto = 0;
        Desconto = 0;
        ValorTotal = 0;
    }

    public VendaItem AdicionarItem(
        Guid produtoId,
        string? codigoSku,
        string nomeProduto,
        string? descricaoProduto,
        int quantidade,
        decimal precoUnitario,
        decimal desconto)
    {
        GarantirRascunho();
        if (_itens.Any(i => i.ProdutoId == produtoId)) throw new InvalidOperationException("Produto já adicionado.");

        var item = new VendaItem(Id, produtoId, codigoSku, nomeProduto, descricaoProduto, quantidade, precoUnitario, desconto);
        _itens.Add(item);
        RecalcularTotais();
        MarcarAtualizado();
        return item;
    }

    public void AtualizarItem(Guid itemId, int quantidade, decimal desconto)
    {
        GarantirRascunho();
        
        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new KeyNotFoundException("Item não encontrado.");

        item.Atualizar(quantidade, desconto);
        RecalcularTotais();
        MarcarAtualizado();
    }

    public void RemoverItem(Guid itemId)
    {
        GarantirRascunho();

        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new KeyNotFoundException("Item não encontrado.");

        _itens.Remove(item);
        RecalcularTotais();
        MarcarAtualizado();
    }

    public void AtualizarCabecalho(
        Guid? clienteId,
        string? clienteNomeSnapshot,
        string? clienteCpfCnpjSnapshot,
        string? observacao)
    {
        GarantirRascunho();

        ClienteId = clienteId;
        ClienteNomeSnapshot = clienteNomeSnapshot;
        ClienteCpfCnpjSnapshot = clienteCpfCnpjSnapshot;
        Observacao = observacao?.Trim();
        MarcarAtualizado();
    }

    private void RecalcularTotais()
    {
        ValorBruto = _itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        Desconto = _itens.Sum(i => i.Desconto);
        ValorTotal = _itens.Sum(i => i.ValorTotal);
    }

    /// <summary>
    /// Valida se a venda pode ser concluída com os dados de pagamento informados,
    /// sem alterar nenhum estado da entidade. Deve ser chamado antes de qualquer
    /// efeito colateral externo (ex.: baixa de estoque) para permitir falha
    /// antecipada sem necessidade de desfazer alterações em memória.
    /// </summary>
    public void ValidarConclusao(
        FormaPagamentoEnum formaPagamento,
        decimal? valorRecebido,
        DateTime dataFinalizacaoUtc)
    {
        ValidarECalcularConclusao(formaPagamento, valorRecebido, dataFinalizacaoUtc);
    }

    public void Concluir(
        FormaPagamentoEnum formaPagamento,
        decimal? valorRecebido,
        DateTime dataFinalizacaoUtc)
    {
        var (valorEfetivamenteRecebido, troco) = ValidarECalcularConclusao(
            formaPagamento,
            valorRecebido,
            dataFinalizacaoUtc);

        Status = StatusVenda.Concluida;
        FormaPagamento = formaPagamento;
        ValorRecebido = valorEfetivamenteRecebido;
        Troco = troco;
        DataFinalizacao = dataFinalizacaoUtc;
        AtualizadoEm = dataFinalizacaoUtc;
    }

    /// <summary>
    /// Regra única de validação e cálculo dos valores de pagamento da conclusão
    /// da venda. Não produz efeitos colaterais: apenas valida e retorna os
    /// valores calculados de ValorRecebido/Troco. Compartilhada por
    /// <see cref="ValidarConclusao"/> e <see cref="Concluir"/> para evitar
    /// duplicação de regras.
    /// </summary>
    private (decimal ValorRecebido, decimal Troco) ValidarECalcularConclusao(
        FormaPagamentoEnum formaPagamento,
        decimal? valorRecebido,
        DateTime dataFinalizacaoUtc)
    {
        if (Status != StatusVenda.Rascunho)
            throw new InvalidOperationException("Apenas vendas em rascunho podem ser concluídas.");

        if (_itens.Count == 0)
        {
            throw new InvalidOperationException(
                "A venda precisa possuir pelo menos um item.");
        }

        if (ValorTotal <= 0)
        {
            throw new InvalidOperationException(
                "O valor total da venda deve ser maior que zero.");
        }

        if (dataFinalizacaoUtc == default)
        {
            throw new ArgumentException(
                "Data de finalização inválida.",
                nameof(dataFinalizacaoUtc));
        }

        if (!Enum.IsDefined(typeof(FormaPagamentoEnum), formaPagamento))
        {
            throw new ArgumentException(
                "Forma de pagamento inválida.",
                nameof(formaPagamento));
        }

        if (formaPagamento == FormaPagamentoEnum.Dinheiro)
        {
            if (!valorRecebido.HasValue)
            {
                throw new ArgumentException(
                    "O valor recebido é obrigatório para pagamento em dinheiro.",
                    nameof(valorRecebido));
            }

            if (valorRecebido.Value < ValorTotal)
            {
                throw new InvalidOperationException(
                    "O valor recebido é insuficiente.");
            }

            return (valorRecebido.Value, valorRecebido.Value - ValorTotal);
        }

        if (valorRecebido.HasValue)
        {
            throw new ArgumentException(
                "Valor recebido deve ser informado apenas para pagamento em dinheiro.",
                nameof(valorRecebido));
        }

        return (ValorTotal, 0);
    }

    private void GarantirRascunho()
    {
        if (Status != StatusVenda.Rascunho)
            throw new InvalidOperationException("Apenas vendas em rascunho podem ser alteradas.");
    }
}
