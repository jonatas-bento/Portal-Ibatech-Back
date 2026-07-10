using Ibatech.Domain.Entities;
namespace Ibatech.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IRepositoryBase<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
}