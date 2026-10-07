using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class EntradaCompraConfiguration
    : IEntityTypeConfiguration<EntradaCompra>
{
    public void Configure(
        EntityTypeBuilder<EntradaCompra> b)
    {
        b.ToTable("EntradasCompras");

        b.HasKey(x => x.Id);

        b.Property(x => x.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        b.Property(x => x.ValorProdutos)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorFrete)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorDesconto)
            .HasPrecision(18, 2);

        b.Property(x => x.OutrasDespesas)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorTotal)
            .HasPrecision(18, 2);

        b.Property(x => x.Observacao)
            .HasMaxLength(1000);

        b.HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Itens)
            .WithOne()
            .HasForeignKey(x => x.EntradaCompraId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.FornecedorId);
        b.HasIndex(x => x.UsuarioId);
        b.HasIndex(x => x.DataEntrada);

        b.HasIndex(x => new
        {
            x.FornecedorId,
            x.NumeroDocumento
        })
        .IsUnique();
    }
}
