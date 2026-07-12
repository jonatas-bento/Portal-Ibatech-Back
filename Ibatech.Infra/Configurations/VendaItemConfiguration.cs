using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public class VendaItemConfiguration : IEntityTypeConfiguration<VendaItem>
{
    public void Configure(EntityTypeBuilder<VendaItem> builder)
    {
        builder.ToTable("VendaItens");

        builder.HasKey(vi => vi.Id);

        builder.Property(vi => vi.CodigoSku).HasMaxLength(50);
        
        builder.Property(vi => vi.NomeProduto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(vi => vi.DescricaoProduto).HasColumnType("longtext");

        builder.Property(vi => vi.Quantidade).IsRequired();
        builder.Property(vi => vi.PrecoUnitario).HasPrecision(18, 2);
        builder.Property(vi => vi.Desconto).HasPrecision(18, 2);
        builder.Property(vi => vi.ValorTotal).HasPrecision(18, 2);

        builder.HasOne<Venda>()
            .WithMany(v => v.Itens)
            .HasForeignKey(vi => vi.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(vi => vi.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(vi => vi.VendaId);
        builder.HasIndex(vi => vi.ProdutoId);
        builder.HasIndex(vi => new { vi.VendaId, vi.ProdutoId }).IsUnique();
    }
}
