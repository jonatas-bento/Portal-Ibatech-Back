// Ibatech.Repository/Implementations/UsuarioRepository.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class UsuarioRepository(IbatechDbContext ctx)
    : RepositoryBase<Usuario>(ctx), IUsuarioRepository
{
    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
    await DbSet
        .IgnoreQueryFilters() // 👈 Ignora filtros ocultos que estejam quebrando a tradução
        .FirstOrDefaultAsync(u => u.Email == email, ct);
}