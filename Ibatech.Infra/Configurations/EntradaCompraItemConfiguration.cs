using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class EntradaCompraItemConfiguration
    : IEntityTypeConfiguration<EntradaCompraItem>
{
    public void Configure(
        EntityTypeBuilder<EntradaCompraItem> b)
    {
        b.ToTable("EntradaCompraItens");

        b.HasKey(x => x.Id);

        b.Property(x => x.CodigoSku)
            .HasMaxLength(50);

        b.Property(x => x.CodigoFornecedor)
            .HasMaxLength(100);

        b.Property(x => x.NomeProduto)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PrecoUnitarioCompra)
            .HasPrecision(18, 4);

        b.Property(x => x.ValorTotalProduto)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorFreteRateado)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorDescontoRateado)
            .HasPrecision(18, 2);

        b.Property(x => x.ValorOutrasDespesasRateado)
            .HasPrecision(18, 2);

        b.Property(x => x.CustoEfetivoUnitario)
            .HasPrecision(18, 4);

        b.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany<MovimentacaoEstoque>()
            .WithOne(x => x.EntradaCompraItem)
            .HasForeignKey(x => x.EntradaCompraItemId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.EntradaCompraId);
        b.HasIndex(x => x.ProdutoId);
    }
}
