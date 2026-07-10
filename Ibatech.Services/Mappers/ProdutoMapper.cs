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

    // Mapeamento para DTOs do Domain
    // Alinhando o DTO de Domínio com o ProdutoResponse do Front-end
    public static ProdutoResponseDto ToDomainDto(this Produto p) =>
        new()
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            CodigoSku = p.CodigoSku,
            Tipo = p.Tipo.ToString(), // Entrega a string exata do Enum ('Computador', 'Peca')
            TipoLabel = p.Tipo.ToLabel(), // Entrega o texto formatado ('Peça', 'Acessório de Celular')
            PrecoCompra = p.PrecoCompra,
            PrecoVenda = p.PrecoVenda,
            Marca = p.Marca,
            Modelo = p.Modelo,
            QuantidadeAtual = p.Estoque?.QuantidadeAtual ?? 0, // Casou com o front
            QuantidadeMinima = p.Estoque?.QuantidadeMinima ?? 0,
            AlertaReposicao = p.Estoque?.EstaBaixoDoMinimo ?? false,
            Ativo = true // Como sua entidade herda de EntityBase, use a flag de ativo se houver, ou true por padrão
        };

    public static IEnumerable<ProdutoResponseDto> ToDomainDtoList(
        this IEnumerable<Produto> lista) =>
        lista.Select(p => p.ToDomainDto());

    private static string ToLabel(this TipoProduto tipo) => tipo switch
    {
        TipoProduto.Computador => "Computador",
        TipoProduto.Peca => "Peça",
        TipoProduto.AcessorioMovel => "Acessório de Celular",
        TipoProduto.Periferico => "Periférico",
        _ => tipo.ToString()
    };
}
