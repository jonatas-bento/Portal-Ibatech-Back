namespace Ibatech.Domain.DTOs;

public sealed class ResumoFinanceiroDetalhadoDto
{
    public decimal TotalReceitas { get; init; }
    public decimal TotalDespesas { get; init; }
    public decimal TotalEstornos { get; init; }
    public decimal TotalLiquido { get; init; }
    public int QuantidadeVendas { get; init; }
    public int QuantidadeEstornos { get; init; }
    public decimal TicketMedio { get; init; }

    public IReadOnlyCollection<ResumoFormaPagamentoDto> FormasPagamento { get; init; } = [];
}
