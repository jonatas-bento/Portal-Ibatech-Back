using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;

namespace Ibatech.Repository.Implementations;

public sealed class ClienteRepository(IbatechDbContext context) 
    : RepositoryBase<Cliente>(context), IClienteRepository
{
}
