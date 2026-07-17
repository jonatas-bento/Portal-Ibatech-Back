// src/Ibatech.Services/Implementations/FinanceiroService.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums; // Garante o acesso ao seu TipoTransacao
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;

namespace Ibatech.Services.Implementations;

public sealed class FinanceiroService(
    IFinanceiroRepository financeiroRepo,
    IUnitOfWork uow) : IFinanceiroService
{
    public async Task<ResumoFinanceiroDto> ObterResumoAsync(CancellationToken ct = default)
    {
        var transacoes = await financeiroRepo.ObterTodosAsync(ct);

        // 1. Correção: Compara direto com o Enum, eliminando o .ToUpper() problemático
        var totalReceitas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita && t.Liquidada) // Ajuste se no Enum estiver em inglês (ex: TipoTransacao.Income ou Receita)
            .Sum(t => t.Valor);

        var totalDespesas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa && t.Liquidada) // Ajuste se no Enum estiver Despesa, Saida ou Expense
            .Sum(t => t.Valor);

        var transacoesAbertas = transacoes.Count(t => !t.Liquidada);

        return new ResumoFinanceiroDto
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            Saldo = totalReceitas - totalDespesas,
            TransacoesAbertas = transacoesAbertas
        };
    }

    public async Task<IEnumerable<TransacaoResponseDto>> ListarAsync(CancellationToken ct = default)
    {
        var transacoes = await financeiroRepo.ObterTodosAsync(ct);

        return transacoes.Select(t => new TransacaoResponseDto
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo.ToString().ToUpper(), // Transforma o Enum em string ("RECEITA" / "DESPESA") para o Angular
            DataVencimento = t.DataVencimento,
            DataPagamento = t.DataPagamento,
            Liquidada = t.Liquidada
        }).OrderByDescending(t => t.DataVencimento);
    }

    public async Task<TransacaoResponseDto> CriarAsync(TransacaoCreateDto dto, CancellationToken ct = default)
    {
        // 2. Correção: Faz o Parse seguro da string vinda do Angular para o seu Enum de Domínio
        if (!Enum.TryParse<TipoTransacao>(dto.Tipo, true, out var tipoEnum))
        {
            throw new ArgumentException($"Tipo de transação '{dto.Tipo}' é inválido.");
        }

        // Instancia a sua entidade usando o construtor correto que você compartilhou
        var novaTransacao = new TransacaoFinanceira(
            dto.Descricao,
            dto.Valor,
            tipoEnum,
            dto.DataVencimento,
            dto.Categoria,
            dto.UsuarioId
        );

        await financeiroRepo.AdicionarAsync(novaTransacao, ct);
        await uow.CommitAsync(ct);

        return new TransacaoResponseDto
        {
            Id = novaTransacao.Id,
            Descricao = novaTransacao.Descricao,
            Valor = novaTransacao.Valor,
            Tipo = novaTransacao.Tipo.ToString().ToUpper(),
            DataVencimento = novaTransacao.DataVencimento,
            DataPagamento = novaTransacao.DataPagamento,
            Liquidada = novaTransacao.Liquidada
        };
    }

    private static void ValidarPeriodo(FinanceiroFiltroDto filtro)
    {
        if (!filtro.DataInicio.HasValue)
            throw new ArgumentException("Data inicial é obrigatória.");

        if (!filtro.DataFim.HasValue)
            throw new ArgumentException("Data final é obrigatória.");

        if (filtro.DataInicio.Value >= filtro.DataFim.Value)
            throw new ArgumentException("A data inicial deve ser anterior à data final.");

        if ((filtro.DataFim.Value - filtro.DataInicio.Value).TotalDays > 366)
            throw new ArgumentException("O período máximo permitido é de 366 dias.");
    }

    private static (int pagina, int tamanhoPagina) NormalizarPaginacao(FinanceiroFiltroDto filtro)
    {
        var paginaNormalizada = filtro.Pagina < 1 ? 1 : filtro.Pagina;

        var tamanhoNormalizado = filtro.TamanhoPagina switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => filtro.TamanhoPagina
        };

        return (paginaNormalizada, tamanhoNormalizado);
    }

    public async Task<ResumoFinanceiroDetalhadoDto> ObterResumoDetalhadoAsync(FinanceiroFiltroDto filtro, CancellationToken ct = default)
    {
        ValidarPeriodo(filtro);

        return await financeiroRepo.ObterResumoDetalhadoAsync(filtro, ct);
    }

    public async Task<ResultadoPaginadoDto<TransacaoFinanceiraResumoDto>> ListarPaginadoAsync(FinanceiroFiltroDto filtro, CancellationToken ct = default)
    {
        ValidarPeriodo(filtro);

        var (paginaNormalizada, tamanhoNormalizado) = NormalizarPaginacao(filtro);

        return await financeiroRepo.ListarPaginadoAsync(filtro, paginaNormalizada, tamanhoNormalizado, ct);
    }
}
