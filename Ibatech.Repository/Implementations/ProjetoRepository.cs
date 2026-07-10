// Ibatech.Repository/Implementations/ProjetoRepository.cs
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Infra.Context;
using Ibatech.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Repository.Implementations;

public sealed class ProjetoRepository(IbatechDbContext ctx)
    : RepositoryBase<ProjetoRequisitos>(ctx), IProjetoRepository
{
    public async Task<IEnumerable<ProjetoRequisitos>> ListarComUsuarioAsync(
        CancellationToken ct = default)
    {
        // PASSO 1: Tente a consulta mais crua possível para verificar a tabela base
        // Se quebrar aqui, o problema é um filtro global complexo no DbContext que o IgnoreQueryFilters() não cobriu
        var query = DbSet.AsNoTracking().IgnoreQueryFilters();

        // PASSO 2: Inclua o relacionamento (Descomente para testar)
        // Se quebrar aqui, o mapeamento da relação entre ProjetoRequisitos e Usuario está incorreto na Fluent API
        query = query.Include(p => p.Usuario);

        // PASSO 3: Aplique a ordenação (Descomente para testar)
        // Se quebrar aqui, 'CriadoEm' não existe mapeado fisicamente como uma coluna no banco de dados (ex: [NotMapped])
        query = query.OrderByDescending(p => p.CriadoEm);

        return await query.ToListAsync(ct);
    }
}