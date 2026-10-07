using Ibatech.Domain.Entities;

namespace Ibatech.Domain.Interfaces.Repositories;

public interface IEntradaCompraRepository
    : IRepositoryBase<EntradaCompra>
{
    Task<EntradaCompra?> ObterComItensAsync(
        Guid id,
        CancellationToken ct = default);

    Task<EntradaCompra?> ObterDetalheAsync(
        Guid id,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<EntradaCompra>> ListarAsync(
        CancellationToken ct = default);

    Task<bool> ExisteDocumentoAsync(
        Guid fornecedorId,
        string numeroDocumento,
        CancellationToken ct = default);

    void AdicionarItem(EntradaCompraItem item);
}
