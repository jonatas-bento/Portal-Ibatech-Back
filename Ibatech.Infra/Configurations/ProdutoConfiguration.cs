// Ibatech.Infra/Configurations/ProdutoConfiguration.cs
using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> b)
    {
        b.ToTable("Produtos");

        b.HasKey(p => p.Id);

        b.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(p => p.CodigoSku)
            .HasMaxLength(50);

        b.HasIndex(p => p.CodigoSku)
            .IsUnique();

        b.Property(p => p.CodigoFornecedor)
            .HasMaxLength(100);

        b.Property(p => p.CodigoBarras)
            .HasMaxLength(50);

        b.Property(p => p.Ncm)
            .HasMaxLength(20);

        b.Property(p => p.UnidadeComercial)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("UN");

        b.Property(p => p.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.Property(p => p.PrecoCompra)
            .HasPrecision(18, 2);

        b.Property(p => p.PrecoVenda)
            .HasPrecision(18, 2);

        b.HasOne(p => p.Estoque)
            .WithOne(e => e.Produto)
            .HasForeignKey<Estoque>(e => e.ProdutoId);
    }
}
