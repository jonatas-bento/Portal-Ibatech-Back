using Ibatech.Domain.Enums;

namespace Ibatech.Domain.DTOs;

public sealed record CriarEntradaCompraDto(
    Guid FornecedorId,
    string NumeroDocumento,
    DateTime DataEntrada,
    decimal ValorFrete,
    decimal ValorDesconto,
    decimal OutrasDespesas,
    string? Observacao);

public sealed record AdicionarEntradaCompraItemDto(
    Guid ProdutoId,
    int Quantidade,
    decimal PrecoUnitarioCompra);

public sealed record EntradaCompraItemDto(
    Guid Id,
    Guid ProdutoId,
    string? CodigoSku,
    string? CodigoFornecedor,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitarioCompra,
    decimal ValorTotalProduto,
    decimal ValorFreteRateado,
    decimal ValorDescontoRateado,
    decimal ValorOutrasDespesasRateado,
    decimal CustoEfetivoUnitario);

public sealed record EntradaCompraResumoDto(
    Guid Id,
    Guid FornecedorId,
    string FornecedorNome,
    string NumeroDocumento,
    DateTime DataEntrada,
    decimal ValorProdutos,
    decimal ValorFrete,
    decimal ValorDesconto,
    decimal OutrasDespesas,
    decimal ValorTotal,
    StatusEntradaCompra Status,
    DateTime? DataConfirmacao);

public sealed record EntradaCompraDetalheDto(
    Guid Id,
    Guid FornecedorId,
    string FornecedorNome,
    string NumeroDocumento,
    DateTime DataEntrada,
    decimal ValorProdutos,
    decimal ValorFrete,
    decimal ValorDesconto,
    decimal OutrasDespesas,
    decimal ValorTotal,
    string? Observacao,
    Guid UsuarioId,
    StatusEntradaCompra Status,
    DateTime? DataConfirmacao,
    IReadOnlyCollection<EntradaCompraItemDto> Itens);
