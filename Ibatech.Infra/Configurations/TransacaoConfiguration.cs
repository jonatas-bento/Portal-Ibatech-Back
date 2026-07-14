using Ibatech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ibatech.Infra.Configurations;

public sealed class TransacaoConfiguration : IEntityTypeConfiguration<TransacaoFinanceira>
{
    public void Configure(EntityTypeBuilder<TransacaoFinanceira> b)
    {
        b.ToTable("transacoes_financeiras");
        b.HasKey(t => t.Id);
        b.Property(t => t.Descricao).IsRequired().HasMaxLength(500);
        b.Property(t => t.Valor).HasPrecision(18, 2).IsRequired();
        b.Property(t => t.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30);
        b.Property(t => t.DataVencimento).IsRequired();
        b.Property(t => t.DataPagamento);
        b.Property(t => t.Liquidada).HasDefaultValue(false);
        b.Property(t => t.Categoria).HasMaxLength(100);
        b.HasIndex(t => t.VendaId);
        b.Property(t => t.Ativo).HasDefaultValue(true);
        b.Property(t => t.CriadoEm).IsRequired();
        b.Property(t => t.AtualizadoEm);

        b.HasOne(t => t.Usuario)
            .WithMany(u => u.Transacoes)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
