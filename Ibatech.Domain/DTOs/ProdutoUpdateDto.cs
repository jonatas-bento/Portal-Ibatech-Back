namespace Ibatech.Domain.DTOs;

public sealed class ProdutoUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;

    public decimal PrecoVenda { get; set; }

    public int QuantidadeMinima { get; set; } = 5;

    public string? Descricao { get; set; }

    public string? CodigoSku { get; set; }
    public string? CodigoFornecedor { get; set; }
    public string? CodigoBarras { get; set; }

    public string? Ncm { get; set; }
    public string UnidadeComercial { get; set; } = "UN";

    public string? Marca { get; set; }
    public string? Modelo { get; set; }
}
