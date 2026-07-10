namespace Ibatech.Domain.DTOs;

public class ResumoFinanceiroDto
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
    public int TransacoesAbertas { get; set; }
}
