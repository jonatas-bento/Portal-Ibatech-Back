// Ibatech.Services/Mappers/FinanceiroMapper.cs
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using FinanceiroServiceDto = Ibatech.Services.DTOs.Financeiro.TransacaoResponseDto;
using ResumoServiceDto = Ibatech.Services.DTOs.Financeiro.ResumoFinanceiroDto;

namespace Ibatech.Services.Mappers;

public static class FinanceiroMapper
{
    public static FinanceiroServiceDto ToDto(this TransacaoFinanceira t) => new(
        t.Id,
        t.Descricao,
        t.Valor,
        t.Tipo,
        t.Tipo == TipoTransacao.Receita ? "Receita" : "Despesa",
        t.DataVencimento,
        t.DataPagamento,
        t.Liquidada,
        t.Categoria,
        t.CriadoEm
    );

    public static IEnumerable<FinanceiroServiceDto> ToDtoList(
        this IEnumerable<TransacaoFinanceira> lista) =>
        lista.Select(t => t.ToDto());

    // Mapeamento para DTOs do Domain
    public static TransacaoResponseDto ToDomainDto(this TransacaoFinanceira t) =>
        new()
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo == TipoTransacao.Receita ? "Receita" : "Despesa",
            DataVencimento = t.DataVencimento,
            DataPagamento = t.DataPagamento,
            Liquidada = t.Liquidada
        };

    public static IEnumerable<TransacaoResponseDto> ToDomainDtoList(
        this IEnumerable<TransacaoFinanceira> lista) =>
        lista.Select(t => t.ToDomainDto());

    public static ResumoFinanceiroDto ToDomainResumoDto(
        decimal totalReceitas,
        decimal totalDespesas,
        int transacoesAbertas) =>
        new()
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            Saldo = totalReceitas - totalDespesas,
            TransacoesAbertas = transacoesAbertas
        };
}
