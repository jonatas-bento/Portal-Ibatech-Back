using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Produto;

public sealed record ProdutoResponseDto(
    Guid Id,
    string Nome,
    string? Descricao,
    string? CodigoSku,
    TipoProduto Tipo,
    string TipoLabel,
    decimal PrecoCompra,
    decimal PrecoVenda,
    string? Marca,
    string? Modelo,
    int QuantidadeAtual,
    int QuantidadeMinima,
    bool AlertaReposicao,
    bool Ativo
);