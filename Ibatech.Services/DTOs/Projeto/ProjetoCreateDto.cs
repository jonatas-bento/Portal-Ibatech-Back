namespace Ibatech.Services.DTOs.Projeto;

public sealed record ProjetoCreateDto(
    string NomeEmpresa,
    string NomeContato,
    string EmailContato,
    string DescricaoDores,
    string InfraAtual,
    bool UtilizaNuvem,
    int VolumetriaUsuarios,
    string? ProvedorNuvem = null,
    string? ObservacoesExtra = null
);