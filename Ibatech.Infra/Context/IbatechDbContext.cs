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
        base.OnModelCreating(mb);

        // Aplica as configurações existentes.
        mb.ApplyConfigurationsFromAssembly(typeof(IbatechDbContext).Assembly);

        // Define explicitamente os nomes físicos das tabelas.
        mb.Entity<Usuario>().ToTable("Usuarios");
        mb.Entity<ProjetoRequisitos>().ToTable("ProjetosRequisitos");
        mb.Entity<Produto>().ToTable("Produtos");
        mb.Entity<Estoque>().ToTable("Estoques");
        mb.Entity<MovimentacaoEstoque>().ToTable("Movimentacoes");
        mb.Entity<TransacaoFinanceira>().ToTable("TransacoesFinanceiras");

        // Filtro global de soft-delete.
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            var propertyAtivo = entityType.ClrType.GetProperty("Ativo");

            if (propertyAtivo is null)
                continue;

            var parametro = System.Linq.Expressions.Expression.Parameter(
                entityType.ClrType,
                "e");

            var corpoPropriedade =
                System.Linq.Expressions.Expression.Property(parametro, "Ativo");

            var constanteTrue =
                System.Linq.Expressions.Expression.Constant(true);

            var comparacaoEqual =
                System.Linq.Expressions.Expression.Equal(
                    corpoPropriedade,
                    constanteTrue);

            var filtroLambda =
                System.Linq.Expressions.Expression.Lambda(
                    comparacaoEqual,
                    parametro);

            mb.Entity(entityType.ClrType)
                .HasQueryFilter(filtroLambda);
        }
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker
                     .Entries()
                     .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is Ibatech.Domain.Entities.Base.EntityBase entity)
                entity.MarcarAtualizado();
        }

        return base.SaveChangesAsync(ct);
    }
}