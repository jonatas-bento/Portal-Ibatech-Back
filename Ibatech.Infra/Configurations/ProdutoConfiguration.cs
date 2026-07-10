// Ibatech.Infra/Configurations/ProdutoConfiguration.cs
using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> b)
    {
        b.ToTable("produtos");
        b.HasKey(p => p.Id);
        b.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        b.Property(p => p.CodigoSku).HasMaxLength(50);
        b.HasIndex(p => p.CodigoSku).IsUnique().HasFilter("codigo_sku IS NOT NULL");
        b.Property(p => p.Tipo).HasConversion<string>().HasMaxLength(30);
        b.Property(p => p.PrecoCompra).HasPrecision(18, 2);
        b.Property(p => p.PrecoVenda).HasPrecision(18, 2);

        b.HasOne(p => p.Estoque)
            .WithOne(e => e.Produto)
            .HasForeignKey<Estoque>(e => e.ProdutoId);
    }
}