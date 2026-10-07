// Ibatech.Services/Mappers/ProdutoMapper.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using ProdutoServiceDto = Ibatech.Services.DTOs.Produto.ProdutoResponseDto;

namespace Ibatech.Services.Mappers;

public static class ProdutoMapper
{
    public static ProdutoServiceDto ToDto(this Produto p) => new(
        p.Id,
        p.Nome,
        p.Descricao,
        p.CodigoSku,
        p.CodigoFornecedor,
        p.CodigoBarras,
        p.Ncm,
        p.UnidadeComercial,
        p.Tipo,
        p.Tipo.ToLabel(),
        p.PrecoCompra,
        p.PrecoVenda,
        p.Marca,
        p.Modelo,
        p.Estoque?.QuantidadeAtual ?? 0,
        p.Estoque?.QuantidadeMinima ?? 0,
        p.Estoque?.EstaBaixoDoMinimo ?? false,
        p.Ativo
    );

    public static IEnumerable<ProdutoServiceDto> ToDtoList(
        this IEnumerable<Produto> lista) =>
        lista.Select(p => p.ToDto());

    public static ProdutoResponseDto ToDomainDto(this Produto p) =>
        new()
        {
            Id = p.Id,

            Nome = p.Nome,
            Descricao = p.Descricao,

            CodigoSku = p.CodigoSku,
            CodigoFornecedor = p.CodigoFornecedor,
            CodigoBarras = p.CodigoBarras,

            Ncm = p.Ncm,
            UnidadeComercial = p.UnidadeComercial,

            Tipo = p.Tipo.ToString(),
            TipoLabel = p.Tipo.ToLabel(),

            PrecoCompra = p.PrecoCompra,
            PrecoVenda = p.PrecoVenda,

            Marca = p.Marca,
            Modelo = p.Modelo,

            QuantidadeAtual =
                p.Estoque?.QuantidadeAtual ?? 0,

            QuantidadeMinima =
                p.Estoque?.QuantidadeMinima ?? 0,

            AlertaReposicao =
                p.Estoque?.EstaBaixoDoMinimo ?? false,

            Ativo = p.Ativo
        };

    public static IEnumerable<ProdutoResponseDto> ToDomainDtoList(
        this IEnumerable<Produto> lista) =>
        lista.Select(p => p.ToDomainDto());

    private static string ToLabel(this TipoProduto tipo) => tipo switch
    {
        TipoProduto.Computador =>
            "Computador",

        TipoProduto.Peca =>
            "Peça",

        TipoProduto.AcessorioMovel =>
            "Acessório de Celular",

        TipoProduto.Periferico =>
            "Periférico",

        _ => tipo.ToString()
    };
}
