using Ibatech.Domain.Enums;

namespace Ibatech.Domain.DTOs;

public record FinalizarVendaDto(
    FormaPagamento FormaPagamento,
    decimal? ValorRecebido
);
