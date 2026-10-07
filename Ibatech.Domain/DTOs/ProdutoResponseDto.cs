namespace Ibatech.Domain.DTOs;

public class ProdutoResponseDto
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    // Identificação
    public string? CodigoSku { get; set; }
    public string? CodigoFornecedor { get; set; }
    public string? CodigoBarras { get; set; }

    // Informações comerciais/fiscais básicas
    public string? Ncm { get; set; }
    public string UnidadeComercial { get; set; } = "UN";

    public string Tipo { get; set; } = string.Empty;
    public string TipoLabel { get; set; } = string.Empty;

    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }

    public string? Marca { get; set; }
    public string? Modelo { get; set; }

    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }

    public bool AlertaReposicao { get; set; }
    public bool Ativo { get; set; }
}
