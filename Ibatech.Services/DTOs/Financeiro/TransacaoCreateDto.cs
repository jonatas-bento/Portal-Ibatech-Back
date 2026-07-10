using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Financeiro;

public sealed record TransacaoCreateDto(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    DateTime DataVencimento,
    string? Categoria = null,
    Guid? UsuarioId = null
);