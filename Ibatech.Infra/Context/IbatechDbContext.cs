// Ibatech.Infra/Context/IbatechDbContext.cs
using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ibatech.Infra.Context;

public sealed class IbatechDbContext(DbContextOptions<IbatechDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<ProjetoRequisitos> ProjetosRequisitos => Set<ProjetoRequisitos>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> Movimentacoes => Set<MovimentacaoEstoque>();
    public DbSet<TransacaoFinanceira> TransacoesFinanceiras => Set<TransacaoFinanceira>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Aplica todas as IEntityTypeConfiguration<T> da assembly corrente
        mb.ApplyConfigurationsFromAssembly(typeof(IbatechDbContext).Assembly);

        // Filtro global de soft-delete para todas as entidades que herdam EntityBase
        // Filtro global de soft-delete para todas as entidades que herdam EntityBase
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            var propertyAtivo = entityType.ClrType.GetProperty("Ativo");
            if (propertyAtivo != null)
            {
                // 1. Instanciamos o parâmetro 'e' uma única vez aqui
                var parametro = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");

                // 2. Montamos o corpo da expressão comparando a propriedade com true
                var corpoPropriedade = System.Linq.Expressions.Expression.Property(parametro, "Ativo");
                var constanteTrue = System.Linq.Expressions.Expression.Constant(true);
                var comparacaoEqual = System.Linq.Expressions.Expression.Equal(corpoPropriedade, constanteTrue);

                // 3. Geramos o Lambda final passando a MESMA instância do parâmetro
                var filtroLambda = System.Linq.Expressions.Expression.Lambda(comparacaoEqual, parametro);

                mb.Entity(entityType.ClrType).HasQueryFilter(filtroLambda);
            }
        }

        base.OnModelCreating(mb);
    }

    // Intercepta SaveChanges para auto-preencher AtualizadoEm
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is Ibatech.Domain.Entities.Base.EntityBase entity)
                entity.MarcarAtualizado();
        }
        return base.SaveChangesAsync(ct);
    }
}