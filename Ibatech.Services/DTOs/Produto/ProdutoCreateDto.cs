using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Produto;

public sealed record ProdutoCreateDto(
    string Nome,
    TipoProduto Tipo,
    decimal PrecoCompra,
    decimal PrecoVenda,
    int QuantidadeInicial,
    int QuantidadeMinima = 5,
    string? Descricao = null,
    string? CodigoSku = null,
    string? Marca = null,
    string? Modelo = null
);