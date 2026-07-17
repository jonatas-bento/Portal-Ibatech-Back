namespace Ibatech.Domain.DTOs;

public sealed class ResumoFormaPagamentoDto
{
    public Ibatech.Domain.Enums.FormaPagamento FormaPagamento { get; init; }
    public decimal TotalReceitas { get; init; }
    public decimal TotalEstornos { get; init; }
    public decimal TotalLiquido { get; init; }
    public int QuantidadeVendas { get; init; }
    public int QuantidadeEstornos { get; init; }
}
