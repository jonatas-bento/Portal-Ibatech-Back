using Ibatech.Domain.Enums;

namespace Ibatech.Domain.DTOs;

public record VendaResumoDto(
    Guid Id,
    string Numero,
    Guid? ClienteId,
    string? ClienteNome,
    Guid VendedorId,
    string VendedorNome,
    StatusVenda Status,
    DateTime DataVenda,
    decimal ValorBruto,
    decimal Desconto,
    decimal ValorTotal,
    int QuantidadeItens);

public record VendaDetalheDto(
    Guid Id,
    string Numero,
    Guid? ClienteId,
    string? ClienteNome,
    string? ClienteCpfCnpj,
    Guid VendedorId,
    string VendedorNome,
    StatusVenda Status,
    DateTime DataVenda,
    decimal ValorBruto,
    decimal Desconto,
    decimal ValorTotal,
    string? Observacao,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    DateTime? DataFinalizacao,
    FormaPagamento? FormaPagamento,
    decimal? ValorRecebido,
    decimal? Troco,
    DateTime? DataCancelamento,
    Guid? CanceladaPorUsuarioId,
    string? MotivoCancelamento,
    DateTime? DataEstorno,
    Guid? EstornadaPorUsuarioId,
    string? MotivoEstorno,
    IReadOnlyCollection<VendaItemDto> Itens);


public record VendaItemDto(
    Guid Id,
    Guid ProdutoId,
    string? CodigoSku,
    string NomeProduto,
    string? DescricaoProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal ValorBruto,
    decimal Desconto,
    decimal ValorTotal);

public record CriarVendaDto(
    Guid? ClienteId,
    string? Observacao);

public record AtualizarVendaDto(
    Guid? ClienteId,
    string? Observacao);

public record AdicionarVendaItemDto(
    Guid ProdutoId,
    int Quantidade,
    decimal Desconto = 0);

public record AtualizarVendaItemDto(
    int Quantidade,
    decimal Desconto);

public record CancelarVendaDto(string Motivo);

public record EstornarVendaDto(string Motivo);


public record VendaFiltroDto(
    string? Numero,
    Guid? ClienteId,
    Guid? VendedorId,
    StatusVenda? Status,
    DateTime? DataInicial,
    DateTime? DataFinal);
