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
}
