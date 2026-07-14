// Ibatech.Domain/Entities/TransacaoFinanceira.cs
using Ibatech.Domain.Entities.Base;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Entities;

public class TransacaoFinanceira : EntityBase
{
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public bool Liquidada { get; private set; }
    public string? Categoria { get; private set; }
    public Guid? UsuarioId { get; private set; }  // responsável/funcionário
    public Usuario? Usuario { get; private set; }
    public Guid? VendaId { get; private set; }
    public Venda? Venda { get; private set; }

    protected TransacaoFinanceira() { }

    public TransacaoFinanceira(
        string descricao,
        decimal valor,
        TipoTransacao tipo,
        DateTime dataVencimento,
        string? categoria = null,
        Guid? usuarioId = null,
        Guid? vendaId = null)
    {
        if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");
        if (vendaId.HasValue && vendaId == Guid.Empty)
            throw new ArgumentException("VendaId inválido.", nameof(vendaId));

        Descricao = descricao;
        Valor = valor;
        Tipo = tipo;
        DataVencimento = dataVencimento;
        Categoria = categoria;
        UsuarioId = usuarioId;
        VendaId = vendaId;
    }

    public void Liquidar(DateTime dataPagamento)
    {
        if (Liquidada) throw new InvalidOperationException("Transação já está liquidada.");
        DataPagamento = dataPagamento;
        Liquidada = true;
        MarcarAtualizado();
    }
}