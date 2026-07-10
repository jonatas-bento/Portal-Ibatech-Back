namespace Ibatech.Domain.DTOs;

public sealed class ProdutoImportacaoErroDto
{
    public int Linha { get; init; }
    public string Campo { get; init; } = string.Empty;
    public string? Valor { get; init; }
    public string Mensagem { get; init; } = string.Empty;
}
