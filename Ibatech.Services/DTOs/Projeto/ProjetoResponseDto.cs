using Ibatech.Domain.Enums;
namespace Ibatech.Services.DTOs.Projeto;

public sealed record ProjetoResponseDto(
    Guid Id,
    string NomeEmpresa,
    string NomeContato,
    string EmailContato,
    string DescricaoDores,
    string InfraAtual,
    bool UtilizaNuvem,
    string? ProvedorNuvem,
    int VolumetriaUsuarios,
    string? ObservacoesExtra,
    string? NotaAnalista,
    StatusProjeto Status,
    string StatusLabel,
    DateTime? DataProposta,
    DateTime? DataAprovacao,
    DateTime CriadoEm,
    string? NomeUsuario
);