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

    public DateTime? DataCancelamento { get; private set; }
    public Guid? CanceladaPorUsuarioId { get; private set; }
    public string? MotivoCancelamento { get; private set; }

    public DateTime? DataEstorno { get; private set; }
    public Guid? EstornadaPorUsuarioId { get; private set; }
    public string? MotivoEstorno { get; private set; }

    public IReadOnlyCollection<TransacaoFinanceira> TransacoesFinanceiras => _transacoesFinanceiras.AsReadOnly();
    public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();
    public IReadOnlyCollection<MovimentacaoEstoque> MovimentacoesEstoque => _movimentacoesEstoque.AsReadOnly();

    private readonly List<TransacaoFinanceira> _transacoesFinanceiras = [];
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

    /// <summary>
    /// Cancela uma venda em rascunho. Não produz nenhum efeito colateral em
    /// estoque ou financeiro: apenas marca o status e registra a auditoria
    /// de quem, quando e por que motivo a venda foi cancelada. Itens e
    /// totais permanecem intactos para fins de histórico.
    /// </summary>
    public void Cancelar(
        string motivo,
        Guid usuarioId,
        DateTime dataCancelamentoUtc)
    {
		if (Status == StatusVenda.Cancelada)
			throw new InvalidOperationException(
				"A venda já foi cancelada.");

		if (Status != StatusVenda.Rascunho)
            throw new InvalidOperationException("Apenas vendas em rascunho podem ser canceladas.");

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário responsável pelo cancelamento inválido.", nameof(usuarioId));

        if (dataCancelamentoUtc == default)
            throw new ArgumentException("Data de cancelamento inválida.", nameof(dataCancelamentoUtc));

        var motivoNormalizado = ValidarMotivo(motivo, nameof(motivo));

        Status = StatusVenda.Cancelada;
        MotivoCancelamento = motivoNormalizado;
        CanceladaPorUsuarioId = usuarioId;
        DataCancelamento = dataCancelamentoUtc;
        AtualizadoEm = dataCancelamentoUtc;
    }

    /// <summary>
    /// Estorna uma venda concluída. Apenas marca o status e registra a
    /// auditoria de quem, quando e por que motivo a venda foi estornada.
    /// Não executa devolução de estoque nem cria transação financeira
    /// compensatória: esses efeitos colaterais são de responsabilidade do
    /// serviço de aplicação, que deve orquestrá-los na mesma transação
    /// atômica em que este método é chamado. Todos os campos de finalização
    /// (FormaPagamento, ValorRecebido, Troco, DataFinalizacao) e os itens
    /// da venda são preservados integralmente.
    /// </summary>
    /// <summary>
    /// Valida se a venda pode ser estornada com os dados informados, sem
    /// alterar nenhum estado da entidade. Deve ser chamado antes de qualquer
    /// efeito colateral externo (ex.: devolução de estoque) para permitir
    /// falha antecipada sem necessidade de desfazer alterações em memória.
    /// </summary>
    public void ValidarEstorno(
        string motivo,
        Guid usuarioId,
        DateTime dataEstornoUtc)
    {
        ValidarEstornoInterno(motivo, usuarioId, dataEstornoUtc);
    }

    public void Estornar(
        string motivo,
        Guid usuarioId,
        DateTime dataEstornoUtc)
    {
        var motivoNormalizado = ValidarEstornoInterno(motivo, usuarioId, dataEstornoUtc);

        Status = StatusVenda.Estornada;
        MotivoEstorno = motivoNormalizado;
        EstornadaPorUsuarioId = usuarioId;
        DataEstorno = dataEstornoUtc;
        AtualizadoEm = dataEstornoUtc;
    }

    /// <summary>
    /// Regra única de validação do estorno, compartilhada por
    /// <see cref="ValidarEstorno"/> e <see cref="Estornar"/> para evitar
    /// duplicação. Não produz efeitos colaterais: apenas valida e retorna o
    /// motivo normalizado.
    /// </summary>
    private string ValidarEstornoInterno(
        string motivo,
        Guid usuarioId,
        DateTime dataEstornoUtc)
    {
        if (Status != StatusVenda.Concluida)
            throw new InvalidOperationException("Apenas vendas concluídas podem ser estornadas.");

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário responsável pelo estorno inválido.", nameof(usuarioId));

        if (dataEstornoUtc == default)
            throw new ArgumentException("Data de estorno inválida.", nameof(dataEstornoUtc));

        return ValidarMotivo(motivo, nameof(motivo));
    }


    /// <summary>
    /// Valida e normaliza um motivo textual (cancelamento ou estorno):
    /// rejeita nulo/vazio/whitespace, aplica Trim e limita a 500 caracteres.
    /// Centraliza a regra para evitar duplicação entre Cancelar e Estornar.
    /// </summary>
    private static string ValidarMotivo(string motivo, string nomeParametro)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Motivo é obrigatório.", nomeParametro);

        var motivoTrimado = motivo.Trim();

        if (motivoTrimado.Length > 500)
            throw new ArgumentException("Motivo não pode exceder 500 caracteres.", nomeParametro);

        return motivoTrimado;
    }
}
