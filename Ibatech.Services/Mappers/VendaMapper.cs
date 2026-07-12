using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;

namespace Ibatech.Services.Mappers;

public static class VendaMapper
{
    public static VendaResumoDto ToResumoDto(Venda venda)
    {
        return new VendaResumoDto(
            venda.Id,
            venda.Numero,
            venda.ClienteId,
            venda.ClienteNomeSnapshot,
            venda.VendedorId,
            venda.VendedorNomeSnapshot,
            venda.Status,
            venda.DataVenda,
            venda.ValorBruto,
            venda.Desconto,
            venda.ValorTotal,
            venda.Itens.Count);
    }

    public static VendaDetalheDto ToDetalheDto(Venda venda)
    {
        var itens = venda.Itens
            .OrderBy(i => i.CriadoEm)
            .ThenBy(i => i.NomeProduto)
            .Select(ToItemDto)
            .ToList();

        return new VendaDetalheDto(
            venda.Id,
            venda.Numero,
            venda.ClienteId,
            venda.ClienteNomeSnapshot,
            venda.ClienteCpfCnpjSnapshot,
            venda.VendedorId,
            venda.VendedorNomeSnapshot,
            venda.Status,
            venda.DataVenda,
            venda.ValorBruto,
            venda.Desconto,
            venda.ValorTotal,
            venda.Observacao,
            venda.CriadoEm,
            venda.AtualizadoEm,
            itens);
    }

    public static VendaItemDto ToItemDto(VendaItem item)
    {
        return new VendaItemDto(
            item.Id,
            item.ProdutoId,
            item.CodigoSku,
            item.NomeProduto,
            item.DescricaoProduto,
            item.Quantidade,
            item.PrecoUnitario,
            item.Quantidade * item.PrecoUnitario,
            item.Desconto,
            item.ValorTotal);
    }
}
