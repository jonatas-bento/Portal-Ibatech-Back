using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Projeto;

public sealed record AtualizarStatusDto(
    StatusProjeto NovoStatus,
    string? NotaAnalista = null
);