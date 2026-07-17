namespace Ibatech.Domain.DTOs;

public sealed class TransacaoFinanceiraResumoDto
{
    public Guid Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal Valor { get; init; }
    public Ibatech.Domain.Enums.TipoTransacao Tipo { get; init; }
    public string Categoria { get; init; } = string.Empty;
    public DateTime DataVencimento { get; init; }
    public DateTime? DataPagamento { get; init; }
    public bool Liquidada { get; init; }
    public Guid? VendaId { get; init; }
    public string? NumeroVenda { get; init; }
    public Ibatech.Domain.Enums.FormaPagamento? FormaPagamento { get; init; }
    public Guid UsuarioId { get; init; }
    public string UsuarioNome { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
}
