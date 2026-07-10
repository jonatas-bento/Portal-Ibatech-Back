// src/Ibatech.Services/Implementations/ProjetoService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;

namespace Ibatech.Services.Implementations;

public sealed class ProjetoService(
    IProjetoRepository projetoRepo,
    IUnitOfWork uow) : IProjetoService
{
    public async Task<ProjetoResponseDto> CriarAsync(
        Guid usuarioId,
        ProjetoCreateDto dto,
        CancellationToken ct = default)
    {
        var projeto = new ProjetoRequisitos(
            usuarioId,
            dto.NomeEmpresa,
            dto.NomeContato,
            dto.EmailContato,
            dto.DescricaoDores,
            dto.InfraAtual,
            dto.UtilizaNuvem,
            dto.VolumetriaUsuarios,
            dto.ProvedorNuvem,
            dto.ObservacoesExtra);

        await projetoRepo.AdicionarAsync(projeto, ct);
        await uow.CommitAsync(ct);

        return projeto.ToDomainDto();
    }

    public async Task<IEnumerable<ProjetoResponseDto>> ListarAsync(
        CancellationToken ct = default)
    {
        var projetos = await projetoRepo.ListarComUsuarioAsync(ct);
        return projetos.ToDomainDtoList();
    }

    public async Task<IEnumerable<ProjetoResponseDto>> ListarPorUsuarioAsync(
        Guid usuarioId,
        CancellationToken ct = default)
    {
        var projetos = await projetoRepo.BuscarAsync(
            p => p.UsuarioId == usuarioId, ct);
        return projetos.ToDomainDtoList();
    }

    // Renomeado para casar com a assinatura do Controller
    public async Task<ProjetoResponseDto> ObterPorIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var projeto = await projetoRepo.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Projeto não encontrado.");
        return projeto.ToDomainDto();
    }

    public async Task<ProjetoResponseDto> AtualizarStatusAsync(
        Guid id,
        AtualizarStatusDto dto,
        CancellationToken ct = default)
    {
        var projeto = await projetoRepo.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Projeto não encontrado.");

        // Padroniza o texto vindo do Angular para o formato de exibição do banco
        string statusFormatado = dto.NovoStatus.ToUpper() switch
        {
            "RECEBIDO" => "Recebido",
            "EM_ANALISE" => "Em Análise",
            "APROVADO" => "Aprovado",
            "REJEITADO" => "Rejeitado",
            "CONCLUIDO" => "Concluído",
            _ => dto.NovoStatus // Fallback mantendo o que vier
        };

        // Altera a propriedade diretamente na Entidade de Domínio
        projeto.AlterarStatus(statusFormatado);

        projetoRepo.Atualizar(projeto);
        await uow.CommitAsync(ct);

        return projeto.ToDomainDto();
    }
}