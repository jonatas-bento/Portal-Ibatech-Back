using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Numero)
            .IsRequired()
            .HasMaxLength(20);
        builder.HasIndex(v => v.Numero).IsUnique();

        builder.Property(v => v.ClienteId);
        builder.HasIndex(v => v.ClienteId);

        builder.Property(v => v.ClienteNomeSnapshot).HasMaxLength(150);
        builder.Property(v => v.ClienteCpfCnpjSnapshot).HasMaxLength(14);

        builder.Property(v => v.VendedorId).IsRequired();
        builder.HasIndex(v => v.VendedorId);

        builder.Property(v => v.VendedorNomeSnapshot)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.HasIndex(v => v.Status);

        builder.Property(v => v.DataVenda).IsRequired();
        builder.HasIndex(v => v.DataVenda);

        builder.Property(v => v.ValorBruto).HasPrecision(18, 2);
        builder.Property(v => v.Desconto).HasPrecision(18, 2);
        builder.Property(v => v.ValorTotal).HasPrecision(18, 2);

        builder.Property(v => v.Observacao).HasMaxLength(1000);

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Venda.Itens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
