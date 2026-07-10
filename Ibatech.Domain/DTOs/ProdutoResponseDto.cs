namespace Ibatech.Domain.DTOs
{
    public class ProdutoResponseDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? CodigoSku { get; set; }
        public string Tipo { get; set; } = string.Empty;       // 'Computador', 'Peca', etc.
        public string TipoLabel { get; set; } = string.Empty;  // 'Computador', 'Peça', etc.
        public decimal PrecoCompra { get; set; }
        public decimal PrecoVenda { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int QuantidadeAtual { get; set; }               // Ajustado para casar com o front
        public int QuantidadeMinima { get; set; }
        public bool AlertaReposicao { get; set; }
        public bool Ativo { get; set; }
    }
}