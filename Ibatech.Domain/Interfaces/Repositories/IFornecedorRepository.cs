using Ibatech.Domain.Entities;

namespace Ibatech.Domain.Interfaces.Repositories;

public interface IFornecedorRepository : IRepositoryBase<Fornecedor>
{
    Task<IReadOnlyCollection<Fornecedor>> ListarAsync(
        CancellationToken ct = default);
}
