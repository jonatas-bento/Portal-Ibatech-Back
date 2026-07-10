using Ibatech.Domain.Entities;
namespace Ibatech.Domain.Interfaces.Repositories;

public interface IProjetoRepository : IRepositoryBase<ProjetoRequisitos>
{
    Task<IEnumerable<ProjetoRequisitos>> ListarComUsuarioAsync(CancellationToken ct = default);
}