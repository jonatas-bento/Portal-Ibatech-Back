using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public sealed class EntradaCompra : EntityBase
{
    private readonly List<EntradaCompraItem> _itens = [];

    public Guid FornecedorId { get; private set; }
    public string NumeroDocumento { get; private set; } = null!;
    public DateTime DataEntrada { get; private set; }

    public decimal ValorProdutos { get; private set; }
    public decimal ValorFrete { get; private set; }
    public decimal ValorDesconto { get; private set; }
    public decimal OutrasDespesas { get; private set; }
    public decimal ValorTotal { get; private set; }

    public string? Observacao { get; private set; }

    public Guid UsuarioId { get; private set; }
    public StatusEntradaCompra Status { get; private set; }
    public DateTime? DataConfirmacao { get; private set; }

    public Fornecedor? Fornecedor { get; private set; }

    public IReadOnlyCollection<EntradaCompraItem> Itens =>
        _itens.AsReadOnly();

    private EntradaCompra() { }

    public EntradaCompra(
        Guid fornecedorId,
        string numeroDocumento,
        DateTime dataEntrada,
        decimal valorFrete,
        decimal valorDesconto,
        decimal outrasDespesas,
        Guid usuarioId,
        string? observacao = null)
    {
        if (fornecedorId == Guid.Empty)
            throw new ArgumentException("FornecedorId é obrigatório.");

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException("Número do documento é obrigatório.");

        if (dataEntrada == default)
            throw new ArgumentException("Data de entrada inválida.");

        if (valorFrete < 0)
            throw new ArgumentException("Valor de frete não pode ser negativo.");

        if (valorDesconto < 0)
            throw new ArgumentException("Valor de desconto não pode ser negativo.");

        if (outrasDespesas < 0)
            throw new ArgumentException("Outras despesas não podem ser negativas.");

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("UsuarioId é obrigatório.");

        FornecedorId = fornecedorId;
        NumeroDocumento = numeroDocumento.Trim();
        DataEntrada = dataEntrada;

        ValorFrete = valorFrete;
        ValorDesconto = valorDesconto;
        OutrasDespesas = outrasDespesas;

        UsuarioId = usuarioId;
        Observacao = Normalizar(observacao);

        Status = StatusEntradaCompra.Rascunho;

        RecalcularTotaisERateios();
    }

    public EntradaCompraItem AdicionarItem(
        Guid produtoId,
        string? codigoSku,
        string? codigoFornecedor,
        string nomeProduto,
        int quantidade,
        decimal precoUnitarioCompra)
    {
        GarantirRascunho();

        if (_itens.Any(i => i.ProdutoId == produtoId))
            throw new InvalidOperationException(
                "Produto já adicionado à entrada.");

        var item = new EntradaCompraItem(
            Id,
            produtoId,
            codigoSku,
            codigoFornecedor,
            nomeProduto,
            quantidade,
            precoUnitarioCompra);

        _itens.Add(item);

        RecalcularTotaisERateios();
        MarcarAtualizado();

        return item;
    }

    public void ValidarConfirmacao(DateTime dataConfirmacaoUtc)
    {
        ValidarConfirmacaoInterno(dataConfirmacaoUtc);
    }

    public void Confirmar(DateTime dataConfirmacaoUtc)
    {
        ValidarConfirmacaoInterno(dataConfirmacaoUtc);

        RecalcularTotaisERateios();

        Status = StatusEntradaCompra.Confirmada;
        DataConfirmacao = dataConfirmacaoUtc;
        AtualizadoEm = dataConfirmacaoUtc;
    }

    private void ValidarConfirmacaoInterno(DateTime dataConfirmacaoUtc)
    {
        GarantirRascunho();

        if (_itens.Count == 0)
            throw new InvalidOperationException(
                "A entrada precisa possuir pelo menos um item.");

        if (dataConfirmacaoUtc == default)
            throw new ArgumentException(
                "Data de confirmação inválida.");

        if (ValorProdutos <= 0)
            throw new InvalidOperationException(
                "O valor dos produtos deve ser maior que zero.");

        if (ValorTotal < 0)
            throw new InvalidOperationException(
                "O valor total da entrada não pode ser negativo.");
    }

    private void RecalcularTotaisERateios()
    {
        ValorProdutos = Math.Round(
            _itens.Sum(i => i.ValorTotalProduto),
            2,
            MidpointRounding.AwayFromZero);

        ValorTotal = Math.Round(
            ValorProdutos
            + ValorFrete
            + OutrasDespesas
            - ValorDesconto,
            2,
            MidpointRounding.AwayFromZero);

        if (ValorTotal < 0)
            throw new InvalidOperationException(
                "O valor total da entrada não pode ser negativo.");

        if (_itens.Count == 0)
            return;

        if (ValorProdutos <= 0)
            throw new InvalidOperationException(
                "O valor dos produtos deve ser maior que zero para realizar o rateio.");

        decimal freteDistribuido = 0;
        decimal descontoDistribuido = 0;
        decimal despesasDistribuidas = 0;

        for (var i = 0; i < _itens.Count; i++)
        {
            var item = _itens[i];
            var ultimoItem = i == _itens.Count - 1;

            decimal freteItem;
            decimal descontoItem;
            decimal despesasItem;

            if (ultimoItem)
            {
                freteItem = ValorFrete - freteDistribuido;
                descontoItem = ValorDesconto - descontoDistribuido;
                despesasItem = OutrasDespesas - despesasDistribuidas;
            }
            else
            {
                var proporcao =
                    item.ValorTotalProduto / ValorProdutos;

                freteItem = Math.Round(
                    ValorFrete * proporcao,
                    2,
                    MidpointRounding.AwayFromZero);

                descontoItem = Math.Round(
                    ValorDesconto * proporcao,
                    2,
                    MidpointRounding.AwayFromZero);

                despesasItem = Math.Round(
                    OutrasDespesas * proporcao,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            item.AplicarRateio(
                freteItem,
                descontoItem,
                despesasItem);

            freteDistribuido += freteItem;
            descontoDistribuido += descontoItem;
            despesasDistribuidas += despesasItem;
        }
    }

    private void GarantirRascunho()
    {
        if (Status != StatusEntradaCompra.Rascunho)
            throw new InvalidOperationException(
                "Apenas entradas em rascunho podem ser alteradas.");
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
}
