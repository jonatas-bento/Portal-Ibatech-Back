namespace Ibatech.Domain.DTOs;

public class TransacaoResponseDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Liquidada { get; set; }
}
