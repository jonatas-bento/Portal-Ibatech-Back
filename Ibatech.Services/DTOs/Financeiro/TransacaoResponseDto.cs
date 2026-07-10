using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Financeiro;

public sealed record TransacaoResponseDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    string TipoLabel,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    bool Liquidada,
    string? Categoria,
    DateTime CriadoEm
);
