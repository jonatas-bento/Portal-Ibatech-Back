// Ibatech.Repository/Implementations/FinanceiroRepository.cs
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
}
