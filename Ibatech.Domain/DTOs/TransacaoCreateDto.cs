namespace Ibatech.Domain.DTOs;

public class TransacaoCreateDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public string? Categoria { get; set; }
    public Guid? UsuarioId { get; set; }
}
