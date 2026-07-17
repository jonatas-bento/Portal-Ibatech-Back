// Ibatech.Repository/Implementations/FinanceiroRepository.cs
using Ibatech.Domain.Constants;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class FinanceiroRepository(IbatechDbContext ctx)
    : RepositoryBase<TransacaoFinanceira>(ctx), IFinanceiroRepository
{
    // herda todos os métodos genéricos
    // queries específicas adicionadas conforme necessidade

    public Task<bool> ExisteTransacaoDaVendaAsync(
        Guid vendaId,
        TipoTransacao tipo,
        string categoria,
        CancellationToken ct = default)
    {
        return ctx.TransacoesFinanceiras
            .AsNoTracking()
            .AnyAsync(t =>
                t.VendaId == vendaId &&
                t.Tipo == tipo &&
                t.Categoria == categoria,
                ct);
    }

    /// <summary>
    /// Query-base filtrada, iniciada com AsNoTracking, aplicando todos os
    /// filtros do painel financeiro diretamente sobre o IQueryable, de modo
    /// que sejam traduzidos para SQL e executados no MySQL — sem qualquer
    /// materialização (ToList/ToListAsync) nesta etapa.
    /// </summary>
    private IQueryable<TransacaoFinanceira> CriarQueryBase(FinanceiroFiltroDto filtro)
    {
        var query = ctx.TransacoesFinanceiras
            .AsNoTracking()
            .Where(t => t.Ativo)
            .AsQueryable();

        if (filtro.VendaId.HasValue)
            query = query.Where(t => t.VendaId == filtro.VendaId.Value);

        if (filtro.Tipo.HasValue)
            query = query.Where(t => t.Tipo == filtro.Tipo.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
            query = query.Where(t => t.Categoria == filtro.Categoria);

        if (filtro.Liquidada.HasValue)
            query = query.Where(t => t.Liquidada == filtro.Liquidada.Value);

        if (filtro.FormaPagamento.HasValue)
        {
            query = query.Where(
                t => t.Venda != null &&
                     t.Venda.FormaPagamento == filtro.FormaPagamento.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Termo))
        {
            var termo = filtro.Termo;
            query = query.Where(t =>
                t.Descricao.Contains(termo) ||
                (t.Categoria != null && t.Categoria.Contains(termo)) ||
                (t.Venda != null && t.Venda.Numero.Contains(termo)) ||
                (t.Usuario != null && t.Usuario.NomeCompleto.Contains(termo)));
        }

        // Regra de data financeira:
        // - Liquidada == true e DataPagamento com valor => usa DataPagamento;
        // - demais casos => usa DataVencimento.
        // DataInicio inclusiva, DataFim exclusiva.
        if (filtro.DataInicio.HasValue)
        {
            var inicio = filtro.DataInicio.Value;
            query = query.Where(t =>
                (t.Liquidada && t.DataPagamento.HasValue
                    ? t.DataPagamento.Value
                    : t.DataVencimento) >= inicio);
        }

        if (filtro.DataFim.HasValue)
        {
            var fim = filtro.DataFim.Value;
            query = query.Where(t =>
                (t.Liquidada && t.DataPagamento.HasValue
                    ? t.DataPagamento.Value
                    : t.DataVencimento) < fim);
        }

        return query;
    }

    public async Task<ResumoFinanceiroDetalhadoDto> ObterResumoDetalhadoAsync(
        FinanceiroFiltroDto filtro,
        CancellationToken ct = default)
    {
        var queryBase = CriarQueryBase(filtro);

        // RECEITAS: Tipo == Receita, Liquidada == true, Categoria == Venda.
        var receitasQuery = queryBase.Where(t =>
            t.Tipo == TipoTransacao.Receita &&
            t.Liquidada &&
            t.Categoria == CategoriasFinanceiras.Venda);

        var totalReceitas = await receitasQuery
            .SumAsync(t => (decimal?)t.Valor, ct) ?? 0m;

        var quantidadeVendas = await receitasQuery
            .Where(t => t.VendaId.HasValue)
            .Select(t => t.VendaId!.Value)
            .Distinct()
            .CountAsync(ct);

        // DESPESAS OPERACIONAIS: Tipo == Despesa, Liquidada == true,
        // Categoria != EstornoVenda.
        var despesasQuery = queryBase.Where(t =>
            t.Tipo == TipoTransacao.Despesa &&
            t.Liquidada &&
            t.Categoria != CategoriasFinanceiras.EstornoVenda);

        var totalDespesas = await despesasQuery
            .SumAsync(t => (decimal?)t.Valor, ct) ?? 0m;

        // ESTORNOS: Tipo == Despesa, Liquidada == true,
        // Categoria == EstornoVenda.
        var estornosQuery = queryBase.Where(t =>
            t.Tipo == TipoTransacao.Despesa &&
            t.Liquidada &&
            t.Categoria == CategoriasFinanceiras.EstornoVenda);

        var totalEstornos = await estornosQuery
            .SumAsync(t => (decimal?)t.Valor, ct) ?? 0m;

        var quantidadeEstornos = await estornosQuery
            .Where(t => t.VendaId.HasValue)
            .Select(t => t.VendaId!.Value)
            .Distinct()
            .CountAsync(ct);

        var totalLiquido = totalReceitas - totalDespesas - totalEstornos;
        var ticketMedio = quantidadeVendas > 0 ? totalReceitas / quantidadeVendas : 0m;

        // RESUMO POR FORMA DE PAGAMENTO:
        // Agrupamento SQL por Venda.FormaPagamento, considerando apenas
        // receitas de venda e estornos de venda (despesas operacionais não
        // entram no total por forma).
        var gruposReceita = await queryBase
            .Where(t =>
                t.Tipo == TipoTransacao.Receita &&
                t.Liquidada &&
                t.Categoria == CategoriasFinanceiras.Venda &&
                t.Venda != null)
            .GroupBy(t => t.Venda!.FormaPagamento)
            .Select(g => new
            {
                FormaPagamento = g.Key,
                Total = g.Sum(t => t.Valor),
                Quantidade = g.Select(t => t.VendaId!.Value).Distinct().Count()
            })
            .ToListAsync(ct);

        var gruposEstorno = await queryBase
            .Where(t =>
                t.Tipo == TipoTransacao.Despesa &&
                t.Liquidada &&
                t.Categoria == CategoriasFinanceiras.EstornoVenda &&
                t.Venda != null)
            .GroupBy(t => t.Venda!.FormaPagamento)
            .Select(g => new
            {
                FormaPagamento = g.Key,
                Total = g.Sum(t => t.Valor),
                Quantidade = g.Select(t => t.VendaId!.Value).Distinct().Count()
            })
            .ToListAsync(ct);

        var receitaPorForma = gruposReceita
            .Where(g => g.FormaPagamento.HasValue)
            .ToDictionary(g => g.FormaPagamento!.Value, g => g);
        var estornoPorForma = gruposEstorno
            .Where(g => g.FormaPagamento.HasValue)
            .ToDictionary(g => g.FormaPagamento!.Value, g => g);

        // Complementa em memória apenas os quatro grupos fixos do enum
        // (no máximo 4 registros agregados, nunca todas as transações).
        var formasPagamento = Enum.GetValues<FormaPagamento>()
            .Select(forma =>
            {
                var receitaForma = receitaPorForma.TryGetValue(forma, out var r) ? r : null;
                var estornoForma = estornoPorForma.TryGetValue(forma, out var e) ? e : null;

                var totalReceitasForma = receitaForma?.Total ?? 0m;
                var totalEstornosForma = estornoForma?.Total ?? 0m;

                return new ResumoFormaPagamentoDto
                {
                    FormaPagamento = forma,
                    TotalReceitas = totalReceitasForma,
                    TotalEstornos = totalEstornosForma,
                    TotalLiquido = totalReceitasForma - totalEstornosForma,
                    QuantidadeVendas = receitaForma?.Quantidade ?? 0,
                    QuantidadeEstornos = estornoForma?.Quantidade ?? 0
                };
            })
            .ToList();

        return new ResumoFinanceiroDetalhadoDto
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            TotalEstornos = totalEstornos,
            TotalLiquido = totalLiquido,
            QuantidadeVendas = quantidadeVendas,
            QuantidadeEstornos = quantidadeEstornos,
            TicketMedio = ticketMedio,
            FormasPagamento = formasPagamento
        };
    }

    public async Task<ResultadoPaginadoDto<TransacaoFinanceiraResumoDto>> ListarPaginadoAsync(
        FinanceiroFiltroDto filtro,
        int pagina,
        int tamanhoPagina,
        CancellationToken ct = default)
    {
        var query = CriarQueryBase(filtro)
            .Include(t => t.Venda)
            .Include(t => t.Usuario);

        // Count executado ANTES do Skip/Take, traduzido para SQL (COUNT).
        var totalItens = await query.CountAsync(ct);

        var itens = await query
            .OrderByDescending(t =>
                t.Liquidada && t.DataPagamento.HasValue
                    ? t.DataPagamento.Value
                    : t.DataVencimento)
            .ThenByDescending(t => t.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(t => new TransacaoFinanceiraResumoDto
            {
                Id = t.Id,
                Descricao = t.Descricao,
                Valor = t.Valor,
                Tipo = t.Tipo,
                Categoria = t.Categoria ?? string.Empty,
                DataVencimento = t.DataVencimento,
                DataPagamento = t.DataPagamento,
                Liquidada = t.Liquidada,
                VendaId = t.VendaId,
                NumeroVenda = t.Venda != null ? t.Venda.Numero : null,
                FormaPagamento = t.Venda != null ? (FormaPagamento?)t.Venda.FormaPagamento : null,
                UsuarioId = t.UsuarioId ?? Guid.Empty,
                UsuarioNome = t.Usuario != null ? t.Usuario.NomeCompleto : string.Empty,
                CriadoEm = t.CriadoEm
            })
            .ToListAsync(ct);

        return new ResultadoPaginadoDto<TransacaoFinanceiraResumoDto>
        {
            Itens = itens,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = tamanhoPagina > 0
                ? (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
                : 0
        };
    }
}
