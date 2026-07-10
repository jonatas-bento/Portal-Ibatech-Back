namespace Ibatech.Domain.DTOs
{
    public class RegistrarMovimentacaoRequestDto
    {
        public string Tipo { get; set; } = string.Empty; // "Entrada" ou "Saida"
        public int Quantidade { get; set; }
        public string? Motivo { get; set; }
    }
}