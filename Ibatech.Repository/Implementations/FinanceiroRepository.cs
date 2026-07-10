// Ibatech.Repository/Implementations/FinanceiroRepository.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;

namespace Ibatech.Repository.Implementations;

public sealed class FinanceiroRepository(IbatechDbContext ctx)
    : RepositoryBase<TransacaoFinanceira>(ctx), IFinanceiroRepository
{
    // herda todos os métodos genéricos
    // queries específicas adicionadas conforme necessidade
}