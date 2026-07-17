using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;

namespace Ibatech.Domain.Interfaces.Repositories;


public interface IFinanceiroRepository : IRepositoryBase<TransacaoFinanceira>
{
    /// <summary>
    /// Verifica se existe, para a venda informada, uma transação com o tipo
    /// e a categoria especificados. Usada para localizar a Receita original
    /// da venda e para detectar uma compensação de estorno já existente,
    /// sem carregar todas as transações do sistema.
    /// </summary>
    Task<bool> ExisteTransacaoDaVendaAsync(
        Guid vendaId,
        TipoTransacao tipo,
        string categoria,
        CancellationToken ct = default);

    /// <summary>
    /// Calcula o resumo financeiro detalhado (receitas, despesas, estornos,
    /// totais líquidos, ticket médio e agrupamento por forma de pagamento)
    /// inteiramente por meio de consultas agregadas no banco de dados
    /// (SUM/COUNT/GROUP BY), sem materializar as transações do período.
    /// </summary>
    Task<ResumoFinanceiroDetalhadoDto> ObterResumoDetalhadoAsync(
        FinanceiroFiltroDto filtro,
        CancellationToken ct = default);

    /// <summary>
    /// Lista as transações financeiras do período de forma paginada,
    /// aplicando Count/Skip/Take diretamente na consulta traduzida para
    /// SQL, sem paginação em memória.
    /// </summary>
    Task<ResultadoPaginadoDto<TransacaoFinanceiraResumoDto>> ListarPaginadoAsync(
        FinanceiroFiltroDto filtro,
        int pagina,
        int tamanhoPagina,
        CancellationToken ct = default);
}
