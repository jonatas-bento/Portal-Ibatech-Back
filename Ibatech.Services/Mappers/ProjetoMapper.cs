// Ibatech.Services/Mappers/ProjetoMapper.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using ProjetoServiceDto = Ibatech.Services.DTOs.Projeto.ProjetoResponseDto;

namespace Ibatech.Services.Mappers;

public static class ProjetoMapper
{
    public static ProjetoServiceDto ToDto(this ProjetoRequisitos p) => new(
        p.Id,
        p.NomeEmpresa,
        p.NomeContato,
        p.EmailContato,
        p.DescricaoDores,
        p.InfraAtual,
        p.UtilizaNuvem,
        p.ProvedorNuvem,
        p.VolumetriaUsuarios,
        p.ObservacoesExtra,
        p.NotaAnalista,
        p.Status,
        p.Status.ToLabel(),
        p.DataProposta,
        p.DataAprovacao,
        p.CriadoEm,
        p.Usuario?.NomeCompleto
    );

    public static IEnumerable<ProjetoServiceDto> ToDtoList(
        this IEnumerable<ProjetoRequisitos> lista) =>
        lista.Select(p => p.ToDto());

    // Mapeamento para DTOs do Domain
    public static ProjetoResponseDto ToDomainDto(this ProjetoRequisitos p) =>
        new()
        {
            Id = p.Id,
            NomeEmpresa = p.NomeEmpresa,
            NomeContato = p.NomeContato,
            EmailContato = p.EmailContato,
            Status = p.Status.ToLabel(),
            CriadoEm = p.CriadoEm
        };

    public static IEnumerable<ProjetoResponseDto> ToDomainDtoList(
        this IEnumerable<ProjetoRequisitos> lista) =>
        lista.Select(p => p.ToDomainDto());

    private static string ToLabel(this StatusProjeto status) => status switch
    {
        StatusProjeto.Recebido => "Recebido",
        StatusProjeto.EmAnalise => "Em Análise",
        StatusProjeto.PropostaEnviada => "Proposta Enviada",
        StatusProjeto.Aprovado => "Aprovado",
        StatusProjeto.Cancelado => "Cancelado",
        _ => status.ToString()
    };
}
