using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;

namespace Ibatech.Services.Mappers;

public static class EntradaCompraMapper
{
    public static EntradaCompraResumoDto ToResumoDto(
        EntradaCompra entrada) =>
        new(
            entrada.Id,
            entrada.FornecedorId,
            entrada.Fornecedor?.Nome ?? string.Empty,
            entrada.NumeroDocumento,
            entrada.DataEntrada,
            entrada.ValorProdutos,
            entrada.ValorFrete,
            entrada.ValorDesconto,
            entrada.OutrasDespesas,
            entrada.ValorTotal,
            entrada.Status,
            entrada.DataConfirmacao);

    public static EntradaCompraDetalheDto ToDetalheDto(
        EntradaCompra entrada) =>
        new(
            entrada.Id,
            entrada.FornecedorId,
            entrada.Fornecedor?.Nome ?? string.Empty,
            entrada.NumeroDocumento,
            entrada.DataEntrada,
            entrada.ValorProdutos,
            entrada.ValorFrete,
            entrada.ValorDesconto,
            entrada.OutrasDespesas,
            entrada.ValorTotal,
            entrada.Observacao,
            entrada.UsuarioId,
            entrada.Status,
            entrada.DataConfirmacao,
            entrada.Itens
                .Select(ToItemDto)
                .ToList());

    private static EntradaCompraItemDto ToItemDto(
        EntradaCompraItem item) =>
        new(
            item.Id,
            item.ProdutoId,
            item.CodigoSku,
            item.CodigoFornecedor,
            item.NomeProduto,
            item.Quantidade,
            item.PrecoUnitarioCompra,
            item.ValorTotalProduto,
            item.ValorFreteRateado,
            item.ValorDescontoRateado,
            item.ValorOutrasDespesasRateado,
            item.CustoEfetivoUnitario);
}
