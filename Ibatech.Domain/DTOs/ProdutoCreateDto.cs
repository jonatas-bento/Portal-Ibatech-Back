namespace Ibatech.Domain.DTOs;

public class ProdutoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }
    public int QuantidadeInicial { get; set; }
    public int QuantidadeMinima { get; set; } = 5;
    public string? Descricao { get; set; }
    public string? CodigoSku { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
}
